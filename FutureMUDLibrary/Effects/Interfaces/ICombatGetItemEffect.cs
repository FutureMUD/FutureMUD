using MudSharp.GameItems;

namespace MudSharp.Effects.Interfaces {
    public interface ICombatGetItemEffect : IEffectSubtype {
        IGameItem TargetItem { get; }
    }
}