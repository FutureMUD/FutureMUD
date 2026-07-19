#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PreIndustrialBaselineTests
{
	private static readonly string[] RequiredNewStableReferences =
	[
		"preindustrial_printing_hand_press",
		"preindustrial_printing_type_case",
		"preindustrial_printing_composing_stick",
		"preindustrial_printing_chase",
		"preindustrial_printing_inking_balls",
		"preindustrial_printing_drying_rack",
		"preindustrial_printing_broadside_sheet",
		"preindustrial_printing_pamphlet",
		"preindustrial_printing_almanac",
		"preindustrial_printing_blank_form",
		"preindustrial_printing_printed_map_sheet",
		"preindustrial_navigation_magnetic_compass",
		"preindustrial_navigation_pair_of_dividers",
		"preindustrial_navigation_cross_staff",
		"preindustrial_navigation_mariner_astrolabe",
		"preindustrial_navigation_chart_case",
		"preindustrial_surveying_measuring_chain",
		"preindustrial_surveying_plane_table",
		"preindustrial_optics_spectacles",
		"preindustrial_optics_magnifying_lens",
		"preindustrial_optics_telescope",
		"preindustrial_science_balance_scales",
		"preindustrial_science_glass_specimen_jar",
		"preindustrial_firearms_powder_horn",
		"preindustrial_firearms_powder_flask",
		"preindustrial_firearms_priming_flask",
		"preindustrial_firearms_shot_pouch",
		"preindustrial_firearms_match_cord_bundle",
		"preindustrial_firearms_musket_wadding_packet",
		"preindustrial_firearms_bullet_mould",
		"preindustrial_firearms_ramrod",
		"preindustrial_firearms_touchhole_pick",
		"preindustrial_firearms_cleaning_rod",
		"preindustrial_trade_tea_chest",
		"preindustrial_trade_coffee_sack",
		"preindustrial_trade_cacao_sack",
		"preindustrial_trade_tobacco_bale",
		"preindustrial_trade_sugar_hogshead",
		"preindustrial_trade_spice_chest",
		"preindustrial_trade_indigo_cake_box",
		"preindustrial_trade_porcelain_packing_crate",
		"preindustrial_trade_glass_bottle_crate",
		"preindustrial_trade_silk_bale",
		"preindustrial_trade_cotton_bale"
	];

	[TestMethod]
	public void SharedBaselineEraSelection_IncludesAllFourPreIndustrialErasOnlyWhenPresent()
	{
		Assert.IsTrue(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting("antiquity"));
		Assert.IsTrue(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting("medieval"));
		Assert.IsTrue(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting("renaissance"));
		Assert.IsTrue(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting("earlymodern"));
		Assert.IsFalse(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting(null));
		Assert.IsFalse(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting(string.Empty));
		Assert.IsFalse(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting("modern"));
	}

	[TestMethod]
	public void SharedBaselineAliasCatalogue_IsCompleteUniqueAndPreservesSources()
	{
		var mappings = PreIndustrialBaselineExpectations.RequiredAliases;
		Assert.AreEqual(342, mappings.Count);
		Assert.AreEqual(mappings.Count, mappings.Values.Distinct(StringComparer.OrdinalIgnoreCase).Count());

		var seederPath = Path.Combine(SourceRoot(), "DatabaseSeeder", "Seeders");
		var allSeederSource = string.Join("\n", Directory
			.GetFiles(seederPath, "ItemSeeder.*.cs")
			.Append(Path.Combine(seederPath, "ItemSeeder.cs"))
			.Where(x => !Path.GetFileName(x).Contains("PreIndustrialBaseline", StringComparison.OrdinalIgnoreCase))
			.Select(File.ReadAllText));
		var aliasSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.PreIndustrialBaseline.Aliases.cs") +
		                  ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.PreIndustrialBaseline.cs");

		foreach (var (source, alias) in mappings)
		{
			StringAssert.Contains(allSeederSource, $"\"{source}\"", $"Missing retained source row {source}.");
			StringAssert.Contains(aliasSource, $"\"{alias}\"", $"Missing alias row {alias}.");
			StringAssert.Contains(ItemSeeder.BuildPreIndustrialAliasBuilderNotesForTesting(source), source);
		}
	}

	[TestMethod]
	public void PreIndustrialStableReferences_AreLowercaseUnderscoreOnlyAndDoNotCollide()
	{
		var aliases = PreIndustrialBaselineExpectations.RequiredAliases.Values;
		var newItems = ItemSeeder.PreIndustrialNewItemSpecsForTesting.Select(x => x.StableReference);
		var all = aliases.Concat(newItems).ToArray();

		Assert.AreEqual(all.Length, all.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		foreach (var stableReference in all)
		{
			Assert.IsTrue(Regex.IsMatch(stableReference, "^preindustrial_[a-z0-9_]+$"), stableReference);
		}
	}

	[TestMethod]
	public void RequiredNewCatalogueRows_AllExist()
	{
		var all = PreIndustrialBaselineExpectations.RequiredAliases.Values
			.Concat(ItemSeeder.PreIndustrialNewItemSpecsForTesting.Select(x => x.StableReference))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		foreach (var stableReference in RequiredNewStableReferences)
		{
			Assert.IsTrue(all.Contains(stableReference), $"Missing required new row {stableReference}.");
		}
	}

	[TestMethod]
	public void PreIndustrialRows_UseMaintainedMaterialsTagsAndComponents()
	{
		var materials = ReadJsonPropertySet("Design Documents", "Data", "Seeded_Materials.json", "Material Name");
		var components = ReadJsonPropertySet("Design Documents", "Data", "Seeded_Item_Components.json", "Component Name");
		var tags = File.ReadLines(Path.Combine(SourceRoot(), "Design Documents", "Data", "SeededTagHierarchy.csv"))
			.Skip(1)
			.Select(x => x.Split('\t'))
			.Where(x => x.Length >= 3)
			.Select(x => x[2])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		var aliasSpecs = PreIndustrialBaselineExpectations.DirectAliasSpecs
			.Select(x => (StableReference: x.AliasStableReference, Material: x.Material, Tags: x.Tags,
				Components: x.Components));
		var runtimeSpecs = ItemSeeder.PreIndustrialNewItemSpecsForTesting
			.Concat(ItemSeeder.PreIndustrialAntiquityAliasSpecsForTesting)
			.Select(x => (StableReference: x.StableReference, Material: x.Material, Tags: x.Tags,
				Components: x.Components));

		var errors = new List<string>();
		foreach (var spec in aliasSpecs.Concat(runtimeSpecs))
		{
			if (!materials.Contains(spec.Material))
			{
				errors.Add($"{spec.StableReference}: missing material {spec.Material}");
			}
			Assert.IsNotNull(spec.Tags, $"{spec.StableReference}: tags must not be null.");
			Assert.IsNotNull(spec.Components, $"{spec.StableReference}: components must not be null.");
			foreach (var tag in spec.Tags)
			{
				if (!tags.Contains(tag))
				{
					errors.Add($"{spec.StableReference}: missing tag {tag}");
				}
			}
			foreach (var component in spec.Components)
			{
				if (!components.Contains(component))
				{
					errors.Add($"{spec.StableReference}: missing component {component}");
				}
			}
		}

		Assert.AreEqual(0, errors.Count, string.Join(Environment.NewLine, errors));
	}

	[TestMethod]
	public void NewPortableRowsAreHoldableAndFixedFixturesAreNot()
	{
		foreach (var spec in ItemSeeder.PreIndustrialNewItemSpecsForTesting)
		{
			var hasHoldable = spec.Components.Contains("Holdable", StringComparer.OrdinalIgnoreCase);
			Assert.AreEqual(!spec.FixedFixture, hasHoldable, spec.StableReference);
		}
	}

	[TestMethod]
	public void AliasLifecycleReferences_DoNotFallBackToMedievalRows()
	{
		var aliasSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.PreIndustrialBaseline.Aliases.cs");
		var medievalReferences = Regex.Matches(aliasSource, "\"medieval_[a-z0-9_]+\"").Count;
		Assert.AreEqual(PreIndustrialBaselineExpectations.DirectAliasSpecs.Count, medievalReferences,
			"Each alias call should contain only its source medieval reference; lifecycle targets must use aliases.");
	}

	[TestMethod]
	public void GunpowderSupportRows_DoNotUseWeaponOrExplosiveComponents()
	{
		var forbiddenPrefixes = new[] { "Gun", "Ammunition", "Bomb", "Explosive" };
		foreach (var spec in ItemSeeder.PreIndustrialNewItemSpecsForTesting.Where(x => x.Group == "GunpowderSupport"))
		{
			Assert.IsFalse(spec.Components.Any(component => forbiddenPrefixes.Any(prefix =>
				component.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))), spec.StableReference);
		}
	}

	[TestMethod]
	public void ExplicitNonGoals_AreNotPromoted()
	{
		var aliases = PreIndustrialBaselineExpectations.RequiredAliases.Values.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var excluded = new[]
		{
			"preindustrial_door_lockable_portcullis_grate",
			"preindustrial_door_lockable_tower_stair_door",
			"preindustrial_door_lockable_town_gate_leaf",
			"preindustrial_clothing_linen_braies",
			"preindustrial_clothing_linen_undertunic",
			"preindustrial_clothing_linen_chemise",
			"preindustrial_clothing_linen_breastband"
		};

		foreach (var stableReference in excluded)
		{
			Assert.IsFalse(aliases.Contains(stableReference), stableReference);
		}
	}

	[TestMethod]
	public void MedievalWritingTagAndComponentStrings_ContainNoBackticks()
	{
		var source = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalWriting.cs");
		Assert.IsFalse(source.Contains('`'), "No medieval writing tag/component string may contain '`'.");
	}

	private static HashSet<string> ReadJsonPropertySet(params string[] partsAndProperty)
	{
		var property = partsAndProperty[^1];
		var path = Path.Combine(new[] { SourceRoot() }.Concat(partsAndProperty[..^1]).ToArray());
		using var document = JsonDocument.Parse(File.ReadAllText(path));
		return document.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty(property).GetString())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.Combine(new[] { SourceRoot() }.Concat(parts).ToArray()));
	}

	private static string SourceRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MudSharp.sln")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate repository root from test output path.");
		return directory.FullName;
	}
}
