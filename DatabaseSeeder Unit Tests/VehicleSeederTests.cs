#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
	public void VehicleExampleCatalogue_HasThirtyTwoUniqueCrossEraExamples()
	{
		var examples = ItemSeeder.VehicleExamplesForTesting;
		Assert.AreEqual(32, examples.Count);
		Assert.AreEqual(32, examples.Select(x => x.StableReference)
			.Distinct(StringComparer.OrdinalIgnoreCase).Count());

		foreach (var era in EraKeys)
		{
			var rows = examples.Where(x => x.EraKey.Equals(era, StringComparison.OrdinalIgnoreCase)).ToArray();
			Assert.AreEqual(4, rows.Length, $"{era} should contain four demonstration vehicles.");
			Assert.AreEqual(2, rows.Count(x => x.Domain == "Terrestrial"), $"{era} terrestrial coverage");
			Assert.AreEqual(2, rows.Count(x => x.Domain == "Aquatic"), $"{era} aquatic coverage");
		}

		foreach (var example in examples)
		{
			Assert.IsTrue(
				Regex.IsMatch(example.StableReference,
					"^vehicle_(antiquity|medieval|renaissance|earlymodern|revolution|modern|atomic|computer)_[a-z0-9_]+$"),
				example.StableReference);
		}
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
				Assert.IsFalse(example.HasExplicitPropulsion, example.StableReference);
			}
		}
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
			"Antiquity, medieval medieval earlymodern unknown COMPUTER");
		CollectionAssert.AreEquivalent(
			new[] { "antiquity", "medieval", "earlymodern", "computer" },
			parsed.ToArray());
		Assert.AreEqual(0, ItemSeeder.ParseVehicleEraTokensForTesting(null).Count);
		Assert.AreEqual(0, ItemSeeder.ParseVehicleEraTokensForTesting("unknown").Count);
	}

	[TestMethod]
	public void VehicleSeeder_IsWiredIntoTheItemSeederHostPipeline()
	{
		var source = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Vehicles.Dispatch.cs");
		StringAssert.Contains(source, "string IDatabaseSeeder.SeedData(");
		Assert.AreEqual(1, Regex.Matches(source, @"\bSeedVehicleItemsAndPrototypes\(eras\);").Count,
			"The IDatabaseSeeder host path should invoke the vehicle subcomponent exactly once.");
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
			"vehicle_computer_electric_drive_module"
		];
		foreach (var stableReference in requiredStableReferences)
		{
			StringAssert.Contains(source, $"\"{stableReference}\"", stableReference);
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
