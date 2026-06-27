namespace Reversi.Core;

public enum ViewMessage
{
    None,
}

public interface IView<TController> where TController : IController
{
    int Main(string[] args);
}
