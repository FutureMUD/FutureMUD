using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces {
    public interface ISwitchable : IGameItemComponent {
        IEnumerable<string> SwitchSettings { get; }
        bool CanSwitch(ICharacter actor, string setting);
        string WhyCannotSwitch(ICharacter actor, string setting);
        bool Switch(ICharacter actor, string setting);
    }
}