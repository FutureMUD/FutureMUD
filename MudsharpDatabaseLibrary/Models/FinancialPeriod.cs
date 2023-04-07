using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class FinancialPeriod
    {
        public FinancialPeriod()
        {
            EconomicZoneRevenues = new HashSet<EconomicZoneRevenue>();
            EconomicZones = new HashSet<EconomicZone>();
            ShopFinancialPeriodResults = new HashSet<ShopFinancialPeriodResult>();
        }

        public long Id { get; set; }
        public long EconomicZoneId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string MudPeriodStart { get; set; }
        public string MudPeriodEnd { get; set; }

        public virtual EconomicZone EconomicZone { get; set; }
        public virtual ICollection<EconomicZoneRevenue> EconomicZoneRevenues { get; set; }
        public virtual ICollection<EconomicZone> EconomicZones { get; set; }
        public virtual ICollection<ShopFinancialPeriodResult> ShopFinancialPeriodResults { get; set; }
    }
}
