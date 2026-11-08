#nullable enable

using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void EnsureVehicleInstallationSupport(string mountType)
	{
		if (mountType.Equals(VehicleLandEngineMountType, StringComparison.OrdinalIgnoreCase))
		{
			EnsureVehicleFuelledDriveModules();
			return;
		}

		if (mountType.Equals(VehicleElectricDriveMountType, StringComparison.OrdinalIgnoreCase))
		{
			EnsureVehicleElectricDriveModule();
		}
	}

	private void EnsureVehicleFuelledDriveModules()
	{
		if (!_liquids.TryGetValue("gasoline", out var gasoline) ||
		    !_liquids.TryGetValue("diesel", out var diesel))
		{
			throw new InvalidOperationException(
				"The vehicle seeder requires the seeded liquids 'gasoline' and 'diesel' to create terrestrial engines.");
		}

		var installable = EnsureVehicleComponent(
			"VehicleSeeder_Installable_LandEngine",
			"Vehicle Installable",
			"Turns an item into a removable fuelled drive module for a matching terrestrial vehicle engine bay.",
			new XElement("Definition",
				new XElement("MountType", VehicleLandEngineMountType),
				new XElement("Role", VehiclePropulsionRole),
				new XElement("MinimumFunctionalCondition", 0.2),
				new XElement("MinimumMovementCondition", 0.35)).ToString());
		var petrolEngine = EnsureVehicleComponent(
			"VehicleSeeder_CombustionEngine_Petrol",
			"Combustion Engine",
			"A compact petrol terrestrial engine matched to the vehicle seeder land-engine mount.",
			new XElement("Definition",
				new XElement("FormFactor", new XCData(VehicleLandEngineMountType)),
				new XElement("MaximumPowerInWatts", 90000.0),
				new XElement("NoiseLevel", "Loud"),
				new XElement("FuelLiquidId", gasoline.Id),
				new XElement("FuelPerSecond", 0.00002)).ToString());
		var dieselEngine = EnsureVehicleComponent(
			"VehicleSeeder_CombustionEngine_Diesel",
			"Combustion Engine",
			"A heavy diesel terrestrial engine matched to the vehicle seeder land-engine mount.",
			new XElement("Definition",
				new XElement("FormFactor", new XCData(VehicleLandEngineMountType)),
				new XElement("MaximumPowerInWatts", 400000.0),
				new XElement("NoiseLevel", "VeryLoud"),
				new XElement("FuelLiquidId", diesel.Id),
				new XElement("FuelPerSecond", 0.00006)).ToString());

		UpsertVehicleEquipmentItem(
			"vehicle_modern_petrol_drive_module",
			new VehicleItemSeedSpec(
				"engine",
				"a compact petrol vehicle drive module",
				"A compact cast-metal engine, gearbox and mounting cradle form a single removable drive module. Fuel lines and control linkages terminate in labelled couplings around the frame, while a small integral tank sits behind a guarded filler neck. The unit is arranged for installation in a compatible terrestrial vehicle rather than operation as a free-standing machine.",
				SizeCategory.Huge,
				ItemQuality.Standard,
				185000.0,
				14500.0m,
				"mild steel",
				"Destroyable_HeavyMetal",
				true),
			["Functions / Vehicles / Vehicle Equipment / Propulsion Equipment", "Market / Transportation / Vehicle Equipment"],
			[installable, petrolEngine],
			["LContainer_FuelCan"]);

		UpsertVehicleEquipmentItem(
			"vehicle_modern_diesel_drive_module",
			new VehicleItemSeedSpec(
				"engine",
				"a heavy diesel vehicle drive module",
				"A long heavy engine block, reduction gearbox and reinforced mounting cradle are assembled as one removable drive module. Thick fuel lines, a guarded intake and substantial cooling fittings surround the dark-painted machinery. An integral service tank and plainly marked couplings prepare the unit for a compatible lorry, coach or other terrestrial vehicle.",
				SizeCategory.Enormous,
				ItemQuality.Standard,
				620000.0,
				32000.0m,
				"mild steel",
				"Destroyable_HeavyMetal",
				true),
			["Functions / Vehicles / Vehicle Equipment / Propulsion Equipment", "Market / Transportation / Vehicle Equipment"],
			[installable, dieselEngine],
			["LContainer_FuelCan"]);
	}

	private void EnsureVehicleElectricDriveModule()
	{
		var installable = EnsureVehicleComponent(
			"VehicleSeeder_Installable_ElectricDrive",
			"Vehicle Installable",
			"Turns an item into a removable electric drive module for a matching terrestrial vehicle drive bay.",
			new XElement("Definition",
				new XElement("MountType", VehicleElectricDriveMountType),
				new XElement("Role", VehiclePropulsionRole),
				new XElement("MinimumFunctionalCondition", 0.2),
				new XElement("MinimumMovementCondition", 0.35)).ToString());
		var electricEngine = EnsureVehicleComponent(
			"VehicleSeeder_ElectricEngine_Traction",
			"Electric Engine",
			"An electric traction engine matched to the vehicle seeder electric-drive mount.",
			new XElement("Definition",
				new XElement("Wattage", 90000.0),
				new XElement("WattageDiscount", 0.0),
				new XElement("Switchable", true),
				new XElement("UseMountHostPowerSource", false),
				new XElement("PowerOnEmote", new XCData("@ whine|whines to life.")),
				new XElement("PowerOffEmote", new XCData("@ wind|winds down.")),
				new XElement("OnPoweredProg", 0),
				new XElement("OnUnpoweredProg", 0),
				new XElement("FormFactor", new XCData(VehicleElectricDriveMountType)),
				new XElement("MaximumPowerInWatts", 180000.0),
				new XElement("NoiseLevel", "Decent")).ToString());

		UpsertVehicleEquipmentItem(
			"vehicle_computer_electric_drive_module",
			new VehicleItemSeedSpec(
				"drive module",
				"a sealed electric vehicle drive module",
				"A sealed aluminium housing contains an electric traction motor, reduction gearing and power-control electronics on a rigid mounting frame. Heavy insulated terminals, coolant couplings and a keyed control socket line one face, while a guarded output shaft projects from the other. A removable car battery pack supplies the module's stored electrical power when it is installed in a compatible vehicle.",
				SizeCategory.Huge,
				ItemQuality.Good,
				165000.0,
				26000.0m,
				"aluminium",
				"Destroyable_HeavyMetal",
				true),
			["Functions / Vehicles / Vehicle Equipment / Propulsion Equipment", "Market / Transportation / Vehicle Equipment"],
			[installable, electricEngine],
			["BatteryPowered_1xCarBattery"]);
	}

	private void EnsureVehicleTowSupport(VehicleTowPointSeedSpec point)
	{
		if (point.TowType.Equals("draft", StringComparison.OrdinalIgnoreCase) && point.MaximumTowedWeight > 5000000.0)
		{
			var heavyHarness = EnsureVehicleComponent(
				"VehicleSeeder_Hitch_HeavyTeamHarness",
				"HitchGear",
				"Turns an item into heavy team harness, yoke and traces for large draft vehicles.",
				new XElement("Definition",
					new XElement("Roles", HitchGearRole.Yoke | HitchGearRole.Harness | HitchGearRole.Traces),
					new XElement("MaximumUsers", 8),
					new XElement("EffortMultiplier", 1.3),
					new XElement("MaximumTowedWeight", 12000000.0)).ToString());
			UpsertVehicleEquipmentItem(
				"vehicle_preindustrial_heavy_team_harness",
				new VehicleItemSeedSpec(
					"harness",
					"a heavy team harness, yoke and traces",
					"A broad timber yoke, layered leather breast straps and several paired traces form a complete harness for a large draft team. Iron rings and buckles divide the strain among multiple animals, while thick padding protects the principal bearing surfaces. Every junction is overbuilt for slow work under loads too heavy for ordinary cart harness.",
					SizeCategory.Large,
					ItemQuality.Good,
					34000.0,
					950.0m,
					"leather",
					"Destroyable_Misc",
					true),
				["Functions / Vehicles / Vehicle Equipment / Hitching Equipment", "Market / Transportation / Vehicle Equipment"],
				[heavyHarness]);
		}

		if (point.TowType.Equals("road", StringComparison.OrdinalIgnoreCase) && point.MaximumTowedWeight > 3500000.0)
		{
			var heavyTowBar = EnsureVehicleComponent(
				"VehicleSeeder_Hitch_HeavyTowBar",
				"HitchGear",
				"Turns an item into a heavy rigid tow bar for large road vehicles.",
				new XElement("Definition",
					new XElement("Roles", HitchGearRole.TowBar),
					new XElement("MaximumUsers", 2),
					new XElement("EffortMultiplier", 1.0),
					new XElement("MaximumTowedWeight", 25000000.0)).ToString());
			UpsertVehicleEquipmentItem(
				"vehicle_modern_heavy_rigid_tow_bar",
				new VehicleItemSeedSpec(
					"tow bar",
					"a heavy articulated steel tow bar",
					"Two deep steel draw arms converge on a massive towing eye through pinned articulated joints. Adjustable vehicle couplings, safety-chain lugs and locking collars are set into the reinforced ends, with load markings stamped beside each fastening. The bar is cumbersome but rated for coaches, lorries and other heavy road vehicles.",
					SizeCategory.Huge,
					ItemQuality.Good,
					86000.0,
					5600.0m,
					"mild steel",
					"Destroyable_HeavyMetal",
					true),
				["Functions / Vehicles / Vehicle Equipment / Hitching Equipment", "Market / Transportation / Vehicle Equipment"],
				[heavyTowBar]);
		}
	}
}
