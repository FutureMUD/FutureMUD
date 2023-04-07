using System;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Models;

namespace MudSharp.Work.Crafts.Tools;

public abstract class BaseTool : LateInitialisingItem, ICraftTool
{
	public sealed override string FrameworkItemType => "CraftTool";

	private ICraft _craft;

	protected BaseTool(CraftTool tool, ICraft craft, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_craft = craft;
		_id = tool.Id;
		IdInitialised = true;
		ToolQualityWeight = tool.ToolQualityWeight;
		DesiredState = (DesiredItemState)tool.DesiredState;
		OriginalAdditionTime = tool.OriginalAdditionTime;
	}

	protected BaseTool(ICraft craft, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_craft = craft;
		ToolQualityWeight = 1.0;
		DesiredState = DesiredItemState.Held;
		OriginalAdditionTime = DateTime.UtcNow;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public DateTime OriginalAdditionTime { get; set; }

	public abstract string ToolType { get; }

	#region Implementation of ICraftTool

	public DesiredItemState DesiredState { get; set; }

	public abstract string HowSeen(IPerceiver voyeur);

	public abstract bool IsTool(IGameItem item);

	public virtual Func<IGameItem, bool> EvaluateToolFunction(ICraft craft, int phaseNumber)
	{
		return IsTool;
	}

	public virtual void UseTool(IGameItem item, TimeSpan phaseLength, bool hasFailed)
	{
		// NOOP
	}

	public virtual double PhaseLengthMultiplier(IGameItem item)
	{
		return 1.0;
	}

	public abstract double ToolFitness(IGameItem item);

	public double ToolQualityWeight { get; set; }

	#endregion

	#region Overrides of SaveableItem

	public bool ToolChanged
	{
		get => Changed;
		set
		{
			Changed = value;
			_craft.CalculateCraftIsValid();
		}
	}

	/// <summary>Tells the object to perform whatever save action it needs to do</summary>
	public override void Save()
	{
		var dbitem = FMDB.Context.CraftTools.Find(Id);
		dbitem.Definition = SaveDefinition();
		dbitem.DesiredState = (int)DesiredState;
		dbitem.ToolQualityWeight = ToolQualityWeight;
		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new CraftTool();
		FMDB.Context.CraftTools.Add(dbitem);
		dbitem.Definition = SaveDefinition();
		dbitem.DesiredState = (int)DesiredState;
		dbitem.ToolQualityWeight = ToolQualityWeight;
		dbitem.ToolType = ToolType;
		dbitem.OriginalAdditionTime = OriginalAdditionTime;
		dbitem.CraftId = _craft.Id;
		dbitem.CraftRevisionNumber = _craft.RevisionNumber;
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((CraftTool)dbitem).Id;
	}

	#endregion

	protected abstract string SaveDefinition();

	public virtual string BuilderHelpString =>
		"The following options are available for this tool type:\n\t<held|worn|wield|room> - the necessary state for this tool to be used\n\tquality <number> - the relative weight of this tool's quality in the overall quality of the output";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Last.ToLowerInvariant())
		{
			case "held":
			case "hold":
			case "hands":
			case "hand":
				return BuildingCommandHeld(actor);
			case "worn":
			case "wear":
				return BuildingCommandWorn(actor);
			case "wield":
			case "wielded":
				return BuildingCommandWield(actor);
			case "room":
			case "inroom":
				return BuildingCommandRoom(actor);
			case "weight":
			case "quality":
			case "qualityweight":
				return BuildingCommandQualityWeight(actor, command);
			default:
				actor.OutputHandler.Send(BuilderHelpString);
				return false;
		}
	}

	private bool BuildingCommandQualityWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.RemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send(
				"What weight do you want to give to the quality of this tool in determining the overall quality of the output?");
			return false;
		}

		ToolQualityWeight = value;
		ToolChanged = true;
		actor.OutputHandler.Send(
			$"This tool now has a weighting of {ToolQualityWeight.ToString("N2", actor).Colour(Telnet.Green)} in determining the overall quality of the product.");
		return true;
	}

	private bool BuildingCommandRoom(ICharacter actor)
	{
		DesiredState = DesiredItemState.InRoom;
		ToolChanged = true;
		actor.OutputHandler.Send("This tool is now only required to be in the room.");
		return true;
	}

	private bool BuildingCommandWield(ICharacter actor)
	{
		DesiredState = DesiredItemState.Wielded;
		ToolChanged = true;
		actor.OutputHandler.Send("This tool is now required to be wielded.");
		return true;
	}

	private bool BuildingCommandWorn(ICharacter actor)
	{
		DesiredState = DesiredItemState.Worn;
		ToolChanged = true;
		actor.OutputHandler.Send("This tool is now required to be worn.");
		return true;
	}

	private bool BuildingCommandHeld(ICharacter actor)
	{
		DesiredState = DesiredItemState.Held;
		ToolChanged = true;
		actor.OutputHandler.Send("This tool is now only required to be held in hand.");
		return true;
	}

	public void CreateNewRevision(MudSharp.Models.Craft dbcraft)
	{
		var dbtool = new CraftTool();
		dbcraft.CraftTools.Add(dbtool);
		dbtool.ToolQualityWeight = ToolQualityWeight;
		dbtool.ToolType = ToolType;
		dbtool.DesiredState = (int)DesiredState;
		dbtool.OriginalAdditionTime = OriginalAdditionTime;
		dbtool.Definition = SaveDefinition();
	}

	public abstract bool IsValid();

	public abstract string WhyNotValid();
}