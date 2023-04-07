using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ShardsCalendars
    {
        public long ShardId { get; set; }
        public long CalendarId { get; set; }

        public virtual Shard Shard { get; set; }
    }
}
