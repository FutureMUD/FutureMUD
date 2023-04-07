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

public class ImplantRadioGameItemComponentProto : ImplantBaseGameItemComponentProto
{
	public override string TypeDescription => "ImplantRadio";

	public double WattageIdle { get; set; }
	public double WattageTransmit { get; set; }
	public double WattageReceive { get; set; }
	public double BroadcastRange { get; set; }
	public string OnPowerOnEmote { get; set; }
	public string OnPowerOffEmote { get; set; }
	public List<double> Channels { get; } = new();
	public List<string> ChannelNames { get; } = new();

	#region Constructors

	protected ImplantRadioGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ImplantRadio")
	{
		WattageIdle = 2.0;
		WattageTransmit = 100.0;
		WattageReceive = 25.0;
		BroadcastRange = 7000 / gameworld.UnitManager.BaseHeightToMetres;
		OnPowerOnEmote = "@ gives a brief beep to indicate that it has been powered on.";
		OnPowerOffEmote = "@ gives two brief beeps to indicate that it has been powered off.";
		var channelNumber = 1;
		for (var i = 476.425; i < 477.400; i = i + 0.025)
		{
			Channels.Add(i);
			ChannelNames.Add($"CB {channelNumber++}");
		}
	}

	protected ImplantRadioGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		WattageIdle = double.Parse(root.Element("WattageIdle").Value);
		WattageTransmit = double.Parse(root.Element("WattageTransmit").Value);
		WattageReceive = double.Parse(root.Element("WattageReceive").Value);
		BroadcastRange = double.Parse(root.Element("BroadcastRange").Value);
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
		var def = SaveToXmlWithoutConvertingToString();
		def.Add(
			new XElement("WattageIdle", WattageIdle),
			new XElement("WattageTransmit", WattageTransmit),
			new XElement("WattageReceive", WattageReceive),
			new XElement("BroadcastRange", BroadcastRange),
			new XElement("OnPowerOffEmote", new XCData(OnPowerOffEmote)),
			new XElement("OnPowerOnEmote", new XCData(OnPowerOnEmote)),
			from channel in Channels select new XElement("Channel", channel),
			from channel in ChannelNames select new XElement("ChannelName", new XCData(channel))
		);
		return def.ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ImplantRadioGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ImplantRadioGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ImplantRadio".ToLowerInvariant(), true,
			(gameworld, account) => new ImplantRadioGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImplantRadio",
			(proto, gameworld) => new ImplantRadioGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ImplantRadio",
			$"An {"[implant]".Colour(Telnet.Pink)} that is also a two-way radio when {"[neurally interfaced]".Colour(Telnet.Pink)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImplantRadioGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbody <body> - sets the body prototype this implant is used with\n\tbodypart <bodypart> - sets the bodypart prototype this implant is used with\n\texternal - toggles whether this implant is external\n\texternaldesc <desc> - an alternate sdesc used when installed and external\n\tpower <watts> - how many watts of power to use\n\tdiscount <watts> - how many watts of power usage to discount per point of quality\n\tgrace <percentage> - the grace percentage of hp damage before implant function reduces\n\tspace <#> - the amount of 'space' in a bodypart that the implant takes up\n\tdifficulty <difficulty> - how difficulty it is for surgeons to install this implant\n\tidle <watts> - wattage when idle\n\treceive <watts> - wattage for 1 second when receiving\n\ttransmit <watts> - wattage for 1 second when transmitting\n\ton <emote> - an emote when this earpiece is switched on. $0 is the radio item.\n\toff <emote> - an emote when this earpiece is switched off. $0 is the radio item.\n\tchannel <freq MHz> <name> - adds a new channel with the specified name\n\tchannel <freq MHz> - removes an existing channel with the specified frequency\n\trange <distance> - the maximum transition range of this radio";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "idle":
				return BuildingCommandIdle(actor, command);
			case "receive":
				return BuildingCommandReceive(actor, command);
			case "transmit":
				return BuildingCommandTransmit(actor, command);
			case "range":
				return BuildingCommandRange(actor, command);
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
				return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
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

		var name = command.SafeRemainingArgument;
		if (ChannelNames.Any(x => x.EqualTo(name)))
		{
			actor.Send("There is already a channel with that name. Channel names must be unique.");
			return false;
		}

		Channels.Add(value);
		ChannelNames.Add(name);

		actor.Send($"This radio now uses channel {name.Colour(Telnet.Green)} ({value:N3}MHz)");
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
		actor.Send($"This handheld radio now uses {WattageIdle:N5} watts of power while idle.");
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
		actor.Send($"This handheld radio now uses {WattageReceive:N5} watts of power while receiving.");
		return false;
	}

	private bool BuildingCommandTransmit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What wattage should this handheld radio use while transmitting?");
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

		WattageTransmit = value;
		Changed = true;
		actor.Send($"This handheld radio now uses {WattageTransmit:N5} watts of power while transmitting.");
		return false;
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What range should the transmissions from this handheld radio have?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Length, out var success);
		if (!success)
		{
			actor.Send("That is not a valid distance.");
			return false;
		}

		BroadcastRange = value;
		Changed = true;
		actor.Send(
			$"This handheld radio will now broadcast at a range of {Gameworld.UnitManager.Describe(BroadcastRange, UnitType.Length, actor).ColourValue()}.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return ComponentDescriptionOLC(actor, "This is an implant radio",
			$"It consumes {WattageIdle.ToString("N5").ColourValue()} watts of power while idle, {WattageTransmit.ToString("N5").ColourValue()} while transmitting and {WattageTransmit.ToString("N5").ColourValue()} while receiving. It can broadcast at a range of {Gameworld.UnitManager.DescribeMostSignificant(BroadcastRange, UnitType.Length, actor).Colour(Telnet.Green)}.\n\nEmote when switched on: {OnPowerOnEmote.ColourCommand()}\nEmote when switched off: {OnPowerOffEmote.ColourCommand()}\nChannels: {Channels.Select(x => $"{ChannelNames[Channels.IndexOf(x)]} ({x.ToString("N3", actor)}MHz)").ListToString()}");
	}
}