using MudClientBlazor.Services;

namespace MudClientTests;

public class HotkeyBindingTests
{
	[Theory]
	[InlineData("Numpad7", "northwest")]
	[InlineData("Numpad8", "north")]
	[InlineData("Numpad9", "northeast")]
	[InlineData("Numpad5", "")]
	[InlineData("Numpad2", "south")]
	[InlineData("Numpad1", "southwest")]
	[InlineData("Numpad3", "southeast")]
	[InlineData("Numpad4", "west")]
	[InlineData("Numpad6", "east")]
	[InlineData("NumpadDivide", "enter")]
	[InlineData("NumpadEnter", "")]
	[InlineData("NumpadMultiply", "leave")]
	[InlineData("NumpadSubtract", "up")]
	[InlineData("NumpadAdd", "down")]
	[InlineData("NumpadDecimal", "score")]
	[InlineData("Numpad0", "look")]
	public void DefaultMap_ResolvesExpectedCommands(string keyCode, string expectedCommand)
	{
		var settings = HotkeySettings.CreateDefault();
		var map = HotkeyBindingMap.Create(settings.Bindings);

		var found = map.TryGetCommand(keyCode, out var command);

		if (string.IsNullOrEmpty(expectedCommand))
		{
			Assert.False(found);
			Assert.Equal(string.Empty, command);
		}
		else
		{
			Assert.True(found);
			Assert.Equal(expectedCommand, command);
		}
	}

	[Fact]
	public void Validate_AllowsDefaultNoOpNumpadButtons()
	{
		var settings = HotkeySettings.CreateDefault();

		var errors = HotkeyBindingValidator.Validate(settings.Bindings);

		Assert.Empty(errors);
	}

	[Fact]
	public void Map_IgnoresDisabledBindings()
	{
		var settings = HotkeySettings.CreateDefault();
		settings.Bindings.Single(binding => binding.Id == "north").IsEnabled = false;
		var map = HotkeyBindingMap.Create(settings.Bindings);

		var found = map.TryGetCommand("Numpad8", out _);

		Assert.False(found);
	}

	[Fact]
	public void Validate_ReportsDuplicateEnabledKeys()
	{
		var settings = HotkeySettings.CreateDefault();
		settings.Bindings.Single(binding => binding.Id == "south").KeyCode = "Numpad8";

		var errors = HotkeyBindingValidator.Validate(settings.Bindings);

		var error = Assert.Single(errors);
		Assert.Contains("Numpad 8", error);
		Assert.Contains("North", error);
		Assert.Contains("South", error);
	}

	[Fact]
	public void Normalize_PreservesSavedValuesAndAddsMissingDefaults()
	{
		var saved = new HotkeySettings
		{
			Bindings =
			[
				new HotkeyBinding
				{
					Id = "north",
					Label = "Forward",
					KeyCode = "KeyW",
					Command = "north",
					IsEnabled = true
				}
			]
		};

		var normalized = HotkeySettings.Normalize(saved);

		var north = normalized.Bindings.Single(binding => binding.Id == "north");
		Assert.Equal("Forward", north.Label);
		Assert.Equal("KeyW", north.KeyCode);
		Assert.Equal("W", north.KeyDisplay);

		Assert.Contains(normalized.Bindings, binding => binding.Id == "look" && binding.KeyCode == "Numpad0");
		Assert.Contains(normalized.Bindings, binding => binding.Id == "numpad5" && binding.KeyCode == "Numpad5" && !binding.RequiresCommand);
		Assert.Contains(normalized.Bindings, binding => binding.Id == "numpad-enter" && binding.KeyCode == "NumpadEnter" && !binding.RequiresCommand);
		Assert.Contains(normalized.Bindings, binding => binding.Id == "up" && binding.KeyCode == "NumpadSubtract");
		Assert.Contains(normalized.Bindings, binding => binding.Id == "down" && binding.KeyCode == "NumpadAdd");
		Assert.Contains(normalized.Bindings, binding => binding.Id == "score" && binding.KeyCode == "NumpadDecimal");
	}

	[Theory]
	[InlineData("NumpadAdd", "Numpad +")]
	[InlineData("NumpadDecimal", "Numpad .")]
	[InlineData("NumpadDivide", "Numpad /")]
	[InlineData("NumpadEnter", "Numpad Enter")]
	[InlineData("NumpadMultiply", "Numpad *")]
	[InlineData("NumpadSubtract", "Numpad -")]
	[InlineData("KeyQ", "Q")]
	[InlineData("Digit7", "7")]
	[InlineData("ArrowLeft", "Left")]
	public void Formatter_ProducesFriendlyLabels(string keyCode, string expected)
	{
		Assert.Equal(expected, HotkeyKeyFormatter.Format(keyCode));
	}

	[Theory]
	[InlineData("KeyW", "w", false)]
	[InlineData("Digit1", "1", false)]
	[InlineData("Space", " ", false)]
	[InlineData("Numpad1", "1", true)]
	[InlineData("NumpadAdd", "+", true)]
	[InlineData("ArrowUp", "ArrowUp", true)]
	[InlineData("F1", "F1", true)]
	public void InputPolicy_IgnoresPrintableNonNumpadKeysInEditableFields(
		string keyCode,
		string key,
		bool expected)
	{
		Assert.Equal(expected, HotkeyInputPolicy.ShouldSendHotkeyFromEditableField(keyCode, key));
	}
}
