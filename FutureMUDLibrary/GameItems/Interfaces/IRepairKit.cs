using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;

namespace MudSharp.GameItems.Interfaces
{
    public interface IRepairKit : IGameItemComponent
    {
        (bool Success, string Reason) CanRepair(IWound wound);
        ITraitDefinition CheckTrait { get; }
        double CheckBonus { get; }
        void Repair(IWound wound, ICharacter repairer);
        void Repair(IEnumerable<IWound> wounds, ICharacter repairer);
        IEnumerable<string> Echoes { get; }
    }
}
