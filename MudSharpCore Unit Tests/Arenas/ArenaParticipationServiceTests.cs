#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaParticipationServiceTests
{
        private ArenaParticipationService _service = null!;
        private Mock<IFuturemud> _gameworld = null!;

        [TestInitialize]
        public void Setup()
        {
                _gameworld = new Mock<IFuturemud>();
                _service = new ArenaParticipationService(_gameworld.Object);
        }

        [TestMethod]
        public void EnsureParticipation_AddsEffectAndRemovesLinkdead()
        {
                var participant = new Mock<ICharacter>();
                participant.SetupGet(x => x.IsPlayerCharacter).Returns(true);
                participant.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
                participant.Setup(x => x.CombinedEffectsOfType<ArenaParticipationEffect>())
                        .Returns(Array.Empty<ArenaParticipationEffect>());

                Predicate<IEffect>? predicate = null;
                participant.Setup(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()))
                        .Callback<Predicate<IEffect>, bool>((pred, _) => predicate = pred);

                IEffect? capturedEffect = null;
                participant.Setup(x => x.AddEffect(It.IsAny<IEffect>()))
                        .Callback<IEffect>(effect => capturedEffect = effect);

                var arenaEvent = new Mock<IArenaEvent>();
                arenaEvent.SetupGet(x => x.Id).Returns(42);
                arenaEvent.SetupGet(x => x.Name).Returns("Test Event");

                _service.EnsureParticipation(participant.Object, arenaEvent.Object);

                participant.Verify(x => x.AddEffect(It.IsAny<ArenaParticipationEffect>()), Times.Once);
                participant.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()), Times.Once);
                Assert.IsNotNull(capturedEffect);
                Assert.IsInstanceOfType(capturedEffect, typeof(ArenaParticipationEffect));
                Assert.IsNotNull(predicate, "Expected Linkdead cleanup predicate to be provided.");
                var linkdead = new LinkdeadLogout(participant.Object);
                Assert.IsTrue(predicate!(linkdead), "Predicate should match linkdead effects.");
        }

        [TestMethod]
        public void EnsureParticipation_WhenEffectExists_DoesNotDuplicate()
        {
                var participant = new Mock<ICharacter>();
                participant.SetupGet(x => x.IsPlayerCharacter).Returns(true);
                participant.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);

                var arenaEvent = new Mock<IArenaEvent>();
                arenaEvent.SetupGet(x => x.Id).Returns(99);
                arenaEvent.SetupGet(x => x.Name).Returns("Existing Event");

                var existingEffect = new ArenaParticipationEffect(participant.Object, arenaEvent.Object);
                participant.Setup(x => x.CombinedEffectsOfType<ArenaParticipationEffect>())
                        .Returns(new[] { existingEffect });

                _service.EnsureParticipation(participant.Object, arenaEvent.Object);

                participant.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Never);
                participant.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void ClearParticipation_RemovesEffect()
        {
                var participant = new Mock<ICharacter>();
                participant.SetupGet(x => x.IsPlayerCharacter).Returns(true);
                participant.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);

                var arenaEvent = new Mock<IArenaEvent>();
                arenaEvent.SetupGet(x => x.Id).Returns(7);
                arenaEvent.SetupGet(x => x.Name).Returns("Cleanup Event");

                var effect = new ArenaParticipationEffect(participant.Object, arenaEvent.Object);
                participant.Setup(x => x.CombinedEffectsOfType<ArenaParticipationEffect>())
                        .Returns(new[] { effect });

                _service.ClearParticipation(participant.Object, arenaEvent.Object);

                participant.Verify(x => x.RemoveEffect(effect, true), Times.Once);
        }

        [TestMethod]
        public void HasParticipation_ReturnsTrueWhenEffectPresent()
        {
                var participant = new Mock<ICharacter>();
                participant.SetupGet(x => x.IsPlayerCharacter).Returns(true);
                participant.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);

                var arenaEvent = new Mock<IArenaEvent>();
                arenaEvent.SetupGet(x => x.Id).Returns(13);
                arenaEvent.SetupGet(x => x.Name).Returns("Status Event");

                var effect = new ArenaParticipationEffect(participant.Object, arenaEvent.Object);
                participant.Setup(x => x.CombinedEffectsOfType<ArenaParticipationEffect>())
                        .Returns(new[] { effect });

                var result = _service.HasParticipation(participant.Object, arenaEvent.Object);

                Assert.IsTrue(result);
        }
}
