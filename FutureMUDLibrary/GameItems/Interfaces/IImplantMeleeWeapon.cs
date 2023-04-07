using MudSharp.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IImplantMeleeWeapon : IImplant, IMeleeWeapon
    {
        bool WeaponIsActive { get; }
    }
}
