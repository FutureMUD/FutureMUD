using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class ExplosiveNaturalAttackMove : NaturalRangedAttackMoveBase
{
	public ExplosiveNaturalAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack, target)
	{
	}

	private IExplosiveRangedAttack ExplosiveAttack => Attack.GetAttackType<IExplosiveRangedAttack>();

	public override string Description => "Launching an explosive natural attack";
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.ExplosiveNaturalAttack;
	protected override CheckType RangedCheck => CheckType.ExplosiveNaturalAttack;

	private ExplosiveDamage BuildExplosionDamage(CheckOutcome attackOutcome, IBodypart bodypart)
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
		var damage = BuildExplosionDamage(attackOutcome, bodypart);

		return target.LocalThingsAndProximities()
		             .Where(x => x.Thing is IHaveWounds && x.Proximity <= ExplosiveAttack.MaximumProximity)
		             .SelectMany(x => ((IHaveWounds)x.Thing).PassiveSufferDamage(damage, x.Proximity, Body.Facing.Front))
		             .ToList();
	}

	protected override void HandleScatterImpact(RangedScatterResult scatter, CheckOutcome attackOutcome)
	{
		scatter.Cell.ExplosionEmantingFromPerceivable(BuildExplosionDamage(attackOutcome, null)).ProcessPassiveWounds();
	}
}
