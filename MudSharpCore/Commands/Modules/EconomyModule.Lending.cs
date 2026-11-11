#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Lending;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Intervals;
using System.Text.Json;
using LendingProduct = MudSharp.Models.LendingProduct;

namespace MudSharp.Commands.Modules;

internal partial class EconomyModule
{
	private const string LoanPlayerHelp = @"
LOANS
View current loans, offers, upcoming payments, statements and recovery.

#3LOANS#0 - list your loans and current offers at this location
#3LOAN <loan id>#0 - show your statement and payoff amount
#3LOAN OFFERS [product]#0 - browse products offered here
#3LOAN ASK <product id|name> <amount> [against <asset>]#0 - request an advance
#3LOAN PAWN <held item> [amount] [using <product id>]#0 - pledge an item to a pawnbroker
#3LOAN BUY <property> WITH <down payment> [USING <product id>]#0 - request purchase financing
#3LOAN ACCEPT <offer id> [into <account>]#0 - accept an advance
#3LOAN DECLINE <offer id>#0 - decline a written offer
#3LOAN PAY <loan id> [amount|all] [from <account>]#0 - make a repayment
#3LOAN AUTOPAY <loan id> [from <account>|off|retry]#0 - view or arrange automatic repayments
#3LOAN COLLECT <loan id> [into <account>]#0 - collect a released pledge or surplus
#3LOAN HISTORY <loan id>#0 - view repayments and changes

Your statement shows the amount due now, the next payment and date, and the payoff amount. The offer explains any collateral, fees, interest, missed-payment grace, and consequences of default.
";

	private const string LoanManagerHelp = @"
LOAN MANAGEMENT
Lender managers can publish products, review requests, agree payment changes, enforce security and resolve losses.

#3LOAN MANAGE#0 - list requests, overdue accounts and recoveries
#3LOAN MANAGE PRODUCT#0 - list lending products
#3LOAN MANAGE PRODUCT NEW <name> <unsecured|pawn|property|mortgage>#0 - create a product
#3LOAN MANAGE PRODUCT EDIT <id> SET <field> <value>#0 - change draft terms or limits
#3LOAN MANAGE PRODUCT ENABLE|DISABLE <id>#0 - publish or withdraw a product
#3LOAN MANAGE <bank|shop> <lender id> <action>#0 - select a lender at a shared location
#3LOAN MANAGE OFFER <borrower> <product> <amount> [against <asset>]#0 - make a direct offer
#3LOAN MANAGE OFFER <request id> <reason>#0 - decide a pending request
#3LOAN MANAGE APPROVE|DENY|ENFORCE|RELIST|WRITEOFF <loan id> <reason>#0 - manage a case
#3LOAN MANAGE PLAN <loan id> <periods> <principal-per-period> <reason>#0 - offer a new schedule
#3LOAN MANAGE PAUSE <loan id> <days> <interest|free> <reason>#0 - offer a payment pause
#3LOAN MANAGE REFINANCE <loan id> <periods> <principal-per-period> <reason>#0 - replace loan terms

Product fields: RATE, INTERVAL, PERIODS, GRACE, MINIMUM (none|interest|principal), INSTALMENT, RECOURSE and AUTOMATIC.
Limits: MAXSECURED, MAXUNSECURED, EXPOSURESECURED, EXPOSUREUNSECURED, MINCOLLATERAL and COVERAGE. Coverage 1.25 permits an advance of up to 80% of the appraisal.
Filters: SECUREDELIGIBILITY and UNSECUREDELIGIBILITY return Boolean. SECUREDMAXIMUM and UNSECUREDMAXIMUM return Number. Set each to a prog name, ID or NONE. Inputs are (Character, requested amount, collateral value, existing principal at this lender). A failed prog declines the application.
Funding: FUNDING CASH|ACCOUNT, FLOOR <amount> and AUCTIONHOUSE <id|NONE>.
";

	[PlayerCommand("Loan", "loan", "loans")]
	[HelpInfo("loan", LoanPlayerHelp, AutoHelp.HelpArg)]
	[ConditionalHelpInfo(nameof(CanSeeLoanManagerHelp), LoanManagerHelp)]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Loan(ICharacter actor, string input)
	{
		if (!LoanService.Enabled(actor) && !LoanService.HasPortfolio(actor.Gameworld))
		{
			actor.OutputHandler.Send("Lending is not available in this world.");
			return;
		}

		var stack = new StringStack(input.RemoveFirstWord());
		var action = stack.PopForSwitch();
		if (string.IsNullOrEmpty(action))
		{
			actor.OutputHandler.Send(LoanService.List(actor));
			return;
		}

		if (long.TryParse(action, NumberStyles.None, CultureInfo.InvariantCulture, out var loanId))
		{
			actor.OutputHandler.Send(LoanService.Show(actor, loanId, false));
			return;
		}

		switch (action)
		{
			case "offers":
				ShowLoanOffers(actor, stack.SafeRemainingArgument);
				return;
			case "ask":
				RequestLoan(actor, stack, LoanRequestKind.General);
				return;
			case "pawn":
				RequestLoan(actor, stack, LoanRequestKind.Pawn);
				return;
			case "buy":
			case "mortgage":
				RequestLoan(actor, stack, LoanRequestKind.Mortgage);
				return;
			case "accept":
				AcceptLoan(actor, stack, false);
				return;
			case "decline":
				DeclineLoan(actor, stack);
				return;
			case "pay":
				PayLoan(actor, stack);
				return;
			case "autopay":
				AutopayLoan(actor, stack);
				return;
			case "collect":
				CollectLoan(actor, stack);
				return;
			case "history":
			case "statement":
				LoanStatement(actor, stack);
				return;
			case "manage":
				ManageLoans(actor, stack);
				return;
			default:
				actor.OutputHandler.Send(LoanPlayerHelp.SubstituteANSIColour());
				return;
		}
	}

	private enum LoanRequestKind
	{
		General,
		Pawn,
		Mortgage
	}

	private static bool CanSeeLoanManagerHelp(ICharacter actor) =>
		LoanService.LocalLenders(actor).Any(x => LoanService.IsManager(actor, x));

	private static void ShowLoanOffers(ICharacter actor, string productName)
	{
		if (!LoanService.Enabled(actor))
		{
			actor.OutputHandler.Send("New loan offers are paused in this world. Existing loans can still be serviced.");
			return;
		}

		var lenders = LoanService.LocalLenders(actor).ToList();
		if (lenders.Count == 0)
		{
			actor.OutputHandler.Send("There is no lender at this location. Find a bank branch or a shop that offers loans.");
			return;
		}

		var result = new StringBuilder();
		foreach (var lender in lenders)
		{
			var products = LoanService.Products(lender).Where(x =>
				string.IsNullOrWhiteSpace(productName) ||
				(long.TryParse(productName, NumberStyles.None, CultureInfo.InvariantCulture, out var productId) && x.Id == productId) ||
				x.Name.EqualTo(productName) || x.Name.Contains(productName, StringComparison.InvariantCultureIgnoreCase) ||
				LoanService.ProductTerms(x).Kind.EqualTo(productName)).ToList();
			if (products.Count == 0)
			{
				continue;
			}

			result.AppendLine(lender.Name.ColourName());
			foreach (var product in products)
			{
				var terms = LoanService.ProductTerms(product);
				result.AppendLine($"  #{product.Id.ToString("N0", actor)} {product.Name.ColourName()} ({terms.Kind})");
				result.AppendLine($"    {terms.Rate.ToString("P2", actor)} per {terms.Interval}, {terms.Periods.ToString("N0", actor)} periods, {terms.Minimum} minimum");
			}
		}

		actor.OutputHandler.Send(result.Length == 0
			? "No lending products match that selection at this location."
			: result.ToString());
	}

	private static void RequestLoan(ICharacter actor, StringStack stack, LoanRequestKind kind)
	{
		if (stack.IsFinished)
		{
			actor.OutputHandler.Send(kind switch
			{
				LoanRequestKind.Pawn => "What held item do you want to pledge?",
				LoanRequestKind.Mortgage => "Which listed property do you want to buy?",
				_ => "Which lending product do you want and how much do you need?"
			});
			return;
		}

		var remainder = string.Empty;
		var asset = string.Empty;
		var productSelector = string.Empty;
		var amountText = string.Empty;
		var downPaymentText = string.Empty;
		var requestedAmount = 0.0M;
		var downPayment = 0.0M;

		if (kind == LoanRequestKind.Pawn)
		{
			asset = stack.PopSpeech().Trim('"');
			remainder = stack.SafeRemainingArgument;
			ExtractProductSelector(ref remainder, ref productSelector);
			amountText = remainder.Trim();
		}
		else if (kind == LoanRequestKind.Mortgage)
		{
			remainder = stack.SafeRemainingArgument;
			ExtractProductSelector(ref remainder, ref productSelector);
			var withIndex = remainder.LastIndexOf(" with ", StringComparison.InvariantCultureIgnoreCase);
			if (withIndex >= 0)
			{
				asset = remainder[..withIndex].Trim().Trim('"');
				downPaymentText = remainder[(withIndex + 6)..].Trim();
			}
			else
			{
				asset = remainder.Trim().Trim('"');
			}
		}
		else
		{
			productSelector = stack.PopSpeech();
			remainder = stack.SafeRemainingArgument;
			var againstIndex = remainder.IndexOf(" against ", StringComparison.InvariantCultureIgnoreCase);
			if (againstIndex >= 0)
			{
				asset = remainder[(againstIndex + 9)..].Trim().Trim('"');
				amountText = remainder[..againstIndex].Trim();
			}
			else
			{
				amountText = remainder.Trim();
			}
		}

		var lenders = LoanService.LocalLenders(actor).ToList();
		var choices = lenders
			.SelectMany(lender => LoanService.Products(lender)
				.Where(product => ProductMatchesKind(LoanService.ProductTerms(product).Kind, kind))
				.Where(product => ProductMatchesSelector(product, LoanService.ProductTerms(product).Kind, productSelector, kind))
				.Select(product => (Lender: lender, Product: product)))
			.ToList();
		if (choices.Count != 1)
		{
			ShowLoanOffers(actor, productSelector);
			actor.OutputHandler.Send(choices.Count == 0
				? "No matching lending product is offered at this location. Select an enabled product ID shown by LOAN OFFERS."
				: "Several products match. Select one by ID with LOAN ASK <product id> <amount> AGAINST <asset>, or add USING <product id> to LOAN PAWN or LOAN BUY.");
			return;
		}

		var selected = choices[0];
		if (kind == LoanRequestKind.Pawn)
		{
			if (!string.IsNullOrWhiteSpace(amountText) && !TryParseCurrency(selected.Lender, amountText, out requestedAmount, out var amountError))
			{
				actor.OutputHandler.Send(amountError);
				return;
			}
		}
		else if (kind == LoanRequestKind.Mortgage)
		{
			if (!string.IsNullOrWhiteSpace(downPaymentText) && !TryParseCurrency(selected.Lender, downPaymentText, out downPayment, out var downPaymentError))
			{
				actor.OutputHandler.Send(downPaymentError);
				return;
			}
		}
		else if (!TryParseCurrency(selected.Lender, amountText, out requestedAmount, out var requestError))
		{
			actor.OutputHandler.Send(requestError);
			return;
		}

		if (!LoanService.Enabled(actor))
		{
			actor.OutputHandler.Send("New lending is disabled in this world.");
			return;
		}

		actor.OutputHandler.Send(LoanService.Request(actor, selected.Lender, selected.Product, requestedAmount, asset, downPayment));
	}

	private static void ExtractProductSelector(ref string arguments, ref string productSelector)
	{
		var prefixed = " " + arguments;
		var usingIndex = prefixed.LastIndexOf(" using ", StringComparison.InvariantCultureIgnoreCase);
		if (usingIndex < 0)
		{
			return;
		}

		productSelector = prefixed[(usingIndex + 7)..].Trim().Trim('"');
		arguments = prefixed[..usingIndex].Trim();
	}

	private static bool ProductMatchesSelector(LendingProduct product, string kind, string selector,
		LoanRequestKind requestKind)
	{
		if (string.IsNullOrWhiteSpace(selector))
		{
			return true;
		}

		if (long.TryParse(selector, NumberStyles.None, CultureInfo.InvariantCulture, out var productId))
		{
			return product.Id == productId;
		}

		if (product.Name.EqualTo(selector))
		{
			return true;
		}

		return requestKind == LoanRequestKind.Pawn && kind.EqualTo("Pawn") &&
		       product.Name.Contains(selector, StringComparison.InvariantCultureIgnoreCase);
	}

	private static bool ProductMatchesKind(string productKind, LoanRequestKind requested) => requested switch
	{
		LoanRequestKind.Pawn => productKind.EqualTo("Pawn"),
		LoanRequestKind.Mortgage => productKind.EqualTo("Mortgage"),
		_ => productKind.EqualTo("Unsecured") || productKind.EqualTo("Pawn") || productKind.EqualTo("Property")
	};

	private static string KindName(string kind) => kind.ToLowerInvariant();

	private static bool TryParseCurrency(IFrameworkItem lender, string amountText, out decimal amount, out string error)
	{
		amount = 0.0M;
		var currency = lender switch
		{
			IBank bank => bank.EconomicZone.Currency,
			IShop shop => shop.EconomicZone.Currency,
			_ => null
		};
		if (currency is not null && currency.TryGetBaseCurrency(amountText, out amount) && amount >= 0.0M)
		{
			error = string.Empty;
			return true;
		}

		error = "Enter a non-negative amount in this lender's currency.";
		return false;
	}

	private static IFrameworkItem? ResolveOneLender(ICharacter actor, out string error)
	{
		var lenders = LoanService.LocalLenders(actor).ToList();
		if (lenders.Count == 1)
		{
			error = string.Empty;
			return lenders[0];
		}

		error = lenders.Count == 0
			? "Find a bank branch or lending shop before starting an application."
			: "Several lenders serve this location. Use LOAN with a product offered by the lender you want.";
		return null;
	}

	private static void AcceptLoan(ICharacter actor, StringStack stack, bool conveyancing)
	{
		if (!long.TryParse(stack.PopSpeech(), NumberStyles.None, CultureInfo.InvariantCulture, out var id))
		{
			actor.OutputHandler.Send("Which offer number do you want to accept?");
			return;
		}

		string? account = null;
		if (!stack.IsFinished)
		{
			var into = stack.PopForSwitch();
			if (into != "into" || stack.IsFinished)
			{
				actor.OutputHandler.Send("For an advance into an account, use LOAN ACCEPT <id> INTO <account>. Mortgages close at the conveyancer.");
				return;
			}

			account = stack.SafeRemainingArgument.Trim().Trim('"');
		}

		actor.OutputHandler.Send(LoanService.Accept(actor, id, conveyancing, account));
	}

	private static void DeclineLoan(ICharacter actor, StringStack stack)
	{
		if (!long.TryParse(stack.PopSpeech(), NumberStyles.None, CultureInfo.InvariantCulture, out var id))
		{
			actor.OutputHandler.Send("Which offer number do you want to decline?");
			return;
		}

		actor.OutputHandler.Send(LoanService.Decline(actor, id));
	}

	private static void AutopayLoan(ICharacter actor, StringStack stack)
	{
		if (!long.TryParse(stack.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Use LOAN AUTOPAY <loan id> [FROM <account>|OFF|RETRY].");
			return;
		}
		if (stack.PeekSpeech().EqualTo("from")) stack.PopSpeech();
		var input = stack.SafeRemainingArgument;
		actor.OutputHandler.Send(input.EqualTo("retry") ? LoanService.RetryAutopay(actor, id) : LoanService.ConfigureAutopay(actor, id, input));
	}

	private static void PayLoan(ICharacter actor, StringStack stack)
	{
		if (!long.TryParse(stack.PopSpeech(), NumberStyles.None, CultureInfo.InvariantCulture, out var id))
		{
			actor.OutputHandler.Send("Which loan number do you want to pay?");
			return;
		}

		var remainder = stack.SafeRemainingArgument;
		string? account = null;
		var prefixed = " " + remainder;
		var fromIndex = prefixed.IndexOf(" from ", StringComparison.InvariantCultureIgnoreCase);
		if (fromIndex >= 0)
		{
			account = prefixed[(fromIndex + 6)..].Trim().Trim('"');
			remainder = prefixed[..fromIndex].Trim();
			if (account.Length == 0)
			{
				actor.OutputHandler.Send("Specify the account to pay from.");
				return;
			}
		}

		actor.OutputHandler.Send(LoanService.Pay(actor, id, remainder, account));
	}

	private static void CollectLoan(ICharacter actor, StringStack stack)
	{
		if (!long.TryParse(stack.PopSpeech(), NumberStyles.None, CultureInfo.InvariantCulture, out var id))
		{
			actor.OutputHandler.Send("Which loan number has an item or surplus to collect?");
			return;
		}

		if (!stack.IsFinished && (!stack.PopSpeech().EqualTo("into") || stack.IsFinished))
		{
			actor.OutputHandler.Send("Use LOAN COLLECT <id> [INTO <account>].");
			return;
		}
		actor.OutputHandler.Send(LoanService.CollectLoan(actor, id, stack.SafeRemainingArgument));
	}

	private static void LoanStatement(ICharacter actor, StringStack stack)
	{
		if (!long.TryParse(stack.PopSpeech(), NumberStyles.None, CultureInfo.InvariantCulture, out var id))
		{
			actor.OutputHandler.Send("Which loan number do you want a statement for?");
			return;
		}

		actor.OutputHandler.Send(LoanService.Show(actor, id, true));
	}

	private static void ManageLoans(ICharacter actor, StringStack stack)
	{
		var lender = ResolveManagementLender(actor, stack, out var error);
		if (lender is null)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!LoanService.IsManager(actor, lender))
		{
			actor.OutputHandler.Send("You are not authorised to manage loans for this lender.");
			return;
		}

		var action = stack.PopForSwitch();
		if (action == "product")
		{
			actor.OutputHandler.Send(ManageLoanProduct(actor, lender, stack));
			return;
		}

		if (action is "list" or "")
		{
			actor.OutputHandler.Send(LoanService.Manage(actor, lender, action, 0, stack));
			return;
		}

		if (action == "offer" && !long.TryParse(stack.PeekSpeech(), NumberStyles.None, CultureInfo.InvariantCulture, out _))
		{
			actor.OutputHandler.Send(DirectLoanOffer(actor, lender, stack));
			return;
		}

		if (!long.TryParse(stack.PopSpeech(), NumberStyles.None, CultureInfo.InvariantCulture, out var loanId))
		{
			actor.OutputHandler.Send("Which loan number do you want to manage?");
			return;
		}

		actor.OutputHandler.Send(LoanService.Manage(actor, lender, action, loanId, stack));
	}

	private static IFrameworkItem? ResolveManagementLender(ICharacter actor, StringStack stack, out string error)
	{
		var lenders = LoanService.LocalLenders(actor).ToList();
		var type = stack.PeekSpeech();
		if (type.EqualTo("bank") || type.EqualTo("shop"))
		{
			stack.PopSpeech();
			if (!long.TryParse(stack.PopSpeech(), NumberStyles.None, CultureInfo.InvariantCulture, out var lenderId))
			{
				error = "Use LOAN MANAGE BANK <id> <action> or LOAN MANAGE SHOP <id> <action>.";
				return null;
			}

			var lender = lenders.FirstOrDefault(x => x.Id == lenderId &&
				(x is IBank && type.EqualTo("bank") || x is IShop && type.EqualTo("shop")));
			if (lender is null)
			{
				error = "That lender is not at this location.";
				return null;
			}

			error = string.Empty;
			return lender;
		}

		if (lenders.Count == 1)
		{
			error = string.Empty;
			return lenders[0];
		}

		error = lenders.Count == 0
			? "There is no lender at this location."
			: "Several lenders serve this location. Begin with LOAN MANAGE BANK <id> or LOAN MANAGE SHOP <id>.";
		return null;
	}

	private static string DirectLoanOffer(ICharacter manager, IFrameworkItem lender, StringStack stack)
	{
		var borrowerName = stack.PopSpeech();
		var borrower = manager.TargetActor(borrowerName);
		if (borrower is null || borrower == manager)
		{
			return "Select a different borrower who is present at this lender.";
		}

		var productName = stack.PopSpeech();
		var remainder = stack.SafeRemainingArgument;
		var againstIndex = remainder.IndexOf(" against ", StringComparison.InvariantCultureIgnoreCase);
		var amountText = againstIndex >= 0 ? remainder[..againstIndex].Trim() : remainder.Trim();
		var asset = againstIndex >= 0 ? remainder[(againstIndex + 9)..].Trim().Trim('"') : string.Empty;
		if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(amountText))
		{
			return "Use LOAN MANAGE OFFER <present borrower> <product> <amount> [against <asset>].";
		}

		var products = LoanService.Products(lender)
			.Where(x =>
				long.TryParse(productName, NumberStyles.None, CultureInfo.InvariantCulture, out var productId)
					? x.Id == productId
					: x.Name.EqualTo(productName) || x.Name.Contains(productName, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (products.Count != 1)
		{
			return products.Count == 0 ? "That lender has no matching enabled product." : "That product name is ambiguous. Use its product ID.";
		}

		if (!TryParseCurrency(lender, amountText, out var amount, out var currencyError) || amount <= 0.0M)
		{
			return string.IsNullOrWhiteSpace(currencyError) ? "The direct offer must be positive." : currencyError;
		}
		if (LoanService.ProductTerms(products[0]).Kind.EqualTo("Mortgage"))
		{
			return "Purchase mortgages must be accepted at the conveyancer with LOAN BUY.";
		}

		var offer = LoanService.Request(borrower, lender, products[0], amount, asset, 0.0M, manager);
		if (offer.StartsWith("Loan #", StringComparison.Ordinal))
			borrower.OutputHandler.Send(offer);
		return offer;
	}

	private static string ManageLoanProduct(ICharacter actor, IFrameworkItem lender, StringStack stack)
	{
		var action = stack.PopForSwitch();
		if (string.IsNullOrEmpty(action) || action == "list")
		{
			var products = LoanService.Products(lender, includeDisabled: true).ToList();
			return products.Count == 0
				? "This lender has no products. Create one with LOAN MANAGE PRODUCT NEW <name> <unsecured|pawn|property|mortgage>."
				: string.Join("\n", products.Select(x =>
				{
					var definition = LoanService.ProductDefinition(x);
					return $"#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} — {definition.Terms.Kind}, {(x.Enabled ? "published" : "withdrawn")}, {(definition.Policy.AutomaticApproval ? "automatic" : "manager approval")}, unsecured cap {definition.Policy.MaximumUnsecuredAmount:N2}, secured cap {definition.Policy.MaximumSecuredAmount:N2}, minimum collateral {definition.Policy.MinimumCollateralValue:N2}, coverage {definition.Policy.MinimumCollateralCoverage:N2} to 1, funding {(definition.Terms.UseShopAccount ? "shop account" : "cash")}, liquidity floor {definition.Terms.LiquidityFloor:N2}, auction house {(definition.Terms.AuctionHouseId == 0 ? "none" : definition.Terms.AuctionHouseId.ToString("N0", actor))}";
				}));
		}

		if (action == "new")
		{
			var name = stack.PopSpeech();
			var kind = stack.PopSpeech();
			if (string.IsNullOrWhiteSpace(name) || name.Length > 200 || kind.ToLowerInvariant() is not ("unsecured" or "pawn" or "property" or "mortgage"))
			{
				return "Choose a product name and a type of unsecured, pawn, property or mortgage.";
			}

			var terms = new LoanTerms { Kind = char.ToUpperInvariant(kind[0]) + kind[1..].ToLowerInvariant() };
			var definition = new LendingProductDefinition
			{
				Terms = terms,
				Policy = new LendingProductPolicy()
			};
			using (new FMDB())
			{
				var duplicates = FMDB.Context.LendingProducts.Any(x => x.LenderId == lender.Id &&
					x.LenderType == lender.FrameworkItemType && x.Name == name);
				if (duplicates)
				{
					return "This lender already has a product with that name.";
				}

				var row = new LendingProduct
				{
					LenderId = lender.Id,
					LenderType = lender.FrameworkItemType,
					EconomicZoneId = LenderZone(lender).Id,
					Name = name,
					Definition = JsonSerializer.Serialize(definition),
					Enabled = false
				};
				FMDB.Context.LendingProducts.Add(row);
				FMDB.Context.SaveChanges();
				return $"Created draft lending product #{row.Id.ToString("N0", actor)} {name.ColourName()}. Set its terms and underwriting rules, then enable it.";
			}
		}

		if (action is "enable" or "disable")
		{
			if (!long.TryParse(stack.PopSpeech(), NumberStyles.None, CultureInfo.InvariantCulture, out var id))
			{
				return "Which product number do you want to enable or disable?";
			}

			using (new FMDB())
			{
				var row = FMDB.Context.LendingProducts.FirstOrDefault(x => x.Id == id && x.LenderId == lender.Id && x.LenderType == lender.FrameworkItemType);
				if (row is null)
				{
					return "There is no such product for this lender.";
				}

				var terms = LoanService.ProductTerms(row);
				if (action == "enable" && terms.Minimum == "Principal" && terms.Instalment <= 0)
					return "Set a positive principal instalment before publishing.";
				row.Enabled = action == "enable";
				if (!row.Enabled)
					foreach (var application in FMDB.Context.LendingContracts.Where(x => x.ProductId == row.Id &&
					             (x.Status == "Requested" || x.Status == "Offered")))
						application.Status = "Withdrawn";
				FMDB.Context.SaveChanges();
				return $"Product {row.Name.ColourName()} is now {(row.Enabled ? "published" : "withdrawn")}.";
			}
		}

		if (action is "edit" or "set")
		{
			if (!long.TryParse(stack.PopSpeech(), NumberStyles.None, CultureInfo.InvariantCulture, out var id))
			{
				return "Which product number do you want to edit?";
			}

			var editAction = stack.PopForSwitch();
			if (editAction == "set")
			{
				var field = stack.PopForSwitch();
				var value = stack.SafeRemainingArgument;
				return SetLoanProduct(actor, lender, id, field, value);
			}

			using (new FMDB())
			{
				var row = FMDB.Context.LendingProducts.FirstOrDefault(x => x.Id == id && x.LenderId == lender.Id && x.LenderType == lender.FrameworkItemType);
				if (row is null)
				{
					return "There is no such product for this lender.";
				}

				var definition = LoanService.ProductDefinition(row);
				return $"{row.Name.ColourName()} is {definition.Terms.Kind}, rate {definition.Terms.Rate.ToString("P2", actor)} per {definition.Terms.Interval}, {definition.Terms.Periods.ToString("N0", actor)} periods, minimum {definition.Terms.Minimum}. Hard caps are {definition.Policy.MaximumUnsecuredAmount:N2} unsecured and {definition.Policy.MaximumSecuredAmount:N2} secured. Set fields with LOAN MANAGE PRODUCT EDIT {id} SET <field> <value>.";
			}
		}

		return "Use LOAN MANAGE PRODUCT, NEW, EDIT, ENABLE or DISABLE.";
	}

	private static IEconomicZone LenderZone(IFrameworkItem lender) => lender switch
	{
		IBank bank => bank.EconomicZone,
		IShop shop => shop.EconomicZone,
		_ => throw new ArgumentException("Only banks and shops can lend.", nameof(lender))
	};

	private static string SetLoanProduct(ICharacter actor, IFrameworkItem lender, long id, string field, string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return "What value do you want to set?";
		}

		using (new FMDB())
		{
			var row = FMDB.Context.LendingProducts.FirstOrDefault(x => x.Id == id && x.LenderId == lender.Id && x.LenderType == lender.FrameworkItemType);
			if (row is null)
			{
				return "There is no such product for this lender.";
			}

			var definition = LoanService.ProductDefinition(row);
			var terms = definition.Terms;
			var policy = definition.Policy;
			var zone = LenderZone(lender);
			var amountField = field is "maxunsecured" or "maxsecured" or "exposureunsecured" or "exposuresecured" or "mincollateral" or "floor";
			decimal amount = 0M;
			if (amountField && (!zone.Currency.TryGetBaseCurrency(value, out amount) || amount < 0.0M))
			{
				return "Enter a non-negative amount in this lender's currency.";
			}

			switch (field)
			{
				case "kind":
					if (value.ToLowerInvariant() is not ("unsecured" or "pawn" or "property" or "mortgage"))
					{
						return "Product type must be unsecured, pawn, property or mortgage.";
					}
					terms.Kind = char.ToUpperInvariant(value[0]) + value[1..].ToLowerInvariant();
					break;
				case "rate":
					if (!TryParseRate(value, out var rate) || rate < 0.0M)
					{
						return "Enter a non-negative rate such as 5% or 0.05.";
					}
					terms.Rate = rate;
					break;
				case "interval":
					if (!RecurringInterval.TryParse(value, zone.FinancialPeriodReferenceCalendar, out var interval, out var intervalError))
					{
						return $"That is not a valid interval: {intervalError}";
					}
					terms.Interval = interval.ToString();
					break;
				case "periods":
					if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var periods) || periods < 1 || periods > 10000)
					{
						return "The term must be from 1 to 10,000 periods.";
					}
					terms.Periods = periods;
					break;
				case "grace":
					if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var graceDays) || graceDays < 0 || graceDays > 3650)
					{
						return "Grace must be from 0 to 3,650 days.";
					}
					terms.GraceDays = graceDays;
					break;
				case "instalment":
					if (!zone.Currency.TryGetBaseCurrency(value, out var instalment) || instalment < 0.0M)
					{
						return "Enter a non-negative principal instalment in this lender's currency.";
					}
					terms.Instalment = instalment;
					break;
				case "minimum":
					var minimum = value.ToLowerInvariant() switch
					{
						"none" => "None",
						"interest" => "Interest",
						"principal" => "Principal",
						_ => null
					};
					if (minimum is null)
					{
						return "Minimum payment must be none, interest or principal.";
					}
					terms.Minimum = minimum;
					break;
				case "recourse":
					if (!bool.TryParse(value, out var recourse))
					{
						return "Set recourse to true or false.";
					}
					terms.Recourse = recourse;
					break;
				case "automatic":
					if (!bool.TryParse(value, out var automatic))
					{
						return "Set automatic approval to true or false.";
					}
					policy.AutomaticApproval = automatic;
					break;
				case "funding":
					if (value.EqualTo("cash")) terms.UseShopAccount = false;
					else if (value.EqualTo("account") && lender is IShop shop && shop.BankAccount is not null) terms.UseShopAccount = true;
					else return "Funding must be cash, or account for a shop with a bank account.";
					break;
				case "floor": terms.LiquidityFloor = amount; break;
				case "auctionhouse":
					if (value.EqualTo("none")) terms.AuctionHouseId = 0;
					else
					{
						var auctionHouse = long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var auctionHouseId)
							? actor.Gameworld.AuctionHouses.Get(auctionHouseId)
							: actor.Gameworld.AuctionHouses.GetByName(value);
						if (auctionHouse is null || auctionHouse.EconomicZone != zone) return "Choose an auction house in the lender's economic zone, or none.";
						terms.AuctionHouseId = auctionHouse.Id;
					}
					break;
				case "maxunsecured": policy.MaximumUnsecuredAmount = amount; break;
				case "maxsecured": policy.MaximumSecuredAmount = amount; break;
				case "exposureunsecured": policy.MaximumUnsecuredExposure = amount; break;
				case "exposuresecured": policy.MaximumSecuredExposure = amount; break;
				case "mincollateral": policy.MinimumCollateralValue = amount; break;
				case "coverage":
					if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var coverage) || coverage < 1.0M || coverage > 100.0M)
					{
						return "Collateral coverage must be from 1 to 100. It is the required value for each unit advanced.";
					}
					policy.MinimumCollateralCoverage = coverage;
					break;
				case "unsecuredeligibility":
					if (!TryResolveProg(actor, value, ProgVariableTypes.Boolean, out var unsecuredEligibility, out var progError)) return progError;
					policy.UnsecuredEligibilityProgId = unsecuredEligibility?.Id;
					break;
				case "unsecuredmaximum":
					if (!TryResolveProg(actor, value, ProgVariableTypes.Number, out var unsecuredMaximum, out progError)) return progError;
					policy.UnsecuredMaximumAmountProgId = unsecuredMaximum?.Id;
					break;
				case "securedeligibility":
					if (!TryResolveProg(actor, value, ProgVariableTypes.Boolean, out var securedEligibility, out progError)) return progError;
					policy.SecuredEligibilityProgId = securedEligibility?.Id;
					break;
				case "securedmaximum":
					if (!TryResolveProg(actor, value, ProgVariableTypes.Number, out var securedMaximum, out progError)) return progError;
					policy.SecuredMaximumAmountProgId = securedMaximum?.Id;
					break;
				default:
					return "Unknown field. Use kind, rate, interval, periods, grace, instalment, minimum, recourse, automatic, funding, floor, auctionhouse, maxunsecured, maxsecured, exposureunsecured, exposuresecured, mincollateral, coverage or one of the secured/unsecured prog fields.";
			}

			row.Definition = JsonSerializer.Serialize(definition);
			FMDB.Context.SaveChanges();
			return $"Updated {row.Name.ColourName()} {field} to {value}.";
		}
	}

	private static bool TryResolveProg(ICharacter actor, string value, ProgVariableTypes returnType, out IFutureProg? prog, out string error)
	{
		prog = null;
		if (value.EqualTo("none"))
		{
			error = string.Empty;
			return true;
		}

		prog = long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var id)
			? actor.Gameworld.FutureProgs.Get(id)
			: actor.Gameworld.FutureProgs.GetByName(value);
		if (prog is null || !prog.Public && !actor.IsAdministrator())
		{
			error = "There is no FutureProg with that name or ID.";
			return false;
		}

		if (!LoanService.HasSignature(prog, returnType))
		{
			error = "The prog must accept (Character, Number, Number, Number) and return the required type.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	private static bool TryParseRate(string value, out decimal rate)
	{
		if (value.EndsWith('%') && decimal.TryParse(value[..^1], NumberStyles.Number, CultureInfo.InvariantCulture, out var percent))
		{
			rate = percent / 100.0M;
			return true;
		}

		return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out rate);
	}
}
