using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class CounterGrappleMove : CombatMoveBase
{
	public override Difficulty RecoveryDifficultySuccess => Difficulty.Normal;

	public override Difficulty RecoveryDifficultyFailure => Difficulty.Hard;

	public override ExertionLevel AssociatedExertion => ExertionLevel.Heavy;

	public override CheckType Check => CheckType.CounterGrappleCheck;

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
		return gameworld.GetStaticDouble("CounterGrappleMoveStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	public override string Description => "Countering a Grapple Move.";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		throw new ApplicationException("Defensive moves should not be resolved.");
	}
}