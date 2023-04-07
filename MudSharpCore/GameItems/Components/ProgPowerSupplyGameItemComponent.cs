using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ProgPowerSupplyGameItemComponent : GameItemComponent, IProducePower, IOnOff
{
	protected ProgPowerSupplyGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ProgPowerSupplyGameItemComponentProto)newProto;
	}

	#region Constructors

	public ProgPowerSupplyGameItemComponent(ProgPowerSupplyGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ProgPowerSupplyGameItemComponent(MudSharp.Models.GameItemComponent component,
		ProgPowerSupplyGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ProgPowerSupplyGameItemComponent(ProgPowerSupplyGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		ProducingPower = bool.Parse(root.Element("ProducingPower").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ProgPowerSupplyGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("ProducingPower", ProducingPower)).ToString();
	}

	#endregion

	#region IOnOff Implementation

	public bool SwitchedOn
	{
		get => ProducingPower;
		set
		{
			ProducingPower = value;
			Changed = true;
			if (value)
			{
				foreach (var item in _connectedConsumers.ToList())
				{
					_powerUsers.Add(item);
					item.OnPowerCutIn();
				}
			}
			else
			{
				foreach (var item in _powerUsers.ToList())
				{
					item.OnPowerCutOut();
				}

				_powerUsers.Clear();
			}
		}
	}

	#endregion

	#region IProducePower Implementation

	public bool PrimaryLoadTimePowerProducer => true;
	public bool PrimaryExternalConnectionPowerProducer => false;
	public bool ProducingPower { get; private set; }

	public double FuelLevel => 1.0;

	private readonly List<IConsumePower> _connectedConsumers = new();
	private readonly List<IConsumePower> _powerUsers = new();

	public void BeginDrawdown(IConsumePower item)
	{
		_connectedConsumers.Add(item);

		if (ProducingPower)
		{
			_powerUsers.Add(item);
			item.OnPowerCutIn();
		}
	}

	public void EndDrawdown(IConsumePower item)
	{
		_connectedConsumers.Remove(item);
		if (_powerUsers.Contains(item))
		{
			item.OnPowerCutOut();
		}

		_powerUsers.Remove(item);
	}

	public bool CanBeginDrawDown(double wattage)
	{
		return _prototype.Wattage - _powerUsers.Sum(x => x.PowerConsumptionInWatts) - wattage >= 0;
	}

	public bool CanDrawdownSpike(double wattage)
	{
		return (_prototype.Wattage - _powerUsers.Sum(x => x.PowerConsumptionInWatts)) * 3600 >= wattage;
	}

	public bool DrawdownSpike(double wattage)
	{
		return CanDrawdownSpike(wattage);
	}

	public double MaximumPowerInWatts => ProducingPower ? _prototype.Wattage : 0.0;

	#endregion
}