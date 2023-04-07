using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class WeatherController
    {
        public WeatherController()
        {
            Areas = new HashSet<Areas>();
            Terrains = new HashSet<Terrain>();
            Zones = new HashSet<Zone>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long FeedClockId { get; set; }
        public long FeedClockTimeZoneId { get; set; }
        public long RegionalClimateId { get; set; }
        public long CurrentWeatherEventId { get; set; }
        public long CurrentSeasonId { get; set; }
        public int ConsecutiveUnchangedPeriods { get; set; }
        public int MinutesCounter { get; set; }
        public long? CelestialId { get; set; }
        public double Elevation { get; set; }
        public double Radius { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int HighestRecentPrecipitationLevel { get; set; }
        public int PeriodsSinceHighestPrecipitation { get; set; }

        public virtual Celestial Celestial { get; set; }
        public virtual Season CurrentSeason { get; set; }
        public virtual WeatherEvent CurrentWeatherEvent { get; set; }
        public virtual Clock FeedClock { get; set; }
        public virtual Timezone FeedClockTimeZone { get; set; }
        public virtual RegionalClimate RegionalClimate { get; set; }
        public virtual ICollection<Areas> Areas { get; set; }
        public virtual ICollection<Terrain> Terrains { get; set; }
        public virtual ICollection<Zone> Zones { get; set; }
    }
}
