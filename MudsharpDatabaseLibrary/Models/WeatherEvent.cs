using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class WeatherEvent
    {
        public WeatherEvent()
        {
            InverseCountsAs = new HashSet<WeatherEvent>();
            WeatherControllers = new HashSet<WeatherController>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string WeatherEventType { get; set; }
        public string WeatherDescription { get; set; }
        public string WeatherRoomAddendum { get; set; }
        public double TemperatureEffect { get; set; }
        public int Precipitation { get; set; }
        public int Wind { get; set; }
        public string AdditionalInfo { get; set; }
        public double PrecipitationTemperatureEffect { get; set; }
        public double WindTemperatureEffect { get; set; }
        public double LightLevelMultiplier { get; set; }
        public bool ObscuresViewOfSky { get; set; }
        public bool PermittedAtNight { get; set; }
        public bool PermittedAtDawn { get; set; }
        public bool PermittedAtMorning { get; set; }
        public bool PermittedAtAfternoon { get; set; }
        public bool PermittedAtDusk { get; set; }
        public long? CountsAsId { get; set; }

        public virtual WeatherEvent CountsAs { get; set; }
        public virtual ICollection<WeatherEvent> InverseCountsAs { get; set; }
        public virtual ICollection<WeatherController> WeatherControllers { get; set; }
    }
}
