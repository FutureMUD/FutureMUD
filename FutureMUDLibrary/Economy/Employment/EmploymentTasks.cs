using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Vehicles;

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
	TaxOwing,
	MarketPrice,
	PayrollLiability,
	StaffingLevel,
	HospitalSupplyStock
}

public enum EmploymentScheduledRuleStatus
{
	Active,
	Paused
}

public enum EmploymentConditionExpressionKind
{
	All,
	Any,
	Not,
	Condition,
	Predicate
}

public interface IEmploymentTaskCondition
{
	EmploymentTaskConditionType ConditionType { get; }
	EmploymentAuthoritySet RequiredAuthority { get; }
	bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason);
}

public sealed record EmploymentConditionExpression(
	EmploymentConditionExpressionKind Kind,
	int? ConditionNumber = null,
	string? PredicateName = null,
	IReadOnlyList<EmploymentConditionExpression>? Children = null)
{
	public static EmploymentConditionExpression All(IEnumerable<EmploymentConditionExpression> children) =>
		new(EmploymentConditionExpressionKind.All, Children: children.ToList());

	public static EmploymentConditionExpression Any(IEnumerable<EmploymentConditionExpression> children) =>
		new(EmploymentConditionExpressionKind.Any, Children: children.ToList());

	public static EmploymentConditionExpression Not(EmploymentConditionExpression child) =>
		new(EmploymentConditionExpressionKind.Not, Children: [child]);

	public static EmploymentConditionExpression Condition(int conditionNumber) =>
		new(EmploymentConditionExpressionKind.Condition, ConditionNumber: conditionNumber);

	public static EmploymentConditionExpression Predicate(string predicateName) =>
		new(EmploymentConditionExpressionKind.Predicate, PredicateName: predicateName);

	public IReadOnlyList<EmploymentConditionExpression> ChildExpressions => Children ?? [];
}

public sealed record EmploymentConditionLeafEvaluation(
	string Label,
	bool Satisfied,
	string Reason);

public sealed record EmploymentConditionExpressionEvaluation(
	bool Satisfied,
	string Reason,
	IReadOnlyList<EmploymentConditionLeafEvaluation> Leaves);

public enum EmploymentActionStepType
{
	SupplierSelection,
	Purchase,
	MoveOrDeliver,
	CraftTrigger,
	Command,
	BankDeposit,
	BankWithdrawal,
	AccountTransfer,
	HostSettlement,
	StoreAccountPayment,
	BoardPost,
	GetItemsById,
	GetItemsByTag,
	GetCommodity,
	DeliverItems,
	ShopStockTransfer,
	AuctionLotListing,
	AuctionSettlement,
	AuctionClaim,
	ArenaEventAdministration,
	CataloguedShell,
	LoadItems,
	UnloadItems,
	ReturnAsset,
	VehicleOperation,
	StableAnimalOperation,
	TaxPayment,
	ShopFloatAdjustment,
	ShopCashReconciliation,
	PhysicalFloat,
	CraftStation,
	PayrollSettlement,
	PriceChange,
	ShopStocktake,
	ShopDealAdministration,
	JobOpeningAdministration,
	ScheduledRuleAdministration,
	ActiveTaskAdministration,
	ManagerGoalAdministration,
	BankAdministration,
	StableAdministration,
	HotelAdministration,
	HospitalService,
	HospitalSupplyPreparation,
	HospitalAdministration
}

public enum EmploymentActionStepStatus
{
	Pending,
	InProgress,
	Completed,
	Failed,
	Blocked,
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

public enum EmploymentTaskSourceKind
{
	Manual,
	ScheduledRule,
	ManagerGoal,
	HostSystem
}

public enum EmploymentPrincipalKind
{
	Character,
	EmploymentContract,
	ScheduledRule,
	ManagerGoal,
	HostSystem,
	Implementor
}

public sealed record EmploymentPrincipal(
	EmploymentPrincipalKind Kind,
	long? CharacterId = null,
	long? EmploymentContractId = null,
	Guid? ScheduledRuleId = null,
	long? ManagerGoalId = null,
	string? Label = null)
{
	public static EmploymentPrincipal ForCharacter(ICharacter character) =>
		new(EmploymentPrincipalKind.Character, CharacterId: character.Id, Label: character.Name);

	public static EmploymentPrincipal ForScheduledRule(IEmploymentScheduledTaskRule rule) =>
		new(EmploymentPrincipalKind.ScheduledRule, ScheduledRuleId: rule.Id, Label: rule.Name);

	public static EmploymentPrincipal ForManagerGoal(IManagerGoal goal) =>
		new(EmploymentPrincipalKind.ManagerGoal, ManagerGoalId: goal.Id, Label: goal.GoalType.DescribeEnum());

	public static EmploymentPrincipal HostSystem(string label) =>
		new(EmploymentPrincipalKind.HostSystem, Label: label);
}

public sealed record EmploymentAuthorisationLimit(long CurrencyId, decimal Amount);

public sealed record EmploymentAuthorisationGrant(
	Guid Id,
	EmploymentPrincipal IssuerPrincipal,
	EmploymentAuthoritySet Authority,
	IReadOnlyDictionary<long, decimal> AmountLimits,
	bool AllowsUnboundedFinancialSteps,
	DateTimeOffset IssuedAt,
	DateTimeOffset? ExpiresAt,
	Guid CorrelationId,
	IReadOnlySet<string> PaymentSourceScopes,
	IReadOnlySet<string> CounterpartyScopes)
{
	public IReadOnlyCollection<EmploymentAuthorisationLimit> Limits =>
		AmountLimits.Select(x => new EmploymentAuthorisationLimit(x.Key, x.Value)).ToList();

	public bool CoversAuthority(EmploymentAuthoritySet requiredAuthority)
	{
		return Authority.ContainsAll(requiredAuthority);
	}

	public bool Covers(MoneyAmount amount, IReadOnlyDictionary<long, decimal> alreadyUsed)
	{
		if (!AmountLimits.TryGetValue(amount.Currency.Id, out var limit))
		{
			return AllowsUnboundedFinancialSteps;
		}

		return limit - alreadyUsed.GetValueOrDefault(amount.Currency.Id) >= amount.Amount;
	}

	public bool CoversPaymentSource(string? paymentSourceScope)
	{
		return CoversScope(PaymentSourceScopes, paymentSourceScope);
	}

	public bool CoversCounterparty(string? counterpartyScope)
	{
		return CoversScope(CounterpartyScopes, counterpartyScope);
	}

	private static bool CoversScope(IReadOnlySet<string> allowedScopes, string? scope)
	{
		if (allowedScopes.Count == 0)
		{
			return true;
		}

		if (string.IsNullOrWhiteSpace(scope))
		{
			return false;
		}

		return allowedScopes.Contains(NormaliseScope(scope));
	}

	public static string NormaliseScope(string scope)
	{
		return scope.Trim().ToLowerInvariant();
	}
}

public sealed record EmploymentTaskProvenance(
	EmploymentTaskSourceKind SourceKind,
	Guid? SourceRuleId,
	long? SourceGoalId,
	EmploymentPrincipal CreatedByPrincipal,
	EmploymentPrincipal AuthorisedByPrincipal,
	EmploymentAuthorisationGrant AuthorisationGrant,
	int Priority,
	DateTimeOffset CreatedAt,
	DateTimeOffset? DueAt = null,
	DateTimeOffset? AssignedAt = null,
	DateTimeOffset? StartedAt = null,
	DateTimeOffset? CompletedAt = null);

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
	string IdempotencyKey { get; }
	EmploymentTaskProvenance Provenance { get; }
	EmploymentTaskSourceKind SourceKind { get; }
	Guid? SourceRuleId { get; }
	long? SourceGoalId { get; }
	EmploymentPrincipal CreatedByPrincipal { get; }
	EmploymentPrincipal AuthorisedByPrincipal { get; }
	EmploymentAuthorisationGrant AuthorisationGrant { get; }
	int Priority { get; }
	DateTimeOffset CreatedAt { get; }
	DateTimeOffset? DueAt { get; }
	DateTimeOffset? AssignedAt { get; }
}

public interface IEmploymentScheduledTaskRule
{
	Guid Id { get; }
	IEmploymentHost Employer { get; }
	string Name { get; }
	string IdempotencyKey { get; }
	IReadOnlyCollection<IEmploymentTaskCondition> Conditions { get; }
	EmploymentConditionExpression? ConditionExpression { get; }
	EmploymentActionPlan ActionPlan { get; }
	EmploymentScheduledRuleStatus Status { get; }
	TimeSpan Cooldown { get; }
	DateTimeOffset? LastSpawnedAt { get; }
	bool CanSpawn(IEmploymentTaskContext context, DateTimeOffset now, out string reason);
}

public interface IEmploymentConditionPredicate
{
	Guid Id { get; }
	IEmploymentHost Employer { get; }
	string Name { get; }
	IReadOnlyCollection<IEmploymentTaskCondition> Conditions { get; }
	EmploymentConditionExpression? ConditionExpression { get; }
	EmploymentAuthoritySet RequiredAuthority { get; }
}

public interface IEmploymentScheduledRuleTemplate
{
	Guid Id { get; }
	IEmploymentHost Employer { get; }
	string Name { get; }
	string IdempotencyKeyPattern { get; }
	IReadOnlyCollection<IEmploymentTaskCondition> Conditions { get; }
	EmploymentConditionExpression? ConditionExpression { get; }
	EmploymentActionPlan ActionPlan { get; }
	TimeSpan Cooldown { get; }
	EmploymentAuthoritySet RequiredAuthority { get; }
}

public interface IEmploymentTaskBoard
{
	IReadOnlyCollection<IEmploymentScheduledTaskRule> ScheduledRules { get; }
	IReadOnlyCollection<IEmploymentConditionPredicate> ConditionPredicates { get; }
	IReadOnlyCollection<IEmploymentScheduledRuleTemplate> ScheduledRuleTemplates { get; }
	IReadOnlyCollection<IEmploymentActiveTask> ActiveTasks { get; }
	IEmploymentScheduledTaskRule CreateScheduledRule(string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentActionPlan actionPlan, TimeSpan cooldown,
		ICharacter? authorisedBy);
	IEmploymentScheduledTaskRule CreateScheduledRule(string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		EmploymentActionPlan actionPlan, TimeSpan cooldown, ICharacter? authorisedBy);
	IEmploymentConditionPredicate CreateConditionPredicate(string name,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		ICharacter? authorisedBy);
	bool CancelConditionPredicate(IEmploymentConditionPredicate predicate, ICharacter? cancelledBy, string reason);
	IEmploymentScheduledRuleTemplate CreateScheduledRuleTemplate(string name, string idempotencyKeyPattern,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		EmploymentActionPlan actionPlan, TimeSpan cooldown, ICharacter? authorisedBy);
	bool CancelScheduledRuleTemplate(IEmploymentScheduledRuleTemplate template, ICharacter? cancelledBy, string reason);
	IEmploymentActiveTask CreateActiveTask(string name, EmploymentActionPlan actionPlan, ICharacter? authorisedBy,
		Guid? correlationId = null, string? idempotencyKey = null, int priority = 0, DateTimeOffset? dueAt = null,
		EmploymentTaskProvenance? provenance = null);
	bool CancelActiveTask(IEmploymentActiveTask task, ICharacter? cancelledBy, string reason);
	bool RetryActiveTask(IEmploymentActiveTask task, ICharacter? retriedBy, string reason);
	bool RequeueActiveTask(IEmploymentActiveTask task, ICharacter? requeuedBy, string reason);
	bool AssignActiveTask(IEmploymentActiveTask task, ICharacter employee, IEmploymentTaskContext context, ICharacter? assignedBy, string reason);
	bool CancelScheduledRule(IEmploymentScheduledTaskRule rule, ICharacter? cancelledBy, string reason);
	bool PauseScheduledRule(IEmploymentScheduledTaskRule rule, ICharacter? pausedBy, string reason);
	bool ResumeScheduledRule(IEmploymentScheduledTaskRule rule, ICharacter? resumedBy, string reason);
	bool HasBlockingActiveTask(string idempotencyKey);
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
	bool CanAssignVehicle(ICharacter actor, IVehicle vehicle, out string reason);
	bool TryAssignVehicle(ICharacter actor, IVehicle vehicle, out string reason,
		out EmploymentActionStepOperationalState operationalState);
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
	bool CanBankTransfer(string targetAccountKey, MoneyAmount amount, out string reason);
	bool TryBankTransfer(ICharacter actor, string targetAccountKey, MoneyAmount amount, out string reason,
		out EmploymentActionStepOperationalState operationalState);
	bool CanHostSettlement(string targetHostKey, MoneyAmount amount, out string reason);
	bool TryHostSettlement(ICharacter actor, string targetHostKey, MoneyAmount amount, out string reason,
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

public enum PriceChangeActionKind
{
	Merchandise,
	MarketCategory
}

public enum ShopStocktakeScope
{
	All,
	Merchandise
}

public enum ShopDealAdministrationActionKind
{
	Create,
	Modify,
	Cancel
}

public enum ArenaEventAdministrationActionKind
{
	Create,
	Transition,
	Abort
}

public enum BankAdministrationActionKind
{
	ReserveAudit,
	ReserveDeposit,
	ReserveWithdrawal,
	AccountCredit,
	AccountStatus,
	AccountClose,
	BranchPost,
	BranchCourier
}

public enum StableAdministrationActionKind
{
	CareInspect,
	CareFeed,
	CareGroom,
	CareExercise,
	FeeAssessment,
	StayReconciliation,
	AccountReconciliation
}

public enum HotelAdministrationActionKind
{
	RoomInspect,
	RoomClean,
	RoomReady,
	RoomMaintenance,
	LostPropertyCheck,
	LostPropertyAudit,
	PatronBalanceAudit
}

public enum HospitalAdministrationActionKind
{
	ServiceAudit,
	DebtAudit,
	TheatrePreparation,
	SupplyAudit,
	RequestAudit
}

public enum JobOpeningAdministrationActionKind
{
	Create,
	Close,
	Modify
}

public enum ScheduledRuleAdministrationActionKind
{
	Pause,
	Resume,
	Cancel,
	Evaluate
}

public enum ActiveTaskAdministrationActionKind
{
	Retry,
	Requeue,
	Assign,
	Cancel
}

public enum ManagerGoalAdministrationActionKind
{
	Evaluate,
	Cancel,
	Reactivate
}

public enum ManagerGoalType
{
	MaintainMinimumStock,
	KeepShopAccountsPaid,
	PayBusinessTaxes,
	AdjustPricesForProfit,
	MaintainCashAndBankBalances,
	MaintainStaffingLevels,
	MaintainHotelOperations,
	MaintainPhysicalCashFloat,
	PayTaxes,
	PayShopAccountsOwing,
	MaintainCraftedMerchandiseStock,
	MaintainCraftMaterialSupply,
	MaintainMinimumPhysicalCashFloat,
	MaintainMaximumPhysicalCashFloat,
	KeepEmploymentPayrollCurrent,
	MaintainMinimumBusinessFunds,
	MaintainMaximumBusinessFunds,
	MaintainHospitalConsumableStock,
	MaintainHospitalReusableEquipmentStock
}

public enum ManagerGoalStatus
{
	Active,
	Suspended,
	Completed,
	Cancelled,
	Blocked,
	Satisfied,
	Failed
}

public sealed record ManagerGoalRiskLimits(
	int? MaximumActiveTasks = 1,
	int? MaximumActionSteps = null,
	bool AllowsUnboundedFinancialSteps = false)
{
	public static ManagerGoalRiskLimits Default { get; } = new();
}

public sealed record ManagerGoalPolicy
{
	public ManagerGoalPolicy(IReadOnlyCollection<MoneyAmount>? budgetLimits = null,
		ManagerGoalRiskLimits? riskLimits = null)
	{
		BudgetLimits = budgetLimits ?? Array.Empty<MoneyAmount>();
		RiskLimits = riskLimits ?? ManagerGoalRiskLimits.Default;
	}

	public static ManagerGoalPolicy Default { get; } = new();
	public IReadOnlyCollection<MoneyAmount> BudgetLimits { get; init; }
	public ManagerGoalRiskLimits RiskLimits { get; init; }
}

public sealed record ManagerGoalConfiguration(
	string Description,
	EmploymentActionPlan? ActionPlan = null,
	IReadOnlyCollection<IEmploymentTaskCondition>? Conditions = null,
	EmploymentConditionExpression? ConditionExpression = null);

public sealed record ManagerGoalDefinition(
	ManagerGoalType GoalType,
	EmploymentAuthoritySet RequiredAuthority,
	ManagerGoalConfiguration Configuration,
	int Priority,
	TimeSpan EvaluationCadence,
	ManagerGoalPolicy? Policy = null);

public interface IManagerGoal
{
	long Id { get; }
	IEmploymentHost Employer { get; }
	ManagerGoalType GoalType { get; }
	EmploymentAuthoritySet RequiredAuthority { get; }
	ManagerGoalStatus Status { get; }
	ManagerGoalConfiguration Configuration { get; }
	int Priority { get; }
	TimeSpan EvaluationCadence { get; }
	ManagerGoalPolicy Policy { get; }
	DateTimeOffset? LastEvaluatedAt { get; }
	string? LastEvaluationResult { get; }
	Guid CorrelationId { get; }
}

public interface IManagerGoalBoard
{
	IReadOnlyCollection<IManagerGoal> Goals { get; }
	IManagerGoal CreateGoal(ManagerGoalDefinition definition, ICharacter authorisedBy);
	void CancelGoal(IManagerGoal goal, ICharacter cancelledBy, string reason);
	bool ReactivateGoal(IManagerGoal goal, ICharacter reactivatedBy, string reason);
	IReadOnlyCollection<IEmploymentActiveTask> EvaluateGoal(IManagerGoal goal, IEmploymentTaskContext context, DateTimeOffset now);
	IReadOnlyCollection<IEmploymentActiveTask> EvaluateGoals(IEmploymentTaskContext context, DateTimeOffset now);
}
