using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class HealingRateDrug : Effect, IHealingRateEffect
{
	public HealingRateDrug(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	protected override string SpecificEffectType => "HealingRateDrug";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Healing {HealingRateMultiplier:P2} faster at {HealingDifficultyStages:N0} degress less difficult due to drugs.";
	}

	public double HealingRateMultiplier { get; set; }
	public int HealingDifficultyStages { get; set; }
}