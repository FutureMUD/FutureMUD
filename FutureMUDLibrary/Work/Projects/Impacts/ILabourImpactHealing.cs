namespace MudSharp.Work.Projects.Impacts
{
    public interface ILabourImpactHealing : ILabourImpact
    {
        double HealingRateMultiplier { get; }
        double HealingCheckBonus { get; }
        double InfectionChanceMultiplier { get; }
    }
}
