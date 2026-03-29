using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MoreLinq.Extensions;
using System.Linq;

namespace MudSharp.Economy.Estates;

public class EstateAsset : SaveableItem, IEstateAsset
{
	public EstateAsset(MudSharp.Models.EstateAsset asset, IEstate estate)
	{
		Gameworld = estate.Gameworld;
		_id = asset.Id;
		Estate = estate;
		_assetReference = new FrameworkItemReference(asset.FrameworkItemId, asset.FrameworkItemType, Gameworld);
		IsPresumedOwnership = asset.IsPresumedOwnership;
		_ownershipShare = asset.OwnershipShare <= 0.0M ? 1.0M : asset.OwnershipShare;
		_isTransferred = asset.IsTransferred;
		_isLiquidated = asset.IsLiquidated;
		_liquidatedValue = asset.LiquidatedValue;
	}

	public EstateAsset(IEstate estate, IFrameworkItem asset, bool presumedOwnership, decimal ownershipShare = 1.0M)
	{
		Gameworld = estate.Gameworld;
		Estate = estate;
		_assetReference = new FrameworkItemReference(asset.Id, asset.FrameworkItemType, Gameworld);
		IsPresumedOwnership = presumedOwnership;
		_ownershipShare = ownershipShare <= 0.0M ? 1.0M : ownershipShare;
		_isTransferred = false;
		_isLiquidated = false;
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.EstateAsset
			{
				EstateId = estate.Id,
				FrameworkItemId = asset.Id,
				FrameworkItemType = asset.FrameworkItemType,
				IsPresumedOwnership = presumedOwnership,
				OwnershipShare = OwnershipShare,
				IsTransferred = false,
				IsLiquidated = false
			};
			FMDB.Context.EstateAssets.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "EstateAsset";

	public override void Save()
	{
		var dbitem = FMDB.Context.EstateAssets.Find(Id);
		dbitem.OwnershipShare = OwnershipShare;
		dbitem.IsTransferred = IsTransferred;
		dbitem.IsLiquidated = IsLiquidated;
		dbitem.LiquidatedValue = LiquidatedValue;
		Changed = false;
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id == 0)
		{
			return;
		}

		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.EstateAssets.Find(Id);
			if (dbitem == null)
			{
				return;
			}

			FMDB.Context.EstateAssets.Remove(dbitem);
			FMDB.Context.SaveChanges();
		}
	}

	private readonly FrameworkItemReference _assetReference;
	private IFrameworkItem _asset;
	public IEstate Estate { get; }
	public IFrameworkItem Asset => _asset ??= _assetReference.GetItem;
	private decimal _ownershipShare;
	public decimal OwnershipShare
	{
		get => _ownershipShare;
		set
		{
			_ownershipShare = value <= 0.0M ? 1.0M : value;
			Changed = true;
		}
	}
	public decimal AssumedValue => Asset switch
	{
		IProperty property => PropertyAssumedValue(property) * OwnershipShare,
		IBankAccount account => ConvertCurrency(account.CurrentBalance, account.Currency, Estate.EconomicZone.Currency),
		IGameItem item => GameItemAssumedValue(item),
		_ => 0.0M
	};
	public bool IsPresumedOwnership { get; }
	private bool _isTransferred;
	public bool IsTransferred
	{
		get => _isTransferred;
		set
		{
			_isTransferred = value;
			Changed = true;
		}
	}

	private bool _isLiquidated;
	public bool IsLiquidated
	{
		get => _isLiquidated;
		set
		{
			_isLiquidated = value;
			Changed = true;
		}
	}

	private decimal? _liquidatedValue;
	public decimal? LiquidatedValue
	{
		get => _liquidatedValue;
		set
		{
			_liquidatedValue = value;
			Changed = true;
		}
	}

	private decimal PropertyAssumedValue(IProperty property)
	{
		if (property.SaleOrder?.ShowForSale == true)
		{
			return property.SaleOrder.ReservePrice;
		}

		return property.LastSaleValue;
	}

	private decimal GameItemAssumedValue(IGameItem item)
	{
		if (item.Prototype.CostInBaseCurrency > 0.0M)
		{
			return item.Prototype.CostInBaseCurrency / Estate.EconomicZone.Currency.BaseCurrencyToGlobalBaseCurrencyConversion;
		}

		var shopPrices = Estate.Gameworld.Shops
			.Where(x => x.EconomicZone == Estate.EconomicZone)
			.SelectMany(x => x.Merchandises
				.Where(y => y.WillSell && y.Item.Id == item.Prototype.Id)
				.Select(y => ConvertCurrency(x.PriceForMerchandise(null, y, 1), x.Currency, Estate.EconomicZone.Currency)))
			.ToList();
		if (shopPrices.Any())
		{
			return shopPrices.Average();
		}

		var vendingPrices = Estate.Gameworld.Items
			.SelectNotNull(x => x.GetItemType<IVendingMachine>())
			.Where(x => x.Parent.TrueLocations.Any(y => global::MudSharp.Economy.Estates.Estate.DetermineZone(Estate.Gameworld, y) == Estate.EconomicZone))
			.SelectMany(x => x.Selections
				.Where(y => y.Prototype?.Id == item.Prototype.Id)
				.Select(y => ConvertCurrency(y.Cost, x.Currency, Estate.EconomicZone.Currency)))
			.ToList();
		if (vendingPrices.Any())
		{
			return vendingPrices.Average();
		}

		return 0.0M;
	}

	private static decimal ConvertCurrency(decimal amount, ICurrency fromCurrency, ICurrency toCurrency)
	{
		if (fromCurrency == null || toCurrency == null)
		{
			return amount;
		}

		if (fromCurrency == toCurrency)
		{
			return amount;
		}

		return amount * fromCurrency.BaseCurrencyToGlobalBaseCurrencyConversion /
		       toCurrency.BaseCurrencyToGlobalBaseCurrencyConversion;
	}
}
