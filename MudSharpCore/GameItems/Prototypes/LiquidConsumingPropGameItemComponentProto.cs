#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class LiquidConsumingPropGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
	public override string TypeDescription => "LiquidConsumingProp";
	public double LiquidCapacity { get; set; }
	public double ConsumptionPerSecond { get; set; }
	public bool Transparent { get; set; }
	public bool CanBeEmptiedWhenInRoom { get; set; }
	public string ContentsPreposition { get; set; }
	public List<ConnectorType> Connections { get; } = [];
	IEnumerable<ConnectorType> IConnectableItemProto.Connections => Connections;

	protected LiquidConsumingPropGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "LiquidConsumingProp")
	{
		LiquidCapacity = 10.0;
		ConsumptionPerSecond = 0.1;
		Transparent = true;
		CanBeEmptiedWhenInRoom = true;
		ContentsPreposition = "in";
		Connections.Add(new ConnectorType(Form.Shape.Gender.Female, "LiquidLine", false));
	}

	protected LiquidConsumingPropGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
		LoadFromXml(XElement.Parse(proto.Definition));
	}

	protected override void LoadFromXml(XElement root)
	{
		LiquidCapacity = double.Parse(root.Element("LiquidCapacity")?.Value ?? "10.0");
		ConsumptionPerSecond = double.Parse(root.Element("ConsumptionPerSecond")?.Value ?? "0.1");
		Transparent = bool.Parse(root.Element("Transparent")?.Value ?? "true");
		CanBeEmptiedWhenInRoom = bool.Parse(root.Element("CanBeEmptiedWhenInRoom")?.Value ?? "true");
		ContentsPreposition = root.Element("ContentsPreposition")?.Value ?? "in";
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

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("LiquidCapacity", LiquidCapacity),
			new XElement("ConsumptionPerSecond", ConsumptionPerSecond),
			new XElement("Transparent", Transparent),
			new XElement("CanBeEmptiedWhenInRoom", CanBeEmptiedWhenInRoom),
			new XElement("ContentsPreposition", ContentsPreposition),
			new XElement("Connectors",
				from connector in Connections
				select new XElement("Connection",
					new XAttribute("gender", (short)connector.Gender),
					new XAttribute("type", connector.ConnectionType),
					new XAttribute("powered", connector.Powered)))
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new LiquidConsumingPropGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new LiquidConsumingPropGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("LiquidConsumingProp".ToLowerInvariant(), true,
			(gameworld, account) => new LiquidConsumingPropGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("LiquidConsumingProp",
			(proto, gameworld) => new LiquidConsumingPropGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"LiquidConsumingProp",
			$"A prop that stores liquid and then {"[consumes]".Colour(Telnet.BoldMagenta)} it at a fixed rate, making it handy for steady drain setups",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new LiquidConsumingPropGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	capacity <amount> - sets the liquid capacity
	consume <amount> - sets the amount consumed per second
	transparent - toggles whether the contents are visible
	emptyroom - toggles whether it can be emptied when in the room
	preposition <prep> - overrides the content preposition
	connection add <gender> <type> <powered> - adds a connection type
	connection remove <gender> <type> - removes a connection type";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "capacity":
				return BuildingCommandCapacity(actor, command);
			case "consume":
			case "consumption":
				return BuildingCommandConsumption(actor, command);
			case "transparent":
				Transparent = !Transparent;
				Changed = true;
				actor.Send("This prop is {0} transparent.", Transparent ? "now" : "no longer");
				return true;
			case "emptyroom":
				CanBeEmptiedWhenInRoom = !CanBeEmptiedWhenInRoom;
				Changed = true;
				actor.Send("This prop is {0} able to be emptied when in the room.", CanBeEmptiedWhenInRoom ? "now" : "no longer");
				return true;
			case "preposition":
				return BuildingCommandPreposition(actor, command);
			case "connection":
			case "connector":
			case "connections":
			case "connectors":
				return BuildingCommandConnection(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much liquid should this prop be able to hold?");
			return false;
		}

		var value = actor.Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume, out var success);
		if (!success || value < 0.0)
		{
			actor.Send("You must enter a valid, non-negative amount of liquid.");
			return false;
		}

		LiquidCapacity = value;
		Changed = true;
		actor.Send($"This prop will now hold {actor.Gameworld.UnitManager.Describe(LiquidCapacity, UnitType.FluidVolume, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandConsumption(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much liquid should this prop consume per second?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.Send("You must enter a non-negative number.");
			return false;
		}

		ConsumptionPerSecond = value;
		Changed = true;
		actor.Send($"This prop will now consume {ConsumptionPerSecond:N2} units per second.");
		return true;
	}

	private bool BuildingCommandPreposition(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What preposition do you want to use for this prop's contents?");
			return false;
		}

		ContentsPreposition = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This prop will now describe its contents as \"{ContentsPreposition}\" it.");
		return true;
	}

	private bool BuildingCommandConnection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to add or remove a connection type for this prop?");
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

		actor.Send("Do you want to add or remove a connection type for this prop?");
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
			$"This prop will now have an additional {(powered ? "powered" : "unpowered")} connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
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
			$"This prop now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis prop can hold {4} and consumes it at {5:N2} per second.",
			"LiquidConsumingProp Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Gameworld.UnitManager.Describe(LiquidCapacity, UnitType.FluidVolume, actor).ColourValue(),
			ConsumptionPerSecond
		);
	}
}
