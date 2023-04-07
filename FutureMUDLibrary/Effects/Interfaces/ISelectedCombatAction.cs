using MudSharp.Character;
using MudSharp.Combat;

namespace MudSharp.Effects.Interfaces
{
    public interface ISelectedCombatAction : IRemoveOnStateChange, ICombatEffect
    {
        ICombatMove GetMove(ICharacter actor);
    }
}