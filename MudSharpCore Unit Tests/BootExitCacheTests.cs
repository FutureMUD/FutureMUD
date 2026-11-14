#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BootExitCacheTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void InitialiseRoom_CachedAndDirectRowsPreserveNativeExitProperties(bool prefetch)
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = new Mock<IFuturemud>();
		var rooms = new All<IRoom>();
		var overlay = new Mock<IRoomOverlay>(); overlay.SetupGet(x => x.ExitIDs).Returns(new long[] { 1, 1, 2, 3 });
		Mock<IRoom> Room(long id)
		{
			var room = new Mock<IRoom>(); room.SetupGet(x => x.Id).Returns(id);
			room.SetupGet(x => x.Overlays).Returns([overlay.Object]); rooms.Add(room.Object); return room;
		}
		var origin = Room(1); Room(2); Room(3);
		world.SetupGet(x => x.Rooms).Returns(rooms);
		var vertical = Row(1, 1, 2); vertical.Direction1 = (int)CardinalDirection.Up; vertical.Direction2 = (int)CardinalDirection.Down;
		vertical.AcceptsDoor = true; vertical.DoorSize = (int)SizeCategory.Large;
		vertical.FallRoom = 1; vertical.IsClimbExit = true; vertical.ClimbDifficulty = (int)Difficulty.Hard;
		vertical.BlockedLayers = ((int)RoomLayer.InAir).ToString(); vertical.TimeMultiplier = 2.5;
		var hatch = Row(2, 1, 3); hatch.Verb1 = "enter"; hatch.Verb2 = "leave";
		hatch.Keywords1 = "hatch"; hatch.Keywords2 = "hatch"; hatch.PrimaryKeyword1 = "hatch"; hatch.PrimaryKeyword2 = "hatch";
		database.Context.Exits.AddRange(vertical, hatch, Row(3, 2, 3)); database.Context.SaveChanges();
		var manager = new ExitManager(world.Object);
		if (prefetch) manager.PrepareBootCache();
		var exits = manager.GetAllExits(origin.Object).ToArray();
		CollectionAssert.AreEquivalent(new long[] { 1, 2 }, exits.Select(x => x.Exit.Id).ToArray());
		var up = exits.Single(x => x.Exit.Id == 1);
		Assert.AreEqual(CardinalDirection.Up, up.OutboundDirection);
		Assert.IsTrue(up.Exit.AcceptsDoor); Assert.AreEqual(SizeCategory.Large, up.Exit.DoorSize);
		Assert.AreSame(origin.Object, up.Exit.FallRoom); Assert.IsTrue(up.Exit.IsClimbExit);
		Assert.AreEqual(Difficulty.Hard, up.Exit.ClimbDifficulty); Assert.AreEqual(2.5, up.Exit.TimeMultiplier);
		CollectionAssert.AreEqual(new[] { RoomLayer.InAir }, up.Exit.BlockedLayers.ToArray());
		Assert.IsInstanceOfType<NonCardinalRoomExit>(exits.Single(x => x.Exit.Id == 2));
		origin.Verify(x => x.OnExitsInitialised(), Times.Once);
		manager.ReleaseBootCache();
	}

	[TestMethod]
	public void InitialiseRoom_UnknownNewExitFallsBackAndOverlayUpdatesRemainVisible()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = new Mock<IFuturemud>(); var rooms = new All<IRoom>();
		var ids = new List<long> { 1 }; var overlay = new Mock<IRoomOverlay>(); overlay.SetupGet(x => x.ExitIDs).Returns(ids);
		var origin = new Mock<IRoom>(); origin.SetupGet(x => x.Id).Returns(1);
		origin.SetupGet(x => x.Overlays).Returns([overlay.Object]);
		var target = new Mock<IRoom>(); target.SetupGet(x => x.Id).Returns(2);
		rooms.Add(origin.Object); rooms.Add(target.Object); world.SetupGet(x => x.Rooms).Returns(rooms);
		database.Context.Exits.Add(Row(1, 1, 2)); database.Context.SaveChanges();
		var manager = new ExitManager(world.Object); manager.PrepareBootCache();
		database.Context.Exits.Add(Row(2, 1, 2)); database.Context.SaveChanges(); ids.Add(2);
		CollectionAssert.AreEquivalent(new long[] { 1, 2 }, manager.GetAllExits(origin.Object).Select(x => x.Exit.Id).ToArray());
		ids.Remove(1); manager.UpdateRoomOverlayExits(origin.Object, overlay.Object);
		CollectionAssert.AreEqual(new long[] { 2 }, manager.GetAllExits(origin.Object).Select(x => x.Exit.Id).ToArray());
	}

	private static MudSharp.Models.Exit Row(long id, long a, long b) => new()
	{
		Id = id, RoomId1 = a, RoomId2 = b, BlockedLayers = "", Direction1 = (int)CardinalDirection.North,
		Direction2 = (int)CardinalDirection.South, InboundDescription1 = "", InboundDescription2 = "",
		OutboundDescription1 = "", OutboundDescription2 = "", InboundTarget1 = "", InboundTarget2 = "",
		OutboundTarget1 = "", OutboundTarget2 = "", TimeMultiplier = 1
	};
}
