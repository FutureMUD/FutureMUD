using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RoomsForagableYield
    {
        public long RoomId { get; set; }
        public string ForagableType { get; set; }
        public double Yield { get; set; }

        public virtual Room Room { get; set; }
    }
}
