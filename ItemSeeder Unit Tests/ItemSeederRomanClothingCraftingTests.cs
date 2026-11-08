#nullable enable

using Recipes = MudSharp_Unit_Tests.CompiledRecipeFixtures;

using System.Linq;

using DatabaseSeeder.Seeders;

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
		"antiquity_fine_wool_palla",
		"antiquity_hooded_wool_paenula",
		"antiquity_fine_toga_praetexta",
		"antiquity_wool_udones",
		"antiquity_wool_hand_wraps",
		"antiquity_roman_wool_focale",
		"antiquity_roman_wool_sagum",
		"antiquity_italic_white_wool_infula"
	];

	[TestMethod]
	public void RomanClothingCrafts_ProduceEveryCurrentRomanClothingPrototype()
	{
		Recipes.ItemsAreCrafted(RomanClothingStableReferences);
	}

	[TestMethod]
	public void RomanFinishedGarmentCrafts_UseRomanKnowledgeGate()
	{
		Recipes.Knowledge("Roman Textilecraft", RomanClothingStableReferences);
	}

	[TestMethod]
	public void RomanGarmentCrafts_UseSharedTextileCommoditiesAndCopyColours()
	{
		Recipes.RequiresTags("Garment Cloth", "Spun Yarn");
		Recipes.CopiesColour(RomanClothingStableReferences);
	}

	[TestMethod]
	public void AntiquityClothingDesignDocument_DocumentsRomanCraftingSlice()
	{
		var designSource = ReadSource("Design Documents", "Seeding", "Antiquity_Hellenic_Clothing_Crafting_Suite.md");

		AssertContains(designSource, "Roman Textilecraft");
		AssertContains(designSource, "## Roman Garment Matrix");
		AssertContains(designSource, "antiquity_wool_toga");
		AssertContains(designSource, "antiquity_wool_stola");
		AssertContains(designSource, "antiquity_roman_wool_focale");
		AssertContains(designSource, "antiquity_roman_wool_sagum");
		AssertContains(designSource, "antiquity_italic_white_wool_infula");
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
