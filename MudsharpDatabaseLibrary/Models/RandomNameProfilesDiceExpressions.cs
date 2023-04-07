using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RandomNameProfilesDiceExpressions
    {
        public long RandomNameProfileId { get; set; }
        public int NameUsage { get; set; }
        public string DiceExpression { get; set; }

        public virtual RandomNameProfile RandomNameProfile { get; set; }
    }
}
