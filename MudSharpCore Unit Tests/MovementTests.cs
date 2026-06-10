#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
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
using System.IO;
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
    public void CharacterMove_MountedRiderDelegatesToMountAuthorizationBeforeCreatingMovement()
    {
        string movementSource = File.ReadAllText(GetCoreSourcePath("Character", "CharacterMovement.cs"));
        int moveStart = movementSource.IndexOf("public bool Move(ICellExit exit", StringComparison.Ordinal);
        int staminaStart = movementSource.IndexOf("protected double StaminaForMovement", StringComparison.Ordinal);

        Assert.IsTrue(moveStart >= 0, "Character.Move(ICellExit) should exist.");
        Assert.IsTrue(staminaStart > moveStart, "Character.Move(ICellExit) should appear before StaminaForMovement.");

        string moveMethod = movementSource[moveStart..staminaStart];
        int mountedGuard = moveMethod.IndexOf("if (RidingMount is not null)", StringComparison.Ordinal);
        int riderMove = moveMethod.IndexOf("return RidingMount.RiderMove(exit, this, emote, ignoreSafeMovement);", StringComparison.Ordinal);
        int createMovement = moveMethod.IndexOf("Movement.CreateMovement(this, exit, emote, ignoreSafeMovement)", StringComparison.Ordinal);

        Assert.IsTrue(mountedGuard >= 0, "Mounted riders must be handled before generic movement creation.");
        Assert.IsTrue(riderMove > mountedGuard, "Mounted rider movement must delegate through RiderMove.");
        Assert.IsTrue(createMovement > riderMove, "RiderMove authorization must happen before creating a movement group.");
    }

    [TestMethod]
    public void EvaluateCharacterForAdditionToMovement_MountedRiderKeepsRiderInMovementSet()
    {
        var exit = new Mock<ICellExit>();
        var rider = CreateMountedCharacterMock("rider");
        var mount = CreateMountedCharacterMock("mount");
        rider.SetupGet(x => x.RidingMount).Returns(mount.Object);
        rider.SetupGet(x => x.Riders).Returns([]);
        mount.SetupGet(x => x.RidingMount).Returns((ICharacter?)null);
        mount.SetupGet(x => x.Riders).Returns([rider.Object]);
        mount.Setup(x => x.IsPrimaryRider(rider.Object)).Returns(true);
        mount.Setup(x => x.PermitControl(rider.Object)).Returns(true);
        mount.Setup(x => x.ControlMountDifficulty(rider.Object)).Returns(MudSharp.RPG.Checks.Difficulty.Automatic);
        mount.Setup(x => x.CanMove(exit.Object, CanMoveFlags.None)).Returns(CanMoveResponse.True);

        var considered = new List<ICharacter>();
        var nonDraggers = new List<ICharacter>();
        var mounts = new List<ICharacter>();
        var draggers = new List<ICharacter>();
        var helpers = new List<ICharacter>();
        var nonConsensual = new List<ICharacter>();
        var targets = new List<IPerceivable>();
        var dragEffects = new List<Dragging>();

        var result = Movement.EvaluateCharacterForAdditionToMovement(null!, rider.Object, exit.Object, considered,
            nonDraggers, mounts, draggers, helpers, nonConsensual, targets, dragEffects, false, false);

        Assert.IsTrue(result);
        CollectionAssert.Contains(nonDraggers, rider.Object);
        CollectionAssert.Contains(nonDraggers, mount.Object);
        CollectionAssert.Contains(mounts, mount.Object);
        CollectionAssert.Contains(nonConsensual, rider.Object);
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

    private static string GetCoreSourcePath(params string[] segments)
    {
        return Path.GetFullPath(Path.Combine(
            new[]
            {
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "MudSharpCore"
            }.Concat(segments).ToArray()));
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

    private static Mock<ICharacter> CreateMountedCharacterMock(string name)
    {
        var body = new Mock<IBody>();
        body.SetupGet(x => x.ExternalItems).Returns([]);
        body.SetupGet(x => x.ExternalItemsForOtherActors).Returns([]);

        var character = new Mock<ICharacter>();
        character.SetupGet(x => x.Name).Returns(name);
        character.SetupGet(x => x.Body).Returns(body.Object);
        character.SetupGet(x => x.IsEngagedInMelee).Returns(false);
        character.Setup(x => x.EffectsOfType<Dragging>()).Returns([]);
        character.Setup(x => x.Equals(It.IsAny<ICharacter>()))
            .Returns<ICharacter>(other => ReferenceEquals(other, character.Object));
        return character;
    }
}
