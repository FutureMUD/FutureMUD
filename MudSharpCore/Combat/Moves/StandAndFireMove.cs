using System.Linq;
using MudSharp.Character;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat.Moves;

public class StandAndFireMove : RangedWeaponAttackBase
{
	public StandAndFireMove(ICharacter assailant, ICharacter target, IRangedWeapon weapon)
		: base(assailant, target, weapon)
	{
		SuppressAttackMessage = false;
	}

	public override string Description
		=>
			$"Standing firm and firing {Weapon.Parent.HowSeen(Assailant)} at {CharacterTargets.First().HowSeen(Assailant)}.";

	protected override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.StandAndFire;
}