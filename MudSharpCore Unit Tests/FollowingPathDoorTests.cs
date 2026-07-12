using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class FollowingPathDoorTests
{
	[TestMethod]
	public void HasOtherWalkers_OtherCharacterUsingSameExit_ReturnsTrue()
	{
		var fixture = CreateFixture(otherUsesSameExit: true);

		Assert.IsTrue(FollowingPath.HasOtherWalkers(fixture.Actor.Object, fixture.CellExit.Object));
	}

	[TestMethod]
	public void HasOtherWalkers_OtherCharacterUsingDifferentExit_ReturnsFalse()
	{
		var fixture = CreateFixture(otherUsesSameExit: false);

		Assert.IsFalse(FollowingPath.HasOtherWalkers(fixture.Actor.Object, fixture.CellExit.Object));
	}

	[TestMethod]
	public void HasOtherWalkers_NoOtherMovement_ReturnsFalse()
	{
		var fixture = CreateFixture(otherUsesSameExit: null);

		Assert.IsFalse(FollowingPath.HasOtherWalkers(fixture.Actor.Object, fixture.CellExit.Object));
	}

	[TestMethod]
	public void CloseDoorBehind_OtherWalkerUsingExit_DoesNotClose()
	{
		var fixture = CreateFixture(otherUsesSameExit: true, includeOpenDoor: true);

		FollowingPath.CloseDoorBehind(fixture.Actor.Object, fixture.CellExit.Object, useKeys: false);

		fixture.Body.Verify(x => x.Close(fixture.Door!.Object, default!, default!), Times.Never);
	}

	[TestMethod]
	public void CloseDoorBehind_MustSecureWithOtherWalker_Closes()
	{
		var fixture = CreateFixture(otherUsesSameExit: true, includeOpenDoor: true);

		FollowingPath.CloseDoorBehind(fixture.Actor.Object, fixture.CellExit.Object, useKeys: false,
			mustSecure: true);

		fixture.Body.Verify(x => x.Close(fixture.Door!.Object, default!, default!), Times.Once);
	}

	private static DoorMovementFixture CreateFixture(bool? otherUsesSameExit, bool includeOpenDoor = false)
	{
		var actor = new Mock<ICharacter>();
		var other = new Mock<ICharacter>();
		actor.Setup(x => x.SamePhysicalInstance(other.Object)).Returns(false);
		var body = new Mock<IBody>();
		actor.Setup(x => x.Body).Returns(body.Object);

		var origin = new Mock<ICell>();
		var destination = new Mock<ICell>();
		origin.Setup(x => x.Characters).Returns(new[] { actor.Object, other.Object });
		destination.Setup(x => x.Characters).Returns([]);

		var exit = new Mock<IExit>();
		Mock<IDoor>? door = null;
		if (includeOpenDoor)
		{
			door = new Mock<IDoor>();
			door.Setup(x => x.IsOpen).Returns(true);
			exit.Setup(x => x.Door).Returns(door.Object);
		}

		var cellExit = new Mock<ICellExit>();
		cellExit.Setup(x => x.Exit).Returns(exit.Object);
		cellExit.Setup(x => x.Origin).Returns(origin.Object);
		cellExit.Setup(x => x.Destination).Returns(destination.Object);

		if (otherUsesSameExit.HasValue)
		{
			var movementExit = otherUsesSameExit.Value ? exit : new Mock<IExit>();
			var movementCellExit = new Mock<ICellExit>();
			movementCellExit.Setup(x => x.Exit).Returns(movementExit.Object);
			var movement = new Mock<IMovement>();
			movement.Setup(x => x.Exit).Returns(movementCellExit.Object);
			movement.Setup(x => x.Cancelled).Returns(false);
			other.Setup(x => x.Movement).Returns(movement.Object);
		}

		return new DoorMovementFixture(actor, body, cellExit, door);
	}

	private sealed record DoorMovementFixture(Mock<ICharacter> Actor, Mock<IBody> Body,
		Mock<ICellExit> CellExit, Mock<IDoor>? Door);
}
