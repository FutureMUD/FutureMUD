using MudSharp.Character;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface ITearable : IGameItemComponent
    {
        IGameItem Tear(ICharacter actor, IEmote emote);
        bool CanTear(ICharacter actor);
        string WhyCannotTear(ICharacter actor);
    }
}
