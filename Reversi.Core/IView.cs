namespace Reversi.Core;

public enum ViewMessage
{
    None,
}

public interface IView
{
    /// <summary>
    /// Запускає View у режим меню/налаштувань. View самостійно показує всі необхідні екрани
    /// (головне меню, вибір режиму гри, мережеві налаштування тощо) і збирає від користувача потрібні дані.
    /// </summary>
    /// <returns>Налаштування для нової гри.</returns>
    GameSettings Start();

    /// <summary>
    /// Завершує ігровий цикл і показує результат гри.
    /// View самостійно вирішує, що пропонувати користувачу далі (зіграти ще раз, повернутись в меню тощо).
    /// </summary>
    /// <param name="gameState">Фінальний стан гри.</param>
    /// <returns>
    /// Налаштування для наступної гри, якщо користувач вирішив продовжити;
    /// <c>null</c>, якщо користувач вирішив завершити програму.
    /// </returns>
    GameSettings? End(GameState gameState);

    /// <summary>
    /// Рендерить поточний стан гри у внутрішній буфер.
    /// </summary>
    /// <param name="gameState">Поточний стан гри.</param>
    /// <param name="message">Повідомлення для відображення (опціонально)</param>
    void RenderGame(GameState gameState, ViewMessage message = ViewMessage.None);

    /// <summary>
    /// Оновлює екран (очищає його і виводить внутрішній буфер).
    /// </summary>
    void Draw();

    /// <summary>
    /// Повертає введені координати.
    /// </summary>
    /// <remarks>
    /// Необхідно валідувати ввід. Якщо некоректний -- запросити повторно.
    /// </remarks>
    /// <param name="possibleMoves">Список валідних координат.</param>
    /// <exception cref="ArgumentException">Виникає якщо передати порожній список.</exception>
    /// <returns>Введені координати.</returns>
    Coords GetMove(Coords[] possibleMoves);
}
