using MudSharp.Character;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class ZeroGravityAnchorGameItemComponent : GameItemComponent, IZeroGravityAnchorItem
{
	private ZeroGravityAnchorGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public ZeroGravityAnchorGameItemComponent(ZeroGravityAnchorGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ZeroGravityAnchorGameItemComponent(MudSharp.Models.GameItemComponent component, ZeroGravityAnchorGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	public ZeroGravityAnchorGameItemComponent(ZeroGravityAnchorGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public bool AllowsZeroGravityPushOff => true;

	public override bool PreventsRepositioning()
	{
		return true;
	}

	public override string WhyPreventsRepositioning()
	{
		return "It is fixed in place.";
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ZeroGravityAnchorGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ZeroGravityAnchorGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}
}
