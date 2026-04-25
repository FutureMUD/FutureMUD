using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class ZeroGravityTetherGameItemComponent : GameItemComponent, IZeroGravityTetherItem
{
	private ZeroGravityTetherGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public ZeroGravityTetherGameItemComponent(ZeroGravityTetherGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ZeroGravityTetherGameItemComponent(MudSharp.Models.GameItemComponent component, ZeroGravityTetherGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	public ZeroGravityTetherGameItemComponent(ZeroGravityTetherGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public int MaximumRooms => _prototype.MaximumRooms;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ZeroGravityTetherGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ZeroGravityTetherGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}
}
