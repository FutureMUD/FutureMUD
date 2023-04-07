using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Celestial
    {
        public Celestial()
        {
            Seasons = new HashSet<Season>();
            WeatherControllers = new HashSet<WeatherController>();
        }

        public long Id { get; set; }
        public string Definition { get; set; }
        public int Minutes { get; set; }
        public long FeedClockId { get; set; }
        public int CelestialYear { get; set; }
        public int LastYearBump { get; set; }
        public string CelestialType { get; set; }

        public virtual ICollection<Season> Seasons { get; set; }
        public virtual ICollection<WeatherController> WeatherControllers { get; set; }
    }
}
