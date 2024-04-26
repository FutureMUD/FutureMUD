using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class ParryMove : CombatMoveBase, IDefenseMove
{
	public IMeleeWeapon Weapon { get; init; }

	public override string Description
		=>
			$"Parrying {CharacterTargets.First().HowSeen(Assailant, type: DescriptionType.Possessive, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)} attack with {Weapon.Parent.HowSeen(Assailant, colour: false)}.";

	public override Difficulty RecoveryDifficultySuccess => Difficulty.Easy;

	public override Difficulty RecoveryDifficultyFailure => Difficulty.Normal;

	public override ExertionLevel AssociatedExertion => ExertionLevel.Heavy;

	private bool _calculatedStamina = false;
	private double _staminaCost = 0.0;

	public override double StaminaCost
	{
		get
		{
			if (!_calculatedStamina)
			{
				_staminaCost = MoveStaminaCost(CharacterTargets.First(), Assailant, Weapon);
				_calculatedStamina = true;
			}

			return _staminaCost;
		}
	}

	public static double MoveStaminaCost(ICharacter attacker, ICharacter defender, IMeleeWeapon Weapon)
	{
		return Weapon.WeaponType.StaminaPerParry * CombatBase.PowerMoveStaminaMultiplier(defender) *
		       CombatBase.RelativeStrengthDefenseStaminaMultiplier(attacker, defender);
		;
	}

	public override CheckType Check => CheckType.ParryCheck;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		// Defensive moves are only obliged to provide a difficulty for recovery, not undertake any other action
		throw new NotSupportedException("Defense Moves should not call ResolveMove.");
	}

	public virtual int DifficultStageUps => 0;

	public void ResolveDefenseUsed(OpposedOutcome outcome)
	{
		// Do nothing
	}

	public virtual BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.Parry;
}