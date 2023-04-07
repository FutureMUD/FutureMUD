using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces {
    public interface ILock : IGameItemComponent {
        bool CanBeInstalled { get; }
        bool IsLocked { get; }
        string LockType { get; }
        int Pattern { get; set; }
        Difficulty ForceDifficulty { get; }
        Difficulty PickDifficulty { get; }
        bool CanUnlock(ICharacter actor, IKey key);
        bool Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote);
        bool CanLock(ICharacter actor, IKey key);
        bool Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote);
        bool SetLocked(bool locked, bool echo);
        void InstallLock(ILockable lockable, IExit exit, ICell installLocation);
        string Inspect(ICharacter actor, string description);
    }
}