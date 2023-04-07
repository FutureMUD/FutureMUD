using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class PropertySaleOrder
	{
		public PropertySaleOrder()
		{
			
		}

		public long Id { get; set; }
		public long PropertyId { get; set; }
		public decimal ReservePrice { get; set; }
		public int OrderStatus { get; set; }
		public string StartOfListing { get; set; }
		public double DurationOfListingDays { get; set; }
		public string PropertyOwnerConsentInfo { get; set; }

		public virtual Property Property { get; set; }
	}
}
