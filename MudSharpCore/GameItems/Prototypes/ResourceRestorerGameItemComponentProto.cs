using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.Magic;

#nullable enable
namespace MudSharp.GameItems.Prototypes;

public sealed class ResourceRestorerGameItemComponentProto : ConditionalMagicItemGameItemComponentProto
{
	protected override ConditionalMagicItemKind Kind => ConditionalMagicItemKind.ResourceRestorer;
	public override string TypeDescription => "ResourceRestorer";
	private ResourceRestorerGameItemComponentProto(IFuturemud world, IAccount account) : base(world, account, "ResourceRestorer") { }
	private ResourceRestorerGameItemComponentProto(MudSharp.Models.GameItemComponentProto model, IFuturemud world) : base(model, world) { }
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("resourcerestorer", true, (w, a) => new ResourceRestorerGameItemComponentProto(w, a));
		manager.AddDatabaseLoader("ResourceRestorer", (m, w) => new ResourceRestorerGameItemComponentProto(m, w));
		manager.AddTypeHelpInfo("ResourceRestorer", "Configurable ResourceRestorer item content.", Help);
	}
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new ResourceRestorerGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent model, IGameItem parent) => new ResourceRestorerGameItemComponent(model, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter actor) => CreateNewRevision(actor, (m, w) => new ResourceRestorerGameItemComponentProto(m, w));
}
