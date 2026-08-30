using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class PowerBankGameItemComponent : ConnectableGameItemComponent, IProducePower, IConsumePower
{
	private PowerBankGameItemComponentProto _powerBankPrototype;
	private IProducePower? _inputSource;
	private bool _inputPowered;
	private bool _heartbeatOn;
	private readonly List<IConsumePower> _connectedConsumers = [];
	private readonly List<IConsumePower> _powerUsers = [];

	public PowerBankGameItemComponent(PowerBankGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(proto, parent, temporary)
	{
		_powerBankPrototype = proto;
		WattHoursRemaining = proto.CapacityInWattHours;
		SubscribeToConnections();
	}

	public PowerBankGameItemComponent(MudSharp.Models.GameItemComponent component, PowerBankGameItemComponentProto proto,
		IGameItem parent) : base(component, proto, parent)
	{
		_powerBankPrototype = proto;
		var root = XElement.Parse(component.Definition);
		WattHoursRemaining = Math.Clamp(double.Parse(root.Element("WattHoursRemaining")?.Value ??
			proto.CapacityInWattHours.ToString()), 0.0, proto.CapacityInWattHours);
		SubscribeToConnections();
	}

	public PowerBankGameItemComponent(PowerBankGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_powerBankPrototype = rhs._powerBankPrototype;
		WattHoursRemaining = rhs.WattHoursRemaining;
		SubscribeToConnections();
	}

	private void SubscribeToConnections()
	{
		Parent.OnConnected += ParentOnConnected;
		Parent.OnDisconnected += ParentOnDisconnected;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new PowerBankGameItemComponent(this, newParent, temporary);

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_powerBankPrototype = (PowerBankGameItemComponentProto)newProto;
		WattHoursRemaining = Math.Min(WattHoursRemaining, _powerBankPrototype.CapacityInWattHours);
	}

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		root.Add(new XElement("WattHoursRemaining", WattHoursRemaining));
		return root.ToString();
	}

	public double WattHoursRemaining { get; private set; }
	public bool PrimaryLoadTimePowerProducer => false;
	public bool PrimaryExternalConnectionPowerProducer => true;
	public double FuelLevel => _powerBankPrototype.CapacityInWattHours > 0.0
		? WattHoursRemaining / _powerBankPrototype.CapacityInWattHours : 0.0;
	public bool ProducingPower => WattHoursRemaining > 0.0;
	public double MaximumPowerInWatts => ProducingPower ? _powerBankPrototype.MaximumOutputInWatts : 0.0;

	public void BeginDrawdown(IConsumePower item)
	{
		if (!IsConnectedToOutput(item))
		{
			return;
		}

		if (!_connectedConsumers.Contains(item))
		{
			_connectedConsumers.Add(item);
		}

		TryPowerConsumer(item);
		CheckHeartbeat();
	}

	public void EndDrawdown(IConsumePower item)
	{
		_connectedConsumers.Remove(item);
		if (_powerUsers.Remove(item))
		{
			item.OnPowerCutOut();
		}
		CheckHeartbeat();
	}

	private void TryPowerConsumer(IConsumePower item)
	{
		if (_powerUsers.Contains(item) || !ProducingPower ||
			_powerUsers.Sum(x => Math.Max(0.0, x.PowerConsumptionInWatts)) +
			Math.Max(0.0, item.PowerConsumptionInWatts) > _powerBankPrototype.MaximumOutputInWatts)
		{
			return;
		}

		_powerUsers.Add(item);
		item.OnPowerCutIn();
	}

	public bool CanBeginDrawDown(double wattage) => ProducingPower && wattage >= 0.0 &&
		_powerUsers.Sum(x => Math.Max(0.0, x.PowerConsumptionInWatts)) + wattage <=
		_powerBankPrototype.MaximumOutputInWatts;
	public bool CanDrawdownSpike(double wattage) => ProducingPower && wattage >= 0.0 &&
		wattage <= _powerBankPrototype.MaximumOutputInWatts && WattHoursRemaining >= wattage / 3600.0;
	public bool DrawdownSpike(double wattage)
	{
		if (!CanDrawdownSpike(wattage))
		{
			return false;
		}
		WattHoursRemaining -= wattage / 3600.0;
		Changed = true;
		CheckHeartbeat();
		return true;
	}

	public double PowerConsumptionInWatts => _inputSource is not null && WattHoursRemaining < _powerBankPrototype.CapacityInWattHours
		? _powerBankPrototype.MaximumInputInWatts : 0.0;
	public void OnPowerCutIn()
	{
		_inputPowered = true;
		CheckHeartbeat();
	}
	public void OnPowerCutOut()
	{
		_inputPowered = false;
		CheckHeartbeat();
	}

	private void CheckHeartbeat()
	{
		var needed = (_inputPowered && WattHoursRemaining < _powerBankPrototype.CapacityInWattHours) || _powerUsers.Any();
		if (needed && !_heartbeatOn)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat += Heartbeat;
			_heartbeatOn = true;
		}
		else if (!needed && _heartbeatOn)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat -= Heartbeat;
			_heartbeatOn = false;
		}
	}

	private void Heartbeat()
	{
		EnforceOutputLimit();
		var output = _powerUsers.Sum(x => Math.Max(0.0, x.PowerConsumptionInWatts));
		var input = _inputPowered ? _powerBankPrototype.MaximumInputInWatts : 0.0;
		WattHoursRemaining = PowerBankEnergyMath.ResolveWattHours(WattHoursRemaining,
			_powerBankPrototype.CapacityInWattHours, input, _powerBankPrototype.ChargingEfficiency, output,
			TimeSpan.FromSeconds(1.0));
		Changed = true;
		if (WattHoursRemaining <= 0.0)
		{
			foreach (var user in _powerUsers.ToList())
			{
				user.OnPowerCutOut();
			}
			_powerUsers.Clear();
		}
		else
		{
			foreach (var consumer in _connectedConsumers)
			{
				TryPowerConsumer(consumer);
			}
		}
		CheckHeartbeat();
	}

	private void EnforceOutputLimit()
	{
		var admittedWatts = 0.0;
		foreach (var user in _powerUsers.ToList())
		{
			var requestedWatts = Math.Max(0.0, user.PowerConsumptionInWatts);
			if (admittedWatts + requestedWatts <= _powerBankPrototype.MaximumOutputInWatts)
			{
				admittedWatts += requestedWatts;
				continue;
			}

			_powerUsers.Remove(user);
			user.OnPowerCutOut();
		}
	}

	private bool IsInputConnection(ConnectorType type)
	{
		return _powerBankPrototype.InputConnections.Any(x => ReferenceEquals(x, type));
	}

	private bool IsOutputConnection(ConnectorType type)
	{
		return _powerBankPrototype.OutputConnections.Any(x => ReferenceEquals(x, type));
	}

	private bool IsConnectedToOutput(IConsumePower item)
	{
		return ConnectedItems.Any(x => x.Item2.Parent == item.Parent && IsOutputConnection(x.Item1));
	}

	private void ParentOnConnected(IConnectable other, ConnectorType type)
	{
		if (!IsInputConnection(type))
		{
			return;
		}
		_inputSource?.EndDrawdown(this);
		_inputSource = other.Parent.GetItemTypes<IProducePower>()
			.FirstOrDefault(x => x.PrimaryExternalConnectionPowerProducer || x.MaximumPowerInWatts > 0.0);
		_inputSource?.BeginDrawdown(this);
	}

	private void ParentOnDisconnected(IConnectable other, ConnectorType type)
	{
		foreach (var consumer in _connectedConsumers.Where(x => x.Parent == other.Parent).ToList())
		{
			EndDrawdown(consumer);
		}

		if (_inputSource?.Parent != other.Parent)
		{
			return;
		}
		_inputSource.EndDrawdown(this);
		_inputSource = null;
		_inputPowered = false;
		CheckHeartbeat();
	}

	public override void Login()
	{
		base.Login();
		RestoreInputSource();
	}

	public override void Quit()
	{
		ReleasePower();
		base.Quit();
	}

	public override void Delete()
	{
		ReleasePower();
		base.Delete();
	}

	private void ReleasePower()
	{
		_inputSource?.EndDrawdown(this);
		_inputSource = null;
		_inputPowered = false;
		if (_heartbeatOn)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat -= Heartbeat;
			_heartbeatOn = false;
		}
		foreach (var user in _powerUsers.ToList())
		{
			user.OnPowerCutOut();
		}
		_powerUsers.Clear();
	}

	private void RestoreInputSource()
	{
		if (_inputSource is not null)
		{
			return;
		}

		var inputConnection = ConnectedItems.FirstOrDefault(x => IsInputConnection(x.Item1));
		if (inputConnection is null)
		{
			return;
		}

		_inputSource = inputConnection.Item2.Parent.GetItemTypes<IProducePower>()
			.FirstOrDefault(x => x.PrimaryExternalConnectionPowerProducer || x.MaximumPowerInWatts > 0.0);
		_inputSource?.BeginDrawdown(this);
	}
}
