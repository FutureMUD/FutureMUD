using Microsoft.Extensions.Configuration;

namespace MudWebSocketProxy.Security;

public static class ProxyConfigurationValidator
{
	public static void Validate(IConfiguration configuration)
	{
		var address = configuration["MudServer:Address"];
		var port = configuration.GetValue<int?>("MudServer:Port");
		if (string.IsNullOrWhiteSpace(address) || port is null or < 1 or > 65_535)
		{
			throw new InvalidOperationException("MudServer:Address and a valid MudServer:Port are required.");
		}

		if (!configuration.GetValue("WebSocketServer:RequireOrigin", true))
		{
			throw new InvalidOperationException("WebSocketServer:RequireOrigin must remain enabled in a deployed proxy.");
		}

		var origins = configuration
			.GetSection("WebSocketServer:AllowedOrigins")
			.GetChildren()
			.Select(section => section.Value)
			.Where(value => !string.IsNullOrWhiteSpace(value))
			.ToList();
		if (origins.Count == 0 || origins.Any(origin => !Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.Scheme is not "https" and not "http" || !string.IsNullOrEmpty(uri.PathAndQuery.Trim('/'))))
		{
			throw new InvalidOperationException("WebSocketServer:AllowedOrigins must contain exact HTTP(S) origins without paths.");
		}
	}
}
