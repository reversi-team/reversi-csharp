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

public class ControllerException(Exception? inner = null)
    : ReversiException("ControllerException", inner);

public class UnexpectedModelException(ModelException inner)
    : ControllerException(inner);

public class OpponentDisconnectedException(Exception inner)
    : ControllerException(inner);

public class ControllerNetworkErrorException(Exception? inner = null)
    : ControllerException(inner);

public class UnhandledException(string message, Exception? inner = null)
    : Exception(message, inner);
