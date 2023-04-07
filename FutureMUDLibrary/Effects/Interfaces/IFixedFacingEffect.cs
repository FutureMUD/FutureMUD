using MudSharp.Body;
using MudSharp.Combat;

namespace MudSharp.Effects.Interfaces {
    public interface IFixedFacingEffect : ICombatEffect {
        Facing Facing { get; }
        bool AppliesTo(ICombatant combatant);
    }
}