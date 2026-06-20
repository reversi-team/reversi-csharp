namespace Reversi.Core;

public class ReversiException(string message, Exception? inner = null)
    : Exception(message, inner);

public class ModelException(string message, Exception? inner = null) 
    : ReversiException(message, inner);

public class ImpossibleMoveException(string message, Exception? inner = null) 
    : ModelException(message, inner);

public class ImpossiblePassException(string message, Exception? inner = null) 
    : ModelException(message, inner);

public class GameEndedException(string message, Exception? inner = null) 
    : ModelException(message, inner);

public class ViewException(string message, Exception? inner = null) 
    : ReversiException(message, inner);
    