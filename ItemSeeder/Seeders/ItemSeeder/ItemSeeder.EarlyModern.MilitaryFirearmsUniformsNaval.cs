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
			.Select(x => x.Metadata.StableKey)
			.ToArray();

	internal static IReadOnlyCollection<EarlyModernMilitaryItemSpecTestData> EarlyModernSupportedMilitaryItemSpecsForTesting =>
		EarlyModernSupportedMilitaryItemSpecs
			.Select(x => new EarlyModernMilitaryItemSpecTestData(
				x.Metadata.StableKey,
				x.Material,
				x.FullDescription,
				x.Tags,
				x.Components))
			.ToArray();

	internal static IReadOnlySet<string> EarlyModernBlackPowderSupportStableReferencesForTesting =>
		EarlyModernBlackPowderSupportStableReferences;

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
			InstallCatalogueItem(spec);
		}

		SeedEarlyModernCrossbowSpanningTools();
	}

	private void SeedEarlyModernBlackPowderSupportItems()
	{
		foreach (var spec in EarlyModernSupportedMilitaryItemSpecs.Where(x =>
			         EarlyModernBlackPowderSupportStableReferences.Contains(x.Metadata.StableKey)))
		{
			InstallCatalogueItem(spec);
		}
	}

	private void SeedEarlyModernCrossbowSpanningTools()
	{
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
			InstallCatalogueItem(CompiledAuthoredItemCatalogue.Get(reference, "SeedEarlyModernCrossbowSpanningTools"));
		}
	}

	private IReadOnlyList<string> ValidateEarlyModernMilitaryDependencies(
		IEnumerable<CatalogueItemDefinition> specs)
	{
		var issues = new List<string>();
		foreach (var spec in specs)
		{
			if (!_materials.ContainsKey(spec.Material))
			{
				issues.Add($"Missing material {spec.Material} for {spec.Metadata.StableKey}");
			}

			issues.AddRange(spec.Tags
				.Where(x => !_tagsByFullPath.ContainsKey(x))
				.Select(x => $"Missing tag {x} for {spec.Metadata.StableKey}"));
			issues.AddRange(spec.Components
				.Where(x => !_components.ContainsKey(x))
				.Select(x => $"Missing component {x} for {spec.Metadata.StableKey}"));
		}

		return issues
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}
}
