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

namespace MudSharp.NPC.AI;

public class CommandableAI : ArtificialIntelligenceBase
{
	private IFutureProg _canCommandProg;
	private IFutureProg _whyCannotCommandProg;
	private List<string> _bannedCommands;
	private List<string> _includedCommands;
	private string _commandIssuedEmoteText;

	public CommandableAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		var root = XElement.Parse(ai.Definition);
		_canCommandProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanCommandProg").Value));
		_whyCannotCommandProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotCommandProg")?.Value ?? "0"));
		_bannedCommands = (root.Element("BannedCommands")?.Elements().Select(x => x.Value.ToLowerInvariant()) ??
		                   Enumerable.Empty<string>()).ToList();
		_includedCommands = (root.Element("IncludedCommands")?.Elements().Select(x => x.Value.ToLowerInvariant()) ??
		                     Enumerable.Empty<string>()).ToList();
		_commandIssuedEmoteText = root.Element("CommandIssuedEmote")?.Value;
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

		var emote = new EmoteOutput(new Emote(string.Format(_commandIssuedEmoteText, commandText), commandCh,
			commandCh, ch));
		commandCh.OutputHandler.Send(emote);
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
	}
}