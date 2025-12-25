#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaLifecycleServiceTests
{
        private ArenaLifecycleService _service = null!;
        private Mock<IArenaScheduler> _scheduler = null!;
        private Mock<IFuturemud> _gameworld = null!;

        [TestInitialize]
        public void Setup()
        {
                _gameworld = new Mock<IFuturemud>();
                _scheduler = new Mock<IArenaScheduler>();
                _service = new ArenaLifecycleService(_gameworld.Object);
                _service.AttachScheduler(_scheduler.Object);
        }

	[TestMethod]
	public void Transition_ForwardState_EnforcesAndSchedules()
	{
		var arenaEvent = BuildEvent(ArenaEventState.Scheduled);

		_service.Transition(arenaEvent.Object, ArenaEventState.RegistrationOpen);

		arenaEvent.Verify(x => x.OpenRegistration(), Times.Once);
		_scheduler.Verify(x => x.Schedule(arenaEvent.Object), Times.Once);
	}

        [TestMethod]
        public void Transition_BackwardOrSameState_NoAction()
        {
                var arenaEvent = BuildEvent(ArenaEventState.Preparing);

		_service.Transition(arenaEvent.Object, ArenaEventState.RegistrationOpen);
		_service.Transition(arenaEvent.Object, ArenaEventState.Preparing);

		arenaEvent.Verify(x => x.OpenRegistration(), Times.Never);
		arenaEvent.Verify(x => x.StartPreparation(), Times.Never);
		_scheduler.Verify(x => x.Schedule(It.IsAny<IArenaEvent>()), Times.Never);
	}

        [TestMethod]
        public void Transition_FinalStates_CancelsScheduling()
        {
		var arenaEvent = BuildEvent(ArenaEventState.Cleanup);

		_service.Transition(arenaEvent.Object, ArenaEventState.Completed);

		arenaEvent.Verify(x => x.Complete(), Times.Once);
		_scheduler.Verify(x => x.Cancel(arenaEvent.Object), Times.Once);
		_scheduler.Verify(x => x.Schedule(It.IsAny<IArenaEvent>()), Times.Never);
	}

        [TestMethod]
        public void Transition_Abort_CancelsScheduling()
        {
		var arenaEvent = BuildEvent(ArenaEventState.Live);

		_service.Transition(arenaEvent.Object, ArenaEventState.Aborted);

		arenaEvent.Verify(x => x.Abort(It.IsAny<string>()), Times.Once);
		_scheduler.Verify(x => x.Cancel(arenaEvent.Object), Times.Once);
		_scheduler.Verify(x => x.Schedule(It.IsAny<IArenaEvent>()), Times.Never);
	}

        [TestMethod]
        public void RebootRecovery_InvokesSchedulerRecovery()
        {
                _service.RebootRecovery();

                _scheduler.Verify(x => x.RecoverAfterReboot(), Times.Once);
        }

        [TestMethod]
        public void Transition_NullEvent_Throws()
        {
                Assert.ThrowsException<ArgumentNullException>(() => _service.Transition(null!, ArenaEventState.Live));
        }

        private static Mock<IArenaEvent> BuildEvent(ArenaEventState initialState)
        {
                var state = initialState;
                var arenaEvent = new Mock<IArenaEvent>();
                arenaEvent.SetupGet(x => x.State).Returns(() => state);
                arenaEvent.Setup(x => x.EnforceState(It.IsAny<ArenaEventState>()))
                        .Callback<ArenaEventState>(next => state = next);
                return arenaEvent;
        }
}
