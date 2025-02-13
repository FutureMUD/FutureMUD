#nullable enable
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
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

		MatchType.OnMatchCreatedProg?.Execute();
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
		// Determine winner
		

		// Finalise bets
		foreach (var bet in Bets)
		{
			
		}

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
		if (MatchType.MatchCanBeginProg.ExecuteBool() || DateTime.UtcNow >= CurrentIntervalEndTime)
		{
			Stage = ArenaMatchStage.PreparingMatch;
		}
	}

	public ICombatArena Arena { get; private set; }
	public IArenaMatchType MatchType { get; private set; }
	public ArenaMatchStage Stage { get; private set; }
	public DateTime MatchStart { get; private set; }
	public MudDateTime MatchStartMDT { get; private set; }
	private readonly List<IArenaCombatantProfile> _combatants = new();
	private readonly CollectionDictionary<int, IArenaCombatantProfile> _combatantsByTeam = new();
	public IReadOnlyCollectionDictionary<int, IArenaCombatantProfile> CombatantsByTeam => _combatantsByTeam.AsReadOnlyCollectionDictionary();
	private readonly Dictionary<IArenaCombatantProfile, int> _combatantTeams = new();

	private readonly List<IArenaMatchBet> _bets = new();
	public IEnumerable<IArenaMatchBet> Bets => _bets;
	public int CurrentRound { get; private set; }
	public DateTime? CurrentRoundEndTime { get; private set; }
	public DateTime? CurrentIntervalEndTime { get; private set; }
	public ICombat? Combat { get; private set; }
	private readonly DoubleCounter<int> _teamScores = new();
	private readonly HashSet<ICharacter> _eliminatedContestants = new();

	public void EliminateContestant(ICharacter contestant)
	{
		var profile = _combatants.FirstOrDefault(x => x.Character == contestant);
		if (profile is null || !_eliminatedContestants.Add(contestant))
		{
			return;
		}

		Changed = true;
		Arena.SendOutput(new EmoteOutput(new Emote("@ ($1) have|has been eliminated!", contestant, contestant, new DummyPerceivable(profile.CombatantName.GetName(NameStyle.FullWithNickname), customColour: Telnet.Orange))));
		CheckForVictory();
	}

	public void CheckForVictory()
	{

	}

	public void HandleScore(ICharacter combatant, ICharacter target, ICombatMove move, CombatMoveResult result)
	{
		switch (MatchType.WinType)
		{
			case ArenaMatchWinType.Points:
				if (result.MoveWasSuccessful && move is IWeaponAttackMove { TargetBodypart.IsVital: true })
				{
					var team1 = _combatantTeams.FirstOrDefault(x => x.Key.Character == combatant).Value;
					var team2 = _combatantTeams.FirstOrDefault(x => x.Key.Character == target).Value;

					if (team1 == team2)
					{
						return;
					}

					_teamScores[team1] += 1;
					Changed = true;
				}
				if (target.IsHelpless)
				{
					EliminateContestant(target);
				}
				break;
			case ArenaMatchWinType.PointsByDegree:
				if (result.MoveWasSuccessful && move is IWeaponAttackMove { TargetBodypart.IsVital: true })
				{
					var team1 = _combatantTeams.FirstOrDefault(x => x.Key.Character == combatant).Value;
					var team2 = _combatantTeams.FirstOrDefault(x => x.Key.Character == target).Value;

					if (team1 == team2)
					{
						return;
					}

					_teamScores[team1] += (int)new OpposedOutcome(result.AttackerOutcome, result.DefenderOutcome).Degree;
					Changed = true;
				}
				if (target.IsHelpless)
				{
					EliminateContestant(target);
				}
				break;
			case ArenaMatchWinType.Grappled:
				if (target.IsHelpless)
				{
					EliminateContestant(target);
				}
				break;
			case ArenaMatchWinType.KnockedOver:
				if (target.IsHelpless || target.PositionState == PositionSprawled.Instance)
				{
					EliminateContestant(target);
				}

				if (combatant.IsHelpless || combatant.PositionState == PositionSprawled.Instance)
				{
					EliminateContestant(combatant);
				}
				break;
			case ArenaMatchWinType.FirstBlood:
				if (result.WoundsCaused.Any(x => x.Parent == target && x.Severity >= WoundSeverity.Minor))
				{
					EliminateContestant(target);
				}
				break;
			case ArenaMatchWinType.Unconscious:
				if (target.State.IsUnconscious())
				{
					EliminateContestant(target);
				}
				break;
			case ArenaMatchWinType.Surrender:
				if (target.IsHelpless)
				{
					EliminateContestant(target);
				}
				break;
			case ArenaMatchWinType.Death:
				if (target.State.IsDead())
				{
					EliminateContestant(target);
				}
				break;
		}
	}

	public void AddCombatant(IArenaCombatantProfile combatant, int team)
	{
		_combatantsByTeam[team].Add(combatant);
	}

	public void WithdrawCombatant(IArenaCombatantProfile combatant)
	{
		_combatantsByTeam.RemoveAll(x => x == combatant);
	}

	public override void Save()
	{
		// TODO
		Changed = false;
	}

	public override string FrameworkItemType => "CombatArena";
}