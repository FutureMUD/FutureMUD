using MoreLinq.Extensions;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System.Runtime.CompilerServices;

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
        AccountCreationDate = MudDateTime.FromStoredStringOrFallback(dbitem.AccountCreationDate, Gameworld,
            StoredMudDateTimeFallback.CurrentDateTime, "BankAccount", dbitem.Id, dbitem.Name, "AccountCreationDate");
        AccountNumber = dbitem.AccountNumber;
        AccountStatus = (BankAccountStatus)dbitem.AccountStatus;
        BankAccountType = Gameworld.BankAccountTypes.Get(dbitem.BankAccountTypeId);
        CurrentBalance = dbitem.CurrentBalance;
        CurrentMonthInterest = dbitem.CurrentMonthInterest;
        CurrentMonthFees = dbitem.CurrentMonthFees;
        _characterOwnerId = dbitem.AccountOwnerCharacterId;
        _clanOwnerId = dbitem.AccountOwnerClanId;
        _shopOwnerId = dbitem.AccountOwnerShopId;
        if (dbitem.AccountOwnerFrameworkItemId.HasValue &&
            !string.IsNullOrWhiteSpace(dbitem.AccountOwnerFrameworkItemType))
        {
            _accountOwnerReference =
                new FrameworkItemReference(dbitem.AccountOwnerFrameworkItemId.Value,
                    dbitem.AccountOwnerFrameworkItemType, Gameworld);
        }
        else if (_characterOwnerId.HasValue)
        {
            _accountOwnerReference = new FrameworkItemReference(_characterOwnerId.Value, "Character", Gameworld);
        }
        else if (_clanOwnerId.HasValue)
        {
            _accountOwnerReference = new FrameworkItemReference(_clanOwnerId.Value, "Clan", Gameworld);
        }
        else if (_shopOwnerId.HasValue)
        {
            _accountOwnerReference = new FrameworkItemReference(_shopOwnerId.Value, "Shop", Gameworld);
        }

        _nominatedBenefactor = dbitem.NominatedBenefactorAccountId;

        if (!string.IsNullOrEmpty(dbitem.AuthorisedBankPaymentItems))
        {
            foreach (long id in dbitem.AuthorisedBankPaymentItems.Split(' ').Select(x => long.Parse(x)))
            {
                _authorisedPaymentItems.Add(id);
            }
        }
    }

    public override string FrameworkItemType => "BankAccount";

    public override void Save()
    {
        Models.BankAccount dbitem = FMDB.Context.BankAccounts.Find(Id);
        dbitem.Name = _name;
        dbitem.AccountCreationDate = AccountCreationDate.GetDateTimeString();
        dbitem.AccountNumber = AccountNumber;
        dbitem.AccountStatus = (int)AccountStatus;
        dbitem.CurrentBalance = CurrentBalance;
        dbitem.BankAccountTypeId = BankAccountType.Id;
        dbitem.CurrentMonthInterest = CurrentMonthInterest;
        dbitem.CurrentMonthFees = CurrentMonthFees;
        SyncLegacyOwnerFields();
        dbitem.AccountOwnerCharacterId = _characterOwnerId;
        dbitem.AccountOwnerClanId = _clanOwnerId;
        dbitem.AccountOwnerShopId = _shopOwnerId;
        dbitem.AccountOwnerFrameworkItemId = _accountOwnerReference?.Id;
        dbitem.AccountOwnerFrameworkItemType = _accountOwnerReference?.FrameworkItemType;
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
    private FrameworkItemReference _accountOwnerReference;
    private IFrameworkItem _accountOwner;

    public bool IsAccountOwner(ICharacter character)
    {
        return _accountOwnerReference?.FrameworkItemType == "Character" &&
               _accountOwnerReference.Id == CharacterInstanceIdentityComparer.IdentityId(character);
    }

    public bool IsAccountOwner(IClan clan)
    {
        return IsAccountOwner((IFrameworkItem)clan);
    }

    public bool IsAccountOwner(IShop shop)
    {
        return IsAccountOwner((IFrameworkItem)shop);
    }

    public bool IsAccountOwner(IFrameworkItem owner)
    {
        if (owner is ICharacter character)
        {
            return IsAccountOwner(character);
        }

        return _accountOwnerReference?.Equals(owner) == true;
    }

    public IFrameworkItem AccountOwner
    {
        get
        {
            if (_accountOwner == null && _accountOwnerReference != null)
            {
                _accountOwner = _accountOwnerReference.GetItem;
            }

            return _accountOwner ?? throw new ApplicationException("There was no account owner set for a bank account.");
        }
    }

	public long AccountOwnerId => _accountOwnerReference?.Id ?? 0;
	public string AccountOwnerFrameworkItemType => _accountOwnerReference?.FrameworkItemType ?? string.Empty;

    public void SetAccountOwner(IFrameworkItem owner)
    {
        _accountOwner = owner;
        _accountOwnerReference = owner == null
            ? null
            : new FrameworkItemReference(CharacterInstanceIdentityComparer.FrameworkItemId(owner), owner.FrameworkItemType, Gameworld);
        SyncLegacyOwnerFields();
        Changed = true;
    }

    public bool IsAuthorisedAccountUser(ICharacter character)
    {
        if (IsAccountOwner(character))
        {
            return true;
        }

        return AccountOwner switch
        {
            IShop shop => shop.IsManager(character),
            IClan clan => clan.Memberships
                .FirstOrDefault(x => x.MemberId == CharacterInstanceIdentityComparer.IdentityId(character))?.NetPrivileges
                .HasFlag(ClanPrivilegeType.CanManageBankAccounts) == true,
            _ => false
        };
    }

    private void SyncLegacyOwnerFields()
    {
        _characterOwnerId = null;
        _clanOwnerId = null;
        _shopOwnerId = null;
        switch (_accountOwnerReference?.FrameworkItemType)
        {
            case "Character":
                _characterOwnerId = _accountOwnerReference.Id;
                break;
            case "Clan":
                _clanOwnerId = _accountOwnerReference.Id;
                break;
            case "Shop":
                _shopOwnerId = _accountOwnerReference.Id;
                break;
        }
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
        IGameItemProto? proto = Gameworld.ItemProtos.Get(BankAccountType.PaymentItemPrototype ?? 0);
        if (proto is null || proto.Status != RevisionStatus.Current)
        {
            return null;
        }

        IGameItem item = proto.CreateNew();
        IBankPaymentItem? payment = item.GetItemType<IBankPaymentItem>();
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
    private readonly List<IBankAccountTransaction> _recentTransactions = new();
    private readonly List<IBankAccountTransaction> _loadedTransactions = new();
    private readonly List<IBankAccountTransaction> _displayedTransactions = new();
    private readonly HashSet<long> _loadedTransactionIds = new();
    private bool _transactionHistoryLoadRequested;
    private bool _transactionHistoryLoadQueued;
    private long? _nextTransactionHistoryId;

    public IEnumerable<IBankAccountTransaction> Transactions => _transactionHistoryLoadRequested
        ? _loadedTransactions.ToList()
        : GetRecentTransactions();

    private IEnumerable<IBankAccountTransaction> GetRecentTransactions()
    {
        List<IBankAccountTransaction> transactions = _recentTransactions.ToList();
        HashSet<long> recentTransactionIds = new(_recentTransactions
            .Where(x => x.Id != 0L)
            .Select(x => x.Id));

        using (new FMDB())
        {
            transactions.AddRange(BankAccountTransactionHistory
                .LoadMostRecent(FMDB.Context.BankAccountTransactions, Id)
                .Where(x => !recentTransactionIds.Contains(x.Id))
                .Select(x => new BankAccountTransaction(x, this)));
        }

        return transactions
            .OrderByDescending(x => x.TransactionTime)
            .ThenByDescending(x => x.Id)
            .Take(BankAccountTransactionHistory.MaximumTransactionHistoryEntries)
            .ToList();
    }

    private void RequestTransactionHistoryLazyLoad()
    {
        if (_transactionHistoryLoadRequested)
        {
            return;
        }

        List<IBankAccountTransaction> persistedTransactions;
        using (new FMDB())
        {
            persistedTransactions = BankAccountTransactionHistory
                .LoadMostRecent(FMDB.Context.BankAccountTransactions, Id)
                .Select(x => (IBankAccountTransaction)new BankAccountTransaction(x, this))
                .ToList();
        }

        _transactionHistoryLoadRequested = true;
        foreach (IBankAccountTransaction transaction in persistedTransactions)
        {
            AddLoadedTransaction(transaction);
        }

        foreach (IBankAccountTransaction transaction in _recentTransactions)
        {
            AddLoadedTransaction(transaction);
        }

        AddDisplayedTransactions(persistedTransactions.Concat(_recentTransactions));
        _nextTransactionHistoryId = persistedTransactions.Count == BankAccountTransactionHistory.MaximumTransactionHistoryEntries
            ? persistedTransactions[^1].Id
            : null;
        QueueNextTransactionHistoryLoad();
    }

    private void QueueNextTransactionHistoryLoad()
    {
        if (!_nextTransactionHistoryId.HasValue || _transactionHistoryLoadQueued)
        {
            return;
        }

        _transactionHistoryLoadQueued = true;
        Gameworld.SaveManager.AddLazyLoad(this);
    }

    private void AddLoadedTransaction(IBankAccountTransaction transaction)
    {
        if (transaction.Id != 0L && !_loadedTransactionIds.Add(transaction.Id))
        {
            return;
        }

        if (transaction.Id == 0L && _loadedTransactions.Contains(transaction))
        {
            return;
        }

        _loadedTransactions.Add(transaction);
    }

    private void AddDisplayedTransactions(IEnumerable<IBankAccountTransaction> transactions)
    {
        foreach (IBankAccountTransaction transaction in transactions)
        {
            if (transaction.Id != 0L && _displayedTransactions.Any(x => x.Id == transaction.Id))
            {
                continue;
            }

            if (transaction.Id == 0L && _displayedTransactions.Contains(transaction))
            {
                continue;
            }

            _displayedTransactions.Add(transaction);
        }

        List<IBankAccountTransaction> displayed = _displayedTransactions
            .OrderByDescending(x => x.TransactionTime)
            .ThenByDescending(x => x.Id)
            .Take(BankAccountTransactionHistory.MaximumTransactionHistoryEntries)
            .ToList();
        _displayedTransactions.Clear();
        _displayedTransactions.AddRange(displayed);
    }

    private void RecordTransaction(BankAccountTransaction transaction)
    {
        _recentTransactions.Add(transaction);
        if (_recentTransactions.Count > BankAccountTransactionHistory.MaximumTransactionHistoryEntries)
        {
            _recentTransactions.RemoveRange(0,
                _recentTransactions.Count - BankAccountTransactionHistory.MaximumTransactionHistoryEntries);
        }

        if (_transactionHistoryLoadRequested)
        {
            AddLoadedTransaction(transaction);
            AddDisplayedTransactions([transaction]);
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
        MudDateTime time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.Withdrawal, amount,
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
        MudDateTime time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.WithdrawalFromTransaction, amount,
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
        MudDateTime time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.WithdrawalFromTransfer, amount,
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
        MudDateTime time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.Deposit, amount, CurrentBalance + amount,
            time, $"Deposit {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} at branch"));
        CurrentBalance += amount;
        BankAccountType.DoTransactionFeesDeposit(this, amount);
        Changed = true;
    }

    public void DepositFromTransfer(decimal amount, string fromBankCode, int fromAccount, string transferReference)
    {
        MudDateTime time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.DepositFromTransfer, amount,
            CurrentBalance + amount, time,
            $"Deposit {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} from {fromAccount} ({fromBankCode}){(!string.IsNullOrEmpty(transferReference) ? $" - {transferReference}" : "")}"));
        CurrentBalance += amount;
        BankAccountType.DoTransactionFeesDepositFromTransfer(this, amount);
        Changed = true;
    }

    public void DepositFromTransaction(decimal amount, string transactionReference)
    {
        MudDateTime time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.DepositFromTransaction, amount,
            CurrentBalance + amount, time,
            $"Deposit {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} from transaction{(!string.IsNullOrEmpty(transactionReference) ? $" - {transactionReference}" : "")}"));
        CurrentBalance += amount;
        BankAccountType.DoTransactionFeesDepositFromTransfer(this, amount);
        Changed = true;
    }

    public void DoAccountCredit(decimal amount, string reason)
    {
        MudDateTime time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.DepositFromTransfer, amount,
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
        decimal change = CurrentBalance + CurrentMonthInterest - CurrentMonthFees;
        CurrentBalance = 0.0M;
        CurrentMonthInterest = 0.0M;
        CurrentMonthFees = 0.0M;
        MudDateTime time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.Withdrawal, change, 0.0M, time,
            $"Account Rolled Over and Finalised"));
        Changed = true;
    }

    public void FinaliseMonth()
    {
        MudDateTime time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
        CurrentBalance -= CurrentMonthFees;
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.ServiceFee, CurrentMonthFees,
            CurrentBalance, time, $"Monthly Account Fees"));
        CurrentBalance += CurrentMonthInterest;
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.InterestEarned, CurrentMonthInterest,
            CurrentBalance, time, $"Interest Earned"));
        CurrentMonthFees = 0.0M;
        CurrentMonthInterest = 0.0M;
        Changed = true;
    }

    public void ChargeFee(decimal amount, BankTransactionType transactionType, string feeDescription)
    {
        MudDateTime time = Bank.EconomicZone.ZoneForTimePurposes.DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar);
        string feeType = "Other Fee";
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
        RecordTransaction(new BankAccountTransaction(this, BankTransactionType.ServiceFee, amount, CurrentBalance, time,
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
        StringBuilder sb = new();
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

        switch (AccountOwner)
        {
            case ICharacter charOwner when CharacterInstanceIdentityComparer.SameIdentity(charOwner, actor):
                sb.AppendLine($"Owner: {"You".ColourCharacter()}");
                break;
            case ICharacter charOwner:
                sb.AppendLine(
                    $"Owner: {charOwner.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreLoadThings)} ({charOwner.PersonalName.GetName(Character.Name.NameStyle.FullName).ColourName()})");
                break;
            case IShop shop:
                sb.AppendLine($"Owner: {shop.Name.ColourName()} (Shop)");
                break;
            case IClan clan:
                sb.AppendLine($"Owner: {clan.FullName.ColourName()} (Clan)");
                break;
            default:
                sb.AppendLine($"Owner: {AccountOwner.FrameworkItemType.ColourName()} #{AccountOwner.Id.ToString("N0", actor)}");
                break;
        }

        sb.AppendLine($"Fees: See {$"bank preview {BankAccountType.Name.ToLowerInvariant()}".MXPSend()}");
        sb.AppendLine();
        sb.AppendLine(
            $"Current Balance: {Currency.Describe(CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        sb.AppendLine(
            $"Available Balance: {Currency.Describe(MaximumWithdrawal(), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        IGameItemProto proto = Gameworld.ItemProtos.Get(BankAccountType.PaymentItemPrototype ?? 0);
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
        RequestTransactionHistoryLazyLoad();
        StringBuilder sb = new();
        if (actor.IsAdministrator())
        {
            sb.AppendLine($"Transaction History for Bank Account #{Id.ToString("N0", actor)} - {Name.ColourName()}");
        }
        else
        {
            sb.AppendLine($"Transaction History for {Name.ColourName()}");
        }

        sb.AppendLine();
        sb.AppendLine($"Showing up to {BankAccountTransactionHistory.MaximumTransactionHistoryEntries.ToString("N0", actor)} most recently recorded transactions.");
        sb.Append(StringUtilities.GetTextTable(
            from transaction in _displayedTransactions
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

    public ProgVariableTypes Type => ProgVariableTypes.BankAccount;
    public object GetObject => this;

    public IProgVariable GetProperty(string property)
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

    private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
    {
        return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", ProgVariableTypes.Number },
            { "name", ProgVariableTypes.Text },
            { "status", ProgVariableTypes.Text },
            { "bank", ProgVariableTypes.Bank },
            { "balance", ProgVariableTypes.Number },
            { "accounttype", ProgVariableTypes.BankAccountType },
            { "accountnumber", ProgVariableTypes.Number }
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

    public static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.BankAccount, DotReferenceHandler(),
            DotReferenceHelp());
    }

    public void DoLoad()
    {
        _transactionHistoryLoadQueued = false;
        if (!_transactionHistoryLoadRequested || !_nextTransactionHistoryId.HasValue)
        {
            return;
        }

        List<IBankAccountTransaction> transactions;
        using (new FMDB())
        {
            transactions = BankAccountTransactionHistory
                .LoadOlderThan(FMDB.Context.BankAccountTransactions, Id, _nextTransactionHistoryId.Value)
                .Select(x => (IBankAccountTransaction)new BankAccountTransaction(x, this))
                .ToList();
        }

        foreach (IBankAccountTransaction transaction in transactions)
        {
            AddLoadedTransaction(transaction);
        }

        _nextTransactionHistoryId = transactions.Count == BankAccountTransactionHistory.MaximumTransactionHistoryEntries
            ? transactions[^1].Id
            : null;
        QueueNextTransactionHistoryLoad();
    }

    #endregion
}
