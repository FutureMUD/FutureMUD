using JetBrains.Annotations;
using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Climate
{

    public interface IWeatherEvent : IEditableItem, IProgVariable
    {
        PrecipitationLevel Precipitation { get; }
        WindLevel Wind { get; }
        string WeatherDescription { get; }
        string WeatherRoomAddendum { get; }
        string RandomFlavourEcho();
        double TemperatureEffect { get; }
        double PrecipitationTemperatureEffect { get; }
        double WindTemperatureEffect { get; }
        string DescribeTransitionTo([CanBeNull] IWeatherEvent oldEvent);
        double LightLevelMultiplier { get; }
        bool ObscuresViewOfSky { get; }
        IEnumerable<TimeOfDay> PermittedTimesOfDay { get; }
        void OnMinuteEvent(ICell cell);
        void OnFiveSecondEvent(ICell cell);
        [CanBeNull] IWeatherEvent CountsAs { get; }
        IWeatherEvent Clone(string name);
    }
}
