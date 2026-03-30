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
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class TelecommunicationsGridFeederGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
	public override string TypeDescription => "TelecommunicationsGridFeeder";
	public double Wattage { get; set; }
	public List<ConnectorType> Connections { get; } = [];
	IEnumerable<ConnectorType> IConnectableItemProto.Connections => Connections;

	protected TelecommunicationsGridFeederGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "TelecommunicationsGridFeeder")
	{
		Wattage = 20.0;
		Connections.Add(new ConnectorType(Form.Shape.Gender.Female, "TelephoneLine", true));
	}

	protected TelecommunicationsGridFeederGameItemComponentProto(Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
		LoadFromXml(XElement.Parse(proto.Definition));
	}

	protected override void LoadFromXml(XElement root)
	{
		Wattage = double.Parse(root.Element("Wattage")?.Value ?? "20.0");
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
			new XElement("Wattage", Wattage),
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
		return new TelecommunicationsGridFeederGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new TelecommunicationsGridFeederGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("TelecommunicationsGridFeeder".ToLowerInvariant(), true,
			(gameworld, account) => new TelecommunicationsGridFeederGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("TelecommunicationsGridFeeder",
			(proto, gameworld) => new TelecommunicationsGridFeederGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"TelecommunicationsGridFeeder",
			$"Creates a telephone jack that can also {"[provide power]".Colour(Telnet.BoldMagenta)} to connected devices and can be linked to a {"[telecommunications grid]".Colour(Telnet.BoldBlue)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new TelecommunicationsGridFeederGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	watts <#> - sets the maximum wattage this feeder can provide
	connection add <gender> <type> <powered> - adds a connection type
	connection remove <gender> <type> - removes a connection type";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "watts":
			case "watt":
			case "wattage":
				return BuildingCommandWatts(actor, command);
			case "connection":
			case "connector":
			case "connections":
			case "connectors":
				return BuildingCommandConnection(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandWatts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many watts should this feeder be able to provide?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			actor.Send("You must enter a non-negative number of watts.");
			return false;
		}

		Wattage = value;
		Changed = true;
		actor.Send($"This feeder will now be able to provide up to {Wattage:N2} watts.");
		return true;
	}

	private bool BuildingCommandConnection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to add or remove a connection type for this feeder?");
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

		actor.Send("Do you want to add or remove a connection type for this feeder?");
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
			$"This feeder will now have an additional {(powered ? "powered" : "unpowered")} connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
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
			$"This feeder now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a telecommunications wall jack that can provide {4:N2} watts to connected devices.\r\nIt has the following connections: {5}.",
			"TelecommunicationsGridFeeder Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Wattage,
			Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} {(x.Powered ? "[P]" : "")} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})").ListToString()
		);
	}
}
