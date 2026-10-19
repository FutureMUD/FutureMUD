using MudSharp.Body;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

internal class AuxiliaryMove : CombatMoveBase
{
    private readonly ICharacter _target;
    private readonly IAuxiliaryCombatAction _action;
    private readonly IPhysicalImpactProfile _impactProfile;
    private readonly Func<bool> _impactParticipants;
    internal IBody ImpactStaminaBody { get; }
    public override bool UsesStaminaWithResult(CombatMoveResult result) =>
        (!IsPhysicalImpact || !ReferenceEquals(result, CombatMoveResult.Irrelevant)) && base.UsesStaminaWithResult(result);

    public AuxiliaryMove(ICharacter assailant, ICharacter target, IAuxiliaryCombatAction action)
    {
        Assailant = assailant;
        _target = target;
        _action = action;
        _impactProfile = (action as IPhysicalImpactAuxiliaryAction)?.PhysicalImpact;
        if (_impactProfile is not null)
        {
            ImpactStaminaBody = assailant.Body;
            var targetBody = target.Body; var actorFocus = ImpactStaminaBody.Actor; var targetFocus = targetBody.Actor;
            var actorInstance = assailant.InstanceId; var targetInstance = target.InstanceId;
            var room = assailant.Location; var targetRoom = target.Location;
            var layer = assailant.RoomLayer; var targetLayer = target.RoomLayer;
            var combat = assailant.Combat; var targetCombat = target.Combat;
            _impactParticipants = () => ReferenceEquals(assailant.Body, ImpactStaminaBody) && ReferenceEquals(target.Body, targetBody) &&
                ReferenceEquals(ImpactStaminaBody.Actor, actorFocus) && ReferenceEquals(targetBody.Actor, targetFocus) &&
                assailant.InstanceId == actorInstance && target.InstanceId == targetInstance &&
                ReferenceEquals(assailant.Location, room) && ReferenceEquals(target.Location, targetRoom) &&
                assailant.RoomLayer == layer && target.RoomLayer == targetLayer &&
                ReferenceEquals(assailant.Combat, combat) && ReferenceEquals(target.Combat, targetCombat);
            _staminaCost = MoveStaminaCost(assailant, action);
            _calculatedStamina = true;
        }
    }

    internal bool IsPhysicalImpact => _impactProfile is not null;

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
        var cost = move is IPhysicalImpactAuxiliaryAction { PhysicalImpact: not null }
            ? 2.0 + Math.Clamp(100.0 - assailant.TraitValue(move.CheckTrait), 0.0, 100.0) / 22.0
            : move.StaminaCost;
        return cost * CombatBase.GraceMoveStaminaMultiplier(assailant);
    }

	internal static bool CanReachTarget(ICharacter assailant, ICharacter target)
	{
		return assailant.Combat is not null && target.Combat == assailant.Combat &&
		       assailant.ColocatedWith(target) && assailant.RoomLayer == target.RoomLayer &&
		       ((assailant.CombatTarget == target && assailant.MeleeRange) ||
		        (target.CombatTarget == assailant && target.MeleeRange));
	}

    /// <inheritdoc />
    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
		using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
		if (_impactProfile is not null)
		{
			if (!_impactParticipants()) return CombatMoveResult.Irrelevant;
			if (_action is not IPhysicalImpactAuxiliaryAction current || !ReferenceEquals(current.PhysicalImpact, _impactProfile)) return CombatMoveResult.Irrelevant;
			return PhysicalImpactResolver.Resolve(this, Assailant, _target, _action, _impactProfile, defenderMove);
		}
		if (!CanReachTarget(Assailant, _target)) return CombatMoveResult.Irrelevant;
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
		if (!CanReachTarget(Assailant, _target)) return CombatMoveResult.Irrelevant;

        WorsenCombatPosition(defenderMove.Assailant, Assailant);
		if (!CanContinueCommand()) return CompletedResult();
        CheckOutcome attackRoll = CombatCheck.For(Gameworld, Check)
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
			if (!CanReachTarget(Assailant, _target)) break;
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
