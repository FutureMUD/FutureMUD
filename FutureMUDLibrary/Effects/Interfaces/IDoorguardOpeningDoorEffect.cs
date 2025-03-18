using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Interfaces {
    public interface IDoorguardOpeningDoorEffect : IEffectSubtype {
        ICellExit Exit { get; }
    }
}