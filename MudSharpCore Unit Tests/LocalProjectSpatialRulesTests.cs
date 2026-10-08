#nullable enable

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.Work.Projects.ConcreteTypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LocalProjectSpatialRulesTests
{
	[TestMethod]
	public void ValidateLoadedSite_RejectsMissingOrOutOfBoundsRouteCoordinates()
	{
		var ordinary = CreateRoom(1L, null);
		var route = CreateRoute(10_000.0);
		var routeRoom = CreateRoom(2L, route.Object);

		var ordinarySite = LocalProjectSpatialRules.ValidateLoadedSite(
			ordinary.Object,
			RoomLayer.GroundLevel,
			null,
			11L);
		Assert.IsNull(ordinarySite.RoutePositionMetres);
		Assert.ThrowsException<InvalidDataException>(() => LocalProjectSpatialRules.ValidateLoadedSite(
			ordinary.Object,
			RoomLayer.GroundLevel,
			1.0,
			12L));
		Assert.ThrowsException<InvalidDataException>(() => LocalProjectSpatialRules.ValidateLoadedSite(
			routeRoom.Object,
			RoomLayer.GroundLevel,
			null,
			13L));
		Assert.ThrowsException<InvalidDataException>(() => LocalProjectSpatialRules.ValidateLoadedSite(
			routeRoom.Object,
			RoomLayer.GroundLevel,
			10_000.001,
			14L));
		Assert.AreEqual(7_150.0, LocalProjectSpatialRules.ValidateLoadedSite(
			routeRoom.Object,
			RoomLayer.GroundLevel,
			7_150.0,
			15L).RoutePositionMetres);
	}

	[TestMethod]
	public void IsAtSite_RouteRoomUsesImmediateDistanceAndLayer_OrdinaryRoomRemainsRoomWide()
	{
		var routeRoom = CreateRoom(20L, CreateRoute(10_000.0).Object);
		var site = new SpatialLocation(routeRoom.Object, RoomLayer.GroundLevel, 7_150.0);
		var near = CreateCharacter(routeRoom.Object, RoomLayer.GroundLevel, 7_153.0);
		var far = CreateCharacter(routeRoom.Object, RoomLayer.GroundLevel, 9_000.0);
		var otherLayer = CreateCharacter(routeRoom.Object, RoomLayer.InTrees, 7_150.0);

		Assert.IsTrue(LocalProjectSpatialRules.IsAtSite(site, near.Object));
		Assert.IsFalse(LocalProjectSpatialRules.IsAtSite(site, far.Object));
		Assert.IsFalse(LocalProjectSpatialRules.IsAtSite(site, otherLayer.Object));

		var ordinary = CreateRoom(21L, null);
		var ordinaryCharacter = CreateCharacter(ordinary.Object, RoomLayer.InTrees, null);
		Assert.IsTrue(LocalProjectSpatialRules.IsAtSite(
			new SpatialLocation(ordinary.Object, RoomLayer.GroundLevel),
			ordinaryCharacter.Object));
	}

	[TestMethod]
	public void ProjectSiteQueries_UseIndexedImmediateNeighbourhood()
	{
		var routeRoom = CreateRoom(30L, CreateRoute(10_000.0).Object);
		var site = new SpatialLocation(routeRoom.Object, RoomLayer.GroundLevel, 7_150.0);
		var nearCharacter = CreateCharacter(routeRoom.Object, RoomLayer.GroundLevel, 7_152.0);
		var farCharacter = CreateCharacter(routeRoom.Object, RoomLayer.GroundLevel, 8_000.0);
		var nearItem = CreateItem(routeRoom.Object, RoomLayer.GroundLevel, 7_147.0);
		var farItem = CreateItem(routeRoom.Object, RoomLayer.GroundLevel, 9_000.0);
		var tracked = new IPerceivable[]
		{
			nearCharacter.Object,
			farCharacter.Object,
			nearItem.Object,
			farItem.Object
		};
		foreach (var perceivable in tracked)
		{
			RouteSpatialService.Instance.TrackPerceivable(perceivable);
		}

		try
		{
			CollectionAssert.AreEquivalent(
				new[] { nearCharacter.Object },
				LocalProjectSpatialRules.CharactersAtSite(site).ToArray());
			CollectionAssert.AreEquivalent(
				new[] { nearItem.Object },
				LocalProjectSpatialRules.GameItemsAtSite(site).ToArray());
		}
		finally
		{
			foreach (var perceivable in tracked)
			{
				RouteSpatialService.Instance.UntrackPerceivable(perceivable);
			}
		}
	}

	[TestMethod]
	public void HandleAtSite_RouteCompletionEchoReachesNearButNotKilometreDistantCharacter()
	{
		var routeRoom = CreateRoom(31L, CreateRoute(10_000.0).Object);
		var site = new SpatialLocation(routeRoom.Object, RoomLayer.GroundLevel, 7_150.0);
		var nearOutput = new Mock<IOutputHandler>();
		var farOutput = new Mock<IOutputHandler>();
		var near = CreateCharacter(routeRoom.Object, RoomLayer.GroundLevel, 7_152.0);
		near.SetupGet(x => x.OutputHandler).Returns(nearOutput.Object);
		var far = CreateCharacter(routeRoom.Object, RoomLayer.GroundLevel, 9_000.0);
		far.SetupGet(x => x.OutputHandler).Returns(farOutput.Object);
		RouteSpatialService.Instance.TrackPerceivable(near.Object);
		RouteSpatialService.Instance.TrackPerceivable(far.Object);

		try
		{
			LocalProjectSpatialRules.HandleAtSite(site, "The harvest is complete.");
			nearOutput.Verify(x => x.Send("The harvest is complete.", true, false), Times.Once);
			farOutput.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
		}
		finally
		{
			RouteSpatialService.Instance.UntrackPerceivable(near.Object);
			RouteSpatialService.Instance.UntrackPerceivable(far.Object);
		}
	}

	[TestMethod]
	public void AmbientScent_RouteCoordinateLimitsAppliesWithoutChangingLegacyGlobalEffects()
	{
		var routeRoom = CreateRoom(40L, CreateRoute(10_000.0).Object);
		var near = CreateCharacter(routeRoom.Object, RoomLayer.GroundLevel, 7_200.0);
		var far = CreateCharacter(routeRoom.Object, RoomLayer.GroundLevel, 9_000.0);
		var localScent = new AmbientScent(
			routeRoom.Object,
			99L,
			"incense",
			"The air smells of cedar.",
			RoomLayer.GroundLevel,
			0,
			Difficulty.Normal,
			routePositionMetres: 7_150.0,
			maximumRouteDistanceMetres: 500.0);
		var legacyGlobalScent = new AmbientScent(
			routeRoom.Object,
			100L,
			"weather",
			"The air smells of rain.",
			RoomLayer.GroundLevel,
			0,
			Difficulty.Normal);

		Assert.IsTrue(localScent.Applies(near.Object));
		Assert.IsFalse(localScent.Applies(far.Object));
		Assert.IsTrue(legacyGlobalScent.Applies(far.Object));
		CollectionAssert.AreEquivalent(
			new[] { localScent, legacyGlobalScent },
			LookingForTracks.ApplicableScentTrails(
				near.Object,
				new[] { localScent, legacyGlobalScent }).ToArray());
		CollectionAssert.AreEquivalent(
			new[] { legacyGlobalScent },
			LookingForTracks.ApplicableScentTrails(
				far.Object,
				new[] { localScent, legacyGlobalScent }).ToArray());
	}

	private static Mock<IRouteRoomDefinition> CreateRoute(double length)
	{
		var route = new Mock<IRouteRoomDefinition>();
		route.SetupGet(x => x.LengthMetres).Returns(length);
		route.SetupGet(x => x.DefaultPositionMetres).Returns(0.0);
		route.SetupGet(x => x.MetresPerRoomEquivalent).Returns(100.0);
		return route;
	}

	private static Mock<IRoom> CreateRoom(long id, IRouteRoomDefinition? route)
	{
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Id).Returns(id);
		room.SetupGet(x => x.RouteDefinition).Returns(route);
		room.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		room.SetupGet(x => x.GameItems).Returns(Array.Empty<IGameItem>());
		room.SetupGet(x => x.Perceivables).Returns(Array.Empty<IPerceivable>());
		return room;
	}

	private static Mock<ICharacter> CreateCharacter(IRoom room, RoomLayer layer, double? position)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Location).Returns(room);
		character.SetupGet(x => x.RoomLayer).Returns(layer);
		character.SetupGet(x => x.RoutePositionMetres).Returns(position);
		character.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(room, layer, position));
		return character;
	}

	private static Mock<IGameItem> CreateItem(IRoom room, RoomLayer layer, double position)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Location).Returns(room);
		item.SetupGet(x => x.RoomLayer).Returns(layer);
		item.SetupGet(x => x.RoutePositionMetres).Returns(position);
		item.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(room, layer, position));
		return item;
	}
}
