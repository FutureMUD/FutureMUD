using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Merits.Interfaces
{
    public interface IMyopiaMerit : ICharacterMerit
    {
        bool CorrectedByGlasses { get; }
    }
}
