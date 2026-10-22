#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Effects.Concrete;

/// <summary>A short lived, non-magical trait penalty caused by a pressure point strike.</summary>
public sealed class MundaneTraitPenaltyEffect : Effect, ITraitBonusEffect
{
	public static void InitialiseEffectType() => RegisterFactory("MundaneTraitPenalty",
		(effect, owner) => new MundaneTraitPenaltyEffect(effect, owner));

	public MundaneTraitPenaltyEffect(IPerceivable owner, ITraitDefinition trait, double penalty) : base(owner)
	{
		Trait = trait;
		Penalty = penalty;
	}

	private MundaneTraitPenaltyEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var definition = root.Element("Effect")!;
		Trait = Gameworld.Traits.Get(long.Parse(definition.Element("Trait")!.Value))!;
		Penalty = double.Parse(definition.Element("Penalty")!.Value,
			System.Globalization.CultureInfo.InvariantCulture);
	}

	public ITraitDefinition Trait { get; }
	public double Penalty { get; }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "MundaneTraitPenalty";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Trait", Trait.Id),
		new XElement("Penalty", Penalty.ToString(System.Globalization.CultureInfo.InvariantCulture)));
	public bool AppliesToTrait(ITraitDefinition trait) => trait == Trait;
	public bool AppliesToTrait(ITrait trait) => trait?.Definition == Trait;
	public double GetBonus(ITrait trait, TraitBonusContext context = TraitBonusContext.None) =>
		AppliesToTrait(trait) ? Penalty : 0.0;
	public override string Describe(IPerceiver voyeur) => $"Temporarily weakened {Trait.Name}.";
}

/// <summary>A short lived, non-magical paralysis status.</summary>
public sealed class MundaneParalysisEffect : Effect, IForceParalysisEffect
{
	public static void InitialiseEffectType() => RegisterFactory("MundaneParalysis",
		(effect, owner) => new MundaneParalysisEffect(effect, owner));

	public MundaneParalysisEffect(IPerceivable owner) : base(owner) { }
	private MundaneParalysisEffect(XElement root, IPerceivable owner) : base(root, owner) { }
	public bool ShouldParalyse => true;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "MundaneParalysis";
	public override string Describe(IPerceiver voyeur) => "Temporarily paralysed by a pressure point strike.";
}

/// <summary>A short lived, non-magical physical speech restriction.</summary>
public sealed class MundaneSilenceEffect : Effect, ISilencedEffect
{
	public static void InitialiseEffectType() => RegisterFactory("MundaneSilence",
		(effect, owner) => new MundaneSilenceEffect(effect, owner));

	public MundaneSilenceEffect(IPerceivable owner) : base(owner) { }
	private MundaneSilenceEffect(XElement root, IPerceivable owner) : base(root, owner) { }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "MundaneSilence";
	public override string Describe(IPerceiver voyeur) => "Temporarily unable to speak from a pressure point strike.";
}
