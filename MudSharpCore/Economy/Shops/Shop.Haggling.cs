#nullable enable
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;

namespace MudSharp.Economy.Shops;

public abstract partial class Shop
{
	public HaggleSettings Haggling { get; private set; } = new();

	private bool BuildingCommandHaggle(ICharacter actor, StringStack command)
	{
		const string help = "Haggle options: show | toggle | difficulty <difficulty|default> | limits <sale-discount> <sale-markup> <buy-bonus> <buy-penalty> (percentages) | exclude <merchandise-id> | pricing <employee> | audit. Negative sales adjustments discount; positive buyback adjustments increase payout.";
		if (command.IsFinished || command.PeekSpeech().EqualTo("show"))
		{
			actor.OutputHandler.Send($"Haggling disabled: {Haggling.Disabled.ToColouredString()}; base difficulty {HaggleService.BaseDifficulty(this).DescribeEnum().ColourName()}.\nSale discount/markup: {Haggling.SaleDiscount.ToString("P1", actor)} / {Haggling.SaleMarkup.ToString("P1", actor)}. Buyback bonus/penalty: {Haggling.BuyBonus.ToString("P1", actor)} / {Haggling.BuyPenalty.ToString("P1", actor)}.\nExcluded merchandise: {Haggling.ExcludedMerchandise.Select(x => x.ToString("N0", actor)).ListToString()}. Pricing grants: {Haggling.PricingEmployees.Select(x => x.ToString("N0", actor)).ListToString()}.\n{help}");
			return true;
		}
		var option = command.PopSpeech().ToLowerInvariant();
		switch (option)
		{
			case "toggle":
				Haggling.Disabled = !Haggling.Disabled;
				break;
			case "difficulty":
				var text = command.PopSpeech();
				if (text.EqualTo("default")) Haggling.Difficulty = null;
				else if (Enum.TryParse<Difficulty>(text, true, out var difficulty) && Enum.IsDefined(difficulty)) Haggling.Difficulty = difficulty;
				else { actor.OutputHandler.Send("Specify a valid difficulty or default."); return false; }
				break;
			case "limits":
				var values = new List<decimal>();
				for (var i = 0; i < 4; i++)
				{
					if (!decimal.TryParse(command.PopSpeech().TrimEnd('%'), System.Globalization.NumberStyles.Number, actor, out var value) || value < 0 || value > (i is 0 or 3 ? 99 : 1000))
					{
						actor.OutputHandler.Send("Supply four nonnegative percentages: discount and penalty at most 99; markup and bonus at most 1000.");
						return false;
					}
					values.Add(value / 100M);
				}
				Haggling.SaleDiscount = values[0]; Haggling.SaleMarkup = values[1];
				Haggling.BuyBonus = values[2]; Haggling.BuyPenalty = values[3];
				break;
			case "exclude":
				if (!long.TryParse(command.PopSpeech(), out var id) || !Merchandises.Any(x => x.Id == id))
				{ actor.OutputHandler.Send("Specify a merchandise ID belonging to this shop."); return false; }
				if (!Haggling.ExcludedMerchandise.Remove(id)) Haggling.ExcludedMerchandise.Add(id);
				break;
			case "pricing":
				var employee = actor.TargetActor(command.PopSpeech());
				if (employee is null || !IsEmployee(employee))
				{ actor.OutputHandler.Send("Specify a present employee of this shop."); return false; }
				var employeeId = CharacterInstanceIdentityComparer.IdentityId(employee);
				if (!Haggling.PricingEmployees.Remove(employeeId)) Haggling.PricingEmployees.Add(employeeId);
				HaggleService.Audit(this, actor, $"pricing employee #{employeeId}: {Haggling.PricingEmployees.Contains(employeeId)}");
				break;
			case "audit":
				actor.OutputHandler.Send(Haggling.Audit.DefaultIfEmpty("No Haggle audit entries.").ListToString(separator: "\n", conjunction: ""));
				return true;
			default:
				actor.OutputHandler.Send(help);
				return false;
		}
		HaggleService.Audit(this, actor, $"settings {option}: disabled={Haggling.Disabled}, difficulty={Haggling.Difficulty}, limits={Haggling.SaleDiscount}/{Haggling.SaleMarkup}/{Haggling.BuyBonus}/{Haggling.BuyPenalty}, exclusions={string.Join(",", Haggling.ExcludedMerchandise)}");
		actor.OutputHandler.Send("Shop Haggle settings updated.");
		return true;
	}

	/// <summary>Deals apply first. The safe buyback ceiling considers even other customers' and volume discounts.</summary>
	private decimal ApplyHagglePrice(ICharacter? customer, IMerchandise merchandise,
		ShopDealApplicability direction, decimal dealAdjustedPrice)
	{
		if (!HaggleService.Enabled(this) || Haggling.ExcludedMerchandise.Contains(merchandise.Id)) return dealAdjustedPrice;
		var price = Math.Max(1M, dealAdjustedPrice * (1M + HaggleService.Adjustment(customer, this, merchandise, direction)));
		if (direction != ShopDealApplicability.Buy) return price;
		var saleDeals = Deals.Where(x => !x.IsExpired && x.Applicability.HasFlag(ShopDealApplicability.Sell) && x.AppliesToMerchandise(merchandise)).ToList();
		var lowestDeals = saleDeals.Where(x => x.IsCumulative).Sum(x => Math.Min(0M, x.PriceAdjustmentPercentage)) +
			saleDeals.Where(x => !x.IsCumulative).Select(x => Math.Min(0M, x.PriceAdjustmentPercentage)).DefaultIfEmpty(0M).Min();
		var lowestResale = Math.Max(1M, merchandise.EffectivePrice * Math.Max(0M, 1M + lowestDeals) * (1M - Haggling.SaleDiscount));
		// Integer merchandise totals truncate. Bound against that actual attainable pretax unit, too.
		if (merchandise.MerchandiseType != MerchandiseType.Commodity) lowestResale = Math.Truncate(lowestResale);
		var spread = Math.Clamp(Gameworld.GetStaticDecimal("HaggleMinimumSpread"), 0.000001M, 0.99M);
		var ceiling = lowestResale * (1M - spread);
		return ceiling < 1M ? 0M : Math.Min(price, ceiling);
	}
}
