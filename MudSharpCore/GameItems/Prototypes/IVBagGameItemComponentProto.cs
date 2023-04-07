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
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Prototypes;

public class IVBagGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "IVBag";

	public double LiquidCapacity { get; set; }
	public bool Closable { get; set; }
	public bool Transparent { get; set; }

	public List<ConnectorType> Connections { get; } = new();

	#region Constructors

	protected IVBagGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"IVBag")
	{
		LiquidCapacity = 1.0 / gameworld.UnitManager.BaseFluidToLitres;
		Transparent = true;
		Closable = true;
		Changed = true;
		Connections.Add(new ConnectorType(Form.Shape.Gender.Male, "cannula", false));
	}

	protected IVBagGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("LiquidCapacity");
		if (attr != null)
		{
			LiquidCapacity = double.Parse(attr.Value);
		}

		attr = root.Attribute("Closable");
		if (attr != null)
		{
			Closable = bool.Parse(attr.Value);
		}

		attr = root.Attribute("Transparent");
		if (attr != null)
		{
			Transparent = bool.Parse(attr.Value);
		}

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
			new XElement("Definition", new XAttribute("LiquidCapacity", LiquidCapacity),
					new XAttribute("Closable", Closable), new XAttribute("Transparent", Transparent),
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
		return new IVBagGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new IVBagGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("IVBag".ToLowerInvariant(), true,
			(gameworld, account) => new IVBagGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("IVBag", (proto, gameworld) => new IVBagGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"IVBag",
			$"A type of {"[liquid container]".Colour(Telnet.BoldGreen)} that is {"[connectable]".Colour(Telnet.BoldBlue)} to a cannula to drain/supply blood",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new IVBagGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tclose - toggles whether this container opens and closes\n\tcapacity <amount> - sets the amount of liquid this container can hold\n\ttransparent - toggles whether you can see the liquid inside when closed\n\ttype add <male|female|neuter> <type name> - adds a connection of the specified gender and name\n\ttype remove <male|female|neuter> <type name> - removes a connection of the specified gender and name";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "close":
			case "openable":
			case "closable":
				return BuildingCommand_Closable(actor, command);
			case "capacity":
			case "liquid capacity":
			case "liquid":
				return BuildingCommand_Capacity(actor, command);
			case "transparent":
				return BuildingCommand_Transparent(actor, command);
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

	private bool BuildingCommand_Transparent(ICharacter actor, StringStack command)
	{
		Transparent = !Transparent;
		actor.Send("This liquid container is {0} transparent.", Transparent ? "now" : "no longer");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Closable(ICharacter actor, StringStack command)
	{
		Closable = !Closable;
		actor.OutputHandler.Send("This liquid container is " + (Closable ? "now" : "no longer") + " closable.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Capacity(ICharacter actor, StringStack command)
	{
		var volumeCmd = command.SafeRemainingArgument;
		var result = actor.Gameworld.UnitManager.GetBaseUnits(volumeCmd, UnitType.FluidVolume, out var success);
		if (success)
		{
			LiquidCapacity = result;
			Changed = true;
			actor.OutputHandler.Send(
				$"This liquid container will now hold {actor.Gameworld.UnitManager.Describe(LiquidCapacity, UnitType.FluidVolume, actor).ColourValue()}.");
			return true;
		}

		actor.OutputHandler.Send("That is not a valid fluid volume.");
		return false;
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
			actor.Send("What type of connection do you want this IV Bag to be?");
			return false;
		}

		var type = command.SafeRemainingArgument;
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
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is an IV Bag and can contain {4} of liquid. It {5} transparent and {6}. It has and has the following connections: {7}",
			"IVBag Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Gameworld.UnitManager.Describe(LiquidCapacity, UnitType.FluidVolume, actor),
			Transparent ? "is" : "is not",
			Closable
				? "can be opened and closed"
				: "cannot be opened and closed",
			Connections.Select(x =>
				           $"{x.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})")
			           .ListToString()
		);
	}
}