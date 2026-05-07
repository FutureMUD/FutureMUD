using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.TimeAndDate;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Economy;

public enum StableStayStatus
{
	Active = 0,
	Redeemed = 1,
	ReleasedByManager = 2,
	Deleted = 3
}

public enum StableLedgerEntryType
{
	LodgeFee = 0,
	DailyFee = 1,
	Payment = 2,
	Waiver = 3,
	AccountCharge = 4,
	AccountPayment = 5,
	ManagerRelease = 6,
	Redeemed = 7
}

public enum StableAccountAuthorisationFailureReason
{
	None,
	NotAuthorisedAccountUser,
	AccountOverbalanced,
	UserOverbalanced,
	AccountSuspended
}

public class StableAccountUser : IHavePersonalName
{
	public long Id { get; init; }
	public IPersonalName PersonalName { get; init; } = null!;
	public decimal? SpendingLimit { get; set; }
}

public interface IStableLedgerEntry : IFrameworkItem
{
	IStableStay Stay { get; }
	StableLedgerEntryType EntryType { get; }
	MudDateTime MudDateTime { get; }
	long? ActorId { get; }
	string? ActorName { get; }
	decimal Amount { get; }
	string Note { get; }
}

public interface IStableStay : IFrameworkItem, ISaveable
{
	IStable Stable { get; }
	ICharacter? Mount { get; }
	long MountId { get; }
	ICharacter? OriginalOwner { get; }
	long OriginalOwnerId { get; }
	IPersonalName? OriginalOwnerName { get; }
	MudDateTime LodgedDateTime { get; }
	MudDateTime LastDailyFeeDateTime { get; set; }
	MudDateTime? ClosedDateTime { get; }
	StableStayStatus Status { get; }
	bool IsActive { get; }
	long? TicketItemId { get; }
	string TicketToken { get; }
	decimal AmountOwing { get; }
	IEnumerable<IStableLedgerEntry> LedgerEntries { get; }
	void RegisterTicket(long ticketItemId, string ticketToken);
	void AddLedgerEntry(StableLedgerEntryType entryType, decimal amount, ICharacter? actor, string note);
	void AddCharge(StableLedgerEntryType entryType, decimal amount, ICharacter? actor, string note);
	void AddPayment(decimal amount, ICharacter? actor, string note);
	void WaiveOutstanding(ICharacter? actor, string note);
	void Close(StableStayStatus status, ICharacter? actor, string note);
	bool TicketMatches(IGameItem item, string token);
}

public interface IStableAccount : IFrameworkItem, ISaveable
{
	IStable Stable { get; }
	ICurrency Currency { get; }
	string AccountName { get; }
	long AccountOwnerId { get; }
	IPersonalName AccountOwnerName { get; }
	decimal Balance { get; }
	decimal CreditLimit { get; }
	bool IsSuspended { get; set; }
	IEnumerable<StableAccountUser> AccountUsers { get; }
	decimal CreditAvailable { get; }
	StableAccountAuthorisationFailureReason IsAuthorisedToUse(ICharacter actor, decimal amount);
	decimal MaximumAuthorisedToUse(ICharacter actor);
	bool IsAccountOwner(ICharacter actor);
	void SetAccountOwner(ICharacter actor);
	void AddAuthorisation(ICharacter actor, decimal? spendingLimit);
	void RemoveAuthorisation(StableAccountUser actor);
	void SetLimit(StableAccountUser user, decimal? spendingLimit);
	void SetCreditLimit(decimal limit);
	void ChargeAccount(decimal amount);
	void PayAccount(decimal amount);
	string Show(ICharacter actor);
}

public interface IStable : IFrameworkItem, ISaveable, IKeywordedItem
{
	IEconomicZone EconomicZone { get; set; }
	ICurrency Currency { get; }
	ICell Location { get; set; }
	IBankAccount? BankAccount { get; set; }
	decimal CashBalance { get; }
	decimal AvailableFunds { get; }
	bool IsTrading { get; }
	bool IsReadyToDoBusiness { get; }
	decimal LodgeFee { get; set; }
	decimal DailyFee { get; set; }
	IFutureProg? LodgeFeeProg { get; set; }
	IFutureProg? DailyFeeProg { get; set; }
	IFutureProg? CanStableProg { get; set; }
	IFutureProg? WhyCannotStableProg { get; set; }
	IEnumerable<IEmployeeRecord> EmployeeRecords { get; }
	IEnumerable<IStableStay> Stays { get; }
	IEnumerable<IStableStay> ActiveStays { get; }
	IEnumerable<IStableAccount> StableAccounts { get; }
	bool IsEmployee(ICharacter actor);
	bool IsManager(ICharacter actor);
	bool IsProprietor(ICharacter actor);
	void AddEmployee(ICharacter actor);
	void RemoveEmployee(IEmployeeRecord employee);
	void RemoveEmployee(ICharacter actor);
	void ClearEmployees();
	void SetManager(ICharacter actor, bool isManager);
	void SetProprietor(ICharacter actor, bool isProprietor);
	void ToggleIsTrading();
	decimal QuoteLodgeFee(ICharacter mount, ICharacter owner);
	decimal QuoteDailyFee(ICharacter mount, ICharacter owner);
	void AssessFees(IStableStay stay);
	void AssessAllActiveStays();
	(bool Truth, string Reason) CanUseStable(ICharacter actor, ICharacter? mount);
	(bool Truth, string Reason) CanLodge(ICharacter actor, ICharacter mount);
	IStableStay Lodge(ICharacter actor, ICharacter mount);
	(bool Truth, string Reason) CanRedeem(ICharacter actor, IStableTicket ticket);
	void Redeem(ICharacter actor, IStableStay stay);
	void Release(ICharacter actor, IStableStay stay, bool waiveFees);
	void AddStableAccount(IStableAccount account);
	void RemoveStableAccount(IStableAccount account);
	IStableAccount? AccountByName(string text);
	string Show(ICharacter actor);
    string ShowToNonEmployee(ICharacter actor);
    string ShowStay(ICharacter actor, IStableStay stay);
	void Delete();
}

public interface IStableTicket : IGameItemComponent
{
	long? StableStayId { get; }
	long? TicketItemId { get; }
	string TicketToken { get; }
	IStableStay? StableStay { get; }
	bool IsValid { get; }
	void InitialiseTicket(IStableStay stay);
}
