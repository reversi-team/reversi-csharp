namespace Reversi.Network.Protocol;

public enum MessageType : byte
{
    StatusOk,
    StatusError,
    Connect,
    AcceptConnect,
    Move,
    Pass,
}
