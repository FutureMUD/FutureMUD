using Microsoft.Extensions.Configuration;

namespace MudClientBlazor;

public static class WebSocketEndpoint
{
	public static Uri FromConfiguration(IConfiguration configuration, Uri? appBaseUri = null)
	{
		var endpoint = configuration["WebSocketServer:Endpoint"];
		if (!string.IsNullOrWhiteSpace(endpoint))
		{
			return CreateWebSocketUri(endpoint, "WebSocketServer:Endpoint", appBaseUri);
		}

		var address = configuration["WebSocketServer:ClientAddress"];
		if (string.IsNullOrWhiteSpace(address))
		{
			throw new InvalidOperationException("WebSocketServer:ClientAddress is not configured.");
		}

		var uriBuilder = new UriBuilder(CreateWebSocketUri(address, "WebSocketServer:ClientAddress", null));

		var port = configuration["WebSocketServer:ClientPort"];
		if (!string.IsNullOrWhiteSpace(port))
		{
			if (!int.TryParse(port, out var parsedPort) || parsedPort is < 1 or > 65535)
			{
				throw new InvalidOperationException("WebSocketServer:ClientPort must be a valid TCP port.");
			}

			uriBuilder.Port = parsedPort;
		}

		uriBuilder.Path = NormalizePath(configuration["WebSocketServer:Path"] ?? "/ws");
		return uriBuilder.Uri;
	}

	private static Uri CreateWebSocketUri(string value, string settingName, Uri? appBaseUri)
	{
		value = value.Trim();
		if (appBaseUri is not null &&
		    TryResolveRelativeEndpoint(value, appBaseUri, out var resolvedRelativeUri))
		{
			return ConvertHttpUriToWebSocketUri(resolvedRelativeUri);
		}

		if (Uri.TryCreate(value, UriKind.Absolute, out var absoluteUri))
		{
			return ValidateWebSocketUri(absoluteUri, settingName);
		}

		var expected = appBaseUri is null
			? "an absolute ws:// or wss:// URI"
			: "an absolute ws:// or wss:// URI, or a relative URI that can be resolved from the application address";
		throw new InvalidOperationException($"{settingName} must be {expected}.");
	}

	private static bool TryResolveRelativeEndpoint(string value, Uri appBaseUri, out Uri resolvedUri)
	{
		if (value.StartsWith('/') && !value.StartsWith("//", StringComparison.Ordinal))
		{
			var appOrigin = new Uri(appBaseUri.GetLeftPart(UriPartial.Authority));
			if (Uri.TryCreate(appOrigin, value, out var rootRelativeUri))
			{
				resolvedUri = rootRelativeUri;
				return true;
			}

			resolvedUri = null!;
			return false;
		}

		if (Uri.TryCreate(value, UriKind.Relative, out var relativeUri))
		{
			resolvedUri = new Uri(appBaseUri, relativeUri);
			return true;
		}

		resolvedUri = null!;
		return false;
	}

	private static Uri ValidateWebSocketUri(Uri uri, string settingName)
	{
		if (uri.Scheme != Uri.UriSchemeWs && uri.Scheme != Uri.UriSchemeWss)
		{
			throw new InvalidOperationException($"{settingName} must be an absolute ws:// or wss:// URI.");
		}

		return uri;
	}

	private static Uri ConvertHttpUriToWebSocketUri(Uri uri)
	{
		if (uri.Scheme == Uri.UriSchemeWs || uri.Scheme == Uri.UriSchemeWss)
		{
			return uri;
		}

		if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
		{
			throw new InvalidOperationException("WebSocketServer:Endpoint could not be resolved to an http://, https://, ws://, or wss:// URI.");
		}

		var uriBuilder = new UriBuilder(uri)
		{
			Scheme = uri.Scheme == Uri.UriSchemeHttps ? Uri.UriSchemeWss : Uri.UriSchemeWs
		};
		return uriBuilder.Uri;
	}

	private static string NormalizePath(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return "/ws";
		}

		return path.StartsWith('/') ? path : $"/{path}";
	}
}
