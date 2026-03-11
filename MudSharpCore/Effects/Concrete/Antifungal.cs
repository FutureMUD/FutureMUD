#nullable enable

using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

public class Antifungal : Effect, IInfectionResistanceEffect
{
	public Antifungal(IBody owner, double intensityPerGramMass) : base(owner)
	{
		IntensityPerGramMass = intensityPerGramMass;
		InfectionBonusToIntensityMultiplier =
			owner.Gameworld.GetStaticDouble("AntifungalInfectionBonusToIntensityMultiplier");
	}

	public double InfectionBonusToIntensityMultiplier { get; set; }
	public double IntensityPerGramMass { get; set; }

	protected override string SpecificEffectType => "Antifungal";

	public double InfectionResistanceBonus => IntensityPerGramMass * InfectionBonusToIntensityMultiplier;

	public bool AppliesToType(InfectionType type)
	{
		return type == InfectionType.FungalGrowth;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Antifungal resistance bonus of {InfectionResistanceBonus:N2}";
	}
}
