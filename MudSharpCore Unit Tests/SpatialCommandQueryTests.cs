#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpatialCommandQueryTests
{
	[TestMethod]
	public void GameItemsInImmediateVicinity_RouteRoomFiltersDistanceAndLayer()
	{
		var room = CreateRouteRoom(91L);
		var actor = CreateCharacter(910L, room.Object, RoomLayer.GroundLevel, 100.0);
		var near = CreateItem(911L, room.Object, RoomLayer.GroundLevel, 102.9);
		var far = CreateItem(912L, room.Object, RoomLayer.GroundLevel, 103.1);
		var otherLayer = CreateItem(913L, room.Object, RoomLayer.InTrees, 100.0);
		var tracked = new IPerceivable[] { actor.Object, near.Object, far.Object, otherLayer.Object };
		room.SetupGet(x => x.Perceivables).Returns(tracked);
		Track(tracked);

		try
		{
			CollectionAssert.AreEquivalent(
				new[] { near.Object },
				room.Object.GameItemsInImmediateVicinity(actor.Object).ToArray());
		}
		finally
		{
			Untrack(tracked);
		}
	}

	[TestMethod]
	public void CharactersInSpatialVicinity_RouteRoomUsesVeryDistantLimit()
	{
		var room = CreateRouteRoom(92L);
		var actor = CreateCharacter(920L, room.Object, RoomLayer.GroundLevel, 1_000.0);
		var near = CreateCharacter(921L, room.Object, RoomLayer.GroundLevel, 1_500.0);
		var far = CreateCharacter(922L, room.Object, RoomLayer.GroundLevel, 1_500.1);
		var tracked = new IPerceivable[] { actor.Object, near.Object, far.Object };
		room.SetupGet(x => x.Perceivables).Returns(tracked);
		Track(tracked);

		try
		{
			CollectionAssert.AreEquivalent(
				new[] { actor.Object, near.Object },
				room.Object.CharactersInSpatialVicinity(actor.Object).ToArray());
		}
		finally
		{
			Untrack(tracked);
		}
	}

	[TestMethod]
	public void ImmediateQueries_OrdinaryRoomPreserveSameLayerWholeRoomBehaviour()
	{
		var room = new Mock<IRoom>();
		var actor = CreateCharacter(930L, room.Object, RoomLayer.GroundLevel, null);
		var sameLayer = CreateItem(931L, room.Object, RoomLayer.GroundLevel, null);
		var otherLayer = CreateItem(932L, room.Object, RoomLayer.InTrees, null);
		room.SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition?)null);
		room.SetupGet(x => x.Perceivables)
			.Returns(new IPerceivable[] { actor.Object, sameLayer.Object, otherLayer.Object });

		CollectionAssert.AreEquivalent(
			new[] { sameLayer.Object },
			room.Object.GameItemsInImmediateVicinity(actor.Object).ToArray());
	}

	private static Mock<IRoom> CreateRouteRoom(long id)
	{
		var definition = new Mock<IRouteRoomDefinition>();
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Id).Returns(id);
		room.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		definition.SetupGet(x => x.Room).Returns(room.Object);
		definition.SetupGet(x => x.LengthMetres).Returns(10_000.0);
		definition.SetupGet(x => x.DefaultPositionMetres).Returns(0.0);
		definition.SetupGet(x => x.MetresPerRoomEquivalent).Returns(100.0);
		return room;
	}

	private static Mock<ICharacter> CreateCharacter(long id, IRoom room, RoomLayer layer, double? position)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Location).Returns(room);
		character.SetupGet(x => x.RoomLayer).Returns(layer);
		character.SetupGet(x => x.RoutePositionMetres).Returns(position);
		character.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(room, layer, position));
		return character;
	}

	private static Mock<IGameItem> CreateItem(long id, IRoom room, RoomLayer layer, double? position)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Location).Returns(room);
		item.SetupGet(x => x.RoomLayer).Returns(layer);
		item.SetupGet(x => x.RoutePositionMetres).Returns(position);
		item.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(room, layer, position));
		return item;
	}

	private static void Track(IEnumerable<IPerceivable> perceivables)
	{
		foreach (var perceivable in perceivables)
		{
			RouteSpatialService.Instance.TrackPerceivable(perceivable);
		}
	}

	private static void Untrack(IEnumerable<IPerceivable> perceivables)
	{
		foreach (var perceivable in perceivables)
		{
			RouteSpatialService.Instance.UntrackPerceivable(perceivable);
		}
	}
}
