namespace Reversi.Network;

/// <summary>
/// Serves as the base class for network-related exceptions in the Reversi game.
/// </summary>
public class NetworkException(string message, Exception? inner = null)
    : Exception(message, inner);

/// <summary>
/// Thrown when the received network data violates the game protocol or an unexpected message sequence occurs.
/// </summary>
public class ProtocolException(string message, Exception? inner = null)
    : NetworkException(message, inner);

/// <summary>
/// Thrown when the remote peer closes the connection or the stream ends unexpectedly.
/// </summary>
public class DisconnectException(string message = "Opponent disconnected.", Exception? inner = null)
    : NetworkException(message, inner);

/// <summary>
/// Thrown when a generic network error or transport-level failure occurs
/// </summary>
public class NetworkErrorException(string message = "Connection lost.", Exception? inner = null)
    : NetworkException(message, inner);

/// <summary>
/// Thrown when an operation is performed on an uninitialized or unestablished network connection.
/// </summary>
/// <remarks>
/// Parent is <see cref="InvalidOperationException"/>.
/// This usually indicates a developer logic mistake and means that the connection reference is null.
/// </remarks>
public class NetworkNotConnectedException(
    string message = "Cannot perform this operation because the network connection has not been established.",
    Exception? inner = null)
    : InvalidOperationException(message, inner);
