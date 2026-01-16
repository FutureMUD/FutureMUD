#nullable enable
using System;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Arenas;

/// <summary>
///     Coordinates arena event state transitions and arranges follow-up scheduling.
/// </summary>
public class ArenaLifecycleService : IArenaLifecycleService
{
	private readonly IFuturemud _gameworld;
	private IArenaScheduler? _scheduler;

	public ArenaLifecycleService(IFuturemud gameworld)
	{
		_gameworld = gameworld ?? throw new ArgumentNullException(nameof(gameworld));
	}

	/// <summary>
	///     Links the lifecycle service with the arena scheduler implementation.
	/// </summary>
	/// <param name="scheduler">Scheduler responsible for timed transitions.</param>
	public void AttachScheduler(IArenaScheduler scheduler)
	{
		_scheduler = scheduler;
	}

	/// <inheritdoc />
	public void Transition(IArenaEvent arenaEvent, ArenaEventState targetState)
	{
		if (arenaEvent is null)
		{
			throw new ArgumentNullException(nameof(arenaEvent));
		}

		var initialState = arenaEvent.State;
		if (!IsForwardTransition(initialState, targetState))
		{
			return;
		}

		ApplyTransition(arenaEvent, targetState);
		if (targetState is ArenaEventState.Completed or ArenaEventState.Aborted)
		{
			_scheduler?.Cancel(arenaEvent);
			return;
		}

		_scheduler?.Schedule(arenaEvent);
	}

	/// <inheritdoc />
	public void RebootRecovery()
	{
		_scheduler?.RecoverAfterReboot();
		CleanupInterruptedEvents();
	}

	private void CleanupInterruptedEvents()
	{
		var interruptedEvents = _gameworld.CombatArenas
			.SelectMany(x => x.ActiveEvents)
			.Where(x => x.State is ArenaEventState.Preparing
				or ArenaEventState.Staged
				or ArenaEventState.Live
				or ArenaEventState.Resolving
				or ArenaEventState.Cleanup)
			.ToList();

		foreach (var arenaEvent in interruptedEvents)
		{
			arenaEvent.Abort("Event cancelled due to server restart.");
			_scheduler?.Cancel(arenaEvent);
		}
	}

	private static bool IsForwardTransition(ArenaEventState current, ArenaEventState target)
	{
		if (current == target)
		{
			return false;
		}

		if (target == ArenaEventState.Aborted)
		{
			return true;
		}

		return current is not (ArenaEventState.Aborted or ArenaEventState.Completed) && target > current;
	}

	private static void ApplyTransition(IArenaEvent arenaEvent, ArenaEventState targetState)
	{
		switch (targetState)
		{
			case ArenaEventState.RegistrationOpen:
				arenaEvent.OpenRegistration();
				break;
			case ArenaEventState.Preparing:
				arenaEvent.StartPreparation();
				break;
			case ArenaEventState.Staged:
				arenaEvent.Stage();
				break;
			case ArenaEventState.Live:
				arenaEvent.StartLive();
				break;
			case ArenaEventState.Resolving:
				arenaEvent.Resolve();
				break;
			case ArenaEventState.Cleanup:
				arenaEvent.Cleanup();
				break;
			case ArenaEventState.Completed:
				arenaEvent.Complete();
				break;
			case ArenaEventState.Aborted:
				arenaEvent.Abort("Event aborted.");
				break;
			default:
				arenaEvent.EnforceState(targetState);
				break;
		}
	}
}
