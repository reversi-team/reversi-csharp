using System.Net;
using System.Net.Sockets;
using Reversi.Network.Protocol;

namespace Reversi.Network;

/// <summary>
/// Provides a base high-level abstraction for network operations in the Reversi game.
/// </summary>
public abstract class Network
{
    protected NetworkConnection? Connection;
    public bool Connected => Connection != null;

    /// <summary>
    /// Closes the active network connection and releases all allocated transport resources.
    /// </summary>
    public void Disconnect()
    {
        if (Connection == null)
        {
            return;
        }

        Connection.Dispose();
        Connection = null;
    }

    /// <summary>
    /// Sends a protocol message to the remote peer.
    /// </summary>
    /// <param name="message">The <see cref="NetworkMessage"/> to transmit.</param>
    /// <exception cref="NetworkNotConnectedException">Thrown when there is no active connection established.</exception>
    /// <exception cref="NetworkErrorException">Thrown when an underlying I/O error occurs during transmission.</exception>
    public void Send(NetworkMessage message)
    {
        if (Connection == null)
        {
            throw new NetworkNotConnectedException();
        }

        Connection.SendMessage(message);
    }

    /// <summary>
    /// Receives the next message from the stream and ensures it matches the expected type.
    /// </summary>
    /// <typeparam name="T">The expected type of the incoming <see cref="NetworkMessage"/>.</typeparam>
    /// <returns>The deserialized network message cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="NetworkNotConnectedException">Thrown when there is no active connection established.</exception>
    /// <exception cref="ProtocolException">Thrown when the received message type does not match <typeparamref name="T"/> or violates the protocol.</exception>
    /// <exception cref="DisconnectException">Thrown when the remote peer gracefully closes the connection.</exception>
    /// <exception cref="NetworkErrorException">Thrown when a generic transport-level error occurs.</exception>
    public T ReceiveMessage<T>() where T : NetworkMessage
    {
        if (Connection == null)
        {
            throw new NetworkNotConnectedException();
        }

        var message = Connection.ReceiveMessage();
        if (message is T expectedMessage)
        {
            return expectedMessage;
        }

        throw new ProtocolException($"Expected {typeof(T).Name}, got {message.GetType().Name}");
    }
}

/// <summary>
/// Represents a network host (server) that listens for and accepts an incoming connection from a client.
/// </summary>
/// <param name="ipAddress">The local <see cref="IPAddress"/> to bind the listener to.</param>
/// <param name="port">The port number on which to listen for incoming connection requests.</param>
public class Host(IPAddress ipAddress, int port) : Network
{
    private readonly TcpListener _listener = new(ipAddress, port);

    /// <summary>
    /// Starts the listener and blocks execution until a remote client connects.
    /// </summary>
    /// <exception cref="NetworkErrorException">Thrown when a socket error occurs while starting the listener or accepting the client.</exception>
    public void AcceptConnection()
    {
        try
        {
            _listener.Start();
            var client = _listener.AcceptTcpClient();
            _listener.Stop();

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            var stream = client.GetStream();
            Connection = new NetworkConnection(stream);
        }
        catch (SocketException e)
        {
            throw new NetworkErrorException("The socket is not available or the port is already in use.", e);
        }
    }
}

/// <summary>
/// Represents a network client that initiates a connection to a remote host.
/// </summary>
public class Client : Network
{
    private readonly TcpClient _client = new();

    /// <summary>
    /// Establishes a connection to a remote host at the specified IP address and port.
    /// </summary>
    /// <param name="ipAddress">The <see cref="IPAddress"/> of the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <exception cref="NetworkErrorException">Thrown when a socket error occurs while attempting to connect to the host.</exception>
    public void Connect(IPAddress ipAddress, int port)
    {
        try
        {
            _client.Connect(ipAddress, port);
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            var stream = _client.GetStream();
            Connection = new NetworkConnection(stream);
        }
        catch (SocketException e)
        {
            throw new NetworkErrorException("Failed to connect to the remote host. The socket is not available.", e);
        }
    }
}
