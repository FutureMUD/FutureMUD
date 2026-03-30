#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction.Grids;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class GridLiquidSourceGameItemComponent : GameItemComponent, ILiquidContainer, ICanConnectToLiquidGrid, IConnectable
{
	protected GridLiquidSourceGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (GridLiquidSourceGameItemComponentProto)newProto;
	}

	private ILiquidGrid? _grid;
	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];

	public ILiquidGrid? LiquidGrid
	{
		get => _grid;
		set
		{
			_grid = value;
			Changed = true;
		}
	}

	public LiquidMixture? LiquidMixture
	{
		get => LiquidGrid?.CurrentLiquidMixture;
		set { }
	}

	public double LiquidCapacity => 0.0;
	public bool CanBeEmptiedWhenInRoom => true;
	public double LiquidVolume => LiquidGrid?.TotalLiquidVolume ?? 0.0;

	public void AddLiquidQuantity(double amount, ICharacter who, string action)
	{
	}

	public void ReduceLiquidQuantity(double amount, ICharacter who, string action)
	{
	}

	public void MergeLiquid(LiquidMixture otherMixture, ICharacter who, string action)
	{
		throw new InvalidOperationException("You cannot add liquid directly to a grid liquid source.");
	}

	public LiquidMixture? RemoveLiquidAmount(double amount, ICharacter who, string action)
	{
		return LiquidGrid?.RemoveLiquidAmount(amount, who, action);
	}

	public GridLiquidSourceGameItemComponent(GridLiquidSourceGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public GridLiquidSourceGameItemComponent(Models.GameItemComponent component,
		GridLiquidSourceGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public GridLiquidSourceGameItemComponent(GridLiquidSourceGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	private void LoadFromXml(XElement root)
	{
		LiquidGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ILiquidGrid;
		var connectors = root.Element("ConnectedItems");
		if (connectors != null)
		{
			foreach (var item in connectors.Elements("Item"))
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

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new GridLiquidSourceGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Grid", LiquidGrid?.Id ?? 0),
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
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full || type == DescriptionType.Contents || type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var mixture = LiquidMixture;
		switch (type)
		{
			case DescriptionType.Contents:
				if (mixture?.IsEmpty == false)
				{
					return $"It is currently supplying {mixture.ColouredLiquidLongDescription}.";
				}

				return $"{description}\n\nIt is currently dry.";
			case DescriptionType.Evaluate:
				return string.Join("\n",
					description,
					$"It is connected to {(LiquidGrid == null ? "no liquid grid".ColourError() : $"liquid grid #{LiquidGrid.Id.ToString("N0", voyeur)}".ColourValue())}.",
					$"The grid currently has {LiquidVolume.ToString("N2", voyeur).ColourValue()} volume units available."
				);
			case DescriptionType.Full:
				var sb = new StringBuilder();
				sb.AppendLine(description);
				sb.AppendLine(
					$"It is connected to {(LiquidGrid == null ? "no liquid grid".ColourError() : $"liquid grid #{LiquidGrid.Id.ToString("N0", voyeur)}".ColourValue())}.");
				if (mixture?.IsEmpty == false)
				{
					sb.AppendLine($"It is currently supplying {mixture.ColouredLiquidLongDescription}.");
				}

				return sb.ToString();
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
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
		return true;
	}

	public bool IsOpen => true;

	public bool CanOpen(IBody opener)
	{
		return false;
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
		return WhyCannotOpenReason.NotOpenable;
	}

	public void Open()
	{
	}

	public bool CanClose(IBody closer)
	{
		return false;
	}

	public WhyCannotCloseReason WhyCannotClose(IBody closer)
	{
		return WhyCannotCloseReason.NotOpenable;
	}

	public void Close()
	{
	}

	public event OpenableEvent? OnOpen;
	public event OpenableEvent? OnClose;

	string ICanConnectToGrid.GridType => "Liquid";

	IGrid? ICanConnectToGrid.Grid
	{
		get => LiquidGrid;
		set => LiquidGrid = value as ILiquidGrid;
	}
}
