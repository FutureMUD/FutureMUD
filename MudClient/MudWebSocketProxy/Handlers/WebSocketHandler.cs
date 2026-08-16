using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using MudWebSocketProxy.Security;

namespace MudWebSocketProxy.Handlers;

public sealed class WebSocketHandler
{
	private const string ClientConfigurationErrorMessage =
		"Proxy configuration error. Check the proxy logs for details.";

	private const string ClientConnectionErrorMessage =
		"Proxy could not connect to the MUD server. Check the proxy logs for details.";

	private readonly ILogger<WebSocketHandler> _logger;
	private readonly IConfiguration _configuration;
	private readonly ProxyLimits _limits;

	public WebSocketHandler(
		ILogger<WebSocketHandler> logger,
		IConfiguration configuration,
		ProxyLimits limits)
	{
		_logger = logger;
		_configuration = configuration;
		_limits = limits;
	}

	public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
	{
		var mudServerAddress = _configuration["MudServer:Address"];
		var mudServerPortText = _configuration["MudServer:Port"];

		if (string.IsNullOrWhiteSpace(mudServerAddress) ||
		    !int.TryParse(mudServerPortText, out var mudServerPort) ||
		    mudServerPort is < 1 or > 65_535)
		{
			_logger.LogError(
				"Proxy configuration error: MudServer:Address and MudServer:Port must be configured before the proxy can connect to the MUD server.");
			await SendErrorAndCloseAsync(webSocket, ClientConfigurationErrorMessage);
			return;
		}

		using var connectionCancellation = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
		try
		{
			using var tcpClient = new TcpClient();
			using (var connectTimeout = CancellationTokenSource.CreateLinkedTokenSource(connectionCancellation.Token))
			{
				connectTimeout.CancelAfter(_limits.MudConnectionTimeout);
				await tcpClient.ConnectAsync(mudServerAddress, mudServerPort, connectTimeout.Token);
			}

			tcpClient.NoDelay = true;
			_logger.LogInformation("MUD connection established to {Address}:{Port}", mudServerAddress, mudServerPort);
			await using var networkStream = tcpClient.GetStream();
			if (_configuration.GetValue("MudServer:SendProxyProtocol", true))
			{
				var clientAddress = context.Connection.RemoteIpAddress;
				if (clientAddress == null)
				{
					_logger.LogWarning("Rejected WebSocket connection because its client address is unavailable.");
					await CloseIfOpenAsync(
						webSocket,
						WebSocketCloseStatus.PolicyViolation,
						"Client address is unavailable");
					return;
				}

				var proxyHeader = ProxyProtocolV1Header.Build(
					clientAddress,
					context.Connection.RemotePort,
					mudServerPort);
				await networkStream.WriteAsync(proxyHeader, connectionCancellation.Token);
			}

			var receiveFromWebSocketTask = ReceiveFromWebSocketAsync(webSocket, networkStream, connectionCancellation.Token);
			var sendToWebSocketTask = SendToWebSocketAsync(webSocket, networkStream, connectionCancellation.Token);

			await Task.WhenAny(receiveFromWebSocketTask, sendToWebSocketTask);
			await connectionCancellation.CancelAsync();

			try
			{
				await Task.WhenAll(receiveFromWebSocketTask, sendToWebSocketTask);
			}
			catch (OperationCanceledException) when (connectionCancellation.IsCancellationRequested)
			{
			}
			catch (Exception ex) when (
				connectionCancellation.IsCancellationRequested &&
				ex is IOException or WebSocketException or ObjectDisposedException)
			{
				_logger.LogDebug("Proxy transport closed while its paired relay task was stopping.");
			}

			await CloseIfOpenAsync(webSocket, WebSocketCloseStatus.NormalClosure, "Connection closed");
			_logger.LogInformation("MUD connection closed");
		}
		catch (OperationCanceledException) when (!context.RequestAborted.IsCancellationRequested)
		{
			_logger.LogWarning(
				"Timed out connecting to MUD server {Address}:{Port} after {Timeout}.",
				mudServerAddress,
				mudServerPort,
				_limits.MudConnectionTimeout);
			await SendErrorAndCloseAsync(webSocket, ClientConnectionErrorMessage);
		}
		catch (OperationCanceledException)
		{
			_logger.LogDebug("WebSocket proxy connection was cancelled.");
		}
		catch (SocketException ex)
		{
			_logger.LogError(
				ex,
				"Proxy could not connect to MUD server {Address}:{Port}. {SocketErrorCode}: {Message}",
				mudServerAddress,
				mudServerPort,
				ex.SocketErrorCode,
				ex.Message);
			await SendErrorAndCloseAsync(webSocket, ClientConnectionErrorMessage);
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Proxy error while connected to MUD server {Address}:{Port}: {Message}",
				mudServerAddress,
				mudServerPort,
				ex.Message);
			await SendErrorAndCloseAsync(webSocket, ClientConnectionErrorMessage);
		}
	}

	private static async Task SendErrorAndCloseAsync(WebSocket webSocket, string message)
	{
		if (webSocket.State != WebSocketState.Open)
		{
			return;
		}

		var bytes = Encoding.UTF8.GetBytes(message);
		await webSocket.SendAsync(
			new ArraySegment<byte>(bytes),
			WebSocketMessageType.Text,
			true,
			CancellationToken.None);

		await webSocket.CloseOutputAsync(
			WebSocketCloseStatus.InternalServerError,
			"Proxy connection error",
			CancellationToken.None);
	}

	private async Task ReceiveFromWebSocketAsync(
		WebSocket webSocket,
		NetworkStream networkStream,
		CancellationToken cancellationToken)
	{
		var buffer = new byte[16_384];
		var trafficLimiter = new ClientTrafficLimiter(_limits, DateTimeOffset.UtcNow);

		while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
		{
			using var message = new MemoryStream();
			WebSocketReceiveResult result;
			do
			{
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
				if (result.MessageType == WebSocketMessageType.Close)
				{
					return;
				}

				if (result.MessageType != WebSocketMessageType.Binary)
				{
					await CloseIfOpenAsync(webSocket, WebSocketCloseStatus.InvalidMessageType, "Binary messages are required");
					return;
				}

				if (message.Length + result.Count > _limits.MaximumClientMessageBytes)
				{
					await CloseIfOpenAsync(webSocket, WebSocketCloseStatus.MessageTooBig, "Client message is too large");
					return;
				}

				message.Write(buffer, 0, result.Count);
			}
			while (!result.EndOfMessage);

			var messageLength = checked((int)message.Length);
			if (!trafficLimiter.TryConsumeMessage(messageLength, DateTimeOffset.UtcNow))
			{
				await CloseIfOpenAsync(webSocket, WebSocketCloseStatus.PolicyViolation, "Client send rate exceeded");
				return;
			}

			if (message.TryGetBuffer(out var segment))
			{
				await networkStream.WriteAsync(segment.AsMemory(0, messageLength), cancellationToken);
			}
		}
	}

	private async Task SendToWebSocketAsync(
		WebSocket webSocket,
		NetworkStream networkStream,
		CancellationToken cancellationToken)
	{
		var buffer = new byte[40_960];
		var trafficLimiter = new ByteTrafficLimiter(_limits.MaximumMudBytesPerSecond, DateTimeOffset.UtcNow);

		while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
		{
			var bytesRead = await networkStream.ReadAsync(buffer, cancellationToken);
			if (bytesRead == 0)
			{
				return;
			}

			if (!trafficLimiter.TryConsume(bytesRead, DateTimeOffset.UtcNow))
			{
				await CloseIfOpenAsync(webSocket, WebSocketCloseStatus.PolicyViolation, "MUD output rate exceeded");
				return;
			}

			await webSocket.SendAsync(
				new ArraySegment<byte>(buffer, 0, bytesRead),
				WebSocketMessageType.Binary,
				true,
				cancellationToken);
		}
	}

	private static async Task CloseIfOpenAsync(
		WebSocket webSocket,
		WebSocketCloseStatus status,
		string description)
	{
		if (webSocket.State == WebSocketState.Open)
		{
			await webSocket.CloseOutputAsync(status, description, CancellationToken.None);
		}
	}
}
