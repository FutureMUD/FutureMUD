using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.GameItems;
using MudSharp.Health;

namespace MudSharp.Effects.Interfaces
{
    public interface IMagicArmour : IEffect, IAbsorbDamage
    {
        IArmourType ArmourType { get; }
        ItemQuality Quality { get; }
        bool AppliesToPart(IBodypart bodypart);
    }
}
