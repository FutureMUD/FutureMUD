using MudSharp.Character;

namespace MudSharp.Effects.Interfaces {
    public interface IRescueEffect : ICombatEffect {
        ICharacter RescueTarget { get; }
    }
}