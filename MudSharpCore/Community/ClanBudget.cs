using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Community;

public class ClanBudget : SaveableItem, IClanBudget
{
	private MudDateTime _currentPeriodStart;
	private MudDateTime _currentPeriodEnd;
	private decimal _currentPeriodDrawdown;
	private long? _bankAccountId;
	private IBankAccount? _bankAccount;
	private readonly List<IClanBudgetTransaction> _transactions = new();

	public ClanBudget(MudSharp.Models.ClanBudget budget, IClan clan)
	{
		Gameworld = ((Clan)clan).Gameworld;
		_id = budget.Id;
		_name = budget.Name;
		Clan = clan;
		Appointment = clan.Appointments.First(x => x.Id == budget.AppointmentId);
		_bankAccountId = budget.BankAccountId;
		Currency = Gameworld.Currencies.Get(budget.CurrencyId)!;
		AmountPerPeriod = budget.AmountPerPeriod;
		PeriodInterval = new RecurringInterval
		{
			Type = (IntervalType)budget.PeriodIntervalType,
			IntervalAmount = budget.PeriodIntervalModifier,
			Modifier = budget.PeriodIntervalOther,
			SecondaryModifier = budget.PeriodIntervalOtherSecondary,
			OrdinalFallbackMode = (OrdinalFallbackMode)budget.PeriodIntervalFallback
		};
		_currentPeriodStart = MudDateTime.FromStoredStringOrFallback(budget.CurrentPeriodStart, Gameworld,
			StoredMudDateTimeFallback.CurrentDateTime, "ClanBudget", budget.Id, budget.Name, "CurrentPeriodStart");
		_currentPeriodEnd = MudDateTime.FromStoredStringOrFallback(budget.CurrentPeriodEnd, Gameworld,
			StoredMudDateTimeFallback.CurrentDateTime, "ClanBudget", budget.Id, budget.Name, "CurrentPeriodEnd");
		_currentPeriodDrawdown = budget.CurrentPeriodDrawdown;
		IsActive = budget.IsActive;

		foreach (var transaction in budget.ClanBudgetTransactions)
		{
			_transactions.Add(new ClanBudgetTransaction(transaction, this));
		}
	}

	public ClanBudget(Clan clan, IAppointment appointment, IBankAccount? bankAccount, ICurrency currency, string name,
		decimal amount, RecurringInterval interval)
	{
		Gameworld = clan.Gameworld;
		Clan = clan;
		Appointment = appointment;
		_bankAccount = bankAccount;
		_bankAccountId = bankAccount?.Id;
		Currency = currency;
		_name = name;
		AmountPerPeriod = amount;
		PeriodInterval = interval;
		_currentPeriodStart = clan.Calendar.CurrentDateTime;
		_currentPeriodEnd = interval.GetNextDateTimeAfter(_currentPeriodStart);
		_currentPeriodDrawdown = 0.0M;
		IsActive = true;

		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.ClanBudget
			{
				ClanId = clan.Id,
				AppointmentId = appointment.Id,
				BankAccountId = bankAccount?.Id,
				CurrencyId = Currency.Id,
				Name = name,
				AmountPerPeriod = amount,
				PeriodIntervalType = (int)interval.Type,
				PeriodIntervalModifier = interval.IntervalAmount,
				PeriodIntervalOther = interval.Modifier,
				PeriodIntervalOtherSecondary = interval.SecondaryModifier,
				PeriodIntervalFallback = (int)interval.OrdinalFallbackMode,
				CurrentPeriodStart = _currentPeriodStart.GetDateTimeString(),
				CurrentPeriodEnd = _currentPeriodEnd.GetDateTimeString(),
				CurrentPeriodDrawdown = _currentPeriodDrawdown,
				IsActive = IsActive
			};
			FMDB.Context.ClanBudgets.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "ClanBudget";

	public IClan Clan { get; }
	public IAppointment Appointment { get; set; }
	public IBankAccount? BankAccount
	{
		get => _bankAccount ??= _bankAccountId.HasValue ? Gameworld.BankAccounts.Get(_bankAccountId.Value) : null;
		set
		{
			_bankAccount = value;
			_bankAccountId = value?.Id;
			Changed = true;
		}
	}
	public ICurrency Currency { get; }
	public decimal AmountPerPeriod { get; set; }
	public decimal CurrentPeriodDrawdown => _currentPeriodDrawdown;
	public decimal RemainingBudget => AmountPerPeriod - CurrentPeriodDrawdown;
	public RecurringInterval PeriodInterval { get; set; }
	public MudDateTime CurrentPeriodStart => _currentPeriodStart;
	public MudDateTime CurrentPeriodEnd => _currentPeriodEnd;
	public bool IsActive { get; set; }
	public IEnumerable<IClanBudgetTransaction> Transactions => _transactions;

	public void RollToCurrentPeriod()
	{
		var now = Clan.Calendar.CurrentDateTime;
		var rolled = false;
		while (_currentPeriodEnd <= now)
		{
			_currentPeriodStart = _currentPeriodEnd;
			_currentPeriodEnd = PeriodInterval.GetNextDateTimeAfter(_currentPeriodStart);
			_currentPeriodDrawdown = 0.0M;
			rolled = true;
		}

		if (rolled)
		{
			Changed = true;
		}
	}

	public void AddDrawdown(IClanBudgetTransaction transaction)
	{
		_transactions.Add(transaction);
		_currentPeriodDrawdown += transaction.Amount;
		Changed = true;
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id == 0)
		{
			return;
		}

		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.ClanBudgets.Find(Id);
			if (dbitem is not null)
			{
				FMDB.Context.ClanBudgets.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ClanBudgets.Find(Id);
			if (dbitem is null)
			{
				return;
			}
			dbitem.Name = Name;
			dbitem.AppointmentId = Appointment.Id;
			dbitem.BankAccountId = BankAccount?.Id;
			dbitem.CurrencyId = Currency.Id;
			dbitem.AmountPerPeriod = AmountPerPeriod;
			dbitem.PeriodIntervalType = (int)PeriodInterval.Type;
			dbitem.PeriodIntervalModifier = PeriodInterval.IntervalAmount;
			dbitem.PeriodIntervalOther = PeriodInterval.Modifier;
			dbitem.PeriodIntervalOtherSecondary = PeriodInterval.SecondaryModifier;
			dbitem.PeriodIntervalFallback = (int)PeriodInterval.OrdinalFallbackMode;
			dbitem.CurrentPeriodStart = CurrentPeriodStart.GetDateTimeString();
			dbitem.CurrentPeriodEnd = CurrentPeriodEnd.GetDateTimeString();
			dbitem.CurrentPeriodDrawdown = CurrentPeriodDrawdown;
			dbitem.IsActive = IsActive;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}
}
