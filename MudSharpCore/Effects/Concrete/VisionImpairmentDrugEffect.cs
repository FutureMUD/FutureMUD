using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class VisionImpairmentDrugEffect : Effect, IVisionLimitEffect
{
    public VisionImpairmentDrugEffect(IPerceivable owner) : base(owner)
    {
    }

    protected override string SpecificEffectType => "VisionImpairmentDrugEffect";

    public double VisionMultiplier { get; set; }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Vision impaired to {VisionMultiplier:P2}";
    }
}
