#nullable enable

using System;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.Arenas;

/// <summary>
///     Coordinates participation effects for arena combatants, ensuring reliability under disconnects.
/// </summary>
public class ArenaParticipationService : IArenaParticipationService
{
        public ArenaParticipationService(IFuturemud gameworld)
        {
                _ = gameworld ?? throw new ArgumentNullException(nameof(gameworld));
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

                var existing = participant.CombinedEffectsOfType<ArenaParticipationEffect>()
                        .FirstOrDefault(x => x.Matches(arenaEvent));
                if (existing is not null)
                {
                        existing.AttachToEvent(arenaEvent);
                        return;
                }

                participant.RemoveAllEffects(effect => effect.IsEffectType<LinkdeadLogout>());
                var effect = new ArenaParticipationEffect(participant, arenaEvent);
                participant.AddEffect(effect);
        }

        public void EnsureParticipation(IArenaEvent arenaEvent)
        {
                if (arenaEvent is null)
                {
                        throw new ArgumentNullException(nameof(arenaEvent));
                }

                foreach (var participant in arenaEvent.Participants
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

                var effect = participant.CombinedEffectsOfType<ArenaParticipationEffect>()
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

                foreach (var participant in arenaEvent.Participants
                                 .Select(x => x.Character)
                                 .OfType<ICharacter>())
                {
                        ClearParticipation(participant, arenaEvent);
                }
        }
}
