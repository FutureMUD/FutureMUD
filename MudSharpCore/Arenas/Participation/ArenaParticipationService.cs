#nullable enable

using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Arenas;

/// <summary>
///     Coordinates participation effects for arena combatants, ensuring reliability under disconnects.
/// </summary>
public class ArenaParticipationService : IArenaParticipationService
{
    private readonly IFuturemud _gameworld;

    public ArenaParticipationService(IFuturemud gameworld)
    {
        _gameworld = gameworld ?? throw new ArgumentNullException(nameof(gameworld));
    }

    public void EnsureParticipation(ICharacter participant, IArenaEvent arenaEvent)
    {
        if (participant is null)
        {
            throw new ArgumentNullException(nameof(participant));
        }

        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        ArenaParticipationEffect? existing = participant.CombinedEffectsOfType<ArenaParticipationEffect>()
            .FirstOrDefault(x => x.Matches(arenaEvent));
        if (existing is not null)
        {
            existing.AttachToEvent(arenaEvent);
            return;
        }

        participant.RemoveAllEffects(effect => effect.IsEffectType<LinkdeadLogout>());
        ArenaParticipationEffect effect = new(participant, arenaEvent);
        participant.AddEffect(effect);
    }

    public void EnsureParticipation(IArenaEvent arenaEvent)
    {
        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        foreach (ICharacter participant in arenaEvent.Participants
                     .Select(x => x.Character)
                     .OfType<ICharacter>())
        {
            EnsureParticipation(participant, arenaEvent);
        }
    }

    public bool HasParticipation(ICharacter participant, IArenaEvent arenaEvent)
    {
        if (participant is null || arenaEvent is null)
        {
            return false;
        }

        return participant.CombinedEffectsOfType<ArenaParticipationEffect>()
            .Any(x => x.Matches(arenaEvent));
    }

    public void ClearParticipation(ICharacter participant, IArenaEvent arenaEvent)
    {
        if (participant is null || arenaEvent is null)
        {
            return;
        }

        ArenaParticipationEffect? effect = participant.CombinedEffectsOfType<ArenaParticipationEffect>()
            .FirstOrDefault(x => x.Matches(arenaEvent));
        if (effect is null)
        {
            return;
        }

        participant.RemoveEffect(effect, true);
    }

    public void ClearParticipation(IArenaEvent arenaEvent)
    {
        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        List<ICharacter> affectedParticipants = _gameworld.Actors
            .Where(x => x.CombinedEffectsOfType<ArenaParticipationEffect>()
                .Any(effect => effect.Matches(arenaEvent)))
            .ToList();
        foreach (ICharacter? participant in affectedParticipants)
        {
            ClearParticipation(participant, arenaEvent);
        }
    }
}
