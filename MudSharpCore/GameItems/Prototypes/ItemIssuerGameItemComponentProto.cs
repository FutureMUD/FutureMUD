using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.Magic;

#nullable enable
namespace MudSharp.GameItems.Prototypes;

public sealed class ItemIssuerGameItemComponentProto : ConditionalMagicItemGameItemComponentProto
{
	protected override ConditionalMagicItemKind Kind => ConditionalMagicItemKind.ItemIssuer;
	public override string TypeDescription => "ItemIssuer";
	private ItemIssuerGameItemComponentProto(IFuturemud world, IAccount account) : base(world, account, "ItemIssuer") { }
	private ItemIssuerGameItemComponentProto(MudSharp.Models.GameItemComponentProto model, IFuturemud world) : base(model, world) { }
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("itemissuer", true, (w, a) => new ItemIssuerGameItemComponentProto(w, a));
		manager.AddDatabaseLoader("ItemIssuer", (m, w) => new ItemIssuerGameItemComponentProto(m, w));
		manager.AddTypeHelpInfo("ItemIssuer", "Configurable ItemIssuer item content.", Help);
	}
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new ItemIssuerGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent model, IGameItem parent) => new ItemIssuerGameItemComponent(model, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter actor) => CreateNewRevision(actor, (m, w) => new ItemIssuerGameItemComponentProto(m, w));
}
