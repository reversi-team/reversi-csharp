namespace Reversi.Network.Protocol;

public abstract class NetworkMessage
{
    public abstract MessageType Type { get; }
}

public class ConnectMessage : NetworkMessage
{
    public override MessageType Type => MessageType.Connect;
}

public class AcceptConnectMessage(Core.Player player) : NetworkMessage
{
    public override MessageType Type => MessageType.AcceptConnect;
    public Core.Player Player { get; } = player;
}

public class StatusOkMessage : NetworkMessage
{
    public override MessageType Type => MessageType.StatusOk;
}

public class StatusErrorMessage : NetworkMessage
{
    public override MessageType Type => MessageType.StatusError;
}

public class MoveMessage(Core.Coords coords) : NetworkMessage
{
    public override MessageType Type => MessageType.Move;
    public Core.Coords Coords { get; } = coords;
}

public class PassMessage : NetworkMessage
{
    public override MessageType Type => MessageType.Pass;
}
