#nullable enable
using System;
using MudSharp.Framework;

namespace MudSharp.Arenas;

/// <summary>
///     Coordinates arena event state transitions and arranges follow-up scheduling.
/// </summary>
public class ArenaLifecycleService : IArenaLifecycleService
{
	private IArenaScheduler? _scheduler;

	public ArenaLifecycleService(IFuturemud gameworld)
	{
		// Retained for future integrations requiring the game context.
		_ = gameworld;
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

		arenaEvent.EnforceState(targetState);
		var currentState = arenaEvent.State;

		if (currentState is ArenaEventState.Completed or ArenaEventState.Aborted)
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
}
