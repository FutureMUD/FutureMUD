using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class FlareAmmunitionGameItemComponent : AmmunitionGameItemComponent
{
    protected FlareAmmunitionGameItemComponentProto _flarePrototype;
    public override IGameItemComponentProto Prototype => _flarePrototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        base.UpdateComponentNewPrototype(newProto);
        _flarePrototype = (FlareAmmunitionGameItemComponentProto)newProto;
    }

    #region Constructors

    public FlareAmmunitionGameItemComponent(FlareAmmunitionGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(proto, parent, temporary)
    {
        _flarePrototype = proto;
    }

    public FlareAmmunitionGameItemComponent(MudSharp.Models.GameItemComponent component,
        FlareAmmunitionGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
    {
        _flarePrototype = proto;
    }

    public FlareAmmunitionGameItemComponent(FlareAmmunitionGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _flarePrototype = rhs._flarePrototype;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new FlareAmmunitionGameItemComponent(this, newParent, temporary);
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition").ToString();
    }

    #endregion

    #region IAmmo Implementation

    public override void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
        OpposedOutcome defenseOutcome, IBodypart bodypart, IGameItem ammo, IRangedWeaponType weaponType,
        IEmoteOutput defenseEmote, RangedFireContext context = null)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);
        var completion = new ProjectileCustodyCompletion(actor, ammo, target);
        try
        {
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) || !completion.IsUnclaimed) return;
            var originRoom = actor.Location;
            if (target == null && originRoom.CurrentOverlay.OutdoorsType == RoomOutdoorsType.Outdoors)
            {
                var zone = originRoom.Zone;
                var rooms = zone.Rooms.ToArray();
                var effect = new FlareEffect(zone, _flarePrototype.FlareIllumination,
                    _flarePrototype.FlareZoneDescription, _flarePrototype.FlareZoneDescriptionColour,
                    _flarePrototype.FlareEndEmote);
                if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) || !completion.IsUnclaimed) return;
                MudSharp.NPC.AI.CommandExecutionScope.MarkCommitted(actor);
                zone.AddEffect(effect, _flarePrototype.FlareDuration);
                if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) || !completion.IsUnclaimed) return;
                zone.RecalculateLightLevel();
                EmoteOutput emote = new(new Emote(_flarePrototype.FlareBeginEmote, actor));
                foreach (IRoom room in rooms)
                {
                    if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) || !completion.IsUnclaimed) return;
                    var outdoors = room.CurrentOverlay.OutdoorsType == RoomOutdoorsType.Outdoors;
                    if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) || !completion.IsUnclaimed) return;
                    if (outdoors) room.Handle(emote);
                }
            }

            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) || !completion.IsUnclaimed) return;
            FirePrepared(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo, weaponType,
                defenseEmote, context, completion);
        }
        finally
        {
            completion.Finish();
        }
    }

    #endregion
}
