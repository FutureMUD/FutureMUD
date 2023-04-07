using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ConnectableGameItemComponent : GameItemComponent, IConnectable
{
	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems =
		new();

	private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections =
		new();

	private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections =
		new();

	private ConnectableGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ConnectableGameItemComponent(this, newParent, temporary);
	}

	public override bool Die(IGameItem newItem, ICell location)
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
				$"It connects to {(_prototype.Connections.Count == 1 ? "a connector" : "connectors")} of type {_prototype.Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(x.Gender).GenderClass(true)})").ListToString()}.");
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

	public override bool Take(IGameItem item)
	{
		if (_connectedItems.RemoveAll(x => x.Item2.Parent == item) > 0)
		{
			Changed = true;
			return true;
		}

		return false;
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

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ConnectableGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ConnectedItems",
				from item in ConnectedItems
				select
					new XElement("Item", new XAttribute("id", item.Item2.Parent.Id),
						new XAttribute("connectiontype", item.Item1),
						new XAttribute("independent", item.Item2.Independent)))
		).ToString();
	}

	#region Constructors

	public ConnectableGameItemComponent(ConnectableGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ConnectableGameItemComponent(MudSharp.Models.GameItemComponent component,
		ConnectableGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("ConnectedItems");
		if (element != null)
		{
			foreach (var item in element.Elements("Item"))
			{
				if (item.Attribute("independent")?.Value == "false")
				{
					_pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id").Value),
						new ConnectorType(
							item.Attribute("connectiontype").Value)));
				}
				else
				{
					_pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id").Value),
						new ConnectorType(
							item.Attribute("connectiontype").Value)));
				}
			}
		}
	}

	public ConnectableGameItemComponent(ConnectableGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_connectedItems = rhs._connectedItems;
	}

	public override void FinaliseLoad()
	{
		foreach (var item in _pendingLoadTimeConnections.ToList())
		{
			var gitem = Gameworld.Items.Get(item.Item1);
			if (gitem == null)
			{
				continue;
			}

			var connectables = gitem.GetItemTypes<IConnectable>().ToList();
			if (!connectables.Any())
			{
				continue;
			}

			if (gitem.Location != Parent.Location)
			{
				continue;
			}

			foreach (var connectable in connectables)
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

			var connectables = gitem.GetItemTypes<IConnectable>().ToList();
			if (!connectables.Any())
			{
				continue;
			}

			foreach (var connectable in connectables)
			{
				if (connectable == null)
				{
					continue;
				}

				Connect(null, connectable);
				break;
			}
		}

		_pendingDependentLoadTimeConnections.Clear();
		Changed = true;
	}

	#endregion
}