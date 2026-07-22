#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RouteMovementOutputTests
{
	[TestMethod]
	public void CharacterLifecycle_ReachesNearbyObserverButNotKilometreDistantObserver()
	{
		var fixture = CreateRouteFixture();
		var sourceOutput = new Mock<IOutputHandler>();
		var source = CreateCharacter(
			fixture.Cell.Object,
			fixture.Gameworld.Object,
			100.0,
			sourceOutput.Object);
		sourceOutput.SetupGet(x => x.Perceiver).Returns(source.Object);
		var nearOutput = new Mock<IOutputHandler>();
		var near = CreateCharacter(
			fixture.Cell.Object,
			fixture.Gameworld.Object,
			150.0,
			nearOutput.Object);
		var farOutput = new Mock<IOutputHandler>();
		var far = CreateCharacter(
			fixture.Cell.Object,
			fixture.Gameworld.Object,
			1_100.0,
			farOutput.Object);
		fixture.Occupants.AddRange([source.Object, near.Object, far.Object]);

		RouteMovementOutput.CharacterBegin(source.Object, "townward");
		RouteMovementOutput.CharacterProgress(source.Object, "townward");
		RouteMovementOutput.CharacterFinished(source.Object, "arrive|arrives at the destination");
		RouteMovementOutput.CharacterFinished(source.Object, "stop|stops travelling");

		nearOutput.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Exactly(4));
		farOutput.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Never);
	}

	[TestMethod]
	public void RoomScaleVehicleLifecycle_SeparatesExteriorAndHostedInteriorAudiences()
	{
		var fixture = CreateRouteFixture();
		var exteriorOutput = new Mock<IOutputHandler>();
		var exterior = CreateExterior(
			fixture.Cell.Object,
			fixture.Gameworld.Object,
			100.0,
			exteriorOutput.Object);
		exteriorOutput.SetupGet(x => x.Perceiver).Returns(exterior.Object);
		var nearOutput = new Mock<IOutputHandler>();
		var near = CreateCharacter(
			fixture.Cell.Object,
			fixture.Gameworld.Object,
			150.0,
			nearOutput.Object);
		var farOutput = new Mock<IOutputHandler>();
		var far = CreateCharacter(
			fixture.Cell.Object,
			fixture.Gameworld.Object,
			1_100.0,
			farOutput.Object);
		fixture.Occupants.AddRange([exterior.Object, near.Object, far.Object]);

		var interiorOutput = new Mock<IOutputHandler>();
		var interiorOccupant = new Mock<ICharacter>();
		interiorOccupant.SetupGet(x => x.OutputHandler).Returns(interiorOutput.Object);
		var interior = new Mock<ICell>();
		interior.SetupGet(x => x.Cells).Returns([interior.Object]);
		interior.SetupGet(x => x.Characters).Returns([interiorOccupant.Object]);
		interior.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel))
			.Returns([interiorOccupant.Object]);
		var compartment = new Mock<IVehicleCompartment>();
		compartment.SetupGet(x => x.InteriorCell).Returns(interior.Object);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.Compartments).Returns([compartment.Object]);

		RouteMovementOutput.VehicleBegin(vehicle.Object, "townward");
		RouteMovementOutput.VehicleProgress(vehicle.Object, "townward");
		RouteMovementOutput.VehicleFinished(vehicle.Object, true);
		RouteMovementOutput.VehicleFinished(vehicle.Object, false);

		nearOutput.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Exactly(4));
		farOutput.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Never);
		interiorOutput.Verify(
			x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Exactly(4));
		interiorOutput.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Never);
		nearOutput.Verify(
			x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Never);
	}

	private static RouteFixture CreateRouteFixture()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("RouteCellVeryDistantDistanceMetres"))
			.Returns(500.0);

		var occupants = new List<IPerceivable>();
		var cell = new Mock<ICell>();
		var definition = new Mock<IRouteCellDefinition>();
		definition.SetupGet(x => x.Cell).Returns(cell.Object);
		definition.SetupGet(x => x.LengthMetres).Returns(2_000.0);
		definition.SetupGet(x => x.DefaultPositionMetres).Returns(0.0);
		definition.SetupGet(x => x.MetresPerRoomEquivalent).Returns(100.0);
		definition.SetupGet(x => x.Landmarks).Returns([]);
		definition.SetupGet(x => x.ExitAnchors).Returns([]);
		cell.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		cell.SetupGet(x => x.Perceivables).Returns(occupants);
		cell.SetupGet(x => x.Cells).Returns([cell.Object]);
		cell.SetupGet(x => x.Characters).Returns(() => occupants.OfType<ICharacter>());
		cell.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel))
			.Returns(() => occupants.OfType<ICharacter>());
		return new RouteFixture(gameworld, cell, occupants);
	}

	private static Mock<ICharacter> CreateCharacter(
		ICell cell,
		IFuturemud gameworld,
		double position,
		IOutputHandler outputHandler)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Name).Returns("route observer");
		character.SetupGet(x => x.Location).Returns(cell);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		character.SetupGet(x => x.RoutePositionMetres).Returns(position);
		character.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell, RoomLayer.GroundLevel, position));
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.OutputHandler).Returns(outputHandler);
		return character;
	}

	private static Mock<IGameItem> CreateExterior(
		ICell cell,
		IFuturemud gameworld,
		double position,
		IOutputHandler outputHandler)
	{
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Name).Returns("room-scale vehicle");
		exterior.SetupGet(x => x.Location).Returns(cell);
		exterior.SetupGet(x => x.TrueLocations).Returns([cell]);
		exterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		exterior.SetupGet(x => x.RoutePositionMetres).Returns(position);
		exterior.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell, RoomLayer.GroundLevel, position));
		exterior.SetupGet(x => x.Gameworld).Returns(gameworld);
		exterior.SetupGet(x => x.OutputHandler).Returns(outputHandler);
		return exterior;
	}

	private sealed record RouteFixture(
		Mock<IFuturemud> Gameworld,
		Mock<ICell> Cell,
		List<IPerceivable> Occupants);
}
