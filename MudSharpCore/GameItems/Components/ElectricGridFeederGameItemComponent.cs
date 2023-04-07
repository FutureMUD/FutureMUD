using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Grids;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ElectricGridFeederGameItemComponent : GameItemComponent, ICanConnectToElectricalGrid, IConnectable,
	IConsumePower, IProducePower
{
	protected ElectricGridFeederGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ElectricGridFeederGameItemComponentProto)newProto;
	}

	#region Constructors

	public ElectricGridFeederGameItemComponent(ElectricGridFeederGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		parent.OnConnected += Parent_OnConnected;
		parent.OnDisconnected += Parent_OnDisconnected;
	}

	public ElectricGridFeederGameItemComponent(MudSharp.Models.GameItemComponent component,
		ElectricGridFeederGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
		parent.OnConnected += Parent_OnConnected;
		parent.OnDisconnected += Parent_OnDisconnected;
	}

	public ElectricGridFeederGameItemComponent(ElectricGridFeederGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		newParent.OnConnected += Parent_OnConnected;
		newParent.OnDisconnected += Parent_OnDisconnected;
	}

	protected void LoadFromXml(XElement root)
	{
		ElectricalGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as IElectricalGrid;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ElectricGridFeederGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Grid", ElectricalGrid?.Id ?? 0)).ToString();
	}

	#endregion

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Full)
		{
			var sb = new StringBuilder();
			sb.AppendLine(description);
			sb.AppendLine(
				$"It has {(_prototype.Connections.Count == 1 ? "a connector" : "connectors")} of type {_prototype.Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(x.Gender).GenderClass(true)})").ListToString()}.");
			if (ConnectedItems.Any())
			{
				sb.AppendLine();
			}

			foreach (var item in ConnectedItems)
			{
				sb.AppendLine(
					$"It is currently connected to {item.Item2.Parent.HowSeen(voyeur)} by a {item.Item1.ConnectionType.Colour(Telnet.Green)} connection.");
			}

			return sb.ToString();
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	#region ICanConnectToElectricalGrid Implementation

	private IElectricalGrid _grid;

	public IElectricalGrid ElectricalGrid
	{
		get => _grid;
		set
		{
			if (_grid != null)
			{
				_grid.LeaveGrid((IProducePower)this);
			}

			_grid = value;
			if (_grid != null)
			{
				_grid.JoinGrid((IProducePower)this);
			}

			Changed = true;
		}
	}

	#endregion

	#region Implementation of IConnectable

	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems =
		new();

	private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections =
		new();

	private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections =
		new();

	public IEnumerable<ConnectorType> Connections => _prototype.Connections;
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;

	public IEnumerable<ConnectorType> FreeConnections
	{
		get
		{
			var rvar = new List<ConnectorType>(Connections);
			foreach (var item in ConnectedItems)
			{
				rvar.Remove(item.Item1);
			}

			return rvar;
		}
	}

	public bool Independent => true;

	public bool CanBeConnectedTo(IConnectable other)
	{
		return true; // TODO
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
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

	public void Connect(ICharacter actor, IConnectable other)
	{
		var connection = FreeConnections.FirstOrDefault(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
		if (connection == null)
		{
			return;
		}

		RawConnect(other, connection);
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(connection)));
		Changed = true;
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		_connectedItems.Add(Tuple.Create(type, other));
		_pendingLoadTimeConnections.RemoveAll(x => x.Item1 == other.Parent.Id && x.Item2.CompatibleWith(type));
		Parent.ConnectedItem(other, type);
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
	{
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
		return true; // TODO - reasons why this might be false
	}

	#endregion

	#region IProducePower Implementation

	public bool PrimaryLoadTimePowerProducer => false;
	public bool PrimaryExternalConnectionPowerProducer => false;
	public double FuelLevel => _connectedPowerSource?.FuelLevel ?? 0.0;
	public double MaximumPowerInWatts => _connectedPowerSource?.MaximumPowerInWatts ?? 0.0;

	public void BeginDrawdown(IConsumePower item)
	{
	}

	public void EndDrawdown(IConsumePower item)
	{
	}

	public bool CanBeginDrawDown(double wattage)
	{
		return _connectedPowerSource?.CanBeginDrawDown(wattage) ?? false;
	}

	public bool CanDrawdownSpike(double wattage)
	{
		return _connectedPowerSource?.CanDrawdownSpike(wattage) ?? false;
	}

	public bool DrawdownSpike(double wattage)
	{
		return _connectedPowerSource?.DrawdownSpike(wattage) ?? false;
	}

	public bool ProducingPower => _connectedPowerSource?.ProducingPower ?? false;

	private IProducePower _connectedPowerSource;
	private ConnectorType _connectedPowerSourceConnector;

	private void Parent_OnConnected(IConnectable other, ConnectorType type)
	{
		if (!type.Powered)
		{
			return;
		}

		var power = other.Parent.GetItemTypes<IProducePower>()
		                 .FirstOrDefault(x => x.PrimaryExternalConnectionPowerProducer);
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
		if (other.Parent == _connectedPowerSource?.Parent && type.Equals(_connectedPowerSourceConnector))
		{
			if (_connectedPowerSource.ProducingPower)
			{
				OnPowerCutOut();
			}

			_connectedPowerSource.EndDrawdown(this);
			_connectedPowerSource = null;
			_connectedPowerSourceConnector = null;
		}
	}

	#endregion

	#region IConsumePower Implementation

	public double PowerConsumptionInWatts => _connectedPowerSource?.MaximumPowerInWatts ?? 0.0;

	public void OnPowerCutIn()
	{
		ElectricalGrid?.RecalculateGrid();
	}

	public void OnPowerCutOut()
	{
		ElectricalGrid?.RecalculateGrid();
	}

	#endregion

	#region ICanConnectToGrid Implementation

	string ICanConnectToGrid.GridType => "Electrical";

	IGrid ICanConnectToGrid.Grid
	{
		get => ElectricalGrid;
		set => ElectricalGrid = value as IElectricalGrid;
	}

	#endregion
}