using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RacesBreathableLiquids
    {
        public long RaceId { get; set; }
        public long LiquidId { get; set; }
        public double Multiplier { get; set; }

        public virtual Liquid Liquid { get; set; }
        public virtual Race Race { get; set; }
    }
}
