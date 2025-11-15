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
        public void Schedule_DraftState_QueuesTransitionToScheduled()
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

                ISchedule? captured = null;
                _scheduler.Setup(x => x.AddSchedule(It.IsAny<ISchedule>()))
                        .Callback<ISchedule>(schedule => captured = schedule);

                _service.Schedule(arenaEvent.Object);

                _scheduler.Verify(x => x.Destroy(arenaEvent.Object, ScheduleType.ArenaEvent), Times.Once);
                Assert.IsNotNull(captured, "Expected a schedule to be created for the next transition.");
                var typed = captured as Schedule<IArenaEvent>;
                Assert.IsNotNull(typed, "Expected an ArenaEvent schedule instance.");
                Assert.AreSame(arenaEvent.Object, typed!.Parameter1);
                Assert.AreEqual(ScheduleType.ArenaEvent, typed.Type);
                Assert.IsTrue(Math.Abs((typed.TriggerETA - scheduledAt).TotalSeconds) < 0.5,
                        $"Trigger should align with the scheduled start. Expected {scheduledAt:o}, got {typed.TriggerETA:o}.");
                Assert.AreEqual(0, _lifecycle.Invocations.Count);

                typed.Action(typed.Parameter1);

                var invocation = _lifecycle.Invocations.Single();
                Assert.AreEqual("Transition", invocation.Method.Name);
                Assert.AreSame(arenaEvent.Object, invocation.Arguments[0]);
                Assert.AreEqual(ArenaEventState.Scheduled, invocation.Arguments[1]);
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
