#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Character.Name;

/// <summary>Validates optional naming declarations, without changing any gameplay state.</summary>
public static class NameSelectionValidation
{
	public static string? Validate(INameCulture culture, IReadOnlyDictionary<NameUsage, List<string>> selections,
		IReadOnlySet<NameUsage>? confirmed = null, IReadOnlyDictionary<NameUsage, bool>? display = null)
	{
		var elements = culture.NameCultureElements.ToDictionary(x => x.Usage);
		if (selections.Keys.Any(x => !elements.ContainsKey(x))) return "This culture does not define that name component.";
		foreach (var element in elements.Values)
		{
			var values = selections.TryGetValue(element.Usage, out var selected) ? selected : [];
			if (element.ValidateValues(values, selections) is { } error) return error;
			if (values.Distinct(System.StringComparer.OrdinalIgnoreCase).Count() != values.Count) return "Duplicate name components are not allowed.";
			if (values.Any(x => !x.Any(char.IsLetter) || NameEntryValidation.HasInvalidCharacters(x, culture.AllowTypographicPunctuation) ||
				NameEntryValidation.HasConsecutivePunctuation(x, culture.AllowTypographicPunctuation))) return "Invalid name component characters.";
			if (values.Count > 0 && element.ConfirmationPrompt is not null && confirmed?.Contains(element.Usage) != true)
				return $"Explicit confirmation is required for {element.Name}.";
		}
		if (confirmed?.Any(x => !elements.TryGetValue(x, out var element) || element.ConfirmationPrompt is null ||
			!selections.TryGetValue(x, out var values) || values.Count == 0) == true) return "Confirmation requires a populated, confirmable component.";
		if (display?.Keys.Any(x => !elements.TryGetValue(x, out var element) || element.DisplayChoicePrompt is null ||
			!selections.TryGetValue(x, out var values) || values.Count == 0) == true) return "Display choices require a populated, configurable component.";
		return null;
	}
}
