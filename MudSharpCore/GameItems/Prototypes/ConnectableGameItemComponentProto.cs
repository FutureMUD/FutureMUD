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

public class ConnectableGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
	protected ConnectableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Connectable")
	{
	}

	protected ConnectableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public List<ConnectorType> Connections { get; } = new();
	IEnumerable<ConnectorType> IConnectableItemProto.Connections => Connections;

	public override string TypeDescription => "Connectable";

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("Connectors");
		if (element != null)
		{
			foreach (var item in element.Elements("Connection"))
			{
				Connections.Add(new ConnectorType((Gender)Convert.ToSByte(item.Attribute("gender").Value),
					item.Attribute("type").Value, bool.Parse(item.Attribute("powered").Value)));
			}
		}
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tconnection add <gender> <type> <powered> - adds a connection to this item\n\tconnection remove <gender> <type>";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "connection":
			case "connection type":
				return BuildingCommandConnectionType(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandConnectionTypeAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What gender of connection do you want to add?");
			return false;
		}

		var gendering = Gendering.Get(command.Pop());
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
		type =
			Gameworld.ItemComponentProtos.Except(this)
			         .OfType<IConnectableItemProto>()
			         .SelectMany(x => x.Connections.Select(y => y.ConnectionType))
			         .FirstOrDefault(x => x.EqualTo(type) && !x.Equals(type, StringComparison.InvariantCulture)) ??
			type;

		if (command.IsFinished)
		{
			actor.Send("Should the connection be powered?");
			return false;
		}

		if (!bool.TryParse(command.Pop(), out var powered))
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
			$"This connector will now have an additional {(powered ? "powered" : "unpowered")} connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandConnectionTypeRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What gender of connection do you want to remove?");
			return false;
		}

		var gendering = Gendering.Get(command.Pop());
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
		if (
			Connections.Any(
				x =>
					x.ConnectionType.Equals(type, StringComparison.InvariantCultureIgnoreCase) &&
					x.Gender == gendering.Enum))
		{
			Connections.Remove(
				Connections.First(
					x =>
						x.ConnectionType.Equals(type,
							StringComparison.InvariantCultureIgnoreCase) && x.Gender == gendering.Enum));
			actor.Send(
				$"This connector now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
			Changed = true;
			return true;
		}

		actor.Send("There is no connection like that to remove.");
		return false;
	}

	private bool BuildingCommandConnectionType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to add or remove a connection type for this connector?");
			return false;
		}

		switch (command.Pop().ToLowerInvariant())
		{
			case "add":
				return BuildingCommandConnectionTypeAdd(actor, command);
			case "remove":
			case "rem":
			case "del":
			case "delete":
				return BuildingCommandConnectionTypeRemove(actor, command);
		}

		actor.Send("Do you want to add or remove a connection type for this connector?");
		return false;
	}

	public override bool CanSubmit()
	{
		return Connections.Any() && Connections.Count(x => x.Powered && x.Gender == Form.Shape.Gender.Male) <= 1 &&
		       base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (!Connections.Any())
		{
			return "You must first add at least one connector type.";
		}

		return Connections.Count(x => x.Powered && x.Gender == Form.Shape.Gender.Male) > 1
			? "You can only have one powered male connection at a time."
			: base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Connectable Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nThis item is a connector and has the following connections: {Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} {(x.Powered ? "[P]" : "")}({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})").ListToString()}.";
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Connectors",
				from connector in Connections
				select
					new XElement("Connection",
						new XAttribute("gender", (short)connector.Gender),
						new XAttribute("type", connector.ConnectionType),
						new XAttribute("powered", connector.Powered)
					)
			)
		).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("connectable", true,
			(gameworld, account) => new ConnectableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Connectable",
			(proto, gameworld) => new ConnectableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Connectable",
			$"Makes an item {"[connectable]".Colour(Telnet.BoldBlue)}, can potentially be powered e.g. a power plug on an appliance",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ConnectableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ConnectableGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ConnectableGameItemComponentProto(proto, gameworld));
	}
}