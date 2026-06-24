using System.Net;
using System.Net.Sockets;
using Reversi.Network.Protocol;

namespace Reversi.Network;

public abstract class Network
{
    protected NetworkConnection? Connection;

    public void Disconnect()
    {
        if (Connection == null)
        {
            return;
        }

        Connection.Dispose();
        Connection = null;
    }

    public void Send(NetworkMessage message)
    {
        if (Connection == null)
        {
            throw new NetworkNotConnectedException();
        }

        Connection.SendMessage(message);
    }

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

public class Host(IPAddress ipAddress, int port) : Network
{
    private readonly TcpListener _listener = new(ipAddress, port);

    public void AcceptConnection()
    {
        _listener.Start();
        var client = _listener.AcceptTcpClient();
        _listener.Stop();

        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        var stream = client.GetStream();
        Connection = new NetworkConnection(stream);
    }
}

public class Client : Network
{
    private readonly TcpClient _client = new();

    public void Connect(IPAddress ipAddress, int port)
    {
        _client.Connect(ipAddress, port);
        _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        var stream = _client.GetStream();
        Connection = new NetworkConnection(stream);
    }
}
