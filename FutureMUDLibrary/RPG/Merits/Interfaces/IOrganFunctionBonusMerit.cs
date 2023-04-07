using MudSharp.Body;
using MudSharp.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Merits.Interfaces
{
    public interface IOrganFunctionBonusMerit : ICharacterMerit
    {
        IEnumerable<(IOrganProto Organ, double Bonus)> OrganFunctionBonuses(IBody body);
    }
}
