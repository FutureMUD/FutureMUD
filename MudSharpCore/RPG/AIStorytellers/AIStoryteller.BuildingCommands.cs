using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.RPG.AIStorytellers;

public partial class AIStoryteller
{
	private const string HelpText = @"You can use the following options:

	#3name <name>#0 - renames this storyteller
	#3description#0 - edits the description text in an editor
	#3model <model>#0 - sets the OpenAI model
	#3reasoning <minimal|low|medium|high>#0 - sets reasoning effort
	#3attention#0 - edits the attention prompt in an editor
	#3system#0 - edits the system prompt in an editor
	#3pause#0 - pauses all storyteller triggers
	#3unpause#0 - unpauses all storyteller triggers
	#3subscribe room|speech|crime|state|5m|10m|30m|1h#0 - toggles trigger subscriptions
	#3statusprog <5m|10m|30m|1h> <prog|none>#0 - sets a heartbeat status prog
	#3customplayerprog <prog|none>#0 - sets an optional custom player info prog
	#3surveillance <...>#0 - edits surveillance strategy details
	#3tool add <name> <prog> [echo]#0 - adds a custom tool
	#3tool remove <name>#0 - removes a custom tool
	#3tool description <name> <text>#0 - sets a tool description
	#3tool parameter <name> <parameter> <description>#0 - sets a parameter description
	#3tool prog <name> <prog>#0 - changes the prog bound to a tool
	#3tool echo <name>#0 - toggles whether a tool is echo-only
	#3refsearch <query>#0 - searches visible reference documents";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor);
			case "model":
				return BuildingCommandModel(actor, command);
			case "reasoning":
			case "effort":
				return BuildingCommandReasoning(actor, command);
			case "attention":
			case "attentionprompt":
				return BuildingCommandAttentionPrompt(actor);
			case "system":
			case "systemprompt":
				return BuildingCommandSystemPrompt(actor);
			case "pause":
				if (IsPaused)
				{
					actor.OutputHandler.Send("This storyteller is already paused.");
					return false;
				}

				Pause();
				actor.OutputHandler.Send("This storyteller is now paused.");
				return true;
			case "unpause":
				if (!IsPaused)
				{
					actor.OutputHandler.Send("This storyteller is already unpaused.");
					return false;
				}

				Unpause();
				actor.OutputHandler.Send("This storyteller is now unpaused.");
				return true;
			case "subscribe":
			case "subscriptions":
				return BuildingCommandSubscribe(actor, command);
			case "statusprog":
			case "heartbeatprog":
				return BuildingCommandStatusProg(actor, command);
			case "customplayerprog":
			case "playerprog":
				return BuildingCommandCustomPlayerProg(actor, command);
			case "surveillance":
			case "watch":
				return BuildingCommandSurveillance(actor, command);
			case "tool":
			case "tools":
				return BuildingCommandTool(actor, command);
			case "refsearch":
			case "reference":
			case "references":
				return BuildingCommandReferenceSearch(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name should this storyteller have?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (Gameworld.AIStorytellers.Any(x => x.Id != Id && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already an AI storyteller named {name.ColourName()}.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"This storyteller is now named {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor)
	{
		actor.OutputHandler.Send("Enter the new description in the editor below.");
		actor.EditorMode(BuildingCommandDescriptionPost, BuildingCommandDescriptionCancel, 1.0, Description);
		return true;
	}

	private void BuildingCommandDescriptionCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the storyteller description.");
	}

	private void BuildingCommandDescriptionPost(string text, IOutputHandler handler, object[] args)
	{
		Description = text;
		Changed = true;
		handler.Send("You update the storyteller description.");
	}

	private bool BuildingCommandModel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which OpenAI model should this storyteller use?");
			return false;
		}

		Model = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This storyteller now uses the {Model.ColourValue()} model.");
		return true;
	}

	private bool BuildingCommandReasoning(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify one of {"minimal".ColourCommand()}, {"low".ColourCommand()}, {"medium".ColourCommand()} or {"high".ColourCommand()}.");
			return false;
		}

		if (!TryParseReasoningEffort(command.SafeRemainingArgument, out var effort))
		{
			actor.OutputHandler.Send(
				$"That is not a valid reasoning effort. Use {"minimal".ColourCommand()}, {"low".ColourCommand()}, {"medium".ColourCommand()} or {"high".ColourCommand()}.");
			return false;
		}

		ReasoningEffort = effort;
		Changed = true;
		actor.OutputHandler.Send(
			$"This storyteller now uses {ReasoningEffort.Describe().ColourValue()} reasoning effort.");
		return true;
	}

	private bool BuildingCommandAttentionPrompt(ICharacter actor)
	{
		actor.OutputHandler.Send("Enter the new attention prompt in the editor below.");
		actor.EditorMode(BuildingCommandAttentionPromptPost, BuildingCommandAttentionPromptCancel, 1.0,
			AttentionAgentPrompt);
		return true;
	}

	private void BuildingCommandAttentionPromptCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the attention prompt.");
	}

	private void BuildingCommandAttentionPromptPost(string text, IOutputHandler handler, object[] args)
	{
		AttentionAgentPrompt = text;
		Changed = true;
		handler.Send("You update the attention prompt.");
	}

	private bool BuildingCommandSystemPrompt(ICharacter actor)
	{
		actor.OutputHandler.Send("Enter the new system prompt in the editor below.");
		actor.EditorMode(BuildingCommandSystemPromptPost, BuildingCommandSystemPromptCancel, 1.0, SystemPrompt);
		return true;
	}

	private void BuildingCommandSystemPromptCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the system prompt.");
	}

	private void BuildingCommandSystemPromptPost(string text, IOutputHandler handler, object[] args)
	{
		SystemPrompt = text;
		Changed = true;
		handler.Send("You update the system prompt.");
	}

	private bool BuildingCommandSubscribe(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify one of {"room".ColourCommand()}, {"speech".ColourCommand()}, {"crime".ColourCommand()}, {"state".ColourCommand()}, {"5m".ColourCommand()}, {"10m".ColourCommand()}, {"30m".ColourCommand()} or {"1h".ColourCommand()}.");
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "room":
			case "echo":
			case "echoes":
				SubscribeToRoomEvents = !SubscribeToRoomEvents;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send($"Subscribe To Room Events is now {SubscribeToRoomEvents.ToColouredString()}.");
				return true;
			case "speech":
			case "speaks":
			case "speak":
			case "say":
				SubscribeToSpeechEvents = !SubscribeToSpeechEvents;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send(
					$"Subscribe To Character Speech Events is now {SubscribeToSpeechEvents.ToColouredString()}.");
				return true;
			case "crime":
			case "crimes":
				SubscribeToCrimeEvents = !SubscribeToCrimeEvents;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send(
					$"Subscribe To Character Crime Events is now {SubscribeToCrimeEvents.ToColouredString()}.");
				return true;
			case "state":
			case "states":
			case "status":
			case "health":
				SubscribeToStateEvents = !SubscribeToStateEvents;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send(
					$"Subscribe To Character State Events is now {SubscribeToStateEvents.ToColouredString()}.");
				return true;
			case "5m":
			case "5":
				SubscribeTo5mHeartbeat = !SubscribeTo5mHeartbeat;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send($"Subscribe To 5m Tick is now {SubscribeTo5mHeartbeat.ToColouredString()}.");
				return true;
			case "10m":
			case "10":
				SubscribeTo10mHeartbeat = !SubscribeTo10mHeartbeat;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send(
					$"Subscribe To 10m Tick is now {SubscribeTo10mHeartbeat.ToColouredString()}.");
				return true;
			case "30m":
			case "30":
				SubscribeTo30mHeartbeat = !SubscribeTo30mHeartbeat;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send(
					$"Subscribe To 30m Tick is now {SubscribeTo30mHeartbeat.ToColouredString()}.");
				return true;
			case "1h":
			case "hour":
			case "60m":
				SubscribeToHourHeartbeat = !SubscribeToHourHeartbeat;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send($"Subscribe To 1h Tick is now {SubscribeToHourHeartbeat.ToColouredString()}.");
				return true;
			default:
				actor.OutputHandler.Send(
					$"You must specify one of {"room".ColourCommand()}, {"speech".ColourCommand()}, {"crime".ColourCommand()}, {"state".ColourCommand()}, {"5m".ColourCommand()}, {"10m".ColourCommand()}, {"30m".ColourCommand()} or {"1h".ColourCommand()}.");
				return false;
		}
	}

	private bool BuildingCommandStatusProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which heartbeat interval do you want to edit? (5m, 10m, 30m, 1h)");
			return false;
		}

		var which = command.PopForSwitch();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog or use #3none#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		IFutureProg prog = null;
		if (!command.SafeRemainingArgument.EqualTo("none"))
		{
			prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
			if (prog is null)
			{
				actor.OutputHandler.Send("There is no such prog.");
				return false;
			}
		}

		switch (which)
		{
			case "5m":
			case "5":
				HeartbeatStatus5mProg = prog;
				break;
			case "10m":
			case "10":
				HeartbeatStatus10mProg = prog;
				break;
			case "30m":
			case "30":
				HeartbeatStatus30mProg = prog;
				break;
			case "1h":
			case "hour":
			case "60m":
				HeartbeatStatus1hProg = prog;
				break;
			default:
				actor.OutputHandler.Send("You must specify one of 5m, 10m, 30m or 1h.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"That heartbeat status prog is now set to {prog?.MXPClickableFunctionName() ?? "None".ColourError()}.");
		return true;
	}

	private bool BuildingCommandCustomPlayerProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog or use #3none#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			CustomPlayerInformationProg = null;
			Changed = true;
			actor.OutputHandler.Send("This storyteller will no longer use a custom player information prog.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog is null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		CustomPlayerInformationProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This storyteller will now use {prog.MXPClickableFunctionName()} for custom player information.");
		return true;
	}

	private bool BuildingCommandSurveillance(ICharacter actor, StringStack command)
	{
		var result = SurveillanceStrategy.BuildingCommand(actor, command);
		if (!result)
		{
			return false;
		}

		Changed = true;
		SubscribeEvents();
		return true;
	}

	private bool BuildingCommandTool(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
			case "new":
				return BuildingCommandToolAdd(actor, command);
			case "remove":
			case "delete":
				return BuildingCommandToolRemove(actor, command);
			case "description":
			case "desc":
				return BuildingCommandToolDescription(actor, command);
			case "parameter":
			case "param":
				return BuildingCommandToolParameter(actor, command);
			case "prog":
				return BuildingCommandToolProg(actor, command);
			case "echo":
				return BuildingCommandToolEcho(actor, command);
			default:
				actor.OutputHandler.Send(@"You can use the following options:

	#3tool add <name> <prog> [echo]#0 - adds a new custom tool
	#3tool remove <name>#0 - removes a custom tool
	#3tool description <name> <text>#0 - sets a custom tool description
	#3tool parameter <name> <parameter> <description>#0 - sets parameter description text
	#3tool prog <name> <prog>#0 - changes the prog bound to a tool
	#3tool echo <name>#0 - toggles whether a tool is echo-only".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandToolAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should this tool be called?");
			return false;
		}

		var name = command.PopSpeech();
		if (CustomToolCalls.Any(x => x.Name.EqualTo(name)) || CustomToolCallsEchoOnly.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a custom tool with that name.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should this tool call?");
			return false;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.PopSpeech());
		if (prog is null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		var includeWithEcho = !command.IsFinished && command.PopForSwitch().StartsWith("echo");
		var tool = new AIStorytellerCustomToolCall(name, $"Custom tool {name}", prog);
		if (includeWithEcho)
		{
			CustomToolCallsEchoOnly.Add(tool);
		}
		else
		{
			CustomToolCalls.Add(tool);
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"You create the custom tool {name.ColourCommand()} using {prog.MXPClickableFunctionName()} ({(includeWithEcho ? "echo-only" : "always available").ColourValue()}).");
		return true;
	}

	private bool BuildingCommandToolRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which custom tool do you want to remove?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		var tool = CustomToolCalls.FirstOrDefault(x => x.Name.EqualTo(name));
		if (tool is not null)
		{
			CustomToolCalls.Remove(tool);
			Changed = true;
			actor.OutputHandler.Send($"You remove the custom tool {tool.Name.ColourCommand()}.");
			return true;
		}

		tool = CustomToolCallsEchoOnly.FirstOrDefault(x => x.Name.EqualTo(name));
		if (tool is not null)
		{
			CustomToolCallsEchoOnly.Remove(tool);
			Changed = true;
			actor.OutputHandler.Send($"You remove the custom tool {tool.Name.ColourCommand()}.");
			return true;
		}

		actor.OutputHandler.Send("There is no custom tool with that name.");
		return false;
	}

	private bool BuildingCommandToolDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which custom tool do you want to edit?");
			return false;
		}

		var tool = FindCustomTool(command.PopSpeech());
		if (tool is null)
		{
			actor.OutputHandler.Send("There is no custom tool with that name.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description should this custom tool have?");
			return false;
		}

		tool.Description = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"Tool {tool.Name.ColourCommand()} now has updated description text.");
		return true;
	}

	private bool BuildingCommandToolParameter(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which custom tool do you want to edit?");
			return false;
		}

		var tool = FindCustomTool(command.PopSpeech());
		if (tool is null)
		{
			actor.OutputHandler.Send("There is no custom tool with that name.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which parameter do you want to edit?");
			return false;
		}

		var which = command.PopSpeech();
		var parameter = tool.Prog?.NamedParameters
			.Select(x => x.Item2)
			.FirstOrDefault(x => x.EqualTo(which));
		if (parameter is null)
		{
			actor.OutputHandler.Send("There is no such named parameter on that tool's prog.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description should this parameter have?");
			return false;
		}

		tool.SetParameterDescription(parameter, command.SafeRemainingArgument);
		Changed = true;
		actor.OutputHandler.Send(
			$"Tool {tool.Name.ColourCommand()} parameter {parameter.ColourCommand()} now has updated description text.");
		return true;
	}

	private bool BuildingCommandToolProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which custom tool do you want to edit?");
			return false;
		}

		var tool = FindCustomTool(command.PopSpeech());
		if (tool is null)
		{
			actor.OutputHandler.Send("There is no custom tool with that name.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should this tool call?");
			return false;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog is null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		tool.Prog = prog;
		tool.RefreshParameterDescriptions();
		Changed = true;
		actor.OutputHandler.Send(
			$"Tool {tool.Name.ColourCommand()} now calls {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandToolEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which custom tool do you want to toggle?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		var tool = CustomToolCalls.FirstOrDefault(x => x.Name.EqualTo(name));
		if (tool is not null)
		{
			CustomToolCalls.Remove(tool);
			CustomToolCallsEchoOnly.Add(tool);
			Changed = true;
			actor.OutputHandler.Send($"Tool {tool.Name.ColourCommand()} is now echo-only.");
			return true;
		}

		tool = CustomToolCallsEchoOnly.FirstOrDefault(x => x.Name.EqualTo(name));
		if (tool is not null)
		{
			CustomToolCallsEchoOnly.Remove(tool);
			CustomToolCalls.Add(tool);
			Changed = true;
			actor.OutputHandler.Send($"Tool {tool.Name.ColourCommand()} is now always available.");
			return true;
		}

		actor.OutputHandler.Send("There is no custom tool with that name.");
		return false;
	}

	private AIStorytellerCustomToolCall FindCustomTool(string name)
	{
		return CustomToolCalls.FirstOrDefault(x => x.Name.EqualTo(name)) ??
		       CustomToolCallsEchoOnly.FirstOrDefault(x => x.Name.EqualTo(name));
	}

	private bool BuildingCommandReferenceSearch(ICharacter actor, StringStack command)
	{
		var query = command.SafeRemainingArgument;
		var docs = Gameworld.AIStorytellerReferenceDocuments
			.Where(IsReferenceDocumentVisibleToStoryteller)
			.Where(x => x.ReturnForSearch(query))
			.ToList();

		if (!docs.Any())
		{
			actor.OutputHandler.Send("There are no matching reference documents.");
			return false;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in docs
			let concrete = item as AIStorytellerReferenceDocument
			select new List<string>
			{
				item.Id.ToStringN0(actor),
				item.Name,
				concrete?.FolderName ?? string.Empty,
				concrete?.DocumentType ?? string.Empty,
				concrete?.Keywords ?? string.Empty
			},
			[
				"Id",
				"Name",
				"Folder",
				"Type",
				"Keywords"
			],
			actor,
			Telnet.Green));
		return false;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"AI Storyteller #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan,
			Telnet.BoldWhite));
		sb.AppendLine($"Model: {Model.ColourValue()}");
		sb.AppendLine($"Reasoning Effort: {ReasoningEffort.Describe().ColourValue()}");
		sb.AppendLine($"Is Paused: {IsPaused.ToColouredString()}");
		sb.AppendLine($"");
		sb.AppendLine("Description".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine($"");
		sb.AppendLine("Surveillance and Events".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine($"Subscribe To Room Events: {SubscribeToRoomEvents.ToColouredString()}");
		sb.AppendLine($"Subscribe To Character Speech Events: {SubscribeToSpeechEvents.ToColouredString()}");
		sb.AppendLine($"Subscribe To Character Crime Events: {SubscribeToCrimeEvents.ToColouredString()}");
		sb.AppendLine($"Subscribe To Character State Events: {SubscribeToStateEvents.ToColouredString()}");
		sb.AppendLine($"Subscribe To 5m Tick: {SubscribeTo5mHeartbeat.ToColouredString()}");
		sb.AppendLine($"Subscribe To 10m Tick: {SubscribeTo10mHeartbeat.ToColouredString()}");
		sb.AppendLine($"Subscribe To 30m Tick: {SubscribeTo30mHeartbeat.ToColouredString()}");
		sb.AppendLine($"Subscribe To 1h Tick: {SubscribeToHourHeartbeat.ToColouredString()}");
		sb.AppendLine($"Status Prog for 5m: {HeartbeatStatus5mProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Status Prog for 10m: {HeartbeatStatus10mProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Status Prog for 30m: {HeartbeatStatus30mProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Status Prog for 1h: {HeartbeatStatus1hProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine(
			$"Custom Player Info Prog: {CustomPlayerInformationProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"");
		sb.AppendLine(SurveillanceStrategy.Show(actor));
		sb.AppendLine($"");
		sb.AppendLine("Attention Prompt".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(AttentionAgentPrompt.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine($"");
		sb.AppendLine("System Prompt".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(SystemPrompt.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine($"");
		sb.AppendLine("Custom Tool Calls".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		if (CustomToolCalls.Count == 0 && CustomToolCallsEchoOnly.Count == 0)
		{
			sb.AppendLine();
			sb.AppendLine("None");
		}
		else
		{
			foreach (var item in CustomToolCalls)
			{
				var valid = item.IsValid; // This call has side effects and so needs to happen before the foreach loop below to guarantee the keys all exist
				sb.AppendLine();
				sb.AppendLine($"\tFunction Name: {item.Name.ColourValue()}");
				sb.AppendLine($"\tFunction Description: {item.Description.ColourValue()}");
				sb.AppendLine($"\tFunction Prog: {item.Prog?.MXPClickableFunctionName() ?? "None".ColourError()}");
				sb.AppendLine($"\tAvailable Context: {"Always".ColourValue()}");
				sb.AppendLine($"\tFunction Parameters:");
				foreach (var par in item.Prog?.NamedParameters ?? [])
				{
					sb.AppendLine($"\t\t{par.Item1.Describe().Colour(Telnet.VariableGreen)} {par.Item2}: {item.ParameterDescriptions[par.Item2]}");
				}
				if (!valid)
				{
					sb.AppendLine("\t\tWarning - this tool call is not valid due to errors.".ColourError());
				}
			}

			foreach (var item in CustomToolCallsEchoOnly)
			{
				var valid = item.IsValid; // This call has side effects and so needs to happen before the foreach loop below to guarantee the keys all exist
				sb.AppendLine();
				sb.AppendLine($"\tFunction Name: {item.Name.ColourValue()}");
				sb.AppendLine($"\tFunction Description: {item.Description.ColourValue()}");
				sb.AppendLine($"\tFunction Prog: {item.Prog?.MXPClickableFunctionName() ?? "None".ColourError()}");
				sb.AppendLine($"\tAvailable Context: {"Echoes Only".ColourValue()}");
				sb.AppendLine($"\tFunction Parameters:");
				foreach (var par in item.Prog?.NamedParameters ?? [])
				{
					sb.AppendLine($"\t\t{par.Item1.Describe().Colour(Telnet.VariableGreen)} {par.Item2}: {item.ParameterDescriptions[par.Item2]}");
				}
				if (!valid)
				{
					sb.AppendLine("\t\tWarning - this tool call is not valid due to errors.".ColourError());
				}
			}
		}
		sb.AppendLine($"");
		sb.AppendLine("Current Situations".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in _situations
			select new List<string> {
				item.Id.ToStringN0(actor),
				item.Name,
				item.CreatedOn.GetLocalDateString(actor, true)
			},
			[
				"Id",
				"Title",
				"Created"
			],
			actor,
			Telnet.Green
		));

		return sb.ToString();
	}

}
