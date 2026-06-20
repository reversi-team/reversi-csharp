using System.Diagnostics;

namespace Reversi.Core;

/// <summary>
/// Колір гравця.
/// </summary>
public enum Player
{
    White,
    Black
}

public static class PlayerExtensions
{
    extension(Player player)
    {
        /// <summary>
        /// Повертає гравця протилежного кольору.
        /// </summary>
        /// <returns>Гравець протилежного кольору.</returns>
        public Player Opposite()
        {
            return player switch
            {
                Player.White => Player.Black,
                Player.Black => Player.White,
                _ => throw new UnreachableException()
            };
        }

        /// <summary>
        /// Повертає тип клітинки, що відпвідає кольору гравця.
        /// </summary>
        /// <returns>Тип клітинки, що відповідає кольору гравця.</returns>
        public BoardCell ToCell()
        {
            return player switch
            {
                Player.White => BoardCell.White,
                Player.Black => BoardCell.Black,
                _ => throw new UnreachableException()
            };
        }
    }
}

/// <summary>
/// Статус гри.
/// </summary>
public enum GameStatus
{
    Continue,
    Draw,
    WhiteWin,
    BlackWin
}

/// <summary>
/// Координати на ігровому полі.
/// </summary>
/// <param name="X">Вісь X (горизонтальна, другий індекс масиву).</param>
/// <param name="Y">Вісь Y (вертикальна, перший індекс масиву)</param>
public record struct Coords(byte X, byte Y);

/// <summary>
/// Стан клітинки на ігровому полі.
/// </summary>
public enum BoardCell
{
    Empty,
    White,
    Black
}

/// <summary>
/// Представлення ігровог поля.
/// </summary>
public class Board
{
    private readonly BoardCell[,] _cells;

    public Board(BoardCell[,] cells)
    {
        _cells = cells;
    }

    public BoardCell this[int row, int col] => _cells[row, col];

    public BoardCell this[Coords coords] => _cells[coords.Y, coords.X];

    public IEnumerator<BoardCell> GetEnumerator() =>
        _cells.Cast<BoardCell>().GetEnumerator();

    public byte WhiteCells => CalcCells(BoardCell.White);
    public byte BlackCells => CalcCells(BoardCell.Black);
    public byte EmptyCells => CalcCells(BoardCell.Empty);

    /// <summary>
    /// Рахує кількість клітинок заданого типу.
    /// </summary>
    /// <param name="cellType">Тип клітинки для підрахунку.</param>
    /// <returns>Кількість клітинок заданого типу.</returns>
    public byte CalcCells(BoardCell cellType)
    {
        byte result = 0;

        foreach (var currentCell in _cells)
        {
            if (currentCell == cellType)
                result++;
        }

        return result;
    }
}

/// <summary>
/// Стан гри.
/// </summary>
public class GameState
{
    public required Board Board { get; init; }
    public required GameStatus CurrentGameStatus { get; init; }
    public required Player CurrentPlayer { get; init; }
    public Player OppositePlayer => CurrentPlayer.Opposite();
    public byte CurrentPlayerScore => Board.CalcCells(CurrentPlayer.ToCell());
    public byte OppositePlayerScore => Board.CalcCells(OppositePlayer.ToCell());
}

/// <summary>
/// Тип гри.
/// </summary>
public enum GameType
{
    Local,
    NetworkHost,
    NetworkClient
}

/// <summary>
/// Налаштування мережі.
/// </summary>
public struct NetworkSettings()
{
    public string Host { get; init; } = "127.0.0.1";
    public required ushort Port { get; init; }
}

/// <summary>
/// Повні налаштування гри для контролера.
/// </summary>
public struct GameSettings
{
    public required GameType GameType { get; init; }
    public NetworkSettings? NetworkSettings { get; init; }
}