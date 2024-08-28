using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Combat
{
    public interface ICombatStrategy {
        CombatStrategyMode Mode { get; }
        ICombatMove ResponseToMove(ICombatMove move, IPerceiver defender, IPerceiver assailant);
        ICombatMove ChooseMove(IPerceiver combatant);
        bool WillAttack(ICharacter ch, ICharacter tch);
        string WhyWontAttack(ICharacter ch, ICharacter tch);
        string WhyWontAttack(ICharacter ch);

    }
}
