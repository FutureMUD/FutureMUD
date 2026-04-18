#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using System;
using System.Linq;

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
        DateTime now = DateTime.UtcNow;
        DateTime scheduledAt = now.AddMinutes(10);
        Mock<IArenaEventType> eventType = BuildEventType();
        Mock<IArenaEvent> arenaEvent = new();
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
        DateTime now = DateTime.UtcNow;
        Mock<IArenaEventType> eventType = BuildEventType();
        Mock<IArenaEvent> arenaEvent = new();
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
    public void Schedule_ScheduledWhileAnotherEventCurrent_DefersRegistrationOpen()
    {
        DateTime now = DateTime.UtcNow;
        Mock<IArenaEventType> eventType = BuildEventType();
        Mock<ICombatArena> arena = new();
        Mock<IArenaEvent> liveEvent = new();
        liveEvent.SetupGet(x => x.Id).Returns(707L);
        liveEvent.SetupGet(x => x.State).Returns(ArenaEventState.Live);
        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Scheduled);
        arenaEvent.SetupGet(x => x.ScheduledAt).Returns(now.AddMinutes(30));
        arenaEvent.SetupGet(x => x.CreatedAt).Returns(now.AddHours(-1));
        arenaEvent.SetupGet(x => x.RegistrationOpensAt).Returns(now.AddMinutes(-5));
        arenaEvent.SetupGet(x => x.EventType).Returns(eventType.Object);
        arenaEvent.SetupGet(x => x.Id).Returns(706L);
        arenaEvent.SetupGet(x => x.Arena).Returns(arena.Object);
        arena.SetupGet(x => x.ActiveEvents).Returns(new[] { arenaEvent.Object, liveEvent.Object });

        _service.Schedule(arenaEvent.Object);

        _scheduler.Verify(x => x.Destroy(arenaEvent.Object, ScheduleType.ArenaEvent), Times.Once);
        _scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Once);
        _lifecycle.Verify(x => x.Transition(arenaEvent.Object, ArenaEventState.RegistrationOpen), Times.Never);
    }

    [TestMethod]
    public void Schedule_RegistrationFull_TransitionsToPreparingImmediately()
    {
        DateTime now = DateTime.UtcNow;
        Mock<IArenaEventTypeSide> side = new();
        side.SetupGet(x => x.Index).Returns(0);
        side.SetupGet(x => x.Capacity).Returns(1);
        Mock<IArenaEventType> eventType = BuildEventType();
        eventType.SetupGet(x => x.Sides).Returns(new[] { side.Object });
        Mock<IArenaParticipant> participant = new();
        participant.SetupGet(x => x.SideIndex).Returns(0);
        Mock<IArenaEvent> arenaEvent = new();
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
        DateTime now = DateTime.UtcNow;
        Mock<IArenaEventType> eventType = BuildEventType();
        eventType.SetupGet(x => x.TimeLimit).Returns((TimeSpan?)null);
        Mock<IArenaEvent> arenaEvent = new();
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
    public void Schedule_ResolvingState_DefersCleanupTransition()
    {
        Mock<IArenaEventType> eventType = BuildEventType();
        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Resolving);
        arenaEvent.SetupGet(x => x.EventType).Returns(eventType.Object);
        arenaEvent.SetupGet(x => x.Id).Returns(9L);

        _service.Schedule(arenaEvent.Object);

        _scheduler.Verify(x => x.Destroy(arenaEvent.Object, ScheduleType.ArenaEvent), Times.Once);
        _scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Once);
        _lifecycle.Verify(x => x.Transition(It.IsAny<IArenaEvent>(), ArenaEventState.Cleanup), Times.Never);
    }

    [TestMethod]
    public void Schedule_CompletedState_OnlyCancels()
    {
        Mock<IArenaEventType> eventType = BuildEventType();
        Mock<IArenaEvent> arenaEvent = new();
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
        Mock<IArenaEventType> eventType = BuildEventType();
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
        Mock<IArenaEventType> eventType = BuildEventType();
        eventType.SetupGet(x => x.AutoScheduleEnabled).Returns(false);

        _service.SyncRecurringSchedule(eventType.Object);

        _scheduler.Verify(x => x.Destroy(eventType.Object, ScheduleType.ArenaRecurringEvent), Times.Once);
        _scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Never);
    }

    private static Mock<IArenaEventType> BuildEventType()
    {
        Mock<IArenaEventType> eventType = new();
        eventType.SetupGet(x => x.RegistrationDuration).Returns(TimeSpan.FromMinutes(15));
        eventType.SetupGet(x => x.PreparationDuration).Returns(TimeSpan.FromMinutes(5));
        eventType.SetupGet(x => x.TimeLimit).Returns(TimeSpan.FromMinutes(20));
        eventType.SetupGet(x => x.Sides).Returns(Array.Empty<IArenaEventTypeSide>());
        return eventType;
    }
}
