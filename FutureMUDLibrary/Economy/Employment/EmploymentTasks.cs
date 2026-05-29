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
	AccountBalance
}

public interface IEmploymentTaskCondition
{
	EmploymentTaskConditionType ConditionType { get; }
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
	DeliverItems
}

public enum EmploymentActionStepStatus
{
	Pending,
	InProgress,
	Completed,
	Failed,
	Blocked
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

public sealed record EmploymentActionStepResult(bool Success, string Message, bool Completed = true)
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
	IReadOnlyCollection<IEmploymentActiveTask> EvaluateScheduledRules(IEmploymentTaskContext context, DateTimeOffset now);
}

public interface IEmploymentTaskContext
{
	IEmploymentHost Employer { get; }
	bool ManualOrderActive(string key);
	int StockLevel(string stockKey);
	decimal AccountBalance(string accountKey);
	bool PaymentAuthorised(IEmploymentActionStep step);
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
	void RecordRegister(EmploymentRegisterEntryType entryType, ICharacter? actor, string description,
		Guid? correlationId = null);
	void RecordLedger(EmploymentLedgerEntryType entryType, ICharacter? actor, MoneyAmount? amount, string description,
		Guid? correlationId = null);
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
