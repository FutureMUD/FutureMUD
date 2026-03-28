#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Economy.Shops;

public abstract partial class Shop
{
	private readonly List<IShopDeal> _deals = new();
	public IEnumerable<IShopDeal> Deals => _deals;

	public void AddDeal(IShopDeal deal)
	{
		_deals.Add(deal);
		Changed = true;
	}

	public void RemoveDeal(IShopDeal deal)
	{
		_deals.Remove(deal);
		deal.Delete();
		Changed = true;
	}

	public IShopPriceCalculation GetPriceCalculation(ICharacter actor, IMerchandise merchandise, int quantity,
		ShopDealApplicability applicability = ShopDealApplicability.Sell)
	{
		var now = EconomicZone.ZoneForTimePurposes.DateTime();
		var baseUnitPrice = applicability switch
		{
			ShopDealApplicability.Buy => merchandise.EffectivePrice * merchandise.BaseBuyModifier,
			_ => merchandise.EffectivePrice
		};

		var applicableDeals = Deals
			.Where(x => x.Applies(merchandise, actor, quantity, applicability, now))
			.ToList();
		var cumulativeDeals = applicableDeals
			.Where(x => x.IsCumulative)
			.ToList();
		var selectedDeals = new List<IShopDeal>(cumulativeDeals);
		var nonCumulativeDeals = applicableDeals
			.Where(x => !x.IsCumulative)
			.ToList();
		if (nonCumulativeDeals.Any())
		{
			selectedDeals.Add(applicability == ShopDealApplicability.Buy
				? nonCumulativeDeals.MaxBy(x => x.PriceAdjustmentPercentage)!
				: nonCumulativeDeals.MinBy(x => x.PriceAdjustmentPercentage)!);
		}

		var totalAdjustment = selectedDeals.Sum(x => x.PriceAdjustmentPercentage);
		var adjustedUnitPrice = Math.Max(0.0M, baseUnitPrice * (1.0M + totalAdjustment));
		var unitTax = applicability == ShopDealApplicability.Buy
			? 0.0M
			: Math.Round(
				EconomicZone.SalesTaxes
					.Where(x => x.Applies(merchandise, actor))
					.Sum(x => x.TaxValue(merchandise, actor, adjustedUnitPrice)),
				0,
				MidpointRounding.AwayFromZero);
		var totalPretax = Math.Truncate(adjustedUnitPrice * quantity);
		var includedTax = Math.Truncate(unitTax * quantity);
		var totalPrice = totalPretax + includedTax;

		return new ShopPriceCalculation
		{
			Shop = this,
			Merchandise = merchandise,
			Applicability = applicability,
			Quantity = quantity,
			BaseUnitPrice = baseUnitPrice,
			AdjustedUnitPrice = adjustedUnitPrice,
			UnitTax = unitTax,
			TotalPretaxPrice = totalPretax,
			IncludedTax = includedTax,
			TotalPrice = totalPrice,
			TotalPriceAdjustmentPercentage = totalAdjustment,
			VolumeDealsExist = Deals.Any(x => VolumeDealWouldApplyAtThreshold(x, merchandise, actor, applicability, now)),
			AppliedDeals = selectedDeals
		};
	}

	private bool VolumeDealWouldApplyAtThreshold(IShopDeal deal, IMerchandise merchandise, ICharacter actor,
		ShopDealApplicability applicability, MudDateTime now)
	{
		if (deal.DealType != ShopDealType.Volume)
		{
			return false;
		}

		return deal.Applies(merchandise, actor, Math.Max(2, deal.MinimumQuantity), applicability, now);
	}

	private string DescribeDealSummary(IMerchandise merchandise, ICharacter purchaser, IPerceiver voyeur)
	{
		var calculation = GetPriceCalculation(purchaser, merchandise, 1);
		var summary = new List<string>();
		if (calculation.AppliedDeals.Any())
		{
			summary.Add(ShopDeal.DescribePercentage(calculation.TotalPriceAdjustmentPercentage, voyeur));
		}

		var now = EconomicZone.ZoneForTimePurposes.DateTime();
		var volumeDeals = Deals
			.Where(x => VolumeDealWouldApplyAtThreshold(x, merchandise, purchaser, ShopDealApplicability.Sell, now))
			.Select(x => x.MinimumQuantity)
			.Distinct()
			.OrderBy(x => x)
			.ToList();
		if (volumeDeals.Any())
		{
			summary.Add($"Vol {volumeDeals.Select(x => x.ToString("N0", voyeur)).ListToString()}+"
				.ColourCommand());
		}

		return summary.Any() ? summary.ListToString() : "None".ColourError();
	}

	private string ShowDealsInternal(ICharacter actor, ICharacter purchaser, IMerchandise? merchandise)
	{
		var deals = merchandise is null
			? Deals.Where(x => !x.IsExpired).OrderBy(x => x.Name).ToList()
			: Deals.Where(x => !x.IsExpired && x.AppliesToMerchandise(merchandise)).OrderBy(x => x.Name).ToList();
		if (!deals.Any())
		{
			return merchandise is null
				? $"{Name.TitleCase().ColourName()} currently has no active deals."
				: $"{Name.TitleCase().ColourName()} currently has no active deals for {merchandise.ListDescription.ColourObject()}.";
		}

		var sb = new StringBuilder();
		sb.AppendLine(Name.TitleCase().ColourName());
		sb.AppendLine(merchandise is null
			? "Active shop deals:"
			: $"Active deals for {merchandise.ListDescription.ColourObject()}:");
		sb.AppendLine();
		var now = EconomicZone.ZoneForTimePurposes.DateTime();
		sb.AppendLine(StringUtilities.GetTextTable(
			from deal in deals
			let targetMerchandise = merchandise ?? ResolveRepresentativeMerchandise(deal)
			let eligible = targetMerchandise is not null &&
			               deal.Applies(targetMerchandise, purchaser, Math.Max(1, deal.MinimumQuantity), deal.Applicability == ShopDealApplicability.Both ? ShopDealApplicability.Sell : deal.Applicability, now)
			select new[]
			{
				deal.Id.ToString("N0", actor),
				deal.Name,
				deal is ShopDeal shopDeal ? shopDeal.DescribeType(actor) : deal.DealType.DescribeEnum().ColourName(),
				deal is ShopDeal targetDeal ? targetDeal.DescribeTarget(actor) : deal.TargetType.DescribeEnum().ColourName(),
				ShopDeal.DescribePercentage(deal.PriceAdjustmentPercentage, actor),
				deal.Applicability.DescribeEnum().ColourName(),
				deal.IsCumulative.ToColouredString(),
				deal.Expiry.Date is null
					? "Never".ColourValue()
					: deal.Expiry.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue(),
				eligible.ToColouredString()
			},
			new[]
			{
				"Id",
				"Name",
				"Type",
				"Target",
				"Adjustment",
				"Applies",
				"Stacks",
				"Expiry",
				"Eligible?"
			},
			actor.LineFormatLength,
			truncatableColumnIndex: 1,
			colour: Telnet.Yellow,
			unicodeTable: actor.Account.UseUnicode
		));
		if (actor != purchaser)
		{
			sb.AppendLine();
			sb.AppendLine(
				$"Note: Showing eligibility and pricing for an individual other than yourself. Only valid when using their account."
					.ColourError());
		}

		return sb.ToString();
	}

	private IMerchandise? ResolveRepresentativeMerchandise(IShopDeal deal)
	{
		return deal.TargetType switch
		{
			ShopDealTargetType.Merchandise => deal.TargetMerchandise,
			ShopDealTargetType.ItemTag => Merchandises.FirstOrDefault(x => deal.AppliesToMerchandise(x)),
			_ => Merchandises.FirstOrDefault()
		};
	}

}
