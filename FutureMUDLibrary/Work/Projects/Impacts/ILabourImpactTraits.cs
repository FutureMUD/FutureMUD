using MudSharp.Body.Traits;

namespace MudSharp.Work.Projects.Impacts
{
    public interface ILabourImpactTraits : ILabourImpact
    {
        double EffectOnTrait(ITrait trait, TraitBonusContext context);
    }
}
