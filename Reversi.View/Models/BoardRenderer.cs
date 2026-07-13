using Reversi.Core;
using Spectre.Console;

namespace Reversi.View.Helpers;

/// <summary>
/// Будує Spectre.Console таблицю з ігрового поля Board з Core.
/// </summary>
internal static class BoardRenderer
{
    private static readonly string[] _colLabels = ["A", "B", "C", "D", "E", "F", "G", "H"];

    internal static Table BuildTable(Board board, Coords[] validMoves)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Green)
          
            .AddColumn(new TableColumn("[grey on darkgreen]  [/]").Centered());

        foreach (var label in _colLabels)
        {
            table.AddColumn(new TableColumn($"[bold green on darkgreen] {label} [/]").Centered());
        }

        for (int row = 0; row < 8; row++)
        {
            var cells = new List<string> { $"[bold grey on darkgreen]{row + 1}[/]" };

            for (int col = 0; col < 8; col++)
            {
                var coords = new Coords((byte)col, (byte)row);
                bool isValid = Array.IndexOf(validMoves, coords) >= 0;
                cells.Add(RenderCell(board[row, col], isValid));
            }

            table.AddRow(cells.ToArray());
        }

        return table;
    }

    private static string RenderCell(BoardCell cell, bool isValid) => cell switch
    {
        BoardCell.Black => "[black on darkgreen] ● [/]",
        BoardCell.White => "[white on darkgreen] ○ [/]",
        _ => isValid ? "[bold white on darkgreen] · [/]" : "[grey on darkgreen]   [/]"
    };
}
