using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class BatteryChargerGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "BatteryCharger";

	public string BatteryType { get; set; }
	public int BatteryQuantity { get; set; }
	public double Wattage { get; set; }
	public double Efficiency { get; set; }

	/// <summary>
	///     Usually either "in" or "on"
	/// </summary>
	public string ContentsPreposition { get; protected set; }

	public bool Transparent { get; protected set; }

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("BatteryType", BatteryType),
			new XElement("BatteryQuantity", BatteryQuantity),
			new XElement("Wattage", Wattage),
			new XElement("Efficiency", Efficiency),
			new XElement("ContentsPreposition", ContentsPreposition),
			new XElement("Transparent", Transparent)
		).ToString();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a battery charger for batteries of type {4}. It can charge {5:N0} at one time and provides {6:N2} watts of power at an efficiency of {7:P3}. It {8} transparent and shows its contents as \"{9}\".",
			"BatteryCharger Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			BatteryType.Colour(Telnet.Green),
			BatteryQuantity,
			Wattage,
			Efficiency,
			Transparent ? "is" : "is not",
			ContentsPreposition
		);
	}

	#region Constructors

	protected BatteryChargerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "BatteryCharger")
	{
		BatteryType = "AAA";
		BatteryQuantity = 8;
		Wattage = 1.0;
		Efficiency = 0.75;
		ContentsPreposition = "in";
		Transparent = true;
	}

	protected BatteryChargerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		BatteryType = root.Element("BatteryType").Value;
		BatteryQuantity = int.Parse(root.Element("BatteryQuantity").Value);
		Wattage = double.Parse(root.Element("Wattage").Value);
		Efficiency = double.Parse(root.Element("Efficiency").Value);
		ContentsPreposition = root.Element("ContentsPreposition").Value;
		Transparent = bool.Parse(root.Element("Transparent").Value);
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BatteryChargerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BatteryChargerGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("BatteryCharger".ToLowerInvariant(), true,
			(gameworld, account) => new BatteryChargerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("BatteryCharger",
			(proto, gameworld) => new BatteryChargerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"BatteryCharger",
			$"A type of {"[container]".Colour(Telnet.BoldGreen)} that when {"[powered]".Colour(Telnet.Magenta)} charges rechargeable {"[battery]".ColourCommand()} items",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new BatteryChargerGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttype <type> - which battery type they take, e.g. AAA\n\tcapacity <number> - how many batteries fit in at once\n\tpreposition <prep> - overrides the 'in' preposition for displaying contents\n\ttransparent - toggles whether you can see the contents when it is closed\n\tpower <watts> - the power in watts that this charger draws\n\tefficiency <%> - the percentage of power drawn transferred to the batteries";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "preposition":
				return BuildingCommand_Preposition(actor, command);
			case "transparent":
				return BuildingCommand_Transparent(actor, command);
			case "efficiency":
				return BuildingCommandEfficiency(actor, command);
			case "type":
			case "battery":
			case "battery type":
				return BuildingCommand_BatteryType(actor, command);
			case "number":
			case "capacity":
			case "quantity":
				return BuildingCommand_Quantity(actor, command);
			case "power":
			case "wattage":
			case "watt":
			case "watts":
				return BuildingCommandWattage(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_Transparent(ICharacter actor, StringStack command)
	{
		Transparent = !Transparent;
		actor.Send("This battery charger is {0} transparent.", Transparent ? "now" : "no longer");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Preposition(ICharacter actor, StringStack command)
	{
		var preposition = command.Pop().ToLowerInvariant();
		if (string.IsNullOrEmpty(preposition))
		{
			actor.OutputHandler.Send("What preposition do you want to use for this container?");
			return false;
		}

		ContentsPreposition = preposition;
		Changed = true;
		actor.OutputHandler.Send("The contents of this container will now be described as \"" + ContentsPreposition +
		                         "\" it.");
		return true;
	}

	private bool BuildingCommand_BatteryType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What battery type should this battery charger be designed for?");
			return false;
		}

		BatteryType = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This battery is now of type {BatteryType.Colour(Telnet.Green)}.");
		if (
			!Gameworld.ItemComponentProtos.OfType<BatteryGameItemComponentProto>()
			          .Any(x => x.BatteryType.EqualTo(BatteryType)))
		{
			actor.Send(
				"Warning: There are no batteries built with this battery type. Are you sure you used the right type?"
					.Colour(Telnet.Yellow));
		}

		return true;
	}

	private bool BuildingCommand_Quantity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many batteries should fit into this charger at a time?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number of batteries.");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("You must enter a positive number of batteries greater than zero.");
			return false;
		}

		BatteryQuantity = value;
		Changed = true;
		actor.Send(
			$"This battery charger will now accept {BatteryQuantity:N0} batter{(BatteryQuantity == 1 ? "y" : "ies")} at a time.");
		return true;
	}

	private bool BuildingCommandWattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many watts should this charger provide when in use?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid amount of watts.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.Send("You must enter a positive, non-zero number of watts.");
			return false;
		}

		Wattage = value;
		actor.Send($"This battery charger now provides {Wattage:N2} watts.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandEfficiency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How efficient should this battery charger be?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid efficiency percentage.");
			return false;
		}

		if (value <= 0.0 || value > 1.0)
		{
			actor.Send("You must enter a number between 0.0 and 1.0 for efficiency.");
			return false;
		}

		Efficiency = value;
		actor.Send($"This battery charger now operates at {Efficiency:P3} efficiency.");
		Changed = true;
		return true;
	}

	#endregion
}