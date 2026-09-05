#nullable enable

using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

/// <summary>The caster owns concentration and upkeep; the beneficiary owns only the reaction.</summary>
public sealed class MaintainedPsychicEffect : ConcentrationConsumingEffect, IMagicEffect
{
	private bool _registered;
	private PsychicReactionEffect? _reaction;
	public PsychicTechniquePower Power { get; }
	public ICharacter Beneficiary { get; }
	public string Mode { get; }
	public static void InitialiseEffectType() => RegisterFactory("MaintainedPsychic", (xml, owner) => new MaintainedPsychicEffect(xml, owner));
	private MaintainedPsychicEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		Power = Gameworld.MagicPowers.Get((long)xml.Element("Effect")!.Element("Power")!) as PsychicTechniquePower ?? throw new ApplicationException("Saved psychic defence references a missing power.");
		Beneficiary = CharacterOwner;
		Mode = "feedback";
	}
	public override bool SavingEffect => Mode == "feedback";
	protected override bool EffectCanPersistOnLogout => Mode == "feedback";
	protected override XElement SaveDefinition() => SaveToXml(new XElement("Power", Power.Id));
	public MaintainedPsychicEffect(ICharacter caster, ICharacter beneficiary, PsychicTechniquePower power, string mode)
		: base(caster, power.School, 1)
	{
		Power = power;
		Beneficiary = beneficiary;
		Mode = mode;
	}
	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	protected override string SpecificEffectType => "MaintainedPsychic";
	public override string Describe(IPerceiver voyeur) => $"Maintains {Power.Name} for {Beneficiary.HowSeen(voyeur)}.";
	public override void InitialEffect() => Login();
	protected override void RegisterEvents()
	{
		if (_registered) return;
		_registered = true;
		base.RegisterEvents();
		if (Mode != "feedback") CharacterOwner.OnQuit += End;
		CharacterOwner.OnDeath += End;
		CharacterOwner.OnStateChanged += StateChanged;
		if (Beneficiary != CharacterOwner)
		{
			Beneficiary.OnQuit += End;
			Beneficiary.OnDeath += End;
		}
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += Sustain;
		_reaction = new PsychicReactionEffect(Beneficiary, this);
		Beneficiary.AddEffect(_reaction);
	}
	private void End(IPerceivable owner) => CharacterOwner.RemoveEffect(this, true);
	private void StateChanged(IPerceivable owner)
	{
		if (!CharacterOwner.State.IsAble()) End(owner);
	}
	public bool Valid => CharacterOwner.State.IsAble() && (Beneficiary == CharacterOwner ||
		(Power.TargetIsInRange(CharacterOwner, Beneficiary, Power.PowerDistance) &&
		 CharacterOwner.EffectsOfType<ConnectMindEffect>().Any(x => x.TargetCharacter == Beneficiary && x.School == School)));
	private void Sustain()
	{
		if (!Valid || !Power.CanAffordToInvokePower(CharacterOwner, Power.Verb).Truth) { End(CharacterOwner); return; }
		foreach (var (resource, cost) in Power.InvocationCosts[Power.Verb]) CharacterOwner.UseResource(resource, cost);
	}
	public override void ReleaseEvents()
	{
		if (!_registered) return;
		_registered = false;
		base.ReleaseEvents();
		CharacterOwner.OnQuit -= End;
		CharacterOwner.OnDeath -= End;
		CharacterOwner.OnStateChanged -= StateChanged;
		Beneficiary.OnQuit -= End;
		Beneficiary.OnDeath -= End;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= Sustain;
		if (_reaction is not null) Beneficiary.RemoveEffect(_reaction);
		_reaction = null;
	}
	public override void RemovalEffect()
	{
		ReleaseEvents();
		Power.SendEcho("EndEcho", CharacterOwner, CharacterOwner, Beneficiary);
		if (Beneficiary != CharacterOwner) Power.SendEcho("TargetEndEcho", Beneficiary, CharacterOwner, Beneficiary);
	}
}

public sealed class PsychicReactionEffect : Effect, IMentalActionReaction, IMentalActionDefence
{
	private readonly MaintainedPsychicEffect _anchor;
	public PsychicReactionEffect(ICharacter owner, MaintainedPsychicEffect anchor) : base(owner) => _anchor = anchor;
	protected override string SpecificEffectType => "PsychicReaction";
	public override string Describe(IPerceiver voyeur) => $"Protected by {_anchor.Power.Name}.";
	public double DefensiveBonus(MentalActionContext context) => _anchor.Valid && _anchor.Mode == "guard" && context.Hostile ? _anchor.Power.Amount : 0;
	public void OnMentalAction(MentalActionContext context, MagicInvocationResult result)
	{
		if (!_anchor.Valid || !context.Hostile || context.Source == context.Target) return;
		_anchor.Power.SendEcho("IntrusionEcho", _anchor.CharacterOwner, _anchor.CharacterOwner, _anchor.Beneficiary);
		if (_anchor.Mode != "feedback") return;
		var resource = Gameworld.MagicResources.Get(_anchor.Power.ResourceId);
		if (_anchor.Power.FeedbackMode == "stun")
		{
			var wounds = context.Source.PassiveSufferDamage(new MudSharp.Health.Damage { ActorOrigin = context.Target,
				DamageType = MudSharp.Health.DamageType.Eldritch, StunAmount = Math.Clamp(_anchor.Power.Amount, 0, 10) }).ToList();
			wounds.ProcessPassiveWounds();
		}
		else if (_anchor.Power.FeedbackMode == "resource" && resource is not null)
		{
			context.Source.MagicResourceAmounts.TryGetValue(resource, out var current);
			context.Source.UseResource(resource, Math.Max(0, Math.Min(current, _anchor.Power.Amount)));
		}
		_anchor.Power.SendEcho("ResponseEcho", context.Source, _anchor.CharacterOwner, context.Source);
	}
}
