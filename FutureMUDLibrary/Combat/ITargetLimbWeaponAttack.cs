using MudSharp.Body;

namespace MudSharp.Combat
{
    public interface ITargetLimbWeaponAttack : IWeaponAttack
    {
        LimbType TargetLimbType { get; }
    }
}
