using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Interfaces
{
    public interface IDoorguardOpeningDoorEffect : IEffectSubtype
    {
        IRoomExit Exit { get; }
    }
}