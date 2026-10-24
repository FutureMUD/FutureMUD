#nullable enable
using System;

namespace MudSharp.Magic;

public readonly record struct PsionicRejuvenationPlan(double Change, double StunCost);

/// <summary>Converts remaining stun into recovery, or a bounded loss on failure.</summary>
public static class PsionicRejuvenationPolicy
{
	public static PsionicRejuvenationPlan Plan(double skill, double remainingStun, double maximumStun,
		double current, double minimum, double maximum, bool success)
	{
		if (!double.IsFinite(skill) || !double.IsFinite(remainingStun) || !double.IsFinite(maximumStun) ||
			!double.IsFinite(current) || !double.IsFinite(minimum) || !double.IsFinite(maximum) ||
			maximumStun <= 0 || maximum <= minimum || current >= maximum || current < minimum) return default;
		skill = Math.Clamp(skill, 0, 100);
		var budget = Math.Min(Math.Max(0, remainingStun - 1), skill * maximumStun / 100);
		var conversion = (200 - skill) / 100;
		var gain = budget / conversion;
		var change = success ? Math.Min(maximum - current, gain) : -Math.Min(current - minimum, gain * 2 / 3);
		return new(change, Math.Abs(change) * conversion);
	}
}
