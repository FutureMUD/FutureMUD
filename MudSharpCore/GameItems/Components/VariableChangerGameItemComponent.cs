using System.Linq;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class VariableChangerGameItemComponent : VariableGameItemComponent, IChangeCharacteristics
{
	private VariableChangerGameItemComponentProto _varCharProto;

	public VariableChangerGameItemComponent(VariableChangerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_varCharProto = proto;
	}

	public VariableChangerGameItemComponent(MudSharp.Models.GameItemComponent component,
		VariableChangerGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_varCharProto = proto;
	}

	public VariableChangerGameItemComponent(VariableChangerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_varCharProto = rhs._varCharProto;
	}

	public override IGameItemComponentProto Prototype => _varCharProto;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_varCharProto = (VariableChangerGameItemComponentProto)newProto;
	}

	#region IChangeCharacteristics Members

	public bool ChangesCharacteristic(ICharacteristicDefinition type)
	{
		return CharacteristicDefinitions.Contains(type);
	}

	public string DescribeCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur, bool basic = false)
	{
		return basic ? GetCharacteristic(type, voyeur).GetBasicValue : GetCharacteristic(type, voyeur).GetValue;
	}

	public ICharacteristicValue GetCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur)
	{
		return GetCharacteristic(type);
	}

	#endregion
}