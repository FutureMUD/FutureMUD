using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class PowerBankGameItemComponent : ConnectableGameItemComponent, IProducePower, IConsumePower
{
	private PowerBankGameItemComponentProto _powerBankPrototype;
	private IProducePower? _inputSource;
	private bool _inputPowered;
	private bool _active;
	private bool _deleted;
	private bool _heartbeatOn;
	private readonly List<IConsumePower> _connectedConsumers = [];
	private readonly List<IConsumePower> _powerUsers = [];

	public PowerBankGameItemComponent(PowerBankGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(proto, parent, temporary)
	{
		_powerBankPrototype = proto;
		WattHoursRemaining = proto.CapacityInWattHours;
	}

	public PowerBankGameItemComponent(MudSharp.Models.GameItemComponent component, PowerBankGameItemComponentProto proto,
		IGameItem parent) : base(component, proto, parent)
	{
		_powerBankPrototype = proto;
		var root = XElement.Parse(component.Definition);
		WattHoursRemaining = Math.Clamp(double.Parse(root.Element("WattHoursRemaining")?.Value ??
			proto.CapacityInWattHours.ToString()), 0.0, proto.CapacityInWattHours);
	}

	public PowerBankGameItemComponent(PowerBankGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_powerBankPrototype = rhs._powerBankPrototype;
		WattHoursRemaining = rhs.WattHoursRemaining;
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
	public bool ProducingPower => _active && WattHoursRemaining > 0.0;
	public double MaximumPowerInWatts => ProducingPower ? _powerBankPrototype.MaximumOutputInWatts : 0.0;

	public void BeginDrawdown(IConsumePower item)
	{
		if (_deleted || !IsConnectedToOutput(item))
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
		if (!_connectedConsumers.Contains(item) || _powerUsers.Contains(item) || !ProducingPower ||
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
	public bool CanDrawdownSpike(double wattage, TimeSpan duration)
	{
		var wattHours = wattage * duration.TotalHours;
		return ProducingPower && double.IsFinite(wattage) && wattage >= 0.0 && duration > TimeSpan.Zero &&
		       double.IsFinite(duration.TotalHours) && double.IsFinite(wattHours) &&
		       wattage <= _powerBankPrototype.MaximumOutputInWatts && WattHoursRemaining >= wattHours;
	}
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
	public bool DrawdownSpike(double wattage, TimeSpan duration)
	{
		if (!CanDrawdownSpike(wattage, duration))
		{
			return false;
		}
		WattHoursRemaining -= wattage * duration.TotalHours;
		Changed = true;
		CheckHeartbeat();
		return true;
	}

	public double PowerConsumptionInWatts => _active && _inputSource is not null && WattHoursRemaining < _powerBankPrototype.CapacityInWattHours
		? _powerBankPrototype.MaximumInputInWatts : 0.0;
	public void OnPowerCutIn()
	{
		if (!_active || _inputSource is null) return;
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
		var needed = _active &&
		             ((_inputPowered && WattHoursRemaining < _powerBankPrototype.CapacityInWattHours) || _powerUsers.Count > 0);
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
		if (!_active) return;
		EnforceOutputLimit();
		if (!_active) return;
		var output = _powerUsers.Sum(x => Math.Max(0.0, x.PowerConsumptionInWatts));
		var input = _inputPowered ? _powerBankPrototype.MaximumInputInWatts : 0.0;
		WattHoursRemaining = PowerBankEnergyMath.ResolveWattHours(WattHoursRemaining,
			_powerBankPrototype.CapacityInWattHours, input, _powerBankPrototype.ChargingEfficiency, output,
			TimeSpan.FromSeconds(1.0));
		Changed = true;
		if (WattHoursRemaining <= 0.0)
		{
			CutPowerToUsers();
		}
		else
		{
			foreach (var consumer in _connectedConsumers.ToArray())
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
			if (!_powerUsers.Contains(user)) continue;
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
		if (!_active || !IsInputConnection(type))
		{
			return;
		}
		ReleaseInputSource();
		RestoreInputSource();
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
		ReleaseInputSource();
		CheckHeartbeat();
	}

	public override void Login()
	{
		if (_active || _deleted) return;
		base.Login();
		if (Parent.Deleted || _deleted) return;
		_active = true;
		Parent.OnConnected += ParentOnConnected;
		Parent.OnDisconnected += ParentOnDisconnected;
		RestoreInputSource();
		foreach (var consumer in _connectedConsumers.ToArray())
		{
			TryPowerConsumer(consumer);
		}
		CheckHeartbeat();
	}

	public override void Quit()
	{
		ReleasePower();
		base.Quit();
	}

	public override void Delete()
	{
		_deleted = true;
		ReleasePower();
		base.Delete();
		_connectedConsumers.Clear();
	}

	private void ReleasePower()
	{
		if (!_active) return;
		_active = false;
		Parent.OnConnected -= ParentOnConnected;
		Parent.OnDisconnected -= ParentOnDisconnected;
		ReleaseInputSource();
		CheckHeartbeat();
		CutPowerToUsers();
	}

	private void ReleaseInputSource()
	{
		var source = _inputSource;
		_inputSource = null;
		_inputPowered = false;
		source?.EndDrawdown(this);
	}

	private void CutPowerToUsers()
	{
		foreach (var user in _powerUsers.ToArray())
		{
			if (_powerUsers.Remove(user)) user.OnPowerCutOut();
		}
	}

	private void RestoreInputSource()
	{
		if (!_active || _inputSource is not null)
		{
			return;
		}

		var inputConnection = ConnectedItems.FirstOrDefault(x => IsInputConnection(x.Item1));
		if (inputConnection is null)
		{
			return;
		}

		_inputSource = inputConnection.Item2.Parent.GetItemTypes<IProducePower>()
			.FirstOrDefault(x => !ReferenceEquals(x, this) &&
			                     (x.PrimaryExternalConnectionPowerProducer || x.MaximumPowerInWatts > 0.0));
		_inputSource?.BeginDrawdown(this);
	}
}
