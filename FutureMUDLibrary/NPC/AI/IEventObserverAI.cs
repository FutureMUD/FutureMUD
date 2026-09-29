#nullable enable

namespace MudSharp.NPC.AI;

/// <summary>
/// An optional AI that observes its subscribed events before ordinary AI selection. Its return value
/// does not consume the event or prevent another AI from acting. NPC pause and state gates still apply.
/// </summary>
public interface IEventObserverAI : IArtificialIntelligence
{
}
