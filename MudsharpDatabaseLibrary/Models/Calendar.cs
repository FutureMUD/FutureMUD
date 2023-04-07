using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Calendar
    {
        public Calendar()
        {
            Clans = new HashSet<Clan>();
            EconomicZones = new HashSet<EconomicZone>();
        }

        public long Id { get; set; }
        public string Definition { get; set; }
        public string Date { get; set; }
        public long FeedClockId { get; set; }

        public virtual ICollection<Clan> Clans { get; set; }
        public virtual ICollection<EconomicZone> EconomicZones { get; set; }
    }
}
