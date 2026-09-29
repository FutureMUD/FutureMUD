#nullable enable

namespace MudSharp.Effects.Concrete;

/// <summary>An ephemeral delay owned by one controller; never persisted or replayed at login.</summary>
public sealed class CreatureEngagementDelay : BlockingDelayedAction
{
	public long AiId { get; }
	public CreatureEngagementDelay(ICharacter owner, long aiId, Action<IPerceivable> action)
		: base(owner, action, "preparing to hunt", ["general", "combat-engage", "movement"], null) => AiId = aiId;
}
