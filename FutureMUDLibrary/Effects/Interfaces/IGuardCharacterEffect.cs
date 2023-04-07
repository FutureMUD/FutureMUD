using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.Effects.Interfaces {
    public interface IGuardCharacterEffect : IAffectedByChangeInGuarding
    {
        IEnumerable<ICharacter> Targets { get; }
        void AddTarget(ICharacter target);
        void RemoveTarget(ICharacter target);
        bool Interdicting { get; set; }
    }
}