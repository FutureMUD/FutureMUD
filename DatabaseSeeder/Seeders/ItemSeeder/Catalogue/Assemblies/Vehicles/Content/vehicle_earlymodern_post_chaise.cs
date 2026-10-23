#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace DatabaseSeeder.Seeders;

public static partial class VehicleStock
{
	public static VehicleSeedSpec VehicleEarlymodernPostChaise() =>
		new VehicleSeedSpec(
		StableReference: "vehicle_earlymodern_post_chaise",
		EraKey: "earlymodern",
		Domain: "Terrestrial",
		Archetype: "PostChaise",
		Name: "Post Chaise",
		Description: "A light enclosed travelling carriage intended for rapid changes of hired horses.",
		Scale: VehicleScale.RoomContainer,
		ExteriorItem: new VehicleItemSeedSpec(
			Noun: "chaise",
			ShortDescription: "a light yellow post chaise",
			FullDescription: "A compact enclosed body sits low between four narrow wheels on flexible springs. A forward-facing upholstered bench fills the cabin, while travel cases strap to a small rear platform beneath a folding hood. Light shafts, prominent lamps and fittings for postilion-driven teams emphasise speed between posting houses rather than heavy carrying.",
			Size: SizeCategory.Enormous,
			Quality: ItemQuality.Good,
			WeightInGrams: 760000,
			Cost: 15400.0m,
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
				Name: "interior",
				Description: "The principal compartment is enclosed against weather and road dirt.",
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
				Capacity: 3,
				RequiredForMovement: false,
				ContributesToPropulsion: false,
				BoatStabilityDifficulty: Difficulty.Normal
			)
		],
		ControlStations: [
			new VehicleControlStationSeedSpec(
				Key: "primary",
				Name: "driver's box",
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
		AccessPoints: [
			new VehicleAccessPointSeedSpec(
				Key: "rear_access",
				Name: "rear access",
				Description: "A closable rear opening provides access to the vehicle's interior.",
				CompartmentKey: "main",
				AccessPointType: VehicleAccessPointType.Door,
				StartsOpen: false,
				MustBeClosedForMovement: true,
				DisplayOrder: 0,
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "door",
					ShortDescription: "the rear access of a light yellow post chaise",
					FullDescription: "This fitted rear closure belongs to a light yellow post chaise. Its hinges, fastenings and framing are integral to the vehicle.",
					Size: SizeCategory.Large,
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
				Key: "cargo",
				Name: "travelling-case rack",
				Description: "A compact rear rack carries fitted travelling cases and mail bags.",
				CompartmentKey: "main",
				RequiredAccessPointKey: "rear_access",
				DisplayOrder: 0,
				ContainerComponent: "Container_Trunk",
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "cargo",
					ShortDescription: "the travelling-case rack of a light yellow post chaise",
					FullDescription: "This cargo projection represents the travelling-case rack built into a light yellow post chaise. It follows the dimensions and access of the vehicle rather than existing as a separate container.",
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
				MaximumTowedWeight: 4200000,
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
				MaximumTowedWeight: 3150000,
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
				MaximumDamage: 420,
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
				MaximumDamage: 168,
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
