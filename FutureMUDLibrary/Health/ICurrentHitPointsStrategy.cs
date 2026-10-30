using System.Linq;

#nullable enable
namespace MudSharp.Health;

/// <summary>Optional computed current-HP accounting for models with a separate healing-capacity ceiling.</summary>
public interface ICurrentHitPointsStrategy
{
	double CurrentHitPoints(IHaveWounds owner);
}

public static class HealthStrategyHitPointExtensions
{
	/// <summary>Uses model-specific current HP when supplied, otherwise preserving native max-minus-wounds semantics.</summary>
	public static double CurrentHitPoints(this IHealthStrategy strategy, IHaveWounds owner) =>
		strategy is ICurrentHitPointsStrategy current ? current.CurrentHitPoints(owner) : strategy.MaxHP(owner) - owner.Wounds.Sum(wound => wound.CurrentDamage);
}
