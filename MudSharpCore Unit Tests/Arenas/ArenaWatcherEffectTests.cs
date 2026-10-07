#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaWatcherEffectTests
{
    private Mock<IFuturemud> _gameworld = null!;
    private Mock<IRoom> _arenaRoom = null!;
    private Mock<IArenaEvent> _arenaEvent = null!;
    private Mock<ICombatArena> _arena = null!;
    private Mock<IOutput> _output = null!;
    private ArenaWatcherEffect _effect = null!;

    [TestInitialize]
    public void Setup()
    {
        _gameworld = new Mock<IFuturemud>();
        _arenaRoom = new Mock<IRoom>();
        _arenaRoom.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
        _arenaRoom.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), false));
        _arenaRoom.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
                It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("the arena floor");

        _arena = new Mock<ICombatArena>();
        _arena.SetupGet(x => x.Name).Returns("The Pit");

        _arenaEvent = new Mock<IArenaEvent>();
        _arenaEvent.SetupGet(x => x.Arena).Returns(_arena.Object);
        _arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Live);

        _output = new Mock<IOutput>();
        _output.SetupGet(x => x.Flags).Returns(OutputFlags.Normal);
        _output.Setup(x => x.ShouldSee(It.IsAny<ICharacter>())).Returns(true);

        _effect = new ArenaWatcherEffect(_arenaRoom.Object, _arenaEvent.Object);
    }

    [TestMethod]
    public void HandleOutput_WatcherMovesWithinObservationRooms_RetainsSubscription()
    {
        Mock<IRoom> observationRoom1 = new();
        observationRoom1.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
        Mock<IRoom> observationRoom2 = new();
        observationRoom2.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
        _arena.Setup(x => x.ObservationRooms).Returns(new[]
        {
                        observationRoom1.Object,
                        observationRoom2.Object
                });

        Mock<IOutputHandler> outputHandler = new();
        outputHandler.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
        Mock<ICharacter> watcher = new();
        watcher.SetupGet(x => x.State).Returns(CharacterState.Conscious);
        watcher.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
        watcher.Setup(x => x.GetHashCode()).Returns(17);
        watcher.Setup(x => x.Equals(It.IsAny<object>()))
            .Returns<object>(obj => ReferenceEquals(obj, watcher.Object));
        watcher.SetupSequence(x => x.Location)
            .Returns(observationRoom1.Object)
            .Returns(observationRoom1.Object)
            .Returns(observationRoom2.Object);

        _effect.AddWatcher(watcher.Object, observationRoom1.Object);
        int sendCountBefore = outputHandler.Invocations.Count(x => x.Method.Name == "Send");
        _effect.HandleOutput(_output.Object, _arenaRoom.Object);
        int sendCountAfterFirst = outputHandler.Invocations.Count(x => x.Method.Name == "Send");
        _effect.HandleOutput(_output.Object, _arenaRoom.Object);
        int sendCountAfterSecond = outputHandler.Invocations.Count(x => x.Method.Name == "Send");

        Assert.IsTrue(sendCountAfterFirst > sendCountBefore);
        Assert.IsTrue(sendCountAfterSecond > sendCountAfterFirst);
        _arenaRoom.Verify(x => x.RemoveEffect(_effect, false), Times.Never());
    }

    [TestMethod]
    public void HandleOutput_CompletedEvent_RemovesEffect()
    {
        _arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Completed);
        _effect.HandleOutput(_output.Object, _arenaRoom.Object);

        _arenaRoom.Verify(x => x.RemoveEffect(_effect, false), Times.Once());
    }

    [TestMethod]
    public void HandleOutput_AwakeWatcher_StillReceivesMirroredOutput()
    {
        Mock<IRoom> observationRoom = new();
        observationRoom.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
        _arena.Setup(x => x.ObservationRooms).Returns(new[] { observationRoom.Object });

        Mock<IOutputHandler> outputHandler = new();
        outputHandler.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
        Mock<ICharacter> watcher = new();
        watcher.SetupGet(x => x.State).Returns(CharacterState.Awake);
        watcher.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
        watcher.SetupGet(x => x.Location).Returns(observationRoom.Object);
        watcher.Setup(x => x.GetHashCode()).Returns(37);
        watcher.Setup(x => x.Equals(It.IsAny<object>()))
            .Returns<object>(obj => ReferenceEquals(obj, watcher.Object));

        _effect.AddWatcher(watcher.Object, observationRoom.Object);
        _effect.HandleOutput(_output.Object, _arenaRoom.Object);

        outputHandler.Verify(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
    }

    [TestMethod]
    public void HandleOutput_StringEcho_WatcherReceivesPrefixedText()
    {
        Mock<IRoom> observationRoom = new();
        observationRoom.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
        _arena.Setup(x => x.ObservationRooms).Returns(new[] { observationRoom.Object });

        Mock<IOutputHandler> outputHandler = new();
        outputHandler.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
        Mock<ICharacter> watcher = new();
        watcher.SetupGet(x => x.State).Returns(CharacterState.Awake);
        watcher.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
        watcher.SetupGet(x => x.Location).Returns(observationRoom.Object);
        watcher.Setup(x => x.GetHashCode()).Returns(57);
        watcher.Setup(x => x.Equals(It.IsAny<object>()))
            .Returns<object>(obj => ReferenceEquals(obj, watcher.Object));

        _effect.AddWatcher(watcher.Object, observationRoom.Object);
        _effect.HandleOutput("A final blow lands!", _arenaRoom.Object);

        outputHandler.Verify(x => x.Send(It.Is<string>(text => text.Contains("A final blow lands!")),
            It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
    }
}
