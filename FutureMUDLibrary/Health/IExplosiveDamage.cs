using MudSharp.Construction;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Health
{
    public enum ExplosionDamageExtent
    {
        IndividualPart,
        AdjacentParts,
        EntireLimb,
        EntireFacing,
        EntireBody
    }

    public interface IExplosiveDamage
    {
        /// <summary>
        /// These damages are used to create the damages suffered by those caught in the explosion
        /// </summary>
        IEnumerable<IDamage> ReferenceDamages { get; }

        /// <summary>
        /// The elevation is used to determine the orientation of bodyparts affected by this explosion
        /// </summary>
        double Elevation { get; }

        /// <summary>
        /// The explosion size is used to determine how much of the target is enveloped in the explosion
        /// </summary>
        SizeCategory ExplosionSize { get; }

        /// <summary>
        /// Explosions originating from inside a target do not propogate "down" to its contents
        /// </summary>
        bool ExplodingFromInside { get; }

        IGameItem InternalExplosionSource { get; }

        Proximity MaximumProximity { get; }
    }
}
