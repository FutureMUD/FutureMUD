using MudSharp.Character;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Interfaces {
    public interface ISelectable : IGameItemComponent {
        bool CanSelect(ICharacter character, string argument);
        bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false);
    }
}