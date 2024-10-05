using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.FutureProg.Statements.Manipulation;

namespace MudSharp.NPC.AI;

public class CommandableAI : ArtificialIntelligenceBase
{
	private IFutureProg _canCommandProg;
	private IFutureProg _whyCannotCommandProg;
	private List<string> _bannedCommands;
	private List<string> _includedCommands;
	private string _commandIssuedEmoteText;

	protected CommandableAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		var root = XElement.Parse(ai.Definition);
		_canCommandProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanCommandProg").Value));
		_whyCannotCommandProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotCommandProg")?.Value ?? "0"));
		_bannedCommands = (root.Element("BannedCommands")?.Elements().Select(x => x.Value.ToLowerInvariant()) ??
		                   Enumerable.Empty<string>()).ToList();
		_includedCommands = (root.Element("IncludedCommands")?.Elements().Select(x => x.Value.ToLowerInvariant()) ??
		                     Enumerable.Empty<string>()).ToList();
		_commandIssuedEmoteText = root.Element("CommandIssuedEmote")?.Value ?? string.Empty;
	}

	private CommandableAI()
	{

	}

	private CommandableAI(IFuturemud gameworld, string name) : base(gameworld, name, "Commandable")
	{
		_canCommandProg = Gameworld.AlwaysFalseProg;
		DatabaseInitialise();
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("CanCommandProg", _canCommandProg?.Id ?? 0L),
			new XElement("WhyCannotCommandProg", _whyCannotCommandProg?.Id ?? 0L),
			new XElement("CommandIssuedEmote", new XCData(_commandIssuedEmoteText)),
			new XElement("BannedCommands", from cmd in _bannedCommands select new XElement("Command", new XCData(cmd))),
			new XElement("IncludedCommands", from cmd in _includedCommands select new XElement("Command", new XCData(cmd)))
		).ToString();
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type != EventType.CommandIssuedToCharacter)
		{
			return false;
		}

		var ch = (ICharacter)arguments[0];
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		var commandCh = (ICharacter)arguments[1];
		var commandText = (string)arguments[2];

		if ((bool?)_canCommandProg.Execute(ch, commandCh, commandText) != true)
		{
			if (_whyCannotCommandProg != null)
			{
				commandCh.Send((string)_whyCannotCommandProg.Execute(ch, commandCh, commandText));
			}
			else
			{
				if (commandCh.CanSee(ch))
				{
					commandCh.Send($"You can't issue commands to {ch.HowSeen(commandCh)}.");
				}
				else
				{
					commandCh.Send("You don't see anyone like that to issue a command to.");
				}
			}

			return true;
		}

		var whichCommand = new StringStack(commandText).Pop();
		var locatedCommand = ch.CommandTree.Commands.LocateCommand(ch, ref whichCommand);
		if (locatedCommand != null)
		{
			if (!string.IsNullOrWhiteSpace(locatedCommand.Name) &&
			    _bannedCommands.Contains(locatedCommand.Name.ToLowerInvariant()))
			{
				commandCh.Send(
					$"You are not allowed to command {ch.HowSeen(commandCh)} to do the {locatedCommand.Name.ColourCommand()} command.");
				return true;
			}

			if (_includedCommands.Count > 0 && !_includedCommands.Contains(locatedCommand.Name.ToLowerInvariant()))
			{
				commandCh.Send(
					$"You are not allowed to command {ch.HowSeen(commandCh)} to do the {locatedCommand.Name.ColourCommand()} command.");
				return true;
			}
		}

		if (_commandIssuedEmoteText is not null)
		{
			var emote = new EmoteOutput(new Emote(string.Format(_commandIssuedEmoteText, commandText), commandCh,
				commandCh, ch));
			commandCh.OutputHandler.Send(emote);
		}

		locatedCommand?.Execute(ch, commandText, ch.State, ch.Account.Authority.Level, ch.OutputHandler);
		return true;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return types.Contains(EventType.CommandIssuedToCharacter);
	}

	public static void RegisterLoader()
	{
		RegisterAIType("Commandable", (ai, gameworld) => new CommandableAI(ai, gameworld));
		RegisterAIBuilderInformation("commandable", (gameworld, name) => new CommandableAI(gameworld, name), new CommandableAI().HelpText);
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Can Command Prog: {_canCommandProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Why Can't Prog: {_whyCannotCommandProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Commanded Emote: {_commandIssuedEmoteText?.ColourCommand() ?? ""}");
		sb.AppendLine($"Included Commands: {(_includedCommands.Any() ? _includedCommands.Select(x => x.ColourCommand()).ListToCommaSeparatedValues(", ") : "all".ColourValue())}");
		sb.AppendLine($"Banned Commands: {(_bannedCommands.Any() ? _bannedCommands.Select(x => x.ColourCommand()).ListToCommaSeparatedValues(", ") : "all".ColourValue())}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => 
		@"	#3can <prog>#0 - sets the prog that controls if a PC can command the NPC
	#3why <prog>#0 - sets a prog to give an error message if the PC can't command the NPC
	#3why clear#0 - clears the custom prog, goes back to default message
	#3emote <emote>#0 - sets an emote the NPC does when commanded. $0 is NPC, $1 is commander
	#3emote clear#0 - removes the command emote
	#3included <command>#0 - adds a command to the included list
	#3included clear#0 - clears all included commands
	#3banned <command>#0 - adds a command to the banned list
	#3banned clear#0 - clears all banned commands

#BNote: By default, all commands are permitted. If you have any included commands at all, only those commands will be permitted. Use either an empty included list with a curated banned list or use an included list to carefully curate the commands.#0";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "canprog":
			case "can":
				return BuildingCommandCanProg(actor, command);
			case "whyprog":
			case "why":
				return BuildingCommandWhyProg(actor, command);
			case "commandemote":
			case "emote":
				return BuildingCommandCommandEmote(actor, command);
			case "banned":
				return BuildingCommandBanned(actor, command);
			case "included":
				return BuildingCommandIncluded(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandIncluded(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a command to toggle or use #3clear#0 to clear all."
				.SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			_includedCommands.Clear();
			Changed = true;
			actor.OutputHandler.Send(
				"This commandable AI no longer has any included commands (it permits all commands unless banned).");
			return true;
		}

		var cmd = command.SafeRemainingArgument.ToLowerInvariant();
		if (_includedCommands.Remove(cmd))
		{
			Changed = true;
			actor.OutputHandler.Send(
				$"The command {cmd.ColourCommand()} is no longer permitted.");
			return true;
		}

		_includedCommands.Add(cmd);
		Changed = true;
		actor.OutputHandler.Send($"The command {cmd.ColourCommand()} is now permitted.");
		return true;
	}

	private bool BuildingCommandBanned(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a command to toggle or use #3clear#0 to clear all."
				.SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			_bannedCommands.Clear();
			Changed = true;
			actor.OutputHandler.Send(
				"This commandable AI no longer has any banned commands.");
			return true;
		}

		var cmd = command.SafeRemainingArgument.ToLowerInvariant();
		if (_bannedCommands.Remove(cmd))
		{
			Changed = true;
			actor.OutputHandler.Send(
				$"The command {cmd.ColourCommand()} is no longer banned.");
			return true;
		}

		_bannedCommands.Add(cmd);
		Changed = true;
		actor.OutputHandler.Send($"The command {cmd.ColourCommand()} is now banned.");
		return true;
	}

	private bool BuildingCommandCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify an emote or use #3clear#0 to clear it."
				.SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			_commandIssuedEmoteText = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This commandable AI no longer issues an emote when commanded.");
			return true;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		_commandIssuedEmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now do the following emote when commanded ($0 is NPC, $1 is commander):\n{_commandIssuedEmoteText.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandWhyProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
		{
			_whyCannotCommandProg = null;
			Changed = true;
			actor.OutputHandler.Send($"There is no longer a custom message for why the NPC can't commanded. A default message will be used instead.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, 
			FutureProgVariableTypes.Text,
			new []{ 
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Text }
			}
			).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_whyCannotCommandProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to send an error message if the player can't command the NPC.");
		return true;
	}

	private bool BuildingCommandCanProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean,
			new[]{
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Text }
			}
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_canCommandProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to control whether a payer can command the NPC.");
		return true;
	}
}