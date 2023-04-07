using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Areas
    {
        public Areas()
        {
            AreasRooms = new HashSet<AreasRooms>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? WeatherControllerId { get; set; }

        public virtual WeatherController WeatherController { get; set; }
        public virtual ICollection<AreasRooms> AreasRooms { get; set; }
    }
}
