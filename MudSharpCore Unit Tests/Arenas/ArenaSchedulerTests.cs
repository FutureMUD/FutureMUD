#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaSchedulerTests
{
        private ArenaScheduler _service = null!;
        private Mock<IFuturemud> _gameworld = null!;
        private Mock<IScheduler> _scheduler = null!;
        private Mock<IArenaLifecycleService> _lifecycle = null!;

        [TestInitialize]
        public void Setup()
        {
                _gameworld = new Mock<IFuturemud>();
                _scheduler = new Mock<IScheduler>();
                _lifecycle = new Mock<IArenaLifecycleService>();
                _gameworld.SetupGet(x => x.Scheduler).Returns(_scheduler.Object);
                _service = new ArenaScheduler(_gameworld.Object, _lifecycle.Object);
        }

	[TestMethod]
	public void Schedule_DraftState_TransitionsImmediately()
	{
		var now = DateTime.UtcNow;
		var scheduledAt = now.AddMinutes(10);
		var eventType = BuildEventType();
		var arenaEvent = new Mock<IArenaEvent>();
                arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Draft);
                arenaEvent.SetupGet(x => x.ScheduledAt).Returns(scheduledAt);
                arenaEvent.SetupGet(x => x.CreatedAt).Returns(now);
                arenaEvent.SetupGet(x => x.RegistrationOpensAt).Returns((DateTime?)null);
                arenaEvent.SetupGet(x => x.EventType).Returns(eventType.Object);
                arenaEvent.SetupGet(x => x.Id).Returns(5L);

		_service.Schedule(arenaEvent.Object);

		_scheduler.Verify(x => x.Destroy(arenaEvent.Object, ScheduleType.ArenaEvent), Times.Once);
		_scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Never);
		_lifecycle.Verify(x => x.Transition(arenaEvent.Object, ArenaEventState.Scheduled), Times.Once);
	}

        [TestMethod]
        public void Schedule_ScheduledPastRegistration_TransitionsImmediately()
        {
                var now = DateTime.UtcNow;
                var eventType = BuildEventType();
                var arenaEvent = new Mock<IArenaEvent>();
                arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Scheduled);
                arenaEvent.SetupGet(x => x.ScheduledAt).Returns(now.AddMinutes(30));
                arenaEvent.SetupGet(x => x.CreatedAt).Returns(now.AddHours(-1));
                arenaEvent.SetupGet(x => x.RegistrationOpensAt).Returns(now.AddMinutes(-5));
                arenaEvent.SetupGet(x => x.EventType).Returns(eventType.Object);
                arenaEvent.SetupGet(x => x.Id).Returns(6L);

                _service.Schedule(arenaEvent.Object);

                _scheduler.Verify(x => x.Destroy(arenaEvent.Object, ScheduleType.ArenaEvent), Times.Once);
                _scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Never);
                _lifecycle.Verify(x => x.Transition(arenaEvent.Object, ArenaEventState.RegistrationOpen), Times.Once);
        }

	[TestMethod]
	public void Schedule_RegistrationFull_TransitionsToPreparingImmediately()
	{
		var now = DateTime.UtcNow;
		var side = new Mock<IArenaEventTypeSide>();
		side.SetupGet(x => x.Index).Returns(0);
		side.SetupGet(x => x.Capacity).Returns(1);
		var eventType = BuildEventType();
		eventType.SetupGet(x => x.Sides).Returns(new[] { side.Object });
		var participant = new Mock<IArenaParticipant>();
		participant.SetupGet(x => x.SideIndex).Returns(0);
		var arenaEvent = new Mock<IArenaEvent>();
		arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.RegistrationOpen);
		arenaEvent.SetupGet(x => x.ScheduledAt).Returns(now.AddMinutes(30));
		arenaEvent.SetupGet(x => x.CreatedAt).Returns(now.AddHours(-1));
		arenaEvent.SetupGet(x => x.RegistrationOpensAt).Returns(now.AddMinutes(-5));
		arenaEvent.SetupGet(x => x.EventType).Returns(eventType.Object);
		arenaEvent.SetupGet(x => x.Participants).Returns(new[] { participant.Object });
		arenaEvent.SetupGet(x => x.Id).Returns(601L);

		_service.Schedule(arenaEvent.Object);

		_scheduler.Verify(x => x.Destroy(arenaEvent.Object, ScheduleType.ArenaEvent), Times.Once);
		_scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Never);
		_lifecycle.Verify(x => x.Transition(arenaEvent.Object, ArenaEventState.Preparing), Times.Once);
	}

        [TestMethod]
        public void Schedule_LiveWithoutTimeLimit_DoesNotSchedule()
        {
                var now = DateTime.UtcNow;
                var eventType = BuildEventType();
                eventType.SetupGet(x => x.TimeLimit).Returns((TimeSpan?)null);
                var arenaEvent = new Mock<IArenaEvent>();
                arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Live);
                arenaEvent.SetupGet(x => x.StartedAt).Returns(now.AddMinutes(-2));
                arenaEvent.SetupGet(x => x.EventType).Returns(eventType.Object);
                arenaEvent.SetupGet(x => x.Id).Returns(7L);

                _service.Schedule(arenaEvent.Object);

                _scheduler.Verify(x => x.Destroy(arenaEvent.Object, ScheduleType.ArenaEvent), Times.Once);
                _scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Never);
                _lifecycle.Verify(x => x.Transition(It.IsAny<IArenaEvent>(), It.IsAny<ArenaEventState>()), Times.Never);
        }

        [TestMethod]
        public void Schedule_CompletedState_OnlyCancels()
        {
                var eventType = BuildEventType();
                var arenaEvent = new Mock<IArenaEvent>();
                arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Completed);
                arenaEvent.SetupGet(x => x.EventType).Returns(eventType.Object);
                arenaEvent.SetupGet(x => x.Id).Returns(8L);

                _service.Schedule(arenaEvent.Object);

                _scheduler.Verify(x => x.Destroy(arenaEvent.Object, ScheduleType.ArenaEvent), Times.Once);
                _scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Never);
                _lifecycle.Verify(x => x.Transition(It.IsAny<IArenaEvent>(), It.IsAny<ArenaEventState>()), Times.Never);
        }

	[TestMethod]
	public void SyncRecurringSchedule_EnabledTemplate_AddsRecurringSchedule()
	{
		var eventType = BuildEventType();
		eventType.SetupGet(x => x.Id).Returns(99L);
		eventType.SetupGet(x => x.AutoScheduleEnabled).Returns(true);
		eventType.SetupGet(x => x.AutoScheduleInterval).Returns(TimeSpan.FromHours(6));
		eventType.SetupGet(x => x.AutoScheduleReferenceTime).Returns(DateTime.UtcNow.AddHours(1));

		_service.SyncRecurringSchedule(eventType.Object);

		_scheduler.Verify(x => x.Destroy(eventType.Object, ScheduleType.ArenaRecurringEvent), Times.Once);
		_scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Once);
	}

	[TestMethod]
	public void SyncRecurringSchedule_DisabledTemplate_OnlyClearsRecurringSchedule()
	{
		var eventType = BuildEventType();
		eventType.SetupGet(x => x.AutoScheduleEnabled).Returns(false);

		_service.SyncRecurringSchedule(eventType.Object);

		_scheduler.Verify(x => x.Destroy(eventType.Object, ScheduleType.ArenaRecurringEvent), Times.Once);
		_scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Never);
	}

        private static Mock<IArenaEventType> BuildEventType()
        {
                var eventType = new Mock<IArenaEventType>();
                eventType.SetupGet(x => x.RegistrationDuration).Returns(TimeSpan.FromMinutes(15));
                eventType.SetupGet(x => x.PreparationDuration).Returns(TimeSpan.FromMinutes(5));
                eventType.SetupGet(x => x.TimeLimit).Returns(TimeSpan.FromMinutes(20));
                eventType.SetupGet(x => x.Sides).Returns(Array.Empty<IArenaEventTypeSide>());
                return eventType;
        }
}
