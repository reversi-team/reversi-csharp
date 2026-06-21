namespace Reversi.Core;

public interface IModel
{
    /// <summary>
    /// Надає актуальний стан гри.
    /// </summary>
    /// <returns>Актуальний стан гри.</returns>
    public GameState CurrentState();

    /// <summary>
    /// Надає список можливих ходів.
    /// </summary>
    /// <remarks>
    /// Controller зобов'язаний викликати цей метод і в разі відсутності можливих ходів викликати Pass.
    /// </remarks>
    /// <returns>Список можливих ходів.</returns>
    public Coords[] PossibleMoves();

    /// <summary>
    /// Виконує хід для активного гравця.
    /// </summary>
    /// <param name="coords">Координати для ходу</param>
    /// <exception cref="GameEndedException">Виникає, якщо викликати метод після закінчення гри.</exception>
    /// <exception cref="ImpossibleMoveException">Виникає, якщо хід за переданими координатами неможливий.</exception>
    /// <returns>Актуальний стан гри.</returns>
    public GameState Move(Coords coords);

    /// <summary>
    /// Пропускає хід, якщо гравець не має можливих ходів.
    /// </summary>
    /// <remarks>
    /// Повинен викликатися виключно в разі відсутності можливих ходів.
    /// </remarks>
    /// <exception cref="GameEndedException">Виникає, якщо викликати метод після закінчення гри.</exception>
    /// <exception cref="ImpossiblePassException">Виникає, якщо гравець має можливі ходи.</exception>
    /// <returns>Актуальний стан гри.</returns>
    public GameState Pass();
}
