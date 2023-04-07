using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Door
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public int Style { get; set; }
        public bool IsOpen { get; set; }
        public long? LockedWith { get; set; }

        public virtual Lock LockedWithNavigation { get; set; }
    }
}
