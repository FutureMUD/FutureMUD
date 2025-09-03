using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Interfaces {
    public interface ISneakMoveEffect : IEffectSubtype {
        void RegisterSawSneaker(IEffect effect);
        bool Subtle { get; }
        Outcome StealthOutcome { get; }
    }
}