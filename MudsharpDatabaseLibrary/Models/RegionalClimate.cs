using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RegionalClimate
    {
        public RegionalClimate()
        {
            RegionalClimatesSeasons = new HashSet<RegionalClimatesSeason>();
            WeatherControllers = new HashSet<WeatherController>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long ClimateModelId { get; set; }

        public virtual ICollection<RegionalClimatesSeason> RegionalClimatesSeasons { get; set; }
        public virtual ICollection<WeatherController> WeatherControllers { get; set; }
    }
}
