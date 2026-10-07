using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using System;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PathfindingServiceLiveMutationTests
{
	[TestMethod]
	public void IdleBuildSurvivesRoomAddedBetweenSlices()
	{
		All<IRoom> allRooms = new();
		Mock<IFuturemud> gameworld = new();
		gameworld.Setup(x => x.Rooms).Returns(allRooms);
		PathfindingService service = new(gameworld.Object)
		{
			MaximumRoomsPerIdleSlice = 1
		};

		RoomStub cellA = new()
		{
			Id = 4001,
			Name = "Original A",
			Gameworld = gameworld.Object,
			Coordinates = (0, 0, 0 ),
			Exits = new List<RoomExitStub>()
		};
		RoomStub cellB = new()
		{
			Id = 4002,
			Name = "Original B",
			Gameworld = gameworld.Object,
			Coordinates = (11, 0, 0 ),
			Exits = new List<RoomExitStub>()
		};
		RoomStub addedRoom = new()
		{
			Id = 4003,
			Name = "Added Mid Build",
			Gameworld = gameworld.Object,
			Coordinates = (22, 0, 0 ),
			Exits = new List<RoomExitStub>()
		};

		List<Mock<IRoom>> initialMocks = new() { cellA.ToMock(), cellB.ToMock() };
		allRooms.Add(cellA.GetObject(initialMocks));
		allRooms.Add(cellB.GetObject(initialMocks));

		service.RequestIndexWarmup();
		service.DoIdleWork(TimeSpan.FromSeconds(1));

		List<Mock<IRoom>> addedMocks = new() { addedRoom.ToMock() };
		allRooms.Add(addedRoom.GetObject(addedMocks));

		service.DoIdleWork(TimeSpan.FromSeconds(1));
		service.DoIdleWork(TimeSpan.FromSeconds(1));

		Assert.IsTrue(service.Diagnostics.CurrentSnapshotVersion > 0,
			"Expected a live cell addition between idle slices not to crash the in-progress index build.");
		Assert.AreEqual(2, service.Diagnostics.SnapshotRoomCount,
			"Expected the in-progress build to publish the complete snapshot it started with.");
		Assert.IsTrue(service.Diagnostics.IsDirty,
			"Expected the completed snapshot to stay dirty because the live cell list changed during the build.");
		Assert.IsTrue(service.Diagnostics.IsBuildQueued,
			"Expected the service to queue another build for the newly added live cell.");
	}
}
