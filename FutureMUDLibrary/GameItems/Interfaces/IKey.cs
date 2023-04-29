using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces {
    public interface IKey : IGameItemComponent {
        string LockType { get; }
        int Pattern { get; set; }
        bool Unlocks(string type, int pattern);
        string Inspect(ICharacter actor, string description);
    }

    public interface IKeyring : IKey
    {

    }
}