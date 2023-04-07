using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Interfaces {
    public interface ISurgeryFinalisationRequiredEffect : IPertainToBodypartEffect {
        Difficulty Difficulty { get; }
        void BumpDifficulty();
    }
}