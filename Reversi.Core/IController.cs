namespace Reversi.Core;

public interface IController
{
    static abstract GameStatus Play(
        GameSettings gameSettings,
        Action<GameState> drawGame,
        Func<Coords[], Coords> askMove
    );
}
