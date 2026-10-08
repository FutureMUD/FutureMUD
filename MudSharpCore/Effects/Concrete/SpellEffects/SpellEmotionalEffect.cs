#nullable enable

using MudSharp.Body.Traits;
using System.Globalization;
using MudSharp.Magic.Emotions;
using MudSharp.Magic.SpellEffects;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>Source grade survives separately from native parent power and editable intensity.</summary>
public abstract class SpellEmotionalEffect : MagicSpellEffectBase
{
	private XElement? _retainedDefinition;
	protected SpellEmotionalEffect(IPerceivable owner, IMagicSpellEffectParent parent, string group,
		int unitSeconds, int capUnits, EmotionalRetainedState state) : base(owner, parent, null!)
	{
		EmotionalSpellPolicy.ValidateState(state);
		if (DetectInvisibleEffect.PolicyError(new(group, unitSeconds, capUnits)) is { } error) throw new ArgumentException(error);
		Group = group; UnitSeconds = unitSeconds; CapUnits = capUnits; State = state;
	}

	protected SpellEmotionalEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		_retainedDefinition = new XElement(root);
		Group = (string?)root.Attribute("group") ?? "";
		UnitSeconds = ReadInt(root, "unitSeconds"); CapUnits = ReadInt(root, "capUnits");
		State = new(ReadInt(root, "grade"), (Magic.SpellPower)ReadInt(root, "nativePower"),
			ReadDouble(root, "intensity"), ReadDouble(root, "endurancePoints"));
		try
		{
			EmotionalSpellPolicy.ValidateState(State);
			if (ReadInt(root, "version") != 1 || DetectInvisibleEffect.PolicyError(new(Group, UnitSeconds, CapUnits)) is not null)
				throw new ArgumentException("Invalid emotional group/lifetime.");
		}
		catch (ArgumentException error) { DefinitionError = error.Message; }
	}
	protected static int ReadInt(XElement root, string name) => int.TryParse((string?)root.Attribute(name), out var value) ? value : -1;
	protected static double ReadDouble(XElement root, string name) =>
		double.TryParse((string?)root.Attribute(name), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : double.NaN;

	public string Group { get; }
	public int UnitSeconds { get; }
	public int CapUnits { get; }
	public EmotionalRetainedState State { get; private set; }
	public string? DefinitionError { get; protected set; }
	public abstract EmotionalSpellKind Kind { get; }
	public override void InitialEffect()
	{
		// EffectHandler has inserted this child before invoking InitialEffect. Only now may
		// its scheduled wrapper serialize it; earlier environmental callbacks see an empty wrapper.
		if (ParentEffect is { } parent && ReferenceEquals(parent.Owner, Owner) &&
			Owner.Effects.Contains(this) && Owner.Effects.Contains(parent) && !parent.SpellEffects.Contains(this))
		{
			parent.AddSpellEffect(this);
			Owner.EffectsChanged = true;
		}
	}
	public void ReduceSourceGrade(int amount)
	{
		if (DefinitionError is not null || amount <= 0 || amount >= State.SourceGrade)
			throw new ArgumentException("Only a valid surviving source strength may be weakened.");
		State = State with { SourceGrade = State.SourceGrade - amount };
		Owner.EffectsChanged = true;
	}

	protected XElement SaveEmotionalDefinition() => DefinitionError is not null && _retainedDefinition is not null
		? new XElement(_retainedDefinition) : new("Effect", new XAttribute("version", 1),
		new XAttribute("group", Group), new XAttribute("unitSeconds", UnitSeconds), new XAttribute("capUnits", CapUnits),
		new XAttribute("grade", State.SourceGrade), new XAttribute("nativePower", (int)State.Power),
		new XAttribute("intensity", State.Intensity), new XAttribute("endurancePoints", State.EndurancePoints));
	public override string Describe(IPerceiver voyeur) => $"{Kind}: source grade {State.SourceGrade.ToString("N0", voyeur)}, intensity {State.Intensity.ToString("N2", voyeur)}.";
}

public sealed class SpellSourceCalmEffect : SpellEmotionalEffect, IPacifismEffect, IAdmittedHostileAttackEffect
{
	public SpellSourceCalmEffect(IPerceivable owner, IMagicSpellEffectParent parent, string group,
		int unitSeconds, int capUnits, EmotionalRetainedState state, bool breakOnAdmittedAttack = true)
		: base(owner, parent, group, unitSeconds, capUnits, state) { BreakOnAdmittedAttack = breakOnAdmittedAttack; }
	private SpellSourceCalmEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var value = (string?)xml.Element("Effect")?.Attribute("attackbreak");
		if (value is null) BreakOnAdmittedAttack = true; // Version-one preparation effects had the stock default.
		else if (bool.TryParse(value, out var enabled)) BreakOnAdmittedAttack = enabled;
		else DefinitionError ??= "Invalid admitted-attack break policy.";
	}
	public bool BreakOnAdmittedAttack { get; }
	public static void InitialiseEffectType() => RegisterFactory("SpellSourceCalm", (xml, owner) => new SpellSourceCalmEffect(xml, owner));
	public override EmotionalSpellKind Kind => EmotionalSpellKind.Calm;
	protected override string SpecificEffectType => "SpellSourceCalm";
	protected override XElement SaveDefinition()
	{
		var result = SaveEmotionalDefinition();
		if (DefinitionError is null) result.SetAttributeValue("attackbreak", BreakOnAdmittedAttack);
		return result;
	}
	public void OnAdmittedHostileAttack(AdmittedHostileAttack attack)
	{
		if (BreakOnAdmittedAttack && DefinitionError is null && ReferenceEquals(attack.Recipient, Owner) &&
			Owner.Effects.Contains(this) && ParentEffect?.SpellEffects.Contains(this) == true)
			Owner.RemoveEffect(this, true);
	}
	public bool IsPeaceful => DefinitionError is not null || State.Intensity > 5;
	public bool IsSuperPeaceful => DefinitionError is not null || State.Intensity > 10;
}

public sealed class SpellSourceFuryEffect : SpellEmotionalEffect, IRageEffect, ITraitBonusEffect
{
	private bool _removing;
	public SpellSourceFuryEffect(IPerceivable owner, IMagicSpellEffectParent parent, string group,
		int unitSeconds, int capUnits, EmotionalRetainedState state, ITraitDefinition enduranceTrait,
		double unitsPerSourcePoint) : base(owner, parent, group, unitSeconds, capUnits, state)
	{
		if (enduranceTrait.TraitType != TraitType.Attribute || !double.IsFinite(unitsPerSourcePoint) || unitsPerSourcePoint <= 0 ||
			!double.IsFinite(state.EndurancePoints * unitsPerSourcePoint))
			throw new ArgumentException("Select an endurance attribute and a finite positive native-units-per-source-point mapping.");
		EnduranceTrait = enduranceTrait; UnitsPerSourcePoint = unitsPerSourcePoint;
	}
	private SpellSourceFuryEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		EnduranceTrait = long.TryParse((string?)root.Attribute("enduranceTrait"), out var traitId) ? Gameworld.Traits.Get(traitId) : null;
		UnitsPerSourcePoint = ReadDouble(root, "unitsPerSourcePoint");
		if (EnduranceTrait?.TraitType != TraitType.Attribute || !double.IsFinite(UnitsPerSourcePoint) || UnitsPerSourcePoint <= 0 ||
			!double.IsFinite(State.EndurancePoints * UnitsPerSourcePoint))
			DefinitionError ??= "Unresolved native endurance mapping.";
	}
	public static void InitialiseEffectType() => RegisterFactory("SpellSourceFury", (xml, owner) => new SpellSourceFuryEffect(xml, owner));
	public override EmotionalSpellKind Kind => EmotionalSpellKind.Fury;
	protected override string SpecificEffectType => "SpellSourceFury";
	public ITraitDefinition? EnduranceTrait { get; }
	public double UnitsPerSourcePoint { get; }
	public override void InitialEffect() { base.InitialEffect(); _removing = false; ReconcileStamina(); }
	public override void Login() { _removing = false; ReconcileStamina(); }
	public override void RemovalEffect()
	{
		// EffectHandler runs this callback before detaching the child. Exclude only
		// this departing bonus while the world's capacity prog still sees the list.
		_removing = true;
		base.RemovalEffect();
		ReconcileStamina();
	}
	private void ReconcileStamina()
	{
		if (Owner is ICharacter { Body: MudSharp.Body.Implementations.Body body }) body.ReconcileEmotionalStaminaCapacity();
	}
	public bool IsRaging => DefinitionError is not null || State.Intensity >= 5;
	public bool IsSuperRaging => DefinitionError is not null || State.Intensity >= 10;
	public bool AppliesToTrait(ITraitDefinition trait) => !_removing && DefinitionError is null && ReferenceEquals(trait, EnduranceTrait);
	public bool AppliesToTrait(ITrait trait) => trait is not null && AppliesToTrait(trait.Definition);
	public double GetBonus(ITrait trait, TraitBonusContext context = TraitBonusContext.None) =>
		AppliesToTrait(trait) ? State.EndurancePoints * UnitsPerSourcePoint : 0;
	protected override XElement SaveDefinition()
	{
		var result = SaveEmotionalDefinition();
		if (DefinitionError is not null) return result;
		result.SetAttributeValue("enduranceTrait", EnduranceTrait?.Id ?? 0);
		result.SetAttributeValue("unitsPerSourcePoint", UnitsPerSourcePoint);
		return result;
	}
}
