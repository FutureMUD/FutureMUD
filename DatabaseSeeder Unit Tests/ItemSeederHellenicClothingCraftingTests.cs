#nullable enable

using Recipes = MudSharp_Unit_Tests.CompiledRecipeFixtures;

using DatabaseSeeder.Seeders;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederHellenicClothingCraftingTests
{
	private static readonly string[] HellenicClothingStableReferences =
	[
		"antiquity_short_wool_chiton",
		"antiquity_wool_himation",
		"antiquity_fine_linen_chiton",
		"antiquity_fine_wool_himation",
		"antiquity_short_wool_chlamys",
		"antiquity_full_length_wool_peplos",
		"antiquity_full_wool_himation",
		"antiquity_fine_long_linen_chiton",
		"antiquity_fine_full_wool_himation",
		"antiquity_light_linen_head_veil",
		"antiquity_wool_exomis",
		"antiquity_heavy_wool_zeira",
		"antiquity_fine_ritual_wool_peplos",
		"antiquity_wool_travel_leg_wraps",
		"antiquity_wool_hand_wraps_hellenic"
	];

	[TestMethod]
	public void HellenicClothingCrafts_ProduceEveryCurrentHellenicClothingPrototype()
	{
		Recipes.ItemsAreCrafted(HellenicClothingStableReferences);
	}

	[TestMethod]
	public void HellenicFinishedGarmentCrafts_UseHellenicKnowledgeGate()
	{
		Recipes.Knowledge("Hellenic Textilecraft", HellenicClothingStableReferences);
	}

	[TestMethod]
	public void UpstreamTextileCrafts_UseCommodityStateTagsAndPreserveColours()
	{
		Recipes.RequiresTags("Garment Cloth", "Spun Yarn");
		Recipes.CopiesColour(HellenicClothingStableReferences);
	}

	[TestMethod]
	public void UsefulTags_DefineTextileCommodityHierarchyAndMissingTools()
	{
		var tagSource = ReadSource("DatabaseSeeder", "Assets", "Manifests", "CoreTags.tsv");

		foreach (var expected in new[]
		{
			"AddTag(context, \"Textile Commodity\", \"Material Functions\")",
			"AddTag(context, \"Raw Textile Fibre\", \"Textile Commodity\")",
			"AddTag(context, \"Prepared Textile Fibre\", \"Textile Commodity\")",
			"AddTag(context, \"Spun Yarn\", \"Textile Commodity\")",
			"AddTag(context, \"Garment Cloth\", \"Textile Commodity\")",
			"AddTag(context, \"Woven Cloth\", \"Garment Cloth\")",
			"AddTag(context, \"Dyed Cloth\", \"Garment Cloth\")",
			"AddTag(context, \"Fulled Cloth\", \"Garment Cloth\")",
			"AddTag(context, \"Textile Dye Stock\", \"Textile Commodity\")",
			"AddTag(context, \"Retting Trough\", \"Flax Processing Tools\")",
			"AddTag(context, \"Flax Break\", \"Flax Processing Tools\")",
			"AddTag(context, \"Hackle\", \"Flax Processing Tools\")",
			"AddTag(context, \"Fibre Comb\", \"Flax Processing Tools\")"
		})
		{
			AssertContains(tagSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityReworkItems_AddPeriodTextileToolPrototypes()
	{
		foreach (var key in new[] { "antiquity_bronze_textile_shears", "antiquity_bronze_sewing_needle", "antiquity_bronze_awl_punch" })
		{
			var item = Recipes.Item(key); Assert.AreEqual("bronze", item.Material);
			Assert.IsTrue(item.Components.Contains("Holdable")); Assert.IsTrue(item.Tags.Any(tag => tag.StartsWith("Functions /", StringComparison.Ordinal)));
		}
	}

	[TestMethod]
	public void MaterialSeeder_AddsHistoricalDyeInputs()
	{
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder", "CoreDataSeeder.Materials.cs");

		foreach (var expected in new[] { "madder root", "indigo dye cake", "ochre pigment", "alum mordant" })
		{
			AssertContains(materialSource, expected);
		}

		AssertContains(ReadSource("DatabaseSeeder", "Assets", "Manifests", "CoreTags.tsv"), "AddTag(context, \"Textile Dye\", \"Materials\")");
		AssertContains(ReadSource("DatabaseSeeder", "Assets", "Manifests", "CoreTags.tsv"), "AddTag(context, \"Textile Mordant\", \"Materials\")");
	}

	[TestMethod]
	public void ItemSeeder_InitialisesFullPathTagLookupForReworkItems()
	{
		var itemSeederSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder", "ItemSeeder.cs");

		AssertContains(itemSeederSource, "BuildTagFullPath");
		AssertContains(itemSeederSource, "_tagsByFullPath = tagList");
		AssertContains(itemSeederSource, ".GroupBy(BuildTagFullPath, StringComparer.OrdinalIgnoreCase)");
		AssertContains(itemSeederSource, ".ToDictionary(x => x.Key, x => x.OrderBy(tag => tag.Id).First(), StringComparer.OrdinalIgnoreCase)");
		Assert.IsFalse(itemSeederSource.Contains("TODO - initialised _tagsByFullPath", StringComparison.OrdinalIgnoreCase));
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
