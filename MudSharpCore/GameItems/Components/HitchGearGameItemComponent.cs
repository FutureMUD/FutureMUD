using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Components;

public class HitchGearGameItemComponent : GameItemComponent, IHitchGear
{
	protected HitchGearGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (HitchGearGameItemComponentProto)newProto;
	}

	public HitchGearGameItemComponent(HitchGearGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public HitchGearGameItemComponent(MudSharp.Models.GameItemComponent component,
		HitchGearGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public HitchGearGameItemComponent(HitchGearGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// Runtime state is prototype-driven for v1 hitch gear.
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new HitchGearGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	public HitchGearRole Roles => _prototype.Roles;
	public double MaximumTowedWeight => _prototype.MaximumTowedWeight;
	public double EffortMultiplier => _prototype.EffortMultiplier;
	public int MaximumUsers => _prototype.MaximumUsers;
}
