using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ClanAdministrationCell
    {
        public long ClanId { get; set; }
        public long CellId { get; set; }

        public virtual Cell Cell { get; set; }
        public virtual Clan Clan { get; set; }
    }
}
