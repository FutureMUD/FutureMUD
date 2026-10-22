using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.Magic;

#nullable enable
namespace MudSharp.GameItems.Prototypes;

public sealed class SpellSelectionShrineGameItemComponentProto : ConditionalMagicItemGameItemComponentProto, ISpellSelectionShrinePrototype
{
	protected override ConditionalMagicItemKind Kind => ConditionalMagicItemKind.SpellSelectionShrine;
	public override string TypeDescription => "SpellSelectionShrine";
	private SpellSelectionShrineGameItemComponentProto(IFuturemud world, IAccount account) : base(world, account, "SpellSelectionShrine") { }
	private SpellSelectionShrineGameItemComponentProto(MudSharp.Models.GameItemComponentProto model, IFuturemud world) : base(model, world) { }
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("spellselectionshrine", true, (w, a) => new SpellSelectionShrineGameItemComponentProto(w, a));
		manager.AddDatabaseLoader("SpellSelectionShrine", (m, w) => new SpellSelectionShrineGameItemComponentProto(m, w));
		manager.AddTypeHelpInfo("SpellSelectionShrine", "Configurable SpellSelectionShrine item content.", Help);
	}
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new SpellSelectionShrineGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent model, IGameItem parent) => new SpellSelectionShrineGameItemComponent(model, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter actor) => CreateNewRevision(actor, (m, w) => new SpellSelectionShrineGameItemComponentProto(m, w));
}
