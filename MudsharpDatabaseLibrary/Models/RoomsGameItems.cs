using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RoomsGameItems
    {
        public long RoomId { get; set; }
        public long GameItemId { get; set; }

        public virtual Room Room { get; set; }
        public virtual GameItem GameItem { get; set; }
    }
}
