namespace MudSharp.Effects.Interfaces {
    public interface INoGetEffect : IEffectSubtype {
        bool CombatRelated { get; }
    }
}