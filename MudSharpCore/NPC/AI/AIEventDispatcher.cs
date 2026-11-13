#nullable enable

using MudSharp.Events;

namespace MudSharp.NPC.AI;

internal static class AIEventDispatcher
{
	internal static bool HandleEvent(IReadOnlyCollection<IArtificialIntelligence> ais, EventType type,
		params dynamic[] arguments)
	{
		return HandleEvent(ais, type, false, arguments);
	}

	internal static bool HandleEvent(IReadOnlyCollection<IArtificialIntelligence> ais, EventType type,
		bool observersOnly, params dynamic[] arguments)
	{
		foreach (var observer in ais.OfType<IEventObserverAI>())
		{
			if (observer.HandlesEvent(type))
			{
				observer.HandleEvent(type, arguments);
			}
		}

		if (observersOnly && type is not (EventType.EngagedInCombat or EventType.EngagedInCombatWitness or
		    EventType.TargettedInCombat or EventType.CharacterDamagedWitness or EventType.WitnessedCrime or
		    EventType.CharacterIncapacitatedWitness or EventType.CharacterDiesWitness))
		{
			return false;
		}

		// Preserve priority and short-circuiting for ordinary behaviour AIs.
		return ais.Any(x => x is not IEventObserverAI && x.HandleEvent(type, arguments));
	}
}
