using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Employment;

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

	protected EmploymentActionStepResult CompleteWithRegister(IEmploymentTaskContext context, ICharacter actor,
		string message)
	{
		context.RecordRegister(EmploymentRegisterEntryType.ActionStepCompleted, actor, message);
		return EmploymentActionStepResult.CompletedResult(message);
	}
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

public sealed class MovementDeliveryActionStep : EmploymentActionStepBase
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

public sealed class CommandActionStep : EmploymentActionStepBase
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
