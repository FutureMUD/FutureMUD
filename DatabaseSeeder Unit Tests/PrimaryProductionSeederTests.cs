#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PrimaryProductionSeederTests
{
	private static readonly (string DisplayName, string ResourceTag, string DepositTag, string UniqueName, string OutputTag)[] ExpectedResources =
	[
		("Bog Iron", "Bog Iron Resource", "Bog Iron Deposit", "primary_production_bog_iron_deposit", "Bog Iron Ore Commodity"),
		("Magnetite Sands", "Magnetite Sands Resource", "Magnetite Sands Deposit", "primary_production_magnetite_sands_deposit", "Magnetite Sand Commodity"),
		("Native Copper", "Native Copper Resource", "Native Copper Deposit", "primary_production_native_copper_deposit", "Native Copper Ore Commodity"),
		("Azurite", "Azurite Resource", "Azurite Deposit", "primary_production_azurite_deposit", "Copper Carbonate Ore Commodity"),
		("Chalcopyrite", "Chalcopyrite Resource", "Chalcopyrite Deposit", "primary_production_chalcopyrite_deposit", "Copper Sulphide Ore Commodity"),
		("Placer Gold", "Placer Gold Resource", "Placer Gold Deposit", "primary_production_placer_gold_deposit", "Placer Concentrate Commodity"),
		("Rock Salt", "Rock Salt Resource", "Rock Salt Deposit", "primary_production_rock_salt_deposit", "Rock Salt Commodity"),
		("Salt Pan", "Salt Pan Resource", "Salt Pan Deposit", "primary_production_salt_pan_deposit", "Salt Commodity"),
		("Kaolin", "Kaolin Resource", "Kaolin Deposit", "primary_production_kaolin_deposit", "Kaolin Commodity"),
		("Volcanic Ash Pozzolana", "Volcanic Ash Pozzolana Resource", "Volcanic Ash Pozzolana Deposit", "primary_production_pozzolana_deposit", "Pozzolana Commodity"),
		("Shell Lime", "Shell Lime Resource", "Shell Lime Deposit", "primary_production_shell_lime_deposit", "Shell Lime Commodity"),
		("Coral Lime", "Coral Lime Resource", "Coral Lime Deposit", "primary_production_coral_lime_deposit", "Coral Lime Commodity"),
		("Jade Greenstone", "Jade Greenstone Resource", "Jade Greenstone Deposit", "primary_production_greenstone_deposit", "Greenstone Rough Commodity"),
		("Obsidian", "Obsidian Resource", "Obsidian Deposit", "primary_production_obsidian_deposit", "Obsidian Rough Commodity"),
		("Turquoise", "Turquoise Resource", "Turquoise Deposit", "primary_production_turquoise_deposit", "Turquoise Rough Commodity"),
		("Lapis", "Lapis Resource", "Lapis Deposit", "primary_production_lapis_deposit", "Lapis Rough Commodity"),
		("Alum", "Alum Resource", "Alum Deposit", "primary_production_alum_deposit", "Alum Commodity"),
		("Saltpeter", "Saltpeter Resource", "Saltpeter Deposit", "primary_production_saltpeter_deposit", "Saltpeter Commodity"),
		("Vitriol Copperas", "Vitriol Copperas Resource", "Vitriol Copperas Deposit", "primary_production_copperas_deposit", "Copperas Commodity")
	];

	[TestMethod]
	public void PrimaryProductionSeeder_DefinesResourceTagsDepositsProjectsAndGatedExtraction()
	{
		string source = ReadSource("DatabaseSeeder", "Seeders", "PrimaryProductionSeeder.cs");
		string tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var itemSpecs = ItemSeeder.PrimaryProductionItemSpecsForTesting.ToArray();
		var projectSpecs = PrimaryProductionSeeder.PrimaryProductionProjectSpecsForTesting.ToArray();

		foreach ((string displayName, string resourceTag, string depositTag, string uniqueName, string outputTag) in ExpectedResources)
		{
			AssertContains(tagSource, $"AddTag(context, \"{resourceTag}\"");
			AssertContains(tagSource, $"AddTag(context, \"{depositTag}\"");

			var item = itemSpecs.SingleOrDefault(x => x.StableReference == uniqueName);
			Assert.IsNotNull(item, $"Missing visible deposit item {uniqueName}.");
			Assert.IsTrue(item.Tags.Any(x => x.EndsWith(resourceTag, StringComparison.Ordinal)),
				$"Deposit item {uniqueName} should carry {resourceTag}.");
			Assert.IsTrue(item.Tags.Any(x => x.EndsWith(depositTag, StringComparison.Ordinal)),
				$"Deposit item {uniqueName} should carry {depositTag}.");

			var prospect = projectSpecs.SingleOrDefault(x => x.Name == $"Stock Primary Production: Prospect for {displayName}");
			Assert.IsNotNull(prospect, $"Missing prospecting project for {displayName}.");
			CollectionAssert.Contains(prospect.RequiredLocationTags.ToArray(), resourceTag);
			CollectionAssert.Contains(prospect.OutputStableReferences.ToArray(), uniqueName);

			var extraction = projectSpecs.SingleOrDefault(x => x.Name == $"Stock Primary Production: Extract {displayName}");
			Assert.IsNotNull(extraction, $"Missing extraction project for {displayName}.");
			Assert.IsFalse(string.IsNullOrWhiteSpace(extraction.CanInitiateProgName),
				$"Extraction project for {displayName} should be resource-gated.");
			CollectionAssert.Contains(extraction.OutputCommodityTags.ToArray(), outputTag);
		}

		var survey = projectSpecs.Single(x => x.Name == "Stock Primary Production: Survey Mineral Signs");
		CollectionAssert.Contains(survey.OutputStableReferences.ToArray(), "primary_production_mineral_signs_marker");
		CollectionAssert.DoesNotContain(survey.OutputStableReferences.ToArray(), "primary_production_hematite_deposit",
			"Generic primary-production survey should not reveal hematite by default.");

		string[] stockExtractionProjects =
		[
			"Stock Primary Production: Extract Iron Ore",
			"Stock Primary Production: Extract Copper Ore",
			"Stock Primary Production: Extract Tin Ore",
			"Stock Primary Production: Extract Lead Ore",
			"Stock Primary Production: Quarry Limestone Blocks",
			"Stock Primary Production: Quarry Sandstone Blocks",
			"Stock Primary Production: Boil Brine For Salt",
			"Stock Primary Production: Collect Natron",
			"Stock Primary Production: Cut Peat Turves",
			"Stock Primary Production: Collect Ochre Earth",
			"Stock Primary Production: Collect Bitumen",
			"Stock Primary Production: Mine Coal"
		];
		foreach (string projectName in stockExtractionProjects)
		{
			var project = projectSpecs.Single(x => x.Name == projectName);
			Assert.IsFalse(string.IsNullOrWhiteSpace(project.CanInitiateProgName),
				$"{projectName} should be resource-gated rather than globally startable.");
		}

		AssertContains(source, "BuildCanInitiateProgText");
		AssertContains(source, "istagged(@ch.Location");
		AssertContains(source, "layeritems(@ch.Location, \"GroundLevel\").any");
	}

	[TestMethod]
	public void PrimaryProductionCrafts_CoverRegionalMethodologiesAndSemanticCommodityFixes()
	{
		string source = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.PrimaryProduction.cs");
		var craftSpecs = ItemSeeder.PrimaryProductionCraftSpecsForTesting.ToArray();

		string[] expectedCraftNames =
		[
			"primary production - sort and roast bog iron ore",
			"primary production - smelt bog iron bloomery charge",
			"primary production - wash magnetite iron sands",
			"primary production - smelt tatara iron sands",
			"primary production - rake solar salt pan",
			"primary production - burn shell quicklime",
			"primary production - burn coral quicklime",
			"primary production - quarry obsidian blade cores",
			"primary production - split greenstone adze blanks",
			"primary production - leach wood ash",
			"primary production - evaporate lye to potash",
			"primary production - burn barilla plants to ash",
			"primary production - burn kelp to ash",
			"primary production - wash kelp ash alkali",
			"primary production - calcine barilla ash soda",
			"primary production - prepare soda lime glass batch",
			"primary production - calcine gypsum plaster"
		];

		foreach (string craftName in expectedCraftNames)
		{
			Assert.IsTrue(craftSpecs.Any(x => x.Name == craftName), $"Missing craft {craftName}.");
		}

		var glassBatchCraft = craftSpecs.Single(x => x.Name == "primary production - prepare soda lime glass batch");
		Assert.IsTrue(glassBatchCraft.Inputs.Any(x => x.Contains("soda ash", StringComparison.Ordinal) &&
		                                             x.Contains("Soda Ash Commodity", StringComparison.Ordinal)));
		Assert.IsFalse(glassBatchCraft.Inputs.Any(x => x.Contains("Potash Commodity", StringComparison.OrdinalIgnoreCase)),
			"Soda-lime glass batch should not require potash as its soda ash input.");

		var plasterCraft = craftSpecs.Single(x => x.Name == "primary production - calcine gypsum plaster");
		Assert.IsTrue(plasterCraft.Products.Any(x => x.Contains("Plaster Commodity", StringComparison.Ordinal)));
		Assert.IsFalse(plasterCraft.Products.Any(x => x.Contains("Mortar Commodity", StringComparison.OrdinalIgnoreCase)),
			"Plaster output should not be tagged as mortar.");

		AssertContains(source, "\"Soda Ash Commodity\"");
		AssertContains(source, "\"Plaster Commodity\"");
	}

	[TestMethod]
	public void PrimaryProductionIntegration_WiresToolsCraftsMaterialsAndMetadata()
	{
		string reworkRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		string craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		string tools = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.PrimaryProductionTools.cs");
		string materials = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");
		string metadata = ReadSource("DatabaseSeeder", "SeederMetadataRegistry.cs");

		AssertContains(reworkRoot, "SeedPrimaryProductionToolsAndProps();");
		AssertContains(craftRoot, "SeedPrimaryProductionCommodityCrafts();");
		AssertContains(metadata, "nameof(PrimaryProductionSeeder)");
		AssertContains(metadata, "SeederRepeatabilityMode.Idempotent");
		AssertContains(metadata, "SeederUpdateCapability.RepairExisting");

		string[] toolReferences =
		[
			"primary_production_mineral_signs_marker",
			"primary_production_bloomery_furnace",
			"primary_production_tatara_furnace",
			"primary_production_solar_salt_pan",
			"primary_production_lime_kiln",
			"primary_production_ore_washing_trough",
			"primary_production_ash_hopper",
			"primary_production_leaching_tub",
			"primary_production_evaporating_pan",
			"primary_production_wedge_set"
		];

		foreach (string toolReference in toolReferences)
		{
			AssertContains(tools, $"\"{toolReference}\"");
		}

		foreach (var (_, _, _, uniqueName, _) in ExpectedResources)
		{
			AssertContains(tools, $"\"{uniqueName}\"");
		}

		AssertContains(materials, "AddMaterial(\"bog iron ore\"");
		AssertContains(materials, "AddMaterial(\"magnetite sand\"");
		AssertContains(materials, "AddMaterial(\"barilla plant\"");
		AssertContains(materials, "AddMaterial(\"barilla ash\"");
		AssertContains(materials, "AddMaterial(\"kelp ash\"");
		AssertContains(materials, "EnsureAlias(materials[\"seaweed\"], \"kelp\")");
		AssertContains(materials, "RemoveTag(materials[\"native gold\"], \"Native Nickel Ore\")");
		AssertContains(materials, "RemoveTag(materials[\"native nickel\"], \"Native Gold Ore\")");
	}

	[TestMethod]
	public void PrimaryProductionProjectPhases_SetDescriptionBeforeFirstSave()
	{
		string source = ReadSource("DatabaseSeeder", "Seeders", "PrimaryProductionSeeder.cs");
		string ensurePhase = SliceFrom(source, "private static ProjectPhase EnsurePhase", "private static void EnsurePrimaryLabour");

		int descriptionIndex = ensurePhase.IndexOf("Description = seed.Description", StringComparison.Ordinal);
		int addIndex = ensurePhase.IndexOf("context.ProjectPhases.Add(phase);", StringComparison.Ordinal);
		int saveIndex = ensurePhase.IndexOf("context.SaveChanges();", StringComparison.Ordinal);

		Assert.IsTrue(descriptionIndex >= 0, "New project phases must have their required description populated.");
		Assert.IsTrue(descriptionIndex < addIndex, "ProjectPhase.Description should be assigned before the phase is added.");
		Assert.IsTrue(descriptionIndex < saveIndex, "ProjectPhase.Description should be assigned before the first SaveChanges.");
	}

	private static void AssertContains(string source, string expected)
	{
		Assert.IsTrue(source.Contains(expected, StringComparison.Ordinal),
			$"Expected source to contain: {expected}");
	}

	private static string SliceFrom(string source, string startMarker, string endMarker)
	{
		int start = source.IndexOf(startMarker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Missing start marker {startMarker}.");
		int end = source.IndexOf(endMarker, start, StringComparison.Ordinal);
		Assert.IsTrue(end > start, $"Missing end marker {endMarker}.");
		return source[start..end];
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
