#nullable enable

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
		"antiquity_light_linen_head_veil"
	];

	[TestMethod]
	public void HellenicClothingCrafts_ProduceEveryCurrentHellenicClothingPrototype()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");

		foreach (var stableReference in HellenicClothingStableReferences)
		{
			AssertContains(itemSource, $"\"{stableReference}\"");
			AssertContains(craftSource, $"StableReference: \"{stableReference}\"");
		}

		AssertContains(craftSource, "StableVariableProduct(garment.StableReference, garment.Fine)");
		AssertContains(craftSource, "SimpleVariableProduct - 1x");
	}

	[TestMethod]
	public void HellenicFinishedGarmentCrafts_UseHellenicKnowledgeGate()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");

		AssertContains(craftSource, "const string hellenicKnowledge = \"Hellenic Textilecraft\"");
		AssertContains(craftSource, "knowledgeSubtype: \"Hellenic\"");
		AssertContains(craftSource, "AddHellenicGarmentCraft(");
		AssertContains(craftSource, "hellenicKnowledge,");
	}

	[TestMethod]
	public void UpstreamTextileCrafts_UseCommodityStateTagsAndPreserveColours()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs");

		foreach (var expected in new[]
		{
			"tag Raw Textile Fibre",
			"tag Prepared Textile Fibre",
			"tag Spun Yarn",
			"tag Woven Cloth",
			"tag Dyed Cloth",
			"tag Fulled Cloth",
			"piletag Garment Cloth",
			"characteristic Colour from $i1",
			"characteristic Fine Colour from $i1",
			"characteristic Colour={basicColour}",
			"\"red\", \"madder red\"",
			"piletag Textile Dye Stock",
			"deep indigo"
		})
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void UsefulTags_DefineTextileCommodityHierarchyAndMissingTools()
	{
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");

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
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");

		foreach (var expected in new[]
		{
			"antiquity_bronze_textile_shears",
			"antiquity_bronze_sewing_needle",
			"antiquity_retting_trough",
			"antiquity_flax_break",
			"antiquity_fibre_hackle",
			"antiquity_drop_spindle",
			"antiquity_distaff",
			"antiquity_warp_weighted_loom",
			"antiquity_clay_loom_weight",
			"antiquity_weaving_shuttle",
			"antiquity_weavers_sword",
			"antiquity_dye_vat",
			"antiquity_mordant_cauldron",
			"antiquity_fullers_trough",
			"antiquity_fullers_mallet",
			"antiquity_tenter_frame"
		})
		{
			AssertContains(itemSource, expected);
		}
	}

	[TestMethod]
	public void MaterialSeeder_AddsHistoricalDyeInputs()
	{
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");

		foreach (var expected in new[] { "madder root", "indigo dye cake", "ochre pigment", "alum mordant" })
		{
			AssertContains(materialSource, expected);
		}

		AssertContains(materialSource, "AddTag(\"Textile Dye\", \"Materials\")");
		AssertContains(materialSource, "AddTag(\"Textile Mordant\", \"Materials\")");
	}

	[TestMethod]
	public void ItemSeeder_InitialisesFullPathTagLookupForReworkItems()
	{
		var itemSeederSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.cs");

		AssertContains(itemSeederSource, "BuildTagFullPath");
		AssertContains(itemSeederSource, "_tagsByFullPath = tagList.ToDictionary");
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
