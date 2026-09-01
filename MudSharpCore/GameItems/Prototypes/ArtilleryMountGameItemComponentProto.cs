using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class ArtilleryMountGameItemComponentProto : GameItemComponentProto, IArtilleryMountPrototype
{
	public ArtilleryMountGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ArtilleryMount")
	{
	}

	protected ArtilleryMountGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "ArtilleryMount";
	public bool Fixed { get; private set; }
	public double TraverseArc { get; private set; } = 360.0;
	public double ElevationArc { get; private set; } = 45.0;

	protected override void LoadFromXml(XElement root)
	{
		Fixed = bool.TryParse(root.Element("Fixed")?.Value, out var fixedMount) && fixedMount;
		TraverseArc = double.TryParse(root.Element("TraverseArc")?.Value, out var traverse) ? traverse : 360.0;
		ElevationArc = double.TryParse(root.Element("ElevationArc")?.Value, out var elevation) ? elevation : 45.0;
	}

	protected override string SaveToXml() => new XElement("Definition",
		new XElement("Fixed", Fixed), new XElement("TraverseArc", TraverseArc), new XElement("ElevationArc", ElevationArc)).ToString();

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new ArtilleryMountGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new ArtilleryMountGameItemComponent(component, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new ArtilleryMountGameItemComponentProto(proto, gameworld));

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("artillerymount", true, (gameworld, account) => new ArtilleryMountGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ArtilleryMount", (proto, gameworld) => new ArtilleryMountGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("ArtilleryMount", "Makes an item a host for an artillery piece", BuildingHelpText);
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3fixed#0 - toggles whether the mount is permanently emplaced
	#3traverse <degrees>#0 - sets the horizontal firing arc
	#3elevation <degrees>#0 - sets the vertical firing arc.";
	public override string ShowBuildingHelp => BuildingHelpText;
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		switch (option)
		{
			case "fixed":
				Fixed = !Fixed;
				Changed = true;
				actor.Send($"This artillery mount is now {(Fixed ? "fixed" : "transportable").ColourValue()}.");
				return true;
			case "traverse":
			case "elevation":
				if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0 || value > 360.0)
				{
					actor.Send("You must enter an arc between 0 and 360 degrees.");
					return false;
				}
				if (option == "traverse") TraverseArc = value; else ElevationArc = value;
				Changed = true;
				return true;
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"{Name.ColourName()} is an artillery mount with {TraverseArc.ToString("N0", actor).ColourValue()} degrees traverse and {ElevationArc.ToString("N0", actor).ColourValue()} degrees elevation.";
}
