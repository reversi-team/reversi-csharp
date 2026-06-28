namespace Reversi.Core;

public interface IController
{
    /// <summary>
    /// Запускає одну ігрову сесію від початку до кінця.
    /// Щоразу створює новий екземпляр моделі всередині.
    /// </summary>
    /// <exception cref="UnexpectedModelException">Неочікувана помилка в моделі. Свідчить про баг в коді.</exception>
    /// <exception cref="OpponentDisconnectedException">Опонент закрив з'єднання під час мережевої гри.</exception>
    /// <exception cref="ControllerNetworkErrorException">Під час обробки мережевих запитів сталася помилка.</exception>
    /// <exception cref="UnhandledException">Неперехоплений виняток. Свідчить про баг в коді.</exception>
    /// <param name="gameSettings">Налаштування для ініціалізації ігрової сесії.</param>
    /// <param name="drawGame">
    /// Колбек який викликається контролером для відображення поточного стану гри.
    /// View відповідає за виведення стану користувачу.
    /// </param>
    /// <param name="askMove">
    /// Колбек який викликається контролером для отримання ходу від активного гравця.
    /// View отримує список валідних ходів і повертає обрані <see cref="Coords"/>.
    /// </param>
    /// <returns><see cref="GameStatus"/> що представляє результат завершеної гри.</returns>
    static abstract GameState Play(
        GameSettings gameSettings,
        Action<GameState> drawGame,
        Func<Coords[], Coords> askMove
    );
}
