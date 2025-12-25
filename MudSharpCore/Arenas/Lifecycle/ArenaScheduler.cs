#nullable enable
using System;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Arenas;

/// <summary>
///     Schedules arena event transitions using the global game scheduler.
/// </summary>
public class ArenaScheduler : IArenaScheduler
{
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
		if (!TryGetNextTransition(arenaEvent, out var nextState, out var trigger))
		{
			return;
		}

		if (trigger <= DateTime.UtcNow)
		{
			_lifecycleService.Transition(arenaEvent, nextState);
			return;
		}

		var pendingState = nextState;
		var schedule = new Schedule<IArenaEvent>(arenaEvent, evt => _lifecycleService.Transition(evt, pendingState),
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
	public void RecoverAfterReboot()
	{
		// Concrete arena loading will reschedule active events once they are reconstructed.
	}

	private static bool TryGetNextTransition(IArenaEvent arenaEvent, out ArenaEventState nextState, out DateTime trigger)
	{
		var now = DateTime.UtcNow;
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
				trigger = ResolveRegistrationOpen(arenaEvent);
				break;
			case ArenaEventState.RegistrationOpen:
				nextState = ArenaEventState.Preparing;
				trigger = ResolveRegistrationOpen(arenaEvent) + arenaEvent.EventType.RegistrationDuration;
				break;
			case ArenaEventState.Preparing:
				nextState = ArenaEventState.Staged;
				trigger = ResolveRegistrationOpen(arenaEvent) + arenaEvent.EventType.RegistrationDuration + arenaEvent.EventType.PreparationDuration;
				break;
			case ArenaEventState.Staged:
				nextState = ArenaEventState.Live;
				trigger = arenaEvent.ScheduledAt;
				break;
			case ArenaEventState.Live:
				if (arenaEvent.EventType.TimeLimit is { } limit)
				{
					nextState = ArenaEventState.Resolving;
					var startedAt = arenaEvent.StartedAt ?? arenaEvent.ScheduledAt;
					if (startedAt == DateTime.MinValue)
					{
						startedAt = now;
					}

					trigger = startedAt + limit;
					break;
				}
				return false;
			case ArenaEventState.Resolving:
				nextState = ArenaEventState.Cleanup;
				trigger = now;
				break;
			case ArenaEventState.Cleanup:
				nextState = ArenaEventState.Completed;
				trigger = now;
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

		var fallback = arenaEvent.ScheduledAt - arenaEvent.EventType.PreparationDuration - arenaEvent.EventType.RegistrationDuration;
		return fallback > arenaEvent.CreatedAt ? fallback : arenaEvent.CreatedAt;
	}
}
