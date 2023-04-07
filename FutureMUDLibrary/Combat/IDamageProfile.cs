using MudSharp.Body.Traits;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat {
    public interface IDamageProfile {
        Difficulty BaseAttackerDifficulty { get; set; }
        Difficulty BaseBlockDifficulty { get; set; }
        Difficulty BaseDodgeDifficulty { get; set; }
        Difficulty BaseParryDifficulty { get; set; }
        double BaseAngleOfIncidence { get; set; }
        ITraitExpression DamageExpression { get; set; }
        ITraitExpression StunExpression { get; set; }
        ITraitExpression PainExpression { get; set; }
        DamageType DamageType { get; set; }
    }
}