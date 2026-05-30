using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Economy.Employment;

public enum EmploymentActionCategory
{
	Planning,
	Authorisation,
	Logistics,
	Purchasing,
	Production,
	Communication,
	Administration
}

public enum EmploymentActionCatalogStatus
{
	Executable,
	AuditOnlyShell,
	Deferred
}

public sealed record EmploymentActionDefinition(
	string Key,
	EmploymentActionCategory Category,
	string Syntax,
	EmploymentActionCatalogStatus Status,
	EmploymentActionStepType? StepType,
	EmploymentAuthoritySet RequiredAuthority,
	IReadOnlySet<EmploymentAICapability> RequiredCapabilities,
	bool RequiresPaymentAuthorisation,
	bool IsFinancial,
	string Summary,
	IReadOnlyCollection<string>? Aliases = null,
	string? DeferredReason = null);

public static class EmploymentActionCatalog
{
	private static readonly EmploymentActionDefinition[] Definitions =
	[
		Executable("getid", EmploymentActionCategory.Logistics,
			"tasks step getid <quantity> <prototype ids|*item ids...> from <here|cell ids...>",
			EmploymentActionStepType.GetItemsById,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(EmploymentAICapability.CanDeliverItems),
			"Collects a quantity of items matching one or more item prototype IDs.",
			Aliases("id", "prototype")),
		Executable("gettag", EmploymentActionCategory.Logistics,
			"tasks step gettag <quantity> <&tag id|&tag name> from <here|cell ids...>",
			EmploymentActionStepType.GetItemsByTag,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(EmploymentAICapability.CanDeliverItems),
			"Collects a quantity of items matching a verified item tag.",
			Aliases("tag")),
		Executable("commodity", EmploymentActionCategory.Logistics,
			"tasks step commodity <weight> <material> [tag <&tag id|&tag name>] from <here|cell ids...> [char <name>=<value> ...]",
			EmploymentActionStepType.GetCommodity,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(EmploymentAICapability.CanDeliverItems),
			"Collects a commodity weight by material, optional verified tag, and optional characteristics.",
			Aliases("material")),
		Executable("deliver", EmploymentActionCategory.Logistics,
			"tasks step deliver to <here|cell id> [container <prototype id|*item id|&tag|keyword>]",
			EmploymentActionStepType.DeliverItems,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(EmploymentAICapability.CanDeliverItems),
			"Delivers all carried task items to a destination, optionally into a container.",
			Aliases("delivery")),
		Executable("load", EmploymentActionCategory.Logistics,
			"tasks step load all into <prototype id|*item id|&tag|keyword> [at <here|cell id>]",
			EmploymentActionStepType.LoadItems,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(EmploymentAICapability.CanDeliverItems),
			"Uses inventory plans to load all carried task items into a container or accessible cargo projection.",
			Aliases("loaditems")),
		Executable("unload", EmploymentActionCategory.Logistics,
			"tasks step unload <prototype id|*item id|&tag|keyword> [at <here|cell id>]",
			EmploymentActionStepType.UnloadItems,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(EmploymentAICapability.CanDeliverItems),
			"Uses inventory plans to retrieve previously task-loaded items from a container or cargo projection.",
			Aliases("unloaditems")),
		Executable("return", EmploymentActionCategory.Logistics,
			"tasks step return container <prototype id|*item id|&tag|keyword> to <here|cell id> [container <prototype id|*item id|&tag|keyword>]",
			EmploymentActionStepType.ReturnAsset,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(EmploymentAICapability.CanDeliverItems),
			"Uses inventory plans to carry and return a reusable container; vehicle and animal return remain separate follow-ups.",
			Aliases("returncontainer")),
		Executable("vehicle", EmploymentActionCategory.Logistics,
			"tasks step vehicle cargo <vehicle id|exterior item id> <cargo id|cargo name>",
			EmploymentActionStepType.VehicleOperation,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(EmploymentAICapability.CanUseVehicles),
			"Validates and records an accessible vehicle cargo-space selection without autonomous driving.",
			Aliases("cargo")),
		Executable("move", EmploymentActionCategory.Logistics,
			"tasks step move to <here|cell id>",
			EmploymentActionStepType.MoveOrDeliver,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(EmploymentAICapability.CanDeliverItems),
			"Moves to the target location and completes there."),
		Executable("board", EmploymentActionCategory.Communication,
			"tasks step board <title> = <text>",
			EmploymentActionStepType.BoardPost,
			EmploymentAuthority.PostToHostBoard,
			Caps(EmploymentAICapability.CanPostToBoard),
			"Posts a notice to the host staff communication board.",
			Aliases("post")),
		Executable("command", EmploymentActionCategory.Communication,
			"tasks step command [at <here|cell id>] <command> [arguments...]",
			EmploymentActionStepType.Command,
			EmploymentAuthority.AssignTasks,
			Caps(EmploymentAICapability.CanExecuteCommandTask),
			"Executes an allowlisted command, optionally at a target location."),
		Executable("report", EmploymentActionCategory.Communication,
			"tasks step report <text>",
			EmploymentActionStepType.CataloguedShell,
			EmploymentAuthority.AssignTasks,
			Caps(),
			"Records a report-style action in the employment register and durable task state."),
		AuditOnly("purchase", EmploymentActionCategory.Purchasing,
			"tasks step purchase <amount> for <description>",
			EmploymentActionStepType.Purchase,
			EmploymentAuthority.ApprovePurchases,
			Caps(EmploymentAICapability.CanPurchaseCommodities),
			"Records purchase audit and ledger evidence without invoking the shop or supplier systems.",
			requiresPaymentAuthorisation: true,
			isFinancial: true),
		Executable("bankdeposit", EmploymentActionCategory.Purchasing,
			"tasks step bankdeposit <amount>",
			EmploymentActionStepType.BankDeposit,
			EmploymentAuthority.DepositBusinessCash,
			Caps(EmploymentAICapability.CanUseBankAccount, EmploymentAICapability.CanHandleCash),
			"Moves employer virtual cash into the linked native bank account and records employment audit evidence.",
			Aliases("deposit"),
			requiresPaymentAuthorisation: true,
			isFinancial: true),
		Executable("bankwithdraw", EmploymentActionCategory.Purchasing,
			"tasks step bankwithdraw <amount>",
			EmploymentActionStepType.BankWithdrawal,
			EmploymentAuthority.WithdrawBusinessCash,
			Caps(EmploymentAICapability.CanUseBankAccount, EmploymentAICapability.CanHandleCash),
			"Moves linked native bank funds into employer virtual cash and records employment audit evidence.",
			Aliases("withdraw", "bankwithdrawal"),
			requiresPaymentAuthorisation: true,
			isFinancial: true),
		AuditOnly("storepay", EmploymentActionCategory.Purchasing,
			"tasks step storepay <account> amount <amount>",
			EmploymentActionStepType.StoreAccountPayment,
			EmploymentAuthority.UseStoreAccount,
			Caps(EmploymentAICapability.CanPurchaseCommodities),
			"Records store-account payment audit and ledger evidence without mutating real store accounts.",
			Aliases("storeaccount", "accountpay"),
			requiresPaymentAuthorisation: true,
			isFinancial: true),
		AuditOnly("craft", EmploymentActionCategory.Production,
			"tasks step craft <description>",
			EmploymentActionStepType.CraftTrigger,
			EmploymentAuthority.ManageCraftRules,
			Caps(EmploymentAICapability.CanCraft),
			"Records a craft trigger without reserving inputs, using tools, or creating craft outputs."),
		Executable("authorise", EmploymentActionCategory.Authorisation,
			"tasks step authorise [<amount> for] <description>",
			EmploymentActionStepType.CataloguedShell,
			EmploymentAuthority.ApprovePurchases,
			Caps(),
			"Records a durable auditable manager authorisation note for later financial steps in the same active task.",
			Aliases("authorize"),
			isFinancial: true),
		Executable("reserve", EmploymentActionCategory.Authorisation,
			"tasks step reserve [<amount> for] <description>",
			EmploymentActionStepType.CataloguedShell,
			EmploymentAuthority.ManageStockRules,
			Caps(),
			"Creates a durable employer-funds reservation for later financial steps in the same active task.",
			isFinancial: true),
		Executable("release", EmploymentActionCategory.Authorisation,
			"tasks step release [reservation <id>|all]",
			EmploymentActionStepType.CataloguedShell,
			EmploymentAuthority.ManageStockRules,
			Caps(),
			"Releases durable employer-funds reservations for the same active task.",
			isFinancial: true),
		Executable("select", EmploymentActionCategory.Planning,
			"tasks step select <description>",
			EmploymentActionStepType.CataloguedShell,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(),
			"Records a durable source, destination, supplier, container, or vehicle selection note."),
		Executable("estimate", EmploymentActionCategory.Planning,
			"tasks step estimate <description>",
			EmploymentActionStepType.CataloguedShell,
			EmploymentAuthority.AssignTasks,
			Caps(),
			"Records a durable estimated cost, route, stock, or timing note."),
		Executable("route", EmploymentActionCategory.Planning,
			"tasks step route to <here|cell id> [description]",
			EmploymentActionStepType.CataloguedShell,
			EmploymentAuthority.ManageDeliveryRoutes,
			Caps(EmploymentAICapability.CanDeliverItems),
			"Records a durable route plan and completes only when the worker can reach the target location."),
		Deferred("transfer", EmploymentActionCategory.Purchasing,
			"tasks step transfer <from> <to> <amount>",
			EmploymentAuthority.WithdrawBusinessCash,
			"Real account transfer execution is deferred until account mutation and ledger boundaries are finalised.",
			isFinancial: true),
		Deferred("float", EmploymentActionCategory.Purchasing,
			"tasks step float <employee|container> <amount>",
			EmploymentAuthority.WithdrawBusinessCash,
			"Employee payment float handling is deferred until cash reservation and recovery flows exist.",
			isFinancial: true),
		Deferred("station", EmploymentActionCategory.Production,
			"tasks step station <reserve|use|release> ...",
			EmploymentAuthority.ManageCraftRules,
			"Real craft station/tool use is deferred until craft execution owns those mutations.",
			Caps(EmploymentAICapability.CanCraft)),
		Deferred("price", EmploymentActionCategory.Administration,
			"tasks step price <merchandise> <new price>",
			EmploymentAuthority.AdjustPrices,
			"Price adjustment remains deferred because it mutates public business state."),
		Deferred("jobopening", EmploymentActionCategory.Administration,
			"tasks step jobopening <create|close|modify> ...",
			EmploymentAuthority.CreateJobOpenings,
			"Job-opening administration remains command-controlled and is not task-executable yet."),
		Deferred("rule", EmploymentActionCategory.Administration,
			"tasks step rule <create|modify|close> ...",
			EmploymentAuthority.CreateScheduledRules,
			"Scheduled-rule administration remains command-controlled and is not task-executable yet."),
		Deferred("admintask", EmploymentActionCategory.Administration,
			"tasks step admintask <create|cancel|assign> ...",
			EmploymentAuthority.AssignTasks,
			"Task administration from inside tasks is deferred to avoid recursive task mutation."),
		Deferred("marktask", EmploymentActionCategory.Administration,
			"tasks step marktask <blocked|failed|completed> <task>",
			EmploymentAuthority.CancelTasks,
			"Task state mutation from inside tasks is deferred to keep audit and dispatcher ownership clear.")
	];

	private static readonly Dictionary<string, EmploymentActionDefinition> Lookup =
		Definitions
			.SelectMany(x => new[] { x.Key }.Concat(x.Aliases ?? Array.Empty<string>())
				.Select(alias => (Alias: alias, Definition: x)))
			.ToDictionary(x => x.Alias, x => x.Definition, StringComparer.InvariantCultureIgnoreCase);

	public static IReadOnlyCollection<EmploymentActionDefinition> All => Definitions;

	public static IReadOnlyCollection<EmploymentActionCategory> Categories =>
		Definitions
			.Select(x => x.Category)
			.Distinct()
			.OrderBy(x => x)
			.ToList();

	public static EmploymentActionDefinition? Get(string keyOrAlias)
	{
		return Lookup.GetValueOrDefault(keyOrAlias);
	}

	public static IReadOnlyCollection<EmploymentActionDefinition> ForCategory(string categoryOrAlias)
	{
		if (!Enum.TryParse<EmploymentActionCategory>(categoryOrAlias, true, out var category))
		{
			return Array.Empty<EmploymentActionDefinition>();
		}

		return Definitions
		       .Where(x => x.Category == category)
		       .OrderBy(x => x.Key)
		       .ToList();
	}

	public static EmploymentActionDefinition? ForStepType(EmploymentActionStepType stepType)
	{
		return Definitions.FirstOrDefault(x => x.StepType == stepType && x.Status != EmploymentActionCatalogStatus.Deferred);
	}

	private static EmploymentActionDefinition Executable(string key, EmploymentActionCategory category, string syntax,
		EmploymentActionStepType stepType, EmploymentAuthority authority,
		IReadOnlySet<EmploymentAICapability> requiredCapabilities, string summary,
		IReadOnlyCollection<string>? aliases = null, bool requiresPaymentAuthorisation = false,
		bool isFinancial = false)
	{
		return new EmploymentActionDefinition(
			key,
			category,
			syntax,
			EmploymentActionCatalogStatus.Executable,
			stepType,
			authority,
			requiredCapabilities,
			requiresPaymentAuthorisation,
			isFinancial,
			summary,
			aliases);
	}

	private static IReadOnlySet<EmploymentAICapability> Caps(params EmploymentAICapability[] capabilities)
	{
		return new HashSet<EmploymentAICapability>(capabilities);
	}

	private static IReadOnlyCollection<string> Aliases(params string[] aliases)
	{
		return aliases;
	}

	private static EmploymentActionDefinition AuditOnly(string key, EmploymentActionCategory category, string syntax,
		EmploymentActionStepType stepType, EmploymentAuthority authority,
		IReadOnlySet<EmploymentAICapability> requiredCapabilities, string summary,
		IReadOnlyCollection<string>? aliases = null, bool requiresPaymentAuthorisation = false, bool isFinancial = false)
	{
		return new EmploymentActionDefinition(
			key,
			category,
			syntax,
			EmploymentActionCatalogStatus.AuditOnlyShell,
			stepType,
			authority,
			requiredCapabilities,
			requiresPaymentAuthorisation,
			isFinancial,
			summary,
			aliases);
	}

	private static EmploymentActionDefinition Deferred(string key, EmploymentActionCategory category, string syntax,
		EmploymentAuthority authority, string deferredReason,
		IReadOnlySet<EmploymentAICapability>? requiredCapabilities = null, bool isFinancial = false)
	{
		return new EmploymentActionDefinition(
			key,
			category,
			syntax,
			EmploymentActionCatalogStatus.Deferred,
			null,
			authority,
			requiredCapabilities ?? new HashSet<EmploymentAICapability>(),
			false,
			isFinancial,
			deferredReason,
			null,
			deferredReason);
	}
}
