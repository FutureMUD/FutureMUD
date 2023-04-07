using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChannelIgnorer
    {
        public long ChannelId { get; set; }
        public long AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Channel Channel { get; set; }
    }
}
