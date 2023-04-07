using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class VariableGameItemComponent : GameItemComponent, IVariable
{
	protected readonly Dictionary<ICharacteristicDefinition, ICharacteristicValue> _characteristicValues =
		new();

	protected VariableGameItemComponentProto _prototype;

	public VariableGameItemComponent(VariableGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		foreach (var definition in _prototype.CharacteristicDefinitions)
		{
			SetRandom(definition);
		}
	}

	public VariableGameItemComponent(MudSharp.Models.GameItemComponent component, VariableGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public VariableGameItemComponent(VariableGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;

		foreach (var value in rhs._characteristicValues)
		{
			_characteristicValues.Add(value.Key, value.Value);
		}
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new VariableGameItemComponent(this, newParent, temporary);
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		var comp = component as IVariable;
		return comp?.CharacteristicDefinitions.Any(
			x =>
				!_characteristicValues.ContainsKey(x) ||
				_characteristicValues[x] != comp.GetCharacteristic(x)) == true;
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemVariable = newItem?.GetItemType<VariableGameItemComponent>();
		if (newItemVariable == null)
		{
			return false;
		}

		foreach (var item in _characteristicValues)
		{
			if (newItemVariable.CharacteristicDefinitions.Contains(item.Key))
			{
				newItemVariable.SetCharacteristic(item.Key, item.Value);
			}
		}

		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (VariableGameItemComponentProto)newProto;
		foreach (
			var value in
			_characteristicValues.Where(x => !_prototype.CharacteristicDefinitions.Contains(x.Key)).ToList())
		{
			_characteristicValues.Remove(value.Key);
			Changed = true;
		}

		foreach (
			var value in
			_prototype.CharacteristicDefinitions.Where(x => !_characteristicValues.ContainsKey(x)).ToList())
		{
			SetRandom(value);
		}

		// Sanity check
		foreach (var definition in _prototype.CharacteristicDefinitions)
		{
			if (_characteristicValues.All(x => x.Key != definition))
			{
				SetRandom(definition);
			}
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new object[]
		{
			from value in _characteristicValues
			select
				new XElement("Value", new XAttribute("Definition", value.Key.Id),
					new XAttribute("Target", value.Value.Id))
		}).ToString();
	}

	protected void LoadFromXml(XElement root)
	{
		foreach (var value in root.Elements("Value"))
		{
			var definition = Gameworld.Characteristics.Get(long.Parse(value.Attribute("Definition").Value));
			if (definition != null)
			{
				var charValue = Gameworld.CharacteristicValues.Get(long.Parse(value.Attribute("Target").Value));
				if (charValue != null)
				{
					_characteristicValues.Add(definition, charValue);
				}
			}
		}

		// Sanity check
		foreach (var definition in _prototype.CharacteristicDefinitions)
		{
			if (_characteristicValues.All(x => x.Key != definition))
			{
				SetRandom(definition);
			}
		}
	}

	#region IVariable Members

	public IEnumerable<ICharacteristicDefinition> CharacteristicDefinitions => _prototype.CharacteristicDefinitions;

	public ICharacteristicValue GetCharacteristic(ICharacteristicDefinition type)
	{
		return !_characteristicValues.TryGetValue(type, out var value) ? null : value;
	}

	public void SetCharacteristic(ICharacteristicDefinition definition, ICharacteristicValue value)
	{
		_characteristicValues[definition] = value;
		Changed = true;
	}

	public void SetRandom(ICharacteristicDefinition definition)
	{
		_characteristicValues[definition] = _prototype.ProfileFor(definition).GetRandomCharacteristic();
		Changed = true;
	}

	public void ExpireDefinition(ICharacteristicDefinition definition)
	{
		_characteristicValues.Remove(definition);
		Changed = true;
	}

	public void ExpireValue(ICharacteristicValue value)
	{
		foreach (var val in _characteristicValues.Where(x => x.Value == value).ToList())
		{
			_characteristicValues[val.Key] = _prototype.ProfileFor(val.Key).GetRandomCharacteristic();
		}

		Changed = true;
	}

	#endregion
}