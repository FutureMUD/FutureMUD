#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace DatabaseSeeder.Seeders;

public static partial class VehicleStock
{
	public static VehicleSeedSpec VehicleAtomicCabinCruiser() =>
		new VehicleSeedSpec(
		StableReference: "vehicle_atomic_cabin_cruiser",
		EraKey: "atomic",
		Domain: "Aquatic",
		Archetype: "CabinCruiser",
		Name: "Cabin Cruiser",
		Description: "A compact leisure cruiser with an enclosed cabin, cockpit and outboard installation.",
		Scale: VehicleScale.RoomContainer,
		ExteriorItem: new VehicleItemSeedSpec(
			Noun: "cruiser",
			ShortDescription: "a compact fiberglass cabin cruiser",
			FullDescription: "A moulded white hull supports an open aft cockpit and a low enclosed cabin beneath a raked windscreen. Stainless rails follow the foredeck, while cushioned seating and a small covered berth make the boat suitable for overnight coastal trips. The broad stern carries a reinforced motor well and a narrow swim platform.",
			Size: SizeCategory.Titanic,
			Quality: ItemQuality.Good,
			WeightInGrams: 3400000,
			Cost: 52000.0m,
			Material: "fiberglass",
			DestroyableComponent: "Destroyable_Misc",
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
				Capacity: 7,
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
					ShortDescription: "the cabin door of a compact fiberglass cabin cruiser",
					FullDescription: "This fitted weatherproof door and frame are integral to the cabin of a compact fiberglass cabin cruiser.",
					Size: SizeCategory.Large,
					Quality: ItemQuality.Standard,
					WeightInGrams: 1,
					Cost: 0.0m,
					Material: "fiberglass",
					DestroyableComponent: "Destroyable_Misc",
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
					ShortDescription: "the built-in locker of a compact fiberglass cabin cruiser",
					FullDescription: "This hidden projection represents the fixed storage locker built into a compact fiberglass cabin cruiser.",
					Size: SizeCategory.Huge,
					Quality: ItemQuality.Standard,
					WeightInGrams: 1,
					Cost: 0.0m,
					Material: "fiberglass",
					DestroyableComponent: "Destroyable_Misc",
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
				MaximumTowedWeight: 6800000,
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
