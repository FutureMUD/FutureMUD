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
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class RadioDetonatorTransmitterGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "RadioDetonatorTransmitter";

	#region Constructors

	protected RadioDetonatorTransmitterGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(
		gameworld, originator, "RadioDetonatorTransmitter")
	{
		PowerConsumptionOnIdle = 0.5;
		PowerConsumptionOnBroadcast = 1000;
		PowerOnEmote = "@ turn|turns on.";
		PowerOffEmote = "@ turn|turns off.";
		DetonateCommandEmote = "@ push|pushes the detonation button on $1";
		DetonationRange = 20;
	}

	protected RadioDetonatorTransmitterGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		PowerConsumptionOnBroadcast = double.Parse(root.Element("PowerConsumptionOnBroadcast").Value);
		PowerConsumptionOnIdle = double.Parse(root.Element("PowerConsumptionOnIdle").Value);
		PowerOnEmote = root.Element("PowerOnEmote").Value;
		PowerOffEmote = root.Element("PowerOffEmote").Value;
		DetonateCommandEmote = root.Element("DetonateCommandEmote").Value;
		DetonationRange = int.Parse(root.Element("DetonationRange").Value);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("PowerConsumptionOnBroadcast", PowerConsumptionOnBroadcast),
			new XElement("PowerConsumptionOnIdle", PowerConsumptionOnIdle),
			new XElement("PowerOnEmote", new XCData(PowerOnEmote)),
			new XElement("PowerOffEmote", new XCData(PowerOffEmote)),
			new XElement("DetonateCommandEmote", new XCData(DetonateCommandEmote)),
			new XElement("DetonationRange", DetonationRange)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new RadioDetonatorTransmitterGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new RadioDetonatorTransmitterGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("RadioDetonatorTransmitter".ToLowerInvariant(), true,
			(gameworld, account) => new RadioDetonatorTransmitterGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("RadioDetonatorTransmitter",
			(proto, gameworld) => new RadioDetonatorTransmitterGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"RadioDetonatorTransmitter",
			$"A {"[powered]".Colour(Telnet.Magenta)} device that acts as a remote {"[trigger]".Colour(Telnet.Yellow)} for radio detonated bombs",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new RadioDetonatorTransmitterGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tidle <watts> - how much power this uses when switched on\n\tbroadcast <watts> - how many watts it uses when it broadcasts detonate signal\n\ton <emote> - the emote when it powers on. $0 is the item\n\toff <emote> - the emote when it powers off. $0 is the item\n\trange <rooms> - the range in rooms that it will send a signal for\n\tdetonate <emote> - the emote when someone selects detonation. $0 is the person and $1 the detonator item.";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			default:
				return base.BuildingCommand(actor, command);
			case "power":
			case "watts":
			case "idle":
				return BuildingCommandIdle(actor, command);
			case "broadcast":
				return BuildingCommandBroadcast(actor, command);
			case "poweron":
			case "poweronemote":
			case "on":
				return BuildingCommandPowerOnEmote(actor, command);
			case "poweroff":
			case "poweroffemote":
			case "off":
				return BuildingCommandPowerOffEmote(actor, command);
			case "range":
				return BuildingCommandRange(actor, command);
			case "detonateemote":
			case "detonationemote":
			case "detonate":
			case "detonation":
				return BuildingCommandDetonationEmote(actor, command);
		}
	}

	private bool BuildingCommandDetonationEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What emote do you want to set when someone selects the detonate option? $0 is the person doing the selecting and $1 is the detonator item.");
			return false;
		}

		var emoteText = command.RemainingArgument.ProperSentences().Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		DetonateCommandEmote = emoteText;
		Changed = true;
		actor.OutputHandler.Send(
			$"The emote when someone selects the detonate option is now {emoteText.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many rooms should the range of this detonator's radio signal be?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 0 || value > 50)
		{
			actor.OutputHandler.Send("You must enter a number between 0 (same room only) and 50.");
			return false;
		}

		DetonationRange = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This detonator now has a range of {DetonationRange.ToString("N0", actor).ColourValue()} room{(DetonationRange == 1 ? "" : "s")}.");
		return true;
	}

	private bool BuildingCommandPowerOffEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What emote do you want to set when the detonator powers off? $0 is the detonator item.");
			return false;
		}

		var emoteText = command.RemainingArgument.ProperSentences().Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		PowerOffEmote = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The emote when this detonator powers off is now {emoteText.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandPowerOnEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What emote do you want to set when the detonator powers on? $0 is the detonator item.");
			return false;
		}

		var emoteText = command.RemainingArgument.ProperSentences().Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		PowerOnEmote = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The emote when this detonator powers on is now {emoteText.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandBroadcast(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How many watts should this detonator use when it broadcasts the detonate command?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0)
		{
			actor.OutputHandler.Send("You must enter a valid number of watts greater than or equal to zero.");
			return false;
		}

		PowerConsumptionOnBroadcast = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This detonator will now use {PowerConsumptionOnBroadcast.ToString("N3", actor).ColourValue()} watts when broadcasting.");
		return true;
	}

	private bool BuildingCommandIdle(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many watts should this detonator use when it is idle?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0)
		{
			actor.OutputHandler.Send("You must enter a valid number of watts greater than or equal to zero.");
			return false;
		}

		PowerConsumptionOnIdle = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This detonator will now use {PowerConsumptionOnBroadcast.ToString("N3", actor).ColourValue()} watts when idle.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a transmitter for a radio detonator. It uses {4} when idle and {5} while transmitting. It has a range of {9} rooms. The detonation emote is {8}. The PowerOn emote is {6}. The PowerOff emote is {7}.",
			"RadioDetonatorTransmitter Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			PowerConsumptionOnIdle.ToString("N3", actor).FluentAppend(" watts", true).ColourValue(),
			PowerConsumptionOnBroadcast.ToString("N3", actor).FluentAppend(" watts", true).ColourValue(),
			PowerOnEmote.ColourCommand(),
			PowerOffEmote.ColourCommand(),
			DetonateCommandEmote.ColourCommand(),
			DetonationRange.ToString("N0", actor).ColourValue()
		);
	}

	public double PowerConsumptionOnIdle { get; protected set; }
	public double PowerConsumptionOnBroadcast { get; protected set; }
	public string PowerOnEmote { get; protected set; }
	public string PowerOffEmote { get; protected set; }
	public string DetonateCommandEmote { get; protected set; }
	public int DetonationRange { get; protected set; }
}