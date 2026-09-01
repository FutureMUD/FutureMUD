#nullable enable

using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public abstract class AccessControlReaderGameItemComponent : PoweredMachineBaseGameItemComponent,
	IAccessControlReader
{
	private AccessControlReaderGameItemComponentProto _accessPrototype;
	private ComputerSignal _currentSignal;
	private DateTime? _activeUntil;
	private bool _heartbeatSubscribed;
	private IConnectable? _mountedHost;
	private long? _pendingMountedHostId;
	private long _selfTargetLockPrototypeId;
	private string _selfTargetLockPrototypeName = string.Empty;

	protected AccessControlReaderGameItemComponent(AccessControlReaderGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_accessPrototype = proto;
		_selfTargetLockPrototypeId = proto.SelfTargetLockPrototypeId;
		_selfTargetLockPrototypeName = proto.SelfTargetLockPrototypeName;
	}

	protected AccessControlReaderGameItemComponent(MudSharp.Models.GameItemComponent component,
		AccessControlReaderGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_accessPrototype = proto;
		LoadAccessState(XElement.Parse(component.Definition));
	}

	protected AccessControlReaderGameItemComponent(AccessControlReaderGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_accessPrototype = rhs._accessPrototype;
		_activeUntil = rhs._activeUntil;
		_currentSignal = rhs._currentSignal;
		_selfTargetLockPrototypeId = rhs._selfTargetLockPrototypeId;
		_selfTargetLockPrototypeName = rhs._selfTargetLockPrototypeName;
	}

	public long LocalSignalSourceIdentifier => Prototype.Id;
	public string EndpointKey => SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public ComputerSignal CurrentSignal => _currentSignal;
	public event SignalChangedEvent? SignalChanged;
	public double CurrentValue => _currentSignal.Value;
	public TimeSpan? Duration => _currentSignal.Duration;
	public TimeSpan? PulseInterval => _currentSignal.PulseInterval;
	public string MountType => _accessPrototype.AccessMountType;
	public bool IsMounted => ResolveMountedHostConnectable() is not null || _pendingMountedHostId.HasValue;
	public IAutomationMountHost? MountHost => ResolveMountedHost();
	private ConnectorType MountConnector => new(Gender.Male, $"Automation:{MountType}", false);
	public IEnumerable<ConnectorType> Connections => [MountConnector];
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems =>
		ResolveMountedHostConnectable() is { } host ? [Tuple.Create(MountConnector, host)] : [];
	public IEnumerable<ConnectorType> FreeConnections => ResolveMountedHostConnectable() is null ? Connections : [];
	public bool Independent => false;
	public long SelfTargetLockPrototypeId => _selfTargetLockPrototypeId;

	protected void LoadAccessState(XElement root)
	{
		if (long.TryParse(root.Element("ActiveUntilTicks")?.Value, out var ticks) && ticks > 0)
		{
			_activeUntil = new DateTime(ticks, DateTimeKind.Utc);
		}

		if (long.TryParse(root.Element("MountedTo")?.Value, out var mountedId) && mountedId > 0)
		{
			_pendingMountedHostId = mountedId;
		}

		_selfTargetLockPrototypeId = long.TryParse(root.Element("SelfTargetLockPrototypeId")?.Value, out var targetId)
			? targetId
			: _accessPrototype.SelfTargetLockPrototypeId;
		_selfTargetLockPrototypeName = root.Element("SelfTargetLockPrototypeName")?.Value ??
										_accessPrototype.SelfTargetLockPrototypeName;
	}

	protected sealed override XElement SaveToXml(XElement root)
	{
		root.Add(new XElement("ActiveUntilTicks", _activeUntil?.Ticks ?? 0));
		var mountedHostId = _mountedHost?.Parent.Id ?? _pendingMountedHostId;
		if (mountedHostId.HasValue)
		{
			root.Add(new XElement("MountedTo", mountedHostId.Value));
		}
		root.Add(new XElement("SelfTargetLockPrototypeId", _selfTargetLockPrototypeId));
		root.Add(new XElement("SelfTargetLockPrototypeName", new XCData(_selfTargetLockPrototypeName)));

		return SaveAccessSubtypeToXml(root);
	}

	protected abstract XElement SaveAccessSubtypeToXml(XElement root);

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_accessPrototype = (AccessControlReaderGameItemComponentProto)newProto;
	}

	public bool ActivateAccessSignal()
	{
		if (!SwitchedOn || !_onAndPowered)
		{
			return false;
		}

		_activeUntil = DateTime.UtcNow + _accessPrototype.SignalDuration;
		Changed = true;
		EnsureHeartbeatSubscription();
		RefreshSignalState(true);
		return true;
	}

	public bool TrySetSelfTarget(ILock? target, out string error)
	{
		if (target is not null && !ReferenceEquals(target.Parent, Parent))
		{
			error = "A self-target lock must be a sibling component on the same item.";
			return false;
		}
		var targetId = target?.Prototype.Id ?? 0L;
		var targetName = target?.Prototype.Name ?? string.Empty;
		if (_selfTargetLockPrototypeId == targetId && _selfTargetLockPrototypeName.EqualTo(targetName))
		{
			error = "That is already the reader's self-target selection.";
			return false;
		}

		ApplySelfTarget(false);
		_selfTargetLockPrototypeId = targetId;
		_selfTargetLockPrototypeName = targetName;
		Changed = true;
		ApplySelfTarget(_activeUntil is not null && _activeUntil > DateTime.UtcNow && SwitchedOn && _onAndPowered);
		error = string.Empty;
		return true;
	}

	public override void FinaliseLoad()
	{
		ResolveMountedHost();
		base.FinaliseLoad();
		NormaliseDeadline();
		RefreshSignalState(false);
	}

	public override void Login()
	{
		ResolveMountedHost();
		base.Login();
		NormaliseDeadline();
		if (_activeUntil is not null)
		{
			EnsureHeartbeatSubscription();
		}
		RefreshSignalState(false);
	}

	public override void Delete()
	{
		RemoveHeartbeatSubscription();
		base.Delete();
	}

	public override void Quit()
	{
		RemoveHeartbeatSubscription();
		base.Quit();
	}

	protected override void OnPowerCutInAction()
	{
		RefreshSignalState(true);
	}

	protected override void OnPowerCutOutAction()
	{
		SetCurrentSignal(default, true);
		ApplySelfTarget(false);
		HandleDescriptionUpdate();
	}

	private void NormaliseDeadline()
	{
		if (_activeUntil is not null && _activeUntil <= DateTime.UtcNow)
		{
			_activeUntil = null;
		}
	}

	private void HeartbeatTick()
	{
		if (_activeUntil is not null && _activeUntil > DateTime.UtcNow)
		{
			return;
		}

		_activeUntil = null;
		RemoveHeartbeatSubscription();
		RefreshSignalState(true);
	}

	private void EnsureHeartbeatSubscription()
	{
		if (_heartbeatSubscribed)
		{
			return;
		}
		Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatTick;
		_heartbeatSubscribed = true;
	}

	private void RemoveHeartbeatSubscription()
	{
		if (!_heartbeatSubscribed)
		{
			return;
		}
		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatTick;
		_heartbeatSubscribed = false;
	}

	private void RefreshSignalState(bool markChanged)
	{
		var active = _activeUntil is not null && _activeUntil > DateTime.UtcNow && SwitchedOn && _onAndPowered;
		SetCurrentSignal(active
			? new ComputerSignal(_accessPrototype.SignalValue, _accessPrototype.SignalDuration, null)
			: default, markChanged);
		ApplySelfTarget(active);
		HandleDescriptionUpdate();
	}

	private void SetCurrentSignal(ComputerSignal signal, bool markChanged)
	{
		if (SignalComponentUtilities.SignalsEqual(_currentSignal, signal))
		{
			return;
		}
		_currentSignal = signal;
		if (markChanged)
		{
			Changed = true;
		}
		SignalChanged?.Invoke(this, signal);
	}

	private void ApplySelfTarget(bool active)
	{
		if (_selfTargetLockPrototypeId <= 0 && string.IsNullOrWhiteSpace(_selfTargetLockPrototypeName))
		{
			return;
		}

		var target = Parent.GetItemTypes<ILock>().FirstOrDefault(x =>
			x.Prototype.Id == _selfTargetLockPrototypeId ||
			(!string.IsNullOrWhiteSpace(_selfTargetLockPrototypeName) &&
			 x.Prototype.Name.EqualTo(_selfTargetLockPrototypeName)));
		target?.SetLocked(!active, true);
	}

	private IConnectable? ResolveMountedHostConnectable()
	{
		if (_mountedHost is not null)
		{
			return _mountedHost;
		}
		ResolveMountedHost();
		return _mountedHost;
	}

	private IAutomationMountHost? ResolveMountedHost()
	{
		if (_mountedHost is IAutomationMountHost host)
		{
			return host;
		}
		if (_mountedHost?.Parent.GetItemType<IAutomationMountHost>() is { } parentHost)
		{
			return parentHost;
		}
		if (!_pendingMountedHostId.HasValue)
		{
			return null;
		}
		var hostItem = Gameworld.TryGetItem(_pendingMountedHostId.Value, true);
		var resolved = hostItem?.GetItemTypes<IAutomationMountHost>()
			.FirstOrDefault(x => x.GetBayNameForMountedItem(Parent) is not null);
		_mountedHost = resolved as IConnectable;
		if (resolved is not null)
		{
			_pendingMountedHostId = null;
		}
		return resolved;
	}

	public bool CanBeConnectedTo(IConnectable other) => false;
	public bool CanConnect(ICharacter? actor, IConnectable other) =>
		_mountedHost is null && other.FreeConnections.Any(x => x.CompatibleWith(MountConnector)) &&
		other.CanBeConnectedTo(this);
	public void Connect(ICharacter? actor, IConnectable other)
	{
		_mountedHost = other;
		_pendingMountedHostId = null;
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(MountConnector)));
		RefreshPowerSourceConnection();
		Changed = true;
	}
	public void RawConnect(IConnectable other, ConnectorType type)
	{
		_mountedHost = other;
		_pendingMountedHostId = null;
		Parent.ConnectedItem(other, type);
		RefreshPowerSourceConnection();
		Changed = true;
	}
	public string WhyCannotConnect(ICharacter? actor, IConnectable other) => IsMounted
		? $"{Parent.HowSeen(actor)} is already mounted."
		: $"There is no compatible {MountType.ColourCommand()} automation bay.";
	public bool CanBeDisconnectedFrom(IConnectable other) => true;
	public bool CanDisconnect(ICharacter actor, IConnectable other) => ReferenceEquals(_mountedHost, other);
	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
		if (actor?.Body.CanGet(Parent, 0) == true)
		{
			actor.Body.Get(Parent, silent: true);
		}
		else
		{
			ILocateable source = (ILocateable?)actor ?? Parent;
			Parent.InsertAtSource(source);
		}
	}
	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		_mountedHost = null;
		_pendingMountedHostId = null;
		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			Parent.DisconnectedItem(other, MountConnector);
			other.Parent.DisconnectedItem(this, MountConnector);
		}
		RefreshPowerSourceConnection();
		Changed = true;
	}
	public string WhyCannotDisconnect(ICharacter actor, IConnectable other) =>
		$"{Parent.HowSeen(actor)} is not installed in {other.Parent.HowSeen(actor)}.";
}
