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
	public void GameItemsInImmediateVicinity_RouteCellFiltersDistanceAndLayer()
	{
		var cell = CreateRouteCell(91L);
		var actor = CreateCharacter(910L, cell.Object, RoomLayer.GroundLevel, 100.0);
		var near = CreateItem(911L, cell.Object, RoomLayer.GroundLevel, 102.9);
		var far = CreateItem(912L, cell.Object, RoomLayer.GroundLevel, 103.1);
		var otherLayer = CreateItem(913L, cell.Object, RoomLayer.InTrees, 100.0);
		var tracked = new IPerceivable[] { actor.Object, near.Object, far.Object, otherLayer.Object };
		cell.SetupGet(x => x.Perceivables).Returns(tracked);
		Track(tracked);

		try
		{
			CollectionAssert.AreEquivalent(
				new[] { near.Object },
				cell.Object.GameItemsInImmediateVicinity(actor.Object).ToArray());
		}
		finally
		{
			Untrack(tracked);
		}
	}

	[TestMethod]
	public void CharactersInSpatialVicinity_RouteCellUsesVeryDistantLimit()
	{
		var cell = CreateRouteCell(92L);
		var actor = CreateCharacter(920L, cell.Object, RoomLayer.GroundLevel, 1_000.0);
		var near = CreateCharacter(921L, cell.Object, RoomLayer.GroundLevel, 1_500.0);
		var far = CreateCharacter(922L, cell.Object, RoomLayer.GroundLevel, 1_500.1);
		var tracked = new IPerceivable[] { actor.Object, near.Object, far.Object };
		cell.SetupGet(x => x.Perceivables).Returns(tracked);
		Track(tracked);

		try
		{
			CollectionAssert.AreEquivalent(
				new[] { actor.Object, near.Object },
				cell.Object.CharactersInSpatialVicinity(actor.Object).ToArray());
		}
		finally
		{
			Untrack(tracked);
		}
	}

	[TestMethod]
	public void ImmediateQueries_OrdinaryCellPreserveSameLayerWholeCellBehaviour()
	{
		var cell = new Mock<ICell>();
		var actor = CreateCharacter(930L, cell.Object, RoomLayer.GroundLevel, null);
		var sameLayer = CreateItem(931L, cell.Object, RoomLayer.GroundLevel, null);
		var otherLayer = CreateItem(932L, cell.Object, RoomLayer.InTrees, null);
		cell.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
		cell.SetupGet(x => x.Perceivables)
			.Returns(new IPerceivable[] { actor.Object, sameLayer.Object, otherLayer.Object });

		CollectionAssert.AreEquivalent(
			new[] { sameLayer.Object },
			cell.Object.GameItemsInImmediateVicinity(actor.Object).ToArray());
	}

	private static Mock<ICell> CreateRouteCell(long id)
	{
		var definition = new Mock<IRouteCellDefinition>();
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		definition.SetupGet(x => x.Cell).Returns(cell.Object);
		definition.SetupGet(x => x.LengthMetres).Returns(10_000.0);
		definition.SetupGet(x => x.DefaultPositionMetres).Returns(0.0);
		definition.SetupGet(x => x.MetresPerRoomEquivalent).Returns(100.0);
		return cell;
	}

	private static Mock<ICharacter> CreateCharacter(long id, ICell cell, RoomLayer layer, double? position)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Location).Returns(cell);
		character.SetupGet(x => x.RoomLayer).Returns(layer);
		character.SetupGet(x => x.RoutePositionMetres).Returns(position);
		character.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(cell, layer, position));
		return character;
	}

	private static Mock<IGameItem> CreateItem(long id, ICell cell, RoomLayer layer, double? position)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Location).Returns(cell);
		item.SetupGet(x => x.RoomLayer).Returns(layer);
		item.SetupGet(x => x.RoutePositionMetres).Returns(position);
		item.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(cell, layer, position));
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
