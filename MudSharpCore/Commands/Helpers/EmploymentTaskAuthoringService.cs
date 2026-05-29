using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Employment;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;

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

		if (!TryParseStep(actor, input, out var step, out message))
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
			if (!TryParseStep(actor, stack, out var step, out message))
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
				sb.AppendLine($"\t{(i + 1).ToString("N0", actor)} - {DescribeStep(draft.Steps[i], actor)}");
			}
		}

		return sb.ToString();
	}

	public string RenderAvailableActions(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Employment Task Actions".ColourName());
		sb.AppendLine("Use these with ".ColourCommand() + "tasks step <action> ...".ColourCommand() + " while you have a draft open.");
		sb.AppendLine("You can also create and finalise a task in one command with ".ColourCommand() + "tasks create <name> <action> then <action>".ColourCommand() + ".");
		sb.AppendLine();
		sb.AppendLine($"{"tasks step getid <quantity> <item prototype ids...> from <here|cell ids...>".ColourCommand()}");
		sb.AppendLine("\tGet a quantity of items matching specific item prototype IDs from one or more source locations.");
		sb.AppendLine($"{"tasks step gettag <quantity> <tag> from <here|cell ids...>".ColourCommand()}");
		sb.AppendLine("\tGet a quantity of items matching a tag from one or more source locations.");
		sb.AppendLine($"{"tasks step commodity <weight> <material> [tag <tag>] from <here|cell ids...> [char <name>=<value> ...]".ColourCommand()}");
		sb.AppendLine("\tGet a total commodity weight by material, optional tag, and optional characteristics.");
		sb.AppendLine($"{"tasks step deliver to <here|cell id> [container <item id>|containertag <tag>]".ColourCommand()}");
		sb.AppendLine("\tDeliver all carried task items to a destination, optionally into a container.");
		return sb.ToString();
	}

	private static string RenderCreatedTaskSummary(ICharacter actor, IEmploymentActiveTask task)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"You create active employment task {task.Name.ColourName()} with {task.ActionPlan.Steps.Count.ToString("N0", actor).ColourValue()} step{(task.ActionPlan.Steps.Count == 1 ? string.Empty : "s")}:");
		for (var i = 0; i < task.ActionPlan.Steps.Count; i++)
		{
			sb.AppendLine($"\t#{(i + 1).ToString("N0", actor)} - {DescribeStep(task.ActionPlan.Steps[i], actor)}");
		}

		return sb.ToString();
	}

	private static bool TryParseStep(ICharacter actor, StringStack input, out IEmploymentActionStep step,
		out string message)
	{
		step = null!;
		if (input.IsFinished)
		{
			message = "Which kind of step do you want to add?";
			return false;
		}

		var stepType = input.PopSpeech().CollapseString().ToLowerInvariant();
		return stepType switch
		{
			"getid" or "id" => TryParseGetId(actor, input, out step, out message),
			"gettag" or "tag" => TryParseGetTag(actor, input, out step, out message),
			"commodity" or "material" => TryParseCommodity(actor, input, out step, out message),
			"deliver" or "delivery" => TryParseDeliver(actor, input, out step, out message),
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
			GetItemsByIdActionStep getId =>
				$"go to {DescribeLocations(getId.SourceLocations, actor)} and collect {getId.Quantity.ToString("N0", actor).ColourValue()}x {DescribeItemPrototypeIds(getId.ItemPrototypeIds, actor)}",
			GetItemsByTagActionStep getTag =>
				$"go to {DescribeLocations(getTag.SourceLocations, actor)} and collect {getTag.Quantity.ToString("N0", actor).ColourValue()}x items tagged {getTag.TagName.ColourCommand()}",
			GetCommodityActionStep commodity =>
				$"go to {DescribeLocations(commodity.SourceLocations, actor)} and collect {commodity.RequiredWeight.ToString("N2", actor).ColourValue()} weight of {commodity.MaterialName.ColourCommand()} commodity{(string.IsNullOrWhiteSpace(commodity.TagName) ? string.Empty : $" tagged {commodity.TagName.ColourCommand()}")}{DescribeCharacteristics(commodity.Characteristics)}",
			DeliverItemsActionStep deliver =>
				$"go to {deliver.Destination.GetFriendlyReference(actor).ColourName()} and deliver all carried task items{DescribeDeliveryContainer(deliver, actor)}",
			_ => step.StepType.DescribeEnum().ColourName()
		};
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
}
