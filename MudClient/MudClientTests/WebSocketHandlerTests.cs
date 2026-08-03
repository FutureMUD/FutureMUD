using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudWebSocketProxy.Handlers;
using MudWebSocketProxy.Security;

namespace MudClientTests;

public class WebSocketHandlerTests
{
	[Fact]
	public async Task HandleWebSocketAsync_WithInvalidProxyConfig_SendsGenericClientError()
	{
		var handler = BuildHandler(new Dictionary<string, string?>());
		var webSocket = new CapturingWebSocket();

		await handler.HandleWebSocketAsync(new DefaultHttpContext(), webSocket);

		var message = Assert.Single(webSocket.TextMessages);
		Assert.Equal("Proxy configuration error. Check the proxy logs for details.", message);
		Assert.DoesNotContain("MudServer", message);
	}

	[Fact]
	public async Task HandleWebSocketAsync_WhenMudConnectionFails_SendsGenericClientError()
	{
		var unusedPort = GetUnusedTcpPort();
		var handler = BuildHandler(new Dictionary<string, string?>
		{
			["MudServer:Address"] = "127.0.0.1",
			["MudServer:Port"] = unusedPort.ToString()
		});
		var webSocket = new CapturingWebSocket();

		await handler.HandleWebSocketAsync(new DefaultHttpContext(), webSocket);

		var message = Assert.Single(webSocket.TextMessages);
		Assert.Equal("Proxy could not connect to the MUD server. Check the proxy logs for details.", message);
		Assert.DoesNotContain("127.0.0.1", message);
		Assert.DoesNotContain(unusedPort.ToString(), message);
	}

	private static WebSocketHandler BuildHandler(Dictionary<string, string?> values)
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(values)
			.Build();

		return new WebSocketHandler(
			new TestLogger<WebSocketHandler>(),
			configuration,
			ProxyLimits.FromConfiguration(configuration));
	}

	private static int GetUnusedTcpPort()
	{
		using var listener = new TcpListener(IPAddress.Loopback, 0);
		listener.Start();
		return ((IPEndPoint)listener.LocalEndpoint).Port;
	}

	private sealed class CapturingWebSocket : WebSocket
	{
		private readonly List<string> _textMessages = new();
		private WebSocketState _state = WebSocketState.Open;

		public IReadOnlyList<string> TextMessages => _textMessages;
		public override WebSocketCloseStatus? CloseStatus { get; }
		public override string? CloseStatusDescription { get; }
		public override WebSocketState State => _state;
		public override string? SubProtocol => null;

		public override void Abort()
		{
			_state = WebSocketState.Aborted;
		}

		public override Task CloseAsync(
			WebSocketCloseStatus closeStatus,
			string? statusDescription,
			CancellationToken cancellationToken)
		{
			_state = WebSocketState.Closed;
			return Task.CompletedTask;
		}

		public override Task CloseOutputAsync(
			WebSocketCloseStatus closeStatus,
			string? statusDescription,
			CancellationToken cancellationToken)
		{
			_state = WebSocketState.CloseSent;
			return Task.CompletedTask;
		}

		public override void Dispose()
		{
			_state = WebSocketState.Closed;
		}

		public override Task<WebSocketReceiveResult> ReceiveAsync(
			ArraySegment<byte> buffer,
			CancellationToken cancellationToken)
		{
			throw new NotSupportedException();
		}

		public override Task SendAsync(
			ArraySegment<byte> buffer,
			WebSocketMessageType messageType,
			bool endOfMessage,
			CancellationToken cancellationToken)
		{
			if (messageType == WebSocketMessageType.Text)
			{
				_textMessages.Add(Encoding.UTF8.GetString(buffer.ToArray()));
			}

			return Task.CompletedTask;
		}
	}

	private sealed class TestLogger<T> : ILogger<T>
	{
		public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		{
			return null;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return false;
		}

		public void Log<TState>(
			LogLevel logLevel,
			EventId eventId,
			TState state,
			Exception? exception,
			Func<TState, Exception?, string> formatter)
		{
		}
	}
}
