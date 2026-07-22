#nullable enable

using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction;
using MudSharp.Database;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RouteCellMutationSafetyTests
{
	private const long CellId = 42L;

	[TestMethod]
	public void InspectOccupancy_ActorMirrorsAreIgnoredButDormantCharacterAndInstanceBlock()
	{
		using var context = BuildContext();
		context.Cells.Add(NewCell());
		var actorCharacter = NewCharacter(1L, CellId);
		var actorInstance = NewCharacterInstance(101L, 1L, CellId, isPrimary: true);
		context.Characters.Add(actorCharacter);
		context.CharacterInstances.Add(actorInstance);
		context.SaveChanges();

		var actor = new RouteCellMutationActor(1L, 101L, true);
		Assert.IsFalse(RouteCellMutationSafety.InspectOccupancy(context, CellId, actor).HasOtherCharacters);

		var dormantCharacter = NewCharacter(2L, CellId);
		context.Characters.Add(dormantCharacter);
		context.SaveChanges();
		Assert.IsTrue(RouteCellMutationSafety.InspectOccupancy(context, CellId, actor).HasOtherCharacters,
			"A dormant compatibility character location must block RouteCell conversion or removal.");

		dormantCharacter.Location = 7L;
		context.CharacterInstances.Add(NewCharacterInstance(202L, 2L, CellId));
		context.SaveChanges();
		Assert.AreEqual(CellId, context.CharacterInstances.Single(x => x.Id == 202L).LocationId);
		Assert.IsTrue(context.CharacterInstances.Any(x => x.LocationId == CellId && x.Id != 101L));
		Assert.IsTrue(RouteCellMutationSafety.InspectOccupancy(context, CellId, actor).HasOtherCharacters,
			"A dormant physical character instance must independently block RouteCell conversion or removal.");
	}

	[TestMethod]
	public void InspectOccupancy_PersistedItemVehicleProjectTrackAndPointLiquidAllBlock()
	{
		using var context = BuildContext();
		context.Cells.Add(NewCell("<Surfaces><Layer id=\"0\" position=\"750\"><Surface /></Layer></Surfaces>"));
		context.GameItems.Add(NewGameItem(301L));
		context.CellsGameItems.Add(new DB.CellsGameItems { CellId = CellId, GameItemId = 301L });
		context.Vehicles.Add(NewVehicle(401L, CellId));
		context.ActiveProjects.Add(new DB.ActiveProject { Id = 501L, CellId = CellId });
		context.Tracks.Add(NewTrack(601L));
		context.SaveChanges();

		var result = RouteCellMutationSafety.InspectOccupancy(
			context,
			CellId,
			new RouteCellMutationActor(1L, 101L, true));

		Assert.IsTrue(result.HasTopLevelItems);
		Assert.IsTrue(result.HasVehicles);
		Assert.IsTrue(result.HasProjects);
		Assert.IsTrue(result.HasTracks);
		Assert.IsTrue(result.HasPointSurfaceLiquid);
	}

	[TestMethod]
	public void InspectLength_AllPersistedCoordinateOwnersBeyondNewLengthBlockShortening()
	{
		using var context = BuildContext();
		context.Cells.Add(NewCell("<Surfaces><Layer id=\"0\" position=\"900\"><Surface /></Layer></Surfaces>"));
		context.Characters.Add(NewCharacter(2L, CellId, 900.0M));
		context.CharacterInstances.Add(NewCharacterInstance(202L, 2L, CellId, routePosition: 900.0M));
		context.GameItems.Add(NewGameItem(301L, 900.0M));
		context.CellsGameItems.Add(new DB.CellsGameItems { CellId = CellId, GameItemId = 301L });
		context.Vehicles.Add(NewVehicle(401L, CellId, 900.0M));
		context.ActiveProjects.Add(new DB.ActiveProject
		{
			Id = 501L,
			CellId = CellId,
			RoutePosition = 900.0M
		});
		context.Tracks.Add(NewTrack(601L, 900.0M));
		context.SaveChanges();

		var result = RouteCellMutationSafety.InspectLength(context, CellId, 500.0);

		Assert.IsTrue(result.HasCharactersBeyondLength);
		Assert.IsTrue(result.HasTopLevelItemsBeyondLength);
		Assert.IsTrue(result.HasVehiclesBeyondLength);
		Assert.IsTrue(result.HasProjectsBeyondLength);
		Assert.IsTrue(result.HasTracksBeyondLength);
		Assert.IsTrue(result.HasPointSurfaceLiquidBeyondLength);
	}

	[TestMethod]
	public void PersistActorSpatialState_PrimaryUpdatesCompatibilityAndPhysicalInstanceTogether()
	{
		using var context = BuildContext();
		var character = NewCharacter(1L, 7L);
		var instance = NewCharacterInstance(101L, 1L, 7L, isPrimary: true);
		context.Characters.Add(character);
		context.CharacterInstances.Add(instance);
		context.SaveChanges();

		RouteCellMutationSafety.PersistActorSpatialState(
			context,
			new RouteCellMutationActor(1L, 101L, true),
			CellId,
			RoomLayer.InAir,
			125.1236);

		Assert.AreEqual(CellId, character.Location);
		Assert.AreEqual((int)RoomLayer.InAir, character.RoomLayer);
		Assert.AreEqual(125.124M, character.RoutePosition);
		Assert.AreEqual(CellId, instance.LocationId);
		Assert.AreEqual((int)RoomLayer.InAir, instance.RoomLayer);
		Assert.AreEqual(125.124M, instance.RoutePosition);
	}

	[TestMethod]
	public void PersistActorSpatialState_SecondaryDoesNotOverwritePrimaryCompatibilityLocation()
	{
		using var context = BuildContext();
		var character = NewCharacter(1L, 7L);
		var instance = NewCharacterInstance(102L, 1L, 8L);
		context.Characters.Add(character);
		context.CharacterInstances.Add(instance);
		context.SaveChanges();

		RouteCellMutationSafety.PersistActorSpatialState(
			context,
			new RouteCellMutationActor(1L, 102L, false),
			CellId,
			RoomLayer.GroundLevel,
			250.0);

		Assert.AreEqual(7L, character.Location,
			"The compatibility character row mirrors the primary instance and must not follow a secondary body.");
		Assert.IsNull(character.RoutePosition);
		Assert.AreEqual(CellId, instance.LocationId);
		Assert.AreEqual(250.0M, instance.RoutePosition);
	}

	[TestMethod]
	public void PointSurfaceLiquidPositions_UniformStateIsSafeAndInvalidCoordinateFailsClosed()
	{
		Assert.AreEqual(0, RouteCellMutationSafety.PointSurfaceLiquidPositions(
			"<Surfaces><Layer id=\"0\"><Surface /></Layer></Surfaces>").Count);
		CollectionAssert.AreEqual(
			new[] { 7_150.125 },
			RouteCellMutationSafety.PointSurfaceLiquidPositions(
				"<Surfaces><Layer id=\"0\" position=\"7150.125\"><Surface /></Layer></Surfaces>").ToArray());
		Assert.ThrowsException<InvalidDataException>(() =>
			RouteCellMutationSafety.PointSurfaceLiquidPositions(
				"<Surfaces><Layer id=\"0\" position=\"not-a-distance\"><Surface /></Layer></Surfaces>"));
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static DB.Cell NewCell(string? surfaceLiquidData = null)
	{
		return new DB.Cell
		{
			Id = CellId,
			EffectData = "<Effects />",
			SurfaceLiquidData = surfaceLiquidData
		};
	}

	private static DB.Character NewCharacter(long id, long location, decimal? routePosition = null)
	{
		return new DB.Character
		{
			Id = id,
			Name = $"Character {id}",
			Location = location,
			EffectData = "<Effects />",
			BirthdayDate = "0-1-1",
			NeedsModel = "NoNeeds",
			RoutePosition = routePosition
		};
	}

	private static DB.CharacterInstance NewCharacterInstance(
		long id,
		long characterId,
		long? location,
		bool isPrimary = false,
		decimal? routePosition = null)
	{
		return new DB.CharacterInstance
		{
			Id = id,
			CharacterId = characterId,
			LocationId = location,
			IsPrimary = isPrimary,
			EffectData = "<Effects />",
			RoutePosition = routePosition
		};
	}

	private static DB.GameItem NewGameItem(long id, decimal? routePosition = null)
	{
		return new DB.GameItem
		{
			Id = id,
			EffectData = "<Effects />",
			RoutePosition = routePosition
		};
	}

	private static DB.Vehicle NewVehicle(long id, long cellId, decimal? routePosition = null)
	{
		return new DB.Vehicle
		{
			Id = id,
			Name = $"Vehicle {id}",
			CurrentCellId = cellId,
			CurrentRoutePosition = routePosition
		};
	}

	private static DB.Track NewTrack(long id, decimal? routePosition = null)
	{
		return new DB.Track
		{
			Id = id,
			CellId = CellId,
			MudDateTime = "test-time",
			RoutePosition = routePosition
		};
	}
}
