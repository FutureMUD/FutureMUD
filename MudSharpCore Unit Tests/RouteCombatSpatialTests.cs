#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RouteCombatSpatialTests
{
	[TestMethod]
	public void AdvanceAlongRouteMove_MaterialisesPhysicalProgressTowardsTarget()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("RouteCellImmediateDistanceMetres")).Returns(3.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellProximateDistanceMetres")).Returns(10.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellDistantDistanceMetres")).Returns(100.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellVeryDistantDistanceMetres")).Returns(500.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellDefaultRoomEquivalentMetres")).Returns(100.0);

		var cell = new Mock<ICell>();
		var route = new Mock<IRouteCellDefinition>();
		route.SetupGet(x => x.Cell).Returns(cell.Object);
		route.SetupGet(x => x.LengthMetres).Returns(10_000.0);
		route.SetupGet(x => x.DefaultPositionMetres).Returns(0.0);
		route.SetupGet(x => x.MetresPerRoomEquivalent).Returns(100.0);
		route.SetupGet(x => x.Landmarks).Returns([]);
		route.SetupGet(x => x.ExitAnchors).Returns([]);
		cell.SetupGet(x => x.RouteDefinition).Returns(route.Object);

		var output = new Mock<IOutputHandler>();
		var target = new Mock<ICharacter>();
		target.SetupGet(x => x.Location).Returns(cell.Object);
		target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		target.SetupGet(x => x.RoutePositionMetres).Returns(200.0);
		target.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 200.0));
		target.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var assailant = new Mock<ICharacter>();
		assailant.SetupGet(x => x.Location).Returns(cell.Object);
		assailant.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		assailant.SetupGet(x => x.RoutePositionMetres).Returns(100.0);
		assailant.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 100.0));
		assailant.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		assailant.SetupGet(x => x.CombatTarget).Returns(target.Object);
		assailant.SetupGet(x => x.OutputHandler).Returns(output.Object);

		var move = new AdvanceAlongRouteMove { Assailant = assailant.Object };
		var result = move.ResolveMove(null!);

		Assert.IsTrue(result.MoveWasSuccessful);
		assailant.Verify(x => x.SetRoutePosition(It.Is<double?>(value =>
			value.HasValue && Math.Abs(value.Value - 103.0) < 0.000001)), Times.Once);
	}

	[TestMethod]
	public void TryEnterMelee_RouteCellRequiresPhysicalImmediateSeparation()
	{
		var (gameworld, cell) = CreateRouteCellFixture();
		var assailant = CreateRouteCharacter(gameworld.Object, cell.Object, 100.0);
		var target = CreateRouteCharacter(gameworld.Object, cell.Object, 200.0);

		Assert.IsFalse(RouteCombatMovementUtilities.TryEnterMelee(assailant.Object, target.Object));
		assailant.VerifySet(x => x.MeleeRange = true, Times.Never);
		target.VerifySet(x => x.MeleeRange = true, Times.Never);

		assailant.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 198.0));
		Assert.IsTrue(RouteCombatMovementUtilities.TryEnterMelee(assailant.Object, target.Object));
		assailant.VerifySet(x => x.MeleeRange = true, Times.Once);
		target.VerifySet(x => x.MeleeRange = true, Times.Once);
	}

	[TestMethod]
	public void TryRetreatAlongRoute_CreatesDurableSeparationWithoutAnExit()
	{
		var (gameworld, cell) = CreateRouteCellFixture();
		var moverPosition = 100.0;
		var mover = CreateRouteCharacter(gameworld.Object, cell.Object, moverPosition);
		mover.SetupGet(x => x.SpatialLocation).Returns(() =>
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, moverPosition));
		mover.Setup(x => x.SetRoutePosition(It.IsAny<double?>()))
			.Callback((double? value) => moverPosition = value!.Value);
		var pursuer = CreateRouteCharacter(gameworld.Object, cell.Object, 100.0);

		Assert.IsTrue(RouteCombatMovementUtilities.TryRetreatAlongRoute(mover.Object, [pursuer.Object]));
		Assert.IsTrue(Math.Abs(moverPosition - 100.0) > 3.0);
		Assert.IsTrue(RouteCombatMovementUtilities.ArePhysicallyImmediate(mover.Object, pursuer.Object) is false);
	}

	private static (Mock<IFuturemud> Gameworld, Mock<ICell> Cell) CreateRouteCellFixture()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("RouteCellImmediateDistanceMetres")).Returns(3.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellProximateDistanceMetres")).Returns(10.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellDistantDistanceMetres")).Returns(100.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellVeryDistantDistanceMetres")).Returns(500.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellDefaultRoomEquivalentMetres")).Returns(1_000.0);
		var cell = new Mock<ICell>();
		var route = new Mock<IRouteCellDefinition>();
		route.SetupGet(x => x.Cell).Returns(cell.Object);
		route.SetupGet(x => x.LengthMetres).Returns(10_000.0);
		route.SetupGet(x => x.DefaultPositionMetres).Returns(0.0);
		route.SetupGet(x => x.MetresPerRoomEquivalent).Returns(1_000.0);
		route.SetupGet(x => x.Landmarks).Returns([]);
		route.SetupGet(x => x.ExitAnchors).Returns([]);
		cell.SetupGet(x => x.RouteDefinition).Returns(route.Object);
		return (gameworld, cell);
	}

	private static Mock<ICharacter> CreateRouteCharacter(
		IFuturemud gameworld,
		ICell cell,
		double position)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.Location).Returns(cell);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		character.SetupGet(x => x.RoutePositionMetres).Returns(position);
		character.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell, RoomLayer.GroundLevel, position));
		return character;
	}
}
