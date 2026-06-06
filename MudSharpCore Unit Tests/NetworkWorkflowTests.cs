#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Network;
using MudSharp.PerceptionEngine.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NetworkWorkflowTests
{
	[TestMethod]
	public async Task PlayerConnection_DoesNotEnqueuePartialCommandBeforeLineEnding()
	{
		var fixture = await CreateConnectionFixture();
		try
		{
			WriteClientBytes(fixture.Client, Encoding.ASCII.GetBytes("loo"));
			await Task.Delay(25);
			fixture.Connection.PrepareIncoming();

			Assert.IsFalse(fixture.Connection.HasIncomingCommands);

			WriteClientBytes(fixture.Client, Encoding.ASCII.GetBytes("k\r"));
			await Task.Delay(25);
			fixture.Connection.PrepareIncoming();

			Assert.IsTrue(fixture.Connection.HasIncomingCommands);
			fixture.Connection.AttemptCommand();
			CollectionAssert.AreEqual(new[] { "look" }, fixture.Commands);
		}
		finally
		{
			fixture.Dispose();
		}
	}

	[TestMethod]
	public async Task PlayerConnection_DisconnectsWhenPartialCommandExceedsBufferLimit()
	{
		var fixture = await CreateConnectionFixture();
		try
		{
			var bytes = Enumerable.Repeat((byte)'x', Constants.PlayerConnectionBufferSize + 1).ToArray();
			WriteClientBytes(fixture.Client, bytes);

			await PumpIncomingUntilClosing(fixture.Connection);

			Assert.AreEqual(ConnectionState.Closing, fixture.Connection.State);
			Assert.IsFalse(fixture.Connection.HasIncomingCommands);
		}
		finally
		{
			fixture.Dispose();
		}
	}

	[TestMethod]
	public async Task PlayerConnection_DisconnectsWhenTelnetSubcommandExceedsBufferLimit()
	{
		var fixture = await CreateConnectionFixture();
		try
		{
			var bytes = new[] { Telnet.IAC, Telnet.SB }
				.Concat(Enumerable.Repeat((byte)'x', Constants.PlayerConnectionBufferSize))
				.ToArray();
			WriteClientBytes(fixture.Client, bytes);

			await PumpIncomingUntilClosing(fixture.Connection);

			Assert.AreEqual(ConnectionState.Closing, fixture.Connection.State);
			Assert.IsFalse(fixture.Connection.HasIncomingCommands);
		}
		finally
		{
			fixture.Dispose();
		}
	}

	[TestMethod]
	public async Task PlayerConnection_PreservesSplitTelnetNegotiationAndUsesEorPrompt()
	{
		var fixture = await CreateConnectionFixture();
		try
		{
			WriteClientBytes(fixture.Client, [Telnet.IAC, Telnet.DO]);
			await Task.Delay(25);
			fixture.Connection.PrepareIncoming();

			Assert.IsFalse(fixture.Connection.HasIncomingCommands);

			WriteClientBytes(fixture.Client, [Telnet.TELOPT_EOR]);
			await Task.Delay(25);
			fixture.Connection.PrepareIncoming();

			fixture.Connection.AddOutgoing("hello");
			fixture.Connection.SendOutgoing();

			var bytes = await ReadAvailableBytes(fixture.Client, 2);
			Assert.IsTrue(bytes.Length >= 2, "Expected output bytes from server.");
			CollectionAssert.AreEqual(new[] { Telnet.IAC, Telnet.EOR }, bytes[^2..]);
		}
		finally
		{
			fixture.Dispose();
		}
	}

	[TestMethod]
	public async Task PlayerConnection_DoesNotWarnWhenTimeoutDisabled()
	{
		var fixture = await CreateConnectionFixture(timeout: 0);
		try
		{
			fixture.Connection.WarnTimeout();

			Assert.IsFalse(fixture.Connection.HasOutgoingCommands);
		}
		finally
		{
			fixture.Dispose();
		}
	}

	[TestMethod]
	public void TcpServer_FloodWindowExpiresEvenAfterThreshold()
	{
		var server = new TCPServer(IPAddress.Loopback, 0);
		var address = IPAddress.Loopback;
		var now = DateTime.UtcNow;

		for (var i = 0; i < 30; i++)
		{
			Assert.IsFalse(server.RecordConnectionAttempt(address, now));
		}

		Assert.IsTrue(server.RecordConnectionAttempt(address, now));

		var later = now + server.IpFloodKeepAlive + TimeSpan.FromSeconds(1);
		Assert.IsFalse(server.RecordConnectionAttempt(address, later));
		Assert.AreEqual(1, server.ConnectionDictionary[address].NumberOfConnections);
	}

	[TestMethod]
	public void PlayerOutputHandler_PaginatesAfterWrappingLongLines()
	{
		var handler = CreatePlayerOutputHandler(pageLength: 10, lineLength: 20);
		var text = string.Join(" ", Enumerable.Repeat("word", 100));

		handler.Send(text);

		StringAssert.Contains(handler.BufferedOutput, "Type more");
	}

	[TestMethod]
	public void PlayerOutputHandler_MoreDoesNotPromptWhenFinalPageExactlyFits()
	{
		var handler = CreatePlayerOutputHandler(pageLength: 10, lineLength: 80);
		var text = string.Join("\n", Enumerable.Range(1, 20).Select(x => $"line {x}"));

		handler.Send(text);
		handler.Flush();
		handler.More();

		Assert.IsFalse(handler.BufferedOutput.Contains("Type more", StringComparison.Ordinal));
	}

	private static async Task PumpIncomingUntilClosing(PlayerConnection connection)
	{
		for (var i = 0; i < 10 && connection.State != ConnectionState.Closing; i++)
		{
			await Task.Delay(25);
			connection.PrepareIncoming();
		}
	}

	private static PlayerOutputHandler CreatePlayerOutputHandler(int pageLength, int lineLength)
	{
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.PageLength).Returns(pageLength);
		account.SetupGet(x => x.LineFormatLength).Returns(lineLength);
		account.SetupGet(x => x.AppendNewlinesBetweenMultipleEchoesPerPrompt).Returns(false);

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Account).Returns(account.Object);

		return new PlayerOutputHandler(new StringBuilder(), character.Object);
	}

	private static async Task<ConnectionFixture> CreateConnectionFixture(int timeout = 60000)
	{
		var listener = new TcpListener(IPAddress.Loopback, 0);
		listener.Start();
		var endpoint = (IPEndPoint)listener.LocalEndpoint;
		var client = new TcpClient();
		var connectTask = client.ConnectAsync(endpoint.Address, endpoint.Port);
		var serverClient = await listener.AcceptTcpClientAsync();
		await connectTask;

		var commands = new List<string>();
		var account = new Mock<IAccount>();
		account.SetupProperty(x => x.UseUnicode, false);
		account.SetupGet(x => x.LineFormatLength).Returns(80);
		account.SetupGet(x => x.PageLength).Returns(50);

		var context = new Mock<IFuturemudControlContext>();
		context.SetupGet(x => x.Account).Returns(account.Object);
		context.SetupGet(x => x.Timeout).Returns(timeout);
		context.SetupGet(x => x.OutputHandler).Returns(new NonPlayerOutputHandler());
		context.SetupGet(x => x.Closing).Returns(false);
		context.Setup(x => x.HandleCommand(It.IsAny<string>()))
		       .Callback<string>(commands.Add);

		var connection = new PlayerConnection(serverClient);
		connection.Bind(context.Object);
		await ReadAvailableBytes(client);

		return new ConnectionFixture(listener, client, connection, commands);
	}

	private static void WriteClientBytes(TcpClient client, byte[] bytes)
	{
		client.GetStream().Write(bytes, 0, bytes.Length);
	}

	private static async Task<byte[]> ReadAvailableBytes(TcpClient client, int minimumBytes = 0)
	{
		var bytes = new List<byte>();
		var buffer = new byte[1024];
		var stopwatch = Stopwatch.StartNew();
		while (stopwatch.ElapsedMilliseconds < 1000)
		{
			while (client.Available > 0)
			{
				var count = client.GetStream().Read(buffer, 0, Math.Min(buffer.Length, client.Available));
				bytes.AddRange(buffer.Take(count));
			}

			if (minimumBytes == 0 || bytes.Count >= minimumBytes)
			{
				break;
			}

			await Task.Delay(10);
		}

		return bytes.ToArray();
	}

	private sealed record ConnectionFixture(
		TcpListener Listener,
		TcpClient Client,
		PlayerConnection Connection,
		List<string> Commands) : IDisposable
	{
		public void Dispose()
		{
			Connection.Dispose();
			Client.Dispose();
			Listener.Stop();
		}
	}
}
