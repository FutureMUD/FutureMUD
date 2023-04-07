using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces
{
    public interface ILodgeConsequence : IGameItemComponent
    {
        Difficulty DifficultyToRemove { get; }
        bool RequiresSurgery { get; }
        IDamage GetDamageOnRemoval(IWound wound, Outcome success);
    }
}
