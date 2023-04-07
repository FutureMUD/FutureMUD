using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenRolesMerit
    {
        public long ChargenRoleId { get; set; }
        public long MeritId { get; set; }

        public virtual ChargenRole ChargenRole { get; set; }
        public virtual Merit Merit { get; set; }
    }
}
