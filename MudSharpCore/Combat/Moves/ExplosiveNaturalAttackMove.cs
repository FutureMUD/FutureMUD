using MudSharp.Body;
using MudSharp.Combat.ScatterStrategies;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Combat.Moves;

public class ExplosiveNaturalAttackMove : NaturalRangedAttackMoveBase
{
    public ExplosiveNaturalAttackMove(ICharacter owner, INaturalAttack attack, IPerceiver target) : base(owner, attack, target)
    {
    }

    private IExplosiveRangedAttack ExplosiveAttack => Attack.GetAttackType<IExplosiveRangedAttack>();

    public override string Description => "Launching an explosive natural attack";
    public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.ExplosiveNaturalAttack;
    protected override CheckType RangedCheck => CheckType.ExplosiveNaturalAttack;

    private ExplosiveDamage BuildExplosionDamage(CheckOutcome attackOutcome, IBodypart? bodypart)
    {
        return new ExplosiveDamage(new[]
        {
            new Damage
            {
                ActorOrigin = Assailant,
                Bodypart = bodypart,
                DamageAmount = Attack.Profile.DamageExpression.Evaluate(Assailant),
                PainAmount = Attack.Profile.PainExpression.Evaluate(Assailant),
                StunAmount = Attack.Profile.StunExpression.Evaluate(Assailant),
                ShockAmount = 0.0,
                DamageType = Attack.Profile.DamageType,
                AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
                PenetrationOutcome = new CheckOutcome { Outcome = attackOutcome.Outcome }
            }
        }, 0.0, ExplosiveAttack.ExplosionSize, ExplosiveAttack.MaximumProximity);
    }

	protected override IEnumerable<IWound> ApplySuccessfulHit(IPerceiver target, CheckOutcome attackOutcome,
		OpposedOutcomeDegree degree, IBodypart bodypart)
	{
		ExplosiveDamage damage = BuildExplosionDamage(attackOutcome, bodypart);

		return SelectExplosionVictims(target, ExplosiveAttack.MaximumProximity)
					 .SelectMany(x => x.Target.PassiveSufferDamage(damage, x.Proximity, Body.Facing.Front))
					 .ToList();
	}

	internal static IEnumerable<(IHaveWounds Target, Proximity Proximity)> SelectExplosionVictims(
		IPerceiver primaryTarget, Proximity maximumProximity)
	{
		return new[] { (Thing: (IPerceivable)primaryTarget, Proximity: Proximity.Intimate) }
			.Concat(primaryTarget.LocalThingsAndProximities())
			.Where(x => x.Thing is IHaveWounds && x.Proximity <= maximumProximity)
			.Select(x => (Target: (IHaveWounds)x.Thing!, x.Proximity))
			.GroupBy(x => (object)x.Target, ReferenceEqualityComparer.Instance)
			.Select(x => (Target: x.First().Target, Proximity: x.Min(y => y.Proximity)));
	}

	internal static IEnumerable<(IHaveWounds Target, Proximity Proximity)> SelectScatterExplosionVictims(
		RangedScatterResult scatter,
		IFuturemud? gameworld,
		Proximity maximumProximity)
	{
		var candidates = ScatterStrategyUtilities
			.GetPerceivablesNearImpact(scatter, gameworld, maximumProximity, false)
			.AsEnumerable();
		if (scatter.Target is not null)
		{
			candidates = candidates.Prepend(scatter.Target);
		}

		return candidates
			.Distinct<IPerceivable>(ReferenceEqualityComparer.Instance)
			.Where(x => x is IHaveWounds)
			.Select(x => (
				Perceivable: x,
				Target: (IHaveWounds)x,
				Proximity: ScatterStrategyUtilities.GetImpactProximity(scatter, x, gameworld)))
			.Where(x => x.Proximity <= maximumProximity)
			.GroupBy(x => x.Perceivable, ReferenceEqualityComparer.Instance)
			.Select(x => (Target: x.First().Target, Proximity: x.Min(y => y.Proximity)));
	}

    protected override void HandleScatterImpact(RangedScatterResult scatter, CheckOutcome attackOutcome)
    {
		var damage = BuildExplosionDamage(attackOutcome, null);
		SelectScatterExplosionVictims(scatter, Assailant.Gameworld, damage.MaximumProximity)
			.SelectMany(x => x.Target.PassiveSufferDamage(
				damage,
				x.Proximity,
				(Body.Facing)RandomUtilities.Random(0, 3)))
			.ProcessPassiveWounds();
    }
}
