#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Movement;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StructuredNoisePropagationTests
{
	[TestMethod]
	public void Find_TopologicalMode_UsesExitCountAndNeverCoordinateAdjacency()
	{
		var fixture = new NoiseGraphFixture();
		var a = fixture.Room();
		var b = fixture.Room();
		var c = fixture.Room();
		var disconnected = fixture.Room();
		fixture.Exit(a, b, 1.0);
		fixture.Exit(b, c, 5.0);
		var bListener = fixture.Listener(b);
		var cListener = fixture.Listener(c);
		var disconnectedListener = fixture.Listener(disconnected);

		var results = fixture.Subject.Find(
			fixture.Location(a),
			1.0,
			AudioPropagationMode.Topological);

		CollectionAssert.Contains(results.Select(x => x.Listener).ToList(), bListener.Object);
		CollectionAssert.DoesNotContain(results.Select(x => x.Listener).ToList(), cListener.Object);
		CollectionAssert.DoesNotContain(results.Select(x => x.Listener).ToList(), disconnectedListener.Object);
	}

	[TestMethod]
	public void Find_CoordinateMode_AccumulatesOneAndFiveOnlyAlongExits()
	{
		var fixture = new NoiseGraphFixture();
		var a = fixture.Room();
		var b = fixture.Room();
		var c = fixture.Room();
		fixture.Exit(a, b, 1.0);
		fixture.Exit(b, c, 5.0);
		var bListener = fixture.Listener(b);
		var cListener = fixture.Listener(c);

		var five = fixture.Subject.Find(
			fixture.Location(a),
			5.0,
			AudioPropagationMode.CoordinateAware);
		var six = fixture.Subject.Find(
			fixture.Location(a),
			6.0,
			AudioPropagationMode.CoordinateAware);

		Assert.AreEqual(1.0, five.Single(x => ReferenceEquals(x.Listener, bListener.Object)).Cost, 0.0001);
		Assert.IsFalse(five.Any(x => ReferenceEquals(x.Listener, cListener.Object)));
		Assert.AreEqual(6.0, six.Single(x => ReferenceEquals(x.Listener, cListener.Object)).Cost, 0.0001);
	}

	[TestMethod]
	public void Find_CyclesAndCompetingRoutes_DeliverListenerOnceByCheapestRoute()
	{
		var fixture = new NoiseGraphFixture();
		var a = fixture.Room();
		var expensive = fixture.Room();
		var cheap = fixture.Room();
		var destination = fixture.Room();
		fixture.Exit(a, expensive, 5.0);
		fixture.Exit(expensive, destination, 5.0);
		fixture.Exit(a, cheap, 1.0);
		fixture.Exit(cheap, destination, 1.0);
		fixture.Exit(destination, a, 1.0);
		var listener = fixture.Listener(destination);

		var results = fixture.Subject.Find(
			fixture.Location(a),
			10.0,
			AudioPropagationMode.CoordinateAware)
			.Where(x => ReferenceEquals(x.Listener, listener.Object))
			.ToList();

		Assert.AreEqual(1, results.Count);
		Assert.AreEqual(2.0, results[0].Cost, 0.0001);
	}

	[TestMethod]
	public void Find_OriginAndLayers_DeliversSameLayerNonSourceOnly()
	{
		var fixture = new NoiseGraphFixture();
		var origin = fixture.Room();
		var sameLayer = fixture.Listener(origin, RoomLayer.GroundLevel);
		var otherLayer = fixture.Listener(origin, RoomLayer.InAir);

		var results = fixture.Subject.Find(
			fixture.Location(origin),
			1.0,
			AudioPropagationMode.Topological);

		CollectionAssert.Contains(results.Select(x => x.Listener).ToList(), sameLayer.Object);
		CollectionAssert.DoesNotContain(results.Select(x => x.Listener).ToList(), otherLayer.Object);
	}

	[TestMethod]
	public void Find_TraversalCeiling_StopsExpansionConservatively()
	{
		var fixture = new NoiseGraphFixture(traversalCeiling: 2);
		var a = fixture.Room();
		var b = fixture.Room();
		var c = fixture.Room();
		fixture.Exit(a, b, 1.0);
		fixture.Exit(b, c, 1.0);
		var bListener = fixture.Listener(b);
		var cListener = fixture.Listener(c);

		var results = fixture.Subject.Find(
			fixture.Location(a),
			10.0,
			AudioPropagationMode.Topological);

		CollectionAssert.Contains(results.Select(x => x.Listener).ToList(), bListener.Object);
		CollectionAssert.DoesNotContain(results.Select(x => x.Listener).ToList(), cListener.Object);
	}

	[TestMethod]
	public void Find_RouteRoom_UsesExactCoordinateAndLayer()
	{
		var fixture = new NoiseGraphFixture();
		var route = fixture.Room(routeLength: 100.0);
		var near = fixture.Listener(route, position: 1.0);
		var far = fixture.Listener(route, position: 6.0);
		var otherLayer = fixture.Listener(route, RoomLayer.InAir, 1.0);

		var results = fixture.Subject.Find(
			fixture.Location(route, position: 0.0),
			5.0,
			AudioPropagationMode.Topological);

		Assert.AreEqual(1.0, results.Single(x => ReferenceEquals(x.Listener, near.Object)).Cost, 0.0001);
		Assert.IsFalse(results.Any(x => ReferenceEquals(x.Listener, far.Object)));
		Assert.IsFalse(results.Any(x => ReferenceEquals(x.Listener, otherLayer.Object)));
	}

	[TestMethod]
	public void Find_ReportsInitialExitDirectionRatherThanTheRouteAggregate()
	{
		var fixture = new NoiseGraphFixture();
		var a = fixture.Room();
		var b = fixture.Room();
		var c = fixture.Room();
		fixture.Exit(a, b, 1.0, CardinalDirection.East, CardinalDirection.West);
		fixture.Exit(b, c, 1.0, CardinalDirection.North, CardinalDirection.South);
		var listener = fixture.Listener(c);

		var result = fixture.Subject.Find(
			fixture.Location(a),
			10.0,
			AudioPropagationMode.Topological)
			.Single(x => ReferenceEquals(x.Listener, listener.Object));

		StringAssert.Contains(result.Direction, "South");
		Assert.IsFalse(result.Direction.Contains("West", StringComparison.Ordinal));
	}

	[TestMethod]
	public void Find_PrioritisesTheListenerRouteRoomLongitudinalDirection()
	{
		var fixture = new NoiseGraphFixture();
		var origin = fixture.Room();
		var route = fixture.Room(routeLength: 100.0, positiveDirection: "upstream", negativeDirection: "downstream");
		var entry = fixture.Exit(origin, route, 1.0, CardinalDirection.North, CardinalDirection.South);
		fixture.SetArrivalAnchor(entry, route, 0.0);
		var listener = fixture.Listener(route, position: 10.0);

		var result = fixture.Subject.Find(
			fixture.Location(origin),
			20.0,
			AudioPropagationMode.Topological)
			.Single(x => ReferenceEquals(x.Listener, listener.Object));

		Assert.AreEqual("from downstream", result.Direction);
	}

	[TestMethod]
	public void Find_RejectsUndefinedPropagationModes()
	{
		var fixture = new NoiseGraphFixture();
		var origin = fixture.Room();

		Assert.ThrowsException<ArgumentOutOfRangeException>(() => fixture.Subject.Find(
			fixture.Location(origin),
			1.0,
			(AudioPropagationMode)99));
	}

	[TestMethod]
	public void Attenuate_UsesBudgetIndependentlyAndRemainsNonSilentWithinBudget()
	{
		Assert.AreEqual(AudioVolume.VeryLoud,
			StructuredNoisePropagation.Attenuate(AudioVolume.VeryLoud, 0.0, 20.0));
		Assert.AreEqual(AudioVolume.Loud,
			StructuredNoisePropagation.Attenuate(AudioVolume.VeryLoud, 4.0, 20.0));
		Assert.AreEqual(AudioVolume.Faint,
			StructuredNoisePropagation.Attenuate(AudioVolume.VeryLoud, 20.0, 20.0));
	}

	private sealed class NoiseGraphFixture
	{
		private readonly Mock<IRouteSpatialService> _spatial = new();
		private readonly Dictionary<IRoom, List<IRoomExit>> _exits = new(ReferenceEqualityComparer.Instance);
		private readonly Dictionary<IRoom, List<ICharacter>> _characters = new(ReferenceEqualityComparer.Instance);
		private long _nextCharacterId = 1;

		public NoiseGraphFixture(int traversalCeiling = StructuredNoisePropagation.DefaultTraversalCeiling)
		{
			_spatial.Setup(x => x.TryValidateLocation(It.IsAny<SpatialLocation>(), out It.Ref<string>.IsAny))
				.Returns((SpatialLocation _, out string error) =>
				{
					error = string.Empty;
					return true;
				});
			Subject = new StructuredNoisePropagation(_spatial.Object, traversalCeiling);
		}

		public StructuredNoisePropagation Subject { get; }

		public Mock<IRoom> Room(
			double? routeLength = null,
			string positiveDirection = "positive",
			string negativeDirection = "negative")
		{
			var room = new Mock<IRoom>();
			var exits = new List<IRoomExit>();
			var characters = new List<ICharacter>();
			_exits[room.Object] = exits;
			_characters[room.Object] = characters;
			if (routeLength.HasValue)
			{
				var route = new Mock<IRouteRoomDefinition>();
				route.SetupGet(x => x.Room).Returns(room.Object);
				route.SetupGet(x => x.LengthMetres).Returns(routeLength.Value);
				route.SetupGet(x => x.MetresPerRoomEquivalent).Returns(1.0);
				route.SetupGet(x => x.PositiveDirectionName).Returns(positiveDirection);
				route.SetupGet(x => x.NegativeDirectionName).Returns(negativeDirection);
				route.SetupGet(x => x.ExitAnchors).Returns(Array.Empty<IRouteExitAnchor>());
				room.SetupGet(x => x.RouteDefinition).Returns(route.Object);
			}
			else
			{
				room.SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition?)null);
			}
			room.SetupGet(x => x.Characters).Returns(characters);
			room.Setup(x => x.ExitsFor(It.IsAny<IPerceiver?>(), It.IsAny<bool>())).Returns(exits);
			room.Setup(x => x.EstimatedDirectDistanceTo(It.IsAny<IRoom>())).Returns(1.0);
			return room;
		}

		public Mock<IRoomExit> Exit(
			Mock<IRoom> origin,
			Mock<IRoom> destination,
			double coordinateCost,
			CardinalDirection outboundDirection = CardinalDirection.Unknown,
			CardinalDirection inboundDirection = CardinalDirection.Unknown)
		{
			var underlying = new Mock<IExit>();
			underlying.SetupProperty(x => x.TimeMultiplier, 1.0);
			var exit = new Mock<IRoomExit>();
			exit.SetupGet(x => x.Exit).Returns(underlying.Object);
			exit.SetupGet(x => x.Origin).Returns(origin.Object);
			exit.SetupGet(x => x.Destination).Returns(destination.Object);
			exit.SetupGet(x => x.OutboundDirection).Returns(outboundDirection);
			var opposite = new Mock<IRoomExit>();
			opposite.SetupGet(x => x.OutboundDirection).Returns(inboundDirection);
			exit.SetupGet(x => x.Opposite).Returns(opposite.Object);
			exit.Setup(x => x.WhichLayersExitAppears()).Returns([RoomLayer.GroundLevel]);
			exit.Setup(x => x.MovementTransition(It.IsAny<IPerceiver>()))
				.Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
			_exits[origin.Object].Add(exit.Object);
			origin.Setup(x => x.EstimatedDirectDistanceTo(destination.Object)).Returns(coordinateCost);
			return exit;
		}

		public void SetArrivalAnchor(Mock<IRoomExit> exit, Mock<IRoom> routeRoom, double arrivalPosition)
		{
			var anchor = new Mock<IRouteExitAnchor>();
			anchor.SetupGet(x => x.ArrivalPositionMetres).Returns(arrivalPosition);
			_spatial.Setup(x => x.TryGetExitAnchor(
					exit.Object,
					routeRoom.Object,
					out It.Ref<IRouteExitAnchor?>.IsAny))
				.Returns((IRoomExit _, IRoom _, out IRouteExitAnchor? resolved) =>
				{
					resolved = anchor.Object;
					return true;
				});
		}

		public Mock<ICharacter> Listener(
			Mock<IRoom> room,
			RoomLayer layer = RoomLayer.GroundLevel,
			double? position = null)
		{
			var listener = new Mock<ICharacter>();
			listener.SetupGet(x => x.Id).Returns(_nextCharacterId++);
			listener.SetupGet(x => x.Location).Returns(room.Object);
			listener.SetupGet(x => x.RoomLayer).Returns(layer);
			listener.SetupGet(x => x.RoutePositionMetres).Returns(position);
			listener.SetupGet(x => x.SpatialLocation).Returns(Location(room, layer, position));
			_characters[room.Object].Add(listener.Object);
			_spatial.Setup(x => x.GetEffectiveLocation(listener.Object)).Returns(Location(room, layer, position));
			return listener;
		}

		public SpatialLocation Location(
			Mock<IRoom> room,
			RoomLayer layer = RoomLayer.GroundLevel,
			double? position = null) => new(room.Object, layer, position);
	}
}
