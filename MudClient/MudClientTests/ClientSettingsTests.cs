using MudClientBlazor.Services;

namespace MudClientTests;

public class ClientSettingsTests
{
	[Fact]
	public void Normalize_ClampsDisplayValuesAndRejectsUnsafeDelimiter()
	{
		var normalized = ClientSettings.Normalize(new ClientSettings
		{
			FontSizePx = 100,
			AppWidthPx = 9000,
			MessageWrapWidthPx = -5,
			MessageLeftOffsetPx = 900,
			MessageLineHeight = 0.1,
			MessageSpacingPx = 900,
			OutputWidthBehavior = (OutputWidthBehavior)999,
			StackedCommandDelayMs = 1,
			CommandStackingDelimiter = "\\"
		});

		Assert.Equal(32, normalized.FontSizePx);
		Assert.Equal(5000, normalized.AppWidthPx);
		Assert.Equal(0, normalized.MessageWrapWidthPx);
		Assert.Equal(500, normalized.MessageLeftOffsetPx);
		Assert.Equal(1.0, normalized.MessageLineHeight);
		Assert.Equal(100, normalized.MessageSpacingPx);
		Assert.Equal(OutputWidthBehavior.Wrap, normalized.OutputWidthBehavior);
		Assert.Equal(50, normalized.StackedCommandDelayMs);
		Assert.Equal(";", normalized.CommandStackingDelimiter);
	}

	[Fact]
	public void Validator_ReportsInvalidEnabledDelimiter()
	{
		var settings = ClientSettings.CreateDefault();
		settings.CommandStackingDelimiter = "  ";

		var error = Assert.Single(ClientSettingsValidator.Validate(settings));

		Assert.Contains("delimiter", error, StringComparison.OrdinalIgnoreCase);
	}
}
