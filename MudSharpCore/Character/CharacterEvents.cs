using Microsoft.VisualBasic;
using MudSharp.Community;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems;
using System;
using System.Linq;

namespace MudSharp.Character;

public partial class Character
{
    #region Overrides of PerceivedItem

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        if (type == EventType.CharacterDiesWitness)
        {
            ICharacter victim = (ICharacter)arguments[0];
            if (victim != this)
            {
                foreach (IClanMembership cm in victim.ClanMemberships)
                {
                    if (ClanMemberships.Any(x => x.Clan == cm.Clan))
                    {
                        AddEffect(new WitnessedClanMemberDeath(this, victim, cm.Clan), TimeSpan.FromDays(7));
                    }
                }
            }
        }

        bool result = HandleCombatEvent(type, arguments);
        switch (type)
        {
            case EventType.CharacterBeginMovementWitness:
            case EventType.CharacterClosedItemWitness:
            case EventType.CharacterDamagedWitness:
            case EventType.CharacterDiesWitness:
            case EventType.CharacterDismountedWitness:
            case EventType.CharacterDoorKnockedOtherSide:
            case EventType.CharacterDoorKnockedSameSide:
            case EventType.CharacterDroppedItemWitness:
            case EventType.CharacterEatWitness:
            case EventType.CharacterEnterCellFinishWitness:
            case EventType.CharacterEnterCellWitness:
            case EventType.CharacterGiveItemWitness:
            case EventType.CharacterGotItemContainerWitness:
            case EventType.CharacterGotItemWitness:
            case EventType.CharacterHidesWitness:
            case EventType.CharacterIncapacitatedWitness:
            case EventType.CharacterLeaveCellWitness:
            case EventType.CharacterMountedWitness:
            case EventType.CharacterOpenedItemWitness:
            case EventType.CharacterPutItemContainerWitness:
            case EventType.CharacterSheatheItemWitness:
            case EventType.CharacterSocialWitness:
            case EventType.CharacterSpeaksDirectWitness:
            case EventType.CharacterSpeaksNearbyWitness:
            case EventType.CharacterSpeaksWitness:
            case EventType.CharacterStopMovementClosedDoorWitness:
            case EventType.CharacterStopMovementWitness:
            case EventType.CharacterSwallowWitness:
            case EventType.CharacterUnwieldedItemWitness:
            case EventType.CharacterWornItemRemovedWitness:
            case EventType.EngagedInCombatWitness:
            case EventType.ItemDamagedWitness:
            case EventType.ItemLockedWitness:
            case EventType.ItemUnlockedWitness:
            case EventType.ItemWieldedWitness:
            case EventType.ItemWornWitness:
            case EventType.WitnessBleedTick:
                foreach (IGameItem item in Body.ExternalItems)
                {
                    result = result || item.HandleEvent(type, arguments);
                }

                break;
            case EventType.CharacterLeaveCell:
                foreach (IGameItem item in Body.ExternalItems)
                {
                    result = result || item.HandleEvent(EventType.CharacterLeaveCellItems, arguments[0], arguments[1], arguments[2], item);
                }
                break;
            case EventType.CharacterEnterCell:
                foreach (IGameItem item in Body.ExternalItems)
                {
                    result = result || item.HandleEvent(EventType.CharacterEnterCellItems, arguments[0], arguments[1], arguments[2], item);
                }
                break;
            case EventType.CharacterBeginMovement:
                foreach (IGameItem item in Body.ExternalItems)
                {
                    result = result || item.HandleEvent(EventType.CharacterBeginMovementItems, arguments[0], arguments[1], arguments[2], item);
                }
                break;
        }


        return result || base.HandleEvent(type, arguments);
    }

    #endregion
}
