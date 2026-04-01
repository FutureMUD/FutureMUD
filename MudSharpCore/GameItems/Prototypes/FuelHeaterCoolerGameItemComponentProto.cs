#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public enum FuelHeaterCoolerFuelMedium
{
	Liquid = 0,
	Gas = 1
}

public class FuelHeaterCoolerGameItemComponentProto : SwitchableThermalSourceGameItemComponentProto, IConnectableItemProto
{
	protected FuelHeaterCoolerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "FuelHeaterCooler")
	{
		FuelMedium = FuelHeaterCoolerFuelMedium.Liquid;
		FuelPerSecond = 0.01;
		Connector = new ConnectorType(Form.Shape.Gender.Male, "LiquidLine", false);
		Changed = true;
	}

	protected FuelHeaterCoolerGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	public override string TypeDescription => "FuelHeaterCooler";
	public FuelHeaterCoolerFuelMedium FuelMedium { get; protected set; }
	public ILiquid? LiquidFuel { get; protected set; }
	public IGas? GasFuel { get; protected set; }
	public double FuelPerSecond { get; protected set; }
	public ConnectorType Connector { get; protected set; }
	IEnumerable<ConnectorType> IConnectableItemProto.Connections => [Connector];

	protected override void LoadFromXml(XElement root)
	{
		LoadSwitchableThermalDefinitionFromXml(root);
		FuelMedium = (FuelHeaterCoolerFuelMedium)int.Parse(root.Element("FuelMedium")?.Value ?? "0");
		LiquidFuel = Gameworld.Liquids.Get(long.Parse(root.Element("LiquidFuel")?.Value ?? "0"));
		GasFuel = Gameworld.Gases.Get(long.Parse(root.Element("GasFuel")?.Value ?? "0"));
		FuelPerSecond = double.Parse(root.Element("FuelPerSecond")?.Value ?? "0.01");
		var connection = root.Element("Connector");
		Connector = connection is null
			? new ConnectorType(Form.Shape.Gender.Male,
				FuelMedium == FuelHeaterCoolerFuelMedium.Gas ? Gameworld.GetStaticConfiguration("DefaultGasSocketType") : "LiquidLine",
				false)
			: new ConnectorType((Gender)short.Parse(connection.Attribute("gender")!.Value),
				connection.Attribute("type")!.Value,
				bool.Parse(connection.Attribute("powered")?.Value ?? "false"));
	}

	protected override string SaveToXml()
	{
		return SaveSwitchableThermalDefinitionToXml(new XElement("Definition",
			new XElement("FuelMedium", (int)FuelMedium),
			new XElement("LiquidFuel", LiquidFuel?.Id ?? 0),
			new XElement("GasFuel", GasFuel?.Id ?? 0),
			new XElement("FuelPerSecond", FuelPerSecond),
			new XElement("Connector",
				new XAttribute("gender", (short)Connector.Gender),
				new XAttribute("type", Connector.ConnectionType),
				new XAttribute("powered", Connector.Powered)))).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new FuelHeaterCoolerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new FuelHeaterCoolerGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("fuelheatercooler", true,
			(gameworld, account) => new FuelHeaterCoolerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("fuel heater cooler", false,
			(gameworld, account) => new FuelHeaterCoolerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("FuelHeaterCooler",
			(proto, gameworld) => new FuelHeaterCoolerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"FuelHeaterCooler",
			$"A connectable thermal source that consumes either a configured liquid or gas fuel",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new FuelHeaterCoolerGameItemComponentProto(proto, gameworld));
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "ambient":
			case "intimate":
			case "immediate":
			case "proximate":
			case "proximity":
			case "distant":
			case "verydistant":
			case "very-distant":
			case "very_distant":
				return BuildingCommandThermalProfile(actor, command.Last, command);
			case "activedesc":
				return BuildingCommandStateDescription(actor, true, command);
			case "inactivedesc":
				return BuildingCommandStateDescription(actor, false, command);
			case "onemote":
				return BuildingCommandSwitchEmote(actor, true, command);
			case "offemote":
				return BuildingCommandSwitchEmote(actor, false, command);
			case "medium":
				return BuildingCommandMedium(actor, command);
			case "fuel":
				return BuildingCommandFuel(actor, command);
			case "rate":
			case "burn":
			case "burnrate":
				return BuildingCommandRate(actor, command);
			case "connection":
			case "connector":
				return BuildingCommandConnection(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandMedium(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Should this consume liquid fuel or gas fuel?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "liquid":
				FuelMedium = FuelHeaterCoolerFuelMedium.Liquid;
				Connector = new ConnectorType(Form.Shape.Gender.Male, "LiquidLine", false);
				break;
			case "gas":
				FuelMedium = FuelHeaterCoolerFuelMedium.Gas;
				Connector = new ConnectorType(Form.Shape.Gender.Male, Gameworld.GetStaticConfiguration("DefaultGasSocketType"), false);
				break;
			default:
				actor.OutputHandler.Send("You must choose either liquid or gas.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This heater/cooler now consumes {FuelMedium.DescribeEnum().ColourName()} fuel.");
		return true;
	}

	private bool BuildingCommandFuel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which {(FuelMedium == FuelHeaterCoolerFuelMedium.Liquid ? "liquid" : "gas")} should this consume?");
			return false;
		}

		if (FuelMedium == FuelHeaterCoolerFuelMedium.Liquid)
		{
			var liquid = long.TryParse(command.SafeRemainingArgument, out var value)
				? Gameworld.Liquids.Get(value)
				: Gameworld.Liquids.GetByName(command.SafeRemainingArgument);
			if (liquid is null)
			{
				actor.OutputHandler.Send("There is no such liquid.");
				return false;
			}

			LiquidFuel = liquid;
			Changed = true;
			actor.OutputHandler.Send($"This heater/cooler now consumes {liquid.Name.ColourName()}.");
			return true;
		}

		var gas = long.TryParse(command.SafeRemainingArgument, out var gasValue)
			? Gameworld.Gases.Get(gasValue)
			: Gameworld.Gases.GetByName(command.SafeRemainingArgument);
		if (gas is null)
		{
			actor.OutputHandler.Send("There is no such gas.");
			return false;
		}

		GasFuel = gas;
		Changed = true;
		actor.OutputHandler.Send($"This heater/cooler now consumes {gas.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much fuel should this consume per second?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive fuel rate.");
			return false;
		}

		FuelPerSecond = value;
		Changed = true;
		actor.OutputHandler.Send($"This heater/cooler now consumes {FuelPerSecond.ToString("N4", actor).ColourValue()} units of fuel per second.");
		return true;
	}

	private bool BuildingCommandConnection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a gender and a connection type.");
			return false;
		}

		var genderText = command.PopSpeech();
		var gender = Gendering.Get(genderText);
		if (gender is null)
		{
			actor.OutputHandler.Send("That is not a valid connector gender.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What connection type should it use?");
			return false;
		}

		Connector = new ConnectorType(gender.Enum, command.SafeRemainingArgument, false);
		Changed = true;
		actor.OutputHandler.Send(
			$"This heater/cooler now uses a {Gendering.Get(Connector.Gender).GenderClass(true).ColourName()} {Connector.ConnectionType.ColourName()} connector.");
		return true;
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tmedium <liquid|gas> - sets what kind of fuel it consumes\n\tfuel <fuel> - sets the specific liquid or gas that it burns\n\trate <amount> - sets the fuel use per second\n\tconnection <gender> <type> - sets the single connector type" +
		SwitchableThermalBuildingHelpText;

	public override string ShowBuildingHelp => BuildingHelpText;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		var fuelText = FuelMedium == FuelHeaterCoolerFuelMedium.Liquid
			? LiquidFuel?.Name.ColourName() ?? "None".ColourError()
			: GasFuel?.Name.ColourName() ?? "None".ColourError();
		return
			$"{"Fuel Heater/Cooler Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nThis is a {FuelMedium.DescribeEnum().ColourName()} fuel thermal source consuming {FuelPerSecond.ToString("N4", actor).ColourValue()} per second of {fuelText}.\nThermal Profile: {ThermalProfileDisplay(actor)}\nConnector: {Connector.ConnectionType.ColourName()} ({Gendering.Get(Connector.Gender).GenderClass(true).ColourName()})";
	}
}
