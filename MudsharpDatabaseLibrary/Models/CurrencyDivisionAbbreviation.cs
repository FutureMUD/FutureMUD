using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CurrencyDivisionAbbreviation
    {
        public string Pattern { get; set; }
        public long CurrencyDivisionId { get; set; }

        public virtual CurrencyDivision CurrencyDivision { get; set; }
    }
}
