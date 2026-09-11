#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureToolkitPack(string Era, IReadOnlyList<JsonElement> Languages,
	IReadOnlyList<JsonElement> Cultures, IReadOnlyList<JsonElement> Ethnicities,
	IReadOnlyList<JsonElement> Groups, IReadOnlyList<JsonElement> DirectedEdges);

public sealed record CultureLanguageCandidates(IReadOnlyList<string> CanonicalKeys,
	IReadOnlyList<string> RetainedSourceReferences, IReadOnlyList<string> Omitted);

/// <summary>
/// Read-only authored content and compile-time selection. It creates no database entities,
/// player answers or proficiency state. Consumers must bind source references before writing.
/// </summary>
public sealed class CultureToolkitCatalogue
{
	public static IReadOnlyList<string> RequiredDocuments { get; } = Array.AsReadOnly(new[]
	{
		"data.accent_era_policy.json",
		"data.stock_accent_roles.json",
		"data.historical_foreign_accents.json",
		"data.catalogue_statistics.json",
		"data.eras.json",
		"data.ethnicity_language_defaults.json",
		"data.ethnicity_reframing_overlay.json",
		"data.final_decisions.json",
		"data.language_choice_groups.json",
		"data.language_grant_policy.json",
		"data.language_identity_and_labels.json",
		"data.language_starting_value_prog_contract.json",
		"data.latin1_fallback_specification.json",
		"data.legacy_native_language_rules.json",
		"data.literacy_script_grants.json",
		"data.mutual_intelligibility.json",
		"data.name_playability_policy.json",
		"data.naming_pattern_tests.json",
		"data.naming_reuse_ledger.json",
		"data.player_prose_policy.json",
		"data.script_policy.json",
		"data.skill_group_selection_specification.json",
		"data.skill_groups.json",
		"data.social_cultures.json",
		"data.targeted_name_corpora.json",
		"data.vernacular_selectors.json",
		"research.latvian_documentary_name_specimen.json",
		"research.prose_revision_ledger.json",
		"research.review_gates.json",
		"research.sources.json",
	});

	private readonly Dictionary<string, JsonElement> _documents = new(StringComparer.Ordinal);
	private static readonly IReadOnlyDictionary<string, double> Factors = new Dictionary<string, double>
	{
		["native"] = 1.0, ["fluent"] = 0.90, ["educated"] = 0.75,
		["conversational"] = 0.50, ["elementary"] = 0.25
	};

	public CultureToolkitCatalogue()
	{
		var assembly = typeof(CultureToolkitCatalogue).Assembly;
		foreach (var resource in assembly.GetManifestResourceNames()
			.Where(x => x.StartsWith("CultureToolkit.", StringComparison.Ordinal)))
		{
			using var stream = assembly.GetManifestResourceStream(resource)!;
			using var document = JsonDocument.Parse(stream);
			ValidateJson(document.RootElement, resource);
			_documents.Add(resource["CultureToolkit.".Length..], document.RootElement.Clone());
		}
		ValidateRequiredDocuments(_documents.Keys);
	}

	private static void ValidateJson(JsonElement value, string path)
	{
		if (value.ValueKind == JsonValueKind.Object)
		{
			var names = new HashSet<string>(StringComparer.Ordinal);
			foreach (var property in value.EnumerateObject())
			{
				if (!names.Add(property.Name)) throw new InvalidDataException($"Duplicate JSON property: {path}.{property.Name}");
				ValidateJson(property.Value, $"{path}.{property.Name}");
			}
		}
		else if (value.ValueKind == JsonValueKind.Array)
		{
			var keys = new HashSet<string>(StringComparer.Ordinal);
			foreach (var element in value.EnumerateArray())
			{
				if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("key", out var key) &&
					!keys.Add(key.GetString()!)) throw new InvalidDataException($"Duplicate JSON key: {path}:{key}");
				ValidateJson(element, path);
			}
		}
	}

	public static void ValidateRequiredDocuments(IEnumerable<string> names)
	{
		var missing = RequiredDocuments.Except(names, StringComparer.Ordinal).ToArray();
		if (missing.Length > 0) throw new InvalidDataException("Missing culture toolkit documents: " + string.Join(", ", missing));
	}

	public JsonElement Document(string name) => _documents.TryGetValue(name, out var value)
		? value : throw new InvalidDataException($"Missing culture toolkit document: {name}");

	public CultureToolkitPack Compose(string era)
	{
		if (!Document("data.eras.json").EnumerateArray().Any(x => Text(x, "key") == era))
		{
			throw new ArgumentException($"Unknown culture toolkit era: {era}", nameof(era));
		}
		var languages = Rows("language_identity_and_labels")
			.Where(x => Text(x, "entity_kind") == "language-specification")
			.Where(x => x.GetProperty("labels").TryGetProperty(era, out _)).ToArray();
		if (languages.Select(x => Text(x.GetProperty("labels"), era))
			.Distinct(StringComparer.OrdinalIgnoreCase).Count() != languages.Length)
		{
			throw new InvalidDataException($"Language labels collide in {era}.");
		}
		var keys = languages.Select(x => Text(x, "key")).ToHashSet(StringComparer.Ordinal);
		var edges = Document("data.mutual_intelligibility.json").GetProperty("directed_edges")
			.EnumerateArray().Where(x => InPack(x, era)).ToArray();
		foreach (var edge in edges)
		{
			var listener = Text(edge, "listener_language");
			var target = Text(edge, "target_language");
			if (!keys.Contains(listener) || !keys.Contains(target) || listener == target ||
				edge.GetProperty("difficulty_value").GetInt32() is < 7 or > 9)
			{
				throw new InvalidDataException($"Invalid directed language edge: {Text(edge, "key")}");
			}
		}
		return new CultureToolkitPack(era, languages, Rows("social_cultures").Where(x => InPack(x, era)).ToArray(),
			Rows("ethnicity_reframing_overlay").Where(x => InPack(x, era)).ToArray(),
			Rows("skill_groups").Where(x => x.GetProperty("eligibility").GetProperty("culture_packs")
				.EnumerateObject().Concat(x.GetProperty("eligibility").GetProperty("ethnicity_packs").EnumerateObject())
				.Any(p => Strings(p.Value).Contains(era, StringComparer.Ordinal))).ToArray(), edges);
	}

	public IReadOnlyList<string> ResolveSelector(string selector, string era, IReadOnlyList<string> native)
	{
		if (selector == "home") return native;
		if (selector.StartsWith("language:", StringComparison.Ordinal)) return [selector[9..]];
		if (!Document("data.vernacular_selectors.json").TryGetProperty(selector, out var definition))
		{
			throw new InvalidDataException($"Unknown language selector: {selector}");
		}
		if (Text(definition, "mode") != "era-stage")
		{
			throw new InvalidDataException($"Selector {selector} requires an exact community/source binding.");
		}
		return definition.GetProperty("by_era").TryGetProperty(era, out var selected)
			? [selected.GetString()!] : [];
	}

	public CultureLanguageCandidates Candidates(string recipeKey, CultureToolkitPack pack)
	{
		var recipe = Document("data.language_choice_groups.json").GetProperty(recipeKey);
		var selectors = recipe.TryGetProperty("selectors_by_era", out var overrides) &&
			overrides.TryGetProperty(pack.Era, out var replacement) ? replacement : recipe.GetProperty("selectors");
		var available = pack.Languages.Select(x => Text(x, "key")).ToHashSet(StringComparer.Ordinal);
		var candidates = Strings(selectors).SelectMany(x => ResolveSelector(x, pack.Era, []))
			.Concat(Strings(recipe.GetProperty("languages"))).Distinct(StringComparer.Ordinal).ToArray();
		var retained = recipe.TryGetProperty("retained_catalogue_options", out var legacy)
			? Strings(legacy).Select(x => $"legacy:{x}").ToArray() : [];
		return new CultureLanguageCandidates(candidates.Where(available.Contains).ToArray(), retained,
			candidates.Where(x => !available.Contains(x)).ToArray());
	}

	/// <summary>Mandatory factors for an explicit overlay. Unresolved retained identities fail closed.</summary>
	public IReadOnlyDictionary<string, double> FixedGrantFactors(string era, string ethnicityKey, string cultureKey)
	{
		var pack = Compose(era);
		var culture = pack.Cultures.Single(x => Text(x, "key") == cultureKey);
		var ethnicity = Document("data.ethnicity_language_defaults.json").GetProperty("defaults")
			.EnumerateArray().Single(x => Text(x, "ethnicity") == ethnicityKey);
		if (!ethnicity.GetProperty("by_era").TryGetProperty(era, out var defaults))
		{
			throw new InvalidDataException($"No fixed native binding for {ethnicityKey} in {era}.");
		}
		var native = Strings(defaults).ToArray();
		var result = new Dictionary<string, double>(StringComparer.Ordinal);
		void Grant(string key, string tier) => result[key] = Math.Max(result.GetValueOrDefault(key), Factors[tier]);
		foreach (var language in native) Grant(language, "native");
		foreach (var language in ResolveSelector(Text(culture, "vernacular_selector"), era, native))
			Grant(language, Text(culture, "cultural_vernacular_proficiency"));
		foreach (var grant in culture.GetProperty("additional_language_grants").EnumerateArray()
			.Where(x => Text(x, "condition") == "all-listed-packs" || Text(x, "condition") == era))
			Grant(Text(grant, "language"), Text(grant, "proficiency"));
		return result;
	}

	/// <summary>Eligibility affects starting values even for ordinary purchases; it grants no skills.</summary>
	public IReadOnlyDictionary<string, double> BackgroundFactors(string era, string ethnicityKey, string cultureKey)
	{
		var result = FixedGrantFactors(era, ethnicityKey, cultureKey).ToDictionary(x => x.Key, x => x.Value);
		var pack = Compose(era);
		foreach (var group in pack.Groups.Where(x => Eligible(x, era, ethnicityKey, cultureKey)))
		{
			var recipeKey = Text(group.GetProperty("membership_recipe"), "source_key");
			var factor = Factors[Text(Document("data.language_choice_groups.json").GetProperty(recipeKey), "proficiency")];
			var candidates = Candidates(recipeKey, pack);
			foreach (var key in candidates.CanonicalKeys.Concat(candidates.RetainedSourceReferences))
				result[key] = Math.Max(result.GetValueOrDefault(key), factor);
		}
		return result;
	}

	public static bool Eligible(JsonElement group, string era, string ethnicityKey, string cultureKey)
	{
		var eligibility = group.GetProperty("eligibility");
		return (eligibility.GetProperty("culture_packs").TryGetProperty(cultureKey, out var culture) &&
			Strings(culture).Contains(era, StringComparer.Ordinal)) ||
			(eligibility.GetProperty("ethnicity_packs").TryGetProperty(ethnicityKey, out var ethnicity) &&
			 Strings(ethnicity).Contains(era, StringComparer.Ordinal));
	}

	private IEnumerable<JsonElement> Rows(string name) => Document($"data.{name}.json").EnumerateArray();
	public static string Text(JsonElement value, string property) => value.GetProperty(property).GetString()!;
	public static IEnumerable<string> Strings(JsonElement value) => value.EnumerateArray().Select(x => x.GetString()!);
	private static bool InPack(JsonElement value, string era) => Strings(value.GetProperty("packs")).Contains(era, StringComparer.Ordinal);
}
