using MudSharp.Body;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat {
    public interface IWeaponAttackMove : ICombatMove {
        IWeaponAttack Attack { get; }
        IBodypart TargetBodypart { get; }
        int Reach { get; }
        IMeleeWeapon Weapon { get; }
        double BloodSprayMultiplier { get; }
    }
}