using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class EnvenomingClinchAttack : ClinchNaturalAttackMove
{
    public EnvenomingClinchAttack(ICharacter assailant, ICharacter target, INaturalAttack attack, IMeleeWeapon weapon) :
        base(assailant, target, attack, weapon)
    {
        Target = target;
    }

    public ICharacter Target { get; }

    public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.EnvenomingAttackClinch;

    public override string Description => $"An envenoming clinch attack";

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
		using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
        CombatMoveResult result = base.ResolveMove(defenderMove);
		if (!CanContinueCommand()) return result;
        IEnvenomingAttack envenoming = (IEnvenomingAttack)Attack;
        if (result.WoundsCaused.Any(x => x.Parent == Target && x.Severity >= envenoming.MinimumWoundSeverity))
        {
            double multiplier = result.AttackerOutcome switch
            {
                Outcome.MajorPass => 1.0,
                Outcome.Pass => 0.8,
                Outcome.MinorPass => 0.5,
                Outcome.MinorFail => 0.3,
                Outcome.Fail => 0.1,
                Outcome.MajorFail => 0.0,
                _ => 1.0
            };
			var quantity = envenoming.MaximumQuantity * multiplier;
			if (double.IsFinite(quantity) && quantity > 0 && envenoming.Liquid is not null)
			{
				Target.Body.HealthStrategy.InjectedLiquid(Target,
					new Form.Material.LiquidMixture(envenoming.Liquid, quantity, Gameworld));
				result.EnvenomDelivered = true;
			}
        }

        return result;
    }
}
