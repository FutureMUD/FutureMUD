using MudSharp.GameItems;
using MudSharp.Construction;
using MudSharp.Construction.Grids;
using MudSharp.GameItems.Prototypes;

#nullable enable annotations

namespace MudSharp.GameItems.Components;

public class ElectricGridOutletGameItemComponent : GameItemComponent, IConnectable, IProducePower, IConsumePower,
    ICanConnectToElectricalGrid
{
    protected ElectricGridOutletGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    private IElectricalGrid _grid;
    private bool _gridActive;

    public IElectricalGrid ElectricalGrid
    {
        get => _grid;
        set
        {
            if (ReferenceEquals(_grid, value)) return;
            if (_gridActive) _grid?.LeaveGrid((IConsumePower)this);
            _grid = value;
            if (_gridActive) _grid?.JoinGrid((IConsumePower)this);
            Changed = true;
        }
    }

    public override void Login()
    {
        FinaliseLoad();
        base.Login();
        if (_gridActive) return;
        _gridActive = true;
        _grid?.JoinGrid((IConsumePower)this);
        _grid?.RecalculateGrid();
    }

    public override void Quit()
    {
        if (!_gridActive) return;
        _gridActive = false;
        if (_powered) OnPowerCutOut();
        _grid?.LeaveGrid((IConsumePower)this);
        base.Quit();
    }

    public override void Delete()
    {
        foreach (var peer in _connectedItems.Select(x => x.Item2).Distinct().ToArray()) RawDisconnect(peer, true);
        Quit();
        ElectricalGrid = null;
        base.Delete();
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (ElectricGridOutletGameItemComponentProto)newProto;
    }

    #region Constructors

    public ElectricGridOutletGameItemComponent(ElectricGridOutletGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public ElectricGridOutletGameItemComponent(MudSharp.Models.GameItemComponent component,
        ElectricGridOutletGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public ElectricGridOutletGameItemComponent(ElectricGridOutletGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
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
                var connection = LoadSavedConnector(item);
                if (item.Attribute("independent")?.Value == "false")
                {
                    _pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id").Value), connection));
                }
                else
                {
                    _pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id").Value), connection));
                }
            }
        }

        element = root.Element("Grid");
        if (element != null)
        {
            ElectricalGrid = Gameworld.Grids.Get(long.Parse(element.Value)) as IElectricalGrid;
        }
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new ElectricGridOutletGameItemComponent(this, newParent, temporary);
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Grid", ElectricalGrid?.Id ?? 0),
            SaveConnections(this, ConnectedItems, _pendingLoadTimeConnections, _pendingDependentLoadTimeConnections)
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
        ElectricalGrid = null;
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
                $"It has {(_prototype.Connections.Count == 1 ? "a connector" : "connectors")} of type {_prototype.Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(x.Gender).GenderClass(true)})").ListToString()}.");
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
        ConsumeSavedConnection(this, other.Independent ? _pendingLoadTimeConnections : _pendingDependentLoadTimeConnections,
            other.Parent.Id, type);
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
            other.RawDisconnect(this, false);
            foreach (Tuple<ConnectorType, IConnectable> connection in _connectedItems.Where(x => x.Item2 == other).ToList())
            {
                Parent.DisconnectedItem(other, connection.Item1);
                other.Parent.DisconnectedItem(this, connection.Item1);
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

    public bool PrimaryLoadTimePowerProducer => false;
    public bool PrimaryExternalConnectionPowerProducer => true;
    public double FuelLevel => 1.0;
    private readonly List<IConsumePower> _connectedConsumers = new();
    private readonly List<IConsumePower> _powerUsers = new();
    private bool _powered;

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

        ElectricalGrid?.RecalculateGrid();
    }

    public void EndDrawdown(IConsumePower item)
    {
        _connectedConsumers.Remove(item);
        if (_powerUsers.Remove(item))
        {
            item.OnPowerCutOut();
        }
        ElectricalGrid?.RecalculateGrid();
    }

    public bool CanBeginDrawDown(double wattage)
    {
        return true;
    }

    public bool CanDrawdownSpike(double wattage)
    {
        return ProducingPower;
    }

    public bool DrawdownSpike(double wattage)
    {
        return ProducingPower && (ElectricalGrid?.DrawdownSpike(wattage) ?? false);
    }

    public double MaximumPowerInWatts =>
        ElectricalGrid != null ? ElectricalGrid.TotalSupply - ElectricalGrid.TotalDrawdown : 0.0;

    public bool ProducingPower => ElectricalGrid != null && _powered;

    #endregion

    #region Implementation of IConsumePower

    public double PowerConsumptionInWatts => _powerUsers.Sum(x => x.PowerConsumptionInWatts);

    public void OnPowerCutIn()
    {
        if (_powered) return;
        _powered = true;
        var newPowerUsers = _connectedConsumers.Where(x => !_powerUsers.Contains(x)).ToList();
        _powerUsers.AddRange(newPowerUsers);
        foreach (IConsumePower item in newPowerUsers)
        {
            item.OnPowerCutIn();
        }
    }

    public void OnPowerCutOut()
    {
        if (!_powered && _powerUsers.Count == 0) return;
        _powered = false;
        var previousPowerUsers = _powerUsers.ToList();
        _powerUsers.Clear();
        foreach (IConsumePower item in previousPowerUsers)
        {
            item.OnPowerCutOut();
        }
    }

    #endregion

    #region ICanConnectToGrid Implementation

    string ICanConnectToGrid.GridType => "Electrical";

    IGrid? ICanConnectToGrid.Grid
    {
        get => ElectricalGrid;
        set => ElectricalGrid = value as IElectricalGrid;
    }

    #endregion
}
