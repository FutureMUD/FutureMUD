using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Timezone
    {
        public Timezone()
        {
            EconomicZones = new HashSet<EconomicZone>();
            WeatherControllers = new HashSet<WeatherController>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OffsetMinutes { get; set; }
        public int OffsetHours { get; set; }
        public long ClockId { get; set; }

        public virtual Clock Clock { get; set; }
        public virtual ICollection<EconomicZone> EconomicZones { get; set; }
        public virtual ICollection<WeatherController> WeatherControllers { get; set; }
    }
}
