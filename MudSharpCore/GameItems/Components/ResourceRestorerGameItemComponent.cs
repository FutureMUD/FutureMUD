using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

#nullable enable
namespace MudSharp.GameItems.Components;

public sealed class ResourceRestorerGameItemComponent : ConditionalMagicItemGameItemComponent
{
	public ResourceRestorerGameItemComponent(ResourceRestorerGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary) { }
	public ResourceRestorerGameItemComponent(MudSharp.Models.GameItemComponent model, ResourceRestorerGameItemComponentProto proto, IGameItem parent) : base(model, proto, parent) { }
	public override IGameItemComponent Copy(IGameItem parent, bool temporary = false) => new ResourceRestorerGameItemComponent((ResourceRestorerGameItemComponentProto)Prototype, parent, temporary);
}
