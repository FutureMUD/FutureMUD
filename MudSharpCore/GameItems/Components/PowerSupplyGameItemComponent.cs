using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class PowerSupplyGameItemComponent : GameItemComponent, IProducePower, IConsumePower
{
	protected PowerSupplyGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PowerSupplyGameItemComponentProto)newProto;
	}

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	#region IProducePower Implementation

	public bool PrimaryLoadTimePowerProducer => false;
	public bool PrimaryExternalConnectionPowerProducer => false;
	public double FuelLevel => _connectedPowerSource?.FuelLevel ?? 0.0;
	private readonly List<IConsumePower> _connectedConsumers = new();
	private readonly List<IConsumePower> _powerUsers = new();
	public double MaximumPowerInWatts => _connectedPowerSource?.MaximumPowerInWatts ?? 0.0;

	public void BeginDrawdown(IConsumePower item)
	{
		if (!_connectedConsumers.Contains(item))
		{
			_connectedConsumers.Add(item);
		}

		if (ProducingPower && !_powerUsers.Contains(item))
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
		return _connectedPowerSource?.CanBeginDrawDown(wattage) ?? false;
	}

	public bool CanDrawdownSpike(double wattage)
	{
		return _connectedPowerSource?.CanDrawdownSpike(wattage) ?? false;
	}

	public bool DrawdownSpike(double wattage)
	{
		return _connectedPowerSource?.DrawdownSpike(wattage) ?? false;
	}

	public bool ProducingPower => _connectedPowerSource?.ProducingPower ?? false;

	private IProducePower _connectedPowerSource;
	private ConnectorType _connectedPowerSourceConnector;

	private void Parent_OnConnected(IConnectable other, ConnectorType type)
	{
		if (!type.Powered)
		{
			return;
		}

		var power = other.Parent.GetItemTypes<IProducePower>()
		                 .FirstOrDefault(x => x.PrimaryExternalConnectionPowerProducer);
		if (power == null)
		{
			return;
		}

		_connectedPowerSource = power;
		_connectedPowerSourceConnector = type;
		power.BeginDrawdown(this);
	}

	private void Parent_OnDisconnected(IConnectable other, ConnectorType type)
	{
		if (other.Parent == _connectedPowerSource?.Parent && type.Equals(_connectedPowerSourceConnector))
		{
			if (_connectedPowerSource.ProducingPower)
			{
				OnPowerCutOut();
			}

			_connectedPowerSource.EndDrawdown(this);
			_connectedPowerSource = null;
			_connectedPowerSourceConnector = null;
		}
	}

	#endregion

	#region IConsumePower Implementation

	public double PowerConsumptionInWatts => _powerUsers.Sum(x => x.PowerConsumptionInWatts);

	public void OnPowerCutIn()
	{
		foreach (var item in _connectedConsumers.Where(x => !_powerUsers.Contains(x)).ToList())
		{
			_powerUsers.Add(item);
		}

		foreach (var item in _powerUsers)
		{
			item.OnPowerCutIn();
		}
	}

	public void OnPowerCutOut()
	{
		foreach (var item in _powerUsers)
		{
			item.OnPowerCutOut();
		}

		_powerUsers.Clear();
	}

	#endregion

	#region Constructors

	public PowerSupplyGameItemComponent(PowerSupplyGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		parent.OnConnected += Parent_OnConnected;
		parent.OnDisconnected += Parent_OnDisconnected;
	}

	public PowerSupplyGameItemComponent(MudSharp.Models.GameItemComponent component,
		PowerSupplyGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
		parent.OnConnected += Parent_OnConnected;
		parent.OnDisconnected += Parent_OnDisconnected;
	}

	public PowerSupplyGameItemComponent(PowerSupplyGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		newParent.OnConnected += Parent_OnConnected;
		newParent.OnDisconnected += Parent_OnDisconnected;
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PowerSupplyGameItemComponent(this, newParent, temporary);
	}

	#endregion
}