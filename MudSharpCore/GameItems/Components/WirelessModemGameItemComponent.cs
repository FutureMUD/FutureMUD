#nullable enable

using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Construction.Grids;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Form.Shape;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class WirelessModemGameItemComponent : PoweredMachineBaseGameItemComponent, INetworkAdapter, IConnectable
{
	private readonly List<long> _pendingConnectionIds = [];
	private WirelessModemGameItemComponentProto _prototype;
	private IConnectable? _connectedHost;
	private ITelecommunicationsGrid? _resolvedTelecommunicationsGrid;
	private ITelecommunicationsGrid? _joinedTelecommunicationsGrid;
	private bool _telecommunicationsGridJoined;
	private bool _loggedIn;

	public WirelessModemGameItemComponent(WirelessModemGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public WirelessModemGameItemComponent(MudSharp.Models.GameItemComponent component,
		WirelessModemGameItemComponentProto proto,
		IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		LoadRuntimeState(XElement.Parse(component.Definition));
	}

	public WirelessModemGameItemComponent(WirelessModemGameItemComponent rhs, IGameItem newParent,
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
	public bool NetworkReady => Powered && ConnectedHost?.Powered == true && TelecommunicationsGrid is not null;
	public string? PreferredNetworkAddress =>
		string.IsNullOrWhiteSpace(_prototype.PreferredNetworkAddress)
			? null
			: _prototype.PreferredNetworkAddress.Trim();
	public string? NetworkAddress => TelecommunicationsGrid?.GetCanonicalNetworkAddress(this) ?? GetFallbackNetworkAddress();
	public long NetworkAdapterItemId => Parent.Id;

	public ITelecommunicationsGrid? TelecommunicationsGrid
	{
		get
		{
			RefreshTelecommunicationsGridMembership();
			return _resolvedTelecommunicationsGrid;
		}
	}

	public IEnumerable<ConnectorType> Connections => [ComputerConnectionTypes.NetworkPlug];

	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems
	{
		get
		{
			return _connectedHost is null
				? Enumerable.Empty<Tuple<ConnectorType, IConnectable>>()
				: [Tuple.Create(ComputerConnectionTypes.NetworkPlug, _connectedHost)];
		}
	}

	public IEnumerable<ConnectorType> FreeConnections =>
		_connectedHost is null
			? [ComputerConnectionTypes.NetworkPlug]
			: Enumerable.Empty<ConnectorType>();

	public bool Independent => true;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new WirelessModemGameItemComponent(this, newParent, temporary);
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

		var tower = ResolveCoverageTower();
		var sb = new StringBuilder(description);
		sb.AppendLine();
		sb.AppendLine();
		sb.AppendLine(
			$"Its wireless modem is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}, {(ConnectedHost is null ? "not connected to any computer host".ColourError() : $"connected to {ConnectedHost.Name.ColourName()}".ColourValue())}, {(tower is null ? "without any cellular network coverage".ColourError() : $"using cellular coverage from {tower.Parent.HowSeen(voyeur)}".ColourValue())}, and {(NetworkReady ? "network-ready".ColourValue() : "offline".ColourError())}.");
		sb.AppendLine($"Its canonical network address is {(NetworkAddress?.ColourName() ?? "unassigned".ColourError())}.");
		sb.AppendLine($"Its device identifier is {DeviceIdentifier.ColourName()}.");
		sb.AppendLine($"It is exposed on {ComputerNetworkRoutingUtilities.DescribeRoutes(NetworkRouteKeys).ColourValue()}.");
		if (TelecommunicationsGrid is not null)
		{
			sb.AppendLine($"It is presently associated with telecommunications grid #{TelecommunicationsGrid.Id.ToString("N0", voyeur).ColourValue()}.");
		}

		return sb.ToString();
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (WirelessModemGameItemComponentProto)newProto;
		RefreshTelecommunicationsGridMembership();
	}

	protected override XElement SaveToXml(XElement root)
	{
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

		base.Delete();
	}

	protected override void OnPowerCutInAction()
	{
		RefreshTelecommunicationsGridMembership();
	}

	protected override void OnPowerCutOutAction()
	{
		RefreshTelecommunicationsGridMembership();
	}

	public bool CanBeConnectedTo(IConnectable other)
	{
		return other is IComputerHost;
	}

	public bool CanConnect(ICharacter? actor, IConnectable other)
	{
		return other is IComputerHost &&
		       _connectedHost is null &&
		       other.FreeConnections.Any(x => x.CompatibleWith(ComputerConnectionTypes.NetworkPlug));
	}

	public void Connect(ICharacter? actor, IConnectable other)
	{
		if (!CanConnect(actor, other))
		{
			return;
		}

		var connection = FreeConnections.First(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
		RawConnect(other, connection);
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(connection)));
		Parent.ConnectedItem(other, connection);
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		if (type.Equals(ComputerConnectionTypes.NetworkPlug))
		{
			_connectedHost = other;
		}

		RefreshTelecommunicationsGridMembership();
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter? actor, IConnectable other)
	{
		return _connectedHost is not null
			? $"{Parent.HowSeen(actor)} is already connected to a computer host."
			: $"{Parent.HowSeen(actor)} cannot connect to {other.Parent.HowSeen(actor)}.";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return true;
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other)
	{
		return ReferenceEquals(_connectedHost, other);
	}

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		if (!ReferenceEquals(_connectedHost, other))
		{
			return;
		}

		_connectedHost = null;
		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			Parent.DisconnectedItem(other, ComputerConnectionTypes.NetworkPlug);
			other.Parent.DisconnectedItem(this, ComputerConnectionTypes.NetworkPlug);
		}

		RefreshTelecommunicationsGridMembership();
		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return $"{Parent.HowSeen(actor)} is not connected to {other.Parent.HowSeen(actor)}.";
	}

	private void LoadRuntimeState(XElement root)
	{
		_pendingConnectionIds.AddRange(root.Element("ConnectedItems")?.Elements("Connection")
			.Select(x => long.TryParse(x.Attribute("id")?.Value, out var id) ? id : 0L)
			.Where(x => x > 0) ?? Enumerable.Empty<long>());
	}

	private void RefreshTelecommunicationsGridMembership()
	{
		var resolvedGrid = ResolveCoverageGrid();
		_resolvedTelecommunicationsGrid = resolvedGrid;
		if (!ReferenceEquals(_joinedTelecommunicationsGrid, resolvedGrid))
		{
			LeaveTelecommunicationsGrid();
			_joinedTelecommunicationsGrid = resolvedGrid;
		}

		if (_loggedIn && !_telecommunicationsGridJoined && _joinedTelecommunicationsGrid is not null)
		{
			JoinTelecommunicationsGrid();
		}
	}

	private ITelecommunicationsGrid? ResolveCoverageGrid()
	{
		return ResolveCoverageTower()?.TelecommunicationsGrid;
	}

	private ICellPhoneTower? ResolveCoverageTower()
	{
		var zones = Parent.TrueLocations
			.Select(x => x?.Zone)
			.Where(x => x is not null)
			.Distinct()
			.ToList();
		if (!zones.Any())
		{
			return null;
		}

		return Gameworld.Items
			.SelectNotNull(x => x!.GetItemType<ICellPhoneTower>())
			.Where(x => zones.Any(y => x.ProvidesCoverage(y!)))
			.OrderBy(x => x.Parent.Id)
			.FirstOrDefault();
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
}
