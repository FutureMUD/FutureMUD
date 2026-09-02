#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	internal sealed record ItemSeederEraDefinitionTestData(
		string Key,
		IReadOnlyCollection<string> Aliases,
		string DisplayName,
		string EraTag,
		string ManifestModule,
		string StableReferencePrefix,
		string VehicleEraKey,
		bool Selectable);

	internal sealed record ItemSeederTechnologyProfileTestData(
		string Key,
		string DisplayName,
		string PowerStandard,
		string PaperStandard,
		string TelecommunicationsStandard,
		string NetworkAndMediaStandard,
		string VehicleConnectorStandard,
		bool IsCustom);

	private sealed record ItemSeederEraDefinition(
		string Key,
		string[] Aliases,
		string DisplayName,
		string EraTag,
		string ManifestModule,
		string StableReferencePrefix,
		string VehicleEraKey,
		bool Selectable);

	private sealed record ItemSeederTechnologyProfile(
		string Key,
		string DisplayName,
		string PowerStandard,
		string PaperStandard,
		string TelecommunicationsStandard,
		string NetworkAndMediaStandard,
		string VehicleConnectorStandard,
		bool IsCustom = false);

	private static readonly IReadOnlyList<ItemSeederEraDefinition> EraDefinitions =
	[
		new("antiquity", [], "Antiquity", "Era / Antiquity Era", "antiquity", "antiquity_", "antiquity", true),
		new("medieval", [], "Medieval", "Era / Medieval Era", "medieval", "medieval_", "medieval", true),
		new("renaissance", [], "Renaissance", "Era / Renaissance Era", "renaissance", "renaissance_", "renaissance", true),
		new("earlymodern", [], "Early Modern", "Era / Early Modern Era", "earlymodern", "earlymodern_", "earlymodern", true),
		new("industrial", ["revolution"], "Industrial", "Era / Industrial Era", "industrial", "industrial_", "revolution", false),
		new("modern", [], "Modern", "Era / Modern Era", "modern", "modern_", "modern", false),
		new("nuclear", ["atomic"], "Nuclear", "Era / Nuclear Era", "nuclear", "nuclear_", "atomic", false),
		new("information", ["computer"], "Information Age", "Era / Information Age Era", "information", "information_", "computer", false)
	];

	private static readonly IReadOnlyDictionary<string, ItemSeederEraDefinition> EraDefinitionsByToken =
		BuildEraDefinitionsByToken();

	private static readonly string[] ImplementedEraKeys = EraDefinitions
		.Where(x => x.Selectable)
		.Select(x => x.Key)
		.ToArray();

	private static readonly HashSet<string> IndustrialisedEraKeys =
	[
		"industrial", "modern", "nuclear", "information"
	];

	private static readonly IReadOnlyDictionary<string, ItemSeederTechnologyProfile> TechnologyProfiles =
		new Dictionary<string, ItemSeederTechnologyProfile>(StringComparer.OrdinalIgnoreCase)
		{
			["neutral"] = new("neutral", "Neutral stock standard", "World-configured mains power", "A4 and Letter",
				"Generic telecommunications", "Generic data and media", "Generic vehicle service connectors"),
			["northamerican"] = new("northamerican", "North American", "North American mains power", "Letter and Legal",
				"North American telecommunications", "North American data and media", "North American vehicle service connectors"),
			["continentaleuropean"] = new("continentaleuropean", "Continental European", "Continental European mains power", "A-series",
				"Continental European telecommunications", "European data and media", "European vehicle service connectors"),
			["britishirish"] = new("britishirish", "British and Irish", "British and Irish mains power", "A-series",
				"British and Irish telecommunications", "British and Irish data and media", "British and Irish vehicle service connectors"),
			["australasian"] = new("australasian", "Australasian", "Australasian mains power", "A-series",
				"Australasian telecommunications", "Australasian data and media", "Australasian vehicle service connectors"),
			["japanese"] = new("japanese", "Japanese", "Japanese mains power", "JIS paper series",
				"Japanese telecommunications", "Japanese data and media", "Japanese vehicle service connectors"),
			["chinese"] = new("chinese", "Chinese", "Chinese mains power", "A-series",
				"Chinese telecommunications", "Chinese data and media", "Chinese vehicle service connectors"),
			["custom"] = new("custom", "Custom composition", "Builder-selected component prototypes", "Builder-selected paper formats",
				"Builder-selected component prototypes", "Builder-selected component prototypes", "Builder-selected component prototypes", true)
		};

	internal static IReadOnlyCollection<ItemSeederEraDefinitionTestData> EraDefinitionsForTesting => EraDefinitions
		.Select(x => new ItemSeederEraDefinitionTestData(x.Key, x.Aliases, x.DisplayName, x.EraTag,
			x.ManifestModule, x.StableReferencePrefix, x.VehicleEraKey, x.Selectable))
		.ToArray();

	internal static IReadOnlyCollection<ItemSeederTechnologyProfileTestData> TechnologyProfilesForTesting =>
		TechnologyProfiles.Values
			.Select(x => new ItemSeederTechnologyProfileTestData(x.Key, x.DisplayName, x.PowerStandard,
				x.PaperStandard, x.TelecommunicationsStandard, x.NetworkAndMediaStandard,
				x.VehicleConnectorStandard, x.IsCustom))
			.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.ToArray();

	private static IReadOnlyDictionary<string, ItemSeederEraDefinition> BuildEraDefinitionsByToken()
	{
		var result = new Dictionary<string, ItemSeederEraDefinition>(StringComparer.OrdinalIgnoreCase);
		foreach (var definition in EraDefinitions)
		{
			result.Add(definition.Key, definition);
			foreach (var alias in definition.Aliases)
			{
				result.Add(alias, definition);
			}
		}

		return result;
	}

	private static (bool Success, string Error) ValidateEraSelection(string text)
	{
		var tokens = SplitSelectionTokens(text);
		if (tokens.Count == 0)
		{
			return (false, "You must select at least one implemented era.");
		}

		foreach (var token in tokens)
		{
			if (!EraDefinitionsByToken.TryGetValue(token, out var definition))
			{
				return (false, $"The option '{token}' is not a valid era selection.");
			}

			if (!definition.Selectable)
			{
				return (false,
					$"The {definition.DisplayName} item catalogue is planned but is not selectable until its manifest contains real stock content.");
			}
		}

		return (true, string.Empty);
	}

	private static IReadOnlyCollection<string> ParseEraTokens(string? eras)
	{
		return SplitSelectionTokens(eras)
			.Select(x => EraDefinitionsByToken.GetValueOrDefault(x))
			.Where(x => x is { Selectable: true })
			.Select(x => x!.Key)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => Array.IndexOf(ImplementedEraKeys, x))
			.ToArray();
	}

	private static IReadOnlyCollection<string> SplitSelectionTokens(string? text)
	{
		return (text ?? string.Empty)
			.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(x => x.ToLowerInvariant())
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	private static string? GetVehicleEraToken(string token)
	{
		return EraDefinitionsByToken.TryGetValue(token, out var definition)
			? definition.VehicleEraKey
			: null;
	}

	private static bool HasRequestedIndustrialisedEra(IReadOnlyDictionary<string, string> answers)
	{
		if (!answers.TryGetValue("eras", out var eras))
		{
			return false;
		}

		return SplitSelectionTokens(eras)
			.Select(x => EraDefinitionsByToken.GetValueOrDefault(x))
			.Any(x => x is not null && IndustrialisedEraKeys.Contains(x.Key));
	}

	private static bool HasSelectedCustomTechnologyProfile(IReadOnlyDictionary<string, string> answers)
	{
		return HasRequestedIndustrialisedEra(answers) &&
		       answers.TryGetValue("technologyprofile", out var profile) &&
		       profile.Equals("custom", StringComparison.OrdinalIgnoreCase);
	}

	private static (bool Success, string Error) ValidateTechnologyProfile(string text)
	{
		var key = text.Trim();
		return TechnologyProfiles.ContainsKey(key)
			? (true, string.Empty)
			: (false,
				$"You must select one of {string.Join(", ", TechnologyProfiles.Keys.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))}.");
	}

	private static (bool Success, string Error) ValidateCustomText(string text, string label)
	{
		return string.IsNullOrWhiteSpace(text)
			? (false, $"You must enter at least one {label}.")
			: (true, string.Empty);
	}

	private static (bool Success, string Error) ValidateCustomComponentList(
		string text,
		FuturemudDatabaseContext context,
		string label)
	{
		var names = text
			.Split([','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		if (names.Length == 0)
		{
			return (false, $"You must enter at least one UsefulSeeder component prototype for {label}.");
		}

		var installedNames = context.GameItemComponentProtos
			.Select(x => x.Name)
			.AsEnumerable()
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var missing = names.Where(x => !installedNames.Contains(x)).ToArray();
		return missing.Length == 0
			? (true, string.Empty)
			: (false,
				$"The following {label} component prototypes are missing: {string.Join(", ", missing)}. Add them to UsefulSeeder's Modern Item Components package and rerun that package before ItemSeeder.");
	}
}
