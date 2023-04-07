using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class BatteryGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Battery";

	public string BatteryType { get; set; }
	public double BaseWattHours { get; set; }
	public double WattHoursPerQuality { get; set; }
	public bool Rechargable { get; set; }

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("BatteryType", new XCData(BatteryType)),
			new XElement("BaseWattHours", BaseWattHours),
			new XElement("WattHoursPerQuality", WattHoursPerQuality),
			new XElement("Rechargable", Rechargable)
		).ToString();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis is a battery of type {4}, that provides a base watt-hours of {5:N2} plus {6:N2} watt-hours per point of quality. It {7} rechargable.",
			"Battery Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			BatteryType,
			BaseWattHours,
			WattHoursPerQuality,
			Rechargable ? "is" : "is not"
		);
	}

	#region Constructors

	protected BatteryGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Battery")
	{
		BatteryType = "AAA";
		BaseWattHours = 1.0;
		WattHoursPerQuality = 0.13;
		Rechargable = false;
	}

	protected BatteryGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		BatteryType = root.Element("BatteryType").Value;
		BaseWattHours = double.Parse(root.Element("BaseWattHours").Value);
		WattHoursPerQuality = double.Parse(root.Element("WattHoursPerQuality").Value);
		Rechargable = bool.Parse(root.Element("Rechargable").Value);
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BatteryGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BatteryGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Battery".ToLowerInvariant(), true,
			(gameworld, account) => new BatteryGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Battery",
			(proto, gameworld) => new BatteryGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Battery",
			$"Turns an item into a {"[battery]".ColourCommand()}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new BatteryGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttype <type> - the type of the battery, e.g. AAA\n\twatts <watthours> - the battery capacity in watt-hours\n\tquality <watts> - bonus watts per point of quality of battery item\n\trechargable - toggles the battery being rechargable";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
				return BuildingCommandType(actor, command);
			case "base":
			case "watts":
			case "watthours":
			case "watt":
				return BuildingCommandWattage(actor, command);
			case "rechargable":
				return BuildingCommandRechargable(actor, command);
			case "quality":
				return BuildingCommandQuality(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandRechargable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			Rechargable = !Rechargable;
			Changed = true;
			actor.Send($"This battery is {(Rechargable ? "now" : "no longer")} rechargable.");
			return true;
		}

		if (!bool.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send(
				"You must either supply no argument to toggle the rechargability of the battery, or supply a true or false value.");
			return false;
		}

		Rechargable = value;
		Changed = true;
		actor.Send($"This battery is {(Rechargable ? "now" : "no longer")} rechargable.");
		return true;
	}

	private bool BuildingCommandQuality(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What amount of watt hours should be provided for each quality point of the parent item?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid amount of watt hours for this battery.");
			return false;
		}

		if (value < 0.0)
		{
			actor.Send("The bonus watt hours must be positive.");
			return false;
		}

		WattHoursPerQuality = value;
		Changed = true;
		actor.Send(
			$"This item now provides {WattHoursPerQuality:N2} watt hours per point of quality of the parent item.");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What type of battery do you want to set this battery to?");
			return false;
		}

		BatteryType = command.SafeRemainingArgument;
		actor.Send($"This battery is now of type {BatteryType.Colour(Telnet.Cyan)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandWattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many base watt hours of power should this battery provide?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid amount of watt hours.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.Send("You must enter a positive, non-zero number of watt hours.");
			return false;
		}

		BaseWattHours = value;
		actor.Send($"This battery now provides a base level of {BaseWattHours:N2} watt hours.");
		Changed = true;
		return true;
	}

	#endregion
}