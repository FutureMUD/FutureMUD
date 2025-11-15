#nullable enable

using System.Linq;
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

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaWatcherEffectTests
{
        private Mock<IFuturemud> _gameworld = null!;
        private Mock<ICell> _arenaCell = null!;
        private Mock<IArenaEvent> _arenaEvent = null!;
        private Mock<ICombatArena> _arena = null!;
        private Mock<IOutput> _output = null!;
        private ArenaWatcherEffect _effect = null!;

        [TestInitialize]
        public void Setup()
        {
                _gameworld = new Mock<IFuturemud>();
                _arenaCell = new Mock<ICell>();
                _arenaCell.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
                _arenaCell.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), false));
                _arenaCell.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
                        It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("the arena floor");

                _arena = new Mock<ICombatArena>();
                _arena.SetupGet(x => x.Name).Returns("The Pit");

                _arenaEvent = new Mock<IArenaEvent>();
                _arenaEvent.SetupGet(x => x.Arena).Returns(_arena.Object);
                _arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Live);

                _output = new Mock<IOutput>();
                _output.SetupGet(x => x.Flags).Returns(OutputFlags.Normal);
                _output.Setup(x => x.ShouldSee(It.IsAny<ICharacter>())).Returns(true);

                _effect = new ArenaWatcherEffect(_arenaCell.Object, _arenaEvent.Object);
        }

        [TestMethod]
        public void HandleOutput_WatcherMovesWithinObservationRooms_RetainsSubscription()
        {
                var observationCell1 = new Mock<ICell>();
                observationCell1.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
                var observationCell2 = new Mock<ICell>();
                observationCell2.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
                _arena.Setup(x => x.ObservationCells).Returns(new[]
                {
                        observationCell1.Object,
                        observationCell2.Object
                });

                var watcherLocation = observationCell1.Object as ICell;
                var outputHandler = new Mock<IOutputHandler>();
                var watcher = new Mock<ICharacter>();
                watcher.SetupGet(x => x.State).Returns(CharacterState.Conscious);
                watcher.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
                watcher.SetupGet(x => x.Location).Returns(() => watcherLocation);

                _effect.AddWatcher(watcher.Object, observationCell1.Object);

                _effect.HandleOutput(_output.Object, _arenaCell.Object);
                outputHandler.Invocations.Clear();

                watcherLocation = observationCell2.Object;
                _effect.HandleOutput(_output.Object, _arenaCell.Object);

                Assert.AreEqual(1, outputHandler.Invocations.Count);
                Assert.AreEqual("Send", outputHandler.Invocations.Single().Method.Name);
        }

        [TestMethod]
        public void HandleOutput_WatcherLeavesObservationRooms_RemovesSubscription()
        {
                var observationCell = new Mock<ICell>();
                observationCell.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
                var otherCell = new Mock<ICell>();
                otherCell.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
                _arena.Setup(x => x.ObservationCells).Returns(new[] { observationCell.Object });

                var watcherLocation = observationCell.Object as ICell;
                var outputHandler = new Mock<IOutputHandler>();
                var watcher = new Mock<ICharacter>();
                watcher.SetupGet(x => x.State).Returns(CharacterState.Conscious);
                watcher.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
                watcher.SetupGet(x => x.Location).Returns(() => watcherLocation);

                _effect.AddWatcher(watcher.Object, observationCell.Object);

                watcherLocation = otherCell.Object;
                _effect.HandleOutput(_output.Object, _arenaCell.Object);

                outputHandler.Verify(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never());
                _arenaCell.Verify(x => x.RemoveEffect(_effect, false), Times.Once());
        }
}
