using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Merits.Interfaces
{
    public interface ITemperatureRangeChangingMerit : ICharacterMerit
    {
        double FloorEffect { get; }
        double CeilingEffect { get; }
    }
}
