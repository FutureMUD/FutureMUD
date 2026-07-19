using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class RidingGearGameItemComponent : GameItemComponent, IRidingGear
{
	protected RidingGearGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (RidingGearGameItemComponentProto)newProto;
	}

	public RidingGearGameItemComponent(RidingGearGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public RidingGearGameItemComponent(MudSharp.Models.GameItemComponent component,
		RidingGearGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public RidingGearGameItemComponent(RidingGearGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// Runtime state is prototype-driven for v1 riding gear.
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new RidingGearGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	public RidingGearRole Roles => _prototype.Roles;
	public double ControlBonus => _prototype.ControlBonus;
	public double StabilityBonus => _prototype.StabilityBonus;
}
