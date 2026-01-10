#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Arenas;

internal enum ArenaCellRole
{
	Waiting = 0,
	ArenaFloor = 1,
	Observation = 2,
	Infirmary = 3,
	NpcStables = 4,
	AfterFight = 5
}

public sealed class CombatArena : SaveableItem, ICombatArena
{
	private readonly HashSet<long> _managerIds = new();
	private readonly Dictionary<ArenaCellRole, List<ICell>> _cells = new();
	private readonly List<ICombatantClass> _combatantClasses = new();
	private readonly List<IArenaEventType> _eventTypes = new();
	private readonly List<IArenaEvent> _events = new();
	private bool _managersDirty;
	private bool _cellsDirty;
	private decimal _virtualBalance;
	private string _signupEcho = DefaultSignupEcho;

	public CombatArena(Arena arena, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = arena.Id;
		_name = arena.Name;
		EconomicZone = Gameworld.EconomicZones.Get(arena.EconomicZoneId);
		Currency = Gameworld.Currencies.Get(arena.CurrencyId);
		_virtualBalance = arena.VirtualBalance;
		BankAccount = arena.BankAccountId.HasValue ? Gameworld.BankAccounts.Get(arena.BankAccountId.Value) : null;
		_signupEcho = arena.SignupEcho ?? DefaultSignupEcho;

		foreach (var manager in arena.ArenaManagers)
		{
			_managerIds.Add(manager.CharacterId);
		}

		foreach (ArenaCellRole role in Enum.GetValues(typeof(ArenaCellRole)))
		{
			_cells[role] = new List<ICell>();
		}

		foreach (var cell in arena.ArenaCells)
		{
			if (!_cells.TryGetValue((ArenaCellRole)cell.Role, out var list))
			{
				continue;
			}

			var resolved = Gameworld.Cells.Get(cell.CellId);
			if (resolved != null)
			{
				list.Add(resolved);
			}
		}

		foreach (var combatant in arena.ArenaCombatantClasses)
		{
			_combatantClasses.Add(new ArenaCombatantClass(combatant, this));
		}

		foreach (var type in arena.ArenaEventTypes)
		{
			_eventTypes.Add(new ArenaEventType(type, this, GetCombatantClass));
		}

		var typesById = _eventTypes.OfType<ArenaEventType>().ToDictionary(x => x.Id);
		foreach (var ev in arena.ArenaEvents)
		{
			if (!typesById.TryGetValue(ev.ArenaEventTypeId, out var eventType))
			{
				continue;
			}

			var arenaEvent = new ArenaEvent(ev, this, eventType);
			_events.Add(arenaEvent);
		}

		foreach (var evt in _events)
		{
			if (evt.State is ArenaEventState.Completed or ArenaEventState.Aborted)
			{
				continue;
			}

			Gameworld.ArenaScheduler.Schedule(evt);
		}
	}

	public CombatArena(IFuturemud gameworld, string name, IEconomicZone zone, ICurrency currency)
	{
		Gameworld = gameworld;
		_name = name;
		EconomicZone = zone;
		Currency = currency;
		_virtualBalance = 0.0m;
		_signupEcho = DefaultSignupEcho;

		using (new FMDB())
		{
			var record = new Arena
			{
				Name = name,
				EconomicZoneId = zone.Id,
				CurrencyId = currency.Id,
				CreatedAt = DateTime.UtcNow,
				VirtualBalance = 0.0m,
				SignupEcho = _signupEcho,
				IsDeleted = false
			};
			FMDB.Context.Arenas.Add(record);
			FMDB.Context.SaveChanges();
			_id = record.Id;
		}
	}

	public override string FrameworkItemType => "CombatArena";

	public IEconomicZone EconomicZone { get; private set; }
	public ICurrency Currency { get; private set; }
	public IBankAccount? BankAccount { get; private set; }
	public string SignupEcho => _signupEcho;
	IBankAccount? ICombatArena.BankAccount
	{
		get => BankAccount;
		set => BankAccount = value;
	}

	public IEnumerable<ICombatantClass> CombatantClasses => _combatantClasses;
	public IEnumerable<ICharacter> Managers =>
		_managerIds.Select(id => Gameworld.TryGetCharacter(id, true)).OfType<ICharacter>();

	public IEnumerable<ICell> WaitingCells => _cells[ArenaCellRole.Waiting];
	public IEnumerable<ICell> ArenaCells => _cells[ArenaCellRole.ArenaFloor];
	public IEnumerable<ICell> ObservationCells => _cells[ArenaCellRole.Observation];
	public IEnumerable<ICell> InfirmaryCells => _cells[ArenaCellRole.Infirmary];
	public IEnumerable<ICell> NpcStablesCells => _cells[ArenaCellRole.NpcStables];
	public IEnumerable<ICell> AfterFightCells => _cells[ArenaCellRole.AfterFight];

	public IEnumerable<IArenaEventType> EventTypes => _eventTypes;
	public IEnumerable<IArenaEvent> ActiveEvents =>
		_events.Where(x => x.State is not (ArenaEventState.Completed or ArenaEventState.Aborted));

	public bool IsManager(ICharacter actor)
	{
		return actor != null && _managerIds.Contains(actor.Id);
	}

	public void AddManager(ICharacter actor)
	{
		if (actor == null || !_managerIds.Add(actor.Id))
		{
			return;
		}

		_managersDirty = true;
		Changed = true;
	}

	public void RemoveManager(ICharacter actor)
	{
		if (actor == null || !_managerIds.Remove(actor.Id))
		{
			return;
		}

		_managersDirty = true;
		Changed = true;
	}

	public (bool Truth, string Reason) IsReadyToHost(IArenaEventType eventType)
	{
		if (!ArenaCells.Any())
		{
			return (false, "The arena does not have any configured combat cells.");
		}

		if (!WaitingCells.Any())
		{
			return (false, "The arena does not have any waiting cells configured.");
		}

		if (eventType.Arena != this)
		{
			return (false, "That event type is not owned by this arena.");
		}

		return (true, string.Empty);
	}

	public IArenaEvent CreateEvent(IArenaEventType eventType, DateTime scheduledFor,
		IEnumerable<IArenaReservation>? reservations = null)
	{
		if (eventType is not ArenaEventType concrete || !ReferenceEquals(concrete.Arena, this))
		{
			throw new InvalidOperationException("Event type did not belong to this arena.");
		}

		var newEvent = new ArenaEvent(concrete, this, scheduledFor, reservations);
		_events.Add(newEvent);
		Gameworld.ArenaScheduler.Schedule(newEvent);
		return newEvent;
	}

	public void AbortEvent(IArenaEvent arenaEvent, string reason, ICharacter? byManager = null)
	{
		if (arenaEvent is not ArenaEvent concrete || !ReferenceEquals(concrete.Arena, this))
		{
			return;
		}

		concrete.Abort(reason);
	}

	public decimal AvailableFunds()
	{
		return BankAccount?.CurrentBalance ?? _virtualBalance;
	}

	public (bool Truth, string Reason) EnsureFunds(decimal amount)
	{
		if (amount <= 0)
		{
			return (true, string.Empty);
		}

		var available = AvailableFunds();
		if (available >= amount)
		{
			return (true, string.Empty);
		}

		return (false, $"Arena {Name} does not have sufficient funds. Required {amount:n2}, available {available:n2}.");
	}

	public void Credit(decimal amount, string reference)
	{
		if (amount <= 0)
		{
			return;
		}

		if (BankAccount != null)
		{
			BankAccount.DepositFromTransaction(amount, reference);
			return;
		}

		_virtualBalance += amount;
		Changed = true;
	}

	public void Debit(decimal amount, string reference)
	{
		if (amount <= 0)
		{
			return;
		}

		if (BankAccount != null)
		{
			BankAccount.WithdrawFromTransaction(amount, reference);
			return;
		}

		_virtualBalance -= amount;
		Changed = true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Combat Arena #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourName()}");
		sb.AppendLine($"Currency: {Currency.Name.ColourValue()}");
		sb.AppendLine($"Signup Echo: {(string.IsNullOrWhiteSpace(SignupEcho) ? "None".ColourError() : SignupEcho.ColourCommand())}");
		sb.AppendLine($"Bank Account: {(BankAccount != null ? BankAccount.AccountReference.ColourValue() : "None".ColourError())}");
		sb.AppendLine($"Funds: {Currency.Describe(AvailableFunds(), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Managers:");
		if (!Managers.Any())
		{
			sb.AppendLine("\tNone.".ColourError());
		}
		else
		{
			foreach (var mgr in Managers)
			{
				sb.AppendLine($"\t{mgr.HowSeen(actor).ColourName()}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Cells:");
		foreach (var role in Enum.GetValues<ArenaCellRole>())
		{
			var cells = _cells.ContainsKey(role) ? _cells[role] : [];
			var text = cells.Any()
				? cells.Select(x => x.GetFriendlyReference(actor).ColourName()).ListToCommaSeparatedValues(", ")
				: "None".ColourError();
			sb.AppendLine($"\t{role.DescribeEnum().ColourName()}: {text}");
		}

		sb.AppendLine();
		sb.AppendLine("Combatant Classes:");
		if (!CombatantClasses.Any())
		{
			sb.AppendLine("\tNone.".ColourError());
		}
		else
		{
			foreach (var combatantClass in CombatantClasses)
			{
				sb.AppendLine($"\t{combatantClass.Id.ToStringN0(actor)} - {combatantClass.Name.ColourName()}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Event Types:");
		if (!EventTypes.Any())
		{
			sb.AppendLine("\tNone.".ColourError());
		}
		else
		{
			foreach (var type in EventTypes)
			{
				sb.AppendLine(
					$"\t{type.Id.ToStringN0(actor)} - {type.Name.ColourName()} ({type.BettingModel.DescribeEnum().ColourValue()})");
			}
		}

		return sb.ToString();
	}

	public string ShowToManager(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Arena {Name.ColourName()}");
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourName()}");
		sb.AppendLine($"Currency: {Currency.Name.ColourValue()}");
		sb.AppendLine($"Signup Echo: {(string.IsNullOrWhiteSpace(SignupEcho) ? "None".ColourError() : SignupEcho.ColourCommand())}");
		sb.AppendLine($"Funds: {Currency.Describe(AvailableFunds(), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Managers:");
		if (!Managers.Any())
		{
			sb.AppendLine("\tNone.".ColourError());
		}
		else
		{
			foreach (var mgr in Managers)
			{
				sb.AppendLine($"\t{mgr.HowSeen(actor).ColourName()}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Active Event Types:");
		foreach (var type in EventTypes)
		{
			sb.AppendLine($"\t{type.Name.ColourName()}");
		}

		return sb.ToString();
	}

	public const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this combat arena
	#3zone <economic zone>#0 - changes the economic zone for this arena
	#3currency <currency>#0 - sets the accounting currency for this arena
	#3bank <account>|none#0 - sets or clears the bank account for this arena
	#3signupecho <emote>|none#0 - sets or clears the signup staging echo
	#3manager add <character id>#0 - adds a manager by character id
	#3manager remove <character id>#0 - removes a manager by character id
	#3cell <role> add <cell id>#0 - adds a cell in a particular role
	#3cell <role> remove <cell id>#0 - removes a cell from a particular role";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "zone":
				return BuildingCommandZone(actor, command);
			case "currency":
				return BuildingCommandCurrency(actor, command);
			case "bank":
				return BuildingCommandBank(actor, command);
			case "signupecho":
				return BuildingCommandSignupEcho(actor, command);
			case "manager":
			case "managers":
				return BuildingCommandManager(actor, command);
			case "cell":
			case "cells":
				return BuildingCommandCells(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this combat arena?".SubstituteANSIColour());
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.CombatArenas.Any(x => !ReferenceEquals(x, this) && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a combat arena called {name.ColourName()}.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"This combat arena is now called {name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone should this arena belong to?".SubstituteANSIColour());
			return false;
		}

		var zone = Gameworld.EconomicZones.GetByIdOrName(command.SafeRemainingArgument);
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such economic zone.".ColourError());
			return false;
		}

		EconomicZone = zone;
		Changed = true;
		actor.OutputHandler.Send($"This arena now belongs to the {zone.Name.ColourName()} economic zone.");
		return true;
	}

	private bool BuildingCommandCurrency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which currency should this arena use?".SubstituteANSIColour());
			return false;
		}

		var currency = Gameworld.Currencies.GetByIdOrName(command.SafeRemainingArgument);
		if (currency == null)
		{
			actor.OutputHandler.Send("There is no such currency.".ColourError());
			return false;
		}

		Currency = currency;
		Changed = true;
		actor.OutputHandler.Send($"This arena now uses the {currency.Name.ColourValue()} currency.");
		return true;
	}

	private bool BuildingCommandBank(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which bank account should be associated with this arena? Use #3none#0 to clear.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			BankAccount = null;
			Changed = true;
			actor.OutputHandler.Send("This arena no longer has a bank account and will use a virtual balance instead."
				.SubstituteANSIColour());
			return true;
		}

		var (account, error) = Economy.Banking.Bank.FindBankAccount(command.SafeRemainingArgument, null, actor);
		if (account == null)
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		BankAccount = account;
		Changed = true;
		actor.OutputHandler.Send($"This arena will now use the bank account {account.AccountReference.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSignupEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What signup echo do you want to use? Use #3none#0 to clear.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			_signupEcho = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This arena will no longer echo when participants sign up.");
			return true;
		}

		var emoteText = command.SafeRemainingArgument;
		var emote = new Emote(emoteText, actor);
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage.ColourError());
			return false;
		}

		_signupEcho = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"Signup echo set to {emoteText.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandManager(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What is the ID of the character you want to add as a manager?");
					return false;
				}

				if (!long.TryParse(command.SafeRemainingArgument, out var addId))
				{
					actor.OutputHandler.Send("You must specify the numeric ID of the manager to add.");
					return false;
				}

				var addCharacter = Gameworld.TryGetCharacter(addId, true);
				if (addCharacter == null)
				{
					actor.OutputHandler.Send("There is no such character.".ColourError());
					return false;
				}

				if (IsManager(addCharacter))
				{
					actor.OutputHandler.Send($"{addCharacter.HowSeen(actor, true).ColourName()} is already a manager.");
					return false;
				}

				AddManager(addCharacter);
				actor.OutputHandler.Send($"{addCharacter.HowSeen(actor, true).ColourName()} is now a manager of this arena.");
				return true;
			case "remove":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What is the ID of the manager you want to remove?");
					return false;
				}

				if (!long.TryParse(command.SafeRemainingArgument, out var removeId))
				{
					actor.OutputHandler.Send("You must specify the numeric ID of the manager to remove.");
					return false;
				}

				var removeCharacter = Gameworld.TryGetCharacter(removeId, true);
				if (removeCharacter == null || !IsManager(removeCharacter))
				{
					actor.OutputHandler.Send("That character is not a manager of this arena.".ColourError());
					return false;
				}

				RemoveManager(removeCharacter);
				actor.OutputHandler.Send(
					$"{removeCharacter.HowSeen(actor, true).ColourName()} is no longer a manager of this arena.");
				return true;
			default:
				actor.OutputHandler.Send("You must use either #3manager add <id>#0 or #3manager remove <id>#0."
					.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandCells(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which role do you want to modify? The valid roles are {Enum.GetValues<ArenaCellRole>().ListToColouredString()}.");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum<ArenaCellRole>(out var role))
		{
			actor.OutputHandler.Send(
				$"That is not a valid role. The valid roles are {Enum.GetValues<ArenaCellRole>().ListToColouredString()}.");
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "add":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which cell you want to add in that role?");
					return false;
				}

				var addCell = RoomBuilderModule.LookupCell(actor, command.SafeRemainingArgument);
				if (addCell == null)
				{
					actor.OutputHandler.Send("There is no such cell.".ColourError());
					return false;
				}

				if (_cells[role].Contains(addCell))
				{
					actor.OutputHandler.Send($"{addCell.GetFriendlyReference(actor).ColourName()} is already in that role.");
					return false;
				}

				_cells[role].Add(addCell);
				_cellsDirty = true;
				Changed = true;
				actor.OutputHandler.Send(
					$"{addCell.GetFriendlyReference(actor).ColourName()} is now configured as {role.DescribeEnum().ColourValue()}.");
				return true;
			case "remove":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which cell you want to add in that role?");
					return false;
				}

				var removeCell = RoomBuilderModule.LookupCell(actor, command.SafeRemainingArgument);
				if (removeCell == null)
				{
					actor.OutputHandler.Send("There is no such cell.".ColourError());
					return false;
				}

				if (!_cells[role].Contains(removeCell))
				{
					actor.OutputHandler.Send("That cell is not configured in that role.".ColourError());
					return false;
				}

				_cells[role].Remove(removeCell);
				_cellsDirty = true;
				Changed = true;
				actor.OutputHandler.Send(
					$"{removeCell.GetFriendlyReference(actor).ColourName()} is no longer configured as {role.DescribeEnum().ColourValue()}.");
				return true;
			default:
				actor.OutputHandler.Send(
					"You must use #3cell <role> add <cell reference>#0 or #3cell <role> remove <cell reference>#0.".SubstituteANSIColour());
				return false;
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbArena = FMDB.Context.Arenas.Find(Id);
			if (dbArena == null)
			{
				return;
			}

			dbArena.VirtualBalance = _virtualBalance;
			dbArena.BankAccountId = BankAccount?.Id;
			dbArena.EconomicZoneId = EconomicZone.Id;
			dbArena.CurrencyId = Currency.Id;
			dbArena.Name = Name;
			dbArena.SignupEcho = _signupEcho;
			if (_cellsDirty)
			{
				FMDB.Context.ArenaCells.RemoveRange(FMDB.Context.ArenaCells.Where(x => x.ArenaId == dbArena.Id));
				foreach (var (role, cells) in _cells)
				{
					foreach (var cell in cells)
					{
						FMDB.Context.ArenaCells.Add(new ArenaCell
						{
							ArenaId = dbArena.Id,
							CellId = cell.Id,
							Role = (int)role
						});
					}
				}

				_cellsDirty = false;
			}
			if (_managersDirty)
			{
				FMDB.Context.ArenaManagers.RemoveRange(dbArena.ArenaManagers);
				foreach (var manager in _managerIds)
				{
					dbArena.ArenaManagers.Add(new ArenaManager
					{
						ArenaId = dbArena.Id,
						CharacterId = manager,
						CreatedAt = DateTime.UtcNow
					});
				}

				_managersDirty = false;
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	internal ICombatantClass? GetCombatantClass(long id)
	{
		return _combatantClasses.FirstOrDefault(x => x.Id == id);
	}

	internal void AddCombatantClass(ArenaCombatantClass combatantClass)
	{
		if (!_combatantClasses.Contains(combatantClass))
		{
			_combatantClasses.Add(combatantClass);
		}
	}

	internal void AddEventType(ArenaEventType eventType)
	{
		_eventTypes.Add(eventType);
	}

	internal void RemoveEvent(ArenaEvent arenaEvent)
	{
		_events.Remove(arenaEvent);
	}

	internal ICell? GetWaitingCell(int sideIndex)
	{
		var waiting = WaitingCells?.ToList() ?? [];
		if (!waiting.Any())
		{
			return null;
		}

		return waiting.ElementAtOrDefault(sideIndex) ?? waiting.FirstOrDefault();
	}

	public const string DefaultSignupEcho = "@ sign|signs up for the upcoming bout.";
}
