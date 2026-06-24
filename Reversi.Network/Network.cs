using System.Net;
using System.Net.Sockets;
using Reversi.Core;
using Reversi.Network.Protocol;

namespace Reversi.Network;

/// <summary>
/// Provides a base high-level abstraction for network operations in the Reversi game.
/// </summary>
public abstract class Network
{
    protected NetworkConnection? Connection;

    /// <summary>
    /// Gets a value indicating whether the network connection is currently active and established.
    /// </summary>
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
/// <param name="port">The port number on which to listen for incoming connection requests. Use 0 to let the OS assign a free port dynamically.</param>
public class Host(IPAddress ipAddress, int port) : Network
{
    private readonly TcpListener _listener = new(ipAddress, port);

    /// <summary>
    /// Gets the actual port number the listener is bound to. Useful when port 0 is provided during initialization.
    /// </summary>
    /// <remarks>
    /// Only valid after <see cref="AcceptConnection"/> has been called.
    /// Accessing this property before calling <see cref="AcceptConnection"/> will throw an <see cref="InvalidOperationException"/>.
    /// </remarks>
    public int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;

    /// <summary>
    /// Starts the listener, blocks execution until a remote client connects,
    /// and performs an application-level handshake by sending <see cref="AcceptConnectMessage"/>.
    /// </summary>
    /// <param name="opponentColor">The <see cref="Player"/> color to be assigned and sent to the connecting client.</param>
    /// <exception cref="NetworkErrorException">
    /// Thrown when a socket error occurs while starting the listener or accepting the client,
    /// or when an I/O error occurs while sending the handshake message.
    /// </exception>
    public void AcceptConnection(Player opponentColor)
    {
        try
        {
            _listener.Start();
            var client = _listener.AcceptTcpClient();
            _listener.Stop();

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            var stream = client.GetStream();
            Connection = new NetworkConnection(stream);

            Send(new AcceptConnectMessage(opponentColor));
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
    /// Establishes a connection to a remote host, performs the application-level handshake,
    /// and retrieves the assigned player color.
    /// </summary>
    /// <param name="ipAddress">The <see cref="IPAddress"/> of the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <returns>The <see cref="Player"/> color assigned to this client by the host.</returns>
    /// <exception cref="NetworkErrorException">Thrown when a socket error occurs while attempting to connect to the host.</exception>
    /// <exception cref="ProtocolException">Thrown if the host sends an unexpected message instead of the <see cref="AcceptConnectMessage"/> handshake.</exception>
    /// <exception cref="DisconnectException">Thrown if the host drops the connection before completing the handshake.</exception>
    public Player Connect(IPAddress ipAddress, int port)
    {
        try
        {
            _client.Connect(ipAddress, port);

            var stream = _client.GetStream();
            Connection = new NetworkConnection(stream);

            var result = ReceiveMessage<AcceptConnectMessage>().Player;
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            return result;
        }
        catch (SocketException e)
        {
            throw new NetworkErrorException("Failed to connect to the remote host. The socket is not available.", e);
        }
    }
}
