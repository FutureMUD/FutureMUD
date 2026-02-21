#nullable enable

using System;
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

				var outputHandler = new Mock<IOutputHandler>();
				outputHandler.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
				var watcher = new Mock<ICharacter>();
				watcher.SetupGet(x => x.State).Returns(CharacterState.Conscious);
				watcher.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
				watcher.Setup(x => x.GetHashCode()).Returns(17);
				watcher.Setup(x => x.Equals(It.IsAny<object>()))
					.Returns<object>(obj => ReferenceEquals(obj, watcher.Object));
				watcher.SetupSequence(x => x.Location)
					.Returns(observationCell1.Object)
					.Returns(observationCell1.Object)
					.Returns(observationCell2.Object);

				_effect.AddWatcher(watcher.Object, observationCell1.Object);
				var sendCountBefore = outputHandler.Invocations.Count(x => x.Method.Name == "Send");
				_effect.HandleOutput(_output.Object, _arenaCell.Object);
				var sendCountAfterFirst = outputHandler.Invocations.Count(x => x.Method.Name == "Send");
				_effect.HandleOutput(_output.Object, _arenaCell.Object);
				var sendCountAfterSecond = outputHandler.Invocations.Count(x => x.Method.Name == "Send");

				Assert.IsTrue(sendCountAfterFirst > sendCountBefore);
				Assert.IsTrue(sendCountAfterSecond > sendCountAfterFirst);
				_arenaCell.Verify(x => x.RemoveEffect(_effect, false), Times.Never());
		}

		[TestMethod]
		public void HandleOutput_CompletedEvent_RemovesEffect()
		{
				_arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Completed);
				_effect.HandleOutput(_output.Object, _arenaCell.Object);

				_arenaCell.Verify(x => x.RemoveEffect(_effect, false), Times.Once());
		}
}
