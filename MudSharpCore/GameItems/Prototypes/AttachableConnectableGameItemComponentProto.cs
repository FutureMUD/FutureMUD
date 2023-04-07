using System;
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

public class AttachableConnectableGameItemComponentProto : GameItemComponentProto
{
	protected AttachableConnectableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Attachable Connectable")
	{
	}

	protected AttachableConnectableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public ConnectorType Connector { get; set; }
	public override string TypeDescription => "Attachable Connectable";

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("Connector");
		if (element != null)
		{
			Connector = new ConnectorType((Gender)Convert.ToSByte(element.Attribute("gender").Value),
				element.Attribute("type").Value);
		}
	}

	#region Overrides of GameItemComponentProto

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttype <type name> - changes the connector type, e.g. 'TruckHitch'\n\tgender <male|female|neuter> - sets the gender of the connector. Male can only connect to female whereas neuter connect to one another universally.";

	public override string ShowBuildingHelp => BuildingHelpText;

	#endregion

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "connection":
			case "connection type":
				return BuildingCommandConnectionType(actor, command);
			case "gender":
			case "connection gender":
				return BuildingCommandConnectionGender(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandConnectionType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What connection type do you want to set for this item?");
			return false;
		}

		Connector = Connector == null
			? new ConnectorType(Form.Shape.Gender.Neuter, command.SafeRemainingArgument.TitleCase())
			: new ConnectorType(Connector.Gender, command.SafeRemainingArgument.TitleCase());
		Changed = true;
		actor.Send(
			$"This item will now connect to other connectables of type {Connector.ConnectionType.Colour(Telnet.Green)}, and is of gender {Gendering.Get(Connector.Gender).GenderClass(true).Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandConnectionGender(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What connection gender do you want to set for this item?");
			return false;
		}

		if (Connector == null)
		{
			actor.Send("You must first set a connection type before you can set a gender.");
			return false;
		}

		var gendering = Gendering.Get(command.Pop());
		if (gendering.Enum == Form.Shape.Gender.Indeterminate)
		{
			actor.Send("You can either set the connection type to male, female or neuter.");
			return false;
		}

		Connector = new ConnectorType(gendering.Enum, Connector.ConnectionType);
		Changed = true;
		actor.Send(
			$"This item will now connect to other connectables of type {Connector.ConnectionType.Colour(Telnet.Green)}, and is of gender {Gendering.Get(Connector.Gender).GenderClass(true).Colour(Telnet.Green)}.");
		return true;
	}

	public override bool CanSubmit()
	{
		return !string.IsNullOrEmpty(Connector?.ConnectionType) && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		return !string.IsNullOrEmpty(Connector?.ConnectionType)
			? "You must first set a connector type."
			: base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Attachable Connectable Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nThis item can be attached to items, and is also a connector of type {Connector?.ConnectionType.Colour(Telnet.Green) ?? "Unknown".Colour(Telnet.Red)} and gender {Gendering.Get(Connector?.Gender ?? Form.Shape.Gender.Indeterminate).GenderClass(true).Colour(Telnet.Green)}.";
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				Connector != null
					? new XElement("Connector", new XAttribute("gender", (short)Connector.Gender),
						new XAttribute("type", Connector.ConnectionType))
					: null).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("attachableconnectable", true,
			(gameworld, account) => new AttachableConnectableGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("attachable connectable", false,
			(gameworld, account) => new AttachableConnectableGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("attachconnect", false,
			(gameworld, account) => new AttachableConnectableGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("attconn", false,
			(gameworld, account) => new AttachableConnectableGameItemComponentProto(gameworld, account));

		manager.AddDatabaseLoader("Attachable Connectable",
			(proto, gameworld) => new AttachableConnectableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"AttachableConnectable",
			$"Makes an item {"[connectable]".Colour(Telnet.BoldBlue)} to others with a non-powered, non-data connection",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new AttachableConnectableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new AttachableConnectableGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new AttachableConnectableGameItemComponentProto(proto, gameworld));
	}
}