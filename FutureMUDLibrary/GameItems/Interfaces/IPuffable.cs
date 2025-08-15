using MudSharp.Character;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Interfaces {
    public interface IPuffable : IGameItemComponent {
        bool CanPuff(ICharacter character);
        string WhyCannotPuff(ICharacter character);
        bool Puff(ICharacter character, IEmote playerEmote);
    }
}
