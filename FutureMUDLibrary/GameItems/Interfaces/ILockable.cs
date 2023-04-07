using MudSharp.Character;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.GameItems.Interfaces {
    public interface ILockable : IGameItemComponent {
        IEnumerable<ILock> Locks { get; }
        bool InstallLock(ILock theLock, ICharacter actor = null);
        bool RemoveLock(ILock theLock);
        bool IsLocked => Locks.Any(x => x.IsLocked);
    }
}