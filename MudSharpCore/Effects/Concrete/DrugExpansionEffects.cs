#nullable enable

using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Effects.Concrete;

public sealed class DrugTraitModifierEffect : Effect, ITraitBonusEffect, IMaximumHitPointsBonusEffect
{
	public DrugTraitModifierEffect(IPerceivable owner) : base(owner) { }
	protected override string SpecificEffectType => "DrugTraitModifier";
	public Dictionary<long, double> TraitBonuses { get; } = new();
	public long SourceBodyId { get; set; }
	public double MaximumHitPointsBonus { get; set; }
	public double MaximumHitPointsMultiplier { get; set; } = 1.0;
	public bool AppliesToTrait(ITraitDefinition trait) => TraitBonuses.ContainsKey(trait.Id);
	public bool AppliesToTrait(ITrait trait) => AppliesToTrait(trait.Definition);
	public double GetBonus(ITrait trait, TraitBonusContext context = TraitBonusContext.None) => TraitBonuses.GetValueOrDefault(trait.Definition.Id);
	private bool IsCurrentSource => Owner is not ICharacter actor || SourceBodyId <= 0 || actor.Body?.Id == SourceBodyId;
	public override bool Applies() => IsCurrentSource && base.Applies();
	public override bool Applies(object target) => IsCurrentSource && base.Applies(target);
	public override bool Applies(object target, object thirdparty) => IsCurrentSource && base.Applies(target, thirdparty);
	public override bool Applies(object target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) => IsCurrentSource && base.Applies(target, flags);
	public override string Describe(IPerceiver voyeur) => "Temporary drug trait and maximum health modifiers.";
}

public sealed class DrugCombatCheckModifierEffect : Effect, ICheckBonusEffect
{
	public DrugCombatCheckModifierEffect(IBody owner, bool offensive) : base(owner) => Offensive = offensive;
	public bool Offensive { get; }
	public double CheckBonus { get; set; }
	protected override string SpecificEffectType => Offensive ? "DrugOffensiveCheckModifier" : "DrugDefensiveCheckModifier";
	public bool AppliesToCheck(CheckType type) => Offensive ? type.IsOffensiveCombatAction() : type.IsDefensiveCombatAction();
	public override string Describe(IPerceiver voyeur) => $"Drug {(Offensive ? "offensive" : "defensive")} check modifier.";
}

public sealed class DrugResourceCapacityModifierEffect : Effect, IMagicResourceCapacityBonusEffect
{
	public DrugResourceCapacityModifierEffect(IBody owner, long resourceId) : base(owner) => MagicResourceId = resourceId;
	protected override string SpecificEffectType => "DrugResourceCapacityModifier";
	public long MagicResourceId { get; }
	public double MagicResourceCapacityBonus { get; set; }
	public double MagicResourceCapacityMultiplier { get; set; } = 1.0;
	public override string Describe(IPerceiver voyeur) => $"Temporary drug capacity modifier for resource {MagicResourceId}.";
}

public class DrugCoagulationEffect : Effect, IBleedingModifierEffect
{
	public DrugCoagulationEffect(IBody owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "DrugCoagulation";

	public double ExternalBleedingMultiplier { get; set; } = 1.0;
	public double WoundReopenMultiplier { get; set; } = 1.0;
	public double InternalBleedingMultiplier { get; set; } = 1.0;

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Drug coagulation modifier: external {ExternalBleedingMultiplier.ToStringP2(voyeur)}, reopen {WoundReopenMultiplier.ToStringP2(voyeur)}, internal {InternalBleedingMultiplier.ToStringP2(voyeur)}";
	}
}

public class DrugRespirationEffect : Effect, IRespirationModifierEffect
{
	public DrugRespirationEffect(IBody owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "DrugRespiration";

	public double BreathingDriveMultiplier { get; set; } = 1.0;
	public double HypoxiaDamageMultiplier { get; set; } = 1.0;
	public double AirwayToleranceMultiplier { get; set; } = 1.0;

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Drug respiration modifier: drive {BreathingDriveMultiplier.ToStringP2(voyeur)}, hypoxia {HypoxiaDamageMultiplier.ToStringP2(voyeur)}, airway {AirwayToleranceMultiplier.ToStringP2(voyeur)}";
	}
}

public class DrugNeedRateEffect : Effect, INeedRateEffect
{
	public DrugNeedRateEffect(IBody owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "DrugNeedRate";

	public double HungerMultiplier { get; set; } = 1.0;
	public double ThirstMultiplier { get; set; } = 1.0;
	public double DrunkennessMultiplier { get; set; } = 1.0;
	public bool AppliesToPassive { get; set; }
	public bool AppliesToActive { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Drug need rate modifier: hunger {HungerMultiplier.ToStringP2(voyeur)}, thirst {ThirstMultiplier.ToStringP2(voyeur)}, drunkenness {DrunkennessMultiplier.ToStringP2(voyeur)}";
	}
}

public class DrugArousalEffect : Effect, ICheckBonusEffect, IConsciousnessThresholdModifierEffect,
	IStaminaRegenerationRateEffect, IStaminaExpenditureEffect
{
	public DrugArousalEffect(IBody owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "DrugArousal";

	public double Intensity { get; set; }
	public double CheckBonusPerIntensity { get; set; }
	public double PainPassOutThresholdMultiplier { get; set; } = 1.0;
	public double StunUnconsciousThresholdMultiplier { get; set; } = 1.0;
	public double AnesthesiaUnconsciousThresholdMultiplier { get; set; } = 1.0;
	public double StaminaRegenMultiplier { get; set; } = 1.0;
	public double StaminaCostMultiplier { get; set; } = 1.0;

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsGeneralActivityCheck();
	}

	public double CheckBonus => CheckBonusPerIntensity * Intensity;
	double IStaminaRegenerationRateEffect.Multiplier => StaminaRegenMultiplier;
	double IStaminaExpenditureEffect.Multiplier => StaminaCostMultiplier;

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Drug arousal modifier: intensity {Intensity.ToString("N2", voyeur)}, check {CheckBonus.ToString("N2", voyeur)}, pain threshold {PainPassOutThresholdMultiplier.ToStringP2(voyeur)}";
	}
}

public class DrugPassOutResistanceEffect : Effect, IPreventPassOut
{
	public DrugPassOutResistanceEffect(IBody owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "DrugPassOutResistance";

	public override string Describe(IPerceiver voyeur)
	{
		return "Drug-induced resistance to passing out.";
	}
}

public class DrugKnockoutEffect : Effect, ILossOfConsciousnessEffect
{
	public DrugKnockoutEffect(IBody owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "DrugKnockout";

	public HealthTickResult UnconType => HealthTickResult.PassOut;

	public override void RemovalEffect()
	{
		base.RemovalEffect();
		((IBody)Owner).CheckHealthStatus();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Drug-induced loss of consciousness.";
	}
}

public class DrugSleepEffect : Effect, ISleepEffect
{
	public DrugSleepEffect(ICharacter owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "DrugSleep";

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (Owner is not ICharacter character ||
			character.State.HasFlag(CharacterState.Sleeping) ||
			character.Combat is not null ||
			character.PositionState.CompareTo(character.Race.MinimumSleepingPosition) == PositionHeightComparison.Higher ||
			character.EffectsOfType<IPreventSleepEffect>().Any(effect => effect.Applies(character)))
		{
			return;
		}

		character.Sleep();
	}

	public override void RemovalEffect()
	{
		if (Owner is ICharacter character &&
			character.State.HasFlag(CharacterState.Sleeping) &&
			character.EffectsOfType<ISleepEffect>().Count() <= 1)
		{
			character.Awaken();
		}

		base.RemovalEffect();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Drug-induced sleep.";
	}
}

public class DrugSleepPreventionEffect : Effect, IPreventSleepEffect
{
	public DrugSleepPreventionEffect(ICharacter owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "DrugSleepPrevention";
	public string SleepPreventionEcho => "Drug-induced wakefulness keeps you from sleeping.";

	public override string Describe(IPerceiver voyeur)
	{
		return "Drug-induced wakefulness.";
	}
}

public class DrugWithdrawalEffect : Effect, ICauseDrugEffect, ICheckBonusEffect, INeedRateEffect,
	IStaminaRegenerationRateEffect, IStaminaExpenditureEffect
{
	public DrugWithdrawalEffect(IBody owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "DrugWithdrawal";

	public Dictionary<DrugType, double> DrugIntensities { get; } = new();
	public double WithdrawalIntensity { get; set; }
	public double CheckBonus { get; set; }
	public double HungerMultiplier { get; set; } = 1.0;
	public double ThirstMultiplier { get; set; } = 1.0;
	public double DrunkennessMultiplier { get; set; } = 1.0;
	public bool AppliesToPassive => true;
	public bool AppliesToActive => true;
	public double StaminaRegenMultiplier { get; set; } = 1.0;
	public double StaminaCostMultiplier { get; set; } = 1.0;

	public IEnumerable<DrugType> AffectedDrugTypes => DrugIntensities.Keys;

	public double AddedIntensity(ICharacter character, DrugType drugtype)
	{
		return DrugIntensities.TryGetValue(drugtype, out var value) ? value : 0.0;
	}

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsGeneralActivityCheck();
	}

	double IStaminaRegenerationRateEffect.Multiplier => StaminaRegenMultiplier;
	double IStaminaExpenditureEffect.Multiplier => StaminaCostMultiplier;

	public override string Describe(IPerceiver voyeur)
	{
		return $"Drug withdrawal intensity {WithdrawalIntensity.ToString("N2", voyeur)}.";
	}
}

public class DrugWithdrawalSleepPreventionEffect : Effect, IPreventSleepEffect
{
	public DrugWithdrawalSleepPreventionEffect(ICharacter owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "DrugWithdrawalSleepPrevention";
	public string SleepPreventionEcho => "Withdrawal restlessness keeps you from sleeping.";

	public override string Describe(IPerceiver voyeur)
	{
		return "Withdrawal restlessness.";
	}
}
