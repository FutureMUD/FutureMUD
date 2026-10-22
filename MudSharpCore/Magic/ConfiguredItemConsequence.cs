using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Health;

#nullable enable
namespace MudSharp.Magic;

/// <summary>Applies authored independent bodily channels without armour multiplication or health-floor overshoot.</summary>
public static class ConfiguredItemConsequence
{
	public static bool Apply(ICharacter target, ItemConsequence consequence)
	{
		if (!TryPlan(target, consequence, out var plan, out var stamina, voluntaryCost: false)) return false;
		if (plan is not null)
		{
			var wounds = target.HealthStrategy.ApplyDirectHealthCost(target, plan);
			if (wounds.Count == 0) return false;
			target.AddWounds(wounds.Where(x => !target.Wounds.Contains(x)).ToArray());
			foreach (var wound in wounds) target.ProcessPassiveWound(wound);
		}
		target.Body.SpendStamina(Math.Min(target.Body.CurrentStamina, stamina));
		if (consequence.Collapse)
			target.SetPosition(PositionSprawled.Instance, PositionModifier.None, null, null);
		if (!string.IsNullOrWhiteSpace(consequence.Message)) target.Send(consequence.Message);
		target.EvaluateWounds();
		target.StartHealthTick();
		return true;
	}
	public static bool CanApply(ICharacter target, ItemConsequence consequence) => TryPlan(target, consequence, out _, out _, voluntaryCost: true);
	private static bool TryPlan(ICharacter target, ItemConsequence consequence, out DirectHealthCostPlan? plan, out double stamina, bool voluntaryCost)
	{
		plan = null;
		stamina = 0;
		if (!consequence.IsValid || target.Body is null || target.HealthStrategy is null || target.State.IsDead()) return false;
		var remaining = target.HealthStrategy.MaxHP(target) - target.Wounds.Sum(x => x.CurrentDamage);
		if (!double.IsFinite(remaining)) return false;
		var health = consequence.HealthLoss(remaining, target.HealthStrategy.MaxHP(target));
		var stun = consequence.Stun + consequence.StunFraction * target.HealthStrategy.MaxStun(target);
		stamina = consequence.Stamina + consequence.StaminaFraction * target.Body.MaximumStamina;
		if (!double.IsFinite(health) || !double.IsFinite(stun) || !double.IsFinite(stamina)) return false;
		if (health > 0 || stun > 0)
		{
			foreach (var part in target.Body.Bodyparts)
			{
				DirectHealthCostPlan candidate;
				var accepted = voluntaryCost
					? target.HealthStrategy.TryPlanDirectHealthCost(target, part, health, 0, stun, WoundSeverity.Horrifying, out candidate, out _)
					: target.HealthStrategy.TryPlanDirectHealthConsequence(target, part, health, 0, stun, out candidate, out _);
				if (!accepted) continue;
				if (Math.Abs(candidate.ExpectedDamage - health) > 0.000001 || Math.Abs(candidate.ExpectedStun - stun) > 0.000001) continue;
				plan = candidate;
				break;
			}
			if (plan is null) return false;
		}
		return true;
	}
}
