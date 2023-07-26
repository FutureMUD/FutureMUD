using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Economy.Banking;

public class Bank : SaveableItem, IBank, ILazyLoadDuringIdleTime
{
#nullable enable
	public static (IBankAccount? Account, string Error) FindBankAccount(string accTarget, IBank? homeBank)
	{
		if (accTarget.Split(':').Length == 2)
		{
			var split = accTarget.Split(':');
			var bankTarget = Futuremud.Games.First().Banks.GetByName(split[0]) ??
			                 Futuremud.Games.First().Banks.FirstOrDefault(x =>
				                 x.Code.StartsWith(split[0], StringComparison.InvariantCultureIgnoreCase));
			if (bankTarget == null)
			{
				return (null, $"There is no bank with the name or bank code '{split[0].ColourValue()}'.");
			}

			if (!int.TryParse(split[1], out var accn) || accn <= 0)
			{
				return (null, "The account number to transfer money into must be a number greater than zero.");
			}

			var accountTarget = bankTarget.BankAccounts.FirstOrDefault(x =>
				x.AccountNumber == accn && x.AccountStatus == BankAccountStatus.Active);
			if (accountTarget == null)
			{
				return (null,
					$"The supplied account number is not a valid account number for {bankTarget.Name.ColourName()}.");
			}

			return (accountTarget, string.Empty);
		}

		if (homeBank != null)
		{
			if (!int.TryParse(accTarget, out var accn) || accn <= 0)
			{
				return (null, "The account number to transfer money into must be a number greater than zero.");
			}

			var accountTarget = homeBank.BankAccounts.FirstOrDefault(x =>
				x.AccountNumber == accn && x.AccountStatus == BankAccountStatus.Active);
			if (accountTarget == null)
			{
				return (null,
					$"The supplied account number is not a valid account number for {homeBank.Name.ColourName()}.");
			}

			return (accountTarget, string.Empty);
		}

		return (null, $"You must specify a bank account in the form {"bankcode:account#".Colour(Telnet.BoldCyan)}.");
	}
#nullable restore

	public Bank(Models.Bank bank, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = bank.Id;
		_name = bank.Name;
		PrimaryCurrency = Gameworld.Currencies.Get(bank.PrimaryCurrencyId);
		EconomicZone = Gameworld.EconomicZones.Get(bank.EconomicZoneId);
		Code = bank.Code;
		MaximumBankAccountsPerCustomer = bank.MaximumBankAccountsPerCustomer;
		foreach (var item in bank.BankCurrencyReserves)
		{
			CurrencyReserves[Gameworld.Currencies.Get(item.CurrencyId)] += item.Amount;
		}

		foreach (var rate in bank.BankExchangeRates)
		{
			ExchangeRates
					[(Gameworld.Currencies.Get(rate.FromCurrencyId), Gameworld.Currencies.Get(rate.ToCurrencyId))] =
				rate.ExchangeRate;
		}

		foreach (var type in bank.BankAccountTypes)
		{
			_bankAccountTypes.Add(new BankAccountType(type, this));
		}

		foreach (var account in bank.BankAccounts)
		{
			_bankAccounts.Add(new BankAccount(account, this));
		}

		foreach (var manager in bank.BankManagers)
		{
			_bankManagerIds.Add(manager.CharacterId);
		}

		foreach (var branch in bank.BankBranches)
		{
			_branchIds.Add(branch.CellId);
		}

		Gameworld.SaveManager.AddLazyLoad(this);
	}

	public Bank(IFuturemud gameworld, string name, string code, IEconomicZone zone)
	{
		Gameworld = gameworld;
		_name = name;
		Code = code;
		EconomicZone = zone;
		PrimaryCurrency = zone.Currency;
		MaximumBankAccountsPerCustomer = Gameworld.GetStaticInt("DefaultMaximumBankAccountsPerCustomer");
		using (new FMDB())
		{
			var dbitem = new Models.Bank
			{
				Name = name,
				Code = code,
				PrimaryCurrencyId = PrimaryCurrency.Id,
				EconomicZoneId = EconomicZone.Id,
				MaximumBankAccountsPerCustomer = MaximumBankAccountsPerCustomer
			};
			FMDB.Context.Banks.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "Bank";

	public override void Save()
	{
		var dbitem = FMDB.Context.Banks.Find(Id);
		dbitem.Name = Name;
		dbitem.PrimaryCurrencyId = PrimaryCurrency.Id;
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.Code = Code;
		dbitem.MaximumBankAccountsPerCustomer = MaximumBankAccountsPerCustomer;

		FMDB.Context.BankCurrencyReserves.RemoveRange(dbitem.BankCurrencyReserves);
		foreach (var item in CurrencyReserves)
		{
			dbitem.BankCurrencyReserves.Add(new BankCurrencyReserve
			{
				Bank = dbitem,
				CurrencyId = item.Key.Id,
				Amount = item.Value
			});
		}

		FMDB.Context.BankExchangeRates.RemoveRange(dbitem.BankExchangeRates);
		foreach (var item in ExchangeRates)
		{
			dbitem.BankExchangeRates.Add(new BankExchangeRate
			{
				Bank = dbitem,
				FromCurrencyId = item.Key.From.Id,
				ToCurrencyId = item.Key.To.Id,
				ExchangeRate = item.Value
			});
		}

		FMDB.Context.BankManagers.RemoveRange(dbitem.BankManagers);
		foreach (var item in _bankManagerIds)
		{
			dbitem.BankManagers.Add(new BankManager
			{
				Bank = dbitem,
				CharacterId = item
			});
		}

		FMDB.Context.BankBranches.RemoveRange(dbitem.BankBranches);
		foreach (var item in _branchIds)
		{
			dbitem.BankBranches.Add(new BankBranch
			{
				Bank = dbitem,
				CellId = item
			});
		}

		Changed = false;
	}

	private const string BuildingCommandHelp = @"You can use the following options with this command:

	#3name <name>#0 - renames this bank
	#3code <code>#0 - changes the code for bank transactions
	#3zone <zone>#0 - changes the economic zone
	#3maximumaccounts <##>#0 - sets the max number of accounts per customer
	#3manager <who|id>#0 - toggles someone being a bank manager
	#3branch#0 - toggles your current location as a bank branch
	#3accounts#0 - view a list of accounts
	#3account <accn>#0 - view info about an account
	#3type add <name>#0 - creates a new bank account type
	#3type remove <name>#0 - deletes a bank account type
	#3type view <which>#0 - views a bank account type
	#3type <which> <...>#0 - edits the properties of a bank account type";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "code":
				return BuildingCommandCode(actor, command);
			case "zone":
				return BuildingCommandZone(actor, command);
			case "maxaccounts":
			case "maximumaccounts":
				return BuildingCommandMaximumAccounts(actor, command);
			case "type":
			case "accounttype":
				return BuildingCommandAccountType(actor, command);
			case "manager":
				return BuildingCommandManager(actor, command);
			case "branch":
				return BuildingCommandBranch(actor, command);
			default:
				actor.OutputHandler.Send(BuildingCommandHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandBranch(ICharacter actor, StringStack command)
	{
		if (BranchLocations.Contains(actor.Location))
		{
			_branchLocations.Remove(actor.Location);
			_branchIds.Remove(actor.Location.Id);
			actor.Location.CellRequestsDeletion -= Branch_CellRequestsDeletion;
			Changed = true;
			actor.OutputHandler.Send($"Your current location is no longer a branch for {Name.ColourName()}.");
			return true;
		}

		_branchLocations.Add(actor.Location);
		_branchIds.Add(actor.Location.Id);
		actor.Location.CellRequestsDeletion -= Branch_CellRequestsDeletion;
		actor.Location.CellRequestsDeletion += Branch_CellRequestsDeletion;
		Changed = true;
		actor.OutputHandler.Send($"Your current location is now a branch for {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandManager(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Who would you like to toggle being a bank manager for this bank?");
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

		if (_bankManagerIds.Contains(target.Id))
		{
			RemoveManager(target);
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} will no longer be a manager for {Name.ColourName()}.");
			return true;
		}

		AddManager(target);
		actor.OutputHandler.Send(
			$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is now a manager for {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandAccountType(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandAccounTypeNew(actor, command);
			case "delete":
			case "remove":
			case "rem":
			case "del":
				return BuildingCommandAccountTypeDelete(actor, command);
			case "view":
			case "show":
				return BuildingCommandAccountTypeShow(actor, command);
		}

		var type = BankAccountTypes.GetByNameOrAbbreviation(command.Last);
		if (type == null)
		{
			actor.OutputHandler.Send(
				@$"You must either use the {"add".ColourCommand()}, {"remove".ColourCommand()} or {"view".ColourCommand()} keywords, or specify the name of an existing account to edit.");
			return false;
		}

		return type.BuildingCommand(actor, command);
	}

	private bool BuildingCommandAccountTypeShow(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which account type would you like to view?");
			return false;
		}

		var type = BankAccountTypes.GetByNameOrAbbreviation(command.SafeRemainingArgument);
		if (type == null)
		{
			actor.OutputHandler.Send("There is no such account type to view.");
			return false;
		}

		actor.OutputHandler.Send(type.Show(actor));
		return true;
	}

	private bool BuildingCommandAccountTypeDelete(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send("Coming soon.");
		return false;
	}

	private bool BuildingCommandAccounTypeNew(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new bank account type?");
			return false;
		}

		var name = command.PopSpeech().TitleCase();
		if (BankAccountTypes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} bank already has a bank account type with that name. Names must be unique.");
			return false;
		}

		var newAccount = new BankAccountType(this, name);
		_bankAccountTypes.Add(newAccount);
		actor.OutputHandler.Send($"You create a new bank account type called {name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandMaximumAccounts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send(
				"You must enter a valid number greater than zero for the maximum number of bank accounts per customer.");
			return false;
		}

		MaximumBankAccountsPerCustomer = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"Each customer will now have a limit of {value.ToString("N0", actor).ColourValue()} {"account".Pluralise(value != 1)}.");
		return true;
	}

	private bool BuildingCommandZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone do you want to set as the linked zone for this bank?");
			return false;
		}

		var zone = Gameworld.EconomicZones.GetByIdOrName(command.SafeRemainingArgument);
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return false;
		}

		if (EconomicZone == zone)
		{
			actor.OutputHandler.Send(
				$"The {EconomicZone.Name.ColourName()} economic zone is already the zone for this bank.");
			return false;
		}

		EconomicZone = zone;
		PrimaryCurrency = zone.Currency;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} bank will now belong to the {EconomicZone.Name.ColourName()} economic zone. This may have also updated its currency.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandCode(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What code do you want to give this bank for use in transactions?");
			return false;
		}

		var code = command.SafeRemainingArgument;
		if (Gameworld.Banks.Any(x => x.Code.EqualTo(code)))
		{
			actor.OutputHandler.Send("There is already a bank with that code. Codes must be unique.");
			return false;
		}

		Code = code;
		Changed = true;
		actor.OutputHandler.Send(
			$"The bank {Name.ColourName()} will now use the code {Code.ColourCommand()} in transactions.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this bank?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Banks.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a bank with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the bank {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public void ManagerCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "account":
				ManagerCommandAccount(actor, command);
				return;
			case "accounts":
				ManagerCommandAccounts(actor, command);
				return;
			case "balance":
				ManagerCommandBalance(actor, command);
				return;
			case "audit":
			case "logs":
			case "auditlogs":
			case "log":
			case "auditlog":
				ManagerCommandAuditLogs(actor, command);
				return;
			case "status":
				ManagerCommandStatus(actor, command);
				return;
			case "close":
				ManagerCommandClose(actor, command);
				return;
			case "credit":
				ManagerCommandCredit(actor, command);
				return;
			case "rollover":
				ManagerCommandRollover(actor, command);
				return;
			case "withdraw":
				ManagerCommandWithdrawal(actor, command);
				return;
			case "deposit":
				ManagerCommandDeposit(actor, command);
				return;
			case "exchange":
				ManagerCommandExchange(actor, command);
				return;
			default:
				actor.OutputHandler.Send(@"You can use the following options with the manager command:

    bank manager balance - shows information about the banks funds and liabilities
    bank manager audit [<who>] - shows audit logs (optionally for a specific person)
    bank manager status <account> <active|suspended|locked> - changes the status of a bank account
    bank manager close <account> - permanently closes an account (can get around restrictions)
    bank manager credit <account> <amount> <comment> - credits an existing account
    bank manager rollover <account> <newaccount> - closes an account and rolls balance into a new one
    bank manager withdraw <amount> - withdraws money from the cash reserves
    bank manager deposit <amount> - deposits money into the cash reserves
    bank manager exchange <from> <to> <rate> - sets the currency exchange rate");
				return;
		}
	}

	private void ManagerCommandAccounts(ICharacter actor, StringStack command)
	{
		var accounts = BankAccounts.ToList();
		while (!command.IsFinished)
		{
			var cmd = command.PopSpeech();
			if (cmd.Length > 1 && cmd[0] == '*')
			{
				cmd = cmd.Substring(1);
				var type = BankAccountTypes.GetByNameOrAbbreviation(cmd);
				if (type is null)
				{
					actor.OutputHandler.Send($"There is no such bank account type as {cmd.ColourCommand()}.");
					return;
				}

				accounts = accounts.Where(x => x.BankAccountType == type).ToList();
				continue;
			}

			accounts = accounts.Where(x =>
				x.AccountOwner.Name.EqualTo(cmd) ||
				x.AccountOwner.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase)).ToList();
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from account in accounts
			select new List<string>
			{
				account.AccountNumber.ToString("F0", actor),
				account.BankAccountType.Name,
				account.AccountOwner switch
				{
					ICharacter ch => ch.PersonalName.GetName(NameStyle.FullName),
					IClan clan => clan.FullName,
					IShop shop => shop.Name,
					_ => "Unknown"
				},
				account.AccountStatus.DescribeEnum(),
				account.Currency.Describe(account.CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal),
				account.Currency.Describe(account.CurrentMonthInterest - account.CurrentMonthFees,
					CurrencyDescriptionPatternType.ShortDecimal)
			},
			new List<string>
			{
				"Accn",
				"Type",
				"Owner",
				"Status",
				"Balance",
				"Pending"
			},
			actor,
			Telnet.BoldYellow
		));
	}

	private void ManagerCommandAccount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which bank account do you want to view?");
			return;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var accn) || accn <= 0)
		{
			actor.OutputHandler.Send("The account number you specify must be a number greater than zero.");
			return;
		}

		var account = BankAccounts.FirstOrDefault(x => x.AccountNumber == accn);
		if (account is null)
		{
			actor.OutputHandler.Send("There is no such bank account.");
			return;
		}

		actor.OutputHandler.Send(account.Show(actor));
	}

	/// <inheritdoc />
	public void ReferenceDateOnDateChanged()
	{
		var newMonth = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDate.Day == 1 &&
		               !EconomicZone.FinancialPeriodReferenceCalendar.CurrentDate.IsIntercalaryDay()
			;
		foreach (var account in _bankAccounts)
		{
			if (account.AccountStatus != BankAccountStatus.Active)
			{
				continue;
			}

			account.BankAccountType.DoDailyAccountFees(account);
			if (newMonth)
			{
				account.FinaliseMonth();
			}
		}
	}

	private void ManagerCommandDeposit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"How much money do you want to deposit into the currency reserves of {Name.ColourName()}?");
			return;
		}

		if (!actor.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The amount {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of {actor.Currency.Name.ColourValue()}.");
			return;
		}

		if (!actor.IsAdministrator())
		{
			var targetCoins = actor.Currency.FindCurrency(
				actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
				amount);
			if (!targetCoins.Any())
			{
				actor.OutputHandler.Send("You aren't holding any currency of that type.");
				return;
			}

			var coinValue = targetCoins.TotalValue();
			if (coinValue < amount)
			{
				actor.OutputHandler.Send(
					$"You aren't holding enough money to make a deposit of that size.\nThe largest deposit that you could make is {actor.Currency.Describe(coinValue, CurrencyDescriptionPatternType.Short).ColourValue()}.");
				return;
			}

			var change = 0.0M;
			if (coinValue > amount)
			{
				change = coinValue - amount;
			}

			foreach (var item in targetCoins)
			{
				if (!item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value))))
				{
					actor.Body.Take(item.Key.Parent);
					item.Key.Parent.Delete();
				}
			}

			if (change > 0.0M)
			{
				var changeItem =
					GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(actor.Currency,
						actor.Currency.FindCoinsForAmount(change, out _));
				if (actor.Body.CanGet(changeItem, 0))
				{
					actor.Body.Get(changeItem, silent: true);
				}
				else
				{
					actor.Location.Insert(changeItem, true);
					actor.OutputHandler.Send("You couldn't hold your change, so it is on the ground.");
				}
			}
		}

		var moneyDescription = actor.Currency.Describe(amount, CurrencyDescriptionPatternType.Short)
		                            .ColourValue();
		actor.OutputHandler.Send($"You deposit {moneyDescription} into the currency reserves of {Name.ColourName()}.");
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ deposits {moneyDescription} into the currency reserves of {Name.ColourName()}.", actor,
				actor), flags: OutputFlags.SuppressSource | OutputFlags.SuppressObscured));
		CurrencyReserves[actor.Currency] += amount;
		InitialiseAuditLogs();
		_auditLogs.Add(new BankManagerAuditLog(this, actor,
			EconomicZone.ZoneForTimePurposes.DateTime(EconomicZone.FinancialPeriodReferenceCalendar),
			$"Deposited {moneyDescription} of {actor.Currency.Name.ColourName()}"));
		Changed = true;
	}

	private void ManagerCommandWithdrawal(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"How much money do you want to withdraw from the currency reserves of {Name.ColourName()}?");
			return;
		}

		if (!actor.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The amount {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of {actor.Currency.Name.ColourValue()}.");
			return;
		}

		if (CurrencyReserves[actor.Currency] < amount)
		{
			actor.OutputHandler.Send(
				$"There are insufficient reserves of {actor.Currency.Name.ColourValue()} to withdraw that much.\nThe maximum that you could withdraw is {actor.Currency.Describe(CurrencyReserves[actor.Currency], CurrencyDescriptionPatternType.ShortDecimal)}.");
			return;
		}

		var moneyDescription = actor.Currency.Describe(amount, CurrencyDescriptionPatternType.Short)
		                            .ColourValue();
		actor.OutputHandler.Send($"You withdraw {moneyDescription} from the currency reserves of {Name.ColourName()}.");
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ withdraws {moneyDescription} from the currency reserves of {Name.ColourName()}.", actor,
				actor), flags: OutputFlags.SuppressSource | OutputFlags.SuppressObscured));
		CurrencyReserves[actor.Currency] -= amount;
		InitialiseAuditLogs();
		_auditLogs.Add(new BankManagerAuditLog(this, actor,
			EconomicZone.ZoneForTimePurposes.DateTime(EconomicZone.FinancialPeriodReferenceCalendar),
			$"Withdrew {moneyDescription} of {actor.Currency.Name.ColourName()}"));

		var currencyItem =
			GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(actor.Currency,
				actor.Currency.FindCoinsForAmount(amount, out _));
		if (actor.Body.CanGet(currencyItem, 0))
		{
			actor.Body.Get(currencyItem, silent: true);
		}
		else
		{
			actor.Location.Insert(currencyItem, true);
			actor.OutputHandler.Send("You couldn't hold your money, so it is on the ground.");
		}

		Changed = true;
	}

	private void ManagerCommandRollover(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which bank account do you want to rollover?");
			return;
		}

		if (!int.TryParse(command.PopSpeech(), out var accn) || !_bankAccounts.Any(x => x.AccountNumber == accn))
		{
			actor.OutputHandler.Send("That is not a valid account number.");
			return;
		}

		var account = _bankAccounts.First(x => x.AccountNumber == accn);
		if (account.AccountStatus != BankAccountStatus.Active)
		{
			actor.OutputHandler.Send(
				$"That account is currently {account.AccountStatus.DescribeEnum().ColourValue()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which account do you want to rollover that account into?");
			return;
		}

		if (!int.TryParse(command.PopSpeech(), out var accn2) || !_bankAccounts.Any(x => x.AccountNumber == accn2))
		{
			actor.OutputHandler.Send("The rolled-over to account is not a valid account number.");
			return;
		}

		var account2 = _bankAccounts.First(x => x.AccountNumber == accn2);
		if (account2.AccountStatus != BankAccountStatus.Active)
		{
			actor.OutputHandler.Send(
				$"The rolled-over to account is currently {account2.AccountStatus.DescribeEnum().ColourValue()}.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to rollover account {account.AccountNumber.ToString("F0", actor).ColourValue()} into {account2.AccountNumber.ToString("F0", actor).ColourValue()}?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				account2.DoAccountCredit(
					account.CurrentBalance + account.CurrentMonthInterest - account.CurrentMonthFees,
					$"Account rollover from {account.AccountNumber.ToString("F0", actor).ColourValue()}");
				account.AccountRolledOver();
				actor.OutputHandler.Send(
					$"You roll over bank account {account.AccountNumber.ToString("F0", actor).ColourValue()} into {account2.AccountNumber.ToString("F0", actor).ColourValue()}.");
				InitialiseAuditLogs();
				_auditLogs.Add(new BankManagerAuditLog(this, actor,
					EconomicZone.ZoneForTimePurposes.DateTime(EconomicZone.FinancialPeriodReferenceCalendar),
					$"Rolled over account {account.AccountNumber.ToString("F0").ColourValue()} into {account2.AccountNumber.ToString("F0", actor).ColourValue()}"));
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to close bank account {account.AccountNumber.ToString("F0", actor).ColourValue()}.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to close bank account {account.AccountNumber.ToString("F0", actor).ColourValue()}.");
			},
			DescriptionString = "Closing a bank account",
			Keywords = new List<string> { "close", "account", "bank" }
		}), TimeSpan.FromSeconds(120));
	}

	private void ManagerCommandCredit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which account do you want to credit?");
			return;
		}

		if (!int.TryParse(command.PopSpeech(), out var accn) || !_bankAccounts.Any(x => x.AccountNumber == accn))
		{
			actor.OutputHandler.Send("That is not a valid account number.");
			return;
		}

		var account = _bankAccounts.First(x => x.AccountNumber == accn);
		if (account.AccountStatus != BankAccountStatus.Active)
		{
			actor.OutputHandler.Send(
				$"That account is currently {account.AccountStatus.DescribeEnum().ColourValue()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much money do you want to deposit into that account?");
			return;
		}

		if (!PrimaryCurrency.TryGetBaseCurrency(command.PopSpeech(), out var amount))
		{
			actor.OutputHandler.Send($"That is not a valid amount of {PrimaryCurrency.Name.ColourName()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must supply a reason for the credit.");
			return;
		}

		account.DoAccountCredit(amount, command.SafeRemainingArgument);
		var moneyDescription = actor.Currency.Describe(amount, CurrencyDescriptionPatternType.Short)
		                            .ColourValue();
		InitialiseAuditLogs();
		_auditLogs.Add(new BankManagerAuditLog(this, actor,
			EconomicZone.ZoneForTimePurposes.DateTime(EconomicZone.FinancialPeriodReferenceCalendar),
			$"Credited {moneyDescription} to account {account.AccountNumber.ToString("F0").ColourValue()} - {command.SafeRemainingArgument.ColourCommand()}"));
		actor.OutputHandler.Send(
			$"You credit {moneyDescription} to account {account.AccountNumber.ToString("F0", actor).ColourValue()} with the reason listed as \"{command.SafeRemainingArgument}\":.");
	}

	private void ManagerCommandClose(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which account do you want to close?");
			return;
		}

		if (!int.TryParse(command.PopSpeech(), out var accn) || !_bankAccounts.Any(x => x.AccountNumber == accn))
		{
			actor.OutputHandler.Send("That is not a valid account number.");
			return;
		}

		var account = _bankAccounts.First(x => x.AccountNumber == accn);
		if (account.AccountStatus == BankAccountStatus.Closed)
		{
			actor.OutputHandler.Send($"That account is already {account.AccountStatus.DescribeEnum().ColourValue()}.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to close account {account.AccountNumber.ToString("F0", actor).ColourValue()}?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				account.SetStatus(BankAccountStatus.Closed);
				actor.OutputHandler.Send(
					$"You close bank account {account.AccountNumber.ToString("F0", actor).ColourValue()}.");
				InitialiseAuditLogs();
				_auditLogs.Add(new BankManagerAuditLog(this, actor,
					EconomicZone.ZoneForTimePurposes.DateTime(EconomicZone.FinancialPeriodReferenceCalendar),
					$"Manually closed account {account.AccountNumber.ToString("F0").ColourValue()}"));
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to close bank account {account.AccountNumber.ToString("F0", actor).ColourValue()}.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to close bank account {account.AccountNumber.ToString("F0", actor).ColourValue()}.");
			},
			DescriptionString = "Closing a bank account",
			Keywords = new List<string> { "close", "account", "bank" }
		}), TimeSpan.FromSeconds(120));
	}

	private void ManagerCommandStatus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which account do you want to change the status of?");
			return;
		}

		if (!int.TryParse(command.PopSpeech(), out var accn) || !_bankAccounts.Any(x => x.AccountNumber == accn))
		{
			actor.OutputHandler.Send("That is not a valid account number.");
			return;
		}

		var account = _bankAccounts.First(x => x.AccountNumber == accn);

		BankAccountStatus status;
		switch (command.SafeRemainingArgument)
		{
			case "active":
				status = BankAccountStatus.Active;
				break;
			case "suspended":
				status = BankAccountStatus.Suspended;
				break;
			case "locked":
				status = BankAccountStatus.Locked;
				break;
			default:
				actor.OutputHandler.Send(
					$"You must specify either {new[] { "active", "suspended", "locked" }.Select(x => x.ColourValue()).ListToString(conjunction: "or ")} for the status.");
				return;
		}

		if (account.AccountStatus == status)
		{
			actor.OutputHandler.Send($"That account is already {status.DescribeEnum().ColourValue()}.");
			return;
		}

		account.SetStatus(status);
		actor.OutputHandler.Send(
			$"You change the status of account {account.AccountNumber.ToString("F0", actor).ColourValue()} to {status.DescribeEnum().ColourValue()}.");
		InitialiseAuditLogs();
		_auditLogs.Add(new BankManagerAuditLog(this, actor,
			EconomicZone.ZoneForTimePurposes.DateTime(EconomicZone.FinancialPeriodReferenceCalendar),
			$"Changed status of account {account.AccountNumber.ToString("F0").ColourValue()} to {status.DescribeEnum().ColourValue()}"));
	}

	private void ManagerCommandAuditLogs(ICharacter actor, StringStack command)
	{
		InitialiseAuditLogs();
		var sb = new StringBuilder();
		sb.AppendLine($"Audit Logs for {Name.ColourName()}");
		sb.AppendLine();
		foreach (var item in AuditLogs.OrderByDescending(x => x.DateTime))
		{
			if (actor.IsAdministrator())
			{
				sb.AppendLine(
					$"[{item.DateTime.Date.Display(CalendarDisplayMode.Short)} {item.DateTime.Time.Display(TimeDisplayTypes.Short)}] {item.Detail} - {item.Character.PersonalName.GetName(NameStyle.FullName).ColourName()} (#{item.Character.Id.ToString("N0", actor)})");
			}
			else
			{
				sb.AppendLine(
					$"[{item.DateTime.Date.Display(CalendarDisplayMode.Short)} {item.DateTime.Time.Display(TimeDisplayTypes.Short)}] {item.Detail} - {item.Character.PersonalName.GetName(NameStyle.FullName).ColourName()}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private void ManagerCommandBalance(ICharacter actor, StringStack command)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Balance Sheet for {Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine(
			$"Total Account Balance: {PrimaryCurrency.Describe(_bankAccounts.Sum(x => x.CurrentBalance), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Uncredited Interest: {PrimaryCurrency.Describe(_bankAccounts.Sum(x => x.CurrentMonthInterest), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Pending Fees: {PrimaryCurrency.Describe(_bankAccounts.Sum(x => x.CurrentMonthFees), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Currency Reserves:");
		foreach (var currency in CurrencyReserves)
		{
			sb.AppendLine(
				$"\t{currency.Key.Name.ColourName()}: {currency.Key.Describe(currency.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		}

		sb.AppendLine();
		sb.AppendLine(
			$"Net Position: {PrimaryCurrency.Describe(CurrencyReserves[PrimaryCurrency] - _bankAccounts.Sum(x => x.CurrentBalance + x.CurrentMonthInterest - x.CurrentMonthFees), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		actor.OutputHandler.Send(sb.ToString());
	}

	private void ManagerCommandExchange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which currency do you want to set the conversion rate from?");
			return;
		}

		var from = Gameworld.Currencies.GetByIdOrName(command.PopSpeech());
		if (from == null)
		{
			actor.OutputHandler.Send($"There is no such currency as {command.Last.ColourCommand()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which currency do you want to set the conversion rate to?");
			return;
		}

		var to = Gameworld.Currencies.GetByIdOrName(command.PopSpeech());
		if (to == null)
		{
			actor.OutputHandler.Send($"There is no such currency as {command.Last.ColourCommand()}.");
			return;
		}

		if (from == to)
		{
			actor.OutputHandler.Send("You cannot set an exchange rate for a currency to itself.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What do you want the rate of conversion between those two currencies to be?");
			return;
		}

		if (!decimal.TryParse(command.SafeRemainingArgument, out var rate))
		{
			actor.OutputHandler.Send("That is not a valid conversion rate.");
			return;
		}

		ExchangeRates[(from, to)] = rate;
		Changed = true;
		actor.OutputHandler.Send(
			$"The exchange rate between {from.Name.ColourName()} and {to.Name.ColourName()} will now be {rate.ToString("N", actor).ColourValue()}.");
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Bank #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourValue()}");
		sb.AppendLine($"Currency: {PrimaryCurrency.Name.ColourValue()}");
		sb.AppendLine($"Maximum Accounts p.p.: {MaximumBankAccountsPerCustomer.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Code (for transactions): {Code.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine($"Currency Reserves:");
		foreach (var currency in CurrencyReserves)
		{
			sb.AppendLine(
				$"\t{currency.Key.Name.ColourName()}: {currency.Key.Describe(currency.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		}

		sb.AppendLine();
		sb.AppendLine($"Exchange Rates:");
		foreach (var currency in ExchangeRates)
		{
			sb.AppendLine(
				$"\t{currency.Key.From.Name.ColourName()} -> {currency.Key.To.Name.ColourName()}: {currency.Value.ToString("N6", actor).ColourValue()}");
		}

		sb.AppendLine();
		sb.AppendLine($"Account Types:");
		foreach (var type in _bankAccountTypes)
		{
			var count = _bankAccounts.Count(x => x.BankAccountType == type);
			sb.AppendLine(
				$"\t#{type.Id.ToString("N0", actor)} - {type.Name.ColourName()} - {$"{count.ToString("N0", actor)} {"account".Pluralise(count != 1)}".ColourValue()}");
		}

		return sb.ToString();
	}

	#region FutureProgs

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Bank;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "code":
				return new TextVariable(Code);
			case "accounttypes":
				return new CollectionVariable(BankAccountTypes.ToList(), FutureProgVariableTypes.BankAccountType);
			case "accounts":
				return new CollectionVariable(BankAccounts.ToList(), FutureProgVariableTypes.BankAccount);
			case "branches":
				return new CollectionVariable(BranchLocations.ToList(), FutureProgVariableTypes.Location);
			case "currency":
				return PrimaryCurrency;
			default:
				throw new ApplicationException($"Invalid property {property} requested in Bank.GetProperty");
		}
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "code", FutureProgVariableTypes.Text },
			{ "accounttypes", FutureProgVariableTypes.BankAccountType | FutureProgVariableTypes.Collection },
			{ "accounts", FutureProgVariableTypes.BankAccount | FutureProgVariableTypes.Collection },
			{ "branches", FutureProgVariableTypes.Location | FutureProgVariableTypes.Collection },
			{ "currency", FutureProgVariableTypes.Currency }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The ID of the bank" },
			{ "name", "The name of the bank" },
			{ "code", "The code the bank uses in transfers" },
			{ "accounttypes", "A collection of all the bank account types" },
			{ "accounts", "A collection of all the bank accounts" },
			{ "branches", "A collection of the locations that this bank uses as branches" },
			{ "currency", "The currency in which the bank operates" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Bank, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion

	public string Code { get; set; }
	public IEconomicZone EconomicZone { get; set; }
	public ICurrency PrimaryCurrency { get; set; }
	public int MaximumBankAccountsPerCustomer { get; set; }

	private readonly List<IBankAccount> _bankAccounts = new();
	public IEnumerable<IBankAccount> BankAccounts => _bankAccounts;

	public DecimalCounter<ICurrency> CurrencyReserves { get; } = new();
	public DecimalCounter<(ICurrency From, ICurrency To)> ExchangeRates { get; } = new();

	private readonly List<IBankAccountType> _bankAccountTypes = new();
	public IEnumerable<IBankAccountType> BankAccountTypes => _bankAccountTypes;

	private readonly List<long> _branchIds = new();
	private List<ICell> _branchLocations;

	public IEnumerable<ICell> BranchLocations
	{
		get
		{
			if (_branchLocations == null)
			{
				_branchLocations = _branchIds.SelectNotNull(x => Gameworld.Cells.Get(x)).ToList();
				foreach (var branch in _branchLocations)
				{
					branch.CellRequestsDeletion -= Branch_CellRequestsDeletion;
					branch.CellRequestsDeletion += Branch_CellRequestsDeletion;
				}
			}

			return _branchLocations;
		}
	}

	private void Branch_CellRequestsDeletion(object sender, EventArgs e)
	{
		var cell = (ICell)sender;
		_branchLocations.Remove(cell);
		_branchIds.Remove(cell.Id);
		Changed = true;
	}

	private readonly List<long> _bankManagerIds = new();
	private List<ICharacter> _bankManagers;

	public IEnumerable<ICharacter> BankManagers
	{
		get
		{
			if (_bankManagers == null)
			{
				_bankManagers = _bankManagerIds.Select(x => Gameworld.TryGetCharacter(x, true)).ToList();
			}

			return _bankManagers;
		}
	}

	public bool IsManager(ICharacter actor)
	{
		return actor.IsAdministrator() || BankManagers.Any(x => x.Id == actor.Id);
	}

	public void AddManager(ICharacter manager)
	{
		_bankManagerIds.Add(manager.Id);
		_bankManagers?.Add(manager);
		Changed = true;
	}

	public void RemoveManager(ICharacter manager)
	{
		_bankManagerIds.Remove(manager.Id);
		_bankManagers?.Remove(manager);
		Changed = true;
	}

	private List<IBankManagerAuditLog> _auditLogs;

	private void InitialiseAuditLogs()
	{
		if (_auditLogs != null)
		{
			return;
		}

		_auditLogs = new List<IBankManagerAuditLog>();
		using (new FMDB())
		{
			var dbbank = FMDB.Context.Banks.Find(Id);
			foreach (var item in dbbank.BankManagerAuditLogs.AsEnumerable())
			{
				_auditLogs.Add(new BankManagerAuditLog(item, this));
			}
		}
	}

	public IEnumerable<IBankManagerAuditLog> AuditLogs
	{
		get
		{
			if (_auditLogs == null)
			{
				InitialiseAuditLogs();
			}

			return _auditLogs;
		}
	}

	public void AddAccount(IBankAccount newAccount)
	{
		_bankAccounts.Add(newAccount);
	}

	public (bool Truth, string Error) CanOpenAccount(ICharacter actor, IBankAccountType type)
	{
		var (baseTruth, baseError) = CanOpenNewBankAccounts(actor);
		if (!baseTruth)
		{
			return (baseTruth, baseError);
		}

		return type.CanOpenAccount(actor);
	}

	public (bool Truth, string Error) CanOpenNewBankAccounts(ICharacter character)
	{
		if (BankAccounts.Count(x => x.AccountStatus != BankAccountStatus.Closed && x.IsAccountOwner(character)) >=
		    MaximumBankAccountsPerCustomer)
		{
			return (false,
				$"Customers may only have a maximum of {MaximumBankAccountsPerCustomer.ToString("N0", character)} open accounts at one time.");
		}

		return (true, string.Empty);
	}

	public void DoLoad()
	{
		InitialiseAuditLogs();
	}
}