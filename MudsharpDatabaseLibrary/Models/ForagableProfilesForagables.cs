using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ForagableProfilesForagables
    {
        public long ForagableProfileId { get; set; }
        public int ForagableProfileRevisionNumber { get; set; }
        public long ForagableId { get; set; }

        public virtual ForagableProfile ForagableProfile { get; set; }
    }
}
