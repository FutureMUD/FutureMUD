using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class ElectricGridOutletGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "ElectricGridOutlet";

	public List<ConnectorType> Connections { get; } = new();

	#region Constructors

	protected ElectricGridOutletGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ElectricGridOutlet")
	{
		Connections.Add(new ConnectorType(Form.Shape.Gender.Female,
			Gameworld.GetStaticConfiguration("DefaultPowerSocketType"), true));
		Connections.Add(new ConnectorType(Form.Shape.Gender.Female,
			Gameworld.GetStaticConfiguration("DefaultPowerSocketType"), true));
	}

	protected ElectricGridOutletGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

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

	#endregion

	#region Saving

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

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ElectricGridOutletGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ElectricGridOutletGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ElectricGridOutlet".ToLowerInvariant(), true,
			(gameworld, account) => new ElectricGridOutletGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ElectricGridOutlet",
			(proto, gameworld) => new ElectricGridOutletGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ElectricGridOutlet",
			$"Item {"[provides power]".Colour(Telnet.BoldMagenta)} to connected {"[connectable]".Colour(Telnet.BoldYellow)} items from an {"[electric grid]".Colour(Telnet.BoldOrange)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ElectricGridOutletGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tconnection add <gender> <type> <powered> - adds a connection\n\tconnection remove <gender> <type> - removes a connection";

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
			actor.Send("Should the connection be powered? You must answer true or false");
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
		if (Connections.All(x => x.Gender != Form.Shape.Gender.Female))
		{
			return "You must first add at least one female connector type.";
		}

		return Connections.Count(x => x.Powered && x.Gender == Form.Shape.Gender.Male) > 1
			? "You can only have one powered male connection at a time."
			: base.WhyCannotSubmit();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item provides power sockets from a grid, with the following connections: {4}.",
			"ElectricGridOutlet Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Connections.Select(
				           x =>
					           $"{x.ConnectionType.Colour(Telnet.Green)} {(x.Powered ? "[P]" : "")} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})")
			           .ListToString()
		);
	}
}