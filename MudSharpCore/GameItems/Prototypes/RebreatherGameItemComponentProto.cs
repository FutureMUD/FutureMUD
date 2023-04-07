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

public class RebreatherGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
	public override string TypeDescription => "Rebreather";

	public ConnectorType Connector { get; private set; }

	IEnumerable<ConnectorType> IConnectableItemProto.Connections => new[] { Connector };

	public bool WaterTight { get; private set; }

	#region Constructors

	protected RebreatherGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Rebreather")
	{
		Connector = new ConnectorType(Form.Shape.Gender.Male, Gameworld.GetStaticConfiguration("DefaultGasSocketType"),
			false);
		WaterTight = false;
	}

	protected RebreatherGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("Connection");
		if (element != null)
		{
			Connector = new ConnectorType((Gender)Convert.ToSByte(element.Attribute("gender").Value),
				element.Attribute("type").Value, false);
		}

		WaterTight = bool.Parse(root.Element("WaterTight").Value);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WaterTight", WaterTight),
			new XElement("Connection",
				new XAttribute("gender", (short)Connector.Gender),
				new XAttribute("type", Connector.ConnectionType),
				new XAttribute("powered", false)
			)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new RebreatherGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new RebreatherGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Rebreather".ToLowerInvariant(), true,
			(gameworld, account) => new RebreatherGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Rebreather",
			(proto, gameworld) => new RebreatherGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Rebreather",
			$"Item is {"[connectable]".Colour(Telnet.BoldBlue)} to a {"gas container".Colour(Telnet.BoldGreen)} and combined with a {"[wearable]".Colour(Telnet.BoldYellow)} allows breathing the gas",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new RebreatherGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttype <type> - sets the connector type\n\twatertight - toggles water-tightness";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "connection":
			case "connection type":
				return BuildingCommandConnectionType(actor, command);
			case "watertight":
			case "water":
			case "tight":
				return BuildingCommandWaterTight(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandWaterTight(ICharacter actor, StringStack command)
	{
		WaterTight = !WaterTight;
		Changed = true;
		actor.OutputHandler.Send($"This rebreather is {(WaterTight ? "now" : "no longer")} water-tight.");
		return true;
	}

	private bool BuildingCommandConnectionType(ICharacter actor, StringStack command)
	{
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
		Connector = new ConnectorType(Form.Shape.Gender.Male, type, false);
		Changed = true;
		actor.OutputHandler.Send($"This item will now have a male connection of type {type.ColourValue()}.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a rebreather, allowing breathing of a connected gas supply. It has a male {4} connection. It {5} water-tight.",
			"Rebreather Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Connector.ConnectionType.ColourValue(),
			WaterTight ? "is" : "is not"
		);
	}
}