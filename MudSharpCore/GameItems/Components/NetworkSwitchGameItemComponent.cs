#nullable enable

using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Construction;
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

public class NetworkSwitchGameItemComponent : PoweredMachineBaseGameItemComponent, INetworkInfrastructure, IConnectable,
	ICanConnectToTelecommunicationsGrid
{
	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
	private NetworkSwitchGameItemComponentProto _prototype;
	private INetworkInfrastructure? _connectedInfrastructure;
	private ITelecommunicationsGrid? _directTelecommunicationsGrid;

	public NetworkSwitchGameItemComponent(NetworkSwitchGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public NetworkSwitchGameItemComponent(MudSharp.Models.GameItemComponent component,
		NetworkSwitchGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		LoadRuntimeState(XElement.Parse(component.Definition));
	}

	public NetworkSwitchGameItemComponent(NetworkSwitchGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
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
			NotifyConnectedAdaptersTopologyChanged();
			Changed = true;
		}
	}

	public bool NetworkTransportReady =>
		IsPowered && SwitchedOn &&
		(_connectedInfrastructure?.NetworkTransportReady ?? _directTelecommunicationsGrid is not null) &&
		TelecommunicationsGrid is not null;
	public IEnumerable<ConnectorType> Connections => _prototype.Connections;
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems.ToList();
	public IEnumerable<ConnectorType> FreeConnections
	{
		get
		{
			var remaining = new List<ConnectorType>(Connections);
			foreach (var item in ConnectedItems)
			{
				remaining.Remove(item.Item1);
			}

			return remaining;
		}
	}

	public bool Independent => true;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new NetworkSwitchGameItemComponent(this, newParent, temporary);
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
			return description;
		}

		var sb = new StringBuilder(description);
		sb.AppendLine();
		sb.AppendLine();
		sb.AppendLine(
			$"Its network switch is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}, {(TelecommunicationsGrid is null ? "not attached to any telecommunications grid".ColourError() : $"attached to telecommunications grid #{TelecommunicationsGrid.Id.ToString("N0", voyeur)}".ColourValue())}, and {(NetworkTransportReady ? "passing network traffic".ColourValue() : "offline".ColourError())}.");
		sb.AppendLine($"It has {ConnectedItems.Count().ToString("N0", voyeur).ColourValue()} active network {"connection".Pluralise(ConnectedItems.Count() != 1)}.");
		if (_connectedInfrastructure is IConnectable infrastructure)
		{
			sb.AppendLine($"Its uplink currently passes through {infrastructure.Parent.HowSeen(voyeur)}.");
		}

		return sb.ToString();
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (NetworkSwitchGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(new XElement("Grid", _directTelecommunicationsGrid?.Id ?? 0));
		root.Add(new XElement("ConnectedItems",
			from item in ConnectedItems
			select new XElement("Item",
				new XAttribute("id", item.Item2.Parent.Id),
				new XAttribute("connectiontype", item.Item1),
				new XAttribute("independent", item.Item2.Independent))));
		return root;
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
		foreach (var item in _pendingLoadTimeConnections.ToList())
		{
			var gitem = Gameworld.Items.Get(item.Item1);
			if (gitem == null || gitem.Location != Parent.Location)
			{
				continue;
			}

			foreach (var connectable in gitem.GetItemTypes<IConnectable>())
			{
				if (!connectable.CanConnect(null, this))
				{
					continue;
				}

				Connect(null, connectable);
				break;
			}
		}

		_pendingLoadTimeConnections.Clear();
		foreach (var item in _pendingDependentLoadTimeConnections.ToList())
		{
			var gitem = Gameworld.Items.Get(item.Item1) ?? Gameworld.TryGetItem(item.Item1, true);
			if (gitem == null)
			{
				continue;
			}

			gitem.FinaliseLoadTimeTasks();
			foreach (var connectable in gitem.GetItemTypes<IConnectable>())
			{
				if (!CanConnect(null, connectable))
				{
					continue;
				}

				Connect(null, connectable);
				break;
			}
		}

		_pendingDependentLoadTimeConnections.Clear();
	}

	public override void Delete()
	{
		foreach (var item in _connectedItems.Select(x => x.Item2).Distinct().ToList())
		{
			RawDisconnect(item, true);
		}

		base.Delete();
	}

	protected override void OnPowerCutInAction()
	{
		NotifyConnectedAdaptersTopologyChanged();
	}

	protected override void OnPowerCutOutAction()
	{
		NotifyConnectedAdaptersTopologyChanged();
	}

	public bool CanBeConnectedTo(IConnectable other)
	{
		return other is INetworkAdapter or INetworkInfrastructure;
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		if (other is not INetworkAdapter && other is not INetworkInfrastructure)
		{
			return false;
		}

		return FreeConnections.Any() &&
		       other.FreeConnections.Any() &&
		       other.FreeConnections.Any(x => Connections.Any(x.CompatibleWith)) &&
		       other.CanBeConnectedTo(this);
	}

	public void Connect(ICharacter actor, IConnectable other)
	{
		var connection = FreeConnections.FirstOrDefault(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
		if (connection is null)
		{
			return;
		}

		RawConnect(other, connection);
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(connection)));
		Parent.ConnectedItem(other, connection);
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		_connectedItems.Add(Tuple.Create(type, other));
		if (other is INetworkInfrastructure infrastructure &&
		    type.Equals(ComputerConnectionTypes.NetworkUplinkPlug))
		{
			_connectedInfrastructure = infrastructure;
		}

		_pendingLoadTimeConnections.RemoveAll(x => x.Item1 == other.Parent.Id && x.Item2.CompatibleWith(type));
		_pendingDependentLoadTimeConnections.RemoveAll(x => x.Item1 == other.Parent.Id && x.Item2.CompatibleWith(type));
		NotifyConnectedAdaptersTopologyChanged();
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
	{
		return $"{Parent.HowSeen(actor)} has no compatible free network switch connection ports.";
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
			foreach (var connection in _connectedItems.Where(x => x.Item2 == other).ToList())
			{
				Parent.DisconnectedItem(other, connection.Item1);
				other.Parent.DisconnectedItem(this, connection.Item1);
			}
		}

		_connectedItems.RemoveAll(x => x.Item2 == other);
		if (ReferenceEquals(_connectedInfrastructure as IConnectable, other))
		{
			_connectedInfrastructure = null;
		}

		NotifyConnectedAdaptersTopologyChanged();
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

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		foreach (var connectedItem in _connectedItems.Select(x => x.Item2).Distinct().ToList())
		{
			connectedItem.RawDisconnect(this, true);
			var newItemConnectable = newItem?.GetItemType<IConnectable>();
			if (newItemConnectable == null)
			{
				location?.Insert(connectedItem.Parent);
				continue;
			}

			if (newItemConnectable.CanConnect(null, connectedItem))
			{
				newItemConnectable.Connect(null, connectedItem);
			}
			else
			{
				location?.Insert(connectedItem.Parent);
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
		var preventingItems = ConnectedItems.Where(
				x => x.Item2.Independent &&
				     (x.Item2.Parent.InInventoryOf == null || x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
			.ToList();
		return $"{Parent.HowSeen(mover)} is still connected to {preventingItems.Select(x => x.Item2.Parent.HowSeen(mover)).ListToString()}.";
	}

	public override void ForceMove()
	{
		foreach (var item in ConnectedItems.Where(
			         x => x.Item2.Independent &&
			              (x.Item2.Parent.InInventoryOf == null || x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
			         .ToList())
		{
			RawDisconnect(item.Item2, true);
		}
	}

	private void LoadRuntimeState(XElement root)
	{
		_directTelecommunicationsGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
		var element = root.Element("ConnectedItems");
		if (element == null)
		{
			return;
		}

		foreach (var item in element.Elements("Item"))
		{
			var tuple = Tuple.Create(
				long.Parse(item.Attribute("id")!.Value),
				new ConnectorType(item.Attribute("connectiontype")!.Value));
			if (item.Attribute("independent")?.Value == "false")
			{
				_pendingDependentLoadTimeConnections.Add(tuple);
			}
			else
			{
				_pendingLoadTimeConnections.Add(tuple);
			}
		}
	}

	private void NotifyConnectedAdaptersTopologyChanged()
	{
		foreach (var adapter in _connectedItems
			         .Select(x => x.Item2)
			         .OfType<NetworkAdapterGameItemComponent>()
			         .ToList())
		{
			adapter.RefreshTelecommunicationsGridTopology();
		}

		foreach (var switchItem in _connectedItems
			         .Select(x => x.Item2)
			         .OfType<NetworkSwitchGameItemComponent>()
			         .ToList())
		{
			switchItem.NotifyConnectedAdaptersTopologyChanged();
		}
	}

	string ICanConnectToGrid.GridType => "Telecommunications";

	IGrid? ICanConnectToGrid.Grid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
	}
}
