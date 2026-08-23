#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MountedCombatServiceTests
{
	[TestMethod]
	public void ResolveContext_PrimaryRiderOnFlyingMount_UsesAerialDomainAndMountSize()
	{
		var rider = new Mock<ICharacter>();
		var mount = new Mock<ICharacter>();
		rider.SetupGet(x => x.RidingMount).Returns(mount.Object);
		mount.Setup(x => x.IsPrimaryRider(rider.Object)).Returns(true);
		mount.SetupGet(x => x.PositionState).Returns(PositionFlying.Instance);
		mount.Setup(x => x.MoveSpeed(null!)).Returns(0.7);
		mount.Setup(x => x.CurrentContextualSize(SizeContext.BeingRiddenAsMount)).Returns(SizeCategory.VeryLarge);

		var context = MountedCombatService.Instance.ResolveContext(rider.Object);

		Assert.IsNotNull(context);
		Assert.AreEqual(MountedCombatDomain.Aerial, context.Domain);
		Assert.AreEqual(SizeCategory.VeryLarge, context.EffectiveSize);
		Assert.AreSame(mount.Object, context.Conveyance);
		Assert.AreEqual(BuiltInCombatMoveType.AerialMountedCharge,
			MountedCombatService.Instance.ChargeMessageType(context));
		Assert.AreEqual(CheckType.AerialMountedChargeCheck,
			MountedCombatService.Instance.ChargeCheckType(context));
	}

	[TestMethod]
	public void ResolveContext_SecondaryRider_DoesNotControlMountedCharge()
	{
		var rider = new Mock<ICharacter>();
		var mount = new Mock<ICharacter>();
		var gameworld = new Mock<IFuturemud>();
		rider.SetupGet(x => x.RidingMount).Returns(mount.Object);
		rider.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		gameworld.SetupGet(x => x.Vehicles).Returns(new All<IVehicle>());
		mount.Setup(x => x.IsPrimaryRider(rider.Object)).Returns(false);

		Assert.IsNull(MountedCombatService.Instance.ResolveContext(rider.Object));
	}

	[TestMethod]
	public void ResolveContext_ControlledSurfaceWaterVehicle_UsesSeafaringChargeDomain()
	{
		var controller = new Mock<ICharacter>();
		var gameworld = new Mock<IFuturemud>();
		var vehicle = new Mock<IVehicle>();
		var exterior = new Mock<IGameItem>();
		var movement = new Mock<IVehicleMovementProfilePrototype>();
		var propulsion = new Mock<IVehiclePropulsionProfilePrototype>();
		controller.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		controller.Setup(x => x.SamePhysicalInstance(controller.Object)).Returns(true);
		vehicle.SetupGet(x => x.Id).Returns(1L);
		gameworld.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { vehicle.Object });
		vehicle.Setup(x => x.IsOccupant(controller.Object)).Returns(true);
		vehicle.SetupGet(x => x.Controller).Returns(controller.Object);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.MovementProfile).Returns(movement.Object);
		vehicle.SetupGet(x => x.ActivePropulsionProfile).Returns(propulsion.Object);
		movement.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.SurfaceWater);
		movement.SetupGet(x => x.RouteSpeedMetresPerSecond).Returns(8.0);
		exterior.SetupGet(x => x.Size).Returns(SizeCategory.Huge);

		var context = MountedCombatService.Instance.ResolveContext(controller.Object);

		Assert.IsNotNull(context);
		Assert.AreEqual(MountedCombatDomain.AquaticVehicle, context.Domain);
		Assert.AreSame(vehicle.Object, context.Vehicle);
		Assert.AreEqual(8.0, context.Momentum, 0.001);
		Assert.AreEqual(CheckType.AquaticVehicleChargeCheck,
			MountedCombatService.Instance.ChargeCheckType(context));
	}

	[TestMethod]
	public void ResolveMountSprawl_PassedSeatCheck_DismountsRiderWithoutInjury()
	{
		var mount = new Mock<ICharacter>();
		var rider = new Mock<ICharacter>();
		var gameworld = new Mock<IFuturemud>();
		var check = SeatCheck(Outcome.Pass);
		var output = new Mock<IOutputHandler>();
		mount.SetupGet(x => x.Riders).Returns([rider.Object]);
		mount.Setup(x => x.CurrentContextualSize(SizeContext.BeingRiddenAsMount)).Returns(SizeCategory.VeryLarge);
		mount.SetupGet(x => x.Weight).Returns(600.0);
		rider.Setup(x => x.CurrentContextualSize(SizeContext.RidingMount)).Returns(SizeCategory.Normal);
		rider.SetupGet(x => x.Weight).Returns(75.0);
		rider.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		rider.SetupGet(x => x.OutputHandler).Returns(output.Object);
		gameworld.Setup(x => x.GetCheck(CheckType.AvoidMountFallCheck)).Returns(check.Object);

		MountedCombatService.Instance.ResolveMountSprawl(mount.Object, 1);

		mount.Verify(x => x.RemoveRider(rider.Object), Times.Once);
		rider.Verify(x => x.DoCombatKnockdown(It.IsAny<int>(), It.IsAny<VehicleCombatDisplacementType>()),
			Times.Never);
		rider.Verify(x => x.DoFallDamage(It.IsAny<double>()), Times.Never);
		check.Verify(x => x.Check(rider.Object, Difficulty.VeryHard, mount.Object, null, -10.0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()), Times.Once);
	}

	[TestMethod]
	public void ResolveMountSprawl_FailedSeatCheck_CrushesAndSprawlsRider()
	{
		var mount = new Mock<ICharacter>();
		var rider = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		var bodypart = new Mock<IBodypart>();
		var gameworld = new Mock<IFuturemud>();
		var check = SeatCheck(Outcome.Fail);
		var output = new Mock<IOutputHandler>();
		mount.SetupGet(x => x.Riders).Returns([rider.Object]);
		mount.Setup(x => x.CurrentContextualSize(SizeContext.BeingRiddenAsMount)).Returns(SizeCategory.Huge);
		mount.SetupGet(x => x.Weight).Returns(900.0);
		rider.Setup(x => x.CurrentContextualSize(SizeContext.RidingMount)).Returns(SizeCategory.Normal);
		rider.SetupGet(x => x.Weight).Returns(75.0);
		rider.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		rider.SetupGet(x => x.OutputHandler).Returns(output.Object);
		rider.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.ExternalItems).Returns([]);
		body.SetupGet(x => x.ExternalItemsForOtherActors).Returns([]);
		body.SetupGet(x => x.RandomBodypart).Returns(bodypart.Object);
		rider.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns([]);
		gameworld.Setup(x => x.GetCheck(CheckType.AvoidMountFallCheck)).Returns(check.Object);

		MountedCombatService.Instance.ResolveMountSprawl(mount.Object, 2);

		mount.Verify(x => x.RemoveRider(rider.Object), Times.Once);
		rider.Verify(x => x.DoCombatKnockdown(1, VehicleCombatDisplacementType.Knockdown), Times.Once);
		rider.Verify(x => x.PassiveSufferDamage(It.Is<IDamage>(damage =>
			damage.ActorOrigin == mount.Object && damage.Bodypart == bodypart.Object &&
			damage.DamageType == DamageType.Crushing && damage.DamageAmount > 15.0)), Times.Once);
	}

	[TestMethod]
	public void ResolveMountSprawl_MissingDedicatedCheck_UsesRidingTraitFallback()
	{
		var mount = new Mock<ICharacter>();
		var rider = new Mock<ICharacter>();
		var gameworld = new Mock<IFuturemud>();
		var missingCheck = new Mock<ICheck>();
		var genericCheck = new Mock<ICheck>();
		var riding = new Mock<ITraitDefinition>();
		var output = new Mock<IOutputHandler>();
		missingCheck.SetupGet(x => x.Type).Returns(CheckType.None);
		genericCheck.SetupGet(x => x.Type).Returns(CheckType.GenericSkillCheck);
		genericCheck.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Pass));
		riding.SetupGet(x => x.Name).Returns("Riding");
		riding.SetupGet(x => x.Id).Returns(1L);
		mount.SetupGet(x => x.Riders).Returns([rider.Object]);
		mount.Setup(x => x.CurrentContextualSize(SizeContext.BeingRiddenAsMount)).Returns(SizeCategory.Large);
		mount.SetupGet(x => x.Weight).Returns(300.0);
		rider.Setup(x => x.CurrentContextualSize(SizeContext.RidingMount)).Returns(SizeCategory.Normal);
		rider.SetupGet(x => x.Weight).Returns(75.0);
		rider.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		rider.SetupGet(x => x.OutputHandler).Returns(output.Object);
		gameworld.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition> { riding.Object });
		gameworld.Setup(x => x.GetCheck(CheckType.AvoidMountFallCheck)).Returns(missingCheck.Object);
		gameworld.Setup(x => x.GetCheck(CheckType.GenericSkillCheck)).Returns(genericCheck.Object);

		MountedCombatService.Instance.ResolveMountSprawl(mount.Object, 1);

		genericCheck.Verify(x => x.Check(rider.Object, Difficulty.Hard, riding.Object, mount.Object, -10.0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()), Times.Once);
		mount.Verify(x => x.RemoveRider(rider.Object), Times.Once);
	}

	[TestMethod]
	public void MountSprawlScaling_UsesForceSizeAndWeightWithBoundedCrushDamage()
	{
		Assert.AreEqual(Difficulty.Normal,
			MountedCombatService.MountSprawlDifficulty(0, 1.0, 1));
		Assert.AreEqual(Difficulty.Insane,
			MountedCombatService.MountSprawlDifficulty(4, 12.0, 3));
		Assert.IsTrue(
			MountedCombatService.MountCrushDamage(3, 12.0, 3, Outcome.MajorFail) >
			MountedCombatService.MountCrushDamage(1, 4.0, 1, Outcome.Fail));
		Assert.AreEqual(50.0,
			MountedCombatService.MountCrushDamage(12, 1000.0, 10, Outcome.MajorFail));
	}

	private static Mock<ICheck> SeatCheck(Outcome outcome)
	{
		var check = new Mock<ICheck>();
		check.SetupGet(x => x.Type).Returns(CheckType.AvoidMountFallCheck);
		check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.AvoidMountFallCheck, outcome));
		return check;
	}
}
