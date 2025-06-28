using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class PropertyLeaseOrder
	{
		public PropertyLeaseOrder()
		{
			PropertyLeases = new HashSet<PropertyLease>();
		}

		public long Id { get; set; }
		public long PropertyId { get; set; }
		public decimal PricePerInterval { get; set; }
		public decimal BondRequired { get; set; }
		public string Interval { get; set; }
		public long? CanLeaseProgCharacterId { get; set; }
		public long? CanLeaseProgClanId { get; set; }
		public double MinimumLeaseDurationDays { get; set; }
		public double MaximumLeaseDurationDays { get; set; }
		public bool AllowAutoRenew { get; set; }
		public bool AutomaticallyRelistAfterLeaseTerm { get; set; }
                public bool AllowLeaseNovation { get; set; }
                public bool RekeyOnLeaseEnd { get; set; }
                public bool ListedForLease { get; set; }
		public decimal FeeIncreasePercentageAfterLeaseTerm { get; set; }
		public string PropertyOwnerConsentInfo { get; set; }

		public virtual Property Property { get; set; }
		public virtual FutureProg CanLeaseProgCharacter { get; set; }
		public virtual FutureProg CanLeaseProgClan { get; set; }
		public virtual ICollection<PropertyLease> PropertyLeases { get; set; }
	}
}
