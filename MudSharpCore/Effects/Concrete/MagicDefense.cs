#nullable enable

using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public sealed class MagicDefense : ConcentrationConsumingEffect, IMagicEffect, ICheckBonusEffect
{
	public MagicDefensePower Power { get; }
	public int Charges { get; private set; }
	public double Capacity { get; private set; }
	private bool _registered;
	public MagicDefense(ICharacter actor, MagicDefensePower power) : base(actor, power.School, power.ConcentrationPointsToSustain)
	{ Power = power; Charges = power.MaximumCharges; Capacity = power.MaximumCapacity; }
	public MagicDefense(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		Power = Gameworld.MagicPowers.Get(long.Parse(root.Element("Power")!.Value)) as MagicDefensePower ?? throw new ApplicationException("Missing defense power for saved effect.");
		Charges = int.Parse(root.Element("Charges")!.Value);
		Capacity = double.Parse(root.Element("Capacity")!.Value, System.Globalization.CultureInfo.InvariantCulture);
		if (Charges < 0 || !double.IsFinite(Capacity) || Capacity < 0) throw new ApplicationException("Invalid saved magical defense capacity.");
	}
	public static void InitialiseEffectType() => RegisterFactory("MagicDefense", (xml, owner) => new MagicDefense(xml, owner));
	protected override string SpecificEffectType => "MagicDefense";
	public override bool SavingEffect => true;
	protected override bool EffectCanPersistOnLogout => true;
	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	public double CheckBonus => Power.SustainPenalty;
	public bool AppliesToCheck(CheckType type) => type.IsDefensiveCombatAction() || type.IsOffensiveCombatAction() || type == CheckType.GenericSkillCheck;
	public override string Describe(IPerceiver voyeur) => $"{Power.Name}: {Power.DefenseMode.DescribeEnum()}, {Charges.ToString("N0", voyeur)} charges, {Capacity.ToString("N2", voyeur)} capacity";
	protected override XElement SaveDefinition() => SaveToXml(new XElement("Power", Power.Id), new XElement("Charges", Charges), new XElement("Capacity", Capacity));
	public bool Available => Power.DefenseMode switch { MagicDefenseMode.Charged => Charges > 0, MagicDefenseMode.Absorption => Capacity > 0, _ => true };
	public void UseCharge() { Charges = Math.Max(0, Charges - 1); Owner.EffectsChanged = true; CheckExpiry(); }
	public double UseCapacity(double amount)
	{
		var used = Math.Min(Capacity, Math.Max(0, amount));
		Capacity -= used;
		Owner.EffectsChanged = true;
		CheckExpiry();
		return used;
	}
	private void CheckExpiry() { if (!Available) Owner.RemoveEffect(this, true); }
	public override void InitialEffect() => RegisterEvents();
	protected override void RegisterEvents()
	{
		if (_registered) return;
		_registered = true;
		base.RegisterEvents();
		CharacterOwner.OnStateChanged += StateChanged;
		CharacterOwner.OnDeath += StateChanged;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += Tick;
	}
	private void StateChanged(IPerceivable owner) { if (!CharacterOwner.State.IsAble()) Owner.RemoveEffect(this, true); }
	private void Tick()
	{
		if (!CharacterOwner.Powers.Contains(Power) || !Available) { Owner.RemoveEffect(this, true); return; }
		Power.DoSustainCostsTick(CharacterOwner, 1.0);
	}
	public override void ReleaseEvents()
	{
		if (!_registered) return;
		_registered = false;
		base.ReleaseEvents();
		CharacterOwner.OnStateChanged -= StateChanged;
		CharacterOwner.OnDeath -= StateChanged;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= Tick;
	}
	public override void RemovalEffect()
	{
		ReleaseEvents();
		Owner.OutputHandler.Handle(new EmoteOutput(new Emote(Power.EndEmote, CharacterOwner, CharacterOwner)));
	}
}
