using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ShardsClocks
    {
        public long ShardId { get; set; }
        public long ClockId { get; set; }

        public virtual Shard Shard { get; set; }
    }
}
