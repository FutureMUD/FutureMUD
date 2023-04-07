using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces {
    public interface ILDescSuffixEffect : IEffectSubtype {
        string SuffixFor(IPerceiver voyeur);
        bool SuffixApplies();
    }
}
