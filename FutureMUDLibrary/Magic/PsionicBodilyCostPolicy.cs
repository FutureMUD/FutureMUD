using System;

#nullable enable
namespace MudSharp.Magic;

public sealed record PsionicBodilyCostPolicy(int Base, long SkillTraitId, long WillpowerTraitId, string[] Verbs)
{
	public static int Price(int basis, double skill, double willpower, int randomAddition)
	{
		if (basis < 0 || !double.IsFinite(skill) || !double.IsFinite(willpower) || randomAddition is < 0 or > 3) throw new ArgumentOutOfRangeException();
		var average = ((int)Math.Clamp(willpower, 1, 100) + (int)Math.Clamp(skill, 1, 100)) / 2;
		return (int)Math.Clamp((long)basis + (long)basis * (50 - average) / 100 + randomAddition, 0, int.MaxValue);
	}
}

public sealed record PsionicStunUpkeepPolicy(long SkillTraitId, long WisdomTraitId, long FreeProgId, int AttemptsPerMinute = 60);
