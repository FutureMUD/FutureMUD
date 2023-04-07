using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Interfaces {
    public interface ILightable : IGameItemComponent {
        bool Lit { get; set; }
        bool CanLight(ICharacter lightee, IPerceivable ignitionSource);
        string WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource);
        bool Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote);
        bool CanExtinguish(ICharacter lightee);
        string WhyCannotExtinguish(ICharacter lightee);
        bool Extinguish(ICharacter lightee, IEmote playerEmote);
    }
}