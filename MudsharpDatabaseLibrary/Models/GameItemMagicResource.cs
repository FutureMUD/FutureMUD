using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GameItemMagicResource
    {
        public long GameItemId { get; set; }
        public long MagicResourceId { get; set; }
        public double Amount { get; set; }

        public virtual GameItem GameItem { get; set; }
        public virtual MagicResource MagicResource { get; set; }
    }
}
