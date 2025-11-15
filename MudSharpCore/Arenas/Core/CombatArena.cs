#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.PerceptionEngine;

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
	private decimal _virtualBalance;

	public CombatArena(Arena arena, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = arena.Id;
		_name = arena.Name;
		EconomicZone = Gameworld.EconomicZones.Get(arena.EconomicZoneId);
		Currency = Gameworld.Currencies.Get(arena.CurrencyId);
		_virtualBalance = arena.VirtualBalance;
		BankAccount = arena.BankAccountId.HasValue ? Gameworld.BankAccounts.Get(arena.BankAccountId.Value) : null;

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

		using (new FMDB())
		{
			var record = new Arena
			{
				Name = name,
				EconomicZoneId = zone.Id,
				CurrencyId = currency.Id,
				CreatedAt = DateTime.UtcNow,
				VirtualBalance = 0.0m,
				IsDeleted = false
			};
			FMDB.Context.Arenas.Add(record);
			FMDB.Context.SaveChanges();
			_id = record.Id;
		}
	}

	public override string FrameworkItemType => "CombatArena";

	public IEconomicZone EconomicZone { get; }
	public ICurrency Currency { get; }
	public IBankAccount? BankAccount { get; private set; }
	IBankAccount? ICombatArena.BankAccount
	{
		get => BankAccount;
		set => BankAccount = value;
	}

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

	public string ShowToManager(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Arena {Name.ColourName()}");
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourName()}");
		sb.AppendLine($"Currency: {Currency.Name.ColourValue()}");
		sb.AppendLine($"Funds: {AvailableFunds().ToString("N2", actor).ColourValue()}");
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

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		return false;
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

	internal void AddEventType(ArenaEventType eventType)
	{
		_eventTypes.Add(eventType);
	}

	internal void RemoveEvent(ArenaEvent arenaEvent)
	{
		_events.Remove(arenaEvent);
	}
}
