#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehiclePropulsionServiceTests
{
	[TestMethod]
	public void BuildReadiness_LegacySurfaceWaterProfile_PreservesLegacyMovement()
	{
		var profile = CreateMovementProfile([]);
		var vehicle = CreateVehicle(profile, null, [], []);
		var actor = new Mock<ICharacter>();

		var result = new VehiclePropulsionService().BuildReadiness(vehicle.Object, actor.Object, null);

		Assert.IsTrue(result.CanMove);
		Assert.IsTrue(result.UsesLegacyMovement);
	}

	[TestMethod]
	public void BuildReadiness_NoneMode_BlocksInitiatedMovement()
	{
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.None);
		var profile = CreateMovementProfile([propulsion.Object]);
		var vehicle = CreateVehicle(profile, propulsion.Object, [], []);
		var actor = new Mock<ICharacter>();

		var result = new VehiclePropulsionService().BuildReadiness(vehicle.Object, actor.Object, null);

		Assert.IsFalse(result.CanMove);
		StringAssert.Contains(result.Reason, "does not permit it to initiate movement");
	}

	[TestMethod]
	public void BuildReadiness_SailMode_RequiresWindAboveStill()
	{
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.Sail);
		var profile = CreateMovementProfile([propulsion.Object]);
		var weather = new Mock<IWeatherEvent>();
		weather.SetupGet(x => x.Wind).Returns(WindLevel.Still);
		var location = new Mock<ICell>();
		location.Setup(x => x.CurrentWeather(null)).Returns(weather.Object);
		var vehicle = CreateVehicle(profile, propulsion.Object, [], [], location.Object);
		var actor = new Mock<ICharacter>();

		var still = new VehiclePropulsionService().BuildReadiness(vehicle.Object, actor.Object, null);
		weather.SetupGet(x => x.Wind).Returns(WindLevel.GaleWind);
		var gale = new VehiclePropulsionService().BuildReadiness(vehicle.Object, actor.Object, null);

		Assert.IsFalse(still.CanMove);
		Assert.IsTrue(gale.CanMove);
		Assert.AreEqual(WindLevel.GaleWind, gale.Wind);
	}

	[TestMethod]
	public void TryCommitDeparture_SailMode_IncreasesWithWindAndFreezesDepartureSample()
	{
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.Sail);
		var profile = CreateMovementProfile([propulsion.Object]);
		var wind = WindLevel.Breeze;
		var weather = new Mock<IWeatherEvent>();
		weather.SetupGet(x => x.Wind).Returns(() => wind);
		var location = new Mock<ICell>();
		location.Setup(x => x.CurrentWeather(null)).Returns(weather.Object);
		var vehicle = CreateVehicle(profile, propulsion.Object, [], [], location.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(0.0);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var exit = CreateExit(1.0);
		var service = new VehiclePropulsionService();

		var breezeReadiness = service.BuildReadiness(vehicle.Object, actor.Object, exit.Object);
		Assert.IsTrue(service.TryCommitDeparture(breezeReadiness, out var breezePlan, out var breezeReason),
			breezeReason);
		wind = WindLevel.MaelstromWind;
		var maelstromReadiness = service.BuildReadiness(vehicle.Object, actor.Object, exit.Object);
		Assert.IsTrue(service.TryCommitDeparture(maelstromReadiness, out var maelstromPlan, out var maelstromReason),
			maelstromReason);

		Assert.AreEqual(WindLevel.Breeze, breezePlan!.Wind);
		Assert.AreEqual(1.15, breezePlan.EffectiveMultiplier, 0.0001);
		Assert.AreEqual(WindLevel.MaelstromWind, maelstromPlan!.Wind);
		Assert.AreEqual(1.9, maelstromPlan.EffectiveMultiplier, 0.0001);
		Assert.IsTrue(maelstromPlan.Duration < breezePlan.Duration);
	}

	[TestMethod]
	public void TryCommitDeparture_SelfPowered_RollsAndChargesOnceAndUsesExplicitDuration()
	{
		var trait = new Mock<ITraitDefinition>();
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.SelfPowered, trait.Object);
		var profile = CreateMovementProfile([propulsion.Object]);
		var actor = CreateContributor(Outcome.Pass, out var check);
		var vehicle = CreateVehicle(profile, propulsion.Object, [], []);
		vehicle.SetupGet(x => x.Controller).Returns(actor.Object);
		vehicle.Setup(x => x.IsOccupant(actor.Object)).Returns(true);
		actor.Setup(x => x.SamePhysicalInstance(actor.Object)).Returns(true);
		var exit = CreateExit(2.0);
		var service = new VehiclePropulsionService();
		var readiness = service.BuildReadiness(vehicle.Object, actor.Object, exit.Object);

		var committed = service.TryCommitDeparture(readiness, out var plan, out var reason);
		var remainsValid = service.ValidateCommittedPlan(plan!, out var validationReason);

		Assert.IsTrue(committed, reason);
		Assert.IsTrue(remainsValid, validationReason);
		Assert.AreEqual(1.3, plan!.EffectiveMultiplier, 0.0001);
		Assert.AreEqual(20000.0 / 1.3, plan.Duration.TotalMilliseconds, 0.01);
		check.Verify(x => x.Check(actor.Object, Difficulty.Normal, trait.Object, null, 0.0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()), Times.Once);
		actor.Verify(x => x.SpendStamina(8.0), Times.Once);
	}

	[TestMethod]
	public void TryCommitDeparture_AutoGeneratedConstantPropulsionCheck_UsesGenericSkillFallback()
	{
		var trait = new Mock<ITraitDefinition>();
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.SelfPowered, trait.Object);
		var profile = CreateMovementProfile([propulsion.Object]);
		var expression = new Mock<ITraitExpression>();
		expression.SetupGet(x => x.NonTraitParameters).Returns([]);
		var generatedCheck = new Mock<ICheck>();
		generatedCheck.SetupGet(x => x.Type).Returns(CheckType.PaddleVehicleCheck);
		generatedCheck.SetupGet(x => x.TargetNumberExpression).Returns(expression.Object);
		var genericCheck = new Mock<ICheck>();
		genericCheck.SetupGet(x => x.Type).Returns(CheckType.GenericSkillCheck);
		genericCheck.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Pass));
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(0.0);
		gameworld.Setup(x => x.GetCheck(CheckType.PaddleVehicleCheck)).Returns(generatedCheck.Object);
		gameworld.Setup(x => x.GetCheck(CheckType.GenericSkillCheck)).Returns(genericCheck.Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
		actor.Setup(x => x.SwimStaminaCost()).Returns(10.0);
		var vehicle = CreateVehicle(profile, propulsion.Object, [], []);
		var exit = CreateExit(1.0);
		var service = new VehiclePropulsionService();
		var readiness = service.BuildReadiness(vehicle.Object, actor.Object, exit.Object);

		var committed = service.TryCommitDeparture(readiness, out _, out var reason);

		Assert.IsTrue(committed, reason);
		generatedCheck.Verify(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string Parameter, object value)[]>()), Times.Never);
		genericCheck.Verify(x => x.Check(actor.Object, Difficulty.Normal, trait.Object, null, 0.0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()), Times.Once);
	}

	[TestMethod]
	public void TryCommitDeparture_MultipleRowers_UsesConditionAndSquareRootAggregation()
	{
		var trait = new Mock<ITraitDefinition>();
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.Rowed, trait.Object);
		var profile = CreateMovementProfile([propulsion.Object]);
		var first = CreateContributor(Outcome.Pass, out _);
		var second = CreateContributor(Outcome.Fail, out _);
		var firstOccupancy = CreateRowerOccupancy(first.Object, 1.0, 1.0);
		var secondOccupancy = CreateRowerOccupancy(second.Object, 2.0, 0.5);
		var vehicle = CreateVehicle(profile, propulsion.Object,
			[firstOccupancy.Occupancy.Object, secondOccupancy.Occupancy.Object], []);
		var exit = CreateExit(1.0);
		var service = new VehiclePropulsionService();
		var readiness = service.BuildReadiness(vehicle.Object, first.Object, exit.Object);

		var committed = service.TryCommitDeparture(readiness, out var plan, out var reason);

		Assert.IsTrue(committed, reason);
		Assert.AreEqual(2, plan!.Contributors.Count);
		Assert.AreEqual(Math.Sqrt(1.3 + 0.7), plan.EffectiveMultiplier, 0.0001);
		first.Verify(x => x.SpendStamina(8.0), Times.Once);
		second.Verify(x => x.SpendStamina(12.0), Times.Once);
	}

	[TestMethod]
	public void TryCommitDeparture_FuelledAndElectricMotors_AggregatesAndConsumesEachOnce()
	{
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.OutboardMotor);
		var profile = CreateMovementProfile([propulsion.Object]);
		var actor = new Mock<ICharacter>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(0.0);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var fuel = new Mock<ILiquid>();
		fuel.SetupGet(x => x.Id).Returns(123);
		fuel.SetupGet(x => x.Density).Returns(1.0);
		var mixture = new LiquidMixture(fuel.Object, 10.0, gameworld.Object);
		var fuelMotor = CreateFuelMotor(actor.Object, fuel.Object.Id, mixture, 1.5, 2.0);
		var electricMotor = CreateElectricMotor(actor.Object, 2.5, 500.0);
		var vehicle = CreateVehicle(profile, propulsion.Object, [],
			[fuelMotor.Installation.Object, electricMotor.Installation.Object]);
		var exit = CreateExit(1.0);
		var service = new VehiclePropulsionService();
		var readiness = service.BuildReadiness(vehicle.Object, actor.Object, exit.Object);

		var committed = service.TryCommitDeparture(readiness, out var plan, out var reason);

		Assert.IsTrue(committed, reason);
		Assert.AreEqual(4.0, plan!.EffectiveMultiplier, 0.0001);
		Assert.AreEqual(8.0, fuelMotor.Container.Object.LiquidMixture.TotalVolume, 0.0001);
		electricMotor.Producer.Verify(x => x.DrawdownSpike(500.0), Times.Once);
	}

	[TestMethod]
	public void TryCommitDeparture_SecondMotorFailsAfterFirst_UsesSuccessfulMotorAndStillDeparts()
	{
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.OutboardMotor);
		var profile = CreateMovementProfile([propulsion.Object]);
		var actor = new Mock<ICharacter>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(0.0);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var firstMotor = CreateElectricMotor(actor.Object, 1.5, 400.0);
		var secondMotor = CreateElectricMotor(actor.Object, 2.5, 500.0);
		secondMotor.Producer.Setup(x => x.DrawdownSpike(500.0)).Returns(false);
		var vehicle = CreateVehicle(profile, propulsion.Object, [],
			[firstMotor.Installation.Object, secondMotor.Installation.Object]);
		var exit = CreateExit(1.0);
		var service = new VehiclePropulsionService();
		var readiness = service.BuildReadiness(vehicle.Object, actor.Object, exit.Object);

		var committed = service.TryCommitDeparture(readiness, out var plan, out var reason);

		Assert.IsTrue(committed, reason);
		Assert.AreEqual(1, plan!.Motors.Count);
		Assert.AreSame(firstMotor.Installation.Object, plan.Motors.Single().Installation);
		Assert.AreEqual(1.5, plan.EffectiveMultiplier, 0.0001);
		firstMotor.Producer.Verify(x => x.DrawdownSpike(400.0), Times.Once);
		secondMotor.Producer.Verify(x => x.DrawdownSpike(500.0), Times.Once);
	}

	[TestMethod]
	public void BuildEngineReadiness_MixedImplementations_AggregatesThroughInterface()
	{
		var first = CreateEngine("automotive", 60000.0, true);
		var second = CreateEngine("automotive", 50000.0, true);
		var profile = CreateMovementProfile([]);
		var vehicle = CreateVehicle(profile, null, [], [first.Installation.Object, second.Installation.Object]);

		var result = new VehiclePropulsionService().BuildEngineReadiness(vehicle.Object, 100000.0);

		Assert.IsTrue(result.CanMove, result.Reason);
		Assert.AreEqual(110000.0, result.AvailablePowerInWatts);
		Assert.AreEqual(2, result.Engines.Count(x => x.Available));
	}

	[TestMethod]
	public void BuildEngineReadiness_StoppedOrMismatchedEngine_ReportsPreciseBlockers()
	{
		var stopped = CreateEngine("automotive", 60000.0, false);
		var mismatched = CreateEngine("marine", 90000.0, true, "automotive");
		var profile = CreateMovementProfile([]);
		var vehicle = CreateVehicle(profile, null, [], [stopped.Installation.Object, mismatched.Installation.Object]);

		var result = new VehiclePropulsionService().BuildEngineReadiness(vehicle.Object, 50000.0);

		Assert.IsFalse(result.CanMove);
		StringAssert.Contains(result.Reason, "switched off");
		StringAssert.Contains(result.Reason, "does not match");
	}

	[TestMethod]
	public void TryCommitDeparture_EngineMode_UsesAggregateMechanicalPower()
	{
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.Engine);
		var profile = CreateMovementProfile([propulsion.Object]);
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.Unrestricted);
		profile.SetupGet(x => x.MinimumEnginePowerInWatts).Returns(100000.0);
		var first = CreateEngine("automotive", 60000.0, true);
		var second = CreateEngine("automotive", 60000.0, true);
		var vehicle = CreateVehicle(profile, propulsion.Object, [],
			[first.Installation.Object, second.Installation.Object]);
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(0.0);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var service = new VehiclePropulsionService();
		var readiness = service.BuildReadiness(vehicle.Object, actor.Object, CreateExit(1.0).Object);

		var committed = service.TryCommitDeparture(readiness, out var plan, out var reason);

		Assert.IsTrue(committed, reason);
		Assert.AreEqual(1.2, plan!.EffectiveMultiplier, 0.0001);
		Assert.AreEqual(10000.0 / 1.2, plan.Duration.TotalMilliseconds, 0.01);
		Assert.AreEqual(2, plan.Engines!.Count(x => x.Available));
	}

	[TestMethod]
	public void TryCommitDeparture_RiderPowered_ChargesTerrainEncumbranceAndExactVehicleModifier()
	{
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.Id).Returns(81);
		terrain.SetupGet(x => x.StaminaCost).Returns(4.0);
		var modifier = new Mock<IVehicleRiderStaminaModifierPrototype>();
		modifier.SetupGet(x => x.Id).Returns(1);
		modifier.SetupGet(x => x.TerrainId).Returns(terrain.Object.Id);
		modifier.SetupGet(x => x.Multiplier).Returns(0.5);
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.RiderPowered);
		propulsion.SetupGet(x => x.RiderStaminaMultiplier).Returns(0.8);
		propulsion.SetupGet(x => x.RiderStaminaModifiers).Returns([modifier.Object]);
		var profile = CreateMovementProfile([propulsion.Object]);
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.Unrestricted);
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(0.0);
		gameworld.Setup(x => x.GetStaticDouble("DefaultTerrainStaminaCost")).Returns(1.0);
		gameworld.Setup(x => x.GetStaticDouble("StaminaMultiplierPerEncumbrancePercentage")).Returns(1.0);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.SetupGet(x => x.EncumbrancePercentage).Returns(0.5);
		actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
		var origin = new Mock<ICell>();
		origin.Setup(x => x.Terrain(actor.Object)).Returns(terrain.Object);
		var vehicle = CreateVehicle(profile, propulsion.Object, [], [], origin.Object);
		var exit = CreateExit(1.0, origin.Object);
		var service = new VehiclePropulsionService();

		var readiness = service.BuildReadiness(vehicle.Object, actor.Object, exit.Object);
		var committed = service.TryCommitDeparture(readiness, out var plan, out var reason);

		Assert.IsTrue(committed, reason);
		Assert.AreEqual(0.5, plan!.RiderStaminaMultiplier, 0.0001);
		Assert.AreEqual(3.0, plan.RiderStaminaCost, 0.0001);
		Assert.AreEqual(1.0, plan.EffectiveMultiplier, 0.0001);
		actor.Verify(x => x.SpendStamina(3.0), Times.Once);
	}

	[TestMethod]
	public void ResolveRiderStaminaMultiplier_UsesDeepestTagBeforeDefault()
	{
		var broadTag = new Mock<ITag>();
		broadTag.SetupGet(x => x.Id).Returns(10);
		var narrowTag = new Mock<ITag>();
		narrowTag.SetupGet(x => x.Id).Returns(11);
		narrowTag.SetupGet(x => x.Parent).Returns(broadTag.Object);
		var broadModifier = new Mock<IVehicleRiderStaminaModifierPrototype>();
		broadModifier.SetupGet(x => x.Id).Returns(1);
		broadModifier.SetupGet(x => x.TerrainTagId).Returns(broadTag.Object.Id);
		broadModifier.SetupGet(x => x.TerrainTag).Returns(broadTag.Object);
		broadModifier.SetupGet(x => x.Multiplier).Returns(0.75);
		var narrowModifier = new Mock<IVehicleRiderStaminaModifierPrototype>();
		narrowModifier.SetupGet(x => x.Id).Returns(2);
		narrowModifier.SetupGet(x => x.TerrainTagId).Returns(narrowTag.Object.Id);
		narrowModifier.SetupGet(x => x.TerrainTag).Returns(narrowTag.Object);
		narrowModifier.SetupGet(x => x.Multiplier).Returns(1.25);
		var profile = CreatePropulsionProfile(VehiclePropulsionType.RiderPowered);
		profile.SetupGet(x => x.RiderStaminaMultiplier).Returns(0.5);
		profile.SetupGet(x => x.RiderStaminaModifiers)
			.Returns([broadModifier.Object, narrowModifier.Object]);
		var terrain = new Mock<ITerrain>();
		terrain.Setup(x => x.IsA(broadTag.Object)).Returns(true);
		terrain.Setup(x => x.IsA(narrowTag.Object)).Returns(true);

		var result = VehiclePropulsionService.ResolveRiderStaminaMultiplier(profile.Object, terrain.Object);

		Assert.AreEqual(1.25, result, 0.0001);
	}

	[TestMethod]
	public void BuildReadiness_RiderPowered_RejectsExhaustedControllerWithoutCharging()
	{
		var propulsion = CreatePropulsionProfile(VehiclePropulsionType.RiderPowered);
		var profile = CreateMovementProfile([propulsion.Object]);
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.Unrestricted);
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.StaminaCost).Returns(5.0);
		var location = new Mock<ICell>();
		var actor = new Mock<ICharacter>();
		location.Setup(x => x.Terrain(actor.Object)).Returns(terrain.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("StaminaMultiplierPerEncumbrancePercentage")).Returns(1.0);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.Setup(x => x.CanSpendStamina(5.0)).Returns(false);
		var vehicle = CreateVehicle(profile, propulsion.Object, [], [], location.Object);

		var result = new VehiclePropulsionService().BuildReadiness(vehicle.Object, actor.Object, null);

		Assert.IsFalse(result.CanMove);
		StringAssert.Contains(result.Reason, "too exhausted");
		actor.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never);
	}

	private static Mock<IVehicleMovementProfilePrototype> CreateMovementProfile(
		IEnumerable<IVehiclePropulsionProfilePrototype> propulsion)
	{
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.Id).Returns(10);
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.SurfaceWater);
		profile.SetupGet(x => x.PropulsionProfiles).Returns(propulsion);
		return profile;
	}

	private static Mock<IVehiclePropulsionProfilePrototype> CreatePropulsionProfile(VehiclePropulsionType type,
		ITraitDefinition? trait = null)
	{
		var profile = new Mock<IVehiclePropulsionProfilePrototype>();
		profile.SetupGet(x => x.Id).Returns(20);
		profile.SetupGet(x => x.PropulsionType).Returns(type);
		profile.SetupGet(x => x.IsDefault).Returns(true);
		profile.SetupGet(x => x.BaseMoveTimeMilliseconds).Returns(10000.0);
		profile.SetupGet(x => x.PropulsionTrait).Returns(trait!);
		profile.SetupGet(x => x.CheckDifficulty).Returns(Difficulty.Normal);
		profile.SetupGet(x => x.SpeedMultiplierExpression).Returns(type switch
		{
			VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed =>
				"max(0.25, 1.0 + (0.15 * outcome))",
			VehiclePropulsionType.Sail => "1.0 + (0.15 * (wind - 1.0))",
			VehiclePropulsionType.OutboardMotor => "output",
			VehiclePropulsionType.Engine => "power / requiredpower",
			VehiclePropulsionType.RiderPowered => "1",
			_ => "0"
		});
		profile.SetupGet(x => x.StaminaCostExpression)
		       .Returns(type switch
		       {
			       VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed =>
				       "swimcost * max(0.5, 1.0 - (0.10 * outcome))",
			       VehiclePropulsionType.RiderPowered =>
				       "terraincost * encumbrance * vehiclemultiplier",
			       _ => "0"
		       });
		profile.SetupGet(x => x.RiderStaminaMultiplier).Returns(1.0);
		profile.SetupGet(x => x.RiderStaminaModifiers)
			.Returns(Array.Empty<IVehicleRiderStaminaModifierPrototype>());
		return profile;
	}

	private static Mock<IVehicle> CreateVehicle(Mock<IVehicleMovementProfilePrototype> movement,
		IVehiclePropulsionProfilePrototype? active, IEnumerable<IVehicleOccupancy> occupancies,
		IEnumerable<IVehicleInstallation> installations, ICell? location = null)
	{
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.MovementProfile).Returns(movement.Object);
		vehicle.SetupGet(x => x.ActivePropulsionProfile).Returns(active!);
		vehicle.SetupGet(x => x.Occupancies).Returns(occupancies);
		vehicle.SetupGet(x => x.Installations).Returns(installations);
		vehicle.SetupGet(x => x.Location).Returns(location!);
		return vehicle;
	}

	private static Mock<ICharacter> CreateContributor(Outcome outcome, out Mock<ICheck> check)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(0.0);
		check = new Mock<ICheck>();
		check.SetupGet(x => x.Type).Returns(CheckType.PaddleVehicleCheck);
		check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string Parameter, object value)[]>())).Returns(CheckOutcome.SimpleOutcome(CheckType.PaddleVehicleCheck, outcome));
		gameworld.Setup(x => x.GetCheck(It.IsAny<CheckType>())).Returns(check.Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
		actor.Setup(x => x.SwimStaminaCost()).Returns(10.0);
		return actor;
	}

	private static (Mock<IVehicleOccupancy> Occupancy, Mock<IGameItem> Item) CreateRowerOccupancy(
		ICharacter character, double efficiency, double condition)
	{
		var oar = new Mock<IVehicleOar>();
		oar.SetupGet(x => x.EfficiencyMultiplier).Returns(efficiency);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Condition).Returns(condition);
		item.Setup(x => x.GetItemType<IVehicleOar>()).Returns(oar.Object);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.HeldOrWieldedItems).Returns([item.Object]);
		Mock.Get(character).SetupGet(x => x.Body).Returns(body.Object);
		var slot = new Mock<IVehicleOccupantSlotPrototype>();
		slot.SetupGet(x => x.ContributesToPropulsion).Returns(true);
		var occupancy = new Mock<IVehicleOccupancy>();
		occupancy.SetupGet(x => x.Occupant).Returns(character);
		occupancy.SetupGet(x => x.Slot).Returns(slot.Object);
		return (occupancy, item);
	}

	private static Mock<ICellExit> CreateExit(double timeMultiplier, ICell? origin = null)
	{
		var exitModel = new Mock<IExit>();
		exitModel.SetupGet(x => x.TimeMultiplier).Returns(timeMultiplier);
		var exit = new Mock<ICellExit>();
		exit.SetupGet(x => x.Exit).Returns(exitModel.Object);
		exit.SetupGet(x => x.Origin).Returns(origin!);
		return exit;
	}

	private static (Mock<IVehicleInstallation> Installation, Mock<ILiquidContainer> Container) CreateFuelMotor(
		ICharacter actor, long liquidId, LiquidMixture mixture, double output, double volume)
	{
		var motor = new Mock<IOutboardMotor>();
		motor.SetupGet(x => x.EnergySource).Returns(OutboardMotorEnergySource.Fuelled);
		motor.SetupGet(x => x.OutputMultiplier).Returns(output);
		motor.SetupGet(x => x.FuelLiquidId).Returns(liquidId);
		motor.SetupGet(x => x.FuelVolumePerMove).Returns(volume);
		var container = new Mock<ILiquidContainer>();
		container.SetupProperty(x => x.LiquidMixture, mixture);
		var item = CreateFunctionalMotorItem(motor.Object, out var installation);
		item.Setup(x => x.GetItemTypes<ILiquidContainer>()).Returns([container.Object]);
		return (installation, container);
	}

	private static (Mock<IVehicleInstallation> Installation, Mock<IProducePower> Producer) CreateElectricMotor(
		ICharacter actor, double output, double watts)
	{
		var motor = new Mock<IOutboardMotor>();
		motor.SetupGet(x => x.EnergySource).Returns(OutboardMotorEnergySource.Electric);
		motor.SetupGet(x => x.OutputMultiplier).Returns(output);
		motor.SetupGet(x => x.RequiredPowerSpikeInWatts).Returns(watts);
		var producer = new Mock<IProducePower>();
		producer.SetupGet(x => x.ProducingPower).Returns(true);
		producer.Setup(x => x.CanDrawdownSpike(watts)).Returns(true);
		producer.Setup(x => x.DrawdownSpike(watts)).Returns(true);
		var item = CreateFunctionalMotorItem(motor.Object, out var installation);
		item.Setup(x => x.GetItemType<IProducePower>()).Returns(producer.Object);
		return (installation, producer);
	}

	private static Mock<IGameItem> CreateFunctionalMotorItem(IOutboardMotor motor,
		out Mock<IVehicleInstallation> installation)
	{
		var installable = new Mock<IVehicleInstallable>();
		var ignored = string.Empty;
		installable.Setup(x => x.IsFunctionalForMovement(out ignored)).Returns(true);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(100);
		item.Setup(x => x.GetItemType<IOutboardMotor>()).Returns(motor);
		item.Setup(x => x.GetItemType<IVehicleInstallable>()).Returns(installable.Object);
		installation = new Mock<IVehicleInstallation>();
		installation.SetupGet(x => x.InstalledItem).Returns(item.Object);
		return item;
	}

	private static (Mock<IVehicleInstallation> Installation, Mock<IVehicleEngine> Engine) CreateEngine(
		string formFactor, double power, bool running, string mountType = "automotive")
	{
		var engine = new Mock<IVehicleEngine>();
		engine.SetupGet(x => x.FormFactor).Returns(formFactor);
		engine.SetupGet(x => x.MaximumPowerInWatts).Returns(power);
		engine.SetupGet(x => x.IsRunning).Returns(running);
		engine.SetupGet(x => x.WhyNotRunning).Returns(running ? string.Empty : "the engine is switched off");
		var installable = new Mock<IVehicleInstallable>();
		var ignored = string.Empty;
		installable.Setup(x => x.IsFunctionalForMovement(out ignored)).Returns(true);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns((long)power + (running ? 1L : 2L));
		item.Setup(x => x.GetItemType<IVehicleEngine>()).Returns(engine.Object);
		item.Setup(x => x.GetItemType<IVehicleInstallable>()).Returns(installable.Object);
		var point = new Mock<IVehicleInstallationPointPrototype>();
		point.SetupGet(x => x.Name).Returns($"{mountType} engine bay");
		point.SetupGet(x => x.MountType).Returns(mountType);
		var installation = new Mock<IVehicleInstallation>();
		installation.SetupGet(x => x.InstalledItem).Returns(item.Object);
		installation.SetupGet(x => x.Prototype).Returns(point.Object);
		return (installation, engine);
	}
}
