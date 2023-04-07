using MudSharp.Form.Material;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat
{
    public interface IEnvenomingAttack : IWeaponAttack
    {
        ILiquid Liquid { get; set; }
        double MaximumQuantity { get; set; }
        WoundSeverity MinimumWoundSeverity { get; set; }
    }
}
