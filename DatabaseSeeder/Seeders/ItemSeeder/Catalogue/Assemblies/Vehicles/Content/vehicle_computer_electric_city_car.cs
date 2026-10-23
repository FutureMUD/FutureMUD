#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace DatabaseSeeder.Seeders;

public static partial class VehicleStock
{
	public static VehicleSeedSpec VehicleComputerElectricCityCar() =>
		new VehicleSeedSpec(
		StableReference: "vehicle_computer_electric_city_car",
		EraKey: "computer",
		Domain: "Terrestrial",
		Archetype: "ElectricCityCar",
		Name: "Electric City Car",
		Description: "A compact aluminium city car powered by a removable electric drive module.",
		Scale: VehicleScale.RoomContainer,
		ExteriorItem: new VehicleItemSeedSpec(
			Noun: "car",
			ShortDescription: "a compact electric city car",
			FullDescription: "A short aluminium body encloses a tall four-seat cabin with a steep windscreen and minimal overhangs. Flush lamps, simple door handles and a sealed nose replace the openings expected on a combustion vehicle. A digital instrument panel faces the driver, while a rear hatch opens onto a small but regular load space.",
			Size: SizeCategory.Gigantic,
			Quality: ItemQuality.Good,
			WeightInGrams: 1380000,
			Cost: 42000.0m,
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
				RequiredForMovement: true,
				ContributesToPropulsion: false,
				BoatStabilityDifficulty: Difficulty.Normal
			),
			new VehicleOccupantSlotSeedSpec(
				Key: "passengers",
				Name: "passenger seats",
				CompartmentKey: "cabin",
				SlotType: VehicleOccupantSlotType.Passenger,
				Capacity: 4,
				RequiredForMovement: false,
				ContributesToPropulsion: false,
				BoatStabilityDifficulty: Difficulty.Normal
			)
		],
		ControlStations: [
			new VehicleControlStationSeedSpec(
				Key: "primary",
				Name: "driving controls",
				OccupantSlotKey: "driver",
				IsPrimary: true
			)
		],
		MovementProfiles: [
			new VehicleMovementProfileSeedSpec(
				Key: "road",
				Name: "powered road travel",
				MovementType: VehicleMovementProfileType.RoomExit,
				Environment: VehicleMovementEnvironment.Unrestricted,
				ExposesOccupantsToWater: false,
				IsDefault: true,
				RequiredPowerSpikeInWatts: 0,
				MinimumEnginePowerInWatts: 41400,
				FuelLiquid: null,
				FuelVolumePerMove: 0,
				RequiredInstalledRole: "propulsion",
				RequiresTowLinksClosed: true,
				RequiresAccessPointsClosed: true,
				RouteSpeedMetresPerSecond: 0,
				RoutePropulsionMode: RouteVehiclePropulsionMode.Powered,
				RouteFuelVolumePerMetre: 0,
				RoutePowerDrawWatts: 0,
				AutomaticOperationCapable: false,
				PropulsionProfiles: [
					new VehiclePropulsionSeedSpec(
						PropulsionType: VehiclePropulsionType.Engine,
						IsDefault: true,
						BaseMoveTimeMilliseconds: 8000,
						TraitCandidates: [],
						CheckDifficulty: Difficulty.Normal,
						SpeedMultiplierExpression: "power / requiredpower",
						StaminaCostExpression: "0",
						RiderStaminaMultiplier: 1
					)
				]
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
					ShortDescription: "the passenger doors of a compact electric city car",
					FullDescription: "These fitted doors and their surrounding frame are integral to a compact electric city car. Handles, latches and seals are arranged for repeated passenger access.",
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
				Name: "luggage compartment",
				Description: "A separate enclosed compartment carries luggage and small personal cargo.",
				CompartmentKey: "cabin",
				RequiredAccessPointKey: "doors",
				DisplayOrder: 0,
				ContainerComponent: "Container_Trunk",
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "cargo",
					ShortDescription: "the cargo compartment of a compact electric city car",
					FullDescription: "This hidden projection represents the built-in cargo compartment of a compact electric city car, including its fixed floor, sides and loading access.",
					Size: SizeCategory.Huge,
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
				MaximumTowedWeight: 2760000,
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
				MaximumTowedWeight: 2070000,
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
						TargetKey: "road",
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
