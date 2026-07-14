using Reversi.View.Models;

namespace Reversi.View.Helpers;

/// <summary>
/// Усі рядки інтерфейсу для конкретної мови.
/// Щоб додати нову мову — додай метод за аналогією з English/Ukrainian.
/// </summary>
internal sealed class Localization
{
    // ── Меню ─────────────────────────────────────────────────────────────────
    public required string MenuNavigationHint { get; init; }
    public required string MenuNewGameLocal { get; init; }
    public required string MenuNewGameNetwork { get; init; }
    public required string MenuNetworkHost { get; init; }
    public required string MenuNetworkClient { get; init; }
    public required string MenuQuit { get; init; }

    // ── Мережеві налаштування ─────────────────────────────────────────────────
    public required string PromptPort { get; init; }
    public required string PromptHost { get; init; }
    public required string PromptChooseColor { get; init; }
    public required string ColorBlack { get; init; }
    public required string ColorWhite { get; init; }

    // ── Рахунок і хід ────────────────────────────────────────────────────────
    public required string LabelBlack { get; init; }
    public required string LabelWhite { get; init; }
    public required string LabelTurn { get; init; }
    public required string LabelToMove { get; init; }

    // ── Введення ходу ────────────────────────────────────────────────────────
    public required string PromptEnterMove { get; init; }
    public required string ErrorInvalidCell { get; init; }  // {0} = введене значення
    public required string ErrorInvalidMove { get; init; }  // хід не у списку допустимих

    // ── Кінець гри ───────────────────────────────────────────────────────────
    public required string GameOverHeader { get; init; }
    public required string BlackWins { get; init; }
    public required string WhiteWins { get; init; }
    public required string Draw { get; init; }
    public required string FinalScore { get; init; }
    public required string PlayAgain { get; init; }

    // ── Повідомлення про події під час гри ───────────────────────────────────
    public required string ErrorInvalidIp { get; init; }
    public required string MsgImpossibleMove { get; init; }
    public required string MsgImpossiblePass { get; init; }
    public required string MsgGameEnded { get; init; }

    // ── Критичні помилки ─────────────────────────────────────────────────────
    public required string ErrUnknown { get; init; }

    // ── Загальне ─────────────────────────────────────────────────────────────
    public required string PressAnyKey { get; init; }

    // ── Фабрика ──────────────────────────────────────────────────────────────
    internal static Localization For(Language lang) => lang switch
    {
        Language.Ukrainian => Ukrainian(),
        _ => English()
    };

    private static Localization English() => new()
    {
        ErrorInvalidIp = "Invalid IP address. Use IPv4 format (e.g. 192.168.1.1).",
        MenuNavigationHint = "Use [[↑↓]] to navigate, [[Enter]] to select",
        MenuNewGameLocal = "▶  Local Game",
        MenuNewGameNetwork = "▶  Network Game",
        MenuNetworkHost = "▶  Create Game (Host)",
        MenuNetworkClient = "▶  Join Game (Client)",
        MenuQuit = "✕  Quit",

        PromptPort = "Enter port number:",
        PromptHost = "Enter host address:",
        PromptChooseColor = "Choose your color:",
        ColorBlack = "[navy]● Black[/]",
        ColorWhite = "[yellow]● White[/]",

        LabelBlack = "[navy]● Black[/]",
        LabelWhite = "[yellow]● White[/]",
        LabelTurn = "Turn",
        LabelToMove = "to move",

        PromptEnterMove = "[bold green]>[/] Enter move (e.g. [bold]D3[/]):",
        ErrorInvalidCell = "'{0}' is not a valid cell. Use A–H and 1–8 (e.g. D3).",
        ErrorInvalidMove = "That cell is not a valid move. Try again.",

        GameOverHeader = " Game Over ",
        BlackWins = "  [navy]● Black[/] wins!  ",
        WhiteWins = "  [yellow]● White[/] wins!  ",
        Draw = "  It's a draw!  ",
        FinalScore = "Final score",
        PlayAgain = "Play again?",

        MsgImpossibleMove = "That move is not allowed. Try again.",
        MsgImpossiblePass = "No valid moves — turn skipped.",
        MsgGameEnded = "The game has already ended.",

        ErrUnknown = "An unexpected error occurred.",

        PressAnyKey = "  Press any key to continue…",
    };

    private static Localization Ukrainian() => new()
    {
        ErrorInvalidIp = "Невірна IP адреса. Використовуй формат IPv4 (напр. 192.168.1.1).",
        MenuNavigationHint = "Використовуй [[↑↓]] для навігації, [[Enter]] для вибору",
        MenuNewGameLocal = "▶  Локальна гра",
        MenuNewGameNetwork = "▶  Мережева гра",
        MenuNetworkHost = "▶  Створити гру (хост)",
        MenuNetworkClient = "▶  Приєднатися до гри (клієнт)",
        MenuQuit = "✕  Вийти",

        PromptPort = "Введи номер порту:",
        PromptHost = "Введи адресу хоста:",
        PromptChooseColor = "Обери свій колір:",
        ColorBlack = "[navy]● Чорні[/]",
        ColorWhite = "[yellow]● Білі[/]",

        LabelBlack = "[navy]● Чорні[/]",
        LabelWhite = "[yellow]● Білі[/]",
        LabelTurn = "Хід",
        LabelToMove = "ходять",

        PromptEnterMove = "[bold green]>[/] Введи хід (напр. [bold]D3[/]):",
        ErrorInvalidCell = "'{0}' — невірна клітинка. Використовуй A–H та 1–8 (напр. D3).",
        ErrorInvalidMove = "Ця клітинка не є допустимим ходом. Спробуй інший.",

        GameOverHeader = " Гра завершена ",
        BlackWins = "  [navy]● Чорні[/] перемогли!  ",
        WhiteWins = "  [yellow]● Білі[/] перемогли!  ",
        Draw = "  Нічия!  ",
        FinalScore = "Фінальний рахунок",
        PlayAgain = "Зіграти ще раз?",

        MsgImpossibleMove = "Цей хід неможливий. Спробуй інший.",
        MsgImpossiblePass = "Немає можливих ходів — хід пропущено.",
        MsgGameEnded = "Гра вже завершена.",

        ErrUnknown = "Сталася неочікувана помилка.",

        PressAnyKey = "  Натисни будь-яку клавішу, щоб продовжити…",
    };
}
