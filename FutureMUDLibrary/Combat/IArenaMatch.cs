using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.Combat;

public interface IArenaMatch : IFrameworkItem, ISaveable
{
	IArenaMatchType MatchType { get; }
	ArenaMatchStage Stage { get; }
	DateTime MatchStart { get; }
	MudDateTime MatchStartMDT { get; }
	IReadOnlyCollectionDictionary<int, IArenaCombatantProfile> CombatantsByTeam { get; }
	IEnumerable<IArenaMatchBet> Bets { get; }
	int CurrentRound { get; }
	DateTime? CurrentRoundEndTime { get; }
	DateTime? CurrentIntervalEndTime { get; }

	void AddCombatant(IArenaCombatantProfile combatant, int team);
	void WithdrawCombatant(IArenaCombatantProfile combatant);
	void HandleScore(ICharacter combatant, ICharacter target, ICombatMove move, CombatMoveResult result);
}
