using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class Property
	{
		public Property()
		{
			LeaseOrders = new HashSet<PropertyLeaseOrder>();
			PropertyLeases = new HashSet<PropertyLease>();
			PropertyOwners = new HashSet<PropertyOwner>();
			PropertyLocations = new List<PropertyLocation>();
			PropertyKeys = new List<PropertyKey>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public long EconomicZoneId { get; set; }
		public string DetailedDescription { get; set; }
		public string LastChangeOfOwnership { get; set; }
		public bool ApplyCriminalCodeInProperty { get; set; }
		public long? LeaseId { get; set; }
		public long? LeaseOrderId { get; set; }
		public long? SaleOrderId { get; set; }
		public decimal LastSaleValue { get; set; }

		public virtual EconomicZone EconomicZone { get; set; }
		public virtual PropertyLease Lease { get; set; }
		public virtual PropertyLeaseOrder LeaseOrder { get; set; }
		public virtual PropertySaleOrder SaleOrder { get; set; }

		public virtual ICollection<PropertyLeaseOrder> LeaseOrders { get; set; }
		public virtual ICollection<PropertyLease> PropertyLeases { get; set; }
		public virtual ICollection<PropertyOwner> PropertyOwners { get; set; }
		public virtual ICollection<PropertyLocation> PropertyLocations { get; set; }
		public virtual ICollection<PropertyKey> PropertyKeys { get; set; }
	}
}
