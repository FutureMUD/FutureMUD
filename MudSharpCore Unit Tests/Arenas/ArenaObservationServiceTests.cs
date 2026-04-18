#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using System;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaObservationServiceTests
{
    private ArenaObservationService _service = null!;
    private Mock<IFuturemud> _gameworld = null!;

    [TestInitialize]
    public void Setup()
    {
        _gameworld = new Mock<IFuturemud>();
        _service = new ArenaObservationService(_gameworld.Object);
    }

    [TestMethod]
    public void CanObserve_WhenNotInObservationCell_ReturnsFalse()
    {
        Mock<ICharacter> observer = new();
        observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
        Mock<ICell> observationCell = new();
        Mock<ICell> otherCell = new();
        observer.SetupGet(x => x.Location).Returns(otherCell.Object);
        Mock<ICombatArena> arena = new();
        arena.Setup(x => x.ObservationCells).Returns(new[] { observationCell.Object });
        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
        arenaEvent.Setup(x => x.State).Returns(ArenaEventState.Live);
        arenaEvent.Setup(x => x.Participants).Returns(Array.Empty<IArenaParticipant>());
        (bool Truth, string Reason) result = _service.CanObserve(observer.Object, arenaEvent.Object);
        Assert.IsFalse(result.Truth);
        StringAssert.Contains(result.Reason, "observation rooms");
    }

    [TestMethod]
    public void CanObserve_WhenRegistrationNotOpen_ReturnsFalse()
    {
        Mock<ICharacter> observer = new();
        observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
        Mock<ICell> observationCell = new();
        observer.SetupGet(x => x.Location).Returns(observationCell.Object);
        Mock<ICombatArena> arena = new();
        arena.Setup(x => x.ObservationCells).Returns(new[] { observationCell.Object });
        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
        arenaEvent.Setup(x => x.State).Returns(ArenaEventState.Scheduled);
        arenaEvent.Setup(x => x.Participants).Returns(Array.Empty<IArenaParticipant>());
        (bool Truth, string Reason) result = _service.CanObserve(observer.Object, arenaEvent.Object);
        Assert.IsFalse(result.Truth);
        StringAssert.Contains(result.Reason, "Registration has not opened");
    }

    [TestMethod]
    public void CanObserve_WhenObserverIsParticipant_ReturnsFalse()
    {
        Mock<ICharacter> observer = new();
        observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
        Mock<ICell> observationCell = new();
        observer.SetupGet(x => x.Location).Returns(observationCell.Object);
        Mock<ICombatArena> arena = new();
        arena.Setup(x => x.ObservationCells).Returns(new[] { observationCell.Object });
        Mock<IArenaParticipant> participant = new();
        participant.SetupGet(x => x.Character).Returns(observer.Object);
        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
        arenaEvent.Setup(x => x.State).Returns(ArenaEventState.Live);
        arenaEvent.Setup(x => x.Participants).Returns(new[] { participant.Object });
        (bool Truth, string Reason) result = _service.CanObserve(observer.Object, arenaEvent.Object);
        Assert.IsFalse(result.Truth);
        StringAssert.Contains(result.Reason, "Participants");
    }

    [TestMethod]
    public void CanObserve_WhenNoObservationRoomsConfigured_ReturnsFalse()
    {
        Mock<ICharacter> observer = new();
        observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
        Mock<ICell> observationCell = new();
        observer.SetupGet(x => x.Location).Returns(observationCell.Object);
        Mock<ICombatArena> arena = new();
        arena.Setup(x => x.Name).Returns("Test Arena");
        arena.Setup(x => x.ObservationCells).Returns(Array.Empty<ICell>());
        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
        arenaEvent.Setup(x => x.State).Returns(ArenaEventState.Live);
        arenaEvent.Setup(x => x.Participants).Returns(Array.Empty<IArenaParticipant>());
        (bool Truth, string Reason) result = _service.CanObserve(observer.Object, arenaEvent.Object);
        Assert.IsFalse(result.Truth);
        StringAssert.Contains(result.Reason, "does not have any observation rooms configured");
    }

    [TestMethod]
    public void CanObserve_WhenObserverIsInObservationCell_ReturnsTrue()
    {
        Mock<ICharacter> observer = new();
        observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
        Mock<ICell> observationCell = new();
        observer.SetupGet(x => x.Location).Returns(observationCell.Object);
        Mock<ICombatArena> arena = new();
        arena.Setup(x => x.ObservationCells).Returns(new[] { observationCell.Object });
        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
        arenaEvent.Setup(x => x.State).Returns(ArenaEventState.RegistrationOpen);
        arenaEvent.Setup(x => x.Participants).Returns(Array.Empty<IArenaParticipant>());
        (bool Truth, string Reason) result = _service.CanObserve(observer.Object, arenaEvent.Object);
        Assert.IsTrue(result.Truth);
        Assert.AreEqual(string.Empty, result.Reason);
    }

    [TestMethod]
    public void CanObserve_WhenPreparingAndInObservationCell_ReturnsTrue()
    {
        Mock<ICharacter> observer = new();
        observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
        Mock<ICell> observationCell = new();
        observer.SetupGet(x => x.Location).Returns(observationCell.Object);
        Mock<ICombatArena> arena = new();
        arena.Setup(x => x.ObservationCells).Returns(new[] { observationCell.Object });
        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
        arenaEvent.Setup(x => x.State).Returns(ArenaEventState.Preparing);
        arenaEvent.Setup(x => x.Participants).Returns(Array.Empty<IArenaParticipant>());
        (bool Truth, string Reason) result = _service.CanObserve(observer.Object, arenaEvent.Object);
        Assert.IsTrue(result.Truth);
        Assert.AreEqual(string.Empty, result.Reason);
    }
}
