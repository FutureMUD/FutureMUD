#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace DatabaseSeeder.Seeders;

public static partial class VehicleStock
{
	public static VehicleSeedSpec VehiclePreindustrialRiverPunt() =>
		new VehicleSeedSpec(
		StableReference: "vehicle_preindustrial_river_punt",
		EraKey: "antiquity",
		Domain: "Aquatic",
		Archetype: "RiverPunt",
		Name: "River Punt",
		Description: "A shallow flat-bottomed punt for poles, paddles and quiet inland water.",
		Scale: VehicleScale.ItemScale,
		ExteriorItem: new VehicleItemSeedSpec(
			Noun: "punt",
			ShortDescription: "a broad flat-bottomed river punt",
			FullDescription: "A rectangular flat bottom rises into low, nearly vertical sides around a broad open working space. Short decks strengthen both ends, and a long setting pole lies beside plain bench seats and a single steering oar. The shallow draught and sacrificial rubbing strips suit reed beds, ferries, fish ponds and muddy river margins.",
			Size: SizeCategory.Enormous,
			Quality: ItemQuality.Standard,
			WeightInGrams: 360000,
			Cost: 1100.0m,
			Material: "oak",
			DestroyableComponent: "Destroyable_WoodenHeavy",
			Portable: false,
			Skinnable: true,
			HiddenFromPlayers: false,
			LongDescription: null
		),
		Compartments: [
			new VehicleCompartmentSeedSpec(
				Key: "hull",
				Name: "open hull",
				Description: "The occupied space lies within the craft's open hull.",
				DisplayOrder: 0,
				InteriorTerrainId: null,
				InteriorOutdoorsType: 0
			)
		],
		CompartmentLinks: [],
		OccupantSlots: [
			new VehicleOccupantSlotSeedSpec(
				Key: "driver",
				Name: "paddler's place",
				CompartmentKey: "hull",
				SlotType: VehicleOccupantSlotType.Driver,
				Capacity: 1,
				RequiredForMovement: true,
				ContributesToPropulsion: true,
				BoatStabilityDifficulty: Difficulty.Normal
			)
		],
		ControlStations: [
			new VehicleControlStationSeedSpec(
				Key: "primary",
				Name: "paddling position",
				OccupantSlotKey: "driver",
				IsPrimary: true
			)
		],
		MovementProfiles: [
			new VehicleMovementProfileSeedSpec(
				Key: "water",
				Name: "surface-water travel",
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
				RequiresAccessPointsClosed: false,
				RouteSpeedMetresPerSecond: 0,
				RoutePropulsionMode: RouteVehiclePropulsionMode.Powered,
				RouteFuelVolumePerMetre: 0,
				RoutePowerDrawWatts: 0,
				AutomaticOperationCapable: false,
				PropulsionProfiles: [
					new VehiclePropulsionSeedSpec(
						PropulsionType: VehiclePropulsionType.SelfPowered,
						IsDefault: true,
						BaseMoveTimeMilliseconds: 8500,
						TraitCandidates: [
							"Swimming",
							"Rowing",
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
		AccessPoints: [],
		CargoSpaces: [
			new VehicleCargoSpaceSeedSpec(
				Key: "cargo",
				Name: "open stowage",
				Description: "Open floor space between the seats can hold modest cargo while keeping the craft balanced.",
				CompartmentKey: "hull",
				RequiredAccessPointKey: null,
				DisplayOrder: 0,
				ContainerComponent: "Container_Open_Bin",
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "cargo",
					ShortDescription: "the open stowage of a broad flat-bottomed river punt",
					FullDescription: "This hidden projection represents the usable open stowage inside a broad flat-bottomed river punt.",
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
				Key: "bow_tow",
				Name: "bow towing eye",
				Description: "A reinforced eye at the bow accepts a line for towing or recovery.",
				RequiredAccessPointKey: null,
				TowType: "marine",
				CanTow: true,
				CanBeTowed: true,
				MaximumTowedWeight: 1080000,
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
						TargetType: VehicleDamageEffectTargetType.MovementProfile,
						TargetKey: "water",
						MinimumStatus: VehicleSystemStatus.Disabled
					)
				]
			)
		],
		ProvidesPassengerService: false,
		ProvidesCargoService: true,
		BuilderNotes: "The controller supplies self-powered aquatic propulsion; a Swimming, Rowing or Athletics trait must exist.",
		AdditionalEraKeys: [
			"medieval",
			"renaissance",
			"earlymodern"
		]
	);
}
