using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces {
    public interface ISharpen : IGameItemComponent {
        bool CanSharpen(ICharacter actor, IGameItem otherItem);
        string WhyCannotSharpen(ICharacter actor, IGameItem otherItem);
        bool Sharpen(ICharacter actor, IGameItem otherItem);
    }
}
