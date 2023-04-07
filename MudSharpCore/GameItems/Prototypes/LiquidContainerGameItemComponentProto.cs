using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.FutureProg;
using MudSharp.Form.Material;

namespace MudSharp.GameItems.Prototypes;

public class LiquidContainerGameItemComponentProto : GameItemComponentProto
{
	public double LiquidCapacity { get; set; }
	public bool Closable { get; set; }
	public bool Transparent { get; set; }
	public double WeightLimit { get; protected set; }
	public IFutureProg AdjustQuantityProg { get; protected set; }
	public bool CanBeEmptiedWhenInRoom { get; protected set; }

	/// <summary>
	///     A container that is OnceOnly can only be opened once - once opened, it can never be closed again
	/// </summary>
	public bool OnceOnly { get; protected set; }

	public ILiquid DefaultLiquid { get; protected set; }

	public override string TypeDescription => "Liquid Container";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("liquidcontainer", true,
			(gameworld, account) => new LiquidContainerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("liquid container", false,
			(gameworld, account) => new LiquidContainerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("lcon", false,
			(gameworld, account) => new LiquidContainerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("lcontainer", false,
			(gameworld, account) => new LiquidContainerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Liquid Container",
			(proto, gameworld) => new LiquidContainerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"LiquidContainer",
			$"Makes the item a {"[liquid container]".Colour(Telnet.BoldGreen)}",
			BuildingHelpText
		);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{5:N0}r{6:N0}, {7})\n\nThis item can contain {1} or {2} of liquid.\nIt {3} transparent and {4}.\n{9}.\nWhen its quantity changes, it will {8}.\nIt {9} be emptied when not being held by someone.",
			"Liquid Container Item Component".Colour(Telnet.Cyan),
			Gameworld.UnitManager.Describe(LiquidCapacity, UnitType.FluidVolume, actor),
			Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor),
			Transparent ? "is" : "is not",
			Closable
				? OnceOnly ? "can only be opened a single time" : "can be opened and closed"
				: "cannot be opened and closed",
			Id,
			RevisionNumber,
			Name,
			AdjustQuantityProg != null
				? $"execute {AdjustQuantityProg.Name} (#{AdjustQuantityProg.Id})"
				: "not execute a prog",
			DefaultLiquid == null
				? "It does not load with a default liquid"
				: $"When first loaded, it is full of {DefaultLiquid.Name.Colour(DefaultLiquid.DisplayColour)}",
			CanBeEmptiedWhenInRoom ? "can" : "cannot"
		);
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

		attr = root.Attribute("WeightLimit");
		if (attr != null)
		{
			WeightLimit = double.Parse(attr.Value);
		}

		attr = root.Attribute("OnceOnly");
		if (attr != null)
		{
			OnceOnly = bool.Parse(attr.Value);
		}

		attr = root.Attribute("AdjustQuantityProg");
		if (attr != null)
		{
			AdjustQuantityProg = Gameworld.FutureProgs.Get(long.Parse(attr.Value));
		}

		attr = root.Attribute("DefaultLiquid");
		if (attr != null)
		{
			DefaultLiquid = Gameworld.Liquids.Get(long.Parse(attr.Value));
		}

		attr = root.Attribute("CanBeEmptiedWhenInRoom");
		CanBeEmptiedWhenInRoom = bool.Parse(attr?.Value ?? "true");
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("LiquidCapacity", LiquidCapacity),
					new XAttribute("Closable", Closable), new XAttribute("Transparent", Transparent),
					new XAttribute("WeightLimit", WeightLimit), new XAttribute("OnceOnly", OnceOnly),
					new XAttribute("AdjustQuantityProg", AdjustQuantityProg?.Id ?? 0),
					new XAttribute("DefaultLiquid", DefaultLiquid?.Id ?? 0),
					new XAttribute("CanBeEmptiedWhenInRoom", CanBeEmptiedWhenInRoom)
				)
				.ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new LiquidContainerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new LiquidContainerGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new LiquidContainerGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	close - toggles whether this container opens and closes
	capacity <amount> - sets the amount of liquid this container can hold
	weight - sets the maximum weight of liquid this container can hold
	transparent - toggles whether you can see the liquid inside when closed
	once - toggles whether this container only opens once
	defaultliquid <liquid>|none - sets or clears a liquid loaded with the item
	adjustquantityprog <prog> - Prog to execute anytime quantity changes
	emptyroom - toggles whether it can be emptied when in the room (i.e. not held)";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "close":
			case "openable":
			case "closable":
				return BuildingCommand_Closable(actor, command);
			case "capacity":
			case "liquidcapacity":
				return BuildingCommand_Capacity(actor, command);
			case "weight":
			case "weightlimit":
			case "weightcapacity":
			case "limit":
				return BuildingCommand_WeightLimit(actor, command);
			case "once":
			case "onceonly":
				return BuildingCommand_OnceOnly(actor, command);
			case "transparent":
				return BuildingCommand_Transparent(actor, command);
			case "adjustquantityprog":
				return BuildingCommand_AdjustQuantityProg(actor, command);
			case "liquid":
			case "defaultliquid":
				return BuildingCommand_DefaultLiquid(actor, command);
			case "emptyroom":
				return BuildingCommand_EmptyRoom(actor);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	#region Building Command SubCommands

	private bool BuildingCommand_DefaultLiquid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a liquid to load, or use the keyword NONE to clear any default liquid.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "delete", "remove"))
		{
			DefaultLiquid = null;
			Changed = true;
			actor.OutputHandler.Send("This container will no longer load with a default liquid.");
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
			$"This container will now load up full of {liquid.Name.Colour(liquid.DisplayColour)}.");
		return true;
	}

	public bool BuildingCommand_OnceOnly(ICharacter actor, StringStack command)
	{
		OnceOnly = !OnceOnly;
		actor.OutputHandler.Send("This container is " + (OnceOnly ? "now" : "no longer") +
		                         " openable only a single time.");
		Changed = true;
		return true;
	}

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

	private bool BuildingCommand_WeightLimit(ICharacter actor, StringStack command)
	{
		var weightCmd = command.SafeRemainingArgument;
		var result = actor.Gameworld.UnitManager.GetBaseUnits(weightCmd, UnitType.Mass, out var success);
		if (success)
		{
			WeightLimit = result;
			Changed = true;
			actor.OutputHandler.Send(
				$"This liquid container will now hold {actor.Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor).ColourValue()}.");
			return true;
		}

		actor.OutputHandler.Send("That is not a valid weight.");
		return false;
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

	private bool BuildingCommand_AdjustQuantityProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (AdjustQuantityProg != null)
			{
				AdjustQuantityProg = null;
				Changed = true;
				actor.Send("There will no longer be any prog executed when the quantity changes.");
				return true;
			}

			actor.Send("Which prog did you want to execute when the quantity changes?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);

		if (prog == null)
		{
			actor.Send("There is no prog with that name or iD.");
			return false;
		}

		if (!prog.MatchesParameters(new[]
		    {
			    FutureProgVariableTypes.Item, FutureProgVariableTypes.Number,
			    FutureProgVariableTypes.Number, FutureProgVariableTypes.Character,
			    FutureProgVariableTypes.Text
		    }))
		{
			actor.Send(
				"The parameter list must be (item, number, number, character, text) for (container, amount, finalQuantity, who, action.");
			return false;
		}

		AdjustQuantityProg = prog;
		actor.Send(
			$"This container will now execute prog {prog.FunctionName} (#{prog.Id}) when its contents are adjusted.");
		Changed = true;
		return true;
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

	#endregion

	#region Constructors

	protected LiquidContainerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected LiquidContainerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Liquid Container")
	{
		LiquidCapacity = 1.0 / gameworld.UnitManager.BaseFluidToLitres;
		WeightLimit = 1.0 / gameworld.UnitManager.BaseWeightToKilograms;
		Transparent = false;
		Closable = false;
		Changed = true;
	}

	#endregion
}