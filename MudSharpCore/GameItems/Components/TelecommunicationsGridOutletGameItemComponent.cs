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

namespace MudSharp.GameItems.Components;

public class TelecommunicationsGridOutletGameItemComponent : GameItemComponent, IConnectable, IProducePower,
	IConsumePower, ICanConnectToTelecommunicationsGrid, ITelephoneNumberOwner
{
	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
	private readonly List<IConsumePower> _connectedConsumers = [];
	private readonly List<IConsumePower> _powerUsers = [];
	private TelecommunicationsGridOutletGameItemComponentProto _prototype;
	private ITelecommunicationsGrid? _grid;
	private bool _powered;
	private string? _phoneNumber;
	private string? _preferredNumber;
	private bool _allowSharedNumber;

	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (TelecommunicationsGridOutletGameItemComponentProto)newProto;
	}

	public TelecommunicationsGridOutletGameItemComponent(
		TelecommunicationsGridOutletGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public TelecommunicationsGridOutletGameItemComponent(Models.GameItemComponent component,
		TelecommunicationsGridOutletGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public TelecommunicationsGridOutletGameItemComponent(TelecommunicationsGridOutletGameItemComponent rhs,
		IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	private void LoadFromXml(XElement root)
	{
		_preferredNumber = root.Element("PreferredNumber")?.Value;
		_allowSharedNumber = bool.Parse(root.Element("AllowSharedNumber")?.Value ?? "false");
		var element = root.Element("ConnectedItems");
		if (element != null)
		{
			foreach (var item in element.Elements("Item"))
			{
				if (item.Attribute("independent")?.Value == "false")
				{
					_pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value),
						new ConnectorType(item.Attribute("connectiontype")!.Value)));
				}
				else
				{
					_pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value),
						new ConnectorType(item.Attribute("connectiontype")!.Value)));
				}
			}
		}

		TelecommunicationsGrid =
			Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new TelecommunicationsGridOutletGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Grid", TelecommunicationsGrid?.Id ?? 0),
			new XElement("PreferredNumber", _preferredNumber ?? string.Empty),
			new XElement("AllowSharedNumber", _allowSharedNumber),
			new XElement("ConnectedItems",
				from item in ConnectedItems
				select new XElement("Item",
					new XAttribute("id", item.Item2.Parent.Id),
					new XAttribute("connectiontype", item.Item1),
					new XAttribute("independent", item.Item2.Independent)))
		).ToString();
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
				Connect(null, connectable);
				break;
			}
		}

		_pendingDependentLoadTimeConnections.Clear();
		Changed = true;
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
			$"It has {(_prototype.Connections.Count == 1 ? "a connector" : "connectors")} of type {_prototype.Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(x.Gender).GenderClass(true)})").ListToString()}.");
		sb.AppendLine($"Its number is {(PhoneNumber?.ColourValue() ?? "unassigned".ColourError())}.");
		sb.AppendLine($"It is {(ProducingPower ? "powered".ColourValue() : "not powered".ColourError())}.");
		foreach (var item in ConnectedItems)
		{
			sb.AppendLine(
				$"It is currently connected to {item.Item2.Parent.HowSeen(voyeur)} by a {item.Item1.ConnectionType.Colour(Telnet.Green)} connection.");
		}

		return sb.ToString();
	}

	public string? PhoneNumber => _phoneNumber;

	public string? PreferredNumber
	{
		get => _preferredNumber;
		set
		{
			_preferredNumber = string.IsNullOrWhiteSpace(value) ? null : value;
			Changed = true;
			TelecommunicationsGrid?.RequestNumber(this, _preferredNumber, _allowSharedNumber);
		}
	}

	public bool AllowSharedNumber
	{
		get => _allowSharedNumber;
		set
		{
			_allowSharedNumber = value;
			Changed = true;
			TelecommunicationsGrid?.RequestNumber(this, _preferredNumber, _allowSharedNumber);
		}
	}

	public ITelecommunicationsGrid? TelecommunicationsGrid
	{
		get => _grid;
		set
		{
			if (_grid == value)
			{
				return;
			}

			_grid?.LeaveGrid((ITelephoneNumberOwner)this);
			_grid?.LeaveGrid((IConsumePower)this);
			_grid = value;
			_grid?.JoinGrid((ITelephoneNumberOwner)this);
			_grid?.JoinGrid((IConsumePower)this);
			Changed = true;
		}
	}

	public IEnumerable<ITelephone> ConnectedTelephones =>
		ConnectedItems.Select(x => x.Item2.Parent.GetItemType<ITelephone>())
		              .Where(x => x != null)
		              .Cast<ITelephone>()
		              .Distinct()
		              .ToList();

	public void AssignPhoneNumber(string? number)
	{
		_phoneNumber = number;
		Changed = true;
	}

	public IEnumerable<ConnectorType> Connections => _prototype.Connections;
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;

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
	public bool CanBeConnectedTo(IConnectable other) => true;

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		return FreeConnections.Any() &&
		       other.FreeConnections.Any() &&
		       other.FreeConnections.Any(x => Connections.Any(x.CompatibleWith)) &&
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

		if (!other.FreeConnections.Any(x => Connections.Any(x.CompatibleWith)))
		{
			return
				$"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as none of the free connection points are compatible.";
		}

		return !other.CanBeConnectedTo(this)
			? $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as that item cannot be connected to."
			: $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} for an unknown reason.";
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other) => _connectedItems.Any(x => x.Item2 == other);

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

	public bool CanBeDisconnectedFrom(IConnectable other) => true;

	public bool PrimaryLoadTimePowerProducer => false;
	public bool PrimaryExternalConnectionPowerProducer => true;
	public double FuelLevel => 1.0;
	public double MaximumPowerInWatts =>
		TelecommunicationsGrid != null ? TelecommunicationsGrid.TotalSupply - TelecommunicationsGrid.TotalDrawdown : 0.0;

	public void BeginDrawdown(IConsumePower item)
	{
		if (_connectedConsumers.Contains(item))
		{
			return;
		}

		_connectedConsumers.Add(item);
		if (ProducingPower)
		{
			if (!_powerUsers.Contains(item))
			{
				_powerUsers.Add(item);
				item.OnPowerCutIn();
			}
		}

		TelecommunicationsGrid?.RecalculateGrid();
	}

	public void EndDrawdown(IConsumePower item)
	{
		if (!_connectedConsumers.Remove(item))
		{
			return;
		}

		if (_powerUsers.Remove(item))
		{
			item.OnPowerCutOut();
		}
		TelecommunicationsGrid?.RecalculateGrid();
	}

	public bool CanBeginDrawDown(double wattage) => true;
	public bool CanDrawdownSpike(double wattage) => ProducingPower;

	public bool DrawdownSpike(double wattage)
	{
		return ProducingPower && (TelecommunicationsGrid?.DrawdownSpike(wattage) ?? false);
	}

	public bool ProducingPower => TelecommunicationsGrid != null && _powered;
	public double PowerConsumptionInWatts => _powerUsers.Sum(x => x.PowerConsumptionInWatts);

	public void OnPowerCutIn()
	{
		_powered = true;
		foreach (var item in _connectedConsumers.Where(x => !_powerUsers.Contains(x)).ToList())
		{
			_powerUsers.Add(item);
			item.OnPowerCutIn();
		}
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		foreach (var item in _powerUsers.ToList())
		{
			item.OnPowerCutOut();
		}

		_powerUsers.Clear();
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		TelecommunicationsGrid = null;
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
		return
			$"{Parent.HowSeen(mover)} is still connected to {preventingItems.Select(x => x.Item2.Parent.HowSeen(mover)).ListToString()}.";
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

	public override bool Take(IGameItem item)
	{
		if (_connectedItems.RemoveAll(x => x.Item2.Parent == item) <= 0)
		{
			return false;
		}

		Changed = true;
		return true;
	}

	public override void Delete()
	{
		base.Delete();
		TelecommunicationsGrid = null;
		foreach (var item in _powerUsers.ToList())
		{
			item.OnPowerCutOut();
		}

		_powerUsers.Clear();
		_connectedConsumers.Clear();
	}

	string ICanConnectToGrid.GridType => "Telecommunications";

	IGrid? ICanConnectToGrid.Grid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
	}
}
