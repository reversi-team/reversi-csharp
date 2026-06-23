namespace Reversi.Network.Protocol;

public class NetworkMessage
{
    public MessageType Type { get; set; }
    public byte[]? Data { get; set; }
}

public class ConnectMessage : NetworkMessage
{
    public new static MessageType Type => MessageType.Connect;
    public new static byte[]? Data => null;
}

public class AcceptConnectMessage(Core.Player player) : NetworkMessage
{
    public new static MessageType Type => MessageType.AcceptConnect;
    public new byte[]? Data { get; init; } = [(byte)player];
}

public class StatusOkMessage : NetworkMessage
{
    public new static MessageType Type => MessageType.StatusOk;
}

public class StatusErrorMessage : NetworkMessage
{
    public new static MessageType Type => MessageType.StatusError;
}

public class MoveMessage(Core.Coords coords) : NetworkMessage
{
    public new static MessageType Type => MessageType.Move;
    public new byte[]? Data { get; init; } = [coords.X, coords.Y];
}

public class PassMessage : NetworkMessage
{
    public new static MessageType Type => MessageType.Pass;
}
