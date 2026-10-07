#nullable enable annotations

using ExpressionEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.ScatterStrategies;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ScatterStrategyUtilitiesTests
{
    [TestMethod]
    public void GetRoomInfos_ReturnsDistancesAndDirections()
    {
        ScatterTestHelpers.RoomNetworkBuilder builder = new();
        ScatterTestHelpers.RoomNetworkBuilder.RoomData origin = builder.AddRoom("origin", (0, 0, 0));
        ScatterTestHelpers.RoomNetworkBuilder.RoomData north = builder.AddRoom("north", (0, 1, 0));
        ScatterTestHelpers.RoomNetworkBuilder.RoomData east = builder.AddRoom("east", (1, 0, 0));
        builder.ConnectTwoWay("origin", "north", CardinalDirection.North);
        builder.ConnectTwoWay("origin", "east", CardinalDirection.East);
        builder.Finalise();

        Mock<IPerceiver> target = ScatterTestHelpers.CreatePerceiver(origin.Room.Object);
        IReadOnlyList<RoomScatterInfo> infos = ScatterStrategyUtilities.GetRoomInfos(target.Object, 2, true);

        Assert.AreEqual(3, infos.Count);
        RoomScatterInfo originInfo = infos.Single(x => x.Room == origin.Room.Object);
        Assert.AreEqual(0, originInfo.Distance);
        Assert.AreEqual(CardinalDirection.Unknown, originInfo.DirectionFromOrigin);

        RoomScatterInfo northInfo = infos.Single(x => x.Room == north.Room.Object);
        Assert.AreEqual(1, northInfo.Distance);
        Assert.AreEqual(CardinalDirection.North, northInfo.DirectionFromOrigin);

        RoomScatterInfo eastInfo = infos.Single(x => x.Room == east.Room.Object);
        Assert.AreEqual(1, eastInfo.Distance);
        Assert.AreEqual(CardinalDirection.East, eastInfo.DirectionFromOrigin);
    }

    [TestMethod]
    public void GetRoomInfos_RespectsClosedDoors()
    {
        ScatterTestHelpers.RoomNetworkBuilder builder = new();
        ScatterTestHelpers.RoomNetworkBuilder.RoomData origin = builder.AddRoom("origin", (0, 0, 0));
        ScatterTestHelpers.RoomNetworkBuilder.RoomData north = builder.AddRoom("north", (0, 1, 0));
        builder.ConnectTwoWay("origin", "north", CardinalDirection.North, true, false);
        builder.Finalise();

        Mock<IPerceiver> target = ScatterTestHelpers.CreatePerceiver(origin.Room.Object);
        IReadOnlyList<RoomScatterInfo> blocked = ScatterStrategyUtilities.GetRoomInfos(target.Object, 1, true);
        Assert.IsFalse(blocked.Any(x => x.Room == north.Room.Object));

        IReadOnlyList<RoomScatterInfo> unblocked = ScatterStrategyUtilities.GetRoomInfos(target.Object, 1, false);
        Assert.IsTrue(unblocked.Any(x => x.Room == north.Room.Object));
    }

    [TestMethod]
    public void DescribeFromDirection_ReturnsCorrectSuffixes()
    {
        Assert.AreEqual(string.Empty, ScatterStrategyUtilities.DescribeFromDirection(CardinalDirection.Unknown));
        Assert.AreEqual(" from above", ScatterStrategyUtilities.DescribeFromDirection(CardinalDirection.Up));
        Assert.AreEqual(" from the South", ScatterStrategyUtilities.DescribeFromDirection(CardinalDirection.South));
    }
}

[TestClass]
public class BallisticScatterStrategyTests
{
    [TestMethod]
    public void GetScatterTarget_PrefersForwardDirectionAndReturnsRoomWhenNoTarget()
    {
        ScatterTestHelpers.SetWeightExpression(typeof(BallisticScatterStrategy), "_weightExpression", "size + proximity + 1");
        ScatterTestHelpers.RoomNetworkBuilder builder = new();
        ScatterTestHelpers.RoomNetworkBuilder.RoomData origin = builder.AddRoom("origin", (0, 0, 0));
        ScatterTestHelpers.RoomNetworkBuilder.RoomData north = builder.AddRoom("north", (0, 1, 0));
        ScatterTestHelpers.RoomNetworkBuilder.RoomData south = builder.AddRoom("south", (0, -1, 0));
        builder.ConnectTwoWay("origin", "north", CardinalDirection.North);
        builder.ConnectTwoWay("origin", "south", CardinalDirection.South);
        builder.Finalise();

        Mock<ICharacter> shooter = ScatterTestHelpers.CreateCharacter(origin.Room.Object);
        Mock<IPerceiver> target = ScatterTestHelpers.CreatePerceiver(origin.Room.Object);
        List<IRoomExit> path = ScatterTestHelpers.CreatePath(CardinalDirection.North);
        HashSet<CardinalDirection> directionSet = ScatterTestHelpers.GetDirectionSet(path);
        IReadOnlyList<RoomScatterInfo> cellInfos = ScatterStrategyUtilities.GetRoomInfos(target.Object, 3, true);
        List<(RoomScatterInfo Info, double Weight)> weights = ScatterTestHelpers.ComputeDirectionalWeights(typeof(BallisticScatterStrategy), cellInfos, directionSet,
        path.Last().OutboundDirection);
        int expectedIndex = ScatterTestHelpers.GetExpectedIndex(weights.Select(x => x.Weight).ToList(), 5);
        ScatterTestHelpers.SeedRandom(5);

        RangedScatterResult result = BallisticScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, path);

        Assert.IsNotNull(result);
        Assert.AreEqual(weights[expectedIndex].Info.Room, result.Room);
        Assert.AreEqual(weights[expectedIndex].Info.Distance, result.DistanceFromTarget);
        Assert.AreEqual(weights[expectedIndex].Info.DirectionFromOrigin, result.DirectionFromTarget);
        Assert.IsNull(result.Target);
        Assert.AreEqual(target.Object.RoomLayer, result.RoomLayer);
    }

    [TestMethod]
    public void GetScatterTarget_ReturnsNullWhenTargetLocationMissing()
    {
        Mock<ICharacter> shooter = new();
        Mock<IPerceiver> target = new();
        target.SetupGet(x => x.Location).Returns((IRoom)null);
        Assert.IsNull(BallisticScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, null));
    }
}

[TestClass]
public class ArcingScatterStrategyTests
{
    [TestMethod]
    public void GetScatterTarget_PrefersCloserRoomsWhenNoTarget()
    {
        ScatterTestHelpers.SetWeightExpression(typeof(ArcingScatterStrategy), "_weightExpression", "size + proximity + 1");
        ScatterTestHelpers.RoomNetworkBuilder builder = new();
        ScatterTestHelpers.RoomNetworkBuilder.RoomData origin = builder.AddRoom("origin", (0, 0, 0));
        ScatterTestHelpers.RoomNetworkBuilder.RoomData north = builder.AddRoom("north", (0, 1, 0));
        builder.ConnectTwoWay("origin", "north", CardinalDirection.North);
        builder.Finalise();

        Mock<ICharacter> shooter = ScatterTestHelpers.CreateCharacter(origin.Room.Object);
        Mock<IPerceiver> target = ScatterTestHelpers.CreatePerceiver(origin.Room.Object);
        IReadOnlyList<RoomScatterInfo> cellInfos = ScatterStrategyUtilities.GetRoomInfos(target.Object, 1, true);
        List<(RoomScatterInfo Info, double Weight)> weights = ScatterTestHelpers.ComputeArcingWeights(cellInfos);
        int expectedIndex = ScatterTestHelpers.GetExpectedIndex(weights.Select(x => x.Weight).ToList(), 11);
        ScatterTestHelpers.SeedRandom(11);

        RangedScatterResult result = ArcingScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, Array.Empty<IRoomExit>());

        Assert.IsNotNull(result);
        Assert.AreEqual(weights[expectedIndex].Info.Room, result.Room);
        Assert.AreEqual(weights[expectedIndex].Info.Distance, result.DistanceFromTarget);
        Assert.AreEqual(weights[expectedIndex].Info.DirectionFromOrigin, result.DirectionFromTarget);
        Assert.IsNull(result.Target);
    }

    [TestMethod]
    public void Weight_IncreasesForHigherLayerTargets()
    {
        ScatterTestHelpers.SetWeightExpression(typeof(ArcingScatterStrategy), "_weightExpression", "size + proximity + 1");
        Mock<IPerceiver> target = ScatterTestHelpers.CreatePerceiver(null);
        target.Object.RoomLayer = RoomLayer.GroundLevel;
        Mock<IPerceiver> lower = ScatterTestHelpers.CreatePerceiver(null);
        lower.Object.RoomLayer = RoomLayer.GroundLevel;
        lower.SetupGet(x => x.Size).Returns(SizeCategory.Small);
        lower.Setup(x => x.GetProximity(It.IsAny<IPerceivable>())).Returns(Proximity.Distant);
        Mock<IPerceiver> higher = ScatterTestHelpers.CreatePerceiver(null);
        higher.Object.RoomLayer = RoomLayer.InTrees;
        higher.SetupGet(x => x.Size).Returns(SizeCategory.Small);
        higher.Setup(x => x.GetProximity(It.IsAny<IPerceivable>())).Returns(Proximity.Distant);

        MethodInfo weightMethod = typeof(ArcingScatterStrategy).GetMethod("Weight", BindingFlags.Static | BindingFlags.NonPublic);
        double lowerWeight = (double)weightMethod!.Invoke(null, new object[] { lower.Object, target.Object });
        double higherWeight = (double)weightMethod.Invoke(null, new object[] { higher.Object, target.Object });
        Assert.IsTrue(higherWeight > lowerWeight);
    }

    [TestMethod]
    public void GetScatterTarget_ReturnsNullWhenTargetLocationMissing()
    {
        Mock<ICharacter> shooter = new();
        Mock<IPerceiver> target = new();
        target.SetupGet(x => x.Location).Returns((IRoom)null);
        Assert.IsNull(ArcingScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, null));
    }
}

[TestClass]
public class LightScatterStrategyTests
{
    [TestMethod]
    public void GetScatterTarget_FavorsContinuationAndReturnsRoom()
    {
        ScatterTestHelpers.SetWeightExpression(typeof(LightScatterStrategy), "_weightExpression", "size + proximity + 1");
        ScatterTestHelpers.RoomNetworkBuilder builder = new();
        ScatterTestHelpers.RoomNetworkBuilder.RoomData origin = builder.AddRoom("origin", (0, 0, 0));
        ScatterTestHelpers.RoomNetworkBuilder.RoomData north = builder.AddRoom("north", (0, 1, 0));
        ScatterTestHelpers.RoomNetworkBuilder.RoomData east = builder.AddRoom("east", (1, 0, 0));
        builder.ConnectTwoWay("origin", "north", CardinalDirection.North);
        builder.ConnectTwoWay("origin", "east", CardinalDirection.East);
        builder.Finalise();

        Mock<ICharacter> shooter = ScatterTestHelpers.CreateCharacter(origin.Room.Object);
        Mock<IPerceiver> target = ScatterTestHelpers.CreatePerceiver(origin.Room.Object);
        List<IRoomExit> path = ScatterTestHelpers.CreatePath(CardinalDirection.North);
        HashSet<CardinalDirection> directionSet = ScatterTestHelpers.GetDirectionSet(path);
        IReadOnlyList<RoomScatterInfo> cellInfos = ScatterStrategyUtilities.GetRoomInfos(target.Object, 5, true);
        List<(RoomScatterInfo Info, double Weight)> weights = ScatterTestHelpers.ComputeDirectionalWeights(typeof(LightScatterStrategy), cellInfos, directionSet,
        path.Last().OutboundDirection);
        int expectedIndex = ScatterTestHelpers.GetExpectedIndex(weights.Select(x => x.Weight).ToList(), 17);
        ScatterTestHelpers.SeedRandom(17);

        RangedScatterResult result = LightScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, path);

        Assert.IsNotNull(result);
        Assert.AreEqual(weights[expectedIndex].Info.Room, result.Room);
        Assert.AreEqual(weights[expectedIndex].Info.Distance, result.DistanceFromTarget);
        Assert.AreEqual(weights[expectedIndex].Info.DirectionFromOrigin, result.DirectionFromTarget);
        Assert.IsNull(result.Target);
        Assert.AreEqual(target.Object.RoomLayer, result.RoomLayer);
    }

    [TestMethod]
    public void GetScatterTarget_ReturnsNullWhenTargetLocationMissing()
    {
        Mock<ICharacter> shooter = new();
        Mock<IPerceiver> target = new();
        target.SetupGet(x => x.Location).Returns((IRoom)null);
        Assert.IsNull(LightScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, null));
    }
}

[TestClass]
public class SpreadScatterStrategyTests
{
    [TestMethod]
    public void GetScatterTarget_DropsIntoOriginWhenNoTargets()
    {
        ScatterTestHelpers.SetWeightExpression(typeof(SpreadScatterStrategy), "_weightExpression", "size + proximity + 1");
        ScatterTestHelpers.RoomNetworkBuilder builder = new();
        ScatterTestHelpers.RoomNetworkBuilder.RoomData origin = builder.AddRoom("origin", (0, 0, 0));
        builder.Finalise();

        Mock<ICharacter> shooter = ScatterTestHelpers.CreateCharacter(origin.Room.Object);
        Mock<IPerceiver> target = ScatterTestHelpers.CreatePerceiver(origin.Room.Object);
        ScatterTestHelpers.SeedRandom(23);

        RangedScatterResult result = SpreadScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, Array.Empty<IRoomExit>());

        Assert.IsNotNull(result);
        Assert.AreEqual(origin.Room.Object, result.Room);
        Assert.AreEqual(0, result.DistanceFromTarget);
        Assert.AreEqual(CardinalDirection.Unknown, result.DirectionFromTarget);
        Assert.IsNull(result.Target);
    }

    [TestMethod]
    public void GetScatterTarget_StrikesAvailableTarget()
    {
        ScatterTestHelpers.SetWeightExpression(typeof(SpreadScatterStrategy), "_weightExpression", "size + proximity + 1");
        ScatterTestHelpers.RoomNetworkBuilder builder = new();
        ScatterTestHelpers.RoomNetworkBuilder.RoomData origin = builder.AddRoom("origin", (0, 0, 0));
        builder.Finalise();

        Mock<ICharacter> shooter = ScatterTestHelpers.CreateCharacter(origin.Room.Object);
        Mock<IPerceiver> target = ScatterTestHelpers.CreatePerceiver(origin.Room.Object);
        Mock<ICharacter> victim = ScatterTestHelpers.CreateCharacter(origin.Room.Object);
        victim.Setup(x => x.GetProximity(It.IsAny<IPerceivable>())).Returns(Proximity.Immediate);
        origin.Characters.Add(victim.Object);
        ScatterTestHelpers.SeedRandom(29);

        RangedScatterResult result = SpreadScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, Array.Empty<IRoomExit>());

        Assert.IsNotNull(result);
        Assert.AreEqual(origin.Room.Object, result.Room);
        Assert.AreEqual(victim.Object, result.Target);
        Assert.AreEqual(target.Object.RoomLayer, result.RoomLayer);
    }

    [TestMethod]
    public void GetScatterTarget_ReturnsNullWhenTargetLocationMissing()
    {
        Mock<ICharacter> shooter = new();
        Mock<IPerceiver> target = new();
        target.SetupGet(x => x.Location).Returns((IRoom)null);
        Assert.IsNull(SpreadScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, null));
    }

	[TestMethod]
	public void GetScatterTarget_RouteRoomWithoutNearbyCandidate_PreservesOriginalCoordinate()
	{
		ScatterTestHelpers.SetWeightExpression(
			typeof(SpreadScatterStrategy),
			"_weightExpression",
			"size + proximity + 1");
		var route = ScatterTestHelpers.CreateRouteRoom("long road", 10_000.0);
		var shooter = ScatterTestHelpers.CreateRouteCharacter(route.Room.Object, 90.0);
		var target = ScatterTestHelpers.CreateRoutePerceiver(route.Room.Object, 100.0);
		var farCandidate = ScatterTestHelpers.CreateRouteCharacter(route.Room.Object, 5_000.0);
		route.Characters.Add(shooter.Object);
		route.Characters.Add(farCandidate.Object);
		ScatterTestHelpers.SeedRandom(31);

		var result = SpreadScatterStrategy.Instance.GetScatterTarget(
			shooter.Object,
			target.Object,
			Array.Empty<IRoomExit>());

		Assert.IsNotNull(result);
		Assert.IsNull(result.Target);
		Assert.AreEqual(100.0, result.RoutePositionMetres!.Value, 0.000001);
		Assert.AreEqual(result.RoutePositionMetres, result.ImpactLocation.RoutePositionMetres);
	}

	[TestMethod]
	public void GetScatterTarget_RouteRoomNearbyCandidate_UsesStruckTargetCoordinate()
	{
		ScatterTestHelpers.SetWeightExpression(
			typeof(SpreadScatterStrategy),
			"_weightExpression",
			"size + proximity + 1");
		var route = ScatterTestHelpers.CreateRouteRoom("long road", 10_000.0);
		var shooter = ScatterTestHelpers.CreateRouteCharacter(route.Room.Object, 90.0);
		var target = ScatterTestHelpers.CreateRoutePerceiver(route.Room.Object, 100.0);
		var nearbyCandidate = ScatterTestHelpers.CreateRouteCharacter(route.Room.Object, 102.5);
		route.Characters.Add(shooter.Object);
		route.Characters.Add(nearbyCandidate.Object);
		ScatterTestHelpers.SeedRandom(37);

		var result = SpreadScatterStrategy.Instance.GetScatterTarget(
			shooter.Object,
			target.Object,
			Array.Empty<IRoomExit>());

		Assert.IsNotNull(result);
		Assert.AreSame(nearbyCandidate.Object, result.Target);
		Assert.AreEqual(102.5, result.RoutePositionMetres!.Value, 0.000001);
	}
}

internal static class ScatterTestHelpers
{
	internal sealed record RouteRoomData(
		Mock<IRoom> Room,
		Mock<IRouteRoomDefinition> Route,
		List<ICharacter> Characters,
		List<IGameItem> GameItems);

	private static long _nextRouteRoomId = 10_000;

    internal sealed class RoomNetworkBuilder
    {
        internal sealed class RoomData
        {
            internal RoomData(Mock<IRoom> room, List<ICharacter> characters, List<IGameItem> gameItems)
            {
                Room = room;
                Characters = characters;
                GameItems = gameItems;
            }

            internal Mock<IRoom> Room { get; }
            internal List<ICharacter> Characters { get; }
            internal List<IGameItem> GameItems { get; }
        }

        private readonly Dictionary<string, RoomData> _cells = new();
        private readonly Dictionary<Mock<IRoom>, List<IRoomExit>> _exits = new();
        private long _nextRoomId = 1;

        internal RoomData AddRoom(string name, (int X, int Y, int Z) coordinates)
        {
            Mock<IRoom> room = new();
            long cellId = _nextRoomId++;
            room.SetupGet(x => x.Name).Returns(name);
            room.SetupGet(x => x.Id).Returns(cellId);
            room.SetupGet(x => x.StoredCoordinates).Returns(coordinates);
            room.SetupGet(x => x.X).Returns(coordinates.X);
            room.SetupGet(x => x.Y).Returns(coordinates.Y);
            room.SetupGet(x => x.Z).Returns(coordinates.Z);
            room.SetupGet(x => x.EventHandlers).Returns(Array.Empty<IHandleEvents>());
            room.SetupGet(x => x.Rooms).Returns(Array.Empty<IRoom>());
            room.Setup(x => x.Equals(It.IsAny<object>())).Returns<object>(obj =>
                obj is IRoom other ? other.Id == cellId : ReferenceEquals(obj, room.Object));
            room.Setup(x => x.GetHashCode()).Returns(cellId.GetHashCode());

            List<ICharacter> characters = new();
            room.SetupGet(x => x.Characters).Returns(characters);
            room.Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns((RoomLayer _) => characters);

            List<IGameItem> items = new();
            room.SetupGet(x => x.GameItems).Returns(items);
            room.Setup(x => x.LayerGameItems(It.IsAny<RoomLayer>())).Returns((RoomLayer _) => items);
			room.SetupGet(x => x.Perceivables)
				.Returns(() => characters.Cast<IPerceivable>().Concat(items));

            RoomData data = new(room, characters, items);
            _cells[name] = data;
            _exits[room] = new List<IRoomExit>();
            return data;
        }

        internal void ConnectTwoWay(string originName, string destinationName, CardinalDirection direction, bool createClosedDoor = false,
        bool canFireThrough = true)
        {
            RoomData origin = _cells[originName];
            RoomData destination = _cells[destinationName];
            Mock<IExit> exit = new();
            exit.SetupGet(x => x.Rooms).Returns(new[] { origin.Room.Object, destination.Room.Object });
            exit.SetupProperty(x => x.Door);

            if (createClosedDoor)
            {
                Mock<IDoor> door = new();
                door.SetupGet(x => x.IsOpen).Returns(false);
                door.SetupGet(x => x.CanFireThrough).Returns(canFireThrough);
                door.SetupGet(x => x.CanPlayersSmash).Returns(false);
                door.SetupGet(x => x.Locks).Returns(Array.Empty<ILock>());
                exit.Object.Door = door.Object;
            }

            Mock<IRoomExit> forward = CreateExit(origin.Room.Object, destination.Room.Object, direction, exit.Object);
            Mock<IRoomExit> reverse = CreateExit(destination.Room.Object, origin.Room.Object, direction.Opposite(), exit.Object);
            forward.SetupGet(x => x.Opposite).Returns(reverse.Object);
            reverse.SetupGet(x => x.Opposite).Returns(forward.Object);

            _exits[origin.Room].Add(forward.Object);
            _exits[destination.Room].Add(reverse.Object);
        }

        private static Mock<IRoomExit> CreateExit(IRoom origin, IRoom destination, CardinalDirection direction, IExit sharedExit)
        {
            Mock<IRoomExit> exit = new();
            exit.SetupGet(x => x.Origin).Returns(origin);
            exit.SetupGet(x => x.Destination).Returns(destination);
            exit.SetupGet(x => x.OutboundDirection).Returns(direction);
            exit.SetupGet(x => x.InboundDirection).Returns(direction.Opposite());
            exit.SetupGet(x => x.Exit).Returns(sharedExit);
            return exit;
        }

        internal void Finalise()
        {
            foreach ((Mock<IRoom> room, List<IRoomExit> exits) in _exits)
            {
                room.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>()))
                .Returns((IPerceiver _, bool __) => exits.ToList());
            }
        }

        internal RoomData this[string name] => _cells[name];
    }

    internal static void SeedRandom(int seed)
    {
        Random newRandom = new(seed);
        FieldInfo[] fields = typeof(Random).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (FieldInfo field in fields)
        {
            field.SetValue(Constants.Random, field.GetValue(newRandom));
        }
    }

    internal static void SetWeightExpression(Type strategyType, string fieldName, string expressionText)
    {
        FieldInfo field = strategyType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        field!.SetValue(null, new Expression(expressionText));
    }

    internal static Mock<ICharacter> CreateCharacter(IRoom location)
    {
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Location).Returns(location);
        character.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
        character.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
        character.Setup(x => x.GetProximity(It.IsAny<IPerceivable>())).Returns(Proximity.Distant);
        character.Setup(x => x.Equals(It.IsAny<object>())).Returns<object>(o => ReferenceEquals(o, character.Object));
        return character;
    }

	internal static RouteRoomData CreateRouteRoom(string name, double lengthMetres)
	{
		var characters = new List<ICharacter>();
		var items = new List<IGameItem>();
		var room = new Mock<IRoom>();
		var route = new Mock<IRouteRoomDefinition>();
		room.SetupGet(x => x.Id).Returns(_nextRouteRoomId++);
		room.SetupGet(x => x.Name).Returns(name);
		room.SetupGet(x => x.RouteDefinition).Returns(route.Object);
		room.SetupGet(x => x.SpatialType).Returns(RoomSpatialType.LinearRoute);
		room.SetupGet(x => x.Characters).Returns(characters);
		room.SetupGet(x => x.GameItems).Returns(items);
		room.SetupGet(x => x.Perceivables)
			.Returns(() => characters.Cast<IPerceivable>().Concat(items));
		room.Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>()))
			.Returns((RoomLayer layer) => characters.Where(x => x.RoomLayer == layer));
		room.Setup(x => x.LayerGameItems(It.IsAny<RoomLayer>()))
			.Returns((RoomLayer layer) => items.Where(x => x.RoomLayer == layer));
		route.SetupGet(x => x.Room).Returns(room.Object);
		route.SetupGet(x => x.LengthMetres).Returns(lengthMetres);
		route.SetupGet(x => x.DefaultPositionMetres).Returns(0.0);
		route.SetupGet(x => x.MetresPerRoomEquivalent).Returns(100.0);
		route.SetupGet(x => x.TopologyVersion).Returns(1L);
		route.SetupGet(x => x.Landmarks).Returns([]);
		route.SetupGet(x => x.ExitAnchors).Returns([]);
		return new RouteRoomData(room, route, characters, items);
	}

	internal static Mock<ICharacter> CreateRouteCharacter(
		IRoom location,
		double routePositionMetres,
		IFuturemud? gameworld = null)
	{
		var character = CreateCharacter(location);
		character.SetupGet(x => x.RoutePositionMetres).Returns(routePositionMetres);
		character.SetupGet(x => x.SpatialLocation).Returns(() => new SpatialLocation(
			location,
			character.Object.RoomLayer,
			routePositionMetres));
		character.SetupGet(x => x.Gameworld).Returns(gameworld!);
		return character;
	}

    internal static Mock<IPerceiver> CreatePerceiver(IRoom location)
    {
        Mock<IPerceiver> perceiver = new();
        perceiver.SetupGet(x => x.Location).Returns(location);
        perceiver.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
        perceiver.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
        perceiver.Setup(x => x.GetProximity(It.IsAny<IPerceivable>())).Returns(Proximity.Distant);
        perceiver.Setup(x => x.Equals(It.IsAny<object>())).Returns<object>(o => ReferenceEquals(o, perceiver.Object));
        return perceiver;
    }

	internal static Mock<IPerceiver> CreateRoutePerceiver(
		IRoom location,
		double routePositionMetres,
		IFuturemud? gameworld = null)
	{
		var perceiver = CreatePerceiver(location);
		perceiver.SetupGet(x => x.RoutePositionMetres).Returns(routePositionMetres);
		perceiver.SetupGet(x => x.SpatialLocation).Returns(() => new SpatialLocation(
			location,
			perceiver.Object.RoomLayer,
			routePositionMetres));
		perceiver.SetupGet(x => x.Gameworld).Returns(gameworld!);
		return perceiver;
	}

    internal static List<IRoomExit> CreatePath(params CardinalDirection[] directions)
    {
        List<IRoomExit> path = new();
        foreach (CardinalDirection direction in directions)
        {
            Mock<IRoomExit> exit = new();
            exit.SetupGet(x => x.OutboundDirection).Returns(direction);
            path.Add(exit.Object);
        }
        return path;
    }

    internal static HashSet<CardinalDirection> GetDirectionSet(IReadOnlyList<IRoomExit> path)
    {
        (int Northness, int Southness, int Westness, int Eastness, int Upness, int Downness) counts = path.CountDirections();
        return new HashSet<CardinalDirection>((counts.Northness, counts.Southness, counts.Westness, counts.Eastness,
        counts.Upness, counts.Downness).ContainedDirections().Where(x => x != CardinalDirection.Unknown));
    }

    internal static List<(RoomScatterInfo Info, double Weight)> ComputeDirectionalWeights(Type strategyType,
    IReadOnlyList<RoomScatterInfo> infos, HashSet<CardinalDirection> preferredDirections, CardinalDirection lastDirection)
    {
        MethodInfo method = strategyType.GetMethod("RoomWeight", BindingFlags.Static | BindingFlags.NonPublic);
        List<(RoomScatterInfo, double)> results = new();
        foreach (RoomScatterInfo info in infos)
        {
            double weight = (double)method!.Invoke(null, new object[] { info, preferredDirections, lastDirection });
            if (weight > 0)
            {
                results.Add((info, weight));
            }
        }
        return results;
    }

    internal static List<(RoomScatterInfo Info, double Weight)> ComputeArcingWeights(IReadOnlyList<RoomScatterInfo> infos)
    {
        MethodInfo method = typeof(ArcingScatterStrategy).GetMethod("RoomWeight", BindingFlags.Static | BindingFlags.NonPublic);
        List<(RoomScatterInfo, double)> results = new();
        foreach (RoomScatterInfo info in infos)
        {
            double weight = (double)method!.Invoke(null, new object[] { info });
            if (weight > 0)
            {
                results.Add((info, weight));
            }
        }
        return results;
    }

    internal static int GetExpectedIndex(IReadOnlyList<double> weights, int seed)
    {
        Random random = new(seed);
        double sum = weights.Sum();
        double roll = random.NextDouble() * sum;
        for (int i = 0; i < weights.Count; i++)
        {
            double weight = weights[i];
            if (weight <= 0)
            {
                continue;
            }

            if ((roll -= weight) <= 0.0)
            {
                return i;
            }
        }

        return weights.Count - 1;
    }
}
