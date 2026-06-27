namespace Reversi.Core;

public interface IController
{
    /// <summary>
    /// Запускає одну ігрову сесію від початку до кінця.
    /// Щоразу створює новий екземпляр моделі всередині.
    /// </summary>
    /// <exception cref="Exception">Поки повний список не буде додано в документацію, можна перехоплювати базовий клас.</exception>
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
    static abstract GameStatus Play(
        GameSettings gameSettings,
        Action<GameState> drawGame,
        Func<Coords[], Coords> askMove
    );
}
