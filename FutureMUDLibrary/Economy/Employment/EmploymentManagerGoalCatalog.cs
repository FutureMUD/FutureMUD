using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Economy.Employment;

public enum EmploymentManagerGoalCategory
{
	Administration,
	Finance,
	Production,
	Stock
}

public sealed record EmploymentManagerGoalDefinition(
	string Key,
	EmploymentManagerGoalCategory Category,
	ManagerGoalType GoalType,
	string Syntax,
	EmploymentAuthoritySet DefaultRequiredAuthority,
	string Summary,
	IReadOnlyCollection<string> SuggestedConditions,
	IReadOnlyCollection<string> SuggestedActions,
	IReadOnlyCollection<string>? Aliases = null);

public static class EmploymentManagerGoalCatalog
{
	private static readonly EmploymentManagerGoalDefinition[] Definitions =
	[
		Definition("cashfloatlow", EmploymentManagerGoalCategory.Finance,
			ManagerGoalType.MaintainMinimumPhysicalCashFloat,
			"goals draft new cashfloatlow <description>",
			EmploymentAuthority.WithdrawBusinessCash,
			"Restores physical register or till cash when it falls below a configured minimum by creating float-fill, issue, or settlement work.",
			Examples(
				"goals condition float all below <minimum amount>",
				"goals condition float register <selector> below <minimum amount>"),
			Examples(
				"goals step authorise <amount> for low cash float adjustment",
				"goals step float fill <amount> [register <selector>]",
				"goals step physicalfloat issue|settle ..."),
			Aliases("cashfloat", "cash", "float", "lowcash", "lowfloat", "cashfloatmin", "physicalcashfloatlow")),
		Definition("cashfloathigh", EmploymentManagerGoalCategory.Finance,
			ManagerGoalType.MaintainMaximumPhysicalCashFloat,
			"goals draft new cashfloathigh <description>",
			EmploymentAuthority.WithdrawBusinessCash,
			"Reduces physical register or till cash when it rises above a configured maximum by creating skim, return, deposit, or settlement work.",
			Examples(
				"goals condition float all atleast <maximum amount>",
				"goals condition float register <selector> atleast <maximum amount>"),
			Examples(
				"goals step authorise <amount> for high cash float adjustment",
				"goals step float skim <amount> [register <selector>]",
				"goals step physicalfloat return|settle ...",
				"goals step bankdeposit <amount>"),
			Aliases("highcash", "highfloat", "cashfloatmax", "physicalcashfloathigh")),
		Definition("taxes", EmploymentManagerGoalCategory.Finance,
			ManagerGoalType.PayTaxes,
			"goals draft new taxes <description>",
			EmploymentAuthority.PayTaxes,
			"Monitors supported host tax liabilities and creates authorised tax-payment work when configured thresholds are reached.",
			Examples(
				"goals condition tax owing above <amount>"),
			Examples(
				"goals step authorise <amount> for tax payment",
				"goals step paytax <amount|all>"),
			Aliases("tax", "paytaxes", "taxpayment")),
		Definition("accounts", EmploymentManagerGoalCategory.Finance,
			ManagerGoalType.PayShopAccountsOwing,
			"goals draft new accounts <description>",
			EmploymentAuthority.UseStoreAccount,
			"Monitors other shop line-of-credit balances and creates payment work when owing balances exceed configured thresholds.",
			Examples(
				"goals condition shopaccount <shop id|name> account <account id|name> owing above <amount>"),
			Examples(
				"goals step authorise <amount> for store account payment",
				"goals step storepay <shop id|name> account <account id|name> amount <amount>"),
			Aliases("shopaccounts", "storeaccounts", "payaccounts", "owing")),
		Definition("craftstock", EmploymentManagerGoalCategory.Stock,
			ManagerGoalType.MaintainCraftedMerchandiseStock,
			"goals draft new craftstock <description>",
			EmploymentAuthority.ManageStockRules | EmploymentAuthority.ManageCraftRules,
			"Maintains shop stock levels for crafted merchandise by creating station and craft-trigger work when stock falls below target.",
			Examples(
				"goals condition stock merch <id|name> below <quantity>",
				"goals condition item <prototype|*item|&tag|keyword> in <here|cell id> below <quantity>"),
			Examples(
				"goals step station <craft station selector|here>",
				"goals step craft <craft id|craft name>",
				"goals step deliver to <here|cell id> [container <selector>]"),
			Aliases("craftedstock", "merchandisecraft", "stockcraft")),
		Definition("craftmaterials", EmploymentManagerGoalCategory.Production,
			ManagerGoalType.MaintainCraftMaterialSupply,
			"goals draft new craftmaterials <description>",
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageDeliveryRoutes,
			"Maintains materials required for crafting merchandise by creating buy, retrieve, load, unload, or delivery tasks.",
			Examples(
				"goals condition commodity <material[|tag][|name=value...]> in <here|cell id> below <weight>",
				"goals condition item <prototype|*item|&tag|keyword> in <here|cell id> below <quantity>"),
			Examples(
				"goals step authorise <amount> for craft materials",
				"goals step purchase <quantity|weight> <merchandise|item|commodity> from <shop|any> [max <amount>]",
				"goals step getid|gettag|commodity ... from <here|cell ids...>",
				"goals step deliver to <here|cell id>"),
			Aliases("materials", "supplies", "buymaterials", "retrievematerials")),
		Definition("hospitalconsumables", EmploymentManagerGoalCategory.Stock,
			ManagerGoalType.MaintainHospitalConsumableStock,
			"goals draft new hospitalconsumables <description>",
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageDeliveryRoutes,
			"Maintains hospital supply-room consumables required by active services by purchasing deficits for a configured number of procedure repeats.",
			Examples(
				"goals condition hospitalstock consumables 30 [from <shop|any>] [max <amount>]"),
			Examples(
				"Generated automatically from active service consumable requirements"),
			Aliases("medicalconsumables", "hospitalstock", "medstock", "procedurestock")),
		Definition("hospitaltools", EmploymentManagerGoalCategory.Stock,
			ManagerGoalType.MaintainHospitalReusableEquipmentStock,
			"goals draft new hospitaltools <description>",
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageDeliveryRoutes,
			"Maintains reusable hospital tools for configured service repeats, counting supply-room stock, theatre stock, and tools carried by medical staff.",
			Examples(
				"goals condition hospitalstock tools 5 [from <shop|any>] [max <amount>]"),
			Examples(
				"Generated automatically from active service reusable-tool requirements"),
			Aliases("medicaltools", "hospitalreusables", "proceduretools")),
		Definition("payroll", EmploymentManagerGoalCategory.Finance,
			ManagerGoalType.KeepEmploymentPayrollCurrent,
			"goals draft new payroll <description>",
			EmploymentAuthority.ManagePayroll,
			"Keeps accrued employment wage liabilities current by creating authorised payroll settlement work when payables or overdue days exceed configured thresholds.",
			Examples(
				"goals condition payroll outstanding above <count>",
				"goals condition payroll amount above <amount>",
				"goals condition payroll overdue above <days>"),
			Examples(
				"goals step payroll settle all [reason]",
				"goals step board Payroll = Review wage liabilities."),
			Aliases("wages", "payables", "payrollsettle")),
		Definition("bankbalancelow", EmploymentManagerGoalCategory.Finance,
			ManagerGoalType.MaintainMinimumBusinessFunds,
			"goals draft new bankbalancelow <description>",
			EmploymentAuthority.WithdrawBusinessCash,
			"Restores employer virtual cash or available funds when supported host finance balances fall below a configured minimum.",
			Examples(
				"goals condition account cash below <amount>",
				"goals condition account available below <amount>"),
			Examples(
				"goals step authorise <amount> for business cash reserve",
				"goals step bankwithdraw <amount>",
				"goals step board Finance = Restore business cash reserve."),
			Aliases("cashreserve", "cashlow", "banklow", "businessfundslow")),
		Definition("bankbalancehigh", EmploymentManagerGoalCategory.Finance,
			ManagerGoalType.MaintainMaximumBusinessFunds,
			"goals draft new bankbalancehigh <description>",
			EmploymentAuthority.DepositBusinessCash,
			"Deposits excess employer virtual cash when supported host finance balances rise above a configured maximum.",
			Examples(
				"goals condition account cash atleast <amount>",
				"goals condition account available atleast <amount>"),
			Examples(
				"goals step authorise <amount> for business cash deposit",
				"goals step bankdeposit <amount>",
				"goals step board Finance = Deposit excess business cash."),
			Aliases("cashhigh", "bankhigh", "businessfundshigh")),
		Definition("pricemargin", EmploymentManagerGoalCategory.Administration,
			ManagerGoalType.AdjustPricesForProfit,
			"goals draft new pricemargin <description>",
			EmploymentAuthority.AdjustPrices,
			"Maintains profitability targets by creating executable merchandise base-price changes or native shop deal actions.",
			Examples(
				"goals condition marketprice merch <id|name> effective below <amount>",
				"goals condition marketprice merch <id|name> multiplier above <amount>"),
			Examples(
				"goals step price merch <id|name> <amount>",
				"goals step sale create seasonal sale target all adjustment -10% expires 7 days",
				"goals step board Pricing = Review listed merchandise prices."),
			Aliases("pricing", "prices", "margin", "profitmargin")),
		Definition("staffing", EmploymentManagerGoalCategory.Administration,
			ManagerGoalType.MaintainStaffingLevels,
			"goals draft new staffing <description>",
			EmploymentAuthority.CreateJobOpenings,
			"Maintains staffing levels by creating executable job-opening administration steps when roles have too few active employees or open positions.",
			Examples(
				"goals condition staffing role employee active below <count>",
				"goals condition staffing role crafter combined below <count>"),
			Examples(
				"goals step jobopening create role employee positions <count> pay fixed <amount> hourly",
				"goals step jobopening create role crafter positions <count> pay fixed <amount> hourly authority default",
				"goals step board Staffing = A role needs hiring review."),
			Aliases("staff", "headcount", "employees", "openings"))
	];

	private static readonly Dictionary<string, EmploymentManagerGoalDefinition> Lookup =
		Definitions
			.SelectMany(x => new[] { x.Key }.Concat(x.Aliases ?? Array.Empty<string>())
				.Select(alias => (Alias: alias, Definition: x)))
			.ToDictionary(x => x.Alias, x => x.Definition, StringComparer.InvariantCultureIgnoreCase);

	public static IReadOnlyCollection<EmploymentManagerGoalDefinition> All => Definitions;

	public static IReadOnlyCollection<EmploymentManagerGoalCategory> Categories =>
		Definitions
			.Select(x => x.Category)
			.Distinct()
			.OrderBy(x => x)
			.ToList();

	public static EmploymentManagerGoalDefinition? Get(string keyOrAlias)
	{
		return Lookup.GetValueOrDefault(keyOrAlias);
	}

	public static EmploymentManagerGoalDefinition? ForGoalType(ManagerGoalType goalType)
	{
		return Definitions.FirstOrDefault(x => x.GoalType == goalType);
	}

	public static IReadOnlyCollection<EmploymentManagerGoalDefinition> ForCategory(string categoryOrAlias)
	{
		return Enum.TryParse<EmploymentManagerGoalCategory>(categoryOrAlias, true, out var category)
			? Definitions
			  .Where(x => x.Category == category)
			  .OrderBy(x => x.Key)
			  .ToList()
			: [];
	}

	private static EmploymentManagerGoalDefinition Definition(string key, EmploymentManagerGoalCategory category,
		ManagerGoalType goalType, string syntax, EmploymentAuthority authority, string summary,
		IReadOnlyCollection<string> suggestedConditions, IReadOnlyCollection<string> suggestedActions,
		IReadOnlyCollection<string>? aliases = null)
	{
		return new EmploymentManagerGoalDefinition(key, category, goalType, syntax,
			new EmploymentAuthoritySet(authority), summary, suggestedConditions, suggestedActions, aliases);
	}

	private static IReadOnlyCollection<string> Examples(params string[] examples)
	{
		return examples;
	}

	private static IReadOnlyCollection<string> Aliases(params string[] aliases)
	{
		return aliases;
	}
}
