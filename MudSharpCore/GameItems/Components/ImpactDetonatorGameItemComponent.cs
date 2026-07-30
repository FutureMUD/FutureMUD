using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class ImpactDetonatorGameItemComponent : GameItemComponent, IImpactDetonator
{
	private ImpactDetonatorGameItemComponentProto _prototype;

	public ImpactDetonatorGameItemComponent(ImpactDetonatorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ImpactDetonatorGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImpactDetonatorGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
	}

	public ImpactDetonatorGameItemComponent(ImpactDetonatorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ImpactDetonatorGameItemComponentProto)newProto;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImpactDetonatorGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}
}
