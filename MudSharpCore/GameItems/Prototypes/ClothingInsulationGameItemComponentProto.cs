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

namespace MudSharp.GameItems.Prototypes;

public class ClothingInsulationGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "ClothingInsulation";
	public double InsulatingDegrees { get; protected set; }
	public double ReflectingDegrees { get; protected set; }

	#region Constructors

	protected ClothingInsulationGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ClothingInsulation")
	{
	}

	protected ClothingInsulationGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		InsulatingDegrees = double.Parse(root.Element("InsulatingDegrees")?.Value ?? "0.0");
		ReflectingDegrees = double.Parse(root.Element("ReflectingDegrees")?.Value ?? "0.0");
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("InsulatingDegrees", InsulatingDegrees),
			new XElement("ReflectingDegrees", ReflectingDegrees)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ClothingInsulationGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ClothingInsulationGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ClothingInsulation".ToLowerInvariant(), true,
			(gameworld, account) => new ClothingInsulationGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ClothingInsulation",
			(proto, gameworld) => new ClothingInsulationGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"ClothingInsulation",
			$"Adds positive or negative insulation to clothes, combined with a {"[wearable]".Colour(Telnet.BoldYellow)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ClothingInsulationGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tinsulation <degrees> - sets the degrees of insulation. Positive means insulating, negative means cooling.\n\treflection <degrees> - sets the degrees of reflection when worn as outerwear";

	public override string ShowBuildingHelp => BuildingHelpText;

	private bool BuildingCommandInsulation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many degrees of insulation should this garment provide when worn?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number of degrees of insulation.");
			return false;
		}

		var tempUnit = Gameworld.UnitManager.Units
		                        .Where(x => x.Type == Framework.Units.UnitType.TemperatureDelta &&
		                                    x.System.EqualTo(actor.Account.UnitPreference)).First(x => x.LastDescriber);
		var adjusted = value * tempUnit.MultiplierFromBase;
		InsulatingDegrees = adjusted;
		Changed = true;
		actor.OutputHandler.Send(
			$"This garment will now offer {Gameworld.UnitManager.DescribeExact(adjusted, Framework.Units.UnitType.TemperatureDelta, actor).Colour(Telnet.Green)} of insulation when worn.");
		return true;
	}

	private bool BuildingCommandReflection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How many degrees of reflection should this garment provide when worn as an outer garment?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number of degrees of reflection.");
			return false;
		}

		var tempUnit = Gameworld.UnitManager.Units
		                        .Where(x => x.Type == Framework.Units.UnitType.TemperatureDelta &&
		                                    x.System.EqualTo(actor.Account.UnitPreference)).First(x => x.LastDescriber);
		var adjusted = value * tempUnit.MultiplierFromBase;
		ReflectingDegrees = adjusted;
		Changed = true;
		actor.OutputHandler.Send(
			$"This garment will now offer {Gameworld.UnitManager.DescribeExact(adjusted, Framework.Units.UnitType.TemperatureDelta, actor).Colour(Telnet.Green)} of reflection when worn.");
		return true;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "insulate":
			case "insulates":
			case "insulation":
			case "insulating":
				return BuildingCommandInsulation(actor, command);
			case "reflect":
			case "reflects":
			case "reflection":
			case "reflecting":
				return BuildingCommandReflection(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nWhen worn, this item adds {4} insulation and {5} reflection.",
			"ClothingInsulation Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Gameworld.UnitManager.DescribeExact(InsulatingDegrees, Framework.Units.UnitType.TemperatureDelta, actor)
			         .Colour(Telnet.Green),
			Gameworld.UnitManager.DescribeExact(ReflectingDegrees, Framework.Units.UnitType.TemperatureDelta, actor)
			         .Colour(Telnet.Green)
		);
	}
}