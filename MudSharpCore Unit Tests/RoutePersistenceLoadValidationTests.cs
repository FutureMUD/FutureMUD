#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.Vehicles;
using System.IO;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoutePersistenceLoadValidationTests
{
	[TestMethod]
	public void TrackLoad_RouteCoordinateOutsideBounds_ThrowsActionableDiagnostic()
	{
		var gameworld = CreateRouteGameworld(42L, 10_000.0);
		var row = new DB.Track
		{
			Id = 7L,
			RoomId = 42L,
			CharacterId = 1L,
			RoutePosition = 10_001.0m,
			RouteDirection = (int)RouteRoomDirection.Positive,
			MudDateTime = "invalid"
		};

		var exception = Assert.ThrowsException<InvalidDataException>(() => new Track(row, gameworld.Object));
		StringAssert.Contains(exception.Message, "Track #7");
		StringAssert.Contains(exception.Message, "0-10,000.000m");
	}

	[TestMethod]
	public void VehicleLoad_RouteRoomWithNullCoordinate_ThrowsRecoveryDiagnostic()
	{
		var gameworld = CreateRouteGameworld(42L, 10_000.0);
		var row = new DB.Vehicle
		{
			Id = 9L,
			Name = "Corrupt Train",
			VehicleProtoId = 1L,
			VehicleProtoRevision = 0,
			LocationType = (int)VehicleLocationType.Route,
			CurrentRoomId = 42L,
			CurrentRoomLayer = (int)RoomLayer.GroundLevel,
			MovementStatus = (int)VehicleMovementStatus.Stationary,
			CurrentRoutePosition = null
		};

		var exception = Assert.ThrowsException<InvalidDataException>(() => new Vehicle(row, gameworld.Object));
		StringAssert.Contains(exception.Message, "Vehicle #9");
		StringAssert.Contains(exception.Message, "vehicle recovery");
	}

	private static Mock<IFuturemud> CreateRouteGameworld(long cellId, double lengthMetres)
	{
		var room = new Mock<IRoom>();
		var definition = new Mock<IRouteRoomDefinition>();
		room.SetupGet(x => x.Id).Returns(cellId);
		room.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		definition.SetupGet(x => x.Room).Returns(room.Object);
		definition.SetupGet(x => x.LengthMetres).Returns(lengthMetres);
		var rooms = new Mock<IUneditableAll<IRoom>>();
		rooms.Setup(x => x.Get(cellId)).Returns(room.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Rooms).Returns(rooms.Object);
		return gameworld;
	}
}
