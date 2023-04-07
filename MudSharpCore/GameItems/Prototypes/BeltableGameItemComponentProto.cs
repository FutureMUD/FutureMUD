using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class BeltableGameItemComponentProto : GameItemComponentProto
{
	private static readonly string _showString = "Beltable Item Component".Colour(Telnet.Cyan) + "\n\n" +
	                                             "This item can be attached to belts.\n";

	protected BeltableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Beltable")
	{
	}

	protected BeltableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Beltable";

	protected override void LoadFromXml(XElement root)
	{
		// Nothing to do
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		command.Pop();
		return base.BuildingCommand(actor, command);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format("{0} (#{1:N0}r{3:N0}, {2})\n\nThis item can be attached to belts.\n",
			"Beltable Item Component".Colour(Telnet.Cyan), Id, Name, RevisionNumber);
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("beltable", true,
			(gameworld, account) => new BeltableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Beltable",
			(proto, gameworld) => new BeltableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Beltable",
			$"Makes this item able to be {"[attached]".Colour(Telnet.Yellow)} to {"[belt]".Colour(Telnet.Yellow)} items",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BeltableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BeltableGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new BeltableGameItemComponentProto(proto, gameworld));
	}
}