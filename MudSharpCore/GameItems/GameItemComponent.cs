using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Components;
using MudSharp.Models;
using System.Threading;

namespace MudSharp.GameItems;

public abstract class GameItemComponent : LateInitialisingItem, IGameItemComponent
{
    private sealed record ConnectionNotificationBatch(Func<bool> IsConnectionStillPresent)
    {
        public List<Action> Notifications { get; } = [];
    }

    private static readonly AsyncLocal<ConnectionNotificationBatch?> DeferredConnectionNotifications = new();
	// A transfer rollback must restore native fields without repeating inventory callbacks.
	// Structural components without a verified adapter hold before any transfer begins.
	#nullable enable annotations
	internal virtual Action? CaptureCustodyRollback()
	{
		if (this is IContainer or ILockable or IBelt or IFirearmAttachmentHost or ISeveredBodypart or IWearable or ILock) return null;
		var holdable = this as IHoldable; var heldBy = holdable?.HeldBy; var description = holdable?.CurrentInventoryDescription;
		var wieldable = this as IWieldable; var wieldLocation = wieldable?.PrimaryWieldedLocation;
		var beltable = this as IBeltable; var connectedTo = beltable?.ConnectedTo;
		return () =>
		{
			if (holdable is not null) { holdable.HeldBy = heldBy; holdable.CurrentInventoryDescription = description; }
			if (wieldable is not null) wieldable.PrimaryWieldedLocation = wieldLocation;
			if (beltable is not null) beltable.ConnectedTo = connectedTo;
		};
	}

	#nullable restore annotations
    private MudSharp.Models.GameItem _parentDBItem;
	private SpatialLocation? _activeDieOrMorphSource;
	protected SpatialLocation? CapturedLifecycleSource => _activeDieOrMorphSource;

    protected GameItemComponent(GameItemComponent rhs, IGameItem newParent, bool temporary = false)
    {
        Parent = newParent;
        _noSave = temporary;
        if (!temporary)
        {
            Gameworld.SaveManager.AddInitialisation(this);
        }
    }

    protected GameItemComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
    {
        Parent = parent;
        _id = component.Id;
        IdInitialised = true;
    }

    protected GameItemComponent(IGameItem parent, IGameItemComponentProto prototype, bool temporary = false)
    {
        Parent = parent;
        _noSave = temporary;
        if (!temporary)
        {
            Gameworld.SaveManager.AddInitialisation(this);
        }
    }

    #region Overrides of LateInitialisingItem

    public override IFuturemud Gameworld
    {
        get => Parent.Gameworld;
        protected set { }
    }

    /// <inheritdoc />
    public override string Name => Prototype.Name;

    #endregion

    public void PrimeComponentForInsertion(MudSharp.Models.GameItem parent)
    {
        _parentDBItem = parent;
    }

    public override string FrameworkItemType => "GameItemComponent";

    /// <summary>
    /// Replays a connection recorded in component XML without turning the replay itself into a save.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A normal connection updates both components and their parent items. During load those updates merely
    /// reconstruct the already-persisted graph, so retaining their <see cref="ISaveable.Changed"/> state causes an
    /// otherwise unnecessary first-save spike after boot.
    /// </para>
    /// <para>
    /// Any change that pre-dated the replay is retained. This is important for genuine repair paths that have
    /// already deliberately marked an item for saving.
    /// </para>
    /// </remarks>
    protected void RestorePersistedConnection(IConnectable other, Action restoreAction)
    {
        var affectedItems = new ISaveable[] { this, other, Parent, other.Parent }
            .Distinct()
            .Select(x => (Item: x, WasChanged: x.Changed))
            .ToList();

        restoreAction();

        foreach (var (item, wasChanged) in affectedItems)
        {
            item.Changed = wasChanged;
            if (!wasChanged)
            {
                Gameworld.SaveManager.Abort(item);
            }
        }
    }

    internal virtual IEnumerable<ConnectorType> PendingPorts(long peerId, bool independent) => [];

    internal virtual bool CanRestoreConnectionFrom(IConnectable peer) => true;

    /// <summary>
    /// Records the exact physical pair before RawConnect invokes callbacks. This lets reentrant observers
    /// save or inspect a connection without having to infer its peer from insertion order.
    /// </summary>
    internal static void ConnectPair(IConnectable first, ConnectorType firstPort,
        IConnectable second, ConnectorType secondPort)
    {
        if (!firstPort.CompatibleWith(secondPort))
        {
            throw new InvalidOperationException("Cannot pair incompatible physical connectors.");
        }

        var (firstConnection, secondConnection) = ConnectorType.CreatePairedConnectionInstances(firstPort, secondPort);

        var firstRetainsPortInstance = false;
        var secondRetainsPortInstance = false;
        var notifications = new ConnectionNotificationBatch(() =>
            IsConnected(first, second, firstConnection, firstRetainsPortInstance) &&
            IsConnected(second, first, secondConnection, secondRetainsPortInstance));
        var previous = DeferredConnectionNotifications.Value;
        DeferredConnectionNotifications.Value = notifications;
        try
        {
            first.RawConnect(second, firstConnection);
            second.RawConnect(first, secondConnection);
            firstRetainsPortInstance = HasConnectionPortInstance(first, second, firstConnection);
            secondRetainsPortInstance = HasConnectionPortInstance(second, first, secondConnection);
        }
        finally
        {
            DeferredConnectionNotifications.Value = previous;
        }

        foreach (var notification in notifications.Notifications)
        {
            notification();
        }

        static bool HasConnectionPortInstance(IConnectable endpoint, IConnectable peer, ConnectorType port) =>
            endpoint.ConnectedItems.Any(x => ReferenceEquals(x.Item2, peer) && ReferenceEquals(x.Item1, port));

        static bool IsConnected(IConnectable endpoint, IConnectable peer, ConnectorType port, bool retainsPortInstance) =>
            endpoint.ConnectedItems.Any(x => ReferenceEquals(x.Item2, peer) &&
                (ReferenceEquals(x.Item1, port) || (!retainsPortInstance && port.PortId is not null && x.Item1.PortId == port.PortId)));
    }

    internal static bool TryDeferConnectionNotification(Action notification)
    {
        var notifications = DeferredConnectionNotifications.Value;
        if (notifications is null) return false;
        notifications.Notifications.Add(() =>
        {
            if (notifications.IsConnectionStillPresent()) notification();
        });
        return true;
    }

    internal static IReadOnlyList<(ConnectorType Local, ConnectorType? Peer)> GetConnectedPortPairs(
        IConnectable endpoint, IConnectable peer)
    {
        var localPorts = endpoint.ConnectedItems.Where(x => ReferenceEquals(x.Item2, peer)).Select(x => x.Item1).ToArray();
        var peerPorts = peer.ConnectedItems.Where(x => ReferenceEquals(x.Item2, endpoint)).Select(x => x.Item1).ToArray();
        return localPorts.Select(local =>
        {
            if (local.PeerPortId is { } peerPortId)
            {
                var exact = peerPorts.Where(x => x.PortId == peerPortId).Take(2).ToArray();
                return (local, exact.Length == 1 ? exact[0] : null);
            }

            if (local.PortId is { } localPortId)
            {
                var reciprocal = peerPorts.Where(x => x.PeerPortId == localPortId).Take(2).ToArray();
                if (reciprocal.Length > 0) return (local, reciprocal.Length == 1 ? reciprocal[0] : null);
            }

            var compatible = peerPorts.Where(x => x.CompatibleWith(local)).Take(2).ToArray();
            return (local, compatible.Length == 1 ? compatible[0] : null);
        }).ToArray();
    }

    protected static ConnectorType LoadSavedConnector(XElement item) => new(
        item.Attribute("connectiontype")!.Value,
        int.TryParse(item.Attribute("portindex")?.Value, out var index) ? index : null,
        int.TryParse(item.Attribute("peerportindex")?.Value, out var peerIndex) ? peerIndex : null,
        item.Attribute("portid")?.Value,
        item.Attribute("peerportid")?.Value);

    protected bool RestoreExactConnection(IConnectable endpoint, ConnectorType saved, IEnumerable<IConnectable> peers)
    {
        var ownPort = FindSavedFreeConnector(endpoint, saved);
        if (ownPort is null) return false;

        var candidates = peers.Where(peer => CanRestoreConnectionFrom(peer) &&
            (peer is not GameItemComponent native || native.CanRestoreConnectionFrom(endpoint))).ToArray();
        if (saved.PeerPortId is not null)
        {
            var exactPeerMatches = candidates.Select(peer => (Peer: peer,
                    Port: FindSavedFreePortById(peer, saved.PeerPortId)))
                .Where(x => x.Port is not null && x.Port.CompatibleWith(ownPort))
                .Take(2).ToArray();
            if (exactPeerMatches.Length != 1) return false;
            var (exactPeer, exactPeerPort) = exactPeerMatches[0];
            RestorePersistedConnection(exactPeer, () => ConnectPair(endpoint, ownPort, exactPeer, exactPeerPort));
            return true;
        }
        if (saved.PeerPortIndex is { } peerPortIndex)
        {
            var exactPeerMatches = candidates.Select(peer => (Peer: peer,
                    Port: FindSavedFreePortByIndex(peer, peerPortIndex)))
                .Where(x => x.Port is not null && x.Port.CompatibleWith(ownPort))
                .Take(2).ToArray();
            if (exactPeerMatches.Length != 1) return false;
            var (exactPeer, exactPeerPort) = exactPeerMatches[0];
            RestorePersistedConnection(exactPeer, () => ConnectPair(endpoint, ownPort, exactPeer, exactPeerPort));
            return true;
        }
        var reciprocal = candidates
            .OfType<GameItemComponent>()
            .SelectMany(component => component.PendingPorts(Parent.Id, endpoint.Independent)
                .Where(port => port.CompatibleWith(ownPort))
                .Select(port => (Peer: (IConnectable)component, Port: port)))
            .ToArray();
        var matches = (reciprocal.Length > 0
                ? reciprocal.Select(x => (x.Peer, Port: FindSavedFreeConnector(x.Peer, x.Port)))
                : candidates.SelectMany(peer => FreeConnectorPorts(peer).Where(port => port.CompatibleWith(ownPort))
                    .Select(port => (Peer: peer, Port: port))))
            .Where(x => x.Port is not null)
            .Take(2)
            .ToArray();
        if (matches.Length != 1) return false;

        var (other, otherPort) = matches[0];
        RestorePersistedConnection(other, () => ConnectPair(endpoint, ownPort, other, otherPort));
        return true;
    }

    internal static ConnectorType? FindSavedFreeConnector(IConnectable endpoint, ConnectorType saved)
    {
        if (saved.PortId is { } portId)
        {
            var matches = endpoint.Connections.Where(x => x.PortId == portId).Take(2).ToArray();
            if (matches.Length != 1 ||
                endpoint.ConnectedItems.Any(x => ReferenceEquals(x.Item1, matches[0]) || x.Item1.PortId == portId)) return null;
            return matches[0];
        }

        if (saved.PortIndex is { } index)
        {
            var physical = endpoint.Connections.ElementAtOrDefault(index);
            var occupied = endpoint.ConnectedItems.Any(x => ReferenceEquals(x.Item1, physical) ||
                (physical?.PortId is not null && x.Item1.PortId == physical.PortId));
            return physical is not null && physical.Equals(saved) && !occupied
                ? physical
                : null;
        }

        return ConnectableGameItemComponent.FindExactFreeConnector(FreeConnectorPorts(endpoint), saved);
    }

    private static ConnectorType? FindSavedFreePortById(IConnectable endpoint, string portId)
    {
        var matches = endpoint.Connections.Where(x => x.PortId == portId).Take(2).ToArray();
        return matches.Length == 1 && !endpoint.ConnectedItems.Any(x => ReferenceEquals(x.Item1, matches[0]) || x.Item1.PortId == portId)
            ? matches[0]
            : null;
    }

    private static ConnectorType? FindSavedFreePortByIndex(IConnectable endpoint, int index)
    {
        var port = endpoint.Connections.ElementAtOrDefault(index);
        return port is not null && !endpoint.ConnectedItems.Any(x => ReferenceEquals(x.Item1, port) ||
            (port.PortId is not null && x.Item1.PortId == port.PortId)) ? port : null;
    }

    private static IEnumerable<ConnectorType> FreeConnectorPorts(IConnectable endpoint) =>
        ConnectableGameItemComponent.GetFreeConnections(endpoint.Connections, endpoint.ConnectedItems);

    protected void RestorePendingConnections(IConnectable endpoint,
        List<Tuple<long, ConnectorType>> pendingConnections, bool loadMissingItems)
    {
        foreach (var saved in pendingConnections.ToList())
        {
            if (!pendingConnections.Contains(saved)) continue;
            var item = Gameworld.Items.Get(saved.Item1);
            if (item is null && loadMissingItems)
            {
                item = Gameworld.TryGetItem(saved.Item1, true);
                item?.FinaliseLoadTimeTasks();
            }

            if (!pendingConnections.Contains(saved) || item is null || (!loadMissingItems && !Parent.ColocatedWith(item)))
            {
                continue;
            }

            RestoreExactConnection(endpoint, saved.Item2, item.GetItemTypes<IConnectable>()
                .Where(peer => peer is not null && !ReferenceEquals(peer, endpoint) && peer.Independent != loadMissingItems));
        }
    }

    protected static XElement SaveConnections(IConnectable endpoint, IEnumerable<Tuple<ConnectorType, IConnectable>> connected,
        IEnumerable<Tuple<long, ConnectorType>> pendingIndependent,
        IEnumerable<Tuple<long, ConnectorType>> pendingDependent) =>
        new("ConnectedItems",
            connected.Select(x => SaveConnectionEntry(x.Item2.Parent.Id, x.Item1, x.Item2.Independent,
                FindPortIndex(endpoint, x.Item1), FindPeerPortIndex(endpoint, x.Item2, x.Item1),
                FindPortId(endpoint, x.Item1), FindPeerPortId(endpoint, x.Item2, x.Item1))),
            pendingIndependent.Select(x => SaveConnectionEntry(x.Item1, x.Item2, true, x.Item2.PortIndex,
                x.Item2.PeerPortIndex, x.Item2.PortId, x.Item2.PeerPortId)),
            pendingDependent.Select(x => SaveConnectionEntry(x.Item1, x.Item2, false, x.Item2.PortIndex,
                x.Item2.PeerPortIndex, x.Item2.PortId, x.Item2.PeerPortId)));

    protected static int? FindPortIndex(IConnectable endpoint, ConnectorType port)
    {
        var ports = endpoint.Connections.ToArray();
        for (var i = 0; i < ports.Length; i++)
        {
            if (ReferenceEquals(ports[i], port)) return i;
        }

        if (port.PortId is not null)
        {
            var identityMatches = ports.Select((candidate, index) => (candidate, index))
                .Where(x => x.candidate.PortId == port.PortId).Take(2).ToArray();
            return identityMatches.Length == 1 ? identityMatches[0].index : null;
        }

        // Equal descriptors are not enough to recover a physical connector identity.
        var matches = ports.Select((candidate, index) => (candidate, index)).Where(x => x.candidate.Equals(port)).ToArray();
        return matches.Length == 1 ? matches[0].index : null;
    }

    protected static int? FindPeerPortIndex(IConnectable endpoint, IConnectable peer, ConnectorType localPort)
    {
        var peerPorts = peer.ConnectedItems.Where(x => ReferenceEquals(x.Item2, endpoint)).Select(x => x.Item1).ToArray();
        if (localPort.PeerPortId is { } peerPortId)
        {
            var exact = peer.Connections.Where(x => x.PortId == peerPortId).Take(2).ToArray();
            var connected = peerPorts.Where(x => x.PortId == peerPortId).Take(2).ToArray();
            return exact.Length == 1 && connected.Length == 1 ? FindPortIndex(peer, exact[0]) : null;
        }

        if (localPort.PortId is { } localPortId)
        {
            var reciprocal = peerPorts.Where(x => x.PeerPortId == localPortId).Take(2).ToArray();
            if (reciprocal.Length == 1) return FindPortIndex(peer, reciprocal[0]);
            if (reciprocal.Length > 1) return null;
        }

        var compatible = peerPorts.Where(x => x.CompatibleWith(localPort)).Take(2).ToArray();
        return compatible.Length == 1 ? FindPortIndex(peer, compatible[0]) : null;
    }

    protected static string? FindPortId(IConnectable endpoint, ConnectorType port)
    {
        if (port.PortId is not null) return port.PortId;
        var physical = endpoint.Connections.Where(x => ReferenceEquals(x, port)).Take(2).ToArray();
        if (physical.Length == 1) return physical[0].PortId;
        var descriptorMatches = endpoint.Connections.Where(x => x.Equals(port)).Take(2).ToArray();
        return descriptorMatches.Length == 1 ? descriptorMatches[0].PortId : null;
    }

    protected static string? FindPeerPortId(IConnectable endpoint, IConnectable peer, ConnectorType localPort)
    {
        var peerPorts = peer.ConnectedItems.Where(x => ReferenceEquals(x.Item2, endpoint)).Select(x => x.Item1).ToArray();
        if (localPort.PeerPortId is { } peerPortId)
        {
            return peer.Connections.Count(x => x.PortId == peerPortId) == 1 && peerPorts.Count(x => x.PortId == peerPortId) == 1
                ? peerPortId
                : null;
        }

        if (localPort.PortId is { } localPortId)
        {
            var reciprocal = peerPorts.Where(x => x.PeerPortId == localPortId).Take(2).ToArray();
            if (reciprocal.Length == 1) return FindPortId(peer, reciprocal[0]);
            if (reciprocal.Length > 1) return null;
        }

        var compatible = peerPorts.Where(x => x.CompatibleWith(localPort)).Take(2).ToArray();
        return compatible.Length == 1 ? FindPortId(peer, compatible[0]) : null;
    }

    protected static XElement SaveConnectionEntry(long id, ConnectorType port, bool independent, int? portIndex = null,
        int? peerPortIndex = null, string? portId = null, string? peerPortId = null) =>
        new("Item", new XAttribute("id", id), new XAttribute("connectiontype", port),
            new XAttribute("independent", independent),
            portIndex is null ? null : new XAttribute("portindex", portIndex.Value),
            peerPortIndex is null ? null : new XAttribute("peerportindex", peerPortIndex.Value),
            portId is null ? null : new XAttribute("portid", portId),
            peerPortId is null ? null : new XAttribute("peerportid", peerPortId));

    protected static void ConsumeSavedConnection(IConnectable endpoint, List<Tuple<long, ConnectorType>> pending, long peerId,
        ConnectorType port)
    {
        var candidates = pending.Select((entry, index) => (entry, index))
            .Where(x => x.entry.Item1 == peerId)
            .ToArray();
        if (port.PortId is { } portId)
        {
            var byIdentity = candidates.Where(x => x.entry.Item2.PortId == portId).ToArray();
            if (byIdentity.Length == 1)
            {
                pending.RemoveAt(byIdentity[0].index);
                return;
            }
            if (byIdentity.Length > 1) return;
        }

        var portIndex = FindPortIndex(endpoint, port);
        if (portIndex is not null)
        {
            var byIndex = candidates.Where(x => x.entry.Item2.PortId is null && x.entry.Item2.PortIndex == portIndex &&
                x.entry.Item2.Equals(port)).ToArray();
            if (byIndex.Length == 1)
            {
                pending.RemoveAt(byIndex[0].index);
                return;
            }
        }

        var legacy = candidates.Where(x => x.entry.Item2.PortId is null && x.entry.Item2.PortIndex is null &&
            x.entry.Item2.Equals(port)).ToArray();
        if (legacy.Length == 1) pending.RemoveAt(legacy[0].index);
    }

    public abstract IGameItemComponent Copy(IGameItem newParent, bool temporary = false);

    public virtual bool PreventsMerging(IGameItemComponent component)
    {
        return false;
    }

    public virtual bool CanBePositionedAgainst(IPositionState state, PositionModifier modifier)
    {
        return true;
    }

    public virtual void Delete()
    {
		ForeignCustodyTransferContext.EnsureItem(Parent, destructive: true);
        _noSave = true;
        Changed = false;
        Gameworld.SaveManager.Abort(this);
    }

    public virtual void Quit()
    {
        // Do nothing
    }

    public virtual void Login()
    {
        // Do nothing
    }

    public virtual bool CheckPrototypeForUpdate()
    {
        if (Prototype.Status == RevisionStatus.Obsolete || Prototype.Status == RevisionStatus.Revised)
        {
            IGameItemComponentProto newProto =
                Gameworld.ItemComponentProtos.FirstOrDefault(
                    x => x.Id == Prototype.Id && x.Status == RevisionStatus.Current);
            if (newProto == null)
            {
                return false;
            }

            var dbcomp = IdHasBeenRegistered ? FMDB.Context.GameItemComponents.Find(Id) : null;
            if (IdHasBeenRegistered && dbcomp is null) return false;
            UpdateComponentNewPrototype(newProto);
            if (dbcomp is not null)
            {
                dbcomp.GameItemComponentProtoRevision = newProto.RevisionNumber;
                dbcomp.Definition = SaveToXml();
            }
            return true;
        }

        return false;
    }

	#nullable enable annotations
	private Dictionary<XName, List<XElement>>? _ambiguousLegacyChildClaims;

	protected IGameItem? RestoreOwnedChild(XElement reference)
	{
		if (!long.TryParse(reference.Value, out var childId) || childId <= 0) return null;
		if (IsAmbiguousLegacyChild(childId))
		{
			PreserveLegacyChildClaim(reference);
			return null;
		}

		var item = Gameworld.TryGetItem(childId, true);
		return item is not null && CanRestoreOwnedChild(item) ? item : null;
	}

	protected IEnumerable<XElement> AmbiguousLegacyChildClaims(XName name) =>
		_ambiguousLegacyChildClaims?.GetValueOrDefault(name) ?? Enumerable.Empty<XElement>();

	protected IGameItem? RestoreOwnedChildSlot(XElement root, XName slot)
	{
		var references = root.Elements(slot).ToArray();
		foreach (var surplus in references.Skip(1)) PreserveLegacyChildClaim(surplus);
		return references.Length == 0 ? null : RestoreOwnedChild(references[0]);
	}

	protected IEnumerable<XElement> SaveOwnedChildSlot(XName slot, long? childId)
	{
		var claims = AmbiguousLegacyChildClaims(slot);
		return childId is { } id ? claims.Prepend(new XElement(slot, id)) : claims.DefaultIfEmpty(new XElement(slot, 0));
	}

	private void PreserveLegacyChildClaim(XElement reference)
	{
		_ambiguousLegacyChildClaims ??= new Dictionary<XName, List<XElement>>();
		if (!_ambiguousLegacyChildClaims.TryGetValue(reference.Name, out var claims))
			_ambiguousLegacyChildClaims[reference.Name] = claims = new List<XElement>();
		if (!claims.Any(claim => XNode.DeepEquals(claim, reference))) claims.Add(new XElement(reference));
	}

	protected void DeleteOwnedContainedChild(IGameItem? child)
	{
		if (child is null || !IsStillOwned(child)) return;
		child.Delete();
		if (IsStillOwned(child))
			throw new InvalidOperationException($"Item #{Parent.Id} still owns item #{child.Id} after deletion.");

		bool IsStillOwned(IGameItem? item) => item is { Deleted: false } && ReferenceEquals(item.ContainedIn, Parent) &&
			!ComponentItemTransfer.HasDirectBodyCustody(item) && ComponentItemTransfer.DirectLocationOf(item) is null &&
			item.GetItemType<IBeltable>()?.ConnectedTo is null;
	}

	private bool IsAmbiguousLegacyChild(long childId) =>
		Gameworld is Futuremud world && world.HasAmbiguousLegacyItemCustody(childId);
	#nullable restore annotations

	private bool CanRestoreOwnedChild(IGameItem item)
	{
		if (!ReferenceEquals(item, Parent) && ComponentItemTransfer.IsDetached(item) &&
		    !ComponentItemTransfer.HasDirectBodyCustody(item) &&
		    (item is not GameItem { LoadedFromDatabase: true } restored ||
		     !restored.HadDirectCustodyAtLoad &&
		     (restored.ContainerIdAtLoad is null || restored.ContainerIdAtLoad == Parent.Id))) return true;

		Gameworld.DebugMessage($"Component {Prototype.Id:N0} on item {Parent.Id:N0} could not restore custody of item {item.Id:N0}.");
		return false;
	}

    public virtual bool HandleDieOrMorph(IGameItem newItem, IRoom location)
    {
        return false;
    }

	public bool HandleDieOrMorph(IGameItem newItem, IRoom location, SpatialLocation? capturedSource)
	{
		var previousSource = _activeDieOrMorphSource;
		_activeDieOrMorphSource = capturedSource;
		try
		{
			return HandleDieOrMorph(newItem, location);
		}
		finally
		{
			_activeDieOrMorphSource = previousSource;
		}
	}

    public virtual bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
    {
        return false;
    }

    public virtual bool Take(IGameItem item)
    {
        return false;
    }

    public virtual bool PreventsMovement()
    {
        return false;
    }

    public virtual void FinaliseLoad()
    {
        // Do nothing
    }

    public virtual string WhyPreventsMovement(ICharacter mover)
    {
        throw new ApplicationException(
            "WhyPreventsMovement called on GameItemComponent that did not implement an override for it.");
    }

    public virtual void ForceMove()
    {
        // Do nothing
    }

    /// <summary>
    ///     Indicates whether anything in the current status of this component prevents the item from being repositioned
    /// </summary>
    /// <returns></returns>
    public virtual bool PreventsRepositioning()
    {
        return false;
    }

    /// <summary>
    ///     Returns an error message concerning why this component cannot be repositioned
    /// </summary>
    /// <returns></returns>
    public virtual string WhyPreventsRepositioning()
    {
        throw new NotSupportedException();
    }

    public virtual double ComponentWeight => 0;
    public virtual double ComponentWeightMultiplier => 1.0;

    public virtual double ComponentBuoyancy(double fluidDensity)
    {
        return 0.0;
    }

    public virtual IBody HaveABody => null;

    public virtual bool OverridesMaterial => false;

    public virtual ISolid OverridenMaterial => null;

    #region ISaveable Members

    public override void Save()
    {
		ForeignCustodyTransferContext.RecordSave(this);
        if (_noSave || Parent is GameItem { HasSupersedingInstance: true })
        {
            Changed = false;
            return;
        }

        Models.GameItemComponent dbcomp = FMDB.Context.GameItemComponents.Find(Id);
        dbcomp?.Definition = SaveToXml();

        Changed = false;
    }

    #endregion

    public override object DatabaseInsert()
    {
        Models.GameItemComponent dbitem = new()
        {
            Definition = SaveToXml(),
            GameItem = _parentDBItem ?? FMDB.Context.GameItems.Find(Parent.Id),
            GameItemComponentProtoId = Prototype.Id,
            GameItemComponentProtoRevision = Prototype.RevisionNumber
        };
        FMDB.Context.GameItemComponents.Add(dbitem);
        _parentDBItem = null;

        return dbitem;
    }

    public override void SetIDFromDatabase(object dbitem)
    {
        Models.GameItemComponent item = (MudSharp.Models.GameItemComponent)dbitem;
        _id = item.Id;
    }

	internal void ActivateCommittedSpellComponent(Models.GameItemComponent row)
	{
		CompleteCommittedInitialisation(row);
		_noSave = false;
	}

    protected abstract void UpdateComponentNewPrototype(IGameItemComponentProto newProto);
    protected abstract string SaveToXml();

    #region IGameItemComponent Members

    /// <summary>
    /// Whether or not we should warn the purger before purging this item
    /// </summary>
    public virtual bool WarnBeforePurge => Prototype.WarnBeforePurge;

    public IGameItem Parent { get; protected set; }

	/// <summary>
	/// Places an item released by this component at the parent's exact spatial position when the
	/// supplied legacy cell is still the parent's current cell. Explicit destinations continue to
	/// use the supplied cell's normal insertion semantics.
	/// </summary>
	protected void InsertAtParentSpatialLocation(IGameItem item, IRoom fallbackLocation, bool newStack = false,
		ILocateable preferredSource = null)
	{
		if (fallbackLocation is null)
		{
			return;
		}

		if (_activeDieOrMorphSource is { } capturedSource &&
			ReferenceEquals(capturedSource.Room, fallbackLocation) &&
			fallbackLocation.RouteDefinition is not null)
		{
			item.InsertAtSpatialLocation(new SpatialLocation(
				capturedSource.Room,
				item.RoomLayer,
				capturedSource.RoutePositionMetres), newStack);
			return;
		}

		var source = preferredSource ?? Parent.LocationLevelPerceivable ?? Parent;
		if (source.Location is null || !ReferenceEquals(source.Location, fallbackLocation))
		{
			fallbackLocation.Insert(item, newStack);
			return;
		}

		item.InsertAtSource(source, newStack);
	}

    public virtual bool DesignedForOffhandUse => false;

    public abstract IGameItemComponentProto Prototype { get; }

    public virtual bool WrapFullDescription { get; } = true;

    public virtual bool AffectsLocationOnDestruction => false;

    public virtual int ComponentDieOrder => 0;

    public virtual void Taken()
    {
        // Do nothing
    }

    public virtual bool ExposeToLiquid(LiquidMixture mixture)
    {
        return false;
    }

    #endregion

    #region Decoration Properties

    public virtual bool DescriptionDecorator(DescriptionType type)
    {
        return false;
    }

    /// <summary>
    ///     Items with multiple decorating components are sorted in DecorationPriority order. Positive values of
    ///     DecorationPriority are decorated BEFORE any colour is applied, Negative values are decorated AFTER colour is
    ///     applied in descending order
    /// </summary>
    public virtual int DecorationPriority => 0;

    public virtual string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        return description;
    }

    public event EventHandler DescriptionUpdate;

    protected void HandleDescriptionUpdate()
    {
        DescriptionUpdate?.Invoke(this, new EventArgs());
    }

    protected void HandleItemLockEvent(EventType itemEvent, EventType witnessEvent, ICharacter actor, IGameItem key,
        IPerceivable containingPerceivable)
    {
        Parent.HandleEvent(itemEvent, Parent, actor, key, containingPerceivable);
        foreach (IHandleEvents witness in Parent.TrueLocations.SelectMany(x => x.EventHandlersFor(Parent.LocationLevelPerceivable)))
        {
            witness.HandleEvent(witnessEvent, Parent, actor, key, containingPerceivable, witness);
        }
    }

    #endregion

    #region IHandleEvents Members

    public virtual bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        return false;
    }

    public virtual bool HandlesEvent(params EventType[] types)
    {
        return false;
    }

    public bool InstallHook(IHook hook)
    {
        return false;
    }

    public bool RemoveHook(IHook hook)
    {
        return false;
    }

    public bool HooksChanged
    {
        get => false;
        set { }
    }

    public IEnumerable<IHook> Hooks => Enumerable.Empty<IHook>();

    #endregion
}
