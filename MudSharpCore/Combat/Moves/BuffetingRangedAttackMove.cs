using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class BuffetingRangedAttackMove : NaturalRangedAttackMoveBase
{
    public BuffetingRangedAttackMove(ICharacter owner, INaturalAttack attack, IPerceiver target) : base(owner, attack, target)
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

		ICell previousCell = Assailant.Location;
		for (int i = 0; i < BuffetingAttack.MaximumPushDistance; i++)
		{
			ICell currentCell = tch.Location;
			ICellExit awayExit = currentCell.ExitsFor(tch)
				.FirstOrDefault(x => x.Destination != previousCell);
			if (awayExit is null)
			{
				break;
			}

			tch.MoveTo(awayExit.Destination, tch.RoomLayer, awayExit);
			if (tch.Location != awayExit.Destination)
			{
				break;
			}

			previousCell = currentCell;
		}

        if (!BuffetingAttack.InflictsDamage)
        {
            return Enumerable.Empty<IWound>();
        }

        return base.ApplySuccessfulHit(target, attackOutcome, degree, bodypart);
    }
}
