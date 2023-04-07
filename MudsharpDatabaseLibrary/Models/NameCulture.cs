using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class NameCulture
    {
        public NameCulture()
        {
	        RandomNameProfiles = new HashSet<RandomNameProfile>();
            CulturesNameCultures = new HashSet<CulturesNameCultures>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public virtual ICollection<RandomNameProfile> RandomNameProfiles { get; set; }
        public virtual ICollection<CulturesNameCultures> CulturesNameCultures { get; set; }
    }
}
