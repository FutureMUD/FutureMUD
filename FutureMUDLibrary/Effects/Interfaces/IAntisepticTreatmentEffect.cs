using MudSharp.Body;

namespace MudSharp.Effects.Interfaces {
    public interface IAntisepticTreatmentEffect : IEffectSubtype {
        IBodypart Bodypart { get; }
    }
}