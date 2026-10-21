#nullable enable

using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Body.Needs;

/// <summary>Opt-in environmental contributions. Counter rates and litres remain independent channels.</summary>
internal static class EnvironmentalNeedsRates
{
	internal static (double Hunger, double Thirst, double Water) Calculate(double hunger, double thirst,
		double water, IEnumerable<EnvironmentalNeedsAdjustment> adjustments)
	{
		var configured = adjustments.ToArray();
		if (configured.Length == 0) return (hunger, thirst, water);
		double? hungerCap = null;
		double? thirstCap = null;
		foreach (var adjustment in configured)
		{
			if (double.IsFinite(adjustment.HungerRateDelta)) hunger += adjustment.HungerRateDelta;
			if (double.IsFinite(adjustment.ThirstRateDelta)) thirst += adjustment.ThirstRateDelta;
			if (double.IsFinite(adjustment.WaterLossLitresPerHour)) water += adjustment.WaterLossLitresPerHour;
			if (adjustment.MaximumHungerRate is { } h && double.IsFinite(h) && h >= 0)
				hungerCap = Math.Min(hungerCap ?? double.MaxValue, h);
			if (adjustment.MaximumThirstRate is { } t && double.IsFinite(t) && t >= 0)
				thirstCap = Math.Min(thirstCap ?? double.MaxValue, t);
		}
		return (Clamp(hunger, hungerCap), Clamp(thirst, thirstCap), Clamp(water, null));
	}

	private static double Clamp(double value, double? cap) =>
		double.IsFinite(value) ? Math.Clamp(value, 0, cap ?? double.MaxValue) : 0;
}
