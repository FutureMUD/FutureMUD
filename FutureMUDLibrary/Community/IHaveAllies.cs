using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.Community {
    public interface IHaveAllies {
        bool IsAlly(ICharacter person);
        bool IsTrustedAlly(ICharacter person);
        void SetAlly(ICharacter person);
        void SetAlly(long ID);
        void RemoveAlly(ICharacter person);
        void RemoveAlly(long ID);
        void SetTrusted(long ID, bool trusted);
        IEnumerable<long> AllyIDs { get; }
        IEnumerable<long> TrustedAllyIDs { get; }
    }
}
