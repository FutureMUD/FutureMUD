using MudSharp.Construction;

namespace MudSharp.Effects.Concrete;

public class AIFleeing : Effect, IEffectSubtype
{
    public List<IRoom> PotentialFleeLocations { get; }

    public AIFleeing(ICharacter owner, IEnumerable<IRoom> fleelocations) : base(owner)
    {
        PotentialFleeLocations = new List<IRoom>(fleelocations);
    }

    protected override string SpecificEffectType => "AIFleeing";

    public override string Describe(IPerceiver voyeur)
    {
        return "An undescribed effect of type AIFleeing.";
    }
}