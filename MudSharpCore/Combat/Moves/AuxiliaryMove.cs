using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

internal class AuxiliaryMove: CombatMoveBase
{
	private readonly ICharacter _target;
	private readonly IAuxiliaryCombatAction _action;

	public AuxiliaryMove(ICharacter assailant, ICharacter target, IAuxiliaryCombatAction action)
	{
		Assailant = assailant;
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
		if (defenderMove == null)
		{
			defenderMove = new HelplessDefenseMove { Assailant = _target };
		}

		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var attackRoll = Gameworld.GetCheck(Check)
		                          .Check(Assailant, CheckDifficulty, _action.CheckTrait, 
			                          defenderMove.Assailant,
			                          Assailant.OffensiveAdvantage);
		Assailant.OffensiveAdvantage = 0;
		var emote = attackRoll.IsPass() ? 
			Gameworld.CombatMessageManager.GetMessageFor(Assailant, defenderMove.Assailant, _action, attackRoll.Outcome) :
			Gameworld.CombatMessageManager.GetFailMessageFor(Assailant, defenderMove.Assailant, _action, attackRoll.Outcome);
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(emote, Assailant, Assailant, _target)));
		foreach (var effect in _action.AuxiliaryEffects)
		{
			effect.ApplyEffect(Assailant, _target, attackRoll);
		}

		return CombatMoveResult.Irrelevant;
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

	/// <inheritdoc />
	public override CheckType Check => CheckType.AuxiliaryMoveCheck;

	/// <inheritdoc />
	public override Difficulty CheckDifficulty => _action.MoveDifficulty;

	#endregion
}