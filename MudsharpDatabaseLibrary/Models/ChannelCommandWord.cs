using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChannelCommandWord
    {
        public string Word { get; set; }
        public long ChannelId { get; set; }

        public virtual Channel Channel { get; set; }
    }
}
