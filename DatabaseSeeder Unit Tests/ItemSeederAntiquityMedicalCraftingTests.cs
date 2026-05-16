#nullable enable

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
		var reworkRootSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityMedical.cs");

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
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityMedical.cs");

		AssertContains(craftRoot, "SeedAntiquityMedicalCrafts();");

		foreach (var expected in new[]
		         {
			         "private void SeedAntiquityMedicalCrafts()",
			         "SeedAntiquityMedicalIntermediateCommodityCrafts();",
			         "AddAntiquityCraft(",
			         "Ancient Medical Treatment Supplies",
			         "Ancient Herbal Remedies",
			         "Ancient Surgical Instruments",
			         "Ancient Prosthetics and Mobility Aids",
			         "knowledgeSubtype:",
			         "knowledgeDescription:"
		         })
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityMedicalCrafts_AddUpstreamCommodityAndTagSurface()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityMedical.cs");
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var tagHierarchy = ReadSource("Design Documents", "SeededTagHierarchy.csv");

		foreach (var expected in ExpectedMedicalStockTags())
		{
			AssertContains(craftSource, expected);
			AssertContains(tagSource, $"AddTag(context, \"{expected}\",");
			AssertContains(tagHierarchy, $"Medical Craft Stock / {expected}");
		}

		foreach (var expected in ExpectedMedicalTreatmentTags())
		{
			AssertContains(tagSource, $"AddTag(context, \"{expected}\",");
			AssertContains(tagHierarchy, $"Medical Treatment / {expected}");
		}

		foreach (var expected in new[]
		         {
			         "Medicine Strainer",
			         "Ointment Spatula",
			         "Cupping Vessel",
			         "Surgical Probe",
			         "Cautery Iron",
			         "Herbal Medicine",
			         "Treatment Supplies",
			         "Surgical Supplies",
			         "Prosthetics and Mobility"
		         })
		{
			AssertContains(tagSource, $"AddTag(context, \"{expected}\",");
			AssertContains(tagHierarchy, expected);
		}
	}

	[TestMethod]
	public void AntiquityMedicalCrafts_CoverEveryMedicalPrototypeAndVisibleTextIsCultureNeutral()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityMedical.cs");

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
		var healthSource = ReadSource("DatabaseSeeder", "Seeders", "HealthSeeder.cs");
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");
		var terrainSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Terrain.cs");

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
