using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class SyringeGameItemComponentProto : GameItemComponentProto
{
	public double LiquidCapacity { get; set; }
	public bool Transparent { get; set; }
	public double WeightLimit { get; protected set; }

	public override string TypeDescription => "Syringe";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("syringe", true,
			(gameworld, account) => new SyringeGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Syringe",
			(proto, gameworld) => new SyringeGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Syringe",
			$"A {"[liquid container]".Colour(Telnet.BoldGreen)} that can be used to {"[inject]".Colour(Telnet.Yellow)} the liquid into someone",
			BuildingHelpText
		);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{4:N0}r{5:N0}, {6})\n\nThis item is a syringe which can contain {1} or {2} of liquid. It {3} transparent.",
			"Syringe Item Component".Colour(Telnet.Cyan),
			Gameworld.UnitManager.Describe(LiquidCapacity, UnitType.FluidVolume, actor),
			Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor),
			Transparent ? "is" : "is not",
			Id,
			RevisionNumber,
			Name
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("LiquidCapacity");
		if (attr != null)
		{
			LiquidCapacity = double.Parse(attr.Value);
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
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("LiquidCapacity", LiquidCapacity),
				new XAttribute("Transparent", Transparent),
				new XAttribute("WeightLimit", WeightLimit)).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new SyringeGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SyringeGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new SyringeGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tcapacity <amount> - the liquid capacity of the syringe\n\tweight <weight> - the weight limit of the syringe\n\ttransparent - toggles whether you can see the contents of the syringe";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
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
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	#region Building Command SubCommands

	private bool BuildingCommand_Transparent(ICharacter actor, StringStack command)
	{
		Transparent = !Transparent;
		actor.Send("This liquid container is {0} transparent.", Transparent ? "now" : "no longer");
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

	#endregion

	#region Constructors

	protected SyringeGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected SyringeGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Syringe")
	{
		LiquidCapacity = 0.005 / gameworld.UnitManager.BaseFluidToLitres;
		WeightLimit = 0.1 / gameworld.UnitManager.BaseWeightToKilograms;
		Transparent = false;
		Changed = true;
	}

	#endregion
}