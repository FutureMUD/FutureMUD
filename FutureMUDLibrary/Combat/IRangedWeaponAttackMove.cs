using MudSharp.Body;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat {
    public interface IRangedWeaponAttackMove : ICombatMove {
        IRangedWeapon Weapon { get; }
        IBodypart TargetBodypart { get; }
    }
}