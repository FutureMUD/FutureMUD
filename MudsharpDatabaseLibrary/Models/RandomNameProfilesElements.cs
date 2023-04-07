using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RandomNameProfilesElements
    {
        public long RandomNameProfileId { get; set; }
        public int NameUsage { get; set; }
        public string Name { get; set; }
        public int Weighting { get; set; }

        public virtual RandomNameProfile RandomNameProfile { get; set; }
    }
}
