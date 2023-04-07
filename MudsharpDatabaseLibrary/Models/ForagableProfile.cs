using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ForagableProfile
    {
        public ForagableProfile()
        {
            ForagableProfilesForagables = new HashSet<ForagableProfilesForagables>();
            ForagableProfilesHourlyYieldGains = new HashSet<ForagableProfilesHourlyYieldGains>();
            ForagableProfilesMaximumYields = new HashSet<ForagableProfilesMaximumYields>();
        }

        public long Id { get; set; }
        public int RevisionNumber { get; set; }
        public long EditableItemId { get; set; }
        public string Name { get; set; }

        public virtual EditableItem EditableItem { get; set; }
        public virtual ICollection<ForagableProfilesForagables> ForagableProfilesForagables { get; set; }
        public virtual ICollection<ForagableProfilesHourlyYieldGains> ForagableProfilesHourlyYieldGains { get; set; }
        public virtual ICollection<ForagableProfilesMaximumYields> ForagableProfilesMaximumYields { get; set; }
    }
}
