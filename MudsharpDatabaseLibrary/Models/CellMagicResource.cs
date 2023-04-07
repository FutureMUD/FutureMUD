using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CellMagicResource
    {
        public long CellId { get; set; }
        public long MagicResourceId { get; set; }
        public double Amount { get; set; }

        public virtual Cell Cell { get; set; }
        public virtual MagicResource MagicResource { get; set; }
    }
}
