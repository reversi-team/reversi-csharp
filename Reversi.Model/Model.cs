using System.Collections.Generic;
using Reversi.Core;

namespace Reversi.Model;

public class Model : IModel
{
    private const int _boardSize = 8;

    private GameState _state;
    private readonly BoardCell[,] _matrix;

    private static readonly int[] _moveX = { 0, 0, 1, -1, 1, 1, -1, -1 };
    private static readonly int[] _moveY = { 1, -1, 0, 0, 1, -1, 1, -1 };

    public Model()
    {
        _matrix = new BoardCell[_boardSize, _boardSize];

        for (int y = 0; y < _boardSize; y++)
        {
            for (int x = 0; x < _boardSize; x++)
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
        return GetPossibleMovesList(_state.CurrentPlayer).ToArray();
    }

    private List<Coords> GetPossibleMovesList(Player player)
    {
        List<Coords> moves = new List<Coords>();

        for (byte y = 0; y < _boardSize; y++)
        {
            for (byte x = 0; x < _boardSize; x++)
            {
                Coords currentCoords = new Coords(x, y);
                List<Coords> flipped = GetFlippedPieces(currentCoords, player);

                if (flipped.Count > 0)
                {
                    moves.Add(currentCoords);
                }
            }
        }

        return moves;
    }

    public GameState Move(Coords coords)
    {
        if (_state.CurrentGameStatus != GameStatus.Continue)
        {
            throw new GameEndedException("Гра вже завершена");
        }

        List<Coords> flipped = GetFlippedPieces(coords, _state.CurrentPlayer);

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

    private List<Coords> GetFlippedPieces(Coords start, Player player)
    {
        if (start.X >= _boardSize || start.Y >= _boardSize)
        {
            throw new ArgumentOutOfRangeException(nameof(start), "Координати знаходяться поза межами поля");
        }

        List<Coords> flipped = new List<Coords>();

        if (_matrix[start.Y, start.X] != BoardCell.Empty)
        {
            return flipped;
        }

        BoardCell myColor = player.ToCell();
        BoardCell enemyColor = player.Opposite().ToCell();

        for (int i = 0; i < _moveX.Length; i++)
        {
            List<Coords> line = new List<Coords>();

            int currentX = start.X + _moveX[i];
            int currentY = start.Y + _moveY[i];

            while (currentX >= 0 && currentX < _boardSize && currentY >= 0 && currentY < _boardSize && _matrix[currentY, currentX] == enemyColor)
            {
                line.Add(new Coords((byte)currentX, (byte)currentY));
                currentX += _moveX[i];
                currentY += _moveY[i];
            }

            if (currentX >= 0 && currentX < _boardSize && currentY >= 0 && currentY < _boardSize && _matrix[currentY, currentX] == myColor && line.Count > 0)
            {
                flipped.AddRange(line);
            }
        }

        return flipped;
    }

    private bool HasAnyMoves(Player player)
    {
        return GetPossibleMovesList(player).Count > 0;
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

        for (int y = 0; y < _boardSize; y++)
        {
            for (int x = 0; x < _boardSize; x++)
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
        BoardCell[,] clone = new BoardCell[_boardSize, _boardSize];
        Array.Copy(source, clone, source.Length);
        return clone;
    }
}
