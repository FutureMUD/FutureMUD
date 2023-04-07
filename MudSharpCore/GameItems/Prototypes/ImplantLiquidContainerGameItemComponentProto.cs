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
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class ImplantLiquidContainerGameItemComponentProto : ImplantBaseGameItemComponentProto
{
	public override string TypeDescription => "ImplantLiquidContainer";

	#region Constructors

	protected ImplantLiquidContainerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ImplantLiquidContainer")
	{
		LiquidCapacity = 1.0 / gameworld.UnitManager.BaseFluidToLitres;
		WeightLimit = 1.0 / gameworld.UnitManager.BaseWeightToKilograms;
		Transparent = false;
		Closable = true;
		Changed = true;
	}

	protected ImplantLiquidContainerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
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

		attr = root.Attribute("AdjustQuantityProg");
		if (attr != null)
		{
			AdjustQuantityProg = Gameworld.FutureProgs.Get(long.Parse(attr.Value));
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return SaveToXmlWithoutConvertingToString(new XAttribute("LiquidCapacity", LiquidCapacity),
			new XAttribute("Closable", Closable), new XAttribute("Transparent", Transparent),
			new XAttribute("WeightLimit", WeightLimit),
			new XAttribute("AdjustQuantityProg", AdjustQuantityProg?.Id ?? 0)).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ImplantLiquidContainerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ImplantLiquidContainerGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ImplantLiquidContainer".ToLowerInvariant(), true,
			(gameworld, account) => new ImplantLiquidContainerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImplantLiquidContainer",
			(proto, gameworld) => new ImplantLiquidContainerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ImplantLiquidContainer",
			$"An {"[implant]".Colour(Telnet.Pink)} that is also a {"[liquid container]".Colour(Telnet.BoldGreen)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImplantLiquidContainerGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbody <body> - sets the body prototype this implant is used with\n\tbodypart <bodypart> - sets the bodypart prototype this implant is used with\n\texternal - toggles whether this implant is external\n\texternaldesc <desc> - an alternate sdesc used when installed and external\n\tpower <watts> - how many watts of power to use\n\tdiscount <watts> - how many watts of power usage to discount per point of quality\n\tgrace <percentage> - the grace percentage of hp damage before implant function reduces\n\tclose - toggles whether this container opens and closes\n\tcapacity <amount> - sets the amount of liquid this container can hold\n\tweight - sets the maximum weight of liquid this container can hold\n\ttransparent - toggles whether you can see the liquid inside when closed\n\tonce - toggles whether this container only opens once\n\tAdjustQuantityProg - Prog to execute anytime quantity changes\n\tspace <#> - the amount of 'space' in a bodypart that the implant takes up\n\tdifficulty <difficulty> - how difficulty it is for surgeons to install this implant";

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
			case "weight":
			case "weight limit":
			case "weight capacity":
			case "limit":
				return BuildingCommand_WeightLimit(actor, command);
			case "transparent":
				return BuildingCommand_Transparent(actor, command);
			case "adjustquantityprog":
				return BuildingCommand_AdjustQuantityProg(actor, command);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
		}
	}

	#region Building Command SubCommands

	private bool BuildingCommand_Transparent(ICharacter actor, StringStack command)
	{
		Transparent = !Transparent;
		actor.Send("This liquid container implant is {0} transparent.", Transparent ? "now" : "no longer");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Closable(ICharacter actor, StringStack command)
	{
		Closable = !Closable;
		actor.OutputHandler.Send("This liquid container implant is " + (Closable ? "now" : "no longer") + " closable.");
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
				$"This liquid container implant will now hold {actor.Gameworld.UnitManager.Describe(LiquidCapacity, UnitType.FluidVolume, actor).ColourValue()}.");
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

	#endregion

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return ComponentDescriptionOLC(actor, "This item is an implantable liquid container",
			$" It can contain {Gameworld.UnitManager.Describe(LiquidCapacity, UnitType.FluidVolume, actor)} or {Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor)} of liquid. It {(Transparent ? "is" : "is not")} transparent and {(Closable ? "can be opened and closed" : "cannot be opened and closed")}.\n\nWhen its quantity changes, it will {(AdjustQuantityProg != null ? $"execute {AdjustQuantityProg.Name} (#{AdjustQuantityProg.Id})" : "not execute a prog")}");
	}

	public double LiquidCapacity { get; set; }
	public bool Closable { get; set; }
	public bool Transparent { get; set; }
	public double WeightLimit { get; protected set; }
	public IFutureProg AdjustQuantityProg { get; protected set; }
}