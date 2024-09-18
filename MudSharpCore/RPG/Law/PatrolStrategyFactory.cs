using MudSharp.RPG.Law.PatrolStrategies;
using System;
using System.Collections.Generic;
using System.Text;
using MudSharp.Framework;

namespace MudSharp.RPG.Law;

public static class PatrolStrategyFactory
{
	public static IEnumerable<string> Strategies { get; } = new string[]
	{
		"ArmedPatrol",
		"StationEnforcer",
		"Judge",
		"Sheriff"
	};

	public static IPatrolStrategy GetStrategy(string which, IFuturemud gameworld)
	{
		switch (which.ToLowerInvariant())
		{
			case "armedpatrol":
				return new ArmedPatrolStrategy(gameworld);
			case "stationenforcer":
				return new StationEnforcerPatrolStrategy(gameworld);
			case "judge":
				return new JudgePatrolStrategy(gameworld);
			case "sheriff":
				return new SheriffPatrolStrategy(gameworld);
			default:
				throw new NotImplementedException($"Unimplemented IPatrolStrategy '{which}' in PatrolStrategyFactory.");
		}
	}
}