#nullable enable

using MudSharp.Body.Traits;

namespace MudSharp.Combat;

/// <summary>A single-target attack delivered from an unseen opening or rear facing.</summary>
public interface IBackstabAttack : ISecondaryDifficultyAttack
{
	long? ProficiencyTraitId { get; }
	ITraitDefinition? ProficiencyTrait { get; }
	string CommandVerb { get; }
	double ReelSecondsPerDegree { get; }
	double AttackerOffensiveAdvantagePerDegree { get; }
	double AttackerDefensiveAdvantagePerDegree { get; }
	double TargetOffensiveDisadvantagePerDegree { get; }
	double TargetDefensiveDisadvantagePerDegree { get; }
	double PenetrationBonus { get; }
	double ArmourBypassChance { get; }
}
