#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederRemainingAntiquityClothingCraftingTests
{
	private static readonly string[] PunicClothingStableReferences =
	[
		"antiquity_short_fitted_linen_tunic",
		"antiquity_patterned_linen_waistcloth",
		"antiquity_short_sleeved_linen_overblouse",
		"antiquity_long_linen_inner_robe",
		"antiquity_one_shoulder_wool_mantle",
		"antiquity_long_folded_linen_robe",
		"antiquity_loose_linen_hood",
		"antiquity_fine_full_linen_gown",
		"antiquity_left_shoulder_overdrape",
		"antiquity_star_bordered_linen_robe"
	];

	private static readonly string[] PersianClothingStableReferences =
	[
		"antiquity_sarapis_wool_tunic",
		"antiquity_fine_sarapis_linen_tunic",
		"antiquity_anaxyrides_wool_trousers",
		"antiquity_fine_patterned_anaxyrides",
		"antiquity_wool_kandys",
		"antiquity_fine_wool_kandys",
		"antiquity_pleated_court_robe",
		"antiquity_fine_pleated_court_gown",
		"antiquity_wide_cloth_belt",
		"antiquity_fine_wide_cloth_belt",
		"antiquity_full_head_and_neck_veil"
	];

	private static readonly string[] EtruscanClothingStableReferences =
	[
		"adjacent_antiquity_short_sleeved_linen_tunic",
		"adjacent_antiquity_bordered_wool_tunic",
		"adjacent_antiquity_curved_tebenna",
		"adjacent_antiquity_fine_curved_tebenna",
		"adjacent_antiquity_wrapped_linen_skirt",
		"adjacent_antiquity_rectangular_shoulder_cloak",
		"adjacent_antiquity_fitted_linen_gown",
		"adjacent_antiquity_linen_head_mantle"
	];

	private static readonly string[] AnatolianClothingStableReferences =
	[
		"adjacent_antiquity_belted_wool_tunic",
		"adjacent_antiquity_fine_banded_tunic",
		"adjacent_antiquity_banded_leg_wraps",
		"adjacent_antiquity_hooded_wool_cloak",
		"adjacent_antiquity_forward_pointing_felt_cap",
		"adjacent_antiquity_short_wool_cape",
		"adjacent_antiquity_fine_patterned_wool_robe",
		"adjacent_antiquity_fringed_wool_mantle",
		"adjacent_antiquity_wool_wrapped_skirt",
		"adjacent_antiquity_fine_rectangular_veil"
	];

	private static readonly string[] ScythianSarmatianClothingStableReferences =
	[
		"adjacent_antiquity_felt_riding_cap",
		"adjacent_antiquity_tall_felt_cap",
		"adjacent_antiquity_riding_tunic",
		"adjacent_antiquity_wool_riding_trousers",
		"adjacent_antiquity_patterned_riding_trousers",
		"adjacent_antiquity_open_riding_caftan",
		"adjacent_antiquity_fur_trimmed_caftan",
		"adjacent_antiquity_split_riding_skirt",
		"adjacent_antiquity_long_felt_coat"
	];

	[TestMethod]
	public void RemainingAntiquityClothingCrafts_ProduceEveryCurrentCultureGarmentPrototype()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.RemainingAntiquity.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");
		var itemCraftingSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");

		AssertReferencesPresent(PunicClothingStableReferences, itemSource, craftSource);
		AssertReferencesPresent(PersianClothingStableReferences, itemSource, craftSource);
		AssertReferencesPresent(EtruscanClothingStableReferences, itemSource, craftSource);
		AssertReferencesPresent(AnatolianClothingStableReferences, itemSource, craftSource);
		AssertReferencesPresent(ScythianSarmatianClothingStableReferences, itemSource, craftSource);

		foreach (var expected in new[]
		{
			"SeedAntiquityPunicClothingCrafts();",
			"SeedAntiquityPersianClothingCrafts();",
			"SeedAntiquityEtruscanClothingCrafts();",
			"SeedAntiquityAnatolianClothingCrafts();",
			"SeedAntiquityScythianSarmatianClothingCrafts();"
		})
		{
			AssertContains(itemCraftingSource, expected);
		}
	}

	[TestMethod]
	public void RemainingAntiquityFinishedGarmentCrafts_UseCultureKnowledgeGates()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.RemainingAntiquity.cs");

		foreach (var expected in new[]
		{
			"const string punicKnowledge = \"Punic Textilecraft\"",
			"\"Punic\"",
			"const string persianKnowledge = \"Persian Textilecraft\"",
			"\"Persian\"",
			"const string etruscanKnowledge = \"Etruscan Textilecraft\"",
			"\"Etruscan\"",
			"const string anatolianKnowledge = \"Anatolian Textilecraft\"",
			"\"Anatolian\"",
			"const string scythianKnowledge = \"Scythian-Sarmatian Textilecraft\"",
			"\"Scythian-Sarmatian\""
		})
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void SharedUpstreamTextileCrafts_AddFeltCommodityPath()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Hellenic.cs");
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");

		foreach (var expected in new[]
		{
			"felt prepared wool fibre into garment felt",
			"Commodity - 700 grams of wool; piletag Prepared Textile Fibre; characteristic Colour any; characteristic Fine Colour any",
			"CommodityProduct - 520 grams of felt commodity; tag Fulled Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1",
			"CommodityProduct - 180 grams of felt commodity; tag Fulled Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"
		})
		{
			AssertContains(craftSource, expected);
		}

		AssertContains(materialSource, "AddMaterial(\"felt\"");
	}

	[TestMethod]
	public void RemainingAntiquitySpecialGarments_UseCommodityInputsAndGeneratedProducts()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.RemainingAntiquity.cs");

		foreach (var expected in new[]
		{
			"StableSimpleProduct(garment.StableReference)",
			"antiquity_fine_patterned_anaxyrides",
			"adjacent_antiquity_fine_banded_tunic",
			"CommodityTag - {garment.Hair} grams of a material tagged as Hair",
			"adjacent_antiquity_fur_trimmed_caftan",
			"piletag Garment Cloth",
			"piletag Spun Yarn"
		})
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityClothingDesignDocument_DocumentsRemainingCultureSlices()
	{
		var designSource = ReadSource("Design Documents", "Antiquity_Hellenic_Clothing_Crafting_Suite.md");

		foreach (var expected in new[]
		{
			"Punic Textilecraft",
			"Persian Textilecraft",
			"Etruscan Textilecraft",
			"Anatolian Textilecraft",
			"Scythian-Sarmatian Textilecraft",
			"## Punic Garment Matrix",
			"## Persian Garment Matrix",
			"## Etruscan Garment Matrix",
			"## Anatolian Garment Matrix",
			"## Scythian-Sarmatian Garment Matrix",
			"Felt prepared wool fibre into garment felt",
			"Leather footwear remains out of this clothing pass"
		})
		{
			AssertContains(designSource, expected);
		}
	}

	private static void AssertReferencesPresent(string[] references, string itemSource, string craftSource)
	{
		foreach (var stableReference in references)
		{
			AssertContains(itemSource, $"\"{stableReference}\"");
			AssertContains(craftSource, $"\"{stableReference}\"");
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
