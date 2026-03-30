#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Grids;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class TelecommunicationsGridFeederGameItemComponent : GameItemComponent, IProducePower, IConnectable, ICanConnectToTelecommunicationsGrid
{
	protected TelecommunicationsGridFeederGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (TelecommunicationsGridFeederGameItemComponentProto)newProto;
	}

	private ITelecommunicationsGrid? _grid;
	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
	private readonly List<IConsumePower> _connectedConsumers = [];
	private readonly List<IConsumePower> _powerUsers = [];

	public ITelecommunicationsGrid? TelecommunicationsGrid
	{
		get => _grid;
		set
		{
			if (_grid == value)
			{
				return;
			}

			_grid = value;
			Changed = true;
		}
	}

	public TelecommunicationsGridFeederGameItemComponent(TelecommunicationsGridFeederGameItemComponentProto proto,
		IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public TelecommunicationsGridFeederGameItemComponent(Models.GameItemComponent component,
		TelecommunicationsGridFeederGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public TelecommunicationsGridFeederGameItemComponent(TelecommunicationsGridFeederGameItemComponent rhs,
		IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	private void LoadFromXml(XElement root)
	{
		TelecommunicationsGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
		var element = root.Element("ConnectedItems");
		if (element != null)
		{
			foreach (var item in element.Elements("Item"))
			{
				var connector = new ConnectorType(item.Attribute("connectiontype")!.Value);
				if (item.Attribute("independent")?.Value == "false")
				{
					_pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value), connector));
				}
				else
				{
					_pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value), connector));
				}
			}
		}
	}

	private void ResolveTelecommunicationsGrid()
	{
		if (TelecommunicationsGrid != null)
		{
			return;
		}

		var connectedGrid = ConnectedItems
			.Select(x => x.Item2.Parent.GetItemType<ICanConnectToTelecommunicationsGrid>()?.TelecommunicationsGrid)
			.FirstOrDefault(x => x != null);
		if (connectedGrid != null)
		{
			TelecommunicationsGrid = connectedGrid;
			return;
		}

		var creatorGrid = Parent.TrueLocations.FirstOrDefault()?
			.GameItems
			.SelectMany(x => x.GetItemTypes<TelecommunicationsGridCreatorGameItemComponent>())
			.Select(x => x.Grid)
			.FirstOrDefault();
		if (creatorGrid != null)
		{
			TelecommunicationsGrid = creatorGrid;
		}
	}

	public override void FinaliseLoad()
	{
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

				connectable.Connect(null, this);
				break;
			}
		}

		_pendingLoadTimeConnections.Clear();

		foreach (var item in _pendingDependentLoadTimeConnections.ToList())
		{
			var gitem = Gameworld.Items.Get(item.Item1);
			if (gitem == null)
			{
				gitem = Gameworld.TryGetItem(item.Item1, true);
				if (gitem == null)
				{
					continue;
				}

				gitem.FinaliseLoadTimeTasks();
			}

			foreach (var connectable in gitem.GetItemTypes<IConnectable>())
			{
				connectable.Connect(null, this);
				break;
			}
		}

		_pendingDependentLoadTimeConnections.Clear();
		ResolveTelecommunicationsGrid();
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new TelecommunicationsGridFeederGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Grid", TelecommunicationsGrid?.Id ?? 0),
			new XElement("ConnectedItems",
				from item in ConnectedItems
				select new XElement("Item",
					new XAttribute("id", item.Item2.Parent.Id),
					new XAttribute("connectiontype", item.Item1),
					new XAttribute("independent", item.Item2.Independent)))
		).ToString();
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

		var sb = new StringBuilder();
		sb.AppendLine(description);
		sb.AppendLine(
			$"It is connected to {(TelecommunicationsGrid == null ? "no telecommunications grid".ColourError() : $"telecommunications grid #{TelecommunicationsGrid.Id.ToString("N0", voyeur)}".ColourValue())}.");
		sb.AppendLine($"It can provide {MaximumPowerInWatts.ToString("N2", voyeur).ColourValue()} watts.");
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

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		if (!_connectedItems.Any())
		{
			return false;
		}

		foreach (var connectedItem in _connectedItems.Select(x => x.Item2).ToList())
		{
			connectedItem.RawDisconnect(this, true);
			var newItemConnectable = newItem?.GetItemType<IConnectable>();
			if (newItemConnectable == null)
			{
				location?.Insert(connectedItem.Parent);
			}
			else
			{
				if (newItemConnectable.CanConnect(null, connectedItem))
				{
					newItemConnectable.Connect(null, connectedItem);
				}
				else
				{
					location?.Insert(connectedItem.Parent);
				}
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
		var preventingItems =
			ConnectedItems.Where(
				              x =>
					              x.Item2.Independent &&
					              (x.Item2.Parent.InInventoryOf == null ||
					               x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
			              .ToList();
		return
			$"{Parent.HowSeen(mover)} is still connected to {preventingItems.Select(x => x.Item2.Parent.HowSeen(mover)).ListToString()}.";
	}

	public override void ForceMove()
	{
		var preventingItems =
			ConnectedItems.Where(
				              x =>
					              x.Item2.Independent &&
					              (x.Item2.Parent.InInventoryOf == null ||
					               x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
			              .ToList();
		foreach (var item in preventingItems)
		{
			RawDisconnect(item.Item2, true);
		}
	}

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
		return true;
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		if (!FreeConnections.Any() || !other.FreeConnections.Any())
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
		_pendingDependentLoadTimeConnections.RemoveAll(x => x.Item1 == other.Parent.Id && x.Item2.CompatibleWith(type));
		Parent.ConnectedItem(other, type);
		ResolveTelecommunicationsGrid();
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
		ResolveTelecommunicationsGrid();
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

	#region IProducePower Implementation

	public bool PrimaryLoadTimePowerProducer => false;
	public bool PrimaryExternalConnectionPowerProducer => false;
	public double FuelLevel => 1.0;
	public double MaximumPowerInWatts => _prototype.Wattage;
	public bool ProducingPower => true;

	public void BeginDrawdown(IConsumePower item)
	{
		if (!_connectedConsumers.Contains(item))
		{
			_connectedConsumers.Add(item);
		}

		if (!_powerUsers.Contains(item))
		{
			_powerUsers.Add(item);
			item.OnPowerCutIn();
		}
	}

	public void EndDrawdown(IConsumePower item)
	{
		_connectedConsumers.Remove(item);
		if (_powerUsers.Contains(item))
		{
			item.OnPowerCutOut();
		}

		_powerUsers.Remove(item);
	}

	public bool CanBeginDrawDown(double wattage)
	{
		return true;
	}

	public bool CanDrawdownSpike(double wattage)
	{
		return true;
	}

	public bool DrawdownSpike(double wattage)
	{
		return true;
	}

	#endregion

	#region ICanConnectToTelecommunicationsGrid Implementation

	public ITelecommunicationsGrid? Grid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value;
	}

	string ICanConnectToGrid.GridType => "Telecommunications";

	IGrid? ICanConnectToGrid.Grid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
	}

	#endregion
}
