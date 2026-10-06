using MudSharp.Body;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

internal class AuxiliaryMove : CombatMoveBase
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
		using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
		defenderMove = MagicDefenseMove.Revalidate(defenderMove, this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
		if (!VehicleCombatService.Instance.CanCrossVehicleBoundary(Assailant, _target, false, false,
			    out var boundaryReason))
		{
			Assailant.OutputHandler.Send(boundaryReason);
			return CombatMoveResult.Irrelevant;
		}
        if (defenderMove == null)
        {
            defenderMove = new HelplessDefenseMove { Assailant = _target };
        }

        WorsenCombatPosition(defenderMove.Assailant, Assailant);
		if (!CanContinueCommand()) return CompletedResult();
        CheckOutcome attackRoll = Gameworld.GetCheck(Check)
                                  .Check(Assailant, CheckDifficulty, _action.CheckTrait,
                                      defenderMove.Assailant,
                                      Assailant.OffensiveAdvantage);
		if (!CanContinueCommand()) return CompletedResult();
        Assailant.OffensiveAdvantage = 0;
        string emote = attackRoll.IsPass() ?
            Gameworld.CombatMessageManager.GetMessageFor(Assailant, defenderMove.Assailant, _action, attackRoll.Outcome) :
            Gameworld.CombatMessageManager.GetFailMessageFor(Assailant, defenderMove.Assailant, _action, attackRoll.Outcome);
        Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(emote, Assailant, Assailant, _target)));
		if (!CanContinueCommand()) return CompletedResult();
        if (defenderMove is MagicDefenseMove magicalDefense && magicalDefense.TryDefend(this, attackRoll, out var magicResult)) return magicResult;

        foreach (IAuxiliaryEffect effect in _action.AuxiliaryEffects)
        {
			if (!CanContinueCommand()) break;
            effect.ApplyEffect(Assailant, _target, attackRoll);
        }

        return CompletedResult();
    }

	private CombatMoveResult CompletedResult() => MudSharp.NPC.AI.CommandExecutionScope.HasCommitted
		? new CombatMoveResult { RecoveryDifficulty = RecoveryDifficultyFailure }
		: CombatMoveResult.Irrelevant;

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
