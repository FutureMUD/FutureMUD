#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class LiquidPumpGameItemComponent : GameItemComponent, IConnectable, IConsumePower
{
    protected LiquidPumpGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
    private IProducePower? _connectedPowerSource;
    private ConnectorType? _connectedPowerSourceConnector;
    private bool _heartbeatOn;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (LiquidPumpGameItemComponentProto)newProto;
    }

    public LiquidPumpGameItemComponent(LiquidPumpGameItemComponentProto proto, IGameItem parent, bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public LiquidPumpGameItemComponent(Models.GameItemComponent component, LiquidPumpGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public LiquidPumpGameItemComponent(LiquidPumpGameItemComponent rhs, IGameItem newParent, bool temporary = false)
        : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    private void LoadFromXml(XElement root)
    {
        XElement? element = root.Element("ConnectedItems");
        if (element != null)
        {
            foreach (XElement item in element.Elements("Item"))
            {
                ConnectorType connector = new(item.Attribute("connectiontype")!.Value);
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

                connectable.Connect(null, this);
                break;
            }
        }

        _pendingLoadTimeConnections.Clear();

        foreach (Tuple<long, ConnectorType>? item in _pendingDependentLoadTimeConnections.ToList())
        {
            IGameItem? gitem = Gameworld.Items.Get(item.Item1);
            if (gitem == null)
            {
                gitem = Gameworld.TryGetItem(item.Item1, true);
                if (gitem == null)
                {
                    continue;
                }

                gitem.FinaliseLoadTimeTasks();
            }

            foreach (IConnectable connectable in gitem.GetItemTypes<IConnectable>())
            {
                connectable.Connect(null, this);
                break;
            }
        }

        _pendingDependentLoadTimeConnections.Clear();
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new LiquidPumpGameItemComponent(this, newParent, temporary);
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("ConnectedItems",
                from item in ConnectedItems
                select new XElement("Item",
                    new XAttribute("id", item.Item2.Parent.Id),
                    new XAttribute("connectiontype", item.Item1),
                    new XAttribute("independent", item.Item2.Independent)))
        ).ToString();
    }

    private void CheckHeartbeat()
    {
        bool hasPath = Source != null && Sink != null;
        if (_heartbeatOn && (!hasPath || _connectedPowerSource == null))
        {
            EndHeartbeat();
            return;
        }

        if (!_heartbeatOn && hasPath && _connectedPowerSource != null)
        {
            StartHeartbeat();
        }
    }

    private void StartHeartbeat()
    {
        if (_heartbeatOn)
        {
            return;
        }

        Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManagerOnSecondHeartbeat;
        _heartbeatOn = true;
    }

    private void HeartbeatManagerOnSecondHeartbeat()
    {
        if (Source == null || Sink == null)
        {
            CheckHeartbeat();
            return;
        }

        double amount = Math.Min(_prototype.FlowRate, Source.LiquidVolume);
        if (amount <= 0.0)
        {
            CheckHeartbeat();
            return;
        }

        if (Sink.LiquidCapacity > 0.0)
        {
            amount = Math.Min(amount, Math.Max(0.0, Sink.LiquidCapacity - Sink.LiquidVolume));
        }

        if (amount <= 0.0)
        {
            CheckHeartbeat();
            return;
        }

        LiquidMixture mixture = Source.RemoveLiquidAmount(amount, null, "pumped");
        if (mixture?.IsEmpty != false)
        {
            CheckHeartbeat();
            return;
        }

        Sink.MergeLiquid(mixture, null, "pumped");
        CheckHeartbeat();
    }

    private void EndHeartbeat()
    {
        if (!_heartbeatOn)
        {
            return;
        }

        Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
        _heartbeatOn = false;
    }

    public override void Delete()
    {
        base.Delete();
        _connectedPowerSource?.EndDrawdown(this);
        _connectedPowerSource = null;
        _connectedPowerSourceConnector = null;
        EndHeartbeat();
        _connectedItems.Clear();
    }

    public override void Quit()
    {
        base.Quit();
        _connectedPowerSource?.EndDrawdown(this);
        _connectedPowerSource = null;
        _connectedPowerSourceConnector = null;
        EndHeartbeat();
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
            $"It draws {PowerConsumptionInWatts.ToString("N2", voyeur).ColourValue()} watts and moves {_prototype.FlowRate.ToString("N2", voyeur).ColourValue()} units per second.");
        sb.AppendLine(
            $"It is currently {(Source == null ? "unconnected".ColourError() : $"connected to {Source.Parent.HowSeen(voyeur)}".ColourValue())} on the input side and {(Sink == null ? "unconnected".ColourError() : $"connected to {Sink.Parent.HowSeen(voyeur)}".ColourValue())} on the output side.");
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

    public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
    {
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
            }
            else
            {
                if (newItemConnectable.CanConnect(null, connectedItem))
                {
                    newItemConnectable.Connect(null, connectedItem);
                }
                else
                {
                    location?.Insert(connectedItem.Parent);
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
        foreach (Tuple<ConnectorType, IConnectable>? item in preventingItems)
        {
            RawDisconnect(item.Item2, true);
        }
    }

    public IEnumerable<ConnectorType> Connections => _prototype.Connections;
    public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;

    public IEnumerable<ConnectorType> FreeConnections
    {
        get
        {
            List<ConnectorType> rvar = new(Connections);
            foreach (Tuple<ConnectorType, IConnectable> item in ConnectedItems)
            {
                rvar.Remove(item.Item1);
            }

            return rvar;
        }
    }

    public bool Independent => true;

    public bool CanBeConnectedTo(IConnectable other)
    {
        return true;
    }

    public bool CanConnect(ICharacter? actor, IConnectable other)
    {
        if (!FreeConnections.Any() || !other.FreeConnections.Any())
        {
            return false;
        }

        return other.FreeConnections.Any(x => _prototype.Connections.Any(x.CompatibleWith)) &&
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
        Parent_OnConnected(other, type);
        CheckHeartbeat();
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
                Parent_OnDisconnected(other, connection.Item1);
            }
        }

        _connectedItems.RemoveAll(x => x.Item2 == other);
        CheckHeartbeat();
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

    private ILiquidContainer? Source =>
        ConnectedItems
            .Where(x => x.Item1.Gender == Gender.Female)
            .Select(x => x.Item2)
            .FirstOrDefault()
            ?.Parent.GetItemType<ILiquidContainer>();

    private ILiquidContainer? Sink =>
        ConnectedItems
            .Where(x => x.Item1.Gender == Gender.Male)
            .Select(x => x.Item2)
            .FirstOrDefault()
            ?.Parent.GetItemType<ILiquidContainer>();

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
        if (other.Parent == _connectedPowerSource?.Parent && _connectedPowerSourceConnector?.CompatibleWith(type) == true)
        {
            _connectedPowerSource.EndDrawdown(this);
            _connectedPowerSource = null;
            _connectedPowerSourceConnector = null;
            CheckHeartbeat();
        }
    }

    public bool PrimaryLoadTimePowerProducer => false;
    public bool PrimaryExternalConnectionPowerProducer => false;
    public double FuelLevel => 1.0;
    public double MaximumPowerInWatts => _prototype.Wattage;
    public bool ProducingPower => true;

    public void BeginDrawdown(IConsumePower item)
    {
        if (_connectedPowerSource == null)
        {
            return;
        }

        if (!_connectedPowerSource.CanBeginDrawDown(item.PowerConsumptionInWatts))
        {
            return;
        }

        if (!_heartbeatOn)
        {
            StartHeartbeat();
        }
    }

    public void EndDrawdown(IConsumePower item)
    {
    }

    public bool CanBeginDrawDown(double wattage)
    {
        return true;
    }

    public bool CanDrawdownSpike(double wattage)
    {
        return true;
    }

    public bool DrawdownSpike(double wattage)
    {
        return true;
    }

    public double PowerConsumptionInWatts => _prototype.Wattage;

    public void OnPowerCutIn()
    {
        CheckHeartbeat();
    }

    public void OnPowerCutOut()
    {
        CheckHeartbeat();
    }
}
