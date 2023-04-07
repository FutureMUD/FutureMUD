using MudSharp.Health;

namespace MudSharp.Effects.Interfaces {
    public interface IInfectionResistanceEffect : IEffectSubtype {
        double InfectionResistanceBonus { get; }
        bool AppliesToType(InfectionType type);
    }
}