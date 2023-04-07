using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces {
    public enum RestraintType {
        Binding,
        Shackle
    }

    public interface IRestraint : IGameItemComponent {
        RestraintType RestraintType { get; }
        bool CanRestrainCreature(ICharacter actor);
        string WhyCannotRestrainCreature(ICharacter actor);
        Difficulty BreakoutDifficulty { get; }
        Difficulty OverpowerDifficulty { get; }
        IGameItem TargetItem { get; set; }
        IEnumerable<LimbType> Limbs { get; }
        IRestraintEffect Effect { get; set; }
    }
}
