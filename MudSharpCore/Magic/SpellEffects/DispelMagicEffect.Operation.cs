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
		if (Mode == DispelMagicMode.Shorten && ShortenDuration.Ticks <= 0)
			return new(MagicEffectOperationStatus.NoChange, null);
		var targets = target.EffectsOfType<IDispelMagicProxyEffect>().SelectMany(x => x.AdditionalDispelTargets)
			.Where(x => x is not null).Prepend(target).Distinct().ToArray();
		var before = targets.SelectMany(owner => owner.EffectsOfType<MagicSpellParent>()
			.Where(parent => MatchesParent(caster, parent) && MatchesContest(power, outcome, parent))
			.Select(parent => (Owner: owner, Parent: parent, Boundary: Boundary(parent)))).ToArray();
		GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
		if (before.Any(x => !x.Owner.EffectsOfType<MagicSpellParent>().Contains(x.Parent) ||
			Mode == DispelMagicMode.Shorten && x.Boundary is {} original && Boundary(x.Parent) is {} current && current < original))
			return new(MagicEffectOperationStatus.Applied, null);
		// A real observed removal/expiry reduction wins even if another scheduler target
		// is unobservable. Otherwise preserve uncertainty rather than invent application.
		return new(Mode == DispelMagicMode.Shorten && before.Any(x => x.Boundary is null || Boundary(x.Parent) is null)
			? MagicEffectOperationStatus.Unknown : MagicEffectOperationStatus.NoChange, null);
	}

	private decimal? Boundary(MagicSpellParent parent) => parent is SubstanceExposureEffect substance
		? substance.IsTimed ? (decimal)substance.RemainingSeconds : decimal.MaxValue
		: Gameworld.EffectScheduler is IEffectExpiryObserver observer
			? observer.ScheduledExpiry(parent)?.Ticks ?? decimal.MaxValue : null;
}
