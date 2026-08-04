namespace MudClientBlazor.Services;

public enum OutputWidthBehavior
{
	Wrap,
	HorizontalScroll
}

public sealed class ClientSettings
{
	public const int CurrentVersion = 2;
	public const string DefaultFontFamily = "Consolas, \"Cascadia Mono\", \"Courier New\", monospace";

	private static readonly IReadOnlyList<ClientFontOption> AvailableFonts =
	[
		new("Consolas", DefaultFontFamily),
		new("Cascadia Mono", "\"Cascadia Mono\", Consolas, \"Courier New\", monospace"),
		new("Courier New", "\"Courier New\", Courier, monospace"),
		new("Segoe UI", "\"Segoe UI\", Tahoma, Geneva, Verdana, sans-serif"),
		new("System Monospace", "ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, \"Liberation Mono\", \"Courier New\", monospace")
	];

	public int Version { get; set; } = CurrentVersion;
	public string FontFamily { get; set; } = DefaultFontFamily;
	public int FontSizePx { get; set; } = 15;
	public int AppWidthPx { get; set; } = 1500;
	public int MessageWrapWidthPx { get; set; }
	public int MessageLeftOffsetPx { get; set; }
	public double MessageLineHeight { get; set; } = 1.48;
	public int MessageSpacingPx { get; set; } = 6;
	public OutputWidthBehavior OutputWidthBehavior { get; set; } = OutputWidthBehavior.Wrap;
	public bool ClearInputAfterSend { get; set; } = true;
	public bool SemicolonCommandStackingEnabled { get; set; } = true;
	public string CommandStackingDelimiter { get; set; } = ";";
	public bool NewlineCommandStackingEnabled { get; set; } = true;
	public int StackedCommandDelayMs { get; set; } = 100;

	public static IReadOnlyList<ClientFontOption> FontOptions => AvailableFonts;

	public ClientSettings Clone()
	{
		return new ClientSettings
		{
			Version = Version,
			FontFamily = FontFamily,
			FontSizePx = FontSizePx,
			AppWidthPx = AppWidthPx,
			MessageWrapWidthPx = MessageWrapWidthPx,
			MessageLeftOffsetPx = MessageLeftOffsetPx,
			MessageLineHeight = MessageLineHeight,
			MessageSpacingPx = MessageSpacingPx,
			OutputWidthBehavior = OutputWidthBehavior,
			ClearInputAfterSend = ClearInputAfterSend,
			SemicolonCommandStackingEnabled = SemicolonCommandStackingEnabled,
			CommandStackingDelimiter = CommandStackingDelimiter,
			NewlineCommandStackingEnabled = NewlineCommandStackingEnabled,
			StackedCommandDelayMs = StackedCommandDelayMs
		};
	}

	public static ClientSettings CreateDefault() => new();

	public static ClientSettings Normalize(ClientSettings? settings)
	{
		var defaults = CreateDefault();
		if (settings == null)
		{
			return defaults;
		}

		var normalized = settings.Clone();
		normalized.Version = CurrentVersion;
		normalized.FontFamily = AvailableFonts.Any(font => string.Equals(font.CssValue, settings.FontFamily, StringComparison.Ordinal))
			? settings.FontFamily
			: defaults.FontFamily;
		normalized.FontSizePx = Math.Clamp(settings.FontSizePx, 8, 32);
		normalized.AppWidthPx = Math.Clamp(settings.AppWidthPx, 1000, 5000);
		normalized.MessageWrapWidthPx = Math.Clamp(settings.MessageWrapWidthPx, 0, 3000);
		normalized.MessageLeftOffsetPx = Math.Clamp(settings.MessageLeftOffsetPx, 0, 500);
		normalized.MessageLineHeight = Math.Clamp(settings.MessageLineHeight, 1.0, 3.0);
		normalized.MessageSpacingPx = Math.Clamp(settings.MessageSpacingPx, 0, 100);
		normalized.OutputWidthBehavior = Enum.IsDefined(settings.OutputWidthBehavior)
			? settings.OutputWidthBehavior
			: defaults.OutputWidthBehavior;
		normalized.CommandStackingDelimiter = NormalizeDelimiter(settings.CommandStackingDelimiter, defaults.CommandStackingDelimiter);
		normalized.StackedCommandDelayMs = Math.Clamp(settings.StackedCommandDelayMs, 50, 2_000);
		return normalized;
	}

	public static bool IsValidDelimiter(string? delimiter)
	{
		return delimiter?.Length == 1 && !char.IsWhiteSpace(delimiter[0]) && delimiter[0] != '\\' && delimiter[0] != '\r' && delimiter[0] != '\n';
	}

	private static string NormalizeDelimiter(string? delimiter, string fallback)
	{
		return IsValidDelimiter(delimiter) ? delimiter! : fallback;
	}
}

public sealed record ClientFontOption(string Label, string CssValue);

public static class ClientSettingsValidator
{
	public static IReadOnlyList<string> Validate(ClientSettings settings)
	{
		ArgumentNullException.ThrowIfNull(settings);

		var errors = new List<string>();
		if (settings.SemicolonCommandStackingEnabled && !ClientSettings.IsValidDelimiter(settings.CommandStackingDelimiter))
		{
			errors.Add("Command stacking needs one non-whitespace character other than backslash as its delimiter.");
		}

		return errors;
	}
}
