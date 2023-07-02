using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

internal class AuxiliaryMove: CombatMoveBase
{
	private readonly ICharacter _assailant;
	private readonly ICharacter _target;
	private readonly IAuxiliaryCombatAction _action;

	public AuxiliaryMove(ICharacter assailant, ICharacter target, IAuxiliaryCombatAction action)
	{
		_assailant = assailant;
		_target = target;
		_action = action;
	}

	private bool _calculatedStamina = false;
	private double _staminaCost = 0.0;

	/// <inheritdoc />
	public override string Description => $"Using the {_action.Name.ColourName()} auxiliary";

	public override double StaminaCost
	{
		get
		{
			if (!_calculatedStamina)
			{
				_staminaCost = MoveStaminaCost(Assailant, _action);
				_calculatedStamina = true;
			}

			return _staminaCost;
		}
	}


	public static double MoveStaminaCost(ICharacter assailant, IAuxiliaryCombatAction move)
	{
		return move.StaminaCost * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	/// <inheritdoc />
	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		
		throw new NotImplementedException();
	}

	#region Overrides of CombatMoveBase

	/// <inheritdoc />
	public override double BaseDelay => _action.BaseDelay;

	/// <inheritdoc />
	public override ExertionLevel AssociatedExertion => _action.ExertionLevel;

	/// <inheritdoc />
	public override Difficulty RecoveryDifficultyFailure => _action.RecoveryDifficultyFailure;

	/// <inheritdoc />
	public override Difficulty RecoveryDifficultySuccess => _action.RecoveryDifficultySuccess;

	/// <inheritdoc />
	public override IPerceiver PrimaryTarget => _target;
	

	#endregion
}