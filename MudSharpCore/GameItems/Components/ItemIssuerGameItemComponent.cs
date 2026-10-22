using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

#nullable enable
namespace MudSharp.GameItems.Components;

public sealed class ItemIssuerGameItemComponent : ConditionalMagicItemGameItemComponent
{
	public ItemIssuerGameItemComponent(ItemIssuerGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary) { }
	public ItemIssuerGameItemComponent(MudSharp.Models.GameItemComponent model, ItemIssuerGameItemComponentProto proto, IGameItem parent) : base(model, proto, parent) { }
	public override IGameItemComponent Copy(IGameItem parent, bool temporary = false) => new ItemIssuerGameItemComponent((ItemIssuerGameItemComponentProto)Prototype, parent, temporary);
}
