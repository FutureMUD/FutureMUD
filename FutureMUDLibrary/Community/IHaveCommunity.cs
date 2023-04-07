using System.Collections.Generic;

namespace MudSharp.Community {
    public interface IHaveCommunity {
        IEnumerable<IClanMembership> ClanMemberships { get; }
        void AddMembership(IClanMembership membership);
        void RemoveMembership(IClanMembership membership);
    }
}