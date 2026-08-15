using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Vehicles;

namespace MudSharp.Combat;

public class NaturalAttack : INaturalAttack
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"Natural Attack of {Attack.Name} (#{Attack.Id:N0}) with {Bodypart.FullDescription()} @ {Quality.DescribeEnum()}";
    }

    public bool IsSimilarTo(IWeaponAttack attack, IBodypart part)
    {
        return Attack == attack && Bodypart == part;
    }

    public IWeaponAttack Attack { get; set; }
    public IBodypart Bodypart { get; set; }
    public ItemQuality Quality { get; set; }

    public bool UsableAttack(ICharacter attacker, IPerceiver target, bool ignorePosition,
        params BuiltInCombatMoveType[] type)
    {
        return type.Contains(Attack.MoveType) &&
		       IsValidTarget(Attack, target) &&
		       attacker.Body.Bodyparts.Contains(Bodypart) &&
               attacker.Body.CanUseBodypart(Bodypart) == CanUseBodypartResult.CanUse &&
               !attacker.Body.HeldItemsFor(Bodypart).Any() &&
               !attacker.Body.WieldedItemsFor(Bodypart).Any() &&
               Attack.Intentions.HasFlag(attacker.CombatSettings.RequiredIntentions) &&
               (Attack.Intentions & attacker.CombatSettings.ForbiddenIntentions) == 0 &&
               (ignorePosition || Attack.RequiredPositionStates.Contains(attacker.PositionState)) &&
               (Attack.UsabilityProg?.ExecuteBool(attacker, null, target) ?? true);
    }

	public static bool IsValidTarget(IWeaponAttack attack, IPerceiver target)
	{
		if (attack.MoveType != BuiltInCombatMoveType.AquaticVehicleAttack)
		{
			return true;
		}

		return target is ICharacter character &&
		       VehicleCombatService.Instance.VehicleFor(character)?.ExteriorItem is { Deleted: false, Destroyed: false };
	}
}
