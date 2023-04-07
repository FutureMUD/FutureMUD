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

public class GasContainerGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
	public override string TypeDescription => "GasContainer";

	public List<ConnectorType> Connections { get; } = new();

	IEnumerable<ConnectorType> IConnectableItemProto.Connections => Connections;

	public double MaximumGasCapacity { get; protected set; }

	public bool ShowGasLevels { get; protected set; }

	#region Constructors

	protected GasContainerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "GasContainer")
	{
		Connections.Add(new ConnectorType(Form.Shape.Gender.Female,
			Gameworld.GetStaticConfiguration("DefaultGasSocketType"), false));
		ShowGasLevels = true;
		MaximumGasCapacity = Gameworld.GetStaticDouble("DefaultGasContainerVolume");
		Changed = true;
	}

	protected GasContainerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
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

		MaximumGasCapacity = double.Parse(root.Element("MaximumGasCapacity")?.Value ??
		                                  Gameworld.GetStaticConfiguration("DefaultGasContainerVolume"));
		ShowGasLevels = bool.Parse(root.Element("ShowGasLevels")?.Value ?? "true");
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MaximumGasCapacity", MaximumGasCapacity),
			new XElement("ShowGasLevels", ShowGasLevels),
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
		return new GasContainerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new GasContainerGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("GasContainer".ToLowerInvariant(), true,
			(gameworld, account) => new GasContainerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("GasContainer",
			(proto, gameworld) => new GasContainerGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"GasContainer",
			$"A {"[gas container]".Colour(Telnet.BoldGreen)} that is {"[connectable]".Colour(Telnet.BoldBlue)} to other gas consumers",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new GasContainerGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tcapacity <1atmo-litres> - sets the capacity for this gas cylinder in 'litres at 1 atmosphere pressure'\n\tshow - toggles whether the gas level is shown to people looking at the container\n\ttype add <male|female|neuter> <type name> - adds a connection of the specified gender and name\n\ttype remove <male|female|neuter> <type name> - removes a connection of the specified gender and name";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "connection":
			case "connection type":
				return BuildingCommandConnectionType(actor, command);
			case "capacity":
				return BuildingCommandCapacity(actor, command);
			case "show":
			case "levels":
				return BuildingCommandShow(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandShow(ICharacter actor, StringStack command)
	{
		ShowGasLevels = !ShowGasLevels;
		Changed = true;
		actor.OutputHandler.Send(
			$"This container {(ShowGasLevels ? "now" : "no longer")} shows its current gas level.");
		return true;
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"How much gas should this gas container hold? The units are {Gameworld.UnitManager.Units.First(x => x.Type == Framework.Units.UnitType.FluidVolume && x.System.EqualTo(actor.Account.UnitPreference) && x.DefaultUnitForSystem).Name.Pluralise()} at 1 atmosphere of pressure.");
			return false;
		}

		var units = Gameworld.UnitManager.GetBaseUnits(command.PopSpeech(), Framework.Units.UnitType.FluidVolume,
			out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid fluid volume.");
			return false;
		}

		MaximumGasCapacity = units;
		actor.OutputHandler.Send(
			$"This gas container will now hold the equivalent of {Gameworld.UnitManager.DescribeMostSignificantExact(MaximumGasCapacity, Framework.Units.UnitType.FluidVolume, actor).ColourValue()} of 1-atmosphere pressure gas.");
		Changed = true;
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

		Connections.Add(new ConnectorType(gendering.Enum, type, false));
		actor.Send(
			$"This connector will now have an additional connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
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
		return Connections.Any(x => x.Gender == Form.Shape.Gender.Female) &&
		       MaximumGasCapacity > 0 &&
		       base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (Connections.All(x => x.Gender != Form.Shape.Gender.Female))
		{
			return "You must first add at least one female connector type.";
		}

		if (MaximumGasCapacity <= 0)
		{
			return "Gas containers must have a capacity greater than zero.";
		}

		return base.WhyCannotSubmit();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item holds gas - the equivalent of {4} at 1-atmosphere pressure. It {5} its current gas levels. It has these connections: {6}.",
			"GasContainer Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Gameworld.UnitManager
			         .DescribeMostSignificantExact(MaximumGasCapacity, Framework.Units.UnitType.FluidVolume, actor)
			         .ColourValue(),
			ShowGasLevels ? "shows" : "does not show",
			Connections.Select(
				           x =>
					           $"{x.ConnectionType.Colour(Telnet.Green)} {(x.Powered ? "[P]" : "")} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})")
			           .ListToString()
		);
	}
}