#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederNorthernAndKushiteClothingCraftingTests
{
	private static readonly string[] CelticClothingStableReferences =
	[
		"antiquity_sleeved_common_wool_tunic",
		"antiquity_wool_braccae",
		"antiquity_rectangular_wool_cloak",
		"antiquity_fine_bordered_wool_tunic",
		"antiquity_fine_wool_braccae",
		"antiquity_fine_checked_wool_cloak",
		"antiquity_long_sleeved_wool_tunic",
		"antiquity_wool_wrap_skirt",
		"antiquity_broad_wool_mantle",
		"antiquity_fine_sleeved_wool_gown",
		"antiquity_fine_bordered_wool_mantle",
		"antiquity_linen_shoulder_veil"
	];

	private static readonly string[] GermanicClothingStableReferences =
	[
		"antiquity_straight_wool_tunic",
		"antiquity_narrow_wool_trousers",
		"antiquity_heavy_wool_cloak",
		"antiquity_fine_banded_wool_tunic",
		"antiquity_fine_tapered_wool_trousers",
		"antiquity_fur_lined_wool_cloak",
		"antiquity_long_straight_wool_tunic",
		"antiquity_overlapping_wool_skirt",
		"antiquity_checked_wool_scarf",
		"antiquity_woolly_skin_cape",
		"antiquity_fine_long_wool_gown",
		"antiquity_fine_heavy_wool_mantle",
		"antiquity_linen_head_veil"
	];

	private static readonly string[] KushiteClothingStableReferences =
	[
		"adjacent_antiquity_narrow_linen_kilt",
		"adjacent_antiquity_linen_shoulder_cloth",
		"adjacent_antiquity_cotton_wrap_skirt",
		"adjacent_antiquity_sleeveless_linen_tunic",
		"adjacent_antiquity_fringed_linen_robe",
		"adjacent_antiquity_tasseled_linen_shawl",
		"adjacent_antiquity_tall_linen_headdress",
		"adjacent_antiquity_beaded_linen_girdle",
		"adjacent_antiquity_cotton_draped_dress",
		"adjacent_antiquity_linen_bead_apron",
		"adjacent_antiquity_plain_cotton_headcloth"
	];

	[TestMethod]
	public void NorthernAndKushiteClothingCrafts_ProduceEveryCurrentGarmentPrototype()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");
		var itemCraftingSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");

		foreach (var stableReference in CelticClothingStableReferences)
		{
			AssertContains(itemSource, $"\"{stableReference}\"");
			AssertContains(craftSource, $"\"{stableReference}\"");
		}

		foreach (var stableReference in GermanicClothingStableReferences)
		{
			AssertContains(itemSource, $"\"{stableReference}\"");
			AssertContains(craftSource, $"\"{stableReference}\"");
		}

		foreach (var stableReference in KushiteClothingStableReferences)
		{
			AssertContains(itemSource, $"\"{stableReference}\"");
			AssertContains(craftSource, $"\"{stableReference}\"");
		}

		AssertContains(itemCraftingSource, "SeedAntiquityCelticClothingCrafts();");
		AssertContains(itemCraftingSource, "SeedAntiquityGermanicClothingCrafts();");
		AssertContains(itemCraftingSource, "SeedAntiquityKushiteClothingCrafts();");
	}

	[TestMethod]
	public void FinishedGarmentCrafts_UseCultureKnowledgeGates()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");

		foreach (var expected in new[]
		{
			"const string celticKnowledge = \"Celtic Textilecraft\"",
			"knowledgeSubtype: \"Celtic\"",
			"const string germanicKnowledge = \"Germanic Textilecraft\"",
			"knowledgeSubtype: \"Germanic\"",
			"const string kushiteKnowledge = \"Kushite Textilecraft\"",
			"knowledgeSubtype: \"Kushite\""
		})
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void SharedUpstreamTextileCrafts_AddCottonCommodityPath()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");

		foreach (var expected in new[]
		{
			"sort cotton bolls for ginning",
			"clean and comb cotton fibre",
			"spin cotton yarn on a drop spindle",
			"weave cotton garment cloth on a warp-weighted loom",
			"Commodity - 1 kilogram 200 grams of cotton crop",
			"CommodityProduct - 600 grams of cotton commodity; tag Prepared Textile Fibre; characteristic Colour=white; characteristic Fine Colour=bone white",
			"CommodityProduct - 400 grams of cotton commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1",
			"CommodityProduct - 380 grams of cotton commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1",
			"dye cotton cloth madder red",
			"dye cotton cloth indigo blue"
		})
		{
			AssertContains(craftSource, expected);
		}

		AssertContains(materialSource, "AddMaterial(\"cotton\"");
		AssertContains(materialSource, "\"cotton crop\"");
	}

	[TestMethod]
	public void SpecialNorthernGarments_UseCommodityInputsAndStableSimpleProducts()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var helperSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");

		AssertContains(helperSource, "private string StableSimpleProduct(string stableReference)");
		AssertContains(craftSource, "StableSimpleProduct(garment.StableReference)");
		AssertContains(craftSource, "StableSimpleProduct(\"antiquity_woolly_skin_cape\")");
		AssertContains(craftSource, "CommodityTag - 300 grams of a material tagged as Hair");
		AssertContains(craftSource, "CommodityTag - 1 kilogram 200 grams of a material tagged as Animal Skin");
		AssertContains(craftSource, "piletag Garment Cloth");
		AssertContains(craftSource, "piletag Spun Yarn");
	}

	[TestMethod]
	public void AntiquityClothingDesignDocument_DocumentsNewCultureSlices()
	{
		var designSource = ReadSource("Design Documents", "Crafting", "Antiquity_Hellenic_Clothing_Crafting_Suite.md");

		foreach (var expected in new[]
		{
			"Celtic Textilecraft",
			"Germanic Textilecraft",
			"Kushite Textilecraft",
			"## Celtic Garment Matrix",
			"## Germanic Garment Matrix",
			"## Kushite Garment Matrix",
			"Sort cotton bolls for ginning",
			"antiquity_fine_checked_wool_cloak",
			"antiquity_woolly_skin_cape",
			"adjacent_antiquity_cotton_draped_dress"
		})
		{
			AssertContains(designSource, expected);
		}
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
