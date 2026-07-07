using System.Collections.Generic;

namespace Reversi.Core;

public class GameModel : IModel
{
    private GameState _state;

    private readonly BoardCell[,] _matrix;

    private readonly int[] _moveX = { 0, 0, 1, -1, 1, 1, -1, -1 };
    private readonly int[] _moveY = { 1, -1, 0, 0, 1, -1, 1, -1 };

    public GameModel()
    {
        _matrix = new BoardCell[8, 8];

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                _matrix[y, x] = BoardCell.Empty;
            }
        }

        _matrix[3, 3] = BoardCell.White;
        _matrix[4, 4] = BoardCell.White;
        _matrix[3, 4] = BoardCell.Black;
        _matrix[4, 3] = BoardCell.Black;

        Board startingBoard = new Board(CloneMatrix(_matrix));

        _state = new GameState
        {
            Board = startingBoard,
            CurrentGameStatus = GameStatus.Continue,
            CurrentPlayer = Player.Black
        };
    }

    public GameState CurrentState() { return _state; }

    public Coords[] PossibleMoves()
    {
        List<Coords> moves = new List<Coords>();

        for (byte y = 0; y < 8; y++)
        {
            for (byte x = 0; x < 8; x++)
            {
                Coords currentCoords = new Coords(x, y);

                List<Coords> flipped = GetFlippedPieces(currentCoords, _state.CurrentPlayer, _matrix);

                if (flipped.Count > 0)
                {
                    moves.Add(currentCoords);
                }
            }
        }

        return moves.ToArray();
    }

    public GameState Move(Coords coords)
    {
        if (_state.CurrentGameStatus != GameStatus.Continue)
        {
            throw new GameEndedException("Гра вже завершена");
        }

        List<Coords> flipped = GetFlippedPieces(coords, _state.CurrentPlayer, _matrix);

        if (flipped.Count == 0)
        {
            throw new ImpossibleMoveException("Цей хід неможливий");
        }

        BoardCell currentCellType = _state.CurrentPlayer.ToCell();
        _matrix[coords.Y, coords.X] = currentCellType;

        for (int i = 0; i < flipped.Count; i++)
        {
            Coords flipTarget = flipped[i];
            _matrix[flipTarget.Y, flipTarget.X] = currentCellType;
        }

        Player nextPlayer = _state.CurrentPlayer.Opposite();
        UpdateGameState(nextPlayer);

        return _state;
    }


    public GameState Pass()
    {
        if (_state.CurrentGameStatus != GameStatus.Continue)
        {
            throw new GameEndedException("Гра вже завершена");
        }

        Coords[] availableMoves = PossibleMoves();
        if (availableMoves.Length > 0)
        {
            throw new ImpossiblePassException("У вас є доступні ходи");
        }

        Player nextPlayer = _state.CurrentPlayer.Opposite();
        UpdateGameState(nextPlayer);

        return _state;
    }

    private List<Coords> GetFlippedPieces(Coords start, Player player, BoardCell[,] cells)
    {
        List<Coords> flipped = new List<Coords>();

        if (cells[start.Y, start.X] != BoardCell.Empty)
        {
            return flipped;
        }

        BoardCell myColor = player.ToCell();
        BoardCell enemyColor = player.Opposite().ToCell();

        for (int i = 0; i < 8; i++)
        {
            List<Coords> line = new List<Coords>();

            int currentX = start.X + _moveX[i];
            int currentY = start.Y + _moveY[i];

            while (currentX >= 0 && currentX < 8 && currentY >= 0 && currentY < 8 && cells[currentY, currentX] == enemyColor)
            {
                line.Add(new Coords((byte)currentX, (byte)currentY));
                currentX += _moveX[i];
                currentY += _moveY[i];
            }

            if (currentX >= 0 && currentX < 8 && currentY >= 0 && currentY < 8 && cells[currentY, currentX] == myColor && line.Count > 0)
            {
                flipped.AddRange(line);
            }
        }

        return flipped;
    }

    private bool HasAnyMoves(Player player)
    {
        for (byte y = 0; y < 8; y++)
        {
            for (byte x = 0; x < 8; x++)
            {
                Coords coords = new Coords(x, y);
                List<Coords> flipped = GetFlippedPieces(coords, player, _matrix);
                if (flipped.Count > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateGameState(Player nextPlayer)
    {
        bool nextPlayerHasMoves = HasAnyMoves(nextPlayer);
        bool currentPlayerHasMoves = HasAnyMoves(nextPlayer.Opposite());

        GameStatus status = GameStatus.Continue;

        if (nextPlayerHasMoves == false && currentPlayerHasMoves == false)
        {
            status = CalculateWinner();
        }

        Board newBoard = new Board(CloneMatrix(_matrix));

        _state = new GameState
        {
            Board = newBoard,
            CurrentGameStatus = status,
            CurrentPlayer = nextPlayer
        };
    }

    private GameStatus CalculateWinner()
    {
        int blackCount = 0;
        int whiteCount = 0;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (_matrix[y, x] == BoardCell.Black)
                {
                    blackCount++;
                }

                if (_matrix[y, x] == BoardCell.White)
                {
                    whiteCount++;
                }
            }
        }

        if (blackCount > whiteCount)
        {
            return GameStatus.BlackWin;
        }

        if (whiteCount > blackCount)
        {
            return GameStatus.WhiteWin;
        }

        return GameStatus.Draw;
    }

    private BoardCell[,] CloneMatrix(BoardCell[,] source)
    {
        BoardCell[,] clone = new BoardCell[8, 8];
        Array.Copy(source, clone, source.Length);
        return clone;
    }
}
