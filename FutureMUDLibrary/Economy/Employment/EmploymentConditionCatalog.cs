using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Economy.Employment;

public enum EmploymentConditionCategory
{
	Environment,
	Finance,
	Inventory,
	Manual,
	Personnel,
	Time,
	Stock
}

public sealed record EmploymentConditionDefinition(
	string Key,
	EmploymentConditionCategory Category,
	EmploymentTaskConditionType ConditionType,
	string Syntax,
	EmploymentAuthoritySet RequiredAuthority,
	string Summary,
	IReadOnlyCollection<string>? Aliases = null);

public static class EmploymentConditionCatalog
{
	private static readonly EmploymentConditionDefinition[] Definitions =
	[
		Definition("manual", EmploymentConditionCategory.Manual, EmploymentTaskConditionType.ManualOrder,
			"tasks rule condition manual <key>",
			EmploymentAuthoritySet.Empty,
			"Satisfied only during an explicit manual evaluation with the matching key.",
			Aliases("order", "trigger")),
		Definition("time", EmploymentConditionCategory.Time, EmploymentTaskConditionType.TimeWindow,
			"tasks rule condition time <HH:mm> to <HH:mm>",
			EmploymentAuthoritySet.Empty,
			"Satisfied when the host's current game time is inside the window; overnight windows are supported.",
			Aliases("window", "between")),
		Definition("item", EmploymentConditionCategory.Inventory, EmploymentTaskConditionType.ItemThreshold,
			"tasks rule condition item <prototype|*item|&tag|keyword> in <here|cell id> [container <prototype|*item|&tag|keyword>] below|atleast <quantity>",
			EmploymentAuthority.ManageStockRules,
			"Satisfied by counting matching physical items at a cell, including nested contents, or inside one matching container at that cell.",
			Aliases("items", "itemcount", "inventory")),
		Definition("commodity", EmploymentConditionCategory.Inventory, EmploymentTaskConditionType.CommodityThreshold,
			"tasks rule condition commodity <material[|tag][|name=value...]> in <here|cell id> [container <prototype|*item|&tag|keyword>] below|atleast <weight>",
			EmploymentAuthority.ManageStockRules,
			"Satisfied by summing commodity weight by material, optional verified tag, and optional characteristics at a cell or inside one matching container.",
			Aliases("commodities", "commodityweight", "material")),
		Definition("stock", EmploymentConditionCategory.Stock, EmploymentTaskConditionType.StockThreshold,
			"tasks rule condition stock merch <id|name> below|atleast <quantity>",
			EmploymentAuthority.ManageStockRules,
			"Satisfied from real shop merchandise stock levels, or from a key-backed test/manual stock counter.",
			Aliases("merchandise", "stocklevel")),
		Definition("hospitalstock", EmploymentConditionCategory.Stock, EmploymentTaskConditionType.HospitalSupplyStock,
			"tasks rule condition hospitalstock consumables|tools <procedure-count> [from <shop id|name|any>] [max <amount>]",
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageDeliveryRoutes,
			"Satisfied when active hospital service supply requirements are below the configured procedure-repeat target.",
			Aliases("medicalstock", "proceduresupplies", "hospitaltools", "hospitalconsumables")),
		Definition("account", EmploymentConditionCategory.Finance, EmploymentTaskConditionType.AccountBalance,
			"tasks rule condition account cash|bank|available|key <key> below|atleast <amount>",
			EmploymentAuthority.CreateScheduledRules,
			"Satisfied from supported host finance balances, or from a key-backed test/manual account balance.",
			Aliases("balance", "finance")),
		Definition("shopaccount", EmploymentConditionCategory.Finance, EmploymentTaskConditionType.ShopAccountOwing,
			"tasks rule condition shopaccount <shop id|name> account <account id|name> owing above <amount>",
			EmploymentAuthority.CreateScheduledRules,
			"Satisfied from another shop's line-of-credit account outstanding balance.",
			Aliases("credit", "creditaccount", "owing")),
		Definition("float", EmploymentConditionCategory.Finance, EmploymentTaskConditionType.ShopFloatThreshold,
			"tasks rule condition float [register <prototype|*item|&tag|keyword>|all] below|atleast <amount>",
			EmploymentAuthority.CreateScheduledRules,
			"Satisfied from real cash piles in a permanent shop's cash register/till items.",
			Aliases("cashfloat", "registerfloat", "tillfloat")),
		Definition("tax", EmploymentConditionCategory.Finance, EmploymentTaskConditionType.TaxOwing,
			"tasks rule condition tax owing above|below <amount>",
			EmploymentAuthority.CreateScheduledRules,
			"Satisfied from supported host native outstanding-tax state.",
			Aliases("taxes", "taxowing")),
		Definition("marketprice", EmploymentConditionCategory.Finance, EmploymentTaskConditionType.MarketPrice,
			"tasks rule condition marketprice merch <id|name> effective|base|multiplier|flat above|below <amount>",
			EmploymentAuthority.CreateScheduledRules,
			"Satisfied from supported shop merchandise pricing and the host market's price multiplier or flat adjustment.",
			Aliases("price", "pricing", "market")),
		Definition("payroll", EmploymentConditionCategory.Finance, EmploymentTaskConditionType.PayrollLiability,
			"tasks rule condition payroll outstanding|amount|overdue above|below <threshold>",
			EmploymentAuthority.ManagePayroll,
			"Satisfied from accrued employment wage liabilities, total outstanding payroll amount, or maximum overdue payroll days.",
			Aliases("wages", "payables", "payrollowing")),
		Definition("staffing", EmploymentConditionCategory.Personnel, EmploymentTaskConditionType.StaffingLevel,
			"tasks rule condition staffing role <role|any> active|open|combined below|atleast <count>",
			EmploymentAuthority.CreateJobOpenings,
			"Satisfied from active employment contracts, open job openings, or their combined coverage for a role.",
			Aliases("staff", "employees", "headcount")),
		Definition("weather", EmploymentConditionCategory.Environment, EmploymentTaskConditionType.WeatherLevel,
			"tasks rule condition weather precip <rain|snow|level> begins OR weather wind <level> begins",
			EmploymentAuthoritySet.Empty,
			"Satisfied when the host location's weather controller has just changed to the selected precipitation or wind level.",
			Aliases("precipitation", "wind"))
	];

	private static readonly Dictionary<string, EmploymentConditionDefinition> Lookup =
		Definitions
			.SelectMany(x => new[] { x.Key }.Concat(x.Aliases ?? Array.Empty<string>())
				.Select(alias => (Alias: alias, Definition: x)))
			.ToDictionary(x => x.Alias, x => x.Definition, StringComparer.InvariantCultureIgnoreCase);

	public static IReadOnlyCollection<EmploymentConditionDefinition> All => Definitions;

	public static IReadOnlyCollection<EmploymentConditionCategory> Categories =>
		Definitions
			.Select(x => x.Category)
			.Distinct()
			.OrderBy(x => x)
			.ToList();

	public static EmploymentConditionDefinition? Get(string keyOrAlias)
	{
		return Lookup.GetValueOrDefault(keyOrAlias);
	}

	public static IReadOnlyCollection<EmploymentConditionDefinition> ForCategory(string category)
	{
		return Enum.TryParse<EmploymentConditionCategory>(category, true, out var parsed)
			? Definitions.Where(x => x.Category == parsed).ToList()
			: [];
	}

	private static EmploymentConditionDefinition Definition(string key, EmploymentConditionCategory category,
		EmploymentTaskConditionType conditionType, string syntax, EmploymentAuthoritySet authority, string summary,
		IReadOnlyCollection<string>? aliases = null)
	{
		return new EmploymentConditionDefinition(key, category, conditionType, syntax, authority, summary, aliases);
	}

	private static EmploymentConditionDefinition Definition(string key, EmploymentConditionCategory category,
		EmploymentTaskConditionType conditionType, string syntax, EmploymentAuthority authority, string summary,
		IReadOnlyCollection<string>? aliases = null)
	{
		return Definition(key, category, conditionType, syntax, new EmploymentAuthoritySet(authority), summary, aliases);
	}

	private static IReadOnlyCollection<string> Aliases(params string[] aliases)
	{
		return aliases;
	}
}
