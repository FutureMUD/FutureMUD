using MudSharp.Body;
using MudSharp.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Merits.Interfaces
{
    public interface IAdditionalBodypartsMerit : ICharacterMerit
    {
        IEnumerable<IBodypart> AddedBodyparts(ICharacter character);
        IEnumerable<IBodypart> RemovedBodyparts(ICharacter character);
    }
}
