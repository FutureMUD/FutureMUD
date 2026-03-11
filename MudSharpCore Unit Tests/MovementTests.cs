#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MovementTests
{
	[TestMethod]
	public void FinalStep_SoloMoverWithQueuedCommand_ExecutesQueuedMove()
	{
		var (movement, mover, _, destination, queuedCommands) = CreateMovementWithMover();

		movement.FinalStep();

		Assert.AreEqual(0, queuedCommands.Count);
		Assert.IsNull(mover.Object.Movement);
		mover.Verify(x => x.Move("north"), Times.Once);
		destination.Verify(x => x.ResolveMovement(movement), Times.Once);
	}

	[TestMethod]
	public void FinalStep_PartyMemberMoverUsesOriginalMoverQueue()
	{
		var leaderQueuedCommands = new Queue<string>(["east"]);
		var leader = CreateMoverMock(leaderQueuedCommands);
		leader.Setup(x => x.Move(It.IsAny<string>())).Returns(true);

		var party = new Mock<IParty>();
		party.SetupGet(x => x.Leader).Returns(leader.Object);
		party.SetupProperty(x => x.Movement);

		var (movement, mover, _, _, queuedCommands) = CreateMovementWithMover(party.Object);
		party.Object.Movement = movement;

		movement.FinalStep();

		Assert.AreEqual(0, queuedCommands.Count);
		Assert.AreEqual(1, leaderQueuedCommands.Count);
		Assert.IsNull(party.Object.Movement);
		mover.Verify(x => x.Move("north"), Times.Once);
		leader.Verify(x => x.Move(It.IsAny<string>()), Times.Never);
	}

	private static (Movement Movement, Mock<ICharacter> Mover, Mock<ICellExit> Exit, Mock<ICell> Destination, Queue<string> QueuedCommands) CreateMovementWithMover(IParty? party = null)
	{
		var destination = new Mock<ICell>();
		destination.Setup(x => x.ResolveMovement(It.IsAny<IMovement>()));

		var exit = new Mock<ICellExit>();
		exit.SetupGet(x => x.Destination).Returns(destination.Object);

		var queuedCommands = new Queue<string>(["north"]);
		var mover = CreateMoverMock(queuedCommands);

		var movement = new Movement(
			mover.Object,
			party,
			Enumerable.Empty<ICharacter>(),
			Enumerable.Empty<ICharacter>(),
			[mover.Object],
			Enumerable.Empty<ICharacter>(),
			Enumerable.Empty<IPerceivable>(),
			Enumerable.Empty<Dragging>(),
			exit.Object);

		mover.Object.Movement = movement;
		return (movement, mover, exit, destination, queuedCommands);
	}

	private static Mock<ICharacter> CreateMoverMock(Queue<string> queuedCommands)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(1000.0);

		var mover = new Mock<ICharacter>();
		mover.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		mover.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		mover.Setup(x => x.EffectsOfType<ISneakEffect>(It.IsAny<Predicate<ISneakEffect>>()))
			.Returns(Enumerable.Empty<ISneakEffect>());
		mover.Setup(x => x.AffectedBy<Immwalk>()).Returns(false);
		mover.Setup(x => x.MoveSpeed(It.IsAny<ICellExit>())).Returns(0.0);
		mover.SetupGet(x => x.RidingMount).Returns((ICharacter?)null);
		mover.SetupGet(x => x.QueuedMoveCommands).Returns(queuedCommands);
		mover.SetupProperty(x => x.Movement);
		mover.Setup(x => x.Move(It.IsAny<string>())).Returns(true);
		return mover;
	}
}
