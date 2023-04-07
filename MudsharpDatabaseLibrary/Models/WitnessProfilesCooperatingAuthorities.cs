using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class WitnessProfilesCooperatingAuthorities
    {
        public long WitnessProfileId { get; set; }
        public long LegalAuthorityId { get; set; }

        public virtual LegalAuthority LegalAuthority { get; set; }
        public virtual WitnessProfile WitnessProfile { get; set; }
    }
}
