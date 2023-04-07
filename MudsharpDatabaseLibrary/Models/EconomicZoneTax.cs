using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public class EconomicZoneTax
    {
        public long Id { get; set; }
        public long EconomicZoneId { get;set; }
        public virtual EconomicZone EconomicZone { get; set; }
        public string Name { get; set; }
        public string MerchantDescription { get; set; }
        public long? MerchandiseFilterProgId { get; set; }
        public virtual FutureProg MerchandiseFilterProg { get; set; }
        public string TaxType { get; set; }
        public string Definition { get; set; }
    }
}
