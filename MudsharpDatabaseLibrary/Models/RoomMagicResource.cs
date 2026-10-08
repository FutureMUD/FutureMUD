using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RoomMagicResource
    {
        public long RoomId { get; set; }
        public long MagicResourceId { get; set; }
        public double Amount { get; set; }

        public virtual Room Room { get; set; }
        public virtual MagicResource MagicResource { get; set; }
    }
}
