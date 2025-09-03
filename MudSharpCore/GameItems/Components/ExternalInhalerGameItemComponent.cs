using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ExternalInhalerGameItemComponent : GameItemComponent, IPuffable, IConnectable
{
    protected ExternalInhalerGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    public ExternalInhalerGameItemComponent(ExternalInhalerGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public ExternalInhalerGameItemComponent(MudSharp.Models.GameItemComponent component, ExternalInhalerGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
    }

    public ExternalInhalerGameItemComponent(ExternalInhalerGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new ExternalInhalerGameItemComponent(this, newParent, temporary);
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (ExternalInhalerGameItemComponentProto)newProto;
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition").ToString();
    }

    #region Connection

    public IConnectable ConnectedItem { get; private set; }
    public IGasSupply ConnectedGasSupply { get; private set; }

    public IEnumerable<ConnectorType> Connections => new[] { _prototype.Connector };

    public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems
    {
        get
        {
            if (ConnectedItem == null)
            {
                return Enumerable.Empty<Tuple<ConnectorType, IConnectable>>();
            }

            return new[] { Tuple.Create(_prototype.Connector, ConnectedItem) };
        }
    }

    public IEnumerable<ConnectorType> FreeConnections
    {
        get
        {
            if (ConnectedItem == null)
            {
                return new[] { _prototype.Connector };
            }

            return Enumerable.Empty<ConnectorType>();
        }
    }

    public bool Independent => true;

    public bool CanBeConnectedTo(IConnectable other) => true;

    public bool CanConnect(ICharacter actor, IConnectable other)
    {
        if (ConnectedItem != null)
        {
            return false;
        }

        if (!other.FreeConnections.Any(x => x.CompatibleWith(_prototype.Connector)))
        {
            return false;
        }

        var canister = other.Parent.GetItemType<InhalerGasCanisterGameItemComponent>();
        if (canister == null)
        {
            return false;
        }

        if (!string.Equals(canister.CanisterType, _prototype.CanisterType, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        return other.CanBeConnectedTo(this);
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
        if (handleEvents && other != null)
        {
            other.RawDisconnect(this, false);
            Parent.DisconnectedItem(other, _prototype.Connector);
            other.Parent.DisconnectedItem(this, other.Connections.First(x => x.CompatibleWith(_prototype.Connector)));
        }

        ConnectedItem = null;
        ConnectedGasSupply = null;
        Changed = true;
    }

    public string WhyCannotConnect(ICharacter actor, IConnectable other)
    {
        if (ConnectedItem != null)
        {
            return $"{Parent.HowSeen(actor)} already has a canister attached.";
        }

        var canister = other.Parent.GetItemType<InhalerGasCanisterGameItemComponent>();
        if (canister == null)
        {
            return $"{other.Parent.HowSeen(actor)} is not a compatible gas canister.";
        }

        if (!string.Equals(canister.CanisterType, _prototype.CanisterType, StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{other.Parent.HowSeen(actor)} is not the right canister type.";
        }

        if (!other.FreeConnections.Any(x => x.CompatibleWith(_prototype.Connector)))
        {
            return $"{other.Parent.HowSeen(actor)} does not have a compatible connector.";
        }

        if (!other.CanBeConnectedTo(this))
        {
            return $"{other.Parent.HowSeen(actor)} cannot be connected.";
        }

        return $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)}.";
    }

    public bool CanBeDisconnectedFrom(IConnectable other) => true;

    public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
    {
        return ConnectedItem != other
            ? $"{Parent.HowSeen(actor)} is not connected to {other.Parent.HowSeen(actor)}."
            : $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} for an unknown reason.";
    }

    #endregion

    #region IPuffable

    public bool CanPuff(ICharacter character)
    {
        return ConnectedGasSupply?.CanConsumeGas(_prototype.GasPerPuff) ?? false;
    }

    public string WhyCannotPuff(ICharacter character)
    {
        if (ConnectedGasSupply == null)
        {
            return $"{Parent.HowSeen(character)} does not have a gas canister attached.";
        }

        if (!ConnectedGasSupply.CanConsumeGas(_prototype.GasPerPuff))
        {
            return $"{ConnectedGasSupply.Parent.HowSeen(character)} is empty.";
        }

        return $"You cannot puff {Parent.HowSeen(character)}.";
    }

    public bool Puff(ICharacter character, IEmote playerEmote)
    {
        if (!CanPuff(character))
        {
            character.Send(WhyCannotPuff(character));
            return false;
        }

        character.OutputHandler.Handle(new MixedEmoteOutput(new Emote("@ puff|puffs on $0", character, Parent), flags: OutputFlags.SuppressObscured).Append(playerEmote));
        var gas = ConnectedGasSupply.Gas;
        if (gas?.Drug != null && gas.Drug.DrugVectors.HasFlag(DrugVector.Inhaled))
        {
            character.Body.Dose(gas.Drug, DrugVector.Inhaled, gas.DrugGramsPerUnitVolume * _prototype.GasPerPuff);
        }

        ConnectedGasSupply.ConsumeGas(_prototype.GasPerPuff);
        return true;
    }

    #endregion
}
