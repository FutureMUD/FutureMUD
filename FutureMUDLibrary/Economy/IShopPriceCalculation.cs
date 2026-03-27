#nullable enable

using System.Collections.Generic;

namespace MudSharp.Economy;

public interface IShopPriceCalculation
{
	IShop Shop { get; }
	IMerchandise Merchandise { get; }
	ShopDealApplicability Applicability { get; }
	int Quantity { get; }
	decimal BaseUnitPrice { get; }
	decimal AdjustedUnitPrice { get; }
	decimal UnitTax { get; }
	decimal TotalPretaxPrice { get; }
	decimal IncludedTax { get; }
	decimal TotalPrice { get; }
	decimal TotalPriceAdjustmentPercentage { get; }
	bool VolumeDealsExist { get; }
	IEnumerable<IShopDeal> AppliedDeals { get; }
}
