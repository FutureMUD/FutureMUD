#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AntiquityVehicleEraTag = "Era / Antiquity Era";
	private const string MedievalVehicleEraTag = "Era / Medieval Era";
	private const string RenaissanceVehicleEraTag = "Era / Renaissance Era";
	private const string EarlyModernVehicleEraTag = "Era / Early Modern Era";

	private static IReadOnlyList<VehiclePrototypeSeedSpec> VehiclePrototypeSeedSpecs()
	{
		return
		[
			AntiquityLightChariot(),
			AntiquityOxCart(),
			AntiquityRiverCanoe(),
			AntiquityOaredTradingBoat(),
			MedievalHandcart(),
			MedievalCoveredWagon(),
			MedievalClinkerRowboat(),
			MedievalCoastalCog(),
			RenaissancePassengerCoach(),
			RenaissanceSupplyWagon(),
			RenaissanceSailingPinnace(),
			RenaissanceLateenMerchantBoat(),
			EarlyModernStagecoach(),
			EarlyModernFreightDray(),
			EarlyModernShipsLongboat(),
			EarlyModernCoastalSloop()
		];
	}

	private static VehicleProjectionItemSeedSpec Exterior(
		string stableReference,
		string noun,
		string shortDescription,
		string longDescription,
		string fullDescription,
		SizeCategory size,
		ItemQuality quality,
		double weightInGrams,
		decimal cost,
		string material,
		string eraTag,
		params string[] classificationTags)
	{
		var tags = new List<string>
		{
			eraTag,
			"Functions / Vehicles",
			"Functions / Vehicles / Projection Items / Exterior Items"
		};
		tags.AddRange(classificationTags);
		return new VehicleProjectionItemSeedSpec(
			stableReference,
			noun,
			shortDescription,
			longDescription,
			fullDescription,
			size,
			quality,
			weightInGrams,
			cost,
			true,
			false,
			material,
			tags.ToArray(),
			["Destroyable_WoodenHeavy"],
			"Player-facing exterior shell for a stock vehicle prototype. The vehicle seeder attaches the internal Vehicle Exterior component.");
	}

	private static VehicleProjectionItemSeedSpec AccessItem(
		string stableReference,
		string noun,
		string shortDescription,
		string fullDescription,
		SizeCategory size,
		double weightInGrams,
		string material,
		string destroyableComponent = "Destroyable_WoodenHeavy")
	{
		return new VehicleProjectionItemSeedSpec(
			stableReference,
			noun,
			shortDescription,
			null,
			fullDescription,
			size,
			ItemQuality.Standard,
			weightInGrams,
			0m,
			false,
			true,
			material,
			["Functions / Vehicles / Projection Items / Access Points"],
			[destroyableComponent],
			"Internal access-point projection. Do not load independently or add an ordinary Door component; state belongs to the canonical vehicle access point.");
	}

	private static VehicleProjectionItemSeedSpec CargoItem(
		string stableReference,
		string noun,
		string shortDescription,
		string fullDescription,
		SizeCategory size,
		double weightInGrams,
		string material,
		string containerComponent,
		string destroyableComponent = "Destroyable_WoodenHeavy")
	{
		return new VehicleProjectionItemSeedSpec(
			stableReference,
			noun,
			shortDescription,
			null,
			fullDescription,
			size,
			ItemQuality.Standard,
			weightInGrams,
			0m,
			false,
			true,
			material,
			["Functions / Vehicles / Projection Items / Cargo Spaces", "Functions / Container"],
			[containerComponent, destroyableComponent],
			"Internal cargo-space projection. The vehicle seeder attaches the Vehicle Cargo Space component; the ordinary container component supplies storage behaviour.");
	}

	private static VehicleCompartmentSeedSpec Compartment(string key, string name, string description, int displayOrder)
	{
		return new VehicleCompartmentSeedSpec(key, name, description, displayOrder);
	}

	private static VehicleOccupantSlotSeedSpec DriverSlot(
		string key,
		string compartmentKey,
		string name,
		bool requiredForMovement,
		bool contributesToPropulsion = false,
		Difficulty stabilityDifficulty = Difficulty.Normal)
	{
		return new VehicleOccupantSlotSeedSpec(
			key,
			compartmentKey,
			name,
			VehicleOccupantSlotType.Driver,
			1,
			requiredForMovement,
			contributesToPropulsion,
			stabilityDifficulty);
	}

	private static VehicleOccupantSlotSeedSpec PassengerSlot(
		string key,
		string compartmentKey,
		string name,
		int capacity,
		Difficulty stabilityDifficulty = Difficulty.Normal)
	{
		return new VehicleOccupantSlotSeedSpec(
			key,
			compartmentKey,
			name,
			VehicleOccupantSlotType.Passenger,
			capacity,
			false,
			false,
			stabilityDifficulty);
	}

	private static VehicleOccupantSlotSeedSpec CrewSlot(
		string key,
		string compartmentKey,
		string name,
		int capacity,
		bool contributesToPropulsion,
		Difficulty stabilityDifficulty)
	{
		return new VehicleOccupantSlotSeedSpec(
			key,
			compartmentKey,
			name,
			VehicleOccupantSlotType.Crew,
			capacity,
			false,
			contributesToPropulsion,
			stabilityDifficulty);
	}

	private static VehicleControlStationSeedSpec PrimaryStation(string slotKey, string name)
	{
		return new VehicleControlStationSeedSpec(slotKey, name, true);
	}

	private static VehicleMovementProfileSeedSpec GroundMovement(bool requiresAccessClosed)
	{
		return new VehicleMovementProfileSeedSpec(
			"ground",
			"Road and Track Movement",
			VehicleMovementProfileType.CellExit,
			VehicleMovementEnvironment.Unrestricted,
			false,
			true,
			0.0,
			null,
			0.0,
			string.Empty,
			true,
			requiresAccessClosed,
			0.0,
			RouteVehiclePropulsionMode.Powered,
			0.0,
			0.0,
			false,
			[]);
	}

	private static VehicleMovementProfileSeedSpec WaterMovement(
		bool exposesOccupantsToWater,
		params VehiclePropulsionSeedSpec[] propulsionProfiles)
	{
		return new VehicleMovementProfileSeedSpec(
			"water",
			"Surface Water Movement",
			VehicleMovementProfileType.CellExit,
			VehicleMovementEnvironment.SurfaceWater,
			exposesOccupantsToWater,
			true,
			0.0,
			null,
			0.0,
			string.Empty,
			true,
			true,
			0.0,
			RouteVehiclePropulsionMode.Powered,
			0.0,
			0.0,
			false,
			propulsionProfiles);
	}

	private static VehiclePropulsionSeedSpec PaddlePropulsion(double baseMoveTimeMilliseconds, Difficulty difficulty)
	{
		return new VehiclePropulsionSeedSpec(
			VehiclePropulsionType.SelfPowered,
			true,
			baseMoveTimeMilliseconds,
			"Swimming",
			difficulty,
			"max(0.25, 1.0 + (0.15 * outcome))",
			"swimcost * max(0.5, 1.0 - (0.10 * outcome))");
	}

	private static VehiclePropulsionSeedSpec RowedPropulsion(
		double baseMoveTimeMilliseconds,
		Difficulty difficulty,
		bool isDefault)
	{
		return new VehiclePropulsionSeedSpec(
			VehiclePropulsionType.Rowed,
			isDefault,
			baseMoveTimeMilliseconds,
			"Swimming",
			difficulty,
			"max(0.25, 1.0 + (0.15 * outcome))",
			"swimcost * max(0.5, 1.0 - (0.10 * outcome))");
	}

	private static VehiclePropulsionSeedSpec SailPropulsion(double baseMoveTimeMilliseconds, bool isDefault)
	{
		return new VehiclePropulsionSeedSpec(
			VehiclePropulsionType.Sail,
			isDefault,
			baseMoveTimeMilliseconds,
			null,
			Difficulty.Normal,
			"1.0 + (0.15 * (wind - 1.0))",
			"0");
	}

	private static VehicleAccessPointSeedSpec Access(
		string key,
		string? compartmentKey,
		string name,
		string description,
		VehicleAccessPointType type,
		bool startsOpen,
		bool mustBeClosedForMovement,
		int displayOrder,
		VehicleProjectionItemSeedSpec item)
	{
		return new VehicleAccessPointSeedSpec(
			key,
			compartmentKey,
			name,
			description,
			type,
			startsOpen,
			mustBeClosedForMovement,
			displayOrder,
			item);
	}

	private static VehicleCargoSpaceSeedSpec Cargo(
		string key,
		string? compartmentKey,
		string? requiredAccessKey,
		string name,
		string description,
		int displayOrder,
		VehicleProjectionItemSeedSpec item)
	{
		return new VehicleCargoSpaceSeedSpec(
			key,
			compartmentKey,
			requiredAccessKey,
			name,
			description,
			displayOrder,
			item);
	}

	private static VehicleTowPointSeedSpec FrontTow(
		string name,
		string description,
		string towType,
		int displayOrder,
		double characterPullMultiplier = 1.0)
	{
		return new VehicleTowPointSeedSpec(
			"front",
			null,
			name,
			description,
			towType,
			false,
			true,
			0.0,
			characterPullMultiplier,
			0.80,
			0.95,
			0.20,
			0.02,
			displayOrder);
	}

	private static VehicleTowPointSeedSpec RearTow(
		string name,
		string description,
		string towType,
		double maximumTowedWeight,
		int displayOrder)
	{
		return new VehicleTowPointSeedSpec(
			"rear",
			null,
			name,
			description,
			towType,
			true,
			false,
			maximumTowedWeight,
			1.0,
			0.80,
			0.95,
			0.20,
			0.02,
			displayOrder);
	}

	private static VehicleDamageZoneSeedSpec Damage(
		string key,
		string name,
		string description,
		double maximumDamage,
		double hitWeight,
		double disabledThreshold,
		double destroyedThreshold,
		bool disablesMovement,
		params VehicleDamageEffectSeedSpec[] effects)
	{
		return new VehicleDamageZoneSeedSpec(
			key,
			name,
			description,
			maximumDamage,
			hitWeight,
			disabledThreshold,
			destroyedThreshold,
			disablesMovement,
			1,
			effects);
	}
}
