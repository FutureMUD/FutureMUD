using MudSharp.Character;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.RPG.Law
{
    public interface IPatrolStrategy
    {
        string Name { get; }
        void HandlePatrolTick(IPatrol patrol);
        IEnumerable<ICharacter> SelectEnforcers(IPatrolRoute patrol, IEnumerable<ICharacter> pool, int numberToPick);
    }
}
