#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Vehicles;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleRouteExitStepLoadTests
{
	[TestMethod]
	public void Constructor_ExitNotPreloaded_InitialisesOriginTopologyAndResolvesExit()
	{
		const long exitId = 401L;
		var (gameworld, origin, _) = CreateGameworld();
		var exitManager = Mock.Get(gameworld.Object.ExitManager);
		var persistentExit = new Mock<IExit>();
		var cellExit = new Mock<ICellExit>();
		cellExit.SetupGet(x => x.Exit).Returns(persistentExit.Object);
		persistentExit.SetupGet(x => x.Id).Returns(exitId);
		persistentExit
			.Setup(x => x.CellExitFor(origin.Object))
			.Returns(cellExit.Object);
		exitManager
			.SetupSequence(x => x.GetExitByID(exitId))
			.Returns((IExit)null!)
			.Returns(persistentExit.Object);
		exitManager
			.Setup(x => x.GetAllExits(origin.Object))
			.Returns([cellExit.Object]);

		var step = new VehicleRouteExitStep(null!, CreateStep(exitId), gameworld.Object);

		Assert.AreSame(cellExit.Object, step.Exit);
		exitManager.Verify(x => x.GetAllExits(origin.Object), Times.Once);
		exitManager.Verify(x => x.GetExitByID(exitId), Times.AtLeastOnce);
	}

	[TestMethod]
	public void Constructor_ExitStillUnavailableAfterTopologyInitialisation_ThrowsActionableDiagnostic()
	{
		const long exitId = 401L;
		var (gameworld, origin, _) = CreateGameworld();
		var exitManager = Mock.Get(gameworld.Object.ExitManager);
		exitManager
			.Setup(x => x.GetExitByID(exitId))
			.Returns((IExit)null!);
		exitManager
			.Setup(x => x.GetAllExits(origin.Object))
			.Returns([]);

		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			new VehicleRouteExitStep(null!, CreateStep(exitId), gameworld.Object));

		StringAssert.Contains(exception.Message, "Vehicle route exit step #77");
		StringAssert.Contains(exception.Message, "exit #401");
		StringAssert.Contains(exception.Message, "origin cell #101");
		exitManager.Verify(x => x.GetAllExits(origin.Object), Times.Once);
	}

	private static DB.VehicleRouteStep CreateStep(long exitId)
	{
		return new DB.VehicleRouteStep
		{
			Id = 77L,
			Sequence = 0,
			StepType = (int)VehicleRouteStepType.CellExit,
			OriginCellId = 101L,
			OriginRoomLayer = (int)RoomLayer.GroundLevel,
			DestinationCellId = 202L,
			DestinationRoomLayer = (int)RoomLayer.GroundLevel,
			RoomEquivalentCost = 1.0m,
			ExitId = exitId
		};
	}

	private static (Mock<IFuturemud> Gameworld, Mock<ICell> Origin, Mock<ICell> Destination) CreateGameworld()
	{
		var origin = new Mock<ICell>();
		origin.SetupGet(x => x.Id).Returns(101L);
		var destination = new Mock<ICell>();
		destination.SetupGet(x => x.Id).Returns(202L);
		var cells = new Mock<IUneditableAll<ICell>>();
		cells.Setup(x => x.Get(101L)).Returns(origin.Object);
		cells.Setup(x => x.Get(202L)).Returns(destination.Object);
		var exitManager = new Mock<IExitManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Cells).Returns(cells.Object);
		gameworld.SetupGet(x => x.ExitManager).Returns(exitManager.Object);
		return (gameworld, origin, destination);
	}
}
