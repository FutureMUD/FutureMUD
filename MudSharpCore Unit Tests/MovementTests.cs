#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MovementTests
{
    [TestMethod]
    public void FinalStep_SoloMoverWithQueuedCommand_ExecutesQueuedMove()
    {
        (Movement? movement, Mock<ICharacter>? mover, Mock<ICellExit> _, Mock<ICell>? destination, Queue<string>? queuedCommands) = CreateMovementWithMover();

        movement.FinalStep();

        Assert.AreEqual(0, queuedCommands.Count);
        Assert.IsNull(mover.Object.Movement);
        mover.Verify(x => x.Move("north"), Times.Once);
        destination.Verify(x => x.ResolveMovement(movement), Times.Once);
    }

    [TestMethod]
    public void FinalStep_PartyMemberMoverUsesOriginalMoverQueue()
    {
        Queue<string> leaderQueuedCommands = new(["east"]);
        Mock<ICharacter> leader = CreateMoverMock(leaderQueuedCommands);
        leader.Setup(x => x.Move(It.IsAny<string>())).Returns(true);

        Mock<IParty> party = new();
        party.SetupGet(x => x.Leader).Returns(leader.Object);
        party.SetupProperty(x => x.Movement);

        (Movement? movement, Mock<ICharacter>? mover, Mock<ICellExit> _, Mock<ICell> _, Queue<string>? queuedCommands) = CreateMovementWithMover(party.Object);
        party.Object.Movement = movement;

        movement.FinalStep();

        Assert.AreEqual(0, queuedCommands.Count);
        Assert.AreEqual(1, leaderQueuedCommands.Count);
        Assert.IsNull(party.Object.Movement);
        mover.Verify(x => x.Move("north"), Times.Once);
        leader.Verify(x => x.Move(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void FinalStep_ZeroGravityMoverStartsDrift()
    {
        (Movement? movement, Mock<ICharacter>? mover, Mock<ICellExit>? exit, Mock<ICell>? destination, Queue<string>? queuedCommands) = CreateMovementWithMover();
        queuedCommands.Clear();

        Mock<ITerrain> terrain = new();
        terrain.SetupGet(x => x.GravityModel).Returns(GravityModel.ZeroGravity);
        destination.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrain.Object);
        destination.Setup(x => x.EffectsOfType<IGravityOverrideEffect>(It.IsAny<Predicate<IGravityOverrideEffect>>()))
            .Returns(Enumerable.Empty<IGravityOverrideEffect>());
        destination.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(false);
        exit.SetupGet(x => x.OutboundDirection).Returns(CardinalDirection.North);
        mover.SetupGet(x => x.Location).Returns(destination.Object);
        mover.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
        mover.Setup(x => x.EffectsOfType<ZeroGravityDrift>(It.IsAny<Predicate<ZeroGravityDrift>>()))
            .Returns(Enumerable.Empty<ZeroGravityDrift>());

        movement.FinalStep();

        mover.Verify(x => x.AddEffect(It.IsAny<ZeroGravityDrift>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    private static (Movement Movement, Mock<ICharacter> Mover, Mock<ICellExit> Exit, Mock<ICell> Destination, Queue<string> QueuedCommands) CreateMovementWithMover(IParty? party = null)
    {
        Mock<ICell> destination = new();
        destination.Setup(x => x.ResolveMovement(It.IsAny<IMovement>()));

        Mock<ICellExit> exit = new();
        exit.SetupGet(x => x.Destination).Returns(destination.Object);

        Queue<string> queuedCommands = new(["north"]);
        Mock<ICharacter> mover = CreateMoverMock(queuedCommands);

        Movement movement = new(
            mover.Object,
            party!,
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
        Mock<IFuturemud> gameworld = new();
        gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(1000.0);

        Mock<ICharacter> mover = new();
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
