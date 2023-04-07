using System.Linq;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ObscurerGameItemComponent : GameItemComponent, IObscureCharacteristics
{
	protected ObscurerGameItemComponentProto _prototype;

	public ObscurerGameItemComponent(ObscurerGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ObscurerGameItemComponent(MudSharp.Models.GameItemComponent component, ObscurerGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	public ObscurerGameItemComponent(ObscurerGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ObscurerGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ObscurerGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	#region IObscureCharacteristics Members

	public bool ObscuresCharacteristic(ICharacteristicDefinition type)
	{
		return _prototype.ObscuredCharacteristics.Contains(type);
	}

	public string DescribeCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur)
	{
		return Parent.ParseCharacteristics(_prototype.GetDescription(type), voyeur);
	}

	public string RemovalEcho => _prototype.RemovalEcho;

	#endregion
}