#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederRomanClothingCraftingTests
{
	private static readonly string[] RomanClothingStableReferences =
	[
		"antiquity_knee_length_wool_tunica",
		"antiquity_wool_travel_mantle",
		"antiquity_fine_linen_tunica",
		"antiquity_wool_toga",
		"antiquity_long_wool_tunica",
		"antiquity_wool_palla",
		"antiquity_fine_long_linen_tunica",
		"antiquity_wool_stola",
		"antiquity_fine_wool_palla"
	];

	[TestMethod]
	public void RomanClothingCrafts_ProduceEveryCurrentRomanClothingPrototype()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Roman.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");

		foreach (var stableReference in RomanClothingStableReferences)
		{
			AssertContains(itemSource, $"\"{stableReference}\"");
			AssertContains(craftSource, $"StableReference: \"{stableReference}\"");
		}

		AssertContains(craftSource, "StableVariableProduct(garment.StableReference, garment.Fine)");
	}

	[TestMethod]
	public void RomanFinishedGarmentCrafts_UseRomanKnowledgeGate()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Roman.cs");
		var itemCraftingSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");

		AssertContains(craftSource, "const string romanKnowledge = \"Roman Textilecraft\"");
		AssertContains(craftSource, "knowledgeSubtype: \"Roman\"");
		AssertContains(craftSource, "AddRomanGarmentCraft(");
		AssertContains(craftSource, "romanKnowledge,");
		AssertContains(itemCraftingSource, "SeedAntiquityRomanClothingCrafts();");
	}

	[TestMethod]
	public void RomanGarmentCrafts_UseSharedTextileCommoditiesAndCopyColours()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Roman.cs");

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
	public void AntiquityClothingDesignDocument_DocumentsRomanCraftingSlice()
	{
		var designSource = ReadSource("Design Documents", "Antiquity_Hellenic_Clothing_Crafting_Suite.md");

		AssertContains(designSource, "Roman Textilecraft");
		AssertContains(designSource, "## Roman Garment Matrix");
		AssertContains(designSource, "antiquity_wool_toga");
		AssertContains(designSource, "antiquity_wool_stola");
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
