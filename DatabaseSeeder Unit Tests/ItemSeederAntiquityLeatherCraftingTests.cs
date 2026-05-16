#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederAntiquityLeatherCraftingTests
{
	private static readonly string[] LeatherClothingStableReferences =
	[
		"antiquity_plain_leather_belt",
		"antiquity_bronze_buckled_leather_belt",
		"antiquity_plain_leather_sandals",
		"antiquity_fine_leather_sandals",
		"antiquity_soft_leather_shoes",
		"antiquity_ankle_leather_boots",
		"antiquity_low_strapped_leather_shoes",
		"antiquity_soft_leather_riding_boots",
		"adjacent_antiquity_pointed_leather_shoes",
		"adjacent_antiquity_fine_pointed_leather_shoes",
		"adjacent_antiquity_soft_riding_boots",
		"adjacent_antiquity_fine_linen_sandals"
	];

	private static readonly string[] LeatherArmourStableReferences =
	[
		"antiquity_celtic_dyed_leather_scale_vest",
		"antiquity_celtic_bronze_studded_leather_belt",
		"antiquity_celtic_leather_war_bracers",
		"antiquity_celtic_fur_lined_war_boots",
		"antiquity_germanic_broad_leather_war_belt",
		"antiquity_germanic_hide_war_bracers",
		"antiquity_germanic_fur_cuffed_high_boots",
		"antiquity_etruscan_pteruges_leather_girdle",
		"antiquity_etruscan_bronze_studded_sandals",
		"antiquity_roman_plated_military_belt",
		"antiquity_roman_aproned_military_belt",
		"antiquity_roman_reinforced_caligae",
		"antiquity_roman_leather_field_boots",
		"antiquity_hellenic_leather_pteruges_girdle",
		"antiquity_hellenic_bronze_studded_sandals",
		"antiquity_punic_bronze_studded_girdle",
		"antiquity_persian_scale_anaxyrides",
		"antiquity_persian_soft_riding_boots",
		"antiquity_egyptian_leather_scale_cuirass",
		"antiquity_egyptian_scale_kilt_guard",
		"antiquity_egyptian_leather_archer_bracer",
		"antiquity_anatolian_leather_cavalry_boots",
		"antiquity_scythian_leather_scale_corselet",
		"antiquity_scythian_conical_scale_cap",
		"antiquity_scythian_scale_trousers",
		"antiquity_scythian_high_riding_boots",
		"antiquity_kushite_leather_scale_breastguard",
		"antiquity_kushite_leather_archer_bracer",
		"antiquity_kushite_leather_kilt_guard",
		"antiquity_kushite_sand_armoured_sandals"
	];

	private static readonly string[] LeatherContainerStableReferences =
	[
		"antiquity_smoked_hide_meat_bag",
		"antiquity_leather_document_case",
		"antiquity_leather_mirror_case",
		"antiquity_plain_leather_belt_pouch",
		"antiquity_double_strap_travel_pack",
		"antiquity_fur_lined_forager_bag",
		"antiquity_deer_leather_game_bag",
		"antiquity_folded_tablet_wallet",
		"antiquity_round_coin_purse",
		"antiquity_leather_dispatch_satchel",
		"antiquity_wide_belt_document_pouch",
		"antiquity_steppe_saddlebag_pack",
		"antiquity_steppe_gorytos_case",
		"antiquity_fur_provision_pouch",
		"antiquity_liquid_steppe_milk_skin",
		"antiquity_liquid_plain_leather_waterskin",
		"antiquity_liquid_wide_mouth_waterskin",
		"antiquity_liquid_leather_belt_oil_flask",
		"antiquity_liquid_hide_ale_skin",
		"antiquity_liquid_birch_stoppered_mead_skin",
		"antiquity_liquid_soldier_shoulder_canteen",
		"antiquity_liquid_sailor_water_skin",
		"antiquity_liquid_caravan_waterskin",
		"antiquity_liquid_silver_tipped_belt_flask",
		"antiquity_liquid_felt_covered_riding_canteen",
		"antiquity_liquid_saddle_waterskin",
		"antiquity_liquid_steppe_kumis_skin",
		"antiquity_liquid_tooled_leather_flask",
		"antiquity_tableware_scythian_leather_travel_cup"
	];

	private static readonly string[] LeatherFurnishingStableReferences =
	[
		"antiquity_fur_door_hanging",
		"antiquity_leather_tent_door_flap"
	];

	private static readonly string[] LeatherCraftStableReferences =
		LeatherClothingStableReferences
			.Concat(LeatherArmourStableReferences)
			.Concat(LeatherContainerStableReferences)
			.Concat(LeatherFurnishingStableReferences)
			.ToArray();

	[TestMethod]
	public void LeatherCrafts_HaveStableReferencesInItemAndCraftSources()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var itemCraftingSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");

		foreach (var expected in new[]
		{
			"AntiquityLeatherClothingStableReferences",
			"AntiquityLeatherArmourStableReferences",
			"AntiquityLeatherContainerStableReferences",
			"AntiquityLeatherFurnishingStableReferences",
			"SeedAntiquityLeatherPreparationCrafts();",
			"SeedAntiquityLeatherClothingCrafts();",
			"SeedAntiquityLeatherArmourCrafts();",
			"SeedAntiquityLeatherContainerCrafts();",
			"SeedAntiquityLeatherFurnishingCrafts();",
			"private void SeedAntiquityLeatherPreparationCrafts()",
			"private void SeedAntiquityLeatherClothingCrafts()",
			"private void SeedAntiquityLeatherArmourCrafts()",
			"private void SeedAntiquityLeatherContainerCrafts()",
			"private void SeedAntiquityLeatherFurnishingCrafts()"
		})
		{
			AssertContains(itemCraftingSource + craftSource, expected);
		}

		foreach (var stableReference in LeatherCraftStableReferences)
		{
			AssertContains(itemSource, $"\"{stableReference}\"");
			AssertContains(craftSource, $"\"{stableReference}\"");
			AssertContains(itemCraftingSource, $"[\"{stableReference}\"]");
		}
	}

	[TestMethod]
	public void LeatherCrafts_CoverEveryAntiquityLeatherMaterialItem()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");
		var leatherItemStableReferences = FindAntiquityLeatherMaterialItemStableReferences(itemSource)
			.ToArray();

		Assert.IsTrue(leatherItemStableReferences.Length >= 70, "Expected the material audit to find the full antiquity leather, hide, and fur item surface.");

		var missingCrafts = leatherItemStableReferences
			.Where(x => !craftSource.Contains($"\"{x}\"", StringComparison.Ordinal))
			.ToArray();

		Assert.AreEqual(
			0,
			missingCrafts.Length,
			"Every antiquity item whose base material is leather, hide, or fur should have a craft. Missing: " +
			string.Join(", ", missingCrafts));
	}

	[TestMethod]
	public void LeatherCrafts_UseLeatherworkingKnowledgeTraitAndCommodityPipeline()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");

		foreach (var expected in new[]
		{
			"private const string AntiquityLeatherKnowledge = \"Ancient Hide and Leatherworking\"",
			"\"Leatherworking\"",
			"\"Leathermaking\"",
			"CommodityTag - 1 kilogram 600 grams of a material tagged as Animal Skin",
			"LiquidUse - 2 litres of Water",
			"LiquidUse - 3 litres of Water",
			"LiquidTagUse - 2 litres of a liquid tagged Tanning Agent",
			"tag Prepared Hide",
			"tag Tanned Leather",
			"tag Leather Sole",
			"tag Leather Strap",
			"tag Leather Thong",
			"tag Leather Panel",
			"tag Hardened Leather Panel",
			"tag Leather Scale",
			"tag Sealed Leather Panel",
			"piletag Tanned Leather",
			"piletag Leather Sole",
			"piletag Leather Strap",
			"piletag Leather Thong",
			"piletag Leather Panel",
			"piletag Hardened Leather Panel",
			"AddLeatherCommodityInput(inputs, armourCraft.Scales, \"Leather Scale\")",
			"AddLeatherCommodityInput(inputs, containerCraft.SealedPanels, \"Sealed Leather Panel\")",
			"Commodity - 120 grams of beeswax",
			"Commodity - 90 grams of beeswax",
			"StableSimpleProduct(",
			"StableVariableProduct("
		})
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void LeatherCrafts_HaveSupportingTagsAndPeriodTools()
	{
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var hierarchySource = ReadSource("Design Documents", "SeededTagHierarchy.csv");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");

		foreach (var expected in new[]
		{
			"AddTag(context, \"Leather Commodity\", \"Material Functions\")",
			"AddTag(context, \"Prepared Hide\", \"Leather Commodity\")",
			"AddTag(context, \"Tanned Leather\", \"Leather Commodity\")",
			"AddTag(context, \"Leather Sole\", \"Leather Commodity\")",
			"AddTag(context, \"Leather Strap\", \"Leather Commodity\")",
			"AddTag(context, \"Leather Thong\", \"Leather Commodity\")",
			"AddTag(context, \"Leather Panel\", \"Leather Commodity\")",
			"AddTag(context, \"Hardened Leather Panel\", \"Leather Commodity\")",
			"AddTag(context, \"Leather Scale\", \"Leather Commodity\")",
			"AddTag(context, \"Sealed Leather Panel\", \"Leather Commodity\")",
			"AddTag(context, \"Shoe Last\", \"Leatherworking Tools\")",
			"AddTag(context, \"Leather Wax Pot\", \"Leatherworking Tools\")"
		})
		{
			AssertContains(tagSource, expected);
		}

		foreach (var expected in new[]
		{
			"Functions / Material Functions / Leather Commodity / Prepared Hide",
			"Functions / Material Functions / Leather Commodity / Tanned Leather",
			"Functions / Material Functions / Leather Commodity / Leather Sole",
			"Functions / Material Functions / Leather Commodity / Leather Strap",
			"Functions / Material Functions / Leather Commodity / Leather Thong",
			"Functions / Material Functions / Leather Commodity / Leather Panel",
			"Functions / Material Functions / Leather Commodity / Hardened Leather Panel",
			"Functions / Material Functions / Leather Commodity / Leather Scale",
			"Functions / Material Functions / Leather Commodity / Sealed Leather Panel",
			"Functions / Tools / Leatherworking Tools / Shoe Last",
			"Functions / Tools / Leatherworking Tools / Leather Wax Pot"
		})
		{
			AssertContains(hierarchySource, expected);
		}

		foreach (var expected in new[]
		{
			"antiquity_bronze_awl_punch",
			"antiquity_oak_stitching_pony",
			"antiquity_bronze_edge_beveller",
			"antiquity_bronze_leather_gouge",
			"antiquity_bronze_leather_creaser",
			"antiquity_oak_shoe_last",
			"antiquity_bronze_leather_wax_pot",
			"antiquity_bronze_hide_scraper",
			"antiquity_oak_tanning_beam",
			"antiquity_oak_tanning_rack",
			"antiquity_oak_tanning_paddle",
			"antiquity_bronze_dehairing_knife",
			"antiquity_oak_brain_tanning_bucket"
		})
		{
			AssertContains(itemSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityCraftVisibleStrings_DoNotUseCultureNamesOrRepairLayer()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var bannedTerms = new[]
		{
			"Hellenic",
			"Egyptian",
			"Kushite",
			"Nubian",
			"Roman",
			"Celtic",
			"Germanic",
			"Punic",
			"Phoenician",
			"Persian",
			"Median",
			"Etruscan",
			"Anatolian",
			"Phrygian",
			"Scythian",
			"Sarmatian"
		};

		var offendingLines = craftSource
			.Split(["\r\n", "\n"], StringSplitOptions.None)
			.Select((Line, Index) => (Line, LineNumber: Index + 1))
			.Where(x => x.Line.Contains('"'))
			.Where(x => bannedTerms.Any(term => x.Line.Contains(term, StringComparison.Ordinal)))
			.Where(x => !IsAllowedCultureMetadataLine(x.Line))
			.ToList();

		Assert.AreEqual(
			0,
			offendingLines.Count,
			"Culture-name terms should only appear in knowledge metadata. Offending lines: " +
			string.Join("; ", offendingLines.Select(x => $"{x.LineNumber}: {x.Line.Trim()}")));

		foreach (var banned in new[]
		{
			"RepairAntiquityCraftVisibleText",
			"AddAntiquityCraftWithVisibleTextRepair",
			"LegacyName",
			"legacyNames"
		})
		{
			Assert.IsFalse(craftSource.Contains(banned, StringComparison.Ordinal), $"Base data should be corrected directly, without {banned}.");
		}
	}

	[TestMethod]
	public void AntiquityDesignDocument_DocumentsFullLeatherSlice()
	{
		var designSource = ReadSource("Design Documents", "Antiquity_Hellenic_Clothing_Crafting_Suite.md");

		foreach (var expected in new[]
		{
			"## Full Leather Item Suite",
			"Ancient Hide and Leatherworking",
			"`Leathermaking`",
			"`Leatherworking`",
			"`Leather Commodity`",
			"`Prepared Hide`",
			"`Tanned Leather`",
			"`Leather Sole`",
			"`Leather Strap`",
			"`Leather Thong`",
			"`Leather Panel`",
			"`Hardened Leather Panel`",
			"`Leather Scale`",
			"`Sealed Leather Panel`",
			"`Leather Wax Pot`",
			"armour, containers, liquid vessels, tableware, and doorway fittings"
		})
		{
			AssertContains(designSource, expected);
		}

		foreach (var stableReference in LeatherCraftStableReferences)
		{
			AssertContains(designSource, $"`{stableReference}`");
		}
	}

	private static IEnumerable<string> FindAntiquityLeatherMaterialItemStableReferences(string itemSource)
	{
		foreach (Match match in Regex.Matches(itemSource, "CreateItem\\(\\s*\"(?<stable>[^\"]+)\"(?<body>.*?)\\n\\s*\\);", RegexOptions.Singleline))
		{
			var material = FindMaterialArgument(match.Value);
			if (material is not null &&
			    (material.Contains("leather", StringComparison.OrdinalIgnoreCase) ||
			     material.Contains("hide", StringComparison.OrdinalIgnoreCase) ||
			     material.Contains("fur", StringComparison.OrdinalIgnoreCase)))
			{
				yield return match.Groups["stable"].Value;
			}
		}
	}

	private static string? FindMaterialArgument(string createItemBlock)
	{
		var lines = createItemBlock.Split(["\r\n", "\n"], StringSplitOptions.None);
		for (var i = 0; i < lines.Length; i++)
		{
			if (!lines[i].Trim().Equals("false,", StringComparison.Ordinal))
			{
				continue;
			}

			for (var j = i + 1; j < Math.Min(i + 5, lines.Length); j++)
			{
				var match = Regex.Match(lines[j], "^\\s*\"(?<material>[^\"]+)\",\\s*$");
				if (match.Success)
				{
					return match.Groups["material"].Value;
				}
			}
		}

		return null;
	}

	private static bool IsAllowedCultureMetadataLine(string line)
	{
		var trimmed = line.Trim();
		return line.Contains("Knowledge", StringComparison.Ordinal) ||
		       line.Contains("knowledge", StringComparison.Ordinal) ||
		       trimmed is "\"Hellenic\"," or "\"Egyptian\"," or "\"Kushite\"," or "\"Roman\"," or "\"Celtic\","
			       or "\"Germanic\"," or "\"Punic\"," or "\"Persian\"," or "\"Etruscan\"," or "\"Anatolian\","
			       or "\"Scythian-Sarmatian\",";
	}

	private static void AssertContains(string source, string expected)
	{
		Assert.IsTrue(source.Contains(expected, StringComparison.Ordinal), $"Expected source to contain: {expected}");
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts))));
	}
}
