using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class DoorGameItemComponent : DoorGameItemComponentBase
{
	public DoorGameItemComponent(DoorGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(proto, parent, temporary)
	{
	}

	public DoorGameItemComponent(MudSharp.Models.GameItemComponent component, DoorGameItemComponentProto proto,
		IGameItem parent)
		: base(component, proto, parent)
	{
	}

	public DoorGameItemComponent(DoorGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new DoorGameItemComponent(this, newParent, temporary);
	}
}
