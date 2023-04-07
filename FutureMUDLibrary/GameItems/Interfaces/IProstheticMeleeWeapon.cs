using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IProstheticMeleeWeapon : IMeleeWeapon, IProsthetic
    {
        bool WeaponIsActive { get; }
    }
}
