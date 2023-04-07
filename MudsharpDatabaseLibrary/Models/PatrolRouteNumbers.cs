using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public partial class PatrolRouteNumbers
    {
        public PatrolRouteNumbers()
        {
        }

        public long PatrolRouteId { get; set; }
        public long EnforcementAuthorityId { get; set; }
        public int NumberRequired { get; set; }

        public virtual PatrolRoute PatrolRoute { get; set; }
        public virtual EnforcementAuthority EnforcementAuthority { get; set; }
    }
}
