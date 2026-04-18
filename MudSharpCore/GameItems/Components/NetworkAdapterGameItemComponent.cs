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
	private ITelecommunicationsGrid? _telecommunicationsGrid;
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
	public bool NetworkReady => Powered && ConnectedHost?.Powered == true && TelecommunicationsGrid is not null;
	public string? PreferredNetworkAddress =>
		string.IsNullOrWhiteSpace(_prototype.PreferredNetworkAddress)
			? null
			: _prototype.PreferredNetworkAddress.Trim();
	public string? NetworkAddress => TelecommunicationsGrid?.GetCanonicalNetworkAddress(this) ?? GetFallbackNetworkAddress();
	public long NetworkAdapterItemId => Parent.Id;
	public ITelecommunicationsGrid? TelecommunicationsGrid
	{
		get => _telecommunicationsGrid;
		set
		{
			if (ReferenceEquals(_telecommunicationsGrid, value))
			{
				return;
			}

			LeaveTelecommunicationsGrid();
			_telecommunicationsGrid = value;
			if (_loggedIn)
			{
				JoinTelecommunicationsGrid();
			}

			Changed = true;
		}
	}
	public IEnumerable<ConnectorType> Connections => [ComputerConnectionTypes.NetworkPlug];
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems =>
		_connectedHost is null
			? Enumerable.Empty<Tuple<ConnectorType, IConnectable>>()
			: new[] { Tuple.Create(ComputerConnectionTypes.NetworkPlug, _connectedHost) };
	public IEnumerable<ConnectorType> FreeConnections => _connectedHost is null ? Connections : Enumerable.Empty<ConnectorType>();
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
		return sb.ToString();
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (NetworkAdapterGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(new XElement("Grid", TelecommunicationsGrid?.Id ?? 0));
		root.Add(new XElement("ConnectedItems",
			_connectedHost is null
				? null
				: new XElement("Connection", new XAttribute("id", _connectedHost.Parent.Id))));
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
	}

	public override void Login()
	{
		base.Login();
		_loggedIn = true;
		JoinTelecommunicationsGrid();
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
	}

	protected override void OnPowerCutOutAction()
	{
	}

	public bool CanBeConnectedTo(IConnectable other)
	{
		return other is IComputerHost;
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		return _connectedHost is null &&
		       other is IComputerHost &&
		       other.FreeConnections.Any(x => x.CompatibleWith(ComputerConnectionTypes.NetworkPlug));
	}

	public void Connect(ICharacter actor, IConnectable other)
	{
		if (!CanConnect(actor, other))
		{
			return;
		}

		_connectedHost = other;
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(ComputerConnectionTypes.NetworkPlug)));
		Changed = true;
		Parent.ConnectedItem(other, ComputerConnectionTypes.NetworkPlug);
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		_connectedHost = other;
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
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

		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return $"{Parent.HowSeen(actor)} is not connected to {other.Parent.HowSeen(actor)}.";
	}

	private void LoadRuntimeState(XElement root)
	{
		_telecommunicationsGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
		_pendingConnectionIds.AddRange(root.Element("ConnectedItems")?.Elements("Connection")
			.Select(x => long.TryParse(x.Attribute("id")?.Value, out var id) ? id : 0L)
			.Where(x => x > 0) ?? Enumerable.Empty<long>());
	}

	private void JoinTelecommunicationsGrid()
	{
		if (_telecommunicationsGridJoined || TelecommunicationsGrid is null)
		{
			return;
		}

		TelecommunicationsGrid.JoinGrid((INetworkAdapter)this);
		_telecommunicationsGridJoined = true;
	}

	private void LeaveTelecommunicationsGrid()
	{
		if (!_telecommunicationsGridJoined || TelecommunicationsGrid is null)
		{
			return;
		}

		TelecommunicationsGrid.LeaveGrid((INetworkAdapter)this);
		_telecommunicationsGridJoined = false;
	}

	private string GetFallbackNetworkAddress()
	{
		return $"adapter-{NetworkAdapterItemId}";
	}

	string ICanConnectToGrid.GridType => "Telecommunications";

	IGrid? ICanConnectToGrid.Grid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
	}
}
