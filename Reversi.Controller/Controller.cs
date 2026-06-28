using System.Diagnostics;
using Reversi.Core;
using Reversi.Network;
using Reversi.Network.Protocol;
using NetworkErrorException = Reversi.Core.NetworkErrorException;

namespace Reversi.Controller;

public class Controller<TModel> : IController
    where TModel : IModel, new()
{
    public static GameState Play(
        GameSettings gameSettings,
        Action<GameState> drawGame,
        Func<Coords[], Coords> askMove
    )
    {
        try
        {
            return _Play(gameSettings, drawGame, askMove);
        }
        catch (ModelException e)
        {
            throw new UnexpectedModelException(e);
        }
        catch (DisconnectException e)
        {
            throw new OpponentDisconnectedException(e);
        }
        catch (NetworkException)
        {
            throw new NetworkErrorException();
        }
        catch (Exception e)
        {
            throw new UnhandledException("Unhandled exception in Controller!", e);
        }
    }
    private static GameState _Play(
        GameSettings gameSettings,
        Action<GameState> drawGame,
        Func<Coords[], Coords> askMove
    )
    {
        switch (gameSettings.GameType)
        {
            case GameType.Local:
                return PlayLocalGame(drawGame, askMove);
            case GameType.NetworkClient:
                if (!gameSettings.ClientSettings.HasValue)
                {
                    throw new UnreachableException();
                }

                return PlayRemoteGameAsClient(gameSettings.ClientSettings.Value, drawGame, askMove);
            case GameType.NetworkHost:
                if (!gameSettings.HostSettings.HasValue)
                {
                    throw new UnreachableException();
                }

                return PlayRemoteGameAsHost(gameSettings.HostSettings.Value, drawGame, askMove);
            default:
                throw new UnreachableException();
        }
    }

    private static GameState PlayLocalGame(
        Action<GameState> drawGame,
        Func<Coords[], Coords> askMove
    )
    {
        var model = new TModel();
        var gameState = model.CurrentState();

        while (true)
        {
            Coords[] possibleMoves;
            possibleMoves = model.PossibleMoves();

            if (possibleMoves.Length == 0)
            {
                gameState = model.Pass();
                continue;
            }

            drawGame(gameState);
            var move = askMove(possibleMoves);
            gameState = model.Move(move);

            if (gameState.CurrentGameStatus != GameStatus.Continue)
            {
                return gameState;
            }
        }
    }

    private static GameState PlayRemoteGameAsHost(
        HostSettings hostSettings,
        Action<GameState> drawGame,
        Func<Coords[], Coords> askMove
    )
    {
        using var host = new Host(
            System.Net.IPAddress.Parse(hostSettings.Host),
            hostSettings.Port
        );
        host.AcceptConnection(hostSettings.HostPlayer.Opposite());

        var localPlayerMove = hostSettings.HostPlayer == Player.Black;

        return PlayRemoteGame(host, localPlayerMove, drawGame, askMove);
    }

    private static GameState PlayRemoteGameAsClient(
        ClientSettings clientSettings,
        Action<GameState> drawGame,
        Func<Coords[], Coords> askMove
    )
    {
        using var client = new Client();
        var localPlayerColor = client.Connect(
            System.Net.IPAddress.Parse(clientSettings.Host),
            clientSettings.Port
        );

        var localPlayerMove = localPlayerColor == Player.Black;
        return PlayRemoteGame(client, localPlayerMove, drawGame, askMove);
    }

    private static GameState PlayRemoteGame(
        Network.Network network,
        bool localPlayerMove,
        Action<GameState> drawGame,
        Func<Coords[], Coords> askMove
    )
    {
        var model = new TModel();
        var gameState = model.CurrentState();

        while (true)
        {
            var possibleMoves = model.PossibleMoves();

            if (possibleMoves.Length == 0)
            {
                gameState = model.Pass();
                if (localPlayerMove)
                {
                    network.Send(new PassMessage());
                    network.ReceiveMessage<StatusOkMessage>();
                }
                else
                {
                    network.ReceiveMessage<PassMessage>();
                    network.Send(new StatusOkMessage());
                }
                continue;
            }

            drawGame(gameState);

            var move = localPlayerMove
                ? askMove(possibleMoves)
                : network.ReceiveMessage<MoveMessage>().Coords;

            if (localPlayerMove)
            {
                network.Send(new MoveMessage(move));
                network.ReceiveMessage<StatusOkMessage>();
            }
            else
            {
                if (!possibleMoves.Contains(move))
                {
                    network.Send(new StatusErrorMessage());
                    throw new NetworkErrorException(new ProtocolException("Impossible move!"));
                }
                network.Send(new StatusOkMessage());
            }

            gameState = model.Move(move);

            if (gameState.CurrentGameStatus != GameStatus.Continue)
            {
                return gameState;
            }

            localPlayerMove = !localPlayerMove;
        }
    }
}
