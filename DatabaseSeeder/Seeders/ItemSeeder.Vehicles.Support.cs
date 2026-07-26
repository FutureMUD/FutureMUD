#nullable enable

using ExpressionEngine;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;


public partial class ItemSeeder
{
	private void SeedVehicleSupportItems(IReadOnlyCollection<string> selectedEras)
	{
		var allPreModern = selectedEras.Any(x => x is "antiquity" or "medieval" or "renaissance" or "earlymodern" or "revolution");
		var allMechanical = selectedEras.Any(x => x is "modern" or "atomic" or "computer");
		var allDraft = allPreModern;

		if (allPreModern)
		{
			var oar = EnsureVehicleComponent(
				"VehicleSeeder_Oar_Preindustrial",
				"Vehicle Oar",
				"Turns an item into a serviceable pre-industrial vehicle oar.",
				new XElement("Definition", new XElement("EfficiencyMultiplier", 1.0)).ToString());
			UpsertVehicleEquipmentItem(
				"vehicle_preindustrial_wooden_oar",
				new VehicleItemSeedSpec(
					"oar",
					"a long-bladed wooden oar",
					"This long oar is shaped from a single length of close-grained timber. Its shaft is rounded smooth where hands and rowlocks bear upon it, while the broad blade thins towards a reinforced edge. Minor scrapes and water-darkening mark it as practical working equipment rather than decoration.",
					SizeCategory.Large,
					ItemQuality.Standard,
					5200.0,
					80.0m,
					"ash",
					"Destroyable_WoodenHeavy",
					true),
				["Functions / Vehicles / Vehicle Equipment / Propulsion Equipment", "Market / Transportation / Vehicle Equipment"],
				[oar]);
		}

		if (allMechanical)
		{
			var oar = EnsureVehicleComponent(
				"VehicleSeeder_Oar_Modern",
				"Vehicle Oar",
				"Turns an item into a durable modern vehicle oar.",
				new XElement("Definition", new XElement("EfficiencyMultiplier", 1.1)).ToString());
			UpsertVehicleEquipmentItem(
				"vehicle_modern_varnished_oar",
				new VehicleItemSeedSpec(
					"oar",
					"a varnished laminated oar",
					"This oar has a straight laminated shaft and a broad, carefully balanced blade. Clear varnish seals the pale wood against spray, leaving the layered grain visible beneath a hard gloss. A shaped grip and a dark rubbing band at the rowlock make it suitable for repeated use in a small boat.",
					SizeCategory.Large,
					ItemQuality.Good,
					4300.0,
					240.0m,
					"ash",
					"Destroyable_WoodenHeavy",
					true),
				["Functions / Vehicles / Vehicle Equipment / Propulsion Equipment", "Market / Transportation / Vehicle Equipment"],
				[oar]);

			if (_liquids.TryGetValue("gasoline", out var gasoline))
			{
				var installable = EnsureVehicleComponent(
					"VehicleSeeder_Installable_OutboardMotor",
					"Vehicle Installable",
					"Turns an item into an outboard-motor module for a matching vehicle installation point.",
					new XElement("Definition",
						new XElement("MountType", VehicleOutboardMountType),
						new XElement("Role", VehiclePropulsionRole),
						new XElement("MinimumFunctionalCondition", 0.2),
						new XElement("MinimumMovementCondition", 0.35)).ToString());
				var motor = EnsureVehicleComponent(
					"VehicleSeeder_OutboardMotor_Petrol",
					"Outboard Motor",
					"Turns an item into a small fuelled outboard motor for surface-water vehicles.",
					new XElement("Definition",
						new XElement("EnergySource", OutboardMotorEnergySource.Fuelled),
						new XElement("OutputMultiplier", 1.0),
						new XElement("FuelLiquidId", gasoline.Id),
						new XElement("FuelVolumePerMove", 0.15),
						new XElement("RequiredPowerSpikeInWatts", 0.0)).ToString());
				UpsertVehicleEquipmentItem(
					"vehicle_modern_petrol_outboard_motor",
					new VehicleItemSeedSpec(
						"motor",
						"a compact petrol outboard motor",
						"A compact metal engine housing sits above a long drive leg and a guarded propeller. A clamp bracket, tiller handle and small integral fuel reservoir make the unit visibly intended to hang from a boat's transom. Painted panels protect most of the machinery, though control cables, fasteners and cooling-water passages remain accessible for maintenance.",
						SizeCategory.Large,
						ItemQuality.Standard,
						42000.0,
						4800.0m,
						"mild steel",
						"Destroyable_HeavyMetal",
						true),
					["Functions / Vehicles / Vehicle Equipment / Propulsion Equipment", "Market / Transportation / Vehicle Equipment"],
					[installable, motor],
					["LContainer_FuelCan"]);
			}
		}

		if (allDraft)
		{
			var harness = EnsureVehicleComponent(
				"VehicleSeeder_Hitch_DraftHarness",
				"HitchGear",
				"Turns an item into draft harness and traces suitable for linking pullers to a vehicle.",
				new XElement("Definition",
					new XElement("Roles", HitchGearRole.Yoke | HitchGearRole.Harness | HitchGearRole.Traces),
					new XElement("MaximumUsers", 4),
					new XElement("EffortMultiplier", 1.25),
					new XElement("MaximumTowedWeight", 5000000.0)).ToString());
			UpsertVehicleEquipmentItem(
				"vehicle_preindustrial_draft_harness",
				new VehicleItemSeedSpec(
					"harness",
					"a stout draft harness and traces",
					"Broad leather straps, a padded breast band and paired traces form a practical set of draft harness. Reinforced rings and buckles distribute pulling force without relying on a single fastening. The pieces show the darkened polish and stretching expected of equipment made to work under steady load.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					11500.0,
					260.0m,
					"leather",
					"Destroyable_Misc",
					true),
				["Functions / Vehicles / Vehicle Equipment / Hitching Equipment", "Market / Transportation / Vehicle Equipment"],
				[harness]);
		}

		if (allMechanical)
		{
			var towBar = EnsureVehicleComponent(
				"VehicleSeeder_Hitch_TowBar",
				"HitchGear",
				"Turns an item into a rigid tow bar for compatible road vehicles.",
				new XElement("Definition",
					new XElement("Roles", HitchGearRole.TowBar),
					new XElement("MaximumUsers", 2),
					new XElement("EffortMultiplier", 1.0),
					new XElement("MaximumTowedWeight", 3500000.0)).ToString());
			UpsertVehicleEquipmentItem(
				"vehicle_modern_rigid_tow_bar",
				new VehicleItemSeedSpec(
					"towbar",
					"a rigid steel tow bar",
					"Two telescoping steel arms meet at a reinforced towing eye, with locking pins securing each joint. The opposite ends carry adjustable couplings intended to fasten to prepared towing points. Scraped paint around the pins and eyes reveals bright metal where the bar takes its working loads.",
					SizeCategory.Large,
					ItemQuality.Standard,
					18500.0,
					900.0m,
					"mild steel",
					"Destroyable_HeavyMetal",
					true),
				["Functions / Vehicles / Vehicle Equipment / Hitching Equipment", "Market / Transportation / Vehicle Equipment"],
				[towBar]);
		}
	}

	private void UpsertVehicleEquipmentItem(
		string stableReference,
		VehicleItemSeedSpec itemSpec,
		IEnumerable<string> tags,
		IEnumerable<GameItemComponentProto> generatedComponents,
		IEnumerable<string>? ordinaryComponents = null)
	{
		var components = new List<string>();
		if (itemSpec.Portable)
		{
			components.Add("Holdable");
		}
		components.Add(itemSpec.DestroyableComponent);
		if (ordinaryComponents is not null)
		{
			components.AddRange(ordinaryComponents);
		}

		var item = UpsertVehicleItem(stableReference, itemSpec, tags, components,
			"Seeder category: shared vehicle equipment.");
		foreach (var component in generatedComponents)
		{
			EnsureItemHasComponent(item, component);
		}
	}

}
