using MudSharp.Character;
using System.Collections.Generic;

namespace MudSharp.Effects.Interfaces
{
    public interface IGuardCharacterEffect : IAffectedByChangeInGuarding
    {
        IEnumerable<ICharacter> Targets { get; }
        void AddTarget(ICharacter target);
        void RemoveTarget(ICharacter target);
        bool Interdicting { get; set; }
    }
}