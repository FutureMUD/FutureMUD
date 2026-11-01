#nullable enable

using System;
using System.Globalization;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Prototypes;

public class ClimbingAidGameItemComponentProto : GameItemComponentProto, IClimbingAidPrototype
{
	public override string TypeDescription => "Climbing Aid";
	public double CheckBonus { get; private set; }
	public bool WorksHeld { get; private set; } = true;
	public bool WorksWorn { get; private set; } = true;

	protected ClimbingAidGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld) { }
	protected ClimbingAidGameItemComponentProto(IFuturemud gameworld, IAccount account) : base(gameworld, account, "Climbing Aid") { }

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("climbingaid", true, (world, account) => new ClimbingAidGameItemComponentProto(world, account));
		manager.AddDatabaseLoader("Climbing Aid", (proto, world) => new ClimbingAidGameItemComponentProto(proto, world));
		manager.AddTypeHelpInfo("ClimbingAid", "Assists climbing while held or worn; only the strongest eligible aid applies.", Help);
	}

	protected override void LoadFromXml(XElement root)
	{
		CheckBonus = (double?)root.Element("CheckBonus") ?? 0.0;
		WorksHeld = (bool?)root.Element("WorksHeld") ?? true;
		WorksWorn = (bool?)root.Element("WorksWorn") ?? true;
	}

	protected override string SaveToXml() => new XElement("Definition",
		new XElement("CheckBonus", CheckBonus), new XElement("WorksHeld", WorksHeld), new XElement("WorksWorn", WorksWorn)).ToString();

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new ClimbingAidGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) => new ClimbingAidGameItemComponent(component, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator, (proto, world) => new ClimbingAidGameItemComponentProto(proto, world));
	public override string ComponentDescriptionOLC(ICharacter actor) => $"Climbing aid #{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}\nBonus: {CheckBonus.ToString("N2", actor).ColourValue()}\nWorks held: {WorksHeld.ToColouredString()}\nWorks worn (any location): {WorksWorn.ToColouredString()}\nOnly the strongest equipped aid applies.";
	private const string Help = "You can use the following options:\n\tname <name> - sets the component name\n\tdesc <description> - sets the component description\n\tbonus <number> - sets a flat native climbing-check bonus\n\theld - toggles use while held\n\tworn - toggles use while worn in any location";
	public override string ShowBuildingHelp => Help;
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "bonus":
				if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor, out var bonus) || !double.IsFinite(bonus))
				{
					actor.Send("Enter a finite climbing bonus.");
					return false;
				}
				CheckBonus = bonus;
				actor.Send($"The climbing bonus is now {CheckBonus.ToString("N2", actor).ColourValue()}.");
				break;
			case "held":
				WorksHeld = !WorksHeld;
				actor.Send($"Works while held: {WorksHeld.ToColouredString()}.");
				break;
			case "worn":
				WorksWorn = !WorksWorn;
				actor.Send($"Works while worn in any location: {WorksWorn.ToColouredString()}.");
				break;
			default:
				return base.BuildingCommand(actor, command);
		}
		Changed = true;
		return true;
	}
}
