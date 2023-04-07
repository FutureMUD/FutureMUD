using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ShopsStoreroomCell
    {
        public long ShopId { get; set; }
        public long CellId { get; set; }

        public virtual Cell Cell { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
