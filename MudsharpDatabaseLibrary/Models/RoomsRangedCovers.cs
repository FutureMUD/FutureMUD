using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RoomsRangedCovers
    {
        public long RoomId { get; set; }
        public long RangedCoverId { get; set; }

        public virtual Room Room { get; set; }
        public virtual RangedCover RangedCover { get; set; }
    }
}
