using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate;

namespace MudSharp.GameItems.Prototypes;

public enum WashingMachineCycles
{
	None,
	Prewash,
	Wash,
	Rinse,
	Spin
}

public class WashingMachineGameItemComponentProto : GameItemComponentProto
{
	public double WeightCapacity { get; set; }
	public double WashingLiquidCapacity { get; set; }
	public double WashingPowderCapacity { get; set; }
	public double PowerUsageInWatts { get; set; }
	public SizeCategory MaximumItemSize { get; set; }
	public TimeSpan NormalCycleTime { get; set; }
	public bool DoorLock { get; set; }
	public bool Transparent { get; set; }

	public override string TypeDescription => "WashingMachine";

	#region Constructors

	protected WashingMachineGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "WashingMachine")
	{
		WeightCapacity = 7.5 / gameworld.UnitManager.BaseWeightToKilograms;
		WashingLiquidCapacity = 0.1 / gameworld.UnitManager.BaseFluidToLitres;
		WashingPowderCapacity = 0.1 / gameworld.UnitManager.BaseWeightToKilograms;
		PowerUsageInWatts = 500;
		MaximumItemSize = SizeCategory.Normal;
		NormalCycleTime = TimeSpan.FromMinutes(1.5);
		DoorLock = true;
		Transparent = true;
	}

	protected WashingMachineGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		WeightCapacity = double.Parse(root.Element("WeightCapacity").Value);
		WashingLiquidCapacity = double.Parse(root.Element("WashingLiquidCapacity").Value);
		WashingPowderCapacity = double.Parse(root.Element("WashingPowderCapacity").Value);
		PowerUsageInWatts = double.Parse(root.Element("PowerUsageInWatts").Value);
		MaximumItemSize = (SizeCategory)int.Parse(root.Element("MaximumItemSize").Value);
		DoorLock = bool.Parse(root.Element("DoorLock").Value);
		Transparent = bool.Parse(root.Element("Transparent").Value);
		NormalCycleTime = TimeSpan.FromSeconds(double.Parse(root.Element("NormalCycleTime").Value));
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new[]
		{
			new XElement("WeightCapacity", WeightCapacity),
			new XElement("WashingLiquidCapacity", WashingLiquidCapacity),
			new XElement("WashingPowderCapacity", WashingPowderCapacity),
			new XElement("PowerUsageInWatts", PowerUsageInWatts),
			new XElement("MaximumItemSize", (int)MaximumItemSize),
			new XElement("DoorLock", DoorLock),
			new XElement("Transparent", Transparent),
			new XElement("NormalCycleTime", NormalCycleTime.TotalSeconds)
		}).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new WashingMachineGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new WashingMachineGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("WashingMachine".ToLowerInvariant(), true,
			(gameworld, account) => new WashingMachineGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("Washing Machine".ToLowerInvariant(), false,
			(gameworld, account) => new WashingMachineGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("WashingMachine",
			(proto, gameworld) => new WashingMachineGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"WashingMachine",
			$"A {"[powered]".Colour(Telnet.Magenta)} machine that is a {"[container]".Colour(Telnet.BoldGreen)} that washes items",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new WashingMachineGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tweight <weight> - sets the maximum weight of laundry\n\tliquid <amount> - sets the maximum volume of laundry detergent\n\twatts <watts> - sets the wattage consumed when on\n\tsize <size> - sets the maximum size of item that can be washed\n\tlock - toggles whether the door locks during use\n\ttransparent - toggles whether the door is transparent\n\tcycle <time> - sets the time for the 4 cycles";

	public override string ShowBuildingHelp => BuildingHelpText;

	private bool BuildingCommandCycleTime(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What time do you want to use for the length of each of the four cycles of washing? (prewash, washing, rinse, spin)");
			return false;
		}


		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, out var result))
		{
			actor.Send("That is not a valid amount of time for the cycle time.");
			return false;
		}

		NormalCycleTime = result;
		Changed = true;
		actor.Send(
			$"The four cycles of this washing machine will now each take {NormalCycleTime.Describe().Colour(Telnet.Green)} to complete.");
		return true;
	}

	private bool BuildingCommandDoorLock(ICharacter actor, StringStack command)
	{
		DoorLock = !DoorLock;
		Changed = true;
		actor.Send(
			$"This washing machine will {(DoorLock ? "now" : "no longer")} have a door lock preventing it from being opened during the cycle.");
		return true;
	}

	private bool BuildingCommandTransparent(ICharacter actor, StringStack command)
	{
		Transparent = !Transparent;
		Changed = true;
		actor.Send($"This washing machine will {(Transparent ? "now" : "no longer")} have a transparent door.");
		return true;
	}

	private bool BuildingCommandMaximumSize(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What should be the maximum permissable size of laundry items able to be placed in this washing machine?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<SizeCategory>(out var size))
		{
			actor.OutputHandler.Send("That is not a valid item size. See SHOW ITEMSIZES for a correct list.");
			return false;
		}

		MaximumItemSize = size;
		Changed = true;
		actor.Send(
			$"This washing machine will now accept items up to size {MaximumItemSize.Describe().Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What capacity of laundry should this washing machine have?");
			return false;
		}

		var capacity = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, Framework.Units.UnitType.Mass,
			out var success);
		if (!success)
		{
			actor.Send("That is not a valid weight.");
			return false;
		}

		WeightCapacity = capacity;
		Changed = true;
		actor.Send(
			$"This washing machine will now be able to take loads up to {Gameworld.UnitManager.Describe(capacity, Framework.Units.UnitType.Mass, actor).Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandLiquidCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What capacity of washing liquid should this washing machine have?");
			return false;
		}

		var capacity = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument,
			Framework.Units.UnitType.FluidVolume, out var success);
		if (!success)
		{
			actor.Send("That is not a valid liquid quantity.");
			return false;
		}

		WashingLiquidCapacity = capacity;
		Changed = true;
		actor.Send(
			$"This washing machine will now be able to take up to {Gameworld.UnitManager.Describe(capacity, Framework.Units.UnitType.FluidVolume, actor).Colour(Telnet.Green)} of washing liquid.");
		return true;
	}

	private bool BuildingCommandWattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What should be the wattage of this appliance when it is switched on?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must specify a valid number of watts for this appliance to use.");
			return false;
		}

		PowerUsageInWatts = value;
		Changed = true;
		actor.Send(
			$"This washing machine will use {$"{PowerUsageInWatts:N2} watts".Colour(Telnet.Green)} of power when in use.");
		return true;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "weight":
			case "capacity":
				return BuildingCommandCapacity(actor, command);
			case "liquid":
			case "liquid capacity":
			case "liquid weight":
			case "detergent":
				return BuildingCommandLiquidCapacity(actor, command);
			case "watts":
			case "watt":
			case "power":
			case "wattage":
				return BuildingCommandWattage(actor, command);
			case "size":
			case "maxsize":
			case "maximum size":
			case "itemsize":
			case "item size":
			case "max size":
				return BuildingCommandMaximumSize(actor, command);
			case "transparent":
			case "see through":
			case "see thru":
				return BuildingCommandTransparent(actor, command);
			case "lock":
			case "doorlock":
			case "door lock":
				return BuildingCommandDoorLock(actor, command);
			case "cycle":
			case "time":
			case "cycle time":
			case "cycletime":
				return BuildingCommandCycleTime(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a washing machine with a capacity of {4} and maximum item size of {5}. It can takes {6} of washing liquid and {7} of washing powder. It uses {8:N2} watts of power. Its normal cycle time is {9}. It {10} a door lock. It {11} transparent.",
			"WashingMachine Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Gameworld.UnitManager.Describe(WeightCapacity, Framework.Units.UnitType.Mass, actor),
			MaximumItemSize.Describe().Colour(Telnet.Green),
			Gameworld.UnitManager.Describe(WashingLiquidCapacity, Framework.Units.UnitType.FluidVolume, actor),
			Gameworld.UnitManager.Describe(WashingPowderCapacity, Framework.Units.UnitType.Mass, actor),
			PowerUsageInWatts,
			NormalCycleTime.Describe().Colour(Telnet.Green),
			DoorLock ? "has" : "does not have",
			Transparent ? "is" : "is not"
		);
	}
}