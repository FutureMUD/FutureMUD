using System;
using System.Linq;
using Microsoft.VisualBasic;
using MudSharp.Effects.Concrete;
using MudSharp.Events;

namespace MudSharp.Character;

public partial class Character
{
	#region Overrides of PerceivedItem

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.CharacterDiesWitness)
		{
			var victim = (ICharacter)arguments[0];
			if (victim != this)
			{
				foreach (var cm in victim.ClanMemberships)
				{
					if (ClanMemberships.Any(x => x.Clan == cm.Clan))
					{
						AddEffect(new WitnessedClanMemberDeath(this, victim, cm.Clan), TimeSpan.FromDays(7));
					}
				}
			}
		}

		var result = HandleCombatEvent(type, arguments);
		switch (type)
		{
			case EventType.CharacterBeginMovementWitness:
			case EventType.CharacterDoorKnockedOtherSide:
			case EventType.CharacterDoorKnockedSameSide:
			case EventType.CharacterDroppedItemWitness:
			case EventType.CharacterEatWitness:
			case EventType.CharacterEnterCellFinishWitness:
			case EventType.CharacterEnterCellWitness:
			case EventType.CharacterGiveItemWitness:
			case EventType.CharacterGotItemContainerWitness:
			case EventType.CharacterGotItemWitness:
			case EventType.CharacterLeaveCellWitness:
			case EventType.CharacterPutItemContainerWitness:
			case EventType.CharacterSheatheItemWitness:
			case EventType.CharacterSocialWitness:
			case EventType.CharacterSpeaksDirectWitness:
			case EventType.CharacterSpeaksNearbyWitness:
			case EventType.CharacterSpeaksWitness:
			case EventType.CharacterStopMovementClosedDoorWitness:
			case EventType.CharacterStopMovementWitness:
			case EventType.CharacterSwallowWitness:
			case EventType.EngagedInCombatWitness:
			case EventType.WitnessBleedTick:
				foreach (var item in Body.ExternalItems)
				{
					result = result || item.HandleEvent(type, arguments);
				}

				break;
			case EventType.CharacterLeaveCell:
				foreach (var item in Body.ExternalItems)
				{
					result = result || item.HandleEvent(EventType.CharacterLeaveCellItems, arguments[0], arguments[1], arguments[2], item);
				}
				break;
			case EventType.CharacterEnterCell:
				foreach (var item in Body.ExternalItems)
				{
					result = result || item.HandleEvent(EventType.CharacterEnterCellItems, arguments[0], arguments[1], arguments[2], item);
				}
				break;
			case EventType.CharacterBeginMovement:
				foreach (var item in Body.ExternalItems)
				{
					result = result || item.HandleEvent(EventType.CharacterBeginMovementItems, arguments[0], arguments[1], arguments[2], item);
				}
				break;
		}


		return result || base.HandleEvent(type, arguments);
	}

	#endregion
}