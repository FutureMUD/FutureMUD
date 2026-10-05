#nullable enable

using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class DispelMagicEffect
{
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null) return new(MagicEffectOperationStatus.Rejected, null);
		var targets = target.EffectsOfType<IDispelMagicProxyEffect>().SelectMany(x => x.AdditionalDispelTargets)
			.Where(x => x is not null).Prepend(target).Distinct().ToArray();
		var before = targets.SelectMany(owner => owner.EffectsOfType<MagicSpellParent>()
			.Where(parent => MatchesParent(caster, parent) && MatchesContest(power, outcome, parent))
			.Select(parent => (Owner: owner, Parent: parent, Duration: Duration(parent)))).ToArray();
		GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
		return new(before.Any(x => !x.Owner.EffectsOfType<MagicSpellParent>().Contains(x.Parent) ||
			Mode == DispelMagicMode.Shorten && Duration(x.Parent) < x.Duration)
			? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
	}

	// Scheduled duration is stable during ordinary clock countdown. Comparing remaining
	// time would report a no-op as applied merely because time elapsed between reads.
	private double Duration(MagicSpellParent parent) => parent is SubstanceExposureEffect { IsTimed: true } substance
		? substance.RemainingSeconds : Gameworld.EffectScheduler.OriginalDuration(parent).TotalSeconds;
}
