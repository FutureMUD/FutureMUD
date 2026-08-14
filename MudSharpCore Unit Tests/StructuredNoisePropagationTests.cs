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
		var a = fixture.Cell();
		var b = fixture.Cell();
		var c = fixture.Cell();
		var disconnected = fixture.Cell();
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
		var a = fixture.Cell();
		var b = fixture.Cell();
		var c = fixture.Cell();
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
		var a = fixture.Cell();
		var expensive = fixture.Cell();
		var cheap = fixture.Cell();
		var destination = fixture.Cell();
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
		Assert.AreEqual(2, results[0].TraversedExits.Count);
	}

	[TestMethod]
	public void Find_OriginAndLayers_DeliversSameLayerNonSourceOnly()
	{
		var fixture = new NoiseGraphFixture();
		var origin = fixture.Cell();
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
		var a = fixture.Cell();
		var b = fixture.Cell();
		var c = fixture.Cell();
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
	public void Find_RouteCell_UsesExactCoordinateAndLayer()
	{
		var fixture = new NoiseGraphFixture();
		var route = fixture.Cell(routeLength: 100.0);
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
		private readonly Dictionary<ICell, List<ICellExit>> _exits = new(ReferenceEqualityComparer.Instance);
		private readonly Dictionary<ICell, List<ICharacter>> _characters = new(ReferenceEqualityComparer.Instance);
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

		public Mock<ICell> Cell(double? routeLength = null)
		{
			var cell = new Mock<ICell>();
			var exits = new List<ICellExit>();
			var characters = new List<ICharacter>();
			_exits[cell.Object] = exits;
			_characters[cell.Object] = characters;
			if (routeLength.HasValue)
			{
				var route = new Mock<IRouteCellDefinition>();
				route.SetupGet(x => x.Cell).Returns(cell.Object);
				route.SetupGet(x => x.LengthMetres).Returns(routeLength.Value);
				route.SetupGet(x => x.MetresPerRoomEquivalent).Returns(1.0);
				route.SetupGet(x => x.ExitAnchors).Returns(Array.Empty<IRouteExitAnchor>());
				cell.SetupGet(x => x.RouteDefinition).Returns(route.Object);
			}
			else
			{
				cell.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
			}
			cell.SetupGet(x => x.Characters).Returns(characters);
			cell.Setup(x => x.ExitsFor(It.IsAny<IPerceiver?>(), It.IsAny<bool>())).Returns(exits);
			cell.Setup(x => x.EstimatedDirectDistanceTo(It.IsAny<ICell>())).Returns(1.0);
			return cell;
		}

		public void Exit(Mock<ICell> origin, Mock<ICell> destination, double coordinateCost)
		{
			var underlying = new Mock<IExit>();
			underlying.SetupProperty(x => x.TimeMultiplier, 1.0);
			var exit = new Mock<ICellExit>();
			exit.SetupGet(x => x.Exit).Returns(underlying.Object);
			exit.SetupGet(x => x.Origin).Returns(origin.Object);
			exit.SetupGet(x => x.Destination).Returns(destination.Object);
			exit.Setup(x => x.WhichLayersExitAppears()).Returns([RoomLayer.GroundLevel]);
			exit.Setup(x => x.MovementTransition(It.IsAny<IPerceiver>()))
				.Returns((CellMovementTransition.GroundToGround, RoomLayer.GroundLevel));
			_exits[origin.Object].Add(exit.Object);
			origin.Setup(x => x.EstimatedDirectDistanceTo(destination.Object)).Returns(coordinateCost);
		}

		public Mock<ICharacter> Listener(
			Mock<ICell> cell,
			RoomLayer layer = RoomLayer.GroundLevel,
			double? position = null)
		{
			var listener = new Mock<ICharacter>();
			listener.SetupGet(x => x.Id).Returns(_nextCharacterId++);
			listener.SetupGet(x => x.Location).Returns(cell.Object);
			listener.SetupGet(x => x.RoomLayer).Returns(layer);
			listener.SetupGet(x => x.RoutePositionMetres).Returns(position);
			listener.SetupGet(x => x.SpatialLocation).Returns(Location(cell, layer, position));
			_characters[cell.Object].Add(listener.Object);
			_spatial.Setup(x => x.GetEffectiveLocation(listener.Object)).Returns(Location(cell, layer, position));
			return listener;
		}

		public SpatialLocation Location(
			Mock<ICell> cell,
			RoomLayer layer = RoomLayer.GroundLevel,
			double? position = null) => new(cell.Object, layer, position);
	}
}
