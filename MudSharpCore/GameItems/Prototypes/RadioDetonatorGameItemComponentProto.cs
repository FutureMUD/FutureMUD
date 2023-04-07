using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class RadioDetonatorGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "RadioDetonator";

	#region Constructors

	protected RadioDetonatorGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "RadioDetonator")
	{
		PowerConsumptionInWatts = 0.1;
		OnPowerOnEmote = "The red LED indicator on $0 turns on, indicating it is now armed.";
		OnPowerOffEmote = "The red LED indicator on $0 turns off, indicating it is no longer armed.";
	}

	protected RadioDetonatorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		PowerConsumptionInWatts = double.Parse(root.Element("PowerConsumptionInWatts").Value);
		OnPowerOnEmote = root.Element("OnPowerOnEmote").Value;
		OnPowerOffEmote = root.Element("OnPowerOffEmote").Value;
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("PowerConsumptionInWatts", PowerConsumptionInWatts),
			new XElement("OnPowerOnEmote", new XCData(OnPowerOnEmote)),
			new XElement("OnPowerOffEmote", new XCData(OnPowerOffEmote))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new RadioDetonatorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new RadioDetonatorGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("RadioDetonator".ToLowerInvariant(), true,
			(gameworld, account) => new RadioDetonatorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("RadioDetonator",
			(proto, gameworld) => new RadioDetonatorGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"RadioDetonator",
			$"This item when {"[powered]".Colour(Telnet.Magenta)} listens for a radio frequency and acts as a {"[trigger]".Colour(Telnet.Yellow)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new RadioDetonatorGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tpower <watts> - the power consumption when switched on\n\ton <emote> - the switched on emote. $0 is the item.\n\toff <emote> - the switched off emote. $0 is the item";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "watts":
			case "watt":
			case "wattage":
			case "power":
				return BuildingCommandPower(actor, command);
			case "onemote":
			case "on":
			case "on emote":
			case "on_emote":
				return BuildingCommandOnEmote(actor, command);
			case "offemote":
			case "off":
			case "off emote":
			case "off_emote":
				return BuildingCommandOffEmote(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandOnEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for the radio detonator when it is switched on?");
			return false;
		}

		OnPowerOnEmote = command.RemainingArgument;
		Changed = true;
		actor.Send($"The emote when this item is switched on is now: {OnPowerOnEmote}");
		return true;
	}

	private bool BuildingCommandOffEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for the radio detonator when it is switched off?");
			return false;
		}

		OnPowerOffEmote = command.RemainingArgument;
		Changed = true;
		actor.Send($"The emote when this item is switched off is now: {OnPowerOffEmote}");
		return true;
	}


	private bool BuildingCommandPower(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What wattage should this radio detonator use while armed?");
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

		PowerConsumptionInWatts = value;
		Changed = true;
		actor.Send($"This radio detonator now uses {PowerConsumptionInWatts:N5} watts of power when armed.");
		return false;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a detonator for explosives that  is triggered by a radio data sequence. When switched on, it draws down {4} watts. The SwitchOn emote is {5} and the SwitchOff emote is {6}.",
			"RadioDetonator Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			PowerConsumptionInWatts.ToString("N3", actor).ColourValue(),
			OnPowerOnEmote.ColourCommand(),
			OnPowerOffEmote.ColourCommand()
		);
	}

	public double PowerConsumptionInWatts { get; protected set; }

	public string OnPowerOnEmote { get; protected set; }

	public string OnPowerOffEmote { get; protected set; }
}