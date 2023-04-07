using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Clock
    {
        public Clock()
        {
            EconomicZones = new HashSet<EconomicZone>();
            Timezones = new HashSet<Timezone>();
            WeatherControllers = new HashSet<WeatherController>();
        }

        public long Id { get; set; }
        public string Definition { get; set; }
        public int Seconds { get; set; }
        public int Minutes { get; set; }
        public int Hours { get; set; }
        public long PrimaryTimezoneId { get; set; }

        public virtual ICollection<EconomicZone> EconomicZones { get; set; }
        public virtual ICollection<Timezone> Timezones { get; set; }
        public virtual ICollection<WeatherController> WeatherControllers { get; set; }
    }
}
