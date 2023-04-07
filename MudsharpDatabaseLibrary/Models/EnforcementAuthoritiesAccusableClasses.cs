using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EnforcementAuthoritiesAccusableClasses
    {
        public long EnforcementAuthorityId { get; set; }
        public long LegalClassId { get; set; }

        public virtual EnforcementAuthority EnforcementAuthority { get; set; }
        public virtual LegalClass LegalClass { get; set; }
    }
}
