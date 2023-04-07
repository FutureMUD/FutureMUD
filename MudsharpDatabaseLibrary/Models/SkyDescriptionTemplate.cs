using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class SkyDescriptionTemplate
    {
        public SkyDescriptionTemplate()
        {
            Shards = new HashSet<Shard>();
            SkyDescriptionTemplatesValues = new HashSet<SkyDescriptionTemplatesValue>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Shard> Shards { get; set; }
        public virtual ICollection<SkyDescriptionTemplatesValue> SkyDescriptionTemplatesValues { get; set; }
    }
}
