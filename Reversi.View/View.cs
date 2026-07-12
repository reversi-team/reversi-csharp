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
/// - Під час гри View передає контролеру делегати:
///     drawGame — малює дошку через AnsiConsole.Live (без блимання)
///     askMove  — читає хід після того як Live завершився
/// - Винятки з контролера перехоплюються у View і відображаються
///   локалізованим повідомленням
/// </summary>
public sealed class View : IView
{
    // ── ASCII art\ ──────────────────────────────────────────────
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

    private GameState? _currentState;
    private Coords[] _currentValidMoves = [];

    // ── IView ─────────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public void Main<TController>() where TController : IController
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        _loc = Localization.For(PickLanguage());

        while (true)
        {
            var settings = ShowMainMenu();
            if (settings is null) break;

            try
            {
                var result = TController.Play(
                    settings.Value,
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
            catch (Exception)
            {
                ShowError(_loc.ErrUnknown);
                Pause();
            }

            if (!AskPlayAgain()) break;
        }
    }

    // ── Делегати які передаються в Controller.Play ────────────────────────────

    /// <summary>
    /// Малює поточний стан гри через AnsiConsole.Live — без блимання.
    /// Загортає дошку і статус у Panel для єдиного фону.
    /// </summary>
    private void ShowGameState(GameState state)
    {
        _currentState = state;

        var panel = BuildGamePanel(state, _currentValidMoves);

        AnsiConsole.Live(panel)
            .AutoClear(false)
            .Start(ctx =>
            {
                ctx.UpdateTarget(BuildGamePanel(state, _currentValidMoves));
                ctx.Refresh();
            });
    }

    /// <summary>
    /// Читає хід від гравця після оновлення дошки.
    /// Перевіряє що введені координати є у списку допустимих ходів.
    /// </summary>
    private Coords AskMove(Coords[] validMoves)
    {
        _currentValidMoves = validMoves;

       
        if (_currentState is not null)
        {
            var panel = BuildGamePanel(_currentState, validMoves);
            AnsiConsole.Live(panel)
                .AutoClear(false)
                .Start(ctx =>
                {
                    ctx.UpdateTarget(BuildGamePanel(_currentState, validMoves));
                    ctx.Refresh();
                });
        }

        while (true)
        {
            var raw = AnsiConsole.Ask<string>(_loc.PromptEnterMove);

            if (!TryParseCoords(raw, out var coords))
            {
                ShowError(string.Format(_loc.ErrorInvalidCell, raw));
                continue;
            }

            if (!validMoves.Contains(coords))
            {
                ShowError(_loc.ErrorInvalidMove);
                continue;
            }

            return coords;
        }
    }

    // ── Будує Panel з дошкою і статусом ─────────────────────────────────────

    private Panel BuildGamePanel(GameState state, Coords[] validMoves)
    {
        var grid = new Grid();
        grid.AddColumn();

        var playerTag = state.CurrentPlayer == Player.Black
            ? $"[bold black on white] {_loc.LabelBlack} [/]"
            : $"[bold white on grey] {_loc.LabelWhite} [/]";

        var scoreText = new Markup(
            $"  [bold white]{_loc.LabelBlack}:[/] [green]{state.Board.BlackCells}[/]   " +
            $"[bold white]{_loc.LabelWhite}:[/] [green]{state.Board.WhiteCells}[/]   " +
            $"[grey]|[/]   {_loc.LabelTurn}: {playerTag}   " +
            $"[grey]{_loc.LabelToMove}[/]");

        grid.AddRow(scoreText);
        grid.AddRow(new Text(""));
        grid.AddRow(BoardRenderer.BuildTable(state.Board, validMoves));

        return new Panel(grid)
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Green)
            .Header($"[bold green] REVERSI [/]", Justify.Center)
            .Padding(1, 0);
    }

    // ── Меню ─────────────────────────────────────────────────────────────────

    private GameSettings? ShowMainMenu()
    {
        Console.Clear();
        DrawTitle();
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[bold green]{_loc.MenuNavigationHint}[/]")
                .HighlightStyle(new Style(Color.Black, Color.Green))
                .AddChoices(_loc.MenuNewGameLocal, _loc.MenuNewGameNetwork, _loc.MenuQuit));

        if (choice == _loc.MenuQuit)
            return null;

        if (choice == _loc.MenuNewGameLocal)
            return new GameSettings { GameType = GameType.Local };

        return ShowNetworkMenu();
    }

    private GameSettings? ShowNetworkMenu()
    {
        Console.Clear();
        DrawTitle();
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[bold green]{_loc.MenuNavigationHint}[/]")
                .HighlightStyle(new Style(Color.Black, Color.Green))
                .AddChoices(_loc.MenuNetworkHost, _loc.MenuNetworkClient, _loc.MenuQuit));

        if (choice == _loc.MenuQuit)
            return null;

        if (choice == _loc.MenuNetworkHost)
        {
            var port = AnsiConsole.Ask<ushort>($"[bold green]{_loc.PromptPort}[/]");

            var colorChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[bold green]{_loc.PromptChooseColor}[/]")
                    .HighlightStyle(new Style(Color.Black, Color.Green))
                    .AddChoices(_loc.ColorBlack, _loc.ColorWhite));

            var hostPlayer = colorChoice == _loc.ColorBlack ? Player.Black : Player.White;

            return new GameSettings
            {
                GameType = GameType.NetworkHost,
                HostSettings = new HostSettings { Port = port, HostPlayer = hostPlayer }
            };
        }
        else
        {
            var host = AnsiConsole.Prompt(
                new TextPrompt<string>($"[bold green]{_loc.PromptHost}[/]")
                    .Validate(ip => System.Net.IPAddress.TryParse(ip, out var addr)
                                    && addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                        ? ValidationResult.Success()
                        : ValidationResult.Error(_loc.ErrorInvalidIp)));

            var port = AnsiConsole.Ask<ushort>($"[bold green]{_loc.PromptPort}[/]");

            return new GameSettings
            {
                GameType = GameType.NetworkClient,
                ClientSettings = new ClientSettings { Host = host, Port = port }
            };
        }
    }

    private void ShowGameOver(GameStatus status)
    {
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

    // ── Допоміжні методи ─────────────────────────────────────────────────────

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
