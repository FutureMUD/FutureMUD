using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

public enum ItemConditionUseKind
{
	MeleeAttack = 0,
	RangedFire = 1,
	Parry = 2,
	ShieldBlock = 3,
	ArmourAbsorb = 4,
	Measurement = 5,
	BreathingFilter = 6
}

public readonly record struct ItemConditionUseContext(
	ItemConditionUseKind UseKind,
	Outcome Outcome = Outcome.NotTested,
	double Degree = 0.0,
	double Damage = 0.0,
	double Absorbed = 0.0,
	double Passed = 0.0);

public interface IConditionDegradingComponent : IAffectQuality
{
	bool ConditionDegradesOnUse { get; }
	void UseCondition(ItemConditionUseContext context);
}
