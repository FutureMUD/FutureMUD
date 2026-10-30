#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DirectionSenseVehicleTests
{
	[TestMethod]
	[DataRow(Outcome.Pass, Outcome.Fail, CardinalDirection.North, 0)]
	[DataRow(Outcome.Fail, Outcome.Pass, CardinalDirection.Unknown, 1)]
	[DataRow(Outcome.Fail, Outcome.Fail, CardinalDirection.East, 1)]
	public void ManualDrive_UsesControllerSequenceAndFullMovement(Outcome initial, Outcome recovery,
		CardinalDirection expected, int recoveryRolls)
	{
		var f = new Fixture(initial, recovery);
		Assert.IsTrue(VehicleMovement.CanStartManualMove(f.Vehicle.Object, f.Actor.Object, f.Intended.Object, out var reason), reason);

		VehicleMovementCommand.TryMoveControlledVehicle(f.Actor.Object, "north !", true);

		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(recoveryRolls, f.RecoveryRolls);
		if (expected == CardinalDirection.Unknown)
		{
			Assert.IsNull(f.Actor.Object.Movement);
			f.Vehicle.Verify(x => x.BeginMoveToRoom(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), It.IsAny<IRoomExit>()), Times.Never);
		}
		else
		{
			Assert.IsNotNull(f.Actor.Object.Movement);
			Assert.AreEqual(expected, f.Actor.Object.Movement.Exit.OutboundDirection);
			Assert.AreEqual(CardinalDirection.North, f.Actor.Object.Movement.ExitFor(f.Actor.Object)!.OutboundDirection);
			Assert.AreEqual(expected, f.Actor.Object.Movement.ExitFor(Mock.Of<IPerceiver>())!.OutboundDirection);
			f.Vehicle.Verify(x => x.BeginMoveToRoom(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), It.IsAny<IRoomExit>()), Times.Once);
		}
	}

	[TestMethod]
	public void ManualDrive_CompassWithExitKeyword_CannotBypassOrientation()
	{
		var f = new Fixture(Outcome.Fail, Outcome.Pass);
		f.Origin.Setup(x => x.GetExit("north", "north", f.Actor.Object)).Returns(f.Intended.Object);
		VehicleMovementCommand.TryMoveControlledVehicle(f.Actor.Object, "north north", true);
		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(1, f.RecoveryRolls);
		Assert.IsNull(f.Actor.Object.Movement);
		f.Vehicle.Verify(x => x.BeginMoveToRoom(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), It.IsAny<IRoomExit>()), Times.Never);
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void ManualDrive_HiddenOrFallAlternative_MakesNoProgressWithoutResourceCommit(bool falling)
	{
		var f = new Fixture(Outcome.Fail, Outcome.Fail);
		if (falling) f.Wrong.SetupGet(x => x.IsFallExit).Returns(true);
		else f.Body.Setup(x => x.CanSee(f.Origin.Object, f.Wrong.Object, PerceiveIgnoreFlags.None)).Returns(false);
		VehicleMovementCommand.TryMoveControlledVehicle(f.Actor.Object, "north !", true);
		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(1, f.RecoveryRolls);
		Assert.IsNull(f.Actor.Object.Movement);
		f.Vehicle.Verify(x => x.BeginMoveToRoom(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), It.IsAny<IRoomExit>()), Times.Never);
		f.Output.Verify(x => x.Send(DirectionSenseMovement.NoProgressMessage, true, false), Times.Once);
	}

	[TestMethod]
	public void ManualDrive_AlternativeBecomesUnusableAtDispatch_ReportsOnlyGenericFailure()
	{
		var f = new Fixture(Outcome.Fail, Outcome.Fail);
		f.Actor.Setup(x => x.MoveSpeed(f.Wrong.Object)).Callback(() => f.Vehicle.SetupGet(x => x.Disabled).Returns(true)).Returns(1000.0);
		VehicleMovementCommand.TryMoveControlledVehicle(f.Actor.Object, "north", true);
		Assert.IsNull(f.Actor.Object.Movement);
		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(1, f.RecoveryRolls);
		f.Output.Verify(x => x.Send(DirectionSenseMovement.NoProgressMessage, true, false), Times.Once);
		f.Output.Verify(x => x.Send(It.Is<string>(s => s.Contains("disabled")), true, false), Times.Never);
		f.Vehicle.Verify(x => x.BeginMoveToRoom(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), It.IsAny<IRoomExit>()), Times.Never);
	}

	[TestMethod]
	public void ManualDrive_InvalidRequestedExit_DoesNotCheckDirectionSense()
	{
		var f = new Fixture(Outcome.Pass, Outcome.Pass);
		f.Body.Setup(x => x.CanSee(f.Origin.Object, f.Intended.Object, PerceiveIgnoreFlags.None)).Returns(false);
		VehicleMovementCommand.TryMoveControlledVehicle(f.Actor.Object, "north", true);
		Assert.AreEqual(0, f.InitialRolls);
		Assert.AreEqual(0, f.RecoveryRolls);
		Assert.IsNull(f.Actor.Object.Movement);
	}

	[TestMethod]
	public void ManualDrive_FullReadinessRejectsDisabledVehicleWithoutOrientation()
	{
		var f = new Fixture(Outcome.Pass, Outcome.Pass);
		f.Vehicle.SetupGet(x => x.Disabled).Returns(true);
		VehicleMovementCommand.TryMoveControlledVehicle(f.Actor.Object, "north", true);
		Assert.AreEqual(0, f.InitialRolls);
		Assert.AreEqual(0, f.RecoveryRolls);
		Assert.IsNull(f.Actor.Object.Movement);
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IBody> Body { get; } = new();
		public Mock<IOutputHandler> Output { get; } = new();
		public Mock<IRoom> Origin { get; } = new();
		public Mock<IVehicle> Vehicle { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IRoomExit> Intended { get; }
		public Mock<IRoomExit> Wrong { get; }
		public int InitialRolls { get; private set; }
		public int RecoveryRolls { get; private set; }

		public Fixture(Outcome initial, Outcome recovery)
		{
			PhysicalManipulationTestHelper.SetUpUsableHands(Actor, Body);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.Location).Returns(Origin.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Output.Object);
			Actor.SetupGet(x => x.QueuedMoveCommands).Returns(new Queue<string>());
			Actor.SetupGet(x => x.InstanceId).Returns(10L);
			Actor.Setup(x => x.SamePhysicalInstance(Actor.Object)).Returns(true);
			Actor.Setup(x => x.MoveSpeed(It.IsAny<IRoomExit>())).Returns(1000.0);
			Actor.SetupProperty(x => x.Movement);
			Origin.SetupGet(x => x.Gameworld).Returns(World.Object);
			Vehicle.SetupGet(x => x.Id).Returns(1L);
			World.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { Vehicle.Object });
			World.Setup(x => x.GetStaticConfiguration(It.IsAny<string>())).Returns("Normal");
			World.Setup(x => x.GetStaticBool("DirectionSenseEnabled")).Returns(true);
			World.Setup(x => x.GetStaticLong("DirectionSenseObscurationProg")).Returns(7);
			var prog = new Mock<IFutureProg>();
			prog.SetupGet(x => x.Id).Returns(7L);
			prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
			prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
			prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
			World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg> { prog.Object });
			Body.Setup(x => x.CanSee(Origin.Object, It.IsAny<IRoomExit>(), PerceiveIgnoreFlags.None)).Returns(true);
			Vehicle.SetupGet(x => x.Id).Returns(1L);
			Vehicle.SetupGet(x => x.Controller).Returns(Actor.Object);
			Vehicle.SetupGet(x => x.Location).Returns(Origin.Object);
			Vehicle.SetupGet(x => x.Gameworld).Returns(World.Object);
			var exterior = new Mock<IGameItem>();
			exterior.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
			exterior.SetupGet(x => x.Location).Returns(Origin.Object);
			Vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
			var profile = new Mock<IVehicleMovementProfilePrototype>();
			profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.RoomExit);
			var prototype = new Mock<IVehiclePrototype>();
			prototype.SetupGet(x => x.Scale).Returns(VehicleScale.ItemScale);
			prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
			Vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
			Vehicle.SetupGet(x => x.MovementProfile).Returns(profile.Object);
			string reason = "";
			Vehicle.Setup(x => x.CanMove(Actor.Object, It.IsAny<IRoomExit>(), out reason)).Returns(true);
			Intended = Exit(CardinalDirection.North);
			Wrong = Exit(CardinalDirection.East);
			Origin.Setup(x => x.GetExit(CardinalDirection.North, Actor.Object)).Returns(Intended.Object);
			Origin.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), false)).Returns([Intended.Object, Wrong.Object]);
			Check(CheckType.DirectionSenseCheck, true, initial, () => InitialRolls++);
			Check(CheckType.DirectionSenseRecoveryCheck, false, recovery, () => RecoveryRolls++);
		}

		private Mock<IRoomExit> Exit(CardinalDirection direction)
		{
			var exit = new Mock<IRoomExit>();
			exit.SetupGet(x => x.Origin).Returns(Origin.Object);
			exit.SetupGet(x => x.Destination).Returns(Mock.Of<IRoom>());
			exit.SetupGet(x => x.OutboundDirection).Returns(direction);
			exit.SetupGet(x => x.Exit).Returns(Mock.Of<IExit>(x => x.MaximumSizeToEnter == SizeCategory.Huge));
			exit.Setup(x => x.MovementTransition(It.IsAny<IPerceiver>())).Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
			return exit;
		}

		private void Check(CheckType type, bool improve, Outcome outcome, Action rolled)
		{
			var expression = new Mock<ITraitExpression>();
			expression.SetupGet(x => x.Parameters).Returns(new Dictionary<string, TraitExpressionParameter>
			{
				["directionsense"] = new() { Trait = Mock.Of<ITraitDefinition>(x => x.Id == 42), CanImprove = improve }
			});
			var check = new Mock<ICheck>();
			check.SetupGet(x => x.Type).Returns(type);
			check.SetupGet(x => x.ImproveTraits).Returns(improve);
			check.SetupGet(x => x.FailIfTraitMissing).Returns(FailIfTraitMissingType.FailIfAnyMissing);
			check.SetupGet(x => x.TargetNumberExpression).Returns(expression.Object);
			check.Setup(x => x.Check(Actor.Object, Difficulty.Normal, (IPerceivable)null!, null, 0.0,
				TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>())).Callback(rolled)
				.Returns(CheckOutcome.SimpleOutcome(type, outcome));
			World.Setup(x => x.GetCheck(type)).Returns(check.Object);
		}
	}
}
