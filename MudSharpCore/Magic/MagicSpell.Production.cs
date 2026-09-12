using MudSharp.Effects.Concrete;
using MudSharp.Body.Traits;

#nullable enable
namespace MudSharp.Magic;

public partial class MagicSpell
{
	internal IReadOnlyList<(IMagicResource Resource, double Cost)> ProductionCosts(ICharacter creator, SpellPower power, int castingLevel, int casterLevel)
	{
		if (creator.CombinedEffectsOfType<MagicSpellLockout>().Any(x => x.Applies(School))) throw new InvalidOperationException("You are locked out from casting spells of this school.");
		var costs = _castingCosts.Select(x => (Resource: x.Key, Cost: x.Value.EvaluateWith(creator, CastingTrait, TraitBonusContext.SpellCost,
			("power", (int)power), ("self", 0), ("spelllevel", SpellLevel), ("castinglevel", castingLevel), ("casterlevel", casterLevel)))).ToArray();
		foreach (var (resource, cost) in costs)
			if (!double.IsFinite(cost) || cost < 0 || !creator.CanUseResource(resource, cost)) throw new InvalidOperationException($"Insufficient or invalid {resource.Name} cost.");
		return costs;
	}
	internal void ApplyProductionLockouts(ICharacter creator)
	{
		if (ExclusiveDelay > TimeSpan.Zero) creator.AddEffect(new MagicSpellLockout(creator, []), ExclusiveDelay);
		if (NonExclusiveDelay > TimeSpan.Zero) creator.AddEffect(new MagicSpellLockout(creator, [School]), NonExclusiveDelay);
	}
}
