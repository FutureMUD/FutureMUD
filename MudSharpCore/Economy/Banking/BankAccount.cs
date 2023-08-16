using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Economy.Banking;

public class BankAccount : SaveableItem, IBankAccount, ILazyLoadDuringIdleTime
{
	public BankAccount(Models.BankAccount dbitem, IBank bank)
	{
		Bank = bank;
		Gameworld = bank.Gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		Gameworld.Add(this);
		AccountCreationDate = new MudDateTime(dbitem.AccountCreationDate, Gameworld);
		AccountNumber = dbitem.AccountNumber;
		AccountStatus = (BankAccountStatus)dbitem.AccountStatus;
		BankAccountType = Gameworld.BankAccountTypes.Get(dbitem.BankAccountTypeId);
		CurrentBalance = dbitem.CurrentBalance;
		CurrentMonthInterest = dbitem.CurrentMonthInterest;
		CurrentMonthFees = dbitem.CurrentMonthFees;
		_characterOwnerId = dbitem.AccountOwnerCharacterId;
		_clanOwnerId = dbitem.AccountOwnerClanId;
		_shopOwnerId = dbitem.AccountOwnerShopId;
		_nominatedBenefactor = dbitem.NominatedBenefactorAccountId;
		if (AccountStatus != BankAccountStatus.Closed)
		{
			Gameworld.SaveManager.AddLazyLoad(this);
		}

		if (!string.IsNullOrEmpty(dbitem.AuthorisedBankPaymentItems))
		{
			foreach (var id in dbitem.AuthorisedBankPaymentItems.Split(' ').Select(x => long.Parse(x)))
			{
				_authorisedPaymentItems.Add(id);
			}
		}
	}

	public override string FrameworkItemType => "BankAccount";

	public override void Save()
	{
		var dbitem = FMDB.Context.BankAccounts.Find(Id);
		dbitem.Name = _name;
		dbitem.AccountCreationDate = AccountCreationDate.GetDateTimeString();
		dbitem.AccountNumber = AccountNumber;
		dbitem.AccountStatus = (int)AccountStatus;
		dbitem.CurrentBalance = CurrentBalance;
		dbitem.BankAccountTypeId = BankAccountType.Id;
		dbitem.CurrentMonthInterest = CurrentMonthInterest;
		dbitem.CurrentMonthFees = CurrentMonthFees;
		dbitem.AccountOwnerCharacterId = _characterOwnerId;
		dbitem.AccountOwnerClanId = _clanOwnerId;
		dbitem.AccountOwnerShopId = _shopOwnerId;
		dbitem.NominatedBenefactorAccountId = _nominatedBenefactor;
		dbitem.AuthorisedBankPaymentItems =
			_authorisedPaymentItems.Select(x => x.ToString("F0")).ListToCommaSeparatedValues(" ");
		Changed = false;
	}

	public IBank Bank { get; set; }
	public int AccountNumber { get; set; }

	public string AccountReference => $"{Bank.Code.ToUpperInvariant()}:{AccountNumber:F0}";
	public string NameWithAlias => $"{AccountReference.ColourName()}{(!string.IsNullOrWhiteSpace(Name) ? $" [{Name}]".ColourCommand() : "")}";
	public IBankAccountType BankAccountType { get; set; }
	public ICurrency Currency => Bank.PrimaryCurrency;
	public decimal CurrentBalance { get; set; }
	public decimal CurrentMonthInterest { get; set; }
	public decimal CurrentMonthFees { get; set; }

	private long? _characterOwnerId;
	private long? _clanOwnerId;
	private long? _shopOwnerId;

	public bool IsAccountOwner(ICharacter character)
	{
		return character.Id == _characterOwnerId;
	}

	public bool IsAccountOwner(IClan clan)
	{
		return clan.Id == _clanOwnerId;
	}

	public bool IsAccountOwner(IShop shop)
	{
		return shop.Id == _shopOwnerId;
	}

	public IFrameworkItem AccountOwner
	{
		get
		{
			if (_characterOwnerId.HasValue)
			{
				return Gameworld.TryGetCharacter(_characterOwnerId.Value, true);
			}

			if (_clanOwnerId.HasValue)
			{
				return Gameworld.Clans.Get(_clanOwnerId.Value);
			}

			if (_shopOwnerId.HasValue)
			{
				return Gameworld.Shops.Get(_shopOwnerId.Value);
			}

			throw new ApplicationException("There was no account owner set for a bank account.");
		}
	}

	public bool IsAuthorisedAccountUser(ICharacter character)
	{
		return character.Id == _characterOwnerId ||
		       (_shopOwnerId != null && Gameworld.Shops.Get(_shopOwnerId.Value)?.IsManager(character) == true) ||
		       (_clanOwnerId != null && Gameworld.Clans.Get(_clanOwnerId.Value)?.Memberships
		                                         .FirstOrDefault(x => x.MemberId == character.Id)?.NetPrivileges
		                                         .HasFlag(ClanPrivilegeType.CanManageBankAccounts) == true);
	}

	private HashSet<long> _authorisedPaymentItems = new();

	public bool IsAuthorisedPaymentItem(IBankPaymentItem item)
	{
		return _authorisedPaymentItems.Contains(item.Parent.Id);
	}

	public void CancelPaymentItems()
	{
		_authorisedPaymentItems.Clear();
		Changed = true;
	}

	public void CancelExistingPaymentItem(IBankPaymentItem item)
	{
		_authorisedPaymentItems.Remove(item.Parent.Id);
		Changed = true;
	}

	public void SetName(string name)
	{
		_name = name;
		Changed = true;
	}
#nullable enable
	public IGameItem? CreateNewPaymentItem()
	{
		var proto = Gameworld.ItemProtos.Get(BankAccountType.PaymentItemPrototype ?? 0);
		if (proto is null || proto.Status != RevisionStatus.Current)
		{
			return null;
		}

		var item = proto.CreateNew();
		var payment = item.GetItemType<IBankPaymentItem>();
		if (payment is null)
		{
			return null;
		}

		payment.BankAccount = this;
		_authorisedPaymentItems.Add(item.Id);
		Changed = true;
		return item;
	}
#nullable restore

	public int NumberOfIssuedPaymentItems => _authorisedPaymentItems.Count;

	public MudDateTime AccountCreationDate { get; set; }
	public BankAccountStatus AccountStatus { get; set; }
	private bool _transactionsLoaded;
	private readonly List<IBankAccountTransaction> _transactions = new();

	public IEnumerable<IBankAccountTransaction> Transactions
	{
		get
		{
			if (!_transactionsLoaded)
			{
				LoadTransactions();
			}

			return _transactions;
		}
	}

	private void LoadTransactions()
	{
		if (_transactionsLoaded)
		{
			return;
		}

		_transactionsLoaded = true;
		using (new FMDB())
		{
			var transactions = FMDB.Context.BankAccountTransactions.Where(x => x.BankAccountId == Id).ToArray();
			foreach (var transaction in transactions)
			{
				_transactions.Add(new BankAccountTransaction(transaction, this));
			}
		}
	}

	private long? _nominatedBenefactor;
	public IBankAccount NominatedBenefactor => Gameworld.BankAccounts.Get(_nominatedBenefactor ?? 0L);

	public decimal MaximumWithdrawal()
	{
		return Math.Max(0.0M, CurrentBalance + BankAccountType.MaximumOverdrawAmount);
	}

	public (bool Truth, string Error) CanWithdraw(decimal amount, bool ignoreCurrencyReserves)
	{
		switch (AccountStatus)
		{
			case BankAccountStatus.Locked:
				return (false, "Your account has been temporarily locked due to suspicious activity.");
			case BankAccountStatus.Suspended:
				return (false, "Your account has been suspended. Please contact your bank manager.");
			case BankAccountStatus.Closed:
				return (false, "Your bank account has been closed and is no longer operational.");
		}

		if (amount <= CurrentBalance + BankAccountType.MaximumOverdrawAmount)
		{
			return (true, string.Empty);
		}

		if (BankAccountType.MaximumOverdrawAmount > 0.0M)
		{
			return (false, "That withdrawal would take you over your account's overdraft limit.");
		}

		if (!ignoreCurrencyReserves && Bank.CurrencyReserves[Currency] < amount)
		{
			return (false, "The bank has insufficient currency reserves to honour your withdrawal.");
		}

		return (false, "Your account has insufficient funds for that withdrawal.");
	}

	public void Withdraw(decimal amount)
	{
		var time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
		LoadTransactions();
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.Withdrawal, amount,
			CurrentBalance - amount, time,
			$"Withdraw {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} at branch"));
		if (CurrentBalance < amount)
		{
			CurrentBalance -= amount;
			BankAccountType.DoOverdrawFees(this, CurrentBalance);
		}
		else
		{
			CurrentBalance -= amount;
		}

		BankAccountType.DoTransactionFeesWithdrawal(this, amount);
		Changed = true;
	}

	public void WithdrawFromTransaction(decimal amount, string transactionReference)
	{
		var time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
		LoadTransactions();
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.WithdrawalFromTransaction, amount,
			CurrentBalance - amount, time,
			$"Withdraw {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} from transaction{(!string.IsNullOrEmpty(transactionReference) ? $" - {transactionReference}" : "")}"));
		if (CurrentBalance < amount)
		{
			CurrentBalance -= amount;
			BankAccountType.DoOverdrawFees(this, CurrentBalance);
		}
		else
		{
			CurrentBalance -= amount;
		}

		BankAccountType.DoTransactionFeesWithdrawal(this, amount);
		Changed = true;
	}

	public void WithdrawFromTransfer(decimal amount, string toBankCode, int toAccount, string transferReference)
	{
		var time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
		LoadTransactions();
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.WithdrawalFromTransfer, amount,
			CurrentBalance - amount, time,
			$"Withdraw {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} from {toAccount} ({toBankCode}){(!string.IsNullOrEmpty(transferReference) ? $" - {transferReference}" : "")}"));
		if (CurrentBalance < amount)
		{
			CurrentBalance -= amount;
			BankAccountType.DoOverdrawFees(this, CurrentBalance);
		}
		else
		{
			CurrentBalance -= amount;
		}

		if (toBankCode.EqualTo(Bank.Code))
		{
			BankAccountType.DoTransactionFeesTransfer(this, amount);
		}
		else
		{
			BankAccountType.DoTransactionFeesTransferOtherBank(this, amount);
		}

		Changed = true;
	}

	public void Deposit(decimal amount)
	{
		LoadTransactions();
		var time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.Deposit, amount, CurrentBalance + amount,
			time, $"Deposit {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} at branch"));
		CurrentBalance += amount;
		BankAccountType.DoTransactionFeesDeposit(this, amount);
		Changed = true;
	}

	public void DepositFromTransfer(decimal amount, string fromBankCode, int fromAccount, string transferReference)
	{
		LoadTransactions();
		var time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.DepositFromTransfer, amount,
			CurrentBalance + amount, time,
			$"Deposit {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} from {fromAccount} ({fromBankCode}){(!string.IsNullOrEmpty(transferReference) ? $" - {transferReference}" : "")}"));
		CurrentBalance += amount;
		BankAccountType.DoTransactionFeesDepositFromTransfer(this, amount);
		Changed = true;
	}

	public void DepositFromTransaction(decimal amount, string transactionReference)
	{
		LoadTransactions();
		var time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.DepositFromTransaction, amount,
			CurrentBalance + amount, time,
			$"Deposit {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} from transaction{(!string.IsNullOrEmpty(transactionReference) ? $" - {transactionReference}" : "")}"));
		CurrentBalance += amount;
		BankAccountType.DoTransactionFeesDepositFromTransfer(this, amount);
		Changed = true;
	}

	public void DoAccountCredit(decimal amount, string reason)
	{
		LoadTransactions();
		var time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.DepositFromTransfer, amount,
			CurrentBalance + amount, time,
			$"Credit {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} - {reason}"));
		CurrentBalance += amount;
		Changed = true;
	}

	public void DepositInterest(decimal amount)
	{
		CurrentMonthInterest += amount;
		Changed = true;
	}

	public void DoDailyFee(decimal amount)
	{
		CurrentMonthFees += amount;
		Changed = true;
	}

	public void AccountRolledOver()
	{
		AccountStatus = BankAccountStatus.Closed;
		var change = CurrentBalance + CurrentMonthInterest - CurrentMonthFees;
		CurrentBalance = 0.0M;
		CurrentMonthInterest = 0.0M;
		CurrentMonthFees = 0.0M;
		LoadTransactions();
		var time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.Withdrawal, change, 0.0M, time,
			$"Account Rolled Over and Finalised"));
		Changed = true;
	}

	public void FinaliseMonth()
	{
		LoadTransactions();
		var time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
		CurrentBalance -= CurrentMonthFees;
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.ServiceFee, CurrentMonthFees,
			CurrentBalance, time, $"Monthly Account Fees"));
		CurrentBalance += CurrentMonthInterest;
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.InterestEarned, CurrentMonthInterest,
			CurrentBalance, time, $"Interest Earned"));
		CurrentMonthFees = 0.0M;
		CurrentMonthInterest = 0.0M;
		Changed = true;
	}

	public void ChargeFee(decimal amount, BankTransactionType transactionType, string feeDescription)
	{
		LoadTransactions();
		var time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
		var feeType = "Other Fee";
		switch (transactionType)
		{
			case BankTransactionType.Withdrawal:
				feeType = "Withdrawal Fee";
				break;
			case BankTransactionType.Deposit:
				feeType = "Deposit Fee";
				break;
			case BankTransactionType.DepositFromTransfer:
				feeType = "Incoming Transfer Fee";
				break;
			case BankTransactionType.Transfer:
				feeType = "Transfer Fee";
				break;
			case BankTransactionType.InterBankTransfer:
				feeType = "Transfer Fee (Other Bank)";
				break;
			case BankTransactionType.CurrencyConversion:
				feeType = "Currency Conversion Fee";
				break;
			case BankTransactionType.InterestCharge:
				feeType = "Interest Charge";
				break;
			case BankTransactionType.InterestEarned:
				feeType = "Interest Earned";
				break;
			case BankTransactionType.ServiceFee:
				feeType = "Service Fee";
				break;
			case BankTransactionType.OverdraftFee:
				feeType = "Overdraft Fee";
				break;
			case BankTransactionType.LoanRepayment:
				feeType = "Loan Repayment";
				break;
			case BankTransactionType.OverdueFee:
				feeType = "Overdue Fee";
				break;
		}

		CurrentBalance -= amount;
		_transactions.Add(new BankAccountTransaction(this, BankTransactionType.ServiceFee, amount, CurrentBalance, time,
			$"{feeType}: {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)}"));
		Changed = true;
	}

	public (bool Truth, string Reason) CloseAccount(ICharacter actor)
	{
		if (CurrentBalance != 0.0M)
		{
			return (false,
				"Only bank accounts with a balance of zero can be closed. Settle any money owing or withdraw your money first.");
		}

		AccountStatus = BankAccountStatus.Closed;
		Changed = true;

		return (true, string.Empty);
	}

	public void SetStatus(BankAccountStatus status)
	{
		AccountStatus = status;
		Changed = true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		if (actor.IsAdministrator())
		{
			sb.AppendLine($"Bank Account #{Id.ToString("N0", actor)}");
		}

		sb.AppendLine($"Type: {BankAccountType.Name.ColourName()}");
		sb.AppendLine($"Account Number: {AccountNumber.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Alias: {(!string.IsNullOrWhiteSpace(Name) ? Name.ColourValue() : "None".ColourError())}");
		sb.AppendLine($"Bank Code: {Bank.Code.ToUpperInvariant().ColourName()}");
		sb.AppendLine($"Created: {AccountCreationDate.Date.Display(CalendarDisplayMode.Short).ColourValue()}");
		if (AccountStatus != BankAccountStatus.Active)
		{
			sb.AppendLine($"Status: {AccountStatus.DescribeEnum().Colour(Telnet.Red)}");
		}

		if (_characterOwnerId.HasValue)
		{
			if (_characterOwnerId.Value == actor.Id)
			{
				sb.AppendLine($"Owner: {"You".ColourCharacter()}");
			}
			else
			{
				var charOwner = Gameworld.TryGetCharacter(_characterOwnerId.Value, true);
				sb.AppendLine(
					$"Owner: {charOwner.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreLoadThings)} ({charOwner.PersonalName.GetName(Character.Name.NameStyle.FullName).ColourName()})");
			}
		}
		else if (_shopOwnerId.HasValue)
		{
			var shop = Gameworld.Shops.Get(_shopOwnerId.Value);
			sb.AppendLine($"Owner: {shop.Name.ColourName()} (Shop)");
		}
		else if (_clanOwnerId.HasValue)
		{
			var clan = Gameworld.Clans.Get(_clanOwnerId.Value);
			sb.AppendLine($"Owner: {clan.FullName.ColourName()} (Clan)");
		}

		sb.AppendLine($"Fees: See {$"bank preview {BankAccountType.Name.ToLowerInvariant()}".MXPSend()}");
		sb.AppendLine();
		sb.AppendLine(
			$"Current Balance: {Currency.Describe(CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Available Balance: {Currency.Describe(MaximumWithdrawal(), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		var proto = Gameworld.ItemProtos.Get(BankAccountType.PaymentItemPrototype ?? 0);
		if (BankAccountType.NumberOfPermittedPaymentItems > 0 &&
		    proto?.Status == RevisionStatus.Current)
		{
			sb.AppendLine(
				$"Payment Item: {proto.ShortDescription.Colour(proto.CustomColour ?? Telnet.Green)} ({NumberOfIssuedPaymentItems.ToString("N0", actor)}/{BankAccountType.NumberOfPermittedPaymentItems.ToString("N0", actor)})");
		}

		return sb.ToString();
	}

	public string ShowTransactions(ICharacter actor)
	{
		var sb = new StringBuilder();
		if (actor.IsAdministrator())
		{
			sb.AppendLine($"Transaction History for Bank Account #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		}
		else
		{
			sb.AppendLine($"Transaction History for {Name.ColourName()}");
		}

		sb.AppendLine();
		sb.Append(StringUtilities.GetTextTable(
			from transaction in Transactions.OrderByDescending(x => x.TransactionTime)
			select new List<string>
			{
				transaction.TransactionTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
				transaction.TransactionDescription,
				Currency.Describe(transaction.Amount, CurrencyDescriptionPatternType.ShortDecimal),
				Currency.Describe(transaction.AccountBalanceAfter, CurrencyDescriptionPatternType.ShortDecimal)
			},
			new List<string>
			{
				"Time",
				"Reference",
				"Amount",
				"Balance"
			},
			actor.LineFormatLength,
			colour: Telnet.Yellow,
			truncatableColumnIndex: 1,
			unicodeTable: actor.Account.UseUnicode
		));
		return sb.ToString();
	}

	#region FutureProgs

	public FutureProgVariableTypes Type => FutureProgVariableTypes.BankAccount;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "accounttype":
				return BankAccountType;
			case "status":
				return new TextVariable(AccountStatus.DescribeEnum());
			case "balance":
				return new NumberVariable(CurrentBalance);
			case "bank":
				return Bank;
			case "accountnumber":
				return new NumberVariable(AccountNumber);
			default:
				throw new ApplicationException($"Invalid property {property} requested in BankAccount.GetProperty");
		}
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "status", FutureProgVariableTypes.Text },
			{ "bank", FutureProgVariableTypes.Bank },
			{ "balance", FutureProgVariableTypes.Number },
			{ "accounttype", FutureProgVariableTypes.BankAccountType },
			{ "accountnumber", FutureProgVariableTypes.Number }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The ID of the bank account" },
			{ "name", "The name of the bank account" },
			{ "status", "The status of the bank account; Active, Locked, Suspended or Closed" },
			{ "bank", "The bank this account is with" },
			{ "balance", "The current balance of this account" },
			{ "accounttype", "The account type for this account" },
			{ "accountnumber", "The account number to access this bank account" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.BankAccount, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public void DoLoad()
	{
		LoadTransactions();
	}

	#endregion
}