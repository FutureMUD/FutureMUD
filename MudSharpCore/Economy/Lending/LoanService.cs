#nullable enable
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{
	private static IEconomicZone ZoneOf(IFrameworkItem lender) => lender switch
	{
		IBank bank => bank.EconomicZone,
		IShop shop => shop.EconomicZone,
		_ => throw new ArgumentException("Only banks and shops can issue loans.", nameof(lender))
	};

	internal static IFrameworkItem Lender(IFuturemud world, LendingContract loan) => loan.LenderType == "Bank"
		? world.Banks.Get(loan.LenderId) : world.Shops.Get(loan.LenderId);

	private static string Money(ICurrency currency, decimal amount) =>
		currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal);

	private static decimal Available(IFrameworkItem lender) => lender switch
	{
		IBank bank => Math.Max(0M, bank.CurrencyReserves[bank.EconomicZone.Currency] -
			bank.BankAccounts.Where(x => x.Currency == bank.EconomicZone.Currency)
				.Sum(x => Math.Max(0M, x.CurrentBalance + x.CurrentMonthInterest - x.CurrentMonthFees))),
		IShop shop when shop.Currency == shop.EconomicZone.Currency => Math.Max(0M, shop.CashBalance + shop.GetCurrencyPilesForShop().Where(x => x.Currency == shop.Currency && !SpellOwnedItemValuePolicy.ContainsTemporaryValue(x.Parent)).Sum(x => x.TotalValue)),
		_ => 0M
	};

	internal static void Credit(IFrameworkItem lender, decimal amount, string reason)
	{
		switch (lender)
		{
			case IBank bank:
				AdjustReserves(bank, bank.EconomicZone.Currency, amount);
				break;
			case IShop shop:
				if (amount < 0 && shop.CashBalance < -amount)
				{
					var physical = -amount - Math.Max(0M, shop.CashBalance);
					TakeSettlementCoins(shop.Currency, shop.GetCurrencyPilesForShop().Where(x => x.Currency == shop.Currency).ToList(), physical);
					amount += physical;
				}
				if (GuardingSettlement)
				{
					var cash = shop.CashBalance;
					var expected = shop.ExpectedCashBalance;
					OnRollback(() => { shop.CashBalance = cash; shop.ExpectedCashBalance = expected; });
				}
				shop.CashBalance += amount;
				shop.ExpectedCashBalance += amount;
				shop.Changed = true;
				break;
			default:
				throw new InvalidOperationException(reason);
		}
	}

	internal static void Post(LendingContract loan, string kind, decimal principal, decimal interest, decimal amount,
		string detail, MudDateTime now, long? actorId = null, string? operationKey = null)
	{
		operationKey ??= Guid.NewGuid().ToString("N");
		if (_settlement != null) _settlement.Receipt ??= (loan.Id, operationKey);
		FMDB.Context.LendingPostings.Add(new LendingPosting
		{
			ContractId = loan.Id, Kind = kind, Principal = principal, Interest = interest, Amount = amount,
			Detail = detail.Length <= 1000 ? detail : detail[..999] + "…", ActorId = actorId,
			MudDate = now.GetDateTimeString(), RealDate = DateTime.UtcNow,
			OperationKey = operationKey
		});
		RecordMovement(loan, kind, interest, amount, now);
	}

	[ThreadStatic] private static bool _settling;

	private static string Settle(ICharacter actor, Func<string> action, bool guard = false)
	{
		if (_settling) return "A lending transaction is already in progress.";
		_settling = true;
		try
		{
			return guard ? GuardedSettlement(actor.Gameworld, action) : InTransaction(actor.Gameworld, action);
		}
		finally { _settling = false; }
	}

	internal static T InTransaction<T>(IFuturemud world, Func<T> action) =>
		GuardingSettlement ? action() : GuardedTransaction(world, action);

	private static decimal Exposure(IFrameworkItem lender, long borrower) =>
		FMDB.Context.LendingContracts.Where(x => x.LenderId == lender.Id && x.LenderType == lender.FrameworkItemType &&
			x.BorrowerId == borrower).Sum(x => (decimal?)x.Principal) ?? 0M;

	internal static string Request(ICharacter actor, IFrameworkItem lender, LendingProduct product,
		decimal amount, string asset, decimal downPayment, ICharacter? manager = null) => Settle(actor, () =>
	{
		var world = actor.Gameworld;
		if (!world.GetStaticBool("LoansEnabled") || !LocalLenders(actor).Contains(lender)) return "New loans are unavailable here.";
		if (manager != null && (!IsManager(manager, lender) || manager.Location != actor.Location ||
		    CharacterInstanceIdentityComparer.SameIdentity(actor, manager))) return "An independent manager must issue this offer at the counter.";
		var live = FMDB.Context.LendingProducts.Find(product.Id);
		var zone = ZoneOf(lender);
		if (live is not { Enabled: true } || live.LenderId != lender.Id || live.LenderType != lender.FrameworkItemType ||
		    live.EconomicZoneId != zone.Id) return "That product is unavailable at this lender.";
		var terms = ProductTerms(live);
		terms.DownPayment = downPayment;
		if (!TrySecurity(actor, lender, terms, asset, out var error)) return error;
		var borrower = CharacterInstanceIdentityComparer.IdentityId(actor);
		var exposure = Exposure(lender, borrower);
		terms.Quantum = zone.Currency.Coins.Where(x => x.UseForChange && x.Value > 0).Select(x => x.Value).DefaultIfEmpty(1M).Min();
		if (terms.Kind == "Mortgage") amount = terms.Price - downPayment;
		else if (terms.Kind == "Pawn" && amount == 0)
		{
			var maximum = MaximumAdvance(actor, lender, live, terms.Appraisal, exposure);
			amount = maximum - maximum % terms.Quantum;
		}
		if (amount <= 0 || downPayment < 0) return "The requested advance must be positive.";
		var valuation = terms.Kind == "Mortgage" ? Math.Min(terms.Price, terms.Appraisal) : terms.Appraisal;
		if (!Underwrite(actor, lender, live, amount, valuation, exposure, out error)) return error;
		if (AvailableForAdvance(lender, terms, amount) < amount) return "The lender has insufficient available funds.";
		if (!LocalLenders(actor).Contains(lender) || manager != null && (!IsManager(manager, lender) || manager.Location != actor.Location))
			return "Return to the lender to request this offer.";
		if (amount % terms.Quantum != 0) return $"Request an amount in multiples of {Money(zone.Currency, terms.Quantum)}.";
		terms.Advance = amount;
		terms.ApprovedBy = manager is null ? null : CharacterInstanceIdentityComparer.IdentityId(manager);
		terms.OfferExpiryUtc = DateTime.UtcNow.AddDays(7);
		var contract = new LendingContract
		{
			ProductId = live.Id, LenderId = lender.Id, LenderType = lender.FrameworkItemType,
			BorrowerId = borrower, EconomicZoneId = zone.Id, CurrencyId = zone.Currency.Id,
			Status = manager != null || Automatic(live) ? "Offered" : "Requested"
		};
		Store(contract, terms);
		FMDB.Context.LendingContracts.Add(contract);
		FMDB.Context.SaveChanges();
		Portfolios.GetOrCreateValue(world);
		Post(contract, contract.Status, 0, 0, amount, live.Name, zone.FinancialPeriodReferenceCalendar.CurrentDateTime, terms.ApprovedBy ?? borrower);
		return Summary(actor, contract, terms);
	});

	internal static string Accept(ICharacter actor, long id, bool conveyancing = false, string? intoAccount = null,
		long? propertyId = null, string? contributionAccount = null) => Settle(actor, () =>
	{
		var world = actor.Gameworld;
		var loan = FMDB.Context.LendingContracts.Find(id);
		var borrower = CharacterInstanceIdentityComparer.IdentityId(actor);
		if (loan is null || loan.BorrowerId != borrower) return "You have no such loan.";
		var terms = Terms(loan);
		var lender = Lender(world, loan);
		if (lender is null) return "The lending establishment is unavailable.";
		var zone = ZoneOf(lender);
		var now = zone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		if (!conveyancing && loan.Status == "Active" && terms.Amendment is not null)
			return AcceptAmendment(actor, loan, terms, lender, now);
		if (loan.Status != "Offered") return $"This loan is {loan.Status.ToLowerInvariant()}.";
		if (!world.GetStaticBool("LoansEnabled") || terms.OfferExpiryUtc <= DateTime.UtcNow)
			return "This offer has expired or new lending has been disabled.";
		if (terms.Kind == "Mortgage" && !conveyancing)
			return $"At the conveyancer use PROPERTY BUY \"{world.Properties.Get(terms.AssetId)?.Name}\" LOAN {id}.";
		if (!AtClosing(actor, lender, terms, conveyancing)) return "Visit the issuing counter or the correct conveyancer.";
		if (conveyancing && propertyId != terms.AssetId) return "This mortgage does not finance that property.";
		var product = FMDB.Context.LendingProducts.Find(loan.ProductId);
		if (product is not { Enabled: true } || product.EconomicZoneId != zone.Id || zone.Currency.Id != loan.CurrencyId ||
		    ProductTerms(product).Kind != terms.Kind)
			return "The product, zone or settlement currency has changed. Request a new offer.";
		var liveTerms = ProductTerms(product);
		if (!CanSecure(actor, lender, terms, out var error)) return error;
		if (terms.ApprovedBy is { } managerId && (world.TryGetCharacter(managerId, true) is not { } approver || !IsManager(approver, lender)))
			return "The approving manager no longer represents the lender. Request a fresh offer.";
		var appraisal = terms.Kind == "Mortgage" ? Math.Min(terms.Price, terms.Appraisal) : terms.Appraisal;
		if (!Underwrite(actor, lender, product, terms.Advance, appraisal, Exposure(lender, borrower), out error)) return error;
		if (loan.Status != "Offered" || !product.Enabled || !world.GetStaticBool("LoansEnabled") ||
		    terms.ApprovedBy is { } recheckManager && (world.TryGetCharacter(recheckManager, true) is not { } approved || !IsManager(approved, lender)))
			return "This offer or the lender's approval changed. Request a fresh offer.";
		// A configurable prog can execute game actions. Revalidate custody, location and funds after it returns.
		if (!AtClosing(actor, lender, terms, conveyancing) || !CanSecure(actor, lender, terms, out error)) return error.Length > 0 ? error : "The closing location changed.";
		if (liveTerms.UseShopAccount != terms.UseShopAccount || AvailableForAdvance(lender, liveTerms, terms.Advance) < terms.Advance)
			return "The lender's funding has changed or its available funds are insufficient.";
		IBankAccount? destination = null;
		Dictionary<ICoin, int>? coins = null;
		if (!string.IsNullOrWhiteSpace(intoAccount))
		{
			if (terms.Kind == "Mortgage") return "Mortgage proceeds go directly to the sellers.";
			var found = Banking.Bank.FindBankAccount(intoAccount, lender as IBank, actor);
			destination = found.Account;
			if (destination is null) return found.Error;
			if (!destination.IsAccountOwner(actor) || destination.Currency != zone.Currency)
				return "Choose your own active account in the loan currency.";
		}
		else if (terms.Kind != "Mortgage")
		{
			coins = zone.Currency.FindCoinsForAmount(terms.Advance, out var exact);
			if (!exact) return "The lender cannot tender that amount in exact coins. Specify INTO <account>.";
		}
		IBankAccount? depositAccount = null;
		if (terms.Kind == "Mortgage" && terms.DownPayment > 0)
		{
			if (string.IsNullOrWhiteSpace(contributionAccount))
			{
				if (!CanTender(actor, zone.Currency, terms.DownPayment)) return "Bring your down payment in cash or specify FROM <account>.";
			}
			else
			{
				var found = Banking.Bank.FindBankAccount(contributionAccount, lender as IBank, actor);
				depositAccount = found.Account;
				if (depositAccount is null) return found.Error;
				if (!depositAccount.IsAuthorisedAccountUser(actor) || depositAccount.Currency != zone.Currency)
					return "Choose an authorised account in the purchase currency.";
				var fee = depositAccount.BankAccountType.QuoteTransactionWithdrawalFees(depositAccount, terms.DownPayment);
				if (!depositAccount.CanWithdraw(terms.DownPayment, false).Truth ||
				    terms.DownPayment + fee > depositAccount.MaximumWithdrawal() ||
				    depositAccount.Bank.CurrencyReserves[zone.Currency] < terms.DownPayment)
					return "The account cannot cover your down payment and bank charges.";
			}
		}
		if (terms.Kind != "Unsecured")
		{
			loan.SecurityKey = $"{(terms.Kind == "Pawn" ? "Item" : "Property")}:{terms.AssetId}";
			if (FMDB.Context.LendingContracts.Any(x => x.SecurityKey == loan.SecurityKey && x.Id != loan.Id))
			{
				loan.SecurityKey = null;
				return "That asset already secures a loan.";
			}
		}
		StartSchedule(terms, now);
		if (depositAccount != null)
		{
			depositAccount.WithdrawFromTransaction(terms.DownPayment, $"Down payment on mortgage #{id}");
			AdjustReserves(depositAccount.Bank, zone.Currency, -terms.DownPayment);
		}
		Secure(actor, lender, loan, terms, depositAccount != null);
		if (terms.UseShopAccount && lender is IShop { BankAccount: { } funding })
		{
			funding.WithdrawFromTransaction(terms.Advance, $"Advance on shop loan #{id}");
			AdjustReserves(funding.Bank, zone.Currency, -terms.Advance);
		}
		else Credit(lender, -terms.Advance, $"Advance on loan #{id}");
		if (destination != null)
		{
			destination.DoAccountCredit(terms.Advance, $"Advance on loan #{id}");
			AdjustReserves(destination.Bank, zone.Currency, terms.Advance);
		}
		else if (coins != null) GiveCash(actor, zone.Currency, coins);
		loan.Principal = terms.Advance;
		loan.Status = "Active";
		Store(loan, terms);
		Post(loan, "Advance", terms.Advance, 0, terms.Advance, product.Name, now, borrower);
		return $"Loan #{id} is active.\n{Summary(actor, loan, terms)}";
	}, guard: true);

	private static bool AtClosing(ICharacter actor, IFrameworkItem lender, LoanTerms terms, bool conveyancing) =>
		conveyancing ? terms.Kind == "Mortgage" && ZoneOf(lender).ConveyancingRooms.Contains(actor.Location)
			: LocalLenders(actor).Contains(lender);

	private static decimal AvailableForAdvance(IFrameworkItem lender, LoanTerms terms, decimal advance)
	{
		var balance = Available(lender);
		if (terms.UseShopAccount && lender is IShop shop)
		{
			var account = shop.BankAccount;
			if (account is null || account.AccountStatus != BankAccountStatus.Active || !account.IsAccountOwner(shop) ||
			    account.Currency != shop.EconomicZone.Currency || shop.Currency != account.Currency) return 0M;
			balance = Math.Min(account.Bank.CurrencyReserves[account.Currency], account.CurrentBalance -
				account.BankAccountType.QuoteTransactionWithdrawalFees(account, advance));
		}
		return Math.Max(0M, balance - terms.LiquidityFloor);
	}

	private static void GiveCash(ICharacter actor, ICurrency currency, Dictionary<ICoin, int> coins)
	{
		if (GuardingSettlement) { PrepareBorrowerChange(actor, currency, coins); return; }
		var pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency, coins);
		pile.SetOwner(actor);
		if (actor.Body.CanGet(pile, 0)) actor.Body.Get(pile, silent: true);
		else pile.InsertAtSource(actor, true);
	}

	internal static string Decline(ICharacter actor, long id) => Settle(actor, () =>
	{
		var loan = FMDB.Context.LendingContracts.Find(id);
		if (loan?.BorrowerId != CharacterInstanceIdentityComparer.IdentityId(actor)) return "You have no such offer.";
		var terms = Terms(loan);
		if (terms.Amendment is not null) terms.Amendment = null;
		else if (loan.Status is "Offered" or "Requested") loan.Status = "Declined";
		else return "There is no outstanding offer.";
		Store(loan, terms);
		return $"You decline the offer on loan #{id}.";
	});
}
