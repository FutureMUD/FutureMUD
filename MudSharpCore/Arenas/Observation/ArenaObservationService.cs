#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;

namespace MudSharp.Arenas;

/// <summary>
/// 	Manages the linkage between arena events and remote observation rooms.
/// </summary>
public class ArenaObservationService : IArenaObservationService
{
	private readonly IFuturemud _gameworld;

	public ArenaObservationService(IFuturemud gameworld)
	{
		_gameworld = gameworld ?? throw new ArgumentNullException(nameof(gameworld));
	}

	public (bool Truth, string Reason) CanObserve(ICharacter observer, IArenaEvent arenaEvent)
	{
		if (observer is null)
		{
			return (false, "There is no observer to watch the arena.");
		}

		if (arenaEvent is null)
		{
			return (false, "There is no such arena event to observe.");
		}

		if (!observer.State.HasFlag(CharacterState.Conscious))
		{
			return (false, "You must be conscious to observe the arena.");
		}

		if (arenaEvent.State is ArenaEventState.Completed or ArenaEventState.Aborted)
		{
			return (false, "That arena event is no longer running.");
		}

		if (arenaEvent.State < ArenaEventState.Staged)
		{
			return (false, "The event has not been staged for observation yet.");
		}

		if (arenaEvent.Participants.Any(x => x.Character == observer))
		{
			return (false, "Participants cannot observe the event from the observation rooms.");
		}

		var observationCells = arenaEvent.Arena.ObservationCells.ToList();
		if (!observationCells.Any())
		{
			return (false, $"{arenaEvent.Arena.Name} does not have any observation rooms configured.");
		}

		if (observer.Location is not ICell currentCell || !observationCells.Contains(currentCell))
		{
			return (false, "You must be in one of the arena's observation rooms to observe the event.");
		}

		return (true, string.Empty);
	}

	public void StartObserving(ICharacter observer, IArenaEvent arenaEvent, ICell observationCell)
	{
		if (observer is null)
		{
			throw new ArgumentNullException(nameof(observer));
		}

		if (arenaEvent is null)
		{
			throw new ArgumentNullException(nameof(arenaEvent));
		}

		if (observationCell is null)
		{
			throw new ArgumentNullException(nameof(observationCell));
		}

		if (!arenaEvent.Arena.ObservationCells.Contains(observationCell))
		{
			throw new InvalidOperationException("The specified cell is not configured as an observation room for this arena.");
		}

		foreach (var cell in arenaEvent.Arena.ArenaCells)
		{
			var effect = cell.EffectsOfType<ArenaWatcherEffect>()
				.FirstOrDefault(x => ReferenceEquals(x.ArenaEvent, arenaEvent));

			if (effect is null)
			{
				effect = new ArenaWatcherEffect(cell, arenaEvent);
				cell.AddEffect(effect);
			}

			effect.AddWatcher(observer, observationCell);
		}
	}

	public void StopObserving(ICharacter observer, IArenaEvent arenaEvent)
	{
		if (observer is null || arenaEvent is null)
		{
			return;
		}

		foreach (var cell in arenaEvent.Arena.ArenaCells)
		{
			foreach (var effect in cell.EffectsOfType<ArenaWatcherEffect>()
				.Where(x => ReferenceEquals(x.ArenaEvent, arenaEvent))
				.ToList())
			{
				effect.RemoveWatcher(observer);
			}
		}
	}
}
