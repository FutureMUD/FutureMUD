using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExpressionEngine;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class HandToolGameItemComponentProto : GameItemComponentProto
{
	public double BaseMultiplier { get; protected set; }
	public double MultiplierReductionPerQuality { get; protected set; }
	public IExpression ToolDurabilitySecondsExpression { get; protected set; }

	public override string TypeDescription => "HandTool";

	#region Constructors

	protected HandToolGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"HandTool")
	{
		BaseMultiplier = 1.5;
		MultiplierReductionPerQuality = 0.1;
		ToolDurabilitySecondsExpression = new Expression("(1+quality) * 3600");
	}

	protected HandToolGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		BaseMultiplier = double.Parse(root.Element("BaseMultiplier").Value);
		MultiplierReductionPerQuality = double.Parse(root.Element("MultiplierReductionPerQuality").Value);
		ToolDurabilitySecondsExpression = new Expression(root.Element("ToolDurabilitySecondsExpression")?.Value ?? "(1+quality) * 3600");
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MultiplierReductionPerQuality", MultiplierReductionPerQuality),
			new XElement("BaseMultiplier", BaseMultiplier),
			new XElement("ToolDurabilitySecondsExpression", ToolDurabilitySecondsExpression?.OriginalExpression ?? "(1+quality) * 3600")
        ).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new HandToolGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new HandToolGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("HandTool".ToLowerInvariant(), true,
			(gameworld, account) => new HandToolGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("HandTool",
			(proto, gameworld) => new HandToolGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"HandTool",
			$"Item will speed up (or slow down) crafting when used as a 'tool' in that craft",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new HandToolGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
        @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3base <percentage>#0 - the base speed multiplier
	#3discount <percentage>#0 - the discount per quality
	#3durability <seconds formula>#0 - the seconds of usage allowed (parameter quality 0-11)";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "base":
				return BuildingCommandBase(actor, command);
			case "discount":
				return BuildingCommandDiscount(actor, command);
			case "durability":
				return BuildingCommandDurability(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

    private bool BuildingCommandDurability(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the expression for calculating tool durability in seconds based on quality? Use 'quality' as the variable.");
			return false;
		}

		var exprText = command.PopSpeech();
		try
		{
			var expr = new Expression(exprText);
			// Test it
			expr.EvaluateDoubleWith(("quality", (int)ItemQuality.Standard));
			ToolDurabilitySecondsExpression = expr;
			Changed = true;
			actor.OutputHandler.Send(
				$"The tool durability expression is now set to: {ToolDurabilitySecondsExpression.OriginalExpression.ColourValue()}");
			return true;
		}
		catch (Exception ex)
		{
			actor.OutputHandler.Send($"There was an error in that expression: {ex.Message.ColourError()}");
			return false;
        }
    }


    private bool BuildingCommandDiscount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the discount percentage to speed of crafts using this tool per point of quality?");
			return false;
		}

		if (!NumberUtilities.TryParsePercentage(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That discount is not a valid percentage.");
			return false;
		}

		MultiplierReductionPerQuality = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The discount percentage for this tool is now {MultiplierReductionPerQuality.ToString("P3", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandBase(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the base percentage speed of crafts using this tool?");
			return false;
		}

		if (!NumberUtilities.TryParsePercentage(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That base multiplier is not a valid percentage.");
			return false;
		}

		BaseMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The base speed multiplier for this tool is now {BaseMultiplier.ToString("P3", actor).ColourValue()}.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
            @"{0} (#{1}r{2}, {3})

This item is a power tool for crafting. It multiplies speed by {4} - best {5} typ {6} worst {7}.
The formula for usage hours is {8} - best {9} typ {10} worst {11}",
			"HandTool Game Item Component".Colour(Telnet.Cyan),
			Id.ToString("N0", actor),
			RevisionNumber.ToString("N0", actor),
			Name,
			$"{BaseMultiplier.ToString("P3", actor)} - (quality * {MultiplierReductionPerQuality.ToString("P3", actor)})"
				.Colour(Telnet.Yellow),
			(BaseMultiplier - (int)ItemQuality.Legendary * MultiplierReductionPerQuality).ToString("P3", actor)
			.ColourValue(),
			(BaseMultiplier - (int)ItemQuality.Standard * MultiplierReductionPerQuality).ToString("P3", actor)
			.ColourValue(),
			(BaseMultiplier - (int)ItemQuality.Terrible * MultiplierReductionPerQuality).ToString("P3", actor)
			.ColourValue(),
			ToolDurabilitySecondsExpression.OriginalExpression.Colour(Telnet.Yellow),
			TimeSpan.FromSeconds(ToolDurabilitySecondsExpression.EvaluateDoubleWith(("quality", (int)ItemQuality.Legendary))).DescribePreciseBrief(actor).ColourValue(),
            TimeSpan.FromSeconds(ToolDurabilitySecondsExpression.EvaluateDoubleWith(("quality", (int)ItemQuality.Standard))).DescribePreciseBrief(actor).ColourValue(),
            TimeSpan.FromSeconds(ToolDurabilitySecondsExpression.EvaluateDoubleWith(("quality", (int)ItemQuality.Terrible))).DescribePreciseBrief(actor).ColourValue()
        );
	}
}