using MudSharp.Framework;

namespace MudSharp.Combat;

public class RangedNaturalAttack : RangedNaturalAttackBase, IRangedNaturalAttack
{
	public RangedNaturalAttack(Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld)
	{
	}

	public RangedNaturalAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
	{
	}
}
