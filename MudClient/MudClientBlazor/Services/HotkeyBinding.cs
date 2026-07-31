namespace MudClientBlazor.Services;

public sealed class HotkeyBinding
{
	public string Id { get; set; } = string.Empty;
	public string Label { get; set; } = string.Empty;
	public string KeyCode { get; set; } = string.Empty;
	public string KeyDisplay { get; set; } = string.Empty;
	public string Command { get; set; } = string.Empty;
	public bool IsEnabled { get; set; } = true;
	public bool RequiresCommand { get; set; } = true;

	public HotkeyBinding Clone()
	{
		return new HotkeyBinding
		{
			Id = Id,
			Label = Label,
			KeyCode = KeyCode,
			KeyDisplay = KeyDisplay,
			Command = Command,
			IsEnabled = IsEnabled,
			RequiresCommand = RequiresCommand
		};
	}
}

public sealed class HotkeySettings
{
	public const int CurrentVersion = 1;

	public int Version { get; set; } = CurrentVersion;
	public List<HotkeyBinding> Bindings { get; set; } = new();

	public static HotkeySettings CreateDefault()
	{
		return new HotkeySettings
		{
			Bindings = CreateDefaultBindings().Select(binding => binding.Clone()).ToList()
		};
	}

	public static IReadOnlyList<HotkeyBinding> CreateDefaultBindings()
	{
		return
		[
			CreateBinding("northwest", "Northwest", "Numpad7", "northwest"),
			CreateBinding("north", "North", "Numpad8", "north"),
			CreateBinding("northeast", "Northeast", "Numpad9", "northeast"),
			CreateBinding("west", "West", "Numpad4", "west"),
			CreateBinding("numpad5", "Numpad 5", "Numpad5", string.Empty, requiresCommand: false),
			CreateBinding("east", "East", "Numpad6", "east"),
			CreateBinding("southwest", "Southwest", "Numpad1", "southwest"),
			CreateBinding("south", "South", "Numpad2", "south"),
			CreateBinding("southeast", "Southeast", "Numpad3", "southeast"),
			CreateBinding("look", "Look", "Numpad0", "look"),
			CreateBinding("numpad-enter", "Numpad Enter", "NumpadEnter", string.Empty, requiresCommand: false),
			CreateBinding("enter", "Enter", "NumpadDivide", "enter"),
			CreateBinding("leave", "Leave", "NumpadMultiply", "leave"),
			CreateBinding("up", "Up", "NumpadSubtract", "up"),
			CreateBinding("down", "Down", "NumpadAdd", "down"),
			CreateBinding("score", "Score", "NumpadDecimal", "score")
		];
	}

	public static HotkeySettings Normalize(HotkeySettings? settings)
	{
		if (settings?.Bindings == null || settings.Bindings.Count == 0)
		{
			return CreateDefault();
		}

		var savedById = settings.Bindings
			.Where(binding => !string.IsNullOrWhiteSpace(binding.Id))
			.GroupBy(binding => binding.Id, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

		var normalized = new HotkeySettings
		{
			Version = CurrentVersion
		};

		foreach (var defaultBinding in CreateDefaultBindings())
		{
			savedById.TryGetValue(defaultBinding.Id, out var savedBinding);
			normalized.Bindings.Add(NormalizeBinding(savedBinding, defaultBinding));
		}

		return normalized;
	}

	private static HotkeyBinding NormalizeBinding(HotkeyBinding? savedBinding, HotkeyBinding defaultBinding)
	{
		var keyCode = savedBinding?.KeyCode?.Trim() ?? defaultBinding.KeyCode;

		return new HotkeyBinding
		{
			Id = defaultBinding.Id,
			Label = string.IsNullOrWhiteSpace(savedBinding?.Label) ? defaultBinding.Label : savedBinding.Label.Trim(),
			KeyCode = keyCode,
			KeyDisplay = string.IsNullOrWhiteSpace(keyCode)
				? string.Empty
				: HotkeyKeyFormatter.Format(keyCode, savedBinding?.KeyDisplay),
			Command = savedBinding?.Command ?? defaultBinding.Command,
			IsEnabled = savedBinding?.IsEnabled ?? defaultBinding.IsEnabled,
			RequiresCommand = defaultBinding.RequiresCommand
		};
	}

	private static HotkeyBinding CreateBinding(string id, string label, string keyCode, string command, bool requiresCommand = true)
	{
		return new HotkeyBinding
		{
			Id = id,
			Label = label,
			KeyCode = keyCode,
			KeyDisplay = HotkeyKeyFormatter.Format(keyCode),
			Command = command,
			IsEnabled = true,
			RequiresCommand = requiresCommand
		};
	}
}

public sealed class HotkeyBindingMap
{
	private readonly Dictionary<string, HotkeyBinding> _bindingsByCode;

	private HotkeyBindingMap(IReadOnlyList<HotkeyBinding> bindings, Dictionary<string, HotkeyBinding> bindingsByCode)
	{
		Bindings = bindings;
		_bindingsByCode = bindingsByCode;
		BoundKeyCodes = bindingsByCode.Values
			.Select(binding => binding.KeyCode)
			.Where(code => !string.IsNullOrWhiteSpace(code))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	public IReadOnlyList<HotkeyBinding> Bindings { get; }
	public IReadOnlyList<string> BoundKeyCodes { get; }

	public static HotkeyBindingMap Create(IEnumerable<HotkeyBinding> bindings)
	{
		var clonedBindings = bindings.Select(binding => binding.Clone()).ToArray();
		var bindingsByCode = new Dictionary<string, HotkeyBinding>(StringComparer.OrdinalIgnoreCase);

		foreach (var binding in clonedBindings)
		{
			if (!binding.IsEnabled ||
			    string.IsNullOrWhiteSpace(binding.KeyCode) ||
			    string.IsNullOrWhiteSpace(binding.Command))
			{
				continue;
			}

			var keyCode = NormalizeKeyCode(binding.KeyCode);
			if (!bindingsByCode.ContainsKey(keyCode))
			{
				bindingsByCode[keyCode] = binding;
			}
		}

		return new HotkeyBindingMap(clonedBindings, bindingsByCode);
	}

	public bool TryGetCommand(string? keyCode, out string command)
	{
		command = string.Empty;
		if (string.IsNullOrWhiteSpace(keyCode))
		{
			return false;
		}

		if (!_bindingsByCode.TryGetValue(NormalizeKeyCode(keyCode), out var binding) ||
		    string.IsNullOrWhiteSpace(binding.Command))
		{
			return false;
		}

		command = binding.Command.Trim();
		return true;
	}

	public static string NormalizeKeyCode(string keyCode)
	{
		return keyCode.Trim();
	}
}

public static class HotkeyInputPolicy
{
	public static bool ShouldSendHotkeyFromEditableField(string? keyCode, string? key)
	{
		return IsNumpadKeyCode(keyCode) || !IsPrintableKey(key);
	}

	public static bool IsNumpadKeyCode(string? keyCode)
	{
		return !string.IsNullOrWhiteSpace(keyCode) &&
		       keyCode.Trim().StartsWith("Numpad", StringComparison.OrdinalIgnoreCase);
	}

	public static bool IsPrintableKey(string? key)
	{
		return key?.Length == 1;
	}
}

public static class HotkeyBindingValidator
{
	public static IReadOnlyList<string> Validate(IEnumerable<HotkeyBinding> bindings)
	{
		var errors = new List<string>();
		var bindingList = bindings.ToArray();

		foreach (var binding in bindingList.Where(binding => binding.IsEnabled))
		{
			if (string.IsNullOrWhiteSpace(binding.Label))
			{
				errors.Add("Every enabled hotkey needs a button label.");
			}

			if (string.IsNullOrWhiteSpace(binding.KeyCode))
			{
				errors.Add($"{DescribeBinding(binding)} needs a key.");
			}

			if (binding.RequiresCommand && string.IsNullOrWhiteSpace(binding.Command))
			{
				errors.Add($"{DescribeBinding(binding)} needs a command.");
			}
		}

		var duplicateKeys = bindingList
			.Where(binding => binding.IsEnabled && !string.IsNullOrWhiteSpace(binding.KeyCode))
			.GroupBy(binding => HotkeyBindingMap.NormalizeKeyCode(binding.KeyCode), StringComparer.OrdinalIgnoreCase)
			.Where(group => group.Count() > 1);

		foreach (var duplicateKey in duplicateKeys)
		{
			var labels = duplicateKey
				.Select(DescribeBinding)
				.Distinct(StringComparer.OrdinalIgnoreCase);
			var display = HotkeyKeyFormatter.Format(duplicateKey.First().KeyCode);
			errors.Add($"{display} is assigned to {string.Join(", ", labels)}.");
		}

		return errors.Distinct(StringComparer.Ordinal).ToArray();
	}

	private static string DescribeBinding(HotkeyBinding binding)
	{
		return string.IsNullOrWhiteSpace(binding.Label) ? "This hotkey" : binding.Label.Trim();
	}
}

public static class HotkeyKeyFormatter
{
	private static readonly Dictionary<string, string> KnownKeyCodes = new(StringComparer.OrdinalIgnoreCase)
	{
		["Numpad0"] = "Numpad 0",
		["Numpad1"] = "Numpad 1",
		["Numpad2"] = "Numpad 2",
		["Numpad3"] = "Numpad 3",
		["Numpad4"] = "Numpad 4",
		["Numpad5"] = "Numpad 5",
		["Numpad6"] = "Numpad 6",
		["Numpad7"] = "Numpad 7",
		["Numpad8"] = "Numpad 8",
		["Numpad9"] = "Numpad 9",
		["NumpadAdd"] = "Numpad +",
		["NumpadDecimal"] = "Numpad .",
		["NumpadDivide"] = "Numpad /",
		["NumpadEnter"] = "Numpad Enter",
		["NumpadMultiply"] = "Numpad *",
		["NumpadSubtract"] = "Numpad -",
		["Space"] = "Space"
	};

	public static string Format(string? keyCode, string? fallback = null)
	{
		if (string.IsNullOrWhiteSpace(keyCode))
		{
			return string.Empty;
		}

		var normalizedCode = keyCode.Trim();
		if (KnownKeyCodes.TryGetValue(normalizedCode, out var display))
		{
			return display;
		}

		if (normalizedCode.StartsWith("Key", StringComparison.OrdinalIgnoreCase) &&
		    normalizedCode.Length == 4)
		{
			return normalizedCode[3].ToString().ToUpperInvariant();
		}

		if (normalizedCode.StartsWith("Digit", StringComparison.OrdinalIgnoreCase) &&
		    normalizedCode.Length > 5)
		{
			return normalizedCode[5..];
		}

		if (normalizedCode.StartsWith("Arrow", StringComparison.OrdinalIgnoreCase) &&
		    normalizedCode.Length > 5)
		{
			return normalizedCode[5..];
		}

		if (!string.IsNullOrWhiteSpace(fallback))
		{
			return fallback.Trim();
		}

		return normalizedCode;
	}
}
