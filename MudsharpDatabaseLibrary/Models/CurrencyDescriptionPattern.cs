using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CurrencyDescriptionPattern
    {
        public CurrencyDescriptionPattern()
        {
            CurrencyDescriptionPatternElements = new HashSet<CurrencyDescriptionPatternElement>();
        }

        public long Id { get; set; }
        public int Type { get; set; }
        public long CurrencyId { get; set; }
        public long? FutureProgId { get; set; }
        public string NegativePrefix { get; set; }
        public int Order { get; set; }
        public bool UseNaturalAggregationStyle { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual FutureProg FutureProg { get; set; }
        public virtual ICollection<CurrencyDescriptionPatternElement> CurrencyDescriptionPatternElements { get; set; }
    }
}
