namespace MudWebSocketProxy.Security;

public static class WebSocketOriginPolicy
{
	public static bool IsAllowed(string? origin, bool requireOrigin, IEnumerable<string?> allowedOrigins)
	{
		ArgumentNullException.ThrowIfNull(allowedOrigins);

		if (string.IsNullOrWhiteSpace(origin))
		{
			return !requireOrigin;
		}

		var normalizedOrigin = Normalize(origin);
		return allowedOrigins
			.Where(allowedOrigin => !string.IsNullOrWhiteSpace(allowedOrigin))
			.Select(allowedOrigin => Normalize(allowedOrigin!))
			.Any(allowedOrigin => string.Equals(allowedOrigin, normalizedOrigin, StringComparison.OrdinalIgnoreCase));
	}

	private static string Normalize(string origin)
	{
		if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
		{
			return uri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
		}

		return origin.Trim().TrimEnd('/');
	}
}
