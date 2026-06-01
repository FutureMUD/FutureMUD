using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.TimeAndDate;

#nullable enable

namespace MudSharp.Economy.Employment;

internal static class EmploymentFinanceService
{
	private const string PayloadPrefix = "finance-v1";
	private const string PayloadSeparator = "||";

	private sealed record FinanceHost(IFrameworkItem Owner, ICurrency Currency, IBankAccount? BankAccount);

	private sealed record FinanceReservation(
		string Operation,
		Guid Id,
		Guid TaskId,
		long CurrencyId,
		decimal Amount,
		string Description);

	private sealed record PurchaseTarget(IShop Shop, IMerchandise Merchandise, decimal Price);

	private sealed class EmploymentFundsPayment : IPaymentMethod
	{
		private readonly FinanceHost _finance;
		private readonly ICharacter _actor;
		private readonly IFrameworkItem _counterparty;
		private readonly string _reference;
		private readonly MudDateTime? _mudDateTime;

		public EmploymentFundsPayment(FinanceHost finance, ICharacter actor, IFrameworkItem counterparty,
			string reference, MudDateTime? mudDateTime)
		{
			_finance = finance;
			_actor = actor;
			_counterparty = counterparty;
			_reference = reference;
			_mudDateTime = mudDateTime;
		}

		public ICurrency Currency => _finance.Currency;

		public decimal AccessibleMoneyForPayment()
		{
			return VirtualCashLedger.AvailableFunds(_finance.Owner, _finance.Currency, _finance.BankAccount);
		}

		public void TakePayment(decimal price)
		{
			VirtualCashLedger.Debit(_finance.Owner, _finance.Currency, price, _actor, _counterparty,
				"ShopPurchase", _reference, _finance.BankAccount, _mudDateTime, out _, _finance.Owner, _reference);
		}

		public decimal AccessibleMoneyForCredit()
		{
			return decimal.MaxValue;
		}

		public void GivePayment(decimal price)
		{
			VirtualCashLedger.Credit(_finance.Owner, _finance.Currency, price, _actor, _counterparty,
				"ShopPurchaseRefund", _reference, _mudDateTime, _finance.Owner, _reference);
		}
	}

	public static bool CanReserveFunds(EmploymentTaskContext context, MoneyAmount amount, out string reason)
	{
		if (!TryResolveFinanceHost(context.Employer, amount, out var finance, out reason))
		{
			return false;
		}

		var available = AvailableFunds(context, finance, amount.Currency);
		if (available >= amount.Amount)
		{
			reason = string.Empty;
			return true;
		}

		reason =
			$"{context.Employer.EmploymentHostName} only has {amount.Currency.Describe(available, CurrencyDescriptionPatternType.ShortDecimal)} available after active employment reservations, but {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} is required.";
		return false;
	}

	public static bool TryReserveFunds(EmploymentTaskContext context, MoneyAmount amount, ICharacter actor,
		string description, out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (context.CurrentTask is null)
		{
			reason = "Employment finance reservations can only be created while executing an active task.";
			return false;
		}

		if (!CanReserveFunds(context, amount, out reason))
		{
			return false;
		}

		var reservation = new FinanceReservation("reserve", Guid.NewGuid(), context.CurrentTask.Id,
			amount.Currency.Id, amount.Amount, description);
		operationalState = new EmploymentActionStepOperationalState(
			ReservationReference: SerializeReservation(reservation));
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Reserved {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} for {description}.",
			context.CurrentTask.CorrelationId);
		reason = string.Empty;
		return true;
	}

	public static bool HasReservedFunds(EmploymentTaskContext context, MoneyAmount amount, out string reason)
	{
		if (context.CurrentTask is null)
		{
			reason = "Employment financial steps require an active task reservation context.";
			return false;
		}

		if (!TryResolveFinanceHost(context.Employer, amount, out _, out reason))
		{
			return false;
		}

		var reserved = ActiveReservations(context)
		               .Where(x => x.TaskId == context.CurrentTask.Id && x.CurrencyId == amount.Currency.Id)
		               .Sum(x => x.Amount);
		if (reserved >= amount.Amount)
		{
			reason = string.Empty;
			return true;
		}

		reason =
			$"This task has only reserved {amount.Currency.Describe(reserved, CurrencyDescriptionPatternType.ShortDecimal)} for employment finance, but {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} is required.";
		return false;
	}

	public static bool TryReleaseReservedFunds(EmploymentTaskContext context, ICharacter actor, string selector,
		out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (context.CurrentTask is null)
		{
			reason = "Employment finance reservations can only be released while executing an active task.";
			return false;
		}

		selector = string.IsNullOrWhiteSpace(selector) ? "all" : selector.Trim();
		var reservations = ActiveReservations(context)
		                   .Where(x => x.TaskId == context.CurrentTask.Id)
		                   .ToList();
		var releaseAll = selector.EqualTo("all");
		Guid reservationId = Guid.Empty;
		if (!releaseAll)
		{
			var cleaned = selector.StartsWith("reservation ", StringComparison.InvariantCultureIgnoreCase)
				? selector["reservation ".Length..].Trim()
				: selector;
			if (!Guid.TryParse(cleaned, out reservationId))
			{
				reason = $"Release steps use {"release all".ColourCommand()} or {"release reservation <id>".ColourCommand()}.";
				return false;
			}

			if (reservations.All(x => x.Id != reservationId))
			{
				reason = $"This task does not have an active employment finance reservation {reservationId:D}.";
				return false;
			}
		}

		var matched = releaseAll ? reservations : reservations.Where(x => x.Id == reservationId).ToList();
		var totalByCurrency = matched
		                      .GroupBy(x => x.CurrencyId)
		                      .Select(x => (CurrencyId: x.Key, Amount: x.Sum(y => y.Amount)))
		                      .ToList();
		var releasePayload = new FinanceReservation("release",
			releaseAll ? Guid.Empty : reservationId,
			context.CurrentTask.Id,
			totalByCurrency.FirstOrDefault().CurrencyId,
			totalByCurrency.Sum(x => x.Amount),
			releaseAll ? "all" : reservationId.ToString("D"));
		operationalState = new EmploymentActionStepOperationalState(
			ReservationReference: SerializeReservation(releasePayload));
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			releaseAll
				? $"Released {matched.Count.ToString("N0", actor)} active employment finance reservation{(matched.Count == 1 ? string.Empty : "s")} for this task."
				: $"Released employment finance reservation {reservationId:D}.",
			context.CurrentTask.CorrelationId);
		reason = string.Empty;
		return true;
	}

	public static bool TryBankDeposit(EmploymentTaskContext context, ICharacter actor, MoneyAmount amount,
		out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!CanBankDeposit(context, amount, out reason))
		{
			return false;
		}

		if (!TryResolveBankFinanceHost(context, amount, out var finance, out reason))
		{
			return false;
		}

		if (!TryBuildReservationConsumption(context, amount, out var consumptionPayload, out reason))
		{
			return false;
		}

		var reference = TransactionReference(context, "employment bank deposit");
		var account = finance.BankAccount!;
		if (!VirtualCashLedger.Debit(finance.Owner, amount.Currency, amount.Amount, actor, finance.BankAccount,
			    "Bank", reference, null, EmploymentClock.CurrentDateTime(context.Employer), out reason,
			    finance.Owner, reference))
		{
			return false;
		}

		account.DepositFromTransaction(amount.Amount, reference);
		account.Bank.CurrencyReserves[amount.Currency] += amount.Amount;
		account.Bank.Changed = true;
		context.RecordLedger(EmploymentLedgerEntryType.BankDeposit, actor, amount,
			$"Deposited employer virtual cash to linked bank account {account.AccountReference}: {reference}.",
			context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Deposited {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} of employer virtual cash to linked bank account {account.AccountReference}.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: $"{account.AccountReference}: {reference}",
			ReservationReference: consumptionPayload);
		reason = string.Empty;
		return true;
	}

	public static bool TryBankWithdrawal(EmploymentTaskContext context, ICharacter actor, MoneyAmount amount,
		out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!CanBankWithdrawal(context, amount, out reason))
		{
			return false;
		}

		if (!TryResolveBankFinanceHost(context, amount, out var finance, out reason))
		{
			return false;
		}

		if (!TryBuildReservationConsumption(context, amount, out var consumptionPayload, out reason))
		{
			return false;
		}

		var reference = TransactionReference(context, "employment bank withdrawal");
		var account = finance.BankAccount!;
		account.WithdrawFromTransaction(amount.Amount, reference);
		account.Bank.CurrencyReserves[amount.Currency] -= amount.Amount;
		account.Bank.Changed = true;
		VirtualCashLedger.Credit(finance.Owner, amount.Currency, amount.Amount, actor, account,
			"Bank", reference, EmploymentClock.CurrentDateTime(context.Employer), finance.Owner, reference);
		context.RecordLedger(EmploymentLedgerEntryType.BankWithdrawal, actor, amount,
			$"Withdrew employer bank funds from linked bank account {account.AccountReference} into virtual cash: {reference}.",
			context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Withdrew {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} from linked bank account {account.AccountReference} into employer virtual cash.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: $"{account.AccountReference}: {reference}",
			ReservationReference: consumptionPayload);
		reason = string.Empty;
		return true;
	}

	public static bool CanPurchase(EmploymentTaskContext context, IEmploymentActionStep step, out string reason)
	{
		if (step is not PurchaseActionStep purchase || !purchase.IsExecutablePurchase)
		{
			reason = string.Empty;
			return true;
		}

		if (!TryResolvePurchaseTarget(context, null, purchase, out var target, out reason))
		{
			return false;
		}

		var amount = new MoneyAmount(target.Shop.Currency, target.Price);
		if (purchase.MaximumAmount is not null && purchase.MaximumAmount.Amount < target.Price)
		{
			reason =
				$"The purchase costs {target.Shop.Currency.Describe(target.Price, CurrencyDescriptionPatternType.ShortDecimal)}, above the maximum of {purchase.MaximumAmount.Currency.Describe(purchase.MaximumAmount.Amount, CurrencyDescriptionPatternType.ShortDecimal)}.";
			return false;
		}

		if (!HasReservedFunds(context, amount, out reason))
		{
			return false;
		}

		if (!TryResolveFinanceHost(context.Employer, amount, out var finance, out reason))
		{
			return false;
		}

		if (AvailableFunds(context, finance, amount.Currency) < amount.Amount)
		{
			reason = $"{context.Employer.EmploymentHostName} does not have enough available funds for this purchase.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public static bool TryPurchase(EmploymentTaskContext context, ICharacter actor, IEmploymentActionStep step,
		out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (step is not PurchaseActionStep purchase || !purchase.IsExecutablePurchase)
		{
			reason = "This purchase step does not identify a supplier and merchandise record.";
			return false;
		}

		if (!TryResolvePurchaseTarget(context, actor, purchase, out var target, out reason))
		{
			return false;
		}

		var amount = new MoneyAmount(target.Shop.Currency, target.Price);
		if (!TryBuildReservationConsumption(context, amount, out var consumptionPayload, out reason))
		{
			return false;
		}

		if (!TryResolveFinanceHost(context.Employer, amount, out var finance, out reason))
		{
			return false;
		}

		var reference = TransactionReference(context, $"employment purchase at {target.Shop.Name}");
		var payment = new EmploymentFundsPayment(finance, actor, target.Shop, reference,
			EmploymentClock.CurrentDateTime(context.Employer));
		var canBuy = target.Shop.CanBuy(actor, target.Merchandise, purchase.Quantity!.Value, payment,
			purchase.KeywordFilter);
		if (!canBuy.Truth)
		{
			reason = canBuy.Reason;
			return false;
		}

		var items = target.Shop.Buy(actor, target.Merchandise, purchase.Quantity.Value, payment,
			purchase.KeywordFilter).ToList();
		context.RecordLedger(EmploymentLedgerEntryType.Purchase, actor, amount,
			$"Purchased {items.Count:N0} item(s) from {target.Shop.Name}: {reference}.",
			context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Purchased {items.Count:N0} item(s) from {target.Shop.Name} for {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)}.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: $"{target.Shop.Name}: {reference}",
			SelectedResources: EmploymentTaskContext.FormatTaskItemCustody("collect", actor.Id, items),
			ReservationReference: consumptionPayload);
		reason = string.Empty;
		return true;
	}

	public static bool CanStoreAccountPayment(EmploymentTaskContext context, string accountKey, MoneyAmount amount,
		out string reason)
	{
		if (!TryResolveStoreAccount(context, accountKey, out _, out var account, out reason))
		{
			return false;
		}

		if (account.Currency.Id != amount.Currency.Id)
		{
			reason = $"The account {account.AccountName} uses {account.Currency.Name}, but this step is for {amount.Currency.Name}.";
			return false;
		}

		if (!HasReservedFunds(context, amount, out reason))
		{
			return false;
		}

		return TryResolveFinanceHost(context.Employer, amount, out _, out reason);
	}

	public static bool TryStoreAccountPayment(EmploymentTaskContext context, ICharacter actor, string accountKey,
		MoneyAmount amount, out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!CanStoreAccountPayment(context, accountKey, amount, out reason))
		{
			return false;
		}

		if (!TryResolveStoreAccount(context, accountKey, out var shop, out var account, out reason))
		{
			return false;
		}

		if (!TryBuildReservationConsumption(context, amount, out var consumptionPayload, out reason))
		{
			return false;
		}

		if (!TryResolveFinanceHost(context.Employer, amount, out var finance, out reason))
		{
			return false;
		}

		var payment = Math.Min(account.OutstandingBalance, amount.Amount);
		var reference = TransactionReference(context, $"employment store account payment to {shop.Name}");
		if (!VirtualCashLedger.Debit(finance.Owner, amount.Currency, payment, actor, account, "StoreAccount",
			    reference, finance.BankAccount, EmploymentClock.CurrentDateTime(context.Employer), out reason,
			    finance.Owner, reference))
		{
			return false;
		}

		account.PayoffAccount(payment);
		var paidAmount = new MoneyAmount(amount.Currency, payment);
		context.RecordLedger(EmploymentLedgerEntryType.StoreAccountPayment, actor, paidAmount,
			$"Paid line-of-credit account {account.AccountName} at {shop.Name}: {reference}.",
			context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Paid {paidAmount.Currency.Describe(paidAmount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} to {shop.Name} account {account.AccountName}.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: $"{shop.Name}/{account.AccountName}: {reference}",
			ReservationReference: consumptionPayload);
		reason = string.Empty;
		return true;
	}

	public static bool CanPayTaxes(EmploymentTaskContext context, MoneyAmount? maximumAmount, out string reason,
		out MoneyAmount? amount)
	{
		amount = null;
		if (!TryResolveTaxOwing(context, maximumAmount, out amount, out reason))
		{
			return false;
		}

		if (amount.Amount <= 0.0M)
		{
			reason = $"{context.Employer.EmploymentHostName} has no supported outstanding taxes to pay.";
			return false;
		}

		if (!HasReservedFunds(context, amount, out reason))
		{
			return false;
		}

		return TryResolveFinanceHost(context.Employer, amount, out _, out reason);
	}

	public static bool TryPayTaxes(EmploymentTaskContext context, ICharacter actor, MoneyAmount? maximumAmount,
		out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!CanPayTaxes(context, maximumAmount, out reason, out var amount) || amount is null)
		{
			return false;
		}

		if (!TryBuildReservationConsumption(context, amount, out var consumptionPayload, out reason))
		{
			return false;
		}

		if (!TryResolveFinanceHost(context.Employer, amount, out var finance, out reason))
		{
			return false;
		}

		var reference = TransactionReference(context, "employment tax payment");
		if (!VirtualCashLedger.Debit(finance.Owner, amount.Currency, amount.Amount, actor, finance.Owner,
			    "TaxPayment", reference, finance.BankAccount, EmploymentClock.CurrentDateTime(context.Employer),
			    out reason, finance.Owner, reference))
		{
			return false;
		}

		switch (context.Employer)
		{
			case IShop shop:
				shop.EconomicZone.PayTaxesForShop(shop, amount.Amount);
				break;
			default:
				reason = $"{context.Employer.EmploymentHostName} does not expose a native supported tax payment adapter.";
				return false;
		}

		context.RecordLedger(EmploymentLedgerEntryType.TaxPayment, actor, amount,
			$"Paid supported host taxes: {reference}.", context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Paid {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} in supported host taxes.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: reference,
			ReservationReference: consumptionPayload);
		reason = string.Empty;
		return true;
	}

	public static bool CanAdjustShopFloat(EmploymentTaskContext context, MoneyAmount amount, bool fillRegister,
		EmploymentItemSelector? registerSelector, out string reason)
	{
		if (context.Employer is not IPermanentShop shop)
		{
			reason = $"{context.Employer.EmploymentHostName} is not a permanent shop with cash registers.";
			return false;
		}

		if (shop.Currency.Id != amount.Currency.Id)
		{
			reason = $"{shop.Name} uses {shop.Currency.Name}, but this step is for {amount.Currency.Name}.";
			return false;
		}

		if (!HasReservedFunds(context, amount, out reason))
		{
			return false;
		}

		if (fillRegister && !TryResolveFinanceHost(context.Employer, amount, out _, out reason))
		{
			return false;
		}

		if (!fillRegister && shop.AvailableCashFromAllSources() < amount.Amount)
		{
			reason =
				$"{shop.Name} has only {shop.Currency.Describe(shop.AvailableCashFromAllSources(), CurrencyDescriptionPatternType.ShortDecimal)} in physical cash to skim.";
			return false;
		}

		if (registerSelector is not null && !shop.TillItems.Any(x => ItemThresholdCondition.MatchesSelector(context, x, registerSelector)))
		{
			reason = $"{shop.Name} does not have a till matching {EmploymentItemSelectorResolver.Describe(registerSelector)}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public static bool TryAdjustShopFloat(EmploymentTaskContext context, ICharacter actor, MoneyAmount amount,
		bool fillRegister, EmploymentItemSelector? registerSelector, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!CanAdjustShopFloat(context, amount, fillRegister, registerSelector, out reason))
		{
			return false;
		}

		var shop = (IPermanentShop)context.Employer;
		if (!TryBuildReservationConsumption(context, amount, out var consumptionPayload, out reason))
		{
			return false;
		}

		var reference = TransactionReference(context, fillRegister ? "employment cash float fill" : "employment cash float skim");
		if (fillRegister)
		{
			if (!TryResolveFinanceHost(context.Employer, amount, out var finance, out reason))
			{
				return false;
			}

			if (!VirtualCashLedger.Debit(finance.Owner, amount.Currency, amount.Amount, actor, shop, "CashFloat",
				    reference, finance.BankAccount, EmploymentClock.CurrentDateTime(context.Employer), out reason,
				    finance.Owner, reference))
			{
				return false;
			}

			shop.AddCurrencyToShop(CurrencyGameItemComponentProto.CreateNewCurrencyPile(amount.Currency,
				amount.Currency.FindCoinsForAmount(amount.Amount, out _)));
		}
		else
		{
			shop.TakeCashFromAllSources(amount.Amount, reference);
			VirtualCashLedger.Credit(shop, amount.Currency, amount.Amount, actor, shop, "CashFloat",
				reference, EmploymentClock.CurrentDateTime(context.Employer), shop, reference);
		}

		context.RecordLedger(fillRegister ? EmploymentLedgerEntryType.BankWithdrawal : EmploymentLedgerEntryType.BankDeposit,
			actor, amount, $"{(fillRegister ? "Filled" : "Skimmed")} shop register float: {reference}.",
			context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"{(fillRegister ? "Filled" : "Skimmed")} {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} of shop register float.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: reference,
			ReservationReference: consumptionPayload);
		reason = string.Empty;
		return true;
	}

	public static bool TryGetTaxOwing(EmploymentTaskContext context, out MoneyAmount amount, out string reason)
	{
		return TryResolveTaxOwing(context, null, out amount, out reason);
	}

	public static bool CanBankDeposit(EmploymentTaskContext context, MoneyAmount amount, out string reason)
	{
		if (!TryResolveBankFinanceHost(context, amount, out var finance, out reason))
		{
			return false;
		}

		if (!HasReservedFunds(context, amount, out reason))
		{
			return false;
		}

		var virtualBalance = VirtualCashLedger.Balance(finance.Owner, amount.Currency);
		if (virtualBalance >= amount.Amount)
		{
			reason = string.Empty;
			return true;
		}

		reason =
			$"{context.Employer.EmploymentHostName} only has {amount.Currency.Describe(virtualBalance, CurrencyDescriptionPatternType.ShortDecimal)} in virtual cash. Bank deposits cannot draw from the linked bank account.";
		return false;
	}

	public static bool CanBankWithdrawal(EmploymentTaskContext context, MoneyAmount amount, out string reason)
	{
		if (!TryResolveBankFinanceHost(context, amount, out var finance, out reason))
		{
			return false;
		}

		if (!HasReservedFunds(context, amount, out reason))
		{
			return false;
		}

		var canWithdraw = finance.BankAccount!.CanWithdraw(amount.Amount, false);
		if (canWithdraw.Truth)
		{
			reason = string.Empty;
			return true;
		}

		reason = canWithdraw.Error;
		return false;
	}

	public static bool TryGetConditionBalance(EmploymentTaskContext context, string accountKey, out decimal balance,
		out string reason)
	{
		balance = 0.0M;
		if (context.Employer is not IShop shop)
		{
			reason =
				$"{context.Employer.EmploymentHostName} does not expose a native employment finance adapter yet. This slice supports shop cash, bank, and available balance conditions.";
			return false;
		}

		switch (accountKey.CollapseString().ToLowerInvariant())
		{
			case "cash":
			case "virtualcash":
			case "hostcash":
				balance = VirtualCashLedger.Balance(shop, shop.Currency);
				reason = string.Empty;
				return true;
			case "bank":
			case "bankaccount":
				balance = shop.BankAccount?.Currency.Id == shop.Currency.Id
					? shop.BankAccount.CurrentBalance
					: 0.0M;
				reason = string.Empty;
				return true;
			case "available":
			case "availablefunds":
			case "total":
				balance = AvailableFunds(context, new FinanceHost(shop, shop.Currency, shop.BankAccount), shop.Currency);
				reason = string.Empty;
				return true;
			default:
				reason = $"There is no supported employment finance balance named {accountKey}.";
				return false;
		}
	}

	private static bool TryResolvePurchaseTarget(EmploymentTaskContext context, ICharacter? actor,
		PurchaseActionStep purchase, out PurchaseTarget target, out string reason)
	{
		target = null!;
		var gameworld = (context.Employer as IHaveFuturemud)?.Gameworld ?? actor?.Gameworld;
		if (gameworld is null)
		{
			reason = "There is no gameworld available to resolve supplier shops.";
			return false;
		}

		var supplierSelector = purchase.SupplierSelector ?? "any";
		var suppliers = supplierSelector.EqualTo("any")
			? gameworld.Shops.ToList()
			: gameworld.Shops.GetByIdOrName(supplierSelector) is { } supplier ? [supplier] : [];
		foreach (var shop in suppliers)
		{
			var merchandise = long.TryParse(purchase.MerchandiseSelector, out var id)
				? shop.Merchandises.FirstOrDefault(x => x.Id == id)
				: shop.Merchandises.FirstOrDefault(x => x.Name.EqualTo(purchase.MerchandiseSelector!)) ??
				  shop.Merchandises.FirstOrDefault(x =>
					  x.Name.StartsWith(purchase.MerchandiseSelector!, StringComparison.InvariantCultureIgnoreCase));
			if (merchandise is null)
			{
				continue;
			}

			var quantity = purchase.Quantity ?? 1;
			if (shop.StockedItems(merchandise).Sum(x => x.Quantity) < quantity)
			{
				continue;
			}

			target = new PurchaseTarget(shop, merchandise, shop.PriceForMerchandise(actor, merchandise, quantity));
			reason = string.Empty;
			return true;
		}

		reason = supplierSelector.EqualTo("any")
			? $"No shop has stocked merchandise matching {purchase.MerchandiseSelector}."
			: $"No supplier shop matching {supplierSelector} has merchandise {purchase.MerchandiseSelector}.";
		return false;
	}

	private static bool TryResolveStoreAccount(EmploymentTaskContext context, string accountKey, out IShop shop,
		out ILineOfCreditAccount account, out string reason)
	{
		shop = null!;
		account = null!;
		var gameworld = (context.Employer as IHaveFuturemud)?.Gameworld;
		if (gameworld is null)
		{
			reason = "There is no gameworld available to resolve shop accounts.";
			return false;
		}

		if (ShopAccountOwingCondition.TryParseKey(accountKey, out var shopId, out var accountId))
		{
			var resolvedShop = gameworld.Shops.Get(shopId);
			if (resolvedShop is not null)
			{
				shop = resolvedShop;
				account = resolvedShop.LineOfCreditAccounts.FirstOrDefault(x => x.Id == accountId)!;
			}
		}
		else
		{
			var match = gameworld.Shops
			                     .SelectMany(x => x.LineOfCreditAccounts.Select(y => (Shop: x, Account: y)))
			                     .FirstOrDefault(x =>
				                     x.Account.Id.ToString("F0").EqualTo(accountKey) ||
				                     x.Account.AccountName.EqualTo(accountKey) ||
				                     x.Account.AccountName.StartsWith(accountKey,
					                     StringComparison.InvariantCultureIgnoreCase));
			shop = match.Shop;
			account = match.Account;
		}

		if (shop is null || account is null)
		{
			reason = $"There is no shop line-of-credit account matching {accountKey}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static bool TryResolveTaxOwing(EmploymentTaskContext context, MoneyAmount? maximumAmount,
		out MoneyAmount amount, out string reason)
	{
		amount = null!;
		switch (context.Employer)
		{
			case IShop shop:
				var owing = shop.EconomicZone.OutstandingTaxesForShop(shop);
				var payable = maximumAmount is null ? owing : Math.Min(owing, maximumAmount.Amount);
				amount = new MoneyAmount(shop.Currency, payable);
				reason = string.Empty;
				return true;
			default:
				reason = $"{context.Employer.EmploymentHostName} does not expose a native supported tax owing adapter.";
				return false;
		}
	}

	private static bool TryResolveBankFinanceHost(EmploymentTaskContext context, MoneyAmount amount,
		out FinanceHost finance, out string reason)
	{
		if (!TryResolveFinanceHost(context.Employer, amount, out finance, out reason))
		{
			return false;
		}

		if (finance.BankAccount is null)
		{
			reason = $"{context.Employer.EmploymentHostName} does not have a linked native bank account for employment bank task steps.";
			return false;
		}

		if (finance.BankAccount.Currency.Id != amount.Currency.Id)
		{
			reason =
				$"The linked bank account uses {finance.BankAccount.Currency.Name}, but this step is for {amount.Currency.Name}.";
			return false;
		}

		return true;
	}

	private static bool TryResolveFinanceHost(IEmploymentHost host, MoneyAmount amount, out FinanceHost finance,
		out string reason)
	{
		finance = null!;
		if (host is not IShop shop)
		{
			reason =
				$"{host.EmploymentHostName} does not expose a native employment finance adapter yet. This slice supports permanent shop finance through the shop's virtual cash and linked bank account.";
			return false;
		}

		if (shop.Currency.Id != amount.Currency.Id)
		{
			reason = $"{host.EmploymentHostName} uses {shop.Currency.Name}, but this step is for {amount.Currency.Name}.";
			return false;
		}

		finance = new FinanceHost(shop, shop.Currency, shop.BankAccount);
		reason = string.Empty;
		return true;
	}

	private static decimal AvailableFunds(EmploymentTaskContext context, FinanceHost finance, ICurrency currency)
	{
		var bankCapacity = finance.BankAccount?.Currency.Id == currency.Id
			? Math.Max(0.0M, finance.BankAccount.MaximumWithdrawal())
			: 0.0M;
		var reserved = ActiveReservations(context)
		               .Where(x => x.CurrencyId == currency.Id)
		               .Sum(x => x.Amount);
		return Math.Max(0.0M, VirtualCashLedger.Balance(finance.Owner, currency) + bankCapacity - reserved);
	}

	private static bool TryBuildReservationConsumption(EmploymentTaskContext context, MoneyAmount amount,
		out string payload, out string reason)
	{
		payload = string.Empty;
		if (context.CurrentTask is null)
		{
			reason = "Employment financial steps require an active task reservation context.";
			return false;
		}

		var reservations = ActiveReservations(context)
		                   .Where(x => x.TaskId == context.CurrentTask.Id && x.CurrencyId == amount.Currency.Id)
		                   .ToList();
		var selected = new List<FinanceReservation>();
		var remaining = amount.Amount;
		foreach (var reservation in reservations)
		{
			if (remaining <= 0.0M)
			{
				break;
			}

			var consumed = Math.Min(reservation.Amount, remaining);
			selected.Add(reservation with { Operation = "consume", Amount = consumed });
			remaining -= consumed;
		}

		if (remaining > 0.0M)
		{
			reason =
				$"This task has only reserved {amount.Currency.Describe(amount.Amount - remaining, CurrencyDescriptionPatternType.ShortDecimal)} for employment finance, but {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} is required.";
			return false;
		}

		payload = string.Join(PayloadSeparator, selected.Select(SerializeReservation));
		reason = string.Empty;
		return true;
	}

	private static IReadOnlyCollection<FinanceReservation> ActiveReservations(EmploymentTaskContext context)
	{
		var active = new List<FinanceReservation>();
		foreach (var task in context.Employer.TaskBoard.ActiveTasks.Where(x =>
			         x.Status is EmploymentTaskStatus.Pending or EmploymentTaskStatus.Assigned or
				         EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked))
		{
			for (var i = 0; i < task.StepOperationalStates.Count; i++)
			{
				if (i >= task.StepStates.Count || task.StepStates[i] != EmploymentActionStepStatus.Completed)
				{
					continue;
				}

				foreach (var reservation in ParseReservations(task.StepOperationalStates[i].ReservationReference))
				{
					ApplyReservationOperation(active, reservation);
				}
			}
		}

		return active;
	}

	private static void ApplyReservationOperation(List<FinanceReservation> active, FinanceReservation reservation)
	{
		switch (reservation.Operation)
		{
			case "reserve":
				active.Add(reservation);
				return;
			case "consume":
				ConsumeReservation(active, reservation);
				return;
			case "release":
				if (reservation.Id == Guid.Empty || reservation.Description.EqualTo("all"))
				{
					active.RemoveAll(x => x.TaskId == reservation.TaskId);
					return;
				}

				active.RemoveAll(x => x.TaskId == reservation.TaskId && x.Id == reservation.Id);
				return;
		}
	}

	private static void ConsumeReservation(List<FinanceReservation> active, FinanceReservation consumption)
	{
		var index = active.FindIndex(x => x.TaskId == consumption.TaskId && x.Id == consumption.Id);
		if (index < 0)
		{
			return;
		}

		var existing = active[index];
		var remaining = existing.Amount - consumption.Amount;
		if (remaining <= 0.0M)
		{
			active.RemoveAt(index);
			return;
		}

		active[index] = existing with { Amount = remaining };
	}

	private static string SerializeReservation(FinanceReservation reservation)
	{
		return string.Join(";",
			PayloadPrefix,
			$"op={reservation.Operation}",
			$"id={reservation.Id:D}",
			$"task={reservation.TaskId:D}",
			$"currency={reservation.CurrencyId.ToString("F0", CultureInfo.InvariantCulture)}",
			$"amount={reservation.Amount.ToString(CultureInfo.InvariantCulture)}",
			$"desc={Uri.EscapeDataString(reservation.Description)}");
	}

	private static IEnumerable<FinanceReservation> ParseReservations(string? payload)
	{
		if (string.IsNullOrWhiteSpace(payload))
		{
			yield break;
		}

		foreach (var segment in payload.Split([PayloadSeparator], StringSplitOptions.RemoveEmptyEntries))
		{
			var parts = segment.Split(';', StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 0 || !parts[0].EqualTo(PayloadPrefix))
			{
				continue;
			}

			var values = parts
			             .Skip(1)
			             .Select(x => x.Split('=', 2))
			             .Where(x => x.Length == 2)
			             .ToDictionary(x => x[0], x => x[1], StringComparer.InvariantCultureIgnoreCase);
			if (!values.TryGetValue("op", out var operation) ||
			    !values.TryGetValue("id", out var idText) ||
			    !Guid.TryParse(idText, out var id) ||
			    !values.TryGetValue("task", out var taskText) ||
			    !Guid.TryParse(taskText, out var taskId) ||
			    !values.TryGetValue("currency", out var currencyText) ||
			    !long.TryParse(currencyText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var currencyId) ||
			    !values.TryGetValue("amount", out var amountText) ||
			    !decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
			{
				continue;
			}

			var description = values.TryGetValue("desc", out var desc)
				? Uri.UnescapeDataString(desc)
				: string.Empty;
			yield return new FinanceReservation(operation, id, taskId, currencyId, amount, description);
		}
	}

	private static string TransactionReference(EmploymentTaskContext context, string action)
	{
		return context.CurrentTask is null
			? action
			: $"{action} for employment task {context.CurrentTask.CorrelationId:D}";
	}
}
