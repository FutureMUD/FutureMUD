using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Hospitals;
using MudSharp.Economy.Property;
using MudSharp.Economy.Shops;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
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

		return includeCarried && EmploymentWorkerItemLocator.IsHeldOrWielded(actor, item) ? item : null;
	}

	private static IEnumerable<IGameItem> CandidateItems(IEmploymentTaskContext context, ICharacter actor,
		ICell? location, bool includeCarried)
	{
		if (includeCarried)
		{
			foreach (var item in EmploymentWorkerItemLocator.TaskHeldItems(context, actor))
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

		var actorKey = CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor);
		var transportBundleIds = actualCarried
		                         .Where(x => !selectedItemIds.Contains(x.Id))
		                         .Select(x => x.Id)
		                         .ToList();
		var contextManagedItemIds = context is EmploymentTaskContext taskContext
			? actualCarried
			  .Where(x => taskContext.IsContextManagedCarriedTaskItem(actor, x))
			  .Select(x => x.Id)
			  .ToList()
			: [];
		return new EmploymentActionStepOperationalState(
			SelectedResources: EmploymentTaskContext.FormatTaskItemCustody("collect", actorKey, actualCarried,
				transportBundleIds, contextManagedItemIds));
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

public sealed class SupplierSelectionActionStep : EmploymentActionStepBase
{
	public SupplierSelectionActionStep(PurchaseActionStep purchase)
		: base(
			EmploymentActionStepType.SupplierSelection,
			EmploymentAuthority.ManageStockRules,
			new[] { EmploymentAICapability.CanPurchaseCommodities },
			false,
			false)
	{
		if (!purchase.IsExecutablePurchase)
		{
			throw new ArgumentException("Supplier selection requires a concrete executable purchase target.", nameof(purchase));
		}

		Purchase = purchase;
	}

	public PurchaseActionStep Purchase { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return context is EmploymentTaskContext concrete &&
		       EmploymentFinanceService.TryPreviewPurchaseTarget(concrete, Purchase, out _, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (context is not EmploymentTaskContext concrete)
		{
			return EmploymentActionStepResult.Blocked("Supplier selection requires the concrete employment task context.");
		}

		if (!EmploymentFinanceService.TryPreviewPurchaseTarget(concrete, Purchase, out var preview, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		var price = preview.Shop.Currency.Describe(preview.Price, CurrencyDescriptionPatternType.ShortDecimal);
		var quantity = preview.CommodityWeight.HasValue
			? $"{preview.CommodityWeight.Value.ToString("N2", actor)} commodity weight"
			: preview.Quantity.ToString("N0", actor);
		var locations = preview.Locations.Any()
			? string.Join(", ", preview.Locations.Select(x => $"{x.Id.ToString("F0", CultureInfo.InvariantCulture)}:{x.Name}"))
			: "none";
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Selected supplier {preview.Shop.Name} for {Purchase.PurchaseDescription}: {quantity} of {preview.Merchandise.Name} at {price}.");

		return new EmploymentActionStepResult(
			true,
			$"Selected supplier {preview.Shop.Name} for {Purchase.PurchaseDescription} at {price}.",
			true,
			new EmploymentActionStepOperationalState(
				SelectedResources: $"supplier={preview.Shop.Id.ToString("F0", CultureInfo.InvariantCulture)};merchandise={preview.Merchandise.Id.ToString("F0", CultureInfo.InvariantCulture)};locations={locations}",
				OperationalPayload: $"supplier={preview.Shop.Id.ToString("F0", CultureInfo.InvariantCulture)};merchandise={preview.Merchandise.Id.ToString("F0", CultureInfo.InvariantCulture)};price={preview.Price.ToString("F2", CultureInfo.InvariantCulture)};quantity={preview.Quantity.ToString("F0", CultureInfo.InvariantCulture)};weight={(preview.CommodityWeight?.ToString("F2", CultureInfo.InvariantCulture) ?? string.Empty)}"));
	}
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

		var payload = operationalState.OperationalPayload ?? string.Empty;
		var failed = payload.Contains("craft-status=failed", StringComparison.InvariantCultureIgnoreCase);
		var completed = !payload.Contains("craft-status=inprogress", StringComparison.InvariantCultureIgnoreCase);
		return new EmploymentActionStepResult(!failed,
			failed ? reason : completed ? "Completed craft " + CraftDescription + "." : "Started or resumed craft " + CraftDescription + ".",
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

public sealed class BankAccountTransferActionStep : EmploymentActionStepBase
{
	public BankAccountTransferActionStep(string targetAccountKey, MoneyAmount amount, string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.AccountTransfer,
			EmploymentAuthority.UseStoreAccount,
			new[] { EmploymentAICapability.CanUseBankAccount },
			true,
			true)
	{
		TargetAccountKey = targetAccountKey;
		Amount = amount;
		ExistingFinancialRecord = existingFinancialRecord;
	}

	public string TargetAccountKey { get; }
	public MoneyAmount Amount { get; }
	public string? ExistingFinancialRecord { get; }
	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return context.CanBankTransfer(TargetAccountKey, Amount, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!context.TryBankTransfer(actor, TargetAccountKey, Amount, out var reason, out var operationalState))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, Amount,
				$"Reused existing account transfer record {ExistingFinancialRecord}.");
		}

		return new EmploymentActionStepResult(true,
			$"Transferred {Amount.Currency.Describe(Amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} from the linked bank account to {TargetAccountKey}.",
			true,
			operationalState);
	}
}

public sealed class BankAdministrationActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public BankAdministrationActionStep(IBank bank, BankAdministrationActionKind operation, MoneyAmount? amount = null,
		string? accountSelector = null, BankAccountStatus? targetStatus = null, ICell? sourceBranch = null,
		ICell? destinationBranch = null, string? reason = null)
		: base(
			EmploymentActionStepType.BankAdministration,
			RequiredAuthorityFor(operation),
			RequiredCapabilitiesFor(operation),
			RequiresPaymentAuthorisationFor(operation),
			RequiresPaymentAuthorisationFor(operation))
	{
		Bank = bank;
		Operation = operation;
		Amount = amount;
		AccountSelector = string.IsNullOrWhiteSpace(accountSelector) ? null : accountSelector.Trim();
		TargetStatus = targetStatus;
		SourceBranch = sourceBranch;
		DestinationBranch = destinationBranch;
		Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
	}

	public IBank Bank { get; }
	public BankAdministrationActionKind Operation { get; }
	public MoneyAmount? Amount { get; }
	public string? AccountSelector { get; }
	public BankAccountStatus? TargetStatus { get; }
	public ICell? SourceBranch { get; }
	public ICell? DestinationBranch { get; }
	public string? Reason { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!IsBankHost(context))
		{
			reason = "Bank administration steps must execute from the owning bank employment task board.";
			return false;
		}

		switch (Operation)
		{
			case BankAdministrationActionKind.ReserveAudit:
				reason = string.Empty;
				return true;
			case BankAdministrationActionKind.ReserveDeposit:
				if (!ValidateAmount(out reason))
				{
					return false;
				}

				var virtualBalance = VirtualCashLedger.Balance(Bank, Amount!.Currency);
				if (virtualBalance < Amount.Amount)
				{
					reason = $"{Bank.Name} only has {Amount.Currency.Describe(virtualBalance, CurrencyDescriptionPatternType.ShortDecimal)} in its employment virtual-cash counterbalance.";
					return false;
				}

				reason = string.Empty;
				return true;
			case BankAdministrationActionKind.ReserveWithdrawal:
				if (!ValidateAmount(out reason))
				{
					return false;
				}

				if (Bank.CurrencyReserves[Amount!.Currency] < Amount.Amount)
				{
					reason = $"{Bank.Name} only has {Amount.Currency.Describe(Bank.CurrencyReserves[Amount.Currency], CurrencyDescriptionPatternType.ShortDecimal)} in native reserves.";
					return false;
				}

				reason = string.Empty;
				return true;
			case BankAdministrationActionKind.AccountCredit:
				if (!ValidateAmount(out reason))
				{
					return false;
				}

				var creditAccount = ResolveAccount();
				if (creditAccount is null)
				{
					reason = MissingAccountReason();
					return false;
				}

				if (creditAccount.AccountStatus != BankAccountStatus.Active)
				{
					reason = $"Bank account {creditAccount.AccountReference} is {creditAccount.AccountStatus.DescribeEnum()} and cannot be credited.";
					return false;
				}

				if (string.IsNullOrWhiteSpace(Reason))
				{
					reason = "Account-credit bank administration steps require an auditable reason.";
					return false;
				}

				reason = string.Empty;
				return true;
			case BankAdministrationActionKind.AccountStatus:
				var statusAccount = ResolveAccount();
				if (statusAccount is null)
				{
					reason = MissingAccountReason();
					return false;
				}

				if (!TargetStatus.HasValue)
				{
					reason = "Account-status bank administration steps require a target status.";
					return false;
				}

				if (TargetStatus.Value == BankAccountStatus.Closed)
				{
					reason = "Use bankadmin account close to close bank accounts with an auditable close reason.";
					return false;
				}

				if (statusAccount.AccountStatus == TargetStatus.Value)
				{
					reason = $"Bank account {statusAccount.AccountReference} is already {TargetStatus.Value.DescribeEnum()}.";
					return false;
				}

				reason = string.Empty;
				return true;
			case BankAdministrationActionKind.AccountClose:
				var closeAccount = ResolveAccount();
				if (closeAccount is null)
				{
					reason = MissingAccountReason();
					return false;
				}

				if (closeAccount.AccountStatus == BankAccountStatus.Closed)
				{
					reason = $"Bank account {closeAccount.AccountReference} is already closed.";
					return false;
				}

				if (string.IsNullOrWhiteSpace(Reason))
				{
					reason = "Account-close bank administration steps require an auditable reason.";
					return false;
				}

				reason = string.Empty;
				return true;
			case BankAdministrationActionKind.BranchPost:
				if (!ValidateBranch(SourceBranch, "branch post", out reason))
				{
					return false;
				}

				if (!context.CanPath(actor, SourceBranch))
				{
					reason = "The assigned employee cannot path to the bank branch.";
					return false;
				}

				if (string.IsNullOrWhiteSpace(Reason))
				{
					reason = "Branch-post bank administration steps require an auditable note.";
					return false;
				}

				reason = string.Empty;
				return true;
			case BankAdministrationActionKind.BranchCourier:
				if (!ValidateBranch(SourceBranch, "source branch", out reason) ||
				    !ValidateBranch(DestinationBranch, "destination branch", out reason))
				{
					return false;
				}

				if (SourceBranch!.Id == DestinationBranch!.Id)
				{
					reason = "Bank branch courier evidence requires two different branches.";
					return false;
				}

				if (!context.CanPath(actor, SourceBranch) || !context.CanPath(actor, DestinationBranch))
				{
					reason = "The assigned employee cannot path to both bank branches.";
					return false;
				}

				if (string.IsNullOrWhiteSpace(Reason))
				{
					reason = "Branch-courier bank administration steps require an auditable note.";
					return false;
				}

				reason = string.Empty;
				return true;
			default:
				reason = "Unsupported bank administration operation.";
				return false;
		}
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		switch (Operation)
		{
			case BankAdministrationActionKind.ReserveAudit:
			{
				var description = $"Recorded bank reserve audit for {Bank.Name}: {ReserveSummary()}.";
				RecordBankAudit(actor, description);
				context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
					CurrentCorrelationId(context));
				return Completed(description, "reserve-audit");
			}
			case BankAdministrationActionKind.ReserveDeposit:
			{
				var reference = ReferenceText(context, "employment bank reserve deposit");
				if (!VirtualCashLedger.Debit(Bank, Amount!.Currency, Amount.Amount, actor, Bank, "BankReserve", reference,
					null, EmploymentClock.CurrentDateTime(context.Employer), out reason, Bank, reference))
				{
					return EmploymentActionStepResult.Blocked(reason);
				}

				Bank.CurrencyReserves[Amount.Currency] += Amount.Amount;
				Bank.Changed = true;
				var description = $"Deposited {DescribeAmount(Amount)} from bank employment virtual cash into native reserves: {reference}.";
				RecordBankAudit(actor, description);
				context.RecordLedger(EmploymentLedgerEntryType.BankDeposit, actor, Amount, description,
					CurrentCorrelationId(context));
				context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor, description,
					CurrentCorrelationId(context));
				return Completed(description, "reserve-deposit", reference);
			}
			case BankAdministrationActionKind.ReserveWithdrawal:
			{
				var reference = ReferenceText(context, "employment bank reserve withdrawal");
				Bank.CurrencyReserves[Amount!.Currency] -= Amount.Amount;
				Bank.Changed = true;
				VirtualCashLedger.Credit(Bank, Amount.Currency, Amount.Amount, actor, Bank, "BankReserve", reference,
					EmploymentClock.CurrentDateTime(context.Employer), Bank, reference);
				var description = $"Withdrew {DescribeAmount(Amount)} from native reserves into bank employment virtual cash: {reference}.";
				RecordBankAudit(actor, description);
				context.RecordLedger(EmploymentLedgerEntryType.BankWithdrawal, actor, Amount, description,
					CurrentCorrelationId(context));
				context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor, description,
					CurrentCorrelationId(context));
				return Completed(description, "reserve-withdrawal", reference);
			}
			case BankAdministrationActionKind.AccountCredit:
			{
				var account = ResolveAccount()!;
				account.DoAccountCredit(Amount!.Amount, Reason!);
				var description = $"Credited bank account {account.AccountReference} by {DescribeAmount(Amount)}: {Reason}.";
				RecordBankAudit(actor, description);
				context.RecordLedger(EmploymentLedgerEntryType.BankAccountCredit, actor, Amount, description,
					CurrentCorrelationId(context));
				context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor, description,
					CurrentCorrelationId(context));
				return Completed(description, "account-credit", account.AccountReference);
			}
			case BankAdministrationActionKind.AccountStatus:
			{
				var account = ResolveAccount()!;
				var previous = account.AccountStatus;
				account.SetStatus(TargetStatus!.Value);
				var description = $"Changed bank account {account.AccountReference} from {previous.DescribeEnum()} to {TargetStatus.Value.DescribeEnum()}{(string.IsNullOrWhiteSpace(Reason) ? "." : $": {Reason}.")}";
				RecordBankAudit(actor, description);
				context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
					CurrentCorrelationId(context));
				return Completed(description, "account-status", account.AccountReference);
			}
			case BankAdministrationActionKind.AccountClose:
			{
				var account = ResolveAccount()!;
				account.SetStatus(BankAccountStatus.Closed);
				var description = $"Closed bank account {account.AccountReference}: {Reason}.";
				RecordBankAudit(actor, description);
				context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
					CurrentCorrelationId(context));
				return Completed(description, "account-close", account.AccountReference);
			}
			case BankAdministrationActionKind.BranchPost:
			{
				var description = $"Recorded branch post at {SourceBranch!.GetFriendlyReference(actor)} for {Bank.Name}: {Reason}.";
				RecordBankAudit(actor, description);
				context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
					CurrentCorrelationId(context));
				return Completed(description, "branch-post", SourceBranch.Id.ToString("F0", CultureInfo.InvariantCulture));
			}
			case BankAdministrationActionKind.BranchCourier:
			{
				var description = $"Recorded branch courier run from {SourceBranch!.GetFriendlyReference(actor)} to {DestinationBranch!.GetFriendlyReference(actor)} for {Bank.Name}: {Reason}.";
				RecordBankAudit(actor, description);
				context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
					CurrentCorrelationId(context));
				return Completed(description, "branch-courier", $"{SourceBranch.Id.ToString("F0", CultureInfo.InvariantCulture)}->{DestinationBranch.Id.ToString("F0", CultureInfo.InvariantCulture)}");
			}
			default:
				return EmploymentActionStepResult.Blocked("Unsupported bank administration operation.");
		}
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return Operation switch
		{
			BankAdministrationActionKind.BranchPost when SourceBranch is not null => [SourceBranch],
			BankAdministrationActionKind.BranchCourier when SourceBranch is not null && DestinationBranch is not null =>
				[SourceBranch, DestinationBranch],
			_ => []
		};
	}

	private bool IsBankHost(IEmploymentTaskContext context)
	{
		return context.Employer.Id == Bank.Id &&
		       context.Employer.FrameworkItemType.Equals(Bank.FrameworkItemType, StringComparison.OrdinalIgnoreCase);
	}

	private bool ValidateAmount(out string reason)
	{
		if (Amount is null || Amount.Amount <= 0.0M)
		{
			reason = "Bank administration money movements require a positive amount.";
			return false;
		}

		if (Amount.Currency.Id != Bank.PrimaryCurrency.Id)
		{
			reason = $"{Bank.Name} uses {Bank.PrimaryCurrency.Name}, but this bank administration step is for {Amount.Currency.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private bool ValidateBranch(ICell? branch, string label, out string reason)
	{
		if (branch is null)
		{
			reason = $"Bank administration {label} steps require a branch location.";
			return false;
		}

		if (Bank.BranchLocations.All(x => x.Id != branch.Id))
		{
			reason = $"{branch.Name} is not a branch of {Bank.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private IBankAccount? ResolveAccount()
	{
		if (string.IsNullOrWhiteSpace(AccountSelector))
		{
			return null;
		}

		var selector = AccountSelector.Trim();
		var numeric = selector.TrimStart('#');
		if (int.TryParse(numeric, NumberStyles.Integer, CultureInfo.InvariantCulture, out var accountNumber))
		{
			var byNumber = Bank.BankAccounts.FirstOrDefault(x => x.AccountNumber == accountNumber);
			if (byNumber is not null)
			{
				return byNumber;
			}
		}

		if (long.TryParse(numeric, NumberStyles.Integer, CultureInfo.InvariantCulture, out var accountId))
		{
			var byId = Bank.BankAccounts.FirstOrDefault(x => x.Id == accountId);
			if (byId is not null)
			{
				return byId;
			}
		}

		return Bank.BankAccounts.FirstOrDefault(x => x.AccountReference.EqualTo(selector)) ??
		       Bank.BankAccounts.FirstOrDefault(x => x.Name.EqualTo(selector)) ??
		       Bank.BankAccounts.FirstOrDefault(x => x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
	}

	private string MissingAccountReason()
	{
		return string.IsNullOrWhiteSpace(AccountSelector)
			? "Bank account administration steps require an account selector."
			: $"There is no bank account at {Bank.Name} matching {AccountSelector}.";
	}

	private string ReserveSummary()
	{
		var reserveText = Bank.CurrencyReserves
		                      .Select(x => $"{x.Key.Name}={x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal)}")
		                      .DefaultIfEmpty("no reserves")
		                      .ListToString();
		var virtualCash = VirtualCashLedger.Balance(Bank, Bank.PrimaryCurrency);
		return $"reserves {reserveText}; employment virtual cash {Bank.PrimaryCurrency.Describe(virtualCash, CurrencyDescriptionPatternType.ShortDecimal)}";
	}

	private void RecordBankAudit(ICharacter actor, string description)
	{
		Bank.RecordManagerAuditLog(actor, description);
	}

	private EmploymentActionStepResult Completed(string description, string operation, string? transactionReference = null)
	{
		return new EmploymentActionStepResult(true, description, true,
			new EmploymentActionStepOperationalState(
				TransactionReference: transactionReference,
				SelectedResources: $"bankadmin:{operation};bank={Bank.Id.ToString("F0", CultureInfo.InvariantCulture)}",
				OperationalPayload: OperationalPayload(operation, transactionReference)));
	}

	private string OperationalPayload(string operation, string? transactionReference)
	{
		return string.Join(";", new[]
		{
			$"bankadmin:{operation}",
			$"bank={Bank.Id.ToString("F0", CultureInfo.InvariantCulture)}",
			Amount is null ? null : $"amount={Amount.Amount.ToString(CultureInfo.InvariantCulture)}",
			Amount is null ? null : $"currency={Amount.Currency.Id.ToString("F0", CultureInfo.InvariantCulture)}",
			string.IsNullOrWhiteSpace(AccountSelector) ? null : $"account={AccountSelector}",
			TargetStatus is null ? null : $"status={TargetStatus.Value}",
			SourceBranch is null ? null : $"source={SourceBranch.Id.ToString("F0", CultureInfo.InvariantCulture)}",
			DestinationBranch is null ? null : $"destination={DestinationBranch.Id.ToString("F0", CultureInfo.InvariantCulture)}",
			string.IsNullOrWhiteSpace(transactionReference) ? null : $"reference={transactionReference}"
		}.Where(x => !string.IsNullOrWhiteSpace(x))!);
	}

	private static string DescribeAmount(MoneyAmount amount)
	{
		return amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal);
	}

	private static string ReferenceText(IEmploymentTaskContext context, string action)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? $"{action} {concrete.CurrentTask.CorrelationId:D}"
			: action;
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
	}

	private static bool RequiresPaymentAuthorisationFor(BankAdministrationActionKind operation)
	{
		return operation is BankAdministrationActionKind.ReserveDeposit or
		       BankAdministrationActionKind.ReserveWithdrawal or
		       BankAdministrationActionKind.AccountCredit;
	}

	private static EmploymentAuthority RequiredAuthorityFor(BankAdministrationActionKind operation)
	{
		return operation switch
		{
			BankAdministrationActionKind.ReserveAudit => EmploymentAuthority.DepositBusinessCash | EmploymentAuthority.WithdrawBusinessCash,
			BankAdministrationActionKind.ReserveDeposit => EmploymentAuthority.DepositBusinessCash,
			BankAdministrationActionKind.ReserveWithdrawal => EmploymentAuthority.WithdrawBusinessCash,
			BankAdministrationActionKind.AccountCredit => EmploymentAuthority.UseStoreAccount,
			BankAdministrationActionKind.AccountStatus => EmploymentAuthority.UseStoreAccount,
			BankAdministrationActionKind.AccountClose => EmploymentAuthority.UseStoreAccount,
			BankAdministrationActionKind.BranchPost => EmploymentAuthority.ManageDeliveryRoutes,
			BankAdministrationActionKind.BranchCourier => EmploymentAuthority.ManageDeliveryRoutes,
			_ => EmploymentAuthority.AssignTasks
		};
	}

	private static IEnumerable<EmploymentAICapability> RequiredCapabilitiesFor(BankAdministrationActionKind operation)
	{
		return operation switch
		{
			BankAdministrationActionKind.ReserveDeposit or BankAdministrationActionKind.ReserveWithdrawal =>
				new[] { EmploymentAICapability.CanHandleCash },
			BankAdministrationActionKind.AccountCredit or BankAdministrationActionKind.AccountStatus or BankAdministrationActionKind.AccountClose =>
				new[] { EmploymentAICapability.CanUseBankAccount },
			BankAdministrationActionKind.BranchPost or BankAdministrationActionKind.BranchCourier =>
				new[] { EmploymentAICapability.CanDeliverItems },
			_ => Array.Empty<EmploymentAICapability>()
		};
	}
}
public sealed class HostSettlementActionStep : EmploymentActionStepBase
{
	public HostSettlementActionStep(string targetHostKey, MoneyAmount amount, string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.HostSettlement,
			EmploymentAuthority.SettleHostAccounts,
			new[] { EmploymentAICapability.CanUseBankAccount },
			true,
			true)
	{
		TargetHostKey = targetHostKey;
		Amount = amount;
		ExistingFinancialRecord = existingFinancialRecord;
	}

	public string TargetHostKey { get; }
	public MoneyAmount Amount { get; }
	public string? ExistingFinancialRecord { get; }
	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return context.CanHostSettlement(TargetHostKey, Amount, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!context.TryHostSettlement(actor, TargetHostKey, Amount, out var reason, out var operationalState))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, Amount,
				$"Reused existing host settlement record {ExistingFinancialRecord}.");
		}

		return new EmploymentActionStepResult(true,
			$"Settled {Amount.Currency.Describe(Amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} to {TargetHostKey}.",
			true,
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

public sealed class ShopCashReconciliationActionStep : EmploymentActionStepBase
{
	public ShopCashReconciliationActionStep(string? note = null)
		: base(
			EmploymentActionStepType.ShopCashReconciliation,
			EmploymentAuthority.WithdrawBusinessCash | EmploymentAuthority.DepositBusinessCash,
			new[] { EmploymentAICapability.CanHandleCash },
			false,
			false)
	{
		Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
	}

	public string? Note { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (context.Employer is not IShop)
		{
			reason = "Cash reconciliation steps can only target shop employment hosts.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (context.Employer is not IShop shop)
		{
			return EmploymentActionStepResult.Blocked("Cash reconciliation steps can only target shop employment hosts.");
		}

		var expectedBefore = shop.ExpectedCashBalance;
		var virtualBefore = shop.CashBalance;
		var physicalBefore = SameCurrencyPhysicalCash(shop);
		var actualBefore = virtualBefore + physicalBefore;
		var varianceBefore = actualBefore - expectedBefore;
		shop.CheckFloat();
		var expectedAfter = shop.ExpectedCashBalance;
		var varianceAfter = actualBefore - expectedAfter;
		var note = string.IsNullOrWhiteSpace(Note) ? string.Empty : $" Note: {Note}";
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Reconciled cash for {shop.Name}: expected {Describe(shop, expectedBefore, actor)}, virtual {Describe(shop, virtualBefore, actor)}, physical {Describe(shop, physicalBefore, actor)}, variance {Describe(shop, varianceBefore, actor)}, adjusted expected {Describe(shop, expectedAfter, actor)}.{note}");

		return new EmploymentActionStepResult(
			true,
			$"Reconciled cash for {shop.Name}: variance {Describe(shop, varianceBefore, actor)}.",
			true,
			new EmploymentActionStepOperationalState(
				OperationalPayload: $"cashreconcile:expected={expectedBefore.ToString("F2", CultureInfo.InvariantCulture)};virtual={virtualBefore.ToString("F2", CultureInfo.InvariantCulture)};physical={physicalBefore.ToString("F2", CultureInfo.InvariantCulture)};variance={varianceBefore.ToString("F2", CultureInfo.InvariantCulture)};adjusted={expectedAfter.ToString("F2", CultureInfo.InvariantCulture)};aftervariance={varianceAfter.ToString("F2", CultureInfo.InvariantCulture)}",
				SelectedResources: $"cash:{shop.Id.ToString("F0", CultureInfo.InvariantCulture)}:{shop.Name}"));
	}

	private static decimal SameCurrencyPhysicalCash(IShop shop)
	{
		return shop.GetCurrencyPilesForShop()
		           .Where(x => x.Currency == shop.Currency)
		           .Sum(x => x.TotalValue);
	}

	private static string Describe(IShop shop, decimal amount, IFormatProvider voyeur)
	{
		return shop.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
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

	public CataloguedActionShellStep(string actionKey, string actionDescription, IEnumerable<ICell> routeStops)
		: this(actionKey, actionDescription, null, routeStops.LastOrDefault(), routeStops)
	{
	}

	public CataloguedActionShellStep(string actionKey, string actionDescription, MoneyAmount? amount,
		ICell? targetLocation = null, IEnumerable<ICell>? routeStops = null)
		: this(EmploymentActionCatalog.Get(actionKey) ??
		       throw new ArgumentException($"Unknown employment action catalogue key {actionKey}.", nameof(actionKey)),
		actionDescription,
		amount,
		targetLocation,
		routeStops)
	{
	}

	private CataloguedActionShellStep(EmploymentActionDefinition definition, string actionDescription,
		MoneyAmount? amount, ICell? targetLocation, IEnumerable<ICell>? routeStops)
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
		var routeStopList = routeStops?.Where(x => x is not null).ToList() ?? [];
		if (routeStopList.Count == 0 && definition.Key.EqualTo("route") && targetLocation is not null)
		{
			routeStopList.Add(targetLocation);
		}

		RouteStops = routeStopList;
		TargetLocation = targetLocation ?? routeStopList.LastOrDefault();
	}

	public string ActionKey { get; }
	public string ActionDescription { get; }
	public MoneyAmount? Amount { get; }
	public ICell? TargetLocation { get; }
	public IReadOnlyList<ICell> RouteStops { get; }
	public EmploymentActionDefinition Definition { get; }

	private sealed record BatchPlan(decimal DemandTarget, decimal StorageCapacity, decimal BatchSize, string Rationale);
	private sealed record RouteBatchPlan(decimal TotalQuantity, decimal PerStopQuantity, string Rationale);
	private sealed record TripCheckPlan(string FuelPolicy, string FeedPolicy, string MaintenancePolicy, string RestPolicy, string Rationale);


	private static bool TryParseBatchPlan(string description, IFormatProvider formatProvider, out BatchPlan? plan,
		out string reason)
	{
		plan = null;
		var input = new StringStack(description);
		if (input.IsFinished || !input.PopSpeech().EqualToAny("demand", "target"))
		{
			reason = "Batch steps use the syntax: tasks step batch demand <target> storage <capacity> size <quantity> <rationale>.";
			return false;
		}

		if (input.IsFinished || !TryParsePositiveBatchNumber(input.PopSpeech(), formatProvider, out var demandTarget))
		{
			reason = "Batch steps need a positive demand target after the demand keyword.";
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualToAny("storage", "capacity", "space"))
		{
			reason = "Batch steps need a storage capacity after the demand target.";
			return false;
		}

		if (input.IsFinished || !TryParsePositiveBatchNumber(input.PopSpeech(), formatProvider, out var storageCapacity))
		{
			reason = "Batch steps need a positive storage capacity.";
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualToAny("size", "batch", "quantity"))
		{
			reason = "Batch steps need a planned batch size after the storage capacity.";
			return false;
		}

		if (input.IsFinished || !TryParsePositiveBatchNumber(input.PopSpeech(), formatProvider, out var batchSize))
		{
			reason = "Batch steps need a positive planned batch size.";
			return false;
		}

		var rationale = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(rationale))
		{
			reason = "Batch steps need a short rationale after the planned size.";
			return false;
		}

		if (batchSize > demandTarget)
		{
			reason = $"The planned batch size {batchSize.ToString("N2", formatProvider)} exceeds the stated demand target {demandTarget.ToString("N2", formatProvider)}.";
			return false;
		}

		if (batchSize > storageCapacity)
		{
			reason = $"The planned batch size {batchSize.ToString("N2", formatProvider)} exceeds the stated storage capacity {storageCapacity.ToString("N2", formatProvider)}.";
			return false;
		}

		plan = new BatchPlan(demandTarget, storageCapacity, batchSize, rationale);
		reason = string.Empty;
		return true;
	}
	private static bool TryParseRouteBatchPlan(string description, IFormatProvider formatProvider,
		out RouteBatchPlan? plan, out string reason)
	{
		plan = null;
		var input = new StringStack(description);
		if (input.IsFinished || !input.PopSpeech().EqualTo("total"))
		{
			reason = "Route batch steps use the syntax: tasks step routebatch total <quantity> each <quantity> to <stops...> <rationale>.";
			return false;
		}

		if (input.IsFinished || !TryParsePositiveBatchNumber(input.PopSpeech(), formatProvider, out var totalQuantity))
		{
			reason = "Route batch steps need a positive total quantity.";
			return false;
		}

		if (input.IsFinished || !input.PopSpeech().EqualToAny("each", "per", "perstop"))
		{
			reason = "Route batch steps need an each quantity after the total quantity.";
			return false;
		}

		if (input.IsFinished || !TryParsePositiveBatchNumber(input.PopSpeech(), formatProvider, out var perStopQuantity))
		{
			reason = "Route batch steps need a positive per-stop quantity.";
			return false;
		}

		var rationale = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(rationale))
		{
			reason = "Route batch steps need a short rationale after the per-stop quantity.";
			return false;
		}

		plan = new RouteBatchPlan(totalQuantity, perStopQuantity, rationale);
		reason = string.Empty;
		return true;
	}

	private static bool TryParseTripCheckPlan(string description, out TripCheckPlan? plan, out string reason)
	{
		plan = null;
		var input = new StringStack(description);
		if (!TryPopTripCheckValue(input, "fuel", out var fuelPolicy, out reason, "refuel"))
		{
			return false;
		}

		if (!TryPopTripCheckValue(input, "feed", out var feedPolicy, out reason, "fodder"))
		{
			return false;
		}

		if (!TryPopTripCheckValue(input, "maintenance", out var maintenancePolicy, out reason, "maint", "service"))
		{
			return false;
		}

		if (!TryPopTripCheckValue(input, "rest", out var restPolicy, out reason, "break"))
		{
			return false;
		}

		var rationale = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(rationale))
		{
			reason = "Trip check steps need a short rationale after the rest policy.";
			return false;
		}

		plan = new TripCheckPlan(fuelPolicy, feedPolicy, maintenancePolicy, restPolicy, rationale);
		reason = string.Empty;
		return true;
	}

	private static bool TryPopTripCheckValue(StringStack input, string keyword, out string value, out string reason,
		params string[] aliases)
	{
		value = string.Empty;
		if (input.IsFinished)
		{
			reason = $"Trip check steps need a {keyword} policy.";
			return false;
		}

		var actualKeyword = input.PopSpeech();
		if (!actualKeyword.EqualTo(keyword) && !aliases.Any(x => actualKeyword.EqualTo(x)))
		{
			reason = "Trip check steps use the syntax: tasks step tripcheck fuel <policy> feed <policy> maintenance <policy> rest <policy> [to <stops...>] <rationale>.";
			return false;
		}

		if (input.IsFinished)
		{
			reason = $"Trip check steps need a {keyword} policy value.";
			return false;
		}

		value = input.PopSpeech().Trim();
		if (string.IsNullOrWhiteSpace(value))
		{
			reason = $"Trip check steps need a {keyword} policy value.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static bool TryParsePositiveBatchNumber(string text, IFormatProvider formatProvider, out decimal value)
	{
		return decimal.TryParse(text, NumberStyles.Number, formatProvider, out value) && value > 0.0M;
	}

	private static string FormatBatchNumber(decimal value)
	{
		return value.ToString("0.###", CultureInfo.InvariantCulture);
	}

	private static EmploymentActionStepOperationalState BatchPlanState(BatchPlan plan, ICharacter actor)
	{
		var demand = FormatBatchNumber(plan.DemandTarget);
		var storage = FormatBatchNumber(plan.StorageCapacity);
		var size = FormatBatchNumber(plan.BatchSize);
		var actorIdentity = CharacterInstanceIdentityComparer.IdentityId(actor);
		var actorKey = CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor);
		return new EmploymentActionStepOperationalState(
			OperationalPayload: $"Batch plan by {actorIdentity}: demand={demand};storage={storage};size={size};note={plan.Rationale}",
			SelectedResources: $"operation=batch;actor={actorKey};demand={demand};storage={storage};size={size};note={plan.Rationale}");
	}

	private EmploymentActionStepOperationalState RoutePlanState(ICharacter actor)
	{
		var stops = RouteStops.Count > 0
			? RouteStops.ToList()
			: TargetLocation is null ? new List<ICell>() : new List<ICell> { TargetLocation };
		if (stops.Count == 0)
		{
			return new EmploymentActionStepOperationalState(RouteResult: ActionDescription);
		}

		var stopIds = string.Join(",", stops.Select(x => x.Id.ToString("F0", CultureInfo.InvariantCulture)));
		var final = stops[^1];
		var finalId = final.Id.ToString("F0", CultureInfo.InvariantCulture);
		var actorIdentity = CharacterInstanceIdentityComparer.IdentityId(actor);
		var actorKey = CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor);
		var stopDescription = string.Join(" -> ", stops.Select(x => $"{x.Id.ToString("F0", CultureInfo.InvariantCulture)}:{x.Name}"));
		return new EmploymentActionStepOperationalState(
			OperationalPayload: $"Route plan by {actorIdentity}: {stopDescription}; note={ActionDescription}",
			SelectedResources: $"operation=route;actor={actorKey};stops={stopIds};final={finalId}",
			RouteResult: $"Route plan by {actorIdentity}: stops={stopIds};final={finalId};note={ActionDescription}");
	}

	private EmploymentActionStepOperationalState RouteBatchPlanState(RouteBatchPlan plan, ICharacter actor)
	{
		var stops = RouteStops.ToList();
		var total = FormatBatchNumber(plan.TotalQuantity);
		var each = FormatBatchNumber(plan.PerStopQuantity);
		var planned = FormatBatchNumber(plan.PerStopQuantity * stops.Count);
		var remainder = FormatBatchNumber(plan.TotalQuantity - plan.PerStopQuantity * stops.Count);
		var stopIds = string.Join(",", stops.Select(x => x.Id.ToString("F0", CultureInfo.InvariantCulture)));
		var final = stops[^1].Id.ToString("F0", CultureInfo.InvariantCulture);
		var actorIdentity = CharacterInstanceIdentityComparer.IdentityId(actor);
		var actorKey = CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor);
		return new EmploymentActionStepOperationalState(
			OperationalPayload: $"Route batch by {actorIdentity}: stops={stopIds};total={total};each={each};planned={planned};remainder={remainder};note={plan.Rationale}",
			SelectedResources: $"operation=routebatch;actor={actorKey};stops={stopIds};final={final};total={total};each={each};planned={planned};remainder={remainder}",
			RouteResult: $"Route batch by {actorIdentity}: stops={stopIds};final={final};total={total};each={each};planned={planned};remainder={remainder};note={plan.Rationale}");
	}

	private EmploymentActionStepOperationalState TripCheckState(TripCheckPlan plan, ICharacter actor)
	{
		var fuel = FormatPolicyValue(plan.FuelPolicy);
		var feed = FormatPolicyValue(plan.FeedPolicy);
		var maintenance = FormatPolicyValue(plan.MaintenancePolicy);
		var rest = FormatPolicyValue(plan.RestPolicy);
		var note = FormatPolicyValue(plan.Rationale);
		var stopIds = RouteStops.Count == 0
			? "none"
			: string.Join(",", RouteStops.Select(x => x.Id.ToString("F0", CultureInfo.InvariantCulture)));
		var final = RouteStops.Count == 0
			? "none"
			: RouteStops[^1].Id.ToString("F0", CultureInfo.InvariantCulture);
		var actorIdentity = CharacterInstanceIdentityComparer.IdentityId(actor);
		var actorKey = CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor);
		return new EmploymentActionStepOperationalState(
			OperationalPayload: $"Trip check by {actorIdentity}: fuel={fuel};feed={feed};maintenance={maintenance};rest={rest};stops={stopIds};note={note}",
			SelectedResources: $"operation=tripcheck;actor={actorKey};fuel={fuel};feed={feed};maintenance={maintenance};rest={rest};stops={stopIds};final={final}",
			RouteResult: RouteStops.Count == 0 ? null : $"Trip check by {actorIdentity}: stops={stopIds};final={final};note={note}");
	}

	private static string FormatPolicyValue(string value)
	{
		return value.Replace(';', ',').Trim();
	}

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

		if (ActionKey.EqualTo("inspect") && !context.CarriedTaskItems(actor).Any())
		{
			reason = "There are no task-custody items available to inspect.";
			return false;
		}

		if (ActionKey.EqualTo("batch") && !TryParseBatchPlan(ActionDescription, actor, out _, out reason))
		{
			return false;
		}

		if (ActionKey.EqualTo("tripcheck") && !TryParseTripCheckPlan(ActionDescription, out _, out reason))
		{
			return false;
		}

		if (ActionKey.EqualTo("routebatch"))
		{
			if (!TryParseRouteBatchPlan(ActionDescription, actor, out var routeBatchPlan, out reason))
			{
				return false;
			}

			if (RouteStops.Count < 2)
			{
				reason = "Route batch steps must include at least two destination stops.";
				return false;
			}

			var plannedQuantity = routeBatchPlan!.PerStopQuantity * RouteStops.Count;
			if (plannedQuantity > routeBatchPlan.TotalQuantity)
			{
				reason = $"The route batch allocates {plannedQuantity.ToString("N2", actor)} total quantity but only {routeBatchPlan.TotalQuantity.ToString("N2", actor)} is available.";
				return false;
			}
		}

		if (ActionKey.EqualToAny("route", "routebatch") || ActionKey.EqualTo("tripcheck") && RouteStops.Count > 0)
		{
			if (!ActionKey.EqualTo("tripcheck") && RouteStops.Count == 0)
			{
				reason = "Route steps must include at least one destination stop.";
				return false;
			}

			var unreachableStop = RouteStops.FirstOrDefault(x => !context.CanPath(actor, x));
			if (unreachableStop is not null)
			{
				reason = $"The assigned employee cannot path to route stop {unreachableStop.Id.ToString("N0", actor)} ({unreachableStop.Name}).";
				return false;
			}
		}
		else if (!ActionKey.EqualTo("tripcheck") && !context.CanPath(actor, TargetLocation))
		{
			reason = "The assigned employee cannot path to the action target location.";
			return false;
		}

		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		BatchPlan? batchPlan = null;
		if (ActionKey.EqualTo("batch") && !TryParseBatchPlan(ActionDescription, actor, out batchPlan, out var batchReason))
		{
			return EmploymentActionStepResult.Blocked(batchReason);
		}

		RouteBatchPlan? routeBatchPlan = null;
		if (ActionKey.EqualTo("routebatch") && !TryParseRouteBatchPlan(ActionDescription, actor, out routeBatchPlan, out var routeBatchReason))
		{
			return EmploymentActionStepResult.Blocked(routeBatchReason);
		}

		TripCheckPlan? tripCheckPlan = null;
		if (ActionKey.EqualTo("tripcheck") && !TryParseTripCheckPlan(ActionDescription, out tripCheckPlan, out var tripCheckReason))
		{
			return EmploymentActionStepResult.Blocked(tripCheckReason);
		}

		if (routeBatchPlan is not null)
		{
			if (RouteStops.Count < 2)
			{
				return EmploymentActionStepResult.Blocked("Route batch steps must include at least two destination stops.");
			}

			var plannedQuantity = routeBatchPlan.PerStopQuantity * RouteStops.Count;
			if (plannedQuantity > routeBatchPlan.TotalQuantity)
			{
				return EmploymentActionStepResult.Blocked($"The route batch allocates {plannedQuantity.ToString("N2", actor)} total quantity but only {routeBatchPlan.TotalQuantity.ToString("N2", actor)} is available.");
			}
		}

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
					? $"Authorised by {CharacterInstanceIdentityComparer.IdentityId(actor)}: {ActionDescription}"
					: $"Authorised by {CharacterInstanceIdentityComparer.IdentityId(actor)}: {Amount.Currency.Describe(Amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} for {ActionDescription}"),
			"reserve" => new EmploymentActionStepOperationalState(
				ReservationReference: $"Reserved by {CharacterInstanceIdentityComparer.IdentityId(actor)}: {ActionDescription}"),
			"select" => new EmploymentActionStepOperationalState(
				SelectedResources: ActionDescription),
			"estimate" => new EmploymentActionStepOperationalState(
				OperationalPayload: $"Estimate: {ActionDescription}"),
			"batch" when batchPlan is not null => BatchPlanState(batchPlan, actor),
			"route" => RoutePlanState(actor),
			"routebatch" when routeBatchPlan is not null => RouteBatchPlanState(routeBatchPlan, actor),
			"tripcheck" when tripCheckPlan is not null => TripCheckState(tripCheckPlan, actor),
			"inspect" => new EmploymentActionStepOperationalState(
				OperationalPayload: "Inspection by " + CharacterInstanceIdentityComparer.IdentityId(actor) + ": " + ActionDescription,
				SelectedResources: EmploymentTaskContext.FormatTaskItemCustody("inspect",
					CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor), context.CarriedTaskItems(actor))),
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
		if (ActionKey.EqualToAny("route", "routebatch", "tripcheck") && RouteStops.Count > 0)
		{
			return RouteStops;
		}

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
				SelectedResources: EmploymentTaskContext.FormatTaskItemCustody("deliver",
					CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor), carried)));
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

public sealed class ShopStockTransferActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public ShopStockTransferActionStep(IPermanentShop sourceShop, IPermanentShop targetShop,
		IMerchandise targetMerchandise, ICell destination, IGameItem? container = null, string? containerTag = null)
		: this(sourceShop, targetShop, targetMerchandise, destination,
			container is not null
				? EmploymentItemSelector.ForItem(container)
				: string.IsNullOrWhiteSpace(containerTag) ? null : EmploymentItemSelector.ForTag(containerTag))
	{
	}

	public ShopStockTransferActionStep(IPermanentShop sourceShop, IPermanentShop targetShop,
		IMerchandise targetMerchandise, ICell destination, EmploymentItemSelector? containerSelector)
		: base(
			EmploymentActionStepType.ShopStockTransfer,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		SourceShop = sourceShop;
		TargetShop = targetShop;
		TargetMerchandise = targetMerchandise;
		Destination = destination;
		ContainerSelector = containerSelector;
	}

	public IPermanentShop SourceShop { get; }
	public IPermanentShop TargetShop { get; }
	public IMerchandise TargetMerchandise { get; }
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

		if (context.Employer.Id != SourceShop.Id || context.Employer.FrameworkItemType != SourceShop.FrameworkItemType)
		{
			reason = "Shop stock transfers must execute from the source shop's employment task board.";
			return false;
		}

		if (!TargetShop.Merchandises.Any(x => x.Id == TargetMerchandise.Id))
		{
			reason = $"{TargetMerchandise.Name} is not merchandise for {TargetShop.Name}.";
			return false;
		}

		if (!context.CanPath(actor, Destination))
		{
			reason = "The assigned employee cannot path to the stock-transfer destination.";
			return false;
		}

		if (!context.CarriedTaskItems(actor).Any())
		{
			reason = "The assigned employee is not carrying any stock to transfer.";
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
		var transferItems = carried
		                    .SelectMany(TransferItemsFor)
		                    .DistinctBy(x => x.Id)
		                    .ToList();
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

		foreach (var item in transferItems)
		{
			var sourceEffect = item.EffectsOfType<ItemOnDisplayInShop>()
			                       .FirstOrDefault(x => x.Shop?.Id == SourceShop.Id);
			var alreadyTargetStock = item.EffectsOfType<ItemOnDisplayInShop>()
			                             .Any(x => x.Shop?.Id == TargetShop.Id && x.Merchandise?.Id == TargetMerchandise.Id);

			if (sourceEffect is not null &&
			    (SourceShop.Id != TargetShop.Id || sourceEffect.Merchandise?.Id != TargetMerchandise.Id))
			{
				SourceShop.DisposeFromStock(actor, item);
				alreadyTargetStock = false;
			}

			if (!alreadyTargetStock)
			{
				TargetShop.AddToStock(actor, item, TargetMerchandise);
			}
		}

		var correlationId = context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: Guid.NewGuid();
		var itemList = transferItems.Select(x => x.Id.ToString("F0", CultureInfo.InvariantCulture)).ListToCommaSeparatedValues();
		var count = transferItems.Sum(x => x.Quantity);
		var description = $"Transferred {count.ToString("N0", actor)} stock item{(count == 1 ? string.Empty : "s")} from {SourceShop.Name} to {TargetShop.Name} as {TargetMerchandise.Name}.";
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description, correlationId);
		if (SourceShop.Id != TargetShop.Id)
		{
			TargetShop.EmploymentRegister.Record(EmploymentRegisterEntryType.AuditActionRecorded, actor,
				$"Received {count.ToString("N0", actor)} stock item{(count == 1 ? string.Empty : "s")} from {SourceShop.Name} as {TargetMerchandise.Name}.",
				correlationId);
		}

		return new EmploymentActionStepResult(true, description, true,
			new EmploymentActionStepOperationalState(
				SelectedResources: $"stocktransfer:source={SourceShop.Id.ToString("F0", CultureInfo.InvariantCulture)};target={TargetShop.Id.ToString("F0", CultureInfo.InvariantCulture)};targetmerchandise={TargetMerchandise.Id.ToString("F0", CultureInfo.InvariantCulture)};destination={Destination.Id.ToString("F0", CultureInfo.InvariantCulture)};items={itemList}",
				OperationalPayload: $"stocktransfer:source={SourceShop.Id.ToString("F0", CultureInfo.InvariantCulture)};target={TargetShop.Id.ToString("F0", CultureInfo.InvariantCulture)};targetmerchandise={TargetMerchandise.Id.ToString("F0", CultureInfo.InvariantCulture)};count={count.ToString("F0", CultureInfo.InvariantCulture)};items={itemList};container={(container?.Id.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty)}"));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return [Destination];
	}

	private IGameItem? ResolveContainer(IEmploymentTaskContext context, ICharacter actor)
	{
		return EmploymentItemSelectorResolver.Resolve(context, actor, ContainerSelector, Destination, false);
	}

	private static IEnumerable<IGameItem> TransferItemsFor(IGameItem item)
	{
		return item.GetItemType<PileGameItemComponent>()?.Contents ?? [item];
	}
}

public sealed class AuctionLotListingActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public AuctionLotListingActionStep(IAuctionHouse auctionHouse, EmploymentItemSelector itemSelector,
		MoneyAmount reservePrice, MoneyAmount? buyoutPrice = null, TimeSpan? duration = null)
		: base(
			EmploymentActionStepType.AuctionLotListing,
			EmploymentAuthority.ManageStockRules,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		AuctionHouse = auctionHouse;
		ItemSelector = itemSelector;
		ReservePrice = reservePrice;
		BuyoutPrice = buyoutPrice;
		Duration = duration;
	}

	public IAuctionHouse AuctionHouse { get; }
	public EmploymentItemSelector ItemSelector { get; }
	public MoneyAmount ReservePrice { get; }
	public MoneyAmount? BuyoutPrice { get; }
	public TimeSpan? Duration { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!IsAuctionHost(context))
		{
			reason = "Auction lot listing steps must execute from the owning auction house employment task board.";
			return false;
		}

		if (ReservePrice.Currency != AuctionHouse.EconomicZone.Currency ||
		    BuyoutPrice is not null && BuyoutPrice.Currency != AuctionHouse.EconomicZone.Currency)
		{
			reason = $"Auction lot prices must use {AuctionHouse.EconomicZone.Currency.Name}.";
			return false;
		}

		if (ReservePrice.Amount <= AuctionHouse.AuctionListingFeeFlat)
		{
			reason = "Auction lot reserve prices must exceed the auction house flat listing fee.";
			return false;
		}

		if (BuyoutPrice is not null && BuyoutPrice.Amount < ReservePrice.Amount)
		{
			reason = "Auction lot buyout prices must be at least the reserve price.";
			return false;
		}

		if (!context.CanPath(actor, AuctionHouse.AuctionHouseCell))
		{
			reason = "The assigned employee cannot path to the auction house.";
			return false;
		}

		var item = ResolveListingItem(context, actor);
		if (item is null)
		{
			reason = $"The assigned employee is not carrying task-custody item {EmploymentItemSelectorResolver.Describe(ItemSelector)}.";
			return false;
		}

		if (IsAlreadyAuctioned(actor, item))
		{
			reason = $"{item.Name} is already tracked by an auction house.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		var item = ResolveListingItem(context, actor)!;
		if (EmploymentWorkerItemLocator.IsHeldOrWielded(actor, item))
		{
			actor.Body.Take(item);
		}

		item.Drop(null);
		var now = AuctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		var finish = new MudDateTime(now) + (Duration ?? AuctionHouse.DefaultListingTime);
		AuctionHouse.AddAuctionItem(new AuctionItem
		{
			Asset = item,
			Seller = AuctionHouse,
			PayoutTarget = AuctionHouse.ProfitsBankAccount is not null ? (IFrameworkItem)AuctionHouse.ProfitsBankAccount : AuctionHouse,
			ListingDateTime = now,
			FinishingDateTime = finish,
			MinimumPrice = ReservePrice.Amount,
			BuyoutPrice = BuyoutPrice?.Amount
		});

		var description = $"Listed {item.Name} as an auction-house lot at {AuctionHouse.Name} with reserve {ReservePrice.Currency.Describe(ReservePrice.Amount, CurrencyDescriptionPatternType.ShortDecimal)}.";
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
			CurrentCorrelationId(context));
		return new EmploymentActionStepResult(true, description, true,
			new EmploymentActionStepOperationalState(
				SelectedResources: $"auctionlist:house={AuctionHouse.Id.ToString("F0", CultureInfo.InvariantCulture)};asset={item.Id.ToString("F0", CultureInfo.InvariantCulture)};assettype={item.FrameworkItemType}",
				OperationalPayload: $"auctionlist:house={AuctionHouse.Id.ToString("F0", CultureInfo.InvariantCulture)};asset={item.Id.ToString("F0", CultureInfo.InvariantCulture)};reserve={ReservePrice.Amount.ToString("F2", CultureInfo.InvariantCulture)};buyout={(BuyoutPrice?.Amount.ToString("F2", CultureInfo.InvariantCulture) ?? string.Empty)};duration={(Duration?.Ticks.ToString(CultureInfo.InvariantCulture) ?? string.Empty)}"));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return [AuctionHouse.AuctionHouseCell];
	}

	private bool IsAuctionHost(IEmploymentTaskContext context)
	{
		return context.Employer.Id == AuctionHouse.Id &&
		       context.Employer.FrameworkItemType.Equals(AuctionHouse.FrameworkItemType, StringComparison.OrdinalIgnoreCase);
	}

	private IGameItem? ResolveListingItem(IEmploymentTaskContext context, ICharacter actor)
	{
		var item = EmploymentItemSelectorResolver.Resolve(context, actor, ItemSelector, actor.Location, true);
		if (item is null)
		{
			return null;
		}

		return context.CarriedTaskItems(actor).Any(x => x.Id == item.Id) ? item : null;
	}

	private static bool IsAlreadyAuctioned(ICharacter actor, IGameItem item)
	{
		return actor.Gameworld.AuctionHouses.Any(x =>
			x.ActiveAuctionItems.Any(y => y.Asset.FrameworkItemEquals(item.Id, item.FrameworkItemType)) ||
			x.UnclaimedItems.Any(y => y.AuctionItem.Asset.FrameworkItemEquals(item.Id, item.FrameworkItemType)));
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
	}
}

public sealed class AuctionSettlementActionStep : EmploymentActionStepBase
{
	public AuctionSettlementActionStep(IAuctionHouse auctionHouse, AuctionItem? targetLot = null)
		: this(auctionHouse, targetLot?.Asset.Id, targetLot?.Asset.FrameworkItemType, targetLot?.Asset.Name)
	{
	}

	public AuctionSettlementActionStep(IAuctionHouse auctionHouse, long? assetId, string? assetType, string? assetName)
		: base(
			EmploymentActionStepType.AuctionSettlement,
			EmploymentAuthority.SettleHostAccounts,
			new[] { EmploymentAICapability.CanUseBankAccount },
			true,
			true)
	{
		AuctionHouse = auctionHouse;
		AssetId = assetId;
		AssetType = assetType;
		AssetName = assetName;
	}

	public IAuctionHouse AuctionHouse { get; }
	public long? AssetId { get; }
	public string? AssetType { get; }
	public string? AssetName { get; }
	public bool SettleAllDue => !AssetId.HasValue;

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!IsAuctionHost(context))
		{
			reason = "Auction settlement steps must execute from the owning auction house employment task board.";
			return false;
		}

		if (SettleAllDue)
		{
			var now = AuctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
			if (!AuctionHouse.ActiveAuctionItems.Any(x => x.FinishingDateTime <= now))
			{
				reason = "There are no due auction lots to settle.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		var lot = ResolveActiveLot();
		if (lot is null)
		{
			reason = $"There is no active auction lot matching {AssetName ?? AssetId?.ToString("F0") ?? "the selected asset"}.";
			return false;
		}

		if (lot.FinishingDateTime > AuctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime)
		{
			reason = $"Auction lot {DescribeAsset(lot.Asset)} is not due for settlement.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		var settled = new List<AuctionItem>();
		if (SettleAllDue)
		{
			settled.AddRange(AuctionHouse.SettleFinishedAuctions());
		}
		else
		{
			var lot = ResolveActiveLot();
			if (lot is null)
			{
				return EmploymentActionStepResult.Blocked("The selected auction lot is no longer active.");
			}

			if (!AuctionHouse.SettleAuctionItem(lot, out reason))
			{
				return EmploymentActionStepResult.Blocked(reason);
			}

			settled.Add(lot);
		}

		if (!settled.Any())
		{
			return EmploymentActionStepResult.Blocked("There were no due auction lots to settle.");
		}

		var assetList = settled.Select(x => $"{x.Asset.FrameworkItemType}:{x.Asset.Id.ToString("F0", CultureInfo.InvariantCulture)}").ListToCommaSeparatedValues();
		var description = $"Settled {settled.Count.ToString("N0", actor)} auction lot{(settled.Count == 1 ? string.Empty : "s")} at {AuctionHouse.Name}.";
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
			CurrentCorrelationId(context));
		return new EmploymentActionStepResult(true, description, true,
			new EmploymentActionStepOperationalState(
				SelectedResources: $"auctionsettle:house={AuctionHouse.Id.ToString("F0", CultureInfo.InvariantCulture)};assets={assetList}",
				OperationalPayload: $"auctionsettle:house={AuctionHouse.Id.ToString("F0", CultureInfo.InvariantCulture)};count={settled.Count.ToString("F0", CultureInfo.InvariantCulture)};assets={assetList}"));
	}

	private AuctionItem? ResolveActiveLot()
	{
		return AssetId.HasValue && !string.IsNullOrWhiteSpace(AssetType)
			? AuctionHouse.ActiveAuctionItems.FirstOrDefault(x =>
				x.Asset.FrameworkItemEquals(AssetId.Value, AssetType))
			: null;
	}

	private bool IsAuctionHost(IEmploymentTaskContext context)
	{
		return context.Employer.Id == AuctionHouse.Id &&
		       context.Employer.FrameworkItemType.Equals(AuctionHouse.FrameworkItemType, StringComparison.OrdinalIgnoreCase);
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
	}

	private static string DescribeAsset(IFrameworkItem asset)
	{
		return $"{asset.Name} (#{asset.Id.ToString("F0", CultureInfo.InvariantCulture)} {asset.FrameworkItemType})";
	}
}

public sealed class AuctionClaimActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public AuctionClaimActionStep(IAuctionHouse auctionHouse, UnclaimedAuctionItem unclaimed)
		: this(auctionHouse, unclaimed.AuctionItem.Asset.Id, unclaimed.AuctionItem.Asset.FrameworkItemType,
			unclaimed.AuctionItem.Asset.Name)
	{
	}

	public AuctionClaimActionStep(IAuctionHouse auctionHouse, long assetId, string assetType, string? assetName)
		: base(
			EmploymentActionStepType.AuctionClaim,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		AuctionHouse = auctionHouse;
		AssetId = assetId;
		AssetType = assetType;
		AssetName = assetName;
	}

	public IAuctionHouse AuctionHouse { get; }
	public long AssetId { get; }
	public string AssetType { get; }
	public string? AssetName { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (context.Employer.Id != AuctionHouse.Id ||
		    !context.Employer.FrameworkItemType.Equals(AuctionHouse.FrameworkItemType, StringComparison.OrdinalIgnoreCase))
		{
			reason = "Auction claim steps must execute from the owning auction house employment task board.";
			return false;
		}

		if (!context.CanPath(actor, AuctionHouse.AuctionHouseCell))
		{
			reason = "The assigned employee cannot path to the auction house.";
			return false;
		}

		var unclaimed = ResolveUnclaimedLot();
		if (unclaimed is null)
		{
			reason = $"There is no unclaimed auction lot matching {AssetName ?? AssetId.ToString("F0", CultureInfo.InvariantCulture)}.";
			return false;
		}

		if (unclaimed.AuctionItem.Item is null)
		{
			reason = "Only movable item auction lots can be claimed into task custody.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		var unclaimed = ResolveUnclaimedLot();
		if (unclaimed?.AuctionItem.Item is not { } item)
		{
			return EmploymentActionStepResult.Blocked("The selected unclaimed auction lot is no longer available.");
		}

		AuctionHouse.ClaimItem(unclaimed.AuctionItem);
		item.Login();
		if (unclaimed.WinningBid?.Bidder is { } winner)
		{
			item.SetOwner(winner);
		}
		else if (unclaimed.AuctionItem.Seller is ICharacter seller)
		{
			item.SetOwner(seller);
		}

		item.RoomLayer = actor.RoomLayer;
		AuctionHouse.AuctionHouseCell.Insert(item, true);
		if (!context.TryCollectTaskItem(actor, item, AuctionHouse.AuctionHouseCell, out reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		var description = $"Claimed auction lot {item.Name} from {AuctionHouse.Name} into task custody.";
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
			CurrentCorrelationId(context));
		return new EmploymentActionStepResult(true, description, true,
			new EmploymentActionStepOperationalState(
				SelectedResources: EmploymentTaskContext.FormatTaskItemCustody("auctionclaim",
					CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor), [item], []),
				OperationalPayload: $"auctionclaim:house={AuctionHouse.Id.ToString("F0", CultureInfo.InvariantCulture)};asset={AssetId.ToString("F0", CultureInfo.InvariantCulture)};assettype={AssetType};item={item.Id.ToString("F0", CultureInfo.InvariantCulture)}"));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return [AuctionHouse.AuctionHouseCell];
	}

	private UnclaimedAuctionItem? ResolveUnclaimedLot()
	{
		return AuctionHouse.UnclaimedItems.FirstOrDefault(x =>
			x.AuctionItem.Asset.FrameworkItemEquals(AssetId, AssetType));
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
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
	public VehicleOperationActionStep(IVehicle vehicle, IVehicleCargoSpace? cargoSpace = null)
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
	public IVehicleCargoSpace? CargoSpace { get; }
	public bool AssignsDriver => CargoSpace is null;
	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (AssignsDriver)
		{
			return context.CanAssignVehicle(actor, Vehicle, out reason);
		}

		var cargoSpace = CargoSpace!;
		if (Vehicle.Disabled || Vehicle.Destroyed)
		{
			reason = $"{Vehicle.Name} is not available for employment cargo work.";
			return false;
		}

		if (cargoSpace.IsDisabled || !cargoSpace.CanAccess(actor, out reason))
		{
			reason = string.IsNullOrWhiteSpace(reason)
				? $"{cargoSpace.Name} is not accessible."
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

		if (!TryResolveCargoContainer(cargoSpace, out _, out var container, out reason))
		{
			return false;
		}

		if (!CanAcceptCarriedTaskItems(context, actor, cargoSpace, container, out reason))
		{
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (AssignsDriver)
		{
			if (!context.TryAssignVehicle(actor, Vehicle, out var reason, out var operationalState))
			{
				return EmploymentActionStepResult.Blocked(reason);
			}

			return new EmploymentActionStepResult(
				true,
				$"Assigned {Vehicle.Name} to driver {actor.Name} for employment vehicle work.",
				true,
				operationalState);
		}

		var cargoSpace = CargoSpace!;
		if (!TryResolveCargoContainer(cargoSpace, out var projection, out var container, out var cargoReason))
		{
			return EmploymentActionStepResult.Blocked(cargoReason);
		}

		if (!CanAcceptCarriedTaskItems(context, actor, cargoSpace, container, out cargoReason))
		{
			return EmploymentActionStepResult.Blocked(cargoReason);
		}

		var carriedCount = context.CarriedTaskItems(actor).Count();
		var capacityState = carriedCount > 0 ? "checked" : "pending";
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Selected cargo space {cargoSpace.Name} on {Vehicle.Name} with {capacityState} capacity validation.");
		return new EmploymentActionStepResult(
			true,
			carriedCount > 0
				? $"Selected cargo space {cargoSpace.Name} on {Vehicle.Name} and confirmed it can accept {carriedCount:N0} carried task item(s)."
				: $"Selected cargo space {cargoSpace.Name} on {Vehicle.Name} for future cargo loading.",
			true,
			new EmploymentActionStepOperationalState(
				SelectedResources:
					$"operation=vehiclecargo;vehicle={Vehicle.Id};cargo={cargoSpace.Id};compartment={cargoSpace.Prototype.Compartment.Id};projection={projection.Id};capacity={capacityState};carried={carriedCount}"));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return Vehicle.Location is null ? [] : [Vehicle.Location];
	}

	private static bool TryResolveCargoContainer(IVehicleCargoSpace cargoSpace, out IGameItem projection,
		out IContainer container, out string reason)
	{
		var cargoProjection = cargoSpace.ProjectionItem;
		if (cargoProjection is null)
		{
			projection = null!;
			container = null!;
			reason = $"Cargo space {cargoSpace.Name} does not have a projection item to use as a task cargo container.";
			return false;
		}

		var cargoContainer = cargoProjection.GetItemType<IContainer>();
		if (cargoContainer is null)
		{
			projection = null!;
			container = null!;
			reason = $"Cargo space {cargoSpace.Name} is projected by {cargoProjection.Name}, but that item is not a container.";
			return false;
		}

		projection = cargoProjection;
		container = cargoContainer;
		reason = string.Empty;
		return true;
	}

	private static bool CanAcceptCarriedTaskItems(IEmploymentTaskContext context, ICharacter actor,
		IVehicleCargoSpace cargoSpace, IContainer container, out string reason)
	{
		var rejected = context.CarriedTaskItems(actor).FirstOrDefault(x => !container.CanPut(x));
		if (rejected is not null)
		{
			reason = $"Cargo space {cargoSpace.Name} cannot contain {rejected.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}
}
public enum EmploymentAnimalOperationKind
{
	Lead,
	Ride,
	Lodge,
	Return
}

public sealed class StableAnimalOperationActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public StableAnimalOperationActionStep(EmploymentAnimalOperationKind operation, ICharacter? mount = null,
		IStable? stable = null, IStableStay? stay = null, ICell? destination = null, bool waiveFees = false)
		: base(
			EmploymentActionStepType.StableAnimalOperation,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanManageStableAnimals },
			false,
			false)
	{
		Operation = operation;
		Mount = mount;
		Stable = stable;
		Stay = stay;
		Destination = destination;
		WaiveFees = waiveFees;
	}

	public EmploymentAnimalOperationKind Operation { get; }
	public ICharacter? Mount { get; }
	public IStable? Stable { get; }
	public IStableStay? Stay { get; }
	public ICell? Destination { get; }
	public bool WaiveFees { get; }
	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		return Operation switch
		{
			EmploymentAnimalOperationKind.Lead => CanLead(context, actor, out reason),
			EmploymentAnimalOperationKind.Ride => CanRide(actor, out reason),
			EmploymentAnimalOperationKind.Lodge => CanLodge(context, actor, out reason),
			EmploymentAnimalOperationKind.Return => CanReturn(context, actor, out reason),
			_ => FailUnknown(out reason)
		};
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return Operation switch
		{
			EmploymentAnimalOperationKind.Lead => ExecuteLead(context, actor),
			EmploymentAnimalOperationKind.Ride => ExecuteRide(context, actor),
			EmploymentAnimalOperationKind.Lodge => ExecuteLodge(context, actor),
			EmploymentAnimalOperationKind.Return => ExecuteReturn(context, actor),
			_ => EmploymentActionStepResult.Blocked("Unknown stable animal operation.")
		};
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return Operation switch
		{
			EmploymentAnimalOperationKind.Lead => new[] { Mount?.Location, Destination }.Where(x => x is not null).Cast<ICell>().Distinct().ToList(),
			EmploymentAnimalOperationKind.Ride => Mount?.Location is null ? [] : [Mount.Location],
			EmploymentAnimalOperationKind.Lodge => new[] { Mount?.Location, Stable?.Location }.Where(x => x is not null).Cast<ICell>().Distinct().ToList(),
			EmploymentAnimalOperationKind.Return => Stable?.Location is null ? [] : [Stable.Location],
			_ => []
		};
	}

	private bool CanLead(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (Mount is null || Destination is null)
		{
			reason = "Animal lead steps need a mount and destination.";
			return false;
		}

		if (!Mount.CanEverBeMounted(actor))
		{
			reason = Mount.WhyCannotBeMountedBy(actor);
			return false;
		}

		if (!context.CanPath(actor, Mount.Location) || !context.CanPath(actor, Destination))
		{
			reason = "The assigned employee cannot path to the animal and destination.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private bool CanRide(ICharacter actor, out string reason)
	{
		if (Mount is null)
		{
			reason = "Animal ride steps need a mount.";
			return false;
		}

		if (!Mount.CanBeMountedBy(actor))
		{
			reason = Mount.WhyCannotBeMountedBy(actor);
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private bool CanLodge(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (Stable is null || Mount is null)
		{
			reason = "Animal lodge steps need a stable and mount.";
			return false;
		}

		if (!context.CanPath(actor, Stable.Location) || !context.CanPath(actor, Mount.Location))
		{
			reason = "The assigned employee cannot path to the stable and animal.";
			return false;
		}

		var can = Stable.CanLodge(actor, Mount);
		reason = can.Reason;
		return can.Truth;
	}

	private bool CanReturn(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (Stable is null || Stay is null)
		{
			reason = "Animal return steps need a stable and active stay.";
			return false;
		}

		if (!Stay.IsActive)
		{
			reason = "The selected stable stay is not active.";
			return false;
		}

		if (!context.CanPath(actor, Stable.Location))
		{
			reason = "The assigned employee cannot path to the stable.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private EmploymentActionStepResult ExecuteLead(IEmploymentTaskContext context, ICharacter actor)
	{
		var state = AnimalState("animallead", actor, Mount, Stable, Stay, Destination);
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Recorded animal lead plan for {Mount!.Name} to {Destination!.Name}.");
		return new EmploymentActionStepResult(true, $"Recorded animal lead plan for {Mount.Name}.", true, state);
	}

	private EmploymentActionStepResult ExecuteRide(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!Mount!.Mount(actor))
		{
			return EmploymentActionStepResult.Blocked(Mount.WhyCannotBeMountedBy(actor));
		}

		var state = AnimalState("animalride", actor, Mount, Stable, Stay, Destination);
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Mounted {Mount.Name} for employment animal work.");
		return new EmploymentActionStepResult(true, $"Mounted {Mount.Name}.", true, state);
	}

	private EmploymentActionStepResult ExecuteLodge(IEmploymentTaskContext context, ICharacter actor)
	{
		var stay = Stable!.Lodge(actor, Mount!);
		var state = AnimalState("animallodge", actor, Mount, Stable, stay, Destination);
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Lodged {Mount!.Name} at {Stable.Name} for employment animal work.");
		return new EmploymentActionStepResult(true, $"Lodged {Mount.Name} at {Stable.Name}.", true, state);
	}

	private EmploymentActionStepResult ExecuteReturn(IEmploymentTaskContext context, ICharacter actor)
	{
		Stable!.Release(actor, Stay!, WaiveFees);
		var state = AnimalState("animalreturn", actor, Stay!.Mount, Stable, Stay, Destination);
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Returned stable stay #{Stay.Id:N0} from {Stable.Name}.");
		return new EmploymentActionStepResult(true, $"Returned stable stay #{Stay.Id:N0}.", true, state);
	}

	private static EmploymentActionStepOperationalState AnimalState(string operation, ICharacter actor, ICharacter? mount,
		IStable? stable, IStableStay? stay, ICell? destination)
	{
		var actorId = CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor);
		var selected = $"operation={operation};actor={actorId:F0}";
		if (mount is not null)
		{
			selected += $";mount={CharacterInstanceIdentityComparer.IdentityId(mount):F0};mountinstance={CharacterInstanceIdentityComparer.InstanceId(mount)?.ToString("F0") ?? string.Empty}";
		}
		else if (stay is not null)
		{
			selected += $";mount={stay.MountId:F0};mountinstance={stay.MountInstanceId?.ToString("F0") ?? string.Empty}";
		}

		if (stable is not null)
		{
			selected += $";stable={stable.Id:F0}";
		}

		if (stay is not null)
		{
			selected += $";stay={stay.Id:F0}";
		}

		if (destination is not null)
		{
			selected += $";destination={destination.Id:F0}";
		}

		return new EmploymentActionStepOperationalState(
			OperationalPayload: $"animal-status={operation}",
			SelectedResources: selected);
	}

	private static bool FailUnknown(out string reason)
	{
		reason = "Unknown stable animal operation.";
		return false;
	}
}

public sealed class StableAdministrationActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public StableAdministrationActionStep(IStable stable, StableAdministrationActionKind operation,
		IStableStay? stay = null, IStableAccount? account = null, string? note = null)
		: base(
			EmploymentActionStepType.StableAdministration,
			RequiredAuthorityFor(operation),
			new[] { EmploymentAICapability.CanManageStableAnimals },
			false,
			false)
	{
		Stable = stable;
		Operation = operation;
		Stay = stay;
		Account = account;
		Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
	}

	public IStable Stable { get; }
	public StableAdministrationActionKind Operation { get; }
	public IStableStay? Stay { get; }
	public IStableAccount? Account { get; }
	public string? Note { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!IsStableHost(context))
		{
			reason = "Stable administration steps must execute from the owning stable employment task board.";
			return false;
		}

		if (!context.CanPath(actor, Stable.Location))
		{
			reason = "The assigned employee cannot path to the stable.";
			return false;
		}

		return Operation switch
		{
			StableAdministrationActionKind.CareInspect or
			StableAdministrationActionKind.CareFeed or
			StableAdministrationActionKind.CareGroom or
			StableAdministrationActionKind.CareExercise or
			StableAdministrationActionKind.StayReconciliation => ValidateStay(out reason),
			StableAdministrationActionKind.FeeAssessment => ValidateOptionalStay(out reason),
			StableAdministrationActionKind.AccountReconciliation => ValidateAccount(out reason),
			_ => FailUnknown(out reason)
		};
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		var description = Operation switch
		{
			StableAdministrationActionKind.FeeAssessment => ExecuteFeeAssessment(),
			StableAdministrationActionKind.AccountReconciliation => AccountSummary(),
			StableAdministrationActionKind.StayReconciliation => StaySummary("stable stay reconciliation"),
			StableAdministrationActionKind.CareInspect => StaySummary("stable inspection"),
			StableAdministrationActionKind.CareFeed => StaySummary("stable feeding"),
			StableAdministrationActionKind.CareGroom => StaySummary("stable grooming"),
			StableAdministrationActionKind.CareExercise => StaySummary("stable exercise"),
			_ => "Recorded stable administration evidence."
		};

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
			CurrentCorrelationId(context));
		return new EmploymentActionStepResult(true, description, true,
			new EmploymentActionStepOperationalState(
				SelectedResources: SelectedResources(),
				OperationalPayload: OperationalPayload()));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return [Stable.Location];
	}

	private bool IsStableHost(IEmploymentTaskContext context)
	{
		return context.Employer.Id == Stable.Id &&
		       context.Employer.FrameworkItemType.Equals(Stable.FrameworkItemType, StringComparison.OrdinalIgnoreCase);
	}

	private bool ValidateStay(out string reason)
	{
		if (Stay is null)
		{
			reason = "Stable administration stay actions require an active stable stay.";
			return false;
		}

		if (Stay.Stable.Id != Stable.Id)
		{
			reason = $"Stable stay #{Stay.Id.ToString("N0", CultureInfo.InvariantCulture)} does not belong to {Stable.Name}.";
			return false;
		}

		if (!Stay.IsActive)
		{
			reason = $"Stable stay #{Stay.Id.ToString("N0", CultureInfo.InvariantCulture)} is not active.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private bool ValidateOptionalStay(out string reason)
	{
		if (Stay is null)
		{
			reason = string.Empty;
			return true;
		}

		return ValidateStay(out reason);
	}

	private bool ValidateAccount(out string reason)
	{
		if (Account is null)
		{
			reason = "Stable account reconciliation actions require a stable account.";
			return false;
		}

		if (Account.Stable.Id != Stable.Id)
		{
			reason = $"Stable account {Account.Name} does not belong to {Stable.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private string ExecuteFeeAssessment()
	{
		if (Stay is null)
		{
			var count = Stable.ActiveStays.Count();
			Stable.AssessAllActiveStays();
			return $"Assessed fees for {count.ToString("N0", CultureInfo.InvariantCulture)} active stays at {Stable.Name}{NoteText()}.";
		}

		var before = Stay.AmountOwing;
		Stable.AssessFees(Stay);
		return $"Assessed fees for stable stay #{Stay.Id.ToString("N0", CultureInfo.InvariantCulture)} at {Stable.Name}; owing changed from {Stable.Currency.Describe(before, CurrencyDescriptionPatternType.ShortDecimal)} to {Stable.Currency.Describe(Stay.AmountOwing, CurrencyDescriptionPatternType.ShortDecimal)}{NoteText()}.";
	}

	private string StaySummary(string label)
	{
		return $"Recorded {label} evidence for stable stay #{Stay!.Id.ToString("N0", CultureInfo.InvariantCulture)} at {Stable.Name}{NoteText()}.";
	}

	private string AccountSummary()
	{
		return $"Recorded stable account reconciliation for {Account!.AccountName} at {Stable.Name}: balance {Account.Currency.Describe(Account.Balance, CurrencyDescriptionPatternType.ShortDecimal)}, credit available {Account.Currency.Describe(Account.CreditAvailable, CurrencyDescriptionPatternType.ShortDecimal)}{NoteText()}.";
	}

	private string NoteText()
	{
		return string.IsNullOrWhiteSpace(Note) ? string.Empty : $": {Note}";
	}

	private string SelectedResources()
	{
		return string.Join(";", new[]
		{
			$"stableadmin:{Operation}",
			$"stable={Stable.Id.ToString("F0", CultureInfo.InvariantCulture)}",
			Stay is null ? null : $"stay={Stay.Id.ToString("F0", CultureInfo.InvariantCulture)}",
			Account is null ? null : $"account={Account.Id.ToString("F0", CultureInfo.InvariantCulture)}"
		}.Where(x => !string.IsNullOrWhiteSpace(x))!);
	}

	private string OperationalPayload()
	{
		return $"stableadmin:{Operation};host={Stable.Id.ToString("F0", CultureInfo.InvariantCulture)}";
	}

	private static EmploymentAuthority RequiredAuthorityFor(StableAdministrationActionKind operation)
	{
		return operation == StableAdministrationActionKind.AccountReconciliation
			? EmploymentAuthority.UseStoreAccount
			: EmploymentAuthority.ManageDeliveryRoutes;
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
	}

	private static bool FailUnknown(out string reason)
	{
		reason = "Unsupported stable administration operation.";
		return false;
	}
}

public sealed class HotelAdministrationActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public HotelAdministrationActionStep(IHotel hotel, HotelAdministrationActionKind operation,
		IHotelRoom? room = null, IHotelLostProperty? lostProperty = null,
		IHotelPatronBalance? patronBalance = null, string? patronSelector = null, string? note = null)
		: base(
			EmploymentActionStepType.HotelAdministration,
			RequiredAuthorityFor(operation),
			new[] { EmploymentAICapability.CanManageHotelRooms },
			false,
			false)
	{
		Hotel = hotel;
		Operation = operation;
		Room = room;
		LostProperty = lostProperty;
		PatronBalance = patronBalance;
		PatronSelector = string.IsNullOrWhiteSpace(patronSelector) ? null : patronSelector.Trim();
		Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
	}

	public IHotel Hotel { get; }
	public HotelAdministrationActionKind Operation { get; }
	public IHotelRoom? Room { get; }
	public IHotelLostProperty? LostProperty { get; }
	public IHotelPatronBalance? PatronBalance { get; }
	public string? PatronSelector { get; }
	public string? Note { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!IsHotelHost(context))
		{
			reason = "Hotel administration steps must execute from the owning hotel employment task board.";
			return false;
		}

		return Operation switch
		{
			HotelAdministrationActionKind.RoomInspect or
			HotelAdministrationActionKind.RoomClean or
			HotelAdministrationActionKind.RoomReady or
			HotelAdministrationActionKind.RoomMaintenance => ValidateRoom(context, actor, out reason),
			HotelAdministrationActionKind.LostPropertyCheck => Succeed(out reason),
			HotelAdministrationActionKind.LostPropertyAudit => ValidateLostProperty(out reason),
			HotelAdministrationActionKind.PatronBalanceAudit => ValidatePatronBalance(out reason),
			_ => FailUnknown(out reason)
		};
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		var description = Operation switch
		{
			HotelAdministrationActionKind.RoomInspect => RoomSummary("room inspection"),
			HotelAdministrationActionKind.RoomClean => RoomSummary("room cleaning"),
			HotelAdministrationActionKind.RoomReady => RoomSummary("room readiness"),
			HotelAdministrationActionKind.RoomMaintenance => RoomSummary("room maintenance"),
			HotelAdministrationActionKind.LostPropertyCheck => ExecuteLostPropertyCheck(),
			HotelAdministrationActionKind.LostPropertyAudit => LostPropertySummary(),
			HotelAdministrationActionKind.PatronBalanceAudit => PatronBalanceSummary(),
			_ => "Recorded hotel administration evidence."
		};

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
			CurrentCorrelationId(context));
		return new EmploymentActionStepResult(true, description, true,
			new EmploymentActionStepOperationalState(
				SelectedResources: SelectedResources(),
				OperationalPayload: OperationalPayload()));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return Room is null ? [] : [Room.Cell];
	}

	private bool IsHotelHost(IEmploymentTaskContext context)
	{
		return context.Employer.Id == Hotel.Id &&
		       context.Employer.FrameworkItemType.Equals(Hotel.FrameworkItemType, StringComparison.OrdinalIgnoreCase);
	}

	private bool ValidateRoom(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (Room is null)
		{
			reason = "Hotel room administration actions require a hotel room.";
			return false;
		}

		if (Hotel.Rooms.All(x => x.Cell.Id != Room.Cell.Id && !x.Name.EqualTo(Room.Name)))
		{
			reason = $"{Room.Name} does not belong to {Hotel.Name}.";
			return false;
		}

		if (!context.CanPath(actor, Room.Cell))
		{
			reason = "The assigned employee cannot path to the hotel room.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private bool ValidateLostProperty(out string reason)
	{
		if (LostProperty is null)
		{
			reason = "Hotel lost-property audit actions require a lost-property record.";
			return false;
		}

		if (Hotel.Property.HotelLostProperties.All(x => x.BundleId != LostProperty.BundleId))
		{
			reason = $"{LostProperty.Description} is not held by {Hotel.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private bool ValidatePatronBalance(out string reason)
	{
		if (PatronBalance is null)
		{
			reason = string.IsNullOrWhiteSpace(PatronSelector)
				? "Hotel patron-balance audit actions require a patron balance."
				: $"There is no hotel patron balance matching {PatronSelector}.";
			return false;
		}

		if (Hotel.Property.HotelPatronBalances.All(x => x.PatronId != PatronBalance.PatronId))
		{
			reason = $"The selected patron balance is not held by {Hotel.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private string RoomSummary(string label)
	{
		return $"Recorded hotel {label} evidence for {Room!.Name} at {Hotel.Name}{NoteText()}.";
	}

	private string ExecuteLostPropertyCheck()
	{
		var before = Hotel.Property.HotelLostProperties.Count();
		Hotel.Property.CheckHotelLostProperty();
		var after = Hotel.Property.HotelLostProperties.Count();
		return $"Checked hotel lost-property expiry for {Hotel.Name}; records changed from {before.ToString("N0", CultureInfo.InvariantCulture)} to {after.ToString("N0", CultureInfo.InvariantCulture)}{NoteText()}.";
	}

	private string LostPropertySummary()
	{
		return $"Recorded hotel lost-property audit for {LostProperty!.Description} at {Hotel.Name}; status {LostProperty.Status.DescribeEnum()}{NoteText()}.";
	}

	private string PatronBalanceSummary()
	{
		return $"Recorded hotel patron-balance audit for {PatronBalance!.PatronId.ToString("N0", CultureInfo.InvariantCulture)} at {Hotel.Name}: {Hotel.Currency.Describe(PatronBalance.Balance, CurrencyDescriptionPatternType.ShortDecimal)}{NoteText()}.";
	}

	private string NoteText()
	{
		return string.IsNullOrWhiteSpace(Note) ? string.Empty : $": {Note}";
	}

	private string SelectedResources()
	{
		return string.Join(";", new[]
		{
			$"hoteladmin:{Operation}",
			$"hotel={Hotel.Id.ToString("F0", CultureInfo.InvariantCulture)}",
			Room is null ? null : $"room={Room.Cell.Id.ToString("F0", CultureInfo.InvariantCulture)}",
			LostProperty is null ? null : $"lost={LostProperty.BundleId.ToString("F0", CultureInfo.InvariantCulture)}",
			PatronBalance is null ? null : $"patron={PatronBalance.PatronId.ToString("F0", CultureInfo.InvariantCulture)}"
		}.Where(x => !string.IsNullOrWhiteSpace(x))!);
	}

	private string OperationalPayload()
	{
		return $"hoteladmin:{Operation};host={Hotel.Id.ToString("F0", CultureInfo.InvariantCulture)}";
	}

	private static EmploymentAuthority RequiredAuthorityFor(HotelAdministrationActionKind operation)
	{
		return operation == HotelAdministrationActionKind.PatronBalanceAudit
			? EmploymentAuthority.UseStoreAccount
			: EmploymentAuthority.ManageDeliveryRoutes;
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
	}

	private static bool Succeed(out string reason)
	{
		reason = string.Empty;
		return true;
	}

	private static bool FailUnknown(out string reason)
	{
		reason = "Unsupported hotel administration operation.";
		return false;
	}
}

public sealed class HospitalPatientPreparationActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public HospitalPatientPreparationActionStep(IHospital hospital, IHospitalServiceRequest request)
		: base(
			EmploymentActionStepType.HospitalPatientPreparation,
			EmploymentAuthority.PerformMedicalServices,
			new[] { EmploymentAICapability.CanPerformMedicalServices },
			false,
			false)
	{
		Hospital = hospital;
		Request = request;
	}

	public IHospital Hospital { get; }
	public IHospitalServiceRequest Request { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!IsHospitalHost(context))
		{
			reason = "Hospital patient-preparation steps must execute from the owning hospital employment task board.";
			return false;
		}

		if (!Hospital.IsTrading)
		{
			reason = $"{Hospital.Name} is not presently trading.";
			return false;
		}

		if (Request.Hospital.Id != Hospital.Id)
		{
			reason = $"Hospital service request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)} does not belong to {Hospital.Name}.";
			return false;
		}

		if (!HospitalMedicalServiceRunner.ShouldUseTreatmentTheatre(Request.Service))
		{
			reason = "This hospital service does not require theatre preparation.";
			return false;
		}

		if (Request.Patient is not { } patient)
		{
			reason = "The patient for this hospital service request is not currently available.";
			return false;
		}

		if (Request.Status is HospitalServiceRequestStatus.Completed or HospitalServiceRequestStatus.Cancelled or
		    HospitalServiceRequestStatus.Declined or HospitalServiceRequestStatus.Failed)
		{
			reason = $"Hospital service request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)} is already {Request.Status.DescribeEnum()}.";
			return false;
		}

		if (!HospitalPatientFlow.TryReserveTreatmentLocation(Hospital, Request, out var theatre, out reason))
		{
			return false;
		}

		if (theatre is null)
		{
			reason = "There is no treatment location for this hospital request.";
			return false;
		}

		if (!actor.ColocatedWith(patient))
		{
			if (patient.Location is null)
			{
				reason = "The patient is not presently in a known location.";
				return false;
			}

			if (!context.CanPath(actor, patient.Location))
			{
				reason = $"The assigned medical employee cannot path to the patient location {patient.Location.Name}.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		if (actor.Location?.Id != theatre.Id && !context.CanPath(actor, theatre))
		{
			reason = $"The assigned medical employee cannot path to the treatment location {theatre.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		var patient = Request.Patient!;
		if (!actor.ColocatedWith(patient))
		{
			Request.MarkStatus(HospitalServiceRequestStatus.Assigned,
				$"Assigned to {actor.HowSeen(actor, colour: false)}; awaiting patient escort.");
			return new EmploymentActionStepResult(true,
				"The assigned medical employee must reach the patient before moving them to theatre.", false);
		}

		if (!HospitalPatientFlow.TryReserveTreatmentLocation(Hospital, Request, out var theatre, out reason) || theatre is null)
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (!HospitalPatientFlow.TransferForTreatment(Hospital, Request, actor, patient, theatre, out reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		Request.MarkStatus(HospitalServiceRequestStatus.Assigned,
			$"{actor.HowSeen(actor, colour: false)} moved {patient.HowSeen(actor, colour: false)} to {theatre.Name} for hospital treatment.");
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Moved patient for hospital request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)} to {theatre.Name}.",
			CurrentCorrelationId(context));
		return new EmploymentActionStepResult(true,
			$"Moved patient for hospital request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)} to {theatre.Name}.",
			true,
			new EmploymentActionStepOperationalState(
				SelectedResources: $"hospitalpatientprep:request={Request.Id.ToString("F0", CultureInfo.InvariantCulture)};patient={Request.PatientId.ToString("F0", CultureInfo.InvariantCulture)};theatre={theatre.Id.ToString("F0", CultureInfo.InvariantCulture)}",
				OperationalPayload: $"hospitalpatientprep;hospital={Hospital.Id.ToString("F0", CultureInfo.InvariantCulture)};request={Request.Id.ToString("F0", CultureInfo.InvariantCulture)};theatre={theatre.Id.ToString("F0", CultureInfo.InvariantCulture)}"));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		if (Request.Patient is not { } patient)
		{
			return [];
		}

		if (!actor.ColocatedWith(patient))
		{
			return patient.Location is null ? [] : [patient.Location];
		}

		return [];
	}

	private bool IsHospitalHost(IEmploymentTaskContext context)
	{
		return context.Employer.Id == Hospital.Id &&
		       context.Employer.FrameworkItemType.Equals(Hospital.FrameworkItemType, StringComparison.OrdinalIgnoreCase);
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
	}
}

public sealed class HospitalSupplyPreparationActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	private const string PhaseCollected = "collected";

	public HospitalSupplyPreparationActionStep(IHospital hospital, IHospitalServiceRequest request)
		: base(
			EmploymentActionStepType.HospitalSupplyPreparation,
			EmploymentAuthority.PrepareMedicalSupplies,
			new[] { EmploymentAICapability.CanPrepareHospitalSupplies },
			false,
			false)
	{
		Hospital = hospital;
		Request = request;
	}

	public IHospital Hospital { get; }
	public IHospitalServiceRequest Request { get; }

	public static bool HasPreparatorySupplyWork(IHospital hospital, IHospitalServiceRequest request)
	{
		if (request.Service.RequiredEquipment.Any())
		{
			return true;
		}

		if (!HospitalMedicalServiceRunner.UsesCommandRoutedWoundCare(request.Service))
		{
			return false;
		}

		var treatmentTypes = HospitalMedicalServiceRunner.ImplicitTreatmentSupplyTypes(request.Service);
		return treatmentTypes.Any() &&
		       hospital.SupplyRooms.Any(room =>
			       (room.GameItems ?? Enumerable.Empty<IGameItem>())
			       .SelectMany(DeepItemsOrSelf)
			       .DistinctBy(x => x.Id)
			       .Any(item => item.GetItemType<ITreatment>() is { } treatment &&
			                    treatmentTypes.Any(treatment.IsTreatmentType)));
	}

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!IsHospitalHost(context))
		{
			reason = "Hospital supply-preparation steps must execute from the owning hospital employment task board.";
			return false;
		}

		if (!Hospital.IsTrading)
		{
			reason = $"{Hospital.Name} is not presently trading.";
			return false;
		}

		if (Request.Hospital.Id != Hospital.Id)
		{
			reason = $"Hospital service request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)} does not belong to {Hospital.Name}.";
			return false;
		}

		if (Request.Status is HospitalServiceRequestStatus.Completed or HospitalServiceRequestStatus.Cancelled or
		    HospitalServiceRequestStatus.Declined or HospitalServiceRequestStatus.Failed)
		{
			reason = $"Hospital service request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)} is already {Request.Status.DescribeEnum()}.";
			return false;
		}

		if (!Request.Service.RequiredEquipment.Any() &&
		    !HospitalMedicalServiceRunner.UsesCommandRoutedWoundCare(Request.Service))
		{
			reason = "This hospital service has no configured equipment or implicit treatment-supply requirements.";
			return false;
		}

		if (Request.SupplyPrepared)
		{
			reason = "This hospital request's supplies have already been prepared.";
			return false;
		}

		if (!HospitalPatientFlow.TryReserveTreatmentLocation(Hospital, Request, out var theatre, out reason))
		{
			return false;
		}

		if (theatre is null)
		{
			reason = "There is no treatment location for these hospital supplies.";
			return false;
		}

		if (TreatmentLocationAlreadyPrepared(context, actor, theatre))
		{
			if (actor.Location?.Id != theatre.Id && !context.CanPath(actor, theatre))
			{
				reason = $"The assigned employee cannot path to the treatment location {theatre.Name}.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		if (context.CarriedTaskItems(actor).Any())
		{
			if (!context.CanPath(actor, theatre))
			{
				reason = $"The assigned employee cannot path to the treatment location {theatre.Name}.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		if (!TryFindSupplyBundle(context, actor, out var source, out _, out reason))
		{
			return false;
		}

		if (DoctorBlockedByAvailableSupplyWorker(context, actor, theatre, source))
		{
			reason = "A reachable, available non-medical supply employee can prepare these hospital supplies.";
			return false;
		}

		if (!context.CanPath(actor, source))
		{
			reason = $"The assigned employee cannot path to the hospital supply room {source.Name}.";
			return false;
		}

		if (!context.CanPath(actor, theatre))
		{
			reason = $"The assigned employee cannot path to the treatment location {theatre.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (!HospitalPatientFlow.TryReserveTreatmentLocation(Hospital, Request, out var theatre, out reason) || theatre is null)
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		var carried = context.CarriedTaskItems(actor).ToList();
		if (carried.Any())
		{
			if (actor.Location?.Id != theatre.Id)
			{
				return new EmploymentActionStepResult(true,
					$"Hospital supplies are ready to deliver to {theatre.Name}.", false,
					new EmploymentActionStepOperationalState(OperationalPayload: SupplyPayload(PhaseCollected, theatre.Id)));
			}

			if (!context.TryDeliverTaskItems(actor, theatre, null, null, out reason))
			{
				return EmploymentActionStepResult.Blocked(reason);
			}

			return CompletePreparedSupplies(context, actor, theatre,
				EmploymentTaskContext.FormatTaskItemCustody("deliver",
					CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor), carried),
				"delivered");
		}

		if (TreatmentLocationAlreadyPrepared(context, actor, theatre))
		{
			if (actor.Location?.Id != theatre.Id)
			{
				return new EmploymentActionStepResult(true,
					$"Hospital supplies are already staged in {theatre.Name}.", false,
					new EmploymentActionStepOperationalState(OperationalPayload: SupplyPayload("staged", theatre.Id)));
			}

			return CompletePreparedSupplies(context, actor, theatre, null, "staged");
		}

		if (!TryFindSupplyBundle(context, actor, out var source, out var items, out reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (actor.Location?.Id != source.Id)
		{
			return new EmploymentActionStepResult(true,
				$"Hospital supplies are available in {source.Name}.", false,
				new EmploymentActionStepOperationalState(OperationalPayload: SupplyPayload("source", theatre.Id, source.Id)));
		}

		var previouslyCarried = context.CarriedTaskItems(actor).Select(x => x.Id).ToHashSet();
		if (!context.TryCollectTaskItems(actor, items.Select(x => (x.Item, x.Source)).ToList(), out reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return new EmploymentActionStepResult(true,
			$"Collected hospital supplies for request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)}; deliver them to {theatre.Name}.",
			false,
			EmploymentActionStepOperationalStateBuilder.CollectedTaskItemCustody(context, actor,
				items.Select(x => x.Item).ToList(), previouslyCarried).Merge(
				new EmploymentActionStepOperationalState(OperationalPayload: SupplyPayload(PhaseCollected, theatre.Id, source.Id))));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!HospitalPatientFlow.TryReserveTreatmentLocation(Hospital, Request, out var theatre, out _) || theatre is null)
		{
			return [];
		}

		if (context.CarriedTaskItems(actor).Any())
		{
			return [theatre];
		}

		if (TreatmentLocationAlreadyPrepared(context, actor, theatre))
		{
			return actor.Location?.Id == theatre.Id ? [] : [theatre];
		}

		return TryFindSupplyBundle(context, actor, out var source, out _, out _)
			? [source]
			: [theatre];
	}

	private EmploymentActionStepResult CompletePreparedSupplies(IEmploymentTaskContext context, ICharacter actor,
		ICell theatre, string? selectedResources, string phase)
	{
		Request.MarkSuppliesPrepared(actor,
			$"{actor.HowSeen(actor, colour: false)} prepared hospital supplies in {theatre.Name}.");
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Prepared supplies for hospital request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)} in {theatre.Name}.",
			CurrentCorrelationId(context));
		return new EmploymentActionStepResult(true,
			$"Prepared hospital supplies for request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)} in {theatre.Name}.",
			true,
			new EmploymentActionStepOperationalState(
				SelectedResources: selectedResources,
				OperationalPayload: SupplyPayload(phase, theatre.Id)));
	}

	private bool TreatmentLocationAlreadyPrepared(IEmploymentTaskContext context, ICharacter actor, ICell theatre)
	{
		var available = TreatmentLocationSupplyItems(context, theatre).ToList();
		if (Request.Service.RequiredEquipment.Any())
		{
			foreach (var requirement in Request.Service.RequiredEquipment)
			{
				var matched = available
				              .Where(x => MatchesSelector(context, actor, x, requirement.Selector))
				              .Take(requirement.Quantity)
				              .ToList();
				if (matched.Count < requirement.Quantity)
				{
					return false;
				}

				foreach (var item in matched)
				{
					available.RemoveAll(x => x.Id == item.Id);
				}
			}

			return true;
		}

		if (!HospitalMedicalServiceRunner.UsesCommandRoutedWoundCare(Request.Service))
		{
			return false;
		}

		var treatmentTypes = HospitalMedicalServiceRunner.ImplicitTreatmentSupplyTypes(Request.Service).ToList();
		if (!treatmentTypes.Any())
		{
			return false;
		}

		var treatments = available
		                 .Select(x => x.GetItemType<ITreatment>())
		                 .Where(x => x is not null)
		                 .Cast<ITreatment>()
		                 .ToList();
		return treatmentTypes.All(type => treatments.Any(treatment => treatment.IsTreatmentType(type)));
	}

	private IEnumerable<IGameItem> TreatmentLocationSupplyItems(IEmploymentTaskContext context, ICell theatre)
	{
		return context.AvailableItems(theatre)
		              .Concat(theatre.GameItems ?? Enumerable.Empty<IGameItem>())
		              .SelectMany(DeepItemsOrSelf)
		              .DistinctBy(x => x.Id);
	}

	private bool TryFindSupplyBundle(IEmploymentTaskContext context, ICharacter actor, out ICell source,
		out IReadOnlyCollection<(IGameItem Item, ICell Source)> items, out string reason)
	{
		if (!Request.Service.RequiredEquipment.Any())
		{
			return TryFindImplicitTreatmentSupplyBundle(context, out source, out items, out reason);
		}

		return TryFindConfiguredSupplyBundle(context, actor, out source, out items, out reason);
	}

	private bool TryFindConfiguredSupplyBundle(IEmploymentTaskContext context, ICharacter actor, out ICell source,
		out IReadOnlyCollection<(IGameItem Item, ICell Source)> items, out string reason)
	{
		source = null!;
		items = [];
		foreach (var room in Hospital.SupplyRooms)
		{
			var available = context.AvailableItems(room)
			                       .SelectMany(DeepItemsOrSelf)
			                       .DistinctBy(x => x.Id)
			                       .ToList();
			var selected = new List<(IGameItem Item, ICell Source)>();
			var used = new HashSet<long>();
			var failed = false;
			foreach (var requirement in Request.Service.RequiredEquipment)
			{
				var matched = available
				              .Where(x => !used.Contains(x.Id) && MatchesSelector(context, actor, x, requirement.Selector))
				              .Take(requirement.Quantity)
				              .ToList();
				if (matched.Count < requirement.Quantity)
				{
					failed = true;
					break;
				}

				selected.AddRange(matched.Select(x => (x, room)));
				foreach (var item in matched)
				{
					used.Add(item.Id);
				}
			}

			if (failed)
			{
				continue;
			}

			source = room;
			items = selected;
			reason = string.Empty;
			return true;
		}

		reason = "No single hospital supply room contains all required equipment for this service.";
		return false;
	}

	private bool TryFindImplicitTreatmentSupplyBundle(IEmploymentTaskContext context, out ICell source,
		out IReadOnlyCollection<(IGameItem Item, ICell Source)> items, out string reason)
	{
		source = null!;
		items = [];
		var treatmentTypes = HospitalMedicalServiceRunner.ImplicitTreatmentSupplyTypes(Request.Service);
		foreach (var room in Hospital.SupplyRooms)
		{
			var available = context.AvailableItems(room)
			                       .SelectMany(DeepItemsOrSelf)
			                       .DistinctBy(x => x.Id)
			                       .ToList();
			var selected = new List<(IGameItem Item, ICell Source)>();
			var used = new HashSet<long>();
			foreach (var treatmentType in treatmentTypes)
			{
				var item = available.FirstOrDefault(x =>
					!used.Contains(x.Id) &&
					x.GetItemType<ITreatment>() is { } treatment &&
					treatment.IsTreatmentType(treatmentType));
				if (item is null)
				{
					continue;
				}

				selected.Add((item, room));
				used.Add(item.Id);
			}

			if (!selected.Any())
			{
				continue;
			}

			source = room;
			items = selected;
			reason = string.Empty;
			return true;
		}

		reason = "No hospital supply room has treatment supplies useful for this service.";
		return false;
	}

	private static IEnumerable<IGameItem> DeepItemsOrSelf(IGameItem item)
	{
		var deepItems = item.DeepItems?.ToList();
		return deepItems?.Any() == true ? deepItems : [item];
	}

	private static bool MatchesSelector(IEmploymentTaskContext context, ICharacter actor, IGameItem item,
		EmploymentItemSelector selector)
	{
		return selector.Kind switch
		{
			EmploymentItemSelectorKind.PrototypeId => item.Prototype.Id == selector.Id,
			EmploymentItemSelectorKind.ItemId => item.Id == selector.Id,
			EmploymentItemSelectorKind.Tag => !string.IsNullOrWhiteSpace(selector.Text) &&
			                                context.ItemHasTag(item, selector.Text),
			EmploymentItemSelectorKind.Keyword => !string.IsNullOrWhiteSpace(selector.Text) &&
			                                   item.HasKeywords(selector.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), actor, true, true),
			_ => false
		};
	}

	private bool DoctorBlockedByAvailableSupplyWorker(IEmploymentTaskContext context, ICharacter actor, ICell theatre,
		ICell source)
	{
		if (!Hospital.HasAuthority(actor, EmploymentAuthority.PerformMedicalServices))
		{
			return false;
		}

		var actorId = CharacterInstanceIdentityComparer.IdentityId(actor);
		return Hospital.ActiveEmploymentContracts().Any(x =>
		{
			if (x.Employee.Id == actorId ||
			    x.Status != EmploymentStatus.Active ||
			    !x.Authority.Contains(EmploymentAuthority.PrepareMedicalSupplies) ||
			    x.Authority.Contains(EmploymentAuthority.PerformMedicalServices) ||
			    !CharacterState.Able.HasFlag(x.Employee.State) ||
			    x.Employee.Location is null)
			{
				return false;
			}

			var employeeId = CharacterInstanceIdentityComparer.IdentityId(x.Employee);
			if (Hospital.TaskBoard.ActiveTasks.Any(task =>
				CharacterInstanceIdentityComparer.IdentityId(task.AssignedEmployee) == employeeId &&
				task.Status is EmploymentTaskStatus.Assigned or EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked))
			{
				return false;
			}

			return base.CanExecute(context, x.Employee, out _) &&
			       context.CanPath(x.Employee, source) &&
			       context.CanPath(x.Employee, theatre);
		});
	}

	private string SupplyPayload(string phase, long theatreId, long? sourceId = null)
	{
		return sourceId is null
			? $"hospitalsupply;hospital={Hospital.Id.ToString("F0", CultureInfo.InvariantCulture)};request={Request.Id.ToString("F0", CultureInfo.InvariantCulture)};phase={phase};theatre={theatreId.ToString("F0", CultureInfo.InvariantCulture)}"
			: $"hospitalsupply;hospital={Hospital.Id.ToString("F0", CultureInfo.InvariantCulture)};request={Request.Id.ToString("F0", CultureInfo.InvariantCulture)};phase={phase};theatre={theatreId.ToString("F0", CultureInfo.InvariantCulture)};source={sourceId.Value.ToString("F0", CultureInfo.InvariantCulture)}";
	}

	private bool IsHospitalHost(IEmploymentTaskContext context)
	{
		return context.Employer.Id == Hospital.Id &&
		       context.Employer.FrameworkItemType.Equals(Hospital.FrameworkItemType, StringComparison.OrdinalIgnoreCase);
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
	}
}
public sealed class HospitalServiceActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public HospitalServiceActionStep(IHospital hospital, IHospitalServiceRequest request)
		: base(
			EmploymentActionStepType.HospitalService,
			EmploymentAuthority.PerformMedicalServices,
			new[] { EmploymentAICapability.CanPerformMedicalServices },
			false,
			false)
	{
		Hospital = hospital;
		Request = request;
	}

	public IHospital Hospital { get; }
	public IHospitalServiceRequest Request { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!IsHospitalHost(context))
		{
			reason = "Hospital service steps must execute from the owning hospital employment task board.";
			return false;
		}

		if (!Hospital.IsTrading)
		{
			reason = $"{Hospital.Name} is not presently trading.";
			return false;
		}

		if (Request.Hospital.Id != Hospital.Id)
		{
			reason = $"Hospital service request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)} does not belong to {Hospital.Name}.";
			return false;
		}

		if (Request.Patient is not { } patient)
		{
			reason = "The patient for this hospital service request is not currently available.";
			return false;
		}

		if (Request.Status is HospitalServiceRequestStatus.Cancelled or HospitalServiceRequestStatus.Declined or
		    HospitalServiceRequestStatus.Failed)
		{
			reason = $"Hospital service request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)} is already {Request.Status.DescribeEnum()}.";
			return false;
		}

		if (Request.Status != HospitalServiceRequestStatus.Completed && Request.Service.RequiredEquipment.Any() &&
		    !Request.SupplyPrepared)
		{
			reason = "The required hospital supplies have not yet been prepared.";
			return false;
		}

		if (Request.Status != HospitalServiceRequestStatus.Completed &&
		    HospitalMedicalServiceRunner.ShouldUseTreatmentTheatre(Request.Service) &&
		    !HospitalPatientFlow.TryReserveTreatmentLocation(Hospital, Request, out _, out reason))
		{
			return false;
		}

		if (Request.Status != HospitalServiceRequestStatus.Completed &&
		    ShouldCollectImplicitTreatmentSupplies(context, actor, out var source, out _, out reason))
		{
			if (!context.CanPath(actor, source))
			{
				reason = $"The assigned employee cannot path to the hospital supply room {source.Name}.";
				return false;
			}
		}

		if (!actor.ColocatedWith(patient) && patient.Location is not null && !context.CanPath(actor, patient.Location))
		{
			reason = $"The assigned employee cannot path to the patient location {patient.Location.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (Request.Status == HospitalServiceRequestStatus.Completed)
		{
			return HospitalMedicalServiceRunner.ExecuteServiceRequest(context, actor, Hospital, Request);
		}

		var patient = Request.Patient!;
		if (Request.Status != HospitalServiceRequestStatus.Completed &&
		    ShouldCollectImplicitTreatmentSupplies(context, actor, out var source, out var supplies, out reason))
		{
			if (actor.Location?.Id != source.Id)
			{
				return new EmploymentActionStepResult(true,
					$"Hospital treatment supplies are available in {source.Name}.", false,
					new EmploymentActionStepOperationalState(OperationalPayload:
						$"hospitalservice:supplies;request={Request.Id.ToString("F0", CultureInfo.InvariantCulture)};source={source.Id.ToString("F0", CultureInfo.InvariantCulture)}"));
			}

			var previouslyCarried = context.CarriedTaskItems(actor).Select(x => x.Id).ToHashSet();
			if (!context.TryCollectTaskItems(actor, supplies.Select(x => (x.Item, x.Source)).ToList(), out reason))
			{
				return EmploymentActionStepResult.Blocked(reason);
			}

			return new EmploymentActionStepResult(true,
				$"Collected treatment supplies for hospital request #{Request.Id.ToString("N0", CultureInfo.InvariantCulture)}.",
				false,
				EmploymentActionStepOperationalStateBuilder.CollectedTaskItemCustody(context, actor,
					supplies.Select(x => x.Item).ToList(), previouslyCarried).Merge(
					new EmploymentActionStepOperationalState(OperationalPayload:
						$"hospitalservice:supplies;request={Request.Id.ToString("F0", CultureInfo.InvariantCulture)};source={source.Id.ToString("F0", CultureInfo.InvariantCulture)};items={supplies.Select(x => x.Item.Id.ToString("F0", CultureInfo.InvariantCulture)).ListToCommaSeparatedValues()}")));
		}

		if (!actor.ColocatedWith(patient))
		{
			Request.MarkStatus(HospitalServiceRequestStatus.Assigned,
				$"Assigned to {actor.HowSeen(actor, colour: false)}; awaiting patient escort.");
			return new EmploymentActionStepResult(true,
				"The assigned employee must reach the patient before starting the hospital service.", false);
		}

		if (HospitalMedicalServiceRunner.ShouldUseTreatmentTheatre(Request.Service))
		{
			if (!HospitalPatientFlow.TryReserveTreatmentLocation(Hospital, Request, out var treatmentLocation, out reason) ||
			    treatmentLocation is null)
			{
				return EmploymentActionStepResult.Blocked(reason);
			}

			if (!HospitalPatientFlow.TransferForTreatment(Hospital, Request, actor, patient, treatmentLocation, out reason))
			{
				return EmploymentActionStepResult.Blocked(reason);
			}
		}
		else
		{
			Request.OperatingTheatreCellId = patient.Location?.Id;
			if (!Request.UsedInPlaceFallback)
			{
				actor.OutputHandler.Send(new EmoteOutput(new Emote(
					"@ keep|keeps $0 here for hospital treatment rather than moving to an operating theatre.", actor,
					patient)));
				Request.UsedInPlaceFallback = true;
			}
		}

		return HospitalMedicalServiceRunner.ExecuteServiceRequest(context, actor, Hospital, Request);
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		if (Request.Patient is not { } patient)
		{
			return [];
		}

		if (ShouldCollectImplicitTreatmentSupplies(context, actor, out var source, out _, out _) &&
		    actor.Location?.Id != source.Id)
		{
			return [source];
		}

		if (!actor.ColocatedWith(patient))
		{
			return patient.Location is null ? [] : [patient.Location];
		}

		return patient.Location is null ? [] : [patient.Location];
	}

	private bool ShouldCollectImplicitTreatmentSupplies(IEmploymentTaskContext context, ICharacter actor,
		out ICell source, out IReadOnlyCollection<(IGameItem Item, ICell Source)> items, out string reason)
	{
		source = null!;
		items = [];
		reason = string.Empty;
		if ((Request.Service.RequiredEquipment?.Any() ?? false) ||
		    !HospitalMedicalServiceRunner.UsesCommandRoutedWoundCare(Request.Service) ||
		    HospitalTreatmentCommandInProgress(context))
		{
			return false;
		}

		var treatmentTypes = HospitalMedicalServiceRunner.ImplicitTreatmentSupplyTypes(Request.Service);
		var missingTreatmentTypes = MissingTreatmentSupplyTypes(context, actor, treatmentTypes);
		if (!missingTreatmentTypes.Any())
		{
			return false;
		}

		return TryFindImplicitSupplyBundle(context, actor, missingTreatmentTypes, out source, out items, out reason);
	}

	private IReadOnlyCollection<TreatmentType> MissingTreatmentSupplyTypes(IEmploymentTaskContext context, ICharacter actor,
		IReadOnlyCollection<TreatmentType> treatmentTypes)
	{
		var supplies = EmploymentWorkerItemLocator.TaskHeldItems(context, actor)
		                      .SelectMany(DeepItemsOrSelf)
		                      .Concat(TreatmentLocationItems(context))
		                      .Concat(CurrentTreatmentRoomItems(actor))
		                      .DistinctBy(x => x.Id)
		                      .Select(x => x.GetItemType<ITreatment>())
		                      .Where(x => x is not null)
		                      .Cast<ITreatment>()
		                      .ToList();
		return treatmentTypes
		       .Where(type => supplies.All(supply => !supply.IsTreatmentType(type)))
		       .ToList();
	}

	private IEnumerable<IGameItem> TreatmentLocationItems(IEmploymentTaskContext context)
	{
		if (Request.OperatingTheatreCellId is not { } theatreId)
		{
			return [];
		}

		var theatre = Hospital.OperatingTheatres.FirstOrDefault(x => x.Id == theatreId);
		return theatre is null
			? []
			: (context.AvailableItems(theatre) ?? Array.Empty<IGameItem>())
			  .Concat(theatre.GameItems ?? Array.Empty<IGameItem>())
			  .SelectMany(DeepItemsOrSelf);
	}

	private IEnumerable<IGameItem> CurrentTreatmentRoomItems(ICharacter actor)
	{
		if (actor.Location is null ||
		    Request.Patient is null ||
		    !actor.ColocatedWith(Request.Patient))
		{
			return [];
		}

		return (actor.Location.GameItems ?? Array.Empty<IGameItem>())
		       .SelectMany(DeepItemsOrSelf);
	}

	private bool TryFindImplicitSupplyBundle(IEmploymentTaskContext context, ICharacter actor,
		IReadOnlyCollection<TreatmentType> treatmentTypes, out ICell source,
		out IReadOnlyCollection<(IGameItem Item, ICell Source)> items, out string reason)
	{
		source = null!;
		items = [];
		foreach (var room in Hospital.SupplyRooms ?? Array.Empty<ICell>())
		{
			var available = context.AvailableItems(room)
			                       .SelectMany(DeepItemsOrSelf)
			                       .DistinctBy(x => x.Id)
			                       .ToList();
			var selected = new List<(IGameItem Item, ICell Source)>();
			var used = new HashSet<long>();
			foreach (var treatmentType in treatmentTypes)
			{
				var item = available.FirstOrDefault(x =>
					!used.Contains(x.Id) &&
					x.GetItemType<ITreatment>() is { } treatment &&
					treatment.IsTreatmentType(treatmentType));
				if (item is null)
				{
					continue;
				}

				selected.Add((item, room));
				used.Add(item.Id);
			}

			if (!selected.Any())
			{
				continue;
			}

			source = room;
			items = selected;
			reason = string.Empty;
			return true;
		}

		reason = "No hospital supply room has treatment supplies useful for this service.";
		return false;
	}

	private static IEnumerable<IGameItem> DeepItemsOrSelf(IGameItem item)
	{
		var deepItems = item.DeepItems?.ToList();
		return deepItems?.Any() == true ? deepItems : [item];
	}

	private static bool HospitalTreatmentCommandInProgress(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete &&
		       concrete.CurrentTask is not null &&
		       (concrete.CurrentTask.StepOperationalStates.ElementAtOrDefault(concrete.CurrentStepIndex)
		                ?.OperationalPayload?.Contains(";active=", StringComparison.InvariantCultureIgnoreCase) ?? false);
	}

	private bool IsHospitalHost(IEmploymentTaskContext context)
	{
		return context.Employer.Id == Hospital.Id &&
		       context.Employer.FrameworkItemType.Equals(Hospital.FrameworkItemType, StringComparison.OrdinalIgnoreCase);
	}
}
public sealed class HospitalAdministrationActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	public HospitalAdministrationActionStep(IHospital hospital, HospitalAdministrationActionKind operation,
		string? note = null)
		: base(
			EmploymentActionStepType.HospitalAdministration,
			RequiredAuthorityFor(operation),
			new[] { EmploymentAICapability.CanPerformMedicalServices },
			false,
			false)
	{
		Hospital = hospital;
		Operation = operation;
		Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
	}

	public IHospital Hospital { get; }
	public HospitalAdministrationActionKind Operation { get; }
	public string? Note { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (context.Employer.Id != Hospital.Id ||
		    !context.Employer.FrameworkItemType.Equals(Hospital.FrameworkItemType, StringComparison.OrdinalIgnoreCase))
		{
			reason = "Hospital administration steps must execute from the owning hospital employment task board.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		var description = Operation switch
		{
			HospitalAdministrationActionKind.ServiceAudit => $"Recorded hospital service audit for {Hospital.Name}{NoteText()}.",
			HospitalAdministrationActionKind.DebtAudit => $"Recorded hospital debt audit for {Hospital.Name}{NoteText()}.",
			HospitalAdministrationActionKind.TheatrePreparation => $"Recorded hospital theatre preparation for {Hospital.Name}{NoteText()}.",
			HospitalAdministrationActionKind.SupplyAudit => $"Recorded hospital supply audit for {Hospital.Name}{NoteText()}.",
			HospitalAdministrationActionKind.RequestAudit => $"Recorded hospital request audit for {Hospital.Name}{NoteText()}.",
			_ => $"Recorded hospital administration evidence for {Hospital.Name}{NoteText()}."
		};

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
			CurrentCorrelationId(context));
		return new EmploymentActionStepResult(true, description, true,
			new EmploymentActionStepOperationalState(
				SelectedResources: $"hospitaladmin:{Operation};hospital={Hospital.Id.ToString("F0", CultureInfo.InvariantCulture)}",
				OperationalPayload: $"hospitaladmin:{Operation};host={Hospital.Id.ToString("F0", CultureInfo.InvariantCulture)}"));
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return Operation switch
		{
			HospitalAdministrationActionKind.TheatrePreparation => Hospital.OperatingTheatres.ToList(),
			HospitalAdministrationActionKind.SupplyAudit => Hospital.SupplyRooms.ToList(),
			_ => Hospital.WaitingRooms.Concat(Hospital.OperatingTheatres).Take(1).ToList()
		};
	}

	private string NoteText()
	{
		return string.IsNullOrWhiteSpace(Note) ? string.Empty : $": {Note}";
	}

	private static EmploymentAuthority RequiredAuthorityFor(HospitalAdministrationActionKind operation)
	{
		return operation switch
		{
			HospitalAdministrationActionKind.DebtAudit => EmploymentAuthority.ManageHospitalAccounts,
			HospitalAdministrationActionKind.TheatrePreparation or HospitalAdministrationActionKind.SupplyAudit =>
				EmploymentAuthority.ManageHospitalFacilities,
			_ => EmploymentAuthority.ManageMedicalServices
		};
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
	}
}
public sealed class ShopStocktakeActionStep : EmploymentActionStepBase
{
	public ShopStocktakeActionStep(ShopStocktakeScope scope = ShopStocktakeScope.All,
		string? merchandiseSelector = null, string? merchandiseName = null)
		: base(
			EmploymentActionStepType.ShopStocktake,
			EmploymentAuthority.ManageStockRules,
			Array.Empty<EmploymentAICapability>(),
			false,
			false)
	{
		Scope = scope;
		MerchandiseSelector = string.IsNullOrWhiteSpace(merchandiseSelector) ? null : merchandiseSelector.Trim();
		MerchandiseName = string.IsNullOrWhiteSpace(merchandiseName) ? MerchandiseSelector : merchandiseName.Trim();
	}

	public ShopStocktakeScope Scope { get; }
	public string? MerchandiseSelector { get; }
	public string? MerchandiseName { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (context.Employer is not IShop shop)
		{
			reason = "Shop stocktake steps can only target shop employment hosts.";
			return false;
		}

		if (Scope == ShopStocktakeScope.Merchandise && ResolveMerchandise(shop) is null)
		{
			reason = $"There is no merchandise belonging to {shop.Name} matching {MerchandiseSelector ?? MerchandiseName ?? "?"}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (context.Employer is not IShop shop)
		{
			return EmploymentActionStepResult.Blocked("Shop stocktake steps can only target shop employment hosts.");
		}

		if (Scope == ShopStocktakeScope.Merchandise)
		{
			var merchandise = ResolveMerchandise(shop);
			if (merchandise is null)
			{
				return EmploymentActionStepResult.Blocked("The merchandise stocktake target is no longer available.");
			}

			var report = BuildReport(shop, merchandise);
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
				$"Stocktook merchandise {merchandise.Name}: {DescribeReport(report, actor)}.");
			return new EmploymentActionStepResult(
				true,
				$"Stocktook {merchandise.Name}: {DescribeReport(report, actor)}.",
				true,
				new EmploymentActionStepOperationalState(OperationalPayload:
					$"stocktake:merchandise={merchandise.Id};{PayloadFor(report)}"));
		}

		var reports = shop.Merchandises
			.OrderBy(x => x.Name)
			.Select(x => BuildReport(shop, x))
			.ToList();
		if (!reports.Any())
		{
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
				$"Stocktook {shop.Name}: no merchandise is configured.");
			return new EmploymentActionStepResult(
				true,
				$"Stocktook {shop.Name}: no merchandise is configured.",
				true,
				new EmploymentActionStepOperationalState(OperationalPayload: "stocktake:all=0"));
		}

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Stocktook {reports.Count.ToString("N0", actor)} merchandise entries for {shop.Name}.");
		return new EmploymentActionStepResult(
			true,
			$"Stocktook {reports.Count.ToString("N0", actor)} merchandise entries for {shop.Name}.",
			true,
			new EmploymentActionStepOperationalState(OperationalPayload:
				$"stocktake:all={reports.Count.ToString("F0", CultureInfo.InvariantCulture)};items={string.Join("|", reports.Select(PayloadFor))}"));
	}

	private IMerchandise? ResolveMerchandise(IShop shop)
	{
		var selector = MerchandiseSelector ?? MerchandiseName;
		if (string.IsNullOrWhiteSpace(selector))
		{
			return null;
		}

		if (long.TryParse(selector.TrimStart('#'), out var id))
		{
			return shop.Merchandises.FirstOrDefault(x => x.Id == id);
		}

		return shop.Merchandises.GetByIdOrName(selector) ??
		       shop.Merchandises.FirstOrDefault(x =>
			       x.Name.EqualTo(selector) ||
			       x.ListDescription.EqualTo(selector));
	}

	private static StocktakeReport BuildReport(IShop shop, IMerchandise merchandise)
	{
		if (merchandise.MerchandiseType == MerchandiseType.Commodity)
		{
			var weight = shop.StocktakeMerchandiseWeight(merchandise);
			return new StocktakeReport(merchandise, true, weight.OnFloorWeight, weight.InStockroomWeight);
		}

		var count = shop.StocktakeMerchandise(merchandise);
		return new StocktakeReport(merchandise, false, count.OnFloorCount, count.InStockroomCount);
	}

	private static string DescribeReport(StocktakeReport report, ICharacter actor)
	{
		return report.UsesWeight
			? $"{report.OnFloor.ToString("N2", actor)} floor weight and {report.InStockroom.ToString("N2", actor)} stockroom weight"
			: $"{((int)report.OnFloor).ToString("N0", actor)} on-floor and {((int)report.InStockroom).ToString("N0", actor)} in-stockroom item(s)";
	}

	private static string PayloadFor(StocktakeReport report)
	{
		var unit = report.UsesWeight ? "weight" : "count";
		return $"{report.Merchandise.Id}:{unit}:{report.OnFloor.ToString("F2", CultureInfo.InvariantCulture)}:{report.InStockroom.ToString("F2", CultureInfo.InvariantCulture)}";
	}

	private sealed record StocktakeReport(IMerchandise Merchandise, bool UsesWeight, double OnFloor,
		double InStockroom);
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
	}

	public PriceChangeActionKind PriceChangeKind { get; }
	public string MerchandiseSelector { get; }
	public MoneyAmount? ExactPrice { get; }
	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

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

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
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
}

public sealed class DeprecatedMarketPriceChangeActionStep : EmploymentActionStepBase
{
	public DeprecatedMarketPriceChangeActionStep(string diagnostic, string? originalPayload = null)
		: base(
			EmploymentActionStepType.PriceChange,
			EmploymentAuthority.AdjustPrices,
			new[] { EmploymentAICapability.CanManagePrices },
			false,
			false)
	{
		Diagnostic = string.IsNullOrWhiteSpace(diagnostic)
			? "Employment market-price actions are deprecated and require builder review."
			: diagnostic.Trim();
		OriginalPayload = originalPayload;
	}

	public string Diagnostic { get; }
	public string? OriginalPayload { get; }
	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		reason = Diagnostic;
		return false;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Blocked deprecated employment market-price action: {Diagnostic}");
		return EmploymentActionStepResult.Blocked(Diagnostic);
	}
}

public sealed class ShopDealAdministrationActionStep : EmploymentActionStepBase
{
	public ShopDealAdministrationActionStep(string name, ShopDealType dealType, ShopDealTargetType targetType,
		string? targetSelector, decimal priceAdjustmentPercentage, int minimumQuantity,
		ShopDealApplicability applicability, IFutureProg? eligibilityProg, bool isCumulative, MudDateTime expiry)
		: base(
			EmploymentActionStepType.ShopDealAdministration,
			EmploymentAuthority.AdjustPrices,
			new[] { EmploymentAICapability.CanManagePrices },
			false,
			false)
	{
		Operation = ShopDealAdministrationActionKind.Create;
		Name = name.Trim();
		DealType = dealType;
		TargetType = targetType;
		TargetSelector = string.IsNullOrWhiteSpace(targetSelector) ? null : targetSelector.Trim();
		PriceAdjustmentPercentage = priceAdjustmentPercentage;
		MinimumQuantity = minimumQuantity;
		Applicability = applicability;
		EligibilityProg = eligibilityProg;
		IsCumulative = isCumulative;
		Expiry = expiry;
		DealSelector = string.Empty;
	}

	public ShopDealAdministrationActionStep(string dealSelector, string name, ShopDealType dealType,
		ShopDealTargetType targetType, string? targetSelector, decimal priceAdjustmentPercentage, int minimumQuantity,
		ShopDealApplicability applicability, IFutureProg? eligibilityProg, bool isCumulative, MudDateTime expiry)
		: base(
			EmploymentActionStepType.ShopDealAdministration,
			EmploymentAuthority.AdjustPrices,
			new[] { EmploymentAICapability.CanManagePrices },
			false,
			false)
	{
		Operation = ShopDealAdministrationActionKind.Modify;
		Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim();
		DealType = dealType;
		TargetType = targetType;
		TargetSelector = string.IsNullOrWhiteSpace(targetSelector) ? null : targetSelector.Trim();
		PriceAdjustmentPercentage = priceAdjustmentPercentage;
		MinimumQuantity = minimumQuantity;
		Applicability = applicability;
		EligibilityProg = eligibilityProg;
		IsCumulative = isCumulative;
		Expiry = expiry;
		DealSelector = dealSelector.Trim();
	}

	public ShopDealAdministrationActionStep(string dealSelector)
		: base(
			EmploymentActionStepType.ShopDealAdministration,
			EmploymentAuthority.AdjustPrices,
			new[] { EmploymentAICapability.CanManagePrices },
			false,
			false)
	{
		Operation = ShopDealAdministrationActionKind.Cancel;
		Name = string.Empty;
		DealType = ShopDealType.Sale;
		TargetType = ShopDealTargetType.AllMerchandise;
		PriceAdjustmentPercentage = 0.0M;
		MinimumQuantity = 0;
		Applicability = ShopDealApplicability.Sell;
		IsCumulative = true;
		Expiry = MudDateTime.Never;
		DealSelector = dealSelector.Trim();
	}

	public ShopDealAdministrationActionKind Operation { get; }
	public string Name { get; }
	public ShopDealType DealType { get; }
	public ShopDealTargetType TargetType { get; }
	public string? TargetSelector { get; }
	public decimal PriceAdjustmentPercentage { get; }
	public int MinimumQuantity { get; }
	public ShopDealApplicability Applicability { get; }
	public IFutureProg? EligibilityProg { get; }
	public bool IsCumulative { get; }
	public MudDateTime Expiry { get; }
	public string DealSelector { get; }
	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (context.Employer is not IShop shop)
		{
			reason = "Shop deal actions can only target shop employment hosts.";
			return false;
		}

		var existingDeal = Operation is ShopDealAdministrationActionKind.Cancel or ShopDealAdministrationActionKind.Modify
			? ResolveDeal(shop)
			: null;
		if (Operation == ShopDealAdministrationActionKind.Cancel)
		{
			if (existingDeal is null)
			{
				reason = $"There is no shop deal matching {DealSelector}.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		if (Operation == ShopDealAdministrationActionKind.Modify && existingDeal is null)
		{
			reason = $"There is no shop deal matching {DealSelector}.";
			return false;
		}

		var effectiveName = Operation == ShopDealAdministrationActionKind.Modify && string.IsNullOrWhiteSpace(Name)
			? existingDeal?.Name ?? string.Empty
			: Name;
		if (string.IsNullOrWhiteSpace(effectiveName))
		{
			reason = Operation == ShopDealAdministrationActionKind.Modify
				? "Shop deal modify actions require an existing deal name."
				: "Shop deal create actions require a deal name.";
			return false;
		}

		if (DealType == ShopDealType.Volume && MinimumQuantity < 2)
		{
			reason = "Volume shop deals require a minimum quantity of 2 or more.";
			return false;
		}

		if (ShopDeals(shop).Any(x => !ReferenceEquals(x, existingDeal) && x.Name.EqualTo(effectiveName) && !x.IsExpired))
		{
			reason = $"{shop.Name} already has an active shop deal named {effectiveName}.";
			return false;
		}

		return TryResolveTarget(shop, actor, out _, out _, out reason);
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (context.Employer is not IShop shop)
		{
			return EmploymentActionStepResult.Blocked("Shop deal actions can only target shop employment hosts.");
		}

		if (Operation == ShopDealAdministrationActionKind.Cancel)
		{
			var existing = ResolveDeal(shop);
			if (existing is null)
			{
				return EmploymentActionStepResult.Blocked($"There is no shop deal matching {DealSelector}.");
			}

			var dealName = existing.Name;
			shop.RemoveDeal(existing);
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
				$"Cancelled native shop deal {dealName}.");
			return new EmploymentActionStepResult(
				true,
				$"Cancelled shop deal {dealName}.",
				true,
				new EmploymentActionStepOperationalState(OperationalPayload: $"sale:cancel={existing.Id};name={dealName}"));
		}

		if (!TryResolveTarget(shop, actor, out var targetMerchandise, out var targetTag, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		if (Operation == ShopDealAdministrationActionKind.Modify)
		{
			var existing = ResolveDeal(shop);
			if (existing is null)
			{
				return EmploymentActionStepResult.Blocked($"There is no shop deal matching {DealSelector}.");
			}

			if (existing is not ShopDeal deal)
			{
				return EmploymentActionStepResult.Blocked("Only native shop deals can be modified by employment automation.");
			}

			var dealName = string.IsNullOrWhiteSpace(Name) ? existing.Name : Name;
			try
			{
				deal.Configure(dealName, DealType, TargetType, targetMerchandise, targetTag, PriceAdjustmentPercentage,
					MinimumQuantity, Applicability, EligibilityProg, IsCumulative, Expiry);
				context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
					$"Modified native shop deal {deal.Name} ({DealType.DescribeEnum()}) for {DescribeTarget()}.");
				return new EmploymentActionStepResult(
					true,
					$"Modified shop deal {deal.Name}.",
					true,
					new EmploymentActionStepOperationalState(
						OperationalPayload:
						$"sale:modify={deal.Id};name={deal.Name};type={DealType};target={TargetType};adjustment={PriceAdjustmentPercentage};minimum={MinimumQuantity};applies={Applicability}"));
			}
			catch (InvalidOperationException ex)
			{
				return EmploymentActionStepResult.Blocked(ex.Message);
			}
		}

		try
		{
			var deal = new ShopDeal(shop, Name);
			deal.Configure(Name, DealType, TargetType, targetMerchandise, targetTag, PriceAdjustmentPercentage,
				MinimumQuantity, Applicability, EligibilityProg, IsCumulative, Expiry);
			shop.AddDeal(deal);
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
				$"Created native shop deal {deal.Name} ({DealType.DescribeEnum()}) for {DescribeTarget()}.");
			return new EmploymentActionStepResult(
				true,
				$"Created shop deal {deal.Name}.",
				true,
				new EmploymentActionStepOperationalState(
					OperationalPayload:
					$"sale:create={deal.Id};name={deal.Name};type={DealType};target={TargetType};adjustment={PriceAdjustmentPercentage};minimum={MinimumQuantity};applies={Applicability}"));
		}
		catch (InvalidOperationException ex)
		{
			return EmploymentActionStepResult.Blocked(ex.Message);
		}
	}

	private IShopDeal? ResolveDeal(IShop shop)
	{
		if (long.TryParse(DealSelector.TrimStart('#'), out var id))
		{
			return ShopDeals(shop).FirstOrDefault(x => x.Id == id);
		}

		return ShopDeals(shop).FirstOrDefault(x => x.Name.EqualTo(DealSelector));
	}

	private bool TryResolveTarget(IShop shop, ICharacter actor, out IMerchandise? merchandise, out ITag? tag,
		out string reason)
	{
		merchandise = null;
		tag = null;
		switch (TargetType)
		{
			case ShopDealTargetType.AllMerchandise:
				reason = string.Empty;
				return true;
			case ShopDealTargetType.Merchandise:
				merchandise = ResolveMerchandise(shop);
				if (merchandise is null)
				{
					reason = $"There is no merchandise belonging to {shop.Name} matching {TargetSelector}.";
					return false;
				}

				reason = string.Empty;
				return true;
			case ShopDealTargetType.ItemTag:
				tag = ResolveTag(actor);
				if (tag is null)
				{
					reason = $"There is no tag matching {TargetSelector}.";
					return false;
				}

				reason = string.Empty;
				return true;
			default:
				reason = "Unsupported shop deal target type.";
				return false;
		}
	}

	private IMerchandise? ResolveMerchandise(IShop shop)
	{
		if (string.IsNullOrWhiteSpace(TargetSelector))
		{
			return null;
		}

		if (long.TryParse(TargetSelector.TrimStart('#'), out var id))
		{
			return shop.Merchandises.FirstOrDefault(x => x.Id == id);
		}

		return shop.Merchandises.GetByIdOrName(TargetSelector) ??
		       shop.Merchandises.FirstOrDefault(x =>
			       x.Name.EqualTo(TargetSelector) ||
			       x.ListDescription.EqualTo(TargetSelector));
	}

	private ITag? ResolveTag(ICharacter actor)
	{
		if (string.IsNullOrWhiteSpace(TargetSelector) || actor.Gameworld?.Tags is null)
		{
			return null;
		}

		if (long.TryParse(TargetSelector.TrimStart('#', '&'), out var id))
		{
			return actor.Gameworld.Tags.Get(id);
		}

		var matched = actor.Gameworld.Tags.FindMatchingTags(TargetSelector.TrimStart('&'));
		return matched.Count == 1 ? matched.Single() : null;
	}

	private string DescribeTarget()
	{
		return TargetType switch
		{
			ShopDealTargetType.AllMerchandise => "all merchandise",
			ShopDealTargetType.Merchandise => $"merchandise {TargetSelector}",
			ShopDealTargetType.ItemTag => $"tag {TargetSelector}",
			_ => TargetType.DescribeEnum()
		};
	}

	private static IEnumerable<IShopDeal> ShopDeals(IShop shop)
	{
		return shop.Deals ?? Array.Empty<IShopDeal>();
	}
}

public sealed class ArenaEventAdministrationActionStep : EmploymentActionStepBase
{
	public ArenaEventAdministrationActionStep(ICombatArena arena, IArenaEventType eventType, DateTime scheduledForUtc)
		: this(arena, ArenaEventAdministrationActionKind.Create, eventType.Id, eventType.Name, null, null,
			scheduledForUtc, null, null)
	{
	}

	public ArenaEventAdministrationActionStep(ICombatArena arena, IArenaEvent arenaEvent, ArenaEventState targetState)
		: this(arena, ArenaEventAdministrationActionKind.Transition, null, null, arenaEvent.Id, arenaEvent.Name,
			null, targetState, null)
	{
	}

	public ArenaEventAdministrationActionStep(ICombatArena arena, IArenaEvent arenaEvent, string reason)
		: this(arena, ArenaEventAdministrationActionKind.Abort, null, null, arenaEvent.Id, arenaEvent.Name,
			null, null, reason)
	{
	}

	public ArenaEventAdministrationActionStep(ICombatArena arena, ArenaEventAdministrationActionKind operation,
		long? eventTypeId, string? eventTypeName, long? eventId, string? eventName, DateTime? scheduledForUtc,
		ArenaEventState? targetState, string? reason)
		: base(
			EmploymentActionStepType.ArenaEventAdministration,
			RequiredAuthorityFor(operation),
			Array.Empty<EmploymentAICapability>(),
			false,
			false)
	{
		Arena = arena;
		Operation = operation;
		EventTypeId = eventTypeId;
		EventTypeName = string.IsNullOrWhiteSpace(eventTypeName) ? null : eventTypeName.Trim();
		EventId = eventId;
		EventName = string.IsNullOrWhiteSpace(eventName) ? null : eventName.Trim();
		ScheduledForUtc = scheduledForUtc;
		TargetState = targetState;
		Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
	}

	public ICombatArena Arena { get; }
	public ArenaEventAdministrationActionKind Operation { get; }
	public long? EventTypeId { get; }
	public string? EventTypeName { get; }
	public long? EventId { get; }
	public string? EventName { get; }
	public DateTime? ScheduledForUtc { get; }
	public ArenaEventState? TargetState { get; }
	public string? Reason { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (!IsArenaHost(context))
		{
			reason = "Arena event steps must execute from the owning combat arena employment task board.";
			return false;
		}

		switch (Operation)
		{
			case ArenaEventAdministrationActionKind.Create:
			{
				if (!ScheduledForUtc.HasValue)
				{
					reason = "Arena event creation steps require a scheduled date/time.";
					return false;
				}

				var eventType = ResolveEventType();
				if (eventType is null)
				{
					reason = $"There is no arena event type matching {EventTypeName ?? EventTypeId?.ToString("F0", CultureInfo.InvariantCulture) ?? "the selected type"}.";
					return false;
				}

				var readiness = Arena.IsReadyToHost(eventType);
				if (!readiness.Truth)
				{
					reason = readiness.Reason;
					return false;
				}

				reason = string.Empty;
				return true;
			}
			case ArenaEventAdministrationActionKind.Transition:
			{
				var arenaEvent = ResolveEvent();
				if (arenaEvent is null)
				{
					reason = $"There is no active arena event matching {EventName ?? EventId?.ToString("F0", CultureInfo.InvariantCulture) ?? "the selected event"}.";
					return false;
				}

				if (!TargetState.HasValue)
				{
					reason = "Arena event phase steps require a target state.";
					return false;
				}

				if (TargetState.Value == ArenaEventState.Aborted)
				{
					reason = "Use an arenaevent abort step to abort an arena event with an auditable reason.";
					return false;
				}

				if (arenaEvent.State == TargetState.Value)
				{
					reason = "That arena event is already in the requested state.";
					return false;
				}

				if (TargetState.Value < arenaEvent.State)
				{
					reason = $"Arena event phase steps can only move forward from {arenaEvent.State.DescribeEnum()}.";
					return false;
				}

				reason = string.Empty;
				return true;
			}
			case ArenaEventAdministrationActionKind.Abort:
			{
				var arenaEvent = ResolveEvent();
				if (arenaEvent is null)
				{
					reason = $"There is no active arena event matching {EventName ?? EventId?.ToString("F0", CultureInfo.InvariantCulture) ?? "the selected event"}.";
					return false;
				}

				if (string.IsNullOrWhiteSpace(Reason))
				{
					reason = "Arena event abort steps require an auditable reason.";
					return false;
				}

				reason = string.Empty;
				return true;
			}
			default:
				reason = "Unsupported arena event operation.";
				return false;
		}
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!CanExecute(context, actor, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		switch (Operation)
		{
			case ArenaEventAdministrationActionKind.Create:
			{
				var eventType = ResolveEventType()!;
				var created = Arena.CreateEvent(eventType, ScheduledForUtc!.Value);
				var description = $"Created arena event {created.Name} for {ScheduledForUtc.Value.ToString("u", CultureInfo.InvariantCulture)}.";
				context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
					CurrentCorrelationId(context));
				return new EmploymentActionStepResult(true, description, true,
					new EmploymentActionStepOperationalState(
						SelectedResources: $"arenaevent:create;arena={Arena.Id.ToString("F0", CultureInfo.InvariantCulture)};event={created.Id.ToString("F0", CultureInfo.InvariantCulture)};type={eventType.Id.ToString("F0", CultureInfo.InvariantCulture)}",
						OperationalPayload: $"arenaevent:create;arena={Arena.Id.ToString("F0", CultureInfo.InvariantCulture)};event={created.Id.ToString("F0", CultureInfo.InvariantCulture)};type={eventType.Id.ToString("F0", CultureInfo.InvariantCulture)};scheduled={ScheduledForUtc.Value.ToString("O", CultureInfo.InvariantCulture)}"));
			}
			case ArenaEventAdministrationActionKind.Transition:
			{
				var arenaEvent = ResolveEvent()!;
				var previous = arenaEvent.State;
				if (actor.Gameworld?.ArenaLifecycleService is null)
				{
					return EmploymentActionStepResult.Blocked("The arena lifecycle service is not available.");
				}

				actor.Gameworld.ArenaLifecycleService.Transition(arenaEvent, TargetState!.Value);
				if (arenaEvent.State == previous)
				{
					return EmploymentActionStepResult.Blocked("The native arena lifecycle service did not accept that transition.");
				}

				var description = $"Moved arena event {arenaEvent.Name} from {previous.DescribeEnum()} to {arenaEvent.State.DescribeEnum()}.";
				context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
					CurrentCorrelationId(context));
				return new EmploymentActionStepResult(true, description, true,
					new EmploymentActionStepOperationalState(
						SelectedResources: $"arenaevent:phase;arena={Arena.Id.ToString("F0", CultureInfo.InvariantCulture)};event={arenaEvent.Id.ToString("F0", CultureInfo.InvariantCulture)};state={arenaEvent.State}",
						OperationalPayload: $"arenaevent:phase;arena={Arena.Id.ToString("F0", CultureInfo.InvariantCulture)};event={arenaEvent.Id.ToString("F0", CultureInfo.InvariantCulture)};from={previous};to={arenaEvent.State}"));
			}
			case ArenaEventAdministrationActionKind.Abort:
			{
				var arenaEvent = ResolveEvent()!;
				Arena.AbortEvent(arenaEvent, Reason!, actor);
				var description = $"Aborted arena event {arenaEvent.Name}: {Reason}.";
				context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, description,
					CurrentCorrelationId(context));
				return new EmploymentActionStepResult(true, description, true,
					new EmploymentActionStepOperationalState(
						SelectedResources: $"arenaevent:abort;arena={Arena.Id.ToString("F0", CultureInfo.InvariantCulture)};event={arenaEvent.Id.ToString("F0", CultureInfo.InvariantCulture)}",
						OperationalPayload: $"arenaevent:abort;arena={Arena.Id.ToString("F0", CultureInfo.InvariantCulture)};event={arenaEvent.Id.ToString("F0", CultureInfo.InvariantCulture)};reason={Reason}"));
			}
			default:
				return EmploymentActionStepResult.Blocked("Unsupported arena event operation.");
		}
	}

	private IArenaEventType? ResolveEventType()
	{
		if (EventTypeId.HasValue)
		{
			return Arena.EventTypes.FirstOrDefault(x => x.Id == EventTypeId.Value);
		}

		return string.IsNullOrWhiteSpace(EventTypeName)
			? null
			: Arena.EventTypes.FirstOrDefault(x => x.Name.EqualTo(EventTypeName));
	}

	private IArenaEvent? ResolveEvent()
	{
		if (EventId.HasValue)
		{
			return Arena.ActiveEvents.FirstOrDefault(x => x.Id == EventId.Value);
		}

		return string.IsNullOrWhiteSpace(EventName)
			? null
			: Arena.ActiveEvents.FirstOrDefault(x => x.Name.EqualTo(EventName));
	}

	private bool IsArenaHost(IEmploymentTaskContext context)
	{
		return context.Employer.Id == Arena.Id &&
		       context.Employer.FrameworkItemType.Equals(Arena.FrameworkItemType, StringComparison.OrdinalIgnoreCase);
	}

	private static EmploymentAuthority RequiredAuthorityFor(ArenaEventAdministrationActionKind operation)
	{
		return operation == ArenaEventAdministrationActionKind.Create
			? EmploymentAuthority.CreateScheduledRules
			: EmploymentAuthority.ModifyScheduledRules;
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
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

public sealed class ScheduledRuleAdministrationActionStep : EmploymentActionStepBase
{
	public ScheduledRuleAdministrationActionStep(ScheduledRuleAdministrationActionKind operation, Guid ruleId,
		string ruleName, string? reason = null, string? manualKey = null)
		: base(
			EmploymentActionStepType.ScheduledRuleAdministration,
			EmploymentAuthority.ModifyScheduledRules,
			Array.Empty<EmploymentAICapability>(),
			false,
			false)
	{
		Operation = operation;
		RuleId = ruleId;
		RuleName = string.IsNullOrWhiteSpace(ruleName) ? ruleId.ToString("D") : ruleName.Trim();
		Reason = string.IsNullOrWhiteSpace(reason) ? DefaultReason(operation) : reason.Trim();
		ManualKey = string.IsNullOrWhiteSpace(manualKey) ? null : manualKey.Trim();
	}

	public ScheduledRuleAdministrationActionKind Operation { get; }
	public Guid RuleId { get; }
	public string RuleName { get; }
	public string Reason { get; }
	public string? ManualKey { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (RuleId == Guid.Empty)
		{
			reason = "Scheduled-rule administration steps require a durable scheduled-rule id.";
			return false;
		}

		if (ResolveRule(context.Employer) is null)
		{
			reason = $"There is no scheduled employment rule matching {RuleName} ({RuleId:D}) for {context.Employer.EmploymentHostName}.";
			return false;
		}

		if (Operation == ScheduledRuleAdministrationActionKind.Evaluate &&
		    !string.IsNullOrWhiteSpace(ManualKey) && context is not EmploymentTaskContext)
		{
			reason = "Manual-trigger scheduled-rule evaluation requires an employment task context that can carry manual triggers.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var rule = ResolveRule(context.Employer);
		if (rule is null)
		{
			return EmploymentActionStepResult.Blocked($"The scheduled rule {RuleName} is no longer available.");
		}

		try
		{
			return Operation switch
			{
				ScheduledRuleAdministrationActionKind.Pause => ExecutePause(context, actor, rule),
				ScheduledRuleAdministrationActionKind.Resume => ExecuteResume(context, actor, rule),
				ScheduledRuleAdministrationActionKind.Cancel => ExecuteCancel(context, actor, rule),
				ScheduledRuleAdministrationActionKind.Evaluate => ExecuteEvaluate(context, actor, rule),
				_ => EmploymentActionStepResult.Failed("Unknown scheduled-rule administration action.")
			};
		}
		catch (InvalidOperationException ex)
		{
			return EmploymentActionStepResult.Blocked(ex.Message);
		}
	}

	private EmploymentActionStepResult ExecutePause(IEmploymentTaskContext context, ICharacter actor,
		IEmploymentScheduledTaskRule rule)
	{
		if (!context.Employer.TaskBoard.PauseScheduledRule(rule, actor, Reason))
		{
			return EmploymentActionStepResult.Blocked($"Could not pause scheduled rule {rule.Name}.");
		}

		return Completed($"Paused scheduled rule {rule.Name}.", rule, "pause", 0, null);
	}

	private EmploymentActionStepResult ExecuteResume(IEmploymentTaskContext context, ICharacter actor,
		IEmploymentScheduledTaskRule rule)
	{
		if (!context.Employer.TaskBoard.ResumeScheduledRule(rule, actor, Reason))
		{
			return EmploymentActionStepResult.Blocked($"Could not resume scheduled rule {rule.Name}.");
		}

		return Completed($"Resumed scheduled rule {rule.Name}.", rule, "resume", 0, null);
	}

	private EmploymentActionStepResult ExecuteCancel(IEmploymentTaskContext context, ICharacter actor,
		IEmploymentScheduledTaskRule rule)
	{
		var name = rule.Name;
		var id = rule.Id;
		if (!context.Employer.TaskBoard.CancelScheduledRule(rule, actor, Reason))
		{
			return EmploymentActionStepResult.Blocked($"Could not cancel scheduled rule {name}.");
		}

		return Completed($"Cancelled scheduled rule {name}.", id, name, "cancel", 0, null);
	}

	private EmploymentActionStepResult ExecuteEvaluate(IEmploymentTaskContext context, ICharacter actor,
		IEmploymentScheduledTaskRule rule)
	{
		if (!string.IsNullOrWhiteSpace(ManualKey) && context is EmploymentTaskContext concreteContext)
		{
			concreteContext.SetManualOrder(ManualKey, true);
		}

		var now = EmploymentClock.CurrentInstant(context.Employer);
		var spawned = context.Employer.TaskBoard.EvaluateScheduledRule(rule, context, now);
		string? blockedReason = null;
		if (!spawned.Any() && !rule.CanSpawn(context, now, out blockedReason))
		{
			blockedReason = string.IsNullOrWhiteSpace(blockedReason) ? "No active task was spawned." : blockedReason;
		}

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Evaluated scheduled task rule {rule.Name} from an employment task and spawned {spawned.Count.ToString("N0", CultureInfo.InvariantCulture)} active task(s).");
		var message = spawned.Any()
			? $"Evaluated scheduled rule {rule.Name} and spawned {spawned.Count.ToString("N0", CultureInfo.InvariantCulture)} active task{(spawned.Count == 1 ? string.Empty : "s")}."
			: $"Evaluated scheduled rule {rule.Name}; no active tasks spawned{(string.IsNullOrWhiteSpace(blockedReason) ? "." : $": {blockedReason}")}";
		return Completed(message, rule, "evaluate", spawned.Count, blockedReason);
	}

	private EmploymentActionStepResult Completed(string message, IEmploymentScheduledTaskRule rule, string operation,
		int spawnedCount, string? diagnostic)
	{
		return Completed(message, rule.Id, rule.Name, operation, spawnedCount, diagnostic);
	}

	private EmploymentActionStepResult Completed(string message, Guid ruleId, string ruleName, string operation,
		int spawnedCount, string? diagnostic)
	{
		var payload = $"rule:{operation}={ruleId:D};name={ruleName};reason={Reason};spawned={spawnedCount.ToString("F0", CultureInfo.InvariantCulture)}";
		if (!string.IsNullOrWhiteSpace(ManualKey))
		{
			payload += $";manual={ManualKey}";
		}

		if (!string.IsNullOrWhiteSpace(diagnostic))
		{
			payload += $";diagnostic={diagnostic}";
		}

		return new EmploymentActionStepResult(
			true,
			message,
			true,
			new EmploymentActionStepOperationalState(OperationalPayload: payload));
	}

	private IEmploymentScheduledTaskRule? ResolveRule(IEmploymentHost host)
	{
		return host.TaskBoard.ScheduledRules.FirstOrDefault(x => x.Id == RuleId) ??
		       host.TaskBoard.ScheduledRules.FirstOrDefault(x => x.Name.EqualTo(RuleName));
	}

	private static string DefaultReason(ScheduledRuleAdministrationActionKind operation)
	{
		return operation switch
		{
			ScheduledRuleAdministrationActionKind.Pause => "Paused by employment task.",
			ScheduledRuleAdministrationActionKind.Resume => "Resumed by employment task.",
			ScheduledRuleAdministrationActionKind.Cancel => "Cancelled by employment task.",
			ScheduledRuleAdministrationActionKind.Evaluate => "Evaluated by employment task.",
			_ => "Administered by employment task."
		};
	}
}

public sealed class ActiveTaskAdministrationActionStep : EmploymentActionStepBase
{
	public ActiveTaskAdministrationActionStep(ActiveTaskAdministrationActionKind operation, Guid taskId,
		string taskName, long? employeeId = null, string? employeeName = null, string? reason = null)
		: base(
			EmploymentActionStepType.ActiveTaskAdministration,
			RequiredAuthorityFor(operation),
			Array.Empty<EmploymentAICapability>(),
			false,
			false)
	{
		Operation = operation;
		TaskId = taskId;
		TaskName = string.IsNullOrWhiteSpace(taskName) ? taskId.ToString("D") : taskName.Trim();
		EmployeeId = employeeId;
		EmployeeName = string.IsNullOrWhiteSpace(employeeName) ? null : employeeName.Trim();
		Reason = string.IsNullOrWhiteSpace(reason) ? DefaultReason(operation) : reason.Trim();
	}

	public ActiveTaskAdministrationActionKind Operation { get; }
	public Guid TaskId { get; }
	public string TaskName { get; }
	public long? EmployeeId { get; }
	public string? EmployeeName { get; }
	public string Reason { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (TaskId == Guid.Empty)
		{
			reason = "The target active task id is missing.";
			return false;
		}

		var task = ResolveTask(context.Employer);
		if (task is null)
		{
			reason = $"There is no active task matching {TaskName}.";
			return false;
		}

		if (IsSelfTarget(context, task))
		{
			reason = "A task administration step cannot target its own active task.";
			return false;
		}

		switch (Operation)
		{
			case ActiveTaskAdministrationActionKind.Retry:
				if (task.Status is EmploymentTaskStatus.Blocked or EmploymentTaskStatus.Failed)
				{
					return true;
				}

				reason = $"Task {task.Name} is {task.Status.DescribeEnum()} and cannot be retried.";
				return false;
			case ActiveTaskAdministrationActionKind.Requeue:
				if (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Cancelled and not EmploymentTaskStatus.Failed)
				{
					return true;
				}

				reason = $"Task {task.Name} is {task.Status.DescribeEnum()} and cannot be requeued.";
				return false;
			case ActiveTaskAdministrationActionKind.Cancel:
				if (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Cancelled and not EmploymentTaskStatus.Failed)
				{
					return true;
				}

				reason = $"Task {task.Name} is {task.Status.DescribeEnum()} and cannot be cancelled.";
				return false;
			case ActiveTaskAdministrationActionKind.Assign:
				var employee = ResolveEmployee(context.Employer, actor);
				if (employee is null)
				{
					reason = $"There is no active employee matching {EmployeeName ?? EmployeeId?.ToString("N0") ?? "?"}.";
					return false;
				}

				if (!context.Employer.EmploymentContracts.Any(x => x.Employee.Id == employee.Id && x.Status == EmploymentStatus.Active))
				{
					reason = $"{employee.Name} does not have an active employment contract with {context.Employer.EmploymentHostName}.";
					return false;
				}

				if (task.Status is EmploymentTaskStatus.Completed or EmploymentTaskStatus.Cancelled or EmploymentTaskStatus.Failed)
				{
					reason = $"Task {task.Name} is {task.Status.DescribeEnum()} and cannot be assigned.";
					return false;
				}

				return true;
			default:
				reason = "Unknown active-task administration operation.";
				return false;
		}
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var task = ResolveTask(context.Employer);
		if (task is null)
		{
			return EmploymentActionStepResult.Failed($"There is no active task matching {TaskName}.");
		}

		if (IsSelfTarget(context, task))
		{
			return EmploymentActionStepResult.Failed("A task administration step cannot target its own active task.");
		}

		try
		{
			var success = Operation switch
			{
				ActiveTaskAdministrationActionKind.Retry =>
					context.Employer.TaskBoard.RetryActiveTask(task, actor, Reason),
				ActiveTaskAdministrationActionKind.Requeue =>
					context.Employer.TaskBoard.RequeueActiveTask(task, actor, Reason),
				ActiveTaskAdministrationActionKind.Cancel =>
					context.Employer.TaskBoard.CancelActiveTask(task, actor, Reason),
				ActiveTaskAdministrationActionKind.Assign when ResolveEmployee(context.Employer, actor) is { } employee =>
					context.Employer.TaskBoard.AssignActiveTask(task, employee, context, actor, Reason),
				_ => false
			};
			if (!success)
			{
				return EmploymentActionStepResult.Failed($"Could not {Operation.ToString().ToLowerInvariant()} active task {task.Name}.");
			}

			var payload = $"taskadmin:{Operation.ToString().ToLowerInvariant()}={task.Id:D};name={task.Name};reason={Reason}";
			if (Operation == ActiveTaskAdministrationActionKind.Assign)
			{
				payload += $";employee={EmployeeId?.ToString("F0") ?? EmployeeName}";
			}

			return new EmploymentActionStepResult(
				true,
				$"{Operation.DescribeEnum()} active task {task.Name}.",
				true,
				new EmploymentActionStepOperationalState(OperationalPayload: payload));
		}
		catch (InvalidOperationException ex)
		{
			return EmploymentActionStepResult.Blocked(ex.Message);
		}
	}

	private IEmploymentActiveTask? ResolveTask(IEmploymentHost host)
	{
		return host.TaskBoard.ActiveTasks.FirstOrDefault(x => x.Id == TaskId) ??
		       host.TaskBoard.ActiveTasks.FirstOrDefault(x => x.Name.EqualTo(TaskName));
	}

	private ICharacter? ResolveEmployee(IEmploymentHost host, ICharacter actor)
	{
		if (EmployeeId.HasValue)
		{
			var contractEmployee = host.EmploymentContracts.FirstOrDefault(x => x.Employee.Id == EmployeeId.Value)?.Employee;
			if (contractEmployee is not null)
			{
				return contractEmployee;
			}

			return TryGetGameworld(host, actor)?.TryGetCharacter(EmployeeId.Value, true);
		}

		if (string.IsNullOrWhiteSpace(EmployeeName))
		{
			return null;
		}

		return host.EmploymentContracts
		           .Select(x => x.Employee)
		           .FirstOrDefault(x => x.Name.EqualTo(EmployeeName)) ??
		       host.EmploymentContracts
		           .Select(x => x.Employee)
		           .FirstOrDefault(x => x.Name.StartsWith(EmployeeName, StringComparison.InvariantCultureIgnoreCase));
	}

	private static bool IsSelfTarget(IEmploymentTaskContext context, IEmploymentActiveTask task)
	{
		return context is EmploymentTaskContext taskContext &&
		       taskContext.CurrentTask?.Id == task.Id;
	}

	private static IFuturemud? TryGetGameworld(IEmploymentHost host, ICharacter actor)
	{
		try
		{
			return actor.Gameworld ?? (host as IHaveFuturemud)?.Gameworld;
		}
		catch (InvalidOperationException)
		{
			return (host as IHaveFuturemud)?.Gameworld;
		}
	}

	private static EmploymentAuthority RequiredAuthorityFor(ActiveTaskAdministrationActionKind operation)
	{
		return operation == ActiveTaskAdministrationActionKind.Cancel
			? EmploymentAuthority.CancelTasks
			: EmploymentAuthority.AssignTasks;
	}

	private static string DefaultReason(ActiveTaskAdministrationActionKind operation)
	{
		return operation switch
		{
			ActiveTaskAdministrationActionKind.Retry => "Retried by employment task.",
			ActiveTaskAdministrationActionKind.Requeue => "Requeued by employment task.",
			ActiveTaskAdministrationActionKind.Assign => "Assigned by employment task.",
			ActiveTaskAdministrationActionKind.Cancel => "Cancelled by employment task.",
			_ => "Administered by employment task."
		};
	}
}

public sealed class ManagerGoalAdministrationActionStep : EmploymentActionStepBase
{
	public ManagerGoalAdministrationActionStep(ManagerGoalAdministrationActionKind operation, long goalId,
		string goalName, string? reason = null)
		: base(
			EmploymentActionStepType.ManagerGoalAdministration,
			EmploymentAuthority.ModifyManagerGoals,
			Array.Empty<EmploymentAICapability>(),
			false,
			false)
	{
		Operation = operation;
		GoalId = goalId;
		GoalName = string.IsNullOrWhiteSpace(goalName) ? goalId.ToString("N0", CultureInfo.InvariantCulture) : goalName.Trim();
		Reason = string.IsNullOrWhiteSpace(reason) ? DefaultReason(operation) : reason.Trim();
	}

	public ManagerGoalAdministrationActionKind Operation { get; }
	public long GoalId { get; }
	public string GoalName { get; }
	public string Reason { get; }

	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		if (!base.CanExecute(context, actor, out reason))
		{
			return false;
		}

		if (GoalId <= 0)
		{
			reason = "The target manager goal id is missing.";
			return false;
		}

		var goal = ResolveGoal(context.Employer);
		if (goal is null)
		{
			reason = $"There is no manager goal matching {GoalName}.";
			return false;
		}

		switch (Operation)
		{
			case ManagerGoalAdministrationActionKind.Evaluate:
				if (goal.Status is ManagerGoalStatus.Active or ManagerGoalStatus.Satisfied)
				{
					return true;
				}

				reason = $"Manager goal #{goal.Id:N0} is {goal.Status.DescribeEnum()} and cannot be evaluated until reactivated.";
				return false;
			case ManagerGoalAdministrationActionKind.Cancel:
				if (goal.Status is not ManagerGoalStatus.Cancelled and not ManagerGoalStatus.Completed)
				{
					return true;
				}

				reason = $"Manager goal #{goal.Id:N0} is {goal.Status.DescribeEnum()} and cannot be cancelled.";
				return false;
			case ManagerGoalAdministrationActionKind.Reactivate:
				if (goal.Status is ManagerGoalStatus.Blocked or ManagerGoalStatus.Failed or ManagerGoalStatus.Satisfied or ManagerGoalStatus.Suspended)
				{
					return true;
				}

				reason = $"Manager goal #{goal.Id:N0} is {goal.Status.DescribeEnum()} and cannot be reactivated.";
				return false;
			default:
				reason = "Unknown manager-goal administration operation.";
				return false;
		}
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var goal = ResolveGoal(context.Employer);
		if (goal is null)
		{
			return EmploymentActionStepResult.Failed($"There is no manager goal matching {GoalName}.");
		}

		try
		{
			return Operation switch
			{
				ManagerGoalAdministrationActionKind.Evaluate => ExecuteEvaluate(context, actor, goal),
				ManagerGoalAdministrationActionKind.Cancel => ExecuteCancel(context, actor, goal),
				ManagerGoalAdministrationActionKind.Reactivate => ExecuteReactivate(context, actor, goal),
				_ => EmploymentActionStepResult.Failed("Unknown manager-goal administration action.")
			};
		}
		catch (InvalidOperationException ex)
		{
			return EmploymentActionStepResult.Blocked(ex.Message);
		}
	}

	private EmploymentActionStepResult ExecuteEvaluate(IEmploymentTaskContext context, ICharacter actor, IManagerGoal goal)
	{
		var now = EmploymentClock.CurrentInstant(context.Employer);
		var spawned = context.Employer.ManagerGoalBoard.EvaluateGoal(goal, context, now);
		var diagnostic = spawned.Any() ? null : goal.LastEvaluationResult;
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Evaluated manager goal #{goal.Id:N0} from an employment task and spawned {spawned.Count.ToString("N0", CultureInfo.InvariantCulture)} active task(s).",
			goal.CorrelationId);
		var message = spawned.Any()
			? $"Evaluated manager goal #{goal.Id:N0} and spawned {spawned.Count.ToString("N0", CultureInfo.InvariantCulture)} active task{(spawned.Count == 1 ? string.Empty : "s")}."
			: $"Evaluated manager goal #{goal.Id:N0}; no active tasks spawned{(string.IsNullOrWhiteSpace(diagnostic) ? "." : $": {diagnostic}")}";
		return Completed(message, goal, "evaluate", spawned.Count, diagnostic);
	}

	private EmploymentActionStepResult ExecuteCancel(IEmploymentTaskContext context, ICharacter actor, IManagerGoal goal)
	{
		context.Employer.ManagerGoalBoard.CancelGoal(goal, actor, Reason);
		return Completed($"Cancelled manager goal #{goal.Id:N0}.", goal, "cancel", 0, null);
	}

	private EmploymentActionStepResult ExecuteReactivate(IEmploymentTaskContext context, ICharacter actor,
		IManagerGoal goal)
	{
		if (!context.Employer.ManagerGoalBoard.ReactivateGoal(goal, actor, Reason))
		{
			return EmploymentActionStepResult.Failed($"Could not reactivate manager goal #{goal.Id:N0}.");
		}

		return Completed($"Reactivated manager goal #{goal.Id:N0}.", goal, "reactivate", 0, null);
	}

	private EmploymentActionStepResult Completed(string message, IManagerGoal goal, string operation,
		int spawnedCount, string? diagnostic)
	{
		var payload = $"goal:{operation}={goal.Id.ToString("F0", CultureInfo.InvariantCulture)};type={goal.GoalType};description={goal.Configuration.Description};reason={Reason};spawned={spawnedCount.ToString("F0", CultureInfo.InvariantCulture)}";
		if (!string.IsNullOrWhiteSpace(diagnostic))
		{
			payload += $";diagnostic={diagnostic}";
		}

		return new EmploymentActionStepResult(
			true,
			message,
			true,
			new EmploymentActionStepOperationalState(OperationalPayload: payload));
	}

	private IManagerGoal? ResolveGoal(IEmploymentHost host)
	{
		return host.ManagerGoalBoard.Goals.FirstOrDefault(x => x.Id == GoalId) ??
		       host.ManagerGoalBoard.Goals.FirstOrDefault(x => x.Configuration.Description.EqualTo(GoalName));
	}

	private static string DefaultReason(ManagerGoalAdministrationActionKind operation)
	{
		return operation switch
		{
			ManagerGoalAdministrationActionKind.Evaluate => "Evaluated by employment task.",
			ManagerGoalAdministrationActionKind.Cancel => "Cancelled by employment task.",
			ManagerGoalAdministrationActionKind.Reactivate => "Reactivated by employment task.",
			_ => "Administered by employment task."
		};
	}
}
