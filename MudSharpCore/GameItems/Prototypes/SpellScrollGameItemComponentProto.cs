using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable
namespace MudSharp.GameItems.Prototypes;

public sealed class SpellScrollGameItemComponentProto : VancianWritingComponentProto, ISpellScrollPrototype
{
	public override string TypeDescription => "SpellScroll";
	public SpellScrollGameItemComponentProto(IFuturemud game, IAccount account) : base(game, account, "SpellScroll") { }
	public SpellScrollGameItemComponentProto(Models.GameItemComponentProto model, IFuturemud game) : base(model, game) { }
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new SpellScrollGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(Models.GameItemComponent model, IGameItem parent) => new SpellScrollGameItemComponent(model, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator, (model, game) => new SpellScrollGameItemComponentProto(model, game));
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("spellscroll", true, (game, account) => new SpellScrollGameItemComponentProto(game, account));
		manager.AddDatabaseLoader("SpellScroll", (model, game) => new SpellScrollGameItemComponentProto(model, game));
		manager.AddTypeHelpInfo("SpellScroll", "A blank surface or prepaid, single-use stored spell", ProductionHelp);
	}
}
