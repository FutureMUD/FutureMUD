using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Interfaces
{
    public interface IGrappling : IEffectSubtype, ILDescSuffixEffect
    {
        ICharacter CharacterOwner { get; }
        ICharacter Target { get; }
        IBeingGrappled TargetEffect { get; }
        IEnumerable<ILimb> LimbsUnderControl { get; }
        /// <summary>
        /// Determines the results of a struggle attempt
        /// </summary>
        /// <param name="degree">The OpposedOutcomeDegree of the successful opposed check</param>
        /// <returns>A tuple with the boolean (true = still grappled, false = escaped grapple) and the enumerable representing which limbs were freed.</returns>
        (bool StillGrappled, IEnumerable<ILimb> FreedLimbs) StruggleResult(OpposedOutcomeDegree degree);
        void AddLimb(ILimb limb);
    }
}
