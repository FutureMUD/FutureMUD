using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

#nullable enable
namespace MudSharp.GameItems.Components;

public sealed class ConditionalBindingGameItemComponent : ConditionalMagicItemGameItemComponent, IConditionalBindingItem
{
	public ConditionalBindingGameItemComponent(ConditionalBindingGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary) { }
	public ConditionalBindingGameItemComponent(MudSharp.Models.GameItemComponent model, ConditionalBindingGameItemComponentProto proto, IGameItem parent) : base(model, proto, parent) { }
	public override IGameItemComponent Copy(IGameItem parent, bool temporary = false) => new ConditionalBindingGameItemComponent((ConditionalBindingGameItemComponentProto)Prototype, parent, temporary);
}
