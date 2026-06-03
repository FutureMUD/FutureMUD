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
	public void IdleBuildSurvivesCellAddedBetweenSlices()
	{
		All<ICell> allCells = new();
		Mock<IFuturemud> gameworld = new();
		gameworld.Setup(x => x.Cells).Returns(allCells);
		PathfindingService service = new(gameworld.Object)
		{
			MaximumCellsPerIdleSlice = 1
		};

		CellStub cellA = new()
		{
			Id = 4001,
			Name = "Original A",
			Gameworld = gameworld.Object,
			Room = new RoomStub { X = 0, Y = 0, Z = 0 }.ToMock(),
			Exits = new List<CellExitStub>()
		};
		CellStub cellB = new()
		{
			Id = 4002,
			Name = "Original B",
			Gameworld = gameworld.Object,
			Room = new RoomStub { X = 11, Y = 0, Z = 0 }.ToMock(),
			Exits = new List<CellExitStub>()
		};
		CellStub addedCell = new()
		{
			Id = 4003,
			Name = "Added Mid Build",
			Gameworld = gameworld.Object,
			Room = new RoomStub { X = 22, Y = 0, Z = 0 }.ToMock(),
			Exits = new List<CellExitStub>()
		};

		List<Mock<ICell>> initialMocks = new() { cellA.ToMock(), cellB.ToMock() };
		allCells.Add(cellA.GetObject(initialMocks));
		allCells.Add(cellB.GetObject(initialMocks));

		service.RequestIndexWarmup();
		service.DoIdleWork(TimeSpan.FromSeconds(1));

		List<Mock<ICell>> addedMocks = new() { addedCell.ToMock() };
		allCells.Add(addedCell.GetObject(addedMocks));

		service.DoIdleWork(TimeSpan.FromSeconds(1));
		service.DoIdleWork(TimeSpan.FromSeconds(1));

		Assert.IsTrue(service.Diagnostics.CurrentSnapshotVersion > 0,
			"Expected a live cell addition between idle slices not to crash the in-progress index build.");
		Assert.AreEqual(2, service.Diagnostics.SnapshotCellCount,
			"Expected the in-progress build to publish the complete snapshot it started with.");
		Assert.IsTrue(service.Diagnostics.IsDirty,
			"Expected the completed snapshot to stay dirty because the live cell list changed during the build.");
		Assert.IsTrue(service.Diagnostics.IsBuildQueued,
			"Expected the service to queue another build for the newly added live cell.");
	}
}
