#nullable enable

using System;

namespace MudSharp.Magic.Environment;

/// <summary>Round the resulting scar upward when needed, so the actual decrement never exceeds earned work.</summary>
public static class ConservativeScarRepair
{
	public static (double Remaining, double Applied) Calculate(double scar, double allowance)
	{
		if (!double.IsFinite(scar) || scar < 0.0 || !double.IsFinite(allowance) || allowance < 0.0)
			throw new ArgumentOutOfRangeException(nameof(allowance));
		allowance = Math.Min(scar, allowance);
		var remaining = scar - allowance;
		if (scar - remaining > allowance) remaining = Math.BitIncrement(remaining);
		var applied = scar - remaining;
		return applied > 0.0 && applied <= allowance ? (remaining, applied) : (scar, 0.0);
	}
}
