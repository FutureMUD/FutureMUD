#nullable enable

using MudSharp.Combat;

namespace MudSharp.Effects.Concrete;

/// <summary>Remembers the original perception result, including consumed and visible openings.</summary>
internal sealed class UnseenCombatOpening : Effect, IRemoveOnCombatEnd
{
	public ICharacter Target { get; }
	public bool Available { get; private set; }
	protected override string SpecificEffectType => "UnseenCombatOpening";

	public UnseenCombatOpening(ICharacter owner, ICharacter target, bool unseen) : base(owner)
	{
		Target = target;
		Available = unseen;
		owner.OnLeaveCombat += ParticipantLeft;
		target.OnLeaveCombat += ParticipantLeft;
	}

	private void ParticipantLeft(IPerceivable participant)
	{
		// Ending a different friendly bout during engagement has not entered this encounter yet.
		if (((ICharacter)Owner).Combat is not null && ((ICharacter)Owner).Combat == Target.Combat)
			Owner.RemoveEffect(this, true);
	}
	public override void RemovalEffect()
	{
		((ICharacter)Owner).OnLeaveCombat -= ParticipantLeft;
		Target.OnLeaveCombat -= ParticipantLeft;
		base.RemovalEffect();
	}
	public override string Describe(IPerceiver voyeur) => $"{(Available ? "Unused" : "Spent")} unseen opening against {Target.HowSeen(voyeur)}";

	internal static bool IsNewEncounter(ICharacter actor, ICharacter target) =>
		actor.Combat is null || target.Combat is null || actor.Combat != target.Combat;

	internal static IDisposable? Capture(ICharacter actor, ICharacter target, bool unseen)
	{
		if (!IsNewEncounter(actor, target) || Find(actor, target) is not null) return null;
		var opening = new UnseenCombatOpening(actor, target, unseen);
		actor.AddEffect(opening);
		return new CaptureScope(actor, opening);
	}

	private sealed class CaptureScope(ICharacter actor, UnseenCombatOpening opening) : IDisposable
	{
		public void Dispose()
		{
			if (actor.Combat is null || actor.Combat != opening.Target.Combat)
				actor.RemoveEffect(opening, true);
		}
	}

	private static UnseenCombatOpening? Find(ICharacter actor, ICharacter target) =>
		actor.EffectsOfType<UnseenCombatOpening>().FirstOrDefault(x => ReferenceEquals(x.Target, target));

	internal static bool HasOpening(ICharacter actor, ICharacter target) =>
		actor.Combat is not null && actor.Combat == target.Combat && Find(actor, target)?.Available == true;

	internal static void Consume(ICharacter actor, ICharacter? target)
	{
		if (target is not null && Find(actor, target) is { } opening) opening.Available = false;
	}
}
