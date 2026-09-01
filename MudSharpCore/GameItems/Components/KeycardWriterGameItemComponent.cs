#nullable enable

using MudSharp.GameItems.Prototypes;
using MudSharp.Construction;

namespace MudSharp.GameItems.Components;

public class KeycardWriterGameItemComponent : PoweredMachineBaseGameItemComponent, IKeycardWriter
{
	private KeycardWriterGameItemComponentProto _prototype;
	private IConnectable? _mountedHost;
	private long? _pendingMountedHostId;
	private ConnectorType MountConnector => new(Gender.Male, $"Automation:{MountType}", false);

	public KeycardWriterGameItemComponent(KeycardWriterGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public KeycardWriterGameItemComponent(MudSharp.Models.GameItemComponent component,
		KeycardWriterGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		if (long.TryParse(XElement.Parse(component.Definition).Element("MountedTo")?.Value, out var mountedId) &&
		    mountedId > 0)
		{
			_pendingMountedHostId = mountedId;
		}
	}

	public KeycardWriterGameItemComponent(KeycardWriterGameItemComponent rhs, IGameItem parent,
		bool temporary = false) : base(rhs, parent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public string MountType => "KeycardWriter";
	public bool IsMounted => ResolveMountedHostConnectable() is not null || _pendingMountedHostId.HasValue;
	public IAutomationMountHost? MountHost => ResolveMountedHost();
	public IEnumerable<ConnectorType> Connections => [MountConnector];
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems =>
		ResolveMountedHostConnectable() is { } host ? [Tuple.Create(MountConnector, host)] : [];
	public IEnumerable<ConnectorType> FreeConnections => ResolveMountedHostConnectable() is null ? Connections : [];
	public bool Independent => false;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new KeycardWriterGameItemComponent(this, newParent, temporary);

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (KeycardWriterGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		var mountedHostId = _mountedHost?.Parent.Id ?? _pendingMountedHostId;
		if (mountedHostId.HasValue)
		{
			root.Add(new XElement("MountedTo", mountedHostId.Value));
		}
		return root;
	}

	public override void FinaliseLoad()
	{
		ResolveMountedHost();
		base.FinaliseLoad();
	}

	public override void Login()
	{
		ResolveMountedHost();
		base.Login();
	}
	protected override void OnPowerCutInAction() => HandleDescriptionUpdate();
	protected override void OnPowerCutOutAction() => HandleDescriptionUpdate();

	public bool CanWrite(out string error)
	{
		if (!SwitchedOn)
		{
			error = "The keycard writer is switched off.";
			return false;
		}
		if (!IsPowered)
		{
			error = "The keycard writer is not powered.";
			return false;
		}
		error = string.Empty;
		return true;
	}

	private IConnectable? ResolveMountedHostConnectable()
	{
		if (_mountedHost is not null)
		{
			return _mountedHost;
		}
		ResolveMountedHost();
		return _mountedHost;
	}

	private IAutomationMountHost? ResolveMountedHost()
	{
		if (_mountedHost is IAutomationMountHost host)
		{
			return host;
		}
		if (_mountedHost?.Parent.GetItemType<IAutomationMountHost>() is { } parentHost)
		{
			return parentHost;
		}
		if (!_pendingMountedHostId.HasValue)
		{
			return null;
		}

		var hostItem = Gameworld.TryGetItem(_pendingMountedHostId.Value, true);
		var resolved = hostItem?.GetItemTypes<IAutomationMountHost>()
			.FirstOrDefault(x => x.GetBayNameForMountedItem(Parent) is not null);
		_mountedHost = resolved as IConnectable;
		if (resolved is not null)
		{
			_pendingMountedHostId = null;
		}
		return resolved;
	}

	public bool CanBeConnectedTo(IConnectable other) => false;
	public bool CanConnect(ICharacter? actor, IConnectable other) =>
		_mountedHost is null && other.FreeConnections.Any(x => x.CompatibleWith(MountConnector)) &&
		other.CanBeConnectedTo(this);

	public void Connect(ICharacter? actor, IConnectable other)
	{
		_mountedHost = other;
		_pendingMountedHostId = null;
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(MountConnector)));
		RefreshPowerSourceConnection();
		Changed = true;
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		_mountedHost = other;
		_pendingMountedHostId = null;
		Parent.ConnectedItem(other, type);
		RefreshPowerSourceConnection();
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter? actor, IConnectable other) => IsMounted
		? $"{Parent.HowSeen(actor)} is already mounted."
		: $"There is no compatible {MountType.ColourCommand()} automation bay.";

	public bool CanBeDisconnectedFrom(IConnectable other) => true;
	public bool CanDisconnect(ICharacter actor, IConnectable other) => ReferenceEquals(_mountedHost, other);

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
		if (actor.Body.CanGet(Parent, 0))
		{
			actor.Body.Get(Parent, silent: true);
			return;
		}
		Parent.InsertAtSource(actor);
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		_mountedHost = null;
		_pendingMountedHostId = null;
		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			Parent.DisconnectedItem(other, MountConnector);
			other.Parent.DisconnectedItem(this, MountConnector);
		}
		RefreshPowerSourceConnection();
		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other) =>
		$"{Parent.HowSeen(actor)} is not installed in {other.Parent.HowSeen(actor)}.";
}
