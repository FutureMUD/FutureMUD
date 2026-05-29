using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Vehicles;

#nullable enable

namespace MudSharp.Commands.Helpers;

internal sealed class EmploymentTaskDraft
{
	private readonly List<IEmploymentActionStep> _steps = new();

	public EmploymentTaskDraft(IEmploymentHost host, string name)
	{
		Host = host;
		Name = name.Trim();
	}

	public IEmploymentHost Host { get; }
	public string Name { get; private set; }
	public IReadOnlyList<IEmploymentActionStep> Steps => _steps;

	public void Rename(string name)
	{
		Name = name.Trim();
	}

	public void AddStep(IEmploymentActionStep step)
	{
		_steps.Add(step);
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
}

internal sealed class EmploymentTaskAuthoringService
{
	public bool TryStartDraft(ICharacter actor, IEmploymentHost host, string name, out string message)
	{
		if (!TryRequireAssignTasks(actor, host, out message))
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			message = "What name do you want to give this employment task draft?";
			return false;
		}

		RemoveDraft(actor, host);
		actor.AddEffect(new EmploymentTaskDraftEffect(actor, new EmploymentTaskDraft(host, name)));
		message = $"You begin a new employment task draft named {name.Trim().ColourName()} for {host.EmploymentHostName.ColourName()}.";
		return true;
	}

	public bool TryRenameDraft(ICharacter actor, IEmploymentHost host, string name, out string message)
	{
		if (!TryRequireAssignTasks(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active employment task draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			message = "What new name do you want to give this employment task draft?";
			return false;
		}

		draft.Rename(name);
		message = $"You rename your employment task draft to {draft.Name.ColourName()}.";
		return true;
	}

	public bool TryRemoveStep(ICharacter actor, IEmploymentHost host, int stepNumber, out string message)
	{
		if (!TryRequireAssignTasks(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active employment task draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!draft.RemoveStep(stepNumber - 1))
		{
			message = "There is no such step in your employment task draft.";
			return false;
		}

		message = $"You remove step {stepNumber.ToString("N0", actor).ColourValue()} from your employment task draft.";
		return true;
	}

	public bool TryDiscardDraft(ICharacter actor, IEmploymentHost host, out string message)
	{
		if (!TryRequireAssignTasks(actor, host, out message))
		{
			return false;
		}

		if (!RemoveDraft(actor, host))
		{
			message = $"You do not have an active employment task draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		message = $"You discard your employment task draft for {host.EmploymentHostName.ColourName()}.";
		return true;
	}

	public bool TryAddStep(ICharacter actor, IEmploymentHost host, StringStack input, out string message)
	{
		if (!TryRequireAssignTasks(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active employment task draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!TryParseStep(actor, host, input, out var step, out message))
		{
			return false;
		}

		draft.AddStep(step);
		message = $"You add a task step to {DescribeStep(step, actor)}.";
		return true;
	}

	public bool TryFinaliseDraft(ICharacter actor, IEmploymentHost host, out IEmploymentActiveTask? task,
		out string message)
	{
		task = null;
		if (!TryRequireAssignTasks(actor, host, out message))
		{
			return false;
		}

		var draft = DraftFor(actor, host);
		if (draft is null)
		{
			message = $"You do not have an active employment task draft for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!draft.Steps.Any())
		{
			message = "You cannot finalise an employment task draft with no steps.";
			return false;
		}

		try
		{
			task = host.TaskBoard.CreateActiveTask(draft.Name, draft.ToActionPlan(), actor);
			RemoveDraft(actor, host);
			message = $"You finalise employment task {task.Name.ColourName()} with {task.ActionPlan.Steps.Count.ToString("N0", actor).ColourValue()} step{(task.ActionPlan.Steps.Count == 1 ? string.Empty : "s")}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryCreateOneShotTask(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActiveTask? task, out string message)
	{
		task = null;
		if (!TryRequireAssignTasks(actor, host, out message))
		{
			return false;
		}

		if (input.IsFinished)
		{
			message = $"One-shot task creation uses the syntax: {"tasks create <name> <action> [then <action> ...]".ColourCommand()}";
			return false;
		}

		var name = input.PopSpeech();
		if (string.IsNullOrWhiteSpace(name))
		{
			message = "What name do you want to give this employment task?";
			return false;
		}

		var stepTokens = PopRemainingTokens(input).ToList();
		if (!stepTokens.Any())
		{
			message = $"Which steps do you want this task to perform? Use {"then".ColourCommand()} between multiple actions.";
			return false;
		}

		var steps = new List<IEmploymentActionStep>();
		foreach (var actionTokens in SplitActionTokens(stepTokens))
		{
			var stack = new StringStack(string.Join(" ", actionTokens));
			if (!TryParseStep(actor, host, stack, out var step, out message))
			{
				return false;
			}

			if (!stack.IsFinished)
			{
				message = $"Could not understand the extra text {stack.SafeRemainingArgument.ColourCommand()} in the task action.";
				return false;
			}

			steps.Add(step);
		}

		if (!steps.Any())
		{
			message = "You must specify at least one task action.";
			return false;
		}

		try
		{
			var plan = new EmploymentActionPlan(steps);
			task = host.TaskBoard.CreateActiveTask(name, plan, actor);
			message = RenderCreatedTaskSummary(actor, task);
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
			return $"You do not have an active employment task draft for {host.EmploymentHostName.ColourName()}.";
		}

		var plan = draft.ToActionPlan();
		var sb = new StringBuilder();
		sb.AppendLine($"Employment task draft {draft.Name.ColourName()} for {host.EmploymentHostName.ColourName()}:");
		sb.AppendLine($"Required Authority: {plan.RequiredAuthority.Authorities.DescribeEnum().ColourName()}");
		sb.AppendLine($"Required AI Capabilities: {(plan.RequiredCapabilities.Any() ? plan.RequiredCapabilities.Select(x => x.DescribeEnum().ColourName()).ListToString() : "none".ColourValue())}");
		sb.AppendLine();
		sb.AppendLine("Steps:");
		if (!draft.Steps.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			for (var i = 0; i < draft.Steps.Count; i++)
			{
				var step = draft.Steps[i];
				sb.AppendLine($"\t{(i + 1).ToString("N0", actor)} - {DescribeStep(step, actor)}");
				sb.AppendLine($"\t\tAuthority: {step.RequiredAuthority.Authorities.DescribeEnum().ColourName()} | AI: {DescribeCapabilities(step.RequiredCapabilities)} | Catalogue: {DescribeStepCatalogueStatus(step)}");
				var warning = DescribeStepBoundaryWarning(step);
				if (!string.IsNullOrWhiteSpace(warning))
				{
					sb.AppendLine($"\t\t{warning}");
				}
			}
		}

		return sb.ToString();
	}

	public string RenderAvailableActions(ICharacter actor)
	{
		return RenderAvailableActions(actor, string.Empty);
	}

	public string RenderAvailableActions(ICharacter actor, string selector)
	{
		var sb = new StringBuilder();
		selector = selector.Trim();
		sb.AppendLine("Employment Task Actions".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine("Use these with ".ColourCommand() + "tasks step <action> ...".ColourCommand() + " while you have a draft open.");
		sb.AppendLine("You can also create and finalise a task in one command with ".ColourCommand() + "tasks create <name> <action> then <action>".ColourCommand() + ".");
		sb.AppendLine("Planning, authorisation, and safe logistics actions persist task state; audit-only actions record evidence without mutating purchasing, banking, store-account, craft, pricing, or administrative systems.");
		sb.AppendLine();

		if (!string.IsNullOrWhiteSpace(selector) && !selector.EqualTo("all"))
		{
			var definition = EmploymentActionCatalog.Get(selector);
			if (definition is not null)
			{
				AppendActionDefinition(sb, definition, detailed: true);
				return sb.ToString();
			}

			var categoryActions = EmploymentActionCatalog.ForCategory(selector);
			if (categoryActions.Any())
			{
				sb.AppendLine($"{categoryActions.First().Category.DescribeEnum().ColourName()} actions:");
				foreach (var action in categoryActions)
				{
					AppendActionDefinition(sb, action, detailed: false);
				}

				return sb.ToString();
			}

			sb.AppendLine($"There is no employment task action or category matching {selector.ColourCommand()}.");
			sb.AppendLine($"Categories: {EmploymentActionCatalog.Categories.Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return sb.ToString();
		}

		foreach (var category in EmploymentActionCatalog.Categories)
		{
			sb.AppendLine($"{category.DescribeEnum().ColourName()}:");
			foreach (var action in EmploymentActionCatalog.ForCategory(category.ToString()))
			{
				AppendActionDefinition(sb, action, detailed: false);
			}
		}

		return sb.ToString();
	}

	private static void AppendActionDefinition(StringBuilder sb, EmploymentActionDefinition action, bool detailed)
	{
		sb.AppendLine($"\t{action.Key.ColourCommand()} - {action.Status.DescribeEnum().ColourValue()} - {action.Summary}");
		sb.AppendLine($"\t\tSyntax: {action.Syntax.ColourCommand()}");
		sb.AppendLine($"\t\tAuthority: {action.RequiredAuthority.Authorities.DescribeEnum().ColourName()} | AI: {DescribeCapabilities(action.RequiredCapabilities)} | Payment Authorisation: {action.RequiresPaymentAuthorisation.ToColouredString()} | Financial: {action.IsFinancial.ToColouredString()}");
		if (action.Status == EmploymentActionCatalogStatus.AuditOnlyShell)
		{
			sb.AppendLine("\t\tAudit-only: records register or ledger evidence but does not mutate the external subsystem.".ColourError());
		}

		if (action.Status == EmploymentActionCatalogStatus.Deferred)
		{
			sb.AppendLine($"\t\tDeferred: {(action.DeferredReason ?? action.Summary).ColourError()}");
		}

		if (detailed && action.Aliases is not null && action.Aliases.Any())
		{
			sb.AppendLine($"\t\tAliases: {action.Aliases.Select(x => x.ColourCommand()).ListToString()}");
		}
	}

	private static string RenderCreatedTaskSummary(ICharacter actor, IEmploymentActiveTask task)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"You create active employment task {task.Name.ColourName()} with {task.ActionPlan.Steps.Count.ToString("N0", actor).ColourValue()} step{(task.ActionPlan.Steps.Count == 1 ? string.Empty : "s")}:");
		for (var i = 0; i < task.ActionPlan.Steps.Count; i++)
		{
			var step = task.ActionPlan.Steps[i];
			sb.AppendLine($"\t#{(i + 1).ToString("N0", actor)} - {DescribeStep(step, actor)}");
			sb.AppendLine($"\t\tAuthority: {step.RequiredAuthority.Authorities.DescribeEnum().ColourName()} | AI: {DescribeCapabilities(step.RequiredCapabilities)} | Catalogue: {DescribeStepCatalogueStatus(step)}");
			var warning = DescribeStepBoundaryWarning(step);
			if (!string.IsNullOrWhiteSpace(warning))
			{
				sb.AppendLine($"\t\t{warning}");
			}
		}

		return sb.ToString();
	}

	private static bool TryParseStep(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = "Which kind of step do you want to add?";
			return false;
		}

		var stepType = input.PopSpeech().CollapseString().ToLowerInvariant();
		var definition = EmploymentActionCatalog.Get(stepType);
		if (definition?.Status == EmploymentActionCatalogStatus.Deferred)
		{
			step = null!;
			message = $"{definition.Key.ColourCommand()} is in the employment task catalogue, but execution is deferred: {definition.DeferredReason ?? definition.Summary}";
			return false;
		}

		return (definition?.Key ?? stepType) switch
		{
			"getid" or "id" => TryParseGetId(actor, input, out step, out message),
			"gettag" or "tag" => TryParseGetTag(actor, input, out step, out message),
			"commodity" or "material" => TryParseCommodity(actor, input, out step, out message),
			"deliver" or "delivery" => TryParseDeliver(actor, input, out step, out message),
			"load" or "loaditems" => TryParseLoad(actor, input, out step, out message),
			"unload" or "unloaditems" => TryParseUnload(actor, input, out step, out message),
			"return" or "returncontainer" => TryParseReturn(actor, input, out step, out message),
			"vehicle" or "cargo" => TryParseVehicle(actor, input, out step, out message),
			"move" => TryParseMove(actor, input, out step, out message),
			"board" => TryParseBoard(input, out step, out message),
			"command" => TryParseCommand(actor, input, out step, out message),
			"purchase" => TryParsePurchase(host, input, out step, out message),
			"bankdeposit" => TryParseBankDeposit(host, input, out step, out message),
			"bankwithdraw" => TryParseBankWithdraw(host, input, out step, out message),
			"storepay" => TryParseStorePay(host, input, out step, out message),
			"craft" => TryParseCraft(input, out step, out message),
			"report" or "authorise" or "reserve" or "release" or "select" or "estimate" =>
				TryParseGenericShell(definition!.Key, input, out step, out message),
			"route" => TryParseRoute(actor, input, out step, out message),
			_ => UnknownStepType(stepType, out step, out message)
		};
	}

	private static bool UnknownStepType(string stepType, out IEmploymentActionStep step, out string message)
	{
		step = null!;
		message = $"The text {stepType.ColourCommand()} is not a supported employment task step type. Use {"tasks actions".ColourCommand()} to see supported actions.";
		return false;
	}

	private static bool TryParseGetId(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished || !int.TryParse(input.PopSpeech(), out var quantity) || quantity <= 0)
		{
			message = "How many items should this step collect?";
			return false;
		}

		var idTokens = PopTokensUntil(input, "from").ToList();
		if (!idTokens.Any() || input.IsFinished)
		{
			message = $"Get-by-id steps use the syntax: {"tasks step getid <quantity> <item prototype ids...> from <here|cell ids...>".ColourCommand()}";
			return false;
		}

		var fromToken = input.PopSpeech();
		if (!fromToken.EqualTo("from"))
		{
			message = $"Get-by-id steps must specify source locations with the {"from".ColourCommand()} keyword.";
			return false;
		}

		if (!TryParseLongs(idTokens, out var itemPrototypeIds, out message))
		{
			return false;
		}

		if (!TryParseLocations(actor, PopRemainingTokens(input), out var locations, out message))
		{
			return false;
		}

		step = new GetItemsByIdActionStep(quantity, itemPrototypeIds, locations);
		return true;
	}

	private static bool TryParseGetTag(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished || !int.TryParse(input.PopSpeech(), out var quantity) || quantity <= 0)
		{
			message = "How many tagged items should this step collect?";
			return false;
		}

		if (input.IsFinished)
		{
			message = "Which tag should this step collect?";
			return false;
		}

		var tag = input.PopSpeech();
		if (input.IsFinished || !input.PopSpeech().EqualTo("from"))
		{
			message = $"Get-by-tag steps use the syntax: {"tasks step gettag <quantity> <tag> from <here|cell ids...>".ColourCommand()}";
			return false;
		}

		if (!TryParseLocations(actor, PopRemainingTokens(input), out var locations, out message))
		{
			return false;
		}

		step = new GetItemsByTagActionStep(quantity, tag, locations);
		return true;
	}

	private static bool TryParseCommodity(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished || !double.TryParse(input.PopSpeech(), actor, out var weight) || weight <= 0.0)
		{
			message = "What positive commodity weight should this step collect?";
			return false;
		}

		if (input.IsFinished)
		{
			message = "Which material should this commodity step collect?";
			return false;
		}

		var material = input.PopSpeech();
		string? tag = null;
		while (!input.IsFinished && !input.PeekSpeech().EqualTo("from"))
		{
			var option = input.PopSpeech();
			if (option.EqualTo("tag"))
			{
				if (input.IsFinished)
				{
					message = "Which commodity tag do you want to require?";
					return false;
				}

				tag = input.PopSpeech();
				continue;
			}

			message = $"The commodity option {option.ColourCommand()} is not valid.";
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualTo("from"))
		{
			message = $"Commodity steps use the syntax: {"tasks step commodity <weight> <material> [tag <tag>] from <here|cell ids...> [char <name>=<value> ...]".ColourCommand()}";
			return false;
		}

		var locationTokens = new List<string>();
		while (!input.IsFinished && !input.PeekSpeech().EqualTo("char"))
		{
			locationTokens.Add(input.PopSpeech());
		}

		if (!TryParseLocations(actor, locationTokens, out var locations, out message))
		{
			return false;
		}

		var characteristics = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		if (!input.IsFinished)
		{
			input.PopSpeech();
			while (!input.IsFinished)
			{
				var pair = input.PopSpeech();
				var index = pair.IndexOf('=');
				if (index <= 0 || index >= pair.Length - 1)
				{
					message = $"Commodity characteristics must use the syntax {"char <name>=<value>".ColourCommand()}.";
					return false;
				}

				characteristics[pair[..index]] = pair[(index + 1)..];
			}
		}

		step = new GetCommodityActionStep(weight, material, tag, characteristics, locations);
		return true;
	}

	private static bool TryParseDeliver(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished || !input.PopSpeech().EqualTo("to"))
		{
			message = $"Delivery steps use the syntax: {"tasks step deliver to <here|cell id> [container <item id>] [containertag <tag>]".ColourCommand()}";
			return false;
		}

		if (input.IsFinished)
		{
			message = "Which destination should this delivery step use?";
			return false;
		}

		if (!TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
		{
			return false;
		}

		IGameItem? container = null;
		string? containerTag = null;
		while (!input.IsFinished)
		{
			var option = input.PopSpeech();
			if (option.EqualTo("container"))
			{
				if (containerTag is not null)
				{
					message = "Specify either a container item id or a container tag, not both.";
					return false;
				}

				if (input.IsFinished || !long.TryParse(input.PopSpeech(), out var containerId))
				{
					message = "Which container item id do you want to deliver to?";
					return false;
				}

				container = actor.Gameworld.TryGetItem(containerId, true);
				if (container is null)
				{
					message = $"There is no item with id {containerId.ToString("N0", actor).ColourValue()}.";
					return false;
				}

				continue;
			}

			if (option.EqualTo("containertag"))
			{
				if (container is not null)
				{
					message = "Specify either a container item id or a container tag, not both.";
					return false;
				}

				if (input.IsFinished)
				{
					message = "Which container tag do you want to deliver to?";
					return false;
				}

				containerTag = input.PopSpeech();
				continue;
			}

			message = $"The delivery option {option.ColourCommand()} is not valid.";
			return false;
		}

		step = new DeliverItemsActionStep(destination, container, containerTag);
		message = string.Empty;
		return true;
	}

	private static bool TryParseLoad(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (!input.IsFinished && input.PeekSpeech().EqualTo("all"))
		{
			input.PopSpeech();
		}

		if (input.IsFinished || !input.PopSpeech().EqualTo("into"))
		{
			message = $"Load steps use the syntax: {"tasks step load all into <item id|tag <tag>> [at <here|cell id>]".ColourCommand()}";
			return false;
		}

		if (!TryParseItemOrTag(actor, input, "load target container", out var container, out var tag, out message))
		{
			return false;
		}

		ICell? location = null;
		if (!input.IsFinished)
		{
			if (!input.PopSpeech().EqualTo("at"))
			{
				message = $"Load location options use the syntax {"at <here|cell id>".ColourCommand()}.";
				return false;
			}

			if (input.IsFinished || !TryResolveLocation(actor, input.PopSpeech(), out location, out message))
			{
				return false;
			}
		}

		if (!input.IsFinished)
		{
			message = $"Could not understand the extra text {input.SafeRemainingArgument.ColourCommand()} in the load step.";
			return false;
		}

		step = new LoadItemsActionStep(container, tag, location);
		message = string.Empty;
		return true;
	}

	private static bool TryParseUnload(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (!TryParseItemOrTag(actor, input, "unload source container", out var container, out var tag, out message))
		{
			return false;
		}

		ICell? location = null;
		if (!input.IsFinished)
		{
			if (!input.PopSpeech().EqualTo("at"))
			{
				message = $"Unload location options use the syntax {"at <here|cell id>".ColourCommand()}.";
				return false;
			}

			if (input.IsFinished || !TryResolveLocation(actor, input.PopSpeech(), out location, out message))
			{
				return false;
			}
		}

		if (!input.IsFinished)
		{
			message = $"Could not understand the extra text {input.SafeRemainingArgument.ColourCommand()} in the unload step.";
			return false;
		}

		step = new UnloadItemsActionStep(container, tag, location);
		message = string.Empty;
		return true;
	}

	private static bool TryParseReturn(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Return steps use the syntax: {"tasks step return container <item id|tag <tag>> to <here|cell id> [container <item id>|containertag <tag>]".ColourCommand()}";
			return false;
		}

		var assetKind = input.PopSpeech();
		if (!assetKind.EqualTo("container"))
		{
			message = $"{assetKind.ColourCommand()} return is not executable yet. This slice supports {"return container".ColourCommand()}; vehicle driving and animal leading remain deferred.";
			return false;
		}

		if (!TryParseItemOrTag(actor, input, "return container", out var container, out var tag, out message))
		{
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualTo("to"))
		{
			message = $"Return steps use the syntax: {"tasks step return container <item id|tag <tag>> to <here|cell id> [container <item id>|containertag <tag>]".ColourCommand()}";
			return false;
		}

		if (input.IsFinished || !TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
		{
			return false;
		}

		IGameItem? destinationContainer = null;
		string? destinationContainerTag = null;
		while (!input.IsFinished)
		{
			var option = input.PopSpeech();
			if (option.EqualTo("container"))
			{
				if (destinationContainerTag is not null)
				{
					message = "Specify either a destination container item id or a destination container tag, not both.";
					return false;
				}

				if (input.IsFinished || !long.TryParse(input.PopSpeech(), out var containerId))
				{
					message = "Which destination container item id do you want to return to?";
					return false;
				}

				destinationContainer = actor.Gameworld.TryGetItem(containerId, true);
				if (destinationContainer is null)
				{
					message = $"There is no item with id {containerId.ToString("N0", actor).ColourValue()}.";
					return false;
				}

				continue;
			}

			if (option.EqualTo("containertag"))
			{
				if (destinationContainer is not null)
				{
					message = "Specify either a destination container item id or a destination container tag, not both.";
					return false;
				}

				if (input.IsFinished)
				{
					message = "Which destination container tag do you want to return to?";
					return false;
				}

				destinationContainerTag = input.PopSpeech();
				continue;
			}

			message = $"The return option {option.ColourCommand()} is not valid.";
			return false;
		}

		step = new ReturnAssetActionStep(container, tag, destination, destinationContainer, destinationContainerTag);
		message = string.Empty;
		return true;
	}

	private static bool TryParseVehicle(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished || !input.PopSpeech().EqualTo("cargo"))
		{
			message = $"Vehicle steps use the syntax: {"tasks step vehicle cargo <vehicle id|exterior item id> <cargo id|cargo name>".ColourCommand()}";
			return false;
		}

		if (input.IsFinished || !long.TryParse(input.PopSpeech(), out var vehicleId))
		{
			message = "Which vehicle id or exterior item id should this cargo step select?";
			return false;
		}

		var vehicle = ResolveVehicle(actor, vehicleId);
		if (vehicle is null)
		{
			message = $"There is no vehicle or vehicle exterior item with id {vehicleId.ToString("N0", actor).ColourValue()}.";
			return false;
		}

		if (input.IsFinished)
		{
			message = "Which cargo space id or name should this vehicle step select?";
			return false;
		}

		var cargoText = input.SafeRemainingArgument.Trim();
		ConsumeRemaining(input);
		var cargoSpace = ResolveCargoSpace(vehicle, cargoText);
		if (cargoSpace is null)
		{
			message = $"Vehicle {vehicle.Name.ColourName()} does not have a cargo space matching {cargoText.ColourCommand()}.";
			return false;
		}

		step = new VehicleOperationActionStep(vehicle, cargoSpace);
		message = string.Empty;
		return true;
	}

	private static bool TryParseMove(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (!input.IsFinished && input.PeekSpeech().EqualTo("to"))
		{
			input.PopSpeech();
		}

		if (input.IsFinished)
		{
			message = $"Move steps use the syntax: {"tasks step move to <here|cell id>".ColourCommand()}";
			return false;
		}

		if (!TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
		{
			return false;
		}

		if (!input.IsFinished)
		{
			message = $"Could not understand the extra text {input.SafeRemainingArgument.ColourCommand()} in the move step.";
			return false;
		}

		step = new MovementDeliveryActionStep($"move to {destination.GetFriendlyReference(actor)}", destination);
		message = string.Empty;
		return true;
	}

	private static bool TryParseBoard(StringStack input, out IEmploymentActionStep step, out string message)
	{
		step = null!;
		var text = input.SafeRemainingArgument.Trim();
		var split = text.IndexOf('=');
		if (split <= 0 || split >= text.Length - 1)
		{
			message = $"Board-post steps use the syntax: {"tasks step board <title> = <text>".ColourCommand()}";
			return false;
		}

		var title = text[..split].Trim().Trim('"');
		var body = text[(split + 1)..].Trim();
		if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
		{
			message = $"Board-post steps use the syntax: {"tasks step board <title> = <text>".ColourCommand()}";
			return false;
		}

		ConsumeRemaining(input);
		step = new BoardPostActionStep(title, body);
		message = string.Empty;
		return true;
	}

	private static bool TryParseCommand(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		ICell? location = null;
		if (!input.IsFinished && input.PeekSpeech().EqualTo("at"))
		{
			input.PopSpeech();
			if (input.IsFinished)
			{
				message = "Which location should this command step execute at?";
				return false;
			}

			if (!TryResolveLocation(actor, input.PopSpeech(), out var resolvedLocation, out message))
			{
				return false;
			}

			location = resolvedLocation;
		}

		if (input.IsFinished)
		{
			message = $"Command steps use the syntax: {"tasks step command [at <here|cell id>] <command> [arguments...]".ColourCommand()}";
			return false;
		}

		var commandName = input.PopSpeech();
		var arguments = input.SafeRemainingArgument;
		ConsumeRemaining(input);
		step = new CommandActionStep(commandName, arguments, location);
		message = string.Empty;
		return true;
	}

	private static bool TryParsePurchase(IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		var amountTokens = PopTokensUntil(input, "for").ToList();
		if (!amountTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("for"))
		{
			message = $"Purchase audit steps use the syntax: {"tasks step purchase <amount> for <description>".ColourCommand()}";
			return false;
		}

		if (!TryParseMoney(host, string.Join(" ", amountTokens), out var amount, out message))
		{
			return false;
		}

		var description = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(description))
		{
			message = "What purchase should this audit step describe?";
			return false;
		}

		ConsumeRemaining(input);
		step = new PurchaseActionStep(description, amount);
		message = string.Empty;
		return true;
	}

	private static bool TryParseBankDeposit(IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (!TryParseMoney(host, input.SafeRemainingArgument, out var amount, out message))
		{
			return false;
		}

		ConsumeRemaining(input);
		step = new BankDepositActionStep(amount);
		message = string.Empty;
		return true;
	}

	private static bool TryParseBankWithdraw(IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (!TryParseMoney(host, input.SafeRemainingArgument, out var amount, out message))
		{
			return false;
		}

		ConsumeRemaining(input);
		step = new BankWithdrawalActionStep(amount);
		message = string.Empty;
		return true;
	}

	private static bool TryParseStorePay(IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Store-account payment audit steps use the syntax: {"tasks step storepay <account> amount <amount>".ColourCommand()}";
			return false;
		}

		var accountName = input.PopSpeech();
		if (input.IsFinished || !input.PopSpeech().EqualTo("amount"))
		{
			message = $"Store-account payment audit steps use the syntax: {"tasks step storepay <account> amount <amount>".ColourCommand()}";
			return false;
		}

		if (!TryParseMoney(host, input.SafeRemainingArgument, out var amount, out message))
		{
			return false;
		}

		ConsumeRemaining(input);
		step = new StoreAccountPaymentActionStep(accountName, amount);
		message = string.Empty;
		return true;
	}

	private static bool TryParseCraft(StringStack input, out IEmploymentActionStep step, out string message)
	{
		step = null!;
		var description = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(description))
		{
			message = $"Craft audit steps use the syntax: {"tasks step craft <description>".ColourCommand()}";
			return false;
		}

		ConsumeRemaining(input);
		step = new CraftTriggerActionStep(description);
		message = string.Empty;
		return true;
	}

	private static bool TryParseGenericShell(string actionKey, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		var description = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(description))
		{
			message = $"{actionKey.ColourCommand()} steps need a short description to audit.";
			return false;
		}

		ConsumeRemaining(input);
		step = new CataloguedActionShellStep(actionKey, description);
		message = string.Empty;
		return true;
	}

	private static bool TryParseRoute(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished || !input.PopSpeech().EqualTo("to"))
		{
			message = $"Route planning steps use the syntax: {"tasks step route to <here|cell id> [description]".ColourCommand()}";
			return false;
		}

		if (input.IsFinished)
		{
			message = "Which destination should this route planning step target?";
			return false;
		}

		if (!TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
		{
			return false;
		}

		var description = input.IsFinished
			? $"route to {destination.GetFriendlyReference(actor)}"
			: input.SafeRemainingArgument.Trim();
		ConsumeRemaining(input);
		step = new CataloguedActionShellStep("route", description, destination);
		message = string.Empty;
		return true;
	}

	private static bool TryParseItemOrTag(ICharacter actor, StringStack input, string noun, out IGameItem? item,
		out string? tag, out string message)
	{
		item = null;
		tag = null;
		if (input.IsFinished)
		{
			message = $"Which {noun} do you want to use?";
			return false;
		}

		var token = input.PopSpeech();
		if (token.EqualTo("tag"))
		{
			if (input.IsFinished)
			{
				message = $"Which tag identifies the {noun}?";
				return false;
			}

			tag = input.PopSpeech();
			message = string.Empty;
			return true;
		}

		if (!long.TryParse(token, out var itemId))
		{
			message = $"The {noun} must be an item id or {"tag <tag>".ColourCommand()}.";
			return false;
		}

		item = actor.Gameworld.TryGetItem(itemId, true);
		if (item is null)
		{
			message = $"There is no item with id {itemId.ToString("N0", actor).ColourValue()}.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static IVehicle? ResolveVehicle(ICharacter actor, long id)
	{
		return actor.Gameworld.Vehicles.Get(id) ??
		       actor.Gameworld.Vehicles.FirstOrDefault(x => x.ExteriorItemId == id || x.ExteriorItem?.Id == id);
	}

	private static IVehicleCargoSpace? ResolveCargoSpace(IVehicle vehicle, string text)
	{
		if (long.TryParse(text, out var id))
		{
			return vehicle.CargoSpaces.FirstOrDefault(x => x.Id == id || x.ProjectionItemId == id);
		}

		return vehicle.CargoSpaces.FirstOrDefault(x => x.Name.EqualTo(text) || x.Prototype.Name.EqualTo(text));
	}

	private static bool TryParseMoney(IEmploymentHost host, string text, out MoneyAmount amount, out string message)
	{
		amount = null!;
		var currency = ResolveHostCurrency(host);
		if (currency is null)
		{
			message = $"Could not determine the currency for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		text = text.Trim();
		if (string.IsNullOrWhiteSpace(text) || !currency.TryGetBaseCurrency(text, out var parsed) || parsed <= 0.0M)
		{
			message = $"Could not parse {text.ColourCommand()} as a positive amount of {currency.Name.ColourName()}.";
			return false;
		}

		amount = new MoneyAmount(currency, parsed);
		message = string.Empty;
		return true;
	}

	private static IEnumerable<string> PopTokensUntil(StringStack input, string token)
	{
		while (!input.IsFinished && !input.PeekSpeech().EqualTo(token))
		{
			yield return input.PopSpeech();
		}
	}

	private static IEnumerable<string> PopRemainingTokens(StringStack input)
	{
		while (!input.IsFinished)
		{
			yield return input.PopSpeech();
		}
	}

	private static void ConsumeRemaining(StringStack input)
	{
		while (!input.IsFinished)
		{
			input.PopSpeech();
		}
	}

	private static IEnumerable<List<string>> SplitActionTokens(IEnumerable<string> tokens)
	{
		var current = new List<string>();
		foreach (var token in tokens)
		{
			if (token.EqualTo("then") || token.EqualTo(";"))
			{
				if (current.Any())
				{
					yield return current;
					current = new List<string>();
				}

				continue;
			}

			current.Add(token);
		}

		if (current.Any())
		{
			yield return current;
		}
	}

	private static bool TryParseLongs(IEnumerable<string> tokens, out List<long> values, out string message)
	{
		values = new List<long>();
		foreach (var token in tokens.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)))
		{
			if (!long.TryParse(token, out var value))
			{
				message = $"The text {token.ColourCommand()} is not a valid numeric prototype id.";
				return false;
			}

			values.Add(value);
		}

		message = string.Empty;
		return values.Any();
	}

	private static bool TryParseLocations(ICharacter actor, IEnumerable<string> tokens, out List<ICell> locations,
		out string message)
	{
		locations = new List<ICell>();
		foreach (var token in tokens)
		{
			foreach (var split in token.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			{
				if (!TryResolveLocation(actor, split, out var location, out message))
				{
					return false;
				}

				locations.Add(location);
			}
		}

		if (!locations.Any())
		{
			message = "You must specify at least one source or destination location.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static bool TryResolveLocation(ICharacter actor, string token, out ICell location, out string message)
	{
		if (token.EqualTo("here"))
		{
			location = actor.Location;
			message = string.Empty;
			return true;
		}

		if (!long.TryParse(token, out var id))
		{
			location = null!;
			message = $"Employment task locations must be {"here".ColourCommand()} or numeric cell ids in this slice.";
			return false;
		}

		location = actor.Gameworld.Cells.Get(id)!;
		if (location is null)
		{
			message = $"There is no cell with id {id.ToString("N0", actor).ColourValue()}.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static bool TryRequireAssignTasks(ICharacter actor, IEmploymentHost host, out string message)
	{
		if (actor.IsAdministrator() || host.HasAuthority(actor, EmploymentAuthority.AssignTasks))
		{
			message = string.Empty;
			return true;
		}

		message = $"You do not have the delegated {EmploymentAuthority.AssignTasks.DescribeEnum().ColourName()} authority for {host.EmploymentHostName.ColourName()}.";
		return false;
	}

	private static ICurrency? ResolveHostCurrency(IEmploymentHost host)
	{
		return host switch
		{
			IShop shop => shop.Currency,
			IAuctionHouse auctionHouse => auctionHouse.EconomicZone.Currency,
			ICombatArena arena => arena.Currency,
			IBank bank => bank.PrimaryCurrency,
			IStable stable => stable.Currency,
			IHotel hotel => hotel.Currency,
			_ => host.EmploymentContracts.Select(x => x.Compensation.FixedRate?.Currency ?? x.Compensation.MinimumEffectivePay?.Currency)
			         .FirstOrDefault(x => x is not null)
		};
	}

	private static string DescribeCapabilities(IReadOnlySet<EmploymentAICapability> capabilities)
	{
		return capabilities.Any()
			? capabilities.Select(x => x.DescribeEnum().ColourName()).ListToString()
			: "none".ColourValue();
	}

	private static EmploymentTaskDraft? DraftFor(ICharacter actor, IEmploymentHost host)
	{
		return actor.EffectsOfType<EmploymentTaskDraftEffect>()
		            .FirstOrDefault(x => x.Draft.Host.EmploymentHostType == host.EmploymentHostType && x.Draft.Host.Id == host.Id)
		            ?.Draft;
	}

	private static bool RemoveDraft(ICharacter actor, IEmploymentHost host)
	{
		return actor.RemoveAllEffects<EmploymentTaskDraftEffect>(
			x => x.Draft.Host.EmploymentHostType == host.EmploymentHostType && x.Draft.Host.Id == host.Id,
			true);
	}

	internal static string DescribeStep(IEmploymentActionStep step, ICharacter actor)
	{
		return step switch
		{
			PurchaseActionStep purchase =>
				$"record an audit-only purchase of {DescribeMoney(purchase.Amount)} for {purchase.PurchaseDescription.ColourName()}",
			MovementDeliveryActionStep move =>
				move.Destination is null
					? $"complete movement/delivery shell: {move.DeliveryDescription.ColourName()}"
					: $"go to {move.Destination.GetFriendlyReference(actor).ColourName()} for {move.DeliveryDescription.ColourName()}",
			CraftTriggerActionStep craft =>
				$"record an audit-only craft trigger for {craft.CraftDescription.ColourName()}",
			CommandActionStep command =>
				$"execute allowlisted command {command.CommandName.ColourCommand()}{(string.IsNullOrWhiteSpace(command.CommandArguments) ? string.Empty : $" {command.CommandArguments.ColourCommand()}")}{(command.ExecutionLocation is null ? string.Empty : $" at {command.ExecutionLocation.GetFriendlyReference(actor).ColourName()}")}",
			BankDepositActionStep deposit =>
				$"record an audit-only bank deposit of {DescribeMoney(deposit.Amount)}",
			BankWithdrawalActionStep withdrawal =>
				$"record an audit-only bank withdrawal of {DescribeMoney(withdrawal.Amount)}",
			StoreAccountPaymentActionStep account =>
				$"record an audit-only payment of {DescribeMoney(account.Amount)} to store account {account.AccountName.ColourName()}",
			BoardPostActionStep board =>
				$"post {board.Title.ColourName()} to the host staff communication board",
			CataloguedActionShellStep shell =>
				shell.TargetLocation is null
					? $"record {shell.ActionKey.ColourCommand()} audit shell: {shell.ActionDescription}"
					: $"go to {shell.TargetLocation.GetFriendlyReference(actor).ColourName()} and record {shell.ActionKey.ColourCommand()} audit shell: {shell.ActionDescription}",
			GetItemsByIdActionStep getId =>
				$"go to {DescribeLocations(getId.SourceLocations, actor)} and collect {getId.Quantity.ToString("N0", actor).ColourValue()}x {DescribeItemPrototypeIds(getId.ItemPrototypeIds, actor)}",
			GetItemsByTagActionStep getTag =>
				$"go to {DescribeLocations(getTag.SourceLocations, actor)} and collect {getTag.Quantity.ToString("N0", actor).ColourValue()}x items tagged {getTag.TagName.ColourCommand()}",
			GetCommodityActionStep commodity =>
				$"go to {DescribeLocations(commodity.SourceLocations, actor)} and collect {commodity.RequiredWeight.ToString("N2", actor).ColourValue()} weight of {commodity.MaterialName.ColourCommand()} commodity{(string.IsNullOrWhiteSpace(commodity.TagName) ? string.Empty : $" tagged {commodity.TagName.ColourCommand()}")}{DescribeCharacteristics(commodity.Characteristics)}",
			DeliverItemsActionStep deliver =>
				$"go to {deliver.Destination.GetFriendlyReference(actor).ColourName()} and deliver all carried task items{DescribeDeliveryContainer(deliver, actor)}",
			LoadItemsActionStep load =>
				$"go to {DescribeOptionalLocation(load.TargetLocation, load.TargetContainer, actor)} and load all carried task items into {DescribeItemOrTag(load.TargetContainer, load.TargetContainerTag, actor)}",
			UnloadItemsActionStep unload =>
				$"go to {DescribeOptionalLocation(unload.SourceLocation, unload.SourceContainer, actor)} and unload task-loaded items from {DescribeItemOrTag(unload.SourceContainer, unload.SourceContainerTag, actor)}",
			ReturnAssetActionStep returnAsset =>
				$"return container {DescribeItemOrTag(returnAsset.Container, returnAsset.ContainerTag, actor)} to {returnAsset.Destination.GetFriendlyReference(actor).ColourName()}{DescribeReturnDestinationContainer(returnAsset, actor)}",
			VehicleOperationActionStep vehicle =>
				$"validate cargo space {vehicle.CargoSpace.Name.ColourName()} on {vehicle.Vehicle.Name.ColourName()}",
			_ => step.StepType.DescribeEnum().ColourName()
		};
	}

	internal static EmploymentActionDefinition? DefinitionFor(IEmploymentActionStep step)
	{
		return step switch
		{
			CataloguedActionShellStep shell => shell.Definition,
			PurchaseActionStep => EmploymentActionCatalog.Get("purchase"),
			MovementDeliveryActionStep => EmploymentActionCatalog.Get("move"),
			CraftTriggerActionStep => EmploymentActionCatalog.Get("craft"),
			CommandActionStep => EmploymentActionCatalog.Get("command"),
			BankDepositActionStep => EmploymentActionCatalog.Get("bankdeposit"),
			BankWithdrawalActionStep => EmploymentActionCatalog.Get("bankwithdraw"),
			StoreAccountPaymentActionStep => EmploymentActionCatalog.Get("storepay"),
			BoardPostActionStep => EmploymentActionCatalog.Get("board"),
			GetItemsByIdActionStep => EmploymentActionCatalog.Get("getid"),
			GetItemsByTagActionStep => EmploymentActionCatalog.Get("gettag"),
			GetCommodityActionStep => EmploymentActionCatalog.Get("commodity"),
			DeliverItemsActionStep => EmploymentActionCatalog.Get("deliver"),
			LoadItemsActionStep => EmploymentActionCatalog.Get("load"),
			UnloadItemsActionStep => EmploymentActionCatalog.Get("unload"),
			ReturnAssetActionStep => EmploymentActionCatalog.Get("return"),
			VehicleOperationActionStep => EmploymentActionCatalog.Get("vehicle"),
			_ => EmploymentActionCatalog.ForStepType(step.StepType)
		};
	}

	internal static string DescribeStepCatalogueStatus(IEmploymentActionStep step)
	{
		var definition = DefinitionFor(step);
		return definition is null
			? "uncatalogued".ColourError()
			: $"{definition.Key.ColourCommand()} / {definition.Status.DescribeEnum().ColourValue()}";
	}

	internal static string DescribeStepBoundaryWarning(IEmploymentActionStep step)
	{
		var definition = DefinitionFor(step);
		if (definition is null)
		{
			return string.Empty;
		}

		return definition.Status switch
		{
			EmploymentActionCatalogStatus.AuditOnlyShell =>
				"Audit-only: this step records employment evidence but does not mutate the external subsystem.".ColourError(),
			EmploymentActionCatalogStatus.Deferred =>
				$"Deferred: {(definition.DeferredReason ?? definition.Summary).ColourError()}",
			_ => string.Empty
		};
	}

	private static string DescribeMoney(MoneyAmount amount)
	{
		return amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
	}

	private static string DescribeLocations(IEnumerable<ICell> locations, ICharacter actor)
	{
		return locations
		       .Select(x => x.GetFriendlyReference(actor).ColourName())
		       .ListToString();
	}

	private static string DescribeItemPrototypeIds(IEnumerable<long> itemPrototypeIds, ICharacter actor)
	{
		return itemPrototypeIds
		       .Select(x =>
		       {
			       var item = actor.Gameworld?.ItemProtos?.Get(x);
			       return item is null
				       ? $"item prototype #{x.ToString("N0", actor)}".ColourValue()
				       : $"{item.ShortDescription.ColourName()} (prototype #{x.ToString("N0", actor).ColourValue()})";
		       })
		       .ListToString();
	}

	private static string DescribeCharacteristics(IReadOnlyDictionary<string, string> characteristics)
	{
		return characteristics.Any()
			? $" with {characteristics.Select(x => $"{x.Key.ColourCommand()}={x.Value.ColourCommand()}").ListToString()}"
			: string.Empty;
	}

	private static string DescribeDeliveryContainer(DeliverItemsActionStep deliver, ICharacter actor)
	{
		if (deliver.Container is not null)
		{
			return $" into {deliver.Container.HowSeen(actor, colour: false).ColourName()}";
		}

		if (!string.IsNullOrWhiteSpace(deliver.ContainerTag))
		{
			return $" into a container tagged {deliver.ContainerTag.ColourCommand()}";
		}

		return string.Empty;
	}

	private static string DescribeOptionalLocation(ICell? location, IGameItem? item, ICharacter actor)
	{
		if (location is not null)
		{
			return location.GetFriendlyReference(actor).ColourName();
		}

		var itemLocation = item?.TrueLocations.FirstOrDefault();
		return itemLocation is null ? "the target location".ColourName() : itemLocation.GetFriendlyReference(actor).ColourName();
	}

	private static string DescribeItemOrTag(IGameItem? item, string? tag, ICharacter actor)
	{
		if (item is not null)
		{
			return item.HowSeen(actor, colour: false).ColourName();
		}

		return string.IsNullOrWhiteSpace(tag)
			? "an unresolved container".ColourError()
			: $"a container tagged {tag.ColourCommand()}";
	}

	private static string DescribeReturnDestinationContainer(ReturnAssetActionStep returnAsset, ICharacter actor)
	{
		if (returnAsset.DestinationContainer is not null)
		{
			return $" into {returnAsset.DestinationContainer.HowSeen(actor, colour: false).ColourName()}";
		}

		if (!string.IsNullOrWhiteSpace(returnAsset.DestinationContainerTag))
		{
			return $" into a container tagged {returnAsset.DestinationContainerTag.ColourCommand()}";
		}

		return string.Empty;
	}
}
