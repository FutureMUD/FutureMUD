using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class DoorGameItemComponentProto : DoorGameItemComponentProtoBase
{
	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3uninstallable <hinge side difficulty> <other side difficulty> <uninstall trait>#0 - sets the door as uninstallable
	#3uninstallable#0 - sets the door as not uninstallable by players
	#3smashable <difficulty>#0 - sets the door as smashable by players
	#3smashable#0 - sets the door as not smashable
	#3installed <keyword>#0 - sets the keyword for this door as viewed in exits (e.g. iron door)
	#3transparent#0 - sets the door as transparent
	#3opaque#0 - sets the door as opaque
	#3fire#0 - toggles whether the door can be fired through (e.g. gate)
	#3openable#0 - toggles whether players can open this door with the OPEN/CLOSE commands";

	protected DoorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected DoorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Door")
	{
	}

	public override string TypeDescription => "Door";

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Door Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\n{DescribeDoorCharacteristics(actor, true)}";
	}

	protected override void LoadFromXml(XElement root)
	{
		LoadDoorPrototypeData(root);
	}

	protected override string SaveToXml()
	{
		return SaveDoorPrototypeData(new XElement("Definition")).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("door", true,
			(gameworld, account) => new DoorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Door", (proto, gameworld) => new DoorGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Door",
			$"Turns the item into a {"[door]".Colour(Telnet.Yellow)} that can be installed in doorways",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new DoorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new DoorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new DoorGameItemComponentProto(proto, gameworld));
	}

	public override string ShowBuildingHelp => BuildingHelpText;
}
