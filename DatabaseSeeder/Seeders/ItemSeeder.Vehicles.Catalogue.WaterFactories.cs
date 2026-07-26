#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static VehicleSeedSpec CreatePaddleCraft(
		string stableReference,
		string eraKey,
		string name,
		string description,
		string noun,
		string shortDescription,
		string fullDescription,
		SizeCategory size,
		ItemQuality quality,
		double weight,
		decimal cost,
		string material,
		string destroyable,
		VehicleScale scale,
		VehiclePropulsionType propulsionType,
		int driverCapacity,
		int crewCapacity,
		bool portable,
		bool hasCargo,
		string archetype)
	{
		var crewSlots = crewCapacity > 0
			? new[] { new VehicleOccupantSlotSeedSpec("rowers", "rowing thwarts", "hull", VehicleOccupantSlotType.Crew, crewCapacity, false, true, Difficulty.Normal) }
			: [];
		var cargo = hasCargo
			? new[]
			{
				new VehicleCargoSpaceSeedSpec("cargo", "open stowage", "Open floor space between the seats can hold modest cargo while keeping the craft balanced.",
					"hull", null, 0, "Container_Open_Bin",
					Projection("cargo", $"the open stowage of {shortDescription}",
						$"This hidden projection represents the usable open stowage inside {shortDescription}.", SizeCategory.Huge, material, destroyable))
			}
			: [];
		var propulsion = propulsionType == VehiclePropulsionType.SelfPowered
			? SelfPoweredProfile(true)
			: RowedProfile(true);
		return new VehicleSeedSpec(
			stableReference, eraKey, VehicleDomainAquatic, archetype, name, description, scale,
			Exterior(noun, shortDescription, fullDescription, size, quality, weight, cost, material, destroyable, portable),
			[new VehicleCompartmentSeedSpec("hull", "open hull", "The occupied space lies within the craft's open hull.", 0)],
			[],
			[
				new VehicleOccupantSlotSeedSpec("driver", propulsionType == VehiclePropulsionType.SelfPowered ? "paddler's place" : "steersman's place",
					"hull", VehicleOccupantSlotType.Driver, driverCapacity, true, propulsionType == VehiclePropulsionType.SelfPowered, Difficulty.Normal),
				.. crewSlots
			],
			[new VehicleControlStationSeedSpec("primary", propulsionType == VehiclePropulsionType.SelfPowered ? "paddling position" : "steering position", "driver", true)],
			[WaterMovement("water", "surface-water travel", false, [propulsion])],
			[], cargo, [],
			[
				new VehicleTowPointSeedSpec("bow_tow", "bow towing eye", "A reinforced eye at the bow accepts a line for towing or recovery.", null,
					"marine", true, true, Math.Max(weight * 3.0, 250000.0), 1.0, 0.85, 1.0, 0.15, 1.0, 0)
			],
			WaterDamageZones("water", null),
			crewCapacity > 0, hasCargo,
			propulsionType == VehiclePropulsionType.Rowed
				? "Every rowing contributor must occupy a propulsion slot and hold a seeded vehicle oar."
				: "The controller supplies self-powered aquatic propulsion; a Swimming, Rowing or Athletics trait must exist.");
	}

	private static VehicleSeedSpec CreateSailCraft(
		string stableReference,
		string eraKey,
		string name,
		string description,
		string noun,
		string shortDescription,
		string fullDescription,
		SizeCategory size,
		ItemQuality quality,
		double weight,
		decimal cost,
		string material,
		string destroyable,
		VehicleScale scale,
		int passengerCapacity,
		int crewCapacity,
		bool hasCargo,
		string archetype)
	{
		var access = new VehicleAccessPointSeedSpec(
			"hold_hatch", "cargo hatch", "A stout deck hatch opens into the lower hold.", "hold",
			VehicleAccessPointType.Hatch, false, true, 0,
			Projection("hatch", $"the cargo hatch of {shortDescription}",
				$"This heavy fitted hatch closes the cargo opening in {shortDescription}; its coaming, hinges and securing bars are part of the vessel.",
				SizeCategory.VeryLarge, material, destroyable));
		var cargo = hasCargo
			? new[]
			{
				new VehicleCargoSpaceSeedSpec("hold", "cargo hold", "The lower hold provides broad stowage beneath the working deck.", "hold", "hold_hatch", 0,
					"Container_Colossal",
					Projection("hold", $"the cargo hold of {shortDescription}",
						$"This hidden projection represents the fixed lower cargo hold of {shortDescription} and is accessed through its deck hatch.",
						SizeCategory.Enormous, material, destroyable))
			}
			: [];
		return new VehicleSeedSpec(
			stableReference, eraKey, VehicleDomainAquatic, archetype, name, description, scale,
			Exterior(noun, shortDescription, fullDescription, size, quality, weight, cost, material, destroyable, false),
			[
				new VehicleCompartmentSeedSpec("deck", "working deck", "The exposed working deck carries the helm, rigging and occupied stations.", 0),
				new VehicleCompartmentSeedSpec("hold", "lower hold", "A lower enclosed compartment lies beneath the main deck.", 1)
			],
			[
				new VehicleCompartmentLinkSeedSpec("deck_to_hold", "deck", "hold", "down", "up", "through the cargo hatch", "up through the cargo hatch")
			],
			[
				new VehicleOccupantSlotSeedSpec("driver", "helm", "deck", VehicleOccupantSlotType.Driver, 1, true, false, Difficulty.Normal),
				new VehicleOccupantSlotSeedSpec("crew", "working crew stations", "deck", VehicleOccupantSlotType.Crew, crewCapacity, false, true, Difficulty.Normal),
				new VehicleOccupantSlotSeedSpec("passengers", "passenger places", "deck", VehicleOccupantSlotType.Passenger, passengerCapacity, false, false, Difficulty.Normal)
			],
			[new VehicleControlStationSeedSpec("primary", "helm", "driver", true)],
			[WaterMovement("water", "sailing profile", true, [SailProfile(true), RowedProfile(false)])],
			[access], cargo, [],
			[
				new VehicleTowPointSeedSpec("bow_tow", "bow towing bitt", "A strong forward bitt accepts towing hawsers and mooring lines.", null,
					"marine", true, true, weight * 2.0, 1.0, 0.85, 1.0, 0.15, 1.0, 0),
				new VehicleTowPointSeedSpec("stern_tow", "stern towing bitt", "A strong after bitt permits another vessel or floating load to be towed astern.", null,
					"marine", true, true, weight * 1.5, 1.0, 0.85, 1.0, 0.15, 1.0, 1)
			],
			SailDamageZones("water", "hold_hatch", hasCargo ? "hold" : null),
			true, hasCargo,
			"Sail is the default propulsion mode and requires wind. Rowing is available as a fallback only when staffed propulsion slots have usable oars.");
	}

	private static VehicleSeedSpec CreateMotorCraft(
		string stableReference,
		string eraKey,
		string name,
		string description,
		string noun,
		string shortDescription,
		string fullDescription,
		SizeCategory size,
		ItemQuality quality,
		double weight,
		decimal cost,
		string material,
		string destroyable,
		VehicleScale scale,
		int passengerCapacity,
		bool hasCabin,
		string archetype)
	{
		var access = hasCabin
			? new VehicleAccessPointSeedSpec(
				"cabin_door", "cabin door", "A fitted weatherproof door closes the vessel's small cabin.", "cabin",
				VehicleAccessPointType.Door, false, true, 0,
				Projection("door", $"the cabin door of {shortDescription}",
					$"This fitted weatherproof door and frame are integral to the cabin of {shortDescription}.", SizeCategory.Large, material, destroyable))
			: null;
		var compartments = hasCabin
			? new[]
			{
				new VehicleCompartmentSeedSpec("cockpit", "cockpit", "The open cockpit contains the helm and principal working space.", 0),
				new VehicleCompartmentSeedSpec("cabin", "cabin", "A compact enclosed cabin provides sheltered seating and stowage.", 1)
			}
			: [new VehicleCompartmentSeedSpec("cockpit", "cockpit", "The open cockpit contains the helm, seating and working space.", 0)];
		var links = hasCabin
			? new[] { new VehicleCompartmentLinkSeedSpec("cockpit_to_cabin", "cockpit", "cabin", "forward", "aft", "through the cabin door", "out through the cabin door") }
			: [];
		var cargo = new[]
		{
			new VehicleCargoSpaceSeedSpec("locker", hasCabin ? "cabin locker" : "forward locker",
				hasCabin ? "A built-in locker occupies part of the sheltered cabin." : "A lidded locker is built beneath the forward deck.",
				hasCabin ? "cabin" : "cockpit", hasCabin ? "cabin_door" : null, 0, "Container_Trunk",
				Projection("locker", $"the built-in locker of {shortDescription}",
					$"This hidden projection represents the fixed storage locker built into {shortDescription}.", SizeCategory.Huge, material, destroyable))
		};
		return new VehicleSeedSpec(
			stableReference, eraKey, VehicleDomainAquatic, archetype, name, description, scale,
			Exterior(noun, shortDescription, fullDescription, size, quality, weight, cost, material, destroyable, false),
			compartments, links,
			[
				new VehicleOccupantSlotSeedSpec("driver", "helm seat", "cockpit", VehicleOccupantSlotType.Driver, 1, true, false, Difficulty.Easy),
				new VehicleOccupantSlotSeedSpec("rowers", "emergency rowing places", "cockpit", VehicleOccupantSlotType.Crew, 2, false, true, Difficulty.Normal),
				new VehicleOccupantSlotSeedSpec("passengers", "passenger places", hasCabin ? "cabin" : "cockpit", VehicleOccupantSlotType.Passenger, passengerCapacity, false, false, Difficulty.Normal)
			],
			[new VehicleControlStationSeedSpec("primary", "helm controls", "driver", true)],
			[WaterMovement("water", "motor and emergency rowing profile", true, [OutboardProfile(true), RowedProfile(false)])],
			access is null ? [] : [access],
			cargo,
			[
				new VehicleInstallationPointSeedSpec("outboard", "outboard transom mount", "A reinforced transom mount accepts a compatible outboard propulsion module.",
					null, VehicleOutboardMountType, VehiclePropulsionRole, false, 0)
			],
			[
				new VehicleTowPointSeedSpec("bow_tow", "bow towing eye", "A rated bow eye accepts recovery and towing lines.", null,
					"marine", true, true, weight * 2.0, 1.0, 0.85, 1.0, 0.15, 1.0, 0)
			],
			WaterDamageZones("water", "outboard"),
			true, true,
			"Install the seeded outboard motor on the transom mount and fill it with gasoline. Emergency rowing remains selectable when staffed rowers hold vehicle oars.");
	}
}
