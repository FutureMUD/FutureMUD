#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.NPC.AI.Strategies;
using MudSharp.RPG.Law.PatrolStrategies;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpatialPathfinderTests
{
	private static long _nextExitId = 1;

	[TestMethod]
	public void TryFindPath_ArbitraryCoordinates_ReturnsExactWeightedLinearStep()
	{
		var route = TestRoom.CreateRoute(1, "route", 10_000.0, 100.0);
		var service = new SpatialPathfinder();

		var found = service.TryFindPath(
			new SpatialLocation(route.Room, RoomLayer.GroundLevel, 1_000.0),
			new SpatialLocation(route.Room, RoomLayer.GroundLevel, 7_250.0),
			out var path);

		Assert.IsTrue(found);
		Assert.IsNotNull(path);
		Assert.AreEqual(1, path.Steps.Count);
		var step = path.Steps[0] as ILinearRoutePathStep;
		Assert.IsNotNull(step);
		Assert.AreEqual(RouteRoomDirection.Positive, step.Direction);
		Assert.AreEqual(6_250.0, step.DistanceMetres, 0.000001);
		Assert.AreEqual(62.5, step.RoomEquivalentCost, 0.000001);
		Assert.AreEqual(6_250.0, path.RouteDistanceMetres, 0.000001);
		Assert.AreEqual(62.5, path.RoomEquivalentCost, 0.000001);
	}

	[TestMethod]
	public void TryFindPath_MidRouteExitBand_TravelsToNearestInclusiveBoundary()
	{
		var route = TestRoom.CreateRoute(1, "route", 10_000.0, 100.0);
		var town = TestRoom.CreateOrdinary(2, "town");
		var connection = Connect(route, town);
		route.Route!.Anchors.Add(new RouteExitAnchorStub(
			connection.FromFirst,
			route.Room,
			7_100.0,
			7_200.0,
			7_100.0));
		var service = new SpatialPathfinder();

		var found = service.TryFindPath(
			new SpatialLocation(route.Room, RoomLayer.GroundLevel, 7_000.0),
			new SpatialLocation(town.Room, RoomLayer.GroundLevel),
			out var path);

		Assert.IsTrue(found);
		Assert.IsNotNull(path);
		Assert.AreEqual(2, path.Steps.Count);
		var linear = path.Steps[0] as ILinearRoutePathStep;
		Assert.IsNotNull(linear);
		Assert.AreEqual(7_100.0, linear.Destination.RoutePositionMetres!.Value, 0.000001);
		Assert.AreEqual(100.0, linear.DistanceMetres, 0.000001);
		Assert.IsInstanceOfType(path.Steps[1], typeof(IExitTraversalPathStep));
		Assert.AreEqual(2.0, path.RoomEquivalentCost, 0.000001);
	}

	[TestMethod]
	public void TryFindPath_ChainedRouteRoomsAndOrdinaryEndpoints_ReturnsTypedPinnedGeometry()
	{
		var townA = TestRoom.CreateOrdinary(1, "town a");
		var routeA = TestRoom.CreateRoute(2, "route a", 1_000.0, 100.0);
		var routeB = TestRoom.CreateRoute(3, "route b", 2_000.0, 100.0);
		var townB = TestRoom.CreateOrdinary(4, "town b");

		var first = Connect(townA, routeA);
		routeA.Route!.Anchors.Add(new RouteExitAnchorStub(
			first.FromSecond,
			routeA.Room,
			0.0,
			0.0,
			0.0));
		var middle = Connect(routeA, routeB);
		routeA.Route.Anchors.Add(new RouteExitAnchorStub(
			middle.FromFirst,
			routeA.Room,
			1_000.0,
			1_000.0,
			1_000.0));
		routeB.Route!.Anchors.Add(new RouteExitAnchorStub(
			middle.FromSecond,
			routeB.Room,
			0.0,
			0.0,
			0.0));
		var last = Connect(routeB, townB);
		routeB.Route.Anchors.Add(new RouteExitAnchorStub(
			last.FromFirst,
			routeB.Room,
			2_000.0,
			2_000.0,
			2_000.0));

		var service = new SpatialPathfinder();
		var found = service.TryFindPath(
			new SpatialLocation(townA.Room, RoomLayer.GroundLevel),
			new SpatialLocation(townB.Room, RoomLayer.GroundLevel),
			out var path);

		Assert.IsTrue(found);
		Assert.IsNotNull(path);
		Assert.AreEqual(5, path.Steps.Count);
		CollectionAssert.AreEqual(
			new[] { false, true, false, true, false },
			path.Steps.Select(x => x is ILinearRoutePathStep).ToArray());
		Assert.AreEqual(3_000.0, path.RouteDistanceMetres, 0.000001);
		Assert.AreEqual(33.0, path.RoomEquivalentCost, 0.000001);
		Assert.AreEqual(3, path.TraversedExits.Count);
	}

	[TestMethod]
	public void FollowingPath_PatrolHybridPath_ExecutesExitLinearExitInOrder()
	{
		var townA = TestRoom.CreateOrdinary(101, "town a");
		var route = TestRoom.CreateRoute(102, "long road", 1_000.0, 100.0);
		var townB = TestRoom.CreateOrdinary(103, "town b");
		var intoRoute = Connect(townA, route);
		route.Route!.Anchors.Add(new RouteExitAnchorStub(
			intoRoute.FromSecond,
			route.Room,
			0.0,
			0.0,
			0.0));
		var outOfRoute = Connect(route, townB);
		route.Route.Anchors.Add(new RouteExitAnchorStub(
			outOfRoute.FromFirst,
			route.Room,
			1_000.0,
			1_000.0,
			1_000.0));
		var pathfinder = new SpatialPathfinder();
		Assert.IsTrue(pathfinder.TryFindPath(
			new SpatialLocation(townA.Room, RoomLayer.GroundLevel),
			new SpatialLocation(townB.Room, RoomLayer.GroundLevel),
			out var path));
		Assert.IsNotNull(path);

		var state = new MutableCharacterLocation(townA.Room, RoomLayer.GroundLevel, null);
		var actor = CreatePathFollower(state, out var removed);
		var exitManager = new Mock<IExitManager>();
		exitManager.SetupGet(x => x.SpatialPathfinder).Returns(pathfinder);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ExitManager).Returns(exitManager.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		Assert.IsTrue(PatrolStrategyBase.TryCreatePatrolPath(
			actor.Object,
			townB.Room,
			20.0,
			_ => true,
			out var patrolPath));
		Assert.IsTrue(patrolPath.IsSpatialPath);
		CollectionAssert.AreEqual(
			new[] { false, true, false },
			patrolPath.SpatialSteps.Select(x => x is ILinearRoutePathStep).ToArray());
		var effect = new SimulatedSpatialFollowingPath(actor.Object, path, state);

		effect.FollowPathAction();
		effect.FollowPathAction();
		effect.FollowPathAction();
		effect.FollowPathAction();

		CollectionAssert.AreEqual(
			new[] { "exit:town a->long road", "linear", "exit:long road->town b" },
			effect.ExecutedSteps.ToArray());
		Assert.AreSame(townB.Room, state.Room);
		Assert.IsNull(state.RoutePositionMetres);
		Assert.AreEqual(0, effect.SpatialSteps.Count);
		Assert.IsTrue(removed());
	}

	[TestMethod]
	public void TryCreatePatrolPath_PerceivableTargetInSameRouteRoom_PreservesExactCoordinate()
	{
		var route = TestRoom.CreateRoute(104, "long road", 10_000.0, 100.0);
		var pathfinder = new SpatialPathfinder();
		var state = new MutableCharacterLocation(route.Room, RoomLayer.GroundLevel, 100.0);
		var actor = CreatePathFollower(state, out _);
		var exitManager = new Mock<IExitManager>();
		exitManager.SetupGet(x => x.SpatialPathfinder).Returns(pathfinder);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ExitManager).Returns(exitManager.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var target = new Mock<IPerceivable>();
		target.SetupGet(x => x.Location).Returns(route.Room);
		target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		target.SetupGet(x => x.RoutePositionMetres).Returns(7_100.0);
		target.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(route.Room, RoomLayer.GroundLevel, 7_100.0));

		Assert.IsTrue(PatrolStrategyBase.TryCreatePatrolPath(
			actor.Object,
			target.Object,
			100.0,
			_ => true,
			out var patrolPath));

		var linear = patrolPath.SpatialSteps.OfType<ILinearRoutePathStep>().Single();
		Assert.AreEqual(100.0, linear.Origin.RoutePositionMetres);
		Assert.AreEqual(7_100.0, linear.Destination.RoutePositionMetres);
		Assert.AreEqual(7_000.0, linear.DistanceMetres);
	}

	[TestMethod]
	public void TryCreatePatrolPath_ConfiguredRouteRoomTarget_UsesSafeDefaultCoordinate()
	{
		var route = TestRoom.CreateRoute(105, "long road", 10_000.0, 100.0);
		var pathfinder = new SpatialPathfinder();
		var state = new MutableCharacterLocation(route.Room, RoomLayer.GroundLevel, 500.0);
		var actor = CreatePathFollower(state, out _);
		var exitManager = new Mock<IExitManager>();
		exitManager.SetupGet(x => x.SpatialPathfinder).Returns(pathfinder);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ExitManager).Returns(exitManager.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		Assert.IsTrue(PatrolStrategyBase.TryCreatePatrolPath(
			actor.Object,
			route.Room,
			10.0,
			_ => true,
			out var patrolPath));

		var linear = patrolPath.SpatialSteps.OfType<ILinearRoutePathStep>().Single();
		Assert.AreEqual(500.0, linear.Origin.RoutePositionMetres);
		Assert.AreEqual(route.Route!.DefaultPositionMetres, linear.Destination.RoutePositionMetres);
		Assert.IsFalse(PatrolStrategyBase.HasReachedPatrolDestination(actor.Object, route.Room));

		state.Set(new SpatialLocation(
			route.Room,
			RoomLayer.GroundLevel,
			route.Route.DefaultPositionMetres));
		Assert.IsTrue(PatrolStrategyBase.HasReachedPatrolDestination(actor.Object, route.Room));
	}

	[TestMethod]
	public void FollowingPath_PatrolHybridPath_TopologyVersionChangeStopsBeforeMovement()
	{
		var townA = TestRoom.CreateOrdinary(111, "town a");
		var route = TestRoom.CreateRoute(112, "long road", 1_000.0, 100.0);
		var townB = TestRoom.CreateOrdinary(113, "town b");
		var intoRoute = Connect(townA, route);
		route.Route!.Anchors.Add(new RouteExitAnchorStub(
			intoRoute.FromSecond,
			route.Room,
			0.0,
			0.0,
			0.0));
		var outOfRoute = Connect(route, townB);
		route.Route.Anchors.Add(new RouteExitAnchorStub(
			outOfRoute.FromFirst,
			route.Room,
			1_000.0,
			1_000.0,
			1_000.0));
		var pathfinder = new SpatialPathfinder();
		Assert.IsTrue(pathfinder.TryFindPath(
			new SpatialLocation(townA.Room, RoomLayer.GroundLevel),
			new SpatialLocation(townB.Room, RoomLayer.GroundLevel),
			out var path));
		Assert.IsNotNull(path);

		var state = new MutableCharacterLocation(townA.Room, RoomLayer.GroundLevel, null);
		var actor = CreatePathFollower(state, out var removed);
		var effect = new SimulatedSpatialFollowingPath(actor.Object, path, state);
		route.Route.TopologyVersion++;

		effect.FollowPathAction();

		Assert.AreEqual(0, effect.ExecutedSteps.Count);
		Assert.AreSame(townA.Room, state.Room);
		Assert.IsTrue(removed());
	}

	[TestMethod]
	public void TryFindPath_NonGroundHybridPath_FiltersSourceLayerAndAppliesExitTransitions()
	{
		var origin = TestRoom.CreateOrdinary(1, "origin");
		var route = TestRoom.CreateRoute(2, "elevated route", 1_000.0, 100.0);
		var destination = TestRoom.CreateOrdinary(3, "destination");

		var intoRoute = Connect(
			origin,
			route,
			firstLayer: RoomLayer.InTrees,
			secondLayer: RoomLayer.HighInTrees);
		route.Route!.Anchors.Add(new RouteExitAnchorStub(
			intoRoute.FromSecond,
			route.Room,
			0.0,
			0.0,
			0.0));
		var outOfRoute = Connect(
			route,
			destination,
			firstLayer: RoomLayer.HighInTrees,
			secondLayer: RoomLayer.InTrees);
		route.Route.Anchors.Add(new RouteExitAnchorStub(
			outOfRoute.FromFirst,
			route.Room,
			1_000.0,
			1_000.0,
			1_000.0));
		var wrongLayerShortcut = Connect(origin, destination, 0.1);

		var service = new SpatialPathfinder();
		var found = service.TryFindPath(
			new SpatialLocation(origin.Room, RoomLayer.InTrees),
			new SpatialLocation(destination.Room, RoomLayer.InTrees),
			out var path);

		Assert.IsTrue(found);
		Assert.IsNotNull(path);
		Assert.AreEqual(3, path.Steps.Count);
		Assert.AreSame(intoRoute.FromFirst, ((IExitTraversalPathStep)path.Steps[0]).Exit);
		Assert.AreEqual(RoomLayer.HighInTrees, path.Steps[0].Destination.Layer);
		Assert.IsInstanceOfType(path.Steps[1], typeof(ILinearRoutePathStep));
		Assert.AreEqual(RoomLayer.HighInTrees, path.Steps[1].Origin.Layer);
		Assert.AreEqual(RoomLayer.HighInTrees, path.Steps[1].Destination.Layer);
		Assert.AreSame(outOfRoute.FromFirst, ((IExitTraversalPathStep)path.Steps[2]).Exit);
		Assert.AreEqual(RoomLayer.InTrees, path.Steps[2].Destination.Layer);
		Assert.IsFalse(path.TraversedExits.Contains(wrongLayerShortcut.FromFirst));
	}

	[TestMethod]
	public void TryBuildCompiledSteps_NonGroundHybridPath_PreservesLayersAndPinsTopologyVersion()
	{
		var origin = TestRoom.CreateOrdinary(1, "origin");
		var route = TestRoom.CreateRoute(2, "elevated route", 1_000.0, 100.0);
		route.Route!.TopologyVersion = 37;
		var destination = TestRoom.CreateOrdinary(3, "destination");
		var intoRoute = Connect(
			origin,
			route,
			firstLayer: RoomLayer.InTrees,
			secondLayer: RoomLayer.HighInTrees);
		route.Route.Anchors.Add(new RouteExitAnchorStub(
			intoRoute.FromSecond,
			route.Room,
			0.0,
			0.0,
			0.0));
		var outOfRoute = Connect(
			route,
			destination,
			firstLayer: RoomLayer.HighInTrees,
			secondLayer: RoomLayer.InTrees);
		route.Route.Anchors.Add(new RouteExitAnchorStub(
			outOfRoute.FromFirst,
			route.Room,
			1_000.0,
			1_000.0,
			1_000.0));
		var service = new SpatialPathfinder();
		Assert.IsTrue(service.TryFindPath(
			new SpatialLocation(origin.Room, RoomLayer.InTrees),
			new SpatialLocation(destination.Room, RoomLayer.InTrees),
			out var path));

		Assert.IsTrue(VehicleRoute.TryBuildCompiledSteps(path!, out var steps, out var reason), reason);
		Assert.AreEqual(3, steps.Count);
		Assert.AreEqual((int)RoomLayer.InTrees, steps[0].OriginRoomLayer);
		Assert.AreEqual((int)RoomLayer.HighInTrees, steps[0].DestinationRoomLayer);
		Assert.IsNull(steps[0].PinnedTopologyVersion);
		Assert.AreEqual(37L, steps[0].DestinationTopologyVersion);
		Assert.AreEqual((int)VehicleRouteStepType.LinearRoute, steps[1].StepType);
		Assert.AreEqual((int)RoomLayer.HighInTrees, steps[1].OriginRoomLayer);
		Assert.AreEqual((int)RoomLayer.HighInTrees, steps[1].DestinationRoomLayer);
		Assert.AreEqual(37L, steps[1].PinnedTopologyVersion);
		Assert.AreEqual(37L, steps[1].DestinationTopologyVersion);
		Assert.AreEqual((int)RoomLayer.HighInTrees, steps[2].OriginRoomLayer);
		Assert.AreEqual((int)RoomLayer.InTrees, steps[2].DestinationRoomLayer);
		Assert.AreEqual(37L, steps[2].PinnedTopologyVersion);
		Assert.IsNull(steps[2].DestinationTopologyVersion);
	}

	[TestMethod]
	public void TryBuildCompiledSteps_RouteToRouteExit_PinsBothAnchorTopologies()
	{
		var first = TestRoom.CreateRoute(1, "first route", 10_000.0, 100.0);
		var second = TestRoom.CreateRoute(2, "second route", 20_000.0, 100.0);
		first.Route!.TopologyVersion = 41;
		second.Route!.TopologyVersion = 73;
		var connection = Connect(first, second);
		first.Route.Anchors.Add(new RouteExitAnchorStub(
			connection.FromFirst,
			first.Room,
			7_100.0,
			7_200.0,
			7_150.0));
		second.Route.Anchors.Add(new RouteExitAnchorStub(
			connection.FromSecond,
			second.Room,
			12_000.0,
			12_100.0,
			12_050.0));
		var service = new SpatialPathfinder();

		Assert.IsTrue(service.TryFindPath(
			new SpatialLocation(first.Room, RoomLayer.GroundLevel, 7_150.0),
			new SpatialLocation(second.Room, RoomLayer.GroundLevel, 12_050.0),
			out var path));
		Assert.IsTrue(VehicleRoute.TryBuildCompiledSteps(path!, out var steps, out var reason), reason);
		Assert.AreEqual(1, steps.Count);
		Assert.AreEqual((int)VehicleRouteStepType.RoomExit, steps[0].StepType);
		Assert.AreEqual(41L, steps[0].PinnedTopologyVersion);
		Assert.AreEqual(73L, steps[0].DestinationTopologyVersion);
	}

	[TestMethod]
	public void TryFindPath_IgnoreLayers_UsesAnAuthoredExitLayerForTransitionResolution()
	{
		var origin = TestRoom.CreateOrdinary(1, "origin");
		var destination = TestRoom.CreateOrdinary(2, "destination");
		Connect(origin, destination);
		var service = new SpatialPathfinder();
		var source = new SpatialLocation(origin.Room, RoomLayer.InTrees);
		var target = new SpatialLocation(destination.Room, RoomLayer.GroundLevel);

		Assert.IsFalse(service.TryFindPath(
			source,
			target,
			null,
			false,
			double.PositiveInfinity,
			out _));
		Assert.IsTrue(service.TryFindPath(
			source,
			target,
			null,
			true,
			double.PositiveInfinity,
			out var path));
		Assert.IsNotNull(path);
		Assert.AreEqual(1, path.Steps.Count);
		Assert.AreEqual(RoomLayer.GroundLevel, path.Steps[0].Destination.Layer);
	}

	[TestMethod]
	public void TryFindPath_CompetingOrdinaryAndRoutePaths_SelectsLowestWeightedCost()
	{
		var origin = TestRoom.CreateOrdinary(1, "origin");
		var ordinaryA = TestRoom.CreateOrdinary(2, "ordinary a");
		var ordinaryB = TestRoom.CreateOrdinary(3, "ordinary b");
		var destination = TestRoom.CreateOrdinary(4, "destination");
		var route = TestRoom.CreateRoute(5, "shortcut", 50.0, 100.0);

		Connect(origin, ordinaryA);
		Connect(ordinaryA, ordinaryB);
		Connect(ordinaryB, destination);

		var intoRoute = Connect(origin, route);
		route.Route!.Anchors.Add(new RouteExitAnchorStub(
			intoRoute.FromSecond,
			route.Room,
			0.0,
			0.0,
			0.0));
		var outOfRoute = Connect(route, destination);
		route.Route.Anchors.Add(new RouteExitAnchorStub(
			outOfRoute.FromFirst,
			route.Room,
			50.0,
			50.0,
			50.0));

		var service = new SpatialPathfinder();
		var found = service.TryFindPath(
			new SpatialLocation(origin.Room, RoomLayer.GroundLevel),
			new SpatialLocation(destination.Room, RoomLayer.GroundLevel),
			out var path);

		Assert.IsTrue(found);
		Assert.IsNotNull(path);
		Assert.AreEqual(2.5, path.RoomEquivalentCost, 0.000001);
		Assert.AreEqual(50.0, path.RouteDistanceMetres, 0.000001);
		Assert.IsTrue(path.Steps.Any(x => x is ILinearRoutePathStep));
	}

	[TestMethod]
	public void TryFindPath_TopologyVersionControlsCachedRouteSnapshot()
	{
		var route = TestRoom.CreateRoute(1, "route", 100.0, 100.0);
		var service = new SpatialPathfinder();
		var origin = new SpatialLocation(route.Room, RoomLayer.GroundLevel, 0.0);
		var destination = new SpatialLocation(route.Room, RoomLayer.GroundLevel, 100.0);

		Assert.IsTrue(service.TryFindPath(origin, destination, out var first));
		Assert.AreEqual(1.0, first!.RoomEquivalentCost, 0.000001);

		route.Route!.MetresPerRoomEquivalent = 50.0;
		Assert.IsTrue(service.TryFindPath(origin, destination, out var cached));
		Assert.AreEqual(1.0, cached!.RoomEquivalentCost, 0.000001,
			"A topology snapshot should remain stable until its version changes.");

		route.Route.TopologyVersion++;
		Assert.IsTrue(service.TryFindPath(origin, destination, out var rebuilt));
		Assert.AreEqual(2.0, rebuilt!.RoomEquivalentCost, 0.000001);
	}

	[TestMethod]
	public void TryFindExitOnlyPath_LongitudinalMovementRequired_FailsClosed()
	{
		var route = TestRoom.CreateRoute(1, "route", 10_000.0, 100.0);
		var town = TestRoom.CreateOrdinary(2, "town");
		var connection = Connect(route, town);
		route.Route!.Anchors.Add(new RouteExitAnchorStub(
			connection.FromFirst,
			route.Room,
			7_100.0,
			7_200.0,
			7_100.0));
		var service = new SpatialPathfinder();

		var legacyFound = service.TryFindExitOnlyPath(
			new SpatialLocation(route.Room, RoomLayer.GroundLevel, 7_000.0),
			new SpatialLocation(town.Room, RoomLayer.GroundLevel),
			null,
			false,
			double.PositiveInfinity,
			out var exits);

		Assert.IsFalse(legacyFound);
		Assert.AreEqual(0, exits.Count);

		var alreadyAtPortal = service.TryFindExitOnlyPath(
			new SpatialLocation(route.Room, RoomLayer.GroundLevel, 7_150.0),
			new SpatialLocation(town.Room, RoomLayer.GroundLevel),
			null,
			false,
			double.PositiveInfinity,
			out exits);

		Assert.IsTrue(alreadyAtPortal);
		Assert.AreEqual(1, exits.Count);
	}

	[TestMethod]
	public void TryFindPath_RouteDestinationWithoutAnchor_FailsClosed()
	{
		var town = TestRoom.CreateOrdinary(1, "town");
		var route = TestRoom.CreateRoute(2, "route", 1_000.0, 100.0);
		Connect(town, route);
		var service = new SpatialPathfinder();

		var found = service.TryFindPath(
			new SpatialLocation(town.Room, RoomLayer.GroundLevel),
			new SpatialLocation(route.Room, RoomLayer.GroundLevel, 500.0),
			out var path);

		Assert.IsFalse(found);
		Assert.IsNull(path);
	}

	[TestMethod]
	public void LegacyPathBetween_OrdinaryEndpointsAcrossRouteRoom_RefusesShortcut()
	{
		var townA = TestRoom.CreateOrdinary(1, "town a");
		var route = TestRoom.CreateRoute(2, "route", 10_000.0, 100.0);
		var townB = TestRoom.CreateOrdinary(3, "town b");
		Connect(townA, route);
		Connect(route, townB);

		var source = new Mock<IPerceivable>();
		source.SetupGet(x => x.Location).Returns(townA.Room);
		var target = new Mock<IPerceivable>();
		target.SetupGet(x => x.Location).Returns(townB.Room);

		var exits = source.Object.PathBetween(target.Object, 10, _ => true).ToList();

		Assert.AreEqual(0, exits.Count,
			"Legacy exit-only callers must not cross a RouteCell as though it were one room.");
	}

	[TestMethod]
	public void DistanceBetween_SameRouteRoom_UsesRoomEquivalentCostAndRoundsUp()
	{
		var route = TestRoom.CreateRoute(1, "route", 10_000.0, 100.0);
		var pathfinder = new SpatialPathfinder();
		var exitManager = new Mock<IExitManager>();
		exitManager.SetupGet(x => x.SpatialPathfinder).Returns(pathfinder);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ExitManager).Returns(exitManager.Object);

		var source = new Mock<IPerceivable>();
		source.SetupGet(x => x.Location).Returns(route.Room);
		source.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		source.SetupGet(x => x.RoutePositionMetres).Returns(100.0);
		source.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(route.Room, RoomLayer.GroundLevel, 100.0));
		source.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var target = new Mock<IPerceivable>();
		target.SetupGet(x => x.Location).Returns(route.Room);
		target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		target.SetupGet(x => x.RoutePositionMetres).Returns(351.0);
		target.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(route.Room, RoomLayer.GroundLevel, 351.0));
		target.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		Assert.AreEqual(2.51, source.Object.RoomEquivalentDistanceBetween(target.Object), 0.000001);
		Assert.AreEqual(3, source.Object.DistanceBetween(target.Object, 10));
		Assert.AreEqual(-1, source.Object.DistanceBetween(target.Object, 2));
	}

	private static ExitPair Connect(
		TestRoom first,
		TestRoom second,
		double timeMultiplier = 1.0,
		RoomLayer firstLayer = RoomLayer.GroundLevel,
		RoomLayer secondLayer = RoomLayer.GroundLevel)
	{
		var underlyingExit = new Mock<IExit>();
		underlyingExit.SetupGet(x => x.Id).Returns(_nextExitId++);
		underlyingExit.SetupGet(x => x.TimeMultiplier).Returns(timeMultiplier);
		underlyingExit.SetupGet(x => x.Rooms).Returns([first.Room, second.Room]);

		var fromFirst = new Mock<IRoomExit>();
		var fromSecond = new Mock<IRoomExit>();
		fromFirst.SetupGet(x => x.Origin).Returns(first.Room);
		fromFirst.SetupGet(x => x.Destination).Returns(second.Room);
		fromFirst.SetupGet(x => x.Exit).Returns(underlyingExit.Object);
		fromFirst.SetupGet(x => x.Opposite).Returns(fromSecond.Object);
		fromFirst.Setup(x => x.WhichLayersExitAppears()).Returns([firstLayer]);
		fromFirst.Setup(x => x.MovementTransition(It.IsAny<IPerceiver>()))
			.Returns((IPerceiver perceiver) => perceiver.RoomLayer == firstLayer
				? (TransitionType(firstLayer, secondLayer), secondLayer)
				: (RoomMovementTransition.NoViableTransition, RoomLayer.GroundLevel));
		fromSecond.SetupGet(x => x.Origin).Returns(second.Room);
		fromSecond.SetupGet(x => x.Destination).Returns(first.Room);
		fromSecond.SetupGet(x => x.Exit).Returns(underlyingExit.Object);
		fromSecond.SetupGet(x => x.Opposite).Returns(fromFirst.Object);
		fromSecond.Setup(x => x.WhichLayersExitAppears()).Returns([secondLayer]);
		fromSecond.Setup(x => x.MovementTransition(It.IsAny<IPerceiver>()))
			.Returns((IPerceiver perceiver) => perceiver.RoomLayer == secondLayer
				? (TransitionType(secondLayer, firstLayer), firstLayer)
				: (RoomMovementTransition.NoViableTransition, RoomLayer.GroundLevel));
		first.Exits.Add(fromFirst.Object);
		second.Exits.Add(fromSecond.Object);
		return new ExitPair(fromFirst.Object, fromSecond.Object);
	}

	private static RoomMovementTransition TransitionType(RoomLayer origin, RoomLayer destination)
	{
		if (origin.IsUnderwater() || destination.IsUnderwater())
		{
			return RoomMovementTransition.SwimOnly;
		}

		return origin == RoomLayer.GroundLevel && destination == RoomLayer.GroundLevel
			? RoomMovementTransition.GroundToGround
			: RoomMovementTransition.TreesToTrees;
	}

	private sealed record ExitPair(IRoomExit FromFirst, IRoomExit FromSecond);

	private static Mock<ICharacter> CreatePathFollower(
		MutableCharacterLocation state,
		out Func<bool> wasRemoved)
	{
		var removed = false;
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.SetupGet(x => x.Location).Returns(() => state.Room);
		actor.SetupGet(x => x.RoomLayer).Returns(() => state.Layer);
		actor.SetupGet(x => x.RoutePositionMetres).Returns(() => state.RoutePositionMetres);
		actor.SetupGet(x => x.SpatialLocation).Returns(() => new SpatialLocation(
			state.Room,
			state.Layer,
			state.RoutePositionMetres));
		actor.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
		actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>((_, _) => removed = true);
		wasRemoved = () => removed;
		return actor;
	}

	private sealed class SimulatedSpatialFollowingPath : FollowingPath
	{
		private readonly Dictionary<IRoomExit, SpatialLocation> _exitDestinations;
		private readonly MutableCharacterLocation _state;

		public SimulatedSpatialFollowingPath(
			ICharacter owner,
			ISpatialPath path,
			MutableCharacterLocation state) : base(owner, path, _ => true)
		{
			_state = state;
			_exitDestinations = path.Steps
				.OfType<IExitTraversalPathStep>()
				.ToDictionary(x => x.Exit, x => x.Destination);
		}

		public List<string> ExecutedSteps { get; } = [];

		protected override MovementStrategyResult TryMoveThroughExit(ICharacter ch, IRoomExit exit)
		{
			var destination = _exitDestinations[exit];
			ExecutedSteps.Add($"exit:{exit.Origin.Name}->{exit.Destination.Name}");
			_state.Set(destination);
			return MovementStrategyResult.Moved;
		}

		protected override bool TryBeginLinearRouteMovement(ICharacter ch, ILinearRoutePathStep step)
		{
			ExecutedSteps.Add("linear");
			_state.Set(step.Destination);
			return true;
		}
	}

	private sealed class MutableCharacterLocation(
		IRoom room,
		RoomLayer layer,
		double? routePositionMetres)
	{
		public IRoom Room { get; private set; } = room;
		public RoomLayer Layer { get; private set; } = layer;
		public double? RoutePositionMetres { get; private set; } = routePositionMetres;

		public void Set(SpatialLocation location)
		{
			Room = location.Room;
			Layer = location.Layer;
			RoutePositionMetres = location.RoutePositionMetres;
		}
	}

	private sealed class TestRoom
	{
		private TestRoom(long id, string name)
		{
			Mock = new Mock<IRoom>();
			Mock.SetupGet(x => x.Id).Returns(id);
			Mock.SetupGet(x => x.Name).Returns(name);
			Mock.SetupGet(x => x.RouteDefinition).Returns(() => Route);
			Mock.SetupGet(x => x.SpatialType).Returns(() => Route is null
				? RoomSpatialType.Ordinary
				: RoomSpatialType.LinearRoute);
			var terrain = new Mock<ITerrain>();
			terrain.SetupGet(x => x.GravityModel).Returns(GravityModel.Normal);
			Mock.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrain.Object);
			Mock.SetupGet(x => x.Effects).Returns([]);
			Mock.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>()))
				.Returns(() => Exits);
		}

		public Mock<IRoom> Mock { get; }
		public IRoom Room => Mock.Object;
		public List<IRoomExit> Exits { get; } = new();
		public RouteRoomDefinitionStub? Route { get; private set; }

		public static TestRoom CreateOrdinary(long id, string name)
		{
			return new TestRoom(id, name);
		}

		public static TestRoom CreateRoute(
			long id,
			string name,
			double lengthMetres,
			double metresPerRoomEquivalent)
		{
			var room = new TestRoom(id, name);
			room.Route = new RouteRoomDefinitionStub(
				room.Room,
				lengthMetres,
				metresPerRoomEquivalent);
			return room;
		}
	}

	private sealed class RouteRoomDefinitionStub : IRouteRoomDefinition
	{
		public RouteRoomDefinitionStub(
			IRoom room,
			double lengthMetres,
			double metresPerRoomEquivalent)
		{
			Room = room;
			LengthMetres = lengthMetres;
			MetresPerRoomEquivalent = metresPerRoomEquivalent;
		}

		public IRoom Room { get; }
		public double LengthMetres { get; set; }
		public double DefaultPositionMetres => 0.0;
		public string PositiveDirectionName => "forward";
		public string NegativeDirectionName => "backward";
		public double MetresPerRoomEquivalent { get; set; }
		public long TopologyVersion { get; set; } = 1;
		public IReadOnlyList<IRouteRoomLandmark> Landmarks =>
			Array.Empty<IRouteRoomLandmark>();
		public List<IRouteExitAnchor> Anchors { get; } = new();
		public IReadOnlyCollection<IRouteExitAnchor> ExitAnchors => Anchors;
	}

	private sealed record RouteExitAnchorStub(
		IRoomExit Exit,
		IRoom Room,
		double MinimumPositionMetres,
		double MaximumPositionMetres,
		double ArrivalPositionMetres) : IRouteExitAnchor;
}
