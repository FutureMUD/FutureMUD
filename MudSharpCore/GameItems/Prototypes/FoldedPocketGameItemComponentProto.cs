#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public sealed class FoldedPocketGameItemComponentProto : ContainerGameItemComponentProto, ISpellPocketPrototype
{
	public override string TypeDescription => "FoldedPocket";
	public FoldedPocketGameItemComponentProto(IFuturemud world, IAccount account) : base(world, account, "FoldedPocket") { }
	private FoldedPocketGameItemComponentProto(Models.GameItemComponentProto row, IFuturemud world) : base(row, world) { }
	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("foldedpocket", true, (world, account) => new FoldedPocketGameItemComponentProto(world, account));
		manager.AddDatabaseLoader("FoldedPocket", (row, world) => new FoldedPocketGameItemComponentProto(row, world));
		manager.AddTypeHelpInfo("FoldedPocket", "Finite spell-bound item storage; unbound copies refuse contents.", "Use ordinary container size/open settings. Spell creation binds actual capacity, access and lifetime; item copies do not copy spell authority.");
	}
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new FoldedPocketGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(Models.GameItemComponent row, IGameItem parent) => new FoldedPocketGameItemComponent(row, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator, (row, world) => new FoldedPocketGameItemComponentProto(row, world));
}
