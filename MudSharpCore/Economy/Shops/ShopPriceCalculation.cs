#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Economy;

namespace MudSharp.Economy.Shops;

public class ShopPriceCalculation : IShopPriceCalculation
{
	public IShop Shop { get; init; } = null!;
	public IMerchandise Merchandise { get; init; } = null!;
	public ShopDealApplicability Applicability { get; init; }
	public int Quantity { get; init; }
	public decimal BaseUnitPrice { get; init; }
	public decimal AdjustedUnitPrice { get; init; }
	public decimal UnitTax { get; init; }
	public decimal TotalPretaxPrice { get; init; }
	public decimal IncludedTax { get; init; }
	public decimal TotalPrice { get; init; }
	public decimal TotalPriceAdjustmentPercentage { get; init; }
	public bool VolumeDealsExist { get; init; }
	public IEnumerable<IShopDeal> AppliedDeals { get; init; } = Enumerable.Empty<IShopDeal>();
}
