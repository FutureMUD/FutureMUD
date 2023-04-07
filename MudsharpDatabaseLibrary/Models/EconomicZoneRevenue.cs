using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EconomicZoneRevenue
    {
        public long EconomicZoneId { get; set; }
        public long FinancialPeriodId { get; set; }
        public decimal TotalTaxRevenue { get; set; }

        public virtual EconomicZone EconomicZone { get; set; }
        public virtual FinancialPeriod FinancialPeriod { get; set; }
    }
}
