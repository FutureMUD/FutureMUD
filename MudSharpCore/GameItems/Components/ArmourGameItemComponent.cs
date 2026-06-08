using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class ArmourGameItemComponent : GameItemComponent, IArmour, IConditionDegradingComponent
{
    protected ArmourGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;
    public bool ConditionDegradesOnUse => _prototype.ConditionMaintenance.ConditionDegradesOnUse;
    public int ItemQualityStages => _prototype.ConditionMaintenance.QualityPenaltyStages(Parent);

    public void UseCondition(ItemConditionUseContext context)
    {
        _prototype.ConditionMaintenance.UseCondition(Parent, context);
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new ArmourGameItemComponent(this, newParent, temporary);
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (ArmourGameItemComponentProto)newProto;
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition").ToString();
    }

    #region IArmour Implementation

    public IArmourType ArmourType => _prototype.ArmourType;

    public bool ApplyArmourPenalties => _prototype.ApplyArmourPenalties;

    #endregion

    #region Implementation of IAbsorbDamage

    public IDamage SufferDamage(IDamage damage, ref List<IWound> wounds)
    {
        ICharacter characterOwner = Parent.GetItemType<WearableGameItemComponent>()?.WornBy?.Actor;
        if (characterOwner == null)
        {
            return damage;
        }

        var result = _prototype.ArmourType.AbsorbDamage(damage, this, characterOwner, ref wounds, false);
        UseCondition(new ItemConditionUseContext(ItemConditionUseKind.ArmourAbsorb,
            damage.PenetrationOutcome, 0.0, damage.DamageAmount,
            Math.Max(0.0, damage.DamageAmount - result.DamageAmount), result.DamageAmount));
        return result;
    }

    public IDamage PassiveSufferDamage(IDamage damage, ref List<IWound> wounds)
    {
        ICharacter characterOwner = Parent.GetItemType<WearableGameItemComponent>()?.WornBy?.Actor;
        if (characterOwner == null)
        {
            return damage;
        }

        var result = _prototype.ArmourType.AbsorbDamage(damage, this, characterOwner, ref wounds, true);
        UseCondition(new ItemConditionUseContext(ItemConditionUseKind.ArmourAbsorb,
            damage.PenetrationOutcome, 0.0, damage.DamageAmount,
            Math.Max(0.0, damage.DamageAmount - result.DamageAmount), result.DamageAmount));
        return result;
    }

    public void ProcessPassiveWound(IWound wound)
    {
        Parent.ProcessPassiveWound(wound);
    }

    #endregion

    #region Constructors

    public ArmourGameItemComponent(ArmourGameItemComponentProto proto, IGameItem parent, bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public ArmourGameItemComponent(MudSharp.Models.GameItemComponent component, ArmourGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    private void LoadFromXml(XElement root)
    {
    }

    public ArmourGameItemComponent(ArmourGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
        newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    #endregion
}
