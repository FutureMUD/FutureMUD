using MudSharp.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Interfaces
{
    public interface ITearable : IGameItemComponent
    {
        IGameItem Tear(ICharacter actor, IEmote emote);
        bool CanTear(ICharacter actor);
        string WhyCannotTear(ICharacter actor);
    }
}
