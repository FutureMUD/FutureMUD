using MudSharp.Character;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Interfaces {
    /// <summary>
    ///     An IInsertable component is a component that allows GameItems to be inserted into it
    /// </summary>
    public interface IInsertable : IGameItemComponent {
        bool CanInsert(ICharacter actor, IGameItem item);
        bool Insert(ICharacter actor, IGameItem item, IEmote playerEmote, bool silent = false);
    }
}