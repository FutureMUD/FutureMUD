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
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class DripGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Drip";

	public List<ConnectorType> Connections { get; } = new();

	#region Constructors

	protected DripGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Drip")
	{
		Connections.Add(new ConnectorType(Form.Shape.Gender.Male, "cannula", false));
		Connections.Add(new ConnectorType(Form.Shape.Gender.Female, "cannula", false));
	}

	protected DripGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
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
					item.Attribute("type").Value));
			}
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
					new XElement("Connectors",
						from connector in Connections
						select
							new XElement("Connection",
								new XAttribute("gender", (short)connector.Gender),
								new XAttribute("type", connector.ConnectionType)
							)
					)
				)
				.ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new DripGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new DripGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Drip".ToLowerInvariant(), true,
			(gameworld, account) => new DripGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Drip", (proto, gameworld) => new DripGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Drip",
			$"Item is {"[connectable]".Colour(Telnet.BoldBlue)} between a cannula and an IV bag to rate limit infusion",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new DripGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttype add <male|female|neuter> <type name> - adds a connection of the specified gender and name\n\ttype remove <male|female|neuter> <type name> - removes a connection of the specified gender and name";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "connection":
			case "connection type":
				return BuildingCommandConnectionType(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}


	public override bool CanSubmit()
	{
		return Connections.Any() && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (!Connections.Any())
		{
			return "You must first add at least one connector type.";
		}

		return base.WhyCannotSubmit();
	}

	#region Building Command SubCommands

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
			actor.Send("What type of connection do you want this IV Bag to be?");
			return false;
		}

		var type = command.PopSpeech();
		type =
			Gameworld.ItemComponentProtos.Except(this)
			         .OfType<IConnectableItemProto>()
			         .SelectMany(x => x.Connections.Select(y => y.ConnectionType))
			         .FirstOrDefault(x => x.EqualTo(type) && !x.Equals(type, StringComparison.InvariantCulture)) ??
			type;

		Connections.Add(new ConnectorType(gendering.Enum, type, false));
		actor.Send(
			$"This IV Bag will now have an additional connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
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
				$"This IV Bag now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
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
			actor.Send("Do you want to add or remove a connection type for this IV Bag?");
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

		actor.Send("Do you want to add or remove a connection type for this IV Bag?");
		return false;
	}

	#endregion

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(
			actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis is a drip which can rate limit between an IV Bag and a cannula. It has the following connections: {4}.",
			"Drip Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Connections.Select(
				           x =>
					           $"{x.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})")
			           .ListToString()
		);
	}
}