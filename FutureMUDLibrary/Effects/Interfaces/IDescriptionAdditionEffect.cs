using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces {
    public interface IDescriptionAdditionEffect : IEffectSubtype {
        string GetAdditionalText(IPerceiver voyeur, bool colour);
        bool PlayerSet { get; }
        bool DescriptionAdditionApplies(IPerceiver voyeur) => Applies(voyeur) && !string.IsNullOrEmpty(GetAdditionalText(voyeur, false));
    }
}