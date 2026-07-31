using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace MudWebSocketProxy.Handlers;

public class WebSocketHandler
{
	private const string ClientConfigurationErrorMessage =
		"Proxy configuration error. Check the proxy logs for details.";

	private const string ClientConnectionErrorMessage =
		"Proxy could not connect to the MUD server. Check the proxy logs for details.";

	private readonly ILogger<WebSocketHandler> _logger;
	private readonly IConfiguration _configuration;

	public WebSocketHandler(ILogger<WebSocketHandler> logger, IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;
	}

	public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
	{
		var mudServerAddress = _configuration["MudServer:Address"];
		var mudServerPortText = _configuration["MudServer:Port"];

		if (string.IsNullOrWhiteSpace(mudServerAddress) ||
		    !int.TryParse(mudServerPortText, out var mudServerPort) ||
		    mudServerPort is < 1 or > 65535)
		{
			_logger.LogError(
				"Proxy configuration error: MudServer:Address and MudServer:Port must be configured before the proxy can connect to the MUD server.");
			await SendErrorAndCloseAsync(webSocket, ClientConfigurationErrorMessage);
			return;
		}

		try
		{
			using var tcpClient = new TcpClient();
			await tcpClient.ConnectAsync(mudServerAddress, mudServerPort);
			_logger.LogInformation("MUD connection established to {Address}:{Port}", mudServerAddress, mudServerPort);
			using var networkStream = tcpClient.GetStream();

			var receiveFromWebSocketTask = ReceiveFromWebSocketAsync(webSocket, networkStream);
			var sendToWebSocketTask = SendToWebSocketAsync(webSocket, networkStream);

			await Task.WhenAny(receiveFromWebSocketTask, sendToWebSocketTask);

			if (webSocket.State == WebSocketState.Open)
			{
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
			}
			_logger.LogInformation("MUD Connection Closed");
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
		if (webSocket.State == WebSocketState.Open)
		{
			var bytes = Encoding.UTF8.GetBytes(message);
			await webSocket.SendAsync(
				new ArraySegment<byte>(bytes),
				WebSocketMessageType.Text,
				true,
				CancellationToken.None);

			await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Proxy connection error", CancellationToken.None);
		}
	}

	private async Task ReceiveFromWebSocketAsync(WebSocket webSocket, NetworkStream networkStream)
	{
		try
		{
			var buffer = new byte[40960];

			while (webSocket.State == WebSocketState.Open)
			{
				var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

				if (result.MessageType == WebSocketMessageType.Close)
				{
					break;
				}

				if (result.MessageType == WebSocketMessageType.Binary)
				{
					await networkStream.WriteAsync(buffer, 0, result.Count);
					await networkStream.FlushAsync();
				}
				else if (result.MessageType == WebSocketMessageType.Text)
				{
					// Optionally handle text messages if needed
					_logger.LogWarning("Received Text message from client; expected Binary.");
				}
				else
				{
					_logger.LogWarning($"Received unsupported message type: {result.MessageType}");
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred in ReceiveFromWebSocketAsync");
		}
	}

	private async Task SendToWebSocketAsync(WebSocket webSocket, NetworkStream networkStream)
	{
		try
		{
			var buffer = new byte[40960];

			while (webSocket.State == WebSocketState.Open)
			{
				int bytesRead = 0;
				try
				{
					bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error reading from network stream.");
					break;
				}

				if (bytesRead == 0)
				{
					// The MUD server closed the connection
					_logger.LogInformation("MUD server closed the connection.");
					break;
				}

				try
				{
					// Send the bytes directly to the WebSocket client as a binary message
					await webSocket.SendAsync(
						new ArraySegment<byte>(buffer, 0, bytesRead),
						WebSocketMessageType.Binary,
						true,
						CancellationToken.None
					);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error sending message to WebSocket client.");
					break;
				}
			}

			// Close the WebSocket connection if it's still open
			if (webSocket.State == WebSocketState.Open)
			{
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred in SendToWebSocketAsync");
		}
	}
}
