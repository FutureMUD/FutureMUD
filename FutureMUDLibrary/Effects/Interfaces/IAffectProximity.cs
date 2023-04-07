using MudSharp.Construction;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Interfaces
{
    public interface IAffectProximity : IEffectSubtype
    {
        (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing);
    }
}
