using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Health;

namespace MudSharp.GameItems.Interfaces {
    public static class MeleeWeaponExtensions {
        public static AttackHandednessOptions HandednessForWeapon(this IMeleeWeapon weapon, ICharacter ch) {
                switch (ch.Body.WieldedHandCount(weapon.Parent))
                {
                    case 1:
                        return AttackHandednessOptions.OneHandedOnly;
                    case 2:
                        return AttackHandednessOptions.TwoHandedOnly;
                }

                return AttackHandednessOptions.Any;
        }
    }

    public interface IMeleeWeapon : IWieldable, IDamageSource, IUseTrait {
        IWeaponType WeaponType { get; }
        WeaponClassification Classification { get; }
    }
}