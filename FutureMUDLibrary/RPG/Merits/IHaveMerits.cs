using System.Collections.Generic;

namespace MudSharp.RPG.Merits
{
    public interface IHaveMerits
    {
        IEnumerable<IMerit> Merits { get; }
        /// <summary>Explicit assignments only; intrinsic body/race contributions must not be copied or saved here.</summary>
        IEnumerable<IMerit> ExplicitMerits => Merits;
        bool AddMerit(IMerit merit);
        bool RemoveMerit(IMerit merit);
    }
}
