using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CellsRangedCovers
    {
        public long CellId { get; set; }
        public long RangedCoverId { get; set; }

        public virtual Cell Cell { get; set; }
        public virtual RangedCover RangedCover { get; set; }
    }
}
