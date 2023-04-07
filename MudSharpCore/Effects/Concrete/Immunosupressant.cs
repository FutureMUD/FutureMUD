using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class Immunosupressant : Effect, IImmuneBonusEffect
{
	public Immunosupressant(IBody owner, double intensity) : base(owner)
	{
		IntensityPerGramMass = intensity;
		ImmuneBonusPerIntensity = owner.Gameworld.GetStaticDouble("ImmunosupressantImmuneBonusPerIntensity");
	}

	public double ImmuneBonusPerIntensity { get; set; }

	public double IntensityPerGramMass { get; set; }

	protected override string SpecificEffectType => "Immunosupressant";

	public double ImmuneBonus => -1 * IntensityPerGramMass * ImmuneBonusPerIntensity;

	public override string Describe(IPerceiver voyeur)
	{
		return $"Immunosupressant drug with bonus of {ImmuneBonus:N2}";
	}
}