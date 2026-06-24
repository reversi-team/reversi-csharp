namespace Reversi.Network;

public class NetworkException(string message, Exception? inner = null)
    : Exception(message, inner);

public class ProtocolException(string message, Exception? inner = null)
    : NetworkException(message, inner);

public class DisconnectException(string message = "Opponent disconnected.", Exception? inner = null)
    : NetworkException(message, inner);

public class NetworkError(string message = "Connection lost.", Exception? inner = null)
    : NetworkException(message, inner);

public class NetworkNotConnectedException(
    string message = "Cannot perform this operation because the network connection has not been established.",
    Exception? inner = null)
    : InvalidOperationException(message, inner);
