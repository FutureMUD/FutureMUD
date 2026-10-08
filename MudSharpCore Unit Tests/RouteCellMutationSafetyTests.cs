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
public class RouteRoomMutationSafetyTests
{
	private const long RoomId = 42L;

	[TestMethod]
	public void InspectOccupancy_ActorMirrorsAreIgnoredButDormantCharacterAndInstanceBlock()
	{
		using var context = BuildContext();
		context.Rooms.Add(NewRoom());
		var actorCharacter = NewCharacter(1L, RoomId);
		var actorInstance = NewCharacterInstance(101L, 1L, RoomId, isPrimary: true);
		context.Characters.Add(actorCharacter);
		context.CharacterInstances.Add(actorInstance);
		context.SaveChanges();

		var actor = new RouteRoomMutationActor(1L, 101L, true);
		Assert.IsFalse(RouteRoomMutationSafety.InspectOccupancy(context, RoomId, actor).HasOtherCharacters);

		var dormantCharacter = NewCharacter(2L, RoomId);
		context.Characters.Add(dormantCharacter);
		context.SaveChanges();
		Assert.IsTrue(RouteRoomMutationSafety.InspectOccupancy(context, RoomId, actor).HasOtherCharacters,
			"A dormant compatibility character location must block RouteCell conversion or removal.");

		dormantCharacter.Location = 7L;
		context.CharacterInstances.Add(NewCharacterInstance(202L, 2L, RoomId));
		context.SaveChanges();
		Assert.AreEqual(RoomId, context.CharacterInstances.Single(x => x.Id == 202L).LocationId);
		Assert.IsTrue(context.CharacterInstances.Any(x => x.LocationId == RoomId && x.Id != 101L));
		Assert.IsTrue(RouteRoomMutationSafety.InspectOccupancy(context, RoomId, actor).HasOtherCharacters,
			"A dormant physical character instance must independently block RouteCell conversion or removal.");
	}

	[TestMethod]
	public void InspectOccupancy_PersistedItemVehicleProjectTrackAndPointLiquidAllBlock()
	{
		using var context = BuildContext();
		context.Rooms.Add(NewRoom("<Surfaces><Layer id=\"0\" position=\"750\"><Surface /></Layer></Surfaces>"));
		context.GameItems.Add(NewGameItem(301L));
		context.RoomsGameItems.Add(new DB.RoomsGameItems { RoomId = RoomId, GameItemId = 301L });
		context.Vehicles.Add(NewVehicle(401L, RoomId));
		context.ActiveProjects.Add(new DB.ActiveProject { Id = 501L, RoomId = RoomId });
		context.Tracks.Add(NewTrack(601L));
		context.SaveChanges();

		var result = RouteRoomMutationSafety.InspectOccupancy(
			context,
			RoomId,
			new RouteRoomMutationActor(1L, 101L, true));

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
		context.Rooms.Add(NewRoom("<Surfaces><Layer id=\"0\" position=\"900\"><Surface /></Layer></Surfaces>"));
		context.Characters.Add(NewCharacter(2L, RoomId, 900.0M));
		context.CharacterInstances.Add(NewCharacterInstance(202L, 2L, RoomId, routePosition: 900.0M));
		context.GameItems.Add(NewGameItem(301L, 900.0M));
		context.RoomsGameItems.Add(new DB.RoomsGameItems { RoomId = RoomId, GameItemId = 301L });
		context.Vehicles.Add(NewVehicle(401L, RoomId, 900.0M));
		context.ActiveProjects.Add(new DB.ActiveProject
		{
			Id = 501L,
			RoomId = RoomId,
			RoutePosition = 900.0M
		});
		context.Tracks.Add(NewTrack(601L, 900.0M));
		context.SaveChanges();

		var result = RouteRoomMutationSafety.InspectLength(context, RoomId, 500.0);

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

		RouteRoomMutationSafety.PersistActorSpatialState(
			context,
			new RouteRoomMutationActor(1L, 101L, true),
			RoomId,
			RoomLayer.InAir,
			125.1236);

		Assert.AreEqual(RoomId, character.Location);
		Assert.AreEqual((int)RoomLayer.InAir, character.RoomLayer);
		Assert.AreEqual(125.124M, character.RoutePosition);
		Assert.AreEqual(RoomId, instance.LocationId);
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

		RouteRoomMutationSafety.PersistActorSpatialState(
			context,
			new RouteRoomMutationActor(1L, 102L, false),
			RoomId,
			RoomLayer.GroundLevel,
			250.0);

		Assert.AreEqual(7L, character.Location,
			"The compatibility character row mirrors the primary instance and must not follow a secondary body.");
		Assert.IsNull(character.RoutePosition);
		Assert.AreEqual(RoomId, instance.LocationId);
		Assert.AreEqual(250.0M, instance.RoutePosition);
	}

	[TestMethod]
	public void PointSurfaceLiquidPositions_UniformStateIsSafeAndInvalidCoordinateFailsClosed()
	{
		Assert.AreEqual(0, RouteRoomMutationSafety.PointSurfaceLiquidPositions(
			"<Surfaces><Layer id=\"0\"><Surface /></Layer></Surfaces>").Count);
		CollectionAssert.AreEqual(
			new[] { 7_150.125 },
			RouteRoomMutationSafety.PointSurfaceLiquidPositions(
				"<Surfaces><Layer id=\"0\" position=\"7150.125\"><Surface /></Layer></Surfaces>").ToArray());
		Assert.ThrowsException<InvalidDataException>(() =>
			RouteRoomMutationSafety.PointSurfaceLiquidPositions(
				"<Surfaces><Layer id=\"0\" position=\"not-a-distance\"><Surface /></Layer></Surfaces>"));
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static DB.Room NewRoom(string? surfaceLiquidData = null)
	{
		return new DB.Room
		{
			Id = RoomId,
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
			CurrentRoomId = cellId,
			CurrentRoutePosition = routePosition
		};
	}

	private static DB.Track NewTrack(long id, decimal? routePosition = null)
	{
		return new DB.Track
		{
			Id = id,
			RoomId = RoomId,
			MudDateTime = "test-time",
			RoutePosition = routePosition
		};
	}
}
