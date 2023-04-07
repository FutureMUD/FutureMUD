using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CurrencyDescriptionPatternElementSpecialValues
    {
        public decimal Value { get; set; }
        public string Text { get; set; }
        public long CurrencyDescriptionPatternElementId { get; set; }

        public virtual CurrencyDescriptionPatternElement CurrencyDescriptionPatternElement { get; set; }
    }
}
