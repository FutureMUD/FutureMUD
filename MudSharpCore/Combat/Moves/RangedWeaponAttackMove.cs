using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat.Moves;

public class RangedWeaponAttackMove : RangedWeaponAttackBase
{
	public RangedWeaponAttackMove(ICharacter assailant, IPerceiver target, IRangedWeapon weapon)
		: base(assailant, target, weapon)
	{
		SuppressAttackMessage = true;
	}

	public override string Description
		=> $"Firing {Weapon.Parent.HowSeen(Assailant)} at {Targets.FirstOrDefault()?.HowSeen(Assailant) ?? "the sky"}.";

	protected override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.RangedWeaponAttack;

	#region Overrides of CombatMoveBase

	public override double BaseDelay
		=> Weapon.WeaponType.FireCombatDelay;

	#endregion
}