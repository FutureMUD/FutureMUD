#nullable enable

using MudSharp.Events;

namespace MudSharp.NPC.AI;

internal static class AIEventDispatcher
{
	internal static bool HandleEvent(IReadOnlyCollection<IArtificialIntelligence> ais, EventType type,
		params dynamic[] arguments)
	{
		foreach (var observer in ais.OfType<IEventObserverAI>())
		{
			if (observer.HandlesEvent(type))
			{
				observer.HandleEvent(type, arguments);
			}
		}

		// Preserve priority and short-circuiting for ordinary behaviour AIs.
		return ais.Any(x => x is not IEventObserverAI && x.HandleEvent(type, arguments));
	}
}
