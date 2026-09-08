#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureNativeBinding(string SourceIdentity, string? OverlayKey, string Rule,
	IReadOnlyList<string> References, IReadOnlyList<long> LanguageIds, IReadOnlyList<string> Unresolved)
{
	public bool HasResolvedReferences => References.Count > 0 && Unresolved.Count == 0;
	public bool IsResolved => HasResolvedReferences && LanguageIds.Count > 0 && LanguageIds.All(x => x > 0);
}

/// <summary>Resolves only authored defaults. Source names, naming structures and language provenance stay distinct.</summary>
public static class CultureToolkitNativeBindings
{
	public static CultureNativeBinding Source(CultureToolkitCatalogue catalogue, string era, string module, Ethnicity ethnicity,
		IReadOnlyDictionary<string, Language> languages)
	{
		var identity = $"source.{module}.ethnicity.{ethnicity.Name}";
		var defaults = catalogue.Document("data.ethnicity_language_defaults.json");
		var legacy = catalogue.Document("data.legacy_native_language_rules.json");
		var overlays = catalogue.Compose(era).Ethnicities.Where(x => CultureToolkitCatalogue.Text(x, "label") == ethnicity.Name ||
			CultureToolkitCatalogue.Strings(x.GetProperty("legacy_aliases")).Contains(ethnicity.Name, StringComparer.Ordinal)).ToArray();
		var overlayKey = overlays.Length == 1 ? CultureToolkitCatalogue.Text(overlays[0], "key") : null;
		IReadOnlyList<string> references = [];
		var rule = "unresolved-source-crosswalk";
		if (module == "earthantiquity" && legacy.GetProperty("antiquity_ethnicity_bindings").TryGetProperty(ethnicity.Name, out var ancient))
		{
			references = [ancient.GetString()!];
			rule = "antiquity-source-rule";
		}
		foreach (var regional in defaults.GetProperty("retained_regional_overrides").EnumerateArray())
		{
			if (references.Count > 0 || !CultureToolkitCatalogue.Strings(regional.GetProperty("match_names")).Contains(ethnicity.Name, StringComparer.Ordinal)) continue;
			references = regional.TryGetProperty("language", out var language) ? [language.GetString()!] : regional.TryGetProperty("selector", out var selector)
				? catalogue.ResolveSelector(selector.GetString()!, era, []) : DefaultReferences(catalogue, era, CultureToolkitCatalogue.Text(regional, "ethnicity_default"));
			if (references.Count > 0) rule = "retained-regional-override";
		}
		if (references.Count == 0 && overlayKey is not null)
		{
			references = DefaultReferences(catalogue, era, overlayKey);
			if (references.Count > 0) rule = "exact-overlay-label-or-authored-alias";
		}
		if (references.Count == 0)
		{
			var templates = legacy.GetProperty("historical_template_bindings");
			var matches = ethnicity.EthnicitiesNameCultures.Select(x => x.NameCulture.Name).Distinct()
				.Where(x => templates.TryGetProperty(x, out _)).Select(x => templates.GetProperty(x).GetString()!).Distinct().ToArray();
			if (matches.Length == 1) { references = matches; rule = "supplied-source-template-rule"; }
		}
		return Resolve(identity, overlayKey, rule, references, languages);
	}

	public static CultureNativeBinding Canonical(CultureToolkitCatalogue catalogue, string era, string ethnicityKey,
		IReadOnlyDictionary<string, Language> languages) => Resolve(ethnicityKey, ethnicityKey, "canonical-ethnicity-default",
		DefaultReferences(catalogue, era, ethnicityKey), languages);

	private static IReadOnlyList<string> DefaultReferences(CultureToolkitCatalogue catalogue, string era, string key)
	{
		var row = catalogue.Document("data.ethnicity_language_defaults.json").GetProperty("defaults").EnumerateArray()
			.SingleOrDefault(x => CultureToolkitCatalogue.Text(x, "ethnicity") == key);
		return row.ValueKind != JsonValueKind.Undefined && row.GetProperty("by_era").TryGetProperty(era, out var value)
			? CultureToolkitCatalogue.Strings(value).ToArray() : [];
	}

	private static CultureNativeBinding Resolve(string identity, string? overlay, string rule, IReadOnlyList<string> references,
		IReadOnlyDictionary<string, Language> languages)
	{
		var missing = references.Where(x => !languages.ContainsKey(x)).Select(x => $"{identity}: unresolved supplied native language {x}").ToList();
		if (references.Count == 0) missing.Add($"{identity}: no supplied source crosswalk; retain source and keep toolkit activation gated.");
		return new(identity, overlay, rule, references, references.Where(languages.ContainsKey).Select(x => languages[x].Id).Distinct().ToArray(), missing);
	}
}
