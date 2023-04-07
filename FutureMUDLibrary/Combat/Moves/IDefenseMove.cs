using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Moves
{
    public interface IDefenseMove : ICombatMove
    {
        int DifficultStageUps { get; }
        void ResolveDefenseUsed(RPG.Checks.OpposedOutcome outcome);
    }
}
