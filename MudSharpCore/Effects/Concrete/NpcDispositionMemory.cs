#nullable enable

using MudSharp.NPC.AI;

namespace MudSharp.Effects.Concrete;

/// <summary>
/// A session-only learned reaction. Both participants' departure, death or deletion ends it;
/// optional expiry is supplied through the ordinary effect scheduler.
/// </summary>
public sealed class NpcDispositionMemory : Effect, IEffectRemovalNotification
{
	public ICharacter CharacterOwner { get; }
	public ICharacter Subject { get; }
	public NpcDisposition Disposition { get; }
	private bool _released;

	public NpcDispositionMemory(ICharacter owner, ICharacter subject, NpcDisposition disposition) : base(owner)
	{
		CharacterOwner = owner;
		Subject = subject;
		Disposition = disposition;
		foreach (var participant in Participants())
		{
			participant.OnQuit += ParticipantInvalid;
			participant.OnDeath += ParticipantInvalid;
			participant.OnDeleted += ParticipantInvalid;
			participant.OnStateChanged += ParticipantStateChanged;
		}
	}

	private IEnumerable<ICharacter> Participants()
	{
		yield return CharacterOwner;
		if (!ReferenceEquals(CharacterOwner, Subject)) yield return Subject;
	}

	private void ParticipantInvalid(IPerceivable participant) => CharacterOwner.RemoveEffect(this, true);

	private void ParticipantStateChanged(IPerceivable participant)
	{
		if (CharacterOwner.State.IsDead() || Subject.State.IsDead()) ParticipantInvalid(participant);
	}

	public override void RemovalEffect()
	{
		OnRemovedFromOwner();
		base.RemovalEffect();
	}

	public void OnRemovedFromOwner()
	{
		if (_released) return;
		_released = true;
		foreach (var participant in Participants())
		{
			participant.OnQuit -= ParticipantInvalid;
			participant.OnDeath -= ParticipantInvalid;
			participant.OnDeleted -= ParticipantInvalid;
			participant.OnStateChanged -= ParticipantStateChanged;
		}
	}

	public static bool Has(ICharacter owner, ICharacter subject, NpcDisposition disposition) =>
		owner.Effects.OfType<NpcDispositionMemory>().Any(x => !x._released &&
			ReferenceEquals(x.Subject, subject) && x.Disposition == disposition);

	protected override string SpecificEffectType => "NpcDispositionMemory";
	public override bool SavingEffect => false;
	public override string Describe(IPerceiver voyeur) =>
		$"Remembers {Subject.HowSeen(voyeur)} with {Disposition.DescribeEnum()}.";
}
