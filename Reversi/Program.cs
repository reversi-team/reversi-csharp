namespace Reversi;

internal static class Program
{
    public static int Main(string[] args)
    {
        var view = new View.View<Controller.Controller<Model.Model>>();
        return view.Main(args);
    }
}
