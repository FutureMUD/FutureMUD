using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class SkirmishResponseMove : CombatMoveBase
{
	public override string Description => "Skirmishing to avoid Melee Combat";

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
		return gameworld.GetStaticDouble("SkirmishResponseStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		throw new NotImplementedException();
	}
}