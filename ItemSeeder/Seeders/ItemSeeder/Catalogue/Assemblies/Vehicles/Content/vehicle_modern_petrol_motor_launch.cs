#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace DatabaseSeeder.Seeders;

public static partial class VehicleStock
{
	public static VehicleSeedSpec VehicleModernPetrolMotorLaunch() =>
		new VehicleSeedSpec(
		StableReference: "vehicle_modern_petrol_motor_launch",
		EraKey: "modern",
		Domain: "Aquatic",
		Archetype: "MotorLaunch",
		Name: "Petrol Motor Launch",
		Description: "A compact open launch prepared for an outboard motor and sheltered-water service.",
		Scale: VehicleScale.RoomContainer,
		ExteriorItem: new VehicleItemSeedSpec(
			Noun: "launch",
			ShortDescription: "a compact petrol motor launch",
			FullDescription: "A hard-chined painted hull surrounds an open cockpit with two bench seats and a small forward locker. The transom is heavily reinforced for an outboard motor, and simple wheel-and-cable steering leads to the stern. Cleats, grab rails and a low spray screen equip the launch for practical transport rather than luxury.",
			Size: SizeCategory.Gigantic,
			Quality: ItemQuality.Standard,
			WeightInGrams: 780000,
			Cost: 12500.0m,
			Material: "mild steel",
			DestroyableComponent: "Destroyable_HeavyMetal",
			Portable: false,
			Skinnable: true,
			HiddenFromPlayers: false,
			LongDescription: null
		),
		Compartments: [
			new VehicleCompartmentSeedSpec(
				Key: "cockpit",
				Name: "cockpit",
				Description: "The open cockpit contains the helm and principal working space.",
				DisplayOrder: 0,
				InteriorTerrainId: null,
				InteriorOutdoorsType: 0
			),
			new VehicleCompartmentSeedSpec(
				Key: "cabin",
				Name: "cabin",
				Description: "A compact enclosed cabin provides sheltered seating and stowage.",
				DisplayOrder: 1,
				InteriorTerrainId: null,
				InteriorOutdoorsType: 0
			)
		],
		CompartmentLinks: [
			new VehicleCompartmentLinkSeedSpec(
				Key: "cockpit_to_cabin",
				SourceCompartmentKey: "cockpit",
				DestinationCompartmentKey: "cabin",
				OutboundDirection: "forward",
				InboundDirection: "aft",
				OutboundDescription: "through the cabin door",
				InboundDescription: "out through the cabin door"
			)
		],
		OccupantSlots: [
			new VehicleOccupantSlotSeedSpec(
				Key: "driver",
				Name: "helm seat",
				CompartmentKey: "cockpit",
				SlotType: VehicleOccupantSlotType.Driver,
				Capacity: 1,
				RequiredForMovement: true,
				ContributesToPropulsion: false,
				BoatStabilityDifficulty: Difficulty.Easy
			),
			new VehicleOccupantSlotSeedSpec(
				Key: "rowers",
				Name: "emergency rowing places",
				CompartmentKey: "cockpit",
				SlotType: VehicleOccupantSlotType.Crew,
				Capacity: 2,
				RequiredForMovement: false,
				ContributesToPropulsion: true,
				BoatStabilityDifficulty: Difficulty.Normal
			),
			new VehicleOccupantSlotSeedSpec(
				Key: "passengers",
				Name: "passenger places",
				CompartmentKey: "cabin",
				SlotType: VehicleOccupantSlotType.Passenger,
				Capacity: 5,
				RequiredForMovement: false,
				ContributesToPropulsion: false,
				BoatStabilityDifficulty: Difficulty.Normal
			)
		],
		ControlStations: [
			new VehicleControlStationSeedSpec(
				Key: "primary",
				Name: "helm controls",
				OccupantSlotKey: "driver",
				IsPrimary: true
			)
		],
		MovementProfiles: [
			new VehicleMovementProfileSeedSpec(
				Key: "water",
				Name: "motor and emergency rowing profile",
				MovementType: VehicleMovementProfileType.RoomExit,
				Environment: VehicleMovementEnvironment.SurfaceWater,
				ExposesOccupantsToWater: false,
				IsDefault: true,
				RequiredPowerSpikeInWatts: 0,
				MinimumEnginePowerInWatts: 0,
				FuelLiquid: null,
				FuelVolumePerMove: 0,
				RequiredInstalledRole: "",
				RequiresTowLinksClosed: true,
				RequiresAccessPointsClosed: true,
				RouteSpeedMetresPerSecond: 0,
				RoutePropulsionMode: RouteVehiclePropulsionMode.Powered,
				RouteFuelVolumePerMetre: 0,
				RoutePowerDrawWatts: 0,
				AutomaticOperationCapable: false,
				PropulsionProfiles: [
					new VehiclePropulsionSeedSpec(
						PropulsionType: VehiclePropulsionType.OutboardMotor,
						IsDefault: true,
						BaseMoveTimeMilliseconds: 5200,
						TraitCandidates: [],
						CheckDifficulty: Difficulty.Easy,
						SpeedMultiplierExpression: "max(0.25, output)",
						StaminaCostExpression: "0.0",
						RiderStaminaMultiplier: 1
					),
					new VehiclePropulsionSeedSpec(
						PropulsionType: VehiclePropulsionType.Rowed,
						IsDefault: false,
						BaseMoveTimeMilliseconds: 9000,
						TraitCandidates: [
							"Rowing",
							"Swimming",
							"Athletics"
						],
						CheckDifficulty: Difficulty.Normal,
						SpeedMultiplierExpression: "1.0 + (0.15 * outcome)",
						StaminaCostExpression: "max(0.5, swimcost * (1.0 - (0.05 * outcome)))",
						RiderStaminaMultiplier: 1
					)
				]
			)
		],
		AccessPoints: [
			new VehicleAccessPointSeedSpec(
				Key: "cabin_door",
				Name: "cabin door",
				Description: "A fitted weatherproof door closes the vessel's small cabin.",
				CompartmentKey: "cabin",
				AccessPointType: VehicleAccessPointType.Door,
				StartsOpen: false,
				MustBeClosedForMovement: true,
				DisplayOrder: 0,
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "door",
					ShortDescription: "the cabin door of a compact petrol motor launch",
					FullDescription: "This fitted weatherproof door and frame are integral to the cabin of a compact petrol motor launch.",
					Size: SizeCategory.Large,
					Quality: ItemQuality.Standard,
					WeightInGrams: 1,
					Cost: 0.0m,
					Material: "mild steel",
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
				Key: "locker",
				Name: "cabin locker",
				Description: "A built-in locker occupies part of the sheltered cabin.",
				CompartmentKey: "cabin",
				RequiredAccessPointKey: "cabin_door",
				DisplayOrder: 0,
				ContainerComponent: "Container_Trunk",
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "locker",
					ShortDescription: "the built-in locker of a compact petrol motor launch",
					FullDescription: "This hidden projection represents the fixed storage locker built into a compact petrol motor launch.",
					Size: SizeCategory.Huge,
					Quality: ItemQuality.Standard,
					WeightInGrams: 1,
					Cost: 0.0m,
					Material: "mild steel",
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
				Key: "outboard",
				Name: "outboard transom mount",
				Description: "A reinforced transom mount accepts a compatible outboard propulsion module.",
				RequiredAccessPointKey: null,
				MountType: "outboard_motor",
				RequiredRole: "propulsion",
				RequiredForMovement: false,
				DisplayOrder: 0
			)
		],
		TowPoints: [
			new VehicleTowPointSeedSpec(
				Key: "bow_tow",
				Name: "bow towing eye",
				Description: "A rated bow eye accepts recovery and towing lines.",
				RequiredAccessPointKey: null,
				TowType: "marine",
				CanTow: true,
				CanBeTowed: true,
				MaximumTowedWeight: 1560000,
				CharacterPullMultiplier: 1,
				TowStressWarningRatio: 0.85,
				TowStressFailureStartRatio: 1,
				TowStressMaximumFailureChance: 0.15,
				TowStressDamageMultiplier: 1,
				DisplayOrder: 0
			)
		],
		DamageZones: [
			new VehicleDamageZoneSeedSpec(
				Key: "hull",
				Name: "hull",
				Description: "Hull damage threatens buoyancy and eventually prevents all movement.",
				MaximumDamage: 420,
				HitWeight: 4,
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
				Key: "propulsion",
				Name: "propulsion fittings",
				Description: "Damage to propulsion fittings prevents the selected movement system from working.",
				MaximumDamage: 180,
				HitWeight: 1.5,
				DisabledThreshold: 0.55,
				DestroyedThreshold: 1,
				DisablesMovement: false,
				DisplayOrder: 1,
				Effects: [
					new VehicleDamageEffectSeedSpec(
						TargetType: VehicleDamageEffectTargetType.InstallationPoint,
						TargetKey: "outboard",
						MinimumStatus: VehicleSystemStatus.Disabled
					)
				]
			)
		],
		ProvidesPassengerService: true,
		ProvidesCargoService: true,
		BuilderNotes: "Install the seeded outboard motor on the transom mount and fill it with gasoline. Emergency rowing remains selectable when staffed rowers hold vehicle oars.",
		AdditionalEraKeys: null
	);
}
