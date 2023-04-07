using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Interfaces
{
    public interface IBeingGrappled : IEffectSubtype, ILimbIneffectiveEffect, ILDescSuffixEffect
    {
        IGrappling Grappling { get; }
        Difficulty StruggleDifficulty { get; }
        bool UnderControl { get; }
    }
}
