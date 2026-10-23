#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace DatabaseSeeder.Seeders;

public static partial class VehicleStock
{
	public static VehicleSeedSpec VehicleMedievalMarketCart() =>
		new VehicleSeedSpec(
		StableReference: "vehicle_medieval_market_cart",
		EraKey: "medieval",
		Domain: "Terrestrial",
		Archetype: "MarketCart",
		Name: "Market Cart",
		Description: "A compact two-wheeled market cart with removable sideboards.",
		Scale: VehicleScale.ItemScale,
		ExteriorItem: new VehicleItemSeedSpec(
			Noun: "cart",
			ShortDescription: "a compact oak market cart",
			FullDescription: "This compact cart rides on two iron-rimmed wheels beneath a shallow oak bed. Removable sideboards slot into upright stakes, allowing the same vehicle to carry baskets, firewood or loose produce. A pair of polished shafts and a simple hand brake show the care expected of a working town cart.",
			Size: SizeCategory.Huge,
			Quality: ItemQuality.Good,
			WeightInGrams: 140000,
			Cost: 850.0m,
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
				Capacity: 1,
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
				Name: "market bed",
				Description: "The shallow cart bed is enclosed by removable slatted sideboards.",
				CompartmentKey: "main",
				RequiredAccessPointKey: null,
				DisplayOrder: 0,
				ContainerComponent: "Container_Open_Bin",
				ProjectionItem: new VehicleItemSeedSpec(
					Noun: "cargo",
					ShortDescription: "the market bed of a compact oak market cart",
					FullDescription: "This cargo projection represents the market bed built into a compact oak market cart. It follows the dimensions and access of the vehicle rather than existing as a separate container.",
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
				MaximumTowedWeight: 1200000,
				CharacterPullMultiplier: 1.25,
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
				MaximumTowedWeight: 900000,
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
				MaximumDamage: 120,
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
				MaximumDamage: 80,
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
