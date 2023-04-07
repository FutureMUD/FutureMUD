using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Health;

namespace MudSharp.Effects.Interfaces
{
    public interface IEffectRemoveOnDamage : IEffectSubtype {
        bool RemovesWith(IDamage damage);
    }
}
