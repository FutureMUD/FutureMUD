using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class DodgeMove : CombatMoveBase, IDefenseMove
{
	public override string Description
		=>
			$"Dodging {CharacterTargets.First().HowSeen(Assailant, type: DescriptionType.Possessive, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)} attack.";

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
		return gameworld.GetStaticDouble("DodgeMoveStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	#region Overrides of CombatMoveBase

	public override CheckType Check => CheckType.DodgeCheck;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		// Defensive moves are only obliged to provide a difficulty for recovery, not undertake any other action
		throw new NotSupportedException("Defense Moves should not call ResolveMove.");
	}

	public override bool UsesStaminaWithResult(CombatMoveResult result)
	{
		return result.MoveWasSuccessful || result.AttackerOutcome.IsPass();
	}

	public virtual int DifficultStageUps => 0;

	public virtual void ResolveDefenseUsed(OpposedOutcome outcome)
	{
		// Do nothing
	}

	#endregion

	public virtual BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.Dodge;
}