using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ClanAdministrationRoom
    {
        public long ClanId { get; set; }
        public long RoomId { get; set; }

        public virtual Room Room { get; set; }
        public virtual Clan Clan { get; set; }
    }
}
