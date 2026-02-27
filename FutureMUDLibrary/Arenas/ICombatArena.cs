#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Arenas;

/// <summary>
/// Represents a combat arena business that can host structured combat events.
/// </summary>
public interface ICombatArena : IEditableItem {
	IEconomicZone EconomicZone { get; }
	ICurrency Currency { get; }
	IBankAccount? BankAccount { get; set; }
	IFutureProg? OnArenaEventPhaseProg { get; set; }
	IEnumerable<ICharacter> Managers { get; }
	string SignupEcho { get; }
	decimal CashBalance { get; }
	decimal BankBalance { get; }

	IEnumerable<ICell> WaitingCells { get; }
	IEnumerable<ICell> ArenaCells { get; }
	IEnumerable<ICell> ObservationCells { get; }
	IEnumerable<ICell> InfirmaryCells { get; }
	IEnumerable<ICell> NpcStablesCells { get; }
	IEnumerable<ICell> AfterFightCells { get; }

	IEnumerable<ICombatantClass> CombatantClasses { get; }
	IEnumerable<IArenaEventType> EventTypes { get; }
	IEnumerable<IArenaEvent> ActiveEvents { get; }

	bool IsManager(ICharacter actor);
	void AddManager(ICharacter actor);
	void RemoveManager(ICharacter actor);

	(bool Truth, string Reason) IsReadyToHost(IArenaEventType eventType);
	IArenaEvent CreateEvent(IArenaEventType eventType, DateTime scheduledFor, IEnumerable<IArenaReservation>? reservations = null);
	void AbortEvent(IArenaEvent arenaEvent, string reason, ICharacter? byManager = null);

	decimal AvailableFunds();
	(bool Truth, string Reason) EnsureFunds(decimal amount);
	(bool Truth, string Reason) EnsureCashFunds(decimal amount);
	void Credit(decimal amount, string reference);
	void Debit(decimal amount, string reference);
	void CreditCash(decimal amount, string reference);
	void DebitCash(decimal amount, string reference);

	string ShowToManager(ICharacter actor);
}
