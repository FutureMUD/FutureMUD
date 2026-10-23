#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace DatabaseSeeder.Seeders;

public static partial class VehicleStock
{
	public static VehicleSeedSpec VehicleAntiquityHeavyOxWagon() =>
		new VehicleSeedSpec(
		StableReference: "vehicle_antiquity_heavy_ox_wagon",
		EraKey: "antiquity",
		Domain: "Terrestrial",
		Archetype: "DraftWagon",
		Name: "Heavy Ox Wagon",
		Description: "A broad four-wheeled wagon intended for slow, heavy overland haulage.",
		Scale: VehicleScale.RoomContainer,
		ExteriorItem: new VehicleItemSeedSpec(
			Noun: "wagon",
			ShortDescription: "a broad timber ox wagon",
			FullDescription: "Four thick wooden wheels support a broad, deeply railed wagon bed. A heavy pole and yoke fittings project from the front axle, while iron straps bind the most highly stressed joints. The body is deliberately massive and plain, suited to sacks, amphorae and timber rather than speed.",
			Size: SizeCategory.Enormous,
			Quality: ItemQuality.Standard,
			WeightInGrams: 650000,
			Cost: 2400.0m,
			Material: "oak",
			DestroyableComponent: "Destroyable_WoodenHeavy",
			Portable: false,
			Skinnable: true,
			HiddenFromPlayers: false,
			LongDescription: null
		),
		Compartments: [
			new VehicleCompartmentSeedSpec(
				Key: "main",
				Name: "platform",
				Description: "The principal compartment is an open working platform.",
				DisplayOrder: 0,
				InteriorTerrainId: null,
				InteriorOutdoorsType: 0
			)
		],
		CompartmentLinks: [],
		OccupantSlots: [
			new VehicleOccupantSlotSeedSpec(
				Key: "driver",
				Name: "driver's place",
				CompartmentKey: "main",
				SlotType: VehicleOccupantSlotType.Driver,
				Capacity: 1,
				RequiredForMovement: true,
				ContributesToPropulsion: false,
				BoatStabilityDifficulty: Difficulty.Normal
			),
			new VehicleOccupantSlotSeedSpec(
				Key: "passengers",
				Name: "passenger places",
				CompartmentKey: "main",
				SlotType: VehicleOccupantSlotType.Passenger,
				Capacity: 2,
				RequiredForMovement: false,
				ContributesToPropulsion: false,
				BoatStabilityDifficulty: Difficulty.Normal
			)
		],
		ControlStations: [
			new VehicleControlStationSeedSpec(
				Key: "primary",
				Name: "driving position",
				OccupantSlotKey: "driver",
				IsPrimary: true
			)
		],
		MovementProfiles: [
			new VehicleMovementProfileSeedSpec(
				Key: "road",
				Name: "ordinary road travel",
				MovementType: VehicleMovementProfileType.RoomExit,
				Environment: VehicleMovementEnvironment.Unrestricted,
				ExposesOccupantsToWater: false,
				IsDefault: true,
				RequiredPowerSpikeInWatts: 0,
				MinimumEnginePowerInWatts: 0,
				FuelLiquid: null,
				FuelVolumePerMove: 0,
				RequiredInstalledRole: "",
				RequiresTowLinksClosed: true,
				RequiresAccessPointsClosed: false,
				RouteSpeedMetresPerSecond: 0,
				RoutePropulsionMode: RouteVehiclePropulsionMode.Powered,
				RouteFuelVolumePerMetre: 0,
				RoutePowerDrawWatts: 0,
				AutomaticOperationCapable: false,
				PropulsionProfiles: [
					new VehiclePropulsionSeedSpec(
						PropulsionType: VehiclePropulsionType.ExternallyPulled,
						IsDefault: true,
						BaseMoveTimeMilliseconds: 8000,
						TraitCandidates: [],
						CheckDifficulty: Difficulty.Normal,
						SpeedMultiplierExpression: "1",
						StaminaCostExpression: "0",
						RiderStaminaMultiplier: 1
					)
				]
			)
		],
		AccessPoints: [],
		CargoSpaces: [
			new VehicleCargoSpaceSeedSpec(
				Key: "cargo",
				Name: "wagon bed",
				Description: "The wagon bed forms a broad open platform enclosed by waist-high slatted sides.",
				CompartmentKey: "main",
				RequiredAccessPointKey: null,
				DisplayOrder: 0,
				ContainerComponent: "Container_Open_Bin",
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "cargo",
					ShortDescription: "the wagon bed of a broad timber ox wagon",
					FullDescription: "This cargo projection represents the wagon bed built into a broad timber ox wagon. It follows the dimensions and access of the vehicle rather than existing as a separate container.",
					Size: SizeCategory.Huge,
					Quality: ItemQuality.Standard,
					WeightInGrams: 1,
					Cost: 0.0m,
					Material: "oak",
					DestroyableComponent: "Destroyable_WoodenHeavy",
					Portable: false,
					Skinnable: false,
					HiddenFromPlayers: true,
					LongDescription: null
				)
			)
		],
		InstallationPoints: [],
		TowPoints: [
			new VehicleTowPointSeedSpec(
				Key: "front_tow",
				Name: "front draft point",
				Description: "A reinforced forward draft point accepts compatible harness or traces.",
				RequiredAccessPointKey: null,
				TowType: "draft",
				CanTow: false,
				CanBeTowed: true,
				MaximumTowedWeight: 5000000,
				CharacterPullMultiplier: 1.1,
				TowStressWarningRatio: 0.8,
				TowStressFailureStartRatio: 1,
				TowStressMaximumFailureChance: 0.2,
				TowStressDamageMultiplier: 1,
				DisplayOrder: 0
			),
			new VehicleTowPointSeedSpec(
				Key: "rear_tow",
				Name: "rear tow point",
				Description: "A reinforced rear point allows another light vehicle or load to be coupled behind.",
				RequiredAccessPointKey: null,
				TowType: "draft",
				CanTow: true,
				CanBeTowed: false,
				MaximumTowedWeight: 3750000,
				CharacterPullMultiplier: 1,
				TowStressWarningRatio: 0.8,
				TowStressFailureStartRatio: 1,
				TowStressMaximumFailureChance: 0.2,
				TowStressDamageMultiplier: 1,
				DisplayOrder: 1
			)
		],
		DamageZones: [
			new VehicleDamageZoneSeedSpec(
				Key: "frame",
				Name: "frame and body",
				Description: "Damage to the principal frame can disable or destroy the whole vehicle.",
				MaximumDamage: 500,
				HitWeight: 3,
				DisabledThreshold: 0.65,
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
				Description: "Damage to the wheels, axle or running gear prevents controlled travel.",
				MaximumDamage: 200,
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
			)
		],
		ProvidesPassengerService: true,
		ProvidesCargoService: true,
		BuilderNotes: "Externally pulled terrestrial example. Fit compatible hitch gear and pullers before movement; route variants require a RouteCell.",
		AdditionalEraKeys: null
	);
}
