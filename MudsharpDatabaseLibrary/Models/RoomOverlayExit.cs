using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RoomOverlayExit
    {
        public long RoomOverlayId { get; set; }
        public long ExitId { get; set; }

        public virtual RoomOverlay RoomOverlay { get; set; }
        public virtual Exit Exit { get; set; }
    }
}
