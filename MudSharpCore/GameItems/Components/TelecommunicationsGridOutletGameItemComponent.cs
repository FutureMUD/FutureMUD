using MudSharp.GameItems;
#nullable enable
using MudSharp.Construction;
using MudSharp.Construction.Grids;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class TelecommunicationsGridOutletGameItemComponent : GameItemComponent, IConnectable, IProducePower,
    IConsumePower, ICanConnectToTelecommunicationsGrid, ITelephoneNumberOwner
{
    private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
    private readonly List<IConsumePower> _connectedConsumers = [];
    private readonly List<IConsumePower> _powerUsers = [];
    private TelecommunicationsGridOutletGameItemComponentProto _prototype;
    private ITelecommunicationsGrid? _grid;
    private bool _powered;
    private string? _phoneNumber;
    private string? _preferredNumber;
    private bool _allowSharedNumber;
    private bool _hostedVoicemailEnabled;

    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (TelecommunicationsGridOutletGameItemComponentProto)newProto;
    }

    public TelecommunicationsGridOutletGameItemComponent(
        TelecommunicationsGridOutletGameItemComponentProto proto, IGameItem parent, bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public TelecommunicationsGridOutletGameItemComponent(Models.GameItemComponent component,
        TelecommunicationsGridOutletGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public TelecommunicationsGridOutletGameItemComponent(TelecommunicationsGridOutletGameItemComponent rhs,
        IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _hostedVoicemailEnabled = rhs._hostedVoicemailEnabled;
    }

    private void LoadFromXml(XElement root)
    {
        _preferredNumber = root.Element("PreferredNumber")?.Value;
        _allowSharedNumber = bool.Parse(root.Element("AllowSharedNumber")?.Value ?? "false");
        _hostedVoicemailEnabled = bool.Parse(root.Element("HostedVoicemailEnabled")?.Value ?? "false");
        XElement? element = root.Element("ConnectedItems");
        if (element != null)
        {
            foreach (XElement item in element.Elements("Item"))
            {
                if (item.Attribute("independent")?.Value == "false")
                {
                    _pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value),
                        LoadSavedConnector(item)));
                }
                else
                {
                    _pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value),
                        LoadSavedConnector(item)));
                }
            }
        }

        TelecommunicationsGrid =
            Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new TelecommunicationsGridOutletGameItemComponent(this, newParent, temporary);
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Grid", TelecommunicationsGrid?.Id ?? 0),
            new XElement("PreferredNumber", _preferredNumber ?? string.Empty),
            new XElement("AllowSharedNumber", _allowSharedNumber),
            new XElement("HostedVoicemailEnabled", _hostedVoicemailEnabled),
            SaveConnections(this, ConnectedItems, _pendingLoadTimeConnections, _pendingDependentLoadTimeConnections)
        ).ToString();
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
        sb.AppendLine($"Its number is {(PhoneNumber?.ColourValue() ?? "unassigned".ColourError())}.");
        sb.AppendLine($"Hosted voicemail is {(_hostedVoicemailEnabled ? "enabled".ColourValue() : "disabled".ColourError())} for this line.");
        sb.AppendLine($"It is {(ProducingPower ? "powered".ColourValue() : "not powered".ColourError())}.");
        foreach (Tuple<ConnectorType, IConnectable> item in ConnectedItems)
        {
            sb.AppendLine(
                $"It is currently connected to {item.Item2.Parent.HowSeen(voyeur)} by a {item.Item1.ConnectionType.Colour(Telnet.Green)} connection.");
        }

        return sb.ToString();
    }

    public string? PhoneNumber => _phoneNumber;

    public string? PreferredNumber
    {
        get => _preferredNumber;
        set
        {
            _preferredNumber = string.IsNullOrWhiteSpace(value) ? null : value;
            Changed = true;
            TelecommunicationsGrid?.RequestNumber(this, _preferredNumber, _allowSharedNumber);
        }
    }

    public bool AllowSharedNumber
    {
        get => _allowSharedNumber;
        set
        {
            _allowSharedNumber = value;
            Changed = true;
            TelecommunicationsGrid?.RequestNumber(this, _preferredNumber, _allowSharedNumber);
        }
    }

    public bool HostedVoicemailEnabled
    {
        get => _hostedVoicemailEnabled;
        set
        {
            _hostedVoicemailEnabled = value;
            Changed = true;
        }
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

            _grid?.LeaveGrid((ITelephoneNumberOwner)this);
            _grid?.LeaveGrid((IConsumePower)this);
            _grid = value;
            _grid?.JoinGrid((ITelephoneNumberOwner)this);
            _grid?.JoinGrid((IConsumePower)this);
            Changed = true;
        }
    }

    public IEnumerable<ITelephone> ConnectedTelephones =>
        TelephoneNetworkHelpers.CollectConnectedTelephones(this)
                               .Distinct()
                               .ToList();

    public void AssignPhoneNumber(string? number)
    {
        _phoneNumber = number;
        Changed = true;
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

        return FreeConnections.Any() &&
               other.FreeConnections.Any() &&
               other.FreeConnections.Any(x => Connections.Any(x.CompatibleWith)) &&
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

        if (!other.FreeConnections.Any(x => Connections.Any(x.CompatibleWith)))
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

    public bool PrimaryLoadTimePowerProducer => false;
    public bool PrimaryExternalConnectionPowerProducer => true;
    public double FuelLevel => 1.0;
    public double MaximumPowerInWatts =>
        TelecommunicationsGrid != null ? TelecommunicationsGrid.TotalSupply - TelecommunicationsGrid.TotalDrawdown : 0.0;

    public void BeginDrawdown(IConsumePower item)
    {
        if (_connectedConsumers.Contains(item))
        {
            return;
        }

        _connectedConsumers.Add(item);
        if (ProducingPower)
        {
            if (!_powerUsers.Contains(item))
            {
                _powerUsers.Add(item);
                item.OnPowerCutIn();
            }
        }

        TelecommunicationsGrid?.RecalculateGrid();
    }

    public void EndDrawdown(IConsumePower item)
    {
        if (!_connectedConsumers.Remove(item))
        {
            return;
        }

        if (_powerUsers.Remove(item))
        {
            item.OnPowerCutOut();
        }
        TelecommunicationsGrid?.RecalculateGrid();
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
        return ProducingPower && (TelecommunicationsGrid?.DrawdownSpike(wattage) ?? false);
    }

    public bool ProducingPower => TelecommunicationsGrid != null && _powered;
    public double PowerConsumptionInWatts => _powerUsers.Sum(x => x.PowerConsumptionInWatts);

    public void OnPowerCutIn()
    {
        _powered = true;
        foreach (IConsumePower? item in _connectedConsumers.Where(x => !_powerUsers.Contains(x)).ToList())
        {
            _powerUsers.Add(item);
            item.OnPowerCutIn();
        }
    }

    public void OnPowerCutOut()
    {
        _powered = false;
        foreach (IConsumePower? item in _powerUsers.ToList())
        {
            item.OnPowerCutOut();
        }

        _powerUsers.Clear();
    }

    public override bool HandleDieOrMorph(IGameItem newItem, IRoom location)
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
                InsertAtParentSpatialLocation(connectedItem.Parent, location);
                continue;
            }

            if (newItemConnectable.CanConnect(null, connectedItem))
            {
                newItemConnectable.Connect(null, connectedItem);
            }
            else
            {
                InsertAtParentSpatialLocation(connectedItem.Parent, location);
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

    public override void Delete()
    {
        foreach (var peer in _connectedItems.Select(x => x.Item2).Distinct().ToArray()) RawDisconnect(peer, true);
        base.Delete();
        TelecommunicationsGrid = null;
        foreach (IConsumePower? item in _powerUsers.ToList())
        {
            item.OnPowerCutOut();
        }

        _powerUsers.Clear();
        _connectedConsumers.Clear();
    }

    string ICanConnectToGrid.GridType => "Telecommunications";

    IGrid? ICanConnectToGrid.Grid
    {
        get => TelecommunicationsGrid;
        set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
    }
}
