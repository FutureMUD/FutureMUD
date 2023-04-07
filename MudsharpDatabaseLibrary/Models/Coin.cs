using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Coin
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public decimal Value { get; set; }
        public long CurrencyId { get; set; }
        public double Weight { get; set; }
        public string GeneralForm { get; set; }
        public string PluralWord { get; set; }

        public virtual Currency Currency { get; set; }
    }
}
