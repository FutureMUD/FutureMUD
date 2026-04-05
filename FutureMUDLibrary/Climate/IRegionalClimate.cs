using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Climate
{
    public interface IRegionalClimate : IEditableItem
    {
        string Description { get; }
        IClimateModel ClimateModel { get; }
        double TemperatureFluctuationStandardDeviation { get; }
        TimeSpan TemperatureFluctuationPeriod { get; }
        IEnumerable<ISeason> Seasons { get; }
        IReadOnlyDictionary<(ISeason Season, int DailyHour), double> HourlyBaseTemperaturesBySeason { get; }
        CircularRange<ISeason> SeasonRotation { get; }
        IRegionalClimate Clone(string name);
    }
}
