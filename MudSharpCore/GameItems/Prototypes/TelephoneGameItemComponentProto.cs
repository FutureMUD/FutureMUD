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

public class TelephoneGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
	public override string TypeDescription => "Telephone";
	public double Wattage { get; set; }
	public string RingEmote { get; set; }
	public string TransmitPremote { get; set; }
	public List<ConnectorType> Connections { get; } = [];
	IEnumerable<ConnectorType> IConnectableItemProto.Connections => Connections;

	protected TelephoneGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Telephone")
	{
		Wattage = 5.0;
		RingEmote = "@ ring|rings loudly.";
		TransmitPremote = "@ speak|speaks into $1 and say|says";
		Connections.Add(new ConnectorType(MudSharp.Form.Shape.Gender.Male, "telephone", true));
	}

	protected TelephoneGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto,
		gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Wattage = double.Parse(root.Element("Wattage")?.Value ?? "5.0");
		RingEmote = root.Element("RingEmote")?.Value ?? "@ ring|rings loudly.";
		TransmitPremote = root.Element("TransmitPremote")?.Value ?? "@ speak|speaks into $1 and say|says";
		Connections.Clear();

		var connectors = root.Element("Connectors");
		if (connectors != null)
		{
			foreach (var item in connectors.Elements("Connection"))
			{
				Connections.Add(new ConnectorType(
					(MudSharp.Form.Shape.Gender)Convert.ToSByte(item.Attribute("gender")?.Value ?? "0"),
					item.Attribute("type")?.Value ?? "telephone",
					bool.Parse(item.Attribute("powered")?.Value ?? "true")));
			}
		}

		if (Connections.Any())
		{
			return;
		}

		var connector = root.Element("Connector");
		Connections.Add(connector == null
			? new ConnectorType(MudSharp.Form.Shape.Gender.Male, "telephone", true)
			: new ConnectorType((MudSharp.Form.Shape.Gender)Convert.ToSByte(connector.Attribute("gender")?.Value ?? "0"),
				connector.Attribute("type")?.Value ?? "telephone",
				bool.Parse(connector.Attribute("powered")?.Value ?? "true")));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Wattage", Wattage),
			new XElement("RingEmote", new XCData(RingEmote)),
			new XElement("TransmitPremote", new XCData(TransmitPremote)),
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
		return new TelephoneGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new TelephoneGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Telephone".ToLowerInvariant(), true,
			(gameworld, account) => new TelephoneGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Telephone",
			(proto, gameworld) => new TelephoneGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Telephone",
			$"Connects an item to a {"[telecommunications grid]".Colour(Telnet.BoldBlue)} and allows ringing, answering and live calls. It can also expose physical telecom connectors.",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new TelephoneGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\twatts <#> - sets how much power the telephone draws when switched on\n\tring <emote> - sets the emote used when the phone rings\n\tpremote <emote> - sets the emote prepended when a character transmits speech into the phone\n\tconnection add <gender> <type> <powered> - adds a connector type\n\tconnection remove <gender> <type> - removes a connector type";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "watts":
			case "watt":
			case "wattage":
				return BuildingCommandWatts(actor, command);
			case "ring":
			case "ringemote":
				return BuildingCommandRing(actor, command);
			case "premote":
			case "transmit":
			case "transmitemote":
				return BuildingCommandPremote(actor, command);
			case "connection":
			case "connector":
			case "connections":
			case "connectors":
				return BuildingCommandConnection(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandConnection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to add or remove a connection type for this telephone?");
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
			default:
				actor.Send("Do you want to add or remove a connection type for this telephone?");
				return false;
		}
	}

	private bool BuildingCommandConnectionAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What gender of connection do you want to add?");
			return false;
		}

		var gendering = Gendering.Get(command.PopSpeech());
		if (gendering.Enum == MudSharp.Form.Shape.Gender.Indeterminate)
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

		if (!bool.TryParse(command.PopSpeech(), out var powered))
		{
			actor.Send("Should the connection be powered? You must answer true or false.");
			return false;
		}

		if (gendering.Enum == MudSharp.Form.Shape.Gender.Neuter && powered)
		{
			actor.Send("Ungendered connections cannot also be powered.");
			return false;
		}

		Connections.Add(new ConnectorType(gendering.Enum, type, powered));
		Changed = true;
		actor.Send(
			$"This telephone now has an additional {(powered ? "powered" : "unpowered")} connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
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
		if (gendering.Enum == MudSharp.Form.Shape.Gender.Indeterminate)
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
			x.ConnectionType.Equals(type, StringComparison.InvariantCultureIgnoreCase) &&
			x.Gender == gendering.Enum);
		if (connector == null)
		{
			actor.Send("There is no connection like that to remove.");
			return false;
		}

		Connections.Remove(connector);
		Changed = true;
		actor.Send(
			$"This telephone now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandWatts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many watts should this telephone draw while switched on?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			actor.Send("You must enter a non-negative number of watts.");
			return false;
		}

		Wattage = value;
		Changed = true;
		actor.Send($"This telephone will now draw {Wattage:N2} watts while switched on.");
		return true;
	}

	private bool BuildingCommandRing(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote should this telephone use when it rings?");
			return false;
		}

		RingEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"The ring emote is now: {RingEmote}");
		return true;
	}

	private bool BuildingCommandPremote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What premote should be used when someone transmits into the phone?");
			return false;
		}

		TransmitPremote = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"The transmit premote is now: {TransmitPremote}");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a telephone that draws {4:N2} watts while on.\r\nIt has the following connections: {5}.",
			"Telephone Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Wattage,
			Connections.Select(
				           x =>
					           $"{x.ConnectionType.Colour(Telnet.Green)} {(x.Powered ? "[P]" : "")} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})")
			           .ListToString()
		);
	}

	public override bool CanSubmit()
	{
		return Connections.Any() && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		return Connections.Any()
			? base.WhyCannotSubmit()
			: "You must first add at least one connector type.";
	}
}
