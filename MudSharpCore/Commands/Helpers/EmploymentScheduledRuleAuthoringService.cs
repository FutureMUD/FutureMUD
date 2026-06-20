using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Commands.Helpers;

internal sealed class EmploymentScheduledRuleDraft
{
	private readonly List<IEmploymentTaskCondition> _conditions = new();
	private readonly List<IEmploymentActionStep> _steps = new();

	public EmploymentScheduledRuleDraft(IEmploymentHost host, string name)
	{
		Host = host;
		Name = name.Trim();
		IdempotencyKey = DefaultKey(name);
		Cooldown = TimeSpan.FromHours(1);
	}

	public EmploymentScheduledRuleDraft(IEmploymentHost host, string name, TimeSpan cooldown,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		IEnumerable<IEmploymentActionStep> steps)
		: this(host, name)
	{
		Cooldown = cooldown;
		_conditions.AddRange(conditions);
		ConditionExpression = conditionExpression;
		_steps.AddRange(steps);
	}

	public IEmploymentHost Host { get; }
	public string Name { get; }
	public string IdempotencyKey { get; private set; }
	public TimeSpan Cooldown { get; private set; }
	public EmploymentConditionExpression? ConditionExpression { get; private set; }
	public IReadOnlyList<IEmploymentTaskCondition> Conditions => _conditions;
	public IReadOnlyList<IEmploymentActionStep> Steps => _steps;

	public void SetKey(string key)
	{
		IdempotencyKey = key.Trim();
	}

	public void SetCooldown(TimeSpan cooldown)
	{
		Cooldown = cooldown;
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

	public bool RemoveStep(int index)
	{
		if (index < 0 || index >= _steps.Count)
		{
			return false;
		}

		_steps.RemoveAt(index);
		return true;
	}

	public EmploymentActionPlan ToActionPlan()
	{
		return new EmploymentActionPlan(_steps);
	}

	private static string DefaultKey(string name)
	{
		var collapsed = name.CollapseString().ToLowerInvariant();
		return string.IsNullOrWhiteSpace(collapsed) ? Guid.NewGuid().ToString("N") : collapsed;
	}
}

internal sealed class EmploymentScheduledRuleAuthoringService
{
	private readonly EmploymentTaskAuthoringService _taskAuthoring;

	public EmploymentScheduledRuleAuthoringService(EmploymentTaskAuthoringService? taskAuthoring = null)
	{
		_taskAuthoring = taskAuthoring ?? new EmploymentTaskAuthoringService();
	}

	public bool TryStartDraft(ICharacter actor, IEmploymentHost host, string name, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			message = "What name do you want to give this scheduled employment rule draft?";
			return false;
		}

		RemoveDraft(actor, host);
		actor.AddEffect(new EmploymentScheduledRuleDraftEffect(actor, new EmploymentScheduledRuleDraft(host, name)));
		message = $"You begin a scheduled employment rule draft named {name.Trim().ColourName()} for {host.EmploymentHostName.ColourName()}.";
		return true;
	}

	public bool TryCopyRuleToDraft(ICharacter actor, IEmploymentHost host, string selector, string? newName,
		out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var rule = RuleBySelector(host, selector);
		if (rule is null)
		{
			message = $"There is no scheduled employment rule matching {selector.ColourCommand()}.";
			return false;
		}

		var name = string.IsNullOrWhiteSpace(newName) ? $"{rule.Name} copy" : newName.Trim();
		RemoveDraft(actor, host);
		actor.AddEffect(new EmploymentScheduledRuleDraftEffect(actor,
			new EmploymentScheduledRuleDraft(host, name, rule.Cooldown, rule.Conditions, rule.ConditionExpression,
				rule.ActionPlan.Steps)));
		message =
			$"You copy scheduled rule {rule.Name.ColourName()} into a new draft named {name.ColourName()}. The draft uses a new idempotency key; use {"tasks rule draft key <key>".ColourCommand()} if you want to override it.";
		return true;
	}

	public bool TrySetDraftKey(ICharacter actor, IEmploymentHost host, string key, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(key))
		{
			message = "What idempotency key should this scheduled rule use?";
			return false;
		}

		draft.SetKey(key);
		message = $"You set the scheduled rule idempotency key to {draft.IdempotencyKey.ColourValue()}.";
		return true;
	}

	public bool TrySetDraftCooldown(ICharacter actor, IEmploymentHost host, string text, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!TryParseDuration(text, out var cooldown) || cooldown <= TimeSpan.Zero)
		{
			message = $"Cooldowns use a positive duration such as {"1h".ColourCommand()}, {"30m".ColourCommand()}, or {"01:00:00".ColourCommand()}.";
			return false;
		}

		draft.SetCooldown(cooldown);
		message = $"You set the scheduled rule cooldown to {cooldown.Describe(actor).ColourValue()}.";
		return true;
	}

	public bool TrySetDraftExpression(ICharacter actor, IEmploymentHost host, string text, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!TryParseConditionExpression(text, draft.Conditions, host.TaskBoard, out var expression, out message))
		{
			return false;
		}

		var authority = EmploymentConditionExpressionEvaluator.RequiredAuthority(expression, draft.Conditions, host.TaskBoard);
		if (!TryRequireAuthority(actor, host, authority.Authorities, out message))
		{
			return false;
		}

		draft.SetConditionExpression(expression);
		message = $"You set the scheduled rule condition expression to {DescribeConditionExpression(expression, draft.Conditions).ColourCommand()}.";
		return true;
	}

	public bool TryAddCondition(ICharacter actor, IEmploymentHost host, StringStack input, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!TryParseCondition(actor, host, input, out var condition, out message))
		{
			return false;
		}

		if (!TryRequireAuthority(actor, host, condition.RequiredAuthority.Authorities, out message))
		{
			return false;
		}

		draft.AddCondition(condition);
		message = $"You add a scheduled rule condition: {DescribeCondition(condition, actor, host)}.";
		return true;
	}

	public bool TryAddStep(ICharacter actor, IEmploymentHost host, StringStack input, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
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
		message = $"You add a scheduled rule action: {EmploymentTaskAuthoringService.DescribeStep(step, actor)}.";
		return true;
	}

	public bool TryRemoveCondition(ICharacter actor, IEmploymentHost host, int number, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!draft.RemoveCondition(number - 1))
		{
			message = "There is no such condition in your scheduled rule draft.";
			return false;
		}

		message = $"You remove condition {number.ToString("N0", actor).ColourValue()} from your scheduled rule draft.";
		return true;
	}

	public bool TryRemoveStep(ICharacter actor, IEmploymentHost host, int number, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!draft.RemoveStep(number - 1))
		{
			message = "There is no such action step in your scheduled rule draft.";
			return false;
		}

		message = $"You remove action step {number.ToString("N0", actor).ColourValue()} from your scheduled rule draft.";
		return true;
	}

	public bool TryDiscardDraft(ICharacter actor, IEmploymentHost host, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		if (!RemoveDraft(actor, host))
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		message = $"You discard your scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
		return true;
	}

	public bool TryFinaliseDraft(ICharacter actor, IEmploymentHost host, out IEmploymentScheduledTaskRule? rule,
		out string message)
	{
		rule = null;
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!draft.Conditions.Any())
		{
			message = "You cannot finalise a scheduled employment rule with no conditions.";
			return false;
		}

		if (!draft.Steps.Any())
		{
			message = "You cannot finalise a scheduled employment rule with no action steps.";
			return false;
		}

		try
		{
			rule = host.TaskBoard.CreateScheduledRule(draft.Name, draft.IdempotencyKey, draft.Conditions,
				draft.ConditionExpression,
				draft.ToActionPlan(), draft.Cooldown, actor);
			RemoveDraft(actor, host);
			message = RenderCreatedRuleSummary(actor, rule);
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryCreateOneShotRule(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentScheduledTaskRule? rule, out string message)
	{
		rule = null;
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		if (input.IsFinished)
		{
			message = $"One-shot scheduled rule creation uses the syntax: {"tasks rule create <name> cooldown <timespan> when <condition> [and <condition> ...] do <action> [then <action> ...]".ColourCommand()}";
			return false;
		}

		var name = input.PopSpeech();
		var tokens = PopRemainingTokens(input).ToList();
		var cooldownIndex = IndexOf(tokens, "cooldown");
		var whenIndex = IndexOf(tokens, "when");
		var doIndex = IndexOf(tokens, "do");
		if (cooldownIndex != 0 || whenIndex <= cooldownIndex + 1 || doIndex <= whenIndex + 1)
		{
			message = $"One-shot scheduled rule creation uses the syntax: {"tasks rule create <name> cooldown <timespan> when <condition> [and <condition> ...] do <action> [then <action> ...]".ColourCommand()}";
			return false;
		}

		if (!TryParseDuration(string.Join(" ", tokens.Skip(cooldownIndex + 1).Take(whenIndex - cooldownIndex - 1)),
			    out var cooldown) || cooldown <= TimeSpan.Zero)
		{
			message = $"Cooldowns use a positive duration such as {"1h".ColourCommand()}, {"30m".ColourCommand()}, or {"01:00:00".ColourCommand()}.";
			return false;
		}

		var conditions = new List<IEmploymentTaskCondition>();
		foreach (var conditionTokens in SplitConditionTokens(tokens.Skip(whenIndex + 1).Take(doIndex - whenIndex - 1)))
		{
			var stack = new StringStack(string.Join(" ", conditionTokens));
			if (!TryParseCondition(actor, host, stack, out var condition, out message))
			{
				return false;
			}

			if (!TryRequireAuthority(actor, host, condition.RequiredAuthority.Authorities, out message))
			{
				return false;
			}

			conditions.Add(condition);
		}

		var steps = new List<IEmploymentActionStep>();
		foreach (var actionTokens in SplitTokens(tokens.Skip(doIndex + 1), "then"))
		{
			var stack = new StringStack(string.Join(" ", actionTokens));
			if (!_taskAuthoring.TryParseActionStep(actor, host, stack, out var step, out message))
			{
				return false;
			}

			if (!TryRequireAuthority(actor, host, step.RequiredAuthority.Authorities, out message))
			{
				return false;
			}

			steps.Add(step);
		}

		if (!conditions.Any() || !steps.Any())
		{
			message = "Scheduled rules need at least one condition and one action step.";
			return false;
		}

		try
		{
			rule = host.TaskBoard.CreateScheduledRule(name, EmploymentScheduledRuleDraftKey(name), conditions,
				new EmploymentActionPlan(steps), cooldown, actor);
			message = RenderCreatedRuleSummary(actor, rule);
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public string RenderDraft(ICharacter actor, IEmploymentHost host)
	{
		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			return $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
		}

		var plan = draft.ToActionPlan();
		var sb = new StringBuilder();
		sb.AppendLine($"Scheduled employment rule draft {draft.Name.ColourName()} for {host.EmploymentHostName.ColourName()}:");
		sb.AppendLine($"Idempotency Key: {draft.IdempotencyKey.ColourValue()}");
		sb.AppendLine($"Cooldown: {draft.Cooldown.Describe(actor).ColourValue()}");
		sb.AppendLine($"Condition Authority: {DescribeAuthority(EmploymentConditionExpressionEvaluator.RequiredAuthority(draft.ConditionExpression, draft.Conditions, host.TaskBoard).Authorities)}");
		sb.AppendLine($"Condition Expression: {DescribeConditionExpression(draft.ConditionExpression, draft.Conditions).ColourCommand()}");
		sb.AppendLine($"Action Authority: {plan.RequiredAuthority.Authorities.DescribeEnum().ColourName()}");
		sb.AppendLine($"Required AI Capabilities: {DescribeCapabilities(plan.RequiredCapabilities)}");
		sb.AppendLine();
		AppendConditionList(sb, actor, host, draft.Conditions);
		sb.AppendLine();
		AppendStepList(sb, actor, draft.Steps);
		return sb.ToString();
	}

	public string RenderAvailableConditions(ICharacter actor, string selector)
	{
		var sb = new StringBuilder();
		selector = selector.Trim();
		sb.AppendLine("Employment Scheduled Rule Conditions".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine("Use these with ".ColourCommand() + "tasks rule condition <condition> ...".ColourCommand() + " while you have a rule draft open.");
		sb.AppendLine("Drafts can use ".ColourCommand() + "tasks rule draft expression <expr>".ColourCommand() + " with #1, @predicate, and/or/not, and parentheses.");
		sb.AppendLine();

		if (!string.IsNullOrWhiteSpace(selector) && !selector.EqualTo("all"))
		{
			var definition = EmploymentConditionCatalog.Get(selector);
			if (definition is not null)
			{
				AppendConditionDefinition(sb, definition, detailed: true);
				return sb.ToString();
			}

			var categoryConditions = EmploymentConditionCatalog.ForCategory(selector);
			if (categoryConditions.Any())
			{
				sb.AppendLine($"{categoryConditions.First().Category.DescribeEnum().ColourName()} conditions:");
				foreach (var condition in categoryConditions)
				{
					AppendConditionDefinition(sb, condition, detailed: false);
				}

				return sb.ToString();
			}

			sb.AppendLine($"There is no employment scheduled-rule condition or category matching {selector.ColourCommand()}.");
			sb.AppendLine($"Categories: {EmploymentConditionCatalog.Categories.Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return sb.ToString();
		}

		foreach (var category in EmploymentConditionCatalog.Categories)
		{
			sb.AppendLine($"{category.DescribeEnum().ColourName()}:");
			foreach (var condition in EmploymentConditionCatalog.ForCategory(category.ToString()))
			{
				AppendConditionDefinition(sb, condition, detailed: false);
			}
		}

		return sb.ToString();
	}

	public string RenderDiagnostics(ICharacter actor, IEmploymentHost host, string selector, string? manualKey)
	{
		var rule = RuleBySelector(host, selector);
		if (rule is null)
		{
			return $"There is no scheduled employment rule matching {selector.ColourCommand()}.";
		}

		var context = BuildEvaluationContext(host, manualKey);
		var now = EmploymentClock.CurrentInstant(host);
		var sb = new StringBuilder();
		sb.AppendLine($"Scheduled rule diagnostics for {rule.Name.ColourName()}:");
		sb.AppendLine($"Manual Trigger: {(string.IsNullOrWhiteSpace(manualKey) ? "none".ColourValue() : manualKey.ColourCommand())}");
		sb.AppendLine($"\tStatus: {rule.Status.DescribeEnum().ColourValue()}");
		if (rule.LastSpawnedAt.HasValue && now - rule.LastSpawnedAt.Value < rule.Cooldown)
		{
			sb.AppendLine($"\tCooldown: {"blocked".ColourError()} - next eligible after {EmploymentClock.DescribeInstant(host, rule.LastSpawnedAt.Value + rule.Cooldown, actor).ColourValue()}");
		}
		else
		{
			sb.AppendLine($"\tCooldown: {"ready".ColourValue()}");
		}

		foreach (var condition in rule.Conditions)
		{
			var satisfied = condition.IsSatisfied(context, now, out var reason);
			sb.AppendLine($"\t{DescribeCondition(condition, actor, host)} - {satisfied.ToColouredString()}{(satisfied ? string.Empty : $" - {reason.ColourError()}")}");
		}

		var expressionResult = EmploymentConditionExpressionEvaluator.Evaluate(
			rule.ConditionExpression,
			rule.Conditions.ToList(),
			context,
			now);
		sb.AppendLine($"\tExpression: {DescribeConditionExpression(rule.ConditionExpression, rule.Conditions.ToList()).ColourCommand()} - {expressionResult.Satisfied.ToColouredString()}");
		foreach (var leaf in expressionResult.Leaves)
		{
			sb.AppendLine($"\t\t{leaf.Label.ColourName()} - {leaf.Satisfied.ToColouredString()}{(leaf.Satisfied || string.IsNullOrWhiteSpace(leaf.Reason) ? string.Empty : $" - {leaf.Reason.ColourError()}")}");
		}

		var duplicate = host.TaskBoard.ActiveTasks.Any(x =>
			x is EmploymentActiveTask active &&
			active.IdempotencyKey.EqualTo(rule.IdempotencyKey) &&
			x.Status is EmploymentTaskStatus.Pending or EmploymentTaskStatus.Assigned or EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked);
		sb.AppendLine($"\tDuplicate Active Task: {duplicate.ToColouredString()}");
		rule.CanSpawn(context, now, out var spawnReason);
		sb.AppendLine($"Result: {(string.IsNullOrWhiteSpace(spawnReason) && !duplicate ? "would spawn".ColourValue() : $"would not spawn - {(duplicate ? "an active task with the same idempotency key already exists" : spawnReason)}".ColourError())}");
		return sb.ToString();
	}

	public bool TryEvaluate(ICharacter actor, IEmploymentHost host, string selector, string? manualKey,
		out string message)
	{
		if (!TryRequireModifyRules(actor, host, out message))
		{
			return false;
		}

		var context = BuildEvaluationContext(host, manualKey);
		var now = EmploymentClock.CurrentInstant(host);
		IReadOnlyCollection<IEmploymentActiveTask> spawned;
		if (selector.EqualTo("all"))
		{
			spawned = host.TaskBoard.EvaluateScheduledRules(context, now);
			message = $"You evaluate all scheduled employment rules for {host.EmploymentHostName.ColourName()} and spawn {spawned.Count.ToString("N0", actor).ColourValue()} active task{(spawned.Count == 1 ? string.Empty : "s")}.";
			return true;
		}

		var rule = RuleBySelector(host, selector);
		if (rule is null)
		{
			message = $"There is no scheduled employment rule matching {selector.ColourCommand()}.";
			return false;
		}

		spawned = host.TaskBoard.EvaluateScheduledRule(rule, context, now);
		if (!spawned.Any() && !rule.CanSpawn(context, now, out var spawnReason))
		{
			message = $"You evaluate scheduled rule {rule.Name.ColourName()} and spawn {"0".ColourValue()} active tasks: {spawnReason.ColourError()}";
			return true;
		}

		message = $"You evaluate scheduled rule {rule.Name.ColourName()} and spawn {spawned.Count.ToString("N0", actor).ColourValue()} active task{(spawned.Count == 1 ? string.Empty : "s")}.";
		return true;
	}

	public bool TryPauseRule(ICharacter actor, IEmploymentHost host, string selector, string reason, out string message)
	{
		return TrySetRuleStatus(actor, host, selector, reason, pause: true, out message);
	}

	public bool TryResumeRule(ICharacter actor, IEmploymentHost host, string selector, string reason, out string message)
	{
		return TrySetRuleStatus(actor, host, selector, reason, pause: false, out message);
	}

	public bool TryCancelRule(ICharacter actor, IEmploymentHost host, string selector, string reason,
		out string message)
	{
		var rule = RuleBySelector(host, selector);
		if (rule is null)
		{
			message = $"There is no scheduled employment rule matching {selector.ColourCommand()}.";
			return false;
		}

		try
		{
			if (!host.TaskBoard.CancelScheduledRule(rule, actor, reason))
			{
				message = $"Could not cancel scheduled rule {rule.Name.ColourName()}.";
				return false;
			}

			message = $"You cancel scheduled employment rule {rule.Name.ColourName()}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public string RenderPredicates(ICharacter actor, IEmploymentHost host, string? selector = null)
	{
		if (!string.IsNullOrWhiteSpace(selector))
		{
			var predicate = PredicateBySelector(host, selector);
			if (predicate is null)
			{
				return $"There is no scheduled condition predicate matching {selector.ColourCommand()}.";
			}

			var sb = new StringBuilder();
			sb.AppendLine($"Scheduled Condition Predicate - {predicate.Name.ColourName()}".GetLineWithTitle(actor,
				Telnet.Cyan, Telnet.BoldWhite));
			sb.AppendLine($"Expression: {DescribeConditionExpression(predicate.ConditionExpression, predicate.Conditions.ToList()).ColourCommand()}");
			sb.AppendLine($"Condition Authority: {DescribeAuthority(predicate.RequiredAuthority.Authorities)}");
			AppendConditionList(sb, actor, host, predicate.Conditions.ToList());
			return sb.ToString();
		}

		var predicates = host.TaskBoard.ConditionPredicates.OrderBy(x => x.Name).ToList();
		var list = new StringBuilder();
		list.AppendLine("Scheduled Condition Predicates".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		if (!predicates.Any())
		{
			list.AppendLine("\tNone");
			return list.ToString();
		}

		for (var i = 0; i < predicates.Count; i++)
		{
			list.AppendLine(
				$"\t#{(i + 1).ToString("N0", actor)} - {predicates[i].Name.ColourName()} - {predicates[i].Conditions.Count.ToString("N0", actor).ColourValue()} condition{(predicates[i].Conditions.Count == 1 ? string.Empty : "s")}");
		}

		return list.ToString();
	}

	public bool TryCreatePredicateFromDraft(ICharacter actor, IEmploymentHost host, string name, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			message = "What name should this scheduled condition predicate use?";
			return false;
		}

		try
		{
			var predicate = host.TaskBoard.CreateConditionPredicate(name.Trim(), draft.Conditions,
				draft.ConditionExpression, actor);
			message = $"You create scheduled condition predicate {predicate.Name.ColourName()}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryCopyPredicateToDraft(ICharacter actor, IEmploymentHost host, string selector, string? newName,
		out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var predicate = PredicateBySelector(host, selector);
		if (predicate is null)
		{
			message = $"There is no scheduled condition predicate matching {selector.ColourCommand()}.";
			return false;
		}

		var name = string.IsNullOrWhiteSpace(newName) ? $"{predicate.Name} rule" : newName.Trim();
		RemoveDraft(actor, host);
		actor.AddEffect(new EmploymentScheduledRuleDraftEffect(actor,
			new EmploymentScheduledRuleDraft(host, name, TimeSpan.FromHours(1), predicate.Conditions,
				predicate.ConditionExpression, [])));
		message = $"You copy scheduled condition predicate {predicate.Name.ColourName()} into a rule draft named {name.ColourName()}.";
		return true;
	}

	public bool TryCancelPredicate(ICharacter actor, IEmploymentHost host, string selector, string reason,
		out string message)
	{
		var predicate = PredicateBySelector(host, selector);
		if (predicate is null)
		{
			message = $"There is no scheduled condition predicate matching {selector.ColourCommand()}.";
			return false;
		}

		try
		{
			if (!host.TaskBoard.CancelConditionPredicate(predicate, actor, reason))
			{
				message = $"Could not cancel scheduled condition predicate {predicate.Name.ColourName()}.";
				return false;
			}

			message = $"You cancel scheduled condition predicate {predicate.Name.ColourName()}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public string RenderTemplates(ICharacter actor, IEmploymentHost host, string? selector = null)
	{
		if (!string.IsNullOrWhiteSpace(selector))
		{
			var template = TemplateBySelector(host, selector);
			if (template is null)
			{
				return $"There is no scheduled rule template matching {selector.ColourCommand()}.";
			}

			var sb = new StringBuilder();
			sb.AppendLine($"Scheduled Rule Template - {template.Name.ColourName()}".GetLineWithTitle(actor,
				Telnet.Cyan, Telnet.BoldWhite));
			sb.AppendLine($"Idempotency Key Pattern: {template.IdempotencyKeyPattern.ColourValue()}");
			sb.AppendLine($"Cooldown: {template.Cooldown.Describe(actor).ColourValue()}");
			sb.AppendLine($"Expression: {DescribeConditionExpression(template.ConditionExpression, template.Conditions.ToList()).ColourCommand()}");
			sb.AppendLine($"Required Authority: {DescribeAuthority(template.RequiredAuthority.Authorities)}");
			AppendConditionList(sb, actor, host, template.Conditions.ToList());
			sb.AppendLine();
			AppendStepList(sb, actor, template.ActionPlan.Steps);
			return sb.ToString();
		}

		var templates = host.TaskBoard.ScheduledRuleTemplates.OrderBy(x => x.Name).ToList();
		var list = new StringBuilder();
		list.AppendLine("Scheduled Rule Templates".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		if (!templates.Any())
		{
			list.AppendLine("\tNone");
			return list.ToString();
		}

		for (var i = 0; i < templates.Count; i++)
		{
			list.AppendLine(
				$"\t#{(i + 1).ToString("N0", actor)} - {templates[i].Name.ColourName()} - {templates[i].Conditions.Count.ToString("N0", actor).ColourValue()} condition{(templates[i].Conditions.Count == 1 ? string.Empty : "s")} - {templates[i].ActionPlan.Steps.Count.ToString("N0", actor).ColourValue()} step{(templates[i].ActionPlan.Steps.Count == 1 ? string.Empty : "s")}");
		}

		return list.ToString();
	}

	public bool TrySaveTemplateFromDraft(ICharacter actor, IEmploymentHost host, string name, out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active scheduled employment rule draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			name = draft.Name;
		}

		if (!draft.Conditions.Any() || !draft.Steps.Any())
		{
			message = "Scheduled rule templates need at least one condition and one action step.";
			return false;
		}

		try
		{
			var template = host.TaskBoard.CreateScheduledRuleTemplate(name.Trim(), draft.IdempotencyKey,
				draft.Conditions, draft.ConditionExpression, draft.ToActionPlan(), draft.Cooldown, actor);
			message = $"You save scheduled rule template {template.Name.ColourName()}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryDraftFromTemplate(ICharacter actor, IEmploymentHost host, string selector, string? newName,
		out string message)
	{
		if (!TryRequireCreateRules(actor, host, out message))
		{
			return false;
		}

		var template = TemplateBySelector(host, selector);
		if (template is null)
		{
			message = $"There is no scheduled rule template matching {selector.ColourCommand()}.";
			return false;
		}

		var name = string.IsNullOrWhiteSpace(newName) ? template.Name : newName.Trim();
		RemoveDraft(actor, host);
		var draft = new EmploymentScheduledRuleDraft(host, name, template.Cooldown, template.Conditions,
			template.ConditionExpression, template.ActionPlan.Steps);
		draft.SetKey(template.IdempotencyKeyPattern);
		actor.AddEffect(new EmploymentScheduledRuleDraftEffect(actor,
			draft));
		message = $"You start a scheduled rule draft from template {template.Name.ColourName()} named {name.ColourName()}.";
		return true;
	}

	public bool TryCancelTemplate(ICharacter actor, IEmploymentHost host, string selector, string reason,
		out string message)
	{
		var template = TemplateBySelector(host, selector);
		if (template is null)
		{
			message = $"There is no scheduled rule template matching {selector.ColourCommand()}.";
			return false;
		}

		try
		{
			if (!host.TaskBoard.CancelScheduledRuleTemplate(template, actor, reason))
			{
				message = $"Could not cancel scheduled rule template {template.Name.ColourName()}.";
				return false;
			}

			message = $"You cancel scheduled rule template {template.Name.ColourName()}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	private static bool TrySetRuleStatus(ICharacter actor, IEmploymentHost host, string selector, string reason,
		bool pause, out string message)
	{
		if (!TryRequireModifyRules(actor, host, out message))
		{
			return false;
		}

		var rule = RuleBySelector(host, selector);
		if (rule is null)
		{
			message = $"There is no scheduled employment rule matching {selector.ColourCommand()}.";
			return false;
		}

		if (pause && rule.Status == EmploymentScheduledRuleStatus.Paused)
		{
			message = $"Scheduled rule {rule.Name.ColourName()} is already paused.";
			return true;
		}

		if (!pause && rule.Status == EmploymentScheduledRuleStatus.Active)
		{
			message = $"Scheduled rule {rule.Name.ColourName()} is already active.";
			return true;
		}

		try
		{
			var changed = pause
				? host.TaskBoard.PauseScheduledRule(rule, actor, reason)
				: host.TaskBoard.ResumeScheduledRule(rule, actor, reason);
			if (!changed)
			{
				message = $"Could not {(pause ? "pause" : "resume")} scheduled rule {rule.Name.ColourName()}.";
				return false;
			}

			message = pause
				? $"You pause scheduled employment rule {rule.Name.ColourName()}."
				: $"You resume scheduled employment rule {rule.Name.ColourName()}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryParseCondition(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		if (input.IsFinished)
		{
			message = "Which scheduled rule condition do you want to add?";
			return false;
		}

		var conditionKey = input.PopSpeech().CollapseString().ToLowerInvariant();
		var definition = EmploymentConditionCatalog.Get(conditionKey);
		return (definition?.Key ?? conditionKey) switch
		{
			"manual" => TryParseManual(input, out condition, out message),
			"time" => TryParseTime(input, out condition, out message),
			"item" => TryParseItem(actor, input, out condition, out message),
			"commodity" => TryParseCommodityThreshold(actor, input, out condition, out message),
			"stock" => TryParseStock(host, input, out condition, out message),
			"account" => TryParseAccount(host, input, out condition, out message),
			"shopaccount" => TryParseShopAccount(actor, input, out condition, out message),
			"float" => TryParseFloat(actor, host, input, out condition, out message),
			"tax" => TryParseTax(host, input, out condition, out message),
			"marketprice" => TryParseMarketPrice(actor, host, input, out condition, out message),
			"payroll" => TryParsePayroll(host, input, out condition, out message),
			"staffing" => TryParseStaffing(input, out condition, out message),
			"weather" => TryParseWeather(input, out condition, out message),
			_ => UnknownCondition(conditionKey, out condition, out message)
		};
	}

	public static string DescribeCondition(IEmploymentTaskCondition condition, ICharacter actor, IEmploymentHost host)
	{
		return condition switch
		{
			ManualOrderCondition manual =>
				$"manual trigger {manual.Key.ColourCommand()}",
			TimeWindowCondition time =>
				$"host time from {FormatClock(time.EarliestTime).ColourValue()} to {FormatClock(time.LatestTime).ColourValue()}",
			StockThresholdCondition stock =>
				$"stock {DescribeStockKey(stock.StockKey, host).ColourName()} {(stock.BelowThreshold ? "below" : "at least").ColourCommand()} {stock.Threshold.ToString("N0", actor).ColourValue()}",
			AccountBalanceCondition account =>
				$"account {DescribeAccountKey(account.AccountKey).ColourName()} {(account.BelowThreshold ? "below" : "at least").ColourCommand()} {account.Threshold.ToString("N2", actor).ColourValue()}",
			ItemThresholdCondition item =>
				$"items matching {ItemThresholdCondition.DescribeKey(item.ItemKey).ColourName()} {(item.BelowThreshold ? "below" : "at least").ColourCommand()} {item.Threshold.ToString("N0", actor).ColourValue()}",
			CommodityThresholdCondition commodity =>
				$"commodity {CommodityThresholdCondition.DescribeKey(commodity.CommodityKey).ColourName()} {(commodity.BelowThreshold ? "below" : "at least").ColourCommand()} {commodity.ThresholdWeight.ToString("N2", actor).ColourValue()} weight",
			ShopAccountOwingCondition owing =>
				$"shop account {ShopAccountOwingCondition.DescribeKey(owing.AccountKey, host).ColourName()} owing {(owing.AboveThreshold ? "more than" : "no more than").ColourCommand()} {DescribeConditionAmount(host, owing.Threshold)}",
			ShopFloatThresholdCondition shopFloat =>
				$"shop float in {ShopFloatThresholdCondition.DescribeKey(shopFloat.FloatKey).ColourName()} {(shopFloat.BelowThreshold ? "below" : "at least").ColourCommand()} {DescribeConditionAmount(host, shopFloat.Threshold)}",
			TaxOwingCondition tax =>
				$"supported host taxes owing {(tax.AboveThreshold ? "above" : "below").ColourCommand()} {DescribeConditionAmount(host, tax.Threshold)}",
			MarketPriceCondition marketPrice =>
				$"market price {MarketPriceCondition.DescribeKey(marketPrice.PriceKey, host).ColourName()} {(marketPrice.AboveThreshold ? "above" : "below").ColourCommand()} {marketPrice.Threshold.ToString("N2", actor).ColourValue()}",
			PayrollLiabilityCondition payroll =>
				$"payroll {PayrollLiabilityCondition.DescribeMetric(payroll.Metric).ColourName()} {(payroll.AboveThreshold ? "above" : "below").ColourCommand()} {DescribePayrollThreshold(host, payroll)}",
			StaffingLevelCondition staffing =>
				$"staffing {StaffingLevelCondition.DescribeKey(staffing.StaffingKey).ColourName()} {(staffing.BelowThreshold ? "below" : "at least").ColourCommand()} {staffing.Threshold.ToString("N0", actor).ColourValue()}",
			WeatherLevelCondition weather =>
				$"weather begins as {WeatherLevelCondition.DescribeKey(weather.WeatherKey).ColourName()}",
			_ => condition.ConditionType.DescribeEnum().ColourName()
		};
	}

	public static string DescribeConditionExpression(EmploymentConditionExpression? expression,
		IReadOnlyList<IEmploymentTaskCondition> conditions)
	{
		return EmploymentConditionExpressionEvaluator.Describe(expression, conditions);
	}

	public static void AppendConditionList(StringBuilder sb, ICharacter actor, IEmploymentHost host,
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
			sb.AppendLine($"\t#{(i + 1).ToString("N0", actor)} - {DescribeCondition(condition, actor, host)}");
			sb.AppendLine($"\t\tType: {condition.ConditionType.DescribeEnum().ColourName()} | Authority: {DescribeAuthority(condition.RequiredAuthority.Authorities)}");
		}
	}

	private static bool TryParseManual(StringStack input, out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		var key = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(key))
		{
			message = $"Manual conditions use the syntax: {"tasks rule condition manual <key>".ColourCommand()}";
			return false;
		}

		ConsumeRemaining(input);
		condition = new ManualOrderCondition(key);
		message = string.Empty;
		return true;
	}

	private static bool TryParseTime(StringStack input, out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		if (!input.IsFinished && input.PeekSpeech().EqualTo("between"))
		{
			input.PopSpeech();
		}

		if (input.IsFinished || !TryParseClockTime(input.PopSpeech(), out var earliest) ||
		    input.IsFinished || !IsAny(input.PopSpeech(), "to", "and") ||
		    input.IsFinished || !TryParseClockTime(input.PopSpeech(), out var latest))
		{
			message = $"Time conditions use the syntax: {"tasks rule condition time <HH:mm> to <HH:mm>".ColourCommand()} or {"tasks rule condition between <HH:mm> and <HH:mm>".ColourCommand()}";
			return false;
		}

		condition = new TimeWindowCondition(earliest, latest);
		message = string.Empty;
		return true;
	}

	private static bool TryParseItem(ICharacter actor, StringStack input, out IEmploymentTaskCondition condition,
		out string message)
	{
		condition = null!;
		if (!EmploymentTaskAuthoringService.TryParseItemSelector(actor, input, "item condition target",
			    out var itemSelector, out message) || itemSelector is null)
		{
			return false;
		}

		if (input.IsFinished || !IsAny(input.PopSpeech(), "in", "at", "room"))
		{
			message = $"Item conditions use the syntax: {"tasks rule condition item <prototype|*item|&tag|keyword> in <here|cell id> [container <prototype|*item|&tag|keyword>] below|atleast <quantity>".ColourCommand()}";
			return false;
		}

		if (input.IsFinished || !TryResolveLocation(actor, input.PopSpeech(), out var location, out message))
		{
			return false;
		}

		EmploymentItemSelector? containerSelector = null;
		if (!input.IsFinished && IsAny(input.PeekSpeech(), "container", "inside", "incontainer"))
		{
			input.PopSpeech();
			if (!EmploymentTaskAuthoringService.TryParseItemSelector(actor, input, "item condition container",
				    out containerSelector, out message))
			{
				return false;
			}
		}

		if (!TryParseComparison(input, out var below, out message))
		{
			return false;
		}

		if (input.IsFinished || !int.TryParse(input.PopSpeech(), out var threshold) || threshold < 0)
		{
			message = "What non-negative item count threshold should this condition use?";
			return false;
		}

		condition = new ItemThresholdCondition(
			ItemThresholdCondition.CreateKey(itemSelector, location.Id, containerSelector),
			threshold,
			below);
		message = string.Empty;
		return true;
	}

	private static bool TryParseCommodityThreshold(ICharacter actor, StringStack input,
		out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		message = string.Empty;
		if (input.IsFinished ||
		    !TryParseCommodityDescriptor(actor, input.PopSpeech(), out var material, out var tag,
			    out var characteristics, out message))
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				message = $"Commodity conditions use the syntax: {"tasks rule condition commodity <material[|tag][|name=value...]> in <here|cell id> [container <prototype|*item|&tag|keyword>] below|atleast <weight>".ColourCommand()}";
			}

			return false;
		}

		if (input.IsFinished || !IsAny(input.PopSpeech(), "in", "at", "room"))
		{
			message = $"Commodity conditions use the syntax: {"tasks rule condition commodity <material[|tag][|name=value...]> in <here|cell id> [container <prototype|*item|&tag|keyword>] below|atleast <weight>".ColourCommand()}";
			return false;
		}

		if (input.IsFinished || !TryResolveLocation(actor, input.PopSpeech(), out var location, out message))
		{
			return false;
		}

		EmploymentItemSelector? containerSelector = null;
		if (!input.IsFinished && IsAny(input.PeekSpeech(), "container", "inside", "incontainer"))
		{
			input.PopSpeech();
			if (!EmploymentTaskAuthoringService.TryParseItemSelector(actor, input, "commodity condition container",
				    out containerSelector, out message))
			{
				return false;
			}
		}

		if (!TryParseComparison(input, out var below, out message))
		{
			return false;
		}

		if (input.IsFinished ||
		    !decimal.TryParse(input.PopSpeech(), NumberStyles.Number, actor, out var threshold) ||
		    threshold < 0.0M)
		{
			message = "What non-negative commodity weight threshold should this condition use?";
			return false;
		}

		condition = new CommodityThresholdCondition(
			CommodityThresholdCondition.CreateKey(material, tag, characteristics, location.Id, containerSelector),
			threshold,
			below);
		message = string.Empty;
		return true;
	}

	private static bool TryParseCommodityDescriptor(ICharacter actor, string descriptor, out string material,
		out string? tag, out IReadOnlyDictionary<string, string> characteristics, out string message)
	{
		material = string.Empty;
		tag = null;
		var parsedCharacteristics = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		characteristics = parsedCharacteristics;
		message = string.Empty;
		var parts = descriptor.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0]))
		{
			message = "Which commodity material should this condition inspect?";
			return false;
		}

		material = parts[0].Trim();
		foreach (var part in parts.Skip(1))
		{
			var index = part.IndexOf('=');
			if (index > 0 && index < part.Length - 1)
			{
				parsedCharacteristics[part[..index].Trim()] = part[(index + 1)..].Trim();
				continue;
			}

			if (index >= 0)
			{
				message = $"Commodity variables must use the syntax {"name=value".ColourCommand()}.";
				return false;
			}

			if (!string.IsNullOrWhiteSpace(tag))
			{
				message = "Commodity conditions can specify only one tag.";
				return false;
			}

			var tagSelector = part.StartsWith('&') ? part[1..] : part;
			var frameworkTag = actor.Gameworld?.Tags.GetByIdOrName(tagSelector);
			if (frameworkTag is null)
			{
				message = $"There is no commodity tag matching {(part.StartsWith('&') ? part : $"&{part}").ColourCommand()}.";
				return false;
			}

			tag = frameworkTag.Name;
		}

		return true;
	}

	private static bool TryParseStock(IEmploymentHost host, StringStack input, out IEmploymentTaskCondition condition,
		out string message)
	{
		condition = null!;
		if (input.IsFinished)
		{
			message = $"Stock conditions use the syntax: {"tasks rule condition stock merch <id|name> below|atleast <quantity>".ColourCommand()}";
			return false;
		}

		var mode = input.PopSpeech().CollapseString().ToLowerInvariant();
		string key;
		switch (mode)
		{
			case "key":
				if (input.IsFinished)
				{
					message = "Which stock key do you want to test?";
					return false;
				}

				key = $"key:{input.PopSpeech()}";
				break;
			case "merch":
			case "merchandise":
				var selectorTokens = PopTokensUntilComparison(input).ToList();
				var selector = string.Join(" ", selectorTokens).Trim();
				if (string.IsNullOrWhiteSpace(selector))
				{
					message = "Which merchandise record should this stock condition inspect?";
					return false;
				}

				if (host is not IShop shop)
				{
					message = $"{host.EmploymentHostName.ColourName()} is not a shop and cannot use merchandise stock conditions.";
					return false;
				}

				var merchandise = long.TryParse(selector, out var id)
					? shop.Merchandises.FirstOrDefault(x => x.Id == id)
					: shop.Merchandises.FirstOrDefault(x => x.Name.EqualTo(selector));
				if (merchandise is null)
				{
					message = $"There is no merchandise record matching {selector.ColourCommand()}.";
					return false;
				}

				key = $"merch:{merchandise.Id}";
				break;
			default:
				message = $"Stock conditions use {"stock merch <id|name>".ColourCommand()} or {"stock key <key>".ColourCommand()}.";
				return false;
		}

		if (!TryParseComparison(input, out var below, out message))
		{
			return false;
		}

		if (input.IsFinished || !int.TryParse(input.PopSpeech(), out var threshold) || threshold < 0)
		{
			message = "What non-negative stock threshold should this condition use?";
			return false;
		}

		condition = new StockThresholdCondition(key, threshold, below);
		message = string.Empty;
		return true;
	}

	private static bool TryParseAccount(IEmploymentHost host, StringStack input,
		out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		if (input.IsFinished)
		{
			message = $"Account conditions use the syntax: {"tasks rule condition account cash|bank|available|key <key> below|atleast <amount>".ColourCommand()}";
			return false;
		}

		var source = input.PopSpeech().CollapseString().ToLowerInvariant();
		var keyBacked = source.EqualTo("key");
		var key = source switch
		{
			"key" when !input.IsFinished => $"key:{input.PopSpeech()}",
			"cash" or "virtualcash" or "hostcash" => "cash",
			"bank" or "bankaccount" => "bank",
			"available" or "availablefunds" or "total" => "available",
			_ => string.Empty
		};
		if (string.IsNullOrWhiteSpace(key))
		{
			message = $"Account conditions use {"account cash".ColourCommand()}, {"account bank".ColourCommand()}, {"account available".ColourCommand()}, or {"account key <key>".ColourCommand()}.";
			return false;
		}

		if (!TryParseComparison(input, out var below, out message))
		{
			return false;
		}

		var amountText = input.SafeRemainingArgument;
		decimal amount = 0.0M;
		var currency = EmploymentTaskAuthoringService.ResolveHostCurrency(host);
		var parsed = currency is not null
			? currency.TryGetBaseCurrency(amountText, out amount)
			: keyBacked && decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out amount);
		if (string.IsNullOrWhiteSpace(amountText) || !parsed || amount < 0.0M)
		{
			message = currency is null && !keyBacked
				? $"{host.EmploymentHostName.ColourName()} does not expose a currency for account conditions."
				: $"What non-negative amount{(currency is null ? string.Empty : $" in {currency.Name.ColourName()}")} should this account condition use?";
			return false;
		}

		ConsumeRemaining(input);
		condition = new AccountBalanceCondition(key, amount, below);
		message = string.Empty;
		return true;
	}

	private static bool TryParseShopAccount(ICharacter actor, StringStack input,
		out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		var shopTokens = PopTokensUntil(input, "account").ToList();
		if (!shopTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("account"))
		{
			message = $"Shop account conditions use the syntax: {"tasks rule condition shopaccount <shop id|name> account <account id|name> owing above <amount>".ColourCommand()}";
			return false;
		}

		var shopSelector = string.Join(" ", shopTokens);
		var shop = actor.Gameworld?.Shops.GetByIdOrName(shopSelector);
		if (shop is null)
		{
			message = $"There is no shop matching {shopSelector.ColourCommand()}.";
			return false;
		}

		var accountTokens = PopTokensUntil(input, "owing").ToList();
		if (!accountTokens.Any())
		{
			accountTokens = PopTokensUntilComparison(input).ToList();
		}

		if (!input.IsFinished && input.PeekSpeech().EqualTo("owing"))
		{
			input.PopSpeech();
		}

		var accountSelector = string.Join(" ", accountTokens).Trim();
		if (string.IsNullOrWhiteSpace(accountSelector))
		{
			message = "Which line-of-credit account should this shop-account condition inspect?";
			return false;
		}

		var account = long.TryParse(accountSelector, out var accountId)
			? shop.LineOfCreditAccounts.FirstOrDefault(x => x.Id == accountId)
			: shop.LineOfCreditAccounts.FirstOrDefault(x => x.AccountName.EqualTo(accountSelector)) ??
			  shop.LineOfCreditAccounts.FirstOrDefault(x =>
				  x.AccountName.StartsWith(accountSelector, StringComparison.InvariantCultureIgnoreCase));
		if (account is null)
		{
			message = $"{shop.Name.ColourName()} does not have a line-of-credit account matching {accountSelector.ColourCommand()}.";
			return false;
		}

		if (!TryParseComparison(input, out var below, out message))
		{
			return false;
		}

		var amountText = input.SafeRemainingArgument;
		if (string.IsNullOrWhiteSpace(amountText) ||
		    !account.Currency.TryGetBaseCurrency(amountText, out var amount) ||
		    amount < 0.0M)
		{
			message = $"What non-negative amount in {account.Currency.Name.ColourName()} should this shop-account condition use?";
			return false;
		}

		ConsumeRemaining(input);
		condition = new ShopAccountOwingCondition(ShopAccountOwingCondition.CreateKey(shop, account), amount, !below);
		message = string.Empty;
		return true;
	}

	private static bool TryParseFloat(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		if (host is not IShop shop)
		{
			message = $"{host.EmploymentHostName.ColourName()} is not a shop and cannot use cash-register float conditions.";
			return false;
		}

		EmploymentItemSelector? registerSelector = null;
		if (!input.IsFinished && IsAny(input.PeekSpeech(), "register", "till", "cashregister"))
		{
			input.PopSpeech();
			if (!EmploymentTaskAuthoringService.TryParseItemSelector(actor, input, "cash register",
				    out registerSelector, out message))
			{
				return false;
			}
		}
		else if (!input.IsFinished && input.PeekSpeech().EqualTo("all"))
		{
			input.PopSpeech();
		}

		if (!TryParseComparison(input, out var below, out message))
		{
			return false;
		}

		var amountText = input.SafeRemainingArgument;
		if (string.IsNullOrWhiteSpace(amountText) ||
		    !shop.Currency.TryGetBaseCurrency(amountText, out var amount) ||
		    amount < 0.0M)
		{
			message = $"What non-negative amount in {shop.Currency.Name.ColourName()} should this cash-register float condition use?";
			return false;
		}

		ConsumeRemaining(input);
		condition = new ShopFloatThresholdCondition(ShopFloatThresholdCondition.CreateKey(registerSelector), amount, below);
		message = string.Empty;
		return true;
	}

	private static bool TryParseTax(IEmploymentHost host, StringStack input,
		out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		if (!input.IsFinished && input.PeekSpeech().EqualTo("owing"))
		{
			input.PopSpeech();
		}

		if (!TryParseComparison(input, out var below, out message))
		{
			return false;
		}

		var currency = EmploymentTaskAuthoringService.ResolveHostCurrency(host);
		var amountText = input.SafeRemainingArgument;
		if (currency is null ||
		    string.IsNullOrWhiteSpace(amountText) ||
		    !currency.TryGetBaseCurrency(amountText, out var amount) ||
		    amount < 0.0M)
		{
			message = currency is null
				? $"{host.EmploymentHostName.ColourName()} does not expose a currency for tax conditions."
				: $"Tax conditions use the syntax: {"tasks rule condition tax owing above|below <amount>".ColourCommand()}";
			return false;
		}

		ConsumeRemaining(input);
		condition = new TaxOwingCondition(amount, !below);
		message = string.Empty;
		return true;
	}

	private static bool TryParseMarketPrice(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		if (input.IsFinished)
		{
			message = $"Market price conditions use the syntax: {"tasks rule condition marketprice merch <id|name> effective|base|multiplier|flat above|below <amount>".ColourCommand()} or {"tasks rule condition marketprice item <prototype> multiplier|flat above|below <amount>".ColourCommand()}";
			return false;
		}

		var mode = input.PopSpeech().CollapseString().ToLowerInvariant();
		string key;
		switch (mode)
		{
			case "merch":
			case "merchandise":
				if (host is not IShop shop)
				{
					message = $"{host.EmploymentHostName.ColourName()} is not a shop and cannot use merchandise market-price conditions.";
					return false;
				}

				var selectorTokens = PopTokensUntil(input, "effective", "base", "multiplier", "flat").ToList();
				var selector = string.Join(" ", selectorTokens).Trim();
				if (string.IsNullOrWhiteSpace(selector))
				{
					message = "Which merchandise record should this market-price condition inspect?";
					return false;
				}

				var merchandise = long.TryParse(selector, out var merchandiseId)
					? shop.Merchandises.FirstOrDefault(x => x.Id == merchandiseId)
					: shop.Merchandises.FirstOrDefault(x => x.Name.EqualTo(selector));
				if (merchandise is null)
				{
					message = $"There is no merchandise record matching {selector.ColourCommand()}.";
					return false;
				}

				if (input.IsFinished)
				{
					message = "Which price metric do you want to inspect: effective, base, multiplier, or flat?";
					return false;
				}

				var merchandiseMetric = input.PopSpeech().CollapseString().ToLowerInvariant();
				if (!IsAny(merchandiseMetric, "effective", "base", "multiplier", "flat"))
				{
					message = "Market-price merchandise metrics are effective, base, multiplier, or flat.";
					return false;
				}

				key = MarketPriceCondition.CreateMerchandiseKey(merchandise, merchandiseMetric);
				break;
			case "item":
			case "prototype":
			case "proto":
				if (!EmploymentTaskAuthoringService.TryParseItemSelector(actor, input, "market price item prototype",
					    out var selectorValue, out message) ||
				    selectorValue?.Kind != EmploymentItemSelectorKind.PrototypeId ||
				    selectorValue.Id is null ||
				    actor.Gameworld?.ItemProtos.Get(selectorValue.Id.Value) is not { } prototype)
				{
					message = string.IsNullOrWhiteSpace(message)
						? "Which item prototype should this market-price condition inspect?"
						: message;
					return false;
				}

				if (input.IsFinished)
				{
					message = "Item market-price conditions support multiplier or flat metrics.";
					return false;
				}

				var itemMetric = input.PopSpeech().CollapseString().ToLowerInvariant();
				if (!IsAny(itemMetric, "multiplier", "flat"))
				{
					message = "Item market-price conditions support multiplier or flat metrics.";
					return false;
				}

				key = MarketPriceCondition.CreateItemKey(prototype, itemMetric);
				break;
			default:
				message = $"Market price conditions use {"marketprice merch <id|name>".ColourCommand()} or {"marketprice item <prototype>".ColourCommand()}.";
				return false;
		}

		if (!TryParseComparison(input, out var below, out message))
		{
			return false;
		}

		if (input.IsFinished ||
		    !decimal.TryParse(input.PopSpeech(), NumberStyles.Number, actor, out var threshold) ||
		    threshold < 0.0M)
		{
			message = "What non-negative threshold should this market-price condition use?";
			return false;
		}

		condition = new MarketPriceCondition(key, threshold, !below);
		message = string.Empty;
		return true;
	}

	private static bool TryParsePayroll(IEmploymentHost host, StringStack input,
		out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		if (input.IsFinished)
		{
			message = $"Payroll conditions use the syntax: {"tasks rule condition payroll outstanding|amount|overdue above|below <threshold>".ColourCommand()}";
			return false;
		}

		var metric = PayrollLiabilityCondition.NormaliseMetric(input.PopSpeech());
		if (metric is not PayrollLiabilityCondition.OutstandingMetric and
		    not PayrollLiabilityCondition.AmountMetric and
		    not PayrollLiabilityCondition.OverdueMetric)
		{
			message = "Payroll metrics are outstanding, amount, or overdue.";
			return false;
		}

		if (!TryParseComparison(input, out var below, out message))
		{
			return false;
		}

		var thresholdText = input.SafeRemainingArgument;
		decimal threshold;
		if (metric == PayrollLiabilityCondition.AmountMetric)
		{
			var currency = EmploymentTaskAuthoringService.ResolveHostCurrency(host);
			if (currency is null ||
			    string.IsNullOrWhiteSpace(thresholdText) ||
			    !currency.TryGetBaseCurrency(thresholdText, out threshold) ||
			    threshold < 0.0M)
			{
				message = currency is null
					? $"{host.EmploymentHostName.ColourName()} does not expose a currency for payroll amount conditions."
					: $"Payroll amount conditions use the syntax: {"tasks rule condition payroll amount above|below <amount>".ColourCommand()}";
				return false;
			}
		}
		else if (string.IsNullOrWhiteSpace(thresholdText) ||
		         !decimal.TryParse(thresholdText, NumberStyles.Number, CultureInfo.InvariantCulture, out threshold) ||
		         threshold < 0.0M)
		{
			message = $"Payroll {metric} conditions require a non-negative numeric threshold.";
			return false;
		}

		ConsumeRemaining(input);
		condition = new PayrollLiabilityCondition(metric, threshold, !below);
		message = string.Empty;
		return true;
	}

	private static bool TryParseStaffing(StringStack input, out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		if (!input.IsFinished && input.PeekSpeech().EqualTo("role"))
		{
			input.PopSpeech();
		}

		if (input.IsFinished)
		{
			message = $"Staffing conditions use the syntax: {"tasks rule condition staffing role <role|any> active|open|combined below|atleast <count>".ColourCommand()}";
			return false;
		}

		var roleText = input.PopSpeech();
		EmploymentRole? role = null;
		if (!roleText.EqualTo("any"))
		{
			if (!roleText.TryParseEnum<EmploymentRole>(out var parsedRole))
			{
				message = $"There is no employment role matching {roleText.ColourCommand()}.";
				return false;
			}

			role = parsedRole;
		}

		if (input.IsFinished)
		{
			message = "Which staffing metric do you want to inspect: active, open, or combined?";
			return false;
		}

		var metric = StaffingLevelCondition.NormaliseMetric(input.PopSpeech());
		if (metric is not StaffingLevelCondition.ActiveMetric and
		    not StaffingLevelCondition.OpenMetric and
		    not StaffingLevelCondition.CombinedMetric)
		{
			message = "Staffing metrics are active, open, or combined.";
			return false;
		}

		if (!TryParseComparison(input, out var below, out message))
		{
			return false;
		}

		if (input.IsFinished || !int.TryParse(input.PopSpeech(), NumberStyles.Integer, CultureInfo.InvariantCulture,
			    out var threshold) || threshold < 0)
		{
			message = "What non-negative staffing threshold should this condition use?";
			return false;
		}

		condition = new StaffingLevelCondition(StaffingLevelCondition.CreateKey(role, metric), threshold, below);
		message = string.Empty;
		return true;
	}

	private static bool TryParseWeather(StringStack input, out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		if (input.IsFinished)
		{
			message = $"Weather conditions use the syntax: {"tasks rule condition weather precip <rain|snow|level> begins".ColourCommand()} or {"tasks rule condition weather wind <level> begins".ColourCommand()}.";
			return false;
		}

		var mode = input.PopSpeech().CollapseString().ToLowerInvariant();
		if (mode is "rain" or "raining" or "snow" or "snowing")
		{
			ConsumeWeatherTrigger(input);
			condition = new WeatherLevelCondition(WeatherLevelCondition.CreatePrecipitationKey(mode));
			message = string.Empty;
			return true;
		}

		var targetTokens = PopRemainingTokens(input)
		                   .Where(x => !IsWeatherTriggerOrFiller(x))
		                   .ToList();
		switch (mode)
		{
			case "precip":
			case "precipitation":
			case "weather":
				if (!TryParsePrecipitationTarget(targetTokens, out var precipitationSelector))
				{
					message = $"Which precipitation level should this weather condition watch for? Use {"rain".ColourCommand()}, {"snow".ColourCommand()}, or a precipitation level such as {"LightRain".ColourCommand()}.";
					return false;
				}

				condition = new WeatherLevelCondition(WeatherLevelCondition.CreatePrecipitationKey(precipitationSelector));
				message = string.Empty;
				return true;
			case "wind":
				if (!TryParseWindTarget(targetTokens, out var wind))
				{
					message = $"Which wind level should this weather condition watch for? Use a wind level such as {"GaleWind".ColourCommand()} or {"gale force".ColourCommand()}.";
					return false;
				}

				condition = new WeatherLevelCondition(WeatherLevelCondition.CreateWindKey(wind));
				message = string.Empty;
				return true;
			default:
				message = $"Weather conditions use {"weather precip <rain|snow|level> begins".ColourCommand()} or {"weather wind <level> begins".ColourCommand()}.";
				return false;
		}
	}

	private static bool UnknownCondition(string conditionKey, out IEmploymentTaskCondition condition, out string message)
	{
		condition = null!;
		message = $"There is no scheduled employment rule condition named {conditionKey.ColourCommand()}. Try {"tasks conditions".ColourCommand()} to see available conditions.";
		return false;
	}

	private static IEmploymentTaskContext BuildEvaluationContext(IEmploymentHost host, string? manualKey)
	{
		var context = new EmploymentTaskContext(host);
		if (!string.IsNullOrWhiteSpace(manualKey))
		{
			context.SetManualOrder(manualKey.Trim(), true);
		}

		return context;
	}

	private static IEmploymentScheduledTaskRule? RuleBySelector(IEmploymentHost host, string selector)
	{
		var rules = host.TaskBoard.ScheduledRules
		                .OrderBy(x => x.Name)
		                .ToList();
		if (!rules.Any())
		{
			return null;
		}

		if (TryParseCommandNumber(selector, out var number))
		{
			if (number >= 1 && number <= rules.Count)
			{
				return rules[(int)number - 1];
			}

			return rules.FirstOrDefault(x => x.Id.ToString("D").StartsWith(number.ToString(), StringComparison.InvariantCultureIgnoreCase));
		}

		return rules.FirstOrDefault(x => x.Name.EqualTo(selector)) ??
		       rules.FirstOrDefault(x => x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
	}

	private static IEmploymentConditionPredicate? PredicateBySelector(IEmploymentHost host, string selector)
	{
		var predicates = host.TaskBoard.ConditionPredicates
		                     .OrderBy(x => x.Name)
		                     .ToList();
		if (!predicates.Any())
		{
			return null;
		}

		if (TryParseCommandNumber(selector, out var number))
		{
			return number >= 1 && number <= predicates.Count ? predicates[(int)number - 1] : null;
		}

		return predicates.FirstOrDefault(x => x.Name.EqualTo(selector)) ??
		       predicates.FirstOrDefault(x => x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
	}

	private static IEmploymentScheduledRuleTemplate? TemplateBySelector(IEmploymentHost host, string selector)
	{
		var templates = host.TaskBoard.ScheduledRuleTemplates
		                    .OrderBy(x => x.Name)
		                    .ToList();
		if (!templates.Any())
		{
			return null;
		}

		if (TryParseCommandNumber(selector, out var number))
		{
			return number >= 1 && number <= templates.Count ? templates[(int)number - 1] : null;
		}

		return templates.FirstOrDefault(x => x.Name.EqualTo(selector)) ??
		       templates.FirstOrDefault(x => x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
	}

	internal static bool TryParseConditionExpression(string text, IReadOnlyList<IEmploymentTaskCondition> conditions,
		IEmploymentTaskBoard board, out EmploymentConditionExpression expression, out string message)
	{
		expression = null!;
		var parser = new ConditionExpressionParser(TokeniseExpression(text));
		if (!parser.TryParse(out expression, out message))
		{
			return false;
		}

		if (!EmploymentConditionExpressionEvaluator.Validate(expression, conditions, board, out message))
		{
			return false;
		}

		return true;
	}

	private static IReadOnlyList<string> TokeniseExpression(string text)
	{
		var tokens = new List<string>();
		var current = new StringBuilder();
		foreach (var ch in text)
		{
			if (char.IsWhiteSpace(ch))
			{
				Flush();
				continue;
			}

			if (ch is '(' or ')')
			{
				Flush();
				tokens.Add(ch.ToString());
				continue;
			}

			current.Append(ch);
		}

		Flush();
		return tokens;

		void Flush()
		{
			if (current.Length == 0)
			{
				return;
			}

			tokens.Add(current.ToString());
			current.Clear();
		}
	}

	private sealed class ConditionExpressionParser
	{
		private readonly IReadOnlyList<string> _tokens;
		private int _position;

		public ConditionExpressionParser(IReadOnlyList<string> tokens)
		{
			_tokens = tokens;
		}

		public bool TryParse(out EmploymentConditionExpression expression, out string message)
		{
			expression = null!;
			if (!_tokens.Any())
			{
				message = "Condition expressions cannot be blank.";
				return false;
			}

			if (!TryParseOr(out expression, out message))
			{
				return false;
			}

			if (_position < _tokens.Count)
			{
				message = $"Unexpected expression token {_tokens[_position].ColourCommand()}.";
				return false;
			}

			return true;
		}

		private bool TryParseOr(out EmploymentConditionExpression expression, out string message)
		{
			if (!TryParseAnd(out expression, out message))
			{
				return false;
			}

			var children = new List<EmploymentConditionExpression> { expression };
			while (Peek("or"))
			{
				_position++;
				if (!TryParseAnd(out var right, out message))
				{
					return false;
				}

				children.Add(right);
			}

			expression = children.Count == 1 ? children[0] : EmploymentConditionExpression.Any(children);
			return true;
		}

		private bool TryParseAnd(out EmploymentConditionExpression expression, out string message)
		{
			if (!TryParseUnary(out expression, out message))
			{
				return false;
			}

			var children = new List<EmploymentConditionExpression> { expression };
			while (Peek("and"))
			{
				_position++;
				if (!TryParseUnary(out var right, out message))
				{
					return false;
				}

				children.Add(right);
			}

			expression = children.Count == 1 ? children[0] : EmploymentConditionExpression.All(children);
			return true;
		}

		private bool TryParseUnary(out EmploymentConditionExpression expression, out string message)
		{
			if (Peek("not"))
			{
				_position++;
				if (!TryParseUnary(out var child, out message))
				{
					expression = null!;
					return false;
				}

				expression = EmploymentConditionExpression.Not(child);
				return true;
			}

			return TryParsePrimary(out expression, out message);
		}

		private bool TryParsePrimary(out EmploymentConditionExpression expression, out string message)
		{
			expression = null!;
			if (_position >= _tokens.Count)
			{
				message = "The expression ended unexpectedly.";
				return false;
			}

			var token = _tokens[_position++];
			if (token.EqualTo("("))
			{
				if (!TryParseOr(out expression, out message))
				{
					return false;
				}

				if (!Peek(")"))
				{
					message = "Missing closing parenthesis in condition expression.";
					return false;
				}

				_position++;
				return true;
			}

			if (token.StartsWith("#") && int.TryParse(token[1..], out var number))
			{
				expression = EmploymentConditionExpression.Condition(number);
				message = string.Empty;
				return true;
			}

			if (token.EqualTo("condition"))
			{
				if (_position >= _tokens.Count || !int.TryParse(_tokens[_position++].TrimStart('#'), out number))
				{
					message = "Condition references use #<number> or condition <number>.";
					return false;
				}

				expression = EmploymentConditionExpression.Condition(number);
				message = string.Empty;
				return true;
			}

			if (token.StartsWith("@") && token.Length > 1)
			{
				expression = EmploymentConditionExpression.Predicate(token[1..]);
				message = string.Empty;
				return true;
			}

			message = $"Unexpected expression token {token.ColourCommand()}.";
			return false;
		}

		private bool Peek(string token)
		{
			return _position < _tokens.Count && _tokens[_position].EqualTo(token);
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

	private static void AppendConditionDefinition(StringBuilder sb, EmploymentConditionDefinition condition,
		bool detailed)
	{
		sb.AppendLine($"\t{condition.Key.ColourCommand()} - {condition.Summary}");
		sb.AppendLine($"\t\tSyntax: {condition.Syntax.ColourCommand()}");
		sb.AppendLine($"\t\tAuthority: {DescribeAuthority(condition.RequiredAuthority.Authorities)} | Type: {condition.ConditionType.DescribeEnum().ColourName()}");
		if (detailed && condition.Aliases is not null && condition.Aliases.Any())
		{
			sb.AppendLine($"\t\tAliases: {condition.Aliases.Select(x => x.ColourCommand()).ListToString()}");
		}
	}

	private static string RenderCreatedRuleSummary(ICharacter actor, IEmploymentScheduledTaskRule rule)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"You create scheduled employment rule {rule.Name.ColourName()} with {rule.Conditions.Count.ToString("N0", actor).ColourValue()} condition{(rule.Conditions.Count == 1 ? string.Empty : "s")} and {rule.ActionPlan.Steps.Count.ToString("N0", actor).ColourValue()} step{(rule.ActionPlan.Steps.Count == 1 ? string.Empty : "s")}:");
		sb.AppendLine($"Status: {rule.Status.DescribeEnum().ColourValue()} | Cooldown: {rule.Cooldown.Describe(actor).ColourValue()} | Idempotency Key: {rule.IdempotencyKey.ColourValue()}");
		AppendConditionList(sb, actor, rule.Employer, rule.Conditions.ToList());
		sb.AppendLine();
		AppendStepList(sb, actor, rule.ActionPlan.Steps);
		return sb.ToString();
	}

	private static EmploymentAuthority RequiredConditionAuthority(IEnumerable<IEmploymentTaskCondition> conditions)
	{
		return conditions.Aggregate(EmploymentAuthority.None,
			(current, condition) => current | condition.RequiredAuthority.Authorities);
	}

	private static bool TryRequireCreateRules(ICharacter actor, IEmploymentHost host, out string message)
	{
		if (actor.IsAdministrator() || host.HasAuthority(actor, EmploymentAuthority.CreateScheduledRules))
		{
			message = string.Empty;
			return true;
		}

		message = $"You do not have the delegated {EmploymentAuthority.CreateScheduledRules.DescribeEnum().ColourName()} authority for {host.EmploymentHostName.ColourName()}.";
		return false;
	}

	private static bool TryRequireModifyRules(ICharacter actor, IEmploymentHost host, out string message)
	{
		if (actor.IsAdministrator() || host.HasAuthority(actor, EmploymentAuthority.ModifyScheduledRules))
		{
			message = string.Empty;
			return true;
		}

		message = $"You do not have the delegated {EmploymentAuthority.ModifyScheduledRules.DescribeEnum().ColourName()} authority for {host.EmploymentHostName.ColourName()}.";
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

	private static EmploymentScheduledRuleDraft? DraftFor(ICharacter actor, IEmploymentHost host)
	{
		return actor.EffectsOfType<EmploymentScheduledRuleDraftEffect>()
		            .FirstOrDefault(x => x.Draft.Host.EmploymentHostType == host.EmploymentHostType && x.Draft.Host.Id == host.Id)
		            ?.Draft;
	}

	private static bool RemoveDraft(ICharacter actor, IEmploymentHost host)
	{
		return actor.RemoveAllEffects<EmploymentScheduledRuleDraftEffect>(
			x => x.Draft.Host.EmploymentHostType == host.EmploymentHostType && x.Draft.Host.Id == host.Id,
			true);
	}

	private static string EmploymentScheduledRuleDraftKey(string name)
	{
		var key = name.CollapseString().ToLowerInvariant();
		return string.IsNullOrWhiteSpace(key) ? Guid.NewGuid().ToString("N") : key;
	}

	private static bool TryResolveLocation(ICharacter actor, string token, out ICell location, out string message)
	{
		location = null!;
		if (token.EqualTo("here") || token.EqualTo("current"))
		{
			location = actor.Location;
			message = string.Empty;
			return true;
		}

		var resolved = long.TryParse(token, out var id)
			? actor.Gameworld?.Cells.Get(id)
			: actor.Gameworld?.Cells.GetByIdOrName(token);
		if (resolved is not null)
		{
			location = resolved;
			message = string.Empty;
			return true;
		}

		message = $"There is no room/cell matching {token.ColourCommand()}.";
		return false;
	}

	private static string DescribeConditionAmount(IEmploymentHost host, decimal amount)
	{
		var currency = EmploymentTaskAuthoringService.ResolveHostCurrency(host);
		return currency is null
			? amount.ToString("N2", CultureInfo.InvariantCulture).ColourValue()
			: currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
	}

	private static string DescribePayrollThreshold(IEmploymentHost host, PayrollLiabilityCondition condition)
	{
		return PayrollLiabilityCondition.NormaliseMetric(condition.Metric) == PayrollLiabilityCondition.AmountMetric
			? DescribeConditionAmount(host, condition.Threshold)
			: condition.Threshold.ToString("N2", CultureInfo.InvariantCulture).ColourValue();
	}

	private static bool TryParseComparison(StringStack input, out bool below, out string message)
	{
		below = true;
		message = string.Empty;
		if (input.IsFinished)
		{
			message = $"Use {"below".ColourCommand()} or {"atleast".ColourCommand()} for the threshold comparison.";
			return false;
		}

		var token = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (token)
		{
			case "below":
			case "under":
			case "less":
			case "lt":
			case "<":
				ConsumeOptional(input, "than");
				below = true;
				return true;
			case "atleast":
			case "minimum":
			case "above":
			case "over":
			case "greater":
			case "more":
			case "gte":
			case ">=":
			case "gt":
			case ">":
				ConsumeOptional(input, "than");
				below = false;
				return true;
			case "at" when !input.IsFinished && input.PeekSpeech().EqualTo("least"):
				input.PopSpeech();
				below = false;
				return true;
			default:
				message = $"Use {"below".ColourCommand()} or {"atleast".ColourCommand()} for the threshold comparison.";
				return false;
		}
	}

	private static bool IsComparisonToken(string text)
	{
		var token = text.CollapseString().ToLowerInvariant();
		return token is "below" or "under" or "less" or "lt" or "<" or "atleast" or "minimum" or "above" or
			"over" or "greater" or "more" or "gte" or ">=" or "gt" or ">" or "at";
	}

	private static IEnumerable<string> PopTokensUntilComparison(StringStack input)
	{
		while (!input.IsFinished && !IsComparisonToken(input.PeekSpeech()))
		{
			yield return input.PopSpeech();
		}
	}

	private static IEnumerable<string> PopTokensUntil(StringStack input, params string[] sentinels)
	{
		while (!input.IsFinished && !sentinels.Any(x => input.PeekSpeech().EqualTo(x)))
		{
			yield return input.PopSpeech();
		}
	}

	private static bool IsAny(string text, params string[] options)
	{
		return options.Any(x => text.EqualTo(x));
	}

	private static void ConsumeOptional(StringStack input, string token)
	{
		if (!input.IsFinished && input.PeekSpeech().EqualTo(token))
		{
			input.PopSpeech();
		}
	}

	private static bool TryParseClockTime(string text, out TimeSpan time)
	{
		return TimeSpan.TryParseExact(text, ["h\\:mm", "hh\\:mm"], CultureInfo.InvariantCulture, out time) &&
		       time >= TimeSpan.Zero &&
		       time < TimeSpan.FromDays(1);
	}

	private static bool TryParsePrecipitationTarget(IReadOnlyList<string> tokens, out string selector)
	{
		selector = string.Join(string.Empty, tokens).CollapseString();
		if (selector.EqualTo("rain") || selector.EqualTo("raining"))
		{
			selector = "rain";
			return true;
		}

		if (selector.EqualTo("snow") || selector.EqualTo("snowing"))
		{
			selector = "snow";
			return true;
		}

		return selector.TryParseEnum<PrecipitationLevel>(out _);
	}

	private static bool TryParseWindTarget(IReadOnlyList<string> tokens, out WindLevel wind)
	{
		wind = WindLevel.None;
		var selector = string.Join(string.Empty, tokens).CollapseString();
		if (string.IsNullOrWhiteSpace(selector))
		{
			return false;
		}

		selector = selector.ToLowerInvariant();
		switch (selector)
		{
			case "none":
				wind = WindLevel.None;
				return true;
			case "still":
				wind = WindLevel.Still;
				return true;
			case "occasionalbreeze":
			case "lightbreeze":
				wind = WindLevel.OccasionalBreeze;
				return true;
			case "breeze":
				wind = WindLevel.Breeze;
				return true;
			case "wind":
				wind = WindLevel.Wind;
				return true;
			case "strong":
			case "strongwind":
				wind = WindLevel.StrongWind;
				return true;
			case "gale":
			case "galewind":
			case "galeforce":
				wind = WindLevel.GaleWind;
				return true;
			case "hurricane":
			case "hurricanewind":
			case "hurricaneforce":
				wind = WindLevel.HurricaneWind;
				return true;
			case "maelstrom":
			case "maelstromwind":
				wind = WindLevel.MaelstromWind;
				return true;
		}

		return selector.TryParseEnum(out wind);
	}

	private static void ConsumeWeatherTrigger(StringStack input)
	{
		while (!input.IsFinished)
		{
			input.PopSpeech();
		}
	}

	private static bool IsWeatherTriggerOrFiller(string text)
	{
		var token = text.CollapseString().ToLowerInvariant();
		return token is "begins" or "begin" or "starts" or "start" or "gets" or "get" or "to" or "reaches" or
			"reach" or "becomes" or "become" or "force" or "level";
	}

	private static string FormatClock(TimeSpan time)
	{
		return $"{(int)time.TotalHours:00}:{time.Minutes:00}";
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
		if (value <= 0.0M)
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

	private static IReadOnlyList<string> PopRemainingTokens(StringStack input)
	{
		var tokens = new List<string>();
		while (!input.IsFinished)
		{
			tokens.Add(input.PopSpeech());
		}

		return tokens;
	}

	private static IReadOnlyCollection<IReadOnlyList<string>> SplitTokens(IEnumerable<string> tokens, string separator)
	{
		var results = new List<IReadOnlyList<string>>();
		var current = new List<string>();
		foreach (var token in tokens)
		{
			if (token.EqualTo(separator))
			{
				if (current.Any())
				{
					results.Add(current.ToList());
					current.Clear();
				}

				continue;
			}

			current.Add(token);
		}

		if (current.Any())
		{
			results.Add(current);
		}

		return results;
	}

	private static IReadOnlyCollection<IReadOnlyList<string>> SplitConditionTokens(IEnumerable<string> tokens)
	{
		var results = new List<IReadOnlyList<string>>();
		var current = new List<string>();
		var waitingForBetweenSecondTime = false;
		foreach (var token in tokens)
		{
			if (token.EqualTo("and") && !waitingForBetweenSecondTime)
			{
				if (current.Any())
				{
					results.Add(current.ToList());
					current.Clear();
				}

				continue;
			}

			current.Add(token);
			if (StartsBetweenCondition(current))
			{
				waitingForBetweenSecondTime = current.Count(x => TryParseClockTime(x, out _)) < 2;
			}
		}

		if (current.Any())
		{
			results.Add(current);
		}

		return results;
	}

	private static bool StartsBetweenCondition(IReadOnlyList<string> tokens)
	{
		return tokens.Count >= 1 && tokens[0].EqualTo("between") ||
		       tokens.Count >= 2 && tokens[0].EqualTo("time") && tokens[1].EqualTo("between");
	}

	private static int IndexOf(IReadOnlyList<string> tokens, string value)
	{
		for (var i = 0; i < tokens.Count; i++)
		{
			if (tokens[i].EqualTo(value))
			{
				return i;
			}
		}

		return -1;
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

	private static void ConsumeRemaining(StringStack input)
	{
		while (!input.IsFinished)
		{
			input.PopSpeech();
		}
	}

	private static string DescribeStockKey(string key, IEmploymentHost host)
	{
		if (key.StartsWith("key:", StringComparison.InvariantCultureIgnoreCase))
		{
			return $"key {key["key:".Length..]}";
		}

		if (key.StartsWith("merch:", StringComparison.InvariantCultureIgnoreCase))
		{
			var selector = key["merch:".Length..];
			if (host is IShop shop && long.TryParse(selector, out var id))
			{
				var merchandise = shop.Merchandises.FirstOrDefault(x => x.Id == id);
				return merchandise is null ? $"merchandise #{id:N0}" : $"{merchandise.Name} (#{id:N0})";
			}

			return $"merchandise {selector}";
		}

		return $"key {key}";
	}

	private static string DescribeAccountKey(string key)
	{
		return key.StartsWith("key:", StringComparison.InvariantCultureIgnoreCase)
			? $"key {key["key:".Length..]}"
			: key;
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
