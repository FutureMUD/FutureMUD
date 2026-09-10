#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static readonly IReadOnlySet<string> EarlyModernBlackPowderSupportStableReferences =
		new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"earlymodern_military_naval_cannon_sponge",
			"earlymodern_military_naval_artillery_linstock",
			"earlymodern_military_firearm_gunflint_packet",
			"earlymodern_military_firearm_pyrite_packet",
			"earlymodern_military_naval_peterero_chamber"
		};

	private sealed record EarlyModernMilitaryItemSpec(
		string StableReference,
		string Noun,
		string ShortDescription,
		string FullDescription,
		SizeCategory Size,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		string Material,
		string[] Tags,
		string[] Components,
		string BuilderNotes);

	internal sealed record EarlyModernMilitaryItemSpecTestData(
		string StableReference,
		string Material,
		string FullDescription,
		IReadOnlyCollection<string> Tags,
		IReadOnlyCollection<string> Components);

	internal static IReadOnlyCollection<string> EarlyModernStandardsAndSignalsStableReferencesForTesting =>
		EarlyModernSupportedMilitaryItemSpecs
			.Where(x => x.Components.Any(component => component.StartsWith("MilitaryStandard_", StringComparison.Ordinal) ||
			                                           component.StartsWith("SignalInstrument_", StringComparison.Ordinal)))
			.Select(x => x.StableReference)
			.ToArray();

	internal static IReadOnlyCollection<EarlyModernMilitaryItemSpecTestData> EarlyModernSupportedMilitaryItemSpecsForTesting =>
		EarlyModernSupportedMilitaryItemSpecs
			.Select(x => new EarlyModernMilitaryItemSpecTestData(
				x.StableReference,
				x.Material,
				x.FullDescription,
				x.Tags,
				x.Components))
			.ToArray();

	internal static IReadOnlySet<string> EarlyModernBlackPowderSupportStableReferencesForTesting =>
		EarlyModernBlackPowderSupportStableReferences;

	private static string BuildEarlyModernMilitaryDescription(
		string stableReference,
		string shortDescription,
		string noun,
		string material,
		ItemQuality quality)
	{
		var omittedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"accessory", "armour", "armor", "artillery", "firearm", "melee", "military", "naval", "ranged",
			"tool", "issue", "reinforced", "ornate", "service"
		};
		var form = string.Join(" ", stableReference
			.Remove(0, "earlymodern_military_".Length)
			.Split('_')
			.Where(word => !omittedWords.Contains(word)));
		string profile;
		if (stableReference.Contains("armor", StringComparison.OrdinalIgnoreCase) || stableReference.Contains("armour", StringComparison.OrdinalIgnoreCase))
			profile = $"The {form} arrangement is shaped to overlap and follow the body, leaving the edges and fastenings plainly visible.";
		else if (stableReference.Contains("shield", StringComparison.OrdinalIgnoreCase))
			profile = $"The {form} arrangement gives it a broad face, clear rim, and readily visible hand fittings.";
		else if (stableReference.Contains("melee", StringComparison.OrdinalIgnoreCase) || stableReference.Contains("boarding", StringComparison.OrdinalIgnoreCase))
			profile = $"The {form} arrangement balances the working end against a firm grip or shaft, giving the weapon a direct, martial line.";
		else if (stableReference.Contains("firearm", StringComparison.OrdinalIgnoreCase) || stableReference.Contains("ranged", StringComparison.OrdinalIgnoreCase))
			profile = $"The {form} arrangement sets its stock, barrel, and small fittings in a compact, deliberate line.";
		else if (stableReference.Contains("artillery", StringComparison.OrdinalIgnoreCase) || stableReference.Contains("cannon", StringComparison.OrdinalIgnoreCase) || stableReference.Contains("gun", StringComparison.OrdinalIgnoreCase))
			profile = $"The {form} arrangement uses heavy fittings and reinforced working surfaces for a stout, service-built appearance.";
		else if (stableReference.Contains("uniform", StringComparison.OrdinalIgnoreCase) || stableReference.Contains("coat", StringComparison.OrdinalIgnoreCase) || stableReference.Contains("sash", StringComparison.OrdinalIgnoreCase))
			profile = $"The {form} cut is defined by its seams and visible fastenings, giving the garment a disciplined, formal appearance.";
		else
			profile = $"The {form} arrangement sets its fittings and working surfaces in a clear, practical pattern.";

		var qualityDetail = quality switch
		{
			ItemQuality.Substandard => "workmanlike but spare, with a few coarse marks in the finish",
			ItemQuality.Standard => "plainly finished, with practical edges and uncomplicated fittings",
			ItemQuality.Good => "carefully finished, with clean joins and a restrained, even surface",
			ItemQuality.VeryGood => "finely finished, with crisp details and a deliberately polished surface",
			ItemQuality.Great => "expertly finished, with precise joins and a richly maintained surface",
			_ => "carefully finished, with clean joins and an even surface"
		};
		var article = char.ToUpperInvariant(shortDescription[0]) + shortDescription[1..];
		return $"{article} is fashioned chiefly from {material}, with the {noun} kept clear in its silhouette. {profile} The {material} is {qualityDetail}. Close inspection picks out the proportions and joinery of this {form} pattern.";
	}

	private void SeedEarlyModernMilitaryFirearmsUniformsAndNaval()
	{
		var dependencyIssues = ValidateEarlyModernMilitaryDependencies(EarlyModernSupportedMilitaryItemSpecs);
		if (!_manifestCaptureOnly && dependencyIssues.Count > 0)
		{
			throw new InvalidOperationException(
				"Supported Early Modern military catalogue cannot be seeded because required dependencies are missing:" +
				Environment.NewLine + string.Join(Environment.NewLine, dependencyIssues.Select(x => $" - {x}")));
		}

		foreach (var spec in EarlyModernSupportedMilitaryItemSpecs)
		{
			CreateItem(
				spec.StableReference,
				spec.Noun,
				spec.ShortDescription,
				null,
				spec.FullDescription,
				spec.Size,
				spec.Quality,
				spec.WeightInGrams,
				spec.Cost,
				false,
				false,
				spec.Material,
				spec.Tags,
				spec.Components,
				null,
				null,
				null,
				null,
				spec.BuilderNotes,
				allowLegacyShortDescriptionMatch: false);
		}

		SeedEarlyModernCrossbowSpanningTools();
	}

	private void SeedEarlyModernBlackPowderSupportItems()
	{
		foreach (var spec in EarlyModernSupportedMilitaryItemSpecs.Where(x =>
			         EarlyModernBlackPowderSupportStableReferences.Contains(x.StableReference)))
		{
			CreateItem(
				spec.StableReference,
				spec.Noun,
				spec.ShortDescription,
				null,
				spec.FullDescription,
				spec.Size,
				spec.Quality,
				spec.WeightInGrams,
				spec.Cost,
				false,
				false,
				spec.Material,
				spec.Tags,
				spec.Components,
				null,
				null,
				null,
				null,
				spec.BuilderNotes,
				allowLegacyShortDescriptionMatch: false);
		}
	}

	private void SeedEarlyModernCrossbowSpanningTools()
	{
		const string eraTag = "Era / Early Modern Era";
		const string militaryTag = "Functions / Military Equipment";
		const string toolMarketTag = "Market / Professional Tools / Standard Tools";
		const string spanningRoot = "Functions / Military Equipment / Crossbow Spanning Tools";
		foreach (var (reference, noun, shortDescription, fullDescription, weight, cost, material, toolTag,
		         destroyableComponent) in
		         new[]
		         {
			         ("earlymodern_military_tool_cranequin", "cranequin", "a steel cranequin",
				         "This compact rack-and-pinion cranequin hooks over a crossbow stock and turns through a geared handle to draw an exceptionally heavy prod.",
				         2900.0, 190.0m, "mild steel", "Cranequin", "Destroyable_HeavyMetal"),
			         ("earlymodern_military_tool_goats_foot", "lever", "an iron goat's-foot lever",
				         "This hinged iron goat's-foot lever braces against a crossbow stock and uses its hooked jaws to draw the string with one strong motion.",
				         1450.0, 80.0m, "wrought iron", "Goat's Foot", "Destroyable_HeavyMetal"),
			         ("earlymodern_military_tool_spanning_lever", "lever", "a wooden spanning lever",
				         "This stout wooden spanning lever has an iron hook and reinforced fulcrum, sized to draw a crossbow by controlled leverage.",
				         1800.0, 48.0m, "oak", "Lever", "Destroyable_WoodenHeavy"),
			         ("earlymodern_military_tool_spanning_hook", "hook", "an iron spanning hook",
				         "This belt-mounted iron spanning hook catches a crossbow string while the user straightens against the stock to draw it.",
				         620.0, 32.0m, "wrought iron", "Spanning Hook", "Destroyable_HeavyMetal"),
			         ("earlymodern_military_tool_windlass", "windlass", "a crossbow windlass",
				         "This twin-crank windlass uses cords, hooks, and geared drums to span a powerful crossbow steadily and with little wasted effort.",
				         3400.0, 150.0m, "wrought iron", "Windlass", "Destroyable_HeavyMetal")
		         })
		{
			CreateItem(reference, noun, shortDescription, null, fullDescription, SizeCategory.Normal,
				ItemQuality.Standard, weight, cost, false, false, material,
				[eraTag, militaryTag, toolMarketTag, $"{spanningRoot} / {toolTag}"],
				["Holdable", destroyableComponent], null, null, null, null,
				"Stock crossbow spanning tool retained from the supported dependency-ledger closure tranche.",
				allowLegacyShortDescriptionMatch: false);
		}
	}

	private IReadOnlyList<string> ValidateEarlyModernMilitaryDependencies(
		IEnumerable<EarlyModernMilitaryItemSpec> specs)
	{
		var issues = new List<string>();
		foreach (var spec in specs)
		{
			if (!_materials.ContainsKey(spec.Material))
			{
				issues.Add($"Missing material {spec.Material} for {spec.StableReference}");
			}

			issues.AddRange(spec.Tags
				.Where(x => !_tagsByFullPath.ContainsKey(x))
				.Select(x => $"Missing tag {x} for {spec.StableReference}"));
			issues.AddRange(spec.Components
				.Where(x => !_components.ContainsKey(x))
				.Select(x => $"Missing component {x} for {spec.StableReference}"));
		}

		return issues
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}
}
