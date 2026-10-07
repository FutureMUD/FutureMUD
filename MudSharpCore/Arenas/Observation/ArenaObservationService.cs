#nullable enable

using MudSharp.Construction;
using MudSharp.Effects;

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

        if (!observer.State.IsConscious())
        {
            return (false, "You must be conscious to observe the arena.");
        }

        if (arenaEvent.State is ArenaEventState.Completed or ArenaEventState.Aborted)
        {
            return (false, "That arena event is no longer running.");
        }

        if (arenaEvent.State < ArenaEventState.RegistrationOpen)
        {
            return (false, "Registration has not opened for that event yet.");
        }

        if (arenaEvent.Participants.Any(x => x.ActiveCharacter is { } active &&
                                             CharacterInstanceIdentityComparer.SamePhysicalInstance(observer, active)))
        {
            return (false, "Participants cannot observe the event from the observation rooms.");
        }

        List<IRoom> observationRooms = arenaEvent.Arena.ObservationRooms.ToList();
        if (!observationRooms.Any())
        {
            return (false, $"{arenaEvent.Arena.Name} does not have any observation rooms configured.");
        }

        if (observer.Location is not IRoom currentRoom || !observationRooms.Contains(currentRoom))
        {
            return (false, "You must be in one of the arena's observation rooms to observe the event.");
        }

        return (true, string.Empty);
    }

    public void StartObserving(ICharacter observer, IArenaEvent arenaEvent, IRoom observationRoom)
    {
        if (observer is null)
        {
            throw new ArgumentNullException(nameof(observer));
        }

        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        if (observationRoom is null)
        {
            throw new ArgumentNullException(nameof(observationRoom));
        }

        if (!arenaEvent.Arena.ObservationRooms.Contains(observationRoom))
        {
            throw new InvalidOperationException("The specified cell is not configured as an observation room for this arena.");
        }

        foreach (IRoom room in arenaEvent.Arena.ArenaRooms)
        {
            ArenaWatcherEffect? effect = room.EffectsOfType<ArenaWatcherEffect>()
                .FirstOrDefault(x => ReferenceEquals(x.ArenaEvent, arenaEvent));

            if (effect is null)
            {
                effect = new ArenaWatcherEffect(room, arenaEvent);
                room.AddEffect(effect);
            }

            effect.AddWatcher(observer, observationRoom);
        }
    }

    public void StopObserving(ICharacter observer, IArenaEvent arenaEvent)
    {
        if (observer is null || arenaEvent is null)
        {
            return;
        }

        foreach (IRoom room in arenaEvent.Arena.ArenaRooms)
        {
            foreach (ArenaWatcherEffect? effect in room.EffectsOfType<ArenaWatcherEffect>()
                .Where(x => ReferenceEquals(x.ArenaEvent, arenaEvent))
                .ToList())
            {
                effect.RemoveWatcher(observer);
            }
        }
    }
}
