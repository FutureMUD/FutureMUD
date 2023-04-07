using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;

namespace MudSharp.Communication;

public enum ChannelSpeakerNameMode
{
	AccountName = 0,
	CharacterName
}

public class Channel : SaveableItem, IChannel
{
	protected readonly List<long> _ignoringAccountIDs = new();
	protected bool _announceChannelJoiners;
	protected bool _announceMissedListeners;
	protected string _channelColour;
	protected IFutureProg _channelListenerProg;

	protected string _channelName;
	protected IFutureProg _channelSpeakerProg;
	protected ChannelSpeakerNameMode _mode;

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
		_channelColour = channel.ChannelColour.SubstituteANSIColour().Trim();
		CommandWords = channel.ChannelCommandWords.Select(x => x.Word).ToList();
	}

	public override string FrameworkItemType => "Channel";

	private string GetSpeakerName(ICharacter source)
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
			default:
				throw new NotSupportedException();
		}
	}

	public static void ChannelCommandDelegate(ICharacter character, string command)
	{
		var ss = new StringStack(command);
		var original = ss.Pop().ToLowerInvariant();

		if (original.Equals("channel", StringComparison.InvariantCultureIgnoreCase))
		{
			character.OutputHandler.Send("Channel command still to do.");
			return;
		}
		else
		{
			var channel = character.Gameworld.Channels.FirstOrDefault(x =>
				x.CommandWords.Any(y => y.StartsWith(original, StringComparison.Ordinal)));
			if (channel == null)
			{
				character.OutputHandler.Send("No such channel.");
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
				character.Send("What do you want to send to the {0} channel?", channel.Name);
				return;
			}

			channel.Send(character, ss.RemainingArgument);
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Channels.Find(Id);
			FMDB.Context.ChannelIgnorers.RemoveRange(dbitem.ChannelIgnorers);
			foreach (var item in _ignoringAccountIDs)
			{
				dbitem.ChannelIgnorers.Add(new ChannelIgnorer { AccountId = item, Channel = dbitem });
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#region IChannel Members

	public IEnumerable<string> CommandWords { get; protected set; }

	public void Send(ICharacter source, string message)
	{
		string output;
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

			output =
				$"{$"[{_channelName}: {GetSpeakerName(source)}]".Colour(_channelColour, Telnet.Black.ToString())} {message.Fullstop().ProperSentences()}";
		}
		else
		{
			output =
				$"{$"[{_channelName}: System]".Colour(_channelColour, Telnet.Black.ToString())} {message.Fullstop().ProperSentences()}";
		}

		var sb = new StringBuilder();
		foreach (var character in Gameworld.Characters.Where(x => (bool?)_channelListenerProg.Execute(x) ?? true))
		{
			if (_announceMissedListeners)
			{
				if (_ignoringAccountIDs.Contains(character.Account.Id))
				{
					sb.AppendLine($"{GetSpeakerName(character).Proper()} is not listening to the channel.");
					continue;
				}

				if (character.OutputHandler.QuietMode && source != null)
				{
					sb.AppendLine($"{GetSpeakerName(character).Proper()} is editing.");
					continue;
				}
			}

			character.OutputHandler.Send(output);
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
				$"{"[System Message]".Colour(Telnet.Green)} {GetSpeakerName(character)} has left the {_channelName.ToLowerInvariant()} channel.";
			Gameworld.SystemMessage(output,
				x =>
					!_ignoringAccountIDs.Contains(x.Account.Id) &&
					((bool?)_channelListenerProg.Execute(x) ?? true));
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
				$"{"[System Message]".Colour(Telnet.Green)} {GetSpeakerName(character)} has joined the {_channelName.ToLowerInvariant()} channel.";
			Gameworld.SystemMessage(output,
				x =>
					!_ignoringAccountIDs.Contains(x.Account.Id) &&
					((bool?)_channelListenerProg.Execute(x) ?? true));
		}

		Changed = true;
		return true;
	}

	#endregion
}