using Microsoft.EntityFrameworkCore.Metadata;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable
namespace MudSharp.Combat.Arenas;

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
			case "manager":
				return BuildingCommandManager(actor, command);
			case "economiczone":
			case "ez":
				return BuildingCommandEconomicZone(actor, command);
			case "arena":
				return BuildingCommandArena(actor, command);
			case "staging":
			case "stage":
				return BuildingCommandStagingArea(actor);
			case "stable":
			case "stables":
				return BuildingCommandStables(actor);
			case "spectator":
				return BuildingCommandSpectator(actor);
		}

		actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandArena(ICharacter actor, StringStack command)
	{
		if (ActiveMatch is not null)
		{
			actor.OutputHandler.Send("You can't edit this property of an arena while there is an active match.");
			return false;
		}

		var cell = actor.Location;
		if (_arenaCells.Contains(cell))
		{
			_arenaCells.Remove(cell);
			Changed = true;
			actor.OutputHandler.Send("This location is no longer part of the arena area.");
			// TODO - remove arena effect
			return false;
		}

		if (_stableCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a stable cell and an arena cell at the same time.");
			return false;
		}

		if (_spectatorCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a spectator cell and an arena cell at the same time.");
			return false;
		}

		if (_stagingCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a staging cell and an arena cell at the same time.");
			return false;
		}

		_arenaCells.Add(cell);
		Changed = true;
		actor.OutputHandler.Send($"This location is now part of the arena area.");
		// TODO - add arena effect
		return true;
	}

	private bool BuildingCommandStagingArea(ICharacter actor)
	{
		if (ActiveMatch is not null)
		{
			actor.OutputHandler.Send("You can't edit this property of an arena while there is an active match.");
			return false;
		}

		var cell = actor.Location;
		if (_stagingCells.Contains(cell))
		{
			_stagingCells.Remove(cell);
			Changed = true;
			actor.OutputHandler.Send("This location is no longer part of the staging area.");
			return false;
		}

		if (_stableCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a stable cell and  a staging cell at the same time.");
			return false;
		}

		if (_spectatorCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a spectator cell and a staging cell at the same time.");
			return false;
		}

		if (_arenaCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a staging cell and an arena cell at the same time.");
			return false;
		}

		_stagingCells.Add(cell);
		Changed = true;
		actor.OutputHandler.Send($"This location is now part of the staging area.");
		return true;
	}

	private bool BuildingCommandStables(ICharacter actor)
	{
		if (ActiveMatch is not null)
		{
			actor.OutputHandler.Send("You can't edit this property of an arena while there is an active match.");
			return false;
		}

		var cell = actor.Location;
		if (_stableCells.Contains(cell))
		{
			_stableCells.Remove(cell);
			Changed = true;
			actor.OutputHandler.Send("This location is no longer part of the stables area.");
			return false;
		}

		if (_stagingCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a stable cell and an staging cell at the same time.");
			return false;
		}

		if (_spectatorCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a spectator cell and an stable cell at the same time.");
			return false;
		}

		if (_arenaCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a stable cell and an arena cell at the same time.");
			return false;
		}

		_stableCells.Add(cell);
		Changed = true;
		actor.OutputHandler.Send($"This location is now part of the stables area.");
		return true;
	}

	private bool BuildingCommandSpectator(ICharacter actor)
	{
		if (ActiveMatch is not null)
		{
			actor.OutputHandler.Send("You can't edit this property of an arena while there is an active match.");
			return false;
		}

		var cell = actor.Location;
		if (_spectatorCells.Contains(cell))
		{
			_spectatorCells.Remove(cell);
			Changed = true;
			actor.OutputHandler.Send("This location is no longer part of the spectator area.");
			return false;
		}

		if (_stagingCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a spectator cell and an staging cell at the same time.");
			return false;
		}

		if (_stableCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a spectator cell and an stable cell at the same time.");
			return false;
		}

		if (_arenaCells.Contains(cell))
		{
			actor.OutputHandler.Send("A cell cannot be a spectator cell and an arena cell at the same time.");
			return false;
		}

		_spectatorCells.Add(cell);
		Changed = true;
		actor.OutputHandler.Send($"This location is now part of the spectator area.");
		return true;
	}

	private bool BuildingCommandEconomicZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone should this combat arena belong to?");
			return false;
		}

		var ez = Gameworld.EconomicZones.GetByIdOrName(command.SafeRemainingArgument);
		if (ez is null)
		{
			actor.OutputHandler.Send($"There is no economic zone identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		EconomicZone = ez;
		if (BankAccount is not null && ez.Currency != BankAccount.Currency)
		{
			BankAccount = null;
		}
		Changed = true;
		actor.OutputHandler.Send($"This combat arena is now part of the {ez.Name.ColourName()} economic zone.");
		return true;
	}

	private bool BuildingCommandManager(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Who would you like to toggle being a manager for this combat arena?");
			return false;
		}

		var target = actor.TargetActor(command.SafeRemainingArgument);
		if (target == null)
		{
			if (!long.TryParse(command.SafeRemainingArgument, out var id))
			{
				actor.OutputHandler.Send("You don't see anyone like that.");
				return false;
			}

			target = Gameworld.TryGetCharacter(id, true);
			if (target == null)
			{
				actor.OutputHandler.Send("There is no character with that Id.");
				return false;
			}
		}

		if (_managerIDs.Contains(target.Id))
		{
			_managerIDs.Remove(target.Id);
			Changed = true;
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} will no longer be a manager for {Name.ColourName()}.");
			return true;
		}

		_managerIDs.Add(target.Id);
		Changed = true;
		actor.OutputHandler.Send(
			$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is now a manager for {Name.ColourName()}.");
		return true;
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
		sb.AppendLine();
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourValue()}");
		sb.AppendLine($"Bank Account: {BankAccount?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Cash on Hand: {Currency.Describe(CashBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Time Between Matches: {TimeBetweenMatches.DescribePrecise(actor).ColourValue()}");
		sb.AppendLine($"Last Match: {LastArenaMatch?.GetLocalDateString(actor, true).ColourValue() ?? "None".ColourError()}");
		return sb.ToString();
	}
	#endregion
}
