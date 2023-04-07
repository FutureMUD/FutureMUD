using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Interfaces
{
    public interface IGuardExitEffect : IRemoveOnMeleeCombat, ILDescSuffixEffect, IRemoveOnMovementEffect, IRemoveOnStateChange, IAffectedByChangeInGuarding
    {
        ICellExit Exit { get; }
        bool PermitAllies { get; set; }
        bool PermittedToCross(ICharacter ch, ICellExit exit);
        void Exempt(ICharacter ch);
        void Exempt(long id, string description);
        void RemoveExemption(ICharacter ch);
        void RemoveExemption(string playerInput);
        void RemoveExemption(long id);
        IEnumerable<(long Id, string Description)> Exemptions { get; }
    }
}
