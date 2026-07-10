using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Commands.Helpers;

internal sealed class EmploymentManagerGoalDraft
{
	private readonly List<IEmploymentTaskCondition> _conditions = new();
	private readonly List<IEmploymentActionStep> _steps = new();

	public EmploymentManagerGoalDraft(IEmploymentHost host, EmploymentManagerGoalDefinition goalDefinition,
		string description)
	{
		Host = host;
		TypeKey = goalDefinition.Key;
		GoalType = goalDefinition.GoalType;
		Description = description.Trim();
		RequiredAuthority = goalDefinition.DefaultRequiredAuthority;
		Priority = 1;
		EvaluationCadence = TimeSpan.FromHours(1);
		Policy = ManagerGoalPolicy.Default;
	}

	public EmploymentManagerGoalDraft(IEmploymentHost host, IManagerGoal goal, string? description = null)
	{
		Host = host;
		var definition = EmploymentManagerGoalCatalog.ForGoalType(goal.GoalType);
		TypeKey = definition?.Key ?? goal.GoalType.ToString();
		GoalType = goal.GoalType;
		Description = string.IsNullOrWhiteSpace(description)
			? goal.Configuration.Description
			: description.Trim();
		RequiredAuthority = goal.RequiredAuthority;
		Priority = goal.Priority;
		EvaluationCadence = goal.EvaluationCadence;
		ConditionExpression = goal.Configuration.ConditionExpression;
		Policy = goal.Policy;
		_conditions.AddRange(goal.Configuration.Conditions ?? []);
		_steps.AddRange(goal.Configuration.ActionPlan?.Steps ?? []);
	}

	public IEmploymentHost Host { get; }
	public string TypeKey { get; private set; }
	public ManagerGoalType GoalType { get; private set; }
	public string Description { get; private set; }
	public EmploymentAuthoritySet RequiredAuthority { get; private set; }
	public int Priority { get; private set; }
	public TimeSpan EvaluationCadence { get; private set; }
	public EmploymentConditionExpression? ConditionExpression { get; private set; }
	public ManagerGoalPolicy Policy { get; private set; }
	public IReadOnlyList<IEmploymentTaskCondition> Conditions => _conditions;
	public IReadOnlyList<IEmploymentActionStep> Steps => _steps;

	public void SetGoalType(EmploymentManagerGoalDefinition goalDefinition)
	{
		TypeKey = goalDefinition.Key;
		GoalType = goalDefinition.GoalType;
		RequiredAuthority = goalDefinition.DefaultRequiredAuthority;
	}

	public void SetDescription(string description)
	{
		Description = description.Trim();
	}

	public void SetRequiredAuthority(EmploymentAuthoritySet authority)
	{
		RequiredAuthority = authority;
	}

	public void SetPriority(int priority)
	{
		Priority = priority;
	}

	public void SetCadence(TimeSpan cadence)
	{
		EvaluationCadence = cadence;
	}

	public void SetPolicy(ManagerGoalPolicy policy)
	{
		Policy = policy;
	}

	public void SetConditionExpression(EmploymentConditionExpression? expression)
	{
		ConditionExpression = expression;
	}

	public void AddCondition(IEmploymentTaskCondition condition)
	{
		_conditions.Add(condition);
	}

	public void AddStep(IEmploymentActionStep step)
	{
		_steps.Add(step);
	}

	public void ClearSteps()
	{
		_steps.Clear();
	}

	public bool RemoveCondition(int index)
	{
		if (index < 0 || index >= _conditions.Count)
		{
			return false;
		}

		_conditions.RemoveAt(index);
		ConditionExpression = null;
		return true;
	}

	public void RemoveConditions(Func<IEmploymentTaskCondition, bool> predicate)
	{
		if (_conditions.RemoveAll(x => predicate(x)) > 0)
		{
			ConditionExpression = null;
		}
	}

	public bool RemoveStep(int index)
	{
		if (index < 0 || index >= _steps.Count)
		{
			return false;
		}

		_steps.RemoveAt(index);
		return true;
	}

	public EmploymentActionPlan? ToActionPlan()
	{
		return _steps.Any() ? new EmploymentActionPlan(_steps) : null;
	}

	public ManagerGoalDefinition ToDefinition()
	{
		return new ManagerGoalDefinition(
			GoalType,
			RequiredAuthority,
			new ManagerGoalConfiguration(
				Description,
				ToActionPlan(),
				_conditions.ToList(),
				ConditionExpression),
			Priority,
			EvaluationCadence,
			Policy);
	}
}

internal sealed class EmploymentManagerGoalAuthoringService
{
	private readonly EmploymentTaskAuthoringService _taskAuthoring;
	private readonly EmploymentScheduledRuleAuthoringService _scheduledRuleAuthoring;

	public EmploymentManagerGoalAuthoringService(EmploymentTaskAuthoringService? taskAuthoring = null,
		EmploymentScheduledRuleAuthoringService? scheduledRuleAuthoring = null)
	{
		_taskAuthoring = taskAuthoring ?? new EmploymentTaskAuthoringService();
		_scheduledRuleAuthoring = scheduledRuleAuthoring ?? new EmploymentScheduledRuleAuthoringService(_taskAuthoring);
	}

	public string RenderGoalTypes(ICharacter actor, string selector)
	{
		var sb = new StringBuilder();
		selector = selector.Trim();
		sb.AppendLine("Employment Manager Goal Types".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine("Use these with ".ColourCommand() + "goals draft new <type> <description>".ColourCommand() + " and then add ".ColourCommand() + "goals condition".ColourCommand() + " and ".ColourCommand() + "goals step".ColourCommand() + " entries before finalising.");
		sb.AppendLine();

		if (!string.IsNullOrWhiteSpace(selector) && !selector.EqualTo("all"))
		{
			var definition = EmploymentManagerGoalCatalog.Get(selector);
			if (definition is not null)
			{
				AppendGoalDefinition(sb, definition, detailed: true);
				return sb.ToString();
			}

			var categoryGoals = EmploymentManagerGoalCatalog.ForCategory(selector);
			if (categoryGoals.Any())
			{
				sb.AppendLine($"{categoryGoals.First().Category.DescribeEnum().ColourName()} goal types:");
				foreach (var goal in categoryGoals)
				{
					AppendGoalDefinition(sb, goal, detailed: false);
				}

				return sb.ToString();
			}

			sb.AppendLine($"There is no manager goal type or category matching {selector.ColourCommand()}.");
			sb.AppendLine($"Categories: {EmploymentManagerGoalCatalog.Categories.Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return sb.ToString();
		}

		foreach (var category in EmploymentManagerGoalCatalog.Categories)
		{
			sb.AppendLine($"{category.DescribeEnum().ColourName()}:");
			foreach (var goal in EmploymentManagerGoalCatalog.ForCategory(category.ToString()))
			{
				AppendGoalDefinition(sb, goal, detailed: false);
				sb.AppendLine();
			}

			sb.AppendLine();
		}

		return sb.ToString();
	}

	public bool TryStartDraft(ICharacter actor, IEmploymentHost host, string typeSelector, string description,
		out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var definition = EmploymentManagerGoalCatalog.Get(typeSelector);
		if (definition is null)
		{
			message = $"There is no manager goal type matching {typeSelector.ColourCommand()}. Use {"goals types".ColourCommand()} to list supported goal types.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(description))
		{
			message = "What short description should this manager goal use?";
			return false;
		}

		if (!TryValidateGoalTypeForHost(host, definition, out message))
		{
			return false;
		}

		RemoveDraft(actor, host);
		var draft = new EmploymentManagerGoalDraft(host, definition, description);
		EnsureNativeHospitalStockDefaults(draft);
		actor.AddEffect(new EmploymentManagerGoalDraftEffect(actor, draft));
		message =
			$"You begin a {definition.Key.ColourCommand()} manager goal draft named {draft.Description.ColourName()} for {host.EmploymentHostName.ColourName()}.";
		return true;
	}

	public bool TryCopyGoalToDraft(ICharacter actor, IEmploymentHost host, string selector, string? description,
		out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var goal = GoalBySelector(host, selector);
		if (goal is null)
		{
			message = $"There is no manager goal matching {selector.ColourCommand()}.";
			return false;
		}

		RemoveDraft(actor, host);
		var draft = new EmploymentManagerGoalDraft(host, goal, description);
		EnsureNativeHospitalStockDefaults(draft);
		actor.AddEffect(new EmploymentManagerGoalDraftEffect(actor, draft));
		message =
			$"You copy manager goal #{goal.Id.ToString("N0", actor).ColourValue()} into a draft named {draft.Description.ColourName()}. Finalise the draft and cancel the old goal when you are ready to replace it.";
		return true;
	}

	public bool TrySetDraftType(ICharacter actor, IEmploymentHost host, string typeSelector, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		var definition = EmploymentManagerGoalCatalog.Get(typeSelector);
		if (definition is null)
		{
			message = $"There is no manager goal type matching {typeSelector.ColourCommand()}.";
			return false;
		}

		if (!TryValidateGoalTypeForHost(host, definition, out message))
		{
			return false;
		}

		draft.SetGoalType(definition);
		EnsureNativeHospitalStockDefaults(draft);
		message = $"You set the manager goal draft type to {definition.Key.ColourCommand()}.";
		return true;
	}

	public bool TrySetDraftDescription(ICharacter actor, IEmploymentHost host, string description, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(description))
		{
			message = "What short description should this manager goal use?";
			return false;
		}

		draft.SetDescription(description);
		message = $"You rename your manager goal draft to {draft.Description.ColourName()}.";
		return true;
	}

	public bool TrySetDraftPriority(ICharacter actor, IEmploymentHost host, string text, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!int.TryParse(text, NumberStyles.Integer, actor, out var priority) || priority < 1)
		{
			message = "Manager goal priorities must be positive whole numbers.";
			return false;
		}

		draft.SetPriority(priority);
		message = $"You set the manager goal draft priority to {priority.ToString("N0", actor).ColourValue()}.";
		return true;
	}

	public bool TrySetDraftCadence(ICharacter actor, IEmploymentHost host, string text, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!TryParseDuration(text, out var cadence) || cadence <= TimeSpan.Zero)
		{
			message = $"Evaluation cadences use a positive duration such as {"1h".ColourCommand()}, {"30m".ColourCommand()}, or {"01:00:00".ColourCommand()}.";
			return false;
		}

		draft.SetCadence(cadence);
		message = $"You set the manager goal draft cadence to {cadence.Describe(actor).ColourValue()}.";
		return true;
	}

	public bool TrySetDraftExpression(ICharacter actor, IEmploymentHost host, string text, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!EmploymentScheduledRuleAuthoringService.TryParseConditionExpression(text, draft.Conditions, host.TaskBoard,
				out var expression, out message))
		{
			return false;
		}

		var authority = EmploymentConditionExpressionEvaluator.RequiredAuthority(expression, draft.Conditions, host.TaskBoard);
		if (!TryRequireAuthority(actor, host, RequiredGoalConditionAuthority(authority.Authorities), out message))
		{
			return false;
		}

		draft.SetConditionExpression(expression);
		message = $"You set the manager goal condition expression to {EmploymentScheduledRuleAuthoringService.DescribeConditionExpression(expression, draft.Conditions).ColourCommand()}.";
		return true;
	}

	public bool TrySetDraftBudget(ICharacter actor, IEmploymentHost host, string text, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (text.EqualToAny("none", "clear", "off", "unlimited"))
		{
			draft.SetPolicy(new ManagerGoalPolicy([], draft.Policy.RiskLimits));
			message = "You clear the manager goal draft budget limits.";
			return true;
		}

		if (!TryParseMoney(host, text, out var budget, out message))
		{
			return false;
		}

		var policy = new ManagerGoalPolicy([budget], draft.Policy.RiskLimits);
		if (!ManagerGoalBoard.TryValidateGoalPolicy(draft.ToActionPlan(), policy, out var policyReason))
		{
			message = policyReason;
			return false;
		}

		draft.SetPolicy(policy);
		message = $"You set the manager goal draft budget limit to {DescribeMoney(budget)}.";
		return true;
	}

	public bool TrySetDraftRiskLimit(ICharacter actor, IEmploymentHost host, StringStack input, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (input.IsFinished)
		{
			message = "Manager goal risk limits use: goals draft risk tasks <count|none>, goals draft risk steps <count|none>, or goals draft risk unbounded <on|off>.";
			return false;
		}

		var limit = input.PopSpeech();
		var risk = draft.Policy.RiskLimits;
		ManagerGoalRiskLimits updated;
		switch (limit.ToLowerInvariant())
		{
			case "tasks":
			case "task":
			case "activetasks":
			case "active":
				if (!TryParseOptionalCount(input, actor, 0, out var maximumActiveTasks, out message))
				{
					return false;
				}

				updated = risk with { MaximumActiveTasks = maximumActiveTasks };
				break;
			case "steps":
			case "step":
			case "actions":
			case "actionsteps":
				if (!TryParseOptionalCount(input, actor, 1, out var maximumActionSteps, out message))
				{
					return false;
				}

				updated = risk with { MaximumActionSteps = maximumActionSteps };
				break;
			case "unbounded":
			case "unboundedfinancial":
			case "financial":
				if (input.IsFinished || !TryParseBoolean(input.PopSpeech(), out var allowsUnbounded))
				{
					message = "Use on/off, true/false, yes/no, or allow/block for the unbounded financial risk limit.";
					return false;
				}

				updated = risk with { AllowsUnboundedFinancialSteps = allowsUnbounded };
				break;
			default:
				message = "Manager goal risk limits use: tasks, steps, or unbounded.";
				return false;
		}

		var policy = new ManagerGoalPolicy(draft.Policy.BudgetLimits, updated);
		if (!ManagerGoalBoard.TryValidateGoalPolicy(draft.ToActionPlan(), policy, out var policyReason))
		{
			message = policyReason;
			return false;
		}

		draft.SetPolicy(policy);
		message = $"You set the manager goal draft risk limits to {DescribeRiskLimits(updated)}.";
		return true;
	}

	public bool TrySetDraftAuthority(ICharacter actor, IEmploymentHost host, StringStack input, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!TryParseAuthoritySet(input, out var authority, out message, allowNone: true))
		{
			return false;
		}

		if (!TryRequireAuthority(actor, host, authority.Authorities, out message))
		{
			return false;
		}

		draft.SetRequiredAuthority(authority);
		message = $"You set the manager goal draft required authority to {DescribeAuthority(authority.Authorities)}.";
		return true;
	}

	public bool TryAddCondition(ICharacter actor, IEmploymentHost host, StringStack input, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!_scheduledRuleAuthoring.TryParseCondition(actor, host, input, out var condition, out message))
		{
			return false;
		}

		if (!TryRequireAuthority(actor, host, RequiredGoalConditionAuthority(condition.RequiredAuthority.Authorities), out message))
		{
			return false;
		}

		draft.AddCondition(condition);
		message = $"You add a manager goal condition: {EmploymentScheduledRuleAuthoringService.DescribeCondition(condition, actor, host)}.";
		return true;
	}

	public bool TryAddStep(ICharacter actor, IEmploymentHost host, StringStack input, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (IsNativeHospitalStockGoal(draft.GoalType))
		{
			message = "Hospital stock manager goals generate their purchase and delivery steps automatically from current service requirements and inventory.";
			return false;
		}

		if (!_taskAuthoring.TryParseActionStep(actor, host, input, out var step, out message))
		{
			return false;
		}

		if (!TryRequireAuthority(actor, host, step.RequiredAuthority.Authorities, out message))
		{
			return false;
		}

		draft.AddStep(step);
		if (!ManagerGoalBoard.TryValidateGoalPolicy(draft.ToActionPlan(), draft.Policy, out var policyReason))
		{
			draft.RemoveStep(draft.Steps.Count - 1);
			message = policyReason;
			return false;
		}

		message = $"You add a manager goal action: {EmploymentTaskAuthoringService.DescribeStep(step, actor)}.";
		return true;
	}

	public bool TryRemoveCondition(ICharacter actor, IEmploymentHost host, int number, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!draft.RemoveCondition(number - 1))
		{
			message = "There is no such condition in your manager goal draft.";
			return false;
		}

		message = $"You remove condition {number.ToString("N0", actor).ColourValue()} from your manager goal draft.";
		return true;
	}

	public bool TryRemoveStep(ICharacter actor, IEmploymentHost host, int number, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!draft.RemoveStep(number - 1))
		{
			message = "There is no such action step in your manager goal draft.";
			return false;
		}

		message = $"You remove action step {number.ToString("N0", actor).ColourValue()} from your manager goal draft.";
		return true;
	}

	public bool TryDiscardDraft(ICharacter actor, IEmploymentHost host, out string message)
	{
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		if (!RemoveDraft(actor, host))
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		message = $"You discard your manager goal draft for {host.EmploymentHostName.ColourName()}.";
		return true;
	}

	public bool TryFinaliseDraft(ICharacter actor, IEmploymentHost host, out IManagerGoal? goal, out string message)
	{
		goal = null;
		if (!TryRequireCreateGoals(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (IsNativeHospitalStockGoal(draft.GoalType) && !HasMatchingHospitalStockCondition(draft))
		{
			message = "Hospital stock manager goals require a matching hospital stock or theatre stock condition.";
			return false;
		}

		if (!draft.Steps.Any() && !IsNativeHospitalStockGoal(draft.GoalType))
		{
			message = "You cannot finalise a manager goal draft with no action steps.";
			return false;
		}

		try
		{
			goal = host.ManagerGoalBoard.CreateGoal(draft.ToDefinition(), actor);
			RemoveDraft(actor, host);
			message = RenderCreatedGoalSummary(actor, goal);
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryCancelGoal(ICharacter actor, IEmploymentHost host, string selector, string reason, out string message)
	{
		var goal = GoalBySelector(host, selector);
		if (goal is null)
		{
			message = $"There is no manager goal matching {selector.ColourCommand()}.";
			return false;
		}

		try
		{
			host.ManagerGoalBoard.CancelGoal(goal, actor, reason);
			message = $"You cancel manager goal #{goal.Id.ToString("N0", actor).ColourValue()}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryEvaluateGoals(ICharacter actor, IEmploymentHost host, out string message)
	{
		if (!TryRequireModifyGoals(actor, host, out message))
		{
			return false;
		}

		var spawned = host.ManagerGoalBoard.EvaluateGoals(new EmploymentTaskContext(host), EmploymentClock.CurrentInstant(host));
		message =
			$"You evaluate manager goals for {host.EmploymentHostName.ColourName()} and spawn {spawned.Count.ToString("N0", actor).ColourValue()} active task{(spawned.Count == 1 ? string.Empty : "s")}.";
		return true;
	}

	public string RenderDraft(ICharacter actor, IEmploymentHost host)
	{
		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			return $"You do not have an active manager goal draft for {host.EmploymentHostName.ColourName()}.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Manager goal draft {draft.Description.ColourName()} for {host.EmploymentHostName.ColourName()}:");
		sb.AppendLine($"Type: {draft.TypeKey.ColourCommand()} ({draft.GoalType.DescribeEnum().ColourName()})");
		sb.AppendLine($"Required Authority: {DescribeAuthority(draft.RequiredAuthority.Authorities)}");
		sb.AppendLine($"Priority: {draft.Priority.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Cadence: {draft.EvaluationCadence.Describe(actor).ColourValue()}");
		sb.AppendLine($"Budget Limits: {DescribeBudgetLimits(draft.Policy.BudgetLimits)}");
		sb.AppendLine($"Risk Limits: {DescribeRiskLimits(draft.Policy.RiskLimits)}");
		sb.AppendLine($"Condition Expression: {EmploymentScheduledRuleAuthoringService.DescribeConditionExpression(draft.ConditionExpression, draft.Conditions).ColourCommand()}");
		sb.AppendLine();
		AppendConditionList(sb, actor, host, draft.Conditions);
		sb.AppendLine();
		AppendStepList(sb, actor, draft.Steps);
		return sb.ToString();
	}

	public string RenderGoalDetail(ICharacter actor, IEmploymentHost host, string selector)
	{
		var goal = GoalBySelector(host, selector);
		if (goal is null)
		{
			return $"There is no manager goal matching {selector.ColourCommand()}.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Manager Goal #{goal.Id.ToString("N0", actor).ColourValue()} - {goal.Configuration.Description.ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {goal.GoalType.DescribeEnum().ColourName()}");
		sb.AppendLine($"Status: {goal.Status.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Required Authority: {DescribeAuthority(goal.RequiredAuthority.Authorities)}");
		sb.AppendLine($"Priority: {goal.Priority.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Cadence: {goal.EvaluationCadence.Describe(actor).ColourValue()}");
		sb.AppendLine($"Budget Limits: {DescribeBudgetLimits(goal.Policy.BudgetLimits)}");
		sb.AppendLine($"Risk Limits: {DescribeRiskLimits(goal.Policy.RiskLimits)}");
		sb.AppendLine($"Condition Expression: {EmploymentScheduledRuleAuthoringService.DescribeConditionExpression(goal.Configuration.ConditionExpression, goal.Configuration.Conditions?.ToList() ?? []).ColourCommand()}");
		sb.AppendLine($"Last Evaluated: {(goal.LastEvaluatedAt.HasValue ? EmploymentClock.DescribeInstant(host, goal.LastEvaluatedAt.Value, actor).ColourValue() : "never".ColourValue())}");
		sb.AppendLine($"Last Result: {(string.IsNullOrWhiteSpace(goal.LastEvaluationResult) ? "none".ColourValue() : goal.LastEvaluationResult)}");
		sb.AppendLine();
		AppendConditionList(sb, actor, host, goal.Configuration.Conditions?.ToList() ?? []);
		sb.AppendLine();
		AppendStepList(sb, actor, goal.Configuration.ActionPlan?.Steps ?? []);
		return sb.ToString();
	}

	private static void AppendGoalDefinition(StringBuilder sb, EmploymentManagerGoalDefinition definition,
		bool detailed)
	{
		sb.AppendLine($"\t{definition.Key.ColourCommand()} - {definition.Summary}");
		sb.AppendLine($"\t\tSyntax: {definition.Syntax.ColourCommand()}");
		sb.AppendLine($"\t\tType: {definition.GoalType.DescribeEnum().ColourName()} | Authority: {DescribeAuthority(definition.DefaultRequiredAuthority.Authorities)}");
		if (definition.SuggestedConditions.Any())
		{
			sb.AppendLine($"\t\tSuggested Conditions: {definition.SuggestedConditions.Select(x => x.ColourCommand()).ListToString()}");
		}

		if (definition.SuggestedActions.Any())
		{
			sb.AppendLine($"\t\tSuggested Actions: {definition.SuggestedActions.Select(x => x.ColourCommand()).ListToString()}");
		}

		if (detailed && definition.Aliases is not null && definition.Aliases.Any())
		{
			sb.AppendLine($"\t\tAliases: {definition.Aliases.Select(x => x.ColourCommand()).ListToString()}");
		}
	}

	private static void AppendStepList(StringBuilder sb, ICharacter actor, IReadOnlyList<IEmploymentActionStep> steps)
	{
		sb.AppendLine("Steps:");
		if (!steps.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		for (var i = 0; i < steps.Count; i++)
		{
			var step = steps[i];
			sb.AppendLine($"\t#{(i + 1).ToString("N0", actor)} - {EmploymentTaskAuthoringService.DescribeStep(step, actor)}");
			sb.AppendLine($"\t\tAuthority: {DescribeAuthority(step.RequiredAuthority.Authorities)} | AI: {DescribeCapabilities(step.RequiredCapabilities)} | Catalogue: {EmploymentTaskAuthoringService.DescribeStepCatalogueStatus(step)}");
		}
	}

	private static void AppendConditionList(StringBuilder sb, ICharacter actor, IEmploymentHost host,
		IReadOnlyList<IEmploymentTaskCondition> conditions)
	{
		sb.AppendLine("Conditions:");
		if (!conditions.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		for (var i = 0; i < conditions.Count; i++)
		{
			var condition = conditions[i];
			sb.AppendLine($"\t#{(i + 1).ToString("N0", actor)} - {EmploymentScheduledRuleAuthoringService.DescribeCondition(condition, actor, host)}");
			sb.AppendLine($"\t\tType: {condition.ConditionType.DescribeEnum().ColourName()} | Goal Authority: {DescribeAuthority(RequiredGoalConditionAuthority(condition.RequiredAuthority.Authorities))}");
		}
	}

	private static string RenderCreatedGoalSummary(ICharacter actor, IManagerGoal goal)
	{
		var conditionCount = goal.Configuration.Conditions?.Count ?? 0;
		var stepCount = goal.Configuration.ActionPlan?.Steps.Count ?? 0;
		var sb = new StringBuilder();
		sb.AppendLine($"You create manager goal #{goal.Id.ToString("N0", actor).ColourValue()} {goal.Configuration.Description.ColourName()} with {conditionCount.ToString("N0", actor).ColourValue()} condition{(conditionCount == 1 ? string.Empty : "s")} and {stepCount.ToString("N0", actor).ColourValue()} step{(stepCount == 1 ? string.Empty : "s")}.");
		sb.AppendLine($"Type: {goal.GoalType.DescribeEnum().ColourName()} | Priority: {goal.Priority.ToString("N0", actor).ColourValue()} | Cadence: {goal.EvaluationCadence.Describe(actor).ColourValue()}");
		sb.AppendLine($"Budget Limits: {DescribeBudgetLimits(goal.Policy.BudgetLimits)} | Risk Limits: {DescribeRiskLimits(goal.Policy.RiskLimits)}");
		return sb.ToString();
	}

	private static bool IsNativeHospitalStockGoal(ManagerGoalType goalType)
	{
		return HospitalSupplyStockGoalPlanner.IsHospitalStockGoal(goalType);
	}

	private static bool TryValidateGoalTypeForHost(IEmploymentHost host, EmploymentManagerGoalDefinition definition,
		out string message)
	{
		if (IsNativeHospitalStockGoal(definition.GoalType) && host is not IHospital)
		{
			message = $"{definition.Key.ColourCommand()} manager goals can only be created for hospital employment hosts.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static void EnsureNativeHospitalStockDefaults(EmploymentManagerGoalDraft draft)
	{
		if (!IsNativeHospitalStockGoal(draft.GoalType))
		{
			return;
		}

		var itemType = HospitalSupplyStockGoalPlanner.ItemTypeForGoal(draft.GoalType);
		draft.ClearSteps();
		if (HospitalTheatreStockGoalPlanner.IsHospitalTheatreStockGoal(draft.GoalType))
		{
			draft.RemoveConditions(x => x is HospitalTheatreStockCondition stock && stock.ItemType != itemType);
			if (!draft.Conditions.OfType<HospitalTheatreStockCondition>().Any(x => x.ItemType == itemType))
			{
				draft.AddCondition(new HospitalTheatreStockCondition(itemType, 1));
			}

			return;
		}

		draft.RemoveConditions(x => x is HospitalSupplyStockCondition stock && stock.ItemType != itemType);
		if (!draft.Conditions.OfType<HospitalSupplyStockCondition>().Any(x => x.ItemType == itemType))
		{
			draft.AddCondition(new HospitalSupplyStockCondition(itemType, itemType == HospitalServiceSupplyItemType.Consumable ? 30 : 5, "any", null));
		}
	}

	private static bool HasMatchingHospitalStockCondition(EmploymentManagerGoalDraft draft)
	{
		if (!IsNativeHospitalStockGoal(draft.GoalType))
		{
			return false;
		}

		var itemType = HospitalSupplyStockGoalPlanner.ItemTypeForGoal(draft.GoalType);
		if (HospitalTheatreStockGoalPlanner.IsHospitalTheatreStockGoal(draft.GoalType))
		{
			return draft.Conditions.OfType<HospitalTheatreStockCondition>().Any(x => x.ItemType == itemType);
		}

		return draft.Conditions.OfType<HospitalSupplyStockCondition>().Any(x => x.ItemType == itemType);
	}

	private static bool TryRequireCreateGoals(ICharacter actor, IEmploymentHost host, out string message)
	{
		if (actor.IsAdministrator() || host.HasAuthority(actor, EmploymentAuthority.CreateManagerGoals))
		{
			message = string.Empty;
			return true;
		}

		message = $"You do not have the delegated {EmploymentAuthority.CreateManagerGoals.DescribeEnum().ColourName()} authority for {host.EmploymentHostName.ColourName()}.";
		return false;
	}

	private static bool TryRequireModifyGoals(ICharacter actor, IEmploymentHost host, out string message)
	{
		if (actor.IsAdministrator() || host.HasAuthority(actor, EmploymentAuthority.ModifyManagerGoals))
		{
			message = string.Empty;
			return true;
		}

		message = $"You do not have the delegated {EmploymentAuthority.ModifyManagerGoals.DescribeEnum().ColourName()} authority for {host.EmploymentHostName.ColourName()}.";
		return false;
	}

	private static bool TryRequireAuthority(ICharacter actor, IEmploymentHost host, EmploymentAuthority authority,
		out string message)
	{
		if (authority == EmploymentAuthority.None || actor.IsAdministrator() || host.HasAuthority(actor, authority))
		{
			message = string.Empty;
			return true;
		}

		message = $"You do not have the delegated {authority.DescribeEnum().ColourName()} authority for {host.EmploymentHostName.ColourName()}.";
		return false;
	}

	private static EmploymentAuthority RequiredGoalConditionAuthority(EmploymentAuthority authority)
	{
		return authority & ~EmploymentAuthority.CreateScheduledRules;
	}

	private static EmploymentManagerGoalDraft? DraftFor(ICharacter actor, IEmploymentHost host)
	{
		return actor.EffectsOfType<EmploymentManagerGoalDraftEffect>()
		            .FirstOrDefault(x => x.Draft.Host.EmploymentHostType == host.EmploymentHostType && x.Draft.Host.Id == host.Id)
		            ?.Draft;
	}

	private static bool RemoveDraft(ICharacter actor, IEmploymentHost host)
	{
		return actor.RemoveAllEffects<EmploymentManagerGoalDraftEffect>(
			x => x.Draft.Host.EmploymentHostType == host.EmploymentHostType && x.Draft.Host.Id == host.Id,
			true);
	}

	private static IManagerGoal? GoalBySelector(IEmploymentHost host, string selector)
	{
		selector = selector.Trim();
		if (string.IsNullOrWhiteSpace(selector))
		{
			return null;
		}

		var goals = host.ManagerGoalBoard.Goals.OrderBy(x => x.Priority).ThenBy(x => x.Id).ToList();
		if (TryParseCommandNumber(selector, out var id))
		{
			return goals.FirstOrDefault(x => x.Id == id);
		}

		var goalDefinition = EmploymentManagerGoalCatalog.Get(selector);
		if (goalDefinition is not null)
		{
			return goals.FirstOrDefault(x => x.GoalType == goalDefinition.GoalType && x.Status == ManagerGoalStatus.Active) ??
			       goals.FirstOrDefault(x => x.GoalType == goalDefinition.GoalType);
		}

		return goals.FirstOrDefault(x => x.GoalType.ToString().EqualTo(selector)) ??
		       goals.FirstOrDefault(x => x.Configuration.Description.EqualTo(selector));
	}

	private static bool TryParseCommandNumber(string text, out long number)
	{
		text = text.Trim();
		if (text.StartsWith("#", StringComparison.Ordinal))
		{
			text = text[1..];
		}

		return long.TryParse(text, out number);
	}

	private static bool TryParseAuthoritySet(StringStack input, out EmploymentAuthoritySet authority,
		out string error, bool allowNone = false)
	{
		authority = EmploymentAuthoritySet.Empty;
		error = string.Empty;
		if (input.IsFinished)
		{
			error = "Which delegated authority do you want this manager goal to require?";
			return false;
		}

		var authorities = EmploymentAuthority.None;
		while (!input.IsFinished)
		{
			var token = input.PopSpeech();
			foreach (var part in token.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			{
				if (part.EqualTo("all"))
				{
					authority = EmploymentAuthoritySet.All;
					return true;
				}

				if (part.EqualTo("none") || part.EqualTo("clear"))
				{
					if (!allowNone)
					{
						error = "Use a specific authority name, or use none only with a setting command.";
						return false;
					}

					authorities = EmploymentAuthority.None;
					continue;
				}

				if (!TryParseAuthority(part, out var parsed))
				{
					error = $"Unknown delegated authority {part.ColourCommand()}. Try {"contracts delegate #1 help".ColourCommand()} for the list.";
					return false;
				}

				authorities |= parsed;
			}
		}

		if (authorities == EmploymentAuthority.None && !allowNone)
		{
			error = "Which delegated authority do you want this manager goal to require?";
			return false;
		}

		authority = new EmploymentAuthoritySet(authorities);
		return true;
	}

	private static bool TryParseAuthority(string text, out EmploymentAuthority authority)
	{
		if (text.TryParseEnum(out authority) && authority != EmploymentAuthority.None)
		{
			return true;
		}

		authority = text.CollapseString().ToLowerInvariant() switch
		{
			"view" or "viewemployees" or "employees" => EmploymentAuthority.ViewEmployees,
			"hire" or "hiring" => EmploymentAuthority.HireEmployees,
			"fire" or "firing" => EmploymentAuthority.FireEmployees,
			"openings" or "createopenings" or "createjobopenings" => EmploymentAuthority.CreateJobOpenings,
			"modifyopenings" or "modifyjobopenings" => EmploymentAuthority.ModifyJobOpenings,
			"pay" or "setpay" or "setpaywithinband" => EmploymentAuthority.SetPayWithinBand,
			"tasks" or "task" or "assign" or "assigntasks" => EmploymentAuthority.AssignTasks,
			"canceltasks" or "cancel" => EmploymentAuthority.CancelTasks,
			"rules" or "schedulerules" or "createscheduledrules" => EmploymentAuthority.CreateScheduledRules,
			"modifyrules" or "modifyscheduledrules" => EmploymentAuthority.ModifyScheduledRules,
			"goals" or "creategoals" or "createmanagergoals" => EmploymentAuthority.CreateManagerGoals,
			"modifygoals" or "modifymanagergoals" => EmploymentAuthority.ModifyManagerGoals,
			"purchases" or "purchase" or "approvepurchases" => EmploymentAuthority.ApprovePurchases,
			"storeaccount" or "usestoreaccount" => EmploymentAuthority.UseStoreAccount,
			"cashwithdraw" or "withdraw" or "withdrawbusinesscash" => EmploymentAuthority.WithdrawBusinessCash,
			"cashdeposit" or "deposit" or "depositbusinesscash" => EmploymentAuthority.DepositBusinessCash,
			"stock" or "stockrules" or "managestockrules" => EmploymentAuthority.ManageStockRules,
			"craft" or "crafting" or "craftrules" or "managecraftrules" => EmploymentAuthority.ManageCraftRules,
			"delivery" or "deliver" or "routes" or "managedeliveryroutes" => EmploymentAuthority.ManageDeliveryRoutes,
			"prices" or "pricing" or "adjustprices" => EmploymentAuthority.AdjustPrices,
			"tax" or "taxes" or "paytaxes" => EmploymentAuthority.PayTaxes,
			"board" or "postboard" or "posttohostboard" => EmploymentAuthority.PostToHostBoard,
			"moderateboard" or "moderatehostboard" => EmploymentAuthority.ModerateHostBoard,
			"payroll" or "wages" or "managepayroll" => EmploymentAuthority.ManagePayroll,
			_ => EmploymentAuthority.None
		};

		return authority != EmploymentAuthority.None;
	}

	private static bool TryParseDuration(string text, out TimeSpan duration)
	{
		duration = TimeSpan.Zero;
		text = text.Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		if (TimeSpan.TryParse(text, CultureInfo.InvariantCulture, out duration))
		{
			return true;
		}

		var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (parts.Length == 2 && decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
		{
			return TryDurationFromValue(value, parts[1], out duration);
		}

		var suffix = text[^1].ToString();
		if (decimal.TryParse(text[..^1], NumberStyles.Number, CultureInfo.InvariantCulture, out value))
		{
			return TryDurationFromValue(value, suffix, out duration);
		}

		return false;
	}

	private static bool TryDurationFromValue(decimal value, string unit, out TimeSpan duration)
	{
		duration = TimeSpan.Zero;
		if (value <= 0.0M || value > 1000000M)
		{
			return false;
		}

		try
		{
			duration = unit.CollapseString().ToLowerInvariant() switch
			{
				"s" or "sec" or "secs" or "second" or "seconds" => TimeSpan.FromSeconds((double)value),
				"m" or "min" or "mins" or "minute" or "minutes" => TimeSpan.FromMinutes((double)value),
				"h" or "hr" or "hrs" or "hour" or "hours" => TimeSpan.FromHours((double)value),
				"d" or "day" or "days" => TimeSpan.FromDays((double)value),
				_ => TimeSpan.Zero
			};
		}
		catch (OverflowException)
		{
			duration = TimeSpan.Zero;
			return false;
		}

		return duration > TimeSpan.Zero;
	}

	private static bool TryParseMoney(IEmploymentHost host, string text, out MoneyAmount amount, out string message)
	{
		amount = null!;
		var currency = EmploymentTaskAuthoringService.ResolveHostCurrency(host);
		if (currency is null)
		{
			message = "This employment host does not expose a currency for manager goal budgets.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(text) || !currency.TryGetBaseCurrency(text, out var parsed) || parsed <= 0.0M)
		{
			message = $"Budget limits use a positive amount in {currency.Name.ColourName()}.";
			return false;
		}

		amount = new MoneyAmount(currency, parsed);
		message = string.Empty;
		return true;
	}

	private static string DescribeMoney(MoneyAmount amount)
	{
		return amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
	}

	private static string DescribeBudgetLimits(IReadOnlyCollection<MoneyAmount> budgets)
	{
		return budgets.Any()
			? budgets.Select(DescribeMoney).ListToString()
			: "none".ColourValue();
	}

	private static bool TryParseOptionalCount(StringStack input, ICharacter actor, int minimum, out int? value,
		out string message)
	{
		value = null;
		if (input.IsFinished)
		{
			message = "Which limit value do you want to set?";
			return false;
		}

		var text = input.PopSpeech();
		if (text.EqualToAny("none", "unlimited", "clear", "off"))
		{
			message = string.Empty;
			return true;
		}

		if (!int.TryParse(text, NumberStyles.Integer, actor, out var count) || count < minimum)
		{
			message = $"Use a whole number of at least {minimum.ToString("N0", actor).ColourValue()}, or none.";
			return false;
		}

		value = count;
		message = string.Empty;
		return true;
	}

	private static bool TryParseBoolean(string text, out bool value)
	{
		var normalised = text.CollapseString().ToLowerInvariant();
		value = normalised switch
		{
			"on" or "true" or "yes" or "y" or "allow" or "allowed" => true,
			"off" or "false" or "no" or "n" or "block" or "blocked" => false,
			_ => false
		};

		return normalised.EqualToAny("on", "true", "yes", "y", "allow", "allowed", "off", "false", "no", "n", "block", "blocked");
	}

	private static string DescribeRiskLimits(ManagerGoalRiskLimits risk)
	{
		var activeTasks = risk.MaximumActiveTasks.HasValue
			? risk.MaximumActiveTasks.Value.ToString("N0", CultureInfo.InvariantCulture)
			: "unlimited";
		var actionSteps = risk.MaximumActionSteps.HasValue
			? risk.MaximumActionSteps.Value.ToString("N0", CultureInfo.InvariantCulture)
			: "unlimited";
		var unboundedFinancial = risk.AllowsUnboundedFinancialSteps ? "allowed" : "blocked";
		return $"active tasks {activeTasks.ColourValue()}, action steps {actionSteps.ColourValue()}, unbounded financial {unboundedFinancial.ColourValue()}";
	}

	private static string DescribeCapabilities(IReadOnlySet<EmploymentAICapability> capabilities)
	{
		return capabilities.Any()
			? capabilities.Select(x => x.DescribeEnum().ColourName()).ListToString()
			: "none".ColourValue();
	}

	private static string DescribeAuthority(EmploymentAuthority authority)
	{
		return authority == EmploymentAuthority.None
			? "none".ColourValue()
			: authority.DescribeEnum().ColourName();
	}
}
