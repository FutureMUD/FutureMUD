using MudSharp.Character;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Interfaces {
    public interface ISmokeable : ILightable {
        bool CanSmoke(ICharacter character);
        bool Smoke(ICharacter character, IEmote playerEmote);
    }
}