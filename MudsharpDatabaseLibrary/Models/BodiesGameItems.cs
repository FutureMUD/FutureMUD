using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodiesGameItems
    {
        public long BodyId { get; set; }
        public long GameItemId { get; set; }
        public int EquippedOrder { get; set; }
        public long? WearProfile { get; set; }
        public int? Wielded { get; set; }

        public virtual Body Body { get; set; }
        public virtual GameItem GameItem { get; set; }
    }
}
