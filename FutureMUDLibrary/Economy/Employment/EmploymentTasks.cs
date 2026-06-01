using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Economy.Employment;

public sealed record JobOpeningDefinition(
	EmploymentRole Role,
	JobRequirementSet Requirements,
	CompensationTerms Compensation,
	WorkSchedule Schedule,
	EmploymentDuration Duration,
	int MaxPositions,
	bool NpcApplicationsOnly,
	PaymentMethod PaymentMethod,
	EmploymentAuthoritySet Authority);

public enum EmploymentTaskConditionType
{
	ManualOrder,
	TimeWindow,
	StockThreshold,
	AccountBalance,
	ItemThreshold,
	CommodityThreshold,
	ShopAccountOwing,
	ShopFloatThreshold,
	WeatherLevel,
	TaxOwing
}

public enum EmploymentScheduledRuleStatus
{
	Active,
	Paused
}

public interface IEmploymentTaskCondition
{
	EmploymentTaskConditionType ConditionType { get; }
	EmploymentAuthoritySet RequiredAuthority { get; }
	bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason);
}

public enum EmploymentActionStepType
{
	Purchase,
	MoveOrDeliver,
	CraftTrigger,
	Command,
	BankDeposit,
	BankWithdrawal,
	StoreAccountPayment,
	BoardPost,
	GetItemsById,
	GetItemsByTag,
	GetCommodity,
	DeliverItems,
	CataloguedShell,
	LoadItems,
	UnloadItems,
	ReturnAsset,
	VehicleOperation,
	TaxPayment,
	ShopFloatAdjustment,
	PhysicalFloat,
	CraftStation
}

public enum EmploymentActionStepStatus
{
	Pending,
	InProgress,
	Completed,
	Failed,
	Blocked
}

public enum EmploymentItemSelectorKind
{
	PrototypeId,
	ItemId,
	Tag,
	Keyword
}

public sealed record EmploymentItemSelector(
	EmploymentItemSelectorKind Kind,
	long? Id = null,
	string? Text = null,
	IGameItem? Item = null)
{
	public static EmploymentItemSelector ForPrototype(long prototypeId) =>
		new(EmploymentItemSelectorKind.PrototypeId, prototypeId);

	public static EmploymentItemSelector ForItemId(long itemId) =>
		new(EmploymentItemSelectorKind.ItemId, itemId);

	public static EmploymentItemSelector ForItem(IGameItem item, string? keyword = null) =>
		new(string.IsNullOrWhiteSpace(keyword) ? EmploymentItemSelectorKind.ItemId : EmploymentItemSelectorKind.Keyword,
			item.Id, keyword, item);

	public static EmploymentItemSelector ForTag(string tag) =>
		new(EmploymentItemSelectorKind.Tag, null, tag);

	public static EmploymentItemSelector ForKeyword(string keyword, IGameItem? item = null) =>
		new(EmploymentItemSelectorKind.Keyword, item?.Id, keyword, item);
}

public interface IEmploymentActionStep
{
	EmploymentActionStepType StepType { get; }
	EmploymentAuthoritySet RequiredAuthority { get; }
	IReadOnlySet<EmploymentAICapability> RequiredCapabilities { get; }
	bool RequiresPaymentAuthorisation { get; }
	bool IsFinancialStep { get; }
	bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason);
	EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor);
}

public interface IEmploymentActionStepLocationHint
{
	IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor);
}

public sealed record EmploymentActionStepOperationalState(
	string? OperationalPayload = null,
	string? TransactionReference = null,
	string? SelectedResources = null,
	string? ReservationReference = null,
	string? RouteResult = null,
	string? CraftJobReference = null,
	string? LoadedAssets = null,
	string? FailureDiagnostic = null)
{
	public static EmploymentActionStepOperationalState Empty { get; } = new();

	public bool IsEmpty =>
		string.IsNullOrWhiteSpace(OperationalPayload) &&
		string.IsNullOrWhiteSpace(TransactionReference) &&
		string.IsNullOrWhiteSpace(SelectedResources) &&
		string.IsNullOrWhiteSpace(ReservationReference) &&
		string.IsNullOrWhiteSpace(RouteResult) &&
		string.IsNullOrWhiteSpace(CraftJobReference) &&
		string.IsNullOrWhiteSpace(LoadedAssets) &&
		string.IsNullOrWhiteSpace(FailureDiagnostic);

	public EmploymentActionStepOperationalState Merge(EmploymentActionStepOperationalState? other)
	{
		if (other is null || other.IsEmpty)
		{
			return this;
		}

		return this with
		{
			OperationalPayload = Choose(other.OperationalPayload, OperationalPayload),
			TransactionReference = Choose(other.TransactionReference, TransactionReference),
			SelectedResources = Choose(other.SelectedResources, SelectedResources),
			ReservationReference = Choose(other.ReservationReference, ReservationReference),
			RouteResult = Choose(other.RouteResult, RouteResult),
			CraftJobReference = Choose(other.CraftJobReference, CraftJobReference),
			LoadedAssets = Choose(other.LoadedAssets, LoadedAssets),
			FailureDiagnostic = Choose(other.FailureDiagnostic, FailureDiagnostic)
		};
	}

	public EmploymentActionStepOperationalState WithFailure(string? failure) =>
		string.IsNullOrWhiteSpace(failure) ? this : this with { FailureDiagnostic = failure };

	public EmploymentActionStepOperationalState WithoutFailure() => this with { FailureDiagnostic = null };

	private static string? Choose(string? replacement, string? existing) =>
		string.IsNullOrWhiteSpace(replacement) ? existing : replacement;
}

public sealed record EmploymentActionStepResult(
	bool Success,
	string Message,
	bool Completed = true,
	EmploymentActionStepOperationalState? OperationalState = null)
{
	public static EmploymentActionStepResult CompletedResult(string message)
	{
		return new EmploymentActionStepResult(true, message);
	}

	public static EmploymentActionStepResult Blocked(string message)
	{
		return new EmploymentActionStepResult(false, message, false);
	}

	public static EmploymentActionStepResult Failed(string message)
	{
		return new EmploymentActionStepResult(false, message);
	}
}

public sealed class EmploymentActionPlan
{
	private readonly List<IEmploymentActionStep> _steps;

	public EmploymentActionPlan(IEnumerable<IEmploymentActionStep> steps)
	{
		_steps = new List<IEmploymentActionStep>(steps);
	}

	public IReadOnlyList<IEmploymentActionStep> Steps => _steps;

	public EmploymentAuthoritySet RequiredAuthority
	{
		get
		{
			var authority = EmploymentAuthority.None;
			foreach (var step in _steps)
			{
				authority |= step.RequiredAuthority.Authorities;
			}

			return authority;
		}
	}

	public IReadOnlySet<EmploymentAICapability> RequiredCapabilities
	{
		get
		{
			var capabilities = new HashSet<EmploymentAICapability>();
			foreach (var step in _steps)
			{
				capabilities.UnionWith(step.RequiredCapabilities);
			}

			return capabilities;
		}
	}
}

public enum EmploymentTaskStatus
{
	Pending,
	Assigned,
	InProgress,
	Blocked,
	Completed,
	Failed,
	Cancelled
}

public enum EmploymentTaskAssignmentAuditOutcome
{
	Requeued,
	Blocked
}

public sealed record EmploymentTaskAssignmentAuditResult(
	Guid TaskId,
	string TaskName,
	EmploymentTaskAssignmentAuditOutcome Outcome,
	string Reason);

public interface IEmploymentActiveTask
{
	Guid Id { get; }
	IEmploymentHost Employer { get; }
	string Name { get; }
	EmploymentActionPlan ActionPlan { get; }
	EmploymentTaskStatus Status { get; }
	ICharacter? AssignedEmployee { get; }
	string? BlockedReason { get; }
	IReadOnlyList<EmploymentActionStepStatus> StepStates { get; }
	IReadOnlyList<EmploymentActionStepOperationalState> StepOperationalStates { get; }
	Guid CorrelationId { get; }
}

public interface IEmploymentScheduledTaskRule
{
	Guid Id { get; }
	IEmploymentHost Employer { get; }
	string Name { get; }
	string IdempotencyKey { get; }
	IReadOnlyCollection<IEmploymentTaskCondition> Conditions { get; }
	EmploymentActionPlan ActionPlan { get; }
	EmploymentScheduledRuleStatus Status { get; }
	TimeSpan Cooldown { get; }
	DateTimeOffset? LastSpawnedAt { get; }
	bool CanSpawn(IEmploymentTaskContext context, DateTimeOffset now, out string reason);
}

public interface IEmploymentTaskBoard
{
	IReadOnlyCollection<IEmploymentScheduledTaskRule> ScheduledRules { get; }
	IReadOnlyCollection<IEmploymentActiveTask> ActiveTasks { get; }
	IEmploymentScheduledTaskRule CreateScheduledRule(string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentActionPlan actionPlan, TimeSpan cooldown,
		ICharacter? authorisedBy);
	IEmploymentActiveTask CreateActiveTask(string name, EmploymentActionPlan actionPlan, ICharacter? authorisedBy,
		Guid? correlationId = null);
	bool CancelActiveTask(IEmploymentActiveTask task, ICharacter? cancelledBy, string reason);
	bool CancelScheduledRule(IEmploymentScheduledTaskRule rule, ICharacter? cancelledBy, string reason);
	bool PauseScheduledRule(IEmploymentScheduledTaskRule rule, ICharacter? pausedBy, string reason);
	bool ResumeScheduledRule(IEmploymentScheduledTaskRule rule, ICharacter? resumedBy, string reason);
	IReadOnlyCollection<EmploymentTaskAssignmentAuditResult> AuditActiveTaskAssignments();
	IReadOnlyCollection<IEmploymentActiveTask> EvaluateScheduledRule(IEmploymentScheduledTaskRule rule,
		IEmploymentTaskContext context, DateTimeOffset now);
	IReadOnlyCollection<IEmploymentActiveTask> EvaluateScheduledRules(IEmploymentTaskContext context, DateTimeOffset now);
}

public interface IEmploymentTaskContext
{
	IEmploymentHost Employer { get; }
	bool ManualOrderActive(string key);
	int StockLevel(string stockKey);
	decimal AccountBalance(string accountKey);
	bool PaymentAuthorised(IEmploymentActionStep step);
	void AuthorisePaymentFor(IEmploymentActionStep step, ICharacter? actor = null, Guid? correlationId = null,
		bool recordRegister = true);
	bool CommandAllowed(string commandName);
	bool CanPath(ICharacter actor, ICell? destination);
	IReadOnlyCollection<IGameItem> AvailableItems(ICell location);
	IReadOnlyCollection<IGameItem> CarriedTaskItems(ICharacter actor);
	bool ItemHasTag(IGameItem item, string tagName);
	double CommodityWeight(IGameItem item, string materialName, string? tagName,
		IReadOnlyDictionary<string, string> characteristics);
	bool TryCollectTaskItem(ICharacter actor, IGameItem item, ICell source, out string reason);
	bool TryCollectTaskItems(ICharacter actor, IReadOnlyCollection<(IGameItem Item, ICell Source)> items,
		out string reason);
	bool TryDeliverTaskItems(ICharacter actor, ICell destination, IGameItem? container, string? containerTag,
		out string reason);
	bool TryLoadCarriedTaskItems(ICharacter actor, IGameItem targetContainer, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool TryUnloadTaskItems(ICharacter actor, IGameItem sourceContainer, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool TryReturnContainer(ICharacter actor, IGameItem container, ICell destination, IGameItem? destinationContainer,
		string? destinationContainerTag, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	IReadOnlyCollection<IGameItem> LoadedTaskItems(ICharacter actor, IGameItem container);
	bool CanReserveFunds(MoneyAmount amount, out string reason);
	bool TryReserveFunds(MoneyAmount amount, ICharacter actor, string description, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool HasReservedFunds(MoneyAmount amount, out string reason);
	bool TryReleaseReservedFunds(ICharacter actor, string selector, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool CanBankDeposit(MoneyAmount amount, out string reason);
	bool CanBankWithdrawal(MoneyAmount amount, out string reason);
	bool TryBankDeposit(ICharacter actor, MoneyAmount amount, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool TryBankWithdrawal(ICharacter actor, MoneyAmount amount, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool CanPurchase(IEmploymentActionStep step, out string reason);
	bool TryPurchase(ICharacter actor, IEmploymentActionStep step, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool CanStoreAccountPayment(string accountKey, MoneyAmount amount, out string reason);
	bool TryStoreAccountPayment(ICharacter actor, string accountKey, MoneyAmount amount, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool CanPayTaxes(MoneyAmount? maximumAmount, out string reason, out MoneyAmount? amount);
	bool TryPayTaxes(ICharacter actor, MoneyAmount? maximumAmount, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool CanAdjustShopFloat(MoneyAmount amount, bool fillRegister, EmploymentItemSelector? registerSelector,
		out string reason);
	bool TryAdjustShopFloat(ICharacter actor, MoneyAmount amount, bool fillRegister,
		EmploymentItemSelector? registerSelector, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool CanHandlePhysicalFloat(PhysicalFloatOperation operation, MoneyAmount? amount, string targetKind,
		EmploymentItemSelector? targetSelector, out string reason);
	bool TryHandlePhysicalFloat(ICharacter actor, PhysicalFloatOperation operation, MoneyAmount? amount,
		string targetKind, EmploymentItemSelector? targetSelector, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool CanUseCraftStation(ICharacter actor, string stationSelector, out string reason);
	bool TryUseCraftStation(ICharacter actor, string stationSelector, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool CanStartCraft(string craftSelector, ICharacter actor, out string reason);
	bool TryStartCraft(ICharacter actor, string craftSelector, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	void HydrateTaskState(IEmploymentActiveTask task, int currentStepIndex);
	void RecordRegister(EmploymentRegisterEntryType entryType, ICharacter? actor, string description,
		Guid? correlationId = null);
	void RecordLedger(EmploymentLedgerEntryType entryType, ICharacter? actor, MoneyAmount? amount, string description,
		Guid? correlationId = null);
}

public enum EmploymentPurchaseTargetKind
{
	Merchandise,
	Item,
	Commodity
}

public enum PhysicalFloatOperation
{
	Issue,
	Return,
	Settle
}

public enum ManagerGoalType
{
	MaintainMinimumStock,
	KeepShopAccountsPaid,
	PayBusinessTaxes,
	AdjustPricesForProfit,
	MaintainCashAndBankBalances,
	MaintainStaffingLevels,
	MaintainHotelOperations
}

public enum ManagerGoalStatus
{
	Active,
	Suspended,
	Completed,
	Cancelled,
	Blocked
}

public sealed record ManagerGoalConfiguration(
	string Description,
	EmploymentActionPlan? ActionPlan = null,
	IReadOnlyCollection<IEmploymentTaskCondition>? Conditions = null);

public sealed record ManagerGoalDefinition(
	ManagerGoalType GoalType,
	EmploymentAuthoritySet RequiredAuthority,
	ManagerGoalConfiguration Configuration,
	int Priority,
	TimeSpan EvaluationCadence);

public interface IManagerGoal
{
	long Id { get; }
	IEmploymentHost Employer { get; }
	ManagerGoalType GoalType { get; }
	EmploymentAuthoritySet RequiredAuthority { get; }
	ManagerGoalStatus Status { get; }
	ManagerGoalConfiguration Configuration { get; }
	int Priority { get; }
	DateTimeOffset? LastEvaluatedAt { get; }
	string? LastEvaluationResult { get; }
	Guid CorrelationId { get; }
}

public interface IManagerGoalBoard
{
	IReadOnlyCollection<IManagerGoal> Goals { get; }
	IManagerGoal CreateGoal(ManagerGoalDefinition definition, ICharacter authorisedBy);
	void CancelGoal(IManagerGoal goal, ICharacter cancelledBy, string reason);
	IReadOnlyCollection<IEmploymentActiveTask> EvaluateGoals(IEmploymentTaskContext context, DateTimeOffset now);
}
