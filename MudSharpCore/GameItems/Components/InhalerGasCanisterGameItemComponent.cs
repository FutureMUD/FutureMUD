using System.Xml.Linq;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class InhalerGasCanisterGameItemComponent : GameItemComponent
{
    protected InhalerGasCanisterGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    public string CanisterType => _prototype.CanisterType;

    public InhalerGasCanisterGameItemComponent(InhalerGasCanisterGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public InhalerGasCanisterGameItemComponent(MudSharp.Models.GameItemComponent component, InhalerGasCanisterGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
    }

    public InhalerGasCanisterGameItemComponent(InhalerGasCanisterGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new InhalerGasCanisterGameItemComponent(this, newParent, temporary);
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (InhalerGasCanisterGameItemComponentProto)newProto;
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition").ToString();
    }

    public override void FinaliseLoad()
    {
        base.FinaliseLoad();
        var container = Parent.GetItemType<IGasContainer>();
        if (container != null && container.Gas == null && _prototype.InitialGas != null)
        {
            container.Gas = _prototype.InitialGas;
            container.GasVolumeAtOneAtmosphere = container.GasCapacityAtOneAtmosphere;
        }
    }
}
