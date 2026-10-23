#nullable enable

using Recipes = MudSharp_Unit_Tests.CompiledRecipeFixtures;

using System.Linq;

using DatabaseSeeder.Seeders;

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
		"adjacent_antiquity_linen_bead_apron",
		"adjacent_antiquity_linen_scribe_kilt",
		"adjacent_antiquity_temple_linen_shift"
	];

	[TestMethod]
	public void EgyptianClothingCrafts_ProduceEveryCurrentEgyptianClothingPrototype()
	{
		Recipes.ItemsAreCrafted(EgyptianClothingStableReferences);
	}

	[TestMethod]
	public void EgyptianFinishedGarmentCrafts_UseEgyptianKnowledgeGate()
	{
		Recipes.Knowledge("Egyptian Textilecraft", EgyptianClothingStableReferences);
	}

	[TestMethod]
	public void EgyptianBeadedGarments_UseCommodityBeadStockAndPeriodNeedle()
	{
		var keys = new[] { "adjacent_antiquity_beaded_linen_girdle", "adjacent_antiquity_linen_bead_apron" };
		Recipes.ItemsAreCrafted(keys);
		foreach (var key in keys) Assert.IsTrue(Recipes.ForItem(key).Any(recipe => recipe.Tools.OfType<CatalogueTagRecipeTool>().Any(tool => tool.Tag == "Beading Needle")), key);
		Assert.IsTrue(Recipes.Items.Any(item => item.Tags.Contains("Functions / Tools / Textilecraft Tools / Beading Needle")));
	}

	[TestMethod]
	public void EgyptianGarmentCrafts_UseSharedTextileCommoditiesAndCopyColours()
	{
		Recipes.RequiresTags("Garment Cloth", "Spun Yarn");
		Recipes.CopiesColour(EgyptianClothingStableReferences);
	}

	[TestMethod]
	public void EgyptianSpottedHideMantle_UsesExistingTempleTextilecraftInputs()
	{
		const string key = "antiquity_egyptian_spotted_hide_priest_mantle";
		Recipes.ItemsAreCrafted([key]);
		Recipes.Knowledge("Egyptian Textilecraft", [key]);
		Assert.IsTrue(Recipes.ForItem(key).Any(recipe => recipe.Inputs.Any(input => input is CatalogueCommodityRecipeInput || input is CatalogueCommodityTagRecipeInput)));
	}

	[TestMethod]
	public void AntiquityClothingDesignDocument_DocumentsEgyptianCraftingSlice()
	{
		var designSource = ReadSource("Design Documents", "Seeding", "Antiquity_Hellenic_Clothing_Crafting_Suite.md");

		AssertContains(designSource, "Egyptian Textilecraft");
		AssertContains(designSource, "## Egyptian Garment Matrix");
		AssertContains(designSource, "adjacent_antiquity_narrow_linen_kilt");
		AssertContains(designSource, "Bead Stock");
		AssertContains(designSource, "antiquity_egyptian_spotted_hide_priest_mantle");
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
