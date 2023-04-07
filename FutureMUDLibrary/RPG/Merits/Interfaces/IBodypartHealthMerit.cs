using MudSharp.Body;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Merits.Interfaces
{
    public interface IBodypartHealthMerit : ICharacterMerit
    {
        bool AppliesToBodypart(IBodypart bodypart);
        double MultiplierForBodypart(IBodypart bodypart);
    }
}
