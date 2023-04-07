using MudSharp.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Interfaces
{
    /// <summary>
    /// IEndOnCombatMove effects potentially end when the owner takes (or doesn't take) an action in combat. 
    /// </summary>
    public interface IEndOnCombatMove : IEffectSubtype
    {
        bool CausesToEnd(ICombatMove move);
    }
}
