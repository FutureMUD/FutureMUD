using JetBrains.Annotations;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Interfaces
{
    public interface IRangedObstructionEffect : IEffectSubtype
    {
        /// <summary>
        /// Whether or not this obstruction effect applies for a particular target/attack origin combination
        /// </summary>
        /// <param name="target">The target of the attack</param>
        /// <param name="intercessor">The intercessor who is getting in between the attack</param>
        /// <param name="attackOrigin">The origin of the attack. This can be null if it does not have a perceivable option.</param>
        /// <returns>True if the intercessor obstructs the target from the attack</returns>
        bool IsObstructedFrom(IPerceivable target, IPerceivable intercessor, [CanBeNull]IPerceivable attackOrigin);
        IPerceivable Obstruction { get; }
    }
}
