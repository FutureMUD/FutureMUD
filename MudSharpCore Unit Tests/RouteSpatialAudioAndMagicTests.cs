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
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RouteSpatialAudioAndMagicTests
{
	private static readonly RouteSpatialConfiguration Configuration = new(
		3.0,
		10.0,
		100.0,
		500.0,
		100.0);

	[TestMethod]
	public void RouteAudio_KilometreSeparatedListener_DoesNotReceiveWholeCellEcho()
	{
		var fixture = CreateRouteFixture(10_000.0, 100.0);
		var sourceHandler = new Mock<IOutputHandler>();
		var nearHandler = new Mock<IOutputHandler>();
		var kilometreHandler = new Mock<IOutputHandler>();
		var source = AddCharacter(fixture, 1L, 100.0, sourceHandler.Object);
		AddCharacter(fixture, 2L, 600.0, nearHandler.Object);
		AddCharacter(fixture, 3L, 1_100.0, kilometreHandler.Object);
		var spatial = new RouteSpatialService(Configuration);
		var reachability = new SpatialPerceivableReachability(spatial, new SpatialPathfinder());
		var audio = new RouteCellAudioPropagation(spatial, reachability);

		var propagated = audio.Propagate(
			fixture.Cell.Object,
			"You hear a test sound {0} at {1} volume.",
			AudioVolume.DangerouslyLoud,
			source.Object,
			RoomLayer.GroundLevel,
			false);

		Assert.IsTrue(propagated);
		nearHandler.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Once);
		kilometreHandler.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Never);
	}

	[TestMethod]
	public void SpatialReachability_ExitBand7100To7200_TraversesOnlyWithinWeightedRange()
	{
		var fixture = CreateRouteFixture(10_000.0, 100.0);
		var portal = CreateAnchoredPlatform(fixture, 7_100.0, 7_200.0, 7_150.0);

		var listener = CreateCharacter(portal.Platform.Object, fixture.Gameworld.Object, 30L, null, 0.0);
		portal.Occupants.Add(listener.Object);
		var insideBand = AddCharacter(fixture, 31L, 7_150.0, null);
		var outsideBand = AddCharacter(fixture, 32L, 7_000.0, null);
		var spatial = new RouteSpatialService(Configuration);
		var pathfinder = new SpatialPathfinder();
		Assert.IsTrue(pathfinder.TryFindPath(
			insideBand.Object.SpatialLocation,
			listener.Object.SpatialLocation,
			static _ => true,
			false,
			1.0,
			out var directPath),
			"The authored in-band portal should form a one-room-equivalent hybrid path.");
		Assert.IsNotNull(directPath);
		var reachability = new SpatialPerceivableReachability(spatial, pathfinder);

		var insideResults = reachability.Find(
			insideBand.Object.SpatialLocation,
			1.0,
			x => ReferenceEquals(x, listener.Object));
		var outsideResults = reachability.Find(
			outsideBand.Object.SpatialLocation,
			1.0,
			x => ReferenceEquals(x, listener.Object));

		Assert.AreEqual(1, insideResults.Count);
		Assert.AreEqual(1.0, insideResults.Single().Path.RoomEquivalentCost, 0.000001);
		Assert.AreEqual(0, outsideResults.Count,
			"One room-equivalent to the band plus the exit traversal must cost two and remain out of range.");
	}

	[TestMethod]
	public void RouteAudio_AnchoredPortal_PropagatesInBothDirectionsOnlyAtBand()
	{
		var fixture = CreateRouteFixture(10_000.0, 100.0);
		var portal = CreateAnchoredPlatform(fixture, 7_100.0, 7_200.0, 7_150.0);
		var platformHandler = new Mock<IOutputHandler>();
		var inBandHandler = new Mock<IOutputHandler>();
		var outsideBandHandler = new Mock<IOutputHandler>();
		var platformSource = CreateCharacter(
			portal.Platform.Object,
			fixture.Gameworld.Object,
			50L,
			platformHandler.Object,
			0.0);
		portal.Occupants.Add(platformSource.Object);
		var inBandSource = AddCharacter(fixture, 51L, 7_150.0, inBandHandler.Object);
		AddCharacter(fixture, 52L, 7_000.0, outsideBandHandler.Object);
		var spatial = new RouteSpatialService(Configuration);
		var reachability = new SpatialPerceivableReachability(spatial, new SpatialPathfinder());
		var audio = new RouteCellAudioPropagation(spatial, reachability);

		Assert.IsTrue(audio.RequiresSpatialPropagation(portal.Platform.Object, AudioVolume.Faint));
		Assert.IsTrue(audio.Propagate(
			portal.Platform.Object,
			"You hear a platform sound {0} at {1} volume.",
			AudioVolume.Faint,
			platformSource.Object,
			RoomLayer.GroundLevel,
			false));
		Assert.IsTrue(audio.Propagate(
			fixture.Cell.Object,
			"You hear a route sound {0} at {1} volume.",
			AudioVolume.Faint,
			inBandSource.Object,
			RoomLayer.GroundLevel,
			false));

		inBandHandler.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Once);
		platformHandler.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Once);
		outsideBandHandler.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Never);
	}

	[TestMethod]
	public void SpatialReachability_LayerChangingAnchor_UsesTransitionTargetLayer()
	{
		var fixture = CreateRouteFixture(10_000.0, 100.0);
		var portal = CreateAnchoredPlatform(
			fixture,
			7_100.0,
			7_200.0,
			7_150.0,
			RoomLayer.GroundLevel,
			RoomLayer.InTrees);
		var source = AddCharacter(fixture, 60L, 7_150.0, null);
		var correctLayer = CreateCharacter(
			portal.Platform.Object,
			fixture.Gameworld.Object,
			61L,
			null,
			0.0,
			RoomLayer.InTrees);
		var wrongLayer = CreateCharacter(
			portal.Platform.Object,
			fixture.Gameworld.Object,
			62L,
			null,
			0.0,
			RoomLayer.GroundLevel);
		portal.Occupants.Add(correctLayer.Object);
		portal.Occupants.Add(wrongLayer.Object);
		var reachability = new SpatialPerceivableReachability(
			new RouteSpatialService(Configuration),
			new SpatialPathfinder());

		var results = reachability.Find(source.Object.SpatialLocation, 1.0);

		CollectionAssert.Contains(results.Select(x => x.Perceivable).ToList(), correctLayer.Object);
		CollectionAssert.DoesNotContain(results.Select(x => x.Perceivable).ToList(), wrongLayer.Object);
	}

	[TestMethod]
	public void MagicLocality_UsesImmediateAndWeightedRouteDistances()
	{
		var fixture = CreateRouteFixture(10_000.0, 100.0);
		var owner = AddCharacter(fixture, 40L, 100.0, null);
		var immediate = AddCharacter(fixture, 41L, 103.0, null);
		var adjacentBoundary = AddCharacter(fixture, 42L, 200.0, null);
		var beyondAdjacent = AddCharacter(fixture, 43L, 200.001, null);
		var kilometreAway = AddCharacter(fixture, 44L, 1_100.0, null);
		var spatial = new RouteSpatialService(Configuration);
		var pathfinder = new SpatialPathfinder();
		var reachability = new SpatialPerceivableReachability(spatial, pathfinder);

		var sameLocation = MagicPowerSpatialTargeting.AcquireTargets(
			owner.Object,
			MagicPowerDistance.SameLocationOnly,
			spatial,
			reachability);
		var adjacent = MagicPowerSpatialTargeting.AcquireTargets(
			owner.Object,
			MagicPowerDistance.AdjacentLocationsOnly,
			spatial,
			reachability);

		CollectionAssert.Contains(sameLocation.ToList(), immediate.Object);
		CollectionAssert.DoesNotContain(sameLocation.ToList(), adjacentBoundary.Object);
		CollectionAssert.DoesNotContain(sameLocation.ToList(), kilometreAway.Object);
		CollectionAssert.Contains(adjacent.ToList(), adjacentBoundary.Object);
		CollectionAssert.DoesNotContain(adjacent.ToList(), beyondAdjacent.Object);
		CollectionAssert.DoesNotContain(adjacent.ToList(), kilometreAway.Object);
		Assert.IsTrue(MagicPowerSpatialTargeting.IsInRange(
			owner.Object,
			adjacentBoundary.Object,
			MagicPowerDistance.AdjacentLocationsOnly,
			spatial,
			pathfinder));
		Assert.IsFalse(MagicPowerSpatialTargeting.IsInRange(
			owner.Object,
			beyondAdjacent.Object,
			MagicPowerDistance.AdjacentLocationsOnly,
			spatial,
			pathfinder));
	}

	private static RouteFixture CreateRouteFixture(double lengthMetres, double roomEquivalentMetres)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("RouteCellImmediateDistanceMetres"))
			.Returns(Configuration.ImmediateDistanceMetres);
		var cell = new Mock<ICell>();
		var occupants = new List<IPerceivable>();
		var exits = new List<ICellExit>();
		var anchors = new List<IRouteExitAnchor>();
		var definition = new Mock<IRouteCellDefinition>();
		definition.SetupGet(x => x.Cell).Returns(cell.Object);
		definition.SetupGet(x => x.LengthMetres).Returns(lengthMetres);
		definition.SetupGet(x => x.DefaultPositionMetres).Returns(0.0);
		definition.SetupGet(x => x.PositiveDirectionName).Returns("forward");
		definition.SetupGet(x => x.NegativeDirectionName).Returns("backward");
		definition.SetupGet(x => x.MetresPerRoomEquivalent).Returns(roomEquivalentMetres);
		definition.SetupGet(x => x.TopologyVersion).Returns(1L);
		definition.SetupGet(x => x.Landmarks).Returns([]);
		definition.SetupGet(x => x.ExitAnchors).Returns(anchors);
		cell.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		cell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		cell.SetupGet(x => x.Perceivables).Returns(occupants);
		cell.Setup(x => x.ExitsFor(It.IsAny<IPerceiver?>(), It.IsAny<bool>()))
			.Returns(exits);
		return new RouteFixture(gameworld, cell, occupants, exits, anchors);
	}

	private static Mock<ICharacter> AddCharacter(
		RouteFixture fixture,
		long id,
		double position,
		IOutputHandler? handler)
	{
		var character = CreateCharacter(
			fixture.Cell.Object,
			fixture.Gameworld.Object,
			id,
			handler,
			position);
		fixture.Occupants.Add(character.Object);
		return character;
	}

	private static Mock<ICharacter> CreateCharacter(
		ICell cell,
		IFuturemud gameworld,
		long id,
		IOutputHandler? handler,
		double position,
		RoomLayer layer = RoomLayer.GroundLevel)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Location).Returns(cell);
		character.SetupGet(x => x.RoomLayer).Returns(layer);
		character.SetupGet(x => x.RoutePositionMetres)
			.Returns(cell.RouteDefinition is null ? null : position);
		character.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(
				cell,
				layer,
				cell.RouteDefinition is null ? null : position));
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.OutputHandler).Returns(handler!);
		return character;
	}

	private static PlatformPortal CreateAnchoredPlatform(
		RouteFixture fixture,
		double minimum,
		double maximum,
		double arrival,
		RoomLayer routeLayer = RoomLayer.GroundLevel,
		RoomLayer platformLayer = RoomLayer.GroundLevel)
	{
		var platform = new Mock<ICell>();
		var platformOccupants = new List<IPerceivable>();
		var platformExits = new List<ICellExit>();
		platform.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
		platform.SetupGet(x => x.Perceivables).Returns(platformOccupants);
		platform.Setup(x => x.ExitsFor(It.IsAny<IPerceiver?>(), It.IsAny<bool>()))
			.Returns(platformExits);

		var underlyingExit = new Mock<IExit>();
		underlyingExit.SetupGet(x => x.Id).Returns(9001L);
		underlyingExit.SetupProperty(x => x.TimeMultiplier, 1.0);
		var routeSide = new Mock<ICellExit>();
		var platformSide = new Mock<ICellExit>();
		routeSide.SetupGet(x => x.Exit).Returns(underlyingExit.Object);
		routeSide.SetupGet(x => x.Origin).Returns(fixture.Cell.Object);
		routeSide.SetupGet(x => x.Destination).Returns(platform.Object);
		routeSide.SetupGet(x => x.Opposite).Returns(platformSide.Object);
		routeSide.Setup(x => x.WhichLayersExitAppears()).Returns([routeLayer]);
		routeSide.Setup(x => x.MovementTransition(It.IsAny<IPerceiver>()))
			.Returns((TransitionType(routeLayer, platformLayer), platformLayer));
		platformSide.SetupGet(x => x.Exit).Returns(underlyingExit.Object);
		platformSide.SetupGet(x => x.Origin).Returns(platform.Object);
		platformSide.SetupGet(x => x.Destination).Returns(fixture.Cell.Object);
		platformSide.SetupGet(x => x.Opposite).Returns(routeSide.Object);
		platformSide.Setup(x => x.WhichLayersExitAppears()).Returns([platformLayer]);
		platformSide.Setup(x => x.MovementTransition(It.IsAny<IPerceiver>()))
			.Returns((TransitionType(platformLayer, routeLayer), routeLayer));
		fixture.Exits.Add(routeSide.Object);
		platformExits.Add(platformSide.Object);
		fixture.Anchors.Add(CreateAnchor(
			fixture.Cell.Object,
			routeSide.Object,
			minimum,
			maximum,
			arrival));
		return new PlatformPortal(platform, platformOccupants);
	}

	private static CellMovementTransition TransitionType(RoomLayer origin, RoomLayer destination)
	{
		return origin == RoomLayer.GroundLevel && destination == RoomLayer.GroundLevel
			? CellMovementTransition.GroundToGround
			: CellMovementTransition.TreesToTrees;
	}

	private static IRouteExitAnchor CreateAnchor(
		ICell cell,
		ICellExit exit,
		double minimum,
		double maximum,
		double arrival)
	{
		var anchor = new Mock<IRouteExitAnchor>();
		anchor.SetupGet(x => x.Cell).Returns(cell);
		anchor.SetupGet(x => x.Exit).Returns(exit);
		anchor.SetupGet(x => x.MinimumPositionMetres).Returns(minimum);
		anchor.SetupGet(x => x.MaximumPositionMetres).Returns(maximum);
		anchor.SetupGet(x => x.ArrivalPositionMetres).Returns(arrival);
		anchor.Setup(x => x.Contains(It.IsAny<double>()))
			.Returns((double position) => position >= minimum && position <= maximum);
		return anchor.Object;
	}

	private sealed record RouteFixture(
		Mock<IFuturemud> Gameworld,
		Mock<ICell> Cell,
		List<IPerceivable> Occupants,
		List<ICellExit> Exits,
		List<IRouteExitAnchor> Anchors);

	private sealed record PlatformPortal(
		Mock<ICell> Platform,
		List<IPerceivable> Occupants);
}
