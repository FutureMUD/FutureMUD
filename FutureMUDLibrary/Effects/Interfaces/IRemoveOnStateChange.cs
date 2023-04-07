using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;

namespace MudSharp.Effects.Interfaces
{
    public interface IRemoveOnStateChange : IEffectSubtype {
        bool ShouldRemove(CharacterState newState);
    }
}
