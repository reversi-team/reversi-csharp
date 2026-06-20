namespace Reversi.Core;

public enum ViewMessage
{
    None,
}

public interface IView
{
    /// <summary>
    /// Рендерить меню у внутрішній буфер.
    /// </summary>
    /// <param name="options">Список опцій в меню.</param>
    /// <exception cref="ArgumentException">Виникає якщо передати порожній список.</exception>
    void RenderMenu(string[] options);

    /// <summary>
    /// Рендерить поточний стан гри у внутрішній буфер.
    /// </summary>
    /// <param name="gameState">Поточний стан гри.</param>
    /// <param name="message">Повідомлення для відображення (опціонально)</param>
    void RenderGame(GameState gameState, ViewMessage message =  ViewMessage.None);
    
    /// <summary>
    /// Оновлює екран (очищає його і виводить внутрішній буфер).
    /// </summary>
    void Draw();
    
    /// <summary>
    /// Повертає 0-базований індекс обраного варіанту.
    /// </summary>
    /// <remarks>
    /// Необхідно валідувати ввід. Якщо некоректний -- запросити повторно.
    /// </remarks>
    /// <param name="options">Список валідних варіантів.</param>
    /// <exception cref="ArgumentException">Виникає якщо передати порожній список.</exception>
    /// <returns>0-базований індекс обраного варіанту.</returns>
    byte GetOption(string[] options);
    
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
