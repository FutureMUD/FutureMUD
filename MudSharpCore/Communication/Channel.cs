using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Statements;
using MudSharp.Models;
using MudSharp.PerceptionEngine;

namespace MudSharp.Communication;


public class Channel : SaveableItem, IChannel
{
	protected readonly List<long> _ignoringAccountIDs = new();
	protected bool _announceChannelJoiners;
	protected bool _announceMissedListeners;
	protected string _channelColour;
	protected IFutureProg _channelListenerProg;
	private bool _addToPlayerCommandTree;
	private bool _addToGuideCommandTree;
	private ulong? _discordChannelId;

	protected string _channelName;
	protected IFutureProg _channelSpeakerProg;
	protected ChannelSpeakerNameMode _mode;

	public bool AnnounceChannelJoiners => _announceChannelJoiners;
	public bool AnnounceMissedListeners => _announceMissedListeners;
	public bool AddToPlayerCommandTree => _addToPlayerCommandTree;
	public bool AddToGuideCommandTree => _addToGuideCommandTree;
	public ChannelSpeakerNameMode Mode => _mode;
	public IFutureProg ChannelListenerProg => _channelListenerProg;
	public IFutureProg ChannelSpeakerProg => _channelSpeakerProg;
	public string ChannelColour => _channelColour;
	public ANSIColour ChannelAnsiColour => Telnet.GetColour(ChannelColour);
	public ulong? DiscordChannelId => _discordChannelId;

	public Channel(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		_channelName = name;
		_channelColour = Telnet.Red.Name;
		_announceChannelJoiners = false;
		_announceMissedListeners = false;
		_addToGuideCommandTree = false;
		_addToPlayerCommandTree = false;
		_channelListenerProg = Gameworld.AlwaysFalseProg;
		_channelSpeakerProg = Gameworld.AlwaysFalseProg;
		_mode = ChannelSpeakerNameMode.AccountName;
		_commandWords.Add(name.ToLowerInvariant().CollapseString());
		using (new FMDB())
		{
			var dbitem = new Models.Channel
			{
				ChannelName = _channelName,
				ChannelListenerProgId = _channelListenerProg.Id,
				ChannelSpeakerProgId = _channelSpeakerProg.Id,
				AnnounceChannelJoiners = _announceChannelJoiners,
				AnnounceMissedListeners = _announceMissedListeners,
				AddToGuideCommandTree = _addToGuideCommandTree,
				AddToPlayerCommandTree = _addToPlayerCommandTree,
				ChannelColour = Telnet.GetColour(_channelColour).Name,
				Mode = (int)_mode
			};
			FMDB.Context.Channels.Add(dbitem);
			dbitem.ChannelCommandWords.Add(new ChannelCommandWord
			{
				Channel = dbitem,
				Word = name.ToLowerInvariant().CollapseString()
			});
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Channel(MudSharp.Models.Channel channel, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = channel.Id;
		_name = channel.ChannelName;
		_channelName = channel.ChannelName;
		_channelListenerProg = gameworld.FutureProgs.Get(channel.ChannelListenerProgId);
		_channelSpeakerProg = gameworld.FutureProgs.Get(channel.ChannelSpeakerProgId);
		_announceChannelJoiners = channel.AnnounceChannelJoiners;
		_announceMissedListeners = channel.AnnounceMissedListeners;
		_mode = (ChannelSpeakerNameMode)channel.Mode;
		_channelColour = channel.ChannelColour;
		_commandWords.AddRange(channel.ChannelCommandWords.Select(x => x.Word));
		_addToGuideCommandTree = channel.AddToGuideCommandTree;
		_addToPlayerCommandTree = channel.AddToPlayerCommandTree;
		_discordChannelId = channel.DiscordChannelId;
	}

	public override string FrameworkItemType => "Channel";

	private string GetSpeakerName(ICharacter source, ICharacter viewer)
	{
		if (source == null)
		{
			return "System";
		}

		switch (_mode)
		{
			case ChannelSpeakerNameMode.AccountName:
				return source.Account.Name;
			case ChannelSpeakerNameMode.CharacterName:
				return source.PersonalName.GetName(NameStyle.GivenOnly);
			case ChannelSpeakerNameMode.CharacterFullName:
				return source.PersonalName.GetName(NameStyle.SimpleFull);
			case ChannelSpeakerNameMode.AnonymousToPlayers:
				return viewer is null || viewer.IsAdministrator() ? source.PersonalName.GetName(NameStyle.SimpleFull) : "Anonymous";
			default:
				throw new NotSupportedException();
		}
	}

	private const string CommandHelp = @"You can use the following options with the channel command:

	#3channel list#0 - lists all of the channels
	#3channel show <which>#0 - shows detailed information about a channel
	#3channel show#0 - an alias for showing your currently edited channel
	#3channel edit <which>#0 - begins editing a particular channel
	#3channel edit#0 - an alias for showing your currently edited channel
	#3channel close#0 - stops editing a channel
	#3channel new <name>#0 - creates a new channel
	#3channel set name <name>#0 - renames the channel
	#3channel set colour <colour>#0 - changes the colour of the channel
	#3channel set player#0 - toggles this being added to player command trees
	#3channel set guide#0 - toggles this being added to guide command trees
	#3channel set listenprog <prog>#0 - sets the prog for listeners
	#3channel set speakerprog <prog>#0 - sets the prog for speakers
	#3channel set joiners#0 - toggles channel join/leave being announced
	#3channel set missed#0 - toggles notifying when people miss your messages
	#3channel set commands <list of command words separated by spaces>#0 - sets the channel commands
	#3channel set mode <accountname|charactername|characterfullname|anonymoustoplayers>#0 - change the mode
	#3channel set discord <channelid>#0 - sets a discord channel to echo messages to
	#3channel set discord none#0 - clears a discord channel";

	public static void ChannelCommandDelegate(ICharacter character, string command)
	{
		var ss = new StringStack(command);
		var original = ss.Pop().ToLowerInvariant();

		if (original.Equals("channel", StringComparison.InvariantCultureIgnoreCase))
		{
			if (!character.IsAdministrator())
			{
				character.OutputHandler.Send("Huh?");
				return;
			}

			switch (ss.PopForSwitch())
			{
				case "list":
					ChannelCommandList(character, ss);
					return;
				case "show":
				case "view":
					ChannelCommandShow(character, ss);
					return;
				case "edit":
					ChannelCommandEdit(character, ss);
					return;
				case "close":
					ChannelCommandClose(character);
					return;
				case "new":
					ChannelCommandNew(character, ss);
					return;
				case "set":
					ChannelCommandSet(character, ss);
					return;
				default:
					character.OutputHandler.Send(CommandHelp.SubstituteANSIColour());
					return;
			}
		}

		var channel = character.Gameworld.Channels.FirstOrDefault(x =>
			x.CommandWords.Any(y => y.StartsWith(original, StringComparison.InvariantCultureIgnoreCase)) &&
			(x.ChannelListenerProg?.ExecuteBool(character) != false || x.ChannelSpeakerProg?.ExecuteBool(character) != false)
			);
		if (channel == null)
		{
			character.OutputHandler.Send("There is no such channel.");
			return;
		}

		if (ss.Peek().Equals("leave", StringComparison.InvariantCultureIgnoreCase) ||
		    ss.Peek().Equals("ignore", StringComparison.InvariantCultureIgnoreCase))
		{
			if (!channel.Ignore(character))
			{
				character.OutputHandler.Send("You are already ignoring that channel.");
			}

			return;
		}

		if (ss.Peek().Equals("join", StringComparison.InvariantCultureIgnoreCase) ||
		    ss.Peek().Equals("acknowledge", StringComparison.InvariantCultureIgnoreCase))
		{
			if (!channel.Acknowledge(character))
			{
				character.OutputHandler.Send("You are already listening to that channel.");
			}

			return;
		}

		if (ss.IsFinished)
		{
			character.OutputHandler.Send($"What do you want to send to the {channel.Name.ColourName()} channel?");
			return;
		}

		channel.Send(character, ss.RemainingArgument);
	}

	private static void ChannelCommandSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IChannel>>().FirstOrDefault();
        if (effect is null)
        {
			actor.OutputHandler.Send("You are not editing any channels.");
			return;
        }

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void ChannelCommandNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new channel?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Channels.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a channel with that name. Names must be unique.");
			return;
		}

		var channel = new Channel(actor.Gameworld, name);
		actor.Gameworld.Add(channel);
		actor.RemoveAllEffects<BuilderEditingEffect<IChannel>>();
		actor.AddEffect(new BuilderEditingEffect<IChannel>(actor) { EditingItem = channel });
		actor.OutputHandler.Send($"You create a new channel called {channel.Name.ColourName()}, which you are now editing.");
	}

	private static void ChannelCommandClose(ICharacter actor)
	{
		actor.RemoveAllEffects<BuilderEditingEffect<IChannel>>();
		actor.OutputHandler.Send("You are no longer editing any channels.");
	}

	private static void ChannelCommandEdit(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IChannel>>().FirstOrDefault();
		if (ss.IsFinished && effect is not null)
		{
			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which channel would you like to edit?");
			return;
		}

		var channel = actor.Gameworld.Channels.GetByIdOrName(ss.SafeRemainingArgument);
		if (channel is null)
		{
			actor.OutputHandler.Send("There is no such channel.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IChannel>>();
		actor.AddEffect(new BuilderEditingEffect<IChannel>(actor) { EditingItem = channel });
		actor.OutputHandler.Send($"You are now editing the {channel.Name.ColourName()} channel.");
	}

	private static void ChannelCommandShow(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IChannel>>().FirstOrDefault();
		if (ss.IsFinished && effect is not null)
		{
			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which channel would you like to view?");
			return;
		}

		var channel = actor.Gameworld.Channels.GetByIdOrName(ss.SafeRemainingArgument);
		if (channel is null)
		{
			actor.OutputHandler.Send("There is no such channel.");
			return;
		}

		actor.OutputHandler.Send(channel.Show(actor));
	}

	private static void ChannelCommandList(ICharacter actor, StringStack ss)
	{
		var channels = actor.Gameworld.Channels.ToList();
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from channel in channels
			select new List<string>
			{
				channel.Id.ToString("N0", actor),
				channel.Name,
				channel.Mode.DescribeEnum(),
				channel.ChannelColour,
				channel.ChannelListenerProg.MXPClickableFunctionName(),
				channel.ChannelSpeakerProg.MXPClickableFunctionName()
			},
			new List<string>
			{
				"Id",
				"Name",
				"Mode",
				"Colour",
				"Listener Prog",
				"Speaker Prog"
			},
			actor,
			Telnet.Red
		));
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Channels.Find(Id);
		dbitem.ChannelName = _channelName;
		dbitem.ChannelListenerProgId = _channelListenerProg.Id;
		dbitem.ChannelSpeakerProgId = _channelSpeakerProg.Id;
		dbitem.AnnounceChannelJoiners = _announceChannelJoiners;
		dbitem.AnnounceMissedListeners = _announceMissedListeners;
		dbitem.AddToGuideCommandTree = _addToGuideCommandTree;
		dbitem.AddToPlayerCommandTree = _addToPlayerCommandTree;
		dbitem.ChannelColour = Telnet.GetColour(_channelColour).Name;
		dbitem.Mode = (int)_mode;
		dbitem.DiscordChannelId = _discordChannelId;
		FMDB.Context.ChannelIgnorers.RemoveRange(dbitem.ChannelIgnorers);
		foreach (var item in _ignoringAccountIDs)
		{
			dbitem.ChannelIgnorers.Add(new ChannelIgnorer { AccountId = item, Channel = dbitem });
		}
		FMDB.Context.ChannelCommandWords.RemoveRange(dbitem.ChannelCommandWords);
		foreach (var command in CommandWords)
		{
			dbitem.ChannelCommandWords.Add(new ChannelCommandWord { Channel = dbitem, Word = command });
		}
		Changed = false;
	}

	#region IChannel Members

	private readonly List<string> _commandWords = new();
	public IEnumerable<string> CommandWords => _commandWords;

	public bool Send(IAccount account, string message)
	{
		Gameworld.LoadAllPlayerCharacters();
		foreach (var character in Gameworld.Characters.Concat(Gameworld.CachedActors).Distinct())
		{
			if (character.Account != account)
			{
				continue;
			}

			if (!_channelSpeakerProg.ExecuteBool(false, character))
			{
				continue;
			}

			message = message.Fullstop().ProperSentences();
			foreach (var tch in Gameworld.Characters.Where(x => (bool?)_channelListenerProg.Execute(x, character) ?? true))
			{
				if (_announceMissedListeners)
				{
					if (_ignoringAccountIDs.Contains(tch.Account.Id))
					{
						continue;
					}

					if (tch.OutputHandler.QuietMode)
					{
						continue;
					}
				}

				tch.OutputHandler.Send($"{$"[{_channelName}: {GetSpeakerName(character, tch)}]".Colour(ChannelAnsiColour)} {message}");
			}

			if (DiscordChannelId is not null)
			{
				Gameworld.DiscordConnection.NotifyInGameChannelUsed(_channelName, DiscordChannelId.Value, GetSpeakerName(character, character), message);
			}

			return true;
		}

		return false;
	}

	public void Send(ICharacter source, string message)
	{
		if (source != null)
		{
			if (!((bool?)_channelSpeakerProg.Execute(source) ?? true))
			{
				source.OutputHandler.Send("Huh?");
				return;
			}

			if (_ignoringAccountIDs.Contains(source.Account.Id))
			{
				source.OutputHandler.Send("You must first join the channel if you wish to speak on it.");
				return;
			}

			if (string.IsNullOrEmpty(message))
			{
				source.OutputHandler.Send("What message would you like to send?");
				return;
			}
		}

		message = message.Fullstop().ProperSentences();
		var sb = new StringBuilder();
		foreach (var character in Gameworld.Characters.Where(x => (bool?)_channelListenerProg.Execute(x, source) ?? true))
		{
			if (_announceMissedListeners)
			{
				if (_ignoringAccountIDs.Contains(character.Account.Id))
				{
					sb.AppendLine($"{GetSpeakerName(character, character).Proper()} is not listening to the channel.");
					continue;
				}

				if (character.OutputHandler.QuietMode && source != null)
				{
					sb.AppendLine($"{GetSpeakerName(character, character).Proper()} is editing.");
					continue;
				}
			}

			character.OutputHandler.Send($"{$"[{_channelName}: {GetSpeakerName(source, character)}]".Colour(ChannelAnsiColour)} {message}");
		}

		if (DiscordChannelId is not null)
		{
			Gameworld.DiscordConnection.NotifyInGameChannelUsed(_channelName, DiscordChannelId.Value, GetSpeakerName(source, source), message);
		}

		if (sb.Length > 0)
		{
			source?.OutputHandler.Send(sb.ToString());
		}
	}

	public bool Ignore(ICharacter character)
	{
		if (_ignoringAccountIDs.Contains(character.Account.Id))
		{
			return false;
		}

		_ignoringAccountIDs.Add(character.Account.Id);

		if (_announceChannelJoiners)
		{
			var output =
				$"{"[System Message]".Colour(Telnet.Green)} {GetSpeakerName(character, null)} has left the {_channelName.ToLowerInvariant()} channel.";
			Gameworld.SystemMessage(output,
				x =>
					!_ignoringAccountIDs.Contains(x.Account.Id) &&
					((bool?)_channelListenerProg.Execute(x, null) ?? true));
		}

		Changed = true;
		return true;
	}

	public bool Acknowledge(ICharacter character)
	{
		if (!_ignoringAccountIDs.Contains(character.Account.Id))
		{
			return false;
		}

		_ignoringAccountIDs.Remove(character.Account.Id);

		if (_announceChannelJoiners)
		{
			var output =
				$"{"[System Message]".Colour(Telnet.Green)} {GetSpeakerName(character, null)} has joined the {_channelName.ToLowerInvariant()} channel.";
			Gameworld.SystemMessage(output,
				x =>
					!_ignoringAccountIDs.Contains(x.Account.Id) &&
					((bool?)_channelListenerProg.Execute(x, null) ?? true));
		}

		Changed = true;
		return true;
	}

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames the channel
	#3colour <colour>#0 - changes the colour of the channel
	#3player#0 - toggles this being added to player command trees
	#3guide#0 - toggles this being added to guide command trees
	#3listenprog <prog>#0 - sets the prog for listeners
	#3speakerprog <prog>#0 - sets the prog for speakers
	#3joiners#0 - toggles channel join/leave being announced
	#3missed#0 - toggles notifying when people miss your messages
	#3commands <list of command words separated by spaces>#0 - sets the channel commands
	#3mode <accountname|charactername|characterfullname|anonymoustoplayers>#0 - change the mode";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
			case "player":
				return BuildingCommandPlayer(actor);
			case "guide":
				return BuildingCommandGuide(actor);
			case "listenprog":
			case "listenerprog":
				return BuildingCommandListenerProg(actor, command);
			case "speakerprog":
			case "speakprog":
				return BuildingCommandSpeakerProg(actor, command);
			case "joiners":
				return BuildingCommandJoiners(actor);
			case "missed":
				return BuildingCommandMissed(actor);
			case "mode":
				return BuildingCommandMode(actor, command);
			case "commands":
				return BuildingCommandCommands(actor, command);
			case "discord":
				return BuildingCommandDiscord(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandDiscord(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.SafeRemainingArgument.EqualToAny("none", "remove", "delete"))
		{
			_discordChannelId = null;
			Changed = true;
			actor.OutputHandler.Send("This channel will no longer broadcast its conversations to discord.");
			return true;
		}

		if (!ulong.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid discord channel ID.");
			return false;
		}

		_discordChannelId = value;
		Changed = true;
		actor.OutputHandler.Send($"This channel will now broadcast its conversations on the {value.ToString("F0", actor).ColourValue()} discord channel.");
		return true;
	}

	private bool BuildingCommandCommands(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which command keywords do you want to use for this channel?");
			return false;
		}

		_commandWords.Clear();
		while (!command.IsFinished)
		{
			_commandWords.Add(command.PopSpeech().ToLowerInvariant());
		}
		Changed = true;
		actor.OutputHandler.Send($"The command words for this channel are now: {CommandWords.Select(x => x.ColourValue()).ListToString()}\nNote: You must restart the MUD before these command words will work.");
		return true;
	}

	private bool BuildingCommandMode(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which mode should the display for this channel be? The options are {Enum.GetValues<ChannelSpeakerNameMode>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<ChannelSpeakerNameMode>(out var value))
		{
			actor.OutputHandler.Send($"That is not a valid display mode. The options are {Enum.GetValues<ChannelSpeakerNameMode>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		_mode = value;
		Changed = true;
		actor.OutputHandler.Send($@"The display mode for this channel is now {value.DescribeEnum().ColourName()}.
For you, this would look like the following:

{$"[{_channelName}: {GetSpeakerName(actor, actor)}]".Colour(_channelColour, Telnet.Black.ToString())} Test message.");
		return true;
	}

	private bool BuildingCommandMissed(ICharacter actor)
	{
		_announceMissedListeners = !_announceMissedListeners;
		Changed = true;
		actor.OutputHandler.Send($"This channel will {_announceMissedListeners.NowNoLonger()} announce that people were ignoring the channel or editing after each message.");
		return true;
	}

	private bool BuildingCommandJoiners(ICharacter actor)
	{
		_announceChannelJoiners = !_announceChannelJoiners;
		Changed = true;
		actor.OutputHandler.Send($"This channel will {_announceChannelJoiners.NowNoLonger()} announce when people join or leave the channel.");
		return true;
	}

	private bool BuildingCommandSpeakerProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use who can speak on this channel?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_channelSpeakerProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This channel will now use the {prog.MXPClickableFunctionName()} prog to control who can speak on this channel.");
		return true;
	}

	private bool BuildingCommandListenerProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use who can listen to this channel?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, 
			new[]{
				new[] {
					ProgVariableTypes.Character
				},
				new[] {
					ProgVariableTypes.Character,
					ProgVariableTypes.Character
				}
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_channelListenerProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This channel will now use the {prog.MXPClickableFunctionName()} prog to control who can listen to this channel.");
		return true;
	}

	private bool BuildingCommandGuide(ICharacter actor)
	{
		_addToGuideCommandTree = !_addToGuideCommandTree;
		Changed = true;
		actor.OutputHandler.Send($"This channel will {_addToGuideCommandTree.NowNoLonger()} be added to guide command trees.\nNote: You must restart the MUD before this will apply.");
		return true;
	}

	private bool BuildingCommandPlayer(ICharacter actor)
	{
		_addToPlayerCommandTree = !_addToPlayerCommandTree;
		Changed = true;
		actor.OutputHandler.Send($"This channel will {_addToGuideCommandTree.NowNoLonger()} be added to guide command trees.\nNote: You must restart the MUD before this will apply.");
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What colour should this channel be? The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour == null)
		{
			actor.OutputHandler.Send(
				$"That is not a valid colour. The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		_channelColour = colour.Name;
		Changed = true;
		actor.OutputHandler.Send($@"This channel is now coloured {colour.Name.Colour(colour)}.

For you, this would look like the following:

{$"[{_channelName}: {GetSpeakerName(actor, actor)}]".Colour(_channelColour, Telnet.Black.ToString())} Test message.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished) {
			actor.OutputHandler.Send("What name do you want to give to this channel?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
        if (Gameworld.Channels.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a channel with that name. Names must be unique.");
			return false;
		}

		_name = name;
		_channelName = name;
		Changed = true;
		actor.OutputHandler.Send($"This channel is now named {name.Colour(Telnet.GetColour(_channelColour))}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Channel #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Orange, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Speaker Prog: {_channelSpeakerProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Listener Prog: {_channelListenerProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Name Mode: {_mode.DescribeEnum(true, Telnet.Cyan)}");
		sb.AppendLine($"Announce Joiners: {_announceChannelJoiners.ToColouredString()}");
		sb.AppendLine($"Announce Missed Speakers: {_announceMissedListeners.ToColouredString()}");
		sb.AppendLine($"Channel Colour: {Telnet.GetColour(_channelColour).Name.Colour(Telnet.GetColour(_channelColour))}");
		sb.AppendLine($"Command Words: {CommandWords.Select(x => x.ColourValue()).ListToCommaSeparatedValues(", ")}");
		sb.AppendLine($"Add To Player Commands: {_addToPlayerCommandTree.ToColouredString()}");
		sb.AppendLine($"Add To Guide Commands: {_addToGuideCommandTree.ToColouredString()}");
		sb.AppendLine($"Discord Channel: {DiscordChannelId?.ToString("F0", actor).ColourValue() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Online Listeners:");
		sb.AppendLine();
		foreach (var ch in Gameworld.Characters)
		{
			sb.AppendLine($"\t{ch.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)} - {ch.PersonalName.GetName(NameStyle.SimpleFull).ColourName()} - {ch.Account.Name.ColourValue()}");
		}
		return sb.ToString();
	}

	#endregion
}