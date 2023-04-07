using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Shard
    {
        public Shard()
        {
            HooksPerceivables = new HashSet<HooksPerceivable>();
            ShardsCalendars = new HashSet<ShardsCalendars>();
            ShardsCelestials = new HashSet<ShardsCelestials>();
            ShardsClocks = new HashSet<ShardsClocks>();
            Zones = new HashSet<Zone>();
        }

        public string Name { get; set; }
        public long Id { get; set; }
        public double MinimumTerrestrialLux { get; set; }
        public long SkyDescriptionTemplateId { get; set; }
        public double SphericalRadiusMetres { get; set; }

        public virtual SkyDescriptionTemplate SkyDescriptionTemplate { get; set; }
        public virtual ICollection<HooksPerceivable> HooksPerceivables { get; set; }
        public virtual ICollection<ShardsCalendars> ShardsCalendars { get; set; }
        public virtual ICollection<ShardsCelestials> ShardsCelestials { get; set; }
        public virtual ICollection<ShardsClocks> ShardsClocks { get; set; }
        public virtual ICollection<Zone> Zones { get; set; }
    }
}
