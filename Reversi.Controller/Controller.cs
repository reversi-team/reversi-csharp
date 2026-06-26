using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Reversi.Core;

namespace Reversi.Controller;

public class Controller<TModel, TView>
    where TModel : IModel, new()
    where TView : IView, new()
{
    private IView _view = new TView();

    public int Main()
    {
        GameSettings? gameSettings = _view.Start();
        do
        {
            var gameResult = Play(gameSettings.Value);
            gameSettings = _view.End(gameResult);
        } while (gameSettings != null);

        return 0;
    }

    [DoesNotReturn]
    private void FatalError()
    {
        Environment.Exit(1);
    }

    private GameState Play(GameSettings gameSettings)
    {
        switch (gameSettings.GameType)
        {
            case GameType.Local:
                return PlayLocalGame();
            case GameType.NetworkClient:
                if (gameSettings.ClientSettings == null)
                {
                    FatalError();
                }
                return PlayRemoteGameAsClient(gameSettings.ClientSettings.Value);
            case GameType.NetworkHost:
                if (gameSettings.HostSettings == null)
                {
                    FatalError();
                }
                return PlayRemoteGameAsHost(gameSettings.HostSettings.Value);
            default:
                throw new UnreachableException();
        }
    }

    private GameState PlayLocalGame()
    {
        var model = new TModel();
        var gameState = model.CurrentState();

        while (true)
        {
            var possibleMoves = model.PossibleMoves();

            if (possibleMoves.Length == 0)
            {
                model.Pass();
                continue;
            }

            var move = DrawGameAndAskMove(gameState, possibleMoves);
            gameState = model.Move(move);

            if (gameState.CurrentGameStatus != GameStatus.Continue)
            {
                return gameState;
            }
        }
    }

    private GameState PlayRemoteGameAsHost(HostSettings hostSettings)
    {
        throw new NotImplementedException();
    }

    private GameState PlayRemoteGameAsClient(ClientSettings clientSettings)
    {
        throw new NotImplementedException();
    }

    private Coords DrawGameAndAskMove(GameState gameState, Coords[] possibleMoves)
    {
        _view.RenderGame(gameState);
        _view.Draw();
        return _view.GetMove(possibleMoves);
    }
}

