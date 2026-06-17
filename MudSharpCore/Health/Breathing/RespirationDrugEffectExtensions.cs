using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using System.Linq;

namespace MudSharp.Health.Breathing;

public static class RespirationDrugEffectExtensions
{
	public static double RespirationBreathingDriveMultiplier(this IBody body)
	{
		return body.EffectsOfType<IRespirationModifierEffect>()
		           .Where(x => x.Applies())
		           .Aggregate(1.0, (current, effect) => current * effect.BreathingDriveMultiplier);
	}

	public static double RespirationHypoxiaDamageMultiplier(this IBody body)
	{
		return body.EffectsOfType<IRespirationModifierEffect>()
		           .Where(x => x.Applies())
		           .Aggregate(1.0, (current, effect) => current * effect.HypoxiaDamageMultiplier);
	}

	public static double RespirationAirwayToleranceMultiplier(this IBody body)
	{
		return body.EffectsOfType<IRespirationModifierEffect>()
		           .Where(x => x.Applies())
		           .Aggregate(1.0, (current, effect) => current * effect.AirwayToleranceMultiplier);
	}
}
