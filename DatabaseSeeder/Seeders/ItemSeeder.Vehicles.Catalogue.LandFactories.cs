#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static VehicleSeedSpec CreateDraftCargoVehicle(
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
		bool enclosed,
		bool routeMovement,
		double maximumTowedWeight,
		double characterPullMultiplier,
		string cargoName,
		string cargoDescription,
		string archetype)
	{
		var access = enclosed
			? new VehicleAccessPointSeedSpec(
				"rear_access", "rear access", "A closable rear opening provides access to the vehicle's interior.", "main",
				VehicleAccessPointType.Door, false, true, 0,
				Projection("door", $"the rear access of {shortDescription}",
					$"This fitted rear closure belongs to {shortDescription}. Its hinges, fastenings and framing are integral to the vehicle.",
					SizeCategory.Large, material, destroyable))
			: null;
		var movement = routeMovement
			? RouteMovement("route", "guided route travel", RouteVehiclePropulsionMode.ExternallyPulled, 4.0, null,
				0.0, 0.0, 0.0, false)
			: LandMovement("road", "ordinary road travel", VehiclePropulsionType.ExternallyPulled, 0.0,
				string.Empty, false);
		var passengerSlots = passengerCapacity > 0
			? new[] { new VehicleOccupantSlotSeedSpec("passengers", "passenger places", "main", VehicleOccupantSlotType.Passenger, passengerCapacity, false, false) }
			: [];
		return new VehicleSeedSpec(
			stableReference, eraKey, VehicleDomainTerrestrial, archetype, name, description, scale,
			Exterior(noun, shortDescription, fullDescription, size, quality, weight, cost, material, destroyable, false),
			[new VehicleCompartmentSeedSpec("main", enclosed ? "interior" : "platform", enclosed
				? "The principal compartment is enclosed against weather and road dirt."
				: "The principal compartment is an open working platform.", 0)],
			[],
			[
				new VehicleOccupantSlotSeedSpec("driver", "driver's place", "main", VehicleOccupantSlotType.Driver, 1, true, false),
				.. passengerSlots
			],
			[new VehicleControlStationSeedSpec("primary", enclosed ? "driver's box" : "driving position", "driver", true)],
			[movement],
			access is null ? [] : [access],
			[
				new VehicleCargoSpaceSeedSpec("cargo", cargoName, cargoDescription, "main", access is null ? null : "rear_access", 0,
					enclosed ? "Container_Trunk" : "Container_Open_Bin",
					Projection("cargo", $"the {cargoName} of {shortDescription}",
						$"This cargo projection represents the {cargoName} built into {shortDescription}. It follows the dimensions and access of the vehicle rather than existing as a separate container.",
						SizeCategory.Huge, material, destroyable))
			],
			[],
			[
				new VehicleTowPointSeedSpec("front_tow", "front draft point", "A reinforced forward draft point accepts compatible harness or traces.", null,
					"draft", false, true, maximumTowedWeight, characterPullMultiplier, 0.8, 1.0, 0.2, 1.0, 0),
				new VehicleTowPointSeedSpec("rear_tow", "rear tow point", "A reinforced rear point allows another light vehicle or load to be coupled behind.", null,
					"draft", true, false, maximumTowedWeight * 0.75, 1.0, 0.8, 1.0, 0.2, 1.0, 1)
			],
			LandDamageZones(routeMovement ? "route" : "road", maximumTowedWeight),
			passengerCapacity > 0, true,
			"Externally pulled terrestrial example. Fit compatible hitch gear and pullers before movement; route variants require a RouteCell.");
	}

	private static VehicleSeedSpec CreatePoweredRoadVehicle(
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
		bool hasCargo,
		bool routeMovement,
		string? fuelLiquid,
		string mountType,
		bool automaticOperation,
		bool serviceVehicle,
		double routeSpeed,
		double routeResourceRate,
		string archetype)
	{
		var isElectric = string.IsNullOrWhiteSpace(fuelLiquid);
		var minimumEnginePower = Math.Max(1000.0, weight * 0.03);
		var movement = routeMovement
			? RouteMovement("route", "powered route travel",
				automaticOperation ? RouteVehiclePropulsionMode.Powered : RouteVehiclePropulsionMode.EnginePowered,
				routeSpeed,
				automaticOperation ? fuelLiquid : null,
				automaticOperation && !isElectric ? routeResourceRate : 0.0,
				automaticOperation && isElectric ? routeResourceRate : 0.0,
				automaticOperation ? 0.0 : minimumEnginePower,
				automaticOperation)
			: LandMovement("road", "powered road travel", VehiclePropulsionType.Engine, minimumEnginePower,
				VehiclePropulsionRole, true);
		var access = new VehicleAccessPointSeedSpec(
			"doors", "passenger doors", "The vehicle's fitted passenger doors open into the main cabin.", "cabin",
			VehicleAccessPointType.Door, false, true, 0,
			Projection("door", $"the passenger doors of {shortDescription}",
				$"These fitted doors and their surrounding frame are integral to {shortDescription}. Handles, latches and seals are arranged for repeated passenger access.",
				SizeCategory.Large, material, destroyable));
		var cargo = hasCargo
			? new[]
			{
				new VehicleCargoSpaceSeedSpec("cargo", serviceVehicle ? "service cargo bay" : "luggage compartment",
					serviceVehicle
						? "A large enclosed cargo bay occupies most of the vehicle behind the driving compartment."
						: "A separate enclosed compartment carries luggage and small personal cargo.",
					"cabin", "doors", 0, serviceVehicle ? "Container_Colossal" : "Container_Trunk",
					Projection("cargo", $"the cargo compartment of {shortDescription}",
						$"This hidden projection represents the built-in cargo compartment of {shortDescription}, including its fixed floor, sides and loading access.",
						serviceVehicle ? SizeCategory.Enormous : SizeCategory.Huge, material, destroyable))
			}
			: [];
		return new VehicleSeedSpec(
			stableReference, eraKey, VehicleDomainTerrestrial, archetype, name, description, scale,
			Exterior(noun, shortDescription, fullDescription, size, quality, weight, cost, material, destroyable, false),
			[new VehicleCompartmentSeedSpec("cabin", "passenger cabin", "The main cabin contains the driving controls and occupied seating.", 0)],
			[],
			[
				new VehicleOccupantSlotSeedSpec("driver", "driver's seat", "cabin", VehicleOccupantSlotType.Driver, 1, !automaticOperation, false),
				new VehicleOccupantSlotSeedSpec("passengers", "passenger seats", "cabin", VehicleOccupantSlotType.Passenger, passengerCapacity, false, false)
			],
			[new VehicleControlStationSeedSpec("primary", automaticOperation ? "manual control console" : "driving controls", "driver", true)],
			[movement],
			[access],
			cargo,
			[
				new VehicleInstallationPointSeedSpec("drive_module", isElectric ? "electric drive bay" : "engine bay",
					isElectric ? "A protected bay accepts a compatible electric vehicle drive module." : "A protected bay accepts a compatible fuelled vehicle drive module.",
					null, mountType, VehiclePropulsionRole, true, 0)
			],
			[
				new VehicleTowPointSeedSpec("front_tow", "front recovery point", "A rated front recovery point accepts compatible towing gear.", null,
					"road", false, true, weight * 2.0, 1.0, 0.85, 1.0, 0.15, 1.0, 0),
				new VehicleTowPointSeedSpec("rear_tow", "rear towing point", "A rated rear towing point allows another compatible vehicle to be coupled.", null,
					"road", true, false, weight * 1.5, 1.0, 0.85, 1.0, 0.15, 1.0, 1)
			],
			PoweredLandDamageZones(routeMovement ? "route" : "road", "drive_module"),
			true, hasCargo,
			isElectric
				? "Install the seeded electric drive module and charge it with compatible batteries before movement."
				: $"Install the seeded {fuelLiquid} drive module and fill its liquid container with {fuelLiquid} before movement.");
	}
}
