#nullable enable

using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleTransitLocalityTests
{
	[TestMethod]
	public void AppendActiveJourneyStatus_ShowsTimetableStopsBoardingAndReason()
	{
		var actor = new Mock<ICharacter>();
		var service = new Mock<IVehicleService>();
		service.SetupGet(x => x.Name).Returns("QA express");
		var current = CreateStop(100L, 0, new Mock<ICell>().Object, null, RoomLayer.GroundLevel);
		var next = CreateStop(101L, 1, new Mock<ICell>().Object, null, RoomLayer.GroundLevel);
		var journey = new Mock<IVehicleJourney>();
		journey.SetupGet(x => x.Service).Returns(service.Object);
		journey.SetupGet(x => x.State).Returns(VehicleJourneyState.Held);
		journey.SetupGet(x => x.ScheduledDeparture).Returns(MudDateTime.Never);
		journey.SetupGet(x => x.ExpectedDeparture).Returns(MudDateTime.Never);
		journey.SetupGet(x => x.Delay).Returns(TimeSpan.FromMinutes(3));
		journey.SetupGet(x => x.CurrentStop).Returns(current.Object);
		journey.SetupGet(x => x.NextStop).Returns(next.Object);
		journey.SetupGet(x => x.BoardingOpen).Returns(true);
		journey.SetupGet(x => x.StatusReason).Returns("waiting for clearance");
		var sb = new StringBuilder();

		VehicleModule.AppendActiveJourneyStatus(sb, actor.Object, journey.Object);

		var text = sb.ToString().StripANSIColour();
		StringAssert.Contains(text, "Journey: Held on QA express");
		StringAssert.Contains(text, "Timetable: scheduled");
		StringAssert.Contains(text, "delay 3 minutes");
		StringAssert.Contains(text, "current stop 100, next stop 101");
		StringAssert.Contains(text, "Boarding: open; waiting for clearance");
	}

	[TestMethod]
	public void FindLocalTransitStop_DistantCoordinateInSameRouteCell_IsNotLocal()
	{
		var routeCell = CreateRouteCell();
		var actor = CreateActor(routeCell.Object, 7_100.0, RoomLayer.GroundLevel, 2.0);
		var stop = CreateStop(1L, 0, routeCell.Object, 100.0, RoomLayer.GroundLevel,
			CreateBinding(10L, routeCell.Object, 2.0).Object);
		var route = CreateRoute(stop.Object);
		var spatial = new Mock<IRouteSpatialService>();
		spatial.Setup(x => x.GetEffectiveLocation(actor.Object))
			.Returns(new SpatialLocation(routeCell.Object, RoomLayer.GroundLevel, 7_100.0));

		var result = VehicleModule.FindLocalTransitStop(actor.Object, route.Object, spatial.Object);

		Assert.IsNull(result);
		spatial.Verify(x => x.GetEffectiveLocation(actor.Object), Times.Once);
	}

	[TestMethod]
	public void FindLocalTransitStop_RouteCellStopsWithinDockingTolerance_SelectsNearestStop()
	{
		var routeCell = CreateRouteCell();
		var otherPlatform = new Mock<ICell>();
		var actor = CreateActor(routeCell.Object, 7_150.0, RoomLayer.GroundLevel, 2.0);
		var farther = CreateStop(2L, 0, routeCell.Object, 7_151.5, RoomLayer.GroundLevel,
			CreateBinding(20L, otherPlatform.Object, 2.0).Object);
		var nearer = CreateStop(3L, 1, routeCell.Object, 7_149.0, RoomLayer.GroundLevel,
			CreateBinding(21L, otherPlatform.Object, 2.0).Object);
		var route = CreateRoute(farther.Object, nearer.Object);
		var spatial = new Mock<IRouteSpatialService>();
		spatial.Setup(x => x.GetEffectiveLocation(actor.Object))
			.Returns(new SpatialLocation(routeCell.Object, RoomLayer.GroundLevel, 7_150.0));

		var result = VehicleModule.FindLocalTransitStop(actor.Object, route.Object, spatial.Object);

		Assert.AreSame(nearer.Object, result);
	}

	[TestMethod]
	public void FindLocalTransitStop_RouteCellWithoutPlatformBinding_UsesConfiguredDefaultToleranceAndLayer()
	{
		var routeCell = CreateRouteCell();
		var actor = CreateActor(routeCell.Object, 100.0, RoomLayer.GroundLevel, 5.0);
		var inRange = CreateStop(4L, 0, routeCell.Object, 104.0, RoomLayer.GroundLevel);
		var wrongLayer = CreateStop(5L, 1, routeCell.Object, 100.0, RoomLayer.HighInAir);
		var route = CreateRoute(wrongLayer.Object, inRange.Object);
		var spatial = new Mock<IRouteSpatialService>();
		spatial.Setup(x => x.GetEffectiveLocation(actor.Object))
			.Returns(new SpatialLocation(routeCell.Object, RoomLayer.GroundLevel, 100.0));

		var result = VehicleModule.FindLocalTransitStop(actor.Object, route.Object, spatial.Object);

		Assert.AreSame(inRange.Object, result);
	}

	[TestMethod]
	public void FindLocalTransitStop_AuthoredPlatformCell_PreservesOrdinaryCellBehavior()
	{
		var platformCell = new Mock<ICell>();
		platformCell.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
		var distantStopCell = CreateRouteCell();
		var actor = CreateActor(platformCell.Object, null, RoomLayer.GroundLevel, 2.0);
		var binding = CreateBinding(30L, platformCell.Object, 0.0);
		var stop = CreateStop(6L, 0, distantStopCell.Object, 9_000.0, RoomLayer.GroundLevel, binding.Object);
		var route = CreateRoute(stop.Object);
		var spatial = new Mock<IRouteSpatialService>();
		spatial.Setup(x => x.GetEffectiveLocation(actor.Object))
			.Returns(new SpatialLocation(platformCell.Object, RoomLayer.GroundLevel, null));

		var result = VehicleModule.FindLocalTransitStop(actor.Object, route.Object, spatial.Object);

		Assert.AreSame(stop.Object, result);
	}

	[TestMethod]
	public void DepartureIsVisibleAt_NoActiveJourneyOnlyListsRouteOrigin()
	{
		var cell = new Mock<ICell>();
		var origin = CreateStop(7L, 0, cell.Object, null, RoomLayer.GroundLevel);
		var middle = CreateStop(8L, 1, cell.Object, null, RoomLayer.GroundLevel);
		var terminal = CreateStop(9L, 2, cell.Object, null, RoomLayer.GroundLevel);
		var route = CreateRoute(origin.Object, middle.Object, terminal.Object);
		var service = new Mock<IVehicleService>();
		service.SetupGet(x => x.Route).Returns(route.Object);
		service.SetupGet(x => x.ActiveJourney).Returns((IVehicleJourney?)null);

		Assert.IsTrue(VehicleModule.DepartureIsVisibleAt(service.Object, origin.Object));
		Assert.IsFalse(VehicleModule.DepartureIsVisibleAt(service.Object, middle.Object));
		Assert.IsFalse(VehicleModule.DepartureIsVisibleAt(service.Object, terminal.Object));
	}

	[DataTestMethod]
	[DataRow(VehicleJourneyState.Boarding, true)]
	[DataRow(VehicleJourneyState.Held, true)]
	[DataRow(VehicleJourneyState.Dwelling, true)]
	[DataRow(VehicleJourneyState.Departing, false)]
	[DataRow(VehicleJourneyState.EnRoute, false)]
	[DataRow(VehicleJourneyState.Arrived, false)]
	public void DepartureIsVisibleAt_ActiveJourneyOnlyListsCurrentBoardingStop(
		VehicleJourneyState state,
		bool expectedAtCurrentStop)
	{
		var cell = new Mock<ICell>();
		var origin = CreateStop(10L, 0, cell.Object, null, RoomLayer.GroundLevel);
		var current = CreateStop(11L, 1, cell.Object, null, RoomLayer.GroundLevel);
		var terminal = CreateStop(12L, 2, cell.Object, null, RoomLayer.GroundLevel);
		var route = CreateRoute(origin.Object, current.Object, terminal.Object);
		var journey = new Mock<IVehicleJourney>();
		journey.SetupGet(x => x.State).Returns(state);
		journey.SetupGet(x => x.CurrentStop).Returns(current.Object);
		var service = new Mock<IVehicleService>();
		service.SetupGet(x => x.Route).Returns(route.Object);
		service.SetupGet(x => x.ActiveJourney).Returns(journey.Object);

		Assert.IsFalse(VehicleModule.DepartureIsVisibleAt(service.Object, origin.Object));
		Assert.AreEqual(expectedAtCurrentStop,
			VehicleModule.DepartureIsVisibleAt(service.Object, current.Object));
		Assert.IsFalse(VehicleModule.DepartureIsVisibleAt(service.Object, terminal.Object));
	}

	private static Mock<ICharacter> CreateActor(
		ICell cell,
		double? routePosition,
		RoomLayer layer,
		double defaultTolerance)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("VehicleRouteDefaultDockingToleranceMetres"))
			.Returns(defaultTolerance);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Location).Returns(cell);
		actor.SetupGet(x => x.RoomLayer).Returns(layer);
		actor.SetupGet(x => x.RoutePositionMetres).Returns(routePosition);
		actor.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(cell, layer, routePosition));
		return actor;
	}

	private static Mock<IVehicleRoute> CreateRoute(params IVehicleRouteStop[] stops)
	{
		var route = new Mock<IVehicleRoute>();
		route.SetupGet(x => x.Stops).Returns(stops);
		return route;
	}

	private static Mock<IVehicleRouteStop> CreateStop(
		long id,
		int sequence,
		ICell cell,
		double? routePosition,
		RoomLayer layer,
		params IVehicleRoutePlatformBinding[] bindings)
	{
		var stop = new Mock<IVehicleRouteStop>();
		stop.SetupGet(x => x.Id).Returns(id);
		stop.SetupGet(x => x.Name).Returns($"stop {id}");
		stop.SetupGet(x => x.Sequence).Returns(sequence);
		stop.SetupGet(x => x.Location).Returns(new SpatialLocation(cell, layer, routePosition));
		stop.SetupGet(x => x.PlatformBindings).Returns(bindings);
		return stop;
	}

	private static Mock<IVehicleRoutePlatformBinding> CreateBinding(
		long id,
		ICell platformCell,
		double tolerance)
	{
		var binding = new Mock<IVehicleRoutePlatformBinding>();
		binding.SetupGet(x => x.Id).Returns(id);
		binding.SetupGet(x => x.PlatformCell).Returns(platformCell);
		binding.SetupGet(x => x.DockingToleranceMetres).Returns(tolerance);
		return binding;
	}

	private static Mock<ICell> CreateRouteCell()
	{
		var routeCell = new Mock<ICell>();
		routeCell.SetupGet(x => x.RouteDefinition).Returns(new Mock<IRouteCellDefinition>().Object);
		return routeCell;
	}
}
