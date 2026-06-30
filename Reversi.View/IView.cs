using Reversi.Core;

namespace Reversi.View;

/// <summary>
/// Контракт View для Reversi.
/// Єдина точка входу — метод Main.
/// Під час гри View передає контролеру делегати для взаємодії з користувачем.
/// </summary>
public interface IView
{
    /// <summary>
    /// Запускає програму: показує меню, керує навігацією,
    /// запускає ігрові сесії через controller.Play(...) і завершує роботу.
    /// </summary>
    void Main<TController>() where TController : IController;
}
