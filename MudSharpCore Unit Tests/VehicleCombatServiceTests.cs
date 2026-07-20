#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System.Collections.Generic;
using System.Reflection;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleCombatServiceTests
{
	[TestMethod]
	public void Direction_SurfaceSwimmerAttackingSurfaceCraft_IsBelow()
	{
		var harness = CreateHarness();

		var direction = VehicleCombatService.Instance.ClassifyRangedCoverDirection(harness.Attacker.Object,
			harness.Vehicle.Object);

		Assert.AreEqual(VehicleRangedCoverDirection.Below, direction);
	}

	[TestMethod]
	public void Direction_AerialAttacker_IsAbove()
	{
		var harness = CreateHarness(RoomLayer.InAir);

		var direction = VehicleCombatService.Instance.ClassifyRangedCoverDirection(harness.Attacker.Object,
			harness.Vehicle.Object);

		Assert.AreEqual(VehicleRangedCoverDirection.Above, direction);
	}

	[TestMethod]
	public void Direction_SurfaceSwimmerInRemoteWaterCell_IsBelow()
	{
		var harness = CreateHarness();
		var remoteWater = new Mock<ICell>();
		remoteWater.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(true);
		harness.Attacker.SetupGet(x => x.Location).Returns(remoteWater.Object);

		var direction = VehicleCombatService.Instance.ClassifyRangedCoverDirection(harness.Attacker.Object,
			harness.Vehicle.Object);

		Assert.AreEqual(VehicleRangedCoverDirection.Below, direction);
	}

	[TestMethod]
	public void ResolveVehicleRangedCover_UsesDirectionalSlotCoverAndExteriorProvider()
	{
		var harness = CreateHarness();

		var result = VehicleCombatService.Instance.ResolveVehicleRangedCover(harness.Attacker.Object,
			harness.Target.Object);

		Assert.IsNotNull(result);
		Assert.AreSame(harness.BelowCover.Object, result.Cover);
		Assert.AreSame(harness.Exterior.Object, result.Provider);
		Assert.AreEqual(VehicleRangedCoverDirection.Below, result.Direction);
		Assert.IsTrue(result.IsVehicleCover);
	}

	[TestMethod]
	public void ResolveVehicleRangedCover_SameVehicleOccupantBypassesVehicleCover()
	{
		var harness = CreateHarness();
		harness.Vehicle.Setup(x => x.IsOccupant(harness.Attacker.Object)).Returns(true);

		var result = VehicleCombatService.Instance.ResolveVehicleRangedCover(harness.Attacker.Object,
			harness.Target.Object);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void CrossBoundary_SurfaceSwimmerContactBlockedButRangedAndAquaticAllowed()
	{
		var harness = CreateHarness();

		Assert.IsFalse(VehicleCombatService.Instance.CanCrossVehicleBoundary(harness.Attacker.Object,
			harness.Target.Object, false, false, out var reason));
		StringAssert.Contains(reason, "must board");
		Assert.IsTrue(VehicleCombatService.Instance.CanCrossVehicleBoundary(harness.Attacker.Object,
			harness.Target.Object, true, false, out _));
		Assert.IsTrue(VehicleCombatService.Instance.CanCrossVehicleBoundary(harness.Attacker.Object,
			harness.Target.Object, false, true, out _));
	}

	[TestMethod]
	public void PersistenceModels_DefaultToNormalStabilityAndTerrestrialPreference()
	{
		Assert.AreEqual((int)Difficulty.Normal, new DB.VehicleOccupantSlotProto().BoatStabilityDifficulty);
		Assert.IsTrue(new DB.CharacterCombatSetting().PreferTerrestrialCombat);
	}

	[TestMethod]
	public void BuilderHelp_ListsDirectionalCoverAndStabilityCommands()
	{
		var help = (string)typeof(VehiclePrototype)
			.GetField("BuildingHelp", BindingFlags.Static | BindingFlags.NonPublic)!
			.GetRawConstantValue()!;

		StringAssert.Contains(help, "slot cover <id> <same|above|below|all>");
		StringAssert.Contains(help, "slot stability <id> <difficulty>");
	}

	[TestMethod]
	public void AquaticVehicleAttack_IsAnAuthoredWeaponAttackType()
	{
		Assert.IsTrue(BuiltInCombatMoveType.AquaticVehicleAttack.IsWeaponAttackType());
		Assert.IsTrue(CheckType.BoatStabilityCheck.IsPhysicalActivityCheck());
	}

	[TestMethod]
	public void ManualBoardingAction_CreatesDelayedCombatMove()
	{
		var harness = CreateHarness();
		var slot = new Mock<IVehicleOccupantSlotPrototype>();
		var access = new Mock<IVehicleAccessPoint>();
		var action = SelectedCombatAction.GetEffectBoardVehicle(harness.Attacker.Object, harness.Vehicle.Object,
			slot.Object, access.Object);

		var move = action.GetMove(harness.Attacker.Object);

		Assert.IsInstanceOfType(move, typeof(BoardVehicleCombatMove));
		Assert.AreEqual(2.0, move.BaseDelay);
	}

	[TestMethod]
	public void ManualAquaticAttack_TargetingOccupant_ResolvesAgainstVehicleExterior()
	{
		var harness = CreateHarness();
		var weaponAttack = new Mock<IWeaponAttack>();
		weaponAttack.SetupGet(x => x.MoveType).Returns(BuiltInCombatMoveType.AquaticVehicleAttack);
		var naturalAttack = new Mock<INaturalAttack>();
		naturalAttack.SetupGet(x => x.Attack).Returns(weaponAttack.Object);

		var move = CombatMoveFactory.CreateNaturalWeaponAttack(harness.Attacker.Object, naturalAttack.Object,
			harness.Target.Object);

		var aquaticMove = move as AquaticVehicleAttackMove;
		Assert.IsNotNull(aquaticMove);
		Assert.AreSame(harness.Exterior.Object, aquaticMove.PrimaryTarget);
	}

	[TestMethod]
	public void ResolveDisplacement_ConfiguredCheck_StagesDifficultyFromSuccessDegrees()
	{
		var harness = CreateHarness();
		harness.Slot.SetupGet(x => x.BoatStabilityDifficulty).Returns(Difficulty.Easy);
		var expression = new Mock<ITraitExpression>();
		expression.SetupGet(x => x.Parameters).Returns(new Dictionary<string, TraitExpressionParameter>());
		expression.SetupGet(x => x.NonTraitParameters).Returns(["variable"]);
		var check = new Mock<ICheck>();
		check.SetupGet(x => x.Type).Returns(CheckType.BoatStabilityCheck);
		check.SetupGet(x => x.TargetNumberExpression).Returns(expression.Object);
		check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.BoatStabilityCheck, Outcome.Pass));
		harness.Gameworld.Setup(x => x.GetCheck(CheckType.BoatStabilityCheck)).Returns(check.Object);

		var result = VehicleCombatService.Instance.ResolveDisplacement(harness.Target.Object,
			VehicleCombatDisplacementType.Push, 3);

		Assert.IsTrue(result.WasApplicable);
		Assert.IsFalse(result.FellOverboard);
		Assert.AreEqual(Difficulty.Hard, result.Difficulty);
		check.Verify(x => x.Check(harness.Target.Object, Difficulty.Hard, harness.Exterior.Object, null, 0.0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()), Times.Once);
	}

	[TestMethod]
	public void ResolveDisplacement_AutoGeneratedConstantCheck_UsesGenericSkillFallback()
	{
		var harness = CreateHarness();
		var expression = new Mock<ITraitExpression>();
		expression.SetupGet(x => x.Parameters).Returns(new Dictionary<string, TraitExpressionParameter>());
		expression.SetupGet(x => x.NonTraitParameters).Returns([]);
		var generatedCheck = new Mock<ICheck>();
		generatedCheck.SetupGet(x => x.Type).Returns(CheckType.BoatStabilityCheck);
		generatedCheck.SetupGet(x => x.TargetNumberExpression).Returns(expression.Object);
		var genericCheck = new Mock<ICheck>();
		genericCheck.SetupGet(x => x.Type).Returns(CheckType.GenericSkillCheck);
		genericCheck.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Pass));
		var fallbackTrait = new Mock<ITraitDefinition>();
		fallbackTrait.SetupGet(x => x.Id).Returns(1);
		fallbackTrait.SetupGet(x => x.Name).Returns("Dodge");
		harness.Gameworld.SetupGet(x => x.Traits)
		       .Returns(new All<ITraitDefinition> { fallbackTrait.Object });
		harness.Gameworld.Setup(x => x.GetCheck(CheckType.BoatStabilityCheck)).Returns(generatedCheck.Object);
		harness.Gameworld.Setup(x => x.GetCheck(CheckType.GenericSkillCheck)).Returns(genericCheck.Object);

		var result = VehicleCombatService.Instance.ResolveDisplacement(harness.Target.Object,
			VehicleCombatDisplacementType.Unbalance);

		Assert.IsTrue(result.WasApplicable);
		Assert.IsFalse(result.FellOverboard);
		genericCheck.Verify(x => x.Check(harness.Target.Object, Difficulty.Normal, fallbackTrait.Object,
			harness.Exterior.Object, 0.0, TraitUseType.Practical,
			It.IsAny<(string Parameter, object value)[]>()), Times.Once);
	}

	[TestMethod]
	public void ForcedLayerMovement_PassedStabilityCheck_RetainsOccupancyWithoutTeleport()
	{
		var harness = CreateHarness();
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.TerrainLayers).Returns([RoomLayer.GroundLevel, RoomLayer.InAir]);
		harness.Location.Setup(x => x.Terrain(harness.Target.Object)).Returns(terrain.Object);
		var expression = new Mock<ITraitExpression>();
		expression.SetupGet(x => x.Parameters).Returns(new Dictionary<string, TraitExpressionParameter>());
		expression.SetupGet(x => x.NonTraitParameters).Returns(["variable"]);
		var check = new Mock<ICheck>();
		check.SetupGet(x => x.Type).Returns(CheckType.BoatStabilityCheck);
		check.SetupGet(x => x.TargetNumberExpression).Returns(expression.Object);
		check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.BoatStabilityCheck, Outcome.Pass));
		harness.Gameworld.Setup(x => x.GetCheck(CheckType.BoatStabilityCheck)).Returns(check.Object);
		harness.Attacker.Setup(x => x.EffectsOfType<ClinchEffect>(null)).Returns([]);
		harness.Target.Setup(x => x.EffectsOfType<ClinchEffect>(null)).Returns([]);
		harness.Attacker.Setup(x => x.EffectsOfType<IGrappling>(null)).Returns([]);
		harness.Target.Setup(x => x.EffectsOfType<IGrappling>(null)).Returns([]);
		harness.Attacker.Setup(x => x.CombinedEffectsOfType<IBeingGrappled>()).Returns([]);
		harness.Target.Setup(x => x.CombinedEffectsOfType<IBeingGrappled>()).Returns([]);

		var moved = CombatForcedMovementUtilities.TryForceLayerMovement(harness.Attacker.Object,
			harness.Target.Object, RoomLayer.InAir, ForcedMovementVerbs.Shove, 2, out var reason);

		Assert.IsFalse(moved);
		StringAssert.Contains(reason, "keeps their footing aboard");
		harness.Target.Verify(x => x.Teleport(It.IsAny<ICell>(), It.IsAny<RoomLayer>(), It.IsAny<bool>(),
			It.IsAny<bool>()), Times.Never);
		harness.Vehicle.Verify(x => x.ForceDisembark(harness.Target.Object, It.IsAny<bool>()), Times.Never);
	}

	private static Harness CreateHarness(RoomLayer attackerLayer = RoomLayer.GroundLevel)
	{
		var location = new Mock<ICell>();
		location.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(true);
		var movement = new Mock<IVehicleMovementProfilePrototype>();
		movement.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.SurfaceWater);
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Location).Returns(location.Object);
		exterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		exterior.SetupGet(x => x.Deleted).Returns(false);
		exterior.SetupGet(x => x.Destroyed).Returns(false);

		var belowCover = new Mock<IRangedCover>();
		belowCover.SetupGet(x => x.MinimumRangedDifficulty).Returns(Difficulty.Hard);
		belowCover.SetupGet(x => x.CoverType).Returns(CoverType.Hard);
		var slot = new Mock<IVehicleOccupantSlotPrototype>();
		slot.SetupGet(x => x.BelowRangedCover).Returns(belowCover.Object);
		slot.SetupGet(x => x.BoatStabilityDifficulty).Returns(Difficulty.Normal);
		var target = new Mock<ICharacter>();
		target.SetupGet(x => x.Location).Returns(location.Object);
		target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		target.Setup(x => x.SamePhysicalInstance(target.Object)).Returns(true);
		var attacker = new Mock<ICharacter>();
		attacker.SetupGet(x => x.Location).Returns(location.Object);
		attacker.SetupGet(x => x.RoomLayer).Returns(attackerLayer);
		var occupancy = new Mock<IVehicleOccupancy>();
		occupancy.SetupGet(x => x.Occupant).Returns(target.Object);
		occupancy.SetupGet(x => x.Slot).Returns(slot.Object);

		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(1);
		vehicle.SetupGet(x => x.MovementProfile).Returns(movement.Object);
		vehicle.SetupGet(x => x.Location).Returns(location.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.Destroyed).Returns(false);
		vehicle.SetupGet(x => x.Occupancies).Returns([occupancy.Object]);
		vehicle.Setup(x => x.IsOccupant(target.Object)).Returns(true);
		vehicle.Setup(x => x.IsOccupant(attacker.Object)).Returns(false);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { vehicle.Object });
		target.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		attacker.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		return new Harness(attacker, target, vehicle, exterior, belowCover, slot, gameworld, location);
	}

	private sealed record Harness(Mock<ICharacter> Attacker, Mock<ICharacter> Target, Mock<IVehicle> Vehicle,
		Mock<IGameItem> Exterior, Mock<IRangedCover> BelowCover, Mock<IVehicleOccupantSlotPrototype> Slot,
		Mock<IFuturemud> Gameworld, Mock<ICell> Location);
}
