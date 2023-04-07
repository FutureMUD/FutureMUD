namespace MudSharp.Effects.Interfaces {
    public interface ISneakMoveEffect : IEffectSubtype {
        void RegisterSawSneaker(IEffect effect);
        bool Subtle { get; }
    }
}