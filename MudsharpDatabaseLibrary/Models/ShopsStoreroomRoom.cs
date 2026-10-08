using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ShopsStoreroomRoom
    {
        public long ShopId { get; set; }
        public long RoomId { get; set; }

        public virtual Room Room { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
