#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureAccentPolicyResult(string Rule, IReadOnlyList<string> AllowedPacks, string Role);

public static class CultureToolkitAccentPolicy
{
	public static CultureAccentPolicyResult Resolve(CultureToolkitCatalogue catalogue, string module,
		string language, string name, string group, MudSharp.Communication.Language.AccentRole accentRole)
	{
		var policy = catalogue.Document("data.accent_era_policy.json");
		bool Same(JsonElement row) => string.Equals(CultureToolkitCatalogue.Text(row, "source_pack"), module, StringComparison.OrdinalIgnoreCase) &&
			string.Equals(CultureToolkitCatalogue.Text(row, "source_language"), language, StringComparison.OrdinalIgnoreCase);
		var exact = policy.GetProperty("exact_accent_overrides").EnumerateArray().SingleOrDefault(x => Same(x) &&
			string.Equals(CultureToolkitCatalogue.Text(x, "accent_name"), name, StringComparison.OrdinalIgnoreCase));
		var role = accentRole != MudSharp.Communication.Language.AccentRole.Native ? "learner_or_foreign" : "native-tradition";
		if (exact.ValueKind != JsonValueKind.Undefined)
			return new($"exact:{module}:{language}:{name}", CultureToolkitCatalogue.Strings(exact.GetProperty("allowed_packs")).ToArray(),
				role);
		var source = policy.GetProperty("source_language_overrides").EnumerateArray().SingleOrDefault(Same);
		var allowed = CultureToolkitCatalogue.Strings(source.ValueKind != JsonValueKind.Undefined
			? source.GetProperty("allowed_packs") : policy.GetProperty("source_module_defaults").GetProperty(module)).ToArray();
		var rule = source.ValueKind != JsonValueKind.Undefined ? $"language:{module}:{language}" : $"module:{module}";
		foreach (var marker in policy.GetProperty("late_tradition_markers").EnumerateArray())
		{
			var text = CultureToolkitCatalogue.Text(marker, "marker");
			if (!name.Contains(text, StringComparison.OrdinalIgnoreCase) && !group.Contains(text, StringComparison.OrdinalIgnoreCase)) continue;
			allowed = allowed.Intersect(CultureToolkitCatalogue.Strings(marker.GetProperty("allowed_packs"))).ToArray();
			rule += $";marker:{text}";
		}
		return new(rule, allowed, role);
	}

}
