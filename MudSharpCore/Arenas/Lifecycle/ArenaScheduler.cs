#nullable enable
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Arenas;

/// <summary>
///     Schedules arena event transitions using the global game scheduler.
/// </summary>
public class ArenaScheduler : IArenaScheduler
{
    private static readonly TimeSpan ResolvingGracePeriod = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan CleanupGracePeriod = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan LivePollingInterval = TimeSpan.FromSeconds(2);

    private readonly IFuturemud _gameworld;
    private readonly IArenaLifecycleService _lifecycleService;

    public ArenaScheduler(IFuturemud gameworld, IArenaLifecycleService lifecycleService)
    {
        _gameworld = gameworld;
        _lifecycleService = lifecycleService;
        if (lifecycleService is ArenaLifecycleService concrete)
        {
            concrete.AttachScheduler(this);
        }
    }

    /// <inheritdoc />
    public void Schedule(IArenaEvent arenaEvent)
    {
        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        if (arenaEvent.State is ArenaEventState.Completed or ArenaEventState.Aborted)
        {
            Cancel(arenaEvent);
            return;
        }

        Cancel(arenaEvent);
        if (arenaEvent.State == ArenaEventState.Live)
        {
            ScheduleLiveMonitoring(arenaEvent);
            return;
        }

        if (!TryGetNextTransition(arenaEvent, out ArenaEventState nextState, out DateTime trigger))
        {
            return;
        }

        if (trigger <= DateTime.UtcNow)
        {
            _lifecycleService.Transition(arenaEvent, nextState);
            return;
        }

        ArenaEventState pendingState = nextState;
        Schedule<IArenaEvent> schedule = new(arenaEvent, evt => _lifecycleService.Transition(evt, pendingState),
            ScheduleType.ArenaEvent, trigger - DateTime.UtcNow,
            $"ArenaEvent #{arenaEvent.Id} -> {pendingState}");
        _gameworld.Scheduler.AddSchedule(schedule);
    }

    /// <inheritdoc />
    public void Cancel(IArenaEvent arenaEvent)
    {
        if (arenaEvent is null)
        {
            return;
        }

        _gameworld.Scheduler.Destroy(arenaEvent, ScheduleType.ArenaEvent);
    }

    /// <inheritdoc />
    public void SyncRecurringSchedule(IArenaEventType eventType)
    {
        if (eventType is null)
        {
            return;
        }

        _gameworld.Scheduler.Destroy(eventType, ScheduleType.ArenaRecurringEvent);
        if (!IsRecurringEnabled(eventType))
        {
            return;
        }

        DateTime now = DateTime.UtcNow;
        DateTime trigger = ResolveNextRecurringTrigger(eventType, now);
        TimeSpan delay = trigger > now ? trigger - now : TimeSpan.Zero;
        DateTime scheduledFor = trigger;
        Schedule<IArenaEventType> schedule = new(eventType, template => HandleRecurringTrigger(template, scheduledFor),
            ScheduleType.ArenaRecurringEvent, delay, $"ArenaEventType #{eventType.Id} recurring trigger");
        _gameworld.Scheduler.AddSchedule(schedule);
    }

    private void ScheduleLiveMonitoring(IArenaEvent arenaEvent)
    {
        if (arenaEvent.TryResolveFromElimination())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;
        DateTime? deadline = ResolveLiveDeadline(arenaEvent, now);
        if (deadline.HasValue && deadline.Value <= now)
        {
            _lifecycleService.Transition(arenaEvent, ArenaEventState.Resolving);
            return;
        }

        if (!deadline.HasValue && !RequiresLiveMonitoring(arenaEvent))
        {
            return;
        }

        DateTime nextTrigger = now + LivePollingInterval;
        if (deadline.HasValue && deadline.Value < nextTrigger)
        {
            nextTrigger = deadline.Value;
        }

        TimeSpan delay = nextTrigger > now ? nextTrigger - now : TimeSpan.Zero;
        Schedule<IArenaEvent> schedule = new(arenaEvent, HandleLiveMonitoringTick, ScheduleType.ArenaEvent, delay,
            $"ArenaEvent #{arenaEvent.Id} live monitor");
        _gameworld.Scheduler.AddSchedule(schedule);
    }

    private void HandleLiveMonitoringTick(IArenaEvent arenaEvent)
    {
        if (arenaEvent.State != ArenaEventState.Live)
        {
            return;
        }

        Schedule(arenaEvent);
    }

    private static DateTime? ResolveLiveDeadline(IArenaEvent arenaEvent, DateTime now)
    {
        if (arenaEvent.EventType.TimeLimit is not { } limit)
        {
            return null;
        }

        DateTime startedAt = arenaEvent.StartedAt ?? arenaEvent.ScheduledAt;
        if (startedAt == DateTime.MinValue)
        {
            startedAt = now;
        }

        return startedAt + limit;
    }

    private static bool RequiresLiveMonitoring(IArenaEvent arenaEvent)
    {
        return arenaEvent.EventType.TimeLimit.HasValue ||
               arenaEvent.EventType.EliminationMode != ArenaEliminationMode.NoElimination;
    }

    /// <inheritdoc />
    public void RecoverAfterReboot()
    {
        foreach (IArenaEventType? eventType in _gameworld.CombatArenas.SelectMany(x => x.EventTypes))
        {
            SyncRecurringSchedule(eventType);
        }
    }

    private static bool TryGetNextTransition(IArenaEvent arenaEvent, out ArenaEventState nextState, out DateTime trigger)
    {
        DateTime now = DateTime.UtcNow;
        trigger = DateTime.MinValue;
        nextState = arenaEvent.State;
        switch (arenaEvent.State)
        {
            case ArenaEventState.Draft:
                nextState = ArenaEventState.Scheduled;
                trigger = now;
                break;
            case ArenaEventState.Scheduled:
                nextState = ArenaEventState.RegistrationOpen;
                if (HasConcurrentCurrentEvent(arenaEvent))
                {
                    // Keep retrying while another event is already in progress in this arena.
                    trigger = now.AddMinutes(1);
                    break;
                }

                trigger = ResolveRegistrationOpen(arenaEvent);
                break;
            case ArenaEventState.RegistrationOpen:
                nextState = ArenaEventState.Preparing;
                trigger = IsEventFull(arenaEvent)
                    ? now
                    : ResolveRegistrationOpen(arenaEvent) + arenaEvent.EventType.RegistrationDuration;
                break;
            case ArenaEventState.Preparing:
                nextState = ArenaEventState.Staged;
                trigger = ResolveRegistrationOpen(arenaEvent) + arenaEvent.EventType.RegistrationDuration + arenaEvent.EventType.PreparationDuration;
                break;
            case ArenaEventState.Staged:
                nextState = ArenaEventState.Live;
                trigger = arenaEvent.ScheduledAt;
                break;
            case ArenaEventState.Resolving:
                nextState = ArenaEventState.Cleanup;
                trigger = now + ResolvingGracePeriod;
                break;
            case ArenaEventState.Cleanup:
                nextState = ArenaEventState.Completed;
                trigger = now + CleanupGracePeriod;
                break;
            default:
                return false;
        }

        if (trigger < now)
        {
            trigger = now;
        }

        return true;
    }

    private static DateTime ResolveRegistrationOpen(IArenaEvent arenaEvent)
    {
        if (arenaEvent.RegistrationOpensAt.HasValue)
        {
            return arenaEvent.RegistrationOpensAt.Value;
        }

        DateTime fallback = arenaEvent.ScheduledAt - arenaEvent.EventType.PreparationDuration - arenaEvent.EventType.RegistrationDuration;
        return fallback > arenaEvent.CreatedAt ? fallback : arenaEvent.CreatedAt;
    }

    private void HandleRecurringTrigger(IArenaEventType eventType, DateTime scheduledFor)
    {
        if (!IsRecurringEnabled(eventType))
        {
            return;
        }

        TryCreateRecurringEvent(eventType, scheduledFor);
        SyncRecurringSchedule(eventType);
    }

    private void TryCreateRecurringEvent(IArenaEventType eventType, DateTime scheduledFor)
    {
        ICombatArena arena = eventType.Arena;
        (bool ready, string _) = arena.IsReadyToHost(eventType);
        if (!ready)
        {
            return;
        }

        if (arena.ActiveEvents.Any(evt =>
                ReferenceEquals(evt.EventType, eventType) &&
                Math.Abs((evt.ScheduledAt - scheduledFor).TotalSeconds) < 1.0))
        {
            return;
        }

        try
        {
            eventType.CreateInstance(scheduledFor);
        }
        catch
        {
            // Keep the recurring schedule running even if one instantiation attempt fails.
        }
    }

    private static bool IsRecurringEnabled(IArenaEventType eventType)
    {
        return eventType.AutoScheduleEnabled &&
               eventType.AutoScheduleInterval.HasValue &&
               eventType.AutoScheduleInterval.Value > TimeSpan.Zero &&
               eventType.AutoScheduleReferenceTime.HasValue;
    }

    private static DateTime ResolveNextRecurringTrigger(IArenaEventType eventType, DateTime now)
    {
        DateTime reference = eventType.AutoScheduleReferenceTime!.Value;
        TimeSpan interval = eventType.AutoScheduleInterval!.Value;
        if (reference >= now)
        {
            return reference;
        }

        long elapsedTicks = now.Ticks - reference.Ticks;
        long intervalTicks = interval.Ticks;
        if (intervalTicks <= 0)
        {
            return now;
        }

        long cycles = elapsedTicks / intervalTicks;
        DateTime next = reference.AddTicks(cycles * intervalTicks);
        if (next < now)
        {
            next = next.AddTicks(intervalTicks);
        }

        return next;
    }

    private static bool IsEventFull(IArenaEvent arenaEvent)
    {
        List<IArenaEventTypeSide> sides = arenaEvent.EventType.Sides.ToList();
        if (!sides.Any())
        {
            return false;
        }

        Dictionary<int, int> participantCounts = arenaEvent.Participants
            .GroupBy(x => x.SideIndex)
            .ToDictionary(x => x.Key, x => x.Count());

        return sides.All(side =>
            participantCounts.TryGetValue(side.Index, out int count) &&
            count >= side.Capacity);
    }

    private static bool HasConcurrentCurrentEvent(IArenaEvent arenaEvent)
    {
        return arenaEvent.Arena?.ActiveEvents.Any(evt =>
            evt.Id != arenaEvent.Id &&
            evt.State > ArenaEventState.Scheduled &&
            evt.State < ArenaEventState.Completed) == true;
    }
}
