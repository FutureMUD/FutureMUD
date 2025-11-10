#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;

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
		var observer = new Mock<ICharacter>();
		observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
		var observationCell = new Mock<ICell>();
		var otherCell = new Mock<ICell>();
		observer.SetupGet(x => x.Location).Returns(otherCell.Object);
		var arena = new Mock<ICombatArena>();
		arena.Setup(x => x.ObservationCells).Returns(new[] { observationCell.Object });
		var arenaEvent = new Mock<IArenaEvent>();
		arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
		arenaEvent.Setup(x => x.State).Returns(ArenaEventState.Live);
		arenaEvent.Setup(x => x.Participants).Returns(Array.Empty<IArenaParticipant>());
		var result = _service.CanObserve(observer.Object, arenaEvent.Object);
		Assert.IsFalse(result.Truth);
		StringAssert.Contains(result.Reason, "observation rooms");
	}

	[TestMethod]
	public void CanObserve_WhenEventNotStaged_ReturnsFalse()
	{
		var observer = new Mock<ICharacter>();
		observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
		var observationCell = new Mock<ICell>();
		observer.SetupGet(x => x.Location).Returns(observationCell.Object);
		var arena = new Mock<ICombatArena>();
		arena.Setup(x => x.ObservationCells).Returns(new[] { observationCell.Object });
		var arenaEvent = new Mock<IArenaEvent>();
		arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
		arenaEvent.Setup(x => x.State).Returns(ArenaEventState.Scheduled);
		arenaEvent.Setup(x => x.Participants).Returns(Array.Empty<IArenaParticipant>());
		var result = _service.CanObserve(observer.Object, arenaEvent.Object);
		Assert.IsFalse(result.Truth);
		StringAssert.Contains(result.Reason, "not been staged");
	}

	[TestMethod]
	public void CanObserve_WhenObserverIsParticipant_ReturnsFalse()
	{
		var observer = new Mock<ICharacter>();
		observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
		var observationCell = new Mock<ICell>();
		observer.SetupGet(x => x.Location).Returns(observationCell.Object);
		var arena = new Mock<ICombatArena>();
		arena.Setup(x => x.ObservationCells).Returns(new[] { observationCell.Object });
		var participant = new Mock<IArenaParticipant>();
		participant.SetupGet(x => x.Character).Returns(observer.Object);
		var arenaEvent = new Mock<IArenaEvent>();
		arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
		arenaEvent.Setup(x => x.State).Returns(ArenaEventState.Live);
		arenaEvent.Setup(x => x.Participants).Returns(new[] { participant.Object });
		var result = _service.CanObserve(observer.Object, arenaEvent.Object);
		Assert.IsFalse(result.Truth);
		StringAssert.Contains(result.Reason, "Participants");
	}

	[TestMethod]
	public void CanObserve_WhenNoObservationRoomsConfigured_ReturnsFalse()
	{
		var observer = new Mock<ICharacter>();
		observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
		var observationCell = new Mock<ICell>();
		observer.SetupGet(x => x.Location).Returns(observationCell.Object);
		var arena = new Mock<ICombatArena>();
		arena.Setup(x => x.Name).Returns("Test Arena");
		arena.Setup(x => x.ObservationCells).Returns(Array.Empty<ICell>());
		var arenaEvent = new Mock<IArenaEvent>();
		arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
		arenaEvent.Setup(x => x.State).Returns(ArenaEventState.Live);
		arenaEvent.Setup(x => x.Participants).Returns(Array.Empty<IArenaParticipant>());
		var result = _service.CanObserve(observer.Object, arenaEvent.Object);
		Assert.IsFalse(result.Truth);
		StringAssert.Contains(result.Reason, "does not have any observation rooms configured");
	}

	[TestMethod]
	public void CanObserve_WhenObserverIsInObservationCell_ReturnsTrue()
	{
		var observer = new Mock<ICharacter>();
		observer.SetupGet(x => x.State).Returns(CharacterState.Conscious);
		var observationCell = new Mock<ICell>();
		observer.SetupGet(x => x.Location).Returns(observationCell.Object);
		var arena = new Mock<ICombatArena>();
		arena.Setup(x => x.ObservationCells).Returns(new[] { observationCell.Object });
		var arenaEvent = new Mock<IArenaEvent>();
		arenaEvent.Setup(x => x.Arena).Returns(arena.Object);
		arenaEvent.Setup(x => x.State).Returns(ArenaEventState.Live);
		arenaEvent.Setup(x => x.Participants).Returns(Array.Empty<IArenaParticipant>());
		var result = _service.CanObserve(observer.Object, arenaEvent.Object);
		Assert.IsTrue(result.Truth);
		Assert.AreEqual(string.Empty, result.Reason);
	}
}
