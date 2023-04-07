using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Combat;

public class NaturalAttack : INaturalAttack
{
	public IWeaponAttack Attack { get; set; }
	public IBodypart Bodypart { get; set; }
	public ItemQuality Quality { get; set; }

	public bool UsableAttack(ICharacter attacker, IPerceiver target, bool ignorePosition,
		params BuiltInCombatMoveType[] type)
	{
		return type.Contains(Attack.MoveType) &&
		       attacker.Body.CanUseBodypart(Bodypart) == CanUseBodypartResult.CanUse &&
		       !attacker.Body.HeldItemsFor(Bodypart).Any() &&
		       !attacker.Body.WieldedItemsFor(Bodypart).Any() &&
		       Attack.Intentions.HasFlag(attacker.CombatSettings.RequiredIntentions) &&
		       (Attack.Intentions & attacker.CombatSettings.ForbiddenIntentions) == 0 &&
		       (ignorePosition || Attack.RequiredPositionStates.Contains(attacker.PositionState)) &&
		       ((bool?)Attack.UsabilityProg?.Execute(attacker, null, target) ?? true);
	}
}