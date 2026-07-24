using MudSharp.Combat;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class MusketCartridgeGameItemComponent : AmmunitionGameItemComponent, IMusketCartridge
{
    protected new MusketCartridgeGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (MusketCartridgeGameItemComponentProto)newProto;
    }

    #region Constructors
    public MusketCartridgeGameItemComponent(MusketCartridgeGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary)
    {
        _prototype = proto;
    }

    public MusketCartridgeGameItemComponent(Models.GameItemComponent component, MusketCartridgeGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public MusketCartridgeGameItemComponent(MusketCartridgeGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    protected void LoadFromXml(XElement root)
    {
        // TODO
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new MusketCartridgeGameItemComponent(this, newParent, temporary);
    }
    #endregion

    #region Saving
    protected override string SaveToXml()
    {
        return new XElement("Definition").ToString();
    }
    #endregion

    public new IAmmunitionType AmmoType => _prototype.AmmoType;
    public double BulletBore => _prototype.BulletBore;
    public IGameItemProto BulletProto => _prototype.BulletProto;
    public double? PowderMass => _prototype.PowderMass;
    public bool IncludesWad => _prototype.IncludesWad;
}
