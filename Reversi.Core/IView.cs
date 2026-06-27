namespace Reversi.Core;

public interface IView<TController> where TController : IController
{
    /// <summary>
    /// Точка входу застосунку. Керує повним життєвим циклом програми,
    /// включно з меню, налаштуваннями та повторними ігровими сесіями через <typeparamref name="TController"/>.
    /// </summary>
    /// <param name="args">Аргументи командного рядка передані програмі.</param>
    /// <returns>Код завершення програми.</returns>
    int Main(string[] args);
}
