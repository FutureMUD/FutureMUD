using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class AreasRooms
    {
        public long AreaId { get; set; }
        public long RoomId { get; set; }

        public virtual Areas Area { get; set; }
        public virtual Room Room { get; set; }
    }
}
