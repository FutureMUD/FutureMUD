using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class LawsVictimClasses
    {
        public long LawId { get; set; }
        public long LegalClassId { get; set; }

        public virtual Law Law { get; set; }
        public virtual LegalClass LegalClass { get; set; }
    }
}
