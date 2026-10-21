#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits;

internal static class PhysiologyMeritEffects
{
	internal static double StaminaCapacity(double baseline, ICharacter actor, IBody? body = null)
	{
		if (body is not null && actor is MudSharp.Character.Character runtime && !ReferenceEquals(actor.Body, body))
			return runtime.EvaluateMeritsForBody(body, () => StaminaCapacity(baseline, actor, body));
		var effective = body is null ? actor.Merits :
			(actor.ExplicitMerits ?? Enumerable.Empty<IMerit>()).Concat(body.Merits ?? Enumerable.Empty<IMerit>());
		effective ??= Enumerable.Empty<IMerit>();
		var modifiers = effective.DistinctBy(x => x.Id).OfType<IStaminaCapacityMerit>()
			.Where(x => x.Applies(actor)).ToArray();
		var bonus = modifiers.Where(x => double.IsFinite(x.StaminaBonus)).Sum(x => x.StaminaBonus);
		var multiplier = modifiers.Where(x => double.IsFinite(x.StaminaMultiplier) && x.StaminaMultiplier >= 0)
			.Aggregate(1.0, (value, merit) => value * merit.StaminaMultiplier);
		var result = (baseline + bonus) * multiplier;
		return double.IsFinite(result) ? Math.Max(0, result) : Math.Max(0, baseline);
	}
}
