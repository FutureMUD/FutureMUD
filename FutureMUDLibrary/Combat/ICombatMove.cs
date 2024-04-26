using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat {
    public class CombatMoveResult {
        public static CombatMoveResult Irrelevant { get; } = new()
        {
            MoveWasSuccessful = false,
            RecoveryDifficulty = Difficulty.Automatic,
            AttackerOutcome = Outcome.NotTested,
            DefenderOutcome = Outcome.NotTested
        };

        public bool MoveWasSuccessful { get; init; }
        public Difficulty RecoveryDifficulty { get; init; } = Difficulty.Automatic;
        public Outcome AttackerOutcome { get; init; } = Outcome.NotTested;
        public Outcome DefenderOutcome { get; init; } = Outcome.NotTested;

        public IEnumerable<IWound> WoundsCaused { get; init; } = Enumerable.Empty<IWound>();
        public IEnumerable<IWound> SelfWoundsCaused { get; set; } = Enumerable.Empty<IWound>();
    }

    public interface ICombatMove : IHaveFuturemud {
        string Description { get; }
        double StaminaCost { get; }
        Difficulty CheckDifficulty { get; }
        CheckType Check { get; }
        ExertionLevel AssociatedExertion { get; }
        ICharacter Assailant { get; }
        Difficulty RecoveryDifficultySuccess { get; }
        Difficulty RecoveryDifficultyFailure { get; }
        IEnumerable<ICharacter> CharacterTargets { get; }
        double BaseDelay { get; }
        CombatMoveResult ResolveMove(ICombatMove defenderMove);
        bool UsesStaminaWithResult(CombatMoveResult result);
    }
}