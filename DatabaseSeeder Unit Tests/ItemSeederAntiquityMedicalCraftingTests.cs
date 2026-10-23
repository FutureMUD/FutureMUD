#nullable enable

using Recipes = MudSharp_Unit_Tests.CompiledRecipeFixtures;

using System.Linq;

using DatabaseSeeder.Seeders;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederAntiquityMedicalCraftingTests
{
	private static readonly string[] ExpectedMedicalStableReferences =
	[
		"antiquity_linen_bandage_roll",
		"antiquity_honeyed_linen_dressing",
		"antiquity_vinegar_wound_cloth",
		"antiquity_yarrow_styptic_pad",
		"antiquity_wool_compress",
		"antiquity_wooden_splint_pair",
		"antiquity_linen_arm_sling",
		"antiquity_linen_tending_kit",
		"antiquity_wound_cleaning_kit",
		"antiquity_honey_antiseptic_kit",
		"antiquity_gut_suture_spool",
		"antiquity_bone_suture_needle",
		"antiquity_suturing_kit",
		"antiquity_bronze_surgical_probe",
		"antiquity_bronze_scalpel",
		"antiquity_bronze_forceps",
		"antiquity_bronze_arterial_clamp",
		"antiquity_bronze_cautery_iron",
		"antiquity_bronze_bone_saw",
		"antiquity_willow_bark_packets",
		"antiquity_poppy_latex_draught",
		"antiquity_mandrake_draught_vial",
		"antiquity_ephedra_brew_packets",
		"antiquity_foxglove_tincture_vial",
		"antiquity_mint_infusion_bundle",
		"antiquity_honey_poultice_pot",
		"antiquity_garlic_salve_pot",
		"antiquity_aloe_burn_salve_pot",
		"antiquity_henbane_fumigation_cone",
		"antiquity_mandrake_smoke_cone",
		"antiquity_herbalist_pouch",
		"antiquity_surgical_tool_roll",
		"antiquity_willow_crutch",
		"antiquity_simple_walking_cane",
		"antiquity_padded_wooden_peg_leg",
		"antiquity_carved_prosthetic_foot",
		"antiquity_carved_prosthetic_hand",
		"antiquity_bronze_hook_hand",
		"antiquity_painted_clay_eye"
	];

	[TestMethod]
	public void AntiquityMedicalItems_AreSeededWithFunctionalComponentAndTagCoverage()
	{
		var reworkRootSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder", "ItemSeeder.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder", "ItemSeeder.AntiquityMedical.cs");

		AssertContains(reworkRootSource, "SeedAntiquityMedicalItems();");

		foreach (var stableReference in ExpectedMedicalStableReferences)
		{
			AssertContains(itemSource, $"\"{stableReference}\"");
		}

		foreach (var expected in new[]
		         {
			         "Bandage_Good",
			         "Bandage_Great",
			         "Clean_Kit",
			         "Antiseptic_Kit",
			         "Tend_Kit",
			         "Suture_Kit",
			         "Limb_Immobilising",
			         "Crutch",
			         "Prosthetic_LLowerLeg",
			         "Prosthetic_RHand_Functional",
			         "TopicalCream_Honey_Poultice",
			         "TopicalCream_Aloe_Burn_Salve",
			         "Pill_Willow_Bark_Tea",
			         "Pill_Poppy_Latex_Draught",
			         "Smokeable_Henbane_Smoke"
		         })
		{
			AssertContains(itemSource, expected);
		}

		foreach (var expected in new[]
		         {
			         "Functions / Medical Treatment / Bandage",
			         "Functions / Medical Treatment / Herbal Remedy",
			         "Functions / Medical Treatment / Prosthetic",
			         "Functions / Medical Treatment / Mobility Aid",
			         "Market / Medicine / Herbal Medicine",
			         "Market / Medicine / Treatment Supplies",
			         "Market / Medicine / Surgical Supplies",
			         "Market / Medicine / Prosthetics and Mobility"
		         })
		{
			AssertContains(itemSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityMedicalCrafts_RegisterKnowledgeGatedSuite()
	{
		foreach (var knowledge in new[] { "Ancient Medical Treatment Supplies", "Ancient Herbal Remedies", "Ancient Surgical Instruments", "Ancient Prosthetics and Mobility Aids" }) Recipes.Knowledge(knowledge);
	}

	[TestMethod]
	public void AntiquityMedicalCrafts_AddUpstreamCommodityAndTagSurface()
	{
		Recipes.RequiresTags(ExpectedMedicalStockTags().ToArray());
	}

	[TestMethod]
	public void AntiquityMedicalCrafts_CoverEveryMedicalPrototypeAndVisibleTextIsCultureNeutral()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder", "ItemSeeder.Crafting.AntiquityMedical.cs");

		foreach (var stableReference in ExpectedMedicalStableReferences)
		{
			AssertContains(craftSource, $"\"{stableReference}\"");
		}

		foreach (var banned in new[]
		         {
			         "roman", "hellenic", "egyptian", "celtic", "germanic", "kushite", "punic", "persian",
			         "etruscan", "anatolian", "scythian", "sarmatian"
		         })
		{
			Assert.IsFalse(craftSource.Contains(banned, StringComparison.OrdinalIgnoreCase),
				$"Visible medical craft construction should not contain culture term {banned}.");
		}
	}

	[TestMethod]
	public void HealthAndTerrainSeeders_AddAntiquityMedicalRemediesAndForageYieldTieIns()
	{
		var healthSource = ReadSource("DatabaseSeeder", "Seeders", "HealthSeeder", "HealthSeeder.cs");
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder", "CoreDataSeeder.Materials.cs");
		var terrainSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder", "CoreDataSeeder.Terrain.cs");

		foreach (var expected in new[]
		         {
			         "Aloe Burn Salve",
			         "Poppy Latex Draught",
			         "Henbane Smoke",
			         "Yarrow Styptic",
			         "Smokeable_",
			         "DrugVector.Inhaled"
		         })
		{
			AssertContains(healthSource, expected);
		}

		foreach (var expected in new[] { "yarrow", "mandrake", "henbane", "foxglove", "ephedra", "poppy latex", "gut" })
		{
			AssertContains(materialSource, $"\"{expected}\"");
		}

		foreach (var expected in new[]
		         {
			         "medicinal-herbs",
			         "willow-bark",
			         "foxglove-leaves",
			         "mandrake-root",
			         "aloe-leaves",
			         "ephedra-stems",
			         "aromatic-resins"
		         })
		{
			AssertContains(terrainSource, expected);
		}
	}

	private static IEnumerable<string> ExpectedMedicalStockTags()
	{
		return
		[
			"Dressing Stock",
			"Poultice Stock",
			"Salve Stock",
			"Decoction Stock",
			"Fumigation Stock",
			"Suture Stock",
			"Splint Stock",
			"Prosthetic Stock",
			"Surgical Tool Blank",
			"Herbal Remedy Stock"
		];
	}

	private static IEnumerable<string> ExpectedMedicalTreatmentTags()
	{
		return
		[
			"Bandage",
			"Splint",
			"Tend Kit",
			"Wound Cleaning",
			"Antiseptic Dressing",
			"Suture Kit",
			"Prosthetic",
			"Mobility Aid",
			"Herbal Remedy"
		];
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
