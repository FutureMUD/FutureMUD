using System;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Models;

public partial class EmploymentHostState
{
	public EmploymentHostState()
	{
		Contracts = new HashSet<EmploymentContractRecord>();
		JobOpenings = new HashSet<EmploymentJobOpeningRecord>();
		ActionPlans = new HashSet<EmploymentActionPlanRecord>();
		ScheduledTaskRules = new HashSet<EmploymentScheduledTaskRuleRecord>();
		ConditionPredicates = new HashSet<EmploymentConditionPredicateRecord>();
		ScheduledRuleTemplates = new HashSet<EmploymentScheduledRuleTemplateRecord>();
		ActiveTasks = new HashSet<EmploymentActiveTaskRecord>();
		ManagerGoals = new HashSet<EmploymentManagerGoalRecord>();
		RegisterEntries = new HashSet<EmploymentRegisterEntryRecord>();
		LedgerEntries = new HashSet<EmploymentLedgerEntryRecord>();
		Payables = new HashSet<EmploymentPayableRecord>();
	}

	public long Id { get; set; }
	public string HostType { get; set; } = string.Empty;
	public long HostId { get; set; }
	public long BoardId { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime LastUpdatedAt { get; set; }

	public virtual Board Board { get; set; } = null!;
	public virtual ICollection<EmploymentContractRecord> Contracts { get; set; }
	public virtual ICollection<EmploymentJobOpeningRecord> JobOpenings { get; set; }
	public virtual ICollection<EmploymentActionPlanRecord> ActionPlans { get; set; }
	public virtual ICollection<EmploymentScheduledTaskRuleRecord> ScheduledTaskRules { get; set; }
	public virtual ICollection<EmploymentConditionPredicateRecord> ConditionPredicates { get; set; }
	public virtual ICollection<EmploymentScheduledRuleTemplateRecord> ScheduledRuleTemplates { get; set; }
	public virtual ICollection<EmploymentActiveTaskRecord> ActiveTasks { get; set; }
	public virtual ICollection<EmploymentManagerGoalRecord> ManagerGoals { get; set; }
	public virtual ICollection<EmploymentRegisterEntryRecord> RegisterEntries { get; set; }
	public virtual ICollection<EmploymentLedgerEntryRecord> LedgerEntries { get; set; }
	public virtual ICollection<EmploymentPayableRecord> Payables { get; set; }
}

public partial class EmploymentContractRecord
{
	public long Id { get; set; }
	public long RuntimeId { get; set; }
	public long EmploymentHostStateId { get; set; }
	public long EmployeeId { get; set; }
	public int Role { get; set; }
	public int Status { get; set; }
	public long Authority { get; set; }
	public long? FixedRateCurrencyId { get; set; }
	public decimal? FixedRateAmount { get; set; }
	public int MarketBindingType { get; set; }
	public decimal? MarketBindingValue { get; set; }
	public int PayCadence { get; set; }
	public long? MinimumEffectivePayCurrencyId { get; set; }
	public decimal? MinimumEffectivePayAmount { get; set; }
	public int EmployerPaymentSource { get; set; }
	public string ScheduleDescription { get; set; } = string.Empty;
	public long? ScheduleStartTicks { get; set; }
	public long? ScheduleEndTicks { get; set; }
	public int DurationType { get; set; }
	public long? DurationTicks { get; set; }
	public int PaymentMethodKind { get; set; }
	public long? PaymentBankAccountId { get; set; }
	public long? PaymentItemId { get; set; }
	public string? PaymentItemType { get; set; }
	public string? PaymentNotes { get; set; }
	public DateTime StartedAt { get; set; }
	public DateTime? EndsAt { get; set; }
	public int? EndReason { get; set; }
	public long? OriginOpeningId { get; set; }
	public long? OriginApplicationId { get; set; }

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
}

public partial class EmploymentJobOpeningRecord
{
	public EmploymentJobOpeningRecord()
	{
		Requirements = new HashSet<EmploymentJobOpeningRequirement>();
		Applications = new HashSet<EmploymentApplicationRecord>();
	}

	public long Id { get; set; }
	public long RuntimeId { get; set; }
	public long EmploymentHostStateId { get; set; }
	public int Role { get; set; }
	public int Status { get; set; }
	public int MaxPositions { get; set; }
	public bool NpcApplicationsOnly { get; set; }
	public long Authority { get; set; }
	public long? FixedRateCurrencyId { get; set; }
	public decimal? FixedRateAmount { get; set; }
	public int MarketBindingType { get; set; }
	public decimal? MarketBindingValue { get; set; }
	public int PayCadence { get; set; }
	public long? MinimumEffectivePayCurrencyId { get; set; }
	public decimal? MinimumEffectivePayAmount { get; set; }
	public int EmployerPaymentSource { get; set; }
	public string ScheduleDescription { get; set; } = string.Empty;
	public long? ScheduleStartTicks { get; set; }
	public long? ScheduleEndTicks { get; set; }
	public int DurationType { get; set; }
	public long? DurationTicks { get; set; }
	public int PaymentMethodKind { get; set; }
	public long? PaymentBankAccountId { get; set; }
	public long? PaymentItemId { get; set; }
	public string? PaymentItemType { get; set; }
	public string? PaymentNotes { get; set; }
	public int RevisionNumber { get; set; }

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
	public virtual ICollection<EmploymentJobOpeningRequirement> Requirements { get; set; }
	public virtual ICollection<EmploymentApplicationRecord> Applications { get; set; }
}

public partial class EmploymentJobOpeningRequirement
{
	public long Id { get; set; }
	public long EmploymentJobOpeningId { get; set; }
	public int RequirementType { get; set; }
	public string Name { get; set; } = string.Empty;
	public double? NumericValue { get; set; }
	public int? Capability { get; set; }

	public virtual EmploymentJobOpeningRecord EmploymentJobOpening { get; set; } = null!;
}

public partial class EmploymentApplicationRecord
{
	public long Id { get; set; }
	public long RuntimeId { get; set; }
	public long EmploymentJobOpeningId { get; set; }
	public long CandidateId { get; set; }
	public DateTime AppliedAt { get; set; }
	public int Status { get; set; }
	public string? DecisionReason { get; set; }
	public int OfferedOpeningRevision { get; set; }
	public string? CandidateProfileJson { get; set; }

	public virtual EmploymentJobOpeningRecord EmploymentJobOpening { get; set; } = null!;
}

public partial class EmploymentPayableRecord
{
	public long Id { get; set; }
	public long RuntimeId { get; set; }
	public long EmploymentHostStateId { get; set; }
	public string CorrelationId { get; set; } = string.Empty;
	public long? ContractRuntimeId { get; set; }
	public long EmployeeId { get; set; }
	public string EmployeeName { get; set; } = string.Empty;
	public int Role { get; set; }
	public long AmountCurrencyId { get; set; }
	public decimal Amount { get; set; }
	public int PayCadence { get; set; }
	public int PaymentMethodKind { get; set; }
	public long? PaymentBankAccountId { get; set; }
	public long? PaymentItemId { get; set; }
	public string? PaymentItemType { get; set; }
	public string? PaymentNotes { get; set; }
	public DateTime PayPeriodStart { get; set; }
	public DateTime PayPeriodEnd { get; set; }
	public DateTime DueAt { get; set; }
	public DateTime AccruedAt { get; set; }
	public int Status { get; set; }
	public DateTime? SettledAt { get; set; }
	public DateTime? ClaimedAt { get; set; }
	public string? SettlementNote { get; set; }

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
}

public partial class EmploymentActionPlanRecord
{
	public EmploymentActionPlanRecord()
	{
		Steps = new HashSet<EmploymentActionStepRecord>();
		ScheduledTaskRules = new HashSet<EmploymentScheduledTaskRuleRecord>();
		ScheduledRuleTemplates = new HashSet<EmploymentScheduledRuleTemplateRecord>();
		ActiveTasks = new HashSet<EmploymentActiveTaskRecord>();
		ManagerGoals = new HashSet<EmploymentManagerGoalRecord>();
	}

	public long Id { get; set; }
	public long EmploymentHostStateId { get; set; }
	public string Name { get; set; } = string.Empty;

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
	public virtual ICollection<EmploymentActionStepRecord> Steps { get; set; }
	public virtual ICollection<EmploymentScheduledTaskRuleRecord> ScheduledTaskRules { get; set; }
	public virtual ICollection<EmploymentScheduledRuleTemplateRecord> ScheduledRuleTemplates { get; set; }
	public virtual ICollection<EmploymentActiveTaskRecord> ActiveTasks { get; set; }
	public virtual ICollection<EmploymentManagerGoalRecord> ManagerGoals { get; set; }
}

public partial class EmploymentActionStepRecord
{
	public long Id { get; set; }
	public long EmploymentActionPlanId { get; set; }
	public int SortOrder { get; set; }
	public int StepType { get; set; }
	public long RequiredAuthority { get; set; }
	public string RequiredCapabilities { get; set; } = string.Empty;
	public bool RequiresPaymentAuthorisation { get; set; }
	public bool IsFinancialStep { get; set; }
	public string? Description { get; set; }
	public long? AmountCurrencyId { get; set; }
	public decimal? Amount { get; set; }
	public string? ExistingFinancialRecord { get; set; }
	public long? DestinationCellId { get; set; }
	public long? ExecutionCellId { get; set; }
	public string? CommandName { get; set; }
	public string? CommandArguments { get; set; }
	public string? AccountName { get; set; }
	public string? BoardTitle { get; set; }
	public string? BoardText { get; set; }

	public virtual EmploymentActionPlanRecord EmploymentActionPlan { get; set; } = null!;
}

public partial class EmploymentScheduledTaskRuleRecord
{
	public EmploymentScheduledTaskRuleRecord()
	{
		Conditions = new HashSet<EmploymentTaskConditionRecord>();
	}

	public long Id { get; set; }
	public string PublicId { get; set; } = string.Empty;
	public long EmploymentHostStateId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string IdempotencyKey { get; set; } = string.Empty;
	public long EmploymentActionPlanId { get; set; }
	public string? ExpressionJson { get; set; }
	public int Status { get; set; }
	public long CooldownTicks { get; set; }
	public DateTime? LastSpawnedAt { get; set; }

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
	public virtual EmploymentActionPlanRecord EmploymentActionPlan { get; set; } = null!;
	public virtual ICollection<EmploymentTaskConditionRecord> Conditions { get; set; }
}

public partial class EmploymentTaskConditionRecord
{
	public long Id { get; set; }
	public long? ScheduledTaskRuleId { get; set; }
	public long? ManagerGoalId { get; set; }
	public long? ConditionPredicateId { get; set; }
	public long? ScheduledRuleTemplateId { get; set; }
	public int SortOrder { get; set; }
	public int ConditionType { get; set; }
	public string? Key { get; set; }
	public int? ThresholdInt { get; set; }
	public decimal? ThresholdDecimal { get; set; }
	public bool? BoolValue { get; set; }
	public long? EarliestTicks { get; set; }
	public long? LatestTicks { get; set; }

	public virtual EmploymentScheduledTaskRuleRecord? ScheduledTaskRule { get; set; }
	public virtual EmploymentManagerGoalRecord? ManagerGoal { get; set; }
	public virtual EmploymentConditionPredicateRecord? ConditionPredicate { get; set; }
	public virtual EmploymentScheduledRuleTemplateRecord? ScheduledRuleTemplate { get; set; }
}

public partial class EmploymentConditionPredicateRecord
{
	public EmploymentConditionPredicateRecord()
	{
		Conditions = new HashSet<EmploymentTaskConditionRecord>();
	}

	public long Id { get; set; }
	public string PublicId { get; set; } = string.Empty;
	public long EmploymentHostStateId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? ExpressionJson { get; set; }

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
	public virtual ICollection<EmploymentTaskConditionRecord> Conditions { get; set; }
}

public partial class EmploymentScheduledRuleTemplateRecord
{
	public EmploymentScheduledRuleTemplateRecord()
	{
		Conditions = new HashSet<EmploymentTaskConditionRecord>();
	}

	public long Id { get; set; }
	public string PublicId { get; set; } = string.Empty;
	public long EmploymentHostStateId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string IdempotencyKeyPattern { get; set; } = string.Empty;
	public long EmploymentActionPlanId { get; set; }
	public string? ExpressionJson { get; set; }
	public long CooldownTicks { get; set; }

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
	public virtual EmploymentActionPlanRecord EmploymentActionPlan { get; set; } = null!;
	public virtual ICollection<EmploymentTaskConditionRecord> Conditions { get; set; }
}

public partial class EmploymentActiveTaskRecord
{
	public EmploymentActiveTaskRecord()
	{
		StepStates = new HashSet<EmploymentActiveTaskStepStateRecord>();
	}

	public long Id { get; set; }
	public string PublicId { get; set; } = string.Empty;
	public long EmploymentHostStateId { get; set; }
	public string Name { get; set; } = string.Empty;
	public long EmploymentActionPlanId { get; set; }
	public int Status { get; set; }
	public long? AssignedEmployeeId { get; set; }
	public string? BlockedReason { get; set; }
	public string CorrelationId { get; set; } = string.Empty;
	public string IdempotencyKey { get; set; } = string.Empty;

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
	public virtual EmploymentActionPlanRecord EmploymentActionPlan { get; set; } = null!;
	public virtual ICollection<EmploymentActiveTaskStepStateRecord> StepStates { get; set; }
}

public partial class EmploymentActiveTaskStepStateRecord
{
	public long Id { get; set; }
	public long EmploymentActiveTaskId { get; set; }
	public int SortOrder { get; set; }
	public int Status { get; set; }
	public string? OperationalPayload { get; set; }
	public string? TransactionReference { get; set; }
	public string? SelectedResources { get; set; }
	public string? ReservationReference { get; set; }
	public string? RouteResult { get; set; }
	public string? CraftJobReference { get; set; }
	public string? LoadedAssets { get; set; }
	public string? FailureDiagnostic { get; set; }

	public virtual EmploymentActiveTaskRecord EmploymentActiveTask { get; set; } = null!;
}

public partial class EmploymentManagerGoalRecord
{
	public EmploymentManagerGoalRecord()
	{
		Conditions = new HashSet<EmploymentTaskConditionRecord>();
	}

	public long Id { get; set; }
	public long RuntimeId { get; set; }
	public long EmploymentHostStateId { get; set; }
	public int GoalType { get; set; }
	public long RequiredAuthority { get; set; }
	public int Status { get; set; }
	public string ConfigurationDescription { get; set; } = string.Empty;
	public long? EmploymentActionPlanId { get; set; }
	public int Priority { get; set; }
	public long EvaluationCadenceTicks { get; set; }
	public DateTime? LastEvaluatedAt { get; set; }
	public string? LastEvaluationResult { get; set; }
	public string CorrelationId { get; set; } = string.Empty;

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
	public virtual EmploymentActionPlanRecord? EmploymentActionPlan { get; set; }
	public virtual ICollection<EmploymentTaskConditionRecord> Conditions { get; set; }
}

public partial class EmploymentRegisterEntryRecord
{
	public long Id { get; set; }
	public long EmploymentHostStateId { get; set; }
	public string CorrelationId { get; set; } = string.Empty;
	public int EntryType { get; set; }
	public long? ActorId { get; set; }
	public string Description { get; set; } = string.Empty;
	public DateTime RecordedAt { get; set; }

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
}

public partial class EmploymentLedgerEntryRecord
{
	public long Id { get; set; }
	public long EmploymentHostStateId { get; set; }
	public string CorrelationId { get; set; } = string.Empty;
	public int EntryType { get; set; }
	public long? ActorId { get; set; }
	public long? AmountCurrencyId { get; set; }
	public decimal? Amount { get; set; }
	public string Description { get; set; } = string.Empty;
	public DateTime RecordedAt { get; set; }

	public virtual EmploymentHostState EmploymentHostState { get; set; } = null!;
}
