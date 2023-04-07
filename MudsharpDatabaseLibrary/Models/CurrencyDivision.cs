using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CurrencyDivision
    {
        public CurrencyDivision()
        {
            CurrencyDescriptionPatternElements = new HashSet<CurrencyDescriptionPatternElement>();
            CurrencyDivisionAbbreviations = new HashSet<CurrencyDivisionAbbreviation>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public decimal BaseUnitConversionRate { get; set; }
        public long CurrencyId { get; set; }
        public bool IgnoreCase { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual ICollection<CurrencyDescriptionPatternElement> CurrencyDescriptionPatternElements { get; set; }
        public virtual ICollection<CurrencyDivisionAbbreviation> CurrencyDivisionAbbreviations { get; set; }
    }
}
