#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleSeederTests
{
	private static readonly string[] EraKeys =
	[
		"antiquity", "medieval", "renaissance", "earlymodern",
		"revolution", "modern", "atomic", "computer"
	];

	[TestMethod]
	public void VehicleExampleCatalogue_HasBroadUniqueCrossEraCoverage()
	{
		var examples = ItemSeeder.VehicleExamplesForTesting;
		Assert.AreEqual(57, examples.Count);
		Assert.AreEqual(57, examples.Select(x => x.StableReference)
			.Distinct(StringComparer.OrdinalIgnoreCase).Count());

		var expectedMinimums = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity"] = 16,
			["medieval"] = 15,
			["renaissance"] = 18,
			["earlymodern"] = 24,
			["revolution"] = 4,
			["modern"] = 4,
			["atomic"] = 4,
			["computer"] = 4
		};
		foreach (var era in EraKeys)
		{
			var rows = examples.Where(x => x.SupportedEraKeys.Contains(era, StringComparer.OrdinalIgnoreCase)).ToArray();
			Assert.IsTrue(rows.Length >= expectedMinimums[era],
				$"{era} catalogue coverage expected at least {expectedMinimums[era]} but found {rows.Length}");
			Assert.IsTrue(rows.Any(x => x.Domain == "Terrestrial"), $"{era} terrestrial coverage");
			Assert.IsTrue(rows.Any(x => x.Domain == "Aquatic"), $"{era} aquatic coverage");
		}

		foreach (var example in examples)
		{
			Assert.IsTrue(
				Regex.IsMatch(example.StableReference,
					"^vehicle_(preindustrial|antiquity|medieval|renaissance|earlymodern|revolution|modern|atomic|computer)_[a-z0-9_]+$"),
				example.StableReference);
		}

		Assert.AreEqual(41, examples.Count(x =>
			x.SupportedEraKeys.Any(era => era is "antiquity" or "medieval" or "renaissance" or "earlymodern")));
	}

	[TestMethod]
	public void VehicleExampleCatalogue_PassesAuthoringValidation()
	{
		ItemSeeder.ValidateVehicleExamplesForTesting();
	}

	[TestMethod]
	public void VehicleExampleCatalogue_ProvidesOperationalSkeletons()
	{
		foreach (var example in ItemSeeder.VehicleExamplesForTesting)
		{
			Assert.IsTrue(example.CompartmentCount >= 1, example.StableReference);
			Assert.IsTrue(example.OccupantSlotCount >= 1, example.StableReference);
			Assert.AreEqual(1, example.PrimaryControlStationCount, example.StableReference);
			Assert.IsTrue(example.MovementProfileCount >= 1, example.StableReference);
			Assert.IsTrue(example.HasDriverSlot, example.StableReference);

			if (example.Domain == "Aquatic")
			{
				Assert.IsTrue(example.HasSurfaceWaterMovement, example.StableReference);
				Assert.IsTrue(example.HasExplicitPropulsion, example.StableReference);
			}
			else
			{
				Assert.IsFalse(example.HasSurfaceWaterMovement, example.StableReference);
				Assert.AreNotEqual(example.HasRouteMovement, example.HasExplicitPropulsion,
					$"{example.StableReference} should use route propulsion or explicit cell-exit propulsion, not both.");
			}
		}
	}

	[TestMethod]
	public void TerrestrialCellExitVehicles_UseCurrentPropulsionContracts()
	{
		var terrestrial = ItemSeeder.VehicleExamplesForTesting
			.Where(x => x.Domain == "Terrestrial" && !x.HasRouteMovement)
			.ToArray();
		Assert.IsTrue(terrestrial.Any());
		Assert.IsTrue(terrestrial.All(x => x.PropulsionTypes.Count == 1));
		Assert.IsTrue(terrestrial.All(x =>
			x.PropulsionTypes.Single() is VehiclePropulsionType.ExternallyPulled or VehiclePropulsionType.Engine));
		Assert.IsTrue(terrestrial
			.Where(x => x.PropulsionTypes.Single() == VehiclePropulsionType.Engine)
			.All(x => x.MinimumEnginePowerInWatts > 0.0));
	}

	[TestMethod]
	public void PreIndustrialCatalogue_CoversLandInlandAndOceanFamilies()
	{
		var examples = ItemSeeder.VehicleExamplesForTesting
			.ToDictionary(x => x.StableReference, StringComparer.OrdinalIgnoreCase);
		string[] requiredReferences =
		[
			"vehicle_antiquity_light_war_chariot",
			"vehicle_preindustrial_farm_wain",
			"vehicle_preindustrial_winter_sledge",
			"vehicle_medieval_timber_wagon",
			"vehicle_renaissance_artillery_limber",
			"vehicle_earlymodern_hackney_coach",
			"vehicle_preindustrial_river_punt",
			"vehicle_preindustrial_ferry_barge",
			"vehicle_preindustrial_river_cargo_barge",
			"vehicle_antiquity_trireme",
			"vehicle_preindustrial_lateen_dhow",
			"vehicle_medieval_longship",
			"vehicle_renaissance_caravel",
			"vehicle_renaissance_carrack",
			"vehicle_renaissance_galleon",
			"vehicle_earlymodern_coastal_schooner",
			"vehicle_earlymodern_packet_ship",
			"vehicle_earlymodern_sailing_frigate",
			"vehicle_earlymodern_ship_of_the_line"
		];
		foreach (var stableReference in requiredReferences)
		{
			Assert.IsTrue(examples.ContainsKey(stableReference), stableReference);
		}

		CollectionAssert.AreEquivalent(
			new[] { "antiquity", "medieval", "renaissance", "earlymodern" },
			examples["vehicle_preindustrial_farm_wain"].SupportedEraKeys.ToArray());
		CollectionAssert.AreEquivalent(
			new[] { "renaissance", "earlymodern" },
			examples["vehicle_renaissance_galleon"].SupportedEraKeys.ToArray());
	}

	[TestMethod]
	public void VehicleExampleCatalogue_DemonstratesProjectionAndMotorPatterns()
	{
		var examples = ItemSeeder.VehicleExamplesForTesting;
		Assert.IsTrue(examples.Count(x => x.HasCargoProjection) >= 24);
		Assert.IsTrue(examples.Count(x => x.HasAccessProjection) >= 16);

		var motorExamples = examples.Where(x => x.HasMotorInstallation).ToArray();
		Assert.AreEqual(4, motorExamples.Length);
		Assert.IsTrue(motorExamples.All(x => x.Domain == "Aquatic"));
		Assert.IsTrue(motorExamples.All(x => x.EraKey is "modern" or "atomic" or "computer"));
	}

	[TestMethod]
	public void VehicleEraParser_NormalisesFiltersAndDeduplicates()
	{
		var parsed = ItemSeeder.ParseVehicleEraTokensForTesting(
			"Antiquity, medieval medieval earlymodern unknown industrial revolution nuclear atomic information COMPUTER");
		CollectionAssert.AreEquivalent(
			new[] { "antiquity", "medieval", "earlymodern", "revolution", "atomic", "computer" },
			parsed.ToArray());
		Assert.AreEqual(0, ItemSeeder.ParseVehicleEraTokensForTesting(null).Count);
		Assert.AreEqual(0, ItemSeeder.ParseVehicleEraTokensForTesting("unknown").Count);
	}

	[TestMethod]
	public void VehicleSeeder_IsWiredIntoTheItemSeederHostPipeline()
	{
		var source = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.cs");
		StringAssert.Contains(source, "public string SeedData(");
		Assert.AreEqual(1, Regex.Matches(source, @"\bSeedVehicleItemsAndPrototypes\(eras\);").Count,
			"The public and interface host path should invoke the vehicle subcomponent exactly once.");
	}

	[TestMethod]
	public void VehicleSeeder_SeedsRequiredSupportEquipmentPatterns()
	{
		var seederPath = Path.Combine(SourceRoot(), "DatabaseSeeder", "Seeders");
		var source = string.Join("\n", Directory.GetFiles(seederPath, "ItemSeeder.Vehicles*.cs")
			.Select(File.ReadAllText));
		string[] requiredStableReferences =
		[
			"vehicle_preindustrial_wooden_oar",
			"vehicle_modern_varnished_oar",
			"vehicle_modern_petrol_outboard_motor",
			"vehicle_preindustrial_draft_harness",
			"vehicle_preindustrial_heavy_team_harness",
			"vehicle_modern_rigid_tow_bar",
			"vehicle_modern_heavy_rigid_tow_bar",
			"vehicle_modern_petrol_drive_module",
			"vehicle_modern_diesel_drive_module",
			"vehicle_computer_electric_drive_module",
			"VehicleSeeder_CombustionEngine_Petrol",
			"VehicleSeeder_CombustionEngine_Diesel",
			"VehicleSeeder_ElectricEngine_Traction"
		];
		foreach (var stableReference in requiredStableReferences)
		{
			StringAssert.Contains(source, $"\"{stableReference}\"", stableReference);
		}

		var catalogue = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Item_Components.json"));
		var componentNames = catalogue.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Component Name").GetString()!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		string[] requiredCatalogueComponents =
		[
			"VehicleSeeder_Oar_Preindustrial",
			"VehicleSeeder_Oar_Modern",
			"VehicleSeeder_Installable_OutboardMotor",
			"VehicleSeeder_OutboardMotor_Petrol",
			"VehicleSeeder_Hitch_DraftHarness",
			"VehicleSeeder_Hitch_TowBar",
			"VehicleSeeder_Installable_LandEngine",
			"VehicleSeeder_CombustionEngine_Petrol",
			"VehicleSeeder_CombustionEngine_Diesel",
			"VehicleSeeder_Installable_ElectricDrive",
			"VehicleSeeder_ElectricEngine_Traction",
			"VehicleSeeder_Hitch_HeavyTeamHarness",
			"VehicleSeeder_Hitch_HeavyTowBar"
		];
		foreach (var componentName in requiredCatalogueComponents)
		{
			Assert.IsTrue(componentNames.Contains(componentName),
				$"Seeded_Item_Components.json is missing {componentName}.");
		}
	}

	[TestMethod]
	public void VehicleSeederDesignReference_CoversTheBuilderContract()
	{
		var document = ReadSource("Design Documents", "Seeding", "Vehicle_Item_Seeder_Design_Reference.md");
		string[] requiredHeadings =
		[
			"## Runtime Architecture",
			"## Seeder API and Data Contract",
			"## Units and Recommended Ranges",
			"## Propulsion and Movement Patterns",
			"## Idempotency and Stable Keys",
			"## Edge Cases and Failure Modes",
			"## Demonstration Catalogue",
			"## Authoring Checklist"
		];
		foreach (var heading in requiredHeadings)
		{
			StringAssert.Contains(document, heading, heading);
		}
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.Combine(new[] { SourceRoot() }.Concat(parts).ToArray()));
	}

	private static string SourceRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MudSharp.sln")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate repository root from test output path.");
		return directory.FullName;
	}
}
