using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class ZeroGravityAnchorGameItemComponentProto : GameItemComponentProto, IZeroGravityAnchorItemPrototype
{
	protected ZeroGravityAnchorGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "ZeroGravityAnchor")
	{
		Changed = true;
	}

	protected ZeroGravityAnchorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public override string TypeDescription => "ZeroGravityAnchor";

	protected override void LoadFromXml(XElement root)
	{
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	private const string BuildingHelpText = "This component marks an item as fixed enough to use as a zero-gravity push-off anchor.";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $"{ "Zero Gravity Anchor Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nThis item can be used as a push-off anchor in zero gravity.";
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ZeroGravityAnchorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ZeroGravityAnchorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new ZeroGravityAnchorGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ZeroGravityAnchor".ToLowerInvariant(), true, (gameworld, account) => new ZeroGravityAnchorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ZeroGravityAnchor", (proto, gameworld) => new ZeroGravityAnchorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("ZeroGravityAnchor", "A fixed item usable as a zero-gravity push-off anchor.", BuildingHelpText);
	}
}
