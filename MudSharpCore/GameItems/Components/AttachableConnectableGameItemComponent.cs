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
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Components;

/// <summary>
///     An attachable connectable item is a connectable that behaves like a IBeltable when connected, in that it is
///     "attached" to the item it is connected to
///     It only supports a single connection at a time
/// </summary>
public class AttachableConnectableGameItemComponent : GameItemComponent, IConnectable
{
	private IConnectable _connectedItem;
	private AttachableConnectableGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override bool Take(IGameItem item)
	{
		if (_connectedItem?.Parent == item)
		{
			_connectedItem.RawDisconnect(this, true);
			_connectedItem = null;
			Changed = true;
			return true;
		}

		return false;
	}

	public IEnumerable<ConnectorType> Connections => new[] { _prototype.Connector };

	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems
		=> new[] { Tuple.Create(_prototype.Connector, _connectedItem) };

	public IEnumerable<ConnectorType> FreeConnections
		=> _connectedItem != null ? Enumerable.Empty<ConnectorType>() : Connections;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new AttachableConnectableGameItemComponent(this, newParent, temporary);
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		if (_connectedItem == null)
		{
			return false;
		}

		var connectedItem = _connectedItem;
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

		return false;
	}

	#region Overrides of GameItemComponent

	public override bool AffectsLocationOnDestruction => true;

	public override int ComponentDieOrder => 1;

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
				$"It attaches to a connector of type {_prototype.Connector.ConnectionType.Colour(Telnet.Green)} and gender {Gendering.Get(_prototype.Connector.Gender).GenderClass(true).Colour(Telnet.Green)}.");
			if (_connectedItem != null)
			{
				sb.AppendLine(
					$"It is currently connected to {_connectedItem.Parent.HowSeen(voyeur)} by {_prototype.Connector.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(_prototype.Connector.Gender).GenderClass(true).Colour(Telnet.Green)})");
			}

			return sb.ToString();
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public bool Independent => false;

	public bool CanBeConnectedTo(IConnectable other)
	{
		return false; // Attachable connectables can never be the target of connections
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		if (_connectedItem != null)
		{
			return false;
		}

		if (!other.FreeConnections.Any())
		{
			return false;
		}

		return other.FreeConnections.Any(x => x.CompatibleWith(_prototype.Connector)) && other.CanBeConnectedTo(this);
	}

	public void Connect(ICharacter actor, IConnectable other)
	{
		_connectedItem = other;
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(_prototype.Connector)));
		Changed = true;
		if (Parent.GetItemType<IHoldable>()?.HeldBy != null)
		{
			Parent.GetItemType<IHoldable>().HeldBy.Take(Parent);
			return;
		}

		if (Parent.ContainedIn != null)
		{
			Parent.ContainedIn.Take(Parent);
			return;
		}

		Parent.Location?.Extract(Parent);
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		_connectedItem = other;
		Parent.ConnectedItem(other, type);
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
	{
		if (_connectedItem != null)
		{
			return
				$"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as it is already connected to {_connectedItem.Parent.HowSeen(actor)}";
		}

		if (!other.FreeConnections.Any())
		{
			return
				$"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as there are no free connection points.";
		}

		if (!other.FreeConnections.Any(x => x.CompatibleWith(_prototype.Connector)))
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
		// TODO - effects that prevent this
		return _connectedItem == other;
	}

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
		if (actor.Body.CanGet(Parent, 0))
		{
			other.Parent.InInventoryOf.Get(Parent, silent: true);
			return;
		}

		actor.Location.Insert(Parent);
		actor.Send($"You drop {Parent.HowSeen(actor)} as your {actor.Body.WielderDescriptionPlural} are full.");
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		_connectedItem = null;
		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			Parent.DisconnectedItem(other, _prototype.Connector);
			other.Parent.DisconnectedItem(this, _prototype.Connector);
		}

		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return _connectedItem != other
			? $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} because they are not connected!"
			: $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} for an unknown reason";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return true; // TODO - reasons why this might be false
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (AttachableConnectableGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ConnectedItem", _connectedItem?.Id ?? 0)
		).ToString();
	}

	#region Constructors

	public AttachableConnectableGameItemComponent(AttachableConnectableGameItemComponentProto proto,
		IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public AttachableConnectableGameItemComponent(MudSharp.Models.GameItemComponent component,
		AttachableConnectableGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private long? _connectedItemId;

	public override void FinaliseLoad()
	{
		_connectedItem = Gameworld.TryGetItem(_connectedItemId ?? 0, true)?.GetItemType<IConnectable>();
	}

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("ConnectedItem");
		if (element != null)
		{
			_connectedItemId = long.Parse(element.Value);
		}
	}

	public AttachableConnectableGameItemComponent(AttachableConnectableGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#endregion
}