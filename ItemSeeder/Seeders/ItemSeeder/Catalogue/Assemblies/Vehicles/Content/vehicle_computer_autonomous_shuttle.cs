#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace DatabaseSeeder.Seeders;

public static partial class VehicleStock
{
	public static VehicleSeedSpec VehicleComputerAutonomousShuttle() =>
		new VehicleSeedSpec(
		StableReference: "vehicle_computer_autonomous_shuttle",
		EraKey: "computer",
		Domain: "Terrestrial",
		Archetype: "AutonomousShuttle",
		Name: "Autonomous Shuttle",
		Description: "A low-speed electric passenger shuttle approved for automatic route operation.",
		Scale: VehicleScale.RoomContainer,
		ExteriorItem: new VehicleItemSeedSpec(
			Noun: "shuttle",
			ShortDescription: "a windowed autonomous electric shuttle",
			FullDescription: "A boxy aluminium passenger pod rides on four small enclosed wheels beneath a roof crowded with cameras and range sensors. Wide sliding doors open onto a level floor and facing bench seats, with only a compact manual control console interrupting the cabin. External status lights and destination displays make its route-service role immediately apparent.",
			Size: SizeCategory.Titanic,
			Quality: ItemQuality.Good,
			WeightInGrams: 4200000,
			Cost: 86000.0m,
			Material: "aluminium",
			DestroyableComponent: "Destroyable_HeavyMetal",
			Portable: false,
			Skinnable: true,
			HiddenFromPlayers: false,
			LongDescription: null
		),
		Compartments: [
			new VehicleCompartmentSeedSpec(
				Key: "cabin",
				Name: "passenger cabin",
				Description: "The main cabin contains the driving controls and occupied seating.",
				DisplayOrder: 0,
				InteriorTerrainId: null,
				InteriorOutdoorsType: 0
			)
		],
		CompartmentLinks: [],
		OccupantSlots: [
			new VehicleOccupantSlotSeedSpec(
				Key: "driver",
				Name: "driver's seat",
				CompartmentKey: "cabin",
				SlotType: VehicleOccupantSlotType.Driver,
				Capacity: 1,
				RequiredForMovement: false,
				ContributesToPropulsion: false,
				BoatStabilityDifficulty: Difficulty.Normal
			),
			new VehicleOccupantSlotSeedSpec(
				Key: "passengers",
				Name: "passenger seats",
				CompartmentKey: "cabin",
				SlotType: VehicleOccupantSlotType.Passenger,
				Capacity: 12,
				RequiredForMovement: false,
				ContributesToPropulsion: false,
				BoatStabilityDifficulty: Difficulty.Normal
			)
		],
		ControlStations: [
			new VehicleControlStationSeedSpec(
				Key: "primary",
				Name: "manual control console",
				OccupantSlotKey: "driver",
				IsPrimary: true
			)
		],
		MovementProfiles: [
			new VehicleMovementProfileSeedSpec(
				Key: "route",
				Name: "powered route travel",
				MovementType: VehicleMovementProfileType.Route,
				Environment: VehicleMovementEnvironment.Unrestricted,
				ExposesOccupantsToWater: false,
				IsDefault: true,
				RequiredPowerSpikeInWatts: 0,
				MinimumEnginePowerInWatts: 0,
				FuelLiquid: null,
				FuelVolumePerMove: 0,
				RequiredInstalledRole: "propulsion",
				RequiresTowLinksClosed: true,
				RequiresAccessPointsClosed: true,
				RouteSpeedMetresPerSecond: 8,
				RoutePropulsionMode: RouteVehiclePropulsionMode.Powered,
				RouteFuelVolumePerMetre: 0,
				RoutePowerDrawWatts: 35,
				AutomaticOperationCapable: true,
				PropulsionProfiles: []
			)
		],
		AccessPoints: [
			new VehicleAccessPointSeedSpec(
				Key: "doors",
				Name: "passenger doors",
				Description: "The vehicle's fitted passenger doors open into the main cabin.",
				CompartmentKey: "cabin",
				AccessPointType: VehicleAccessPointType.Door,
				StartsOpen: false,
				MustBeClosedForMovement: true,
				DisplayOrder: 0,
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "door",
					ShortDescription: "the passenger doors of a windowed autonomous electric shuttle",
					FullDescription: "These fitted doors and their surrounding frame are integral to a windowed autonomous electric shuttle. Handles, latches and seals are arranged for repeated passenger access.",
					Size: SizeCategory.Large,
					Quality: ItemQuality.Standard,
					WeightInGrams: 1,
					Cost: 0.0m,
					Material: "aluminium",
					DestroyableComponent: "Destroyable_HeavyMetal",
					Portable: false,
					Skinnable: false,
					HiddenFromPlayers: true,
					LongDescription: null
				)
			)
		],
		CargoSpaces: [
			new VehicleCargoSpaceSeedSpec(
				Key: "cargo",
				Name: "service cargo bay",
				Description: "A large enclosed cargo bay occupies most of the vehicle behind the driving compartment.",
				CompartmentKey: "cabin",
				RequiredAccessPointKey: "doors",
				DisplayOrder: 0,
				ContainerComponent: "Container_Colossal",
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "cargo",
					ShortDescription: "the cargo compartment of a windowed autonomous electric shuttle",
					FullDescription: "This hidden projection represents the built-in cargo compartment of a windowed autonomous electric shuttle, including its fixed floor, sides and loading access.",
					Size: SizeCategory.Enormous,
					Quality: ItemQuality.Standard,
					WeightInGrams: 1,
					Cost: 0.0m,
					Material: "aluminium",
					DestroyableComponent: "Destroyable_HeavyMetal",
					Portable: false,
					Skinnable: false,
					HiddenFromPlayers: true,
					LongDescription: null
				)
			)
		],
		InstallationPoints: [
			new VehicleInstallationPointSeedSpec(
				Key: "drive_module",
				Name: "electric drive bay",
				Description: "A protected bay accepts a compatible electric vehicle drive module.",
				RequiredAccessPointKey: null,
				MountType: "electric_drive",
				RequiredRole: "propulsion",
				RequiredForMovement: true,
				DisplayOrder: 0
			)
		],
		TowPoints: [
			new VehicleTowPointSeedSpec(
				Key: "front_tow",
				Name: "front recovery point",
				Description: "A rated front recovery point accepts compatible towing gear.",
				RequiredAccessPointKey: null,
				TowType: "road",
				CanTow: false,
				CanBeTowed: true,
				MaximumTowedWeight: 8400000,
				CharacterPullMultiplier: 1,
				TowStressWarningRatio: 0.85,
				TowStressFailureStartRatio: 1,
				TowStressMaximumFailureChance: 0.15,
				TowStressDamageMultiplier: 1,
				DisplayOrder: 0
			),
			new VehicleTowPointSeedSpec(
				Key: "rear_tow",
				Name: "rear towing point",
				Description: "A rated rear towing point allows another compatible vehicle to be coupled.",
				RequiredAccessPointKey: null,
				TowType: "road",
				CanTow: true,
				CanBeTowed: false,
				MaximumTowedWeight: 6300000,
				CharacterPullMultiplier: 1,
				TowStressWarningRatio: 0.85,
				TowStressFailureStartRatio: 1,
				TowStressMaximumFailureChance: 0.15,
				TowStressDamageMultiplier: 1,
				DisplayOrder: 1
			)
		],
		DamageZones: [
			new VehicleDamageZoneSeedSpec(
				Key: "body",
				Name: "body and chassis",
				Description: "Serious structural damage compromises the vehicle as a whole.",
				MaximumDamage: 500,
				HitWeight: 3,
				DisabledThreshold: 0.7,
				DestroyedThreshold: 1,
				DisablesMovement: true,
				DisplayOrder: 0,
				Effects: [
					new VehicleDamageEffectSeedSpec(
						TargetType: VehicleDamageEffectTargetType.WholeVehicleMovement,
						TargetKey: null,
						MinimumStatus: VehicleSystemStatus.Disabled
					)
				]
			),
			new VehicleDamageZoneSeedSpec(
				Key: "running_gear",
				Name: "wheels and running gear",
				Description: "Damage to wheels, suspension or steering prevents controlled road movement.",
				MaximumDamage: 300,
				HitWeight: 2,
				DisabledThreshold: 0.55,
				DestroyedThreshold: 1,
				DisablesMovement: false,
				DisplayOrder: 1,
				Effects: [
					new VehicleDamageEffectSeedSpec(
						TargetType: VehicleDamageEffectTargetType.MovementProfile,
						TargetKey: "route",
						MinimumStatus: VehicleSystemStatus.Disabled
					)
				]
			),
			new VehicleDamageZoneSeedSpec(
				Key: "drive",
				Name: "drive module and mount",
				Description: "Damage around the drive bay can disable the installed propulsion module.",
				MaximumDamage: 220,
				HitWeight: 1.5,
				DisabledThreshold: 0.55,
				DestroyedThreshold: 1,
				DisablesMovement: false,
				DisplayOrder: 2,
				Effects: [
					new VehicleDamageEffectSeedSpec(
						TargetType: VehicleDamageEffectTargetType.InstallationPoint,
						TargetKey: "drive_module",
						MinimumStatus: VehicleSystemStatus.Disabled
					)
				]
			)
		],
		ProvidesPassengerService: true,
		ProvidesCargoService: true,
		BuilderNotes: "Install the seeded electric drive module and charge it with compatible batteries before movement.",
		AdditionalEraKeys: null
	);
}
