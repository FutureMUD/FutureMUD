using MudSharp.Body;

namespace MudSharp.Effects.Interfaces {
    public enum LimbIneffectiveReason {
        Pain,
        Damage,
        Severing,
        Grappling,
        Restrained,
        Boneless,
        SpinalDamage
    }

    public interface ILimbIneffectiveEffect : IEffectSubtype {
        bool AppliesToLimb(ILimb limb);
        LimbIneffectiveReason Reason { get; }
    }
}