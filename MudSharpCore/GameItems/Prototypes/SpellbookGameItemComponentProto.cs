using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable
namespace MudSharp.GameItems.Prototypes;

public sealed class SpellbookGameItemComponentProto : VancianWritingComponentProto, ISpellbookPrototype
{
	public override string TypeDescription => "Spellbook";
	public SpellbookGameItemComponentProto(IFuturemud game, IAccount account) : base(game, account, "Spellbook") { }
	public SpellbookGameItemComponentProto(Models.GameItemComponentProto model, IFuturemud game) : base(model, game) { }
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new SpellbookGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(Models.GameItemComponent model, IGameItem parent) => new SpellbookGameItemComponent(model, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator, (model, game) => new SpellbookGameItemComponentProto(model, game));
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("spellbook", true, (game, account) => new SpellbookGameItemComponentProto(game, account));
		manager.AddDatabaseLoader("Spellbook", (model, game) => new SpellbookGameItemComponentProto(model, game));
		manager.AddTypeHelpInfo("Spellbook", "Stores structured spell formulae on each item instance", ProductionHelp);
	}
}
