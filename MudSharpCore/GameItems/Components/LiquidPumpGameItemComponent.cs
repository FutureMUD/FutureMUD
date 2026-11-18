using MudSharp.GameItems;
#nullable enable
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.GameItems.Prototypes;

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

    internal override IEnumerable<ConnectorType> PendingPorts(long peerId, bool independent) =>
        (independent ? _pendingLoadTimeConnections : _pendingDependentLoadTimeConnections)
        .Where(x => x.Item1 == peerId).Select(x => x.Item2);

    public override void Login() => FinaliseLoad();

    public override void FinaliseLoad()
    {
        RestorePendingConnections(this, _pendingLoadTimeConnections, loadMissingItems: false);
        RestorePendingConnections(this, _pendingDependentLoadTimeConnections, loadMissingItems: true);
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new LiquidPumpGameItemComponent(this, newParent, temporary);
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            SaveConnections(this, ConnectedItems, _pendingLoadTimeConnections, _pendingDependentLoadTimeConnections)
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
        foreach (var peer in _connectedItems.Select(x => x.Item2).Distinct().ToArray()) RawDisconnect(peer, true);
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

    public override bool HandleDieOrMorph(IGameItem newItem, IRoom location)
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

        ConnectorType? connection = FreeConnections.FirstOrDefault(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
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
        Parent_OnConnected(other, type);
        CheckHeartbeat();
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
