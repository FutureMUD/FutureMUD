using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Framework;
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

public sealed class PurchaseActionStep : EmploymentActionStepBase
{
	public PurchaseActionStep(string purchaseDescription, MoneyAmount amount, string? existingFinancialRecord = null)
		: base(
			EmploymentActionStepType.Purchase,
			EmploymentAuthority.ApprovePurchases,
			new[] { EmploymentAICapability.CanPurchaseCommodities },
			true,
			true)
	{
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

	public string PurchaseDescription { get; }
	public MoneyAmount Amount { get; }
	public int? Quantity { get; }
	public string? MerchandiseSelector { get; }
	public string? SupplierSelector { get; }
	public MoneyAmount? MaximumAmount { get; }
	public string? KeywordFilter { get; }
	public string? ExistingFinancialRecord { get; }

	public bool IsExecutablePurchase => Quantity.HasValue && !string.IsNullOrWhiteSpace(MerchandiseSelector);

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

		return new EmploymentActionStepResult(true, $"Started craft {CraftDescription}.", true, operationalState);
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

		if (ResolveContainer(context, actor) is null)
		{
			reason = $"There is no return container matching {EmploymentItemSelectorResolver.Describe(ContainerSelector)}.";
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
		return context.CarriedTaskItems(actor).Any(x => x.Id == item.Id) ||
		       actor.Inventory.Any(x => x.Id == item.Id);
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
