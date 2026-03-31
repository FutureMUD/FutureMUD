using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class BatteryPoweredGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
	public override string TypeDescription => "BatteryPowered";

	public string BatteryType { get; set; }
	public int BatteryQuantity { get; set; }
	public bool BatteriesInSeries { get; set; }
	public double ChargeWattage { get; set; }

	/// <summary>
	///     Usually either "in" or "on"
	/// </summary>
	public string ContentsPreposition { get; protected set; }

	public bool Transparent { get; protected set; }
	public List<ConnectorType> Connections { get; } = [];
	IEnumerable<ConnectorType> IConnectableItemProto.Connections => Connections;

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("BatteryType", BatteryType),
			new XElement("BatteryQuantity", BatteryQuantity),
			new XElement("BatteriesInSeries", BatteriesInSeries),
			new XElement("ChargeWattage", ChargeWattage),
			new XElement("Transparent", Transparent),
			new XElement("ContentsPreposition", ContentsPreposition),
			new XElement("Connectors",
				from connector in Connections
				select new XElement("Connection",
					new XAttribute("gender", (short)connector.Gender),
					new XAttribute("type", connector.ConnectionType),
					new XAttribute("powered", connector.Powered)))
		).ToString();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item provides battery power to its parent item. It takes {4:N0} batteries {9} of type {5}. It {6} transparent and shows its contents as \"{7}\". It can charge at {8:N2} watts when connected.\nIt has the following connections: {10}.",
			"BatteryPowered Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			BatteryQuantity,
			BatteryType.Colour(Telnet.Green),
			Transparent ? "is" : "is not",
			ContentsPreposition,
			ChargeWattage,
			BatteriesInSeries ? "in series" : "in parallel",
			Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} {(x.Powered ? "[P]" : "")} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})").ListToString()
		);
	}

	#region Constructors

	protected BatteryPoweredGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "BatteryPowered")
	{
		BatteryType = "AAA";
		BatteryQuantity = 4;
		BatteriesInSeries = true;
		ChargeWattage = 20.0;
		ContentsPreposition = "in";
		Transparent = false;
	}

	protected BatteryPoweredGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
		LoadFromXml(XElement.Parse(proto.Definition));
	}

	protected override void LoadFromXml(XElement root)
	{
		BatteryType = root.Element("BatteryType").Value;
		BatteryQuantity = int.Parse(root.Element("BatteryQuantity").Value);
		BatteriesInSeries = bool.Parse(root.Element("BatteriesInSeries").Value);
		ChargeWattage = double.Parse(root.Element("ChargeWattage")?.Value ?? "20.0");
		Transparent = bool.Parse(root.Element("Transparent").Value);
		ContentsPreposition = root.Element("ContentsPreposition").Value;

		Connections.Clear();
		var element = root.Element("Connectors");
		if (element != null)
		{
			foreach (var item in element.Elements("Connection"))
			{
				Connections.Add(new ConnectorType((Gender)Convert.ToSByte(item.Attribute("gender")!.Value),
					item.Attribute("type")!.Value, bool.Parse(item.Attribute("powered")!.Value)));
			}
		}
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BatteryPoweredGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BatteryPoweredGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("batterypowered", true,
			(gameworld, account) => new BatteryPoweredGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("batterypower", false,
			(gameworld, account) => new BatteryPoweredGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("bpowered", false,
			(gameworld, account) => new BatteryPoweredGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("BatteryPowered",
			(proto, gameworld) => new BatteryPoweredGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"BatteryPowered",
			$"A {"[container]".Colour(Telnet.BoldGreen)} that {"[provides power]".Colour(Telnet.BoldMagenta)} to other components via inserted batteries and can expose a physical connector",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new BatteryPoweredGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	type <type> - sets the battery type, e.g. AAA
	number <amount> - how many batteries it can hold
	preposition <prep> - overrides the 'in' container preposition
	transparent - toggles whether you can see contents when closed
	series - toggles whether the batteries are treated as in series or in parallel
	charge <watts> - sets the maximum rate the batteries can recharge at when connected to power
	connection add <gender> <type> <powered> - adds a connection type
	connection remove <gender> <type> - removes a connection type.

Note:

    In series batteries would be used for something needing a higher voltage, but drains the batteries faster.
    In parallel batteries split the drain amongst themselves so drain slower";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "preposition":
				return BuildingCommand_Preposition(actor, command);
			case "transparent":
				return BuildingCommand_Transparent(actor, command);
			case "type":
			case "battery":
			case "battery type":
				return BuildingCommand_BatteryType(actor, command);
			case "number":
			case "capacity":
			case "quantity":
				return BuildingCommand_Quantity(actor, command);
			case "series":
			case "inseries":
			case "in series":
				return BuildingCommandSeries(actor, command);
			case "charge":
			case "chargewattage":
			case "charge watts":
				return BuildingCommandChargeWattage(actor, command);
			case "connection":
			case "connector":
			case "connections":
			case "connectors":
				return BuildingCommandConnection(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandChargeWattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many watts should this battery powered item be able to charge at when connected to power?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.Send("You must enter a non-negative number of watts.");
			return false;
		}

		ChargeWattage = value;
		Changed = true;
		actor.Send($"This battery powered item will now charge at up to {ChargeWattage:N2} watts.");
		return true;
	}

	private bool BuildingCommandConnection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to add or remove a connection type for this battery powered item?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
				return BuildingCommandConnectionAdd(actor, command);
			case "remove":
			case "rem":
			case "del":
			case "delete":
				return BuildingCommandConnectionRemove(actor, command);
		}

		actor.Send("Do you want to add or remove a connection type for this battery powered item?");
		return false;
	}

	private bool BuildingCommandConnectionAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What gender of connection do you want to add?");
			return false;
		}

		var gendering = Gendering.Get(command.PopSpeech());
		if (gendering.Enum == Form.Shape.Gender.Indeterminate)
		{
			actor.Send("You can either set the connection type to male, female or neuter.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What type of connection do you want this connector to be?");
			return false;
		}

		var type = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.Send("Should the connection be powered?");
			return false;
		}

		if (!bool.TryParse(command.PopSpeech(), out var powered))
		{
			actor.Send("Should the connection be powered? You must answer true or false.");
			return false;
		}

		if (gendering.Enum == Form.Shape.Gender.Neuter && powered)
		{
			actor.Send("Ungendered connections cannot also be powered.");
			return false;
		}

		Connections.Add(new ConnectorType(gendering.Enum, type, powered));
		actor.Send(
			$"This battery powered item will now have an additional {(powered ? "powered" : "unpowered")} connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandConnectionRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What gender of connection do you want to remove?");
			return false;
		}

		var gendering = Gendering.Get(command.PopSpeech());
		if (gendering.Enum == Form.Shape.Gender.Indeterminate)
		{
			actor.Send("Connection types can be male, female or neuter.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What type of connection do you want to remove?");
			return false;
		}

		var type = command.SafeRemainingArgument;
		var connector = Connections.FirstOrDefault(x =>
			x.ConnectionType.Equals(type, StringComparison.InvariantCultureIgnoreCase) && x.Gender == gendering.Enum);
		if (connector == null)
		{
			actor.Send("There is no connection like that to remove.");
			return false;
		}

		Connections.Remove(connector);
		actor.Send(
			$"This battery powered item now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Transparent(ICharacter actor, StringStack command)
	{
		Transparent = !Transparent;
		actor.Send("This battery powered item is {0} transparent.", Transparent ? "now" : "no longer");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Preposition(ICharacter actor, StringStack command)
	{
		var preposition = command.PopSpeech().ToLowerInvariant();
		if (string.IsNullOrEmpty(preposition))
		{
			actor.OutputHandler.Send("What preposition do you want to use for this battery powered item?");
			return false;
		}

		ContentsPreposition = preposition;
		Changed = true;
		actor.OutputHandler.Send("The contents of this battery powered item will now be described as \"" +
		                         ContentsPreposition +
		                         "\" it.");
		return true;
	}

	private bool BuildingCommand_BatteryType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What battery type should this battery powered item be designed for?");
			return false;
		}

		BatteryType = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This battery powered item is now of type {BatteryType.Colour(Telnet.Green)}.");
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
			actor.Send("How many batteries should fit into this battery powered item at a time?");
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
			$"This battery powered item will now accept {BatteryQuantity:N0} batter{(BatteryQuantity == 1 ? "y" : "ies")} at a time.");
		return true;
	}

	private bool BuildingCommandSeries(ICharacter actor, StringStack command)
	{
		BatteriesInSeries = !BatteriesInSeries;
		actor.Send(
			$"This battery powered item has its batteries {(BatteriesInSeries ? "now" : "no longer")} in series.");
		Changed = true;
		return true;
	}

	#endregion
}
