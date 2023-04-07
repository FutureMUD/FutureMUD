using MudSharp.Character;

namespace MudSharp.Effects.Interfaces {
    public interface IRecentlyRescuedTargetEffect : ICombatEffect {
        ICharacter Rescued { get; }
        ICharacter Rescuer { get; }
    }
}