using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class WaterSourceGameItemComponentProto : GameItemComponentProto
{
	public double LiquidCapacity { get; set; }
	public bool Closable { get; set; }
	public bool Transparent { get; set; }
	public bool OnceOnly { get; protected set; }
	public ILiquid DefaultLiquid { get; protected set; }
	public double RefillRate { get; protected set; }
	public IFutureProg RefillingProg { get; protected set; }
	public bool UseOnOffForRefill { get; protected set; }
	public bool CanBeEmptiedWhenInRoom { get; protected set; }

	public override string TypeDescription => "WaterSource";

	public override bool CanSubmit()
	{
		if (DefaultLiquid == null)
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (DefaultLiquid == null)
		{
			return "You must set a default liquid for a water source component.";
		}

		return base.WhyCannotSubmit();
	}

	#region Constructors

	protected WaterSourceGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"WaterSource")
	{
		LiquidCapacity = 1.0 / gameworld.UnitManager.BaseFluidToLitres;
		RefillRate = 0.05 / gameworld.UnitManager.BaseFluidToLitres;
		Transparent = false;
		Closable = false;
		Changed = true;
	}

	protected WaterSourceGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto,
		gameworld)
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

		attr = root.Attribute("OnceOnly");
		if (attr != null)
		{
			OnceOnly = bool.Parse(attr.Value);
		}

		attr = root.Attribute("DefaultLiquid");
		if (attr != null)
		{
			DefaultLiquid = Gameworld.Liquids.Get(long.Parse(attr.Value));
		}

		attr = root.Attribute("RefillRate");
		if (attr != null)
		{
			RefillRate = double.Parse(attr.Value);
		}

		attr = root.Attribute("UseOnOffForRefill");
		if (attr != null)
		{
			UseOnOffForRefill = bool.Parse(attr.Value);
		}

		attr = root.Attribute("RefillingProg");
		if (attr != null)
		{
			RefillingProg = Gameworld.FutureProgs.Get(long.Parse(attr.Value));
		}

		attr = root.Attribute("CanBeEmptiedWhenInRoom");
		CanBeEmptiedWhenInRoom = bool.Parse(attr?.Value ?? "true");
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
					new XAttribute("LiquidCapacity", LiquidCapacity),
					new XAttribute("Closable", Closable),
					new XAttribute("Transparent", Transparent),
					new XAttribute("OnceOnly", OnceOnly),
					new XAttribute("DefaultLiquid", DefaultLiquid?.Id ?? 0),
					new XAttribute("RefillRate", RefillRate),
					new XAttribute("UseOnOffForRefill", UseOnOffForRefill),
					new XAttribute("RefillingProg", RefillingProg?.Id ?? 0),
					new XAttribute("CanBeEmptiedWhenInRoom", CanBeEmptiedWhenInRoom)
				)
				.ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new WaterSourceGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new WaterSourceGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("WaterSource".ToLowerInvariant(), true,
			(gameworld, account) => new WaterSourceGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("WaterSource",
			(proto, gameworld) => new WaterSourceGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"WaterSource",
			$"Makes the item a self-replenishing {"[liquid container]".Colour(Telnet.BoldGreen)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new WaterSourceGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:

	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	close - toggles whether this container opens and closes
	capacity <amount> - sets the amount of liquid this container can hold
	transparent - toggles whether you can see the liquid inside when closed
	defaultliquid <liquid> - sets the liquid loaded with the item and refilled on each tick
	adjustquantityprog <prog> - Prog to execute anytime quantity changes
	refillrate <amount> - sets the refill rate per minute
	onoff - toggles using an on/off function for refilling
	prog <prog>|clear - sets or clears a prog for determing whether to refill. Incompatible with OnOff option.
	emptyroom - toggles whether it can be emptied when in the room (i.e. not held)";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "close":
			case "openable":
			case "closable":
				return BuildingCommand_Closable(actor, command);
			case "capacity":
			case "liquidcapacity":
				return BuildingCommand_Capacity(actor, command);
			case "once":
			case "onceonly":
				return BuildingCommand_OnceOnly(actor, command);
			case "transparent":
				return BuildingCommand_Transparent(actor, command);
			case "defaultliquid":
			case "liquid":
				return BuildingCommand_DefaultLiquid(actor, command);
			case "refill":
			case "rate":
			case "refillrate":
				return BuildingCommand_RefillRate(actor, command);
			case "onoff":
				return BuildingCommand_OnOff(actor, command);
			case "prog":
				return BuildingCommand_Prog(actor, command);
			case "emptyroom":
				return BuildingCommand_EmptyRoom(actor);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_EmptyRoom(ICharacter actor)
	{
		CanBeEmptiedWhenInRoom = !CanBeEmptiedWhenInRoom;
		Changed = true;
		if (CanBeEmptiedWhenInRoom)
		{
			actor.OutputHandler.Send(
				"This container can now be emptied when it is in the room, not requiring that the person be holding it in order to empty it.");
		}
		else
		{
			actor.OutputHandler.Send(
				"This container can no longer be emptied when it is in the room, instead requiring that the person be holding it in order to empty it.");
		}

		return true;
	}

	#region Building Command SubCommands

	private bool BuildingCommand_RefillRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a refill rate per minute as a liquid quantity.");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume, actor,
				out var value))
		{
			actor.OutputHandler.Send(
				$"The value {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of liquid.");
			return false;
		}

		RefillRate = value / 12.0;
		Changed = true;
		actor.OutputHandler.Send(
			$"This water source will now refill at a rate of {Gameworld.UnitManager.DescribeExact(value, UnitType.FluidVolume, actor).ColourValue()} per minute.");
		return true;
	}

	private bool BuildingCommand_Prog(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.PeekSpeech().EqualToAny("clear", "none", "delete", "remove"))
		{
			if (UseOnOffForRefill)
			{
				actor.OutputHandler.Send("This water source already does not use a prog to control refilling.");
				return false;
			}

			RefillingProg = null;
			Changed = true;
			actor.OutputHandler.Send(
				"This water source will no longer use a prog to determine whether the refilling effect is active, and will instead always be on.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send($"There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Item }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single item as a parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		RefillingProg = prog;
		UseOnOffForRefill = false;
		Changed = true;
		actor.OutputHandler.Send(
			$"This water source will now use the prog {prog.MXPClickableFunctionName()} to determine whether the refilling effect is active.");
		return true;
	}

	private bool BuildingCommand_OnOff(ICharacter actor, StringStack command)
	{
		UseOnOffForRefill = !UseOnOffForRefill;
		if (UseOnOffForRefill)
		{
			RefillingProg = null;
		}

		actor.OutputHandler.Send(
			$"This water source will {(UseOnOffForRefill ? "now" : "no longer")} use an on/off switch mechanism to control refilling.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_DefaultLiquid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a liquid to load.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "delete", "remove"))
		{
			DefaultLiquid = null;
			Changed = true;
			actor.OutputHandler.Send("Water sources must always have a default liquid set.");
			return false;
		}

		var liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
		if (liquid == null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return false;
		}

		DefaultLiquid = liquid;
		Changed = true;
		actor.OutputHandler.Send(
			$"This container will now load up full of {liquid.Name.Colour(liquid.DisplayColour)}, and also replenish with this liquid.");
		return true;
	}

	public bool BuildingCommand_OnceOnly(ICharacter actor, StringStack command)
	{
		OnceOnly = !OnceOnly;
		actor.OutputHandler.Send(
			$"This container is {(OnceOnly ? "now" : "no longer")} openable only a single time.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Transparent(ICharacter actor, StringStack command)
	{
		Transparent = !Transparent;
		actor.OutputHandler.Send($"This liquid container is {(Transparent ? "now" : "no longer")} transparent.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Closable(ICharacter actor, StringStack command)
	{
		Closable = !Closable;
		actor.OutputHandler.Send($"This liquid container is {(Closable ? "now" : "no longer")} closable.");
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

	#endregion

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis is a water container that replenishes itself over time.\nIt can contain {4} of liquid.\nIt replenishes {5} per minute of {6}.\nIt {7} transparent and {8}.\n{9}.\nIt {10} be emptied when not being held by someone.",
			"WaterSource Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Gameworld.UnitManager.Describe(LiquidCapacity, UnitType.FluidVolume, actor).ColourValue(),
			Gameworld.UnitManager.Describe(RefillRate * 12.0, UnitType.FluidVolume, actor).ColourValue(),
			DefaultLiquid == null
				? "an unspecified liquid".Colour(Telnet.Red)
				: DefaultLiquid.Name.Colour(DefaultLiquid.DisplayColour),
			Transparent ? "is" : "is not",
			Closable
				? OnceOnly ? "can only be opened a single time" : "can be opened and closed"
				: "cannot be opened and closed",
			UseOnOffForRefill
				? "It uses the on/off mechanism for flow control"
				: RefillingProg != null
					? $"It uses the prog {RefillingProg.MXPClickableFunctionName()} to determine whether it is refilling"
					: "It is always on",
			CanBeEmptiedWhenInRoom ? "can" : "cannot"
		);
	}
}