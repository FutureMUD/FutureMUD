#nullable enable
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomExitLayerTests
{
	[TestMethod]
	[DataRow(CardinalDirection.Up)]
	[DataRow(CardinalDirection.Down)]
	public void OrdinaryVerticalExit_MultilayerTerrain_AllowsGroundLevelWalking(CardinalDirection direction)
	{
		var origin = Room(RoomLayer.GroundLevel, RoomLayer.OnRooftops);
		var destination = Room(RoomLayer.GroundLevel);
		var parent = new Mock<IExit>();
		parent.SetupGet(exit => exit.BlockedLayers).Returns([]);
		var exit = new RoomExit(parent.Object, origin.Object, destination.Object, direction, direction.Opposite());
		CollectionAssert.AreEqual(new[] { RoomLayer.GroundLevel }, exit.WhichLayersExitAppears().ToArray());
		var perceiver = Mock.Of<IPerceiver>(value => value.Location == origin.Object && value.RoomLayer == RoomLayer.GroundLevel);
		var transition = exit.MovementTransition(perceiver);
		Assert.AreEqual(RoomMovementTransition.GroundToGround, transition.TransitionType);
		Assert.AreEqual(RoomLayer.GroundLevel, transition.TargetLayer);
	}

	[TestMethod]
	[DataRow(CardinalDirection.Up)]
	[DataRow(CardinalDirection.Down)]
	public void OrdinaryVerticalExit_BlockedGroundLayer_DoesNotExposeBlockedExit(CardinalDirection direction)
	{
		var origin = Room(RoomLayer.GroundLevel, RoomLayer.OnRooftops);
		var destination = Room(RoomLayer.GroundLevel, RoomLayer.OnRooftops);
		var parent = new Mock<IExit>();
		parent.SetupGet(exit => exit.BlockedLayers).Returns([RoomLayer.GroundLevel]);
		var exit = new RoomExit(parent.Object, origin.Object, destination.Object, direction, direction.Opposite());
		CollectionAssert.AreEqual(new[] { RoomLayer.OnRooftops }, exit.WhichLayersExitAppears().ToArray());
	}

	[TestMethod]
	[DataRow(CardinalDirection.Up, true, false)]
	[DataRow(CardinalDirection.Down, true, false)]
	[DataRow(CardinalDirection.Up, false, true)]
	[DataRow(CardinalDirection.Down, false, true)]
	public void ClimbAndFallExits_KeepTerrainBoundaryLayers(CardinalDirection direction, bool climb, bool fall)
	{
		var origin = Room(RoomLayer.GroundLevel, RoomLayer.OnRooftops);
		var destination = Room(RoomLayer.GroundLevel);
		var parent = new Mock<IExit>();
		parent.SetupGet(exit => exit.BlockedLayers).Returns([]);
		parent.SetupGet(exit => exit.IsClimbExit).Returns(climb);
		if (fall) parent.SetupGet(exit => exit.FallRoom).Returns(direction == CardinalDirection.Up ? origin.Object : destination.Object);
		var exit = new RoomExit(parent.Object, origin.Object, destination.Object, direction, direction.Opposite());
		CollectionAssert.AreEqual(new[] { direction == CardinalDirection.Up ? RoomLayer.OnRooftops : RoomLayer.GroundLevel },
			exit.WhichLayersExitAppears().ToArray());
	}

	private static Mock<IRoom> Room(params RoomLayer[] layers)
	{
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(value => value.TerrainLayers).Returns(layers);
		var room = new Mock<IRoom>();
		room.Setup(value => value.Terrain(It.IsAny<IPerceiver>())).Returns(terrain.Object);
		room.SetupGet(value => value.Location).Returns(room.Object);
		room.Setup(value => value.ExitsFor(It.IsAny<IPerceiver>(), true)).Returns([]);
		return room;
	}
}
