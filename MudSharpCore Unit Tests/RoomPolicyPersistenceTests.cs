#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using NativeExit = MudSharp.Construction.Boundary.Exit;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomPolicyPersistenceTests
{
	[TestMethod]
	public void ClonedVerticalExit_ReversedOriginRetainsFallClimbAndBlockedLayers()
	{
		Mock<IRoom> Room(long id) { var room = new Mock<IRoom>(); room.SetupGet(x => x.Id).Returns(id); return room; }
		var lower = Room(1); var upper = Room(2); var newUpper = Room(22); var newLower = Room(11);
		var rooms = new All<IRoom>(); rooms.Add(lower.Object); rooms.Add(upper.Object);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.Rooms).Returns(rooms);
		var source = new NativeExit(new MudSharp.Models.Exit { Id = 1, RoomId1 = 1, RoomId2 = 2,
			Direction1 = (int)CardinalDirection.Up, Direction2 = (int)CardinalDirection.Down,
			IsClimbExit = true, ClimbDifficulty = (int)Difficulty.Hard, FallRoom = 1,
			BlockedLayers = ((int)RoomLayer.InAir).ToString() }, world.Object);
		var clone = new NativeExit(world.Object, newUpper.Object, newLower.Object, source, upper.Object);
		Assert.AreEqual(CardinalDirection.Down, clone.RoomExitFor(newUpper.Object).OutboundDirection);
		Assert.AreEqual(CardinalDirection.Up, clone.RoomExitFor(newLower.Object).OutboundDirection);
		Assert.AreSame(newLower.Object, clone.FallRoom);
		Assert.IsTrue(clone.IsClimbExit); Assert.AreEqual(Difficulty.Hard, clone.ClimbDifficulty);
		CollectionAssert.AreEqual(new[] { RoomLayer.InAir }, clone.BlockedLayers.ToArray());
	}
}
