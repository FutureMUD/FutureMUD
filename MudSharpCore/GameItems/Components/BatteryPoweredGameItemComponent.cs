using MudSharp.GameItems;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.GameItems.Prototypes;

#nullable enable
#nullable disable warnings

namespace MudSharp.GameItems.Components;

public class BatteryPoweredGameItemComponent : GameItemComponent, IContainer, IOpenable, IProducePower, IConsumePower, IConnectable
{
    protected BatteryPoweredGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;
    private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
    private IProducePower? _connectedPowerSource;
    private ConnectorType? _connectedPowerSourceConnector;
    private bool _charging;
    private bool _active;

    public override void Delete()
    {
        base.Delete();
        SuspendPower();
        foreach (IGameItem item in Contents.ToList())
        {
            DeleteOwnedContainedChild(item);
            _contents.Remove(item);
        }

        foreach (ILock item in Locks.ToList())
        {
            DeleteOwnedContainedChild(item.Parent);
            _locks.Remove(item);
        }

        _batteries.Clear();
        foreach (var peer in _connectedItems.Select(x => x.Item2).Distinct().ToArray()) RawDisconnect(peer, true);
        _connectedConsumers.Clear();
        _pendingLoadTimeConnections.Clear();
        _pendingDependentLoadTimeConnections.Clear();
    }

    public override void Quit()
    {
        SuspendPower();
        foreach (IGameItem item in Contents.ToArray())
        {
            item.Quit();
        }

        foreach (ILock item in Locks.ToArray())
        {
            item.Quit();
        }
    }

    public override void Login()
    {
        if (_active) return;
        FinaliseLoad();
        if (Parent.Deleted) return;
        _active = true;
        Parent.OnConnected += Parent_OnConnected;
        Parent.OnDisconnected += Parent_OnDisconnected;
        foreach (var item in Contents.ToArray())
        {
            if (Parent.Deleted || !_active) return;
            if (_contents.Contains(item)) item.Login();
        }

        foreach (var theLock in Locks.ToArray())
        {
            if (Parent.Deleted || !_active) return;
            if (_locks.Contains(theLock)) theLock.Parent.Login();
        }

        if (Parent.Deleted || !_active) return;
        foreach (var (type, peer) in _connectedItems.ToArray())
        {
            Parent_OnConnected(peer, type);
            if (_connectedPowerSource is not null) break;
        }
        CheckHeartbeat();
    }

    private void SuspendPower()
    {
        if (!_active) return;
        _active = false;
        Parent.OnConnected -= Parent_OnConnected;
        Parent.OnDisconnected -= Parent_OnDisconnected;
        var source = _connectedPowerSource;
        _connectedPowerSource = null;
        _connectedPowerSourceConnector = null;
        _charging = false;
        source?.EndDrawdown(this);
        EndHeartbeat();
    }

    public override bool HandleDieOrMorph(IGameItem newItem, IRoom location)
    {
        ILockable newItemLockable = newItem?.GetItemType<ILockable>();
        if (newItemLockable != null)
        {
            foreach (ILock thelock in Locks)
            {
                newItemLockable.InstallLock(thelock);
            }
        }
        else
        {
            foreach (ILock thelock in Locks)
            {
                if (location != null)
                {
                    InsertAtParentSpatialLocation(thelock.Parent, location);
                    thelock.Parent.ContainedIn = null;
                }
                else
                {
                    thelock.Parent.Delete();
                }
            }
        }

        _locks.Clear();

        IContainer newItemContainer = newItem?.GetItemType<IContainer>();
        if (newItemContainer != null)
        {
            IOpenable newItemOpenable = newItem.GetItemType<IOpenable>();
            if (newItemOpenable != null)
            {
                if (IsOpen)
                {
                    newItemOpenable.Open();
                }
                else
                {
                    newItemOpenable.Close();
                }
            }

            if (Contents.Any())
            {
                foreach (IGameItem item in Contents.ToList())
                {
                    if (newItemContainer.CanPut(item))
                    {
                        newItemContainer.Put(null, item);
                    }
                    else if (location != null)
                    {
                        InsertAtParentSpatialLocation(item, location);
                        item.ContainedIn = null;
                    }
                    else
                    {
                        item.Delete();
                    }
                }

                _contents.Clear();
            }
        }
        else
        {
            foreach (IGameItem item in Contents.ToList())
            {
                if (location != null)
                {
                    InsertAtParentSpatialLocation(item, location);
                    item.ContainedIn = null;
                }
                else
                {
                    item.Delete();
                }
            }

            _contents.Clear();
        }

        return false;
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return (IsOpen && type == DescriptionType.Contents) || type == DescriptionType.Full;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        switch (type)
        {
            case DescriptionType.Contents:
                if (_contents.Any())
                {
                    return description + "\n\nIt contains the following batteries:\n" +
                           (from item in _contents
                            select "\t" + item.HowSeen(voyeur)).ListToString(separator: "\n", conjunction: "",
                               twoItemJoiner: "\n");
                }

                return description + "\n\nIt does not currently contain any batteries.";
            case DescriptionType.Full:
                StringBuilder sb = new();
                sb.Append(description);
                sb.AppendLine();
                sb.AppendLine(
                    $"It accepts {_prototype.BatteryQuantity} batteries of type {_prototype.BatteryType.Colour(Telnet.Cyan)}.");
                if ((voyeur as ICharacter)?.IsAdministrator() ?? false)
                {
                    sb.AppendLine(
                        $"{"[Admin Info]:".Colour(Telnet.BoldWhite)} The batteries are in the following state:");

                    foreach (IBattery battery in _batteries)
                    {
                        sb.AppendLine(
                            $"\t{battery.Parent.HowSeen(voyeur)}: {battery.WattHoursRemaining.ToString("N4", voyeur).ColourValue()} watt-hours of {battery.TotalWattHours.ToString("N4", voyeur).ColourValue()} total.");
                    }
                }

                sb.AppendLine($"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}."
                    .Colour(Telnet.Yellow));
                if (Locks.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine("It has the following locks:");
                    foreach (ILock thelock in Locks)
                    {
                        sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
                    }
                }

                return sb.ToString();
        }

        return description;
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (BatteryPoweredGameItemComponentProto)newProto;
    }

    #region Saving

    protected override string SaveToXml()
    {
        return
            new XElement("Definition", new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
                new XElement("Locks", Locks.Select(x => new XElement("Lock", x.Parent.Id)),
                    AmbiguousLegacyChildClaims("Lock")),
                Contents.Select(x => new XElement("Contained", x.Id)), AmbiguousLegacyChildClaims("Contained"),
                new XElement("ConnectedItems",
                    ConnectedItems.Select(x => SaveConnectionEntry(x.Item2.Parent.Id, x.Item1, x.Item2.Independent,
                        FindPortIndex(this, x.Item1), FindPeerPortIndex(this, x.Item2, x.Item1), FindPortId(this, x.Item1), FindPeerPortId(this, x.Item2, x.Item1))),
                    _pendingLoadTimeConnections.Select(x => SaveConnectionEntry(x.Item1, x.Item2, true, x.Item2.PortIndex,
                        x.Item2.PeerPortIndex, x.Item2.PortId, x.Item2.PeerPortId)),
                    _pendingDependentLoadTimeConnections.Select(x => SaveConnectionEntry(x.Item1, x.Item2, false, x.Item2.PortIndex,
                        x.Item2.PeerPortIndex, x.Item2.PortId, x.Item2.PeerPortId))))
            .ToString();
    }

    #endregion

    #region Constructors

    public BatteryPoweredGameItemComponent(BatteryPoweredGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public BatteryPoweredGameItemComponent(MudSharp.Models.GameItemComponent component,
        BatteryPoweredGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public BatteryPoweredGameItemComponent(BatteryPoweredGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    protected void LoadFromXml(XElement root)
    {
        XAttribute attr = root.Attribute("Open");
        if (attr != null)
        {
            _isOpen = attr.Value == "true";
        }

        foreach (var reference in root.Element("Locks")?.Elements("Lock") ?? [])
        {
            if (RestoreOwnedChild(reference)?.GetItemType<ILock>() is { } itemLock) InstallLock(itemLock);
        }

        foreach (var reference in root.Elements("Contained"))
        {
            if (RestoreOwnedChild(reference) is not { } item || item.GetItemType<IBattery>() is not { } battery) continue;
            _contents.Add(item);
            _batteries.Add(battery);
            item.LoadTimeSetContainedIn(Parent);
        }

        XElement connectors = root.Element("ConnectedItems");
        if (connectors != null)
        {
            foreach (XElement item in connectors.Elements("Item"))
            {
                ConnectorType connector = LoadSavedConnector(item);
                if (item.Attribute("independent")?.Value == "false")
                {
                    _pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value), connector));
                }
                else
                {
                    _pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value), connector));
                }
            }
        }
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new BatteryPoweredGameItemComponent(this, newParent, temporary);
    }

    public override void FinaliseLoad()
    {
        foreach (IGameItem item in _contents.ToArray())
        {
            item.FinaliseLoadTimeTasks();
        }

        RestorePendingConnections(this, _pendingLoadTimeConnections, loadMissingItems: false);
        RestorePendingConnections(this, _pendingDependentLoadTimeConnections, loadMissingItems: true);
    }

    internal override IEnumerable<ConnectorType> PendingPorts(long peerId, bool independent) =>
        (independent ? _pendingLoadTimeConnections : _pendingDependentLoadTimeConnections)
        .Where(x => x.Item1 == peerId).Select(x => x.Item2);

    #endregion

    #region IProducePower Implementation

    private bool _heartbeatOn;

    private void CheckHeartbeat()
    {
        if (!_active) return;
        bool hasChargeRoom = _charging && _batteries.Any(x => x.Rechargable && x.WattHoursRemaining < x.TotalWattHours);
        bool needsHeartbeat = (_connectedConsumers.Any() && ProducingPower) || hasChargeRoom;
        if (_heartbeatOn && !needsHeartbeat)
        {
            EndHeartbeat();
            return;
        }

        if (!_heartbeatOn && needsHeartbeat)
        {
            Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManagerOnSecondHeartbeat;
            _heartbeatOn = true;
        }

        foreach (var user in _powerUsers.ToArray())
        {
            if (!ProducingPower && _powerUsers.Remove(user)) user.OnPowerCutOut();
        }

        foreach (var user in _connectedConsumers.ToArray())
        {
            if (!ProducingPower) break;
            if (!_connectedConsumers.Contains(user) || _powerUsers.Contains(user)) continue;
            _powerUsers.Add(user);
            user.OnPowerCutIn();
        }
    }

    private void HeartbeatManagerOnSecondHeartbeat()
    {
        if (!_active || !_heartbeatOn) return;
        double powerUsage = _powerUsers.Sum(x => x.PowerConsumptionInWatts) / 3600.0 /
                         (_prototype.BatteriesInSeries ? 1.0 : _prototype.BatteryQuantity);
#if DEBUG
        if (powerUsage < 0)
        {
            Console.WriteLine("Negative power usage!");
        }
#endif
        foreach (IBattery battery in _batteries)
        {
            battery.WattHoursRemaining -= powerUsage;
        }

        if (_charging)
        {
            double chargeRate = _prototype.ChargeWattage / 3600.0 /
                             (_prototype.BatteriesInSeries ? 1.0 : _prototype.BatteryQuantity);
            foreach (IBattery battery in _batteries.Where(x => x.Rechargable))
            {
                battery.WattHoursRemaining = Math.Min(battery.TotalWattHours,
                    battery.WattHoursRemaining + chargeRate);
            }
        }

        CheckHeartbeat();
    }

    private void EndHeartbeat()
    {
        if (_heartbeatOn)
        {
            Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
            _heartbeatOn = false;
            foreach (IConsumePower user in _powerUsers.ToList())
            {
                _powerUsers.Remove(user);
                user.OnPowerCutOut();
            }
        }
    }

    public bool PrimaryLoadTimePowerProducer => true;
    public bool PrimaryExternalConnectionPowerProducer => false;

    public double MaximumPowerInWatts => ProducingPower ? _powerUsers.Sum(x => x.PowerConsumptionInWatts) : 0.0;

    public double FuelLevel => _batteries.Any()
        ? _batteries.Sum(x => x.WattHoursRemaining / x.TotalWattHours) / _batteries.Count
        : 0.0;

    public bool ProducingPower
        => _active && _batteries.Sum(x => x.Parent.GetItemType<IStackable>()?.Quantity ?? 1) >= _prototype.BatteryQuantity &&
           _batteries.All(x => x.WattHoursRemaining > 0);

    private readonly List<IConsumePower> _connectedConsumers = new();
    private readonly List<IConsumePower> _powerUsers = new();

    public void BeginDrawdown(IConsumePower item)
    {
        if (!_connectedConsumers.Contains(item))
        {
            _connectedConsumers.Add(item);
        }

        CheckHeartbeat();
    }

    public void EndDrawdown(IConsumePower item)
    {
        _connectedConsumers.Remove(item);
        if (_powerUsers.Remove(item)) item.OnPowerCutOut();
        CheckHeartbeat();
    }

    public bool CanBeginDrawDown(double wattage)
    {
        return CanDrawdownSpike(wattage);
    }

    public bool CanDrawdownSpike(double wattage)
    {
        if (!ProducingPower || !double.IsFinite(wattage) || wattage < 0.0 || _batteries.Count == 0)
        {
            return false;
        }

        double watthours = wattage / 3600.0;
        if (_prototype.BatteriesInSeries)
        {
            return _batteries.All(x => x.WattHoursRemaining >= watthours);
        }

        return _batteries.All(x => x.WattHoursRemaining >= watthours / _batteries.Count);
    }

	public bool CanDrawdownSpike(double wattage, TimeSpan duration)
	{
		if (!ProducingPower || !double.IsFinite(wattage) || wattage < 0.0 || duration <= TimeSpan.Zero ||
		    !double.IsFinite(duration.TotalHours) || _batteries.Count == 0)
		{
			return false;
		}

		var wattHours = wattage * duration.TotalHours;
		if (!double.IsFinite(wattHours))
		{
			return false;
		}

		return _prototype.BatteriesInSeries
			? _batteries.All(x => x.WattHoursRemaining >= wattHours)
			: _batteries.All(x => x.WattHoursRemaining >= wattHours / _batteries.Count);
	}

    public bool DrawdownSpike(double wattage)
    {
        if (!CanDrawdownSpike(wattage))
        {
            return false;
        }

        if (_prototype.BatteriesInSeries)
        {
            foreach (IBattery battery in _batteries)
            {
                battery.WattHoursRemaining -= wattage / 3600.0;
            }
        }
        else
        {
            foreach (IBattery battery in _batteries)
            {
                battery.WattHoursRemaining -= wattage / (_prototype.BatteryQuantity * 3600.0);
            }
        }

        CheckHeartbeat();
        return true;
    }

	public bool DrawdownSpike(double wattage, TimeSpan duration)
	{
		if (!CanDrawdownSpike(wattage, duration))
		{
			return false;
		}

		var wattHours = wattage * duration.TotalHours;
		var perBattery = _prototype.BatteriesInSeries ? wattHours : wattHours / _batteries.Count;
		foreach (var battery in _batteries)
		{
			battery.WattHoursRemaining -= perBattery;
		}

		CheckHeartbeat();
		return true;
	}

    #endregion

    #region IConsumePower Implementation

    public double PowerConsumptionInWatts => _charging ? _prototype.ChargeWattage : 0.0;

    public void OnPowerCutIn()
    {
        if (!_active) return;
        _charging = true;
        CheckHeartbeat();
    }

    public void OnPowerCutOut()
    {
        _charging = false;
        CheckHeartbeat();
    }

    #endregion

    #region IConnectable Implementation

    public IEnumerable<ConnectorType> Connections => _prototype.Connections;
    public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;

    public IEnumerable<ConnectorType> FreeConnections
    {
        get
        {
            return ConnectableGameItemComponent.GetFreeConnections(Connections, ConnectedItems);
        }
    }

    public bool Independent => true;

    public bool CanBeConnectedTo(IConnectable other)
    {
        return true;
    }

    public bool CanConnect(ICharacter? actor, IConnectable other)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out _, Parent, other?.Parent))
		{
			return false;
		}

        if (!FreeConnections.Any() || !other.FreeConnections.Any())
        {
            return false;
        }

        return other.FreeConnections.Any(x => _prototype.Connections.Any(x.CompatibleWith)) &&
               other.CanBeConnectedTo(this);
    }

    public void Connect(ICharacter? actor, IConnectable other)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent, other?.Parent))
		{
			actor?.OutputHandler.Send(manipulationReason);
			return;
		}

        ConnectorType connection = FreeConnections.FirstOrDefault(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
        if (connection == null)
        {
            return;
        }

        var otherConnection = other.FreeConnections.First(x => x.CompatibleWith(connection));

        ConnectPair(this, connection, other, otherConnection);
        Changed = true;
    }

    public void RawConnect(IConnectable other, ConnectorType type)
    {
        _connectedItems.Add(Tuple.Create(type, other));
        var pending = other.Independent ? _pendingLoadTimeConnections : _pendingDependentLoadTimeConnections;
        ConsumeSavedConnection(this, pending, other.Parent.Id, type);
        Parent.ConnectedItem(other, type);
        Changed = true;
    }

    public string WhyCannotConnect(ICharacter? actor, IConnectable other)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent, other?.Parent))
		{
			return manipulationReason;
		}

        if (!FreeConnections.Any())
        {
            return
                $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as the former has no free connection points.";
        }

        if (!other.FreeConnections.Any())
        {
            return
                $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as the latter has no free connection points.";
        }

        if (!other.FreeConnections.Any(x => _prototype.Connections.Any(x.CompatibleWith)))
        {
            return
                $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as none of the free connection points are compatible.";
        }

        return !other.CanBeConnectedTo(this)
            ? $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as that item cannot be connected to."
            : $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} for an unknown reason.";
    }

    public bool CanDisconnect(ICharacter actor, IConnectable other)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out _, Parent, other?.Parent))
		{
			return false;
		}

        return _connectedItems.Any(x => x.Item2 == other);
    }

    public void Disconnect(ICharacter actor, IConnectable other)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent, other?.Parent))
		{
			actor?.OutputHandler.Send(manipulationReason);
			return;
		}

        RawDisconnect(other, true);
    }

    public void RawDisconnect(IConnectable other, bool handleEvents)
    {
        if (handleEvents)
        {
            var connections = GetConnectedPortPairs(this, other);
            other.RawDisconnect(this, false);
            foreach (var connection in connections)
            {
                Parent.DisconnectedItem(other, connection.Local);
                other.Parent.DisconnectedItem(this, connection.Peer ?? connection.Local);
            }
        }

        _connectedItems.RemoveAll(x => x.Item2 == other);
        Changed = true;
    }

    public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent, other?.Parent))
		{
			return manipulationReason;
		}

        return _connectedItems.All(x => x.Item2 != other)
            ? $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} because they are not connected!"
            : $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} for an unknown reason";
    }

    public bool CanBeDisconnectedFrom(IConnectable other)
    {
        return true;
    }

    private void Parent_OnConnected(IConnectable other, ConnectorType type)
    {
        if (!_active || !type.Powered || _connectedPowerSource is not null)
        {
            return;
        }

        IProducePower power = other.Parent.GetItemTypes<IProducePower>()
                         .FirstOrDefault(x => x.PrimaryExternalConnectionPowerProducer || x.MaximumPowerInWatts > 0.0);
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
        if (other.Parent == _connectedPowerSource?.Parent && (_connectedPowerSourceConnector?.Equals(type) == true || _connectedPowerSourceConnector?.CompatibleWith(type) == true))
        {
            _connectedPowerSource.EndDrawdown(this);
            _connectedPowerSource = null;
            _connectedPowerSourceConnector = null;
            _charging = false;
            CheckHeartbeat();
        }
    }

    #endregion

    #region IContainer Implementation

    private readonly List<IBattery> _batteries = new();
    internal IReadOnlyCollection<IBattery> RouteCheckpointBatteries => _batteries;
    private readonly List<IGameItem> _contents = new();
    public IEnumerable<IGameItem> Contents => _contents;

    public string ContentsPreposition => _prototype.ContentsPreposition;

    public bool Transparent => _prototype.Transparent;

    public override double ComponentWeight => Contents.Sum(x => x.Weight);

    public override double ComponentBuoyancy(double fluidDensity)
    {
        return Contents.Sum(x => x.Buoyancy(fluidDensity));
    }

    public bool CanPut(IGameItem item)
    {
        return
            IsOpen &&
            (item.GetItemType<IBattery>()?.BatteryType.EqualTo(_prototype.BatteryType) ?? false) &&
            _contents.Sum(x => x.Quantity) + item.Quantity <= _prototype.BatteryQuantity;
    }

    public int CanPutAmount(IGameItem item)
    {
        return _prototype.BatteryQuantity - _contents.Sum(x => x.Quantity);
    }

    public void Put(ICharacter? putter, IGameItem item, bool allowMerge = true)
    {
        if (_contents.Contains(item))
        {
#if DEBUG
            throw new ApplicationException("Item duplication in container.");
#else
            return;
#endif
        }

        if (allowMerge)
        {
            IGameItem mergeTarget = _contents.FirstOrDefault(x => x.CanMerge(item));
            if (mergeTarget != null)
            {
                mergeTarget.Merge(item);
                item.Delete();
                return;
            }
        }

        _contents.Add(item);
        _batteries.Add(item.GetItemType<IBattery>());
        item.ContainedIn = Parent;
        CheckHeartbeat();
        Changed = true;
    }

    public WhyCannotPutReason WhyCannotPut(IGameItem item)
    {
        if (!IsOpen)
        {
            return WhyCannotPutReason.ContainerClosed;
        }

        if (!item.GetItemType<IBattery>()?.BatteryType.EqualTo(_prototype.BatteryType) ?? true)
        {
            return WhyCannotPutReason.NotCorrectItemType;
        }

        if (_contents.Sum(x => x.Quantity) + item.Quantity > _prototype.BatteryQuantity)
        {
            int capacity = _prototype.BatteryQuantity - _contents.Sum(x => x.Quantity);
            if (item.Quantity <= 1 || capacity <= 0)
            {
                return WhyCannotPutReason.ContainerFull;
            }

            return WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity;
        }

        return WhyCannotPutReason.NotContainer;
    }

    public bool CanTake(ICharacter taker, IGameItem item, int quantity)
    {
        return IsOpen && _contents.Contains(item) && item.CanGet(quantity).AsBool();
    }

    public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
    {
        Changed = true;
        if (quantity == 0 || item.DropsWhole(quantity))
        {
            _contents.Remove(item);
            _batteries.Remove(item.GetItemType<IBattery>());
            item.ContainedIn = null;
            CheckHeartbeat();
            return item;
        }

        return item.Get(null, quantity);
    }

    public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
    {
        if (!IsOpen)
        {
            return WhyCannotGetContainerReason.ContainerClosed;
        }

        return !_contents.Contains(item)
            ? WhyCannotGetContainerReason.NotContained
            : WhyCannotGetContainerReason.NotContainer;
    }

    public override bool Take(IGameItem item)
    {
        if (Contents.Contains(item))
        {
            _contents.Remove(item);
            _batteries.RemoveAll(x => x.Parent == item);
            CheckHeartbeat();
            Changed = true;
            return true;
        }

        return false;
    }

    public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
    {
        if (_contents.Contains(existingItem))
        {
            _contents[_contents.IndexOf(existingItem)] = newItem;
            _batteries.RemoveAll(x => x.Parent == existingItem);
            if (newItem.IsItemType<IBattery>())
            {
                _batteries.Add(newItem.GetItemType<IBattery>());
            }

            CheckHeartbeat();
            newItem.ContainedIn = Parent;
            Changed = true;
            existingItem.ContainedIn = null;
            return true;
        }

        if (_locks.Any(x => x.Parent == existingItem) && newItem.IsItemType<ILock>())
        {
            _locks[_locks.IndexOf(existingItem.GetItemType<ILock>())] = newItem.GetItemType<ILock>();
            existingItem.ContainedIn = null;
            newItem.ContainedIn = Parent;
            Changed = true;
            return true;
        }

        return false;
    }

    public void Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)
    {
        if (emptier is not null)
        {
            var targets = intoContainer is null ? new[] { Parent } : new[] { Parent, intoContainer.Parent };
            if (!ItemManipulationGuard.CanManipulate(emptier, out var reason, targets))
            {
				emptier.Send(reason);
				return;
            }

            if (Contents.Any(x => !CanTake(emptier, x, 0)))
            {
				emptier.Send("You cannot empty that container while some of its contents cannot be taken.");
				return;
            }
        }

        IRoom location = emptier?.Location ?? Parent.TrueLocations.FirstOrDefault();
        List<IGameItem> contents = Contents.ToList();
        _contents.Clear();
        if (emptier is not null)
        {
            if (intoContainer == null)
            {
                emptier.OutputHandler.Handle(
                    new MixedEmoteOutput(new Emote("@ empty|empties $0 onto the ground.", emptier, Parent)).Append(
                        playerEmote));
            }
            else
            {
                emptier.OutputHandler.Handle(
                    new MixedEmoteOutput(new Emote($"@ empty|empties $1 {intoContainer.ContentsPreposition}to $2.",
                        emptier, emptier, Parent, intoContainer.Parent)).Append(playerEmote));
            }
        }

        foreach (IGameItem item in contents)
        {
            item.ContainedIn = null;
            if (intoContainer != null)
            {
                if (intoContainer.CanPut(item))
                {
                    intoContainer.Put(emptier, item);
                }
                else if (location != null)
                {
                    InsertAtParentSpatialLocation(item, location, preferredSource: emptier);
                    emptier?.OutputHandler.Handle(new EmoteOutput(new Emote(
                            "@ cannot put $1 into $2, so #0 set|sets it down on the ground.", emptier, emptier, item,
                            intoContainer.Parent)));
                }
                else
                {
                    item.Delete();
                }

                continue;
            }

            if (location != null)
            {
                InsertAtParentSpatialLocation(item, location, preferredSource: emptier);
            }
            else
            {
                item.Delete();
            }
        }

        CheckHeartbeat();
        Changed = true;
    }

    #endregion

    #region IOpenable Members

    private bool _isOpen = true;

    public bool IsOpen
    {
        get => _isOpen;
        protected set
        {
            _isOpen = value;
            Changed = true;
        }
    }

    public bool CanOpen(IBody opener)
    {
        return !IsOpen && Locks.All(x => !x.IsLocked);
    }

    public WhyCannotOpenReason WhyCannotOpen(IBody opener)
    {
        if (IsOpen)
        {
            return WhyCannotOpenReason.AlreadyOpen;
        }

        return Locks.Any(x => x.IsLocked) ? WhyCannotOpenReason.Locked : WhyCannotOpenReason.NotOpenable;
    }

    public void Open()
    {
        IsOpen = true;
        OnOpen?.Invoke(this);
    }

    public bool CanClose(IBody closer)
    {
        return IsOpen;
    }

    public WhyCannotCloseReason WhyCannotClose(IBody closer)
    {
        return !IsOpen ? WhyCannotCloseReason.AlreadyClosed : WhyCannotCloseReason.NotOpenable;
    }

    public void Close()
    {
        IsOpen = false;
        OnClose?.Invoke(this);
    }

    public event OpenableEvent OnOpen;
    public event OpenableEvent OnClose;

    #endregion

    #region ILockable Members

    private readonly List<ILock> _locks = new();
    public IEnumerable<ILock> Locks => _locks;

    public bool InstallLock(ILock theLock)
    {
        _locks.Add(theLock);
        if (_noSave)
        {
            theLock.Parent.LoadTimeSetContainedIn(Parent);
        }
        else
        {
            theLock.Parent.ContainedIn = Parent;
        }

        return true;
    }

    public bool RemoveLock(ILock theLock)
    {
        if (_locks.Contains(theLock))
        {
            theLock.Parent.ContainedIn = null;
            _locks.Remove(theLock);
            Changed = true;
            return true;
        }

        return false;
    }

    #endregion
}
