using System;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class Analgesic : Effect, IPainReductionEffect
{
	public Analgesic(IPerceivable owner, double analgesicIntensityPerGramMass)
		: base(owner)
	{
		AnalgesicIntensityPerGramMass = analgesicIntensityPerGramMass;
	}

	public double AnalgesicIntensityPerGramMass { get; set; }

	protected override string SpecificEffectType => "Analgesic";

	public double FlatPainReductionAmount => Math.Max(0, (AnalgesicIntensityPerGramMass - 1) * 1.75);

	public double PainReductionMultiplier => Math.Max(0, 1 - 0.02 * AnalgesicIntensityPerGramMass);

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Analgesic reducing pain by {1.0 - PainReductionMultiplier:P2} and taking off a flat {FlatPainReductionAmount:N2}";
	}
}