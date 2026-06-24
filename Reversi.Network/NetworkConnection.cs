using System.Net.Sockets;
using Reversi.Core;
using Reversi.Network.Protocol;

namespace Reversi.Network;

/// <summary>
/// Manages sending and receiving protocol messages over a network stream using binary serialization.
/// </summary>
/// <param name="stream">The underlying network stream used for data transport.</param>
public class NetworkConnection(NetworkStream stream) : IDisposable
{
    private readonly BinaryReader _reader = new(stream);
    private readonly BinaryWriter _writer = new(stream);

    /// <summary>
    /// Releases all resources used by the readers, writers, and the underlying stream.
    /// </summary>
    public void Dispose()
    {
        _reader.Dispose();
        _writer.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Sends a protocol message to the remote peer.
    /// </summary>
    /// <param name="message">The network message to serialize and transmit.</param>
    /// <exception cref="NetworkErrorException">Thrown when an I/O error occurs while writing data to the stream.</exception>
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
            throw new NetworkErrorException(inner: e);
        }
    }

    /// <summary>
    /// Receives and deserializes the next protocol message from the network stream.
    /// </summary>
    /// <returns>The deserialized <see cref="NetworkMessage"/> sent by the remote peer.</returns>
    /// <exception cref="DisconnectException">Thrown when the stream ends, indicating the remote peer has closed the connection.</exception>
    /// <exception cref="NetworkErrorException">Thrown when a generic transport-level or I/O error occurs.</exception>
    /// <exception cref="ProtocolException">Thrown when the received message type is invalid or unrecognized by the game protocol.</exception>
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
            throw new NetworkErrorException(inner: e);
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
