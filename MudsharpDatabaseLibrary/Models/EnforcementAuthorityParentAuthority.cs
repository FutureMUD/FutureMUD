using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EnforcementAuthorityParentAuthority
    {
        public long ParentId { get; set; }
        public long ChildId { get; set; }

        public virtual EnforcementAuthority Child { get; set; }
        public virtual EnforcementAuthority Parent { get; set; }
    }
}
