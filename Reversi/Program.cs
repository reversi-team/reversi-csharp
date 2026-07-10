using Reversi.Controller;
using Reversi.Model;
using Reversi.View;

namespace Reversi;

internal static class Program
{
    public static int Main(string[] args)
    {
        var view = new View<Controller<GameModel>>();
        return view.Main(args);
    }
}
