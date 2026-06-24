using System.Net.Sockets;
using Reversi.Core;
using Reversi.Network.Protocol;

namespace Reversi.Network;

public class NetworkConnection(NetworkStream stream) : IDisposable
{
    private readonly BinaryReader _reader = new(stream);
    private readonly BinaryWriter _writer = new(stream);

    public void Dispose()
    {
        _reader.Dispose();
        _writer.Dispose();

        GC.SuppressFinalize(this);
    }

    public void SendMessage(NetworkMessage message)
    {
        try
        {
            _writer.Write((byte)message.Type);
            switch (message)
            {
                case AcceptConnectMessage m:
                    _writer.Write((byte)m.Player);
                    break;
                case MoveMessage m:
                    _writer.Write(m.Coords.X);
                    _writer.Write(m.Coords.Y);
                    break;
            }

            _writer.Flush();
        }
        catch (IOException e)
        {
            throw new NetworkError(inner: e);
        }
    }

    public NetworkMessage ReceiveMessage()
    {
        MessageType type;
        try
        {
            type = (MessageType)_reader.ReadByte();
        }
        catch (EndOfStreamException e)
        {
            throw new DisconnectException(inner: e);
        }
        catch (IOException e)
        {
            throw new NetworkError(inner: e);
        }


        switch (type)
        {
            case MessageType.AcceptConnect:
                var player = (Player)_reader.ReadByte();
                return new AcceptConnectMessage(player);
            case MessageType.Move:
                var coords = new Coords
                {
                    X = _reader.ReadByte(),
                    Y = _reader.ReadByte()
                };
                return new MoveMessage(coords);
            case MessageType.StatusOk:
                return new StatusOkMessage();
            case MessageType.StatusError:
                return new StatusErrorMessage();
            case MessageType.Connect:
                return new ConnectMessage();
            case MessageType.Pass:
                return new PassMessage();
            default:
                throw new ProtocolException("Unknown message type: " + (byte)type);
        }
    }
}
