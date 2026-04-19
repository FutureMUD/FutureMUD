#nullable enable

using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Construction.Grids;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class NetworkAdapterGameItemComponent : PoweredMachineBaseGameItemComponent, INetworkAdapter, IConnectable,
	ICanConnectToTelecommunicationsGrid
{
	private readonly List<long> _pendingConnectionIds = [];
	private NetworkAdapterGameItemComponentProto _prototype;
	private IConnectable? _connectedHost;
	private INetworkInfrastructure? _connectedInfrastructure;
	private ITelecommunicationsGrid? _directTelecommunicationsGrid;
	private ITelecommunicationsGrid? _joinedTelecommunicationsGrid;
	private bool _telecommunicationsGridJoined;
	private bool _loggedIn;

	public NetworkAdapterGameItemComponent(NetworkAdapterGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public NetworkAdapterGameItemComponent(MudSharp.Models.GameItemComponent component,
		NetworkAdapterGameItemComponentProto proto,
		IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		LoadRuntimeState(XElement.Parse(component.Definition));
	}

	public NetworkAdapterGameItemComponent(NetworkAdapterGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public IComputerHost? ConnectedHost => _connectedHost as IComputerHost;
	public bool Powered => IsPowered;
	public bool PublicNetworkEnabled => _prototype.PublicNetworkEnabled;
	public string? ExchangeSubnetId =>
		string.IsNullOrWhiteSpace(_prototype.ExchangeSubnetId)
			? null
			: _prototype.ExchangeSubnetId.Trim();
	public IEnumerable<string> VpnNetworkIds => _prototype.VpnNetworkIds.ToList();
	public IEnumerable<string> NetworkRouteKeys => ComputerNetworkRoutingUtilities.GetRouteKeys(this);
	public string DeviceIdentifier => ComputerNetworkRoutingUtilities.GetDeviceIdentifier(NetworkAdapterItemId);
	public bool NetworkReady => Powered && ConnectedHost?.Powered == true && NetworkTransportReady && TelecommunicationsGrid is not null;
	public string? PreferredNetworkAddress =>
		string.IsNullOrWhiteSpace(_prototype.PreferredNetworkAddress)
			? null
			: _prototype.PreferredNetworkAddress.Trim();
	public string? NetworkAddress => TelecommunicationsGrid?.GetCanonicalNetworkAddress(this) ?? GetFallbackNetworkAddress();
	public long NetworkAdapterItemId => Parent.Id;
	public bool NetworkTransportReady =>
		_connectedInfrastructure?.NetworkTransportReady ??
		(_directTelecommunicationsGrid is not null);
	public ITelecommunicationsGrid? TelecommunicationsGrid
	{
		get => _connectedInfrastructure?.TelecommunicationsGrid ?? _directTelecommunicationsGrid;
		set
		{
			if (ReferenceEquals(_directTelecommunicationsGrid, value))
			{
				return;
			}

			_directTelecommunicationsGrid = value;
			RefreshTelecommunicationsGridMembership();
			Changed = true;
		}
	}

	public IEnumerable<ConnectorType> Connections =>
	[
		ComputerConnectionTypes.NetworkPlug,
		ComputerConnectionTypes.NetworkUplinkPlug
	];

	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems
	{
		get
		{
			List<Tuple<ConnectorType, IConnectable>> items = [];
			if (_connectedHost is not null)
			{
				items.Add(Tuple.Create(ComputerConnectionTypes.NetworkPlug, _connectedHost));
			}

			if (_connectedInfrastructure is IConnectable connectable)
			{
				items.Add(Tuple.Create(ComputerConnectionTypes.NetworkUplinkPlug, connectable));
			}

			return items;
		}
	}

	public IEnumerable<ConnectorType> FreeConnections
	{
		get
		{
			List<ConnectorType> free = [];
			if (_connectedHost is null)
			{
				free.Add(ComputerConnectionTypes.NetworkPlug);
			}

			if (_connectedInfrastructure is null)
			{
				free.Add(ComputerConnectionTypes.NetworkUplinkPlug);
			}

			return free;
		}
	}

	public bool Independent => true;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new NetworkAdapterGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour,
		PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return description;
		}

		var sb = new StringBuilder(description);
		sb.AppendLine();
		sb.AppendLine();
		sb.AppendLine(
			$"Its network adapter is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}, {(ConnectedHost is null ? "not connected to any computer host".ColourError() : $"connected to {ConnectedHost.Name.ColourName()}".ColourValue())}, {(TelecommunicationsGrid is null ? "not attached to any telecommunications grid".ColourError() : $"attached to telecommunications grid #{TelecommunicationsGrid.Id.ToString("N0", voyeur)}".ColourValue())}, and {(NetworkReady ? "network-ready".ColourValue() : "offline".ColourError())}.");
		sb.AppendLine($"Its canonical network address is {(NetworkAddress?.ColourName() ?? "unassigned".ColourError())}.");
		sb.AppendLine($"Its device identifier is {DeviceIdentifier.ColourName()}.");
		sb.AppendLine($"It is exposed on {ComputerNetworkRoutingUtilities.DescribeRoutes(NetworkRouteKeys).ColourValue()}.");
		if (_connectedInfrastructure is IConnectable infrastructure)
		{
			sb.AppendLine($"It is uplinked through {infrastructure.Parent.HowSeen(voyeur)}.");
		}

		return sb.ToString();
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (NetworkAdapterGameItemComponentProto)newProto;
		RefreshTelecommunicationsGridMembership();
	}

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(new XElement("Grid", _directTelecommunicationsGrid?.Id ?? 0));
		root.Add(new XElement("ConnectedItems",
			from item in ConnectedItems
			select new XElement("Connection", new XAttribute("id", item.Item2.Parent.Id))));
		return root;
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
		foreach (var id in _pendingConnectionIds.ToList())
		{
			var item = Gameworld.TryGetItem(id, true);
			var connectable = item?.GetItemTypes<IConnectable>().FirstOrDefault(x => CanConnect(null, x));
			if (connectable is null)
			{
				continue;
			}

			Connect(null, connectable);
		}

		_pendingConnectionIds.Clear();
		RefreshTelecommunicationsGridMembership();
	}

	public override void Login()
	{
		base.Login();
		_loggedIn = true;
		RefreshTelecommunicationsGridMembership();
	}

	public override void Quit()
	{
		_loggedIn = false;
		LeaveTelecommunicationsGrid();
		base.Quit();
	}

	public override void Delete()
	{
		_loggedIn = false;
		LeaveTelecommunicationsGrid();
		if (_connectedHost is not null)
		{
			RawDisconnect(_connectedHost, true);
		}

		if (_connectedInfrastructure is IConnectable connectable)
		{
			RawDisconnect(connectable, true);
		}

		base.Delete();
	}

	protected override void OnPowerCutInAction()
	{
	}

	protected override void OnPowerCutOutAction()
	{
	}

	public bool CanBeConnectedTo(IConnectable other)
	{
		return other is IComputerHost or INetworkInfrastructure;
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		if (other is IComputerHost)
		{
			return _connectedHost is null &&
			       other.FreeConnections.Any(x => x.CompatibleWith(ComputerConnectionTypes.NetworkPlug));
		}

		if (other is INetworkInfrastructure)
		{
			return _connectedInfrastructure is null &&
			       other.FreeConnections.Any(x => x.CompatibleWith(ComputerConnectionTypes.NetworkUplinkPlug));
		}

		return false;
	}

	public void Connect(ICharacter actor, IConnectable other)
	{
		if (!CanConnect(actor, other))
		{
			return;
		}

		var connection = FreeConnections.First(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
		RawConnect(other, connection);
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(connection)));
		Parent.ConnectedItem(other, connection);
		other.Parent.ConnectedItem(this, connection);
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		if (type.ConnectionType.EqualTo(ComputerConnectionTypes.NetworkPlug.ConnectionType))
		{
			_connectedHost = other;
		}
		else if (type.ConnectionType.EqualTo(ComputerConnectionTypes.NetworkUplinkPlug.ConnectionType))
		{
			_connectedInfrastructure = other as INetworkInfrastructure;
		}

		RefreshTelecommunicationsGridMembership();
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
	{
		if (other is IComputerHost && _connectedHost is not null)
		{
			return $"{Parent.HowSeen(actor)} is already connected to a computer host.";
		}

		if (other is INetworkInfrastructure && _connectedInfrastructure is not null)
		{
			return $"{Parent.HowSeen(actor)} is already uplinked to a network infrastructure device.";
		}

		return $"{Parent.HowSeen(actor)} cannot connect to {other.Parent.HowSeen(actor)}.";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return true;
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other)
	{
		return ReferenceEquals(_connectedHost, other) ||
		       ReferenceEquals(_connectedInfrastructure as IConnectable, other);
	}

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		bool removed = false;
		if (ReferenceEquals(_connectedHost, other))
		{
			_connectedHost = null;
			removed = true;
		}

		if (ReferenceEquals(_connectedInfrastructure as IConnectable, other))
		{
			_connectedInfrastructure = null;
			removed = true;
		}

		if (!removed)
		{
			return;
		}

		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			Parent.DisconnectedItem(other, GetConnectorTypeFor(other));
			other.Parent.DisconnectedItem(this, GetConnectorTypeFor(other));
		}

		RefreshTelecommunicationsGridMembership();
		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return $"{Parent.HowSeen(actor)} is not connected to {other.Parent.HowSeen(actor)}.";
	}

	internal void RefreshTelecommunicationsGridTopology()
	{
		RefreshTelecommunicationsGridMembership();
	}

	private void LoadRuntimeState(XElement root)
	{
		_directTelecommunicationsGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
		_pendingConnectionIds.AddRange(root.Element("ConnectedItems")?.Elements("Connection")
			.Select(x => long.TryParse(x.Attribute("id")?.Value, out var id) ? id : 0L)
			.Where(x => x > 0) ?? Enumerable.Empty<long>());
	}

	private void RefreshTelecommunicationsGridMembership()
	{
		var resolvedGrid = TelecommunicationsGrid;
		if (ReferenceEquals(_joinedTelecommunicationsGrid, resolvedGrid))
		{
			return;
		}

		LeaveTelecommunicationsGrid();
		_joinedTelecommunicationsGrid = resolvedGrid;
		if (_loggedIn && _joinedTelecommunicationsGrid is not null)
		{
			JoinTelecommunicationsGrid();
		}
	}

	private void JoinTelecommunicationsGrid()
	{
		if (_telecommunicationsGridJoined || _joinedTelecommunicationsGrid is null)
		{
			return;
		}

		_joinedTelecommunicationsGrid.JoinGrid((INetworkAdapter)this);
		_telecommunicationsGridJoined = true;
	}

	private void LeaveTelecommunicationsGrid()
	{
		if (!_telecommunicationsGridJoined || _joinedTelecommunicationsGrid is null)
		{
			return;
		}

		_joinedTelecommunicationsGrid.LeaveGrid((INetworkAdapter)this);
		_telecommunicationsGridJoined = false;
	}

	private string GetFallbackNetworkAddress()
	{
		return $"adapter-{NetworkAdapterItemId}";
	}

	private static ConnectorType GetConnectorTypeFor(IConnectable other)
	{
		return other is INetworkInfrastructure
			? ComputerConnectionTypes.NetworkUplinkPlug
			: ComputerConnectionTypes.NetworkPlug;
	}

	string ICanConnectToGrid.GridType => "Telecommunications";

	IGrid? ICanConnectToGrid.Grid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
	}
}
