using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class LegalAuthoritiesZones
    {
        public long ZoneId { get; set; }
        public long LegalAuthorityId { get; set; }

        public virtual LegalAuthority LegalAuthority { get; set; }
        public virtual Zone Zone { get; set; }
    }
}
