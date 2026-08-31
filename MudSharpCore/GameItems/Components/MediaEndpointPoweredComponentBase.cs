#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

/// <summary>
/// Common physical media connector behaviour. Media packets are routed by <see cref="IMediaChannelService"/>,
/// while the normal item connection topology determines which sink endpoint has been bound to which source.
/// </summary>
public abstract class MediaEndpointPoweredComponentBase : PoweredMachineBaseGameItemComponent, IConnectable
{
	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
	private readonly List<long> _pendingConnectionIds = [];

	protected MediaEndpointPoweredComponentBase(PoweredMachineBaseGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
	}

	protected MediaEndpointPoweredComponentBase(MudSharp.Models.GameItemComponent component,
		PoweredMachineBaseGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
	}

	protected MediaEndpointPoweredComponentBase(MediaEndpointPoweredComponentBase rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
	}

	protected virtual int MediaInputPorts => this is IMediaSink ? 1 : 0;
	protected virtual int MediaOutputPorts => this is IMediaSource ? 1 : 0;
	protected virtual MediaCapabilities ConnectorCapabilities => this is IMediaEndpoint endpoint
		? endpoint.MediaCapabilities
		: MediaCapabilities.None;

	public IEnumerable<ConnectorType> Connections =>
		Enumerable.Repeat(MediaConnectionTypes.Input, MediaInputPorts)
			.Concat(Enumerable.Repeat(MediaConnectionTypes.Output, MediaOutputPorts))
			.ToList();

	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems.ToList();

	public IEnumerable<ConnectorType> FreeConnections
	{
		get
		{
			var connectors = Connections.ToList();
			foreach (var connection in _connectedItems)
			{
				connectors.Remove(connection.Item1);
			}

			return connectors;
		}
	}

	public bool Independent => true;

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
		foreach (var itemId in _pendingConnectionIds.ToList())
		{
			var item = Gameworld.TryGetItem(itemId, true);
			var connectable = item?.GetItemTypes<IConnectable>().FirstOrDefault(x => CanConnect(null, x));
			if (connectable is not null)
			{
				Connect(null, connectable);
			}
		}

		_pendingConnectionIds.Clear();
	}

	public override void Delete()
	{
		foreach (var connected in _connectedItems.Select(x => x.Item2).ToList())
		{
			RawDisconnect(connected, true);
		}

		base.Delete();
	}

	public override void Quit()
	{
		base.Quit();
	}

	public bool CanBeConnectedTo(IConnectable other)
	{
		return other is IMediaEndpoint endpoint &&
		       (ConnectorCapabilities & endpoint.MediaCapabilities) != MediaCapabilities.None;
	}

	public bool CanConnect(ICharacter? actor, IConnectable other)
	{
		if (other is not IMediaEndpoint endpoint ||
		    (ConnectorCapabilities & endpoint.MediaCapabilities) == MediaCapabilities.None ||
		    !Parent.ColocatedWith(other.Parent) || !FreeConnections.Any() || !other.FreeConnections.Any())
		{
			return false;
		}

		return other.FreeConnections.Any(x => FreeConnections.Any(y => y.CompatibleWith(x))) &&
		       other.CanBeConnectedTo(this);
	}

	public void Connect(ICharacter? actor, IConnectable other)
	{
		if (!CanConnect(actor, other))
		{
			return;
		}

		var ownConnector = FreeConnections.First(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
		var otherConnector = other.FreeConnections.First(x => x.CompatibleWith(ownConnector));
		RawConnect(other, ownConnector);
		other.RawConnect(this, otherConnector);
		BindMediaEndpoints(other, ownConnector, otherConnector);
		Parent.ConnectedItem(other, ownConnector);
		other.Parent.ConnectedItem(this, otherConnector);
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		if (_connectedItems.Any(x => ReferenceEquals(x.Item2, other)))
		{
			return;
		}

		_connectedItems.Add(Tuple.Create(type, other));
		_pendingConnectionIds.Remove(other.Parent.Id);
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter? actor, IConnectable other)
	{
		if (!Parent.ColocatedWith(other.Parent))
		{
			return $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} because they are not colocated.";
		}

		if (other is not IMediaEndpoint endpoint ||
		    (ConnectorCapabilities & endpoint.MediaCapabilities) == MediaCapabilities.None)
		{
			return $"{Parent.HowSeen(actor)} and {other.Parent.HowSeen(actor)} do not have compatible media ports.";
		}

		return !FreeConnections.Any()
			? $"{Parent.HowSeen(actor)} has no free media ports."
			: $"{other.Parent.HowSeen(actor)} has no compatible free media ports.";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return _connectedItems.Any(x => ReferenceEquals(x.Item2, other));
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other)
	{
		return CanBeDisconnectedFrom(other);
	}

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		var connections = _connectedItems.Where(x => ReferenceEquals(x.Item2, other)).ToList();
		if (!connections.Any())
		{
			return;
		}

		_connectedItems.RemoveAll(x => ReferenceEquals(x.Item2, other));
		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			foreach (var connection in connections)
			{
				Parent.DisconnectedItem(other, connection.Item1);
				other.Parent.DisconnectedItem(this, connection.Item1);
			}
		}

		ClearMediaBindingFor(other);
		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return CanBeDisconnectedFrom(other)
			? string.Empty
			: $"{Parent.HowSeen(actor)} is not connected to {other.Parent.HowSeen(actor)}.";
	}

	protected void LoadMediaEndpointState(XElement root)
	{
		_pendingConnectionIds.AddRange(root.Element("MediaConnections")?.Elements("Connection")
			.Select(x => long.TryParse(x.Attribute("id")?.Value, out var id) ? id : 0L)
			.Where(x => x > 0L) ?? Enumerable.Empty<long>());
	}

	protected XElement SaveMediaEndpointState(XElement root)
	{
		root.Add(new XElement("MediaConnections",
			_connectedItems.Select(x => new XElement("Connection", new XAttribute("id", x.Item2.Parent.Id)))));
		return root;
	}

	private void BindMediaEndpoints(IConnectable other, ConnectorType ownConnector, ConnectorType otherConnector)
	{
		if (ownConnector.Gender == Gender.Female && otherConnector.Gender == Gender.Male &&
		    this is IMediaBoundSink ownSink && other is IMediaSource otherSource)
		{
			ownSink.BindSource(otherSource.MediaEndpoint, out _);
		}

		if (ownConnector.Gender == Gender.Male && otherConnector.Gender == Gender.Female &&
		    other is IMediaBoundSink otherSink && this is IMediaSource ownSource)
		{
			otherSink.BindSource(ownSource.MediaEndpoint, out _);
		}

		if (this is CameraGameItemComponent camera)
		{
			camera.PublishCurrentSnapshot();
		}

		if (other is CameraGameItemComponent otherCamera)
		{
			otherCamera.PublishCurrentSnapshot();
		}
	}

	private void ClearMediaBindingFor(IConnectable other)
	{
		if (this is IMediaBoundSink ownSink && other is IMediaSource otherSource &&
			ownSink.SourceBinding == otherSource.MediaEndpoint)
		{
			ownSink.ClearSourceBinding();
		}

		if (other is IMediaBoundSink otherSink && this is IMediaSource ownSource &&
			otherSink.SourceBinding == ownSource.MediaEndpoint)
		{
			otherSink.ClearSourceBinding();
		}
	}
}

public abstract class MediaBoundSinkPoweredComponentBase : MediaEndpointPoweredComponentBase, IMediaBoundSink
{
	private MediaEndpointAddress? _sourceBinding;

	protected MediaBoundSinkPoweredComponentBase(PoweredMachineBaseGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
	}

	protected MediaBoundSinkPoweredComponentBase(MudSharp.Models.GameItemComponent component,
		PoweredMachineBaseGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
	}

	protected MediaBoundSinkPoweredComponentBase(MediaBoundSinkPoweredComponentBase rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_sourceBinding = rhs._sourceBinding;
	}

	public abstract MediaEndpointAddress MediaEndpoint { get; }
	public virtual MediaEndpointAddress MediaInputEndpoint => MediaEndpoint;
	public abstract MediaCapabilities MediaCapabilities { get; }
	public abstract bool MediaAvailable { get; }
	public MediaEndpointAddress? SourceBinding => _sourceBinding;
	protected virtual bool AcceptSiblingSources => false;

	public virtual bool Accepts(MediaPacket packet)
	{
		if (!MediaAvailable || packet.Source == MediaInputEndpoint || packet.HasVisited(MediaInputEndpoint) ||
		    (packet.Capabilities & MediaCapabilities) == MediaCapabilities.None)
		{
			return false;
		}

		return _sourceBinding is { } binding
			? packet.Source == binding
			: AcceptSiblingSources && packet.Source.ItemId == Parent.Id;
	}

	public abstract void ReceiveMedia(MediaPacket packet);

	public bool BindSource(MediaEndpointAddress source, out string error)
	{
		if (!source.IsValid)
		{
			error = "That is not a valid media source endpoint.";
			return false;
		}

		_sourceBinding = source;
		Changed = true;
		error = string.Empty;
		return true;
	}

	public void ClearSourceBinding()
	{
		if (_sourceBinding is null)
		{
			return;
		}

		_sourceBinding = null;
		Changed = true;
	}

	protected void ActivateMediaSink()
	{
		Gameworld.MediaChannelService.RegisterSink(this);
	}

	protected void DeactivateMediaSink()
	{
		Gameworld.MediaChannelService.UnregisterSink(this);
	}

	protected void LoadMediaSinkState(XElement root)
	{
		LoadMediaEndpointState(root);
		var binding = root.Element("SourceBinding");
		if (binding is null)
		{
			return;
		}

		_sourceBinding = new MediaEndpointAddress(
			long.TryParse(binding.Attribute("item")?.Value, out var itemId) ? itemId : 0L,
			long.TryParse(binding.Attribute("component")?.Value, out var componentId) ? componentId : 0L,
			binding.Attribute("endpoint")?.Value ?? string.Empty);
	}

	protected XElement SaveMediaSinkState(XElement root)
	{
		SaveMediaEndpointState(root);
		if (_sourceBinding is { } binding)
		{
			root.Add(new XElement("SourceBinding",
				new XAttribute("item", binding.ItemId),
				new XAttribute("component", binding.ComponentId),
				new XAttribute("endpoint", binding.EndpointKey)));
		}

		return root;
	}

	public override void Delete()
	{
		DeactivateMediaSink();
		base.Delete();
	}

	public override void Quit()
	{
		DeactivateMediaSink();
		base.Quit();
	}
}

public static class MediaConnectionTypes
{
	public static readonly ConnectorType Input = new(Gender.Female, "Media", false);
	public static readonly ConnectorType Output = new(Gender.Male, "Media", false);
}
