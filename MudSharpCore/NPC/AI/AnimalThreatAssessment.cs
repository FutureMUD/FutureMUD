#nullable enable

namespace MudSharp.NPC.AI;

/// <summary>A deliberately imperfect judgement, not a combat simulation or probability of victory.</summary>
public sealed class AnimalThreatAssessment : IAnimalThreatAssessment
{
	public static AnimalThreatAssessment Instance { get; } = new();
	public AnimalAssessmentResult Assess(AnimalAssessmentInput input, IReadOnlyDictionary<string, double> weights,
		double confidenceBias = 0.0)
	{
		static double Bound(double value, double min, double max) => double.IsFinite(value) ? Math.Clamp(value, min, max) : 0;
		var factors = new Dictionary<string, double>
		{
			["size"] = Bound(input.SizeAdvantage, -3, 3), ["injury"] = Bound(input.VisibleInjury, 0, 1),
			["vulnerability"] = Bound(input.VisibleVulnerability, 0, 1), ["tactic"] = Bound(input.TacticalAdvantage, 0, 1),
			["support"] = Bound(input.SupportAdvantage, -3, 3), ["weapons"] = Bound(input.VisibleArmament, 0, 1),
			["owninjury"] = Bound(input.OwnInjury, 0, 1), ["fatigue"] = Bound(input.OwnFatigue, 0, 1)
		};
		var contributions = factors.ToDictionary(x => x.Key, x =>
			x.Value * (weights.TryGetValue(x.Key, out var weight) ? Bound(weight, -1000, 1000) : 0));
		contributions["baseline"] = 50;
		contributions["confidence"] = Bound(confidenceBias, -100, 100);
		return new AnimalAssessmentResult(Math.Clamp(contributions.Values.Sum(), 0, 100), contributions);
	}
}
