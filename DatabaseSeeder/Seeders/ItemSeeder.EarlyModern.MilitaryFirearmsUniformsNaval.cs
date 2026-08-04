#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
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
				x.Tags,
				x.Components))
			.ToArray();

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
