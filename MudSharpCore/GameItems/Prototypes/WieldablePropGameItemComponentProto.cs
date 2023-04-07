using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class WieldablePropGameItemComponentProto : GameItemComponentProto
{
	private static readonly string _showString = "Wieldable Prop Item Component".Colour(Telnet.Cyan) + "\n\n" +
	                                             "This item can be wielded by Characters.\n";

	protected WieldablePropGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Wieldable")
	{
	}

	protected WieldablePropGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Wieldable";

	protected override void LoadFromXml(XElement root)
	{
		// Nothing to do
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		command.Pop();
		return base.BuildingCommand(actor, command);
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\n\nThis item can be wielded by Characters.",
			"Wieldable Prop Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name);
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("wieldable", true,
			(gameworld, account) => new WieldablePropGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Wieldable",
			(proto, gameworld) => new WieldablePropGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Wieldable",
			$"Makes the item able to be wielded, even though it's not a weapon. Use for props.",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new WieldablePropGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new WieldablePropGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new WieldablePropGameItemComponentProto(proto, gameworld));
	}
}