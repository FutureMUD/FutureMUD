using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Health;
using System.Linq;

namespace MudSharp.GameItems.Interfaces
{
    public static class MeleeWeaponExtensions
    {
        public static AttackHandednessOptions HandednessForWeapon(this IMeleeWeapon weapon, ICharacter ch)
        {
            switch (ch.Body.WieldedHandCount(weapon.Parent))
            {
                case 1:
					if (ch.Body.WieldedItems.Any(x => x != weapon.Parent &&
					                                      !x.IsItemType<IShield>() &&
					                                      x.IsItemType<IMeleeWeapon>()))
					{
						return AttackHandednessOptions.DualWieldOnly;
					}

                    return AttackHandednessOptions.OneHandedOnly;
                case 2:
                    return AttackHandednessOptions.TwoHandedOnly;
            }

            return AttackHandednessOptions.Any;
        }
    }

    public interface IMeleeWeapon : IWieldable, IUseTrait
    {
        IWeaponType WeaponType { get; }
        WeaponClassification Classification { get; }
    }
}
