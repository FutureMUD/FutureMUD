using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class WardDefenseMove : CombatMoveBase, IDefenseMove
{
	public override string Description
		=>
			$"Warding against {CharacterTargets.First().HowSeen(Assailant, type: DescriptionType.Possessive, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)} attack.";

	public override Difficulty RecoveryDifficultySuccess => Difficulty.Normal;

	public override Difficulty RecoveryDifficultyFailure => Difficulty.Hard;

	public override ExertionLevel AssociatedExertion => ExertionLevel.Heavy;

	private bool _calculatedStamina = false;
	private double _staminaCost = 0.0;

	public override double StaminaCost
	{
		get
		{
			if (!_calculatedStamina)
			{
				_staminaCost = MoveStaminaCost(Assailant);
				_calculatedStamina = true;
			}

			return _staminaCost;
		}
	}

	public static double BaseStaminaCost(IFuturemud gameworld)
	{
		return gameworld.GetStaticDouble("WardMoveStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	public IMeleeWeapon WardWeapon { get; set; }

	public int Reach => WardWeapon?.WeaponType.Reach ?? 0; // TODO - natural reach

	public Difficulty GetWarderDifficulty(IWeaponAttackMove move)
	{
		return Difficulty.Normal.StageDown(Reach - move.Reach);
	}

	public Difficulty GetWardeeDifficulty(IWeaponAttackMove move)
	{
		return Difficulty.Normal.StageUp((Reach - move.Reach) / 2);
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		throw new NotSupportedException("Defense Moves should not call ResolveMove.");
	}

	public int DifficultStageUps => 0;

	public void ResolveDefenseUsed(OpposedOutcome outcome)
	{
		// Do nothing
	}
}