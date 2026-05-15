#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederEgyptianClothingCraftingTests
{
	private static readonly string[] EgyptianClothingStableReferences =
	[
		"adjacent_antiquity_narrow_linen_kilt",
		"adjacent_antiquity_linen_shoulder_cloth",
		"adjacent_antiquity_sleeveless_linen_tunic",
		"adjacent_antiquity_fringed_linen_robe",
		"adjacent_antiquity_tasseled_linen_shawl",
		"adjacent_antiquity_tall_linen_headdress",
		"adjacent_antiquity_beaded_linen_girdle",
		"adjacent_antiquity_linen_bead_apron"
	];

	[TestMethod]
	public void EgyptianClothingCrafts_ProduceEveryCurrentEgyptianClothingPrototype()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");

		foreach (var stableReference in EgyptianClothingStableReferences)
		{
			AssertContains(itemSource, $"\"{stableReference}\"");
			AssertContains(craftSource, $"StableReference: \"{stableReference}\"");
		}

		AssertContains(craftSource, "StableVariableProduct(garment.StableReference, garment.Fine)");
	}

	[TestMethod]
	public void EgyptianFinishedGarmentCrafts_UseEgyptianKnowledgeGate()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var itemCraftingSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");

		AssertContains(craftSource, "const string egyptianKnowledge = \"Egyptian Textilecraft\"");
		AssertContains(craftSource, "knowledgeSubtype: \"Egyptian\"");
		AssertContains(craftSource, "AddEgyptianGarmentCraft(");
		AssertContains(craftSource, "egyptianKnowledge,");
		AssertContains(itemCraftingSource, "SeedAntiquityEgyptianClothingCrafts();");
	}

	[TestMethod]
	public void EgyptianBeadedGarments_UseCommodityBeadStockAndPeriodNeedle()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");

		AssertContains(tagSource, "AddTag(context, \"Bead Stock\", \"Material Functions\")");
		AssertContains(itemSource, "Functions / Tools / Textilecraft Tools / Beading Needle");
		AssertContains(craftSource, "\"sort glass beads for textile edging\"");
		AssertContains(craftSource, "CommodityProduct - 110 grams of glass commodity; tag Bead Stock");
		AssertContains(craftSource, "piletag Bead Stock");
		AssertContains(craftSource, "TagTool - Held - an item with the Beading Needle tag");
	}

	[TestMethod]
	public void EgyptianGarmentCrafts_UseSharedTextileCommoditiesAndCopyColours()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");

		foreach (var expected in new[]
		{
			"piletag Garment Cloth",
			"piletag Spun Yarn",
			"characteristic Colour any",
			"characteristic Fine Colour any",
			"StableVariableProduct(garment.StableReference, garment.Fine)"
		})
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityClothingDesignDocument_DocumentsEgyptianCraftingSlice()
	{
		var designSource = ReadSource("Design Documents", "Antiquity_Hellenic_Clothing_Crafting_Suite.md");

		AssertContains(designSource, "Egyptian Textilecraft");
		AssertContains(designSource, "## Egyptian Garment Matrix");
		AssertContains(designSource, "adjacent_antiquity_narrow_linen_kilt");
		AssertContains(designSource, "Bead Stock");
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
