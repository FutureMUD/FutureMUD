using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;

namespace MudSharp.Combat;

public interface IArenaMatch : IFrameworkItem, ISaveable
{
	IArenaMatchType MatchType { get; }
	ArenaMatchStage Stage { get; }
	DateTime MatchStart { get; }
	MudDateTime MatchStartMDT { get; }
	IEnumerable<IArenaCombatantProfile> CombatantsTeam1 { get; }
	IEnumerable<IArenaCombatantProfile> CombatantsTeam2 { get; }
	IEnumerable<IArenaCombatantProfile> CombatantsTeam3 { get; }
	IEnumerable<IArenaCombatantProfile> CombatantsTeam4 { get; }
	IEnumerable<IArenaMatchBet> Bets { get; }
	int CurrentRound { get; }
	DateTime? CurrentRoundEndTime { get; }
	DateTime? CurrentIntervalEndTime { get; }
}
