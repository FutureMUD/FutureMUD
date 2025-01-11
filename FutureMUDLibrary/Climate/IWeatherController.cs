using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework.Revision;

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
        void HandleWeatherTick();
        event WeatherEchoDelegate WeatherEcho;
        event WeatherChangedDelegate WeatherChanged;
        event WeatherRoomTickDelegate WeatherRoomTick;

        void SetWeather(IWeatherEvent newEvent);

        PrecipitationLevel HighestRecentPrecipitationLevel { get; }
    }
}
