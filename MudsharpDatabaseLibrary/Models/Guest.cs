using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Guest
    {
        public long CharacterId { get; set; }

        public virtual Character Character { get; set; }
    }
}
