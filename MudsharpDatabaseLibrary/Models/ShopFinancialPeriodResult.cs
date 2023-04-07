using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ShopFinancialPeriodResult
    {
        public long EconomicZoneId { get; set; }
        public long ShopId { get; set; }
        public long FinancialPeriodId { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal SalesTax { get; set; }
        public decimal ProfitsTax { get; set; }

        public virtual EconomicZone EconomicZone { get; set; }
        public virtual FinancialPeriod FinancialPeriod { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
