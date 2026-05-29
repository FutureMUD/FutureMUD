using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Employment;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Economy.Employment;

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

	public string PurchaseDescription { get; }
	public MoneyAmount Amount { get; }
	public string? ExistingFinancialRecord { get; }

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Used payment authorisation for purchase: {PurchaseDescription}.");
		context.RecordLedger(EmploymentLedgerEntryType.Purchase, actor, Amount, PurchaseDescription);
		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, Amount,
				$"Reused existing financial record {ExistingFinancialRecord} for purchase.");
		}

		return EmploymentActionStepResult.CompletedResult($"Purchased {PurchaseDescription}.");
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

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, null,
				$"Reused existing material-cost record {ExistingFinancialRecord} for craft trigger.");
		}

		return EmploymentActionStepResult.CompletedResult($"Triggered craft: {CraftDescription}.");
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

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			"Used payment authorisation for bank deposit.");
		context.RecordLedger(EmploymentLedgerEntryType.BankDeposit, actor, Amount, "Bank deposit task step.");
		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, Amount,
				$"Reused existing bank deposit record {ExistingFinancialRecord}.");
		}

		return EmploymentActionStepResult.CompletedResult("Deposited business cash.");
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

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			"Used payment authorisation for bank withdrawal.");
		context.RecordLedger(EmploymentLedgerEntryType.BankWithdrawal, actor, Amount, "Bank withdrawal task step.");
		if (!string.IsNullOrWhiteSpace(ExistingFinancialRecord))
		{
			context.RecordLedger(EmploymentLedgerEntryType.ExistingFinancialRecordReuse, actor, Amount,
				$"Reused existing bank withdrawal record {ExistingFinancialRecord}.");
		}

		return EmploymentActionStepResult.CompletedResult("Withdrew business cash.");
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

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
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

		return EmploymentActionStepResult.CompletedResult($"Paid store account {AccountName}.");
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

public sealed class GetItemsByIdActionStep : EmploymentActionStepBase, IEmploymentActionStepLocationHint
{
	private readonly List<long> _itemPrototypeIds;
	private readonly List<ICell> _sourceLocations;

	public GetItemsByIdActionStep(int quantity, IEnumerable<long> itemPrototypeIds, IEnumerable<ICell> sourceLocations)
		: base(
			EmploymentActionStepType.GetItemsById,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		Quantity = quantity;
		_itemPrototypeIds = itemPrototypeIds.Distinct().ToList();
		_sourceLocations = sourceLocations.Distinct().ToList();
	}

	public int Quantity { get; }
	public IReadOnlyList<long> ItemPrototypeIds => _itemPrototypeIds;
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

		if (!_itemPrototypeIds.Any())
		{
			reason = "Item retrieval steps must specify at least one item prototype id.";
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

		if (!context.TryCollectTaskItems(actor, items.Select(x => (x.Item, x.Source)).ToList(), out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return EmploymentActionStepResult.CompletedResult($"Collected {items.Count:N0} item(s) by prototype id.");
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
		foreach (var source in sources)
		foreach (var item in context.AvailableItems(source).Where(x => prototypeIds.Contains(x.Prototype.Id)))
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

		if (!context.TryCollectTaskItems(actor, items.Select(x => (x.Item, x.Source)).ToList(), out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return EmploymentActionStepResult.CompletedResult($"Collected {items.Count:N0} item(s) tagged {TagName}.");
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

		if (!context.TryCollectTaskItems(actor, collected.Select(x => (x.Item, x.Source)).ToList(), out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return EmploymentActionStepResult.CompletedResult(
			$"Collected {totalWeight:N2} weight of {MaterialName} commodity in {collected.Count:N0} item(s).");
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
		: base(
			EmploymentActionStepType.DeliverItems,
			EmploymentAuthority.ManageDeliveryRoutes,
			new[] { EmploymentAICapability.CanDeliverItems },
			false,
			false)
	{
		Destination = destination;
		Container = container;
		ContainerTag = containerTag;
	}

	public ICell Destination { get; }
	public IGameItem? Container { get; }
	public string? ContainerTag { get; }

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
			reason = "The assigned employee is not carrying any task items to deliver.";
			return false;
		}

		if (Container is null && !string.IsNullOrWhiteSpace(ContainerTag) &&
		    !context.AvailableItems(Destination).Any(x => context.ItemHasTag(x, ContainerTag)))
		{
			reason = $"There is no destination container tagged {ContainerTag}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
	{
		var count = context.CarriedTaskItems(actor).Count;
		if (!context.TryDeliverTaskItems(actor, Destination, Container, ContainerTag, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return EmploymentActionStepResult.CompletedResult($"Delivered {count:N0} task item(s).");
	}

	public IReadOnlyCollection<ICell> ExecutionLocationHints(IEmploymentTaskContext context, ICharacter actor)
	{
		return [Destination];
	}
}
