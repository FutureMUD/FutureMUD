using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Employment;

#nullable enable

namespace MudSharp.Economy.Employment;

public sealed class EmploymentTaskContext : IEmploymentTaskContext
{
	private readonly Dictionary<string, bool> _manualOrders = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly Dictionary<string, int> _stockLevels = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly Dictionary<string, decimal> _accountBalances = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly HashSet<IEmploymentActionStep> _paymentAuthorisations = new();
	private readonly HashSet<string> _allowedCommands = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly HashSet<long> _unreachableCellIds = new();

	public EmploymentTaskContext(IEmploymentHost employer)
	{
		Employer = employer;
	}

	public IEmploymentHost Employer { get; }

	public void SetManualOrder(string key, bool active)
	{
		_manualOrders[key] = active;
	}

	public bool ManualOrderActive(string key)
	{
		return _manualOrders.TryGetValue(key, out var active) && active;
	}

	public void SetStockLevel(string stockKey, int level)
	{
		_stockLevels[stockKey] = level;
	}

	public int StockLevel(string stockKey)
	{
		return _stockLevels.GetValueOrDefault(stockKey);
	}

	public void SetAccountBalance(string accountKey, decimal balance)
	{
		_accountBalances[accountKey] = balance;
	}

	public decimal AccountBalance(string accountKey)
	{
		return _accountBalances.GetValueOrDefault(accountKey);
	}

	public void AuthorisePaymentFor(IEmploymentActionStep step)
	{
		_paymentAuthorisations.Add(step);
		RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationGranted, null,
			$"Payment authorised for {step.StepType} action.");
	}

	public bool PaymentAuthorised(IEmploymentActionStep step)
	{
		return !step.RequiresPaymentAuthorisation || _paymentAuthorisations.Contains(step);
	}

	public void AllowCommand(string commandName)
	{
		_allowedCommands.Add(commandName);
	}

	public bool CommandAllowed(string commandName)
	{
		return _allowedCommands.Contains(commandName);
	}

	public void SetPathBlocked(ICell cell)
	{
		_unreachableCellIds.Add(cell.Id);
	}

	public bool CanPath(ICharacter actor, ICell? destination)
	{
		return destination is null || !_unreachableCellIds.Contains(destination.Id);
	}

	public void RecordRegister(EmploymentRegisterEntryType entryType, ICharacter? actor, string description,
		Guid? correlationId = null)
	{
		Employer.EmploymentRegister.Record(entryType, actor, description, correlationId);
	}

	public void RecordLedger(EmploymentLedgerEntryType entryType, ICharacter? actor, MoneyAmount? amount,
		string description, Guid? correlationId = null)
	{
		Employer.BusinessLedger.Record(entryType, actor, amount, description, correlationId);
	}
}

public sealed record ManualOrderCondition(string Key) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.ManualOrder;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var satisfied = context.ManualOrderActive(Key);
		reason = satisfied ? string.Empty : $"Manual order {Key} is not active.";
		return satisfied;
	}
}

public sealed record TimeWindowCondition(TimeSpan EarliestTime, TimeSpan LatestTime) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.TimeWindow;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var current = now.TimeOfDay;
		var satisfied = current >= EarliestTime && current <= LatestTime;
		reason = satisfied ? string.Empty : $"Current time {current} is outside the task window.";
		return satisfied;
	}
}

public sealed record StockThresholdCondition(string StockKey, int Threshold, bool BelowThreshold) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.StockThreshold;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var current = context.StockLevel(StockKey);
		var satisfied = BelowThreshold ? current < Threshold : current >= Threshold;
		reason = satisfied ? string.Empty : $"Stock {StockKey} is {current}, which does not satisfy threshold {Threshold}.";
		return satisfied;
	}
}

public sealed record AccountBalanceCondition(string AccountKey, decimal Threshold, bool BelowThreshold) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.AccountBalance;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var current = context.AccountBalance(AccountKey);
		var satisfied = BelowThreshold ? current < Threshold : current >= Threshold;
		reason = satisfied
			? string.Empty
			: $"Account {AccountKey} balance {current:N2} does not satisfy threshold {Threshold:N2}.";
		return satisfied;
	}
}

public sealed class EmploymentTaskBoard : IEmploymentTaskBoard
{
	private readonly IEmploymentHost _host;
	private readonly List<IEmploymentScheduledTaskRule> _scheduledRules = new();
	private readonly List<IEmploymentActiveTask> _activeTasks = new();
	private readonly IEmploymentPersistenceStore? _persistence;

	public EmploymentTaskBoard(IEmploymentHost host)
	{
		_host = host;
	}

	internal EmploymentTaskBoard(IEmploymentHost host, IEmploymentPersistenceStore persistence,
		IEnumerable<IEmploymentScheduledTaskRule> scheduledRules, IEnumerable<IEmploymentActiveTask> activeTasks)
	{
		_host = host;
		_persistence = persistence;
		_scheduledRules.AddRange(scheduledRules);
		_activeTasks.AddRange(activeTasks);
	}

	public IReadOnlyCollection<IEmploymentScheduledTaskRule> ScheduledRules => _scheduledRules;
	public IReadOnlyCollection<IEmploymentActiveTask> ActiveTasks => _activeTasks;

	public IEmploymentScheduledTaskRule CreateScheduledRule(string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentActionPlan actionPlan, TimeSpan cooldown,
		ICharacter? authorisedBy)
	{
		if (authorisedBy is not null && !_host.HasAuthority(authorisedBy, EmploymentAuthority.CreateScheduledRules))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create scheduled task rules for {_host.EmploymentHostName}.");
		}

		var rule = new EmploymentScheduledTaskRule(_host, name, idempotencyKey, conditions, actionPlan, cooldown);
		_scheduledRules.Add(rule);
		_persistence?.SaveScheduledRule(rule);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleCreated, authorisedBy,
			$"Created scheduled task rule {name}.");
		return rule;
	}

	public IEmploymentActiveTask CreateActiveTask(string name, EmploymentActionPlan actionPlan, ICharacter? authorisedBy,
		Guid? correlationId = null)
	{
		if (authorisedBy is not null && !_host.HasAuthority(authorisedBy, EmploymentAuthority.AssignTasks))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create tasks for {_host.EmploymentHostName}.");
		}

		var task = new EmploymentActiveTask(_host, name, actionPlan, correlationId ?? Guid.NewGuid(), _persistence);
		_activeTasks.Add(task);
		_persistence?.SaveActiveTask(task);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskCreated, authorisedBy,
			$"Created active task {name}.", task.CorrelationId);
		return task;
	}

	public IReadOnlyCollection<IEmploymentActiveTask> EvaluateScheduledRules(IEmploymentTaskContext context, DateTimeOffset now)
	{
		var spawned = new List<IEmploymentActiveTask>();
		foreach (var rule in _scheduledRules.OfType<EmploymentScheduledTaskRule>())
		{
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleEvaluated, null,
				$"Evaluated scheduled task rule {rule.Name}.");
			if (!rule.CanSpawn(context, now, out _))
			{
				continue;
			}

			if (_activeTasks.OfType<EmploymentActiveTask>().Any(x =>
				    x.IdempotencyKey.Equals(rule.IdempotencyKey, StringComparison.InvariantCultureIgnoreCase) &&
				    x.Status is EmploymentTaskStatus.Pending or EmploymentTaskStatus.Assigned or EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked))
			{
				continue;
			}

			var task = new EmploymentActiveTask(_host, rule.Name, rule.ActionPlan, Guid.NewGuid(), _persistence)
			{
				IdempotencyKey = rule.IdempotencyKey
			};
			_activeTasks.Add(task);
			_persistence?.SaveActiveTask(task);
			rule.MarkSpawned(now);
			_persistence?.SaveScheduledRuleState(rule);
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskCreated, null,
				$"Spawned active task {rule.Name} from scheduled rule.", task.CorrelationId);
			spawned.Add(task);
		}

		return spawned;
	}
}

public sealed class EmploymentScheduledTaskRule : IEmploymentScheduledTaskRule
{
	private readonly List<IEmploymentTaskCondition> _conditions;

	public EmploymentScheduledTaskRule(IEmploymentHost employer, string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentActionPlan actionPlan, TimeSpan cooldown)
	{
		Id = Guid.NewGuid();
		Employer = employer;
		Name = name;
		IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? Id.ToString("N") : idempotencyKey.Trim();
		_conditions = conditions.ToList();
		ActionPlan = actionPlan;
		Cooldown = cooldown;
	}

	internal EmploymentScheduledTaskRule(Guid id, IEmploymentHost employer, string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentActionPlan actionPlan, TimeSpan cooldown,
		DateTimeOffset? lastSpawnedAt)
	{
		Id = id;
		Employer = employer;
		Name = name;
		IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? Id.ToString("N") : idempotencyKey.Trim();
		_conditions = conditions.ToList();
		ActionPlan = actionPlan;
		Cooldown = cooldown;
		LastSpawnedAt = lastSpawnedAt;
	}

	public Guid Id { get; }
	public IEmploymentHost Employer { get; }
	public string Name { get; }
	public string IdempotencyKey { get; }
	public IReadOnlyCollection<IEmploymentTaskCondition> Conditions => _conditions;
	public EmploymentActionPlan ActionPlan { get; }
	public TimeSpan Cooldown { get; }
	public DateTimeOffset? LastSpawnedAt { get; private set; }

	public bool CanSpawn(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (LastSpawnedAt.HasValue && now - LastSpawnedAt.Value < Cooldown)
		{
			reason = "Rule is still inside its cooldown window.";
			return false;
		}

		foreach (var condition in _conditions)
		{
			if (!condition.IsSatisfied(context, now, out reason))
			{
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	public void MarkSpawned(DateTimeOffset now)
	{
		LastSpawnedAt = now;
	}
}

public sealed class EmploymentActiveTask : IEmploymentActiveTask
{
	private readonly List<EmploymentActionStepStatus> _stepStates;
	private readonly IEmploymentPersistenceStore? _persistence;

	public EmploymentActiveTask(IEmploymentHost employer, string name, EmploymentActionPlan actionPlan, Guid correlationId)
	{
		Id = Guid.NewGuid();
		Employer = employer;
		Name = name;
		ActionPlan = actionPlan;
		CorrelationId = correlationId;
		_stepStates = actionPlan.Steps.Select(_ => EmploymentActionStepStatus.Pending).ToList();
	}

	internal EmploymentActiveTask(IEmploymentHost employer, string name, EmploymentActionPlan actionPlan,
		Guid correlationId, IEmploymentPersistenceStore? persistence)
		: this(employer, name, actionPlan, correlationId)
	{
		_persistence = persistence;
	}

	internal EmploymentActiveTask(Guid id, IEmploymentHost employer, string name, EmploymentActionPlan actionPlan,
		EmploymentTaskStatus status, ICharacter? assignedEmployee, string? blockedReason,
		IEnumerable<EmploymentActionStepStatus> stepStates, Guid correlationId, string idempotencyKey,
		IEmploymentPersistenceStore persistence)
	{
		Id = id;
		Employer = employer;
		Name = name;
		ActionPlan = actionPlan;
		Status = status;
		AssignedEmployee = assignedEmployee;
		BlockedReason = blockedReason;
		CorrelationId = correlationId;
		IdempotencyKey = idempotencyKey;
		_persistence = persistence;
		_stepStates = stepStates.ToList();
		if (_stepStates.Count != actionPlan.Steps.Count)
		{
			_stepStates = actionPlan.Steps.Select(_ => EmploymentActionStepStatus.Pending).ToList();
		}
	}

	public Guid Id { get; }
	public IEmploymentHost Employer { get; }
	public string Name { get; }
	public EmploymentActionPlan ActionPlan { get; }
	public EmploymentTaskStatus Status { get; private set; } = EmploymentTaskStatus.Pending;
	public ICharacter? AssignedEmployee { get; private set; }
	public string? BlockedReason { get; private set; }
	public IReadOnlyList<EmploymentActionStepStatus> StepStates => _stepStates;
	public Guid CorrelationId { get; }
	public string IdempotencyKey { get; init; } = string.Empty;

	public int NextStepIndex => _stepStates.FindIndex(x => x is EmploymentActionStepStatus.Pending or EmploymentActionStepStatus.Blocked);

	public void Assign(ICharacter employee)
	{
		AssignedEmployee = employee;
		Status = EmploymentTaskStatus.Assigned;
		BlockedReason = null;
		_persistence?.SaveActiveTaskState(this);
	}

	public void Block(string reason)
	{
		Status = EmploymentTaskStatus.Blocked;
		BlockedReason = reason;
		_persistence?.SaveActiveTaskState(this);
	}

	public void MarkStep(int index, EmploymentActionStepStatus status)
	{
		_stepStates[index] = status;
		if (_stepStates.All(x => x == EmploymentActionStepStatus.Completed))
		{
			Status = EmploymentTaskStatus.Completed;
		}
		else if (status == EmploymentActionStepStatus.Failed)
		{
			Status = EmploymentTaskStatus.Failed;
		}
		else if (Status == EmploymentTaskStatus.Assigned)
		{
			Status = EmploymentTaskStatus.InProgress;
		}

		_persistence?.SaveActiveTaskState(this);
	}
}

public sealed class EmploymentTaskDispatcher
{
	public bool TryAssignTask(IEmploymentActiveTask task, IEnumerable<EmploymentCandidateProfile> candidates,
		IEmploymentTaskContext context, out string reason)
	{
		if (task is not EmploymentActiveTask concrete)
		{
			reason = "Unsupported active task implementation.";
			return false;
		}

		foreach (var candidate in candidates)
		{
			if (!task.Employer.EmploymentContracts.Any(x =>
				    x.Employee.Id == candidate.Candidate.Id &&
				    x.Status == EmploymentStatus.Active))
			{
				continue;
			}

			if (!task.ActionPlan.RequiredCapabilities.All(x => candidate.Capabilities.Contains(x)))
			{
				continue;
			}

			var canExecuteAllSteps = true;
			foreach (var step in task.ActionPlan.Steps)
			{
				if (step.CanExecute(context, candidate.Candidate, out _))
				{
					continue;
				}

				canExecuteAllSteps = false;
				break;
			}

			if (!canExecuteAllSteps)
			{
				continue;
			}

			concrete.Assign(candidate.Candidate);
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskAssigned, candidate.Candidate,
				$"Assigned task {task.Name}.", task.CorrelationId);
			reason = string.Empty;
			return true;
		}

		reason = "No active employee is eligible to complete the task.";
		concrete.Block(reason);
		context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskBlocked, null, reason, task.CorrelationId);
		return false;
	}

	public EmploymentActionStepResult AdvanceTask(IEmploymentActiveTask task, IEmploymentTaskContext context)
	{
		if (task is not EmploymentActiveTask concrete)
		{
			return EmploymentActionStepResult.Failed("Unsupported active task implementation.");
		}

		if (task.AssignedEmployee is null)
		{
			concrete.Block("Task has no assigned employee.");
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskBlocked, null, concrete.BlockedReason!, task.CorrelationId);
			return EmploymentActionStepResult.Blocked(concrete.BlockedReason!);
		}

		var index = concrete.NextStepIndex;
		if (index < 0)
		{
			return EmploymentActionStepResult.CompletedResult("Task is already complete.");
		}

		var step = task.ActionPlan.Steps[index];
		if (!step.CanExecute(context, task.AssignedEmployee, out var reason))
		{
			concrete.MarkStep(index, EmploymentActionStepStatus.Blocked);
			concrete.Block(reason);
			context.RecordRegister(EmploymentRegisterEntryType.ActionStepFailed, task.AssignedEmployee, reason, task.CorrelationId);
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskBlocked, task.AssignedEmployee, reason, task.CorrelationId);
			return EmploymentActionStepResult.Blocked(reason);
		}

		concrete.MarkStep(index, EmploymentActionStepStatus.InProgress);
		context.RecordRegister(EmploymentRegisterEntryType.ActionStepStarted, task.AssignedEmployee,
			$"Started {step.StepType} action.", task.CorrelationId);
		var result = step.Execute(context, task.AssignedEmployee);
		concrete.MarkStep(index, result.Success
			? EmploymentActionStepStatus.Completed
			: result.Completed ? EmploymentActionStepStatus.Failed : EmploymentActionStepStatus.Blocked);
		context.RecordRegister(result.Success
				? EmploymentRegisterEntryType.ActionStepCompleted
				: EmploymentRegisterEntryType.ActionStepFailed,
			task.AssignedEmployee,
			result.Message,
			task.CorrelationId);
		if (concrete.Status == EmploymentTaskStatus.Completed)
		{
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskCompleted, task.AssignedEmployee,
				$"Completed active task {task.Name}.", task.CorrelationId);
		}

		return result;
	}
}

public sealed class ManagerGoalBoard : IManagerGoalBoard
{
	private readonly IEmploymentHost _host;
	private readonly List<IManagerGoal> _goals = new();
	private readonly IEmploymentPersistenceStore? _persistence;
	private long _nextId;

	public ManagerGoalBoard(IEmploymentHost host)
	{
		_host = host;
	}

	internal ManagerGoalBoard(IEmploymentHost host, IEmploymentPersistenceStore persistence, IEnumerable<IManagerGoal> goals)
	{
		_host = host;
		_persistence = persistence;
		_goals.AddRange(goals);
		_nextId = _goals.Select(x => x.Id).DefaultIfEmpty().Max();
	}

	public IReadOnlyCollection<IManagerGoal> Goals => _goals;

	public IManagerGoal CreateGoal(ManagerGoalDefinition definition, ICharacter authorisedBy)
	{
		if (!_host.HasAuthority(authorisedBy, EmploymentAuthority.CreateManagerGoals))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create manager goals for {_host.EmploymentHostName}.");
		}

		if (!_host.EmploymentContracts.Where(x => x.Employee.Id == authorisedBy.Id && x.Status == EmploymentStatus.Active)
		          .Any(x => x.Authority.ContainsAll(definition.RequiredAuthority)))
		{
			throw new InvalidOperationException("A manager cannot create a goal that requires authority they do not possess.");
		}

		var goal = new ManagerGoal(
			Interlocked.Increment(ref _nextId),
			_host,
			definition.GoalType,
			definition.RequiredAuthority,
			ManagerGoalStatus.Active,
			definition.Configuration,
			definition.Priority,
			definition.EvaluationCadence,
			Guid.NewGuid(),
			_persistence);
		_goals.Add(goal);
		_persistence?.SaveManagerGoal(goal);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalCreated, authorisedBy,
			$"Created manager goal {definition.GoalType}.", goal.CorrelationId);
		return goal;
	}

	public void CancelGoal(IManagerGoal goal, ICharacter cancelledBy, string reason)
	{
		if (goal is not ManagerGoal concrete || !ReferenceEquals(concrete.Employer, _host))
		{
			return;
		}

		if (!_host.HasAuthority(cancelledBy, EmploymentAuthority.ModifyManagerGoals))
		{
			throw new InvalidOperationException($"{cancelledBy.HowSeen(cancelledBy, colour: false)} is not authorised to cancel manager goals for {_host.EmploymentHostName}.");
		}

		concrete.Cancel(reason);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalCancelled, cancelledBy, reason,
			concrete.CorrelationId);
	}

	public IReadOnlyCollection<IEmploymentActiveTask> EvaluateGoals(IEmploymentTaskContext context, DateTimeOffset now)
	{
		var tasks = new List<IEmploymentActiveTask>();
		foreach (var goal in _goals.OfType<ManagerGoal>().Where(x => x.Status == ManagerGoalStatus.Active))
		{
			if (goal.LastEvaluatedAt.HasValue && now - goal.LastEvaluatedAt.Value < goal.EvaluationCadence)
			{
				continue;
			}

			if (goal.Configuration.Conditions?.All(x => x.IsSatisfied(context, now, out _)) == false)
			{
				goal.MarkEvaluated(now, "Conditions were not satisfied.");
				_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
					goal.LastEvaluationResult!, goal.CorrelationId);
				continue;
			}

			if (goal.Configuration.ActionPlan is null)
			{
				goal.MarkEvaluated(now, "Goal had no action plan to create.");
				_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
					goal.LastEvaluationResult!, goal.CorrelationId);
				continue;
			}

			var task = _host.TaskBoard.CreateActiveTask(goal.Configuration.Description, goal.Configuration.ActionPlan, null,
				goal.CorrelationId);
			tasks.Add(task);
			goal.MarkEvaluated(now, $"Created active task {task.Name}.");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				goal.LastEvaluationResult!, goal.CorrelationId);
		}

		return tasks;
	}
}

public sealed class ManagerGoal : IManagerGoal
{
	private readonly IEmploymentPersistenceStore? _persistence;

	public ManagerGoal(long id, IEmploymentHost employer, ManagerGoalType goalType,
		EmploymentAuthoritySet requiredAuthority, ManagerGoalStatus status, ManagerGoalConfiguration configuration,
		int priority, TimeSpan evaluationCadence, Guid correlationId)
	{
		Id = id;
		Employer = employer;
		GoalType = goalType;
		RequiredAuthority = requiredAuthority;
		Status = status;
		Configuration = configuration;
		Priority = priority;
		EvaluationCadence = evaluationCadence;
		CorrelationId = correlationId;
	}

	internal ManagerGoal(long id, IEmploymentHost employer, ManagerGoalType goalType,
		EmploymentAuthoritySet requiredAuthority, ManagerGoalStatus status, ManagerGoalConfiguration configuration,
		int priority, TimeSpan evaluationCadence, Guid correlationId, IEmploymentPersistenceStore? persistence)
		: this(id, employer, goalType, requiredAuthority, status, configuration, priority, evaluationCadence, correlationId)
	{
		_persistence = persistence;
	}

	internal ManagerGoal(long id, IEmploymentHost employer, ManagerGoalType goalType,
		EmploymentAuthoritySet requiredAuthority, ManagerGoalStatus status, ManagerGoalConfiguration configuration,
		int priority, TimeSpan evaluationCadence, DateTimeOffset? lastEvaluatedAt, string? lastEvaluationResult,
		Guid correlationId, IEmploymentPersistenceStore persistence)
		: this(id, employer, goalType, requiredAuthority, status, configuration, priority, evaluationCadence, correlationId)
	{
		LastEvaluatedAt = lastEvaluatedAt;
		LastEvaluationResult = lastEvaluationResult;
		_persistence = persistence;
	}

	public long Id { get; }
	public IEmploymentHost Employer { get; }
	public ManagerGoalType GoalType { get; }
	public EmploymentAuthoritySet RequiredAuthority { get; }
	public ManagerGoalStatus Status { get; private set; }
	public ManagerGoalConfiguration Configuration { get; }
	public int Priority { get; }
	public TimeSpan EvaluationCadence { get; }
	public DateTimeOffset? LastEvaluatedAt { get; private set; }
	public string? LastEvaluationResult { get; private set; }
	public Guid CorrelationId { get; }

	public void MarkEvaluated(DateTimeOffset now, string result)
	{
		LastEvaluatedAt = now;
		LastEvaluationResult = result;
		_persistence?.SaveManagerGoalState(this);
	}

	public void Cancel(string reason)
	{
		Status = ManagerGoalStatus.Cancelled;
		LastEvaluationResult = reason;
		_persistence?.SaveManagerGoalState(this);
	}
}
