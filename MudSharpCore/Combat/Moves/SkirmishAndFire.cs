using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class SkirmishAndFire : RangedWeaponAttackBase
{
	public SkirmishAndFire(ICharacter assailant, ICharacter target, IRangedWeapon weapon)
		: base(assailant, target, weapon)
	{
		SuppressAttackMessage = false;
	}

	public override string Description
		=>
			$"Skirmishing and firing {Weapon.Parent.HowSeen(Assailant)} at {CharacterTargets.First().HowSeen(Assailant)}.";

	protected override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.SkirmishAndFire;

	private bool _calculatedStamina = false;
	private double _staminaCost = 0.0;

	public override double StaminaCost
	{
		get
		{
			if (!_calculatedStamina)
			{
				_staminaCost = MoveStaminaCost(Assailant, Weapon);
				_calculatedStamina = true;
			}

			return _staminaCost;
		}
	}

	public static double BaseStaminaCost(IFuturemud gameworld)
	{
		return gameworld.GetStaticDouble("SkirmishAndFireStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant, IRangedWeapon weapon)
	{
		return weapon.WeaponType.StaminaToFire * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	#region Overrides of CombatMoveBase

	public override double BaseDelay => 1.0;

	#endregion
}