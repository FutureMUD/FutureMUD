using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class FuelGeneratorGameItemComponentProto : GameItemComponentProto
{
	protected FuelGeneratorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Fuel Generator")
	{
		Changed = true;
		FuelPerSecond = 0.00168241 / gameworld.UnitManager.BaseFluidToLitres;
		WattageProvided = 20000;
		SwitchOnEmote = "@ switch|switches $1 on";
		SwitchOffEmote = "@ switch|switches $1 off";
		FuelExpendedEmote = "@ run|runs out of fuel and turn|turns off";
		FuelCapacity = 50 / gameworld.UnitManager.BaseFluidToLitres;
	}

	protected FuelGeneratorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public ILiquid LiquidFuel { get; protected set; }
	public double FuelPerSecond { get; protected set; }
	public double WattageProvided { get; protected set; }
	public string SwitchOnEmote { get; protected set; }
	public string SwitchOffEmote { get; protected set; }
	public string FuelExpendedEmote { get; protected set; }
	public double FuelCapacity { get; protected set; }
	public IFutureProg SwitchOnProg { get; protected set; }
	public IFutureProg SwitchOffProg { get; protected set; }
	public IFutureProg FuelOutProg { get; protected set; }

	public override string TypeDescription => "Fuel Generator";

	protected override void LoadFromXml(XElement root)
	{
		SwitchOnEmote = root.Element("SwitchOnEmote").Value;
		SwitchOffEmote = root.Element("SwitchOffEmote").Value;
		FuelExpendedEmote = root.Element("FuelExpendedEmote").Value;
		FuelPerSecond = double.Parse(root.Element("FuelPerSecond").Value);
		FuelCapacity = double.Parse(root.Element("FuelCapacity").Value);
		WattageProvided = double.Parse(root.Element("WattageProvided").Value);
		SwitchOnProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("SwitchOnProg").Value));
		SwitchOffProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("SwitchOffProg").Value));
		FuelOutProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("FuelOutProg").Value));
		LiquidFuel = Gameworld.Liquids.Get(long.Parse(root.Element("LiquidFuel")?.Value ?? "0"));
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", 
				new XElement("SwitchOnEmote", new XCData(SwitchOnEmote)),
				new XElement("SwitchOffEmote", new XCData(SwitchOffEmote)),
				new XElement("FuelExpendedEmote", new XCData(FuelExpendedEmote)),
				new XElement("FuelPerSecond", FuelPerSecond),
				new XElement("FuelCapacity", FuelCapacity),
				new XElement("WattageProvided", WattageProvided),
				new XElement("SwitchOnProg", SwitchOnProg?.Id ?? 0),
				new XElement("SwitchOffProg", SwitchOffProg?.Id ?? 0),
				new XElement("FuelOutProg", FuelOutProg?.Id ?? 0),
				new XElement("LiquidFuel", LiquidFuel?.Id ?? 0)
			).ToString();
	}

	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	liquid <which> - which liquid this generator requires
	consumption <amount> - a liquid volume of fuel consumed per hour of operation
    capacity <amount> - a liquid volume of maximum fuel capacity
    watts <watts> - how many watts supplied when operating
    onemote <emote> - sets the echo when switched on. $0 for the person turning it on, $1 for the generator item.
    offemote <emote> - sets the echo when switched off. $0 for the person turning it off, $1 for the generator item.
    fuelemote <emote> - sets the echo when it runs out of fuel. $0 for the generator item.
    onprog <prog> - sets a prog to be executed when the generator switches on
    offprog <prog> - sets a prog to be executed when the generator switches off
    fuelprog <prog> - sets a prog to be executed when the generator runs out of fuel";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "liquid":
				return BuildingCommand_Liquid(actor, command);
			case "fuel":
			case "fuelconsumption":
			case "consumption":
				return BuildingCommand_Fuel(actor, command);
			case "capacity":
				return BuildingCommand_Capacity(actor, command);
			case "onemote":
				return BuildingCommand_OnEmote(actor, command);
			case "onprog":
				return BuildingCommand_OnProg(actor, command);
			case "offemote":
				return BuildingCommand_OffEmote(actor, command);
			case "offprog":
				return BuildingCommand_OffProg(actor, command);
			case "wattage":
			case "watts":
			case "watt":
			case "power":
				return BuildingCommand_Wattage(actor, command);
			case "fuelemote":
				return BuildingCommand_FuelEmote(actor, command);
			case "fuelprog":
				return BuildingCommand_FuelProg(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_Liquid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What liquid do you want this generator to consume as fuel?\n{"Hint: Use the most generic form of the fuel if possible.".Colour(Telnet.Yellow)}");
			return false;
		}

		var liquid = long.TryParse(command.SafeRemainingArgument, out var value)
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
			$"This generator will now consume the {LiquidFuel.Name.Colour(LiquidFuel.DisplayColour)} liquid as fuel.");
		return true;
	}

	private bool BuildingCommand_FuelProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (FuelOutProg != null)
			{
				FuelOutProg = null;
				Changed = true;
				actor.Send("There will no longer be any prog executed when this generator runs out of fuel.");
				return true;
			}

			actor.Send("Which prog do you want to execute when this generator runs out of fuel?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.Send("There is no such prog by that name or ID.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Item }))
		{
			actor.Send("The prog must take only a single item as a parameter.");
			return false;
		}

		FuelOutProg = prog;
		actor.Send(
			$"This generator will now execute prog {prog.FunctionName} (#{prog.Id}) when it runs out of fuel.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_FuelEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for the fuel out of this? Use $0 for the generator.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for this generator running out of fuel is now \"{0}\"", command.RemainingArgument);
		FuelExpendedEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Wattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many watts should this generator produce when switched on?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.Send("You must enter a valid number of watts.");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("You must enter a positive number of watts for this generator to produce.");
			return false;
		}

		WattageProvided = value;
		actor.Send($"This generator now produces {WattageProvided:N2} watts of power.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_OffProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (SwitchOffProg != null)
			{
				SwitchOffProg = null;
				Changed = true;
				actor.Send("There will no longer be any prog executed when this generator is switched off.");
				return true;
			}

			actor.Send("Which prog do you want to execute when this generator is switched off?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.Send("There is no such prog by that name or ID.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item }))
		{
			actor.Send("The prog must take only a single character and item as a parameter.");
			return false;
		}

		SwitchOffProg = prog;
		actor.Send($"This generator will now execute prog {prog.FunctionName} (#{prog.Id}) when it is switched off.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_OffEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for the turning off of this generator? Use $0 for the lightee, $1 for the generator.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for turning this generator off is now \"{0}\"", command.RemainingArgument);
		SwitchOffEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_OnProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (SwitchOnProg != null)
			{
				SwitchOnProg = null;
				Changed = true;
				actor.Send("There will no longer be any prog executed when this generator is switched on.");
				return true;
			}

			actor.Send("Which prog do you want to execute when this generator is switched on?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.Send("There is no such prog by that name or ID.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item }))
		{
			actor.Send("The prog must take only a single character and item as a parameter.");
			return false;
		}

		SwitchOnProg = prog;
		actor.Send($"This generator will now execute prog {prog.FunctionName} (#{prog.Id}) when it is switched on.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_OnEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for the turning on of this generator? Use $0 for the lightee, $1 for the generator.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for turning this generator on is now \"{0}\"", command.RemainingArgument);
		SwitchOnEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Capacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much fuel should this generator be able to store?");
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

		actor.Send(
			$"This generator will now hold {Gameworld.UnitManager.Describe(value, UnitType.FluidVolume, actor).Colour(Telnet.Green)} of fuel.");
		return true;
	}

	private bool BuildingCommand_Fuel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much fuel should this generator use per hour at peak capacity?");
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
			$"This generator will now use {Gameworld.UnitManager.Describe(value, UnitType.FluidVolume, actor).Colour(Telnet.Green)} of fuel per hour.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$@"{"Fuel Generator Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})

This is a generator that consumes liquid fuel. It has a capacity of {Gameworld.UnitManager.DescribeExact(FuelCapacity, UnitType.FluidVolume, actor).Colour(Telnet.Green)} of {LiquidFuel?.Name.Colour(LiquidFuel.DisplayColour) ?? "Not Set".Colour(Telnet.Red)}, which it consumes at a rate of {Gameworld.UnitManager.DescribeExact(FuelPerSecond * 3600, UnitType.FluidVolume, actor).Colour(Telnet.Green)} per hour. It produces {WattageProvided:N2} watts of power.

Switch On Emote: {SwitchOnEmote}
Switch Off Emote: {SwitchOffEmote}
Fuel Out Emote: {FuelExpendedEmote}
Switch On Prog: {(SwitchOnProg != null ? SwitchOnProg.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {SwitchOnProg.Id}'") : "None".Colour(Telnet.Red))}
Switch Off Prog: {(SwitchOffProg != null ? SwitchOffProg.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {SwitchOffProg.Id}'") : "None".Colour(Telnet.Red))}
Fuel Out Prog: {(FuelOutProg != null ? FuelOutProg.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {FuelOutProg.Id}'") : "None".Colour(Telnet.Red))}";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("fuel generator", true,
			(gameworld, account) => new FuelGeneratorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("fuelgenerator", false,
			(gameworld, account) => new FuelGeneratorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("fuel_generator", false,
			(gameworld, account) => new FuelGeneratorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("fuelgen", false,
			(gameworld, account) => new FuelGeneratorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("fuel_gen", false,
			(gameworld, account) => new FuelGeneratorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("fgen", false,
			(gameworld, account) => new FuelGeneratorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("fgenerator", false,
			(gameworld, account) => new FuelGeneratorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Fuel Generator",
			(proto, gameworld) => new FuelGeneratorGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"FuelGenerator",
			$"A {"[liquid container]".Colour(Telnet.BoldGreen)} that {"[produces power]".Colour(Telnet.BoldMagenta)} by burning its liquid",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new FuelGeneratorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new FuelGeneratorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new FuelGeneratorGameItemComponentProto(proto, gameworld));
	}
}