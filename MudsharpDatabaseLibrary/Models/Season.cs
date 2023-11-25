using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Season
    {
        public Season()
        {
            RegionalClimatesSeasons = new HashSet<RegionalClimatesSeason>();
            WeatherControllers = new HashSet<WeatherController>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string SeasonGroup { get; set; }
        public int CelestialDayOnset { get; set; }
        public long CelestialId { get; set; }

        public virtual Celestial Celestial { get; set; }
        public virtual ICollection<RegionalClimatesSeason> RegionalClimatesSeasons { get; set; }
        public virtual ICollection<WeatherController> WeatherControllers { get; set; }
        public virtual ICollection<ClimateModelSeason> ClimateModelSeasons { get; set; }
    }
}
