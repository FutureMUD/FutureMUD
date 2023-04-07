using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Merits.Interfaces
{
    public interface IFixedBloodTypeMerit : ICharacterMerit
    {
        IBloodtype Bloodtype { get; }
    }
}
