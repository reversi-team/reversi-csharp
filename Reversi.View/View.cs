using System.Text;
using Reversi.Core;
using Reversi.View.Helpers;
using Reversi.View.Models;
using Spectre.Console;

namespace Reversi.View;


/// <summary>
/// Консольна реалізація IView на базі Spectre.Console.
/// 
/// Архітектура:
/// - View.Main() повністю керує потоком програми (меню, цикл ігор, вихід)
/// - Під час гри View передає контролеру три делегати:
///     drawGame   — контролер викликає щоб оновити дошку на екрані
///     askMove    — контролер викликає щоб отримати хід від гравця
/// - Винятки з контролера (ImpossibleMoveException, ImpossiblePassException тощо)
///   перехоплюються у View і відображаються локалізованим повідомленням
/// </summary>
public sealed class View : IView
{
    // ── ASCII art (Figlet "Big") ───────────────────────────────────────────────
    private const string Title =
        """
         _____  ________      ________ _____   _____ _____ 
        |  __ \|  ____\ \    / /  ____|  __ \ / ____|_   _|
        | |__) | |__   \ \  / /| |__  | |__) | (___   | |  
        |  _  /|  __|   \ \/ / |  __| |  _  / \___ \  | |  
        | | \ \| |____   \  /  | |____| | \ \ ____) |_| |_ 
        |_|  \_\______|   \/   |______|_|  \_\_____/|_____|
        """;

    // ── Стан ─────────────────────────────────────────────────────────────────
    private Localization _loc = Localization.For(Language.English);

    // ── IView ─────────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public void Main<TController>() where TController : IController
    {
        Console.OutputEncoding = Encoding.GetEncoding(1251);
        Console.InputEncoding = Encoding.GetEncoding(1251);

        _loc = Localization.For(PickLanguage());

        while (true)
        {
            int choice = ShowMainMenu();
            if (choice == 1) break;

            var settings = new GameSettings { GameType = GameType.Local };

            try
            {
                var result = TController.Play(
                      settings,
                    drawGame: ShowGameState,
                    askMove: AskMove
                );

                ShowGameOver(result.CurrentGameStatus);
            }
            catch (ImpossibleMoveException)
            {
                ShowError(_loc.MsgImpossibleMove);
                Pause();
            }
            catch (ImpossiblePassException)
            {
                ShowError(_loc.MsgImpossiblePass);
                Pause();
            }
            catch (GameEndedException)    
            {
                ShowError(_loc.MsgGameEnded);
                Pause();
            }
            catch (Exception e)
            {
                ShowError($"{_loc.ErrUnknown}: {e.Message}");
                Pause();
            }

            if (!AskPlayAgain()) break;
        }
    }

    // ── Делегати які передаються в Controller.Play ────────────────────────────

    /// <summary>
    /// Малює поточний стан гри. Передається контролеру як делегат drawGame.
    /// </summary>
    private void ShowGameState(GameState state)
    {
        Console.Clear();
        DrawTitle();
        AnsiConsole.WriteLine();
        DrawScorePanel(state);
        AnsiConsole.WriteLine();

        var validMoves = state.CurrentGameStatus == GameStatus.Continue
            ? new HashSet<Coords>()  // контролер сам знає валідні ходи
            : new HashSet<Coords>();

        var table = BoardRenderer.BuildTable(state.Board, validMoves);
        AnsiConsole.Write(table);

        AnsiConsole.WriteLine();
        DrawCurrentPlayerPrompt(state.CurrentPlayer);
    }

    /// <summary>
    /// Читає хід від гравця. Передається контролеру як делегат askMove.
    /// validMoves — список допустимих ходів для підсвічування на дошці.
    /// </summary>
    private Coords AskMove(Coords[] validMoves)
    {
        var validSet = new HashSet<Coords>(validMoves);

        while (true)
        {
            var raw = AnsiConsole.Ask<string>(_loc.PromptEnterMove);

            if (TryParseCoords(raw, out var coords))
                return coords;

            ShowError(string.Format(_loc.ErrorInvalidCell, raw));
        }
    }

    // ── Приватні методи відображення ──────────────────────────────────────────

    private int ShowMainMenu()
    {
        Console.Clear();
        DrawTitle();
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[bold green]{_loc.MenuNavigationHint}[/]")
                .HighlightStyle(new Style(Color.Black, Color.Green))
                .AddChoices(_loc.MenuNewGameLocal, _loc.MenuQuit));

        return choice == _loc.MenuNewGameLocal ? 0 : 1;
    }

    private void ShowGameOver(GameStatus status)
    {
        Console.Clear();
        DrawTitle();
        AnsiConsole.WriteLine();

        var resultText = status switch
        {
            GameStatus.BlackWin => $"[bold black on white]{_loc.BlackWins}[/]",
            GameStatus.WhiteWin => $"[bold white on grey]{_loc.WhiteWins}[/]",
            _ => $"[bold yellow]{_loc.Draw}[/]"
        };

        var panel = new Panel(Align.Center(new Markup(resultText)))
            .Border(BoxBorder.Double)
            .BorderColor(Color.Green)
            .Header($"[bold green]{_loc.GameOverHeader}[/]", Justify.Center)
            .Padding(2, 1);

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    private bool AskPlayAgain()
    {
        AnsiConsole.WriteLine();
        return AnsiConsole.Confirm($"[bold green]{_loc.PlayAgain}[/]");
    }

    private void ShowError(string message)
    {
        AnsiConsole.MarkupLine($"[bold red]  ✗ {Markup.Escape(message)}[/]");
    }

    private void Pause()
    {
        AnsiConsole.MarkupLine($"[grey]{_loc.PressAnyKey}[/]");
        Console.ReadKey(intercept: true);
    }

    // ── Допоміжні методи малювання ────────────────────────────────────────────

    private static Language PickLanguage()
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[bold green]{Markup.Escape(Title)}[/]");
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold green]Select language / Оберіть мову[/]")
                .HighlightStyle(new Style(Color.Black, Color.Green))
                .AddChoices("🇬🇧  English", "🇺🇦  Українська"));

        return choice.Contains("English") ? Language.English : Language.Ukrainian;
    }

    private static void DrawTitle()
    {
        AnsiConsole.MarkupLine($"[bold green]{Markup.Escape(Title)}[/]");
    }

    private void DrawScorePanel(GameState state)
    {
        var playerTag = state.CurrentPlayer == Player.Black
            ? $"[bold black on white] {_loc.LabelBlack} [/]"
            : $"[bold white on grey] {_loc.LabelWhite} [/]";

        AnsiConsole.MarkupLine(
            $"  [bold white]{_loc.LabelBlack}:[/] [green]{state.Board.BlackCells}[/]   " +
            $"[bold white]{_loc.LabelWhite}:[/] [green]{state.Board.WhiteCells}[/]   " +
            $"[grey]|[/]   {_loc.LabelTurn}: {playerTag}");
    }

    private void DrawCurrentPlayerPrompt(Player player)
    {
        var label = player == Player.Black
            ? $"[bold black on white] {_loc.LabelBlack} [/]"
            : $"[bold white on grey] {_loc.LabelWhite} [/]";

        AnsiConsole.MarkupLine($"  {label} [grey]{_loc.LabelToMove}[/]");
    }

    /// <summary>
    /// Парсить "D3", "d3" тощо у Coords з Core (0-based X=col, Y=row).
    /// </summary>
    private static bool TryParseCoords(string raw, out Coords coords)
    {
        coords = default;

        var clean = raw.Trim().ToUpperInvariant().Replace(" ", "");
        if (clean.Length < 2) return false;

        char colChar = clean[0];
        if (colChar < 'A' || colChar > 'H') return false;
        if (!int.TryParse(clean[1..], out int rowNum)) return false;
        if (rowNum < 1 || rowNum > 8) return false;

        coords = new Coords((byte)(colChar - 'A'), (byte)(rowNum - 1));
        return true;
    }
}
