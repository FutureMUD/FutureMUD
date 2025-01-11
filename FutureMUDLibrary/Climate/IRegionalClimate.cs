using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework.Revision;

namespace MudSharp.Climate
{
    public interface IRegionalClimate : IEditableItem
    {
        IClimateModel ClimateModel { get; }
        IEnumerable<ISeason> Seasons { get; }
        IReadOnlyDictionary<(ISeason Season, int DailyHour),double> HourlyBaseTemperaturesBySeason { get; }
        CircularRange<ISeason> SeasonRotation { get; }
    }
}
