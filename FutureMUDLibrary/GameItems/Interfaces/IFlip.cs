using MudSharp.Character;
using MudSharp.PerceptionEngine;

#nullable enable annotations

namespace MudSharp.GameItems.Interfaces
{
    public interface IFlip : IGameItemComponent
    {
        bool Flipped { get; set; }
        bool Flip(ICharacter flipper, IEmote? playerEmote = null, bool silent = false);
        bool CanFlip(ICharacter flipper);
        string WhyCannotFlip(ICharacter flipper);
    }
}
