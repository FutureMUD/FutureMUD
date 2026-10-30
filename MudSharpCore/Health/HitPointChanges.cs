using MudSharp.Character;
using MudSharp.Framework;

#nullable enable
namespace MudSharp.Health;

/// <summary>Explicit nonmagical HP repair or harm using native wounds, without changing base attributes.</summary>
public static class HitPointChanges
{
	public static bool Apply(ICharacter actor, double amount)
	{
		if (!double.IsFinite(amount) || actor.State.IsDead() || actor.Body is null ||
			actor.HealthStrategy is not ICurrentHitPointsStrategy) return false;
		var body = actor.Body;
		if (amount >= 0)
		{
			var remaining = amount;
			foreach (var wound in actor.Wounds.OrderByDescending(x => x.CurrentDamage).ToArray())
			{
				if (!ReferenceEquals(actor.Body, body) || actor.State.IsDead()) return false;
				var before = wound.CurrentDamage;
				wound.CurrentDamage = Math.Max(0, before - remaining);
				remaining -= Math.Max(0, before - wound.CurrentDamage);
				if (remaining <= 0) break;
			}
		}
		else
		{
			DirectHealthCostPlan? plan = null;
			foreach (var part in body.Bodyparts)
			{
				if (!actor.HealthStrategy.TryPlanDirectHealthConsequence(actor, part, -amount, 0, 0, out var candidate, out _)) continue;
				plan = candidate;
				break;
			}
			if (plan is null) return false;
			var wounds = actor.HealthStrategy.ApplyDirectHealthCost(actor, plan);
			if (wounds.Count == 0) return false;
			actor.AddWounds(wounds.Where(x => !actor.Wounds.Contains(x)).ToArray());
			foreach (var wound in wounds)
			{
				if (!ReferenceEquals(actor.Body, body) || actor.State.IsDead()) return false;
				actor.ProcessPassiveWound(wound);
			}
		}
		if (!ReferenceEquals(actor.Body, body)) return false;
		actor.CheckHealthStatus();
		if (ReferenceEquals(actor.Body, body) && !actor.State.IsDead()) actor.StartHealthTick();
		return true;
	}
}
