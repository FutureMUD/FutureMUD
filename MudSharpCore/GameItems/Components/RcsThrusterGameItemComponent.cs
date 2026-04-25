using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class RcsThrusterGameItemComponent : GameItemComponent, IZeroGravityPropulsion, IConnectable
{
	private RcsThrusterGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public RcsThrusterGameItemComponent(RcsThrusterGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public RcsThrusterGameItemComponent(MudSharp.Models.GameItemComponent component, RcsThrusterGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	public RcsThrusterGameItemComponent(RcsThrusterGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		ConnectedItem = rhs.ConnectedItem;
		ConnectedGasSupply = rhs.ConnectedGasSupply;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new RcsThrusterGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (RcsThrusterGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	public IConnectable ConnectedItem { get; private set; }
	public IGasSupply ConnectedGasSupply { get; private set; }

	public IEnumerable<ConnectorType> Connections => new[] { _prototype.Connector };

	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => ConnectedItem is null
		? Enumerable.Empty<Tuple<ConnectorType, IConnectable>>()
		: new[] { Tuple.Create(_prototype.Connector, ConnectedItem) };

	public IEnumerable<ConnectorType> FreeConnections => ConnectedItem is null
		? new[] { _prototype.Connector }
		: Enumerable.Empty<ConnectorType>();

	public bool Independent => true;

	public bool CanBeConnectedTo(IConnectable other)
	{
		return true;
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		return ConnectedItem is null &&
		       other.Parent.GetItemType<IGasSupply>() is not null &&
		       other.FreeConnections.Any(x => x.CompatibleWith(_prototype.Connector)) &&
		       other.CanBeConnectedTo(this);
	}

	public void Connect(ICharacter actor, IConnectable other)
	{
		if (!CanConnect(actor, other))
		{
			return;
		}

		RawConnect(other, _prototype.Connector);
		var otherConnector = other.FreeConnections.First(x => x.CompatibleWith(_prototype.Connector));
		other.RawConnect(this, otherConnector);
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		ConnectedItem = other;
		ConnectedGasSupply = other.Parent.GetItemType<IGasSupply>();
		Parent.ConnectedItem(other, type);
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
	{
		if (ConnectedItem is not null)
		{
			return $"{Parent.HowSeen(actor)} is already connected to a gas supply.";
		}

		if (other.Parent.GetItemType<IGasSupply>() is null)
		{
			return $"{other.Parent.HowSeen(actor)} is not a gas supply.";
		}

		if (!other.FreeConnections.Any(x => x.CompatibleWith(_prototype.Connector)))
		{
			return $"{other.Parent.HowSeen(actor)} does not have a compatible connector.";
		}

		return $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)}.";
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
		if (handleEvents && other is not null)
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
			? $"{Parent.HowSeen(actor)} is not connected to {other.Parent.HowSeen(actor)}."
			: $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)}.";
	}

	public bool CanPropel(ICharacter character)
	{
		return ConnectedGasSupply?.CanConsumeGas(_prototype.GasPerThrust) == true;
	}

	public string WhyCannotPropel(ICharacter character)
	{
		if (ConnectedGasSupply is null)
		{
			return $"{Parent.HowSeen(character)} does not have a gas supply connected.";
		}

		if (!ConnectedGasSupply.CanConsumeGas(_prototype.GasPerThrust))
		{
			return $"{ConnectedGasSupply.Parent.HowSeen(character)} does not have enough gas.";
		}

		return $"You cannot use {Parent.HowSeen(character)} for propulsion.";
	}

	public bool ConsumePropellant(ICharacter character)
	{
		if (!CanPropel(character))
		{
			return false;
		}

		ConnectedGasSupply.ConsumeGas(_prototype.GasPerThrust);
		return true;
	}
}
