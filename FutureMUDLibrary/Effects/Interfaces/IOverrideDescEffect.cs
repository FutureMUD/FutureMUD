using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces {
    public interface IOverrideDescEffect : IEffectSubtype {
        bool OverrideApplies(IPerceiver voyeur, DescriptionType type);
        string Description(DescriptionType type, bool colour);
    }
}
