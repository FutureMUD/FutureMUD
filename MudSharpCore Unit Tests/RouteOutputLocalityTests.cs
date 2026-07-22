#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RouteOutputLocalityTests
{
	[TestMethod]
	public void Handle_LocalOutputInRouteCell_UsesIndexedSpatialRange()
	{
		var fixture = CreateFixture();

		fixture.SourceHandler.Object.Handle(fixture.Output.Object, OutputRange.Local);

		fixture.SourceRecipient.Verify(x => x.Send(fixture.Output.Object, true, true), Times.Once);
		fixture.NearRecipient.Verify(x => x.Send(fixture.Output.Object, true, true), Times.Once);
		fixture.FarRecipient.Verify(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void Handle_RoomOutputInRouteCell_RemainsExplicitWholeCellBroadcast()
	{
		var fixture = CreateFixture();

		fixture.SourceHandler.Object.Handle(fixture.Output.Object, OutputRange.Room);

		fixture.SourceRecipient.Verify(x => x.Send(fixture.Output.Object, true, true), Times.Once);
		fixture.NearRecipient.Verify(x => x.Send(fixture.Output.Object, true, true), Times.Once);
		fixture.FarRecipient.Verify(x => x.Send(fixture.Output.Object, true, true), Times.Once);
	}

	[TestMethod]
	public void Handle_SourceLessLocalOutputInRouteCell_FailsClosedWithoutRoomEcho()
	{
		var fixture = CreateFixture();

		fixture.Cell.Object.Handle(fixture.Output.Object);
		fixture.Cell.Object.Handle("source-less route output");

		fixture.SourceRecipient.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Never);
		fixture.SourceRecipient.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
		fixture.NearRecipient.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Never);
		fixture.FarRecipient.Verify(
			x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()),
			Times.Never);
		fixture.Cell.Verify(x => x.HandleRoomEcho(It.IsAny<IEmoteOutput>(), It.IsAny<RoomLayer?>()), Times.Never);
		fixture.Cell.Verify(x => x.HandleRoomEcho(It.IsAny<string>(), It.IsAny<RoomLayer?>()), Times.Never);
	}

	[TestMethod]
	public void HandleLocal_ItemSourceInRouteCell_PreservesItsCoordinate()
	{
		var fixture = CreateFixture();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Location).Returns(fixture.Cell.Object);
		item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		item.SetupGet(x => x.RoutePositionMetres).Returns(100.0);
		item.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(fixture.Cell.Object, RoomLayer.GroundLevel, 100.0));
		item.SetupGet(x => x.Gameworld).Returns(fixture.Gameworld.Object);

		fixture.Cell.Object.HandleLocal(item.Object, RoomLayer.GroundLevel, "item route output");

		fixture.SourceRecipient.Verify(x => x.Send("item route output", true, false), Times.Once);
		fixture.NearRecipient.Verify(x => x.Send("item route output", true, false), Times.Once);
		fixture.FarRecipient.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void HandleLocal_RemoteObserversRequireSpatialAnchorButRoomBroadcastDoesNot()
	{
		var fixture = CreateFixture();
		var anchoredObserver = new Mock<IRemoteObservationEffect>();
		anchoredObserver.Setup(x => x.Observes(It.Is<SpatialLocation>(y => y.RoutePositionMetres == 100.0)))
			.Returns(true);
		var distantObserver = new Mock<IRemoteObservationEffect>();
		distantObserver.Setup(x => x.Observes(It.IsAny<SpatialLocation>())).Returns(false);
		fixture.Cell.SetupGet(x => x.Cells).Returns([fixture.Cell.Object]);
		fixture.Cell.Setup(x => x.EffectsOfType<IRemoteObservationEffect>(
				It.IsAny<Predicate<IRemoteObservationEffect>>()))
			.Returns([anchoredObserver.Object, distantObserver.Object]);

		fixture.Cell.Object.HandleLocal(
			fixture.SourceHandler.Object.Perceiver,
			RoomLayer.GroundLevel,
			fixture.Output.Object);

		anchoredObserver.Verify(x => x.HandleOutput(fixture.Output.Object, fixture.Cell.Object), Times.Once);
		distantObserver.Verify(x => x.HandleOutput(It.IsAny<IOutput>(), It.IsAny<ILocation>()), Times.Never);

		fixture.SourceHandler.Object.Handle(fixture.Output.Object, OutputRange.Room);

		anchoredObserver.Verify(x => x.HandleOutput(fixture.Output.Object, fixture.Cell.Object), Times.Exactly(2));
		distantObserver.Verify(x => x.HandleOutput(fixture.Output.Object, fixture.Cell.Object), Times.Once,
			"An explicit whole-RouteCell broadcast remains available to topology-wide observers.");
	}

	private static OutputFixture CreateFixture()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("RouteCellVeryDistantDistanceMetres")).Returns(500.0);

		var cell = new Mock<ICell>();
		var definition = new Mock<IRouteCellDefinition>();
		definition.SetupGet(x => x.Cell).Returns(cell.Object);
		definition.SetupGet(x => x.LengthMetres).Returns(2_000.0);
		definition.SetupGet(x => x.DefaultPositionMetres).Returns(0.0);
		definition.SetupGet(x => x.MetresPerRoomEquivalent).Returns(100.0);
		definition.SetupGet(x => x.Landmarks).Returns([]);
		definition.SetupGet(x => x.ExitAnchors).Returns([]);
		cell.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		cell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var sourceRecipient = new Mock<IOutputHandler>();
		var nearRecipient = new Mock<IOutputHandler>();
		var farRecipient = new Mock<IOutputHandler>();
		var source = CreateCharacter(cell.Object, gameworld.Object, 100.0, sourceRecipient.Object);
		var near = CreateCharacter(cell.Object, gameworld.Object, 550.0, nearRecipient.Object);
		var far = CreateCharacter(cell.Object, gameworld.Object, 650.001, farRecipient.Object);
		var occupants = new IPerceivable[] { source.Object, near.Object, far.Object };
		cell.SetupGet(x => x.Perceivables).Returns(occupants);
		cell.SetupGet(x => x.Characters).Returns(occupants.OfType<ICharacter>());
		cell.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns(occupants.OfType<ICharacter>());

		var sourceHandler = new Mock<IOutputHandler>();
		sourceHandler.SetupGet(x => x.Perceiver).Returns(source.Object);
		var output = new Mock<IOutput>();
		output.SetupGet(x => x.Style).Returns(OutputStyle.Normal);
		output.SetupGet(x => x.Flags).Returns(OutputFlags.Normal);
		output.Setup(x => x.ParseFor(It.IsAny<IPerceiver>()))
			.Returns("route output");

		return new OutputFixture(
			gameworld,
			cell,
			sourceHandler,
			output,
			sourceRecipient,
			nearRecipient,
			farRecipient);
	}

	private static Mock<ICharacter> CreateCharacter(
		ICell cell,
		IFuturemud gameworld,
		double position,
		IOutputHandler outputHandler)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Location).Returns(cell);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		character.SetupGet(x => x.RoutePositionMetres).Returns(position);
		character.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell, RoomLayer.GroundLevel, position));
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.OutputHandler).Returns(outputHandler);
		character.Setup(x => x.EffectsOfType<IRemoteObservationEffect>(
				It.IsAny<Predicate<IRemoteObservationEffect>>()))
			.Returns([]);
		return character;
	}

	private sealed record OutputFixture(
		Mock<IFuturemud> Gameworld,
		Mock<ICell> Cell,
		Mock<IOutputHandler> SourceHandler,
		Mock<IOutput> Output,
		Mock<IOutputHandler> SourceRecipient,
		Mock<IOutputHandler> NearRecipient,
		Mock<IOutputHandler> FarRecipient);
}
