#nullable enable

using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Checks;
using System.Xml;

namespace MudSharp.Effects.Concrete;

/// <summary>A configurable, saved combat frenzy that is valid only for its originating body.</summary>
public sealed class CombatFrenzyEffect : Effect, IRageEffect, IContextualCheckBonusEffect, IFixedMeleeDamageBonusEffect,
	INeedRateEffect, IPreventPositionChange, IPreventSleepEffect, INoMercyEffect,
	IPreventStunUnconsciousnessEffect, IExpireAtZeroStaminaEffect, IHealthCheckAfterRemovalEffect
{
	public static void InitialiseEffectType() => RegisterFactory("CombatFrenzy", (root, owner) => new CombatFrenzyEffect(root, owner));

	public CombatFrenzyEffect(ICharacter owner, double offenceBonus, double defenceBonus, double meleeDamageBonus,
		double hungerMultiplier) : base(owner)
	{
		SourceBodyId = owner.Body?.Id ?? 0;
		_offenceBonus = offenceBonus;
		_defenceBonus = defenceBonus;
		_meleeDamageBonus = meleeDamageBonus;
		_hungerMultiplier = hungerMultiplier;
	}

	private CombatFrenzyEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var definition = root.Element("Effect")!;
		SourceBodyId = XmlConvert.ToInt64(definition.Element("SourceBodyId")!.Value);
		_offenceBonus = XmlConvert.ToDouble(definition.Element("OffenceBonus")!.Value);
		_defenceBonus = XmlConvert.ToDouble(definition.Element("DefenceBonus")!.Value);
		_meleeDamageBonus = XmlConvert.ToDouble(definition.Element("MeleeDamageBonus")!.Value);
		_hungerMultiplier = XmlConvert.ToDouble(definition.Element("HungerMultiplier")!.Value);
	}

	protected override string SpecificEffectType => "CombatFrenzy";
	public override bool SavingEffect => true;
	public long SourceBodyId { get; }
	private double _offenceBonus;
	private double _defenceBonus;
	public double OffenceBonus => IsCurrentBody ? _offenceBonus : 0.0;
	public double DefenceBonus => IsCurrentBody ? _defenceBonus : 0.0;
	private double _meleeDamageBonus;
	private double _hungerMultiplier;
	public double MeleeDamageBonus => IsCurrentBody ? _meleeDamageBonus : 0.0;
	public double HungerMultiplier => IsCurrentBody ? _hungerMultiplier : 1.0;
	public bool IsRaging => IsCurrentBody;
	public bool IsSuperRaging => false;
	public double ThirstMultiplier => 1.0;
	public double DrunkennessMultiplier => 1.0;
	public bool AppliesToPassive => IsCurrentBody;
	public bool AppliesToActive => IsCurrentBody;
	public string SleepPreventionEcho => IsCurrentBody ? "You are too consumed by your frenzy to sleep." : string.Empty;
	public double CheckBonus => IsCurrentBody ? OffenceBonus : 0.0;
	public double CheckBonusFor(CheckType type) => !IsCurrentBody ? 0.0 :
		type.IsOffensiveCombatAction() ? OffenceBonus :
		type.IsDefensiveCombatAction() ? DefenceBonus : 0.0;
	public bool AppliesToCheck(CheckType type) => IsCurrentBody &&
		(type.IsOffensiveCombatAction() || type.IsDefensiveCombatAction());
	public override bool Applies() => IsCurrentBody && base.Applies();
	public override bool Applies(object target) => IsCurrentBody && base.Applies(target);
	public override bool Applies(object target, object thirdparty) => IsCurrentBody && base.Applies(target, thirdparty);
	public override bool Applies(object target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) => IsCurrentBody && base.Applies(target, flags);

	private bool IsCurrentBody => Owner is ICharacter character && character.Body?.Id == SourceBodyId;

	public bool PreventsChange(IPositionState oldPosition, IPositionState newPosition) =>
		IsCurrentBody && (newPosition is PositionSitting or PositionLyingDown);
	public bool ShouldCheckHealthAfterRemoval(ICharacter character) => character.Body?.Id == SourceBodyId;

	public string WhyPreventsChange(IPositionState oldPosition, IPositionState newPosition) =>
		IsCurrentBody ? "You are too consumed by your frenzy to rest." : string.Empty;

	public override string Describe(IPerceiver voyeur) => "A combat frenzy that heightens attacks and resistance.";

	protected override XElement SaveDefinition() => new("Effect",
		new XElement("SourceBodyId", SourceBodyId),
		new XElement("OffenceBonus", _offenceBonus),
		new XElement("DefenceBonus", _defenceBonus),
		new XElement("MeleeDamageBonus", _meleeDamageBonus),
		new XElement("HungerMultiplier", _hungerMultiplier));
}
