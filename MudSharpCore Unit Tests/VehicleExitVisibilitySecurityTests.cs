#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleExitVisibilitySecurityTests
{
	[DataTestMethod]
	[DataRow(true, false)]
	[DataRow(false, false)]
	[DataRow(true, true)]
	[DataRow(false, true)]
	public void TryMoveControlledVehicle_ResolvedExit_RechecksDriverVisibility(bool cardinal, bool visible)
	{
		var fixture = new Fixture(cardinal);
		fixture.Body.Setup(x => x.CanSee(fixture.Room.Object, fixture.Exit.Object, PerceiveIgnoreFlags.None)).Returns(visible);

		var result = VehicleMovementCommand.TryMoveControlledVehicle(fixture.Actor.Object, cardinal ? "north" : "portal", true);

		Assert.AreEqual(VehicleMovementCommandResult.Failed, result);
		fixture.Vehicle.Verify(x => x.CanMove(fixture.Actor.Object, fixture.Exit.Object, out It.Ref<string>.IsAny), visible ? Times.Once() : Times.Never());
		fixture.Output.Verify(x => x.Send("There is no such exit for the vehicle to use.", true, false), visible ? Times.Never() : Times.Once());
	}

	[TestMethod]
	public void TryMoveControlledVehicle_QueuedCardinalExitBecomesHidden_ReplayRejectsIt()
	{
		var fixture = new Fixture(true);
		fixture.Actor.SetupGet(x => x.Movement).Returns(Mock.Of<IMovement>());
		Assert.AreEqual(VehicleMovementCommandResult.StartedOrQueued,
			VehicleMovementCommand.TryMoveControlledVehicle(fixture.Actor.Object, "north", true));
		Assert.AreEqual(1, fixture.Queue.Count);
		fixture.Actor.SetupGet(x => x.Movement).Returns((IMovement)null!);

		Assert.AreEqual(VehicleMovementCommandResult.Failed,
			VehicleMovementCommand.TryMoveControlledVehicle(fixture.Actor.Object, fixture.Queue.Dequeue(), true));
		fixture.Vehicle.Verify(x => x.CanMove(fixture.Actor.Object, fixture.Exit.Object, out It.Ref<string>.IsAny), Times.Never);
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IBody> Body { get; } = new();
		public Mock<IRoom> Room { get; } = new();
		public Mock<IRoomExit> Exit { get; } = new();
		public Mock<IVehicle> Vehicle { get; } = new();
		public Mock<IOutputHandler> Output { get; } = new();
		public Queue<string> Queue { get; } = new();

		public Fixture(bool cardinal)
		{
			var world = new Mock<IFuturemud>();
			Vehicle.SetupGet(x => x.Id).Returns(1L);
			Actor.SetupGet(x => x.Id).Returns(10L);
			Actor.SetupGet(x => x.InstanceId).Returns(10L);
			Actor.Setup(x => x.SamePhysicalInstance(Actor.Object)).Returns(true);
			world.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { Vehicle.Object });
			Actor.SetupGet(x => x.Gameworld).Returns(world.Object);
			Actor.SetupGet(x => x.Location).Returns(Room.Object);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Output.Object);
			Actor.SetupGet(x => x.QueuedMoveCommands).Returns(Queue);
			Vehicle.SetupGet(x => x.Controller).Returns(Actor.Object);
			Vehicle.SetupGet(x => x.Location).Returns(Room.Object);
			Vehicle.SetupGet(x => x.Prototype).Returns(Mock.Of<IVehiclePrototype>(x => x.Scale == VehicleScale.ItemScale));
			if (cardinal)
			{
				Room.Setup(x => x.GetExit(CardinalDirection.North, Actor.Object)).Returns(Exit.Object);
			}
			else
			{
				Room.Setup(x => x.GetExit("portal", "", Actor.Object)).Returns(Exit.Object);
			}
			string reason = "ordinary preflight reached";
			Vehicle.Setup(x => x.CanMove(Actor.Object, Exit.Object, out reason)).Returns(false);
		}
	}
}
