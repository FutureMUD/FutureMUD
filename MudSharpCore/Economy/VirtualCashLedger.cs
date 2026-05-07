using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Economy;

public static class VirtualCashLedger
{
	private static readonly object InMemoryLock = new();
	private static readonly Dictionary<(string OwnerType, long OwnerId, long CurrencyId), decimal> InMemoryBalances = new();
	private static readonly List<MudSharp.Models.VirtualCashLedgerEntry> InMemoryLedger = new();

	private static bool UseInMemoryLedger => string.IsNullOrWhiteSpace(FMDB.ConnectionString);

	public static void ClearInMemoryForTests()
	{
		lock (InMemoryLock)
		{
			InMemoryBalances.Clear();
			InMemoryLedger.Clear();
		}
	}

	private static string OwnerName(IFrameworkItem? owner)
	{
		return owner switch
		{
			null => string.Empty,
			ICharacter character => character.PersonalName.GetName(NameStyle.FullName),
			_ => owner.Name
		};
	}

	private static MudSharp.Models.VirtualCashBalance BalanceRecord(string ownerType, long ownerId, ICurrency currency)
	{
		var record = FMDB.Context.VirtualCashBalances.FirstOrDefault(x =>
			x.OwnerType == ownerType &&
			x.OwnerId == ownerId &&
			x.CurrencyId == currency.Id);
		if (record is not null)
		{
			return record;
		}

		record = new MudSharp.Models.VirtualCashBalance
		{
			OwnerType = ownerType,
			OwnerId = ownerId,
			CurrencyId = currency.Id,
			Balance = 0.0M
		};
		FMDB.Context.VirtualCashBalances.Add(record);
		return record;
	}

	public static decimal Balance(IFrameworkItem owner, ICurrency currency)
	{
		if (UseInMemoryLedger)
		{
			lock (InMemoryLock)
			{
				return InMemoryBalances.TryGetValue((owner.FrameworkItemType, owner.Id, currency.Id), out var value)
					? value
					: 0.0M;
			}
		}

		using (new FMDB())
		{
			return FMDB.Context.VirtualCashBalances
			           .AsNoTracking()
			           .Where(x => x.OwnerType == owner.FrameworkItemType &&
			                       x.OwnerId == owner.Id &&
			                       x.CurrencyId == currency.Id)
			           .Select(x => x.Balance)
			           .FirstOrDefault();
		}
	}

	public static decimal AvailableFunds(IFrameworkItem owner, ICurrency currency, IBankAccount? bankAccount)
	{
		var balance = Balance(owner, currency);
		if (bankAccount?.Currency != currency)
		{
			return balance;
		}

		return balance + bankAccount.MaximumWithdrawal();
	}

	public static IReadOnlyList<MudSharp.Models.VirtualCashLedgerEntry> LedgerEntries(IFrameworkItem owner, int count = 50)
	{
		if (UseInMemoryLedger)
		{
			lock (InMemoryLock)
			{
				return InMemoryLedger
					.Where(x => x.OwnerType == owner.FrameworkItemType && x.OwnerId == owner.Id)
					.OrderByDescending(x => x.RealDateTime)
					.ThenByDescending(x => x.Id)
					.Take(count)
					.ToList();
			}
		}

		using (new FMDB())
		{
			return FMDB.Context.VirtualCashLedgerEntries
			           .AsNoTracking()
			           .Where(x => x.OwnerType == owner.FrameworkItemType && x.OwnerId == owner.Id)
			           .OrderByDescending(x => x.RealDateTime)
			           .ThenByDescending(x => x.Id)
			           .Take(count)
			           .ToList();
		}
	}

	public static void Record(
		IFrameworkItem owner,
		ICurrency currency,
		decimal amount,
		decimal balanceAfter,
		ICharacter? actor,
		IFrameworkItem? counterparty,
		string sourceKind,
		string destinationKind,
		string reason,
		MudDateTime? mudDateTime = null,
		IBankAccount? linkedBankAccount = null,
		IFrameworkItem? reference = null,
		string? referenceText = null)
	{
		if (UseInMemoryLedger)
		{
			lock (InMemoryLock)
			{
				InMemoryLedger.Add(new MudSharp.Models.VirtualCashLedgerEntry
				{
					Id = InMemoryLedger.Count + 1,
					OwnerType = owner.FrameworkItemType,
					OwnerId = owner.Id,
					CurrencyId = currency.Id,
					RealDateTime = DateTime.UtcNow,
					MudDateTime = mudDateTime?.GetDateTimeString() ?? string.Empty,
					ActorId = actor?.Id,
					ActorName = actor is null ? null : actor.PersonalName.GetName(NameStyle.FullName),
					CounterpartyId = counterparty?.Id,
					CounterpartyType = counterparty?.FrameworkItemType,
					CounterpartyName = OwnerName(counterparty),
					Amount = amount,
					BalanceAfter = balanceAfter,
					SourceKind = sourceKind,
					DestinationKind = destinationKind,
					LinkedBankAccountId = linkedBankAccount?.Id,
					ReferenceType = reference?.FrameworkItemType,
					ReferenceId = reference?.Id,
					Reference = referenceText,
					Reason = reason
				});
			}
			return;
		}

		using (new FMDB())
		{
			FMDB.Context.VirtualCashLedgerEntries.Add(new MudSharp.Models.VirtualCashLedgerEntry
			{
				OwnerType = owner.FrameworkItemType,
				OwnerId = owner.Id,
				CurrencyId = currency.Id,
				RealDateTime = DateTime.UtcNow,
				MudDateTime = mudDateTime?.GetDateTimeString() ?? string.Empty,
				ActorId = actor?.Id,
				ActorName = actor is null ? null : actor.PersonalName.GetName(NameStyle.FullName),
				CounterpartyId = counterparty?.Id,
				CounterpartyType = counterparty?.FrameworkItemType,
				CounterpartyName = OwnerName(counterparty),
				Amount = amount,
				BalanceAfter = balanceAfter,
				SourceKind = sourceKind,
				DestinationKind = destinationKind,
				LinkedBankAccountId = linkedBankAccount?.Id,
				ReferenceType = reference?.FrameworkItemType,
				ReferenceId = reference?.Id,
				Reference = referenceText,
				Reason = reason
			});
			FMDB.Context.SaveChanges();
		}
	}

	public static decimal Credit(
		IFrameworkItem owner,
		ICurrency currency,
		decimal amount,
		ICharacter? actor,
		IFrameworkItem? counterparty,
		string sourceKind,
		string reason,
		MudDateTime? mudDateTime = null,
		IFrameworkItem? reference = null,
		string? referenceText = null)
	{
		if (amount <= 0.0M)
		{
			return Balance(owner, currency);
		}

		if (UseInMemoryLedger)
		{
			lock (InMemoryLock)
			{
				var key = (owner.FrameworkItemType, owner.Id, currency.Id);
				InMemoryBalances[key] = InMemoryBalances.GetValueOrDefault(key) + amount;
				InMemoryLedger.Add(new MudSharp.Models.VirtualCashLedgerEntry
				{
					Id = InMemoryLedger.Count + 1,
					OwnerType = owner.FrameworkItemType,
					OwnerId = owner.Id,
					CurrencyId = currency.Id,
					RealDateTime = DateTime.UtcNow,
					MudDateTime = mudDateTime?.GetDateTimeString() ?? string.Empty,
					ActorId = actor?.Id,
					ActorName = actor is null ? null : actor.PersonalName.GetName(NameStyle.FullName),
					CounterpartyId = counterparty?.Id,
					CounterpartyType = counterparty?.FrameworkItemType,
					CounterpartyName = OwnerName(counterparty),
					Amount = amount,
					BalanceAfter = InMemoryBalances[key],
					SourceKind = sourceKind,
					DestinationKind = "VirtualCash",
					ReferenceType = reference?.FrameworkItemType,
					ReferenceId = reference?.Id,
					Reference = referenceText,
					Reason = reason
				});
				return InMemoryBalances[key];
			}
		}

		using (new FMDB())
		{
			var record = BalanceRecord(owner.FrameworkItemType, owner.Id, currency);
			record.Balance += amount;
			FMDB.Context.VirtualCashLedgerEntries.Add(new MudSharp.Models.VirtualCashLedgerEntry
			{
				OwnerType = owner.FrameworkItemType,
				OwnerId = owner.Id,
				CurrencyId = currency.Id,
				RealDateTime = DateTime.UtcNow,
				MudDateTime = mudDateTime?.GetDateTimeString() ?? string.Empty,
				ActorId = actor?.Id,
				ActorName = actor is null ? null : actor.PersonalName.GetName(NameStyle.FullName),
				CounterpartyId = counterparty?.Id,
				CounterpartyType = counterparty?.FrameworkItemType,
				CounterpartyName = OwnerName(counterparty),
				Amount = amount,
				BalanceAfter = record.Balance,
				SourceKind = sourceKind,
				DestinationKind = "VirtualCash",
				ReferenceType = reference?.FrameworkItemType,
				ReferenceId = reference?.Id,
				Reference = referenceText,
				Reason = reason
			});
			FMDB.Context.SaveChanges();
			return record.Balance;
		}
	}

	public static bool CanDebit(IFrameworkItem owner, ICurrency currency, decimal amount, IBankAccount? bankAccount, out string error)
	{
		error = string.Empty;
		if (amount <= 0.0M)
		{
			return true;
		}

		var balance = Balance(owner, currency);
		if (balance >= amount)
		{
			return true;
		}

		var remainder = amount - balance;
		if (bankAccount?.Currency != currency)
		{
			error = $"Only {currency.Describe(balance, CurrencyDescriptionPatternType.ShortDecimal)} is available in virtual cash and there is no valid bank account for the balance.";
			return false;
		}

		var (truth, bankError) = bankAccount.CanWithdraw(remainder, false);
		if (truth)
		{
			return true;
		}

		error = bankError;
		return false;
	}

	public static bool Debit(
		IFrameworkItem owner,
		ICurrency currency,
		decimal amount,
		ICharacter? actor,
		IFrameworkItem? counterparty,
		string destinationKind,
		string reason,
		IBankAccount? bankAccount,
		MudDateTime? mudDateTime,
		out string error,
		IFrameworkItem? reference = null,
		string? referenceText = null)
	{
		if (!CanDebit(owner, currency, amount, bankAccount, out error))
		{
			return false;
		}

		if (amount <= 0.0M)
		{
			return true;
		}

		var bankAmount = 0.0M;
		decimal balanceAfter;
		if (UseInMemoryLedger)
		{
			lock (InMemoryLock)
			{
				var key = (owner.FrameworkItemType, owner.Id, currency.Id);
				var balance = InMemoryBalances.GetValueOrDefault(key);
				var virtualAmount = Math.Min(balance, amount);
				InMemoryBalances[key] = balance - virtualAmount;
				balanceAfter = InMemoryBalances[key];
				bankAmount = amount - virtualAmount;
				InMemoryLedger.Add(new MudSharp.Models.VirtualCashLedgerEntry
				{
					Id = InMemoryLedger.Count + 1,
					OwnerType = owner.FrameworkItemType,
					OwnerId = owner.Id,
					CurrencyId = currency.Id,
					RealDateTime = DateTime.UtcNow,
					MudDateTime = mudDateTime?.GetDateTimeString() ?? string.Empty,
					ActorId = actor?.Id,
					ActorName = actor is null ? null : actor.PersonalName.GetName(NameStyle.FullName),
					CounterpartyId = counterparty?.Id,
					CounterpartyType = counterparty?.FrameworkItemType,
					CounterpartyName = OwnerName(counterparty),
					Amount = -amount,
					BalanceAfter = balanceAfter,
					SourceKind = bankAmount > 0.0M && virtualAmount > 0.0M
						? "VirtualCash+Bank"
						: bankAmount > 0.0M ? "Bank" : "VirtualCash",
					DestinationKind = destinationKind,
					LinkedBankAccountId = bankAmount > 0.0M ? bankAccount?.Id : null,
					ReferenceType = reference?.FrameworkItemType,
					ReferenceId = reference?.Id,
					Reference = referenceText,
					Reason = reason
				});
			}
		}
		else
		{
		using (new FMDB())
		{
			var record = BalanceRecord(owner.FrameworkItemType, owner.Id, currency);
			var virtualAmount = Math.Min(record.Balance, amount);
			record.Balance -= virtualAmount;
			balanceAfter = record.Balance;
			bankAmount = amount - virtualAmount;
			FMDB.Context.VirtualCashLedgerEntries.Add(new MudSharp.Models.VirtualCashLedgerEntry
			{
				OwnerType = owner.FrameworkItemType,
				OwnerId = owner.Id,
				CurrencyId = currency.Id,
				RealDateTime = DateTime.UtcNow,
				MudDateTime = mudDateTime?.GetDateTimeString() ?? string.Empty,
				ActorId = actor?.Id,
				ActorName = actor is null ? null : actor.PersonalName.GetName(NameStyle.FullName),
				CounterpartyId = counterparty?.Id,
				CounterpartyType = counterparty?.FrameworkItemType,
				CounterpartyName = OwnerName(counterparty),
				Amount = -amount,
				BalanceAfter = balanceAfter,
				SourceKind = bankAmount > 0.0M && virtualAmount > 0.0M
					? "VirtualCash+Bank"
					: bankAmount > 0.0M ? "Bank" : "VirtualCash",
				DestinationKind = destinationKind,
				LinkedBankAccountId = bankAmount > 0.0M ? bankAccount?.Id : null,
				ReferenceType = reference?.FrameworkItemType,
				ReferenceId = reference?.Id,
				Reference = referenceText,
				Reason = reason
			});
			FMDB.Context.SaveChanges();
		}
		}

		if (bankAmount <= 0.0M || bankAccount is null)
		{
			return true;
		}

		bankAccount.WithdrawFromTransaction(bankAmount, reason);
		bankAccount.Bank.CurrencyReserves[currency] -= bankAmount;
		bankAccount.Bank.Changed = true;
		return true;
	}

	public static void CreditBankOrVirtual(
		IFrameworkItem owner,
		ICurrency currency,
		decimal amount,
		ICharacter? actor,
		IFrameworkItem? counterparty,
		string sourceKind,
		string reason,
		IBankAccount? bankAccount,
		MudDateTime? mudDateTime,
		IFrameworkItem? reference = null,
		string? referenceText = null)
	{
		if (amount <= 0.0M)
		{
			return;
		}

		if (bankAccount?.Currency == currency)
		{
			bankAccount.DepositFromTransaction(amount, reason);
			bankAccount.Bank.CurrencyReserves[currency] += amount;
			bankAccount.Bank.Changed = true;
			Record(owner, currency, amount, Balance(owner, currency), actor, counterparty, sourceKind, "Bank", reason,
				mudDateTime, bankAccount, reference, referenceText);
			return;
		}

		Credit(owner, currency, amount, actor, counterparty, sourceKind, reason, mudDateTime, reference, referenceText);
	}
}
