namespace MudSharp.Combat;

public interface IBreathWeaponAttack : IRangedNaturalAttack
{
	int AdditionalTargetLimit { get; }
	int BodypartsHitPerTarget { get; }
	IFireProfile FireProfile { get; }
	double IgniteChance { get; }
}
