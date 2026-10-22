#nullable enable

using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;
using MudSharp.Character;

namespace MudSharp.Effects.Concrete;

/// <summary>Persistent player state identifying a configured clan communion channel.</summary>
public sealed class PsionicCathexisEffect : Effect, IMagicEffect
{
	public static void InitialiseEffectType() => RegisterFactory("PsionicCathexis", (effect, owner) => new PsionicCathexisEffect(effect, owner));
	public PsionicCathexisEffect(ICharacter owner, PsionicCathexisPower power, long clanId) : base(owner)
	{
		Power = power; ClanId = clanId;
		Login();
	}
	private PsionicCathexisEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect")!;
		Power = Gameworld.MagicPowers.Get(long.Parse(root.Element("Power")!.Value)) as PsionicCathexisPower
		        ?? throw new ApplicationException("A cathexis effect referenced a missing power.");
		ClanId = long.Parse(root.Element("ClanId")!.Value);
	}
	public PsionicCathexisPower Power { get; }
	private bool _subscribed;
	public override void Login()
	{
		if (_subscribed || Owner is not ICharacter actor) return;
		_subscribed = true;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += Maintain;
		actor.OnQuit += Stop;
		actor.OnDeath += End;
		actor.OnStateChanged += StateChanged;
	}
	private void End(IPerceivable owner) => Owner.RemoveEffect(this, true);
	private void StateChanged(IPerceivable owner)
	{
		if (owner is ICharacter actor && !actor.State.IsAble()) actor.RemoveEffect(this, true);
	}
	private void Maintain()
	{
		if (Owner is ICharacter actor && (!actor.State.IsAble() ||
			!actor.ClanMemberships.Any(x => x.Clan.Id == ClanId) || !actor.Powers.Contains(Power) || !Power.MaintainCathexis(actor)))
			actor.RemoveEffect(this, true);
	}
	private void Stop(IPerceivable owner)
	{
		if (!_subscribed || Owner is not ICharacter actor) return;
		_subscribed = false;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= Maintain;
		actor.OnQuit -= Stop;
		actor.OnDeath -= End;
		actor.OnStateChanged -= StateChanged;
	}
	public override void RemovalEffect() => Stop(Owner);
	public long ClanId { get; }
	public IMagicSchool School => Power.School;
	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "PsionicCathexis";
	public override string Describe(IPerceiver voyeur) => $"Maintaining cathexis with clan #{ClanId.ToString("N0", voyeur)}.";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Power", Power.Id), new XElement("ClanId", ClanId));
}
