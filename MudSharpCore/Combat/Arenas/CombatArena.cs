using Microsoft.EntityFrameworkCore.Metadata;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable
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

internal class CombatArena : SaveableItem, ICombatArena
{
	public override void Save()
	{
		// TODO on DB stuff

		Changed = false;
	}

	public override string FrameworkItemType => "CombatArena";

	private readonly List<ICell> _arenaCells = new();
	public IEnumerable<ICell> ArenaCells => _arenaCells;

	private readonly List<ICell> _stagingCells = new();
	public IEnumerable<ICell> StagingCells => _stagingCells;

	private readonly List<ICell> _stableCells = new();
	public IEnumerable<ICell> StableCells => _stableCells;

	private readonly List<ICell> _spectatorCells = new();
	public IEnumerable<ICell> SpectatorCells => _spectatorCells;

	private readonly List<IArenaCombatantType> _arenaCombatantTypes = new();
	public IEnumerable<IArenaCombatantType> ArenaCombatantTypes => _arenaCombatantTypes;

	private readonly List<IArenaMatchType> _arenaMatchTypes = new();
	public IEnumerable<IArenaMatchType> ArenaMatchTypes => _arenaMatchTypes;

	private readonly List<IArenaCombatantProfile> _arenaCombatantProfiles = new();
	public IEnumerable<IArenaCombatantProfile> ArenaCombatantProfiles => _arenaCombatantProfiles;

	private readonly List<IArenaMatchBet> _arenaMatchBets = new();
	
	public IEnumerable<IArenaMatchBet> ArenaMatchBets => _arenaMatchBets;

	private readonly List<IArenaMatch> _arenaMatches = new();
	public IEnumerable<IArenaMatch> ArenaMatches => _arenaMatches;

	private IArenaMatch? _activeMatch;
	public IArenaMatch? ActiveMatch => _activeMatch;
	public IEconomicZone EconomicZone { get; private set; }
	public ICurrency Currency => EconomicZone.Currency;
	public decimal CashBalance { get; private set; }
	public IBankAccount? BankAccount { get; private set; }
	public DateTime? LastArenaMatch { get; private set; }
	public TimeSpan TimeBetweenMatches { get; private set; }

	private readonly HashSet<long> _managerIDs = new();
	public bool IsManager(ICharacter actor)
	{
		return _managerIDs.Contains(actor.Id);
	}

	public void FiveSecondTick()
	{
		// Is a match in progress?
		if (_activeMatch is not null)
		{
			FiveSecondTickActiveMatch();
			return;
		}

		// Should we start a match?
		if (
				LastArenaMatch is null ||
				(DateTime.UtcNow - LastArenaMatch.Value) >= TimeBetweenMatches
			)
		{
			CheckForMatchStart();
			return;
		}
	}

	private void CheckForMatchStart()
	{
		var eligibleTypes = ArenaMatchTypes.Where(x => x.MatchCanBeginProg.ExecuteBool()).ToList();
		if (!eligibleTypes.Any())
		{
			return;
		}

		var matchType = eligibleTypes.GetWeightedRandom(x => x.RelativePriorityForMatchType);
		if (matchType is null)
		{
			return;
		}

		// Create new match
	}

	private void FiveSecondTickActiveMatch()
	{
		throw new NotImplementedException();
	}

	#region Building
	public const string BuildingHelp = @"";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch()) {
			case "name":
				return BuildingCommandName(actor, command);
			case "bank":
			case "bankaccount":
			case "account":
				return BuildingCommandBankAccount(actor, command);
		}

		actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandBankAccount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a bank account or use {"none".ColourCommand()} to remove a bank account.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "remove", "delete", "clear"))
		{
			BankAccount = null;
			Changed = true;
			actor.OutputHandler.Send("This combat arena will no longer use any bank account to back its transactions.");
			return true;
		}

		var (account, error) = Bank.FindBankAccount(command.SafeRemainingArgument, null, actor);
		if (account is null)
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		BankAccount = account;
		Changed = true;
		actor.OutputHandler.Send(
			$"This combat arena will now use the bank account {account.AccountReference.ColourName()} to back its financial transactions.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename this arena to?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.CombatArenas.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a combat arena with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the combat arena {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Combat Arena #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.BoldRed, Telnet.BoldWhite));
		return sb.ToString();
	}
	#endregion
}
