using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.RPG.Merits.Interfaces {
    public interface IInfectionResistanceMerit : ICharacterMerit {
        Difficulty GetNewInfectionDifficulty(Difficulty original, InfectionType type);
    }
}