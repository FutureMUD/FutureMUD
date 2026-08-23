#nullable enable

using MudSharp.Character;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat.Moves;

public sealed class MountedWeaponAttackMove : MeleeWeaponAttack
{
	private readonly bool _isChargeFollowUp;

	public MountedWeaponAttackMove(ICharacter owner, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target,
		bool isChargeFollowUp = false)
		: base(owner, weapon, attack, target)
	{
		_isChargeFollowUp = isChargeFollowUp;
	}

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.UseWeaponAttack;
	public override string Description => "delivering a weapon attack from a mounted charge";

	public static bool CanUse(ICharacter assailant, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target)
	{
		return MountedCombatService.Instance.ResolveContext(assailant) is not null &&
		       attack.MoveType == BuiltInCombatMoveType.MountedWeaponAttack &&
		       attack.UsableAttack(assailant, weapon.Parent, target, weapon.HandednessForWeapon(assailant), false,
			       BuiltInCombatMoveType.MountedWeaponAttack);
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (!_isChargeFollowUp || MountedCombatService.Instance.ResolveContext(Assailant) is null)
		{
			Assailant.OutputHandler.Send("That attack can only be delivered as part of a mounted charge.");
			return CombatMoveResult.Irrelevant;
		}

		return base.ResolveMove(defenderMove);
	}
}
