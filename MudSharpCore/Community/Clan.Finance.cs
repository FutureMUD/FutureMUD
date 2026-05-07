using MoreLinq.Extensions;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace MudSharp.Community;

public partial class Clan
{
	private const string ClanBudgetHelp = @"You can use the clan budget command in the following ways:

	#3clan budget <clan> list#0 - lists appointment budgets
	#3clan budget <clan> view <budget>#0 - reviews a budget and recent drawdowns
	#3clan budget <clan> audit [<budget>]#0 - reviews drawdown audit history
	#3clan budget <clan> create <appointment> <amount> ""<interval>"" <name>#0 - creates an appointment budget
	#3clan budget <clan> draw <budget> <amount> <reason>#0 - draws money from a budget
	#3clan budget <clan> close <budget>#0 - closes an active budget";

	public void BudgetCommand(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(ClanBudgetHelp.SubstituteANSIColour());
			return;
		}

		switch (command.PopForSwitch())
		{
			case "list":
			case "show":
				BudgetList(actor);
				return;
			case "view":
				BudgetView(actor, command);
				return;
			case "audit":
			case "history":
				BudgetAudit(actor, command);
				return;
			case "create":
			case "new":
			case "add":
				BudgetCreate(actor, command);
				return;
			case "draw":
			case "drawdown":
			case "withdraw":
				BudgetDraw(actor, command);
				return;
			case "close":
			case "delete":
			case "remove":
				BudgetClose(actor, command);
				return;
			default:
				actor.OutputHandler.Send(ClanBudgetHelp.SubstituteANSIColour());
				return;
		}
	}

	private bool CanReviewFinance(ICharacter actor, out IClanMembership? membership)
	{
		membership = ActorMembership(actor);
		return CanViewFinances(actor, membership);
	}

	private bool CanCreateBudget(ICharacter actor, out IClanMembership? membership)
	{
		return HasPrivilege(actor, ClanPrivilegeType.CanCreateBudgets, out membership);
	}

	private static string DescribeAmount(ICurrency currency, decimal amount)
	{
		return currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
	}

	private static string DescribeTotals(IEnumerable<(ICurrency Currency, decimal Amount)> totals)
	{
		var list = totals
			.GroupBy(x => x.Currency)
			.Select(x => DescribeAmount(x.Key, x.Sum(y => y.Amount)))
			.ToList();
		return list.Any() ? list.ListToString() : "None".ColourError();
	}

	private void BudgetList(ICharacter actor)
	{
		if (!CanReviewFinance(actor, out _))
		{
			actor.OutputHandler.Send($"You are not allowed to review the finances of {FullName.ColourName()}.");
			return;
		}

		foreach (var budget in Budgets)
		{
			budget.RollToCurrentPeriod();
		}

		if (!Budgets.Any())
		{
			actor.OutputHandler.Send($"{FullName.ColourName()} has no appointment budgets.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from budget in Budgets.OrderBy(x => x.Appointment.Name).ThenBy(x => x.Name)
			select new[]
			{
				budget.Id.ToString("N0", actor),
				budget.Name.TitleCase(),
				budget.Appointment.Name.TitleCase(),
				DescribeAmount(budget.Currency, budget.AmountPerPeriod),
				DescribeAmount(budget.Currency, budget.CurrentPeriodDrawdown),
				DescribeAmount(budget.Currency, budget.RemainingBudget),
				budget.PeriodInterval.Describe(Calendar),
				budget.IsActive.ToColouredString()
			},
			new[] { "Id", "Budget", "Appointment", "Per Period", "Drawn", "Remaining", "Period", "Active?" },
			actor.Account.LineFormatLength,
			colour: Telnet.Green,
			truncatableColumnIndex: 1,
			unicodeTable: actor.Account.UseUnicode));
	}

	private void BudgetView(ICharacter actor, StringStack command)
	{
		if (!CanReviewFinance(actor, out _))
		{
			actor.OutputHandler.Send($"You are not allowed to review the finances of {FullName.ColourName()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which budget do you want to view?");
			return;
		}

		var budget = Budgets.GetByIdOrName(command.SafeRemainingArgument);
		if (budget is null)
		{
			actor.OutputHandler.Send($"{FullName.ColourName()} has no such budget.");
			return;
		}

		budget.RollToCurrentPeriod();
		var sb = new StringBuilder();
		sb.AppendLine($"Budget #{budget.Id.ToString("N0", actor)} - {budget.Name.TitleCase().ColourName()}");
		sb.AppendLine();
		sb.AppendLine($"Appointment: {budget.Appointment.Name.TitleCase().ColourName()}");
		sb.AppendLine($"Account: {budget.BankAccount?.AccountReference.ColourName() ?? "Virtual Treasury".ColourName()}");
		sb.AppendLine($"Treasury Balance: {DescribeAmount(budget.Currency, VirtualCashLedger.Balance(this, budget.Currency))}");
		sb.AppendLine($"Status: {budget.IsActive.ToColouredString()}");
		sb.AppendLine($"Period: {budget.PeriodInterval.Describe(Calendar).ColourValue()}");
		sb.AppendLine($"Current Window: {budget.CurrentPeriodStart.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()} to {budget.CurrentPeriodEnd.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
		sb.AppendLine($"Per Period: {DescribeAmount(budget.Currency, budget.AmountPerPeriod)}");
		sb.AppendLine($"Drawn This Period: {DescribeAmount(budget.Currency, budget.CurrentPeriodDrawdown)}");
		sb.AppendLine($"Remaining This Period: {DescribeAmount(budget.Currency, budget.RemainingBudget)}");
		sb.AppendLine();
		AppendBudgetTransactions(actor, sb, budget.Transactions.OrderByDescending(x => x.TransactionTime).Take(10));
		actor.OutputHandler.Send(sb.ToString());
	}

	private void BudgetAudit(ICharacter actor, StringStack command)
	{
		if (!CanReviewFinance(actor, out _))
		{
			actor.OutputHandler.Send($"You are not allowed to review the finances of {FullName.ColourName()}.");
			return;
		}

		IEnumerable<IClanBudgetTransaction> transactions;
		if (command.IsFinished)
		{
			transactions = Budgets.SelectMany(x => x.Transactions);
		}
		else
		{
			var budget = Budgets.GetByIdOrName(command.SafeRemainingArgument);
			if (budget is null)
			{
				actor.OutputHandler.Send($"{FullName.ColourName()} has no such budget.");
				return;
			}

			transactions = budget.Transactions;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Budget Audit for {FullName.ColourName()}");
		sb.AppendLine();
		AppendBudgetTransactions(actor, sb, transactions.OrderByDescending(x => x.TransactionTime).Take(50));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void AppendBudgetTransactions(ICharacter actor, StringBuilder sb,
		IEnumerable<IClanBudgetTransaction> transactions)
	{
		var rows = transactions.ToList();
		if (!rows.Any())
		{
			sb.AppendLine("No drawdowns have been recorded.");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			from transaction in rows
			select new[]
			{
				transaction.TransactionTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
				transaction.Budget.Name.TitleCase(),
				transaction.Actor?.PersonalName.GetName(NameStyle.FullName) ?? "Unknown",
				DescribeAmount(transaction.Currency, transaction.Amount),
				DescribeAmount(transaction.Currency, transaction.BankBalanceAfter),
				transaction.Reason
			},
			new[] { "Date", "Budget", "Actor", "Amount", "Balance After", "Reason" },
			actor.Account.LineFormatLength,
			colour: Telnet.Green,
			truncatableColumnIndex: 5,
			unicodeTable: actor.Account.UseUnicode));
	}

	private void BudgetCreate(ICharacter actor, StringStack command)
	{
		if (!CanCreateBudget(actor, out _))
		{
			actor.OutputHandler.Send($"You are not allowed to create budgets for {FullName.ColourName()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which appointment should this budget belong to?");
			return;
		}

		var currency = ClanBankAccount?.Currency ?? actor.Currency;
		if (currency is null)
		{
			actor.OutputHandler.Send(
				$"{FullName.ColourName()} does not have a default bank account configured, so you must first set your active currency.");
			return;
		}

		var appointment = Appointments.GetByIdOrName(command.PopSpeech());
		if (appointment is null)
		{
			actor.OutputHandler.Send($"{FullName.ColourName()} has no such appointment.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much should this budget allow per period?");
			return;
		}

		if (!currency.TryGetBaseCurrency(command.PopSpeech(), out var amount) || amount <= 0.0M)
		{
			actor.OutputHandler.Send($"That is not a valid positive amount of {currency.Name.ColourName()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What recurring interval should this budget cover?");
			return;
		}

		var intervalText = command.PopSpeech();
		if (!RecurringInterval.TryParse(intervalText, Calendar, out var interval, out var error))
		{
			actor.OutputHandler.Send($"That is not a valid recurring interval: {error}");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should this budget be called?");
			return;
		}

		var name = command.SafeRemainingArgument;
		if (Budgets.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"{FullName.ColourName()} already has a budget called {name.ColourName()}.");
			return;
		}

		var budget = new ClanBudget(this, appointment, ClanBankAccount, currency, name, amount, interval);
		Budgets.Add(budget);
		actor.OutputHandler.Send(
			$"You create the {budget.Name.ColourName()} budget for {appointment.Name.ColourName()}, allowing {DescribeAmount(budget.Currency, amount)} {interval.Describe(Calendar).ColourValue()} from {(budget.BankAccount?.AccountReference.ColourName() ?? "the virtual treasury".ColourName())}.");
	}

	private void BudgetClose(ICharacter actor, StringStack command)
	{
		if (!CanCreateBudget(actor, out _))
		{
			actor.OutputHandler.Send($"You are not allowed to close budgets for {FullName.ColourName()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which budget do you want to close?");
			return;
		}

		var budget = Budgets.GetByIdOrName(command.SafeRemainingArgument);
		if (budget is null)
		{
			actor.OutputHandler.Send($"{FullName.ColourName()} has no such budget.");
			return;
		}

		budget.IsActive = false;
		budget.Changed = true;
		actor.OutputHandler.Send($"The {budget.Name.ColourName()} budget is now closed.");
	}

	private void BudgetDraw(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which budget do you want to draw from?");
			return;
		}

		var budget = Budgets.GetByIdOrName(command.PopSpeech());
		if (budget is null)
		{
			actor.OutputHandler.Send($"{FullName.ColourName()} has no such budget.");
			return;
		}

		var actorMembership = ActorMembership(actor);
		var canDraw = actor.IsAdministrator(PermissionLevel.Admin) ||
		              actorMembership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateBudgets) == true ||
		              ClanCommandUtilities.HoldsOrControlsAppointment(actorMembership, budget.Appointment);
		if (!canDraw)
		{
			actor.OutputHandler.Send($"You are not allowed to draw down the {budget.Name.ColourName()} budget.");
			return;
		}

		if (!budget.IsActive)
		{
			actor.OutputHandler.Send($"The {budget.Name.ColourName()} budget is not active.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much do you want to draw from that budget?");
			return;
		}

		if (!budget.Currency.TryGetBaseCurrency(command.PopSpeech(), out var amount) || amount <= 0.0M)
		{
			actor.OutputHandler.Send($"That is not a valid positive amount of {budget.Currency.Name.ColourName()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the audit reason for this drawdown?");
			return;
		}

		budget.RollToCurrentPeriod();
		if (amount > budget.RemainingBudget)
		{
			actor.OutputHandler.Send(
				$"That would exceed the remaining budget for this period. Remaining: {DescribeAmount(budget.Currency, budget.RemainingBudget)}.");
			return;
		}

		var reason = command.SafeRemainingArgument;
		if (!VirtualCashLedger.Debit(budget.Clan, budget.Currency, amount, actor, budget, "Cash",
			    $"Clan budget {budget.Name}: {reason}", budget.BankAccount, Calendar.CurrentDateTime, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var transaction = new ClanBudgetTransaction(budget, actor, amount, reason);
		budget.AddDrawdown(transaction);

		var payAmount = budget.Currency.FindCoinsForAmount(amount, out _);
		IGameItem newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(budget.Currency, payAmount);
		actor.Gameworld.Add(newItem);
		if (actor.Body.CanGet(newItem, 0))
		{
			actor.Body.Get(newItem, silent: true);
		}
		else
		{
			newItem.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(newItem, true);
		}

		actor.OutputHandler.Send(
			$"You draw {DescribeAmount(budget.Currency, amount)} from the {budget.Name.ColourName()} budget. {newItem.HowSeen(actor).ColourName()} has been issued to you.");
	}

	public void ShowBalanceSheet(ICharacter actor)
	{
		if (!CanReviewFinance(actor, out _))
		{
			actor.OutputHandler.Send($"You are not allowed to review the finances of {FullName.ColourName()}.");
			return;
		}

		foreach (var budget in Budgets)
		{
			budget.RollToCurrentPeriod();
		}

		var accounts = Gameworld.Banks
			.SelectMany(x => x.BankAccounts)
			.Where(x => x.IsAccountOwner(this))
			.ToList();
		var ownedProperties = Gameworld.Properties
			.Where(x => x.PropertyOwners.Any(y => y.Owner == this))
			.ToList();
		var leasedProperties = Gameworld.Properties
			.Where(x => x.Lease?.Leaseholder == this)
			.ToList();
		var propertyShops = ownedProperties.Concat(leasedProperties)
			.SelectMany(x => x.PropertyLocations.Select(y => y.Shop).Where(y => y is not null))
			.Distinct()
			.ToList();
		var bankOwnedShops = Gameworld.Shops
			.Where(x => x.BankAccount?.IsAccountOwner(this) == true)
			.ToList();
		var shops = propertyShops
			.Concat(bankOwnedShops)
			.Distinct()
			.ToList();
		var controlledZones = Gameworld.EconomicZones
			.Where(x => x.ControllingClan == this)
			.ToList();

		var sb = new StringBuilder();
		sb.AppendLine($"Balance Sheet for {FullName.ColourName()}");
		sb.AppendLine();
		sb.AppendLine("Bank Assets:");
		if (accounts.Any())
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from account in accounts.OrderBy(x => x.Bank.Name).ThenBy(x => x.AccountNumber)
				select new[]
				{
					account.AccountReference,
					account.Name,
					account.Bank.Name,
					account.Currency.Name.TitleCase(),
					DescribeAmount(account.Currency, account.CurrentBalance),
					DescribeAmount(account.Currency, account.MaximumWithdrawal())
				},
				new[] { "Account", "Name", "Bank", "Currency", "Balance", "Available" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 1,
				unicodeTable: actor.Account.UseUnicode));
		}
		else
		{
			sb.AppendLine("\tNo clan-owned bank accounts.");
		}

		var virtualTreasury = Gameworld.Currencies
			.Select(x => (Currency: x, Balance: VirtualCashLedger.Balance(this, x)))
			.Where(x => x.Balance != 0.0M)
			.ToList();
		sb.AppendLine();
		sb.AppendLine("Virtual Treasury:");
		if (virtualTreasury.Any())
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from item in virtualTreasury.OrderBy(x => x.Currency.Name)
				select new[]
				{
					item.Currency.Name.TitleCase(),
					DescribeAmount(item.Currency, item.Balance),
					DescribeAmount(item.Currency, VirtualCashLedger.AvailableFunds(this, item.Currency,
						ClanBankAccount?.Currency == item.Currency ? ClanBankAccount : null))
				},
				new[] { "Currency", "Cash", "Available With Bank" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 0,
				unicodeTable: actor.Account.UseUnicode));
		}
		else
		{
			sb.AppendLine("\tNo virtual treasury balances.");
		}

		sb.AppendLine();
		sb.AppendLine("Property Position:");
		sb.AppendLine($"Owned Value: {DescribeTotals(ownedProperties.SelectMany(PropertyValueTotals))}");
		sb.AppendLine($"Owned Lease Revenue: {DescribeTotals(ownedProperties.SelectMany(PropertyRevenueTotals))}");
		sb.AppendLine($"Leased Commitments: {DescribeTotals(leasedProperties.Select(x => (x.EconomicZone.Currency, x.Lease?.PricePerInterval ?? 0.0M)))}");
		if (ownedProperties.Any() || leasedProperties.Any())
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from property in ownedProperties.Concat(leasedProperties).Distinct().OrderBy(x => x.Name)
				select new[]
				{
					property.Name,
					ownedProperties.Contains(property) ? "Owner" : "Leaseholder",
					property.EconomicZone.Currency.Describe(property.LastSaleValue, CurrencyDescriptionPatternType.ShortDecimal),
					property.Lease is null ? "None" : property.EconomicZone.Currency.Describe(property.Lease.PricePerInterval, CurrencyDescriptionPatternType.ShortDecimal),
					property.Lease is null ? "None" : property.EconomicZone.Currency.Describe(property.Lease.PaymentBalance, CurrencyDescriptionPatternType.ShortDecimal)
				},
				new[] { "Property", "Role", "Last Value", "Lease Rate", "Lease Balance" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 0,
				unicodeTable: actor.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.AppendLine("Shop Position:");
		if (shops.Any())
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from shop in shops.OrderBy(x => x.Name)
				let periodStart = shop.EconomicZone.CurrentFinancialPeriod.FinancialPeriodStart
				let transactions = shop.TransactionRecords.Where(x => x.RealDateTime >= periodStart).ToList()
				select new[]
				{
					shop.Name,
					shop.EconomicZone.Name,
					DescribeAmount(shop.Currency, transactions.Where(x => x.TransactionType == ShopTransactionType.Sale).Sum(x => x.PretaxValue)),
					DescribeAmount(shop.Currency, transactions.Sum(x => x.NetValue)),
					DescribeAmount(shop.Currency, shop.CashBalance),
					shop.BankAccount is null ? "None" : DescribeAmount(shop.BankAccount.Currency, shop.BankAccount.CurrentBalance),
					DescribeAmount(shop.Currency, shop.EconomicZone.OutstandingTaxesForShop(shop))
				},
				new[] { "Shop", "Zone", "Gross CFP", "Net CFP", "Cash", "Bank", "Taxes" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 0,
				unicodeTable: actor.Account.UseUnicode));
		}
		else
		{
			sb.AppendLine("\tNo shops are tied to clan accounts or clan-owned/leased properties.");
		}

		sb.AppendLine();
		sb.AppendLine("Economic Zone Revenues:");
		if (controlledZones.Any())
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from zone in controlledZones.OrderBy(x => x.Name)
				select new[]
				{
					zone.Name,
					DescribeAmount(zone.Currency, zone.TotalRevenueHeld),
					DescribeAmount(zone.Currency, zone.HistoricalRevenues.Where(x => x.Period == zone.CurrentFinancialPeriod).Select(x => x.TotalTaxRevenue).DefaultIfEmpty(0.0M).First()),
					DescribeAmount(zone.Currency, zone.OutstandingTaxesForShop(null))
				},
				new[] { "Zone", "Held Revenue", "Current Revenue", "Shop Taxes" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 0,
				unicodeTable: actor.Account.UseUnicode));
		}
		else
		{
			sb.AppendLine("\tNo economic zones are controlled by this clan.");
		}

		sb.AppendLine();
		sb.AppendLine($"Payroll Commitment Per Payday: {DescribeTotals(PayrollCommitments())}");
		sb.AppendLine($"Budget Commitment Per Period: {DescribeTotals(Budgets.Where(x => x.IsActive).Select(x => (x.Currency, x.AmountPerPeriod)))}");
		sb.AppendLine($"Budget Remaining This Period: {DescribeTotals(Budgets.Where(x => x.IsActive).Select(x => (x.Currency, x.RemainingBudget)))}");
		sb.AppendLine($"Backpay Liability: {DescribeTotals(Memberships.SelectMany(x => x.BackPayDiciontary.Select(y => (y.Key, y.Value))))}");
		actor.OutputHandler.Send(sb.ToString());
	}

	private IEnumerable<(ICurrency Currency, decimal Amount)> PropertyValueTotals(IProperty property)
	{
		return property.PropertyOwners
			.Where(x => x.Owner == this)
			.Select(x => (property.EconomicZone.Currency, property.LastSaleValue * x.ShareOfOwnership));
	}

	private IEnumerable<(ICurrency Currency, decimal Amount)> PropertyRevenueTotals(IProperty property)
	{
		if (property.Lease is null)
		{
			return Enumerable.Empty<(ICurrency Currency, decimal Amount)>();
		}

		return property.PropertyOwners
			.Where(x => x.Owner == this)
			.Select(x => (property.EconomicZone.Currency, property.Lease.PricePerInterval * x.ShareOfOwnership));
	}

	private IEnumerable<(ICurrency Currency, decimal Amount)> PayrollCommitments()
	{
		foreach (var membership in Memberships.Where(x => !x.IsArchivedMembership))
		{
			if (membership.Paygrade is not null)
			{
				yield return (membership.Paygrade.PayCurrency, membership.Paygrade.PayAmount);
			}

			foreach (var appointment in membership.Appointments.Where(x => x.Paygrade is not null))
			{
				yield return (appointment.Paygrade.PayCurrency, appointment.Paygrade.PayAmount);
			}
		}
	}

	public void ShowPayrollHistory(ICharacter actor, StringStack command)
	{
		if (!CanReviewFinance(actor, out _))
		{
			actor.OutputHandler.Send($"You are not allowed to review payroll history for {FullName.ColourName()}.");
			return;
		}

		long? characterId = null;
		long? rankId = null;
		long? appointmentId = null;
		string filterDescription = "all members";

		if (!command.IsFinished)
		{
			switch (command.PopForSwitch())
			{
				case "member":
				case "person":
				case "individual":
					if (command.IsFinished)
					{
						actor.OutputHandler.Send("Which member do you want to review?");
						return;
					}

					var membership = GetMembershipByName(command.SafeRemainingArgument);
					if (membership is null)
					{
						var target = actor.TargetActor(command.SafeRemainingArgument);
						membership = target?.ClanMemberships.FirstOrDefault(x => x.Clan == this);
					}

					if (membership is null)
					{
						actor.OutputHandler.Send($"{FullName.ColourName()} has no such member.");
						return;
					}

					characterId = membership.MemberId;
					filterDescription = membership.PersonalName.GetName(NameStyle.FullName);
					break;
				case "rank":
					if (command.IsFinished)
					{
						actor.OutputHandler.Send("Which rank do you want to review?");
						return;
					}

					var rank = Ranks.GetByIdOrName(command.SafeRemainingArgument);
					if (rank is null)
					{
						actor.OutputHandler.Send($"{FullName.ColourName()} has no such rank.");
						return;
					}

					rankId = rank.Id;
					filterDescription = $"rank {rank.Name.TitleCase()}";
					break;
				case "appointment":
				case "position":
					if (command.IsFinished)
					{
						actor.OutputHandler.Send("Which appointment do you want to review?");
						return;
					}

					var appointment = Appointments.GetByIdOrName(command.SafeRemainingArgument);
					if (appointment is null)
					{
						actor.OutputHandler.Send($"{FullName.ColourName()} has no such appointment.");
						return;
					}

					appointmentId = appointment.Id;
					filterDescription = $"appointment {appointment.Name.TitleCase()}";
					break;
				case "all":
				case "summary":
					break;
				default:
					actor.OutputHandler.Send(
						"Use #3clan payroll <clan>#0, #3clan payroll <clan> member <who>#0, #3clan payroll <clan> rank <rank>#0, or #3clan payroll <clan> appointment <appointment>#0.".SubstituteANSIColour());
					return;
			}
		}

		List<MudSharp.Models.ClanPayrollHistory> rows;
		using (new FMDB())
		{
			var query = FMDB.Context.ClanPayrollHistories.Where(x => x.ClanId == Id);
			if (characterId.HasValue)
			{
				query = query.Where(x => x.CharacterId == characterId.Value);
			}

			if (rankId.HasValue)
			{
				query = query.Where(x => x.RankId == rankId.Value);
			}

			if (appointmentId.HasValue)
			{
				query = query.Where(x => x.AppointmentId == appointmentId.Value);
			}

			rows = query.OrderByDescending(x => x.Id).ToList();
		}

		if (!rows.Any())
		{
			actor.OutputHandler.Send($"There is no payroll history for {filterDescription.ColourName()} in {FullName.ColourName()}.");
			return;
		}

		var entries = rows.Select(x => new ClanPayrollHistoryEntry(x, this)).ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"Payroll History for {FullName.ColourName()} ({filterDescription})");
		sb.AppendLine();
		sb.AppendLine($"Total Accrued: {DescribeTotals(entries.Where(x => x.EntryType != ClanPayrollHistoryType.PayCollected).Select(x => (x.Currency, x.Amount)))}");
		sb.AppendLine($"Total Collected: {DescribeTotals(entries.Where(x => x.EntryType == ClanPayrollHistoryType.PayCollected).Select(x => (x.Currency, -x.Amount)))}");
		if (entries.Count > 50)
		{
			sb.AppendLine($"Showing Latest: {50.ToString("N0", actor)} of {entries.Count.ToString("N0", actor)} entries");
		}
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from entry in entries.Take(50)
			select new[]
			{
				entry.DateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
				entry.EntryType.DescribeEnum(),
				entry.Character?.PersonalName.GetName(NameStyle.FullName) ?? "Unknown",
				entry.Rank?.Name.TitleCase() ?? "Unknown",
				entry.Appointment?.Name.TitleCase() ?? "None",
				DescribeAmount(entry.Currency, entry.Amount),
				entry.Description
			},
			new[] { "Date", "Type", "Member", "Rank", "Appointment", "Amount", "Detail" },
			actor.Account.LineFormatLength,
			colour: Telnet.Green,
			truncatableColumnIndex: 6,
			unicodeTable: actor.Account.UseUnicode));
		actor.OutputHandler.Send(sb.ToString());
	}
}
