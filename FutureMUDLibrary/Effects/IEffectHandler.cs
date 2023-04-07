using MudSharp.Framework;

namespace MudSharp.Effects {
    public interface IEffectHandler : IHaveEffects {
        IPerceivable Parent { get; }
    }
}