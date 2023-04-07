using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Climate
{
    public interface IRegionalClimate : IFrameworkItem
    {
        IClimateModel ClimateModel { get; }
        IEnumerable<ISeason> Seasons { get; }
        IReadOnlyDictionary<(ISeason Season, int DailyHour),double> HourlyBaseTemperaturesBySeason { get; }
        string Show(ICharacter voyeur);
        CircularRange<ISeason> SeasonRotation { get; }
    }
}
