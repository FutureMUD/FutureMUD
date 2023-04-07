using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

public class Antibiotic : Effect, IInfectionResistanceEffect
{
	public Antibiotic(IBody owner, double intensitypergrammass) : base(owner)
	{
		IntensityPerGramMass = intensitypergrammass;
		InfectionBonusToIntensityMultiplier =
			owner.Gameworld.GetStaticDouble("AntibioticInfectionBonusToIntensityMultiplier");
	}

	public double InfectionBonusToIntensityMultiplier { get; set; }
	public double IntensityPerGramMass { get; set; }

	protected override string SpecificEffectType => "Antibiotic";

	public double InfectionResistanceBonus => IntensityPerGramMass * InfectionBonusToIntensityMultiplier;

	public bool AppliesToType(InfectionType type)
	{
		switch (type)
		{
			case InfectionType.Simple:
			case InfectionType.Infectious:
			case InfectionType.Necrotic:
			case InfectionType.Gangrene:
				return true;
			default:
				return false;
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Antibiotic resistance bonus of {InfectionResistanceBonus:N2}";
	}
}