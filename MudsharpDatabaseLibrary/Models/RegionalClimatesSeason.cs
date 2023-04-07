using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RegionalClimatesSeason
    {
        public long RegionalClimateId { get; set; }
        public long SeasonId { get; set; }
        public string TemperatureInfo { get; set; }

        public virtual RegionalClimate RegionalClimate { get; set; }
        public virtual Season Season { get; set; }
    }
}
