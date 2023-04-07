using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ShardsCelestials
    {
        public long ShardId { get; set; }
        public long CelestialId { get; set; }

        public virtual Shard Shard { get; set; }
    }
}
