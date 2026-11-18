using MudSharp.GameItems;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.GameItems.Prototypes;

#nullable enable annotations

namespace MudSharp.GameItems.Components;

public class PowerSocketGameItemComponent : GameItemComponent, IConnectable, IProducePower, IConsumePower
{
    protected PowerSocketGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (PowerSocketGameItemComponentProto)newProto;
    }

    #region Constructors

    public PowerSocketGameItemComponent(PowerSocketGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public PowerSocketGameItemComponent(MudSharp.Models.GameItemComponent component,
        PowerSocketGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public PowerSocketGameItemComponent(PowerSocketGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
        base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    protected void LoadFromXml(XElement root)
    {
        XElement element = root.Element("ConnectedItems");
        if (element != null)
        {
            foreach (XElement item in element.Elements("Item"))
            {
                if (item.Attribute("independent")?.Value == "false")
                {
                    _pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id").Value),
                        LoadSavedConnector(item)));
                }
                else
                {
                    _pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id").Value),
                        LoadSavedConnector(item)));
                }
            }
        }
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new PowerSocketGameItemComponent(this, newParent, temporary);
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("ConnectedItems",
                ConnectedItems.Select(x => SaveConnectionEntry(x.Item2.Parent.Id, x.Item1, x.Item2.Independent,
                    FindPortIndex(this, x.Item1), FindPeerPortIndex(this, x.Item2, x.Item1), FindPortId(this, x.Item1), FindPeerPortId(this, x.Item2, x.Item1))),
                _pendingLoadTimeConnections.Select(x => SaveConnectionEntry(x.Item1, x.Item2, true, x.Item2.PortIndex,
                    x.Item2.PeerPortIndex, x.Item2.PortId, x.Item2.PeerPortId)),
                _pendingDependentLoadTimeConnections.Select(x => SaveConnectionEntry(x.Item1, x.Item2, false, x.Item2.PortIndex,
                    x.Item2.PeerPortIndex, x.Item2.PortId, x.Item2.PeerPortId)))
        ).ToString();
    }

    #endregion

    private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems =
        new();

    private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections =
        new();

    private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections =
        new();

    public override bool HandleDieOrMorph(IGameItem newItem, IRoom location)
    {
        if (!_connectedItems.Any())
        {
            return false;
        }

        foreach (IConnectable connectedItem in _connectedItems.Select(x => x.Item2).ToList())
        {
            connectedItem.RawDisconnect(this, true);
            IConnectable newItemConnectable = newItem?.GetItemType<IConnectable>();
            if (newItemConnectable == null)
            {
                InsertAtParentSpatialLocation(connectedItem.Parent, location);
            }
            else
            {
                if (newItemConnectable.CanConnect(null, connectedItem))
                {
                    newItemConnectable.Connect(null, connectedItem);
                }
                else
                {
                    InsertAtParentSpatialLocation(connectedItem.Parent, location);
                }
            }
        }

        return false;
    }

    public override bool AffectsLocationOnDestruction => true;

    public override bool PreventsMovement()
    {
        return (Parent.InInventoryOf != null && ConnectedItems.Any(x => x.Item2.Independent)) ||
               ConnectedItems.Any(x => x.Item2.Independent && x.Item2.Parent.InInventoryOf != Parent.InInventoryOf);
    }

    public override string WhyPreventsMovement(ICharacter mover)
    {
        List<Tuple<ConnectorType, IConnectable>> preventingItems =
            ConnectedItems.Where(
                              x =>
                                  x.Item2.Independent &&
                                  (x.Item2.Parent.InInventoryOf == null ||
                                   x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
                          .ToList();
        return
            $"{Parent.HowSeen(mover)} is still connected to {preventingItems.Select(x => x.Item2.Parent.HowSeen(mover)).ListToString()}.";
    }

    public override void ForceMove()
    {
        List<Tuple<ConnectorType, IConnectable>> preventingItems =
            ConnectedItems.Where(
                              x =>
                                  x.Item2.Independent &&
                                  (x.Item2.Parent.InInventoryOf == null ||
                                   x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
                          .ToList();
        foreach (Tuple<ConnectorType, IConnectable> item in preventingItems)
        {
            RawDisconnect(item.Item2, true);
        }
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Full;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        if (type == DescriptionType.Full)
        {
            StringBuilder sb = new();
            sb.AppendLine(description);
            sb.AppendLine(
                $"It connects to {(_prototype.Connections.Count == 1 ? "a connector" : "connectors")} of type {_prototype.Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(x.Gender).GenderClass(true)})").ListToString()}.");
            if (ConnectedItems.Any())
            {
                sb.AppendLine();
            }

            foreach (Tuple<ConnectorType, IConnectable> item in ConnectedItems)
            {
                sb.AppendLine(
                    $"It is currently connected to {item.Item2.Parent.HowSeen(voyeur)} by a {item.Item1.ConnectionType.Colour(Telnet.Green)} connection.");
            }

            return sb.ToString();
        }

        return base.Decorate(voyeur, name, description, type, colour, flags);
    }

    public override bool Take(IGameItem item)
    {
        if (_connectedItems.RemoveAll(x => x.Item2.Parent == item) > 0)
        {
            Changed = true;
            return true;
        }

        return false;
    }

    internal override IEnumerable<ConnectorType> PendingPorts(long peerId, bool independent) =>
        (independent ? _pendingLoadTimeConnections : _pendingDependentLoadTimeConnections)
        .Where(x => x.Item1 == peerId).Select(x => x.Item2);

    public override void FinaliseLoad()
    {
        RestorePendingConnections(this, _pendingLoadTimeConnections, loadMissingItems: false);
        RestorePendingConnections(this, _pendingDependentLoadTimeConnections, loadMissingItems: true);
    }

    public override void Login()
    {
        FinaliseLoad();
        if (_connectedPowerSource is not null) return;
        var candidates = Parent.GetItemTypes<IProducePower>()
            .Where(x => !ReferenceEquals(x, this) && x.PrimaryLoadTimePowerProducer).ToArray();
        if (candidates.Length != 1)
        {
            Gameworld.DebugMessage($"Power socket on item {Parent.Id:N0} requires one primary producer; found {candidates.Length}.");
            return;
        }

        _connectedPowerSource = candidates[0];
        _connectedPowerSource.BeginDrawdown(this);
    }

    public override void Quit() => ReleasePowerSource();

    public override void Delete()
    {
        ReleasePowerSource();
        foreach (var peer in _connectedItems.Select(x => x.Item2).Distinct().ToArray()) RawDisconnect(peer, true);
        base.Delete();
    }

    private void ReleasePowerSource()
    {
        var source = _connectedPowerSource;
        _connectedPowerSource = null;
        source?.EndDrawdown(this);
        OnPowerCutOut();
    }

    #region Implementation of IConnectable

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
        return true; // TODO
    }

    public bool CanConnect(ICharacter? actor, IConnectable other)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out _, Parent, other?.Parent))
		{
			return false;
		}

        if (!FreeConnections.Any())
        {
            return false;
        }

        if (!other.FreeConnections.Any())
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
        return true; // TODO - reasons why this might be false
    }

    #endregion

    #region Implementation of IProducePower

    public double MaximumPowerInWatts => _connectedPowerSource?.MaximumPowerInWatts ?? 0.0;
    public bool PrimaryLoadTimePowerProducer => false;
    public bool PrimaryExternalConnectionPowerProducer => true;
    public double FuelLevel => _connectedPowerSource?.FuelLevel ?? 0.0;
    private readonly List<IConsumePower> _connectedConsumers = new();
    private readonly List<IConsumePower> _powerUsers = new();

    public void BeginDrawdown(IConsumePower item)
    {
        if (_connectedConsumers.Contains(item)) return;
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
        if (_powerUsers.Remove(item)) item.OnPowerCutOut();
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

	public bool CanDrawdownSpike(double wattage, TimeSpan duration) =>
		double.IsFinite(wattage) && wattage >= 0.0 && duration > TimeSpan.Zero &&
		(_connectedPowerSource?.CanDrawdownSpike(wattage, duration) ?? false);

	public bool DrawdownSpike(double wattage, TimeSpan duration) =>
		double.IsFinite(wattage) && wattage >= 0.0 && duration > TimeSpan.Zero &&
		(_connectedPowerSource?.DrawdownSpike(wattage, duration) ?? false);

    public bool ProducingPower => _connectedPowerSource?.ProducingPower ?? false;

    private IProducePower _connectedPowerSource;

    #endregion

    #region Implementation of IConsumePower

    public double PowerConsumptionInWatts => _powerUsers.Sum(x => x.PowerConsumptionInWatts);

    public void OnPowerCutIn()
    {
        foreach (var item in _connectedConsumers.ToArray())
        {
            if (_connectedPowerSource is null) break;
            if (!_connectedConsumers.Contains(item) || _powerUsers.Contains(item)) continue;
            _powerUsers.Add(item);
            item.OnPowerCutIn();
        }
    }

    public void OnPowerCutOut()
    {
        var powered = _powerUsers.ToArray();
        _powerUsers.Clear();
        foreach (var item in powered) item.OnPowerCutOut();
    }

    #endregion
}
