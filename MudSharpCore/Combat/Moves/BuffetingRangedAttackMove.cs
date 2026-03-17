using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class BuffetingRangedAttackMove : NaturalRangedAttackMoveBase
{
	public BuffetingRangedAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack, target)
	{
	}

	private IBuffetingRangedAttack BuffetingAttack => Attack.GetAttackType<IBuffetingRangedAttack>();

	public override string Description => "Using a buffeting ranged attack";
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.BuffetingNaturalAttack;
	protected override CheckType RangedCheck => CheckType.BuffetingNaturalAttack;

	protected override IEnumerable<IWound> ApplySuccessfulHit(IPerceiver target, CheckOutcome attackOutcome,
		OpposedOutcomeDegree degree, IBodypart bodypart)
	{
		if (target is not ICharacter tch)
		{
			return Enumerable.Empty<IWound>();
		}

		tch.OffensiveAdvantage += BuffetingAttack.OffensiveAdvantagePerDegree * attackOutcome.SuccessDegrees();
		tch.DefensiveAdvantage += BuffetingAttack.DefensiveAdvantagePerDegree * attackOutcome.SuccessDegrees();

		var awayExit = tch.Location.ExitsFor(tch)
		                  .FirstOrDefault(x => x.Destination != Assailant.Location);
		if (awayExit is not null && BuffetingAttack.MaximumPushDistance > 0)
		{
			tch.MoveTo(awayExit.Destination, tch.RoomLayer, awayExit);
		}

		if (!BuffetingAttack.InflictsDamage)
		{
			return Enumerable.Empty<IWound>();
		}

		return base.ApplySuccessfulHit(target, attackOutcome, degree, bodypart);
	}
}
