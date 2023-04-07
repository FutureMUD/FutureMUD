using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Interfaces {
    public interface IReplantedBodypartsEffect : IPertainToBodypartEffect {
        Difficulty ResistRejectionDifficulty { get; }
    }
}