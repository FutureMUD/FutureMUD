using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Community.Boards;
using MudSharp.Economy.Currency;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Economy.Employment;

public enum EmploymentHostType
{
	Shop,
	AuctionHouse,
	Arena,
	Bank,
	Stable,
	Hotel,
	Other
}

public enum EmploymentRole
{
	Employee,
	Manager,
	Proprietor,
	Clerk,
	Courier,
	Crafter,
	StableHand,
	BankTeller,
	HotelWorker
}

public enum EmploymentStatus
{
	Active,
	Suspended,
	Ended
}

[Flags]
public enum EmploymentAuthority
{
	None = 0,
	ViewEmployees = 1 << 0,
	HireEmployees = 1 << 1,
	FireEmployees = 1 << 2,
	CreateJobOpenings = 1 << 3,
	ModifyJobOpenings = 1 << 4,
	SetPayWithinBand = 1 << 5,
	AssignTasks = 1 << 6,
	CancelTasks = 1 << 7,
	CreateScheduledRules = 1 << 8,
	ModifyScheduledRules = 1 << 9,
	CreateManagerGoals = 1 << 10,
	ModifyManagerGoals = 1 << 11,
	ApprovePurchases = 1 << 12,
	UseStoreAccount = 1 << 13,
	WithdrawBusinessCash = 1 << 14,
	DepositBusinessCash = 1 << 15,
	ManageStockRules = 1 << 16,
	ManageCraftRules = 1 << 17,
	ManageDeliveryRoutes = 1 << 18,
	AdjustPrices = 1 << 19,
	PayTaxes = 1 << 20,
	PostToHostBoard = 1 << 21,
	ModerateHostBoard = 1 << 22,
	ManagePayroll = 1 << 23
}

public readonly record struct EmploymentAuthoritySet(EmploymentAuthority Authorities)
{
	public static EmploymentAuthoritySet Empty => new(EmploymentAuthority.None);
	public static EmploymentAuthoritySet All => new(
		EmploymentAuthority.ViewEmployees |
		EmploymentAuthority.HireEmployees |
		EmploymentAuthority.FireEmployees |
		EmploymentAuthority.CreateJobOpenings |
		EmploymentAuthority.ModifyJobOpenings |
		EmploymentAuthority.SetPayWithinBand |
		EmploymentAuthority.AssignTasks |
		EmploymentAuthority.CancelTasks |
		EmploymentAuthority.CreateScheduledRules |
		EmploymentAuthority.ModifyScheduledRules |
		EmploymentAuthority.CreateManagerGoals |
		EmploymentAuthority.ModifyManagerGoals |
		EmploymentAuthority.ApprovePurchases |
		EmploymentAuthority.UseStoreAccount |
		EmploymentAuthority.WithdrawBusinessCash |
		EmploymentAuthority.DepositBusinessCash |
		EmploymentAuthority.ManageStockRules |
		EmploymentAuthority.ManageCraftRules |
		EmploymentAuthority.ManageDeliveryRoutes |
		EmploymentAuthority.AdjustPrices |
		EmploymentAuthority.PayTaxes |
		EmploymentAuthority.PostToHostBoard |
		EmploymentAuthority.ModerateHostBoard |
		EmploymentAuthority.ManagePayroll);

	public bool Contains(EmploymentAuthority authority)
	{
		return authority == EmploymentAuthority.None || (Authorities & authority) == authority;
	}

	public bool ContainsAll(EmploymentAuthoritySet required)
	{
		return (Authorities & required.Authorities) == required.Authorities;
	}

	public EmploymentAuthoritySet Grant(EmploymentAuthority authority)
	{
		return new EmploymentAuthoritySet(Authorities | authority);
	}

	public EmploymentAuthoritySet Revoke(EmploymentAuthority authority)
	{
		return new EmploymentAuthoritySet(Authorities & ~authority);
	}

	public static implicit operator EmploymentAuthoritySet(EmploymentAuthority authority)
	{
		return new EmploymentAuthoritySet(authority);
	}
}

public enum EmploymentTerminationReason
{
	Fired,
	Resigned,
	Expired,
	HostClosed,
	ManuallyCancelled,
	SystemCancelled,
	UnpaidWages
}

public enum PayCadence
{
	Unpaid,
	PerTask,
	Hourly,
	Daily,
	Weekly,
	Salary,
	Commission,
	Mixed
}

public enum MarketRateBindingType
{
	None,
	Multiplier,
	Floor,
	Premium
}

public enum PaymentSource
{
	HostCash,
	HostBankAccount,
	SpecifiedEmployerAccount,
	StoreAccount,
	EmployeeFloat
}

public enum PaymentMethodKind
{
	Cash,
	EmployeeBankAccount,
	SpecifiedBankAccount,
	PaymentItem,
	EmployerFloat
}

public enum JobOpeningStatus
{
	Open,
	Suspended,
	Filled,
	Closed
}

public enum JobApplicationStatus
{
	Pending,
	Accepted,
	Rejected,
	Withdrawn
}

public enum EmploymentAICapability
{
	CanPurchaseCommodities,
	CanDeliverItems,
	CanUseBankAccount,
	CanUseVehicles,
	CanCraft,
	CanExecuteCommandTask,
	CanPostToBoard,
	CanManagePrices,
	CanHandleCash,
	CanManageStableAnimals,
	CanManageHotelRooms
}

public sealed record MoneyAmount(ICurrency Currency, decimal Amount)
{
	public bool IsPositive => Amount > 0.0M;
}

public sealed record MarketRateBinding(MarketRateBindingType BindingType, decimal Value);

public sealed record CompensationTerms(
	MoneyAmount? FixedRate,
	MarketRateBinding? MarketBinding,
	PayCadence Cadence,
	MoneyAmount? MinimumEffectivePay,
	PaymentSource EmployerPaymentSource)
{
	public bool IsPaid => Cadence != PayCadence.Unpaid;
	public decimal NominalAmount => FixedRate?.Amount ?? MinimumEffectivePay?.Amount ?? 0.0M;
}

public sealed record WorkSchedule(string Description, TimeSpan? StartsAt = null, TimeSpan? EndsAt = null)
{
	public static WorkSchedule AnyTime { get; } = new("Any time");
}

public enum EmploymentDurationType
{
	Indefinite,
	FixedTerm,
	Seasonal,
	TaskLimited
}

public sealed record EmploymentDuration(EmploymentDurationType DurationType, TimeSpan? Length = null)
{
	public static EmploymentDuration Indefinite { get; } = new(EmploymentDurationType.Indefinite);
}

public sealed record PaymentMethod(
	PaymentMethodKind MethodKind,
	IBankAccount? BankAccount = null,
	IFrameworkItem? PaymentItemPrototype = null,
	string? Notes = null);

public sealed record EmploymentOffer(
	EmploymentRole Role,
	CompensationTerms Compensation,
	WorkSchedule Schedule,
	EmploymentDuration Duration,
	PaymentMethod PaymentMethod,
	EmploymentAuthoritySet Authority);

public interface IEmploymentContract
{
	long Id { get; }
	IEmploymentHost Employer { get; }
	ICharacter Employee { get; }
	EmploymentRole Role { get; }
	EmploymentStatus Status { get; }
	EmploymentAuthoritySet Authority { get; }
	CompensationTerms Compensation { get; }
	WorkSchedule Schedule { get; }
	EmploymentDuration Duration { get; }
	PaymentMethod PaymentMethod { get; }
	DateTimeOffset StartedAt { get; }
	DateTimeOffset? EndsAt { get; }
	EmploymentTerminationReason? EndReason { get; }
}

public interface IJobOpening
{
	long Id { get; }
	IEmploymentHost Employer { get; }
	EmploymentRole Role { get; }
	JobRequirementSet Requirements { get; }
	CompensationTerms Compensation { get; }
	WorkSchedule Schedule { get; }
	EmploymentDuration Duration { get; }
	PaymentMethod PaymentMethod { get; }
	EmploymentAuthoritySet Authority { get; }
	JobOpeningStatus Status { get; }
	int MaxPositions { get; }
	bool NpcApplicationsOnly { get; }
	bool AcceptsApplications { get; }
}

public sealed record SkillRequirement(string SkillName, double MinimumValue);

public sealed record KnowledgeRequirement(string KnowledgeName);

public sealed record AICapabilityRequirement(EmploymentAICapability Capability);

public sealed record TagRequirement(string TagName);

public sealed record JobRequirementSet(
	IReadOnlyCollection<SkillRequirement> Skills,
	IReadOnlyCollection<KnowledgeRequirement> Knowledges,
	IReadOnlyCollection<AICapabilityRequirement> Capabilities,
	IReadOnlyCollection<TagRequirement> Tags)
{
	public static JobRequirementSet None { get; } = new(
		Array.Empty<SkillRequirement>(),
		Array.Empty<KnowledgeRequirement>(),
		Array.Empty<AICapabilityRequirement>(),
		Array.Empty<TagRequirement>());
}

public sealed record EmploymentCandidateProfile(
	ICharacter Candidate,
	decimal ReservationWage,
	IReadOnlyDictionary<string, double> Skills,
	IReadOnlySet<string> Knowledges,
	IReadOnlySet<EmploymentAICapability> Capabilities,
	IReadOnlySet<string> Tags,
	IReadOnlyCollection<PaymentMethodKind> AcceptedPaymentMethods,
	ICurrency? ReservationWageCurrency = null);

public interface IEmploymentApplication
{
	long Id { get; }
	IJobOpening Opening { get; }
	ICharacter Candidate { get; }
	DateTimeOffset AppliedAt { get; }
	JobApplicationStatus Status { get; }
	string? DecisionReason { get; }
}

public enum EmploymentLedgerEntryType
{
	Wage,
	Purchase,
	BankDeposit,
	BankWithdrawal,
	StoreAccountPayment,
	TaxPayment,
	PaymentAuthorisation,
	ExistingFinancialRecordReuse
}

public interface IEmploymentLedgerEntry
{
	Guid CorrelationId { get; }
	EmploymentLedgerEntryType EntryType { get; }
	IEmploymentHost Employer { get; }
	ICharacter? Actor { get; }
	MoneyAmount? Amount { get; }
	string Description { get; }
	DateTimeOffset RecordedAt { get; }
}

public interface IEmploymentLedger
{
	IReadOnlyCollection<IEmploymentLedgerEntry> Entries { get; }
	IEmploymentLedgerEntry Record(EmploymentLedgerEntryType entryType, ICharacter? actor, MoneyAmount? amount,
		string description, Guid? correlationId = null);
}

public enum EmploymentPayableStatus
{
	Accrued,
	ReadyToClaim,
	Claimed,
	Settled,
	Waived
}

public interface IEmploymentPayable
{
	long Id { get; }
	Guid CorrelationId { get; }
	IEmploymentHost Employer { get; }
	long? ContractId { get; }
	long EmployeeId { get; }
	string EmployeeName { get; }
	EmploymentRole Role { get; }
	MoneyAmount Amount { get; }
	PayCadence Cadence { get; }
	PaymentMethod PaymentMethod { get; }
	DateTimeOffset PayPeriodStart { get; }
	DateTimeOffset PayPeriodEnd { get; }
	DateTimeOffset DueAt { get; }
	DateTimeOffset AccruedAt { get; }
	DateTimeOffset? SettledAt { get; }
	DateTimeOffset? ClaimedAt { get; }
	EmploymentPayableStatus Status { get; }
	string? SettlementNote { get; }
	int DaysOverdue(DateTimeOffset now);
}

public interface IEmploymentPayroll
{
	IReadOnlyCollection<IEmploymentPayable> Payables { get; }
	IReadOnlyCollection<IEmploymentPayable> OutstandingLiabilities { get; }
	IReadOnlyCollection<IEmploymentPayable> EvaluatePayroll();
	IReadOnlyCollection<IEmploymentPayable> EvaluatePayroll(DateTimeOffset now);
	IReadOnlyCollection<IEmploymentPayable> ClaimablePayablesFor(ICharacter employee);
	int MaximumOverdueDays();
	int MaximumOverdueDays(DateTimeOffset now);
	bool TrySettlePayables(IEnumerable<IEmploymentPayable> payables, ICharacter? actor, bool makeClaimable,
		string reason, out string message);
	bool TryClaimPayable(IEmploymentPayable payable, ICharacter actor, out string message);
}

public enum EmploymentRegisterEntryType
{
	ContractHired,
	ContractSuspended,
	ContractEnded,
	JobOpeningCreated,
	JobOpeningModified,
	JobOpeningClosed,
	ApplicationSubmitted,
	ApplicationAccepted,
	ApplicationRejected,
	AuthorityChanged,
	BoardPostCreated,
	ManagerGoalCreated,
	ManagerGoalEvaluated,
	ManagerGoalCancelled,
	ScheduledRuleCreated,
	ScheduledRuleEvaluated,
	ActiveTaskCreated,
	ActiveTaskAssigned,
	ActiveTaskBlocked,
	ActiveTaskCompleted,
	ActiveTaskFailed,
	ActionStepStarted,
	ActionStepCompleted,
	ActionStepFailed,
	PaymentAuthorisationGranted,
	PaymentAuthorisationUsed,
	CommandExecuted,
	ActiveTaskCancelled,
	WageAccrued,
	WageSettled,
	WageClaimed,
	EmployeeResignedUnpaid,
	AuditActionRecorded
}

public interface IEmploymentRegisterEntry
{
	Guid CorrelationId { get; }
	EmploymentRegisterEntryType EntryType { get; }
	IEmploymentHost Employer { get; }
	ICharacter? Actor { get; }
	string Description { get; }
	DateTimeOffset RecordedAt { get; }
}

public interface IEmploymentRegister
{
	IReadOnlyCollection<IEmploymentRegisterEntry> Entries { get; }
	IEmploymentRegisterEntry Record(EmploymentRegisterEntryType entryType, ICharacter? actor, string description,
		Guid? correlationId = null);
}
