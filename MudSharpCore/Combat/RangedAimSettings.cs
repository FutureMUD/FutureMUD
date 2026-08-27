using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Combat;

public static class RangedAimSettings
{
	public static double PostureMultiplier(IFuturemud gameworld, IPositionState position)
	{
		if (position.CompareTo(PositionKneeling.Instance) == PositionHeightComparison.Equivalent)
		{
			return gameworld.GetStaticDouble("RangedAimKneelingMultiplier");
		}

		return position.CompareTo(PositionKneeling.Instance) == PositionHeightComparison.Lower
			? gameworld.GetStaticDouble("RangedAimProneMultiplier")
			: gameworld.GetStaticDouble("RangedAimStandingMultiplier");
	}

	public static double CombatOutcomeMultiplier(IFuturemud gameworld, Outcome outcome)
	{
		return outcome switch
		{
			Outcome.Fail => gameworld.GetStaticDouble("CombatAimFailMultiplier"),
			Outcome.MinorFail => gameworld.GetStaticDouble("CombatAimMinorFailMultiplier"),
			Outcome.MinorPass => gameworld.GetStaticDouble("CombatAimMinorPassMultiplier"),
			Outcome.Pass => gameworld.GetStaticDouble("CombatAimPassMultiplier"),
			Outcome.MajorPass => gameworld.GetStaticDouble("CombatAimMajorPassMultiplier"),
			_ => 0.0
		};
	}

	public static double OutOfCombatOutcomeMultiplier(IFuturemud gameworld, Outcome outcome)
	{
		return outcome switch
		{
			Outcome.MajorFail => gameworld.GetStaticDouble("OutOfCombatAimMajorFailMultiplier"),
			Outcome.Fail => gameworld.GetStaticDouble("OutOfCombatAimFailMultiplier"),
			Outcome.MinorFail => gameworld.GetStaticDouble("OutOfCombatAimMinorFailMultiplier"),
			Outcome.MinorPass => gameworld.GetStaticDouble("OutOfCombatAimMinorPassMultiplier"),
			Outcome.Pass => gameworld.GetStaticDouble("OutOfCombatAimPassMultiplier"),
			Outcome.MajorPass => gameworld.GetStaticDouble("OutOfCombatAimMajorPassMultiplier"),
			_ => 0.0
		};
	}
}
