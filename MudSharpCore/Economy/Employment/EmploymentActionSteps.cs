using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Markets;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using MudSharp.Vehicles;

#nullable enable

namespace MudSharp.Economy.Employment;

internal static class EmploymentItemSelectorResolver
{
	public static IGameItem? Resolve(IEmploymentTaskContext context, ICharacter actor,
		EmploymentItemSelector? selector, ICell? location, bool includeCarried)
	{
		if (selector is null)
		{
			return null;
		}

		var candidates = CandidateItems(context, actor, location, includeCarried)
		                 .DistinctBy(x => x.Id)
		                 .ToList();
		return selector.Kind switch
		{
			EmploymentItemSelectorKind.PrototypeId =>
				candidates.FirstOrDefault(x => x.Prototype.Id == selector.Id),
			EmploymentItemSelectorKind.ItemId =>
				ResolveSpecificItem(actor, selector, candidates, location, includeCarried),
			EmploymentItemSelectorKind.Keyword =>
				ResolveKeyword(actor, selector, candidates, location, includeCarried),
			EmploymentItemSelectorKind.Tag =>
				candidates.FirstOrDefault(x => !string.IsNullOrWhiteSpace(selector.Text) &&
				                                context.ItemHasTag(x, selector.Text)),
			_ => null
		};
	}

	public static string Describe(EmploymentItemSelector? selector)
	{
		if (selector is null)
		{
			return "nothing";
		}

		return selector.Kind switch
		{
			EmploymentItemSelectorKind.PrototypeId => $"item prototype #{selector.Id?.ToString("F0") ?? "?"}",
			EmploymentItemSelectorKind.ItemId => $"item #{selector.Id?.ToString("F0") ?? "?"}",
			EmploymentItemSelectorKind.Keyword => $"item keyword {selector.Text ?? "?"}",
			EmploymentItemSelectorKind.Tag => $"item tag {selector.Text ?? "?"}",
			_ => "unknown item selector"
		};
	}

	private static IGameItem? ResolveKeyword(ICharacter actor, EmploymentItemSelector selector,
		IReadOnlyCollection<IGameItem> candidates, ICell? location, bool includeCarried)
	{
		if (selector.Id.HasValue)
		{
			var specific = ResolveSpecificItem(actor, selector, candidates, location, includeCarried);
			if (specific is not null)
			{
				return specific;
			}
		}

		return string.IsNullOrWhiteSpace(selector.Text)
			? null
			: candidates.GetFromItemListByKeyword(selector.Text, actor);
	}

	private static IGameItem? ResolveSpecificItem(ICharacter actor, EmploymentItemSelector selector,
		IReadOnlyCollection<IGameItem> candidates, ICell? location, bool includeCarried)
	{
		if (!selector.Id.HasValue)
		{
			return null;
		}

		var item = selector.Item ?? actor.Gameworld?.TryGetItem(selector.Id.Value, true);
		if (item is null)
		{
			return null;
		}

		if (candidates.Any(x => x.Id == item.Id))
		{
			return item;
		}

		if (location is null)
		{
			return item;
		}

		if (item.TrueLocations.Any(x => x.Id == location.Id))
		{
			return item;
		}

		return includeCarried && ActorCarriesItem(actor, item) ? item : null;
	}

	private static IEnumerable<IGameItem> CandidateItems(IEmploymentTaskContext context, ICharacter actor,
		ICell? location, bool includeCarried)
	{
		if (includeCarried)
		{
			foreach (var item in context.CarriedTaskItems(actor).Concat(actor.Inventory))
			{
				yield return item;
			}
		}

		var targetLocation = location ?? actor.Location;
		if (targetLocation is null)
		{
			yield break;
		}

		foreach (var item in context.AvailableItems(targetLocation))
		{
			yield return item;
		}
	}

	private static bool ActorCarriesItem(ICharacter actor, IGameItem item)
	{
		return item.InInventoryOf == actor.Body || actor.Inventory.Any(x => x.Id == item.Id);
	}
}

internal static class EmploymentActionStepOperationalStateBuilder
{
	public static EmploymentActionStepOperationalState CollectedTaskItemCustody(
		IEmploymentTaskContext context,
		ICharacter actor,
		IReadOnlyCollection<IGameItem> selectedItems,
		IReadOnlySet<long> previouslyCarriedItemIds)
	{
		var selectedItemIds = selectedItems.Select(x => x.Id).ToHashSet();
		var actualCarried = context.CarriedTaskItems(actor)
		                           .Where(x => !previouslyCarriedItemIds.Contains(x.Id))
		                           .DistinctBy(x => x.Id)
		                           .ToList();
		if (!actualCarried.Any())
		{
			actualCarried = context.CarriedTaskItems(actor)
			                       .Where(x => selectedItemIds.Contains(x.Id))
			                       .DistinctBy(x => x.Id)
			                       .ToList();
		}

		if (!actualCarried.Any())
		{
			actualCarried = selectedItems.DistinctBy(x => x.Id).ToList();
		}

		var transportBundleIds = actualCarried
		                         .Where(x => !selectedItemIds.Contains(x.Id))
		                         .Select(x => x.Id)
		                         .ToList();
		return new EmploymentActionStepOperationalState(
			SelectedResources: EmploymentTaskContext.FormatTaskItemCustody("collect", actor.Id, actualCarried,
				transportBundleIds));
	}
}

public abstract class EmploymentActionStepBase : IEmploymentActionStep
{
	private readonly HashSet<EmploymentAICapability> _requiredCapabilities;

	protected EmploymentActionStepBase(EmploymentActionStepType stepType, EmploymentAuthoritySet requiredAuthority,
		IEnumerable<EmploymentAICapability> requiredCapabilities, bool requiresPaymentAuthorisation,
		bool isFinancialStep)
	{
		StepType = stepType;
		RequiredAuthority = requiredAuthority;
		_requiredCapabilities = new HashSet<EmploymentAICapability>(requiredCapabilities);
		RequiresPaymentAuthorisation = requiresPaymentAuthorisation;
		IsFinancialStep = isFinancialStep;
	}

	public EmploymentActionStepType StepType { get; }
	public EmploymentAuthoritySet RequiredAuthority { get; }
	public IReadOnlySet<EmploymentAICapability> RequiredCapabilities => _requiredCapabilities;
	public bool RequiresPaymentAuthorisation { get; }
	public bool IsFinancialStep { get; }

	public virtual bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (actor is null)
		{
			reason = "There is no employee assigned to this action step.";
			return false;
		}

		if (!context.Employer.HasAuthority(actor, RequiredAuthority.Authorities))
		{
			reason = $"{actor.HowSeen(actor, colour: false)} lacks the authority required to perform {StepType}.";
			return false;
		}

		if (RequiresPaymentAuthorisation && !context.PaymentAuthorised(this))
		{
			reason = $"{StepType} requires an auditable payment authorisation.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public abstract EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor);
}

public sealed class PurchaseActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public PurchaseActionStep(string purchaseDescription, MoneyAmount amount, string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.Purchase,
			EmploymentAuthority.ApprovePurchases,
			new[] { EmploymentAICapability.CanPurchaseCommodities },
			true,
			true)
	{
		TargetKind = EmploymentPurchaseTargetKind.Merchandise;
		PurchaseDescription = purchaseDescription;
		Amount = amount;
		ExistingFinancialRecord = existingFinancialRecord;
	}

	public PurchaseActionStep(int quantity, string merchandiseSelector, string supplierSelector, ICurrency currency,
		MoneyAmount? maximumAmount,
		string? keywordFilter = null, string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.Purchase,
			EmploymentAuthority.ApprovePurchases,
			new[] { EmploymentAICapability.CanPurchaseCommodities },
			true,
			true)
	{
		TargetKind = EmploymentPurchaseTargetKind.Merchandise;
		Quantity = quantity;
		MerchandiseSelector = merchandiseSelector.Trim();
		SupplierSelector = string.IsNullOrWhiteSpace(supplierSelector) ? "any" : supplierSelector.Trim();
		MaximumAmount = maximumAmount;
		KeywordFilter = keywordFilter;
		ExistingFinancialRecord = existingFinancialRecord;
		PurchaseDescription =
			$"buy {quantity:N0}x {MerchandiseSelector} from {SupplierSelector}{(maximumAmount is null ? string.Empty : $" up to {maximumAmount.Currency.Describe(maximumAmount.Amount, CurrencyDescriptionPatternType.ShortDecimal)}")}";
		Amount = maximumAmount ?? new MoneyAmount(currency, 0.0M);
	}

	public PurchaseActionStep(int quantity, EmploymentItemSelector itemSelector, string supplierSelector,
		ICurrency currency, MoneyAmount? maximumAmount, string? keywordFilter = null,
		string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.Purchase,
			EmploymentAuthority.ApprovePurchases,
			new[] { EmploymentAICapability.CanPurchaseCommodities },
			true,
			true)
	{
		TargetKind = EmploymentPurchaseTargetKind.Item;
		Quantity = quantity;
		ItemSelector = itemSelector;
		SupplierSelector = string.IsNullOrWhiteSpace(supplierSelector) ? "any" : supplierSelector.Trim();
		MaximumAmount = maximumAmount;
		KeywordFilter = keywordFilter;
		ExistingFinancialRecord = existingFinancialRecord;
		PurchaseDescription =
			$"buy {quantity:N0}x {EmploymentItemSelectorResolver.Describe(itemSelector)} from {SupplierSelector}{(maximumAmount is null ? string.Empty : $" up to {maximumAmount.Currency.Describe(maximumAmount.Amount, CurrencyDescriptionPatternType.ShortDecimal)}")}";
		Amount = maximumAmount ?? new MoneyAmount(currency, 0.0M);
	}

	public PurchaseActionStep(double commodityWeight, string commodityDescriptor, string supplierSelector,
		ICurrency currency, MoneyAmount? maximumAmount, string? keywordFilter = null,
		string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.Purchase,
			EmploymentAuthority.ApprovePurchases,
			new[] { EmploymentAICapability.CanPurchaseCommodities },
			true,
			true)
	{
		TargetKind = EmploymentPurchaseTargetKind.Commodity;
		CommodityWeight = commodityWeight;
		CommodityDescriptor = commodityDescriptor.Trim();
		SupplierSelector = string.IsNullOrWhiteSpace(supplierSelector) ? "any" : supplierSelector.Trim();
		MaximumAmount = maximumAmount;
		KeywordFilter = keywordFilter;
		ExistingFinancialRecord = existingFinancialRecord;
		PurchaseDescription =
			$"buy {commodityWeight:N2} weight of commodity {CommodityDescriptor} from {SupplierSelector}{(maximumAmount is null ? string.Empty : $" up to {maximumAmount.Currency.Describe(maximumAmount.Amount, CurrencyDescriptionPatternType.ShortDecimal)}")}";
		Amount = maximumAmount ?? new MoneyAmount(currency, 0.0M);
	}

	public EmploymentPurchaseTargetKind TargetKind { get; }
	public string PurchaseDescription { get; }
	public MoneyAmount Amount { get; }
	public int? Quantity { get; }
	public string? MerchandiseSelector { get; }
	public EmploymentItemSelector? ItemSelector { get; }
	public double? CommodityWeight { get; }
	public string? CommodityDescriptor { get; }
	public string? SupplierSelector { get; }
	public MoneyAmount? MaximumAmount { get; }
	public string? KeywordFilter { get; }
	public string? ExistingFinancialRecord { get; }

	public bool IsExecutablePurchase =>
		(TargetKind == EmploymentPurchaseTargetKind.Merchandise && Quantity.HasValue &&
		 !string.IsNullOrWhiteSpace(MerchandiseSelector)) ||
		(TargetKind == EmploymentPurchaseTargetKind.Item && Quantity.HasValue && ItemSelector is not null) ||
		(TargetKind == EmploymentPurchaseTargetKind.Commodity && CommodityWeight.HasValue &&
		 !string.IsNullOrWhiteSpace(CommodityDescriptor));

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return !IsExecutablePurchase || context.CanPurchase(this, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (IsExecutablePurchase)
		{
			if (!context.TryPurchase(actor, this, out var reason, out var operationalState))
			{
				return EmploymentActionStepResult.Blocked(reason);
			}

			if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
			{
				context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, MaximumAmount,
					$"Reused existing financial record {ExistingFinancialRecord} for purchase.");
			}

			return new EmploymentActionStepResult(true, $"Completed purchase: {PurchaseDescription}.", true,
				operationalState);
		}

		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Used payment authorisation for purchase: {PurchaseDescription}.");
		context.RecordLedger(EmploymentLedgerEntryType.Purchase, actor, Amount, PurchaseDescription);
		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, Amount,
				$"Reused existing financial record {ExistingFinancialRecord} for purchase.");
		}

		return EmploymentActionStepResult.CompletedResult($"Recorded audit-only purchase for {PurchaseDescription}.");
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return IsExecutablePurchase ? EmploymentFinanceService.PurchaseLocationHints(context, actor, this) : [];
	}
}

public sealed class MovementDeliveryActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public MovementDeliveryActionStep(string deliveryDescription, ICell? destination = null)
		: base(
			EmploymentActionStepType.MoveOrDeliver,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		DeliveryDescription = deliveryDescription;
		Destination = destination;
	}

	public string DeliveryDescription { get; }
	public ICell? Destination { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!context.CanPath(actor, Destination))
		{
			reason = "The assigned employee cannot path to the delivery destination.";
			return false;
		}

		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		return EmploymentActionStepResult.CompletedResult($"Completed delivery: {DeliveryDescription}.");
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return Destination is null ? [] : [Destination];
	}
}

public sealed class CraftTriggerActionStep : EmploymentActionStepBase
{
	public CraftTriggerActionStep(string craftDescription, string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.CraftTrigger,
			EmploymentAuthority.ManageCraftRules,
			new[] { EmploymentAICapability.CanCraft },
			false,
			false)
	{
		CraftDescription = craftDescription;
		ExistingFinancialRecord = existingFinancialRecord;
	}

	public string CraftDescription { get; }
	public string? ExistingFinancialRecord { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return context.CanStartCraft(CraftDescription, actor, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!context.TryStartCraft(actor, CraftDescription, out var reason, out var operationalState))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, null,
				$"Reused existing material-cost record {ExistingFinancialRecord} for craft trigger.");
		}

		var completed = operationalState.OperationalPayload?.Contains("craft-status=inprogress",
			StringComparison.InvariantCultureIgnoreCase) != true;
		return new EmploymentActionStepResult(true,
			completed ? $"Completed craft {CraftDescription}." : $"Started or resumed craft {CraftDescription}.",
			completed,
			operationalState);
	}
}

public sealed class CraftStationActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public CraftStationActionStep(string stationSelector)
		: base(
			EmploymentActionStepType.CraftStation,
			EmploymentAuthority.ManageCraftRules,
			new[] { EmploymentAICapability.CanCraft },
			false,
			false)
	{
		StationSelector = string.IsNullOrWhiteSpace(stationSelector) ? "here" : stationSelector.Trim();
	}

	public string StationSelector { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return context.CanUseCraftStation(actor, StationSelector, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		return context.TryUseCraftStation(actor, StationSelector, out var reason, out var operationalState)
			? new EmploymentActionStepResult(true, $"Validated craft station {StationSelector}.", true,
				operationalState)
			: EmploymentActionStepResult.Blocked(reason);
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		if (StationSelector.EqualTo("here"))
		{
			return [];
		}

		return long.TryParse(StationSelector, out var id) && actor.Gameworld.Cells.Get(id) is { } cell
			? [cell]
			: [];
	}
}

public sealed class CommandActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public CommandActionStep(string commandName, string commandArguments, ICell? executionLocation = null)
		: base(
			EmploymentActionStepType.Command,
			EmploymentAuthority.AssignTasks,
			new[] { EmploymentAICapability.CanExecuteCommandTask },
			false,
			false)
	{
		CommandName = commandName;
		CommandArguments = commandArguments;
		ExecutionLocation = executionLocation;
	}

	public string CommandName { get; }
	public string CommandArguments { get; }
	public ICell? ExecutionLocation { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!context.CommandAllowed(CommandName))
		{
			reason = $"Command {CommandName} is not allowlisted for employment task execution.";
			return false;
		}

		if (!context.CanPath(actor, ExecutionLocation))
		{
			reason = "The assigned employee cannot path to the command execution location.";
			return false;
		}

		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		context.RecordRegister(EmploymentRegisterEntryType.CommandExecuted, actor,
			$"Executed allowlisted command {CommandName} {CommandArguments}.");
		return EmploymentActionStepResult.CompletedResult($"Executed command {CommandName}.");
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return ExecutionLocation is null ? [] : [ExecutionLocation];
	}
}

public sealed class BankDepositActionStep : EmploymentActionStepBase
{
	public BankDepositActionStep(MoneyAmount amount, string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.BankDeposit,
			EmploymentAuthority.DepositBusinessCash,
			new[] { EmploymentAICapability.CanUseBankAccount, EmploymentAICapability.CanHandleCash },
			true,
			true)
	{
		Amount = amount;
		ExistingFinancialRecord = existingFinancialRecord;
	}

	public MoneyAmount Amount { get; }
	public string? ExistingFinancialRecord { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return context.CanBankDeposit(Amount, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!context.TryBankDeposit(actor, Amount, out var reason, out var operationalState))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, Amount,
				$"Reused existing bank deposit record {ExistingFinancialRecord}.");
		}

		return new EmploymentActionStepResult(true, "Moved employer virtual cash into the linked bank account.", true,
			operationalState);
	}
}

public sealed class BankWithdrawalActionStep : EmploymentActionStepBase
{
	public BankWithdrawalActionStep(MoneyAmount amount, string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.BankWithdrawal,
			EmploymentAuthority.WithdrawBusinessCash,
			new[] { EmploymentAICapability.CanUseBankAccount, EmploymentAICapability.CanHandleCash },
			true,
			true)
	{
		Amount = amount;
		ExistingFinancialRecord = existingFinancialRecord;
	}

	public MoneyAmount Amount { get; }
	public string? ExistingFinancialRecord { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return context.CanBankWithdrawal(Amount, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!context.TryBankWithdrawal(actor, Amount, out var reason, out var operationalState))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, Amount,
				$"Reused existing bank withdrawal record {ExistingFinancialRecord}.");
		}

		return new EmploymentActionStepResult(true, "Moved employer bank funds into virtual cash.", true,
			operationalState);
	}
}

public sealed class StoreAccountPaymentActionStep : EmploymentActionStepBase
{
	public StoreAccountPaymentActionStep(string accountName, MoneyAmount amount, string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.StoreAccountPayment,
			EmploymentAuthority.UseStoreAccount,
			new[] { EmploymentAICapability.CanPurchaseCommodities },
			true,
			true)
	{
		AccountName = accountName;
		Amount = amount;
		ExistingFinancialRecord = existingFinancialRecord;
	}

	public string AccountName { get; }
	public MoneyAmount Amount { get; }
	public string? ExistingFinancialRecord { get; }
	public bool IsExecutableStorePayment => AccountName.StartsWith("shopaccount:v1|", StringComparison.InvariantCultureIgnoreCase);

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (IsExecutableStorePayment)
		{
			return context.CanStoreAccountPayment(AccountName, Amount, out reason);
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!IsExecutableStorePayment)
		{
			context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
				$"Used payment authorisation for store account {AccountName}.");
			context.RecordLedger(EmploymentLedgerEntryType.StoreAccountPayment, actor, Amount,
				$"Store account payment for {AccountName}.");
			if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
			{
				context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, Amount,
					$"Reused existing store account payment record {ExistingFinancialRecord}.");
			}

			return EmploymentActionStepResult.CompletedResult($"Recorded legacy store account payment audit for {AccountName}.");
		}

		if (!context.TryStoreAccountPayment(actor, AccountName, Amount, out var reason, out var operationalState))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, Amount,
				$"Reused existing store account payment record {ExistingFinancialRecord}.");
		}

		return new EmploymentActionStepResult(true, $"Paid store account {AccountName}.", true, operationalState);
	}
}

public sealed class TaxPaymentActionStep : EmploymentActionStepBase
{
	public TaxPaymentActionStep(MoneyAmount? maximumAmount)
		: base(
			EmploymentActionStepType.TaxPayment,
			EmploymentAuthority.PayTaxes,
			new[] { EmploymentAICapability.CanUseBankAccount },
			true,
			true)
	{
		MaximumAmount = maximumAmount;
	}

	public MoneyAmount? MaximumAmount { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return context.CanPayTaxes(MaximumAmount, out reason, out _);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		return context.TryPayTaxes(actor, MaximumAmount, out var reason, out var operationalState)
			? new EmploymentActionStepResult(true, "Paid supported host taxes.", true, operationalState)
			: EmploymentActionStepResult.Blocked(reason);
	}
}

public sealed class PayrollSettlementActionStep : EmploymentActionStepBase
{
	public PayrollSettlementActionStep(string selector, string? reason = null)
		: base(
			EmploymentActionStepType.PayrollSettlement,
			EmploymentAuthority.ManagePayroll,
			new[] { EmploymentAICapability.CanUseBankAccount, EmploymentAICapability.CanHandleCash },
			false,
			true)
	{
		Selector = string.IsNullOrWhiteSpace(selector) ? "all" : selector.Trim();
		Reason = string.IsNullOrWhiteSpace(reason) ? "Employment task payroll settlement." : reason.Trim();
	}

	public string Selector { get; }
	public string Reason { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		context.Employer.Payroll.EvaluatePayroll();
		var targets = SelectPayables(context.Employer, Selector).ToList();
		if (!targets.Any())
		{
			reason = $"There are no outstanding employment payables matching {Selector}.";
			return false;
		}

		foreach (var amount in targets
			         .Where(x => x.Status == EmploymentPayableStatus.Accrued)
			         .GroupBy(x => x.Amount.Currency.Id)
			         .Select(x => new MoneyAmount(x.First().Amount.Currency, x.Sum(y => y.Amount.Amount))))
		{
			if (!EmploymentFinanceService.CanSettlePayroll(context.Employer, amount, out reason))
			{
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		context.Employer.Payroll.EvaluatePayroll();
		var targets = SelectPayables(context.Employer, Selector).ToList();
		if (!context.Employer.Payroll.TrySettlePayables(targets, actor, true, Reason, out var message))
		{
			return EmploymentActionStepResult.Blocked(message);
		}

		return new EmploymentActionStepResult(true, message, true,
			new EmploymentActionStepOperationalState(
				OperationalPayload: $"Payroll selector {Selector}",
				TransactionReference: message));
	}

	private static IEnumerable<IEmploymentPayable> SelectPayables(IEmploymentHost host, string selector)
	{
		var outstanding = host.Payroll.OutstandingLiabilities
		                      .OrderBy(x => x.DueAt)
		                      .ThenBy(x => x.Id)
		                      .ToList();
		if (selector.EqualTo("all") || selector.EqualTo("outstanding"))
		{
			return outstanding;
		}

		var trimmed = selector.TrimStart('#');
		return long.TryParse(trimmed, out var id)
			? outstanding.Where(x => x.Id == id)
			: [];
	}
}

public sealed class ShopFloatAdjustmentActionStep : EmploymentActionStepBase
{
	public ShopFloatAdjustmentActionStep(bool fillRegister, MoneyAmount amount, EmploymentItemSelector? registerSelector = null)
		: base(
			EmploymentActionStepType.ShopFloatAdjustment,
			EmploymentAuthority.WithdrawBusinessCash,
			new[] { EmploymentAICapability.CanHandleCash },
			true,
			true)
	{
		FillRegister = fillRegister;
		Amount = amount;
		RegisterSelector = registerSelector;
	}

	public bool FillRegister { get; }
	public MoneyAmount Amount { get; }
	public EmploymentItemSelector? RegisterSelector { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return context.CanAdjustShopFloat(Amount, FillRegister, RegisterSelector, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		return context.TryAdjustShopFloat(actor, Amount, FillRegister, RegisterSelector, out var reason, out var operationalState)
			? new EmploymentActionStepResult(true,
				$"{(FillRegister ? "Filled" : "Skimmed")} shop cash-register float.", true, operationalState)
			: EmploymentActionStepResult.Blocked(reason);
	}
}

public sealed class PhysicalFloatActionStep : EmploymentActionStepBase
{
	public PhysicalFloatActionStep(PhysicalFloatOperation operation, MoneyAmount? amount, string targetKind,
		EmploymentItemSelector? targetSelector = null)
		: base(
			EmploymentActionStepType.PhysicalFloat,
			EmploymentAuthority.WithdrawBusinessCash,
			new[] { EmploymentAICapability.CanHandleCash },
			true,
			true)
	{
		Operation = operation;
		Amount = amount;
		TargetKind = string.IsNullOrWhiteSpace(targetKind) ? "bank" : targetKind.Trim();
		TargetSelector = targetSelector;
	}

	public PhysicalFloatOperation Operation { get; }
	public MoneyAmount? Amount { get; }
	public string TargetKind { get; }
	public EmploymentItemSelector? TargetSelector { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return context.CanHandlePhysicalFloat(Operation, Amount, TargetKind, TargetSelector, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		return context.TryHandlePhysicalFloat(actor, Operation, Amount, TargetKind, TargetSelector, out var reason,
			       out var operationalState)
			? new EmploymentActionStepResult(true, $"Handled physical cash float: {Operation.DescribeEnum()}.", true,
				operationalState)
			: EmploymentActionStepResult.Blocked(reason);
	}
}

public sealed class BoardPostActionStep : EmploymentActionStepBase
{
	public BoardPostActionStep(string title, string text)
		: base(
			EmploymentActionStepType.BoardPost,
			EmploymentAuthority.PostToHostBoard,
			new[] { EmploymentAICapability.CanPostToBoard },
			false,
			false)
	{
		Title = title;
		Text = text;
	}

	public string Title { get; }
	public string Text { get; }

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		context.Employer.Board.MakeNewPost(actor, Title, Text);
		context.RecordRegister(EmploymentRegisterEntryType.BoardPostCreated, actor, $"Posted to host board: {Title}.");
		return EmploymentActionStepResult.CompletedResult($"Posted {Title} to host board.");
	}
}

public sealed class CataloguedActionShellStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public CataloguedActionShellStep(string actionKey, string actionDescription, ICell? targetLocation = null)
		: this(actionKey, actionDescription, null, targetLocation)
	{
	}

	public CataloguedActionShellStep(string actionKey, string actionDescription, MoneyAmount? amount,
		ICell? targetLocation = null)
		: this(EmploymentActionCatalog.Get(actionKey) ??
		       throw new ArgumentException($"Unknown employment action catalogue key {actionKey}.", nameof(actionKey)),
			actionDescription,
			amount,
			targetLocation)
	{
	}

	private CataloguedActionShellStep(EmploymentActionDefinition definition, string actionDescription,
		MoneyAmount? amount, ICell? targetLocation)
		: base(
			EmploymentActionStepType.CataloguedShell,
			definition.Status == EmploymentActionCatalogStatus.Deferred
				? throw new ArgumentException($"Deferred employment action {definition.Key} cannot be instantiated.", nameof(definition))
				: definition.RequiredAuthority,
			definition.RequiredCapabilities,
			definition.RequiresPaymentAuthorisation,
			definition.IsFinancial)
	{
		Definition = definition;
		ActionKey = definition.Key;
		ActionDescription = actionDescription.Trim();
		Amount = amount;
		TargetLocation = targetLocation;
	}

	public string ActionKey { get; }
	public string ActionDescription { get; }
	public MoneyAmount? Amount { get; }
	public ICell? TargetLocation { get; }
	public EmploymentActionDefinition Definition { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (ActionKey.EqualTo("reserve") && Amount is not null && !context.CanReserveFunds(Amount, out reason))
		{
			return false;
		}

		if (!context.CanPath(actor, TargetLocation))
		{
			reason = "The assigned employee cannot path to the action target location.";
			return false;
		}

		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (ActionKey.EqualTo("reserve") && Amount is not null)
		{
			if (!context.TryReserveFunds(Amount, actor, ActionDescription, out var reason, out var operationalState))
			{
				return EmploymentActionStepResult.Blocked(reason);
			}

			return new EmploymentActionStepResult(true,
				$"Reserved {Amount.Currency.Describe(Amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} for {ActionDescription}.",
				true, operationalState);
		}

		if (ActionKey.EqualTo("release"))
		{
			if (!context.TryReleaseReservedFunds(actor, ActionDescription, out var reason, out var operationalState))
			{
				return EmploymentActionStepResult.Blocked(reason);
			}

			return new EmploymentActionStepResult(true, $"Released employment finance reservation {ActionDescription}.",
				true, operationalState);
		}

		var state = ActionKey switch
		{
			"authorise" => new EmploymentActionStepOperationalState(
				OperationalPayload: Amount is null
					? $"Authorised by {actor.Id}: {ActionDescription}"
					: $"Authorised by {actor.Id}: {Amount.Currency.Describe(Amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} for {ActionDescription}"),
			"reserve" => new EmploymentActionStepOperationalState(
				ReservationReference: $"Reserved by {actor.Id}: {ActionDescription}"),
			"select" => new EmploymentActionStepOperationalState(
				SelectedResources: ActionDescription),
			"estimate" => new EmploymentActionStepOperationalState(
				OperationalPayload: $"Estimate: {ActionDescription}"),
			"route" => new EmploymentActionStepOperationalState(
				RouteResult: TargetLocation is null
					? ActionDescription
					: $"Route target {TargetLocation.Id} ({TargetLocation.Name}): {ActionDescription}"),
			"report" => new EmploymentActionStepOperationalState(
				OperationalPayload: $"Report: {ActionDescription}"),
			_ => new EmploymentActionStepOperationalState(
				OperationalPayload: ActionDescription)
		};
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Recorded {Definition.Status} employment action {ActionKey}: {ActionDescription}.");
		if (ActionKey.EqualTo("authorise") && Amount is not null)
		{
			context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationGranted, actor,
				$"Authorised {Amount.Currency.Describe(Amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} for {ActionDescription}.");
			context.RecordLedger(EmploymentLedgerEntryType.PaymentAuthorisation, actor, Amount,
				$"Authorised employment task spending for {ActionDescription}.");
		}
		else if (ActionKey.EqualTo("authorise"))
		{
			context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationGranted, actor,
				$"Authorised later financial employment task steps for {ActionDescription}.");
		}

		return new EmploymentActionStepResult(true, $"Recorded {ActionKey} action: {ActionDescription}.", true, state);
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return TargetLocation is null ? [] : [TargetLocation];
	}
}

public sealed class GetItemsByIdActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	private readonly List<long> _itemPrototypeIds;
	private readonly List<long> _specificItemIds;
	private readonly List<ICell> _sourceLocations;

	public GetItemsByIdActionStep(int quantity, IEnumerable<long> itemPrototypeIds, IEnumerable<ICell> sourceLocations,
		IEnumerable<long>? specificItemIds = null)
		: base(
			EmploymentActionStepType.GetItemsById,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		Quantity = quantity;
		_itemPrototypeIds = itemPrototypeIds.Distinct().ToList();
		_specificItemIds = (specificItemIds ?? []).Distinct().ToList();
		_sourceLocations = sourceLocations.Distinct().ToList();
	}

	public int Quantity { get; }
	public IReadOnlyList<long> ItemPrototypeIds => _itemPrototypeIds;
	public IReadOnlyList<long> SpecificItemIds => _specificItemIds;
	public IReadOnlyList<ICell> SourceLocations => _sourceLocations;

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (Quantity <= 0)
		{
			reason = "Item retrieval steps must request a positive quantity.";
			return false;
		}

		if (!_itemPrototypeIds.Any() && !_specificItemIds.Any())
		{
			reason = "Item retrieval steps must specify at least one item prototype id or specific item id.";
			return false;
		}

		var reachableSources = ReachableSources(context, actor).ToList();
		if (!reachableSources.Any())
		{
			reason = "The assigned employee cannot path to any source location.";
			return false;
		}

		var matchingCount = MatchingItems(context, actor, reachableSources).Count();
		if (matchingCount < Quantity)
		{
			reason = $"There are only {matchingCount.ToString("N0", actor)} matching item{(matchingCount == 1 ? string.Empty : "s")} for {Quantity.ToString("N0", actor)} requested item{(Quantity == 1 ? string.Empty : "s")} in reachable source locations.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var items = MatchingItems(context, actor).Take(Quantity).ToList();
		if (items.Count < Quantity)
		{
			return EmploymentActionStepResult.Blocked("There are not enough matching items to collect.");
		}

		var previouslyCarried = context.CarriedTaskItems(actor).Select(x => x.Id).ToHashSet();
		if (!context.TryCollectTaskItems(actor, items.Select(x => (x.Item, x.Source)).ToList(), out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return new EmploymentActionStepResult(true, $"Collected {items.Count:N0} item(s) by selector.", true,
			EmploymentActionStepOperationalStateBuilder.CollectedTaskItemCustody(
				context,
				actor,
				items.Select(x => x.Item).ToList(),
				previouslyCarried));
	}

	private IEnumerable<ICell> ReachableSources(IEmploymentTaskContext context, ICharacter actor)
	{
		return _sourceLocations.Where(x => context.CanPath(actor, x));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return ReachableSources(context, actor).ToList();
	}

	private IEnumerable<(ICell Source, IGameItem Item)> MatchingItems(IEmploymentTaskContext context, ICharacter actor)
	{
		return MatchingItems(context, actor, ReachableSources(context, actor));
	}

	private IEnumerable<(ICell Source, IGameItem Item)> MatchingItems(IEmploymentTaskContext context, ICharacter actor,
		IEnumerable<ICell> sources)
	{
		var prototypeIds = _itemPrototypeIds.ToHashSet();
		var itemIds = _specificItemIds.ToHashSet();
		foreach (var source in sources)
		foreach (var item in context.AvailableItems(source)
		                    .Where(x => prototypeIds.Contains(x.Prototype.Id) || itemIds.Contains(x.Id)))
		{
			yield return (source, item);
		}
	}
}

public sealed class GetItemsByTagActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	private readonly List<ICell> _sourceLocations;

	public GetItemsByTagActionStep(int quantity, string tagName, IEnumerable<ICell> sourceLocations)
		: base(
			EmploymentActionStepType.GetItemsByTag,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		Quantity = quantity;
		TagName = tagName;
		_sourceLocations = sourceLocations.Distinct().ToList();
	}

	public int Quantity { get; }
	public string TagName { get; }
	public IReadOnlyList<ICell> SourceLocations => _sourceLocations;

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (Quantity <= 0)
		{
			reason = "Item retrieval steps must request a positive quantity.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(TagName))
		{
			reason = "Item retrieval steps must specify a tag.";
			return false;
		}

		if (!ReachableSources(context, actor).Any())
		{
			reason = "The assigned employee cannot path to any source location.";
			return false;
		}

		if (MatchingItems(context, actor).Count() < Quantity)
		{
			reason = $"There are not enough items tagged {TagName} in reachable source locations.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var items = MatchingItems(context, actor).Take(Quantity).ToList();
		if (items.Count < Quantity)
		{
			return EmploymentActionStepResult.Blocked($"There are not enough items tagged {TagName} to collect.");
		}

		var previouslyCarried = context.CarriedTaskItems(actor).Select(x => x.Id).ToHashSet();
		if (!context.TryCollectTaskItems(actor, items.Select(x => (x.Item, x.Source)).ToList(), out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return new EmploymentActionStepResult(true, $"Collected {items.Count:N0} item(s) tagged {TagName}.", true,
			EmploymentActionStepOperationalStateBuilder.CollectedTaskItemCustody(
				context,
				actor,
				items.Select(x => x.Item).ToList(),
				previouslyCarried));
	}

	private IEnumerable<ICell> ReachableSources(IEmploymentTaskContext context, ICharacter actor)
	{
		return _sourceLocations.Where(x => context.CanPath(actor, x));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return ReachableSources(context, actor).ToList();
	}

	private IEnumerable<(ICell Source, IGameItem Item)> MatchingItems(IEmploymentTaskContext context, ICharacter actor)
	{
		foreach (var source in ReachableSources(context, actor))
		foreach (var item in context.AvailableItems(source).Where(x => context.ItemHasTag(x, TagName)))
		{
			yield return (source, item);
		}
	}
}

public sealed class GetCommodityActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	private readonly Dictionary<string, string> _characteristics;
	private readonly List<ICell> _sourceLocations;

	public GetCommodityActionStep(double requiredWeight, string materialName, string? tagName,
		IReadOnlyDictionary<string, string>? characteristics, IEnumerable<ICell> sourceLocations)
		: base(
			EmploymentActionStepType.GetCommodity,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		RequiredWeight = requiredWeight;
		MaterialName = materialName;
		TagName = tagName;
		_characteristics = new Dictionary<string, string>(
			characteristics ?? new Dictionary<string, string>(),
			StringComparer.InvariantCultureIgnoreCase);
		_sourceLocations = sourceLocations.Distinct().ToList();
	}

	public double RequiredWeight { get; }
	public string MaterialName { get; }
	public string? TagName { get; }
	public IReadOnlyDictionary<string, string> Characteristics => _characteristics;
	public IReadOnlyList<ICell> SourceLocations => _sourceLocations;

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (RequiredWeight <= 0.0)
		{
			reason = "Commodity retrieval steps must request a positive weight.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(MaterialName))
		{
			reason = "Commodity retrieval steps must specify a material.";
			return false;
		}

		if (!ReachableSources(context, actor).Any())
		{
			reason = "The assigned employee cannot path to any source location.";
			return false;
		}

		var available = MatchingItems(context, actor)
			.Sum(x => context.CommodityWeight(x.Item, MaterialName, TagName, _characteristics));
		if (available < RequiredWeight)
		{
			reason = $"There is only {available:N2} matching commodity weight available for {MaterialName}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var collected = new List<(ICell Source, IGameItem Item)>();
		var totalWeight = 0.0;
		foreach (var item in MatchingItems(context, actor).ToList())
		{
			collected.Add(item);
			totalWeight += context.CommodityWeight(item.Item, MaterialName, TagName, _characteristics);
			if (totalWeight >= RequiredWeight)
			{
				break;
			}
		}

		if (totalWeight < RequiredWeight)
		{
			return EmploymentActionStepResult.Blocked($"There is not enough {MaterialName} commodity to collect.");
		}

		var previouslyCarried = context.CarriedTaskItems(actor).Select(x => x.Id).ToHashSet();
		if (!context.TryCollectTaskItems(actor, collected.Select(x => (x.Item, x.Source)).ToList(), out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return new EmploymentActionStepResult(true,
			$"Collected {totalWeight:N2} weight of {MaterialName} commodity in {collected.Count:N0} item(s).", true,
			EmploymentActionStepOperationalStateBuilder.CollectedTaskItemCustody(
				context,
				actor,
				collected.Select(x => x.Item).ToList(),
				previouslyCarried));
	}

	private IEnumerable<ICell> ReachableSources(IEmploymentTaskContext context, ICharacter actor)
	{
		return _sourceLocations.Where(x => context.CanPath(actor, x));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return ReachableSources(context, actor).ToList();
	}

	private IEnumerable<(ICell Source, IGameItem Item)> MatchingItems(IEmploymentTaskContext context, ICharacter actor)
	{
		foreach (var source in ReachableSources(context, actor))
		foreach (var item in context.AvailableItems(source)
		                            .Where(x => context.CommodityWeight(x, MaterialName, TagName, _characteristics) > 0.0))
		{
			yield return (source, item);
		}
	}
}

public sealed class DeliverItemsActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public DeliverItemsActionStep(ICell destination, IGameItem? container = null, string? containerTag = null)
		: this(destination, container is not null
			? EmploymentItemSelector.ForItem(container)
			: string.IsNullOrWhiteSpace(containerTag) ? null : EmploymentItemSelector.ForTag(containerTag))
	{
	}

	public DeliverItemsActionStep(ICell destination, EmploymentItemSelector? containerSelector)
		: base(
			EmploymentActionStepType.DeliverItems,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		Destination = destination;
		ContainerSelector = containerSelector;
	}

	public ICell Destination { get; }
	public EmploymentItemSelector? ContainerSelector { get; }
	public IGameItem? Container => ContainerSelector?.Item;
	public string? ContainerTag => ContainerSelector?.Kind == EmploymentItemSelectorKind.Tag ? ContainerSelector.Text : null;

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!context.CanPath(actor, Destination))
		{
			reason = "The assigned employee cannot path to the delivery destination.";
			return false;
		}

		if (!context.CarriedTaskItems(actor).Any())
		{
			reason = "The assigned employee is no longer carrying any task items to deliver.";
			return false;
		}

		var container = ResolveContainer(context, actor);
		if (ContainerSelector is not null && container is null)
		{
			reason = $"There is no destination container matching {EmploymentItemSelectorResolver.Describe(ContainerSelector)}.";
			return false;
		}

		if (container is not null && container.GetItemType<IContainer>() is null)
		{
			reason = $"{container.Name} is not a container.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var carried = context.CarriedTaskItems(actor).ToList();
		var count = carried.Count;
		var container = ResolveContainer(context, actor);
		if (ContainerSelector is not null && container is null)
		{
			return EmploymentActionStepResult.Blocked(
				$"There is no destination container matching {EmploymentItemSelectorResolver.Describe(ContainerSelector)}.");
		}

		if (!context.TryDeliverTaskItems(actor, Destination, container, null, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return new EmploymentActionStepResult(true, $"Delivered {count:N0} task item(s).", true,
			new EmploymentActionStepOperationalState(
				SelectedResources: EmploymentTaskContext.FormatTaskItemCustody("deliver", actor.Id, carried)));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return [Destination];
	}

	private IGameItem? ResolveContainer(IEmploymentTaskContext context, ICharacter actor)
	{
		return EmploymentItemSelectorResolver.Resolve(context, actor, ContainerSelector, Destination, false);
	}
}

public sealed class LoadItemsActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public LoadItemsActionStep(IGameItem? targetContainer, string? targetContainerTag, ICell? targetLocation)
		: this(targetContainer is not null
			? EmploymentItemSelector.ForItem(targetContainer)
			: string.IsNullOrWhiteSpace(targetContainerTag) ? null : EmploymentItemSelector.ForTag(targetContainerTag),
			targetLocation)
	{
	}

	public LoadItemsActionStep(EmploymentItemSelector? targetContainerSelector, ICell? targetLocation)
		: base(
			EmploymentActionStepType.LoadItems,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		TargetContainerSelector = targetContainerSelector;
		TargetLocation = targetLocation;
	}

	public EmploymentItemSelector? TargetContainerSelector { get; }
	public IGameItem? TargetContainer => TargetContainerSelector?.Item;
	public string? TargetContainerTag => TargetContainerSelector?.Kind == EmploymentItemSelectorKind.Tag ? TargetContainerSelector.Text : null;
	public ICell? TargetLocation { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (TargetLocation is not null && !context.CanPath(actor, TargetLocation))
		{
			reason = "The assigned employee cannot path to the load location.";
			return false;
		}

		if (!context.CarriedTaskItems(actor).Any())
		{
			reason = "The assigned employee is not carrying any task items to load.";
			return false;
		}

		var target = ResolveTargetContainer(context, actor);
		if (target is null)
		{
			reason = $"There is no load target container matching {EmploymentItemSelectorResolver.Describe(TargetContainerSelector)}.";
			return false;
		}

		if (target.GetItemType<IContainer>() is null)
		{
			reason = $"{target.Name} is not a container.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var target = ResolveTargetContainer(context, actor);
		if (target is null)
		{
			return EmploymentActionStepResult.Blocked("The load target container is not available.");
		}

		if (!context.TryLoadCarriedTaskItems(actor, target, out var reason, out var state))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Loaded task items into {target.Name}.");
		return new EmploymentActionStepResult(true, $"Loaded task items into {target.Name}.", true, state);
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		if (TargetLocation is not null)
		{
			return [TargetLocation];
		}

		var target = ResolveTargetContainer(context, actor);
		return target?.TrueLocations.ToList() ?? [];
	}

	private IGameItem? ResolveTargetContainer(IEmploymentTaskContext context, ICharacter actor)
	{
		return EmploymentItemSelectorResolver.Resolve(context, actor, TargetContainerSelector, TargetLocation, true);
	}
}

public sealed class UnloadItemsActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public UnloadItemsActionStep(IGameItem? sourceContainer, string? sourceContainerTag, ICell? sourceLocation)
		: this(sourceContainer is not null
			? EmploymentItemSelector.ForItem(sourceContainer)
			: string.IsNullOrWhiteSpace(sourceContainerTag) ? null : EmploymentItemSelector.ForTag(sourceContainerTag),
			sourceLocation)
	{
	}

	public UnloadItemsActionStep(EmploymentItemSelector? sourceContainerSelector, ICell? sourceLocation)
		: base(
			EmploymentActionStepType.UnloadItems,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		SourceContainerSelector = sourceContainerSelector;
		SourceLocation = sourceLocation;
	}

	public EmploymentItemSelector? SourceContainerSelector { get; }
	public IGameItem? SourceContainer => SourceContainerSelector?.Item;
	public string? SourceContainerTag => SourceContainerSelector?.Kind == EmploymentItemSelectorKind.Tag ? SourceContainerSelector.Text : null;
	public ICell? SourceLocation { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (SourceLocation is not null && !context.CanPath(actor, SourceLocation))
		{
			reason = "The assigned employee cannot path to the unload location.";
			return false;
		}

		var source = ResolveSourceContainer(context, actor);
		if (source is null)
		{
			reason = $"There is no unload source container matching {EmploymentItemSelectorResolver.Describe(SourceContainerSelector)}.";
			return false;
		}

		if (!context.LoadedTaskItems(actor, source).Any())
		{
			reason = $"There are no task-loaded items recorded in {source.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var source = ResolveSourceContainer(context, actor);
		if (source is null)
		{
			return EmploymentActionStepResult.Blocked("The unload source container is not available.");
		}

		if (!context.TryUnloadTaskItems(actor, source, out var reason, out var state))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Unloaded task items from {source.Name}.");
		return new EmploymentActionStepResult(true, $"Unloaded task items from {source.Name}.", true, state);
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		if (SourceLocation is not null)
		{
			return [SourceLocation];
		}

		var source = ResolveSourceContainer(context, actor);
		return source?.TrueLocations.ToList() ?? [];
	}

	private IGameItem? ResolveSourceContainer(IEmploymentTaskContext context, ICharacter actor)
	{
		return EmploymentItemSelectorResolver.Resolve(context, actor, SourceContainerSelector, SourceLocation, true);
	}
}

public sealed class ReturnAssetActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public ReturnAssetActionStep(IGameItem? container, string? containerTag, ICell destination,
		IGameItem? destinationContainer = null, string? destinationContainerTag = null)
		: this(container is not null
				? EmploymentItemSelector.ForItem(container)
				: string.IsNullOrWhiteSpace(containerTag) ? null : EmploymentItemSelector.ForTag(containerTag),
			destination,
			destinationContainer is not null
				? EmploymentItemSelector.ForItem(destinationContainer)
				: string.IsNullOrWhiteSpace(destinationContainerTag) ? null : EmploymentItemSelector.ForTag(destinationContainerTag))
	{
	}

	public ReturnAssetActionStep(EmploymentItemSelector? containerSelector, ICell destination,
		EmploymentItemSelector? destinationContainerSelector = null)
		: base(
			EmploymentActionStepType.ReturnAsset,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		ContainerSelector = containerSelector;
		Destination = destination;
		DestinationContainerSelector = destinationContainerSelector;
	}

	public EmploymentItemSelector? ContainerSelector { get; }
	public IGameItem? Container => ContainerSelector?.Item;
	public string? ContainerTag => ContainerSelector?.Kind == EmploymentItemSelectorKind.Tag ? ContainerSelector.Text : null;
	public ICell Destination { get; }
	public EmploymentItemSelector? DestinationContainerSelector { get; }
	public IGameItem? DestinationContainer => DestinationContainerSelector?.Item;
	public string? DestinationContainerTag => DestinationContainerSelector?.Kind == EmploymentItemSelectorKind.Tag ? DestinationContainerSelector.Text : null;

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!context.CanPath(actor, Destination))
		{
			reason = "The assigned employee cannot path to the return destination.";
			return false;
		}

		var container = ResolveContainer(context, actor);
		if (container is null)
		{
			reason = $"There is no return container matching {EmploymentItemSelectorResolver.Describe(ContainerSelector)}.";
			return false;
		}

		if (container.GetItemType<IContainer>() is null)
		{
			reason = $"{container.Name} is not a container.";
			return false;
		}

		if (DestinationContainerSelector is not null && ResolveDestinationContainer(context, actor) is null)
		{
			reason = $"There is no destination container matching {EmploymentItemSelectorResolver.Describe(DestinationContainerSelector)}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var container = ResolveContainer(context, actor);
		if (container is null)
		{
			return EmploymentActionStepResult.Blocked("The return container is not available.");
		}

		if (!ActorCarriesTaskItem(context, actor, container))
		{
			var source = actor.Location;
			if (source is null || !container.TrueLocations.Any(x => x.Id == source.Id))
			{
				return EmploymentActionStepResult.Blocked($"The assigned employee must go to {container.Name} before returning it.");
			}

			if (!context.TryCollectTaskItem(actor, container, source, out var collectReason))
			{
				return EmploymentActionStepResult.Blocked(collectReason);
			}

			return new EmploymentActionStepResult(
				true,
				$"Collected {container.Name} for return to {Destination.Name}.",
				false,
				new EmploymentActionStepOperationalState(SelectedResources: $"Return container {container.Id}"));
		}

		if (actor.Location is not null && actor.Location.Id != Destination.Id)
		{
			return new EmploymentActionStepResult(
				true,
				$"Carrying {container.Name} to {Destination.Name}.",
				false,
				new EmploymentActionStepOperationalState(SelectedResources: $"Return container {container.Id}"));
		}

		var destinationContainer = ResolveDestinationContainer(context, actor);
		if (DestinationContainerSelector is not null && destinationContainer is null)
		{
			return EmploymentActionStepResult.Blocked(
				$"There is no destination container matching {EmploymentItemSelectorResolver.Describe(DestinationContainerSelector)}.");
		}

		if (!context.TryReturnContainer(actor, container, Destination, destinationContainer, null,
			    out var reason, out var state))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Returned container {container.Name}.");
		return new EmploymentActionStepResult(true, $"Returned container {container.Name}.", true, state);
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		var container = ResolveContainer(context, actor);
		if (container is null)
		{
			return [Destination];
		}

		if (ActorCarriesTaskItem(context, actor, container))
		{
			return [Destination];
		}

		var sourceLocations = container.TrueLocations.ToList();
		return sourceLocations.Any() ? sourceLocations : [Destination];
	}

	private IGameItem? ResolveContainer(IEmploymentTaskContext context, ICharacter actor)
	{
		return EmploymentItemSelectorResolver.Resolve(context, actor, ContainerSelector, actor.Location, true);
	}

	private IGameItem? ResolveDestinationContainer(IEmploymentTaskContext context, ICharacter actor)
	{
		return EmploymentItemSelectorResolver.Resolve(context, actor, DestinationContainerSelector, Destination, false);
	}

	private static bool ActorCarriesTaskItem(IEmploymentTaskContext context, ICharacter actor, IGameItem item)
	{
		return context.CarriedTaskItems(actor).Any(x => x.Id == item.Id);
	}
}

public sealed class VehicleOperationActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public VehicleOperationActionStep(IVehicle vehicle, IVehicleCargoSpace cargoSpace)
		: base(
			EmploymentActionStepType.VehicleOperation,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanUseVehicles },
			false,
			false)
	{
		Vehicle = vehicle;
		CargoSpace = cargoSpace;
	}

	public IVehicle Vehicle { get; }
	public IVehicleCargoSpace CargoSpace { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (Vehicle.Disabled || Vehicle.Destroyed)
		{
			reason = $"{Vehicle.Name} is not available for employment cargo work.";
			return false;
		}

		if (CargoSpace.IsDisabled || !CargoSpace.CanAccess(actor, out reason))
		{
			reason = string.IsNullOrWhiteSpace(reason)
				? $"{CargoSpace.Name} is not accessible."
				: reason;
			return false;
		}

		var vehicleLocation = Vehicle.Location;
		if (vehicleLocation is null)
		{
			reason = $"{Vehicle.Name} is not currently at a reachable location.";
			return false;
		}

		if (!context.CanPath(actor, vehicleLocation))
		{
			reason = "The assigned employee cannot path to the vehicle cargo space.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Selected cargo space {CargoSpace.Name} on {Vehicle.Name}.");
		return new EmploymentActionStepResult(
			true,
			$"Selected cargo space {CargoSpace.Name} on {Vehicle.Name}.",
			true,
			new EmploymentActionStepOperationalState(
				SelectedResources: $"vehicle={Vehicle.Id};cargo={CargoSpace.Id};projection={CargoSpace.ProjectionItemId?.ToString("F0") ?? string.Empty}"));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return Vehicle.Location is null ? [] : [Vehicle.Location];
	}
}

public sealed class PriceChangeActionStep : EmploymentActionStepBase
{
	public PriceChangeActionStep(string merchandiseSelector, MoneyAmount exactPrice)
		: base(
			EmploymentActionStepType.PriceChange,
			EmploymentAuthority.AdjustPrices,
			new[] { EmploymentAICapability.CanManagePrices },
			false,
			false)
	{
		PriceChangeKind = PriceChangeActionKind.Merchandise;
		MerchandiseSelector = merchandiseSelector.Trim();
		ExactPrice = exactPrice;
		MarketSelector = "host";
		CategorySelector = string.Empty;
		InfluenceName = string.Empty;
	}

	public PriceChangeActionStep(string marketSelector, string categorySelector, double supplyImpact,
		double demandImpact, double flatPriceImpact, string? influenceName = null, TimeSpan? duration = null,
		string? untilText = null)
		: base(
			EmploymentActionStepType.PriceChange,
			EmploymentAuthority.AdjustPrices,
			new[] { EmploymentAICapability.CanManagePrices },
			false,
			false)
	{
		PriceChangeKind = PriceChangeActionKind.MarketCategory;
		MerchandiseSelector = string.Empty;
		MarketSelector = string.IsNullOrWhiteSpace(marketSelector) ? "host" : marketSelector.Trim();
		CategorySelector = categorySelector.Trim();
		SupplyImpact = supplyImpact;
		DemandImpact = demandImpact;
		FlatPriceImpact = flatPriceImpact;
		InfluenceName = string.IsNullOrWhiteSpace(influenceName)
			? $"Employment price adjustment - {CategorySelector}"
			: influenceName.Trim();
		Duration = duration;
		UntilText = string.IsNullOrWhiteSpace(untilText) ? null : untilText.Trim();
	}

	public PriceChangeActionKind PriceChangeKind { get; }
	public string MerchandiseSelector { get; }
	public MoneyAmount? ExactPrice { get; }
	public string MarketSelector { get; }
	public string CategorySelector { get; }
	public double SupplyImpact { get; }
	public double DemandImpact { get; }
	public double FlatPriceImpact { get; }
	public string InfluenceName { get; }
	public TimeSpan? Duration { get; }
	public string? UntilText { get; }
	private bool HasExplicitExpiry => Duration.HasValue || !string.IsNullOrWhiteSpace(UntilText);

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (PriceChangeKind == PriceChangeActionKind.Merchandise)
		{
			if (context.Employer is not IShop shop)
			{
				reason = "Exact merchandise price changes can only target shop employment hosts.";
				return false;
			}

			var merchandise = ResolveMerchandise(shop);
			if (merchandise is null)
			{
				reason = $"There is no merchandise belonging to {shop.Name} matching {MerchandiseSelector}.";
				return false;
			}

			if (ExactPrice is null || !merchandise.CanSetBasePrice(ExactPrice.Amount, out reason))
			{
				reason = string.IsNullOrWhiteSpace(reason) ? "The requested merchandise price is invalid." : reason;
				return false;
			}

			reason = string.Empty;
			return true;
		}

		var market = ResolveMarket(context.Employer, actor);
		if (market is null)
		{
			reason = $"There is no market matching {MarketSelector}.";
			return false;
		}

		var category = ResolveCategory(actor);
		if (category is null)
		{
			reason = $"There is no market category matching {CategorySelector}.";
			return false;
		}

		if (!market.MarketCategories.Any(x => x.Id == category.Id))
		{
			reason = $"{category.Name} is not part of {market.Name}.";
			return false;
		}

		if (!TryResolveAppliesUntil(market, actor, out _, out reason))
		{
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (PriceChangeKind == PriceChangeActionKind.Merchandise)
		{
			if (context.Employer is not IShop shop)
			{
				return EmploymentActionStepResult.Blocked("Exact merchandise price changes can only target shop employment hosts.");
			}

			var merchandise = ResolveMerchandise(shop);
			if (merchandise is null || ExactPrice is null)
			{
				return EmploymentActionStepResult.Blocked("The merchandise price target is no longer available.");
			}

			if (!merchandise.CanSetBasePrice(ExactPrice.Amount, out var priceReason))
			{
				return EmploymentActionStepResult.Blocked(priceReason);
			}

			var oldPrice = merchandise.BasePrice;
			merchandise.SetBasePrice(ExactPrice.Amount, actor);
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
				$"Changed merchandise {merchandise.Name} base price from {shop.Currency.Describe(oldPrice, CurrencyDescriptionPatternType.ShortDecimal)} to {shop.Currency.Describe(ExactPrice.Amount, CurrencyDescriptionPatternType.ShortDecimal)}.");
			return new EmploymentActionStepResult(
				true,
				$"Changed {merchandise.Name} base price to {shop.Currency.Describe(ExactPrice.Amount, CurrencyDescriptionPatternType.ShortDecimal)}.",
				true,
				new EmploymentActionStepOperationalState(
					OperationalPayload:
					$"price:merchandise={merchandise.Id};old={oldPrice};new={ExactPrice.Amount};currency={ExactPrice.Currency.Id}"));
		}

		var market = ResolveMarket(context.Employer, actor);
		var category = ResolveCategory(actor);
		if (market is null || category is null)
		{
			return EmploymentActionStepResult.Blocked("The market price target is no longer available.");
		}

		if (!market.MarketCategories.Any(x => x.Id == category.Id))
		{
			return EmploymentActionStepResult.Blocked($"{category.Name} is not part of {market.Name}.");
		}

		if (!TryResolveAppliesUntil(market, actor, out var appliesUntil, out var untilReason))
		{
			return EmploymentActionStepResult.Blocked(untilReason);
		}

		var influence = market.MarketInfluences.FirstOrDefault(x => x.Name.EqualTo(InfluenceName));
		if (influence is null)
		{
			var now = market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
			influence = new MarketInfluence(
				market,
				InfluenceName,
				$"Employment generated price adjustment for {category.Name}.",
				now,
				appliesUntil!);
			actor.Gameworld.Add(influence);
			market.ApplyMarketInfluence(influence);
		}
		else if (HasExplicitExpiry)
		{
			influence.SetAppliesUntil(appliesUntil);
		}

		influence.SetCategoryImpact(category, SupplyImpact, DemandImpact, FlatPriceImpact);
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Applied market price influence {InfluenceName} to category {category.Name} on {market.Name}.");
		return new EmploymentActionStepResult(
			true,
			$"Applied market price influence {InfluenceName} to {category.Name}.",
			true,
			new EmploymentActionStepOperationalState(
				OperationalPayload:
				$"price:market={market.Id};category={category.Id};influence={influence.Id};supply={SupplyImpact};demand={DemandImpact};flat={FlatPriceImpact}"));
	}

	private IMerchandise? ResolveMerchandise(IShop shop)
	{
		if (long.TryParse(MerchandiseSelector.TrimStart('#'), out var id))
		{
			return shop.Merchandises.FirstOrDefault(x => x.Id == id);
		}

		return shop.Merchandises.GetByIdOrName(MerchandiseSelector) ??
		       shop.Merchandises.FirstOrDefault(x =>
			       x.Name.EqualTo(MerchandiseSelector) ||
			       x.ListDescription.EqualTo(MerchandiseSelector));
	}

	private IMarket? ResolveMarket(IEmploymentHost host, ICharacter actor)
	{
		if (MarketSelector.EqualTo("host") || MarketSelector.EqualTo("market"))
		{
			return host.Market ?? (host as IShop)?.MarketForPricingPurposes;
		}

		if (long.TryParse(MarketSelector.TrimStart('#'), out var id))
		{
			return actor.Gameworld.Markets.Get(id);
		}

		return actor.Gameworld.Markets.GetByIdOrName(MarketSelector);
	}

	private IMarketCategory? ResolveCategory(ICharacter actor)
	{
		if (long.TryParse(CategorySelector.TrimStart('#'), out var id))
		{
			return actor.Gameworld.MarketCategories.Get(id);
		}

		return actor.Gameworld.MarketCategories.GetByIdOrName(CategorySelector);
	}

	private bool TryResolveAppliesUntil(IMarket market, ICharacter actor, out MudDateTime? appliesUntil,
		out string reason)
	{
		appliesUntil = null;
		if (!string.IsNullOrWhiteSpace(UntilText))
		{
			if (UntilText.EqualToAny("never", "always", "forever"))
			{
				reason = string.Empty;
				return true;
			}

			if (!MudDateTime.TryParse(UntilText, market.EconomicZone.FinancialPeriodReferenceCalendar,
				    market.EconomicZone.FinancialPeriodReferenceClock, actor, out var until, out reason))
			{
				return false;
			}

			appliesUntil = until;
			return true;
		}

		if (Duration.HasValue)
		{
			appliesUntil = market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime + Duration.Value;
		}

		reason = string.Empty;
		return true;
	}
}

public sealed class JobOpeningAdministrationActionStep : EmploymentActionStepBase
{
	public JobOpeningAdministrationActionStep(JobOpeningDefinition definition, string? reason = null)
		: base(
			EmploymentActionStepType.JobOpeningAdministration,
			EmploymentAuthority.CreateJobOpenings,
			Array.Empty<EmploymentAICapability>(),
			false,
			false)
	{
		Operation = JobOpeningAdministrationActionKind.Create;
		Definition = definition;
		Reason = string.IsNullOrWhiteSpace(reason) ? "Created by employment task." : reason.Trim();
	}

	public JobOpeningAdministrationActionStep(JobOpeningAdministrationActionKind operation, long openingId,
		JobOpeningDefinition? definition = null, string? reason = null)
		: base(
			EmploymentActionStepType.JobOpeningAdministration,
			operation == JobOpeningAdministrationActionKind.Create
				? EmploymentAuthority.CreateJobOpenings
				: EmploymentAuthority.ModifyJobOpenings,
			Array.Empty<EmploymentAICapability>(),
			false,
			false)
	{
		Operation = operation;
		OpeningId = openingId;
		Definition = definition;
		Reason = string.IsNullOrWhiteSpace(reason)
			? operation == JobOpeningAdministrationActionKind.Close
				? "Closed by employment task."
				: "Modified by employment task."
			: reason.Trim();
	}

	public JobOpeningAdministrationActionKind Operation { get; }
	public long? OpeningId { get; }
	public JobOpeningDefinition? Definition { get; }
	public string Reason { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (Operation == JobOpeningAdministrationActionKind.Create)
		{
			if (Definition is null)
			{
				reason = "Job-opening create steps require an opening definition.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		var opening = ResolveOpening(context.Employer);
		if (opening is null)
		{
			reason = $"There is no job opening #{OpeningId?.ToString("F0") ?? "?"} for {context.Employer.EmploymentHostName}.";
			return false;
		}

		if (Operation == JobOpeningAdministrationActionKind.Modify)
		{
			if (opening.Status == JobOpeningStatus.Closed)
			{
				reason = "Closed job openings cannot be modified.";
				return false;
			}

			if (Definition is null)
			{
				reason = "Job-opening modify steps require an opening definition.";
				return false;
			}
		}
		else if (opening.Status == JobOpeningStatus.Closed)
		{
			reason = "That job opening is already closed.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		try
		{
			switch (Operation)
			{
				case JobOpeningAdministrationActionKind.Create:
				{
					if (Definition is null)
					{
						return EmploymentActionStepResult.Failed("Job-opening create steps require an opening definition.");
					}

					var opening = context.Employer.Employment.CreateJobOpening(Definition, actor);
					return new EmploymentActionStepResult(
						true,
						$"Created {opening.Role.DescribeEnum()} job opening #{opening.Id:N0}.",
						true,
						new EmploymentActionStepOperationalState(
							OperationalPayload: $"jobopening:create={opening.Id};role={opening.Role}"));
				}
				case JobOpeningAdministrationActionKind.Close:
				{
					var opening = ResolveOpening(context.Employer);
					if (opening is null)
					{
						return EmploymentActionStepResult.Blocked("The job opening to close is no longer available.");
					}

					context.Employer.Employment.CloseJobOpening(opening, actor, Reason);
					return new EmploymentActionStepResult(
						true,
						$"Closed {opening.Role.DescribeEnum()} job opening #{opening.Id:N0}.",
						true,
						new EmploymentActionStepOperationalState(
							OperationalPayload: $"jobopening:close={opening.Id};reason={Reason}"));
				}
				case JobOpeningAdministrationActionKind.Modify:
				{
					var opening = ResolveOpening(context.Employer);
					if (opening is null)
					{
						return EmploymentActionStepResult.Blocked("The job opening to modify is no longer available.");
					}

					if (Definition is null)
					{
						return EmploymentActionStepResult.Failed("Job-opening modify steps require an opening definition.");
					}

					context.Employer.Employment.ModifyJobOpening(opening, Definition, actor, Reason);
					return new EmploymentActionStepResult(
						true,
						$"Modified {opening.Role.DescribeEnum()} job opening #{opening.Id:N0}.",
						true,
						new EmploymentActionStepOperationalState(
							OperationalPayload: $"jobopening:modify={opening.Id};role={opening.Role};reason={Reason}"));
				}
				default:
					return EmploymentActionStepResult.Failed("Unknown job-opening administration action.");
			}
		}
		catch (InvalidOperationException ex)
		{
			return EmploymentActionStepResult.Blocked(ex.Message);
		}
	}

	private IJobOpening? ResolveOpening(IEmploymentHost host)
	{
		return OpeningId.HasValue
			? host.JobOpenings.FirstOrDefault(x => x.Id == OpeningId.Value)
			: null;
	}
}
