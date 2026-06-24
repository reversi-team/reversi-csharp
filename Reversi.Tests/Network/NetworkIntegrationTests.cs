using System.Net;
using Reversi.Core;
using Reversi.Network;
using Reversi.Network.Protocol;

namespace Reversi.Tests.Network;

public class NetworkIntegrationTests
{
    private readonly IPAddress _localhost = IPAddress.Loopback;
    private const int _port = 0;
    private const int _waitForConnection = 50;
    private const int _waitForMessage = 2000;

    private (Host, Client, Player) EstablishConnection(Player player = Player.White)
    {
        var host = new Host(_localhost, _port);
        var client = new Client();

        var hostThread = new Thread(() => host.AcceptConnection(player));
        hostThread.Start();
        Thread.Sleep(_waitForConnection);

        var receivedPlayer = client.Connect(_localhost, host.Port);
        hostThread.Join(_waitForMessage);

        return (host, client, receivedPlayer);
    }

    [Fact]
    public void TestNetwork_SetConnection_Connected()
    {
        var (host, client, _) = EstablishConnection();

        Assert.True(host.Connected);
        Assert.True(client.Connected);
    }

    public static IEnumerable<object[]> GetMessagesToTest()
    {
        yield return [new StatusOkMessage()];
        yield return [new StatusErrorMessage()];
        yield return [new ConnectMessage()];
        yield return [new PassMessage()];
    }

    [Theory]
    [MemberData(nameof(GetMessagesToTest))]
    public void SendAndReceiveNodataMessage_VariousMessages_SuccessfullyTransmitted(NetworkMessage msgToSend)
    {
        var (host, client, _) = EstablishConnection();

        {
            host.Send(msgToSend);
            var msgToReceive = client.ReceiveMessage<NetworkMessage>();

            Assert.Equal(msgToSend.Type, msgToReceive.Type);
        }
        {
            client.Send(msgToSend);
            var msgToReceive = host.ReceiveMessage<NetworkMessage>();

            Assert.Equal(msgToSend.Type, msgToReceive.Type);
        }
    }

    [Fact]
    public void ReceiveAcceptMessage_PlayerWhite_SuccessfullyTransmitted()
    {
        var (_, _, receivedPlayer) = EstablishConnection(Player.White);
        var expected = new AcceptConnectMessage(Player.White);

        Assert.Equal(expected.Player, receivedPlayer);
    }

    [Fact]
    public void ReceiveAcceptMessage_PlayerBlack_SuccessfullyTransmitted()
    {
        var (_, _, receivedPlayer) = EstablishConnection(Player.Black);
        var expected = new AcceptConnectMessage(Player.Black);

        Assert.Equal(expected.Player, receivedPlayer);
    }

    [Fact]
    public void SendAndReceiveMoveMessage_ExactCoords_SuccessfullyTransmitted()
    {
        var (host, client, _) = EstablishConnection();
        var msgToSend = new MoveMessage(new Coords(1, 2));

        {
            host.Send(msgToSend);
            var msgToReceive = client.ReceiveMessage<MoveMessage>();

            Assert.Equal(msgToSend.Type, msgToReceive.Type);
            Assert.Equal(msgToSend.Coords, msgToReceive.Coords);
        }
        {
            client.Send(msgToSend);
            var msgToReceive = host.ReceiveMessage<MoveMessage>();

            Assert.Equal(msgToSend.Type, msgToReceive.Type);
            Assert.Equal(msgToSend.Coords, msgToReceive.Coords);
        }
    }

    [Fact]
    public void ReceiveMessage_HostDisconnects_DisconnectException()
    {
        var (host, client, _) = EstablishConnection();

        host.Disconnect();
        Assert.Throws<DisconnectException>(client.ReceiveMessage<NetworkMessage>);

        client.Disconnect();
    }

    [Fact]
    public void ReceiveMessage_ClientDisconnects_DisconnectException()
    {
        var (host, client, _) = EstablishConnection();

        client.Disconnect();
        Assert.Throws<DisconnectException>(host.ReceiveMessage<NetworkMessage>);

        host.Disconnect();
    }

    [Fact]
    public void ReceiveMessage_WrongType_ProtocolException()
    {
        var (host, client, _) = EstablishConnection();

        client.Send(new PassMessage());
        Assert.Throws<ProtocolException>(host.ReceiveMessage<MoveMessage>);

        host.Send(new PassMessage());
        Assert.Throws<ProtocolException>(client.ReceiveMessage<MoveMessage>);

        host.Disconnect();
        client.Disconnect();
    }

    [Fact]
    public void SendMessage_NetworkNotConnected_NetworkNotConnectedException()
    {
        var host = new Host(_localhost, _port);
        var client = new Client();

        Assert.Throws<NetworkNotConnectedException>(() => host.Send(new StatusOkMessage()));
        Assert.Throws<NetworkNotConnectedException>(() => client.Send(new StatusOkMessage()));

        host.Disconnect();
        client.Disconnect();
    }

    [Fact]
    public void ReceiveMessage_NetworkNotConnected_NetworkNotConnectedException()
    {
        var host = new Host(_localhost, _port);
        var client = new Client();

        Assert.Throws<NetworkNotConnectedException>(host.ReceiveMessage<MoveMessage>);
        Assert.Throws<NetworkNotConnectedException>(client.ReceiveMessage<MoveMessage>);

        host.Disconnect();
        client.Disconnect();
    }
}
