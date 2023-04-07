using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Zone
    {
        public Zone()
        {
            HooksPerceivables = new HashSet<HooksPerceivable>();
            LegalAuthoritiesZones = new HashSet<LegalAuthoritiesZones>();
            Rooms = new HashSet<Room>();
            ZonesTimezones = new HashSet<ZonesTimezones>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long ShardId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public long? DefaultCellId { get; set; }
        public double AmbientLightPollution { get; set; }
        public long? ForagableProfileId { get; set; }
        public long? WeatherControllerId { get; set; }

        public virtual Cell DefaultCell { get; set; }
        public virtual Shard Shard { get; set; }
        public virtual WeatherController WeatherController { get; set; }
        public virtual ICollection<HooksPerceivable> HooksPerceivables { get; set; }
        public virtual ICollection<LegalAuthoritiesZones> LegalAuthoritiesZones { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<ZonesTimezones> ZonesTimezones { get; set; }
    }
}
