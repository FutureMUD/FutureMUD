using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Climate
{
    public delegate void WeatherEchoDelegate(IWeatherController sender, string echo);
    public delegate void WeatherChangedDelegate(IWeatherController sender, IWeatherEvent oldWeather, IWeatherEvent newWeather);
    public delegate void WeatherRoomTickDelegate(Action<ICell> visitor);

    public interface IWeatherController : IEditableItem
    {
        IRegionalClimate RegionalClimate { get; }
        IWeatherEvent CurrentWeatherEvent { get; }
        ISeason CurrentSeason { get; }
        string DescribeCurrentWeather { get; }
        string CurrentWeatherRoomAddendum { get; }
        double CurrentTemperature { get; }
        double CurrentTemperatureFluctuation { get; }
        bool OppositeHemisphere { get; }
        void HandleWeatherTick();
        event WeatherEchoDelegate WeatherEcho;
        event WeatherChangedDelegate WeatherChanged;
        event WeatherRoomTickDelegate WeatherRoomTick;

        void SetWeather(IWeatherEvent newEvent);

        bool WeatherFrozen { get; }
        void FreezeWeather();
        void UnfreezeWeather();

        PrecipitationLevel HighestRecentPrecipitationLevel { get; }
        IClock FeedClock { get; }
        IMudTimeZone FeedClockTimeZone { get; }
        ICelestialObject Celestial
        {
            get;
        }
        GeographicCoordinate GeographyForTimeOfDay
        {
            get;
        }

        int MinuteCounter { get; }
        int ConsecutiveUnchangedPeriods
        {
            get;
        }
    }
}
