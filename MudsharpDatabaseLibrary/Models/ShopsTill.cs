using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ShopsTill
    {
        public long ShopId { get; set; }
        public long GameItemId { get; set; }

        public virtual GameItem GameItem { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
