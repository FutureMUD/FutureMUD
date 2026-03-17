namespace MudSharp.Combat;

public interface IBuffetingRangedAttack : IRangedNaturalAttack
{
	int MaximumPushDistance { get; }
	double OffensiveAdvantagePerDegree { get; }
	double DefensiveAdvantagePerDegree { get; }
	bool InflictsDamage { get; }
}
