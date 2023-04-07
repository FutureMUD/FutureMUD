using System.Collections.Generic;

namespace MudSharp.RPG.Merits {
    public interface IHaveMerits {
        IEnumerable<IMerit> Merits { get; }
        bool AddMerit(IMerit merit);
        bool RemoveMerit(IMerit merit);
    }
}