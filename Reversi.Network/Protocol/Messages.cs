namespace Reversi.Network.Protocol;

public abstract class NetworkMessage
{
    public abstract MessageType Type { get; }
}

/// <summary>
/// Requests a connection from the client to the host.
/// </summary>
public class ConnectMessage : NetworkMessage
{
    public override MessageType Type => MessageType.Connect;
}

/// <summary>
/// Notifies that the host accepted the connection and assigns a side to the client.
/// </summary>
/// <param name="player">The color allocated for the connected client.</param>
public class AcceptConnectMessage(Core.Player player) : NetworkMessage
{
    public override MessageType Type => MessageType.AcceptConnect;
    public Core.Player Player { get; } = player;
}

/// <summary>
/// Sent when the last received message was valid and successfully processed.
/// </summary>
public class StatusOkMessage : NetworkMessage
{
    public override MessageType Type => MessageType.StatusOk;
}

/// <summary>
/// Sent when the last received message was invalid (e.g., an impossible move).
/// </summary>
/// <remarks>Usually, the host terminates the connection after sending or receiving this message.</remarks>
public class StatusErrorMessage : NetworkMessage
{
    public override MessageType Type => MessageType.StatusError;
}

/// <summary>
/// Transmits the player's move coordinates to the remote peer.
/// </summary>
/// <param name="coords">The <see cref="Core.Coords"/> where the disk should be placed.</param>
public class MoveMessage(Core.Coords coords) : NetworkMessage
{
    public override MessageType Type => MessageType.Move;
    public Core.Coords Coords { get; } = coords;
}

/// <summary>
/// Sent when a player has no valid moves available and must skip a turn.
/// </summary>
public class PassMessage : NetworkMessage
{
    public override MessageType Type => MessageType.Pass;
}
