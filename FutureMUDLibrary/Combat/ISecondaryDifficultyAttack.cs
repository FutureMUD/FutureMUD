using MudSharp.RPG.Checks;

namespace MudSharp.Combat {
    public interface ISecondaryDifficultyAttack : IWeaponAttack {
        Difficulty SecondaryDifficulty { get; }
    }
}
