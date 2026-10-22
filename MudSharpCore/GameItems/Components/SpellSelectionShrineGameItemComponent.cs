using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

#nullable enable
namespace MudSharp.GameItems.Components;

public sealed class SpellSelectionShrineGameItemComponent : ConditionalMagicItemGameItemComponent, ISpellSelectionShrine
{
	public SpellSelectionShrineGameItemComponent(SpellSelectionShrineGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary) { }
	public SpellSelectionShrineGameItemComponent(MudSharp.Models.GameItemComponent model, SpellSelectionShrineGameItemComponentProto proto, IGameItem parent) : base(model, proto, parent) { }
	public override IGameItemComponent Copy(IGameItem parent, bool temporary = false) => new SpellSelectionShrineGameItemComponent((SpellSelectionShrineGameItemComponentProto)Prototype, parent, temporary);
}
