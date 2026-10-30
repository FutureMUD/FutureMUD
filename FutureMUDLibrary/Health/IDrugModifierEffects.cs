using MudSharp.Effects;

#nullable enable
namespace MudSharp.Health;

/// <summary>A separately timed consequence that can be selected explicitly when cleansing one drug.</summary>
public interface IDrugAftereffect : IEffectSubtype
{
	long SourceDrugId { get; }
	string? SourceFamily { get; }
}

/// <summary>Prevents spontaneous stun recovery, without preventing explicit treatment or magical healing.</summary>
public interface IPreventNaturalStunRecoveryEffect : IEffectSubtype
{
	bool PreventNaturalStunRecovery { get; }
}

/// <summary>Opt-in policy for native forced paralysis to clear the affected participant's combat targeting.</summary>
public interface ICeaseCombatOnParalysisEffect : IEffectSubtype
{
	bool CeaseCombatOnParalysis { get; }
}

/// <summary>An opt-in capacity penalty that need not also subtract current HP in supporting health strategies.</summary>
public interface ICapacityOnlyHitPointsPenalty : IMaximumHitPointsBonusEffect
{
	bool CapacityOnly { get; }
}

/// <summary>Optional native HP-model ceiling applied to natural healing and explicit wound repair alike.</summary>
public interface IHitPointHealingCeiling
{
	double ConstrainHealedDamage(IWound wound, double proposedDamage);
}

/// <summary>Temporary additive maximum hit point change supplied by an active native effect.</summary>
public interface IMaximumHitPointsBonusEffect : IEffectSubtype
{
	double MaximumHitPointsBonus { get; }
	double MaximumHitPointsMultiplier { get; }
}

/// <summary>Temporary additive capacity change for one exact magic resource.</summary>
public interface IMagicResourceCapacityBonusEffect : IEffectSubtype
{
	long MagicResourceId { get; }
	double MagicResourceCapacityBonus { get; }
	double MagicResourceCapacityMultiplier { get; }
}
