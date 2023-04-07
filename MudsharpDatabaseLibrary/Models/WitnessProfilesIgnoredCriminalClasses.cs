using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class WitnessProfilesIgnoredCriminalClasses
    {
        public long WitnessProfileId { get; set; }
        public long LegalClassId { get; set; }

        public virtual LegalClass LegalClass { get; set; }
        public virtual WitnessProfile WitnessProfile { get; set; }
    }
}
