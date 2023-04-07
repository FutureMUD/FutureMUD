using System.Linq;
using System.Xml.Linq;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ChangerGameItemComponent : GameItemComponent, IChangeCharacteristics
{
	private ChangerGameItemComponentProto _prototype;

	public ChangerGameItemComponent(ChangerGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ChangerGameItemComponent(MudSharp.Models.GameItemComponent component, ChangerGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	public ChangerGameItemComponent(ChangerGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ChangerGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ChangerGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	protected void LoadFromXml(XElement root)
	{
		// Nothing to do
	}

	#region IChangeCharacteristics Members

	public bool ChangesCharacteristic(ICharacteristicDefinition type)
	{
		if (_prototype.TargetWearProfile != null)
		{
			var wdata = Parent.GetItemType<IWearable>();
			return wdata != null && wdata.CurrentProfile == _prototype.TargetWearProfile &&
			       _prototype.Definitions.Contains(type);
		}

		return _prototype.Definitions.Contains(type);
	}

	public string DescribeCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur, bool basic = false)
	{
		return basic ? GetCharacteristic(type, voyeur).GetBasicValue : GetCharacteristic(type, voyeur).GetValue;
	}

	public ICharacteristicValue GetCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur)
	{
		return _prototype.ValueFor(type);
	}

	#endregion
}