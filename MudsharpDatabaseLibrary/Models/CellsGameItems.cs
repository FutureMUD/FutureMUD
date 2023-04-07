using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CellsGameItems
    {
        public long CellId { get; set; }
        public long GameItemId { get; set; }

        public virtual Cell Cell { get; set; }
        public virtual GameItem GameItem { get; set; }
    }
}
