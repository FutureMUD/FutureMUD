using MudSharp.Body.Traits;

namespace MudSharp.Work.Projects.Impacts
{
    public interface ILabourImpactTraitCaps : ILabourImpact
    {
        double EffectOnTrait(ITraitDefinition trait);
    }
}
