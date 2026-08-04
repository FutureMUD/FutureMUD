using Microsoft.Extensions.Configuration;

namespace MudClientBlazor.Services;

public sealed class ClientBrandingOptions
{
	public const string DefaultTitle = "FutureMUD Web Client";
	public const string DefaultIconUrl = "icon-192.png";
	public const string DefaultAboutText = "A browser-based ANSI, Telnet, and MXP client for FutureMUD.";

	public string Title { get; set; } = DefaultTitle;
	public string IconUrl { get; set; } = DefaultIconUrl;
	public string AboutText { get; set; } = DefaultAboutText;

	public static ClientBrandingOptions FromConfiguration(IConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration);

		return Normalize(new ClientBrandingOptions
		{
			Title = configuration["ClientBranding:Title"] ?? DefaultTitle,
			IconUrl = configuration["ClientBranding:IconUrl"] ?? DefaultIconUrl,
			AboutText = configuration["ClientBranding:AboutText"] ?? DefaultAboutText
		});
	}

	public static ClientBrandingOptions Normalize(ClientBrandingOptions? options)
	{
		var defaults = new ClientBrandingOptions();
		if (options == null)
		{
			return defaults;
		}

		return new ClientBrandingOptions
		{
			Title = NormalizeText(options.Title, defaults.Title, 100),
			IconUrl = IsSafeRelativeAssetUrl(options.IconUrl) ? options.IconUrl.Trim() : defaults.IconUrl,
			AboutText = NormalizeText(options.AboutText, defaults.AboutText, 500)
		};
	}

	private static string NormalizeText(string? text, string fallback, int maximumLength)
	{
		var normalized = text?.Trim();
		if (string.IsNullOrWhiteSpace(normalized))
		{
			return fallback;
		}

		return normalized.Length <= maximumLength ? normalized : normalized[..maximumLength];
	}

	private static bool IsSafeRelativeAssetUrl(string? url)
	{
		if (string.IsNullOrWhiteSpace(url))
		{
			return false;
		}

		var normalized = url.Trim();
		if (normalized.StartsWith("//", StringComparison.Ordinal) ||
		    normalized.Contains('\\') ||
		    normalized.Any(char.IsControl) ||
		    !Uri.TryCreate(normalized, UriKind.Relative, out _))
		{
			return false;
		}

		var path = normalized.Split(['?', '#'], 2)[0];
		return !path.Split('/', StringSplitOptions.RemoveEmptyEntries)
			.Any(segment => segment is "." or "..");
	}
}
