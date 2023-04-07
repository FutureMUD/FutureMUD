using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class EarpieceRadioGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "EarpieceRadio";

	public double WattageIdle { get; set; }
	public double WattageReceive { get; set; }
	public string OnPowerOnEmote { get; set; }
	public string OnPowerOffEmote { get; set; }
	public List<double> Channels { get; } = new();
	public List<string> ChannelNames { get; } = new();

	#region Constructors

	protected EarpieceRadioGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "EarpieceRadio")
	{
		WattageIdle = 0.5;
		WattageReceive = 5.0;
		OnPowerOnEmote = "@ crackle|crackles to life with a brief burst of static.";
		OnPowerOffEmote = "@ give|gives a final, brief burst of static and then power|powers off.";
		var channelNumber = 1;
		for (var i = 476.425; i < 477.400; i = i + 0.025)
		{
			Channels.Add(i);
			ChannelNames.Add($"CB {channelNumber++}");
		}
	}

	protected EarpieceRadioGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		WattageIdle = double.Parse(root.Element("WattageIdle").Value);
		WattageReceive = double.Parse(root.Element("WattageReceive").Value);
		OnPowerOffEmote = root.Element("OnPowerOffEmote").Value;
		OnPowerOnEmote = root.Element("OnPowerOnEmote").Value;
		foreach (var item in root.Elements("Channel"))
		{
			Channels.Add(double.Parse(item.Value));
		}

		foreach (var item in root.Elements("ChannelName"))
		{
			ChannelNames.Add(item.Value);
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WattageIdle", WattageIdle),
			new XElement("WattageReceive", WattageReceive),
			new XElement("OnPowerOffEmote", new XCData(OnPowerOffEmote)),
			new XElement("OnPowerOnEmote", new XCData(OnPowerOnEmote)),
			from channel in Channels select new XElement("Channel", channel),
			from channel in ChannelNames select new XElement("ChannelName", new XCData(channel))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new EarpieceRadioGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new EarpieceRadioGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("EarpieceRadio".ToLowerInvariant(), true,
			(gameworld, account) => new EarpieceRadioGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("EarpieceRadio",
			(proto, gameworld) => new EarpieceRadioGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"EarpieceRadio",
			$"An item that when {"[powered]".Colour(Telnet.Magenta)} and combined with an ear {"[wearable]".Colour(Telnet.BoldYellow)} makes a receive-only radio",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new EarpieceRadioGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tidle <watts> - wattage when idle\n\treceive <watts> - wattage for 1 second when receiving\n\ton <emote> - an emote when this earpiece is switched on. $0 is the radio item.\n\toff <emote> - an emote when this earpiece is switched off. $0 is the radio item.\n\tchannel <freq MHz> <name> - adds a new channel with the specified name\n\tchannel <freq MHz> - removes an existing channel with the specified frequency";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "idle":
				return BuildingCommandIdle(actor, command);
			case "receive":
				return BuildingCommandReceive(actor, command);
			case "on":
			case "onemote":
			case "emoteon":
				return BuildingCommandOn(actor, command);
			case "off":
			case "offemote":
			case "emoteoff":
				return BuildingCommandOff(actor, command);
			case "channel":
				return BuildingCommandChannel(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandChannel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What MHz range do you want to toggle as a channel for this radio?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid frequency.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.Send("Frequencies must be positive numbers greater than zero.");
			return false;
		}

		if (Channels.Contains(value))
		{
			Channels.Remove(value);
			actor.Send($"This radio no longer uses channel {value:N3}");
			Changed = true;
			return true;
		}

		if (command.IsFinished)
		{
			actor.Send("You must give a name for this channel, e.g. CB 40.");
			return false;
		}

		if (ChannelNames.Any(x => x.EqualTo(command.SafeRemainingArgument)))
		{
			actor.Send("There is already a channel with that name. Channel names must be unique.");
			return false;
		}

		Channels.Add(value);
		ChannelNames.Add(command.SafeRemainingArgument);

		actor.Send(
			$"This radio now uses channel {command.SafeRemainingArgument.Colour(Telnet.Green)} ({value.ToString("N3", actor)}MHz)");
		Changed = true;
		return true;
	}

	private bool BuildingCommandOn(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for the radio when it is switched on?");
			return false;
		}

		OnPowerOnEmote = command.RemainingArgument;
		Changed = true;
		actor.Send($"The emote when this item is switched on is now: {OnPowerOnEmote}");
		return true;
	}

	private bool BuildingCommandOff(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for the radio when it is switched off?");
			return false;
		}

		OnPowerOffEmote = command.RemainingArgument;
		Changed = true;
		actor.Send($"The emote when this item is switched off is now: {OnPowerOffEmote}");
		return true;
	}

	private bool BuildingCommandIdle(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What wattage should this handheld radio use while idle?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must specify a valid number of watts.");
			return false;
		}

		if (value < 0.0)
		{
			actor.Send("The value must be a positive number.");
			return false;
		}

		WattageIdle = value;
		Changed = true;
		actor.Send(
			$"This handheld radio now uses {WattageIdle.ToString("N5", actor).ColourValue()} watts of power while idle.");
		return false;
	}

	private bool BuildingCommandReceive(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What wattage should this handheld radio use while receiving?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must specify a valid number of watts.");
			return false;
		}

		if (value < 0.0)
		{
			actor.Send("The value must be a positive number.");
			return false;
		}

		WattageReceive = value;
		Changed = true;
		actor.Send(
			$"This handheld radio now uses {WattageReceive.ToString("N5", actor).ColourValue()} watts of power while receiving.");
		return false;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is an earpiece radio. It consumes {4:N5} watts of power while idle, {5:N5} while receiving.\n\nEmote when switched on: {6}\nEmote when switched off: {7}\nChannels: {8}",
			"EarpieceRadio Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			WattageIdle,
			WattageReceive,
			OnPowerOnEmote.Colour(Telnet.Yellow),
			OnPowerOffEmote.Colour(Telnet.Yellow),
			Channels.Select(x => $"{ChannelNames[Channels.IndexOf(x)]} ({x.ToString("N3", actor)}MHz)").ListToString()
		);
	}
}