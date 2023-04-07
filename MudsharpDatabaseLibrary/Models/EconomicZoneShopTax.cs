using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EconomicZoneShopTax
    {
        public long EconomicZoneId { get; set; }
        public long ShopId { get; set; }
        public decimal OutstandingProfitTaxes { get; set; }
        public decimal OutstandingSalesTaxes { get; set; }
        public decimal TaxesInCredits { get; set; }

        public virtual EconomicZone EconomicZone { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
