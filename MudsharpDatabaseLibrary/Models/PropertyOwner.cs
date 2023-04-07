using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class PropertyOwner
	{
		public PropertyOwner()
		{
			
		}

		public long Id { get; set; }
		public long PropertyId { get; set; }
		public long FrameworkItemId { get; set; }
		public string FrameworkItemType { get; set; }
		public decimal ShareOfOwnership { get; set; }
		public long? RevenueAccountId { get; set; }

		public virtual Property Property { get; set; }
		public virtual BankAccount RevenueAccount { get; set; }
	}
}
