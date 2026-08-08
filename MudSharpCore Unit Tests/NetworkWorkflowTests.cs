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
using System.Threading;
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

			var eorPrompt = new[] { Telnet.IAC, Telnet.EOR };
			var bytes = await ReadUntilSuffix(fixture.Client, eorPrompt);
			Assert.IsTrue(bytes.Length >= 2, "Expected output bytes from server.");
			CollectionAssert.AreEqual(eorPrompt, bytes[^2..]);
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
	public async Task TcpServer_AdmitsAcceptedConnectionOnProcessingThread()
	{
		var connections = new List<IPlayerConnection>();
		var context = CreateControlContext(new List<string>());
		var callbackThread = 0;
		var server = new TCPServer(IPAddress.Loopback, 0);
		server.Bind(connections, connection =>
		{
			callbackThread = Environment.CurrentManagedThreadId;
			connection.Bind(context.Object);
			connections.Add(connection);
		});

		await server.StartAsync();
		using var client = new TcpClient();
		await client.ConnectAsync(IPAddress.Loopback, server.BoundPort);
		await WaitUntil(() => server.GetNetworkPerformanceSnapshot().AcceptedConnections == 1);

		var processingThread = Environment.CurrentManagedThreadId;
		server.ProcessPendingConnections();

		Assert.AreEqual(processingThread, callbackThread);
		Assert.AreEqual(1, connections.Count);
		await server.StopAsync();
		connections[0].Dispose();
	}

	[TestMethod]
	public async Task TcpServer_DuplicateStartThrowsAndRepeatedStopIsSafe()
	{
		var server = new TCPServer(IPAddress.Loopback, 0);
		server.Bind(new List<IPlayerConnection>(), _ => { });
		await server.StartAsync();

		await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await server.StartAsync());
		await server.StopAsync();
		await server.StopAsync();

		Assert.IsFalse(server.IsListeningAndResponding);
	}

	[TestMethod]
	public async Task PlayerConnection_InputBackpressurePreservesEveryCommandInOrder()
	{
		var fixture = await CreateConnectionFixture();
		try
		{
			var expected = Enumerable.Range(0, 24).Select(x => $"command{x}").ToArray();
			var payload = Encoding.ASCII.GetBytes(string.Join("\r", expected) + "\r");
			await fixture.Client.GetStream().WriteAsync(payload);

			for (var i = 0; i < expected.Length; i++)
			{
				await WaitUntil(() => fixture.Connection.HasIncomingCommands);
				fixture.Connection.AttemptCommand();
			}

			CollectionAssert.AreEqual(expected, fixture.Commands);
		}
		finally
		{
			fixture.Dispose();
		}
	}

	[TestMethod]
	public async Task PlayerConnection_CharsetAccountMutationWaitsForGameThreadProcessing()
	{
		var fixture = await CreateConnectionFixture();
		try
		{
			var acknowledgement = new byte[] { Telnet.IAC, Telnet.SB, Telnet.CHARSET, Telnet.ACCEPTED }
				.Concat(Encoding.ASCII.GetBytes("UTF-8"))
				.Concat(new byte[] { Telnet.IAC, Telnet.SE })
				.ToArray();
			await fixture.Client.GetStream().WriteAsync(acknowledgement);
			await Task.Delay(50);

			Assert.IsFalse(fixture.Account.Object.UseUnicode);
			fixture.Connection.ProcessPendingTransportEvents();
			Assert.IsTrue(fixture.Account.Object.UseUnicode);
		}
		finally
		{
			fixture.Dispose();
		}
	}

	[TestMethod]
	public async Task PlayerConnection_PartialWritesDrainInOrderBeforeGracefulClose()
	{
		var transport = new TestConnectionTransport(maximumWriteSize: 2);
		var telemetry = new TestNetworkTelemetry();
		var connection = new PlayerConnection(transport, TimeProvider.System, telemetry);
		var context = CreateControlContext(new List<string>());
		connection.Bind(context.Object);
		connection.StartTransport();
		connection.AddOutgoing("hello");
		connection.SendOutgoing();
		Assert.IsFalse(connection.HasOutgoingCommands, "Committed frames must not be re-enqueued every game tick.");
		connection.RequestClose(ConnectionCloseMode.Drain);

		await connection.TransportCompletion.WaitAsync(TimeSpan.FromSeconds(2));

		var output = transport.Output;
		CollectionAssert.AreEqual(new byte[] { Telnet.IAC, Telnet.WILL, Telnet.TELOPT_MXP }, output[..3]);
		StringAssert.Contains(Encoding.ASCII.GetString(output), "hello");
		CollectionAssert.AreEqual(new byte[] { Telnet.IAC, Telnet.GA }, output[^2..]);
		Assert.IsTrue(connection.IsReadyForDisposal);
		Assert.IsTrue(telemetry.WriteOperations > 1);
		connection.Dispose();
	}

	[TestMethod]
	public async Task PlayerConnection_StagedOutputLimitDisconnectsSlowClient()
	{
		var transport = new TestConnectionTransport();
		var telemetry = new TestNetworkTelemetry();
		var connection = new PlayerConnection(transport, TimeProvider.System, telemetry);
		connection.Bind(CreateControlContext(new List<string>()).Object);
		connection.StartTransport();

		connection.AddOutgoing(new string('x', 1_048_577));

		Assert.AreEqual(ConnectionState.Closing, connection.State);
		Assert.AreEqual(1, telemetry.SlowClientDisconnects);
		await connection.TransportCompletion.WaitAsync(TimeSpan.FromSeconds(2));
		connection.Dispose();
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
		connection.StartTransport();
		await ReadAvailableBytes(client);

		return new ConnectionFixture(listener, client, connection, commands, account);
	}

	private static Mock<IFuturemudControlContext> CreateControlContext(List<string> commands, int timeout = 60000)
	{
		var account = new Mock<IAccount>();
		account.SetupProperty(x => x.UseUnicode, false);
		account.SetupGet(x => x.LineFormatLength).Returns(80);
		account.SetupGet(x => x.PageLength).Returns(50);

		var context = new Mock<IFuturemudControlContext>();
		context.SetupGet(x => x.Account).Returns(account.Object);
		context.SetupGet(x => x.Timeout).Returns(timeout);
		context.SetupGet(x => x.OutputHandler).Returns(new NonPlayerOutputHandler());
		context.SetupGet(x => x.Closing).Returns(false);
		context.Setup(x => x.HandleCommand(It.IsAny<string>())).Callback<string>(commands.Add);
		return context;
	}

	private static async Task WaitUntil(Func<bool> predicate)
	{
		var stopwatch = Stopwatch.StartNew();
		while (!predicate() && stopwatch.Elapsed < TimeSpan.FromSeconds(2))
		{
			await Task.Delay(10);
		}

		Assert.IsTrue(predicate(), "Timed out waiting for the asynchronous network condition.");
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

	private static async Task<byte[]> ReadUntilSuffix(TcpClient client, byte[] suffix)
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

			if (bytes.Count >= suffix.Length && bytes.Skip(bytes.Count - suffix.Length).SequenceEqual(suffix))
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
		List<string> Commands,
		Mock<IAccount> Account) : IDisposable
	{
		public void Dispose()
		{
			Connection.Dispose();
			Client.Dispose();
			Listener.Stop();
		}
	}

	private sealed class TestConnectionTransport(int maximumWriteSize = int.MaxValue) : IConnectionTransport
	{
		private readonly List<byte> _output = [];

		public string IP => IPAddress.Loopback.ToString();
		public EndPoint RemoteEndPoint => new IPEndPoint(IPAddress.Loopback, 4000);
		public byte[] Output
		{
			get
			{
				lock (_output)
				{
					return _output.ToArray();
				}
			}
		}

		public async ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
		{
			await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
			return 0;
		}

		public ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
		{
			var count = Math.Min(maximumWriteSize, buffer.Length);
			lock (_output)
			{
				_output.AddRange(buffer.Span[..count].ToArray());
			}

			return ValueTask.FromResult(count);
		}

		public void Close() { }
		public void Dispose() { }
	}

	private sealed class TestNetworkTelemetry : INetworkTelemetrySink
	{
		public int WriteOperations { get; private set; }
		public int SlowClientDisconnects { get; private set; }
		public void RecordRead(int bytes) { }
		public void RecordWrite(int bytes) => WriteOperations++;
		public void RecordInputQueueDepth(int depth) { }
		public void RecordOutputQueueBytes(long bytes) { }
		public void RecordSlowClientDisconnect() => SlowClientDisconnects++;
		public void RecordReadError() { }
		public void RecordWriteError() { }
	}
}
