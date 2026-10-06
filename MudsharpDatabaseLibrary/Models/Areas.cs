using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Areas
    {
        public Areas()
        {
            AreasCells = new HashSet<AreasCells>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? WeatherControllerId { get; set; }

        public virtual WeatherController WeatherController { get; set; }
        public virtual ICollection<AreasCells> AreasCells { get; set; }
    }
}
