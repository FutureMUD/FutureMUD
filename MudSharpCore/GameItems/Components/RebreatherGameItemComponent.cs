using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class RebreatherGameItemComponent : GameItemComponent, IConnectable, IProvideGasForBreathing
{
	protected RebreatherGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (RebreatherGameItemComponentProto)newProto;
	}

	public IConnectable ConnectedItem { get; private set; }
	public IGasSupply ConnectedGasSupply { get; private set; }

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
				$"It has a connector of type {_prototype.Connector.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(_prototype.Connector.Gender).GenderClass(true)}).");
			if (ConnectedItem == null)
			{
				sb.AppendLine();
			}
			else
			{
				sb.AppendLine(
					$"It is currently connected to {ConnectedItem.Parent.HowSeen(voyeur)} by a {_prototype.Connector.ConnectionType.Colour(Telnet.Green)} connection.");
				if (ConnectedGasSupply != null)
				{
					sb.AppendLine("It is currently supplying gas to breathe when worn over the mouth.");
				}
			}

			return sb.ToString();
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		if (ConnectedItem == null)
		{
			return false;
		}

		var connectedItem = ConnectedItem;
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

	public override bool AffectsLocationOnDestruction => true;

	public override bool PreventsMovement()
	{
		return ConnectedItems.Any(x => x.Item2.Independent && x.Item2.Parent.InInventoryOf != Parent.InInventoryOf);
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

	#region Constructors

	public RebreatherGameItemComponent(RebreatherGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public RebreatherGameItemComponent(MudSharp.Models.GameItemComponent component,
		RebreatherGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public RebreatherGameItemComponent(RebreatherGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new RebreatherGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	#region IConnectable Implementation

	public IEnumerable<ConnectorType> Connections => new[] { _prototype.Connector };

	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems =>
		ConnectedItem != null
			? new[] { Tuple.Create(_prototype.Connector, ConnectedItem) }
			: Enumerable.Empty<Tuple<ConnectorType, IConnectable>>();

	public IEnumerable<ConnectorType> FreeConnections =>
		ConnectedItem == null ? new[] { _prototype.Connector } : Enumerable.Empty<ConnectorType>();

	public bool Independent => true;

	public bool CanBeConnectedTo(IConnectable other)
	{
		return true;
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		if (ConnectedItem != null)
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
		if (!CanConnect(actor, other))
		{
			return;
		}

		RawConnect(other, _prototype.Connector);
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(_prototype.Connector)));
		Changed = true;
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		ConnectedItem = other;
		ConnectedGasSupply = ConnectedItem?.Parent.GetItemType<IGasSupply>();
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

		if (!other.FreeConnections.Any(x => x.CompatibleWith(_prototype.Connector)))
		{
			return
				$"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as none of the free connection points are compatible.";
		}

		return !other.CanBeConnectedTo(this)
			? $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as that item cannot be connected to."
			: $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} for an unknown reason.";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return true;
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other)
	{
		return ConnectedItem == other;
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
			Parent.DisconnectedItem(other, _prototype.Connector);
			other.Parent.DisconnectedItem(this, other.Connections.First(x => x.CompatibleWith(_prototype.Connector)));
		}

		ConnectedItem = null;
		ConnectedGasSupply = null;
		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return ConnectedItem != other
			? $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} because they are not connected!"
			: $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} for an unknown reason";
	}

	#endregion

	#region IProvideGasForBreathing

	public IGas Gas => ConnectedGasSupply?.Gas;

	public bool CanConsumeGas(double volume)
	{
		return ConnectedGasSupply?.CanConsumeGas(volume) ?? false;
	}

	public bool ConsumeGas(double volume)
	{
		return ConnectedGasSupply?.ConsumeGas(volume) ?? false;
	}

	public bool WaterTight => _prototype.WaterTight;

	#endregion
}