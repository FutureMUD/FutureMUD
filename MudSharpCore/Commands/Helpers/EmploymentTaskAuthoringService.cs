using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Property;
using MudSharp.Economy.Shops;
using MudSharp.FutureProg;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
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

	public bool TryParseActionStep(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		return TryParseStep(actor, host, input, out step, out message);
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
		sb.AppendLine("Planning, authorisation, shop bank/virtual-cash finance, and safe logistics actions persist task state; audit-only actions record evidence without mutating purchasing, store-account, craft, pricing, or administrative systems.");
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
		sb.AppendLine($"\t\tAuthority: {action.RequiredAuthority.Authorities.DescribeEnum().ColourName()} | AI: {DescribeCapabilities(action.RequiredCapabilities)} | Payment Authorisation: {action.RequiresPaymentAuthorisation.ToColouredString()} | Financial: {action.IsFinancial.ToColouredString()} | Sources: {action.InvocationSources.DescribeEnum().ColourName()}");
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
			"stocktransfer" or "transferstock" or "stockmove" =>
				TryParseShopStockTransfer(actor, host, input, out step, out message),
			"auctionlist" or "lotlist" or "listauction" =>
				TryParseAuctionLotListing(actor, host, input, out step, out message),
			"auctionsettle" or "settleauction" or "lotsettle" =>
				TryParseAuctionSettlement(actor, host, input, out step, out message),
			"auctionclaim" or "claimauction" or "lotclaim" =>
				TryParseAuctionClaim(actor, host, input, out step, out message),
			"arenaevent" or "event" or "arenaadmin" or "arenaeventadmin" =>
				TryParseArenaEventAdministration(actor, host, input, out step, out message),
			"bankadmin" or "bank" or "bankreserve" or "bankaccount" =>
				TryParseBankAdministration(actor, host, input, out step, out message),
			"stableadmin" or "stablecare" or "stablereconcile" =>
				TryParseStableAdministration(actor, host, input, out step, out message),
			"hoteladmin" or "hotelroom" or "hotelreconcile" =>
				TryParseHotelAdministration(actor, host, input, out step, out message),
			"load" or "loaditems" => TryParseLoad(actor, input, out step, out message),
			"unload" or "unloaditems" => TryParseUnload(actor, input, out step, out message),
			"return" or "returncontainer" => TryParseReturn(actor, input, out step, out message),
			"vehicle" or "cargo" => TryParseVehicle(actor, input, out step, out message),
			"animal" or "mount" or "stableanimal" => TryParseAnimal(actor, input, out step, out message),
			"move" => TryParseMove(actor, input, out step, out message),
			"board" => TryParseBoard(input, out step, out message),
			"command" => TryParseCommand(actor, input, out step, out message),
			"supplier" or "findsupplier" or "suppliercheck" or "source" =>
				TryParseSupplierSelection(actor, host, input, out step, out message),
			"purchase" => TryParsePurchase(actor, host, input, out step, out message),
			"bankdeposit" => TryParseBankDeposit(host, input, out step, out message),
			"bankwithdraw" => TryParseBankWithdraw(host, input, out step, out message),
			"transfer" => TryParseBankTransfer(host, input, out step, out message),
			"settle" => TryParseHostSettlement(host, input, out step, out message),
			"storepay" => TryParseStorePay(host, input, out step, out message),
			"paytax" => TryParsePayTax(host, input, out step, out message),
			"payroll" => TryParsePayrollSettlement(input, out step, out message),
			"cashreconcile" or "reconcilecash" or "cashcheck" or "tillcheck" =>
				TryParseCashReconciliation(host, input, out step, out message),
			"float" => TryParseShopFloat(actor, host, input, out step, out message),
			"physicalfloat" or "cashtrip" or "employeefloat" =>
				TryParsePhysicalFloat(actor, host, input, out step, out message),
			"stocktake" or "stockcount" or "inventorycount" =>
				TryParseShopStocktake(actor, host, input, out step, out message),
			"price" or "pricing" => TryParsePriceChange(actor, host, input, out step, out message),
			"sale" or "deal" or "shopdeal" => TryParseShopDealAdministration(actor, host, input, out step, out message),
			"jobopening" or "opening" or "job" =>
				TryParseJobOpeningAdministration(actor, host, input, out step, out message),
			"rule" or "scheduledrule" or "schedulerule" =>
				TryParseScheduledRuleAdministration(actor, host, input, out step, out message),
			"admintask" or "taskadmin" or "taskctl" =>
				TryParseActiveTaskAdministration(actor, host, input, out step, out message),
			"goal" or "managergoal" or "goaladmin" =>
				TryParseManagerGoalAdministration(actor, host, input, out step, out message),
			"craft" => TryParseCraft(input, out step, out message),
			"station" => TryParseCraftStation(input, out step, out message),
			"report" or "authorise" or "reserve" or "release" or "select" or "estimate" =>
				TryParseGenericShell(host, definition!.Key, input, out step, out message),
			"route" => TryParseRoute(actor, input, out step, out message),
			"routebatch" or "deliverybatch" or "multistop" => TryParseRouteBatch(actor, input, out step, out message),
			"tripcheck" or "routepolicy" or "logisticspolicy" or "logisticscheck" =>
				TryParseTripCheck(actor, input, out step, out message),
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
			message = $"Get-by-id steps use the syntax: {"tasks step getid <quantity> <prototype ids|*item ids...> from <here|cell ids...>".ColourCommand()}";
			return false;
		}

		var fromToken = input.PopSpeech();
		if (!fromToken.EqualTo("from"))
		{
			message = $"Get-by-id steps must specify source locations with the {"from".ColourCommand()} keyword.";
			return false;
		}

		if (!TryParseRetrievalIds(actor, idTokens, out var itemPrototypeIds, out var specificItemIds, out message))
		{
			return false;
		}

		if (!TryParseLocations(actor, PopRemainingTokens(input), out var locations, out message))
		{
			return false;
		}

		step = new GetItemsByIdActionStep(quantity, itemPrototypeIds, locations, specificItemIds);
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
			message = $"Get-by-tag steps use the syntax: {"tasks step gettag <quantity> <&tag id|&tag name> from <here|cell ids...>".ColourCommand()}";
			return false;
		}

		if (!TryParseTagName(actor, tag, "retrieval tag", out tag, out message))
		{
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

				var tagToken = input.PopSpeech();
				if (!TryParseTagName(actor, tagToken, "commodity tag", out tag, out message))
				{
					return false;
				}

				continue;
			}

			message = $"The commodity option {option.ColourCommand()} is not valid.";
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualTo("from"))
		{
			message = $"Commodity steps use the syntax: {"tasks step commodity <weight> <material> [tag <&tag id|&tag name>] from <here|cell ids...> [char <name>=<value> ...]".ColourCommand()}";
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
			message = $"Delivery steps use the syntax: {"tasks step deliver to <here|cell id> [container <prototype id|*item id|&tag|keyword>]".ColourCommand()}";
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

		EmploymentItemSelector? containerSelector = null;
		while (!input.IsFinished)
		{
			var option = input.PopSpeech();
			if (option.EqualTo("container"))
			{
				if (containerSelector is not null)
				{
					message = "Specify only one destination container selector.";
					return false;
				}

				if (!TryParseItemSelector(actor, input, "destination container", out containerSelector, out message))
				{
					return false;
				}

				continue;
			}

			if (option.EqualTo("containertag"))
			{
				if (containerSelector is not null)
				{
					message = "Specify only one destination container selector.";
					return false;
				}

				if (input.IsFinished)
				{
					message = "Which container tag do you want to deliver to?";
					return false;
				}

				if (!TryParseTagSelector(actor, input.PopSpeech(), "destination container", out containerSelector,
					    out message))
				{
					return false;
				}

				continue;
			}

			message = $"The delivery option {option.ColourCommand()} is not valid.";
			return false;
		}

		step = new DeliverItemsActionStep(destination, containerSelector);
		message = string.Empty;
		return true;
	}

	private static bool TryParseShopStockTransfer(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not IPermanentShop sourceShop)
		{
			message = "Stock transfer steps can only be authored for permanent shop employment hosts.";
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualTo("to"))
		{
			message = $"Stock transfer steps use the syntax: {"tasks step stocktransfer to <shop id|name|self> merch <target merchandise id|name> [destination <here|cell id>] [container <selector>]".ColourCommand()}.";
			return false;
		}

		if (input.IsFinished)
		{
			message = "Which shop should receive this stock transfer?";
			return false;
		}

		var targetShopSelector = input.PopSpeech();
		var targetShop = targetShopSelector.EqualToAny("self", "source", "here")
			? sourceShop
			: actor.Gameworld.Shops.GetByIdOrName(targetShopSelector) as IPermanentShop;
		if (targetShop is null)
		{
			message = $"There is no permanent shop matching {targetShopSelector.ColourCommand()}.";
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualToAny("merch", "merchandise", "as"))
		{
			message = "Which target merchandise should receive the transferred stock?";
			return false;
		}

		if (input.IsFinished)
		{
			message = "Which target merchandise should receive the transferred stock?";
			return false;
		}

		var merchandiseSelector = input.PopSpeech();
		var targetMerchandise = ResolveShopStocktakeMerchandise(targetShop, merchandiseSelector);
		if (targetMerchandise is null)
		{
			message = $"There is no merchandise belonging to {targetShop.Name.ColourName()} matching {merchandiseSelector.ColourCommand()}.";
			return false;
		}

		var destination = targetShop.StockroomCell ?? targetShop.ShopfrontCells.FirstOrDefault();
		EmploymentItemSelector? containerSelector = null;
		while (!input.IsFinished)
		{
			var option = input.PopSpeech();
			if (option.EqualToAny("destination", "dest", "at"))
			{
				if (input.IsFinished)
				{
					message = "Which destination should this stock transfer use?";
					return false;
				}

				if (!TryResolveLocation(actor, input.PopSpeech(), out var parsedDestination, out message))
				{
					return false;
				}

				destination = parsedDestination;
				continue;
			}

			if (option.EqualTo("container"))
			{
				if (containerSelector is not null)
				{
					message = "Specify only one stock-transfer destination container selector.";
					return false;
				}

				if (!TryParseItemSelector(actor, input, "stock-transfer destination container", out containerSelector,
					    out message))
				{
					return false;
				}

				continue;
			}

			if (option.EqualTo("containertag"))
			{
				if (containerSelector is not null)
				{
					message = "Specify only one stock-transfer destination container selector.";
					return false;
				}

				if (input.IsFinished)
				{
					message = "Which container tag do you want to transfer stock into?";
					return false;
				}

				if (!TryParseTagSelector(actor, input.PopSpeech(), "stock-transfer destination container",
					    out containerSelector, out message))
				{
					return false;
				}

				continue;
			}

			message = $"The stock transfer option {option.ColourCommand()} is not valid.";
			return false;
		}

		if (destination is null)
		{
			message = $"{targetShop.Name.ColourName()} does not have a stockroom or shopfront destination.";
			return false;
		}

		if (!targetShop.AllShopCells.Any(x => x.Id == destination.Id))
		{
			message = "Stock transfer steps must deliver to one of the target shop's locations.";
			return false;
		}

		step = new ShopStockTransferActionStep(sourceShop, targetShop, targetMerchandise, destination,
			containerSelector);
		message = string.Empty;
		return true;
	}

	private static bool TryParseAuctionLotListing(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not IAuctionHouse auctionHouse)
		{
			message = "Auction lot listing steps can only be used from auction-house employment hosts.";
			return false;
		}

		if (!TryParseItemSelector(actor, input, "auction lot item", out var selector, out message) || selector is null)
		{
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualTo("reserve"))
		{
			message = $"Auction lot listing steps use the syntax: {"tasks step auctionlist <item selector> reserve <amount> [buyout <amount>] [duration <timespan>]".ColourCommand()}.";
			return false;
		}

		var reserveTokens = PopTokensUntilAny(input, ["buyout", "duration"]).ToList();
		if (!TryParseMoney(host, string.Join(" ", reserveTokens), out var reserve, out message))
		{
			return false;
		}

		MoneyAmount? buyout = null;
		TimeSpan? duration = null;
		while (!input.IsFinished)
		{
			var keyword = input.PopSpeech().CollapseString().ToLowerInvariant();
			switch (keyword)
			{
				case "buyout":
					var buyoutTokens = PopTokensUntilAny(input, ["duration"]).ToList();
					if (!TryParseMoney(host, string.Join(" ", buyoutTokens), out var parsedBuyout, out message))
					{
						return false;
					}

					buyout = parsedBuyout;
					break;
				case "duration":
				case "time":
					var durationText = input.SafeRemainingArgument.Trim();
					if (string.IsNullOrWhiteSpace(durationText) ||
					    !TimeSpan.TryParse(durationText, actor, out var parsedDuration) || parsedDuration <= TimeSpan.Zero)
					{
						message = "Auction listing duration must be a positive time span.";
						return false;
					}

					duration = parsedDuration;
					ConsumeRemaining(input);
					break;
				default:
					message = $"Unknown auction listing option {keyword.ColourCommand()}.";
					return false;
			}
		}

		step = new AuctionLotListingActionStep(auctionHouse, selector, reserve, buyout, duration);
		message = string.Empty;
		return true;
	}

	private static bool TryParseAuctionSettlement(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not IAuctionHouse auctionHouse)
		{
			message = "Auction settlement steps can only be used from auction-house employment hosts.";
			return false;
		}

		if (input.IsFinished)
		{
			step = new AuctionSettlementActionStep(auctionHouse, null, null, null);
			message = string.Empty;
			return true;
		}

		var mode = input.PopSpeech();
		if (mode.EqualToAny("due", "all"))
		{
			ConsumeRemaining(input);
			step = new AuctionSettlementActionStep(auctionHouse, null, null, null);
			message = string.Empty;
			return true;
		}

		var target = mode.EqualTo("lot") || mode.EqualTo("item")
			? input.SafeRemainingArgument.Trim()
			: string.Join(" ", new[] { mode }.Concat(PopRemainingTokens(input))).Trim();
		if (string.IsNullOrWhiteSpace(target))
		{
			message = "Which auction lot should this settlement step target?";
			return false;
		}

		var lot = ResolveActiveAuctionLot(actor, auctionHouse, target);
		if (lot is null)
		{
			message = $"There is no active auction lot matching {target.ColourCommand()}.";
			return false;
		}

		ConsumeRemaining(input);
		step = new AuctionSettlementActionStep(auctionHouse, lot);
		message = string.Empty;
		return true;
	}

	private static bool TryParseAuctionClaim(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not IAuctionHouse auctionHouse)
		{
			message = "Auction claim steps can only be used from auction-house employment hosts.";
			return false;
		}

		var target = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(target))
		{
			message = $"Auction claim steps use the syntax: {"tasks step auctionclaim <asset id|keyword>".ColourCommand()}.";
			return false;
		}

		var lot = ResolveUnclaimedAuctionLot(actor, auctionHouse, target);
		if (lot is null)
		{
			message = $"There is no unclaimed auction lot matching {target.ColourCommand()}.";
			return false;
		}

		ConsumeRemaining(input);
		step = new AuctionClaimActionStep(auctionHouse, lot);
		message = string.Empty;
		return true;
	}

	private static AuctionItem? ResolveActiveAuctionLot(ICharacter actor, IAuctionHouse auctionHouse, string target)
	{
		var listings = auctionHouse.ActiveAuctionItems.ToList();
		return ResolveAuctionItem(actor, listings, target);
	}

	private static UnclaimedAuctionItem? ResolveUnclaimedAuctionLot(ICharacter actor, IAuctionHouse auctionHouse,
		string target)
	{
		var unclaimed = auctionHouse.UnclaimedItems.ToList();
		var item = ResolveAuctionItem(actor, unclaimed.Select(x => x.AuctionItem), target);
		return item is null ? null : unclaimed.FirstOrDefault(x => ReferenceEquals(x.AuctionItem, item));
	}

	private static AuctionItem? ResolveAuctionItem(ICharacter actor, IEnumerable<AuctionItem> items, string target)
	{
		if (string.IsNullOrWhiteSpace(target))
		{
			return null;
		}

		var list = items.ToList();
		if (long.TryParse(target, out var id))
		{
			return list.FirstOrDefault(x => x.Asset.Id == id || x.Item?.Id == id);
		}

		return list.GetFromItemListByKeyword(target, actor) ??
		       list.FirstOrDefault(x => x.Asset.Name.Equals(target, StringComparison.InvariantCultureIgnoreCase)) ??
		       list.FirstOrDefault(x => x.Asset.Name.StartsWith(target, StringComparison.InvariantCultureIgnoreCase)) ??
		       list.FirstOrDefault(x => x.Asset.Name.Contains(target, StringComparison.InvariantCultureIgnoreCase));
	}

	private static bool TryParseBankAdministration(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not IBank bank)
		{
			message = "Bank administration steps can only be drafted for bank employment hosts.";
			return false;
		}

		if (input.IsFinished)
		{
			message = $"Bank administration steps use the syntax: {"tasks step bankadmin reserve audit|deposit <amount>|withdraw <amount> OR bankadmin account credit <account> <amount> for <reason>|status <account> <active|suspended|locked> [reason]|close <account> <reason> OR bankadmin branch post <here|cell id> <note>|courier <from> to <to> <note>".ColourCommand()}";
			return false;
		}

		switch (input.PopForSwitch())
		{
			case "reserve":
			case "reserves":
				return TryParseBankReserveAdministration(bank, host, input, out step, out message);
			case "account":
			case "accounts":
				return TryParseBankAccountAdministration(bank, host, input, out step, out message);
			case "branch":
			case "branches":
				return TryParseBankBranchAdministration(actor, bank, input, out step, out message);
			case "audit":
			case "balance":
				step = new BankAdministrationActionStep(bank, BankAdministrationActionKind.ReserveAudit);
				message = string.Empty;
				return true;
			default:
				message = "Bank administration steps must target reserve, account, or branch work.";
				return false;
		}
	}

	private static bool TryParseBankReserveAdministration(IBank bank, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Bank reserve administration uses the syntax: {"bankadmin reserve audit|deposit <amount>|withdraw <amount>".ColourCommand()}";
			return false;
		}

		switch (input.PopForSwitch())
		{
			case "audit":
			case "balance":
			case "check":
				step = new BankAdministrationActionStep(bank, BankAdministrationActionKind.ReserveAudit);
				message = string.Empty;
				return true;
			case "deposit":
			case "depositcash":
				if (!TryParseMoney(host, input.SafeRemainingArgument, out var deposit, out message))
				{
					return false;
				}

				ConsumeRemaining(input);
				step = new BankAdministrationActionStep(bank, BankAdministrationActionKind.ReserveDeposit, deposit);
				message = string.Empty;
				return true;
			case "withdraw":
			case "withdrawal":
			case "withdrawcash":
				if (!TryParseMoney(host, input.SafeRemainingArgument, out var withdrawal, out message))
				{
					return false;
				}

				ConsumeRemaining(input);
				step = new BankAdministrationActionStep(bank, BankAdministrationActionKind.ReserveWithdrawal, withdrawal);
				message = string.Empty;
				return true;
			default:
				message = "Bank reserve administration must be audit, deposit, or withdraw.";
				return false;
		}
	}

	private static bool TryParseBankAccountAdministration(IBank bank, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Bank account administration uses the syntax: {"bankadmin account credit <account> <amount> for <reason>|status <account> <active|suspended|locked> [reason]|close <account> <reason>".ColourCommand()}";
			return false;
		}

		switch (input.PopForSwitch())
		{
			case "credit":
			{
				if (input.IsFinished)
				{
					message = "Which bank account should be credited?";
					return false;
				}

				var accountSelector = input.PopSpeech();
				var amountTokens = PopTokensUntil(input, "for").ToList();
				if (!amountTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("for"))
				{
					message = $"Bank account credit steps use the syntax: {"bankadmin account credit <account> <amount> for <reason>".ColourCommand()}";
					return false;
				}

				if (!TryParseMoney(host, string.Join(" ", amountTokens), out var amount, out message))
				{
					return false;
				}

				var reason = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(reason))
				{
					message = "What audit reason should be recorded for the bank account credit?";
					return false;
				}

				ConsumeRemaining(input);
				step = new BankAdministrationActionStep(bank, BankAdministrationActionKind.AccountCredit, amount,
					accountSelector, reason: reason);
				message = string.Empty;
				return true;
			}
			case "status":
			{
				if (input.IsFinished)
				{
					message = "Which bank account status should be changed?";
					return false;
				}

				var accountSelector = input.PopSpeech();
				if (input.IsFinished)
				{
					message = "What status should the account be set to?";
					return false;
				}

				if (!TryParseBankAccountStatus(input.PopSpeech(), out var status))
				{
					message = $"Bank account status must be {new[] { "active", "suspended", "locked" }.Select(x => x.ColourCommand()).ListToString(conjunction: "or ")}.";
					return false;
				}

				var reason = input.SafeRemainingArgument.Trim();
				ConsumeRemaining(input);
				step = new BankAdministrationActionStep(bank, BankAdministrationActionKind.AccountStatus,
					accountSelector: accountSelector, targetStatus: status, reason: reason);
				message = string.Empty;
				return true;
			}
			case "close":
			{
				if (input.IsFinished)
				{
					message = "Which bank account should be closed?";
					return false;
				}

				var accountSelector = input.PopSpeech();
				var reason = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(reason))
				{
					message = "What audit reason should be recorded for closing the bank account?";
					return false;
				}

				ConsumeRemaining(input);
				step = new BankAdministrationActionStep(bank, BankAdministrationActionKind.AccountClose,
					accountSelector: accountSelector, reason: reason);
				message = string.Empty;
				return true;
			}
			default:
				message = "Bank account administration must be credit, status, or close.";
				return false;
		}
	}

	private static bool TryParseBankBranchAdministration(ICharacter actor, IBank bank, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Bank branch administration uses the syntax: {"bankadmin branch post <here|cell id> <note>|courier <from> to <to> <note>".ColourCommand()}";
			return false;
		}

		switch (input.PopForSwitch())
		{
			case "post":
			case "staff":
			case "teller":
			{
				if (input.IsFinished)
				{
					message = "Which bank branch should receive this post evidence?";
					return false;
				}

				if (!TryResolveLocation(actor, input.PopSpeech(), out var branch, out message))
				{
					return false;
				}

				var note = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(note))
				{
					message = "What branch staffing or teller note should be recorded?";
					return false;
				}

				ConsumeRemaining(input);
				step = new BankAdministrationActionStep(bank, BankAdministrationActionKind.BranchPost,
					sourceBranch: branch, reason: note);
				message = string.Empty;
				return true;
			}
			case "courier":
			case "run":
			{
				if (input.IsFinished)
				{
					message = "Which branch should the courier run start from?";
					return false;
				}

				if (!TryResolveLocation(actor, input.PopSpeech(), out var source, out message))
				{
					return false;
				}

				if (input.IsFinished || !input.PopSpeech().EqualTo("to"))
				{
					message = $"Bank branch courier steps use the syntax: {"bankadmin branch courier <from> to <to> <note>".ColourCommand()}";
					return false;
				}

				if (input.IsFinished || !TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
				{
					return false;
				}

				var note = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(note))
				{
					message = "What courier note should be recorded?";
					return false;
				}

				ConsumeRemaining(input);
				step = new BankAdministrationActionStep(bank, BankAdministrationActionKind.BranchCourier,
					sourceBranch: source, destinationBranch: destination, reason: note);
				message = string.Empty;
				return true;
			}
			default:
				message = "Bank branch administration must be post or courier.";
				return false;
		}
	}

	private static bool TryParseBankAccountStatus(string text, out BankAccountStatus status)
	{
		switch (text.ToLowerInvariant())
		{
			case "active":
				status = BankAccountStatus.Active;
				return true;
			case "suspended":
			case "suspend":
				status = BankAccountStatus.Suspended;
				return true;
			case "locked":
			case "lock":
				status = BankAccountStatus.Locked;
				return true;
			default:
				status = default;
				return false;
		}
	}
	private static bool TryParseStableAdministration(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not IStable stable)
		{
			message = "Stable administration steps can only be drafted for stable employment hosts.";
			return false;
		}

		if (input.IsFinished)
		{
			message = $"Stable administration uses: {"stableadmin care <inspect|feed|groom|exercise> <stay id> <note> OR stableadmin fees <all|stay id> [note] OR stableadmin stay <stay id> <note> OR stableadmin account <account id|name> <note>".ColourCommand()}";
			return false;
		}

		switch (input.PopForSwitch())
		{
			case "care":
			case "welfare":
			{
				if (input.IsFinished || !TryParseStableCareKind(input.PopSpeech(), out var operation))
				{
					message = "Stable care action must be inspect, feed, groom, or exercise.";
					return false;
				}

				if (input.IsFinished)
				{
					message = "Which stable stay should this action target?";
					return false;
				}

				if (!TryResolveStableStay(stable, input.PopSpeech(), out var stay, out message))
				{
					return false;
				}

				var note = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(note))
				{
					message = "What stable care note should be recorded?";
					return false;
				}

				ConsumeRemaining(input);
				step = new StableAdministrationActionStep(stable, operation, stay: stay, note: note);
				message = string.Empty;
				return true;
			}
			case "fees":
			case "fee":
			case "assess":
			{
				IStableStay? stay = null;
				if (!input.IsFinished)
				{
					var selector = input.PopSpeech();
					if (!selector.EqualTo("all"))
					{
						if (!TryResolveStableStay(stable, selector, out stay, out message))
						{
							return false;
						}
					}
				}

				var note = input.SafeRemainingArgument.Trim();
				ConsumeRemaining(input);
				step = new StableAdministrationActionStep(stable, StableAdministrationActionKind.FeeAssessment,
					stay: stay, note: note);
				message = string.Empty;
				return true;
			}
			case "stay":
			case "ticket":
			{
				if (input.IsFinished)
				{
					message = "Which stable stay should this action target?";
					return false;
				}

				if (!TryResolveStableStay(stable, input.PopSpeech(), out var stay, out message))
				{
					return false;
				}

				var note = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(note))
				{
					message = "What stay or ticket reconciliation note should be recorded?";
					return false;
				}

				ConsumeRemaining(input);
				step = new StableAdministrationActionStep(stable, StableAdministrationActionKind.StayReconciliation,
					stay: stay, note: note);
				message = string.Empty;
				return true;
			}
			case "account":
			case "acct":
			{
				if (input.IsFinished)
				{
					message = "Which stable account should this action target?";
					return false;
				}

				if (!TryResolveStableAccount(stable, input.PopSpeech(), out var account, out message))
				{
					return false;
				}

				var note = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(note))
				{
					message = "What account reconciliation note should be recorded?";
					return false;
				}

				ConsumeRemaining(input);
				step = new StableAdministrationActionStep(stable, StableAdministrationActionKind.AccountReconciliation,
					account: account, note: note);
				message = string.Empty;
				return true;
			}
			default:
				message = "Stable administration must target care, fees, stay, or account work.";
				return false;
		}
	}

	private static bool TryParseStableCareKind(string text, out StableAdministrationActionKind operation)
	{
		switch (text.ToLowerInvariant())
		{
			case "inspect":
			case "inspection":
				operation = StableAdministrationActionKind.CareInspect;
				return true;
			case "feed":
			case "feeding":
				operation = StableAdministrationActionKind.CareFeed;
				return true;
			case "groom":
			case "grooming":
				operation = StableAdministrationActionKind.CareGroom;
				return true;
			case "exercise":
			case "exercised":
				operation = StableAdministrationActionKind.CareExercise;
				return true;
			default:
				operation = default;
				return false;
		}
	}

	private static bool TryResolveStableStay(IStable stable, string selector, out IStableStay stay, out string message)
	{
		stay = null!;
		if (!long.TryParse(selector, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
		{
			message = "Stable stay selectors must be numeric stay ids in this slice.";
			return false;
		}

		stay = stable.ActiveStays.FirstOrDefault(x => x.Id == id)!;
		if (stay is null)
		{
			message = $"There is no active stable stay #{id.ToString("N0", CultureInfo.InvariantCulture)} at {stable.Name.ColourName()}.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static bool TryResolveStableAccount(IStable stable, string selector, out IStableAccount account,
		out string message)
	{
		account = stable.AccountByName(selector)!;
		if (account is null)
		{
			message = $"There is no stable account matching {selector.ColourCommand()} at {stable.Name.ColourName()}.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static bool TryParseHotelAdministration(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not IHotel hotel)
		{
			message = "Hotel administration steps can only be drafted for hotel employment hosts.";
			return false;
		}

		if (input.IsFinished)
		{
			message = $"Hotel administration uses: {"hoteladmin room <inspect|clean|ready|maintenance> <room id|name|here> <note> OR hoteladmin lost check [note]|audit <lost #> <note> OR hoteladmin balance <patron id|name> <note>".ColourCommand()}";
			return false;
		}

		switch (input.PopForSwitch())
		{
			case "room":
			{
				if (input.IsFinished || !TryParseHotelRoomKind(input.PopSpeech(), out var operation))
				{
					message = "Hotel room action must be inspect, clean, ready, or maintenance.";
					return false;
				}

				if (input.IsFinished)
				{
					message = "Which hotel room should this action target?";
					return false;
				}

				if (!TryResolveHotelRoom(actor, hotel, input.PopSpeech(), out var room, out message))
				{
					return false;
				}

				var note = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(note))
				{
					message = "What room note should be recorded?";
					return false;
				}

				ConsumeRemaining(input);
				step = new HotelAdministrationActionStep(hotel, operation, room: room, note: note);
				message = string.Empty;
				return true;
			}
			case "lost":
			case "lostproperty":
			{
				if (input.IsFinished)
				{
					message = "Hotel lost-property actions must be check or audit.";
					return false;
				}

				switch (input.PopForSwitch())
				{
					case "check":
					case "sweep":
						var note = input.SafeRemainingArgument.Trim();
						ConsumeRemaining(input);
						step = new HotelAdministrationActionStep(hotel, HotelAdministrationActionKind.LostPropertyCheck,
							note: note);
						message = string.Empty;
						return true;
					case "audit":
					case "inspect":
						if (input.IsFinished)
						{
							message = "Which lost-property bundle should be audited?";
							return false;
						}

						if (!TryResolveHotelLostProperty(hotel, input.PopSpeech(), out var lost, out message))
						{
							return false;
						}

						var auditNote = input.SafeRemainingArgument.Trim();
						if (string.IsNullOrWhiteSpace(auditNote))
						{
							message = "What lost-property audit note should be recorded?";
							return false;
						}

						ConsumeRemaining(input);
						step = new HotelAdministrationActionStep(hotel, HotelAdministrationActionKind.LostPropertyAudit,
							lostProperty: lost, note: auditNote);
						message = string.Empty;
						return true;
					default:
						message = "Hotel lost-property actions must be check or audit.";
						return false;
				}
			}
			case "balance":
			case "patron":
			{
				if (input.IsFinished)
				{
					message = "Which patron balance should be audited?";
					return false;
				}

				var selector = input.PopSpeech();
				if (!TryResolveHotelPatronBalance(hotel, selector, out var balance, out message))
				{
					return false;
				}

				var note = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(note))
				{
					message = "What patron-balance note should be recorded?";
					return false;
				}

				ConsumeRemaining(input);
				step = new HotelAdministrationActionStep(hotel, HotelAdministrationActionKind.PatronBalanceAudit,
					patronBalance: balance, patronSelector: selector, note: note);
				message = string.Empty;
				return true;
			}
			default:
				message = "Hotel administration must target room, lost-property, or patron-balance work.";
				return false;
		}
	}

	private static bool TryParseHotelRoomKind(string text, out HotelAdministrationActionKind operation)
	{
		switch (text.ToLowerInvariant())
		{
			case "inspect":
			case "inspection":
				operation = HotelAdministrationActionKind.RoomInspect;
				return true;
			case "clean":
			case "cleaning":
				operation = HotelAdministrationActionKind.RoomClean;
				return true;
			case "ready":
			case "readiness":
				operation = HotelAdministrationActionKind.RoomReady;
				return true;
			case "maintenance":
			case "maintain":
				operation = HotelAdministrationActionKind.RoomMaintenance;
				return true;
			default:
				operation = default;
				return false;
		}
	}

	private static bool TryResolveHotelRoom(ICharacter actor, IHotel hotel, string selector, out IHotelRoom room,
		out string message)
	{
		room = null!;
		if (selector.EqualTo("here"))
		{
			room = hotel.Rooms.FirstOrDefault(x => x.Cell.Id == actor.Location.Id)!;
		}
		else if (long.TryParse(selector, NumberStyles.Integer, CultureInfo.InvariantCulture, out var cellId))
		{
			room = hotel.Rooms.FirstOrDefault(x => x.Cell.Id == cellId)!;
		}
		else
		{
			room = hotel.Rooms.FirstOrDefault(x => x.Name.EqualTo(selector)) ??
			       hotel.Rooms.FirstOrDefault(x => x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase))!;
		}

		if (room is null)
		{
			message = $"There is no hotel room matching {selector.ColourCommand()} at {hotel.Name.ColourName()}.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static bool TryResolveHotelLostProperty(IHotel hotel, string selector, out IHotelLostProperty lost,
		out string message)
	{
		lost = null!;
		if (!int.TryParse(selector, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index) || index <= 0)
		{
			message = "Lost-property selectors are 1-based list numbers in this slice.";
			return false;
		}

		lost = hotel.Property.HotelLostProperties.ElementAtOrDefault(index - 1)!;
		if (lost is null)
		{
			message = $"There is no lost-property record #{index.ToString("N0", CultureInfo.InvariantCulture)} at {hotel.Name.ColourName()}.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static bool TryResolveHotelPatronBalance(IHotel hotel, string selector, out IHotelPatronBalance balance,
		out string message)
	{
		if (long.TryParse(selector, NumberStyles.Integer, CultureInfo.InvariantCulture, out var patronId))
		{
			balance = hotel.Property.HotelPatronBalances.FirstOrDefault(x => x.PatronId == patronId)!;
		}
		else
		{
			balance = hotel.Property.HotelPatronBalances.FirstOrDefault(x => x.Patron?.Name.EqualTo(selector) == true) ??
			          hotel.Property.HotelPatronBalances.FirstOrDefault(x => x.Patron?.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase) == true)!;
		}

		if (balance is null)
		{
			message = $"There is no patron balance matching {selector.ColourCommand()} at {hotel.Name.ColourName()}.";
			return false;
		}

		message = string.Empty;
		return true;
	}
	private static bool TryParseArenaEventAdministration(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not ICombatArena arena)
		{
			message = "Arena event steps can only be used from combat arena employment hosts.";
			return false;
		}

		if (input.IsFinished)
		{
			message = $"Arena event steps use the syntax: {"tasks step arenaevent create <event type id|name> at <date/time> OR arenaevent phase <event id|name> <state> OR arenaevent abort <event id|name> <reason>".ColourCommand()}.";
			return false;
		}

		var operation = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (operation)
		{
			case "create":
			case "schedule":
			case "new":
			{
				var typeText = string.Join(" ", PopTokensUntilAny(input, ["at", "when", "for"])).Trim();
				if (string.IsNullOrWhiteSpace(typeText) || input.IsFinished || !input.PopSpeech().EqualToAny("at", "when", "for"))
				{
					message = $"Arena event creation uses the syntax: {"tasks step arenaevent create <event type id|name> at <date/time>".ColourCommand()}.";
					return false;
				}

				if (!DateUtilities.TryParseDateTimeOrRelative(input.SafeRemainingArgument, actor.Account, true,
					    out var scheduledForUtc))
				{
					message = "That is not a valid arena event date/time.";
					return false;
				}

				var eventType = ResolveArenaEventType(arena, typeText);
				if (eventType is null)
				{
					message = $"There is no arena event type matching {typeText.ColourCommand()} for {arena.Name.ColourName()}.";
					return false;
				}

				ConsumeRemaining(input);
				step = new ArenaEventAdministrationActionStep(arena, eventType, scheduledForUtc);
				message = string.Empty;
				return true;
			}
			case "phase":
			case "state":
			case "transition":
			case "advance":
			{
				var tokens = PopRemainingTokens(input).ToList();
				if (tokens.Count < 2)
				{
					message = $"Arena event phase steps use the syntax: {"tasks step arenaevent phase <event id|name> <state>".ColourCommand()}.";
					return false;
				}

				var stateText = tokens[^1];
				var eventTokens = tokens.Take(tokens.Count - 1).ToList();
				if (eventTokens.Count > 1 && eventTokens[^1].EqualToAny("to", "state", "phase"))
				{
					eventTokens.RemoveAt(eventTokens.Count - 1);
				}

				var eventText = string.Join(" ", eventTokens).Trim();
				if (string.IsNullOrWhiteSpace(eventText) || !TryParseArenaEventState(stateText, out var targetState))
				{
					message = $"Arena event phase steps use the syntax: {"tasks step arenaevent phase <event id|name> <state>".ColourCommand()}.";
					return false;
				}

				var arenaEvent = ResolveArenaEvent(arena, eventText);
				if (arenaEvent is null)
				{
					message = $"There is no active arena event matching {eventText.ColourCommand()} for {arena.Name.ColourName()}.";
					return false;
				}

				step = new ArenaEventAdministrationActionStep(arena, arenaEvent, targetState);
				message = string.Empty;
				return true;
			}
			case "abort":
			case "cancel":
			{
				if (input.IsFinished)
				{
					message = $"Arena event abort steps use the syntax: {"tasks step arenaevent abort <event id|name> <reason>".ColourCommand()}.";
					return false;
				}

				var eventText = input.PopSpeech();
				var reason = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(reason))
				{
					message = "Arena event abort steps require a reason after the event selector.";
					return false;
				}

				var arenaEvent = ResolveArenaEvent(arena, eventText);
				if (arenaEvent is null)
				{
					message = $"There is no active arena event matching {eventText.ColourCommand()} for {arena.Name.ColourName()}.";
					return false;
				}

				ConsumeRemaining(input);
				step = new ArenaEventAdministrationActionStep(arena, arenaEvent, reason);
				message = string.Empty;
				return true;
			}
			default:
				message = $"Unknown arena event operation {operation.ColourCommand()}. Use create, phase, or abort.";
				return false;
		}
	}

	private static IArenaEventType? ResolveArenaEventType(ICombatArena arena, string target)
	{
		if (long.TryParse(target.TrimStart('#'), out var id))
		{
			return arena.EventTypes.FirstOrDefault(x => x.Id == id);
		}

		return arena.EventTypes.FirstOrDefault(x => x.Name.EqualTo(target)) ??
		       arena.EventTypes.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.InvariantCultureIgnoreCase));
	}

	private static IArenaEvent? ResolveArenaEvent(ICombatArena arena, string target)
	{
		if (long.TryParse(target.TrimStart('#'), out var id))
		{
			return arena.ActiveEvents.FirstOrDefault(x => x.Id == id);
		}

		return arena.ActiveEvents.FirstOrDefault(x => x.Name.EqualTo(target)) ??
		       arena.ActiveEvents.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.InvariantCultureIgnoreCase));
	}

	private static bool TryParseArenaEventState(string text, out ArenaEventState state)
	{
		state = ArenaEventState.Draft;
		if (text.TryParseEnum(out state))
		{
			return true;
		}

		switch (text.CollapseString().ToLowerInvariant())
		{
			case "registration":
			case "open":
			case "regopen":
				state = ArenaEventState.RegistrationOpen;
				return true;
			case "prep":
			case "preparation":
				state = ArenaEventState.Preparing;
				return true;
			case "stage":
				state = ArenaEventState.Staged;
				return true;
			case "fight":
			case "combat":
				state = ArenaEventState.Live;
				return true;
			case "resolve":
				state = ArenaEventState.Resolving;
				return true;
			case "clean":
				state = ArenaEventState.Cleanup;
				return true;
			case "complete":
				state = ArenaEventState.Completed;
				return true;
			case "abort":
			case "cancelled":
				state = ArenaEventState.Aborted;
				return true;
			default:
				return false;
		}
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
			message = $"Load steps use the syntax: {"tasks step load all into <prototype id|*item id|&tag|keyword> [at <here|cell id>]".ColourCommand()}";
			return false;
		}

		if (!TryParseItemSelector(actor, input, "load target container", out var selector, out message))
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

			if (input.IsFinished)
			{
				message = "Which destination should this load step use?";
				return false;
			}

			if (!TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
			{
				return false;
			}

			location = destination;
		}

		if (!input.IsFinished)
		{
			message = $"Could not understand the extra text {input.SafeRemainingArgument.ColourCommand()} in the load step.";
			return false;
		}

		step = new LoadItemsActionStep(selector, location);
		message = string.Empty;
		return true;
	}

	private static bool TryParseUnload(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (!TryParseItemSelector(actor, input, "unload source container", out var selector, out message))
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

			if (input.IsFinished)
			{
				message = "Which destination should this unload step use?";
				return false;
			}

			if (!TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
			{
				return false;
			}

			location = destination;
		}

		if (!input.IsFinished)
		{
			message = $"Could not understand the extra text {input.SafeRemainingArgument.ColourCommand()} in the unload step.";
			return false;
		}

		step = new UnloadItemsActionStep(selector, location);
		message = string.Empty;
		return true;
	}

	private static bool TryParseReturn(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Return steps use the syntax: {"tasks step return container <prototype id|*item id|&tag|keyword> to <here|cell id> [container <prototype id|*item id|&tag|keyword>]".ColourCommand()}";
			return false;
		}

		var assetKind = input.PopSpeech();
		if (!assetKind.EqualTo("container"))
		{
			message = $"{assetKind.ColourCommand()} return is not executable yet. This slice supports {"return container".ColourCommand()}; vehicle driving and animal leading remain deferred.";
			return false;
		}

		if (!TryParseItemSelector(actor, input, "return container", out var selector, out message))
		{
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualTo("to"))
		{
			message = $"Return steps use the syntax: {"tasks step return container <prototype id|*item id|&tag|keyword> to <here|cell id> [container <prototype id|*item id|&tag|keyword>]".ColourCommand()}";
			return false;
		}

		if (input.IsFinished || !TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
		{
			return false;
		}

		EmploymentItemSelector? destinationContainerSelector = null;
		while (!input.IsFinished)
		{
			var option = input.PopSpeech();
			if (option.EqualTo("container"))
			{
				if (destinationContainerSelector is not null)
				{
					message = "Specify only one destination container selector.";
					return false;
				}

				if (!TryParseItemSelector(actor, input, "destination container", out destinationContainerSelector,
					    out message))
				{
					return false;
				}

				continue;
			}

			if (option.EqualTo("containertag"))
			{
				if (destinationContainerSelector is not null)
				{
					message = "Specify only one destination container selector.";
					return false;
				}

				if (input.IsFinished)
				{
					message = "Which destination container tag do you want to return to?";
					return false;
				}

				if (!TryParseTagSelector(actor, input.PopSpeech(), "destination container",
					    out destinationContainerSelector, out message))
				{
					return false;
				}

				continue;
			}

			message = $"The return option {option.ColourCommand()} is not valid.";
			return false;
		}

		step = new ReturnAssetActionStep(selector, destination, destinationContainerSelector);
		message = string.Empty;
		return true;
	}

	private static bool TryParseAnimal(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = AnimalStepSyntax();
			return false;
		}

		var operation = input.PopSpeech();
		switch (operation.ToLowerInvariant())
		{
			case "lead":
			{
				if (input.IsFinished)
				{
					message = "Which animal should this lead step manage?";
					return false;
				}

				var mountText = input.PopSpeech();
				if (input.IsFinished || !input.PopSpeech().EqualTo("to"))
				{
					message = $"Animal lead steps use the syntax: {"tasks step animal lead <mount id|name> to <here|cell id>".ColourCommand()}";
					return false;
				}

				if (input.IsFinished)
				{
					message = "Which destination should this lead step use?";
					return false;
				}

				if (!TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
				{
					return false;
				}

				if (!input.IsFinished)
				{
					message = $"Could not understand the extra text {input.SafeRemainingArgument.ColourCommand()} in the animal lead step.";
					return false;
				}

				if (!TryResolveEmploymentMount(actor, mountText, out var mount, out message))
				{
					return false;
				}

				step = new StableAnimalOperationActionStep(EmploymentAnimalOperationKind.Lead, mount, destination: destination);
				message = string.Empty;
				return true;
			}
			case "ride":
			{
				var mountText = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(mountText))
				{
					message = $"Animal ride steps use the syntax: {"tasks step animal ride <mount id|name>".ColourCommand()}";
					return false;
				}

				ConsumeRemaining(input);
				if (!TryResolveEmploymentMount(actor, mountText, out var mount, out message))
				{
					return false;
				}

				step = new StableAnimalOperationActionStep(EmploymentAnimalOperationKind.Ride, mount);
				message = string.Empty;
				return true;
			}
			case "lodge":
			{
				if (input.IsFinished)
				{
					message = "Which animal should this lodge step manage?";
					return false;
				}

				var mountText = input.PopSpeech();
				if (input.IsFinished || !input.PopSpeech().EqualTo("at"))
				{
					message = $"Animal lodge steps use the syntax: {"tasks step animal lodge <mount id|name> at <stable id|name>".ColourCommand()}";
					return false;
				}

				var stableText = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(stableText))
				{
					message = "Which stable should lodge this animal?";
					return false;
				}

				ConsumeRemaining(input);
				if (!TryResolveEmploymentMount(actor, mountText, out var mount, out message) ||
				    !TryResolveStable(actor, stableText, out var stable, out message))
				{
					return false;
				}

				step = new StableAnimalOperationActionStep(EmploymentAnimalOperationKind.Lodge, mount, stable);
				message = string.Empty;
				return true;
			}
			case "return":
			{
				if (input.IsFinished || !long.TryParse(input.PopSpeech(), out var stayId))
				{
					message = $"Animal return steps use the syntax: {"tasks step animal return <stay id> from <stable id|name> [waive]".ColourCommand()}";
					return false;
				}

				if (input.IsFinished || !input.PopSpeech().EqualTo("from"))
				{
					message = $"Animal return steps use the syntax: {"tasks step animal return <stay id> from <stable id|name> [waive]".ColourCommand()}";
					return false;
				}

				var stableText = input.SafeRemainingArgument.Trim();
				var waiveFees = false;
				if (stableText.EndsWith(" waive", StringComparison.InvariantCultureIgnoreCase))
				{
					waiveFees = true;
					stableText = stableText[..^6].Trim();
				}

				if (string.IsNullOrWhiteSpace(stableText))
				{
					message = "Which stable should return this animal?";
					return false;
				}

				ConsumeRemaining(input);
				if (!TryResolveStable(actor, stableText, out var stable, out message))
				{
					return false;
				}

				var stay = stable.Stays.FirstOrDefault(x => x.Id == stayId);
				if (stay is null)
				{
					message = $"Stable {stable.Name.ColourName()} has no stay with id {stayId.ToString("N0", actor).ColourValue()}.";
					return false;
				}

				step = new StableAnimalOperationActionStep(EmploymentAnimalOperationKind.Return, stable: stable,
					stay: stay, waiveFees: waiveFees);
				message = string.Empty;
				return true;
			}
			default:
				message = AnimalStepSyntax();
				return false;
		}
	}

	private static string AnimalStepSyntax()
	{
		return $"Animal steps use the syntax: {"tasks step animal lead <mount id|name> to <here|cell id>".ColourCommand()}, {"animal ride <mount id|name>".ColourCommand()}, {"animal lodge <mount id|name> at <stable id|name>".ColourCommand()}, or {"animal return <stay id> from <stable id|name> [waive]".ColourCommand()}";
	}

	private static bool TryResolveEmploymentMount(ICharacter actor, string text, out ICharacter mount, out string message)
	{
		mount = null!;
		if (long.TryParse(text, out var id))
		{
			mount = actor.Gameworld.TryGetCharacter(id, true);
		}
		else
		{
			mount = actor.Gameworld.Characters.GetByIdOrName(text) ?? actor.Gameworld.Characters.GetByPersonalName(text);
		}

		if (mount is null)
		{
			message = $"There is no animal or mount matching {text.ColourCommand()}.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static bool TryResolveStable(ICharacter actor, string text, out IStable stable, out string message)
	{
		stable = actor.Gameworld.Stables.GetByIdOrName(text)!;
		if (stable is null)
		{
			message = $"There is no stable matching {text.ColourCommand()}.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static bool TryParseVehicle(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Vehicle steps use the syntax: {"tasks step vehicle assign <vehicle id|exterior item id>".ColourCommand()} or {"tasks step vehicle cargo <vehicle id|exterior item id> <cargo id|cargo name>".ColourCommand()}";
			return false;
		}

		var operation = input.PopSpeech();
		if (!operation.EqualToAny("assign", "driver", "cargo"))
		{
			message = $"Vehicle steps use the syntax: {"tasks step vehicle assign <vehicle id|exterior item id>".ColourCommand()} or {"tasks step vehicle cargo <vehicle id|exterior item id> <cargo id|cargo name>".ColourCommand()}";
			return false;
		}

		if (input.IsFinished || !long.TryParse(input.PopSpeech(), out var vehicleId))
		{
			message = "Which vehicle id or exterior item id should this vehicle step select?";
			return false;
		}

		var vehicle = ResolveVehicle(actor, vehicleId);
		if (vehicle is null)
		{
			message = $"There is no vehicle or vehicle exterior item with id {vehicleId.ToString("N0", actor).ColourValue()}.";
			return false;
		}

		if (operation.EqualToAny("assign", "driver"))
		{
			if (!input.IsFinished)
			{
				message = $"Could not understand the extra text {input.SafeRemainingArgument.ColourCommand()} in the vehicle assignment step.";
				return false;
			}

			step = new VehicleOperationActionStep(vehicle);
			message = string.Empty;
			return true;
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

	private static bool TryParseSupplierSelection(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (!TryParsePurchase(actor, host, input, out var parsed, out message))
		{
			return false;
		}

		if (parsed is not PurchaseActionStep purchase || !purchase.IsExecutablePurchase)
		{
			message = $"Supplier steps use the executable purchase-target syntax: {"tasks step supplier <quantity> <merchandise id|name> from <shop id|name|any> [max <amount>]".ColourCommand()}.";
			return false;
		}

		step = new SupplierSelectionActionStep(purchase);
		message = string.Empty;
		return true;
	}
	private static bool TryParsePurchase(ICharacter actor, IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		var raw = input.SafeRemainingArgument.Trim();
		if (input.IsFinished)
		{
			return TryParseLegacyPurchaseAudit(host, new StringStack(raw), out step, out message);
		}

		var first = input.PopSpeech();
		if (double.TryParse(first, actor, out var commodityWeight) && commodityWeight > 0.0 &&
		    !input.IsFinished && input.PeekSpeech().EqualTo("commodity"))
		{
			input.PopSpeech();
			var descriptorTokens = PopTokensUntil(input, "from").ToList();
			if (!descriptorTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("from"))
			{
				message = $"Commodity purchase steps use the syntax: {"tasks step purchase <weight> commodity <descriptor> from <shop id|name|any> [max <amount>]".ColourCommand()}";
				return false;
			}

			if (!TryParsePurchaseSupplierAndOptions(host, input, out var supplier, out var maximum, out var keyword,
				    out message))
			{
				return false;
			}

			var commodityCurrency = ResolveHostCurrency(host);
			if (commodityCurrency is null)
			{
				message = $"Could not determine the currency for {host.EmploymentHostName.ColourName()}.";
				return false;
			}

			step = new PurchaseActionStep(commodityWeight, string.Join(" ", descriptorTokens), supplier, commodityCurrency,
				maximum, keyword);
			message = string.Empty;
			return true;
		}

		if (!int.TryParse(first, out var quantity) || quantity <= 0)
		{
			return TryParseLegacyPurchaseAudit(host, new StringStack(raw), out step, out message);
		}

		if (!input.IsFinished && input.PeekSpeech().EqualTo("item"))
		{
			input.PopSpeech();
			if (!TryParseItemSelector(actor, input, "purchase item", out var itemSelector, out message) ||
			    itemSelector is null)
			{
				return false;
			}

			if (input.IsFinished || !input.PopSpeech().EqualTo("from"))
			{
				message = $"Item purchase steps use the syntax: {"tasks step purchase <quantity> item <prototype id|*item id|&tag|keyword> from <shop id|name|any> [max <amount>]".ColourCommand()}";
				return false;
			}

			if (!TryParsePurchaseSupplierAndOptions(host, input, out var supplier, out var maximum, out var keyword,
				    out message))
			{
				return false;
			}

			var itemCurrency = ResolveHostCurrency(host);
			if (itemCurrency is null)
			{
				message = $"Could not determine the currency for {host.EmploymentHostName.ColourName()}.";
				return false;
			}

			step = new PurchaseActionStep(quantity, itemSelector, supplier, itemCurrency, maximum, keyword);
			message = string.Empty;
			return true;
		}

		var merchandiseTokens = PopTokensUntil(input, "from").ToList();
		if (!merchandiseTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("from"))
		{
			if (raw.Contains(" for ", StringComparison.InvariantCultureIgnoreCase))
			{
				return TryParseLegacyPurchaseAudit(host, new StringStack(raw), out step, out message);
			}

			message = $"Purchase steps use the syntax: {"tasks step purchase <quantity> <merchandise id|name> from <shop id|name|any> [max <amount>] [keyword <keywords>]".ColourCommand()}";
			return false;
		}

		if (!TryParsePurchaseSupplierAndOptions(host, input, out var supplierSelector, out var maximumAmount,
			    out var keywordFilter, out message))
		{
			return false;
		}

		var merchandiseCurrency = ResolveHostCurrency(host);
		if (merchandiseCurrency is null)
		{
			message = $"Could not determine the currency for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		step = new PurchaseActionStep(quantity, string.Join(" ", merchandiseTokens),
			supplierSelector, merchandiseCurrency, maximumAmount, keywordFilter);
		message = string.Empty;
		return true;
	}

	private static bool TryParsePurchaseSupplierAndOptions(IEmploymentHost host, StringStack input,
		out string supplierSelector, out MoneyAmount? maximum, out string? keyword, out string message)
	{
		supplierSelector = string.Empty;
		maximum = null;
		keyword = null;
		var supplierTokens = new List<string>();
		while (!input.IsFinished && !IsAny(input.PeekSpeech(), "max", "keyword"))
		{
			supplierTokens.Add(input.PopSpeech());
		}

		if (!supplierTokens.Any())
		{
			message = "Which supplier shop should this purchase use? Use " + "any".ColourCommand() + " to search all shops.";
			return false;
		}

		supplierSelector = string.Join(" ", supplierTokens);
		while (!input.IsFinished)
		{
			var option = input.PopSpeech();
			if (option.EqualTo("max"))
			{
				var amountTokens = new List<string>();
				while (!input.IsFinished && !input.PeekSpeech().EqualTo("keyword"))
				{
					amountTokens.Add(input.PopSpeech());
				}

				if (!TryParseMoney(host, string.Join(" ", amountTokens), out maximum, out message))
				{
					return false;
				}

				continue;
			}

			if (option.EqualTo("keyword"))
			{
				keyword = input.SafeRemainingArgument.Trim();
				ConsumeRemaining(input);
				break;
			}

			message = $"Unknown purchase option {option.ColourCommand()}.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	private static bool TryParseLegacyPurchaseAudit(IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
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

	private static bool TryParseBankTransfer(IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		var amountTokens = PopTokensUntil(input, "to").ToList();
		if (!amountTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("to"))
		{
			message = $"Bank transfer steps use the syntax: {"tasks step transfer <amount> to <bank account id|bankcode:account|alias>".ColourCommand()}";
			return false;
		}

		if (!TryParseMoney(host, string.Join(" ", amountTokens), out var amount, out message))
		{
			return false;
		}

		var targetAccountKey = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(targetAccountKey))
		{
			message = "Which target bank account should this transfer use?";
			return false;
		}

		ConsumeRemaining(input);
		step = new BankAccountTransferActionStep(targetAccountKey, amount);
		message = string.Empty;
		return true;
	}
	private static bool TryParseHostSettlement(IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		var amountTokens = PopTokensUntil(input, "to").ToList();
		if (!amountTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("to"))
		{
			message = $"Host settlement steps use the syntax: {"tasks step settle <amount> to <host type> <id|name>".ColourCommand()}";
			return false;
		}

		if (!TryParseMoney(host, string.Join(" ", amountTokens), out var amount, out message))
		{
			return false;
		}

		if (input.IsFinished)
		{
			message = "Which target employment host type should this settlement use?";
			return false;
		}

		var hostType = input.PopSpeech();
		var identifier = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(identifier))
		{
			message = "Which target employment host should this settlement use?";
			return false;
		}

		ConsumeRemaining(input);
		step = new HostSettlementActionStep($"{hostType.CollapseString().ToLowerInvariant()}:{identifier}", amount);
		message = string.Empty;
		return true;
	}
	private static bool TryParseStorePay(IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		var raw = input.SafeRemainingArgument.Trim();
		if (input.IsFinished)
		{
			message = $"Store-account payment steps use the syntax: {"tasks step storepay <shop id|name> account <account id|name> amount <amount>".ColourCommand()}";
			return false;
		}

		var shopTokens = PopTokensUntil(input, "account").ToList();
		if (!shopTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("account"))
		{
			return TryParseLegacyStorePay(host, new StringStack(raw), out step, out message);
		}

		var accountTokens = PopTokensUntil(input, "amount").ToList();
		if (!accountTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("amount"))
		{
			message = $"Store-account payment steps use the syntax: {"tasks step storepay <shop id|name> account <account id|name> amount <amount>".ColourCommand()}";
			return false;
		}

		var gameworld = (host as IHaveFuturemud)?.Gameworld;
		var shopSelector = string.Join(" ", shopTokens).Trim();
		var shop = gameworld?.Shops.GetByIdOrName(shopSelector);
		if (shop is null)
		{
			message = $"There is no shop matching {shopSelector.ColourCommand()}.";
			return false;
		}

		var accountSelector = string.Join(" ", accountTokens).Trim();
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

		if (!TryParseMoney(host, input.SafeRemainingArgument, out var amount, out message))
		{
			return false;
		}

		ConsumeRemaining(input);
		step = new StoreAccountPaymentActionStep(ShopAccountOwingCondition.CreateKey(shop, account), amount);
		message = string.Empty;
		return true;
	}

	private static bool TryParseLegacyStorePay(IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Store-account payment steps use the syntax: {"tasks step storepay <shop id|name> account <account id|name> amount <amount>".ColourCommand()}";
			return false;
		}

		var accountName = input.PopSpeech();
		if (input.IsFinished || !input.PopSpeech().EqualTo("amount"))
		{
			message = $"Store-account payment steps use the syntax: {"tasks step storepay <shop id|name> account <account id|name> amount <amount>".ColourCommand()}";
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

	private static bool TryParsePayTax(IEmploymentHost host, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Tax payment steps use the syntax: {"tasks step paytax <amount|all>".ColourCommand()}";
			return false;
		}

		if (input.PeekSpeech().EqualTo("all"))
		{
			input.PopSpeech();
			step = new TaxPaymentActionStep(null);
			message = string.Empty;
			return true;
		}

		if (!TryParseMoney(host, input.SafeRemainingArgument, out var amount, out message))
		{
			return false;
		}

		ConsumeRemaining(input);
		step = new TaxPaymentActionStep(amount);
		message = string.Empty;
		return true;
	}

	private static bool TryParsePayrollSettlement(StringStack input, out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (!input.IsFinished && input.PeekSpeech().EqualTo("settle"))
		{
			input.PopSpeech();
		}

		if (input.IsFinished)
		{
			message = $"Payroll settlement steps use the syntax: {"tasks step payroll settle <all|#payable> [reason]".ColourCommand()}";
			return false;
		}

		var selector = input.PopSpeech();
		var reason = input.SafeRemainingArgument.Trim();
		ConsumeRemaining(input);
		step = new PayrollSettlementActionStep(selector, reason);
		message = string.Empty;
		return true;
	}

	private static bool TryParseCashReconciliation(IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not IShop)
		{
			message = "Cash reconciliation steps can only be authored for shop employment hosts.";
			return false;
		}

		var note = input.SafeRemainingArgument.Trim();
		ConsumeRemaining(input);
		step = new ShopCashReconciliationActionStep(note);
		message = string.Empty;
		return true;
	}
	private static bool TryParseShopFloat(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (input.IsFinished || !IsAny(input.PeekSpeech(), "fill", "skim"))
		{
			message = $"Shop float steps use the syntax: {"tasks step float fill|skim <amount> [register <prototype|*item|&tag|keyword>]".ColourCommand()}";
			return false;
		}

		var fill = input.PopSpeech().EqualTo("fill");
		var amountTokens = new List<string>();
		while (!input.IsFinished && !IsAny(input.PeekSpeech(), "register", "till", "cashregister"))
		{
			amountTokens.Add(input.PopSpeech());
		}

		if (!TryParseMoney(host, string.Join(" ", amountTokens), out var amount, out message))
		{
			return false;
		}

		EmploymentItemSelector? registerSelector = null;
		if (!input.IsFinished)
		{
			input.PopSpeech();
			if (!TryParseItemSelector(actor, input, "cash register", out registerSelector, out message))
			{
				return false;
			}
		}

		step = new ShopFloatAdjustmentActionStep(fill, amount, registerSelector);
		message = string.Empty;
		return true;
	}

	private static bool TryParsePhysicalFloat(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (input.IsFinished || !input.PopSpeech().TryParseEnum<PhysicalFloatOperation>(out var operation))
		{
			message = $"Physical float steps use the syntax: {"tasks step physicalfloat issue <amount> from <bank|register> OR physicalfloat return <amount|all> to <bank|register|container <selector>> OR physicalfloat settle <amount|all>".ColourCommand()}";
			return false;
		}

		MoneyAmount? amount = null;
		if (operation == PhysicalFloatOperation.Issue)
		{
			var amountTokens = PopTokensUntil(input, "from").ToList();
			if (!amountTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("from"))
			{
				message = $"Issue-float steps use the syntax: {"tasks step physicalfloat issue <amount> from <bank|register>".ColourCommand()}";
				return false;
			}

			if (!TryParseMoney(host, string.Join(" ", amountTokens), out amount, out message))
			{
				return false;
			}

			if (input.IsFinished)
			{
				message = "Which source should issue the physical float? Use bank or register.";
				return false;
			}

			var source = input.PopSpeech();
			step = new PhysicalFloatActionStep(operation, amount, source);
			message = string.Empty;
			return true;
		}

		if (operation == PhysicalFloatOperation.Settle)
		{
			if (!input.IsFinished && !input.PeekSpeech().EqualTo("all"))
			{
				if (!TryParseMoney(host, input.SafeRemainingArgument, out amount, out message))
				{
					return false;
				}
			}

			ConsumeRemaining(input);
			step = new PhysicalFloatActionStep(operation, amount, "virtual");
			message = string.Empty;
			return true;
		}

		var returnTokens = PopTokensUntil(input, "to").ToList();
		if (!returnTokens.Any() || input.IsFinished || !input.PopSpeech().EqualTo("to"))
		{
			message = $"Return-float steps use the syntax: {"tasks step physicalfloat return <amount|all> to <bank|register|container <selector>>".ColourCommand()}";
			return false;
		}

		var returnAmountText = string.Join(" ", returnTokens);
		if (!returnAmountText.EqualTo("all"))
		{
			if (!TryParseMoney(host, returnAmountText, out amount, out message))
			{
				return false;
			}
		}

		if (input.IsFinished)
		{
			message = "Where should the physical float be returned?";
			return false;
		}

		var target = input.PopSpeech();
		EmploymentItemSelector? selector = null;
		if (target.EqualTo("container"))
		{
			if (!TryParseItemSelector(actor, input, "physical float return container", out selector, out message))
			{
				return false;
			}
		}

		step = new PhysicalFloatActionStep(operation, amount, target, selector);
		message = string.Empty;
		return true;
	}

	private static bool TryParseManagerGoalAdministration(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Manager-goal administration steps use the syntax: {"tasks step goal evaluate|cancel|reactivate <#|type|description> [reason]".ColourCommand()}.";
			return false;
		}

		var operationText = input.PopSpeech().CollapseString().ToLowerInvariant();
		ManagerGoalAdministrationActionKind operation;
		switch (operationText)
		{
			case "evaluate":
			case "eval":
			case "run":
				operation = ManagerGoalAdministrationActionKind.Evaluate;
				break;
			case "cancel":
			case "delete":
			case "remove":
				operation = ManagerGoalAdministrationActionKind.Cancel;
				break;
			case "reactivate":
			case "activate":
			case "resume":
			case "retry":
			case "unblock":
				operation = ManagerGoalAdministrationActionKind.Reactivate;
				break;
			default:
				message = $"Unknown manager-goal administration operation {operationText.ColourCommand()}. Use {"evaluate".ColourCommand()}, {"cancel".ColourCommand()}, or {"reactivate".ColourCommand()}.";
				return false;
		}

		if (input.IsFinished)
		{
			message = "Which manager goal should this step administer?";
			return false;
		}

		var selector = input.PopSpeech();
		var goal = ResolveManagerGoalForStep(host, selector);
		if (goal is null)
		{
			message = $"There is no manager goal matching {selector.ColourCommand()}.";
			return false;
		}

		var reason = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(reason))
		{
			reason = null;
		}

		ConsumeRemaining(input);
		step = new ManagerGoalAdministrationActionStep(operation, goal.Id, goal.Configuration.Description, reason);
		message = string.Empty;
		return true;
	}

	private static IManagerGoal? ResolveManagerGoalForStep(IEmploymentHost host, string selector)
	{
		var goals = host.ManagerGoalBoard.Goals
			.OrderBy(x => x.Priority)
			.ThenBy(x => x.Id)
			.ToList();
		if (!goals.Any())
		{
			return null;
		}

		selector = selector.Trim();
		if (TryParseCommandNumber(selector, out var id))
		{
			return goals.FirstOrDefault(x => x.Id == id);
		}

		var definition = EmploymentManagerGoalCatalog.Get(selector);
		if (definition is not null)
		{
			return goals.FirstOrDefault(x => x.GoalType == definition.GoalType && x.Status == ManagerGoalStatus.Active) ??
			       goals.FirstOrDefault(x => x.GoalType == definition.GoalType);
		}

		return goals.FirstOrDefault(x => x.GoalType.ToString().EqualTo(selector)) ??
		       goals.FirstOrDefault(x => x.Configuration.Description.EqualTo(selector)) ??
		       goals.FirstOrDefault(x => x.Configuration.Description.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
	}

	private static bool TryParseActiveTaskAdministration(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Active-task administration steps use the syntax: {"tasks step admintask retry|requeue|cancel <#|name|id> [reason]".ColourCommand()} or {"tasks step admintask assign <#|name|id> to <employee id|name> [reason]".ColourCommand()}.";
			return false;
		}

		var operationText = input.PopSpeech().CollapseString().ToLowerInvariant();
		ActiveTaskAdministrationActionKind operation;
		switch (operationText)
		{
			case "retry":
			case "rerun":
				operation = ActiveTaskAdministrationActionKind.Retry;
				break;
			case "requeue":
			case "release":
			case "unassign":
				operation = ActiveTaskAdministrationActionKind.Requeue;
				break;
			case "assign":
			case "reassign":
				operation = ActiveTaskAdministrationActionKind.Assign;
				break;
			case "cancel":
			case "abort":
				operation = ActiveTaskAdministrationActionKind.Cancel;
				break;
			default:
				message = $"Unknown active-task administration operation {operationText.ColourCommand()}. Use {"retry".ColourCommand()}, {"requeue".ColourCommand()}, {"assign".ColourCommand()}, or {"cancel".ColourCommand()}.";
				return false;
		}

		if (input.IsFinished)
		{
			message = "Which active task should this step administer?";
			return false;
		}

		var selector = input.PopSpeech();
		var task = ResolveActiveTaskForStep(host, selector);
		if (task is null)
		{
			message = $"There is no active employment task matching {selector.ColourCommand()}.";
			return false;
		}

		long? employeeId = null;
		string? employeeName = null;
		if (operation == ActiveTaskAdministrationActionKind.Assign)
		{
			if (input.IsFinished || !input.PopSpeech().EqualTo("to"))
			{
				message = $"Active-task assignment steps use the syntax: {"tasks step admintask assign <#|name|id> to <employee id|name> [reason]".ColourCommand()}.";
				return false;
			}

			if (input.IsFinished)
			{
				message = "Which employee should this task be assigned to?";
				return false;
			}

			var employeeSelector = input.PopSpeech();
			var employee = ResolveActiveTaskEmployeeForStep(host, actor, employeeSelector);
			if (employee is null)
			{
				message = $"There is no active employee matching {employeeSelector.ColourCommand()}.";
				return false;
			}

			employeeId = employee.Id;
			employeeName = employee.Name;
		}

		var reason = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(reason))
		{
			reason = null;
		}

		ConsumeRemaining(input);
		step = new ActiveTaskAdministrationActionStep(operation, task.Id, task.Name, employeeId, employeeName, reason);
		message = string.Empty;
		return true;
	}

	private static IEmploymentActiveTask? ResolveActiveTaskForStep(IEmploymentHost host, string selector)
	{
		var tasks = host.TaskBoard.ActiveTasks
			.OrderBy(x => x.Name)
			.ToList();
		if (!tasks.Any())
		{
			return null;
		}

		selector = selector.Trim();
		if (selector.StartsWith("#") && int.TryParse(selector[1..], out var number))
		{
			return number > 0 && number <= tasks.Count ? tasks[number - 1] : null;
		}

		if (Guid.TryParse(selector, out var id))
		{
			return tasks.FirstOrDefault(x => x.Id == id);
		}

		return tasks.FirstOrDefault(x => x.Name.EqualTo(selector)) ??
		       tasks.FirstOrDefault(x => x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
	}

	private static ICharacter? ResolveActiveTaskEmployeeForStep(IEmploymentHost host, ICharacter actor, string selector)
	{
		selector = selector.Trim();
		if (TryParseCommandNumber(selector, out var id))
		{
			return host.EmploymentContracts
			           .Where(x => x.Status == EmploymentStatus.Active)
			           .Select(x => x.Employee)
			           .FirstOrDefault(x => x.Id == id) ??
			       actor.Gameworld?.TryGetCharacter(id, true);
		}

		return host.EmploymentContracts
		           .Where(x => x.Status == EmploymentStatus.Active)
		           .Select(x => x.Employee)
		           .FirstOrDefault(x => x.Name.EqualTo(selector)) ??
		       host.EmploymentContracts
		           .Where(x => x.Status == EmploymentStatus.Active)
		           .Select(x => x.Employee)
		           .FirstOrDefault(x => x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
	}

	private static bool TryParseScheduledRuleAdministration(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Scheduled-rule administration steps use the syntax: {"tasks step rule pause|resume|cancel <#|name|id> [reason]".ColourCommand()} or {"tasks step rule evaluate <#|name|id> [manual <key>]".ColourCommand()}.";
			return false;
		}

		var operationText = input.PopSpeech().CollapseString().ToLowerInvariant();
		ScheduledRuleAdministrationActionKind operation;
		switch (operationText)
		{
			case "pause":
			case "suspend":
				operation = ScheduledRuleAdministrationActionKind.Pause;
				break;
			case "resume":
			case "activate":
			case "unpause":
				operation = ScheduledRuleAdministrationActionKind.Resume;
				break;
			case "cancel":
			case "delete":
			case "remove":
				operation = ScheduledRuleAdministrationActionKind.Cancel;
				break;
			case "evaluate":
			case "eval":
			case "run":
				operation = ScheduledRuleAdministrationActionKind.Evaluate;
				break;
			default:
				message = $"Unknown scheduled-rule administration operation {operationText.ColourCommand()}. Use {"pause".ColourCommand()}, {"resume".ColourCommand()}, {"cancel".ColourCommand()}, or {"evaluate".ColourCommand()}.";
				return false;
		}

		if (input.IsFinished)
		{
			message = "Which scheduled rule should this step administer?";
			return false;
		}

		var selector = input.PopSpeech();
		var rule = ResolveScheduledRuleForStep(host, selector);
		if (rule is null)
		{
			message = $"There is no scheduled employment rule matching {selector.ColourCommand()}.";
			return false;
		}

		string? manualKey = null;
		string? reason = null;
		if (operation == ScheduledRuleAdministrationActionKind.Evaluate)
		{
			if (!input.IsFinished)
			{
				var keyword = input.PopSpeech();
				if (!keyword.EqualTo("manual"))
				{
					message = $"Scheduled-rule evaluate steps use the syntax: {"tasks step rule evaluate <#|name|id> [manual <key>]".ColourCommand()}.";
					return false;
				}

				manualKey = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(manualKey))
				{
					message = "Which manual trigger key should this evaluation use?";
					return false;
				}
			}
		}
		else
		{
			reason = input.SafeRemainingArgument.Trim();
			if (string.IsNullOrWhiteSpace(reason))
			{
				reason = null;
			}
		}

		ConsumeRemaining(input);
		step = new ScheduledRuleAdministrationActionStep(operation, rule.Id, rule.Name, reason, manualKey);
		message = string.Empty;
		return true;
	}

	private static IEmploymentScheduledTaskRule? ResolveScheduledRuleForStep(IEmploymentHost host, string selector)
	{
		var rules = host.TaskBoard.ScheduledRules
			.OrderBy(x => x.Name)
			.ToList();
		if (!rules.Any())
		{
			return null;
		}

		selector = selector.Trim();
		if (selector.StartsWith("#") && int.TryParse(selector[1..], out var number))
		{
			return number > 0 && number <= rules.Count ? rules[number - 1] : null;
		}

		if (Guid.TryParse(selector, out var id))
		{
			return rules.FirstOrDefault(x => x.Id == id);
		}

		return rules.FirstOrDefault(x => x.Name.EqualTo(selector)) ??
		       rules.FirstOrDefault(x => x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
	}

	private static bool TryParseShopStocktake(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not IShop shop)
		{
			message = "Stocktake steps can only be authored for shop employment hosts.";
			return false;
		}

		if (input.IsFinished)
		{
			message = $"Stocktake steps use the syntax: {"tasks step stocktake all".ColourCommand()} or {"tasks step stocktake merch <id|name>".ColourCommand()}.";
			return false;
		}

		var mode = input.PopSpeech();
		if (mode.EqualTo("all"))
		{
			if (!input.IsFinished)
			{
				message = $"Stocktake-all steps use the syntax: {"tasks step stocktake all".ColourCommand()}.";
				return false;
			}

			step = new ShopStocktakeActionStep();
			message = string.Empty;
			return true;
		}

		string selector;
		if (mode.EqualToAny("merch", "merchandise", "item"))
		{
			selector = input.SafeRemainingArgument.Trim();
			if (string.IsNullOrWhiteSpace(selector))
			{
				message = "Which merchandise should this stocktake step count?";
				return false;
			}
		}
		else
		{
			selector = string.Join(" ", new[] { mode, input.SafeRemainingArgument }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
		}

		var merchandise = ResolveShopStocktakeMerchandise(shop, selector);
		if (merchandise is null)
		{
			message = $"There is no merchandise belonging to {shop.Name.ColourName()} matching {selector.ColourCommand()}.";
			return false;
		}

		ConsumeRemaining(input);
		step = new ShopStocktakeActionStep(ShopStocktakeScope.Merchandise,
			merchandise.Id.ToString("F0", CultureInfo.InvariantCulture), merchandise.Name);
		message = string.Empty;
		return true;
	}

	private static IMerchandise? ResolveShopStocktakeMerchandise(IShop shop, string selector)
	{
		if (long.TryParse(selector.TrimStart('#'), out var id))
		{
			return shop.Merchandises.FirstOrDefault(x => x.Id == id);
		}

		return shop.Merchandises.GetByIdOrName(selector) ??
		       shop.Merchandises.FirstOrDefault(x =>
			       x.Name.EqualTo(selector) ||
			       x.ListDescription.EqualTo(selector));
	}
	private static bool TryParsePriceChange(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Price steps use the syntax: {"tasks step price merch <id|name> <amount>".ColourCommand()}.";
			return false;
		}

		var mode = input.PopSpeech().CollapseString().ToLowerInvariant();
		if (mode.EqualTo("merch") || mode.EqualTo("merchandise") || mode.EqualTo("item"))
		{
			var tokens = PopRemainingTokens(input).ToList();
			if (!TrySplitTrailingMoney(host, tokens, out var selector, out var amount, out message))
			{
				return false;
			}

			step = new PriceChangeActionStep(selector, amount);
			message = string.Empty;
			return true;
		}

		if (mode.EqualTo("market"))
		{
			message = "Employment market-price actions are deprecated. Use merchandise repricing or native shop deal actions instead.";
			return false;
		}

		message = $"Unknown price mode {mode.ColourCommand()}. Use {"merch".ColourCommand()}.";
		return false;
	}

	private static bool TryParseShopDealAdministration(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (host is not IShop shop)
		{
			message = "Shop deal steps can only be authored for shop employment hosts.";
			return false;
		}

		if (input.IsFinished)
		{
			message = $"Shop deal steps use the syntax: {"tasks step sale create <name> target all|merchandise <which>|tag <which> adjustment <signed %> [type sale|volume <quantity>] [applies sell|buy|both] [eligibility none|<prog>] [cumulative true|false] [expires never|<datetime|duration>]".ColourCommand()} or {"tasks step sale modify <deal id|name> target ... adjustment ...".ColourCommand()} or {"tasks step sale cancel <deal id|name>".ColourCommand()}.";
			return false;
		}

		var operation = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (operation)
		{
			case "create":
			case "new":
				return TryParseShopDealCreate(actor, shop, input, out step, out message);
			case "modify":
			case "edit":
				return TryParseShopDealModify(actor, shop, input, out step, out message);
			case "cancel":
			case "delete":
			case "remove":
				var selector = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(selector))
				{
					message = "Which shop deal do you want to cancel?";
					return false;
				}

				step = new ShopDealAdministrationActionStep(selector);
				message = string.Empty;
				return true;
			default:
				message = $"Unknown shop deal operation {operation.ColourCommand()}. Use {"create".ColourCommand()}, {"modify".ColourCommand()}, or {"cancel".ColourCommand()}.";
				return false;
		}
	}

	private static bool TryParseShopDealCreate(ICharacter actor, IShop shop, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		var name = string.Join(" ", PopTokensUntil(input, "target")).Trim();
		if (string.IsNullOrWhiteSpace(name) || input.IsFinished || !input.PopSpeech().EqualTo("target"))
		{
			message = $"Shop deal create steps use the syntax: {"tasks step sale create <name> target all|merchandise <which>|tag <which> adjustment <signed %> ...".ColourCommand()}.";
			return false;
		}

		if (input.IsFinished)
		{
			message = "Should this deal target all merchandise, one merchandise record, or a tag?";
			return false;
		}

		var targetMode = input.PopSpeech().CollapseString().ToLowerInvariant();
		ShopDealTargetType targetType;
		string? targetSelector = null;
		switch (targetMode)
		{
			case "all":
				targetType = ShopDealTargetType.AllMerchandise;
				break;
			case "merch":
			case "merchandise":
				targetType = ShopDealTargetType.Merchandise;
				targetSelector = string.Join(" ", PopTokensUntilAny(input, ShopDealCreateOptions)).Trim();
				if (string.IsNullOrWhiteSpace(targetSelector))
				{
					message = "Which merchandise record should this shop deal target?";
					return false;
				}

				break;
			case "tag":
				targetType = ShopDealTargetType.ItemTag;
				targetSelector = string.Join(" ", PopTokensUntilAny(input, ShopDealCreateOptions)).Trim();
				if (string.IsNullOrWhiteSpace(targetSelector))
				{
					message = "Which tag should this shop deal target?";
					return false;
				}

				break;
			default:
				message = $"Unknown shop deal target {targetMode.ColourCommand()}. Use {"all".ColourCommand()}, {"merchandise".ColourCommand()}, or {"tag".ColourCommand()}.";
				return false;
		}

		var dealType = ShopDealType.Sale;
		var minimumQuantity = 0;
		decimal? adjustment = null;
		var applicability = ShopDealApplicability.Sell;
		IFutureProg? eligibilityProg = null;
		var cumulative = true;
		var expiry = MudDateTime.Never;

		while (!input.IsFinished)
		{
			var option = input.PopSpeech().CollapseString().ToLowerInvariant();
			switch (option)
			{
				case "adjustment":
				case "discount":
				case "modifier":
					if (input.IsFinished || !input.PopSpeech().TryParsePercentageDecimal(actor.Account.Culture, out var parsedAdjustment))
					{
						message = "What signed percentage adjustment should this shop deal use?";
						return false;
					}

					adjustment = parsedAdjustment;
					break;
				case "type":
					if (input.IsFinished)
					{
						message = "Should this be a sale deal or a volume deal?";
						return false;
					}

					var type = input.PopSpeech().CollapseString().ToLowerInvariant();
					if (type.EqualTo("sale"))
					{
						dealType = ShopDealType.Sale;
						minimumQuantity = 0;
						break;
					}

					if (type.EqualTo("volume"))
					{
						if (input.IsFinished || !int.TryParse(input.PopSpeech(), out minimumQuantity) || minimumQuantity < 2)
						{
							message = "Volume shop deals require a minimum quantity of 2 or more.";
							return false;
						}

						dealType = ShopDealType.Volume;
						break;
					}

					message = $"Unknown shop deal type {type.ColourCommand()}. Use {"sale".ColourCommand()} or {"volume <quantity>".ColourCommand()}.";
					return false;
				case "volume":
					if (input.IsFinished || !int.TryParse(input.PopSpeech(), out minimumQuantity) || minimumQuantity < 2)
					{
						message = "Volume shop deals require a minimum quantity of 2 or more.";
						return false;
					}

					dealType = ShopDealType.Volume;
					break;
				case "applies":
				case "scope":
					if (input.IsFinished || !TryParseShopDealApplicability(input.PopSpeech(), out applicability))
					{
						message = "Should this shop deal apply to sell, buy, or both prices?";
						return false;
					}

					break;
				case "eligibility":
				case "prog":
					var progText = string.Join(" ", PopTokensUntilAny(input, ShopDealCreateOptions)).Trim();
					if (!TryParseShopDealEligibility(actor, progText, out eligibilityProg, out message))
					{
						return false;
					}

					break;
				case "cumulative":
				case "stack":
					if (input.IsFinished || IsShopDealCreateOption(input.PeekSpeech()))
					{
						cumulative = true;
						break;
					}

					if (!TryParseBoolean(input.PopSpeech(), out cumulative))
					{
						message = "Should this shop deal be cumulative? Use true or false.";
						return false;
					}

					break;
				case "expires":
				case "expiry":
					var expiryText = string.Join(" ", PopTokensUntilAny(input, ShopDealCreateOptions)).Trim();
					if (!TryParseShopDealExpiry(actor, shop, expiryText, out expiry, out message))
					{
						return false;
					}

					break;
				default:
					message = $"Unknown shop deal option {option.ColourCommand()}.";
					return false;
			}
		}

		if (!adjustment.HasValue)
		{
			message = "Shop deal create steps require an adjustment percentage.";
			return false;
		}

		step = new ShopDealAdministrationActionStep(name, dealType, targetType, targetSelector,
			adjustment.Value, minimumQuantity, applicability, eligibilityProg, cumulative, expiry);
		message = string.Empty;
		return true;
	}

	private static bool TryParseShopDealModify(ICharacter actor, IShop shop, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		var selector = string.Join(" ", PopTokensUntil(input, "target")).Trim();
		if (string.IsNullOrWhiteSpace(selector) || input.IsFinished || !input.PopSpeech().EqualTo("target"))
		{
			message = $"Shop deal modify steps use the syntax: {"tasks step sale modify <deal id|name> target all|merchandise <which>|tag <which> adjustment <signed %> ...".ColourCommand()}.";
			return false;
		}

		var createInput = new StringStack($"{selector} target {input.SafeRemainingArgument}");
		if (!TryParseShopDealCreate(actor, shop, createInput, out var parsedStep, out message))
		{
			message = message.Replace("create", "modify", StringComparison.InvariantCultureIgnoreCase);
			return false;
		}

		if (parsedStep is not ShopDealAdministrationActionStep parsed)
		{
			message = "The shop deal modify step could not be parsed.";
			return false;
		}

		step = new ShopDealAdministrationActionStep(selector, string.Empty, parsed.DealType, parsed.TargetType,
			parsed.TargetSelector, parsed.PriceAdjustmentPercentage, parsed.MinimumQuantity, parsed.Applicability,
			parsed.EligibilityProg, parsed.IsCumulative, parsed.Expiry);
		message = string.Empty;
		return true;
	}

	private static bool TryParseShopDealEligibility(ICharacter actor, string text, out IFutureProg? prog,
		out string message)
	{
		prog = null;
		if (string.IsNullOrWhiteSpace(text) || text.EqualToAny("none", "clear", "off"))
		{
			message = string.Empty;
			return true;
		}

		prog = new ProgLookupFromBuilderInput(actor, text, ProgVariableTypes.Boolean,
			new[]
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Shop,
				ProgVariableTypes.Merchandise,
				ProgVariableTypes.Number,
				ProgVariableTypes.MudDateTime
			}).LookupProg();
		message = prog is null ? $"There is no matching eligibility prog for {text.ColourCommand()}." : string.Empty;
		return prog is not null;
	}

	private static bool TryParseShopDealExpiry(ICharacter actor, IShop shop, string text, out MudDateTime expiry,
		out string message)
	{
		expiry = MudDateTime.Never;
		if (string.IsNullOrWhiteSpace(text) || text.EqualToAny("never", "none", "off", "clear"))
		{
			message = string.Empty;
			return true;
		}

		if (MudTimeSpan.TryParse(text, actor, out var duration))
		{
			expiry = shop.EconomicZone.ZoneForTimePurposes.DateTime() + duration;
			message = string.Empty;
			return true;
		}

		if (MudDateTime.TryParse(text, shop.EconomicZone.FinancialPeriodReferenceCalendar,
			    shop.EconomicZone.FinancialPeriodReferenceClock, actor, out expiry, out message))
		{
			return true;
		}

		return false;
	}

	private static bool TryParseShopDealApplicability(string text, out ShopDealApplicability applicability)
	{
		if (text.EqualTo("sell"))
		{
			applicability = ShopDealApplicability.Sell;
			return true;
		}

		if (text.EqualTo("buy"))
		{
			applicability = ShopDealApplicability.Buy;
			return true;
		}

		if (text.EqualTo("both"))
		{
			applicability = ShopDealApplicability.Both;
			return true;
		}

		applicability = ShopDealApplicability.Sell;
		return false;
	}

	private static bool TryParseJobOpeningAdministration(ICharacter actor, IEmploymentHost host, StringStack input,
		out IEmploymentActionStep step, out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = $"Job-opening steps use the syntax: {"tasks step jobopening create ...".ColourCommand()}, {"tasks step jobopening close <#opening> [reason]".ColourCommand()}, or {"tasks step jobopening modify <#opening> ...".ColourCommand()}.";
			return false;
		}

		var operation = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (operation)
		{
			case "create":
			case "new":
				if (!TryParseJobOpeningDefinition(actor, host, input, null, out var definition, out message))
				{
					return false;
				}

				step = new JobOpeningAdministrationActionStep(definition, "Created by employment task.");
				message = string.Empty;
				return true;
			case "close":
			case "closed":
			case "cancel":
				if (input.IsFinished || !TryParseCommandNumber(input.PopSpeech(), out var closeId))
				{
					message = "Which job opening should this step close?";
					return false;
				}

				step = new JobOpeningAdministrationActionStep(
					JobOpeningAdministrationActionKind.Close,
					closeId,
					reason: input.IsFinished ? "Closed by employment task." : input.SafeRemainingArgument);
				ConsumeRemaining(input);
				message = string.Empty;
				return true;
			case "modify":
			case "edit":
				if (input.IsFinished || !TryParseCommandNumber(input.PopSpeech(), out var modifyId))
				{
					message = "Which job opening should this step modify?";
					return false;
				}

				var opening = host.JobOpenings.FirstOrDefault(x => x.Id == modifyId);
				if (opening is null)
				{
					message = $"There is no job opening #{modifyId.ToString("N0", actor)} for {host.EmploymentHostName.ColourName()}.";
					return false;
				}

				if (!TryParseJobOpeningDefinition(actor, host, input, opening, out var modifiedDefinition, out message))
				{
					return false;
				}

				step = new JobOpeningAdministrationActionStep(
					JobOpeningAdministrationActionKind.Modify,
					modifyId,
					modifiedDefinition,
					"Modified by employment task.");
				message = string.Empty;
				return true;
			default:
				message = $"Unknown job-opening operation {operation.ColourCommand()}. Use {"create".ColourCommand()}, {"close".ColourCommand()}, or {"modify".ColourCommand()}.";
				return false;
		}
	}

	private static bool TryParseJobOpeningDefinition(ICharacter actor, IEmploymentHost host, StringStack input,
		IJobOpening? existing, out JobOpeningDefinition definition, out string message)
	{
		definition = null!;
		var role = existing?.Role;
		var requirements = existing?.Requirements ?? JobRequirementSet.None;
		var compensation = existing?.Compensation;
		var schedule = existing?.Schedule ?? WorkSchedule.AnyTime;
		var duration = existing?.Duration ?? EmploymentDuration.Indefinite;
		var maxPositions = existing?.MaxPositions ?? 1;
		var npcOnly = existing?.NpcApplicationsOnly ?? true;
		var paymentMethod = existing?.PaymentMethod ?? new PaymentMethod(PaymentMethodKind.Cash);
		EmploymentAuthoritySet? authority = existing?.Authority;
		PaymentSource? paymentSourceOverride = null;
		var requirementSkills = requirements.Skills.ToList();
		var requirementKnowledges = requirements.Knowledges.ToList();
		var requirementCapabilities = requirements.Capabilities.ToList();
		var requirementTags = requirements.Tags.ToList();
		var requirementsExplicit = false;

		if (existing is null && !input.IsFinished && !IsJobOpeningDefinitionKeyword(input.PeekSpeech()) &&
		    input.PeekSpeech().TryParseEnum<EmploymentRole>(out var legacyRole))
		{
			role = legacyRole;
			input.PopSpeech();
			if (!input.IsFinished && !IsJobOpeningDefinitionKeyword(input.PeekSpeech()))
			{
				var legacyTokens = PopRemainingTokens(input).ToList();
				if (!TryParseLegacyOpeningPay(host, legacyTokens, out compensation, out maxPositions, out message))
				{
					return false;
				}
			}
		}

		while (!input.IsFinished)
		{
			var keyword = input.PopSpeech().CollapseString().ToLowerInvariant();
			switch (keyword)
			{
				case "role":
					if (input.IsFinished || !input.PopSpeech().TryParseEnum<EmploymentRole>(out var parsedRole))
					{
						message = $"That is not a valid employment role. The options are {Enum.GetValues<EmploymentRole>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.";
						return false;
					}
					role = parsedRole;
					break;
				case "positions":
				case "position":
					if (input.IsFinished || !int.TryParse(input.PopSpeech(), out maxPositions) || maxPositions <= 0)
					{
						message = "How many positive positions should this job opening advertise?";
						return false;
					}
					break;
				case "npc":
					if (input.IsFinished || !TryParseBoolean(input.PopSpeech(), out npcOnly))
					{
						message = "Should this job opening accept NPC applications only? Use true or false.";
						return false;
					}
					break;
				case "pay":
					if (!TryParseJobOpeningPay(actor, host, input, out compensation, out message))
					{
						return false;
					}
					break;
				case "paymentsource":
				case "source":
					if (input.IsFinished || !TryParsePaymentSource(input.PopSpeech(), out var source))
					{
						message = "Which employer payment source should this opening use?";
						return false;
					}
					paymentSourceOverride = source;
					break;
				case "payment":
				case "method":
					if (!TryParsePaymentMethod(actor, input, out paymentMethod, out message))
					{
						return false;
					}
					break;
				case "schedule":
					if (!TryParseWorkSchedule(actor, input, out schedule, out message))
					{
						return false;
					}
					break;
				case "duration":
					if (!TryParseEmploymentDuration(actor, input, out duration, out message))
					{
						return false;
					}
					break;
				case "authority":
				case "authorities":
					if (!TryParseOpeningAuthority(input, role, out authority, out message))
					{
						return false;
					}
					break;
				case "requires":
				case "require":
					if (!requirementsExplicit)
					{
						requirementSkills.Clear();
						requirementKnowledges.Clear();
						requirementCapabilities.Clear();
						requirementTags.Clear();
						requirementsExplicit = true;
					}

					if (!TryParseJobRequirement(input, requirementSkills, requirementKnowledges,
						    requirementCapabilities, requirementTags, out message))
					{
						return false;
					}
					break;
				default:
					message = $"Unknown job-opening definition keyword {keyword.ColourCommand()}.";
					return false;
			}
		}

		if (!role.HasValue)
		{
			message = "Which employment role should this job opening use?";
			return false;
		}

		if (compensation is null)
		{
			message = existing is null
				? $"New job-opening steps must specify pay, for example {"pay fixed 12.50 hourly".ColourCommand()} or {"pay unpaid".ColourCommand()}."
				: "The existing job opening does not have compensation terms to copy.";
			return false;
		}

		if (paymentSourceOverride.HasValue)
		{
			compensation = compensation with { EmployerPaymentSource = paymentSourceOverride.Value };
		}

		authority ??= DefaultOpeningAuthority(role.Value);
		definition = new JobOpeningDefinition(
			role.Value,
			new JobRequirementSet(requirementSkills, requirementKnowledges, requirementCapabilities, requirementTags),
			compensation,
			schedule,
			duration,
			maxPositions,
			npcOnly,
			paymentMethod,
			authority.Value);
		message = string.Empty;
		return true;
	}

	private static bool TryParseCraft(StringStack input, out IEmploymentActionStep step, out string message)
	{
		step = null!;
		var description = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(description))
		{
			message = $"Craft steps use the syntax: {"tasks step craft <craft id|craft name>".ColourCommand()}";
			return false;
		}

		ConsumeRemaining(input);
		step = new CraftTriggerActionStep(description);
		message = string.Empty;
		return true;
	}

	private static bool TryParseCraftStation(StringStack input, out IEmploymentActionStep step, out string message)
	{
		step = null!;
		var selector = input.IsFinished ? "here" : input.SafeRemainingArgument.Trim();
		ConsumeRemaining(input);
		step = new CraftStationActionStep(selector);
		message = string.Empty;
		return true;
	}

	private static bool TryParseGenericShell(IEmploymentHost host, string actionKey, StringStack input,
		out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (actionKey.EqualTo("release") && input.IsFinished)
		{
			step = new CataloguedActionShellStep(actionKey, "all");
			message = string.Empty;
			return true;
		}

		if (actionKey.EqualTo("authorise") || actionKey.EqualTo("reserve"))
		{
			var amountTokens = PopTokensUntil(input, "for").ToList();
			if (amountTokens.Any() && !input.IsFinished && input.PeekSpeech().EqualTo("for"))
			{
				input.PopSpeech();
				if (!TryParseMoney(host, string.Join(" ", amountTokens), out var amount, out message))
				{
					return false;
				}

				var amountDescription = input.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(amountDescription))
				{
					message = $"{actionKey.ColourCommand()} steps need a short description after the amount.";
					return false;
				}

				ConsumeRemaining(input);
				step = new CataloguedActionShellStep(actionKey, amountDescription, amount);
				message = string.Empty;
				return true;
			}

			var tokenDescription = string.Join(" ", amountTokens).Trim();
			if (string.IsNullOrWhiteSpace(tokenDescription))
			{
				message = $"{actionKey.ColourCommand()} steps need a short description to audit.";
				return false;
			}

			step = new CataloguedActionShellStep(actionKey, tokenDescription);
			message = string.Empty;
			return true;
		}

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
			message = $"Route planning steps use the syntax: {"tasks step route to <here|cell id> [then <here|cell id> ...] [description]".ColourCommand()}";
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

		var routeStops = new List<ICell> { destination };
		while (!input.IsFinished && input.PeekSpeech().EqualToAny("then", "via", "and"))
		{
			input.PopSpeech();
			if (input.IsFinished)
			{
				message = "Which route stop should come after the route connector?";
				return false;
			}

			if (!TryResolveLocation(actor, input.PopSpeech(), out var routeStop, out message))
			{
				return false;
			}

			routeStops.Add(routeStop);
		}

		var description = input.IsFinished
			? $"route through {string.Join(" -> ", routeStops.Select(x => x.GetFriendlyReference(actor)))}"
			: input.SafeRemainingArgument.Trim();
		ConsumeRemaining(input);
		step = new CataloguedActionShellStep("route", description, routeStops);
		message = string.Empty;
		return true;
	}

	private static bool TryParseRouteBatch(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished || !input.PopSpeech().EqualTo("total"))
		{
			message = $"Route batch steps use the syntax: {"tasks step routebatch total <quantity> each <quantity> to <here|cell id> [then <here|cell id> ...] <rationale>".ColourCommand()}";
			return false;
		}

		if (input.IsFinished || !decimal.TryParse(input.PopSpeech(), NumberStyles.Number, actor, out var totalQuantity) || totalQuantity <= 0.0M)
		{
			message = "Route batch steps need a positive total quantity after total.";
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualToAny("each", "per", "perstop"))
		{
			message = "Route batch steps need an each quantity after the total quantity.";
			return false;
		}

		if (input.IsFinished || !decimal.TryParse(input.PopSpeech(), NumberStyles.Number, actor, out var perStopQuantity) || perStopQuantity <= 0.0M)
		{
			message = "Route batch steps need a positive per-stop quantity after each.";
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualTo("to"))
		{
			message = "Route batch steps need the to keyword before the first route stop.";
			return false;
		}

		if (input.IsFinished)
		{
			message = "Which first destination should this route batch target?";
			return false;
		}

		if (!TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
		{
			return false;
		}

		var routeStops = new List<ICell> { destination };
		while (!input.IsFinished && input.PeekSpeech().EqualToAny("then", "via", "and"))
		{
			input.PopSpeech();
			if (input.IsFinished)
			{
				message = "Which route stop should come after the route batch connector?";
				return false;
			}

			if (!TryResolveLocation(actor, input.PopSpeech(), out var routeStop, out message))
			{
				return false;
			}

			routeStops.Add(routeStop);
		}

		if (routeStops.Count < 2)
		{
			message = "Route batch steps need at least two route stops.";
			return false;
		}

		var rationale = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(rationale))
		{
			message = "Route batch steps need a short rationale after the final route stop.";
			return false;
		}

		ConsumeRemaining(input);
		var description = $"total {totalQuantity.ToString("0.###", CultureInfo.InvariantCulture)} each {perStopQuantity.ToString("0.###", CultureInfo.InvariantCulture)} {rationale}";
		step = new CataloguedActionShellStep("routebatch", description, routeStops);
		message = string.Empty;
		return true;
	}

	private static bool TryParseTripCheck(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (!TryParseTripCheckPolicy(input, "fuel", out var fuelPolicy, out message, "refuel"))
		{
			return false;
		}

		if (!TryParseTripCheckPolicy(input, "feed", out var feedPolicy, out message, "fodder"))
		{
			return false;
		}

		if (!TryParseTripCheckPolicy(input, "maintenance", out var maintenancePolicy, out message, "maint", "service"))
		{
			return false;
		}

		if (!TryParseTripCheckPolicy(input, "rest", out var restPolicy, out message, "break"))
		{
			return false;
		}

		var routeStops = new List<ICell>();
		if (!input.IsFinished && input.PeekSpeech().EqualTo("to"))
		{
			input.PopSpeech();
			if (input.IsFinished)
			{
				message = "Which first destination should this trip check target?";
				return false;
			}

			if (!TryResolveLocation(actor, input.PopSpeech(), out var destination, out message))
			{
				return false;
			}

			routeStops.Add(destination);
			while (!input.IsFinished && input.PeekSpeech().EqualToAny("then", "via", "and"))
			{
				input.PopSpeech();
				if (input.IsFinished)
				{
					message = "Which route stop should come after the trip check connector?";
					return false;
				}

				if (!TryResolveLocation(actor, input.PopSpeech(), out var routeStop, out message))
				{
					return false;
				}

				routeStops.Add(routeStop);
			}
		}

		var rationale = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(rationale))
		{
			message = "Trip check steps need a short rationale after the rest policy or final route stop.";
			return false;
		}

		ConsumeRemaining(input);
		var description = $"fuel {fuelPolicy} feed {feedPolicy} maintenance {maintenancePolicy} rest {restPolicy} {rationale}";
		step = routeStops.Count == 0
			? new CataloguedActionShellStep("tripcheck", description)
			: new CataloguedActionShellStep("tripcheck", description, routeStops);
		message = string.Empty;
		return true;
	}

	private static bool TryParseTripCheckPolicy(StringStack input, string keyword, out string value, out string message,
		params string[] aliases)
	{
		value = string.Empty;
		if (input.IsFinished)
		{
			message = $"Trip check steps need a {keyword} policy.";
			return false;
		}

		var actualKeyword = input.PopSpeech();
		if (!actualKeyword.EqualTo(keyword) && !aliases.Any(x => actualKeyword.EqualTo(x)))
		{
			message = $"Trip check steps use the syntax: {"tasks step tripcheck fuel <policy> feed <policy> maintenance <policy> rest <policy> [to <here|cell id> [then <here|cell id> ...]] <rationale>".ColourCommand()}";
			return false;
		}

		if (input.IsFinished)
		{
			message = $"Trip check steps need a {keyword} policy value.";
			return false;
		}

		value = input.PopSpeech().Trim();
		if (string.IsNullOrWhiteSpace(value))
		{
			message = $"Trip check steps need a {keyword} policy value.";
			return false;
		}

		message = string.Empty;
		return true;
	}

	internal static bool TryParseItemSelector(ICharacter actor, StringStack input, string noun,
		out EmploymentItemSelector? selector, out string message)
	{
		selector = null;
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
				message = $"Which {"&tag".ColourCommand()} identifies the {noun}?";
				return false;
			}

			return TryParseTagSelector(actor, input.PopSpeech(), noun, out selector, out message);
		}

		if (token.StartsWith('&'))
		{
			return TryParseTagSelector(actor, token, noun, out selector, out message);
		}

		if (token.EqualTo("keyword") || token.EqualTo("kw"))
		{
			if (input.IsFinished)
			{
				message = $"Which keyword should identify the {noun}?";
				return false;
			}

			var keyword = input.PopSpeech();
			var target = actor.TargetLocalOrHeldItem(keyword);
			if (target is null)
			{
				message = $"You cannot see any item matching {keyword.ColourCommand()} here.";
				return false;
			}

			selector = EmploymentItemSelector.ForItem(target, keyword);
			message = string.Empty;
			return true;
		}

		if (token.StartsWith('*'))
		{
			if (!long.TryParse(token[1..], out var itemId))
			{
				message = $"The {noun} item id must be in the form {"*123".ColourCommand()}.";
				return false;
			}

			var item = actor.Gameworld.TryGetItem(itemId, true);
			if (item is null)
			{
				message = $"There is no item with id {itemId.ToString("N0", actor).ColourValue()}.";
				return false;
			}

			selector = EmploymentItemSelector.ForItem(item);
			message = string.Empty;
			return true;
		}

		if (long.TryParse(token, out var prototypeId))
		{
			if (actor.Gameworld.ItemProtos.Get(prototypeId) is null)
			{
				message = $"There is no item prototype with id {prototypeId.ToString("N0", actor).ColourValue()}.";
				return false;
			}

			selector = EmploymentItemSelector.ForPrototype(prototypeId);
			message = string.Empty;
			return true;
		}

		var keywordTarget = actor.TargetLocalOrHeldItem(token);
		if (keywordTarget is null)
		{
			message = $"You cannot see any {noun} matching {token.ColourCommand()} here. Use a prototype id, {"*item id".ColourCommand()}, or {"&tag".ColourCommand()} if you do not mean a local keyword target.";
			return false;
		}

		selector = EmploymentItemSelector.ForItem(keywordTarget, token);
		message = string.Empty;
		return true;
	}

	private static bool TryParseTagSelector(ICharacter actor, string token, string noun,
		out EmploymentItemSelector? selector, out string message)
	{
		selector = null;
		if (!TryParseTagName(actor, token, noun, out var tagName, out message))
		{
			return false;
		}

		selector = EmploymentItemSelector.ForTag(tagName);
		message = string.Empty;
		return true;
	}

	private static bool TryParseTagName(ICharacter actor, string token, string noun, out string tagName,
		out string message)
	{
		tagName = string.Empty;
		if (string.IsNullOrWhiteSpace(token) || !token.StartsWith('&'))
		{
			message = $"The {noun} must use explicit tag syntax like {"&tag".ColourCommand()} or {"&123".ColourCommand()}. Bare numbers select item prototypes and bare text selects a visible item keyword.";
			return false;
		}

		var tagText = token[1..].Trim();
		if (string.IsNullOrWhiteSpace(tagText))
		{
			message = $"Which tag should {"&".ColourCommand()} refer to?";
			return false;
		}

		var tag = actor.Gameworld?.Tags?.GetByIdOrName(tagText);
		if (tag is null)
		{
			message = $"There is no tag matching {"&".ColourCommand()}{tagText.ColourCommand()}.";
			return false;
		}

		tagName = tag.FullName;
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

	private static readonly string[] ShopDealCreateOptions =
	[
		"adjustment",
		"discount",
		"modifier",
		"type",
		"volume",
		"applies",
		"scope",
		"eligibility",
		"prog",
		"cumulative",
		"stack",
		"expires",
		"expiry"
	];

	private static readonly string[] JobOpeningDefinitionKeywords =
	[
		"role",
		"positions",
		"position",
		"npc",
		"pay",
		"paymentsource",
		"source",
		"payment",
		"method",
		"schedule",
		"duration",
		"authority",
		"authorities",
		"requires",
		"require"
	];

	private static bool TrySplitTrailingMoney(IEmploymentHost host, IReadOnlyList<string> tokens, out string selector,
		out MoneyAmount amount, out string message)
	{
		selector = string.Empty;
		amount = null!;
		if (tokens.Count < 2)
		{
			message = $"Price merchandise steps use the syntax: {"tasks step price merch <id|name> <amount>".ColourCommand()}.";
			return false;
		}

		for (var split = 1; split < tokens.Count; split++)
		{
			var selectorText = string.Join(" ", tokens.Take(split)).Trim();
			var amountText = string.Join(" ", tokens.Skip(split)).Trim();
			if (string.IsNullOrWhiteSpace(selectorText))
			{
				continue;
			}

			if (TryParseMoney(host, amountText, out amount, out _))
			{
				selector = selectorText;
				message = string.Empty;
				return true;
			}
		}

		message = $"Could not parse a positive price amount at the end of {string.Join(" ", tokens).ColourCommand()}.";
		return false;
	}

	private static bool TryParseLegacyOpeningPay(IEmploymentHost host, IReadOnlyList<string> tokens,
		out CompensationTerms compensation, out int maxPositions, out string message)
	{
		compensation = null!;
		maxPositions = 1;
		if (!tokens.Any())
		{
			message = "What positive hourly rate should this opening advertise?";
			return false;
		}

		var amountTokens = tokens;
		if (tokens.Count > 1 && int.TryParse(tokens[^1], out var parsedPositions) && parsedPositions > 0)
		{
			maxPositions = parsedPositions;
			amountTokens = tokens.Take(tokens.Count - 1).ToList();
		}

		if (!TryParseMoney(host, string.Join(" ", amountTokens), out var hourlyRate, out message))
		{
			return false;
		}

		compensation = new CompensationTerms(
			hourlyRate,
			null,
			PayCadence.Hourly,
			hourlyRate,
			PaymentSource.HostCash);
		return true;
	}

	private static bool TryParseJobOpeningPay(ICharacter actor, IEmploymentHost host, StringStack input,
		out CompensationTerms compensation, out string message)
	{
		compensation = null!;
		if (input.IsFinished)
		{
			message = "What kind of pay should this opening advertise?";
			return false;
		}

		var payKind = input.PopSpeech().CollapseString().ToLowerInvariant();
		if (payKind.EqualTo("unpaid"))
		{
			compensation = new CompensationTerms(null, null, PayCadence.Unpaid, null, PaymentSource.HostCash);
			message = string.Empty;
			return true;
		}

		if (payKind.EqualTo("fixed"))
		{
			var amountTokens = new List<string>();
			while (!input.IsFinished && !input.PeekSpeech().TryParseEnum<PayCadence>(out _) &&
			       !input.PeekSpeech().EqualTo("min"))
			{
				amountTokens.Add(input.PopSpeech());
			}

			if (!amountTokens.Any())
			{
				message = "What positive fixed pay amount should this opening advertise?";
				return false;
			}

			if (!TryParseMoney(host, string.Join(" ", amountTokens), out var fixedRate, out message))
			{
				return false;
			}

			if (input.IsFinished || !input.PopSpeech().TryParseEnum<PayCadence>(out var cadence) ||
			    cadence == PayCadence.Unpaid)
			{
				message = "Which paid cadence should this fixed pay use?";
				return false;
			}

			var minimum = fixedRate;
			if (!input.IsFinished && input.PeekSpeech().EqualTo("min"))
			{
				input.PopSpeech();
				var minTokens = PopTokensUntilAny(input, JobOpeningDefinitionKeywords).ToList();
				if (!TryParseMoney(host, string.Join(" ", minTokens), out minimum, out message))
				{
					return false;
				}
			}

			compensation = new CompensationTerms(fixedRate, null, cadence, minimum, PaymentSource.HostCash);
			message = string.Empty;
			return true;
		}

		if (payKind.EqualTo("market"))
		{
			if (input.IsFinished || !input.PopSpeech().TryParseEnum<MarketRateBindingType>(out var bindingType) ||
			    bindingType == MarketRateBindingType.None)
			{
				message = "Which market binding should this opening use: multiplier, floor, or premium?";
				return false;
			}

			if (input.IsFinished || !decimal.TryParse(input.PopSpeech(), NumberStyles.Any, actor.Account.Culture,
				    out var bindingValue))
			{
				message = "What value should this market binding use?";
				return false;
			}

			if (input.IsFinished || !input.PopSpeech().TryParseEnum<PayCadence>(out var cadence) ||
			    cadence == PayCadence.Unpaid)
			{
				message = "Which paid cadence should this market pay use?";
				return false;
			}

			MoneyAmount? minimum = null;
			if (!input.IsFinished && input.PeekSpeech().EqualTo("min"))
			{
				input.PopSpeech();
				var minTokens = PopTokensUntilAny(input, JobOpeningDefinitionKeywords).ToList();
				if (!TryParseMoney(host, string.Join(" ", minTokens), out var parsedMinimum, out message))
				{
					return false;
				}

				minimum = parsedMinimum;
			}

			if (minimum is null)
			{
				message = "Market-rate openings must specify a positive minimum effective pay with the min keyword.";
				return false;
			}

			compensation = new CompensationTerms(
				null,
				new MarketRateBinding(bindingType, bindingValue),
				cadence,
				minimum,
				PaymentSource.HostCash);
			message = string.Empty;
			return true;
		}

		message = $"Unknown pay kind {payKind.ColourCommand()}. Use {"unpaid".ColourCommand()}, {"fixed".ColourCommand()}, or {"market".ColourCommand()}.";
		return false;
	}

	private static bool TryParsePaymentSource(string text, out PaymentSource source)
	{
		if (text.TryParseEnum(out source))
		{
			return true;
		}

		var parsed = text.CollapseString().ToLowerInvariant() switch
		{
			"hostcash" or "cash" => (PaymentSource?)PaymentSource.HostCash,
			"hostbank" or "hostbankaccount" or "bank" => PaymentSource.HostBankAccount,
			"specifiedemployeraccount" or "employeraccount" or "specifiedaccount" => PaymentSource.SpecifiedEmployerAccount,
			"store" or "storeaccount" => PaymentSource.StoreAccount,
			"float" or "employeefloat" => PaymentSource.EmployeeFloat,
			_ => null
		};
		source = parsed ?? PaymentSource.HostCash;
		return parsed.HasValue;
	}

	private static bool TryParsePaymentMethod(ICharacter actor, StringStack input, out PaymentMethod method,
		out string message)
	{
		method = null!;
		if (input.IsFinished)
		{
			message = "Which payment method should this opening use?";
			return false;
		}

		var kind = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (kind)
		{
			case "cash":
				method = new PaymentMethod(PaymentMethodKind.Cash);
				message = string.Empty;
				return true;
			case "employeebankaccount":
			case "employeebank":
				method = new PaymentMethod(PaymentMethodKind.EmployeeBankAccount);
				message = string.Empty;
				return true;
			case "specifiedbankaccount":
			case "bankaccount":
				if (input.IsFinished || !long.TryParse(input.PopSpeech().TrimStart('#'), out var bankAccountId))
				{
					message = "Which bank account id should this payment method use?";
					return false;
				}

				var bankAccount = actor.Gameworld.BankAccounts.Get(bankAccountId);
				if (bankAccount is null)
				{
					message = $"There is no bank account #{bankAccountId.ToString("N0", actor)}.";
					return false;
				}

				method = new PaymentMethod(PaymentMethodKind.SpecifiedBankAccount, bankAccount);
				message = string.Empty;
				return true;
			case "paymentitem":
			case "item":
				if (input.IsFinished || !long.TryParse(input.PopSpeech().TrimStart('#'), out var itemProtoId))
				{
					message = "Which payment item prototype id should this payment method use?";
					return false;
				}

				var prototype = actor.Gameworld.ItemProtos.Get(itemProtoId);
				if (prototype is null)
				{
					message = $"There is no item prototype #{itemProtoId.ToString("N0", actor)}.";
					return false;
				}

				method = new PaymentMethod(PaymentMethodKind.PaymentItem, PaymentItemPrototype: prototype);
				message = string.Empty;
				return true;
			case "employerfloat":
			case "float":
				method = new PaymentMethod(PaymentMethodKind.EmployerFloat);
				message = string.Empty;
				return true;
			default:
				message = $"Unknown payment method {kind.ColourCommand()}.";
				return false;
		}
	}

	private static bool TryParseWorkSchedule(ICharacter actor, StringStack input, out WorkSchedule schedule,
		out string message)
	{
		schedule = WorkSchedule.AnyTime;
		if (input.IsFinished)
		{
			message = "What schedule should this opening use?";
			return false;
		}

		var mode = input.PopSpeech().CollapseString().ToLowerInvariant();
		if (mode.EqualTo("any"))
		{
			message = string.Empty;
			return true;
		}

		if (mode.EqualTo("description") || mode.EqualTo("desc"))
		{
			var description = string.Join(" ", PopTokensUntilAny(input, JobOpeningDefinitionKeywords)).Trim();
			if (string.IsNullOrWhiteSpace(description))
			{
				message = "What schedule description should this opening use?";
				return false;
			}

			schedule = new WorkSchedule(description);
			message = string.Empty;
			return true;
		}

		if (mode.EqualTo("between"))
		{
			if (input.IsFinished || !TimeSpan.TryParse(input.PopSpeech(), actor.Account.Culture, out var start))
			{
				message = "What start time should this schedule use?";
				return false;
			}

			if (!input.IsFinished && input.PeekSpeech().EqualTo("and"))
			{
				input.PopSpeech();
			}

			if (input.IsFinished || !TimeSpan.TryParse(input.PopSpeech(), actor.Account.Culture, out var end))
			{
				message = "What end time should this schedule use?";
				return false;
			}

			schedule = new WorkSchedule($"Between {start:g} and {end:g}", start, end);
			message = string.Empty;
			return true;
		}

		message = $"Unknown schedule mode {mode.ColourCommand()}. Use any, between, or description.";
		return false;
	}

	private static bool TryParseEmploymentDuration(ICharacter actor, StringStack input, out EmploymentDuration duration,
		out string message)
	{
		duration = EmploymentDuration.Indefinite;
		if (input.IsFinished)
		{
			message = "What duration should this opening use?";
			return false;
		}

		var mode = input.PopSpeech().CollapseString().ToLowerInvariant();
		if (mode.EqualTo("indefinite"))
		{
			message = string.Empty;
			return true;
		}

		var durationType = mode switch
		{
			"fixed" or "fixedterm" => EmploymentDurationType.FixedTerm,
			"seasonal" or "season" => EmploymentDurationType.Seasonal,
			"tasklimited" or "task" => EmploymentDurationType.TaskLimited,
			_ => (EmploymentDurationType?)null
		};
		if (!durationType.HasValue)
		{
			message = $"Unknown duration mode {mode.ColourCommand()}.";
			return false;
		}

		var durationText = string.Join(" ", PopTokensUntilAny(input, JobOpeningDefinitionKeywords)).Trim();
		if (!TryParseTimeSpan(actor, durationText, out var length) || length <= TimeSpan.Zero)
		{
			message = $"Could not parse {durationText.ColourCommand()} as a positive duration.";
			return false;
		}

		duration = new EmploymentDuration(durationType.Value, length);
		message = string.Empty;
		return true;
	}

	private static bool TryParseOpeningAuthority(StringStack input, EmploymentRole? role,
		out EmploymentAuthoritySet? authority, out string message)
	{
		authority = null;
		var tokens = PopTokensUntilAny(input, JobOpeningDefinitionKeywords).ToList();
		if (!tokens.Any())
		{
			message = "Which delegated authority should this opening use?";
			return false;
		}

		if (tokens.Count == 1 && tokens[0].EqualTo("default"))
		{
			if (!role.HasValue)
			{
				message = "Set the role before using default authority.";
				return false;
			}

			authority = DefaultOpeningAuthority(role.Value);
			message = string.Empty;
			return true;
		}

		return TryParseAuthoritySet(tokens, out authority, out message, allowNone: true);
	}

	private static bool TryParseJobRequirement(StringStack input, List<SkillRequirement> skills,
		List<KnowledgeRequirement> knowledges, List<AICapabilityRequirement> capabilities,
		List<TagRequirement> tags, out string message)
	{
		if (input.IsFinished)
		{
			message = "Which requirement kind should this opening use?";
			return false;
		}

		var kind = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (kind)
		{
			case "skill":
			{
				var tokens = PopTokensUntilAny(input, JobOpeningDefinitionKeywords).ToList();
				if (tokens.Count < 2 || !double.TryParse(tokens[^1], out var minimum))
				{
					message = "Skill requirements use the syntax: requires skill <name> <minimum>.";
					return false;
				}

				skills.Add(new SkillRequirement(string.Join(" ", tokens.Take(tokens.Count - 1)), minimum));
				message = string.Empty;
				return true;
			}
			case "knowledge":
			{
				var name = string.Join(" ", PopTokensUntilAny(input, JobOpeningDefinitionKeywords)).Trim();
				if (string.IsNullOrWhiteSpace(name))
				{
					message = "Knowledge requirements use the syntax: requires knowledge <name>.";
					return false;
				}

				knowledges.Add(new KnowledgeRequirement(name));
				message = string.Empty;
				return true;
			}
			case "capability":
			{
				var name = string.Join(" ", PopTokensUntilAny(input, JobOpeningDefinitionKeywords)).Trim();
				if (!name.TryParseEnum<EmploymentAICapability>(out var capability))
				{
					message = $"Unknown employment AI capability {name.ColourCommand()}.";
					return false;
				}

				capabilities.Add(new AICapabilityRequirement(capability));
				message = string.Empty;
				return true;
			}
			case "tag":
			{
				var name = string.Join(" ", PopTokensUntilAny(input, JobOpeningDefinitionKeywords)).Trim();
				if (string.IsNullOrWhiteSpace(name))
				{
					message = "Tag requirements use the syntax: requires tag <tag>.";
					return false;
				}

				tags.Add(new TagRequirement(name));
				message = string.Empty;
				return true;
			}
			default:
				message = $"Unknown requirement kind {kind.ColourCommand()}.";
				return false;
		}
	}

	private static bool TryParseAuthoritySet(IEnumerable<string> tokens, out EmploymentAuthoritySet? authority,
		out string error, bool allowNone = false)
	{
		authority = null;
		error = string.Empty;
		var authorities = EmploymentAuthority.None;
		foreach (var token in tokens)
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
					error = "Use a specific authority name.";
					return false;
				}

				authorities = EmploymentAuthority.None;
				continue;
			}

			if (!TryParseAuthority(part, out var parsed))
			{
				error = $"Unknown delegated authority {part.ColourCommand()}.";
				return false;
			}

			authorities |= parsed;
		}

		if (authorities == EmploymentAuthority.None && !allowNone)
		{
			error = "Which delegated authority do you want to use?";
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

	private static EmploymentAuthoritySet DefaultOpeningAuthority(EmploymentRole role)
	{
		return role switch
		{
			EmploymentRole.Proprietor => EmploymentAuthoritySet.All,
			EmploymentRole.Manager => new EmploymentAuthoritySet(
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
				EmploymentAuthority.ManageStockRules |
				EmploymentAuthority.ManageCraftRules |
				EmploymentAuthority.ManageDeliveryRoutes |
				EmploymentAuthority.AdjustPrices |
				EmploymentAuthority.ManagePayroll |
				EmploymentAuthority.PostToHostBoard |
				EmploymentAuthority.ModerateHostBoard),
			EmploymentRole.Employee or
			EmploymentRole.Clerk or
			EmploymentRole.Courier or
			EmploymentRole.StableHand or
			EmploymentRole.HotelWorker => new EmploymentAuthoritySet(EmploymentAuthority.ManageDeliveryRoutes),
			EmploymentRole.Crafter => new EmploymentAuthoritySet(
				EmploymentAuthority.ManageCraftRules |
				EmploymentAuthority.ManageDeliveryRoutes),
			EmploymentRole.BankTeller => new EmploymentAuthoritySet(
				EmploymentAuthority.DepositBusinessCash |
				EmploymentAuthority.WithdrawBusinessCash),
			_ => EmploymentAuthoritySet.Empty
		};
	}

	private static bool TryParseBoolean(string text, out bool value)
	{
		if (bool.TryParse(text, out value))
		{
			return true;
		}

		if (text.EqualToAny("yes", "y", "on"))
		{
			value = true;
			return true;
		}

		if (text.EqualToAny("no", "n", "off"))
		{
			value = false;
			return true;
		}

		return false;
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

	private static bool TryParseTimeSpan(ICharacter actor, string text, out TimeSpan timespan)
	{
		if (TimeSpan.TryParse(text, actor.Account.Culture, out timespan))
		{
			return true;
		}

		if (MudTimeSpan.TryParse(text, actor.Account.Culture, out var mudTimeSpan))
		{
			timespan = mudTimeSpan.AsTimeSpan();
			return true;
		}

		return false;
	}

	private static IEnumerable<string> PopTokensUntil(StringStack input, string token)
	{
		while (!input.IsFinished && !input.PeekSpeech().EqualTo(token))
		{
			yield return input.PopSpeech();
		}
	}

	private static IEnumerable<string> PopTokensUntilAny(StringStack input, IReadOnlyCollection<string> tokens)
	{
		while (!input.IsFinished && !tokens.Any(x => input.PeekSpeech().EqualTo(x)))
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

	private static bool IsAny(string text, params string[] options)
	{
		return options.Any(x => text.EqualTo(x));
	}

	private static bool IsShopDealCreateOption(string text)
	{
		return ShopDealCreateOptions.Any(x => text.EqualTo(x));
	}
	private static bool IsJobOpeningDefinitionKeyword(string text)
	{
		return JobOpeningDefinitionKeywords.Any(x => text.EqualTo(x));
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

	private static bool TryParseRetrievalIds(ICharacter actor, IEnumerable<string> tokens,
		out List<long> prototypeIds, out List<long> specificItemIds, out string message)
	{
		prototypeIds = new List<long>();
		specificItemIds = new List<long>();
		foreach (var token in tokens.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)))
		{
			if (token.StartsWith('*'))
			{
				if (!long.TryParse(token[1..], out var itemId))
				{
					message = $"The text {token.ColourCommand()} is not a valid specific item id. Use {"*123".ColourCommand()} for item ids.";
					return false;
				}

				if (actor.Gameworld.TryGetItem(itemId, true) is null)
				{
					message = $"There is no item with id {itemId.ToString("N0", actor).ColourValue()}.";
					return false;
				}

				specificItemIds.Add(itemId);
				continue;
			}

			if (!long.TryParse(token, out var prototypeId))
			{
				message = $"The text {token.ColourCommand()} is not a valid numeric prototype id or {"*item id".ColourCommand()}.";
				return false;
			}

			if (actor.Gameworld.ItemProtos.Get(prototypeId) is null)
			{
				message = $"There is no item prototype with id {prototypeId.ToString("N0", actor).ColourValue()}.";
				return false;
			}

			prototypeIds.Add(prototypeId);
		}

		message = string.Empty;
		return prototypeIds.Any() || specificItemIds.Any();
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

	internal static ICurrency? ResolveHostCurrency(IEmploymentHost host)
	{
		return host switch
		{
			IShop shop => shop.Currency,
			IAuctionHouse auctionHouse => auctionHouse.EconomicZone.Currency,
			ICombatArena arena => arena.Currency,
			IBank bank => bank.PrimaryCurrency,
			IStable stable => stable.Currency,
			IHotel hotel => hotel.Currency,
			IClan clan => clan.ClanBankAccount?.Currency ?? ResolveContractCurrency(host),
			_ => host.EmploymentContracts.Select(x => x.Compensation.FixedRate?.Currency ?? x.Compensation.MinimumEffectivePay?.Currency)
			         .FirstOrDefault(x => x is not null)
		};
	}

	private static ICurrency? ResolveContractCurrency(IEmploymentHost host)
	{
		return host.EmploymentContracts
		           .Select(x => x.Compensation.FixedRate?.Currency ?? x.Compensation.MinimumEffectivePay?.Currency)
		           .FirstOrDefault(x => x is not null);
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
				purchase.IsExecutablePurchase
					? DescribePurchaseStep(purchase, actor)
					: $"record an audit-only purchase of {DescribeMoney(purchase.Amount)} for {purchase.PurchaseDescription.ColourName()}",
			MovementDeliveryActionStep move =>
				move.Destination is null
					? $"complete movement/delivery shell: {move.DeliveryDescription.ColourName()}"
					: $"go to {move.Destination.GetFriendlyReference(actor).ColourName()} for {move.DeliveryDescription.ColourName()}",
			CraftTriggerActionStep craft =>
				$"start or resume craft {craft.CraftDescription.ColourName()} and adopt item outputs into task custody",
			CraftStationActionStep station =>
				$"validate craft station {station.StationSelector.ColourCommand()}",
			CommandActionStep command =>
				$"execute allowlisted command {command.CommandName.ColourCommand()}{(string.IsNullOrWhiteSpace(command.CommandArguments) ? string.Empty : $" {command.CommandArguments.ColourCommand()}")}{(command.ExecutionLocation is null ? string.Empty : $" at {command.ExecutionLocation.GetFriendlyReference(actor).ColourName()}")}",
			BankDepositActionStep deposit =>
				$"deposit {DescribeMoney(deposit.Amount)} of employer virtual cash into the linked bank account",
			BankWithdrawalActionStep withdrawal =>
				$"withdraw {DescribeMoney(withdrawal.Amount)} from the linked bank account into employer virtual cash",
			BankAccountTransferActionStep transfer =>
				$"transfer {DescribeMoney(transfer.Amount)} from the linked bank account to {transfer.TargetAccountKey.ColourName()}",
			BankAdministrationActionStep bankAdmin =>
				DescribeBankAdministrationStep(bankAdmin, actor),
			HostSettlementActionStep settlement =>
				$"settle {DescribeMoney(settlement.Amount)} from this host to {settlement.TargetHostKey.ColourName()}",
			StoreAccountPaymentActionStep account =>
				$"pay {DescribeMoney(account.Amount)} to store account {account.AccountName.ColourName()}",
			TaxPaymentActionStep tax =>
				tax.MaximumAmount is null
					? "pay all supported host taxes owing"
					: $"pay supported host taxes up to {DescribeMoney(tax.MaximumAmount)}",
			PayrollSettlementActionStep payroll =>
				$"settle payroll payables matching {payroll.Selector.ColourCommand()}",
			ShopCashReconciliationActionStep cashReconciliation =>
				DescribeCashReconciliationStep(cashReconciliation),
			ShopFloatAdjustmentActionStep shopFloat =>
				$"{(shopFloat.FillRegister ? "fill" : "skim")} shop cash-register float by {DescribeMoney(shopFloat.Amount)}{(shopFloat.RegisterSelector is null ? string.Empty : $" at {DescribeItemSelector(shopFloat.RegisterSelector, actor)}")}",
			PhysicalFloatActionStep physicalFloat =>
				DescribePhysicalFloatStep(physicalFloat, actor),
			SupplierSelectionActionStep supplier =>
				DescribeSupplierSelectionStep(supplier, actor),
			DeprecatedMarketPriceChangeActionStep deprecated =>
				deprecated.Diagnostic.ColourError(),
			ShopStocktakeActionStep stocktake =>
				DescribeShopStocktakeStep(stocktake, actor),
			PriceChangeActionStep price =>
				DescribePriceChangeStep(price, actor),
			ShopDealAdministrationActionStep deal =>
				DescribeShopDealAdministrationStep(deal, actor),
			JobOpeningAdministrationActionStep opening =>
				DescribeJobOpeningAdministrationStep(opening, actor),
			ScheduledRuleAdministrationActionStep rule =>
				DescribeScheduledRuleAdministrationStep(rule, actor),
			ActiveTaskAdministrationActionStep task =>
				DescribeActiveTaskAdministrationStep(task, actor),
			ManagerGoalAdministrationActionStep goal =>
				DescribeManagerGoalAdministrationStep(goal, actor),
			StableAdministrationActionStep stable =>
				DescribeStableAdministrationStep(stable),
			HotelAdministrationActionStep hotel =>
				DescribeHotelAdministrationStep(hotel),
			BoardPostActionStep board =>
				$"post {board.Title.ColourName()} to the host staff communication board",
			CataloguedActionShellStep shell =>
				DescribeCataloguedShellStep(shell, actor),
			GetItemsByIdActionStep getId =>
				$"go to {DescribeLocations(getId.SourceLocations, actor)} and collect {getId.Quantity.ToString("N0", actor).ColourValue()}x {DescribeRetrievalTargets(getId, actor)}",
			GetItemsByTagActionStep getTag =>
				$"go to {DescribeLocations(getTag.SourceLocations, actor)} and collect {getTag.Quantity.ToString("N0", actor).ColourValue()}x items tagged {getTag.TagName.ColourCommand()}",
			GetCommodityActionStep commodity =>
				$"go to {DescribeLocations(commodity.SourceLocations, actor)} and collect {commodity.RequiredWeight.ToString("N2", actor).ColourValue()} weight of {commodity.MaterialName.ColourCommand()} commodity{(string.IsNullOrWhiteSpace(commodity.TagName) ? string.Empty : $" tagged {commodity.TagName.ColourCommand()}")}{DescribeCharacteristics(commodity.Characteristics)}",
			DeliverItemsActionStep deliver =>
				$"go to {deliver.Destination.GetFriendlyReference(actor).ColourName()} and deliver all carried task items{DescribeDeliveryContainer(deliver, actor)}",
			ShopStockTransferActionStep transfer =>
				$"go to {transfer.Destination.GetFriendlyReference(actor).ColourName()} and transfer carried stock to {transfer.TargetShop.Name.ColourName()} as {transfer.TargetMerchandise.Name.ColourName()}{DescribeStockTransferContainer(transfer, actor)}",
			AuctionLotListingActionStep auctionList =>
				$"list {DescribeItemSelector(auctionList.ItemSelector, actor)} on {auctionList.AuctionHouse.Name.ColourName()} with reserve {DescribeMoney(auctionList.ReservePrice)}{(auctionList.BuyoutPrice is null ? string.Empty : $" and buyout {DescribeMoney(auctionList.BuyoutPrice)}")}",
			AuctionSettlementActionStep auctionSettlement =>
				auctionSettlement.SettleAllDue
					? $"settle due auction lots at {auctionSettlement.AuctionHouse.Name.ColourName()}"
					: $"settle auction lot {(auctionSettlement.AssetName ?? auctionSettlement.AssetId?.ToString("N0", actor) ?? "?").ColourName()} at {auctionSettlement.AuctionHouse.Name.ColourName()}",
			AuctionClaimActionStep auctionClaim =>
				$"claim auction lot {(auctionClaim.AssetName ?? auctionClaim.AssetId.ToString("N0", actor)).ColourName()} from {auctionClaim.AuctionHouse.Name.ColourName()} into task custody",
			ArenaEventAdministrationActionStep arenaEvent =>
				DescribeArenaEventAdministrationStep(arenaEvent, actor),
			LoadItemsActionStep load =>
				$"go to {DescribeOptionalLocation(load.TargetLocation, load.TargetContainer, actor)} and load all carried task items into {DescribeItemSelector(load.TargetContainerSelector, actor)}",
			UnloadItemsActionStep unload =>
				$"go to {DescribeOptionalLocation(unload.SourceLocation, unload.SourceContainer, actor)} and unload task-loaded items from {DescribeItemSelector(unload.SourceContainerSelector, actor)}",
			ReturnAssetActionStep returnAsset =>
				$"return container {DescribeItemSelector(returnAsset.ContainerSelector, actor)} to {returnAsset.Destination.GetFriendlyReference(actor).ColourName()}{DescribeReturnDestinationContainer(returnAsset, actor)}",
			StableAnimalOperationActionStep animal =>
				DescribeStableAnimalOperationStep(animal, actor),
			VehicleOperationActionStep { AssignsDriver: true } vehicle =>
				$"assign {vehicle.Vehicle.Name.ColourName()} to the task driver",
			VehicleOperationActionStep vehicle =>
				$"validate cargo space {vehicle.CargoSpace!.Name.ColourName()} on {vehicle.Vehicle.Name.ColourName()}",
			_ => step.StepType.DescribeEnum().ColourName()
		};
	}

	private static string DescribeStableAdministrationStep(StableAdministrationActionStep stable)
	{
		return stable.Operation switch
		{
			StableAdministrationActionKind.CareInspect => $"record stable inspection evidence for stay #{stable.Stay?.Id.ToString("N0") ?? "?"}",
			StableAdministrationActionKind.CareFeed => $"record stable feeding evidence for stay #{stable.Stay?.Id.ToString("N0") ?? "?"}",
			StableAdministrationActionKind.CareGroom => $"record stable grooming evidence for stay #{stable.Stay?.Id.ToString("N0") ?? "?"}",
			StableAdministrationActionKind.CareExercise => $"record stable exercise evidence for stay #{stable.Stay?.Id.ToString("N0") ?? "?"}",
			StableAdministrationActionKind.FeeAssessment when stable.Stay is null => $"assess fees for all active stays at {stable.Stable.Name.ColourName()}",
			StableAdministrationActionKind.FeeAssessment => $"assess fees for stable stay #{stable.Stay?.Id.ToString("N0") ?? "?"} at {stable.Stable.Name.ColourName()}",
			StableAdministrationActionKind.StayReconciliation => $"record stable stay reconciliation for stay #{stable.Stay?.Id.ToString("N0") ?? "?"}",
			StableAdministrationActionKind.AccountReconciliation => $"record stable account reconciliation for {stable.Account?.AccountName.ColourName() ?? "?".ColourError()}",
			_ => "administer stable operations"
		};
	}

	private static string DescribeHotelAdministrationStep(HotelAdministrationActionStep hotel)
	{
		return hotel.Operation switch
		{
			HotelAdministrationActionKind.RoomInspect => $"record hotel room inspection evidence for {hotel.Room?.Name.ColourName() ?? "?".ColourError()}",
			HotelAdministrationActionKind.RoomClean => $"record hotel room cleaning evidence for {hotel.Room?.Name.ColourName() ?? "?".ColourError()}",
			HotelAdministrationActionKind.RoomReady => $"record hotel room readiness evidence for {hotel.Room?.Name.ColourName() ?? "?".ColourError()}",
			HotelAdministrationActionKind.RoomMaintenance => $"record hotel room maintenance evidence for {hotel.Room?.Name.ColourName() ?? "?".ColourError()}",
			HotelAdministrationActionKind.LostPropertyCheck => $"check lost-property expiry for {hotel.Hotel.Name.ColourName()}",
			HotelAdministrationActionKind.LostPropertyAudit => $"record lost-property audit for {hotel.LostProperty?.Description.ColourName() ?? "?".ColourError()}",
			HotelAdministrationActionKind.PatronBalanceAudit => $"record patron-balance audit for {hotel.PatronSelector?.ColourName() ?? hotel.PatronBalance?.PatronId.ToString("N0").ColourName() ?? "?".ColourError()}",
			_ => "administer hotel operations"
		};
	}
	private static string DescribeBankAdministrationStep(BankAdministrationActionStep bankAdmin, ICharacter actor)
	{
		var amount = bankAdmin.Amount is null ? string.Empty : DescribeMoney(bankAdmin.Amount);
		return bankAdmin.Operation switch
		{
			BankAdministrationActionKind.ReserveAudit => $"audit native reserves for {bankAdmin.Bank.Name.ColourName()}",
			BankAdministrationActionKind.ReserveDeposit => $"deposit {amount} from bank employment virtual cash into {bankAdmin.Bank.Name.ColourName()} reserves",
			BankAdministrationActionKind.ReserveWithdrawal => $"withdraw {amount} from {bankAdmin.Bank.Name.ColourName()} reserves into bank employment virtual cash",
			BankAdministrationActionKind.AccountCredit => $"credit bank account {bankAdmin.AccountSelector?.ColourName() ?? "?".ColourError()} by {amount}: {bankAdmin.Reason}",
			BankAdministrationActionKind.AccountStatus => $"set bank account {bankAdmin.AccountSelector?.ColourName() ?? "?".ColourError()} to {bankAdmin.TargetStatus?.DescribeEnum().ColourName() ?? "?".ColourError()}",
			BankAdministrationActionKind.AccountClose => $"close bank account {bankAdmin.AccountSelector?.ColourName() ?? "?".ColourError()}: {bankAdmin.Reason}",
			BankAdministrationActionKind.BranchPost => $"record bank branch post at {bankAdmin.SourceBranch?.GetFriendlyReference(actor).ColourName() ?? "?".ColourError()}: {bankAdmin.Reason}",
			BankAdministrationActionKind.BranchCourier => $"record bank branch courier from {bankAdmin.SourceBranch?.GetFriendlyReference(actor).ColourName() ?? "?".ColourError()} to {bankAdmin.DestinationBranch?.GetFriendlyReference(actor).ColourName() ?? "?".ColourError()}: {bankAdmin.Reason}",
			_ => "administer bank operations"
		};
	}
	private static string DescribeArenaEventAdministrationStep(ArenaEventAdministrationActionStep arenaEvent, ICharacter actor)
	{
		return arenaEvent.Operation switch
		{
			ArenaEventAdministrationActionKind.Create =>
				$"create arena event {DescribeArenaEventReference(arenaEvent.EventTypeName, arenaEvent.EventTypeId, actor)} at {arenaEvent.Arena.Name.ColourName()} for {DescribeArenaEventDate(arenaEvent.ScheduledForUtc, actor)}",
			ArenaEventAdministrationActionKind.Transition =>
				$"move arena event {DescribeArenaEventReference(arenaEvent.EventName, arenaEvent.EventId, actor)} at {arenaEvent.Arena.Name.ColourName()} to {arenaEvent.TargetState?.DescribeEnum().ColourValue() ?? "?".ColourError()}",
			ArenaEventAdministrationActionKind.Abort =>
				$"abort arena event {DescribeArenaEventReference(arenaEvent.EventName, arenaEvent.EventId, actor)} at {arenaEvent.Arena.Name.ColourName()}: {arenaEvent.Reason}",
			_ => "manage arena event"
		};
	}

	private static string DescribeArenaEventReference(string? name, long? id, ICharacter actor)
	{
		if (!string.IsNullOrWhiteSpace(name))
		{
			return name.ColourName();
		}

		return id.HasValue ? id.Value.ToString("N0", actor).ColourValue() : "?".ColourError();
	}

	private static string DescribeArenaEventDate(DateTime? scheduledForUtc, ICharacter actor)
	{
		return scheduledForUtc.HasValue ? scheduledForUtc.Value.ToString("f", actor).ColourValue() : "?".ColourError();
	}
	private static string DescribeStableAnimalOperationStep(StableAnimalOperationActionStep animal, ICharacter actor)
	{
		return animal.Operation switch
		{
			EmploymentAnimalOperationKind.Lead => $"lead {animal.Mount?.Name.ColourName() ?? "an animal".ColourName()} to {animal.Destination?.GetFriendlyReference(actor).ColourName() ?? "a destination".ColourName()}",
			EmploymentAnimalOperationKind.Ride => $"ride {animal.Mount?.Name.ColourName() ?? "an animal".ColourName()}",
			EmploymentAnimalOperationKind.Lodge => $"lodge {animal.Mount?.Name.ColourName() ?? "an animal".ColourName()} at {animal.Stable?.Name.ColourName() ?? "a stable".ColourName()}",
			EmploymentAnimalOperationKind.Return => $"return stable stay #{animal.Stay?.Id.ToString("N0", actor).ColourValue() ?? "?"} from {animal.Stable?.Name.ColourName() ?? "a stable".ColourName()}",
			_ => animal.Operation.DescribeEnum().ColourName()
		};
	}

	private static string DescribeCataloguedShellStep(CataloguedActionShellStep shell, ICharacter actor)
	{
		var amount = shell.Amount is null ? string.Empty : $"{DescribeMoney(shell.Amount)} for ";
		var description = shell.ActionKey switch
		{
			"authorise" => $"authorise {amount}{shell.ActionDescription}",
			"reserve" => $"reserve {amount}{shell.ActionDescription}",
			"release" => $"release employment finance reservation {shell.ActionDescription.ColourCommand()}",
			"report" => $"record report: {shell.ActionDescription}",
			"select" => $"record selection: {shell.ActionDescription}",
			"estimate" => $"record estimate: {shell.ActionDescription}",
			"route" => $"record route plan: {shell.ActionDescription}",
			"routebatch" => $"record multi-stop route batch: {shell.ActionDescription}",
			"tripcheck" => $"record transport policy check: {shell.ActionDescription}",
			_ => $"record {shell.ActionKey.ColourCommand()} action: {shell.ActionDescription}"
		};
		return shell.TargetLocation is null
			? description
			: $"go to {shell.TargetLocation.GetFriendlyReference(actor).ColourName()} and {description}";
	}

	private static string DescribeSupplierSelectionStep(SupplierSelectionActionStep supplier, ICharacter actor)
	{
		return $"find supplier for {DescribePurchaseStep(supplier.Purchase, actor)}";
	}
	private static string DescribePurchaseStep(PurchaseActionStep purchase, ICharacter actor)
	{
		var target = purchase.TargetKind switch
		{
			EmploymentPurchaseTargetKind.Item =>
				$"{purchase.Quantity!.Value.ToString("N0", actor).ColourValue()}x {DescribeItemSelector(purchase.ItemSelector!, actor)}",
			EmploymentPurchaseTargetKind.Commodity =>
				$"{purchase.CommodityWeight!.Value.ToString("N2", actor).ColourValue()} weight of commodity {purchase.CommodityDescriptor!.ColourCommand()}",
			_ =>
				$"{purchase.Quantity!.Value.ToString("N0", actor).ColourValue()}x merchandise {purchase.MerchandiseSelector!.ColourName()}"
		};
		return $"buy {target} from {(purchase.SupplierSelector ?? "any").ColourName()}{(purchase.MaximumAmount is null ? string.Empty : $" up to {DescribeMoney(purchase.MaximumAmount)}")}";
	}

	private static string DescribeCashReconciliationStep(ShopCashReconciliationActionStep cashReconciliation)
	{
		return string.IsNullOrWhiteSpace(cashReconciliation.Note)
			? "reconcile shop cash against expected float"
			: $"reconcile shop cash against expected float: {cashReconciliation.Note.ColourCommand()}";
	}
	private static string DescribePhysicalFloatStep(PhysicalFloatActionStep physicalFloat, ICharacter actor)
	{
		var amount = physicalFloat.Amount is null ? "all".ColourCommand() : DescribeMoney(physicalFloat.Amount);
		return physicalFloat.Operation switch
		{
			PhysicalFloatOperation.Issue =>
				$"issue {amount} as physical employment float from {physicalFloat.TargetKind.ColourCommand()}",
			PhysicalFloatOperation.Return =>
				$"return {amount} of physical employment float to {physicalFloat.TargetKind.ColourCommand()}{(physicalFloat.TargetSelector is null ? string.Empty : $" {DescribeItemSelector(physicalFloat.TargetSelector, actor)}")}",
			PhysicalFloatOperation.Settle =>
				$"settle {amount} of carried physical employment float back to employer virtual cash",
			_ => $"handle physical employment float with {physicalFloat.Operation.DescribeEnum().ColourCommand()}"
		};
	}

	private static string DescribeManagerGoalAdministrationStep(ManagerGoalAdministrationActionStep goal,
		ICharacter actor)
	{
		var target = $"{goal.GoalName.ColourName()} (#{goal.GoalId.ToString("N0", actor).ColourCommand()})";
		return goal.Operation switch
		{
			ManagerGoalAdministrationActionKind.Evaluate => $"evaluate manager goal {target}",
			ManagerGoalAdministrationActionKind.Cancel => $"cancel manager goal {target}",
			ManagerGoalAdministrationActionKind.Reactivate => $"reactivate manager goal {target}",
			_ => $"administer manager goal {target}"
		};
	}

	private static string DescribeActiveTaskAdministrationStep(ActiveTaskAdministrationActionStep task,
		ICharacter actor)
	{
		var target = $"{task.TaskName.ColourName()} ({task.TaskId.ToString("D").ColourCommand()})";
		return task.Operation switch
		{
			ActiveTaskAdministrationActionKind.Retry => $"retry active task {target}",
			ActiveTaskAdministrationActionKind.Requeue => $"requeue active task {target}",
			ActiveTaskAdministrationActionKind.Cancel => $"cancel active task {target}",
			ActiveTaskAdministrationActionKind.Assign => $"assign active task {target} to {(task.EmployeeName ?? task.EmployeeId?.ToString("N0", actor) ?? "?").ColourName()}",
			_ => $"administer active task {target}"
		};
	}

	private static string DescribeScheduledRuleAdministrationStep(ScheduledRuleAdministrationActionStep rule,
		ICharacter actor)
	{
		var target = $"{rule.RuleName} ({rule.RuleId:D})".ColourName();
		return rule.Operation switch
		{
			ScheduledRuleAdministrationActionKind.Pause => $"pause scheduled rule {target}",
			ScheduledRuleAdministrationActionKind.Resume => $"resume scheduled rule {target}",
			ScheduledRuleAdministrationActionKind.Cancel => $"cancel scheduled rule {target}",
			ScheduledRuleAdministrationActionKind.Evaluate => string.IsNullOrWhiteSpace(rule.ManualKey)
				? $"evaluate scheduled rule {target}"
				: $"evaluate scheduled rule {target} with manual trigger {rule.ManualKey.ColourCommand()}",
			_ => $"administer scheduled rule {target}"
		};
	}

	private static string DescribeShopStocktakeStep(ShopStocktakeActionStep stocktake, ICharacter actor)
	{
		return stocktake.Scope == ShopStocktakeScope.All
			? "stocktake all shop merchandise"
			: $"stocktake merchandise {(stocktake.MerchandiseName ?? stocktake.MerchandiseSelector ?? "?").ColourName()}";
	}
	private static string DescribePriceChangeStep(PriceChangeActionStep price, ICharacter actor)
	{
		return $"set merchandise {price.MerchandiseSelector.ColourName()} base price to {DescribeMoney(price.ExactPrice!)}";
	}

	private static string DescribeShopDealAdministrationStep(ShopDealAdministrationActionStep deal, ICharacter actor)
	{
		if (deal.Operation == ShopDealAdministrationActionKind.Cancel)
		{
			return $"cancel shop deal {deal.DealSelector.ColourCommand()}";
		}

		var operation = deal.Operation == ShopDealAdministrationActionKind.Modify
			? $"modify shop deal {deal.DealSelector.ColourCommand()} as"
			: "create";
		var name = string.IsNullOrWhiteSpace(deal.Name) ? "existing name".ColourValue() : deal.Name.ColourName();
		var target = deal.TargetType switch
		{
			ShopDealTargetType.AllMerchandise => "all merchandise".ColourValue(),
			ShopDealTargetType.Merchandise => $"merchandise {deal.TargetSelector?.ColourName() ?? "?".ColourError()}",
			ShopDealTargetType.ItemTag => $"tag {deal.TargetSelector?.ColourCommand() ?? "?".ColourError()}",
			_ => deal.TargetType.DescribeEnum().ColourName()
		};
		var type = deal.DealType == ShopDealType.Volume
			? $"volume {deal.MinimumQuantity.ToString("N0", actor).ColourValue()}+"
			: "sale".ColourName();
		var expiry = deal.Expiry.Date is null
			? "never".ColourValue()
			: deal.Expiry.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue();
		return $"{operation} {type} shop deal {name} targeting {target} with {ShopDeal.DescribePercentage(deal.PriceAdjustmentPercentage, actor)}, applies {deal.Applicability.DescribeEnum().ColourName()}, expires {expiry}";
	}

	private static string DescribeJobOpeningAdministrationStep(JobOpeningAdministrationActionStep opening,
		ICharacter actor)
	{
		return opening.Operation switch
		{
			JobOpeningAdministrationActionKind.Create when opening.Definition is not null =>
				$"create {opening.Definition.Role.DescribeEnum().ColourName()} job opening for {opening.Definition.MaxPositions.ToString("N0", actor).ColourValue()} position{(opening.Definition.MaxPositions == 1 ? string.Empty : "s")}",
			JobOpeningAdministrationActionKind.Close =>
				$"close job opening #{opening.OpeningId?.ToString("N0", actor) ?? "?".ColourError()}",
			JobOpeningAdministrationActionKind.Modify when opening.Definition is not null =>
				$"modify job opening #{opening.OpeningId?.ToString("N0", actor) ?? "?".ColourError()} to {opening.Definition.Role.DescribeEnum().ColourName()} / {opening.Definition.MaxPositions.ToString("N0", actor).ColourValue()} position{(opening.Definition.MaxPositions == 1 ? string.Empty : "s")}",
			_ => $"administer job opening with {opening.Operation.DescribeEnum().ColourCommand()}"
		};
	}

	internal static EmploymentActionDefinition? DefinitionFor(IEmploymentActionStep step)
	{
		return step switch
		{
			CataloguedActionShellStep shell => shell.Definition,
			SupplierSelectionActionStep => EmploymentActionCatalog.Get("supplier"),
			PurchaseActionStep => EmploymentActionCatalog.Get("purchase"),
			MovementDeliveryActionStep => EmploymentActionCatalog.Get("move"),
			CraftTriggerActionStep => EmploymentActionCatalog.Get("craft"),
			CraftStationActionStep => EmploymentActionCatalog.Get("station"),
			CommandActionStep => EmploymentActionCatalog.Get("command"),
			BankDepositActionStep => EmploymentActionCatalog.Get("bankdeposit"),
			BankWithdrawalActionStep => EmploymentActionCatalog.Get("bankwithdraw"),
			BankAccountTransferActionStep => EmploymentActionCatalog.Get("transfer"),
			BankAdministrationActionStep => EmploymentActionCatalog.Get("bankadmin"),
			HostSettlementActionStep => EmploymentActionCatalog.Get("settle"),
			StoreAccountPaymentActionStep => EmploymentActionCatalog.Get("storepay"),
			TaxPaymentActionStep => EmploymentActionCatalog.Get("paytax"),
			PayrollSettlementActionStep => EmploymentActionCatalog.Get("payroll"),
			ShopCashReconciliationActionStep => EmploymentActionCatalog.Get("cashreconcile"),
			ShopFloatAdjustmentActionStep => EmploymentActionCatalog.Get("float"),
			PhysicalFloatActionStep => EmploymentActionCatalog.Get("physicalfloat"),
			DeprecatedMarketPriceChangeActionStep => EmploymentActionCatalog.Get("price"),
			ShopStocktakeActionStep => EmploymentActionCatalog.Get("stocktake"),
			PriceChangeActionStep => EmploymentActionCatalog.Get("price"),
			ShopDealAdministrationActionStep => EmploymentActionCatalog.Get("sale"),
			JobOpeningAdministrationActionStep => EmploymentActionCatalog.Get("jobopening"),
			ScheduledRuleAdministrationActionStep => EmploymentActionCatalog.Get("rule"),
			ActiveTaskAdministrationActionStep => EmploymentActionCatalog.Get("admintask"),
			ManagerGoalAdministrationActionStep => EmploymentActionCatalog.Get("goal"),
			StableAdministrationActionStep => EmploymentActionCatalog.Get("stableadmin"),
			HotelAdministrationActionStep => EmploymentActionCatalog.Get("hoteladmin"),
			BoardPostActionStep => EmploymentActionCatalog.Get("board"),
			GetItemsByIdActionStep => EmploymentActionCatalog.Get("getid"),
			GetItemsByTagActionStep => EmploymentActionCatalog.Get("gettag"),
			GetCommodityActionStep => EmploymentActionCatalog.Get("commodity"),
			DeliverItemsActionStep => EmploymentActionCatalog.Get("deliver"),
			ShopStockTransferActionStep => EmploymentActionCatalog.Get("stocktransfer"),
			AuctionLotListingActionStep => EmploymentActionCatalog.Get("auctionlist"),
			AuctionSettlementActionStep => EmploymentActionCatalog.Get("auctionsettle"),
			AuctionClaimActionStep => EmploymentActionCatalog.Get("auctionclaim"),
			ArenaEventAdministrationActionStep => EmploymentActionCatalog.Get("arenaevent"),
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

	private static string DescribeSpecificItemIds(IEnumerable<long> itemIds, ICharacter actor)
	{
		return itemIds
		       .Select(x =>
		       {
			       var item = actor.Gameworld?.TryGetItem(x, true);
			       return item is null
				       ? $"item #{x.ToString("N0", actor)}".ColourValue()
				       : $"{item.HowSeen(actor, colour: false).ColourName()} (item #{x.ToString("N0", actor).ColourValue()})";
		       })
		       .ListToString();
	}

	private static string DescribeRetrievalTargets(GetItemsByIdActionStep getId, ICharacter actor)
	{
		var parts = new List<string>();
		if (getId.ItemPrototypeIds.Any())
		{
			parts.Add(DescribeItemPrototypeIds(getId.ItemPrototypeIds, actor));
		}

		if (getId.SpecificItemIds.Any())
		{
			parts.Add(DescribeSpecificItemIds(getId.SpecificItemIds, actor));
		}

		return parts.ListToString();
	}

	private static string DescribeCharacteristics(IReadOnlyDictionary<string, string> characteristics)
	{
		return characteristics.Any()
			? $" with {characteristics.Select(x => $"{x.Key.ColourCommand()}={x.Value.ColourCommand()}").ListToString()}"
			: string.Empty;
	}

	private static string DescribeStockTransferContainer(ShopStockTransferActionStep transfer, ICharacter actor)
	{
		return transfer.ContainerSelector is null
			? string.Empty
			: $" into {DescribeItemSelector(transfer.ContainerSelector, actor)}";
	}

	private static string DescribeDeliveryContainer(DeliverItemsActionStep deliver, ICharacter actor)
	{
		return deliver.ContainerSelector is null ? string.Empty : $" into {DescribeItemSelector(deliver.ContainerSelector, actor)}";
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

	private static string DescribeItemSelector(EmploymentItemSelector? selector, ICharacter actor)
	{
		if (selector is null)
		{
			return "an unresolved item".ColourError();
		}

		if (selector.Item is not null)
		{
			return selector.Item.HowSeen(actor, colour: false).ColourName();
		}

		return selector.Kind switch
		{
			EmploymentItemSelectorKind.PrototypeId =>
				$"an item of prototype #{selector.Id?.ToString("N0", actor).ColourValue()}",
			EmploymentItemSelectorKind.ItemId =>
				$"item #{selector.Id?.ToString("N0", actor).ColourValue()}",
			EmploymentItemSelectorKind.Keyword =>
				$"an item matching keyword {selector.Text?.ColourCommand() ?? "?".ColourError()}",
			EmploymentItemSelectorKind.Tag =>
				$"an item tagged {selector.Text?.ColourCommand() ?? "?".ColourError()}",
			_ => "an unresolved item".ColourError()
		};
	}

	private static string DescribeReturnDestinationContainer(ReturnAssetActionStep returnAsset, ICharacter actor)
	{
		return returnAsset.DestinationContainerSelector is null
			? string.Empty
			: $" into {DescribeItemSelector(returnAsset.DestinationContainerSelector, actor)}";
	}
}
