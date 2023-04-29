using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Form.Material;
using MudSharp.Framework.Units;

namespace MudSharp.GameItems.Prototypes;

public class LanternGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Lantern";

	#region Constructors
	protected LanternGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "Lantern")
	{
		FuelPerSecond = 0.000007892522 / gameworld.UnitManager.BaseFluidToLitres;
		FuelCapacity = 0.2273046 / gameworld.UnitManager.BaseFluidToLitres;
		IlluminationProvided = 25;
		RequiresIgnitionSource = false;
		LightEmote = "@ light|lights $1";
		ExtinguishEmote = "@ extinguish|extinguishes $1";
		TenPercentFuelEcho = "$0 begin|begins to splutter as the fuel runs low";
		FuelExpendedEcho = "$0 have|has completely exhausted its fuel";
	}

	protected LanternGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		IlluminationProvided = double.Parse(root.Element("IlluminationProvided").Value);
		RequiresIgnitionSource = bool.Parse(root.Element("RequiresIgnitionSource").Value);
		LightEmote = root.Element("LightEmote").Value;
		ExtinguishEmote = root.Element("ExtinguishEmote").Value;
		TenPercentFuelEcho = root.Element("TenPercentFuelEcho").Value;
		FuelExpendedEcho = root.Element("FuelExpendedEcho").Value;
		LiquidFuel = Gameworld.Liquids.Get(long.Parse(root.Element("LiquidFuel")?.Value ?? "0"));
		FuelPerSecond = double.Parse(root.Element("FuelPerSecond").Value);
		FuelCapacity = double.Parse(root.Element("FuelCapacity").Value);
	}
	#endregion

	#region Saving
	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("LiquidFuel", LiquidFuel?.Id ?? 0),
			new XElement("FuelPerSecond", FuelPerSecond),
			new XElement("FuelCapacity", FuelCapacity),
			new XElement("IlluminationProvided", IlluminationProvided),
			new XElement("RequiresIgnitionSource", RequiresIgnitionSource),
			new XElement("LightEmote", new XCData(LightEmote)),
			new XElement("ExtinguishEmote", new XCData(ExtinguishEmote)),
			new XElement("TenPercentFuelEcho", new XCData(TenPercentFuelEcho)),
			new XElement("FuelExpendedEcho", new XCData(FuelExpendedEcho))
		).ToString();
	}
	#endregion

	public double IlluminationProvided { get; protected set; }
	public string LightEmote { get; protected set; }
	public string ExtinguishEmote { get; protected set; }
	public bool RequiresIgnitionSource { get; protected set; }
	public string TenPercentFuelEcho { get; protected set; }
	public string FuelExpendedEcho { get; protected set; }
	public ILiquid LiquidFuel { get; protected set; }
	public double FuelPerSecond { get; protected set; }
	public double FuelCapacity { get; protected set; }

	#region Component Instance Initialising Functions
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new LanternGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new LanternGameItemComponent(component, this, parent);
	}
	#endregion

	#region Initialisation Tasks
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Lantern".ToLowerInvariant(), true, (gameworld, account) => new LanternGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Lantern", (proto, gameworld) => new LanternGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Lantern",
			$"A {"[liquid container]".Colour(Telnet.BoldGreen)} that burns liquid fuel when {"[lit]".Colour(Telnet.Red)} to be a {"[light source]".Colour(Telnet.BoldPink)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new LanternGameItemComponentProto(proto, gameworld));
	}
	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:

	<name> - sets the name of the component
	desc <desc> - sets the description of the component
	illumination <lux> - the illumination in lux provided by the lantern
	ignition - toggles whether this requires an ignition source to light
	lit <emote> - the emote when this lantern is lit. Use $0 for the lightee, $1 for the lantern, and $2 for the ignition source (if applicable)
	extinguished <emote> - sets an emote for when the lantern is extinguished. $0 for the lightee, $1 for the lantern
	ten <emote> - sets an emote when the lantern reaches 10% fuel. Use $0 for the lantern item.
	zero <emote> - sets an emote when the lantern runs out of fuel. Use $0 for the lantern item.
	liquid <which> - which liquid this lantern requires
	consumption <amount> - a liquid volume of fuel consumed per hour of operation
    capacity <amount> - a liquid volume of maximum fuel capacity";
	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "lux":
			case "illumination":
				return BuildingCommand_Illumination(actor, command);
			case "ignition":
				return BuildingCommand_Ignition(actor, command);
			case "lit":
			case "light":
				return BuildingCommand_LightEmote(actor, command);
			case "extinguish":
			case "extinguished":
				return BuildingCommand_ExtinguishEmote(actor, command);
			case "tenpercent":
			case "ten":
				return BuildingCommand_TenPercent(actor, command);
			case "expended":
			case "zeropercent":
			case "zero":
				return BuildingCommand_FuelExpended(actor, command);
			case "liquid":
				return BuildingCommand_Liquid(actor, command);
			case "fuel":
			case "fuelconsumption":
			case "consumption":
				return BuildingCommand_Fuel(actor, command);
			case "capacity":
				return BuildingCommand_Capacity(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_Liquid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What liquid do you want this lantern to consume as fuel?\n{"Hint: Use the most generic form of the fuel if possible.".Colour(Telnet.Yellow)}");
			return false;
		}

		var liquid = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(command.Last);
		if (liquid == null)
		{
			actor.Send("There is no such liquid with that name or ID.");
			return false;
		}

		LiquidFuel = liquid;
		Changed = true;
		actor.Send(
			$"This lantern will now consume the {LiquidFuel.Name.Colour(LiquidFuel.DisplayColour)} liquid as fuel.");
		return true;
	}

	private bool BuildingCommand_Capacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much fuel should this lantern be able to store?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume,
			out var success);
		if (!success)
		{
			actor.Send("That is not a valid liquid quantity.");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("You must enter a positive amount of fuel capacity.");
			return false;
		}

		FuelCapacity = value;
		Changed = true;

		actor.OutputHandler.Send(
			$"This lantern will now hold {Gameworld.UnitManager.Describe(value, UnitType.FluidVolume, actor).Colour(Telnet.Green)} of fuel.");
		return true;
	}

	private bool BuildingCommand_Fuel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much fuel should this lantern use per hour of operation?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume,
			out var success);
		if (!success)
		{
			actor.Send("That is not a valid liquid quantity.");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("You must enter a positive amount of fuel usage.");
			return false;
		}

		FuelPerSecond = value / 3600;
		Changed = true;

		actor.Send(
			$"This lantern will now use {Gameworld.UnitManager.Describe(value, UnitType.FluidVolume, actor).Colour(Telnet.Green)} of fuel per hour.");
		return true;
	}

	private bool BuildingCommand_Illumination(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many lux of illumination should this lantern provide when lit?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.Send("How many lux of illumination should this lantern provide when lit?");
			return false;
		}

		if (value < 1)
		{
			actor.Send("Lanterns must provide a positive amount of illumination.");
			return false;
		}

		IlluminationProvided = value;
		Changed = true;
		actor.Send("This lantern will now provide {0:N2} lux of illumination when lit.", IlluminationProvided);
		return true;
	}

	private bool BuildingCommand_Ignition(ICharacter actor, StringStack command)
	{
		actor.Send(RequiresIgnitionSource
			? "This lantern no longer requires an independent ignition source to be lit."
			: "This lantern now requires an independent ignition source to be lit.");
		RequiresIgnitionSource = !RequiresIgnitionSource;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_LightEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for the lighting of this lantern? Use $0 for the lightee, $1 for the lantern, and $2 for the ignition source.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for lighting this lantern is now \"{0}\"", command.RemainingArgument);
		LightEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_ExtinguishEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for the extinguishing of this lantern? Use $0 for the lightee and $1 for the lantern.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for extinguishing this lantern is now \"{0}\"", command.RemainingArgument);
		ExtinguishEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_TenPercent(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for ten percent fuel being reached with this lantern? Use $0 for the lantern.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for ten percent fuel for this lantern is now \"{0}\"", command.RemainingArgument);
		TenPercentFuelEcho = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_FuelExpended(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for fuel exhaustion with this lantern? Use $0 for the lantern.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for fuel exhaustion for this lantern is now \"{0}\"", command.RemainingArgument);
		FuelExpendedEcho = command.RemainingArgument;
		Changed = true;
		return true;
	}
	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			@"{7} (#{8:N0}r{9:N0}, {10})

This is a lantern that provides {0:N2} lux of illumination when lit. It {2} require an ignition source.
It has a capacity of {11} of {12}, which it consumes at a rate of {13} per hour.

When lit, it echoes: {3}
When extinguished it echoes: {4}
At 10 percent fuel it echoes: {5}
At 0 percent fuel it echoes: {6}",
			IlluminationProvided,
			null,
			RequiresIgnitionSource ? "does" : "does not",
			LightEmote.ColourCommand(),
			ExtinguishEmote.ColourCommand(),
			TenPercentFuelEcho.ColourCommand(),
			FuelExpendedEcho.ColourCommand(),
			"Lantern Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Gameworld.UnitManager.DescribeExact(FuelCapacity, UnitType.FluidVolume, actor).Colour(Telnet.Green),
			LiquidFuel?.Name.Colour(LiquidFuel.DisplayColour) ?? "Not Set".Colour(Telnet.Red),
			Gameworld.UnitManager.DescribeExact(FuelPerSecond * 3600, UnitType.FluidVolume, actor).Colour(Telnet.Green)
		);
	}
}