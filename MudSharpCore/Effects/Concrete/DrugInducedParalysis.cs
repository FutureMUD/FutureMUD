#nullable enable

using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class DrugInducedParalysis : Effect, IForceParalysisEffect, IScoreAddendumEffect
{
	private double _intensityPerGramMass;

	public DrugInducedParalysis(IBody owner, double intensityPerGramMass) : base(owner)
	{
		IntensityPerGramMass = intensityPerGramMass;
		ParalysisThreshold = owner.Gameworld.GetStaticDouble("DrugParalysisThreshold");
	}

	public double ParalysisThreshold { get; set; }

	public double IntensityPerGramMass
	{
		get => _intensityPerGramMass;
		set
		{
			if (value >= ParalysisThreshold && _intensityPerGramMass < ParalysisThreshold)
			{
				Owner.OutputHandler.Send("Your limbs suddenly stop responding to your will.");
			}
			else if (value < ParalysisThreshold && _intensityPerGramMass >= ParalysisThreshold)
			{
				Owner.OutputHandler.Send("Control slowly returns to your limbs.");
			}
			else if (value >= ParalysisThreshold * 0.5 && _intensityPerGramMass < ParalysisThreshold * 0.5)
			{
				Owner.OutputHandler.Send("A numb heaviness spreads through your limbs.");
			}
			else if (value < ParalysisThreshold * 0.5 && _intensityPerGramMass >= ParalysisThreshold * 0.5)
			{
				Owner.OutputHandler.Send("The numb heaviness in your limbs starts to fade.");
			}

			_intensityPerGramMass = value;
		}
	}

	protected override string SpecificEffectType => "DrugInducedParalysis";

	public bool ShouldParalyse => IntensityPerGramMass >= ParalysisThreshold;

	public bool ShowInScore => IntensityPerGramMass >= ParalysisThreshold * 0.5;
	public bool ShowInHealth => true;
	public string ScoreAddendum => ShouldParalyse
		? "You are chemically paralysed.".Colour(Telnet.BoldRed)
		: "Your limbs feel numb and sluggish.".Colour(Telnet.BoldYellow);

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Drug-induced paralysis of intensity {IntensityPerGramMass:N2} with threshold {ParalysisThreshold:N2}.";
	}
}
