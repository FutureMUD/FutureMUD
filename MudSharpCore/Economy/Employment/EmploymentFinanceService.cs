using System.Globalization;
using MudSharp.Arenas;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
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

	private sealed record PurchaseTarget(IShop Shop, IMerchandise Merchandise, decimal Price, int Quantity,
		string? KeywordFilter, IReadOnlyList<IGameItem> ExactStockItems, double? CommodityWeight = null);

	internal sealed record EmploymentPurchaseTargetPreview(IShop Shop, IMerchandise Merchandise, decimal Price, int Quantity,
		double? CommodityWeight, IReadOnlyCollection<ICell> Locations);

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
			return AvailableFundsWithoutReservations(_finance);
		}

		public void TakePayment(decimal price)
		{
			DebitAvailable(_finance, price, _actor, _counterparty, "ShopPurchase", _reference, _mudDateTime, out _);
		}

		public decimal AccessibleMoneyForCredit()
		{
			return decimal.MaxValue;
		}

		public void GivePayment(decimal price)
		{
			CreditVirtual(_finance, price, _actor, _counterparty, "ShopPurchaseRefund", _reference, _mudDateTime);
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
		if (!DebitVirtual(finance, amount.Amount, actor, finance.BankAccount, "Bank", reference,
			    EmploymentClock.CurrentDateTime(context.Employer), out reason))
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
		CreditVirtual(finance, amount.Amount, actor, account, "Bank", reference,
			EmploymentClock.CurrentDateTime(context.Employer));
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

	public static bool TryBankTransfer(EmploymentTaskContext context, ICharacter actor, string targetAccountKey,
		MoneyAmount amount, out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!CanBankTransfer(context, targetAccountKey, amount, out reason))
		{
			return false;
		}

		if (!TryResolveBankFinanceHost(context, amount, out var finance, out reason))
		{
			return false;
		}

		if (!TryResolveTargetBankAccount(context, targetAccountKey, amount, out var targetAccount, out reason))
		{
			return false;
		}

		if (!TryBuildReservationConsumption(context, amount, out var consumptionPayload, out reason))
		{
			return false;
		}

		var sourceAccount = finance.BankAccount!;
		var reference = TransactionReference(context, $"employment account transfer to {targetAccount.AccountReference}");
		sourceAccount.WithdrawFromTransfer(amount.Amount, targetAccount.Bank.Code, targetAccount.AccountNumber, reference);
		targetAccount.DepositFromTransfer(amount.Amount, sourceAccount.Bank.Code, sourceAccount.AccountNumber, reference);
		if (sourceAccount.Bank.Id != targetAccount.Bank.Id)
		{
			sourceAccount.Bank.CurrencyReserves[amount.Currency] -= amount.Amount;
			sourceAccount.Bank.Changed = true;
			targetAccount.Bank.CurrencyReserves[amount.Currency] += amount.Amount;
			targetAccount.Bank.Changed = true;
		}

		context.RecordLedger(EmploymentLedgerEntryType.AccountTransfer, actor, amount,
			$"Transferred employer bank funds from linked bank account {sourceAccount.AccountReference} to {targetAccount.AccountReference}: {reference}.",
			context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Transferred {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} from linked bank account {sourceAccount.AccountReference} to {targetAccount.AccountReference}.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: $"{sourceAccount.AccountReference}->{targetAccount.AccountReference}: {reference}",
			ReservationReference: consumptionPayload);
		reason = string.Empty;
		return true;
	}
	public static bool TryHostSettlement(EmploymentTaskContext context, ICharacter actor, string targetHostKey,
		MoneyAmount amount, out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!CanHostSettlement(context, targetHostKey, amount, out reason))
		{
			return false;
		}

		if (!TryResolveFinanceHost(context.Employer, amount, out var sourceFinance, out reason))
		{
			return false;
		}

		if (!TryResolveTargetEmploymentHost(context, targetHostKey, out var targetHost, out reason) ||
		    !TryResolveFinanceHost(targetHost, amount, out var targetFinance, out reason))
		{
			return false;
		}

		if (!TryBuildReservationConsumption(context, amount, out var consumptionPayload, out reason))
		{
			return false;
		}

		var reference = TransactionReference(context, $"employment host settlement to {targetHost.EmploymentHostName}");
		var mudDateTime = EmploymentClock.CurrentDateTime(context.Employer);
		if (!DebitAvailable(sourceFinance, amount.Amount, actor, targetFinance.Owner, "HostSettlement", reference,
			    mudDateTime, out reason))
		{
			return false;
		}

		CreditVirtual(targetFinance, amount.Amount, actor, sourceFinance.Owner, "HostSettlement", reference, mudDateTime);
		context.RecordLedger(EmploymentLedgerEntryType.HostSettlement, actor, amount,
			$"Settled employer funds from {context.Employer.EmploymentHostName} to {targetHost.EmploymentHostName}: {reference}.",
			context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Settled {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} from {context.Employer.EmploymentHostName} to {targetHost.EmploymentHostName}.",
			context.CurrentTask?.CorrelationId);
		targetHost.BusinessLedger.Record(EmploymentLedgerEntryType.HostSettlement, actor, amount,
			$"Received employment host settlement from {context.Employer.EmploymentHostName}: {reference}.",
			context.CurrentTask?.CorrelationId);
		targetHost.EmploymentRegister.Record(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Received {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} settlement from {context.Employer.EmploymentHostName}: {reference}.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: $"{context.Employer.EmploymentHostName}->{targetHost.EmploymentHostName}: {reference}",
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
		var canBuy = target.CommodityWeight.HasValue
			? target.Shop.CanBuyCommodityWeight(actor, target.Merchandise, target.CommodityWeight.Value, payment,
				target.ExactStockItems)
			: target.ExactStockItems.Any()
			? target.Shop.CanBuyExact(actor, target.Merchandise, target.Quantity, payment, target.ExactStockItems)
			: target.Shop.CanBuy(actor, target.Merchandise, target.Quantity, payment, target.KeywordFilter);
		if (!canBuy.Truth)
		{
			reason = canBuy.Reason;
			return false;
		}

		var items = target.CommodityWeight.HasValue
			? target.Shop.BuyCommodityWeight(actor, target.Merchandise, target.CommodityWeight.Value, payment,
				target.ExactStockItems).ToList()
			: target.ExactStockItems.Any()
			? target.Shop.BuyExact(actor, target.Merchandise, target.Quantity, payment, target.ExactStockItems).ToList()
			: target.Shop.Buy(actor, target.Merchandise, target.Quantity, payment, target.KeywordFilter).ToList();
		ItemOwnershipService.AssignOwner(items, context.Employer);
		context.RecordLedger(EmploymentLedgerEntryType.Purchase, actor, amount,
			$"Purchased {items.Count:N0} item(s) from {target.Shop.Name}: {reference}.",
			context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Purchased {items.Count:N0} item(s) from {target.Shop.Name} for {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)}.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: $"{target.Shop.Name}: {reference}",
			SelectedResources: context.FormatTaskItemCustodyForActor("collect", actor, items),
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
		if (!DebitAvailable(finance, payment, actor, account, "StoreAccount", reference,
			    EmploymentClock.CurrentDateTime(context.Employer), out reason))
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
		if (!DebitAvailable(finance, amount.Amount, actor, finance.Owner, "TaxPayment", reference,
			    EmploymentClock.CurrentDateTime(context.Employer), out reason))
		{
			return false;
		}

		switch (context.Employer)
		{
			case IShop shop:
				shop.EconomicZone.PayTaxesForShop(shop, amount.Amount);
				break;
			case IHotel hotel:
				hotel.Property.HotelOutstandingTaxes = Math.Max(0.0M,
					hotel.Property.HotelOutstandingTaxes - amount.Amount);
				hotel.Property.Changed = true;
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

			if (!DebitAvailable(finance, amount.Amount, actor, shop, "CashFloat", reference,
				    EmploymentClock.CurrentDateTime(context.Employer), out reason))
			{
				return false;
			}

			shop.AddCurrencyToShop(CurrencyGameItemComponentProto.CreateNewCurrencyPile(amount.Currency,
				amount.Currency.FindCoinsForAmount(amount.Amount, out _)));
		}
		else
		{
			if (!TryResolveFinanceHost(context.Employer, amount, out var finance, out reason))
			{
				return false;
			}

			shop.TakeCashFromAllSources(amount.Amount, reference);
			CreditVirtual(finance, amount.Amount, actor, shop, "CashFloat", reference,
				EmploymentClock.CurrentDateTime(context.Employer));
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

	public static bool CanHandlePhysicalFloat(EmploymentTaskContext context, PhysicalFloatOperation operation,
		MoneyAmount? amount, string targetKind, EmploymentItemSelector? targetSelector, out string reason)
	{
		targetKind = NormalisePhysicalFloatTarget(targetKind);
		if (operation == PhysicalFloatOperation.Issue && amount is null)
		{
			reason = "Issuing physical employment float requires an amount.";
			return false;
		}

		if (operation == PhysicalFloatOperation.Issue && amount is not null &&
		    !HasReservedFunds(context, amount, out reason))
		{
			return false;
		}

		if (!TryResolveFinanceHost(context.Employer, amount, out var finance, out reason))
		{
			return false;
		}

		switch (operation)
		{
			case PhysicalFloatOperation.Issue:
				if (targetKind.EqualTo("register"))
				{
					if (context.Employer is not IPermanentShop shop)
					{
						reason = $"{context.Employer.EmploymentHostName} is not a permanent shop with cash registers.";
						return false;
					}

					if (shop.AvailableCashFromAllSources() < amount!.Amount)
					{
						reason =
							$"{shop.Name} only has {shop.Currency.Describe(shop.AvailableCashFromAllSources(), CurrencyDescriptionPatternType.ShortDecimal)} in shop cash registers.";
						return false;
					}
				}
				else if (amount is not null && AvailableFunds(context, finance, amount.Currency) < amount.Amount)
				{
					reason = $"{context.Employer.EmploymentHostName} does not have enough available employer funds to issue the physical float.";
					return false;
				}

				reason = string.Empty;
				return true;
			case PhysicalFloatOperation.Return:
				if (targetKind.EqualTo("bank"))
				{
					return TryResolveBankFinanceHost(context, amount ?? new MoneyAmount(finance.Currency, 0.01M),
						out _, out reason);
				}

				if (targetKind.EqualTo("register") && context.Employer is not IPermanentShop)
				{
					reason = $"{context.Employer.EmploymentHostName} is not a permanent shop with cash registers.";
					return false;
				}

				if (targetKind.EqualTo("container") && targetSelector is null)
				{
					reason = "Returning physical float to a container requires a container selector.";
					return false;
				}

				if (targetKind.EqualTo("container") && amount is not null)
				{
					reason = "Returning physical float to a container currently moves whole carried currency piles; use all rather than a partial amount.";
					return false;
				}

				reason = string.Empty;
				return true;
			case PhysicalFloatOperation.Settle:
				reason = string.Empty;
				return true;
			default:
				reason = $"Unsupported physical float operation {operation.DescribeEnum()}.";
				return false;
		}
	}

	public static bool TryHandlePhysicalFloat(EmploymentTaskContext context, ICharacter actor,
		PhysicalFloatOperation operation, MoneyAmount? amount, string targetKind, EmploymentItemSelector? targetSelector,
		out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		targetKind = NormalisePhysicalFloatTarget(targetKind);
		if (!CanHandlePhysicalFloat(context, operation, amount, targetKind, targetSelector, out reason))
		{
			return false;
		}

		return operation switch
		{
			PhysicalFloatOperation.Issue when amount is not null =>
				TryIssuePhysicalFloat(context, actor, amount, targetKind, out reason, out operationalState),
			PhysicalFloatOperation.Return =>
				TryReturnPhysicalFloat(context, actor, amount, targetKind, targetSelector, out reason,
					out operationalState),
			PhysicalFloatOperation.Settle =>
				TrySettlePhysicalFloat(context, actor, amount, out reason, out operationalState),
			_ => UnsupportedPhysicalFloat(operation, out reason, out operationalState)
		};
	}

	private static bool TryIssuePhysicalFloat(EmploymentTaskContext context, ICharacter actor, MoneyAmount amount,
		string sourceKind, out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!TryBuildReservationConsumption(context, amount, out var consumptionPayload, out reason))
		{
			return false;
		}

		var reference = TransactionReference(context, $"employment physical float issue from {sourceKind}");
		if (sourceKind.EqualTo("register"))
		{
			if (context.Employer is not IPermanentShop shop)
			{
				reason = $"{context.Employer.EmploymentHostName} is not a permanent shop with cash registers.";
				return false;
			}

			shop.TakeCashFromAllSources(amount.Amount, reference);
		}
		else
		{
			if (!TryResolveFinanceHost(context.Employer, amount, out var finance, out reason))
			{
				return false;
			}

			if (!DebitAvailable(finance, amount.Amount, actor, finance.Owner, "PhysicalFloat", reference,
				    EmploymentClock.CurrentDateTime(context.Employer), out reason))
			{
				return false;
			}
		}

		var pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(amount.Currency,
			amount.Currency.FindCoinsForAmount(amount.Amount, out _));
		pile.RoomLayer = actor.RoomLayer;
		pile.SetOwner(context.Employer);
		actor.Location.Insert(pile, true);
		if (!context.TryCollectTaskItem(actor, pile, actor.Location, out reason))
		{
			pile.Delete();
			return false;
		}

		context.RecordLedger(EmploymentLedgerEntryType.BankWithdrawal, actor, amount,
			$"Issued physical employment float from {sourceKind}: {reference}.",
			context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Issued {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} as physical employment float from {sourceKind}.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: reference,
			SelectedResources: context.FormatTaskItemCustodyForActor("collect", actor, [pile]),
			ReservationReference: consumptionPayload);
		reason = string.Empty;
		return true;
	}

	private static bool TryReturnPhysicalFloat(EmploymentTaskContext context, ICharacter actor, MoneyAmount? amount,
		string targetKind, EmploymentItemSelector? targetSelector, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!TryFindCarriedCurrency(context, actor, amount, out var currency, out var selectedCoins, out var total,
			    out reason))
		{
			return false;
		}

		var reference = TransactionReference(context, $"employment physical float return to {targetKind}");
		if (targetKind.EqualTo("container"))
		{
			if (targetSelector is null)
			{
				reason = "Returning physical float to a container requires a container selector.";
				return false;
			}

			var container = EmploymentItemSelectorResolver.Resolve(context, actor, targetSelector, actor.Location, true);
			if (container is null)
			{
				reason = $"There is no container matching {EmploymentItemSelectorResolver.Describe(targetSelector)} here.";
				return false;
			}

			var piles = selectedCoins.Keys.Select(x => x.Parent).DistinctBy(x => x.Id).ToList();
			if (!EmploymentInventoryPlanLogistics.TryPutItemsIntoContainer(actor, piles, container, out var placedItems,
				    out reason))
			{
				return false;
			}

			operationalState = new EmploymentActionStepOperationalState(
				TransactionReference: reference,
				SelectedResources: EmploymentTaskContext.FormatTaskItemCustody("deliver",
					CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor), placedItems));
			reason = string.Empty;
			return true;
		}

		RemoveHeldCoins(actor, selectedCoins);
		var money = new MoneyAmount(currency, total);
		if (targetKind.EqualTo("bank"))
		{
			if (!TryResolveBankFinanceHost(context, money, out var finance, out reason))
			{
				return false;
			}

			var account = finance.BankAccount!;
			account.DepositFromTransaction(total, reference);
			account.Bank.CurrencyReserves[currency] += total;
			account.Bank.Changed = true;
		}
		else if (targetKind.EqualTo("register"))
		{
			if (context.Employer is not IPermanentShop shop)
			{
				reason = $"{context.Employer.EmploymentHostName} is not a permanent shop with cash registers.";
				return false;
			}

			shop.AddCurrencyToShop(CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
				currency.FindCoinsForAmount(total, out _)));
		}
		else
		{
			if (!TryResolveFinanceHost(context.Employer, money, out var finance, out reason))
			{
				return false;
			}

			CreditVirtual(finance, total, actor, finance.Owner, "PhysicalFloat", reference,
				EmploymentClock.CurrentDateTime(context.Employer));
		}

		context.RecordLedger(EmploymentLedgerEntryType.BankDeposit, actor, money,
			$"Returned physical employment float to {targetKind}: {reference}.", context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Returned {currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal)} of physical employment float to {targetKind}.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: reference,
			SelectedResources: $"Returned physical float {currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal)} to {targetKind}.");
		reason = string.Empty;
		return true;
	}

	private static bool TrySettlePhysicalFloat(EmploymentTaskContext context, ICharacter actor, MoneyAmount? amount,
		out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!TryFindCarriedCurrency(context, actor, amount, out var currency, out var selectedCoins, out var total,
			    out reason))
		{
			return false;
		}

		var money = new MoneyAmount(currency, total);
		if (!TryResolveFinanceHost(context.Employer, money, out var finance, out reason))
		{
			return false;
		}

		var reference = TransactionReference(context, "employment physical float settlement");
		RemoveHeldCoins(actor, selectedCoins);
		CreditVirtual(finance, total, actor, finance.Owner, "PhysicalFloat", reference,
			EmploymentClock.CurrentDateTime(context.Employer));
		context.RecordLedger(EmploymentLedgerEntryType.BankDeposit, actor, money,
			$"Settled physical employment float back to employer virtual cash: {reference}.",
			context.CurrentTask?.CorrelationId);
		context.RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationUsed, actor,
			$"Settled {currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal)} of physical employment float back to employer virtual cash.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			TransactionReference: reference,
			SelectedResources: $"Settled physical float {currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal)}.");
		reason = string.Empty;
		return true;
	}

	private static bool TryFindCarriedCurrency(EmploymentTaskContext context, ICharacter actor, MoneyAmount? amount,
		out ICurrency currency, out Dictionary<ICurrencyPile, Dictionary<ICoin, int>> selectedCoins,
		out decimal total, out string reason)
	{
		var resolvedCurrency = amount?.Currency ?? ResolveHostCurrency(context);
		currency = null!;
		selectedCoins = new Dictionary<ICurrencyPile, Dictionary<ICoin, int>>();
		total = 0.0M;
		if (resolvedCurrency is null)
		{
			reason = $"Could not determine a currency for {context.Employer.EmploymentHostName}.";
			return false;
		}

		if (actor.Body is null)
		{
			reason = "The assigned employee does not have a usable body inventory.";
			return false;
		}

		currency = resolvedCurrency;
		var piles = context.CarriedTaskItems(actor)
		                   .Where(x => x is not null)
		                   .Select(x => x.GetItemType<ICurrencyPile>())
		                   .Where(x => x is not null)
		                   .Cast<ICurrencyPile>()
		                   .Where(x => x.Currency.Id == resolvedCurrency.Id)
		                   .DistinctBy(x => x.Parent.Id)
		                   .ToList();
		if (!piles.Any())
		{
			reason = "The assigned employee is not carrying any task-custody currency piles.";
			return false;
		}

		if (amount is null)
		{
			foreach (var pile in piles)
			{
				selectedCoins[pile] = pile.Coins.ToDictionary(x => x.Item1, x => x.Item2);
			}

			total = selectedCoins.TotalValue();
			reason = string.Empty;
			return total > 0.0M;
		}

		selectedCoins = currency.FindCurrency(piles, amount.Amount);
		total = selectedCoins.TotalValue();
		if (total < amount.Amount)
		{
			reason =
				$"The assigned employee is carrying only {currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal)} of task-custody currency, but {currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} is required.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static void RemoveHeldCoins(ICharacter actor,
		Dictionary<ICurrencyPile, Dictionary<ICoin, int>> selectedCoins)
	{
		foreach (var coinItem in selectedCoins)
		{
			if (!coinItem.Key.RemoveCoins(coinItem.Value.Select(x => Tuple.Create(x.Key, x.Value))))
			{
				actor.Body.Take(coinItem.Key.Parent);
				coinItem.Key.Parent.Delete();
			}
		}
	}

	private static ICurrency? ResolveHostCurrency(EmploymentTaskContext context)
	{
		return TryResolveFinanceHost(context.Employer, null, out var finance, out _) ? finance.Currency : null;
	}

	private static string NormalisePhysicalFloatTarget(string targetKind)
	{
		targetKind = targetKind.CollapseString().ToLowerInvariant();
		return targetKind switch
		{
			"" => "bank",
			"cashregister" or "till" => "register",
			"containerselector" => "container",
			_ => targetKind
		};
	}

	private static bool UnsupportedPhysicalFloat(PhysicalFloatOperation operation, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		reason = $"Unsupported physical float operation {operation.DescribeEnum()}.";
		return false;
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

		var virtualBalance = VirtualBalance(finance);
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

	public static bool CanBankTransfer(EmploymentTaskContext context, string targetAccountKey, MoneyAmount amount,
		out string reason)
	{
		if (amount.Amount <= 0.0M)
		{
			reason = "Bank transfer steps must use a positive amount.";
			return false;
		}

		if (!TryResolveBankFinanceHost(context, amount, out var finance, out reason))
		{
			return false;
		}

		if (!HasReservedFunds(context, amount, out reason))
		{
			return false;
		}

		if (!TryResolveTargetBankAccount(context, targetAccountKey, amount, out var targetAccount, out reason))
		{
			return false;
		}

		var sourceAccount = finance.BankAccount!;
		if (targetAccount.Id == sourceAccount.Id)
		{
			reason = "Employment bank transfers cannot transfer funds from an account to itself.";
			return false;
		}

		var canWithdraw = sourceAccount.CanWithdraw(amount.Amount, false);
		if (canWithdraw.Truth)
		{
			reason = string.Empty;
			return true;
		}

		reason = canWithdraw.Error;
		return false;
	}
	public static bool CanHostSettlement(EmploymentTaskContext context, string targetHostKey, MoneyAmount amount,
		out string reason)
	{
		if (amount.Amount <= 0.0M)
		{
			reason = "Host settlement steps must use a positive amount.";
			return false;
		}

		if (!TryResolveFinanceHost(context.Employer, amount, out var sourceFinance, out reason))
		{
			return false;
		}

		if (!HasReservedFunds(context, amount, out reason))
		{
			return false;
		}

		if (!TryResolveTargetEmploymentHost(context, targetHostKey, out var targetHost, out reason))
		{
			return false;
		}

		if (targetHost.EmploymentHostType == context.Employer.EmploymentHostType && targetHost.Id == context.Employer.Id)
		{
			reason = "Employment host settlements cannot settle funds from a host to itself.";
			return false;
		}

		if (!TryResolveFinanceHost(targetHost, amount, out _, out reason))
		{
			return false;
		}

		if (AvailableFunds(context, sourceFinance, amount.Currency) >= amount.Amount)
		{
			reason = string.Empty;
			return true;
		}

		reason = $"{context.Employer.EmploymentHostName} does not have enough available funds for this settlement.";
		return false;
	}
	public static bool TryGetConditionBalance(EmploymentTaskContext context, string accountKey, out decimal balance,
		out string reason)
	{
		balance = 0.0M;
		if (!TryResolveFinanceHost(context.Employer, null, out var finance, out reason))
		{
			return false;
		}

		switch (accountKey.CollapseString().ToLowerInvariant())
		{
			case "cash":
			case "virtualcash":
			case "hostcash":
				balance = VirtualBalance(finance);
				reason = string.Empty;
				return true;
			case "bank":
			case "bankaccount":
				balance = finance.BankAccount?.Currency.Id == finance.Currency.Id
					? finance.BankAccount.CurrentBalance
					: 0.0M;
				reason = string.Empty;
				return true;
			case "available":
			case "availablefunds":
			case "total":
				balance = AvailableFunds(context, finance, finance.Currency);
				reason = string.Empty;
				return true;
			default:
				reason = $"There is no supported employment finance balance named {accountKey}.";
				return false;
		}
	}

	internal static bool CanSettlePayroll(IEmploymentHost host, MoneyAmount amount, out string reason)
	{
		if (!TryResolveFinanceHost(host, amount, out var finance, out reason))
		{
			return false;
		}

		var available = AvailableFundsWithoutReservations(finance);
		if (available >= amount.Amount)
		{
			reason = string.Empty;
			return true;
		}

		reason =
			$"{host.EmploymentHostName} only has {amount.Currency.Describe(available, CurrencyDescriptionPatternType.ShortDecimal)} available for payroll settlement, but {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)} is required.";
		return false;
	}

	internal static bool TrySettlePayroll(IEmploymentHost host, ICharacter? actor, MoneyAmount amount,
		Guid correlationId, out string reason)
	{
		if (!TryResolveFinanceHost(host, amount, out var finance, out reason))
		{
			return false;
		}

		return DebitAvailable(finance, amount.Amount, actor, finance.Owner, "Payroll",
			$"employment payroll settlement {correlationId:D}", EmploymentClock.CurrentDateTime(host), out reason);
	}

	internal static bool CanDisbursePayroll(IEmploymentHost host, EmploymentPayable payable, out string reason)
	{
		if (!TryResolveFinanceHost(host, payable.Amount, out _, out reason))
		{
			return false;
		}

		switch (payable.PaymentMethod.MethodKind)
		{
			case PaymentMethodKind.Cash:
				reason = string.Empty;
				return true;
			case PaymentMethodKind.EmployeeBankAccount:
			case PaymentMethodKind.SpecifiedBankAccount:
				var account = payable.PaymentMethod.BankAccount;
				if (account is null)
				{
					reason = "The wage payment method does not have a destination bank account.";
					return false;
				}

				if (account.Currency.Id != payable.Amount.Currency.Id)
				{
					reason = $"The destination bank account uses {account.Currency.Name}, but the payable is in {payable.Amount.Currency.Name}.";
					return false;
				}

				reason = string.Empty;
				return true;
			default:
				reason = $"Employment wage disbursement by {payable.PaymentMethod.MethodKind.DescribeEnum()} is not implemented yet; the payable remains outstanding.";
				return false;
		}
	}

	internal static bool TryDisbursePayroll(IEmploymentHost host, ICharacter? actor, EmploymentPayable payable,
		out string reason)
	{
		if (!CanDisbursePayroll(host, payable, out reason))
		{
			return false;
		}

		if (!TryResolveFinanceHost(host, payable.Amount, out var finance, out reason))
		{
			return false;
		}

		var reference = $"employment payroll settlement {payable.CorrelationId:D}";
		switch (payable.PaymentMethod.MethodKind)
		{
			case PaymentMethodKind.Cash:
				return DebitAvailable(finance, payable.Amount.Amount, actor, finance.Owner, "Payroll", reference,
					EmploymentClock.CurrentDateTime(host), out reason);
			case PaymentMethodKind.EmployeeBankAccount:
			case PaymentMethodKind.SpecifiedBankAccount:
				var account = payable.PaymentMethod.BankAccount!;
				if (!DebitAvailable(finance, payable.Amount.Amount, actor, account, "Payroll", reference,
					    EmploymentClock.CurrentDateTime(host), out reason))
				{
					return false;
				}

				account.DepositFromTransaction(payable.Amount.Amount, reference);
				account.Bank.CurrencyReserves[payable.Amount.Currency] += payable.Amount.Amount;
				account.Bank.Changed = true;
				reason = string.Empty;
				return true;
			default:
				reason = $"Employment wage disbursement by {payable.PaymentMethod.MethodKind.DescribeEnum()} is not implemented yet; the payable remains outstanding.";
				return false;
		}
	}

	internal static IReadOnlyCollection<ICell> PurchaseLocationHints(IEmploymentTaskContext context, ICharacter? actor,
		PurchaseActionStep purchase)
	{
		var gameworld = (context.Employer as IHaveFuturemud)?.Gameworld ?? actor?.Gameworld;
		if (gameworld is null)
		{
			return [];
		}

		return SupplierShops(gameworld, purchase.SupplierSelector ?? "any")
		       .SelectMany(x => x.CurrentLocations)
		       .Where(x => x is not null)
		       .DistinctBy(x => x.Id)
		       .ToList();
	}

	internal static bool TryPreviewPurchaseTarget(EmploymentTaskContext context, PurchaseActionStep purchase,
		out EmploymentPurchaseTargetPreview preview, out string reason)
	{
		preview = null!;
		if (!purchase.IsExecutablePurchase)
		{
			reason = "This supplier step does not identify a concrete purchase target.";
			return false;
		}

		if (!TryResolvePurchaseTarget(context, null, purchase, out var target, out reason))
		{
			return false;
		}

		if (purchase.MaximumAmount is not null && purchase.MaximumAmount.Amount < target.Price)
		{
			reason = $"The cheapest matching supplier price is {target.Shop.Currency.Describe(target.Price, CurrencyDescriptionPatternType.ShortDecimal)}, above the maximum of {purchase.MaximumAmount.Currency.Describe(purchase.MaximumAmount.Amount, CurrencyDescriptionPatternType.ShortDecimal)}.";
			return false;
		}

		preview = new EmploymentPurchaseTargetPreview(
			target.Shop,
			target.Merchandise,
			target.Price,
			target.Quantity,
			target.CommodityWeight,
			target.Shop.CurrentLocations.Where(x => x is not null).DistinctBy(x => x.Id).ToList());
		reason = string.Empty;
		return true;
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
		var suppliers = SupplierShops(gameworld, supplierSelector);
		var candidates = suppliers
		                 .Where(shop => actor is null ||
		                                shop.CurrentLocations.Any(location => actor.Location?.Id == location.Id))
		                 .Select(shop => ResolvePurchaseTargetInShop(context, actor, shop, purchase))
		                 .Where(x => x is not null)
		                 .Cast<PurchaseTarget>()
		                 .OrderBy(x => x.Price)
		                 .ThenBy(x => x.Shop.Name)
		                 .ToList();
		if (candidates.Any())
		{
			target = candidates.First();
			reason = string.Empty;
			return true;
		}

		reason = supplierSelector.EqualTo("any")
			? $"No supplier shop at the assigned employee's current location has stock matching {purchase.PurchaseDescription}."
			: $"No supplier shop matching {supplierSelector} at the assigned employee's current location has stock matching {purchase.PurchaseDescription}.";
		return false;
	}

	private static IReadOnlyCollection<IShop> SupplierShops(IFuturemud gameworld, string supplierSelector)
	{
		supplierSelector = string.IsNullOrWhiteSpace(supplierSelector) ? "any" : supplierSelector.Trim();
		return supplierSelector.EqualTo("any")
			? gameworld.Shops.ToList()
			: gameworld.Shops.GetByIdOrName(supplierSelector) is { } supplier ? [supplier] : [];
	}

	private static PurchaseTarget? ResolvePurchaseTargetInShop(EmploymentTaskContext context, ICharacter? actor,
		IShop shop, PurchaseActionStep purchase)
	{
		return purchase.TargetKind switch
		{
			EmploymentPurchaseTargetKind.Item when purchase.ItemSelector is not null =>
				ResolveItemPurchaseTarget(context, actor, shop, purchase),
			EmploymentPurchaseTargetKind.Commodity when purchase.CommodityWeight.HasValue &&
			                                       !string.IsNullOrWhiteSpace(purchase.CommodityDescriptor) =>
				ResolveCommodityPurchaseTarget(context, actor, shop, purchase),
			_ => ResolveMerchandisePurchaseTarget(actor, shop, purchase)
		};
	}

	private static PurchaseTarget? ResolveMerchandisePurchaseTarget(ICharacter? actor, IShop shop,
		PurchaseActionStep purchase)
	{
		var merchandise = long.TryParse(purchase.MerchandiseSelector, out var id)
			? shop.Merchandises.FirstOrDefault(x => x.Id == id)
			: shop.Merchandises.FirstOrDefault(x => x.Name.EqualTo(purchase.MerchandiseSelector!)) ??
			  shop.Merchandises.FirstOrDefault(x =>
				  x.Name.StartsWith(purchase.MerchandiseSelector!, StringComparison.InvariantCultureIgnoreCase));
		if (merchandise is null)
		{
			return null;
		}

		if (merchandise.MerchandiseType == MerchandiseType.Commodity)
		{
			return null;
		}

		var quantity = purchase.Quantity ?? 1;
		var stockedItems = shop.StockedItems(merchandise).ToList();
		if (!string.IsNullOrWhiteSpace(purchase.KeywordFilter))
		{
			stockedItems = stockedItems
			               .Where(x => actor is null || x.HasKeywords(purchase.KeywordFilter.Split('.'), actor, true))
			               .ToList();
		}

		if (stockedItems.Sum(x => x.Quantity) < quantity)
		{
			return null;
		}

		return new PurchaseTarget(shop, merchandise, shop.PriceForMerchandise(actor, merchandise, quantity),
			quantity, purchase.KeywordFilter, []);
	}

	private static PurchaseTarget? ResolveItemPurchaseTarget(EmploymentTaskContext context, ICharacter? actor,
		IShop shop, PurchaseActionStep purchase)
	{
		foreach (var merchandise in shop.Merchandises)
		{
			if (merchandise.MerchandiseType == MerchandiseType.Commodity)
			{
				continue;
			}

			var matching = shop.StockedItems(merchandise)
			                   .Where(x => ItemThresholdCondition.MatchesSelector(context, x, purchase.ItemSelector!))
			                   .ToList();
			var quantity = purchase.Quantity ?? 1;
			if (matching.Sum(x => x.Quantity) < quantity)
			{
				continue;
			}

			var exactItems = SelectExactStockItemsForQuantity(matching, quantity);
			return new PurchaseTarget(shop, merchandise, shop.PriceForMerchandise(actor, merchandise, quantity),
				quantity, null, exactItems);
		}

		return null;
	}

	private static PurchaseTarget? ResolveCommodityPurchaseTarget(EmploymentTaskContext context, ICharacter? actor,
		IShop shop, PurchaseActionStep purchase)
	{
		var descriptor = ParseCommodityDescriptor(purchase.CommodityDescriptor!);
		foreach (var merchandise in shop.Merchandises.Where(x => x.MerchandiseType == MerchandiseType.Commodity))
		{
			if (!CommodityMerchandiseMatchesDescriptor(merchandise, descriptor))
			{
				continue;
			}

			var selected = new List<IGameItem>();
			var accumulated = 0.0;
			foreach (var item in shop.StockedItems(merchandise))
			{
				var weight = context.CommodityWeight(item, descriptor.Material, descriptor.Tag, descriptor.Characteristics);
				if (weight <= 0.0)
				{
					continue;
				}

				selected.Add(item);
				accumulated += weight;
				if (accumulated >= purchase.CommodityWeight!.Value)
				{
					break;
				}
			}

			if (accumulated < purchase.CommodityWeight!.Value)
			{
				continue;
			}

			var pricingWeight = merchandise.CommodityPricingWeight > 0.0 ? merchandise.CommodityPricingWeight : 1.0;
			var quantity = Math.Max(1, (int)Math.Ceiling(purchase.CommodityWeight!.Value / pricingWeight));
			return new PurchaseTarget(shop, merchandise,
				shop.PriceForMerchandiseWeight(actor, merchandise, purchase.CommodityWeight.Value),
				quantity, null, selected, purchase.CommodityWeight.Value);
		}

		return null;
	}

	private static bool CommodityMerchandiseMatchesDescriptor(IMerchandise merchandise, CommodityDescriptor descriptor)
	{
		if (merchandise.CommodityMaterial is null)
		{
			return false;
		}

		if (!merchandise.CommodityMaterial.Name.EqualTo(descriptor.Material) &&
		    !merchandise.CommodityMaterial.Id.ToString("F0").EqualTo(descriptor.Material))
		{
			return false;
		}

		if (!string.IsNullOrWhiteSpace(descriptor.Tag) &&
		    !(merchandise.CommodityTag?.Name.EqualTo(descriptor.Tag) == true ||
		      merchandise.CommodityTag?.FullName.EqualTo(descriptor.Tag) == true ||
		      merchandise.CommodityTag?.Id.ToString("F0").EqualTo(descriptor.Tag) == true))
		{
			return false;
		}

		foreach (var characteristic in descriptor.Characteristics)
		{
			var match = merchandise.CommodityCharacteristicRequirements.Any(x =>
				x.Key.Name.EqualTo(characteristic.Key) &&
				(
					x.Value.Name.EqualTo(characteristic.Value) ||
					x.Value.GetValue.EqualTo(characteristic.Value) ||
					x.Value.GetBasicValue.EqualTo(characteristic.Value) ||
					x.Value.GetFancyValue.EqualTo(characteristic.Value)
				));
			if (!match)
			{
				return false;
			}
		}

		return true;
	}

	private static IReadOnlyList<IGameItem> SelectExactStockItemsForQuantity(IEnumerable<IGameItem> items, int quantity)
	{
		var selected = new List<IGameItem>();
		var remaining = quantity;
		foreach (var item in items)
		{
			selected.Add(item);
			remaining -= item.Quantity;
			if (remaining <= 0)
			{
				break;
			}
		}

		return selected;
	}

	private sealed record CommodityDescriptor(string Material, string? Tag,
		IReadOnlyDictionary<string, string> Characteristics);

	private static CommodityDescriptor ParseCommodityDescriptor(string descriptor)
	{
		var parts = descriptor.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		var material = parts.FirstOrDefault() ?? descriptor;
		string? tag = null;
		var characteristics = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var part in parts.Skip(1))
		{
			var index = part.IndexOf('=');
			if (index > 0 && index < part.Length - 1)
			{
				characteristics[part[..index]] = part[(index + 1)..];
				continue;
			}

			tag ??= part;
		}

		return new CommodityDescriptor(material, tag, characteristics);
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
			case IHotel hotel:
				var hotelOwing = hotel.Property.HotelOutstandingTaxes;
				var hotelPayable = maximumAmount is null ? hotelOwing : Math.Min(hotelOwing, maximumAmount.Amount);
				amount = new MoneyAmount(hotel.Currency, hotelPayable);
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

	private static bool TryResolveTargetEmploymentHost(EmploymentTaskContext context, string targetHostKey,
		out IEmploymentHost host, out string reason)
	{
		host = null!;
		var gameworld = (context.Employer as IHaveFuturemud)?.Gameworld;
		if (gameworld is null)
		{
			reason = $"{context.Employer.EmploymentHostName} cannot resolve employment hosts for settlement steps.";
			return false;
		}

		var parts = targetHostKey.Split(':', 2, StringSplitOptions.TrimEntries);
		if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
		{
			reason = "Host settlement targets must be stored as '<host type>:<id|name>'.";
			return false;
		}

		var hostType = parts[0].CollapseString().ToLowerInvariant();
		var identifier = parts[1];
		if (hostType is "clan" or "organisation" or "organization")
		{
			var clan = gameworld.Clans.GetByIdOrName(identifier);
			if (clan?.IsTemplate == true)
			{
				reason = $"Clan templates cannot be used as employment settlement targets: {clan.FullName}.";
				return false;
			}

			if (clan is null)
			{
				reason = $"There is no {parts[0]} employment host matching {identifier}.";
				return false;
			}

			host = clan;
			reason = string.Empty;
			return true;
		}

		IEmploymentHost? resolved = hostType switch
		{
			"shop" or "store" => (IEmploymentHost?)gameworld.Shops.GetByIdOrName(identifier),
			"auction" or "auctionhouse" => (IEmploymentHost?)gameworld.AuctionHouses.GetByIdOrName(identifier),
			"arena" or "combatarena" => (IEmploymentHost?)gameworld.CombatArenas.GetByIdOrName(identifier),
			"bank" => (IEmploymentHost?)gameworld.Banks.GetByIdOrName(identifier),
			"stable" => (IEmploymentHost?)gameworld.Stables.GetByIdOrName(identifier),
			"hospital" or "clinic" or "infirmary" => (IEmploymentHost?)gameworld.Hospitals.GetByIdOrName(identifier),
			"hotel" => gameworld.Properties.GetByIdOrName(identifier)?.Hotel,
			_ => null
		};
		if (resolved is null)
		{
			reason = $"There is no {parts[0]} employment host matching {identifier}.";
			return false;
		}

		host = resolved;
		reason = string.Empty;
		return true;
	}
	private static bool TryResolveTargetBankAccount(EmploymentTaskContext context, string targetAccountKey,
		MoneyAmount amount, out IBankAccount account, out string reason)
	{
		account = null!;
		var gameworld = (context.Employer as IHaveFuturemud)?.Gameworld;
		if (gameworld is null)
		{
			reason = $"{context.Employer.EmploymentHostName} cannot resolve bank accounts for employment transfer steps.";
			return false;
		}

		var selector = targetAccountKey.Trim();
		if (string.IsNullOrWhiteSpace(selector))
		{
			reason = "Which target bank account should this transfer use?";
			return false;
		}

		IBankAccount? resolved = null;
		var numericSelector = selector.TrimStart('#');
		if (long.TryParse(numericSelector, NumberStyles.Integer, CultureInfo.InvariantCulture, out var accountId))
		{
			resolved = gameworld.BankAccounts.Get(accountId);
		}

		resolved ??= gameworld.BankAccounts.FirstOrDefault(x => x.AccountReference.EqualTo(selector));
		resolved ??= gameworld.BankAccounts.FirstOrDefault(x => x.Name.EqualTo(selector));
		resolved ??= gameworld.BankAccounts.FirstOrDefault(x =>
			x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
		if (resolved is null)
		{
			reason = $"There is no bank account matching {selector}.";
			return false;
		}

		account = resolved;
		if (account.AccountStatus != BankAccountStatus.Active)
		{
			reason = $"Bank account {account.AccountReference} is {account.AccountStatus.DescribeEnum()} and cannot receive employment transfers.";
			return false;
		}

		if (account.Currency.Id != amount.Currency.Id)
		{
			reason =
				$"Bank account {account.AccountReference} uses {account.Currency.Name}, but this transfer is for {amount.Currency.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}
	private static bool TryResolveFinanceHost(IEmploymentHost host, MoneyAmount? amount, out FinanceHost finance,
		out string reason)
	{
		finance = null!;
		switch (host)
		{
			case IShop shop:
				finance = new FinanceHost(shop, shop.Currency, shop.BankAccount);
				break;
			case IStable stable:
				finance = new FinanceHost(stable, stable.Currency, stable.BankAccount);
				break;
			case IHospital hospital:
				finance = new FinanceHost(hospital, hospital.Currency, hospital.BankAccount);
				break;
			case IHotel hotel:
				finance = new FinanceHost(hotel.Property, hotel.Currency, hotel.BankAccount);
				break;
			case IAuctionHouse auctionHouse:
				finance = new FinanceHost((IFrameworkItem)auctionHouse, auctionHouse.EconomicZone.Currency,
					auctionHouse.ProfitsBankAccount);
				break;
			case ICombatArena arena:
				finance = new FinanceHost(arena, arena.Currency, arena.BankAccount);
				break;
			case IBank bank:
				finance = new FinanceHost(bank, bank.PrimaryCurrency, null);
				break;
			case IClan clan:
				var clanCurrency = clan.ClanBankAccount?.Currency ?? amount?.Currency ?? ResolveContractCurrency(clan);
				if (clanCurrency is null)
				{
					reason =
						$"{host.EmploymentHostName} does not have a clan bank account or existing contract currency for employment finance.";
					return false;
				}

				finance = new FinanceHost(
					clan,
					clanCurrency,
					clan.ClanBankAccount?.Currency == clanCurrency ? clan.ClanBankAccount : null);
				break;
			case IHaveCurrency currencyHost:
				finance = new FinanceHost(host, currencyHost.Currency, null);
				break;
			default:
				reason =
					$"{host.EmploymentHostName} does not expose a native employment finance adapter yet.";
				return false;
		}

		if (amount is not null && finance.Currency.Id != amount.Currency.Id)
		{
			reason = $"{host.EmploymentHostName} uses {finance.Currency.Name}, but this step is for {amount.Currency.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static ICurrency? ResolveContractCurrency(IEmploymentHost host)
	{
		return host.EmploymentContracts
		           .Select(x => x.Compensation.FixedRate?.Currency ?? x.Compensation.MinimumEffectivePay?.Currency)
		           .FirstOrDefault(x => x is not null);
	}

	private static decimal AvailableFunds(EmploymentTaskContext context, FinanceHost finance, ICurrency currency)
	{
		var bankCapacity = finance.BankAccount?.Currency.Id == currency.Id
			? Math.Max(0.0M, finance.BankAccount.MaximumWithdrawal())
			: 0.0M;
		var reserved = ActiveReservations(context)
		               .Where(x => x.CurrencyId == currency.Id)
		               .Sum(x => x.Amount);
		return Math.Max(0.0M, VirtualBalance(finance) + bankCapacity - reserved);
	}

	private static decimal AvailableFundsWithoutReservations(FinanceHost finance)
	{
		var bankCapacity = finance.BankAccount?.Currency.Id == finance.Currency.Id
			? Math.Max(0.0M, finance.BankAccount.MaximumWithdrawal())
			: 0.0M;
		return Math.Max(0.0M, VirtualBalance(finance) + bankCapacity);
	}

	private static decimal VirtualBalance(FinanceHost finance)
	{
		return finance.Owner switch
		{
			ICombatArena arena => arena.CashBalance,
			_ => VirtualCashLedger.Balance(finance.Owner, finance.Currency)
		};
	}

	private static bool DebitAvailable(FinanceHost finance, decimal amount, ICharacter? actor,
		IFrameworkItem counterparty, string sourceKind, string reference, MudDateTime? mudDateTime,
		out string reason)
	{
		if (finance.Owner is ICombatArena arena)
		{
			var canDebit = arena.EnsureFunds(amount);
			if (!canDebit.Truth)
			{
				reason = canDebit.Reason;
				return false;
			}

			arena.Debit(amount, reference);
			reason = string.Empty;
			return true;
		}

		return VirtualCashLedger.Debit(finance.Owner, finance.Currency, amount, actor, counterparty,
			sourceKind, reference, finance.BankAccount, mudDateTime, out reason, finance.Owner, reference);
	}

	private static bool DebitVirtual(FinanceHost finance, decimal amount, ICharacter? actor,
		IFrameworkItem? counterparty, string sourceKind, string reference, MudDateTime? mudDateTime,
		out string reason)
	{
		if (finance.Owner is ICombatArena arena)
		{
			var canDebit = arena.EnsureCashFunds(amount);
			if (!canDebit.Truth)
			{
				reason = canDebit.Reason;
				return false;
			}

			arena.DebitCash(amount, reference);
			reason = string.Empty;
			return true;
		}

		return VirtualCashLedger.Debit(finance.Owner, finance.Currency, amount, actor, counterparty,
			sourceKind, reference, null, mudDateTime, out reason, finance.Owner, reference);
	}

	private static void CreditVirtual(FinanceHost finance, decimal amount, ICharacter? actor,
		IFrameworkItem? counterparty, string sourceKind, string reference, MudDateTime? mudDateTime)
	{
		if (finance.Owner is ICombatArena arena)
		{
			arena.CreditCash(amount, reference);
			return;
		}

		VirtualCashLedger.Credit(finance.Owner, finance.Currency, amount, actor, counterparty,
			sourceKind, reference, mudDateTime, finance.Owner, reference);
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
