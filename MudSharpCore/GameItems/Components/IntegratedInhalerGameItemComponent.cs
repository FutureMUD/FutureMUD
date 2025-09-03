using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class IntegratedInhalerGameItemComponent : GameItemComponent, IPuffable
{
    protected IntegratedInhalerGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    private IGasContainer InternalContainer => Parent.GetItemType<IGasContainer>();

    public IntegratedInhalerGameItemComponent(IntegratedInhalerGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public IntegratedInhalerGameItemComponent(MudSharp.Models.GameItemComponent component, IntegratedInhalerGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
    }

    public IntegratedInhalerGameItemComponent(IntegratedInhalerGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new IntegratedInhalerGameItemComponent(this, newParent, temporary);
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (IntegratedInhalerGameItemComponentProto)newProto;
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition").ToString();
    }

    public override void FinaliseLoad()
    {
        base.FinaliseLoad();
        if (InternalContainer != null && InternalContainer.Gas == null && _prototype.InitialGas != null)
        {
            InternalContainer.Gas = _prototype.InitialGas;
            InternalContainer.GasVolumeAtOneAtmosphere = InternalContainer.GasCapacityAtOneAtmosphere;
        }
    }

    #region IPuffable

    public bool CanPuff(ICharacter character)
    {
        return InternalContainer?.Gas != null && InternalContainer.GasVolumeAtOneAtmosphere >= _prototype.GasPerPuff;
    }

    public string WhyCannotPuff(ICharacter character)
    {
        if (InternalContainer?.Gas == null || InternalContainer.GasVolumeAtOneAtmosphere <= 0)
        {
            return $"{Parent.HowSeen(character, true)} is empty.";
        }

        if (InternalContainer.GasVolumeAtOneAtmosphere < _prototype.GasPerPuff)
        {
            return $"{Parent.HowSeen(character, true)} does not have enough gas for a puff.";
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
        var gas = InternalContainer.Gas;
        if (gas?.Drug != null && gas.Drug.DrugVectors.HasFlag(DrugVector.Inhaled))
        {
            character.Body.Dose(gas.Drug, DrugVector.Inhaled, gas.DrugGramsPerUnitVolume * _prototype.GasPerPuff);
        }

        InternalContainer.GasVolumeAtOneAtmosphere -= _prototype.GasPerPuff;
        return true;
    }

    #endregion
}
