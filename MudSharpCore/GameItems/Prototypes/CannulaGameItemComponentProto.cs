using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
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

public class CannulaGameItemComponentProto : GameItemComponentProto
{
	public List<ConnectorType> Connections { get; } = new();

	public string ExternalDescription { get; protected set; }

	public IBodyPrototype TargetBody { get; protected set; }

	public override string TypeDescription => "Cannula";

	#region Constructors

	protected CannulaGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Cannula")
	{
		ExternalDescription = "a cannula";
		Connections.Add(new ConnectorType(Form.Shape.Gender.Female, "cannula", false));
	}

	protected CannulaGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
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

		TargetBody = Gameworld.BodyPrototypes.Get(long.Parse(root.Element("TargetBody")?.Value ?? "0"));
		ExternalDescription = root.Element("ExternalDescription").Value;
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
						new XAttribute("type", connector.ConnectionType)
					)
			),
			new XElement("TargetBody", TargetBody?.Id ?? 0L),
			new XElement("ExternalDescription", new XCData(ExternalDescription))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new CannulaGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new CannulaGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Cannula".ToLowerInvariant(), true,
			(gameworld, account) => new CannulaGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Cannula", (proto, gameworld) => new CannulaGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Cannula",
			$"An externally {"[connectable]".Colour(Telnet.BoldBlue)} {"[implant]".Colour(Telnet.Pink)} allowing access to the blood stream",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new CannulaGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttype add <male|female|neuter> <type name> - adds a connection of the specified gender and name\n\ttype remove <male|female|neuter> <type name> - removes a connection of the specified gender and name\n\tbody <id> - sets the body type to target for installation\n\tdesc <desc> - how this item appears on someone's full description when installed. Note, you can use $bodypart to reference the bodypart installed";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "connection":
			case "connection type":
				return BuildingCommandConnectionType(actor, command);
			case "body":
			case "targetbody":
			case "target_body":
			case "target body":
			case "target":
				return BuildingCommandBody(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandBody(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which body type do you want this cannula to be installed in?");
			return false;
		}

		var body = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.BodyPrototypes.Get(value)
			: Gameworld.BodyPrototypes.GetByName(command.SafeRemainingArgument);
		if (body == null)
		{
			actor.Send("There is no such body type for you to select.");
			return false;
		}

		TargetBody = body;
		Changed = true;
		actor.Send(
			$"You set the body type for this cannula to be #{TargetBody.Id.ToString("N0", actor)} ({TargetBody.Name.Colour(Telnet.Green)}).");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What text would you like to display in the full description of a character when this cannula is installed on their person?");
			return false;
		}

		ExternalDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.Send(
			$"When this cannula is installed, the following short description will now be used:\n\t{ExternalDescription.Colour(Telnet.Yellow)}");
		return true;
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
			actor.Send("What type of connection do you want this cannula to be?");
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
			$"This cannula will now have an additional connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
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
				$"This cannula now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
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
			actor.Send("Do you want to add or remove a connection type for this cannula?");
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

		actor.Send("Do you want to add or remove a connection type for this cannula?");
		return false;
	}

	public override bool CanSubmit()
	{
		return Connections.Any() && TargetBody != null && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (!Connections.Any())
		{
			return "You must first add at least one connector type.";
		}

		if (TargetBody == null)
		{
			return "You must set a target body.";
		}

		return base.WhyCannotSubmit();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Cannula Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nThis item is a cannula, and implantable connector designed for {(TargetBody != null ? $"body type {TargetBody.Name} (#{TargetBody.Id})" : "an unknown body type".Colour(Telnet.Red))}, and has the following connections: {Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})").ListToString()}.";
	}
}