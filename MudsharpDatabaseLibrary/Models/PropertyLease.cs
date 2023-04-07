using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class PropertyLease
	{
		public PropertyLease()
		{
			
		}

		public long Id { get; set; }
		public long PropertyId { get; set; }
		public long LeaseOrderId { get; set; }
		public string LeaseholderReference { get; set; }
		public decimal PricePerInterval { get; set; }
		public decimal BondPayment { get; set; }
		public decimal PaymentBalance { get; set; }
		public decimal BondClaimed { get; set; }
		public string LeaseStart { get; set; }
		public string LeaseEnd { get; set; }
		public string LastLeasePayment { get; set; }
		public bool AutoRenew { get; set; }
		public bool BondReturned { get; set; }
		public string Interval { get; set; }
		public string TenantInfo { get; set; }

		public virtual Property Property { get; set; }
		public virtual PropertyLeaseOrder LeaseOrder { get; set; }
	}
}
