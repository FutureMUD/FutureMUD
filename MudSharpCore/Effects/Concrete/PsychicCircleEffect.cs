#nullable enable
using MudSharp.Magic;
using MudSharp.Magic.Powers;

namespace MudSharp.Effects.Concrete;

public sealed class PsychicCircleEffect : ConcentrationConsumingEffect, IMagicEffect
{
	private bool _registered;
	private readonly List<ICharacter> _members = [];
	public PsychicCirclePower Power { get; }
	public IMagicPower PowerOrigin => Power;
	public MudSharp.RPG.Checks.Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	public IReadOnlyList<ICharacter> Members => _members;
	public PsychicCircleEffect(ICharacter leader, PsychicCirclePower power) : base(leader, power.School, 1) => Power = power;
	protected override string SpecificEffectType => "PsychicCircle";
	public override string Describe(IPerceiver voyeur) => $"Leading a psychic circle with {_members.Count.ToString("N0", voyeur)} members.";
	public override void InitialEffect() { Login(); Join(CharacterOwner); }
	protected override void RegisterEvents()
	{
		if (_registered) return;
		_registered = true;
		base.RegisterEvents();
		CharacterOwner.OnQuit += End;
		CharacterOwner.OnDeath += End;
		CharacterOwner.OnStateChanged += ChangedState;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += Sustain;
	}
	public bool Join(ICharacter member)
	{
		if (!_registered || !member.State.IsAble() || _members.Count >= Power.CircleMemberLimit || member.AffectedBy<PsychicCircleMembership>()) return false;
		if (member != CharacterOwner && (!Power.TargetIsInRange(CharacterOwner, member, Power.PowerDistance) ||
		    MagicInterdictionHelper.GetInterdiction(CharacterOwner, member, School, false) is not null)) return false;
		_members.Add(member);
		member.AddEffect(new PsychicCircleMembership(member, this));
		member.OnQuit += MemberLeft; member.OnDeath += MemberLeft;
		Power.SendEcho("JoinEcho", member, CharacterOwner, member);
		return true;
	}
	private void MemberLeft(IPerceivable member) { if (member is ICharacter character) Leave(character); }
	public void Leave(ICharacter member)
	{
		if (!_members.Remove(member)) return;
		Power.SendEcho("LeaveEcho", member, CharacterOwner, member);
		member.OnQuit -= MemberLeft; member.OnDeath -= MemberLeft;
		member.RemoveAllEffects<PsychicCircleMembership>(x => x.Circle == this);
		if (member == CharacterOwner && _registered) End(member);
	}
	public void Send(ICharacter speaker, string text)
	{
		if (!_registered || !speaker.State.IsAble() || !_members.Contains(speaker) || string.IsNullOrWhiteSpace(text) || text.Length > 2000) return;
		text = text.Sanitise().RawText();
		foreach (var member in _members.ToList())
		{
			if (!Power.TargetIsInRange(CharacterOwner, member, Power.PowerDistance) && member != CharacterOwner) { Leave(member); continue; }
			if (MagicInterdictionHelper.GetInterdiction(speaker, member, School, false) is not null) continue;
			member.OutputHandler.Send(speaker == member ? Power.FormatEcho("SelfSpeechEcho", text) :
				Power.FormatEcho("SpeechEcho", PsionicTrafficHelper.SourceDescription(speaker, member, School), text));
		}
		PsionicActivityNotifier.Notify(speaker, Power, "psychic circle speech", _members);
	}
	private void ChangedState(IPerceivable owner) { if (!CharacterOwner.State.IsAble()) End(owner); }
	private void End(IPerceivable owner) => CharacterOwner.RemoveEffect(this, true);
	private void Sustain()
	{
		foreach (var member in _members.Where(x => x != CharacterOwner && !Power.TargetIsInRange(CharacterOwner, x, Power.PowerDistance)).ToList()) Leave(member);
		if (!CharacterOwner.State.IsAble() || Power.InvocationCosts[Power.Verb].Any(x => !CharacterOwner.CanUseResource(x.Resource, x.Cost * _members.Count)))
		{ End(CharacterOwner); return; }
		foreach (var (resource, cost) in Power.InvocationCosts[Power.Verb]) CharacterOwner.UseResource(resource, cost * _members.Count);
	}
	public override void ReleaseEvents()
	{
		if (!_registered) return;
		_registered = false;
		base.ReleaseEvents();
		CharacterOwner.OnQuit -= End; CharacterOwner.OnDeath -= End; CharacterOwner.OnStateChanged -= ChangedState;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= Sustain;
		foreach (var member in _members.ToList()) Leave(member);
	}
	public override void RemovalEffect() => ReleaseEvents();
}

public sealed class PsychicCircleMembership(IPerceivable owner, PsychicCircleEffect circle) : Effect(owner)
{
	public PsychicCircleEffect Circle { get; } = circle;
	protected override string SpecificEffectType => "PsychicCircleMembership";
	public override string Describe(IPerceiver voyeur) => "Participating in a psychic circle.";
}
public sealed class PsychicCircleInvitation(IPerceivable owner, PsychicCircleEffect circle) : Effect(owner)
{
	public PsychicCircleEffect Circle { get; } = circle;
	protected override string SpecificEffectType => "PsychicCircleInvitation";
	public override string Describe(IPerceiver voyeur) => "Invited to a psychic circle.";
}
