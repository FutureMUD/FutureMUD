using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class AuctionHouse
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public long EconomicZoneId { get; set; }
		public long AuctionHouseCellId { get; set; }
		public long ProfitsBankAccountId { get; set; }
		public decimal AuctionListingFeeFlat { get; set; }
		public decimal AuctionListingFeeRate { get; set; }
		public string Definition { get; set; }
		public double DefaultListingTime { get; set; }

		public virtual EconomicZone EconomicZone { get;set; }
		public virtual Cell AuctionHouseCell {get; set; }
		public virtual BankAccount ProfitsBankAccount { get; set; }
	}
}
