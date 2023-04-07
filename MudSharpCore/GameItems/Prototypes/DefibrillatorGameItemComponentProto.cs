using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class DefibrillatorGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Defibrillator";

	public double WattagePerShock { get; set; }
	public string DefibrillationEmote { get; set; }
	public bool CanShockHealthyHearts { get; set; }

	#region Constructors

	protected DefibrillatorGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "Defibrillator")
	{
		WattagePerShock = 20000;
		DefibrillationEmote =
			"$0 rub|rubs $2 together as they make a sharp whine, and then put|puts them to $1's chest and deliver|delivers a shock";
	}

	protected DefibrillatorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		WattagePerShock = double.Parse(root.Element("WattagePerShock").Value);
		DefibrillationEmote = root.Element("DefibrillationEmote").Value;
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WattagePerShock", WattagePerShock),
			new XElement("DefibrillationEmote", new XCData(DefibrillationEmote))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new DefibrillatorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new DefibrillatorGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Defibrillator".ToLowerInvariant(), true,
			(gameworld, account) => new DefibrillatorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Defibrillator",
			(proto, gameworld) => new DefibrillatorGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Defibrillator",
			$"Makes an item a defibrillator that when {"[powered]".Colour(Telnet.Magenta)} can defibrillate arrythmic hearts",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new DefibrillatorGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\twatts <watts> - the wattage consumed per shock\n\temote <emote> - the emote when using. Use $0 for the doctor, $1 for the patient and $2 for the item\n\tsafe - toggles whether there is a safety switch that prevents shocking healthy hearts";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "wattage":
			case "watts":
			case "watt":
			case "power":
				return BuildingCommandWattage(actor, command);
			case "emote":
				return BuildingCommandEmote(actor, command);
			case "safe":
				return BuildingCommandSafe(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandSafe(ICharacter actor, StringStack command)
	{
		CanShockHealthyHearts = !CanShockHealthyHearts;
		Changed = true;
		actor.Send($"This defibrillator can {(CanShockHealthyHearts ? "now" : "no longer")} shock healthy hearts.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set when this item is used? Use $0 for the doctor, $1 for the patient and $2 for the item.");
			return false;
		}

		DefibrillationEmote = command.RemainingArgument;
		actor.Send(
			$"This defibrillator will now use the following emote echo: {DefibrillationEmote.Colour(Telnet.Yellow)}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandWattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"How many watts of power do you want this item to consume each time it is used to shock a patient?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.Send("You must enter a valid, positive number of watts of power for this item to use.");
			return false;
		}

		WattagePerShock = value;
		Changed = true;
		actor.Send($"This item will now use {WattagePerShock:N3} watts of power per shock.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a defibrillator, used to restart hearts. It {6} shock healthy hearts. It consumes {4:N3} watts of power per shock.\nWhen employed, it uses the following emote: {5}",
			"Defibrillator Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			WattagePerShock,
			DefibrillationEmote.Colour(Telnet.Yellow),
			CanShockHealthyHearts ? "can" : "cannot"
		);
	}
}