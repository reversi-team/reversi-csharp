using Reversi.Core;
using Spectre.Console;

namespace Reversi.View.Helpers;

/// <summary>
/// Будує Spectre.Console таблицю з ігрового поля Board з Core.
/// </summary>
internal static class BoardRenderer
{
    private static readonly string[] ColLabels = ["A", "B", "C", "D", "E", "F", "G", "H"];

    internal static Table BuildTable(Board board, IReadOnlySet<Coords> validMoves)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Green)
            .AddColumn(new TableColumn("[grey]  [/]").Centered());

        foreach (var label in ColLabels)
            table.AddColumn(new TableColumn($"[bold green] {label} [/]").Centered());

        for (int row = 0; row < 8; row++)
        {
            var cells = new List<string> { $"[bold grey]{row + 1}[/]" };

            for (int col = 0; col < 8; col++)
            {
                var coords = new Coords((byte)col, (byte)row);
                bool isValid = validMoves.Contains(coords);
                cells.Add(RenderCell(board[row, col], isValid));
            }

            table.AddRow(cells.ToArray());
        }

        return table;
    }

    private static string RenderCell(BoardCell cell, bool isValid) => cell switch
    {
        BoardCell.Black => "[black on white] ● [/]",
        BoardCell.White => "[white on grey] ○ [/]",
        _ => isValid ? "[bold yellow] · [/]" : "[grey]   [/]"
    };
}
