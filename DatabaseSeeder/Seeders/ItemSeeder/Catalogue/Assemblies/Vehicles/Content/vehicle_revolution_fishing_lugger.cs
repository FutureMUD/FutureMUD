#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace DatabaseSeeder.Seeders;

public static partial class VehicleStock
{
	public static VehicleSeedSpec VehicleRevolutionFishingLugger() =>
		new VehicleSeedSpec(
		StableReference: "vehicle_revolution_fishing_lugger",
		EraKey: "revolution",
		Domain: "Aquatic",
		Archetype: "IndustrialSailingVessel",
		Name: "Fishing Lugger",
		Description: "A sail-driven fishing lugger for industrial-era coastal and harbour work.",
		Scale: VehicleScale.RoomContainer,
		ExteriorItem: new VehicleItemSeedSpec(
			Noun: "vessel",
			ShortDescription: "a working fishing lugger",
			FullDescription: "A practical planked hull supports the masts, rigging and broad working deck of this fishing lugger. Its deck fittings favour cargo, passengers and repeated coastal service over decorative finish.",
			Size: SizeCategory.Titanic,
			Quality: ItemQuality.Standard,
			WeightInGrams: 4200000,
			Cost: 28500.0m,
			Material: "oak",
			DestroyableComponent: "Destroyable_WoodenHeavy",
			Portable: false,
			Skinnable: true,
			HiddenFromPlayers: false,
			LongDescription: null
		),
		Compartments: [
			new VehicleCompartmentSeedSpec(
				Key: "deck",
				Name: "working deck",
				Description: "The exposed working deck carries the helm, rigging and occupied stations.",
				DisplayOrder: 0,
				InteriorTerrainId: null,
				InteriorOutdoorsType: 0
			),
			new VehicleCompartmentSeedSpec(
				Key: "hold",
				Name: "lower hold",
				Description: "A lower enclosed compartment lies beneath the main deck.",
				DisplayOrder: 1,
				InteriorTerrainId: null,
				InteriorOutdoorsType: 0
			)
		],
		CompartmentLinks: [
			new VehicleCompartmentLinkSeedSpec(
				Key: "deck_to_hold",
				SourceCompartmentKey: "deck",
				DestinationCompartmentKey: "hold",
				OutboundDirection: "down",
				InboundDirection: "up",
				OutboundDescription: "through the cargo hatch",
				InboundDescription: "up through the cargo hatch"
			)
		],
		OccupantSlots: [
			new VehicleOccupantSlotSeedSpec(
				Key: "driver",
				Name: "helm",
				CompartmentKey: "deck",
				SlotType: VehicleOccupantSlotType.Driver,
				Capacity: 1,
				RequiredForMovement: true,
				ContributesToPropulsion: false,
				BoatStabilityDifficulty: Difficulty.Normal
			),
			new VehicleOccupantSlotSeedSpec(
				Key: "crew",
				Name: "working crew stations",
				CompartmentKey: "deck",
				SlotType: VehicleOccupantSlotType.Crew,
				Capacity: 5,
				RequiredForMovement: false,
				ContributesToPropulsion: true,
				BoatStabilityDifficulty: Difficulty.Normal
			),
			new VehicleOccupantSlotSeedSpec(
				Key: "passengers",
				Name: "passenger places",
				CompartmentKey: "deck",
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
				Name: "helm",
				OccupantSlotKey: "driver",
				IsPrimary: true
			)
		],
		MovementProfiles: [
			new VehicleMovementProfileSeedSpec(
				Key: "water",
				Name: "sailing profile",
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
						PropulsionType: VehiclePropulsionType.Sail,
						IsDefault: true,
						BaseMoveTimeMilliseconds: 7800,
						TraitCandidates: [],
						CheckDifficulty: Difficulty.Normal,
						SpeedMultiplierExpression: "0.4 + (0.2 * wind)",
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
				Key: "hold_hatch",
				Name: "cargo hatch",
				Description: "A stout deck hatch opens into the lower hold.",
				CompartmentKey: "hold",
				AccessPointType: VehicleAccessPointType.Hatch,
				StartsOpen: false,
				MustBeClosedForMovement: true,
				DisplayOrder: 0,
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "hatch",
					ShortDescription: "the cargo hatch of a working fishing lugger",
					FullDescription: "This heavy fitted hatch closes the cargo opening in a working fishing lugger; its coaming, hinges and securing bars are part of the vessel.",
					Size: SizeCategory.VeryLarge,
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
		CargoSpaces: [
			new VehicleCargoSpaceSeedSpec(
				Key: "hold",
				Name: "cargo hold",
				Description: "The lower hold provides broad stowage beneath the working deck.",
				CompartmentKey: "hold",
				RequiredAccessPointKey: "hold_hatch",
				DisplayOrder: 0,
				ContainerComponent: "Container_Colossal",
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "hold",
					ShortDescription: "the cargo hold of a working fishing lugger",
					FullDescription: "This hidden projection represents the fixed lower cargo hold of a working fishing lugger and is accessed through its deck hatch.",
					Size: SizeCategory.Enormous,
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
				Name: "bow towing bitt",
				Description: "A strong forward bitt accepts towing hawsers and mooring lines.",
				RequiredAccessPointKey: null,
				TowType: "marine",
				CanTow: true,
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
				Key: "stern_tow",
				Name: "stern towing bitt",
				Description: "A strong after bitt permits another vessel or floating load to be towed astern.",
				RequiredAccessPointKey: null,
				TowType: "marine",
				CanTow: true,
				CanBeTowed: true,
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
				Key: "hull",
				Name: "hull",
				Description: "Serious hull damage compromises buoyancy and the vessel as a whole.",
				MaximumDamage: 900,
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
				Key: "rig",
				Name: "mast, sails and rigging",
				Description: "Damage to the sailing rig prevents effective propulsion.",
				MaximumDamage: 420,
				HitWeight: 2,
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
			),
			new VehicleDamageZoneSeedSpec(
				Key: "hold",
				Name: "hold and hatch",
				Description: "Damage around the hold can jam its hatch or make cargo inaccessible.",
				MaximumDamage: 300,
				HitWeight: 1,
				DisabledThreshold: 0.6,
				DestroyedThreshold: 1,
				DisablesMovement: false,
				DisplayOrder: 2,
				Effects: [
					new VehicleDamageEffectSeedSpec(
						TargetType: VehicleDamageEffectTargetType.AccessPoint,
						TargetKey: "hold_hatch",
						MinimumStatus: VehicleSystemStatus.Disabled
					),
					new VehicleDamageEffectSeedSpec(
						TargetType: VehicleDamageEffectTargetType.CargoSpace,
						TargetKey: "hold",
						MinimumStatus: VehicleSystemStatus.Disabled
					)
				]
			)
		],
		ProvidesPassengerService: true,
		ProvidesCargoService: true,
		BuilderNotes: "Sail is the default propulsion mode and requires wind. Rowing is available as a fallback only when staffed propulsion slots have usable oars.",
		AdditionalEraKeys: null
	);
}
