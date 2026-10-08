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
		WeatherHazardSettings Hazards { get; }
        PrecipitationLevel Precipitation { get; }
        WindLevel Wind { get; }
        string WeatherDescription { get; }
        string WeatherRoomAddendum { get; }
        bool RequiresRoomFiveSecondTick { get; }
        string RandomFlavourEcho();
        double TemperatureEffect { get; }
        double PrecipitationTemperatureEffect { get; }
        double WindTemperatureEffect { get; }
        string DescribeTransitionTo([CanBeNull] IWeatherEvent oldEvent);
        double LightLevelMultiplier { get; }
        bool ObscuresViewOfSky { get; }
        IEnumerable<TimeOfDay> PermittedTimesOfDay { get; }
        void OnMinuteEvent(IRoom room);
        void OnFiveSecondEvent(IRoom room);
        [CanBeNull] IWeatherEvent CountsAs { get; }
        IWeatherEvent Clone(string name);
    }
}
