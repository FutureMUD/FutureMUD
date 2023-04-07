using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CellsForagableYield
    {
        public long CellId { get; set; }
        public string ForagableType { get; set; }
        public double Yield { get; set; }

        public virtual Cell Cell { get; set; }
    }
}
