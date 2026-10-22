using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.Magic;

#nullable enable
namespace MudSharp.GameItems.Prototypes;

public sealed class ConditionalBindingGameItemComponentProto : ConditionalMagicItemGameItemComponentProto, IConditionalBindingItemPrototype
{
	protected override ConditionalMagicItemKind Kind => ConditionalMagicItemKind.ConditionalBinding;
	public override string TypeDescription => "ConditionalBinding";
	private ConditionalBindingGameItemComponentProto(IFuturemud world, IAccount account) : base(world, account, "ConditionalBinding") { }
	private ConditionalBindingGameItemComponentProto(MudSharp.Models.GameItemComponentProto model, IFuturemud world) : base(model, world) { }
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("conditionalbinding", true, (w, a) => new ConditionalBindingGameItemComponentProto(w, a));
		manager.AddDatabaseLoader("ConditionalBinding", (m, w) => new ConditionalBindingGameItemComponentProto(m, w));
		manager.AddTypeHelpInfo("ConditionalBinding", "Configurable ConditionalBinding item content.", Help);
	}
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new ConditionalBindingGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent model, IGameItem parent) => new ConditionalBindingGameItemComponent(model, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter actor) => CreateNewRevision(actor, (m, w) => new ConditionalBindingGameItemComponentProto(m, w));
}
