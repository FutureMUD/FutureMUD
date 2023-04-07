using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CurrencyDescriptionPatternElement
    {
        public CurrencyDescriptionPatternElement()
        {
            CurrencyDescriptionPatternElementSpecialValues = new HashSet<CurrencyDescriptionPatternElementSpecialValues>();
        }

        public long Id { get; set; }
        public string Pattern { get; set; }
        public int Order { get; set; }
        public bool ShowIfZero { get; set; }
        public long CurrencyDivisionId { get; set; }
        public long CurrencyDescriptionPatternId { get; set; }
        public string PluraliseWord { get; set; }
        public string AlternatePattern { get; set; }
        public int RoundingMode { get; set; }
        public bool SpecialValuesOverrideFormat { get; set; }

        public virtual CurrencyDescriptionPattern CurrencyDescriptionPattern { get; set; }
        public virtual CurrencyDivision CurrencyDivision { get; set; }
        public virtual ICollection<CurrencyDescriptionPatternElementSpecialValues> CurrencyDescriptionPatternElementSpecialValues { get; set; }
    }
}
