using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RoomsTags
    {
        public long RoomId { get; set; }
        public long TagId { get; set; }

        public virtual Room Room { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
