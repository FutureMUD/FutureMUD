using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces {
    public interface ITreatment : IGameItemComponent {
        Difficulty GetTreatmentDifficulty(Difficulty baseDifficulty);
        bool IsTreatmentType(TreatmentType type);
        void UseTreatment();
    }
}