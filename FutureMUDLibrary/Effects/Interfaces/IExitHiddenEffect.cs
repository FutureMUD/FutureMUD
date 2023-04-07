using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Interfaces {
    public interface IExitHiddenEffect : IEffectSubtype {
        IExit Exit { get; }
        PerceptionTypes HiddenTypes { get; }
    }
}