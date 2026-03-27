using System;

namespace MudSharp.Models
{
	public partial class ShopDeal
	{
		public long Id { get; set; }
		public long ShopId { get; set; }
		public string Name { get; set; }
		public int DealType { get; set; }
		public int TargetType { get; set; }
		public long? MerchandiseId { get; set; }
		public long? TagId { get; set; }
		public decimal PriceAdjustmentPercentage { get; set; }
		public int? MinimumQuantity { get; set; }
		public int Applicability { get; set; }
		public long? EligibilityProgId { get; set; }
		public string ExpiryDateTime { get; set; }
		public bool IsCumulative { get; set; }

		public virtual FutureProg EligibilityProg { get; set; }
		public virtual Merchandise Merchandise { get; set; }
		public virtual Shop Shop { get; set; }
		public virtual Tag Tag { get; set; }
	}
}
