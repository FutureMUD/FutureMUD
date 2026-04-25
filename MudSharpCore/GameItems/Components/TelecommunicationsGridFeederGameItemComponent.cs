#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Grids;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class TelecommunicationsGridFeederGameItemComponent : GameItemComponent, ICanConnectToTelecommunicationsGrid,
    IConnectable, IConsumePower, IProducePower
{
    private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
    private TelecommunicationsGridFeederGameItemComponentProto _prototype;
    private ITelecommunicationsGrid? _grid;
    private IProducePower? _connectedPowerSource;
    private ConnectorType? _connectedPowerSourceConnector;

    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (TelecommunicationsGridFeederGameItemComponentProto)newProto;
    }

    public TelecommunicationsGridFeederGameItemComponent(TelecommunicationsGridFeederGameItemComponentProto proto,
        IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
        parent.OnConnected += Parent_OnConnected;
        parent.OnDisconnected += Parent_OnDisconnected;
    }

    public TelecommunicationsGridFeederGameItemComponent(Models.GameItemComponent component,
        TelecommunicationsGridFeederGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
        parent.OnConnected += Parent_OnConnected;
        parent.OnDisconnected += Parent_OnDisconnected;
    }

    public TelecommunicationsGridFeederGameItemComponent(TelecommunicationsGridFeederGameItemComponent rhs,
        IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        newParent.OnConnected += Parent_OnConnected;
        newParent.OnDisconnected += Parent_OnDisconnected;
    }

    private void LoadFromXml(XElement root)
    {
        XElement? element = root.Element("ConnectedItems");
        if (element != null)
        {
            foreach (XElement item in element.Elements("Item"))
            {
                if (item.Attribute("independent")?.Value == "false")
                {
                    _pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value),
                        new ConnectorType(item.Attribute("connectiontype")!.Value)));
                }
                else
                {
                    _pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value),
                        new ConnectorType(item.Attribute("connectiontype")!.Value)));
                }
            }
        }

        TelecommunicationsGrid =
            Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new TelecommunicationsGridFeederGameItemComponent(this, newParent, temporary);
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Grid", TelecommunicationsGrid?.Id ?? 0),
            new XElement("ConnectedItems",
                from item in ConnectedItems
                select new XElement("Item",
                    new XAttribute("id", item.Item2.Parent.Id),
                    new XAttribute("connectiontype", item.Item1),
                    new XAttribute("independent", item.Item2.Independent)))
        ).ToString();
    }

    public override void FinaliseLoad()
    {
        foreach (Tuple<long, ConnectorType>? item in _pendingLoadTimeConnections.ToList())
        {
            IGameItem? gitem = Gameworld.Items.Get(item.Item1);
            if (gitem == null || gitem.Location != Parent.Location)
            {
                continue;
            }

            foreach (IConnectable connectable in gitem.GetItemTypes<IConnectable>())
            {
                if (!connectable.CanConnect(null, this))
                {
                    continue;
                }

                Connect(null, connectable);
                break;
            }
        }

        _pendingLoadTimeConnections.Clear();

        foreach (Tuple<long, ConnectorType>? item in _pendingDependentLoadTimeConnections.ToList())
        {
            IGameItem gitem = Gameworld.Items.Get(item.Item1) ?? Gameworld.TryGetItem(item.Item1, true);
            if (gitem == null)
            {
                continue;
            }

            gitem.FinaliseLoadTimeTasks();
            foreach (IConnectable connectable in gitem.GetItemTypes<IConnectable>())
            {
                Connect(null, connectable);
                break;
            }
        }

        _pendingDependentLoadTimeConnections.Clear();
        Changed = true;
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Full;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        if (type != DescriptionType.Full)
        {
            return base.Decorate(voyeur, name, description, type, colour, flags);
        }

        StringBuilder sb = new();
        sb.AppendLine(description);
        sb.AppendLine(
            $"It has {(_prototype.Connections.Count == 1 ? "a connector" : "connectors")} of type {_prototype.Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(x.Gender).GenderClass(true)})").ListToString()}.");
        sb.AppendLine(
            $"It is {(ProducingPower ? "feeding power into the telecommunications grid".ColourValue() : "not currently feeding the telecommunications grid".ColourError())}.");
        foreach (Tuple<ConnectorType, IConnectable> item in ConnectedItems)
        {
            sb.AppendLine(
                $"It is currently connected to {item.Item2.Parent.HowSeen(voyeur)} by a {item.Item1.ConnectionType.Colour(Telnet.Green)} connection.");
        }

        return sb.ToString();
    }

    public ITelecommunicationsGrid? TelecommunicationsGrid
    {
        get => _grid;
        set
        {
            if (_grid == value)
            {
                return;
            }

            _grid?.LeaveGrid((IProducePower)this);
            _grid = value;
            _grid?.JoinGrid((IProducePower)this);
            Changed = true;
        }
    }

    public override void Delete()
    {
        base.Delete();
        Parent.OnConnected -= Parent_OnConnected;
        Parent.OnDisconnected -= Parent_OnDisconnected;
        TelecommunicationsGrid = null;
    }

    public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
    {
        TelecommunicationsGrid = null;
        if (!_connectedItems.Any())
        {
            return false;
        }

        foreach (IConnectable? connectedItem in _connectedItems.Select(x => x.Item2).ToList())
        {
            connectedItem.RawDisconnect(this, true);
            IConnectable? newItemConnectable = newItem?.GetItemType<IConnectable>();
            if (newItemConnectable == null)
            {
                location?.Insert(connectedItem.Parent);
                continue;
            }

            if (newItemConnectable.CanConnect(null, connectedItem))
            {
                newItemConnectable.Connect(null, connectedItem);
            }
            else
            {
                location?.Insert(connectedItem.Parent);
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
        List<Tuple<ConnectorType, IConnectable>> preventingItems = ConnectedItems.Where(
                x => x.Item2.Independent &&
                     (x.Item2.Parent.InInventoryOf == null || x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
                                      .ToList();
        return
            $"{Parent.HowSeen(mover)} is still connected to {preventingItems.Select(x => x.Item2.Parent.HowSeen(mover)).ListToString()}.";
    }

    public override void ForceMove()
    {
        foreach (Tuple<ConnectorType, IConnectable>? item in ConnectedItems.Where(
                x => x.Item2.Independent &&
                     (x.Item2.Parent.InInventoryOf == null || x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
                                         .ToList())
        {
            RawDisconnect(item.Item2, true);
        }
    }

    public override bool Take(IGameItem item)
    {
        if (_connectedItems.RemoveAll(x => x.Item2.Parent == item) <= 0)
        {
            return false;
        }

        Changed = true;
        return true;
    }

    public IEnumerable<ConnectorType> Connections => _prototype.Connections;
    public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;

    public IEnumerable<ConnectorType> FreeConnections
    {
        get
        {
            List<ConnectorType> remaining = new(Connections);
            foreach (Tuple<ConnectorType, IConnectable> item in ConnectedItems)
            {
                remaining.Remove(item.Item1);
            }

            return remaining;
        }
    }

    public bool Independent => true;
    public bool CanBeConnectedTo(IConnectable other)
    {
        return true;
    }

    public bool CanConnect(ICharacter? actor, IConnectable other)
    {
        return FreeConnections.Any() &&
               other.FreeConnections.Any() &&
               other.FreeConnections.Any(x => _prototype.Connections.Any(x.CompatibleWith)) &&
               other.CanBeConnectedTo(this);
    }

    public void Connect(ICharacter? actor, IConnectable other)
    {
        ConnectorType? connection = FreeConnections.FirstOrDefault(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
        if (connection == null)
        {
            return;
        }

        RawConnect(other, connection);
        other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(connection)));
        Changed = true;
    }

    public void RawConnect(IConnectable other, ConnectorType type)
    {
        _connectedItems.Add(Tuple.Create(type, other));
        _pendingLoadTimeConnections.RemoveAll(x => x.Item1 == other.Parent.Id && x.Item2.CompatibleWith(type));
        _pendingDependentLoadTimeConnections.RemoveAll(x => x.Item1 == other.Parent.Id && x.Item2.CompatibleWith(type));
        Parent.ConnectedItem(other, type);
        Changed = true;
    }

    public string WhyCannotConnect(ICharacter? actor, IConnectable other)
    {
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
        return _connectedItems.Any(x => x.Item2 == other);
    }

    public void Disconnect(ICharacter actor, IConnectable other)
    {
        RawDisconnect(other, true);
    }

    public void RawDisconnect(IConnectable other, bool handleEvents)
    {
        if (handleEvents)
        {
            other.RawDisconnect(this, false);
            foreach (Tuple<ConnectorType, IConnectable>? connection in _connectedItems.Where(x => x.Item2 == other).ToList())
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
        return _connectedItems.All(x => x.Item2 != other)
            ? $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} because they are not connected!"
            : $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} for an unknown reason";
    }

    public bool CanBeDisconnectedFrom(IConnectable other)
    {
        return true;
    }

    public bool PrimaryLoadTimePowerProducer => false;
    public bool PrimaryExternalConnectionPowerProducer => false;
    public double FuelLevel => _connectedPowerSource?.FuelLevel ?? 0.0;
    public double MaximumPowerInWatts => _connectedPowerSource?.MaximumPowerInWatts ?? 0.0;
    public bool ProducingPower => _connectedPowerSource?.ProducingPower ?? false;

    public void BeginDrawdown(IConsumePower item)
    {
    }

    public void EndDrawdown(IConsumePower item)
    {
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

    public double PowerConsumptionInWatts => _connectedPowerSource?.MaximumPowerInWatts ?? 0.0;

    public void OnPowerCutIn()
    {
        TelecommunicationsGrid?.RecalculateGrid();
    }

    public void OnPowerCutOut()
    {
        TelecommunicationsGrid?.RecalculateGrid();
    }

    private void Parent_OnConnected(IConnectable other, ConnectorType type)
    {
        if (!type.Powered)
        {
            return;
        }

        IProducePower? power = other.Parent.GetItemTypes<IProducePower>()
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
        if (other.Parent != _connectedPowerSource?.Parent || _connectedPowerSourceConnector?.Equals(type) != true)
        {
            return;
        }

        if (_connectedPowerSource.ProducingPower)
        {
            OnPowerCutOut();
        }

        _connectedPowerSource.EndDrawdown(this);
        _connectedPowerSource = null;
        _connectedPowerSourceConnector = null;
    }

    string ICanConnectToGrid.GridType => "Telecommunications";

    IGrid? ICanConnectToGrid.Grid
    {
        get => TelecommunicationsGrid;
        set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
    }

    ITelecommunicationsGrid? ICanConnectToTelecommunicationsGrid.TelecommunicationsGrid
    {
        get => TelecommunicationsGrid;
        set => TelecommunicationsGrid = value;
    }
}
