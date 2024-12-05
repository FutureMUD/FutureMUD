#nullable enable
using System;
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Combat.Arenas;

internal class ArenaMatch : SaveableItem, IArenaMatch
{
	public ArenaMatch(ICombatArena arena, IArenaMatchType type)
	{
		Gameworld = arena.Gameworld;
		Arena = arena;
		MatchType = type;
		Stage = ArenaMatchStage.OpenForRegistration;
		MatchStart = DateTime.UtcNow;
		MatchStartMDT = Arena.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		CurrentIntervalEndTime = MatchStart + type.SignUpTime;
		if (Stage < ArenaMatchStage.MatchFinished)
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HeartbeatManager_FuzzyFiveSecondHeartbeat;
		}
	}

	private void HeartbeatManager_FuzzyFiveSecondHeartbeat()
	{
		switch (Stage)
		{
			case ArenaMatchStage.OpenForRegistration:
				Heartbeat_OpenForRegistration();
				return;
			case ArenaMatchStage.PreparingMatch:
				Heartbeat_PreparingMatch();
				return;
			case ArenaMatchStage.MatchUnderway:
				Heartbeat_MatchUnderway();
				return;
			case ArenaMatchStage.MatchFinished:
				Heartbeat_MatchFinished();
				return;
		}
	}

	private void Heartbeat_MatchFinished()
	{
		// Finalise bets

		// Deregister heartbeat
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HeartbeatManager_FuzzyFiveSecondHeartbeat;
	}

	private void Heartbeat_MatchUnderway()
	{
		throw new NotImplementedException();
	}

	private void Heartbeat_PreparingMatch()
	{
		throw new NotImplementedException();
	}

	private void Heartbeat_OpenForRegistration()
	{
		if (DateTime.UtcNow >= CurrentIntervalEndTime)
		{
			Stage = ArenaMatchStage.PreparingMatch;
		}
		throw new NotImplementedException();
	}

	public ICombatArena Arena { get; private set; }
	public IArenaMatchType MatchType { get; private set; }
	public ArenaMatchStage Stage { get; private set; }
	public DateTime MatchStart { get; private set; }
	public MudDateTime MatchStartMDT { get; private set; }
	private readonly CollectionDictionary<int, IArenaCombatantProfile> _combatants = new();
	public IReadOnlyCollectionDictionary<int, IArenaCombatantProfile> CombatantsByTeam => _combatants.AsReadOnlyCollectionDictionary();

	private readonly List<IArenaMatchBet> _bets = new();
	public IEnumerable<IArenaMatchBet> Bets => _bets;
	public int CurrentRound { get; private set; }
	public DateTime? CurrentRoundEndTime { get; private set; }
	public DateTime? CurrentIntervalEndTime { get; private set; }

	public void AddCombatant(IArenaCombatantProfile combatant, int team)
	{
		_combatants[team].Add(combatant);
	}

	public void WithdrawCombatant(IArenaCombatantProfile combatant)
	{
		_combatants.RemoveAll(x => x == combatant);
	}

	public override void Save()
	{
		// TODO
		Changed = false;
	}

	public override string FrameworkItemType => "CombatArena";
}