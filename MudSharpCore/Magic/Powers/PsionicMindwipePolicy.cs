#nullable enable

namespace MudSharp.Magic.Powers;

/// <summary>Pure source-derived conversions for mindwipe duration and reserve losses.</summary>
public static class PsionicMindwipePolicy
{
	public static int DurationTicks(double learnedSkill)
	{
		if (!double.IsFinite(learnedSkill) || learnedSkill < 0.0 || learnedSkill < 20.0) return 1;
		if (learnedSkill >= 90.0) return 8;
		return (int)Math.Floor(learnedSkill / 10.0) - 1;
	}

	public static TimeSpan Duration(double learnedSkill) => TimeSpan.FromSeconds(DurationTicks(learnedSkill) * 600.0);

	public static double QuarterMaximum(double maximum)
	{
		return double.IsFinite(maximum) && maximum > 0.0 ? maximum / 4.0 : 0.0;
	}

	public static double FractionOfMaximum(double maximum, double fraction)
	{
		return double.IsFinite(maximum) && maximum > 0.0 && double.IsFinite(fraction) && fraction is >= 0.0 and <= 1.0
			? maximum * fraction
			: 0.0;
	}
}
