using MudSharp.GameItems;
using MudSharp.Construction;
using MudSharp.GameItems.Prototypes;

#nullable enable annotations

namespace MudSharp.GameItems.Components;

public class ConnectableGameItemComponent : GameItemComponent, IConnectable
{
    private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems =
        new();

    private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections =
        new();

    private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections =
        new();

    private ConnectableGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new ConnectableGameItemComponent(this, newParent, temporary);
    }

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

    public override void Delete()
    {
        foreach (var peer in _connectedItems.Select(x => x.Item2).Distinct().ToArray()) RawDisconnect(peer, true);
        base.Delete();
    }

    public override bool AffectsLocationOnDestruction => true;

    public override bool PreventsMovement()
    {
        return MovementBreakingConnections().Any();
    }

    public override string WhyPreventsMovement(ICharacter mover)
    {
        List<Tuple<ConnectorType, IConnectable>> preventingItems = MovementBreakingConnections().ToList();
        return
            $"{Parent.HowSeen(mover)} is still connected to {preventingItems.Select(x => x.Item2.Parent.HowSeen(mover)).ListToString()}.";
    }

    public override void ForceMove()
    {
        List<Tuple<ConnectorType, IConnectable>> preventingItems = MovementBreakingConnections().ToList();
        foreach (Tuple<ConnectorType, IConnectable> item in preventingItems)
        {
            RawDisconnect(item.Item2, true);
        }
    }

    private IEnumerable<Tuple<ConnectorType, IConnectable>> MovementBreakingConnections()
    {
        return ConnectedItems.Where(x => x.Item2.Independent &&
                                         (Parent.InInventoryOf != null ||
                                          !ItemsRemainCoLocated(Parent, x.Item2.Parent)));
    }

    private static bool ItemsRemainCoLocated(IGameItem parent, IGameItem other)
    {
        if (parent.InInventoryOf != null || other.InInventoryOf != null)
        {
            return parent.InInventoryOf == other.InInventoryOf;
        }

        if (parent.ContainedIn != null || other.ContainedIn != null)
        {
            return parent.ContainedIn == other.ContainedIn;
        }

		return parent.ColocatedWith(other);
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

    public IEnumerable<ConnectorType> Connections => _prototype.Connections;
    public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;

    public IEnumerable<ConnectorType> FreeConnections
    {
        get
        {
            return GetFreeConnections(Connections, ConnectedItems);
        }
    }

    internal static IEnumerable<ConnectorType> GetFreeConnections(IEnumerable<ConnectorType> connections,
        IEnumerable<Tuple<ConnectorType, IConnectable>> connected)
    {
        var free = connections.ToList();
        foreach (var (port, _) in connected)
        {
            var index = free.FindIndex(x => ReferenceEquals(x, port));
            if (index < 0 && port.PortId is not null)
            {
                index = free.FindIndex(x => x.PortId == port.PortId);
                if (index < 0) continue;
            }

            if (index < 0) index = free.FindIndex(x => x.Equals(port));
            if (index >= 0) free.RemoveAt(index);
        }

        return free;
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

		if (Parent.Location is not null && other.Parent.Location is not null &&
			!Parent.ColocatedWith(other.Parent))
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

		if (Parent.Location is not null && other.Parent.Location is not null &&
			!Parent.ColocatedWith(other.Parent))
		{
			return $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} because they are too far apart.";
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

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (ConnectableGameItemComponentProto)newProto;
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            SaveConnections(this, ConnectedItems, _pendingLoadTimeConnections, _pendingDependentLoadTimeConnections))
            .ToString();
    }

    #region Constructors

    public ConnectableGameItemComponent(ConnectableGameItemComponentProto proto, IGameItem parent,
        bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public ConnectableGameItemComponent(MudSharp.Models.GameItemComponent component,
        ConnectableGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    private void LoadFromXml(XElement root)
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

    public ConnectableGameItemComponent(ConnectableGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
		// Connections belong to the concrete item instance and must never be shared by a clone.
		_connectedItems = new List<Tuple<ConnectorType, IConnectable>>();
    }

    internal static ConnectorType? FindExactFreeConnector(IEnumerable<ConnectorType> available, ConnectorType saved)
    {
        var matches = available.Where(x => x.Equals(saved)).Take(2).ToArray();
        return matches.Length == 1 ? matches[0] : null;
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

    #endregion
}
