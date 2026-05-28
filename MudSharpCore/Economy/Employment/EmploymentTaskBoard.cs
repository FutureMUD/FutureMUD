using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

#nullable enable

namespace MudSharp.Economy.Employment;

public sealed class EmploymentTaskContext : IEmploymentTaskContext
{
	private sealed record CommodityProfile(
		long ItemId,
		string MaterialName,
		string? TagName,
		IReadOnlyDictionary<string, string> Characteristics,
		double Weight);

	private readonly Dictionary<string, bool> _manualOrders = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly Dictionary<string, int> _stockLevels = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly Dictionary<string, decimal> _accountBalances = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly HashSet<IEmploymentActionStep> _paymentAuthorisations = new();
	private readonly HashSet<string> _allowedCommands = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly HashSet<long> _unreachableCellIds = new();
	private readonly Dictionary<long, List<IGameItem>> _locationItems = new();
	private readonly HashSet<long> _configuredLocationItems = new();
	private readonly Dictionary<long, List<IGameItem>> _carriedTaskItems = new();
	private readonly Dictionary<long, List<IGameItem>> _containerContents = new();
	private readonly Dictionary<long, HashSet<string>> _itemTags = new();
	private readonly List<CommodityProfile> _commodityProfiles = new();
	private readonly bool _usePhysicalItemMovement;

	public EmploymentTaskContext(IEmploymentHost employer, bool usePhysicalItemMovement = false)
	{
		Employer = employer;
		_usePhysicalItemMovement = usePhysicalItemMovement;
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

	public void SetAvailableItems(ICell location, IEnumerable<IGameItem> items)
	{
		_locationItems[location.Id] = items.ToList();
		_configuredLocationItems.Add(location.Id);
	}

	public IReadOnlyCollection<IGameItem> AvailableItems(ICell location)
	{
		if (_usePhysicalItemMovement && !_configuredLocationItems.Contains(location.Id))
		{
			return location.GameItems
			               .SelectMany(x => x.DeepItems)
			               .DistinctBy(x => x.Id)
			               .ToList();
		}

		return LocationItems(location);
	}

	public IReadOnlyCollection<IGameItem> CarriedTaskItems(ICharacter actor)
	{
		return _carriedTaskItems.TryGetValue(actor.Id, out var items) ? items : [];
	}

	public IReadOnlyCollection<IGameItem> ContainedItems(IGameItem container)
	{
		return _containerContents.TryGetValue(container.Id, out var items) ? items : [];
	}

	public void SetItemTags(IGameItem item, params string[] tags)
	{
		_itemTags[item.Id] = new HashSet<string>(tags, StringComparer.InvariantCultureIgnoreCase);
	}

	public bool ItemHasTag(IGameItem item, string tagName)
	{
		if (string.IsNullOrWhiteSpace(tagName))
		{
			return false;
		}

		if (_itemTags.TryGetValue(item.Id, out var configuredTags) && configuredTags.Contains(tagName))
		{
			return true;
		}

		return item.Tags.Any(x =>
			x.Name.EqualTo(tagName) ||
			x.FullName.EqualTo(tagName) ||
			x.Id.ToString("F0").EqualTo(tagName));
	}

	public void SetCommodityWeight(IGameItem item, string materialName, double weight, string? tagName = null,
		IReadOnlyDictionary<string, string>? characteristics = null)
	{
		_commodityProfiles.RemoveAll(x => x.ItemId == item.Id);
		_commodityProfiles.Add(new CommodityProfile(
			item.Id,
			materialName,
			tagName,
			new Dictionary<string, string>(characteristics ?? new Dictionary<string, string>(),
				StringComparer.InvariantCultureIgnoreCase),
			weight));
	}

	public double CommodityWeight(IGameItem item, string materialName, string? tagName,
		IReadOnlyDictionary<string, string> characteristics)
	{
		var configured = _commodityProfiles.FirstOrDefault(x =>
			x.ItemId == item.Id &&
			x.MaterialName.EqualTo(materialName) &&
			(string.IsNullOrWhiteSpace(tagName) || (x.TagName?.EqualTo(tagName) ?? false)) &&
			CharacteristicsMatch(x.Characteristics, characteristics));
		if (configured is not null)
		{
			return configured.Weight;
		}

		var commodity = item.GetItemType<ICommodity>();
		if (commodity is null)
		{
			return 0.0;
		}

		if (!commodity.Material.Name.EqualTo(materialName) &&
		    !commodity.Material.Id.ToString("F0").EqualTo(materialName))
		{
			return 0.0;
		}

		if (!string.IsNullOrWhiteSpace(tagName) &&
		    !(commodity.Tag?.Name.EqualTo(tagName) == true ||
		      commodity.Tag?.FullName.EqualTo(tagName) == true ||
		      commodity.Tag?.Id.ToString("F0").EqualTo(tagName) == true))
		{
			return 0.0;
		}

		if (!CommodityCharacteristicsMatch(commodity, characteristics))
		{
			return 0.0;
		}

		return commodity.Weight;
	}

	public bool TryCollectTaskItem(ICharacter actor, IGameItem item, ICell source, out string reason)
	{
		if (!CanPath(actor, source))
		{
			reason = "The assigned employee cannot path to the source location.";
			return false;
		}

		var sourceItems = LocationItems(source);
		var index = sourceItems.FindIndex(x => x.Id == item.Id);
		if (index < 0 && (!_usePhysicalItemMovement || !source.GameItems.SelectMany(x => x.DeepItems).Any(x => x.Id == item.Id)))
		{
			reason = "The item is no longer at the source location.";
			return false;
		}

		var collected = index >= 0
			? sourceItems[index]
			: source.GameItems.SelectMany(x => x.DeepItems).First(x => x.Id == item.Id);
		if (index >= 0)
		{
			sourceItems.RemoveAt(index);
		}

		if (_usePhysicalItemMovement && !_configuredLocationItems.Contains(source.Id))
		{
			if (collected.ContainedIn is not null)
			{
				collected.ContainedIn.Take(collected);
			}
			else
			{
				source.Extract(collected);
			}
		}

		if (!_carriedTaskItems.TryGetValue(actor.Id, out var carried))
		{
			carried = new List<IGameItem>();
			_carriedTaskItems[actor.Id] = carried;
		}

		carried.Add(collected);
		reason = string.Empty;
		return true;
	}

	public bool TryDeliverTaskItems(ICharacter actor, ICell destination, IGameItem? container, string? containerTag,
		out string reason)
	{
		if (!CanPath(actor, destination))
		{
			reason = "The assigned employee cannot path to the delivery destination.";
			return false;
		}

		if (!_carriedTaskItems.TryGetValue(actor.Id, out var carried) || carried.Count == 0)
		{
			reason = "The assigned employee is not carrying any task items to deliver.";
			return false;
		}

		var destinationItems = LocationItems(destination);
		var targetContainer = container;
		if (targetContainer is null && !string.IsNullOrWhiteSpace(containerTag))
		{
			targetContainer = AvailableItems(destination).FirstOrDefault(x => ItemHasTag(x, containerTag));
		}

		if (targetContainer is null && !string.IsNullOrWhiteSpace(containerTag))
		{
			reason = $"There is no destination container tagged {containerTag}.";
			return false;
		}

		if (targetContainer is null)
		{
			destinationItems.AddRange(carried);
			if (_usePhysicalItemMovement && !_configuredLocationItems.Contains(destination.Id))
			{
				foreach (var item in carried)
				{
					destination.Insert(item);
				}
			}
		}
		else
		{
			var containerComponent = targetContainer.GetItemType<IContainer>();
			if (containerComponent is null)
			{
				reason = $"{targetContainer.Name} is not a container.";
				return false;
			}

			var rejected = carried.FirstOrDefault(x => !containerComponent.CanPut(x));
			if (rejected is not null)
			{
				reason = $"{targetContainer.Name} cannot contain {rejected.Name}.";
				return false;
			}

			if (!_containerContents.TryGetValue(targetContainer.Id, out var contents))
			{
				contents = new List<IGameItem>();
				_containerContents[targetContainer.Id] = contents;
			}

			contents.AddRange(carried);
			if (_usePhysicalItemMovement && !_configuredLocationItems.Contains(destination.Id))
			{
				foreach (var item in carried)
				{
					containerComponent.Put(actor, item);
				}
			}
		}

		carried.Clear();
		reason = string.Empty;
		return true;
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

	private List<IGameItem> LocationItems(ICell location)
	{
		if (!_locationItems.TryGetValue(location.Id, out var items))
		{
			items = location.GameItems?.SelectMany(x => x.DeepItems).DistinctBy(x => x.Id).ToList() ?? [];
			_locationItems[location.Id] = items;
		}

		return items;
	}

	private static bool CharacteristicsMatch(IReadOnlyDictionary<string, string> configured,
		IReadOnlyDictionary<string, string> required)
	{
		foreach (var characteristic in required)
		{
			if (!configured.TryGetValue(characteristic.Key, out var value) || !value.EqualTo(characteristic.Value))
			{
				return false;
			}
		}

		return true;
	}

	private static bool CommodityCharacteristicsMatch(ICommodity commodity,
		IReadOnlyDictionary<string, string> required)
	{
		foreach (var characteristic in required)
		{
			var match = commodity.CommodityCharacteristics.Any(x =>
				x.Key.Name.EqualTo(characteristic.Key) &&
				(
					x.Value.Name.EqualTo(characteristic.Value) ||
					x.Value.GetValue.EqualTo(characteristic.Value) ||
					x.Value.GetBasicValue.EqualTo(characteristic.Value) ||
					x.Value.GetFancyValue.EqualTo(characteristic.Value)
				));
			if (!match)
			{
				return false;
			}
		}

		return true;
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

		if (authorisedBy is not null && actionPlan.RequiredAuthority.Authorities != EmploymentAuthority.None &&
		    !_host.HasAuthority(authorisedBy, actionPlan.RequiredAuthority.Authorities))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create scheduled task rules with {actionPlan.RequiredAuthority.Authorities.DescribeEnum()} authority for {_host.EmploymentHostName}.");
		}

		var rule = new EmploymentScheduledTaskRule(_host, name, idempotencyKey, conditions, actionPlan, cooldown);
		_scheduledRules.Add(rule);
		_persistence?.SaveScheduledRule(rule);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleCreated, authorisedBy,
			$"Created scheduled task rule {name}.");
		_host.DebugEmployment($"Created scheduled task rule {name}.");
		return rule;
	}

	public IEmploymentActiveTask CreateActiveTask(string name, EmploymentActionPlan actionPlan, ICharacter? authorisedBy,
		Guid? correlationId = null)
	{
		if (authorisedBy is not null && !_host.HasAuthority(authorisedBy, EmploymentAuthority.AssignTasks))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create tasks for {_host.EmploymentHostName}.");
		}

		if (authorisedBy is not null && actionPlan.RequiredAuthority.Authorities != EmploymentAuthority.None &&
		    !_host.HasAuthority(authorisedBy, actionPlan.RequiredAuthority.Authorities))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create tasks with {actionPlan.RequiredAuthority.Authorities.DescribeEnum()} authority for {_host.EmploymentHostName}.");
		}

		var task = new EmploymentActiveTask(_host, name, actionPlan, correlationId ?? Guid.NewGuid(), _persistence);
		_activeTasks.Add(task);
		_persistence?.SaveActiveTask(task);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskCreated, authorisedBy,
			$"Created active task {name}.", task.CorrelationId);
		_host.DebugEmployment($"Created active task {name} ({task.CorrelationId}).", authorisedBy?.Gameworld);
		return task;
	}

	public IReadOnlyCollection<IEmploymentActiveTask> EvaluateScheduledRules(IEmploymentTaskContext context, DateTimeOffset now)
	{
		var spawned = new List<IEmploymentActiveTask>();
		foreach (var rule in _scheduledRules.OfType<EmploymentScheduledTaskRule>())
		{
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleEvaluated, null,
				$"Evaluated scheduled task rule {rule.Name}.");
			_host.DebugEmployment($"Evaluating scheduled task rule {rule.Name}.");
			if (!rule.CanSpawn(context, now, out var spawnReason))
			{
				_host.DebugEmployment($"Scheduled task rule {rule.Name} did not spawn: {spawnReason}");
				continue;
			}

			if (_activeTasks.OfType<EmploymentActiveTask>().Any(x =>
				    x.IdempotencyKey.Equals(rule.IdempotencyKey, StringComparison.InvariantCultureIgnoreCase) &&
				    x.Status is EmploymentTaskStatus.Pending or EmploymentTaskStatus.Assigned or EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked))
			{
				_host.DebugEmployment(
					$"Scheduled task rule {rule.Name} did not spawn because an active task with idempotency key {rule.IdempotencyKey} already exists.");
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
			_host.DebugEmployment($"Scheduled task rule {rule.Name} spawned active task {task.CorrelationId}.");
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
		IEmploymentTaskContext context, out string reason, bool blockWhenNoCandidateMatches = true)
	{
		if (task is not EmploymentActiveTask concrete)
		{
			reason = "Unsupported active task implementation.";
			context.Employer.DebugEmployment($"Could not assign task {task.Name}: {reason}");
			return false;
		}

		var rejectionReasons = new List<string>();
		foreach (var candidate in candidates)
		{
			if (!task.Employer.EmploymentContracts.Any(x =>
				    x.Employee.Id == candidate.Candidate.Id &&
				    x.Status == EmploymentStatus.Active))
			{
				rejectionReasons.Add($"{candidate.Candidate.Name}: no active employment contract");
				context.Employer.DebugEmployment(
					$"Skipped {candidate.Candidate.Name} for task {task.Name}: no active employment contract.");
				continue;
			}

			var missingCapabilities = task.ActionPlan.RequiredCapabilities
			                          .Where(x => !candidate.Capabilities.Contains(x))
			                          .ToList();
			if (missingCapabilities.Any())
			{
				rejectionReasons.Add(
					$"{candidate.Candidate.Name}: missing {missingCapabilities.Select(x => x.DescribeEnum()).ListToString()} capability");
				context.Employer.DebugEmployment(
					$"Skipped {candidate.Candidate.Name} for task {task.Name}: missing required AI capabilities.");
				continue;
			}

			var nextStepIndex = concrete.NextStepIndex;
			if (nextStepIndex < 0)
			{
				rejectionReasons.Add($"{candidate.Candidate.Name}: task has no pending steps");
				context.Employer.DebugEmployment($"Skipped task {task.Name}: it has no pending steps.");
				continue;
			}

			if (!task.ActionPlan.Steps[nextStepIndex].CanExecute(context, candidate.Candidate, out var stepReason))
			{
				rejectionReasons.Add($"{candidate.Candidate.Name}: {stepReason}");
				context.Employer.DebugEmployment(
					$"Skipped {candidate.Candidate.Name} for task {task.Name}: next step cannot execute ({stepReason}).");
				continue;
			}

			concrete.Assign(candidate.Candidate);
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskAssigned, candidate.Candidate,
				$"Assigned task {task.Name}.", task.CorrelationId);
			context.Employer.DebugEmployment($"Assigned task {task.Name} to {candidate.Candidate.Name}.",
				candidate.Candidate.Gameworld);
			reason = string.Empty;
			return true;
		}

		reason = rejectionReasons.Any()
			? $"No active employee is eligible to complete the task: {rejectionReasons.ListToString()}."
			: "No active employee is eligible to complete the task.";
		if (!blockWhenNoCandidateMatches)
		{
			return false;
		}

		concrete.Block(reason);
		context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskBlocked, null, reason, task.CorrelationId);
		context.Employer.DebugEmployment($"Blocked task {task.Name}: {reason}");
		return false;
	}

	public EmploymentActionStepResult AdvanceTask(IEmploymentActiveTask task, IEmploymentTaskContext context)
	{
		if (task is not EmploymentActiveTask concrete)
		{
			context.Employer.DebugEmployment($"Could not advance task {task.Name}: unsupported active task implementation.");
			return EmploymentActionStepResult.Failed("Unsupported active task implementation.");
		}

		if (task.AssignedEmployee is null)
		{
			concrete.Block("Task has no assigned employee.");
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskBlocked, null, concrete.BlockedReason!, task.CorrelationId);
			context.Employer.DebugEmployment($"Blocked task {task.Name}: {concrete.BlockedReason}");
			return EmploymentActionStepResult.Blocked(concrete.BlockedReason!);
		}

		var index = concrete.NextStepIndex;
		if (index < 0)
		{
			context.Employer.DebugEmployment($"Task {task.Name} is already complete.");
			return EmploymentActionStepResult.CompletedResult("Task is already complete.");
		}

		var step = task.ActionPlan.Steps[index];
		if (!step.CanExecute(context, task.AssignedEmployee, out var reason))
		{
			concrete.MarkStep(index, EmploymentActionStepStatus.Blocked);
			concrete.Block(reason);
			context.RecordRegister(EmploymentRegisterEntryType.ActionStepFailed, task.AssignedEmployee, reason, task.CorrelationId);
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskBlocked, task.AssignedEmployee, reason, task.CorrelationId);
			context.Employer.DebugEmployment(
				$"Blocked task {task.Name} at step {index + 1:N0} ({step.StepType.DescribeEnum()}): {reason}",
				task.AssignedEmployee.Gameworld);
			return EmploymentActionStepResult.Blocked(reason);
		}

		concrete.MarkStep(index, EmploymentActionStepStatus.InProgress);
		context.RecordRegister(EmploymentRegisterEntryType.ActionStepStarted, task.AssignedEmployee,
			$"Started {step.StepType} action.", task.CorrelationId);
		context.Employer.DebugEmployment(
			$"{task.AssignedEmployee.Name} started step {index + 1:N0} ({step.StepType.DescribeEnum()}) of task {task.Name}.",
			task.AssignedEmployee.Gameworld);
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
		context.Employer.DebugEmployment(
			$"{task.AssignedEmployee.Name} {(result.Success ? "completed" : "did not complete")} step {index + 1:N0} of task {task.Name}: {result.Message}",
			task.AssignedEmployee.Gameworld);
		if (concrete.Status == EmploymentTaskStatus.Completed)
		{
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskCompleted, task.AssignedEmployee,
				$"Completed active task {task.Name}.", task.CorrelationId);
			context.Employer.DebugEmployment($"Completed active task {task.Name}.", task.AssignedEmployee.Gameworld);
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

		if (!authorisedBy.IsAdministrator() &&
		    !_host.EmploymentContracts.Where(x => x.Employee.Id == authorisedBy.Id && x.Status == EmploymentStatus.Active)
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
		_host.DebugEmployment($"Created manager goal {definition.GoalType.DescribeEnum()} #{goal.Id:N0}.",
			authorisedBy.Gameworld);
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
		_host.DebugEmployment($"Cancelled manager goal #{concrete.Id:N0}: {reason}", cancelledBy.Gameworld);
	}

	public IReadOnlyCollection<IEmploymentActiveTask> EvaluateGoals(IEmploymentTaskContext context, DateTimeOffset now)
	{
		var tasks = new List<IEmploymentActiveTask>();
		foreach (var goal in _goals.OfType<ManagerGoal>().Where(x => x.Status == ManagerGoalStatus.Active))
		{
			if (goal.LastEvaluatedAt.HasValue && now - goal.LastEvaluatedAt.Value < goal.EvaluationCadence)
			{
				_host.DebugEmployment($"Skipped manager goal #{goal.Id:N0}: evaluation cadence has not elapsed.");
				continue;
			}

			if (goal.Configuration.Conditions?.All(x => x.IsSatisfied(context, now, out _)) == false)
			{
				goal.MarkEvaluated(now, "Conditions were not satisfied.");
				_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
					goal.LastEvaluationResult!, goal.CorrelationId);
				_host.DebugEmployment($"Manager goal #{goal.Id:N0} did not create work: {goal.LastEvaluationResult}");
				continue;
			}

			if (goal.Configuration.ActionPlan is null)
			{
				goal.MarkEvaluated(now, "Goal had no action plan to create.");
				_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
					goal.LastEvaluationResult!, goal.CorrelationId);
				_host.DebugEmployment($"Manager goal #{goal.Id:N0} did not create work: {goal.LastEvaluationResult}");
				continue;
			}

			var task = _host.TaskBoard.CreateActiveTask(goal.Configuration.Description, goal.Configuration.ActionPlan, null,
				goal.CorrelationId);
			tasks.Add(task);
			goal.MarkEvaluated(now, $"Created active task {task.Name}.");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				goal.LastEvaluationResult!, goal.CorrelationId);
			_host.DebugEmployment($"Manager goal #{goal.Id:N0} created active task {task.Name}.");
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
