using MudSharp.Framework;
using MudSharp.RPG.Law.PatrolStrategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.RPG.Law;

public static class PatrolStrategyFactory
{
    public static IEnumerable<string> Strategies { get; } = new string[]
    {
        "ArmedPatrol",
        "ReactivePatrol",
        "InvestigationPatrol",
        "CorpseRecovery",
        "StationEnforcer",
        "Judge",
        "Sheriff",
        "Prosecutor",
        "ExecutionPatrol"
    };

    public static IPatrolStrategy GetStrategy(string which, IFuturemud gameworld)
    {
        return GetStrategy(which, null, gameworld);
    }

    public static IPatrolStrategy GetStrategy(string which, string strategyData, IFuturemud gameworld)
    {
        switch (which.ToLowerInvariant())
        {
            case "armedpatrol":
                return new ArmedPatrolStrategy(gameworld);
            case "reactive":
            case "reactivepatrol":
            case "response":
            case "responsepatrol":
                return new ReactivePatrolStrategy(gameworld, strategyData);
            case "investigation":
            case "investigationpatrol":
            case "investigation patrol":
                return new InvestigationPatrolStrategy(gameworld, strategyData);
            case "corpserecovery":
            case "corpse recovery":
                return new CorpseRecoveryPatrolStrategy(gameworld);
            case "stationenforcer":
                return new StationEnforcerPatrolStrategy(gameworld);
            case "judge":
                return new JudgePatrolStrategy(gameworld);
            case "sheriff":
                return new SheriffPatrolStrategy(gameworld);
            case "prosecutor":
                return new ProsectutorPatrolStrategy(gameworld);
            case "execution":
            case "executionpatrol":
                return new ExecutionPatrolStrategy(gameworld, strategyData);
            default:
                throw new NotImplementedException($"Unimplemented IPatrolStrategy '{which}' in PatrolStrategyFactory.");
        }
    }
}
