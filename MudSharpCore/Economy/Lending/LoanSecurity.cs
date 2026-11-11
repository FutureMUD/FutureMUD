#nullable enable

using MudSharp.Commands;
using System.Threading;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.GameItems.Prototypes;
using Estate = MudSharp.Economy.Estates.Estate;
using EstateClaim = MudSharp.Economy.Estates.EstateClaim;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.Economy.Property;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Economy.Auctions;
using MudSharp.Models;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{

	private static readonly AsyncLocal<HashSet<long>?> _authorisedSecurityTransfers = new();

	private static decimal PawnAppraisal(IGameItem item, IEconomicZone zone)
	{
		if (item.Prototype.Morphs || item.GetItemType<ICurrencyPile>() != null ||
		    item.GetItemType<IContainer>()?.Contents.Any() == true || item.GetItemType<ICommodity>() != null ||
		    item.Quantity > 1 ||
		    SpellOwnedItemValuePolicy.ContainsTemporaryValue(item) || zone.Currency.BaseCurrencyToGlobalBaseCurrencyConversion <= 0)
			return 0M;
		return item.Prototype.CostInBaseCurrency * Math.Clamp(item.Condition, 0.0, 1.0).ToAccountingDecimal() /
		       zone.Currency.BaseCurrencyToGlobalBaseCurrencyConversion;
	}

	private static bool HasBusiness(IProperty property) =>
		property.PropertyLocations.Any(x => x.Shop != null) || property.ExistingHotel != null ||
		property.PropertyHospitals.Any() || property.EconomicZone.Gameworld.Stables.Any(x => property.PropertyLocations.Contains(x.Location));

	private static bool CanPledgeProperty(IProperty property, bool purchase, out string error)
	{
		error = "";
		if (HasBusiness(property))
			error = "This property hosts a business and cannot be pledged.";
		else if ((!purchase && property.SaleOrder != null) || property.EconomicZone.Gameworld.AuctionHouses
		         .SelectMany(x => x.ActiveAuctionItems).Any(x => x.Asset == property))
			error = "Clear the other sale or auction listing before pledging this property.";
		else if (purchase && (property.Lease == null && property.PropertyKeys.Any(x => !x.IsReturned) ||
		                      property.PropertyOwners.Sum(x => x.ShareOfOwnership) != 1M ||
		                      property.PropertyOwners.Any(x => x.Owner == null || x.ShareOfOwnership <= 0M)))
			error = "The seller must clear the outstanding keys and have valid full title before closing.";
		return error.Length == 0;
	}

	internal static bool TrySecurity(ICharacter actor, IFrameworkItem lender, LoanTerms terms, string asset,
		out string error)
	{
		error = string.Empty;
		if (terms.Kind == "Pawn")
		{
			var item = long.TryParse(asset, out var itemId)
				? actor.Body.HeldItems.FirstOrDefault(x => x.Id == itemId)
				: actor.Body.HeldItems.GetFromItemListByKeyword(asset, actor);
			if (item is null)
			{
				error = "You must hold the item you want to pledge.";
				return false;
			}

			if (item.Owner != actor)
			{
				error = "The item must be registered as yours before you pledge it.";
				return false;
			}

			if (item.Quantity > 1 || item.GetItemType<ICommodity>() != null)
			{
				error = "Pledge a single item. Separate one item from the stack first. Bulk commodities cannot be pledged.";
				return false;
			}

			if (item.GetItemType<ICurrencyPile>() is not null || item.GetItemType<IContainer>()?.Contents.Any() == true)
			{
				error = "Currency and containers holding other items cannot be pledged.";
				return false;
			}

			terms.AssetId = item.Id;
			terms.CustodyStatus = "Held";
			terms.Appraisal = PawnAppraisal(item, ZoneOf(lender));

			if (terms.Appraisal <= 0)
			{
				error = "This lender requires a durable item with a positive prototype value and usable condition.";
				return false;
			}

			return true;
		}

		if (terms.Kind is not ("Property" or "Mortgage"))
		{
			return true;
		}

		var property = actor.Gameworld.Properties.FirstOrDefault(x =>
			asset.Equals(x.Id.ToString(), StringComparison.OrdinalIgnoreCase) ||
			x.Name.Equals(asset, StringComparison.OrdinalIgnoreCase));
		if (property is null || property.EconomicZone != ZoneOf(lender))
		{
			error = "That property is not in this lender's economic zone.";
			return false;
		}

		if (!CanPledgeProperty(property, terms.Kind == "Mortgage", out error)) return false;
		terms.AssetId = property.Id;
		terms.ExistingLeaseId = property.Lease?.Id;
		if (terms.Kind == "Mortgage")
		{
			if (property.SaleOrder?.ShowForSale != true)
			{
				error = "That property is no longer available for purchase.";
				return false;
			}

			terms.Price = Math.Truncate(property.SaleOrder.ReservePrice);
			terms.SaleOrderId = property.SaleOrder.Id;
			terms.Appraisal = Math.Min(property.LastSaleValue, terms.Price);
			terms.SellerShares = property.PropertyOwners
				.Select(x => new LoanSellerShare(x.Owner.Id, x.Owner.FrameworkItemType, x.ShareOfOwnership))
				.ToList();
			if (terms.Appraisal <= 0)
			{
				error = "The lender needs a positive property appraisal before offering a mortgage.";
				return false;
			}

			return true;
		}

		var propertyOwners = property.PropertyOwners.ToList();
		if (propertyOwners.Count != 1 || propertyOwners[0].Owner != actor || propertyOwners[0].ShareOfOwnership != 1M)
		{
			error = "Only a property you own outright can secure this loan.";
			return false;
		}

		terms.Appraisal = property.LastSaleValue;
		if (terms.Appraisal <= 0)
		{
			error = "The lender needs a positive property appraisal before offering this loan.";
			return false;
		}

		return true;
	}

	internal static bool CanSecure(ICharacter actor, IFrameworkItem lender, LoanTerms terms, out string error)
	{
		error = string.Empty;
		var zone = ZoneOf(lender);
		var atLender = lender switch
		{
			IBank bank => bank.BranchLocations.Contains(actor.Location),
			IShop shop => shop.CurrentLocations.Contains(actor.Location),
			_ => false
		};
		if (terms.Kind != "Mortgage" && !atLender)
		{
			error = "Visit the lending counter to pledge this asset.";
			return false;
		}
		if (terms.Kind == "Unsecured") return true;

		if (terms.Kind == "Pawn")
		{
			var item = actor.Gameworld.TryGetItem(terms.AssetId);
			if (item is null || !actor.Body.HeldItems.Contains(item) || item.Owner != actor)
			{
				error = "You must still hold and own the pledged item.";
				return false;
			}
			if (PawnAppraisal(item, zone) < terms.Appraisal)
			{
				error = "The item's condition, value or pledge eligibility changed. Request a new offer.";
				return false;
			}
		}
		else if (terms.Kind is "Property" or "Mortgage")
		{
			var property = actor.Gameworld.Properties.Get(terms.AssetId);
			if (property is null || property.EconomicZone != zone)
			{
				error = "The pledged property is unavailable or has moved to another economic zone.";
				return false;
			}

			if (!CanPledgeProperty(property, terms.Kind == "Mortgage", out error)) return false;
			if (property.Lease?.Id != terms.ExistingLeaseId)
			{
				error = "The tenancy changed. Request updated loan terms.";
				return false;
			}
			if (property.LastSaleValue < terms.Appraisal)
			{
				error = "The property's valuation fell. Request a new offer.";
				return false;
			}
			if (terms.Kind == "Mortgage")
			{
				if (property.SaleOrder?.ShowForSale != true || property.SaleOrder.Id != terms.SaleOrderId ||
				    Math.Truncate(property.SaleOrder.ReservePrice) != terms.Price)
				{
					error = "The property terms have changed. Request a new mortgage offer.";
					return false;
				}

				if (property.PropertyOwners.Any(x => x.Owner == actor))
				{
					error = "You already own this property. Request a secured loan against it instead.";
					return false;
				}

				var currentSellers = property.PropertyOwners
					.OrderBy(x => x.Owner.FrameworkItemType, StringComparer.Ordinal)
					.ThenBy(x => x.Owner.Id)
					.Select(x => new LoanSellerShare(x.Owner.Id, x.Owner.FrameworkItemType, x.ShareOfOwnership))
					.ToList();
				var agreedSellers = terms.SellerShares
					.OrderBy(x => x.OwnerType, StringComparer.Ordinal)
					.ThenBy(x => x.OwnerId)
					.ToList();
				if (!currentSellers.SequenceEqual(agreedSellers))
				{
					error = "The property owners or their shares changed. Request an updated mortgage offer.";
					return false;
				}
			}
			else
			{
				var owners = property.PropertyOwners.ToList();
				if (owners.Count != 1 || owners[0].Owner != actor || owners[0].ShareOfOwnership != 1M)
				{
					error = "You must still own the whole property to pledge it.";
					return false;
				}
			}

			if (FMDB.Context.LendingContracts.Any(x =>
				    x.SecurityKey == $"Property:{property.Id}" && x.Id != 0 &&
				    (x.Status == "Active" || x.Status == "Recovery")))
			{
				error = "That property already secures an outstanding loan.";
				return false;
			}
		}

		if (terms.Kind is not ("Pawn" or "Property" or "Mortgage"))
		{
			error = "This product does not use collateral.";
			return false;
		}

		if (FMDB.Context.LendingContracts.Any(x =>
			    x.SecurityKey == $"{(terms.Kind == "Pawn" ? "Item" : "Property")}:{terms.AssetId}" &&
			    (x.Status == "Active" || x.Status == "Recovery")))
		{
			error = "That asset already secures an outstanding loan.";
			return false;
		}

		return true;
	}

	internal static void Secure(ICharacter actor, IFrameworkItem lender, LendingContract contract, LoanTerms terms, bool contributionPaid = false)
	{
		if (terms.Kind == "Pawn")
		{
			var item = actor.Gameworld.TryGetItem(terms.AssetId, true) ??
			           throw new InvalidOperationException("The pledged item no longer exists.");
			if (GuardingSettlement && item is MudSharp.GameItems.GameItem native && actor.Body is MudSharp.Body.Implementations.Body body)
			{
				body.TakePledgedItem(native);
				native.SetLendingPosition(null);
				native.SetLendingOwner(lender);
			}
			else
			{
				actor.Body.Take(item);
				item.SetOwner(lender);
			}

			return;
		}

		if (terms.Kind == "Property")
		{
			return;
		}

		if (terms.Kind != "Mortgage")
		{
			return;
		}

		var property = actor.Gameworld.Properties.Get(terms.AssetId) ??
		               throw new InvalidOperationException("The mortgaged property no longer exists.");
		var currency = property.EconomicZone.Currency;
		if (!contributionPaid && terms.DownPayment > 0 && !CanTender(actor, currency, terms.DownPayment))
		{
			throw new InvalidOperationException("The borrower no longer has the required down payment.");
		}

		if (!contributionPaid && terms.DownPayment > 0)
		{
			TakeSettlementCash(actor, currency, terms.DownPayment);
		}

		foreach (var owner in property.PropertyOwners)
		{
			var proceeds = terms.Price * owner.ShareOfOwnership;
			var reason = $"Sale of {property.Name} financed by loan #{contract.Id}";
			var account = owner.RevenueAccount is { AccountStatus: BankAccountStatus.Active } revenue && revenue.Currency == currency ? revenue : null;
			if (account == null && owner.Owner is IBank sellingBank)
			{
				AdjustReserves(sellingBank, currency, proceeds);
			}
			else if (account == null && owner.Owner is IShop sellingShop && sellingShop.Currency == currency)
			{
				Credit(sellingShop, proceeds, reason);
			}
			else
			{
				VirtualCashLedger.CreditBankOrVirtual(owner.Owner, currency, proceeds, actor, property, "PropertyRevenue",
					reason, account, property.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime);
			}
		}

		property.SellProperty(actor);
		actor.Gameworld.EconomyAnalytics?.RecordActivity(new EconomicActivityEvent(EconomicActivityType.Property,
			EconomicVolumeClassification.Exchange, currency.Id, terms.Price, property.EconomicZone.Id,
			CharacterInstanceIdentityComparer.IdentityId(actor), "Character", ReferenceId: property.Id,
			ReferenceType: property.FrameworkItemType, ReferenceText: $"Mortgage purchase, loan #{contract.Id}"));
	}

	internal static void Release(IFuturemud world, LendingContract contract, LoanTerms terms)
	{
		if (terms.Kind == "Pawn")
		{
			if (terms.CustodyStatus != "Held")
			{
				contract.SecurityKey = null;
				return;
			}

			var item = world.TryGetItem(terms.AssetId, true);
			var borrower = world.TryGetCharacter(contract.BorrowerId, true);
			if (item is null || borrower is null)
			{
				return;
			}

			if (GuardingSettlement)
			{
				var row = FMDB.Context.GameItems.Find(item.Id) ?? throw new InvalidOperationException("The pledged item is missing.");
				row.OwnerId = CharacterInstanceIdentityComparer.IdentityId(borrower);
				row.OwnerType = borrower.FrameworkItemType;
				AfterCommit(() => { item.SetOwner(borrower); item.Login(); });
			}
			else
			{
				item.SetOwner(borrower);
				item.Login();
			}
			terms.CustodyStatus = "Ready";
		}

		contract.SecurityKey = null;
	}

	internal static string Collect(ICharacter actor, LendingContract contract, LoanTerms terms, IBankAccount? destination)
	{
		if (contract.Surplus > 0)
		{
			var amount = contract.Surplus;
			var lender = Lender(actor.Gameworld, contract);
			if (lender is null)
			{
				return "The lending establishment is unavailable. Your surplus remains held.";
			}

			var currency = ZoneOf(lender).Currency;
			var coins = currency.FindCoinsForAmount(amount, out var exact);
			if (!exact && destination is null)
			{
				return $"The surplus cannot be paid in exact coins. Use LOAN COLLECT {contract.Id} INTO <account>.";
			}


			contract.Surplus = 0;
			if (destination != null)
			{
				destination.DoAccountCredit(amount, $"Surplus on loan #{contract.Id}");
				AdjustReserves(destination.Bank, currency, amount);
			}
			else GiveCash(actor, currency, coins);
			Post(contract, "SurplusDistribution", 0, 0, amount, destination?.AccountReference ?? "Cash",
				ZoneOf(lender).FinancialPeriodReferenceCalendar.CurrentDateTime, CharacterInstanceIdentityComparer.IdentityId(actor));
			return $"You collect {currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} in loan surplus.";
		}

		if (terms.Kind != "Pawn" || contract.Status is not ("Paid" or "Closed"))
		{
			return "There is no pledged item ready for collection on this loan.";
		}

		if (terms.CustodyStatus != "Ready")
		{
			return "The pledged item has already been returned or sold.";
		}

		var item = actor.Gameworld.TryGetItem(terms.AssetId, true);
		if (item is null || !item.Owner.FrameworkItemEquals(contract.BorrowerId, "Character") ||
		    item.Location is not null || item.ContainedIn is not null || item.InInventoryOf is not null)
		{
			return "The pledged item is not available for collection. Ask the lender to review the loan.";
		}

		if (GuardingSettlement && item is MudSharp.GameItems.GameItem native)
		{
			native.SetLendingOwner(actor);
			PrepareItemCollection(actor, native);
			terms.CustodyStatus = "Returned";
		}
		else
		{
			item.SetOwner(actor);
			if (actor.Body.CanGet(item, 0)) actor.Body.Get(item, silent: true);
			else item.InsertAtSource(actor, true);
			terms.CustodyStatus = "Returned";
		}

		contract.Status = "Closed";
		return $"You collect {item.HowSeen(actor)} from the lender.";
	}

	internal static string Enforce(ICharacter actor, LendingContract contract, LoanTerms terms)
	{
		var relisting = contract.Status == "Recovery";
		if ((!relisting && contract.Status != "Active") || contract.Principal + contract.Interest <= 0)
		{
			return "This loan is not eligible for collateral recovery.";
		}

		var lender = Lender(actor.Gameworld, contract);
		if (lender is null || lender is IBank bank && !bank.IsManager(actor) ||
		    lender is IShop shop && !shop.IsManager(actor) && !shop.IsProprietor(actor))
		{
			return "Only a manager of the lending establishment can recover collateral.";
		}

		if (terms.Kind is "Property" or "Mortgage" && actor.Gameworld.Properties.Get(terms.AssetId) is { } pledged && HasBusiness(pledged))
			return "This property now hosts a business. Resolve its business use before foreclosure.";
		if (terms.Kind is "Pawn" or "Property" or "Mortgage")
		{
			if (terms.AuctionHouseId <= 0)
				return RetainCollateral(actor, lender, contract, terms, relisting);
			if (actor.Gameworld.AuctionHouses.Get(terms.AuctionHouseId) is not { } house ||
			    house.EconomicZone != ZoneOf(lender))
			{
				return "The loan has no compatible auction house configured for recovery.";
			}

			var item = terms.Kind == "Pawn" ? actor.Gameworld.TryGetItem(terms.AssetId, true) : null;
			var property = terms.Kind == "Pawn" ? null : actor.Gameworld.Properties.Get(terms.AssetId);
			var borrower = actor.Gameworld.TryGetCharacter(contract.BorrowerId, true);
			if (terms.Kind == "Pawn" && (item is null || item.Owner != lender))
				return "The pledged item is missing from lender custody.";
			if (terms.Kind == "Pawn" && terms.CustodyStatus != "Held")
				return "This pledged item is no longer in recoverable lender custody.";
			if (terms.Kind != "Pawn" && (property is null || property.PropertyOwners.Count() != 1 ||
			    property.PropertyOwners.First().Owner != (relisting ? lender : borrower)))
				return "The pledged property is no longer owned by the borrower or lender.";
			if (house.ActiveAuctionItems.Any(x => x.RecoveryContractId == contract.Id))
				return "This loan already has collateral listed at auction.";

			var asset = (IFrameworkItem?)item ?? property!;
			if (terms.Kind != "Pawn" && !relisting)
			{
				contract.Status = "Recovery";
				using (AuthorizeSecurityTransfer(property!.Id))
				{
					property.TransferProperty(lender);
				}
			}
			house.AddAuctionItem(new AuctionItem
			{
				Asset = asset,
				Seller = lender,
				PayoutTarget = lender,
				RecoveryContractId = contract.Id,
				ListingDateTime = ZoneOf(lender).FinancialPeriodReferenceCalendar.CurrentDateTime,
				FinishingDateTime = new MudDateTime(ZoneOf(lender).FinancialPeriodReferenceCalendar.CurrentDateTime) +
					house.DefaultListingTime,
				PropertyShare = 1M,
				MinimumPrice = Math.Max(0M, terms.Appraisal * 0.5M),
				BuyoutPrice = null
			});
			contract.Status = "Recovery";
			Post(contract, "Repossession", 0, 0, contract.Principal + contract.Interest,
				"Collateral taken into auction recovery", ZoneOf(lender).FinancialPeriodReferenceCalendar.CurrentDateTime,
				CharacterInstanceIdentityComparer.IdentityId(actor), $"repossession:{contract.Id}:{DateTime.UtcNow.Ticks}");
			return $"The collateral is now listed at {house.Name}. You can view the listing and any surplus from loan #{contract.Id}.";
		}

		return "Property foreclosure requires an auction house configured for the lender's economic zone.";
	}

	private static string RetainCollateral(ICharacter actor, IFrameworkItem lender, LendingContract contract,
		LoanTerms terms, bool relisting)
	{
		var equity = Math.Max(0M, terms.Appraisal - contract.Principal - contract.Interest);
		if (Available(lender) < equity)
			return "The lender cannot fund the collateral's borrower equity, so it cannot retain this security.";
		if (terms.Kind == "Pawn")
		{
			var item = actor.Gameworld.TryGetItem(terms.AssetId, true);
			if (item is null || item.Owner != lender || terms.CustodyStatus != "Held")
				return "The pledged item is no longer in recoverable lender custody.";
			if (!actor.Body.CanGet(item, 0))
				return "You cannot take the retained pawn item into physical custody. Configure an auction or arrange stock space first.";
			terms.CustodyStatus = "Retained";
		}
		else
		{
			var property = actor.Gameworld.Properties.Get(terms.AssetId);
			var expectedOwner = relisting ? lender : actor.Gameworld.TryGetCharacter(contract.BorrowerId, true);
			if (property is null || property.PropertyOwners.Count() != 1 || property.PropertyOwners.Single().Owner != expectedOwner)
				return "The pledged property is no longer owned by the borrower or lender.";
			if (!relisting)
			{
				using (AuthorizeSecurityTransfer(property.Id)) property.TransferProperty(lender);
			}
		}

		var now = ZoneOf(lender).FinancialPeriodReferenceCalendar.CurrentDateTime;
		if (equity > 0)
		{
			Credit(lender, -equity, $"Borrower equity held for retained collateral on loan #{contract.Id}");
			contract.Surplus += equity;
		}
		var recovery = Math.Min(terms.Appraisal, contract.Principal + contract.Interest);
		if (recovery > 0)
		{
			var (principal, interest) = ApplyPayment(contract, terms, recovery);
			Post(contract, "CollateralRecovery", principal, interest, recovery,
				$"Collateral retained at its recorded appraisal for loan #{contract.Id}", now,
				CharacterInstanceIdentityComparer.IdentityId(actor),
				$"retention:{contract.Id}");
			if (terms.Kind == "Pawn")
			{
				AfterCommit(() =>
				{
					RegisterRetainedPawn(actor.Gameworld, contract, terms, interest);
					actor.Body.Get(actor.Gameworld.TryGetItem(terms.AssetId, true), silent: true);
				});
			}
		}
		contract.SecurityKey = null;
		if (contract.Principal + contract.Interest <= 0)
		{
			contract.Principal = contract.Interest = 0;
			contract.Status = "Closed";
		}
		else if (!terms.Recourse)
		{
			Post(contract, "WriteOff", contract.Principal - terms.RolledInterest, contract.Interest + terms.RolledInterest,
				contract.Principal + contract.Interest, "Non-recourse collateral retention shortfall", now,
				operationKey: $"retention:{contract.Id}:writeoff");
			contract.Principal = contract.Interest = 0;
			contract.Status = "Closed";
		}
		else
		{
			contract.Status = "Active";
			PauseRecoveryDeficiency(contract, terms, now);
		}
		Store(contract, terms);
		return $"The lender retains the collateral against loan #{contract.Id}. " +
		       (contract.Status == "Active" ? "The remaining recourse balance is now unsecured." : "The loan is settled.");
	}

	internal static bool SettleRecoveryAuction(IAuctionHouse house, AuctionItem item, long contractId,
		AuctionBid bid, decimal netProceeds, MudDateTime now)
	{
		using (new FMDB())
		{
			var contract = FMDB.Context.LendingContracts.Find(contractId);
			if (contract is null || contract.Status != "Recovery" || contract.EstateId.HasValue || Terms(contract).AuctionHouseId != house.Id ||
			    Terms(contract).AssetId != item.Asset.Id || netProceeds < 0)
			{
				return false;
			}

			var terms = Terms(contract);
			var lender = Lender(house.Gameworld, contract);
			if (lender is null) return false;
			var debt = contract.Principal + contract.Interest;
			var payment = Math.Min(netProceeds, debt);
			if (netProceeds > 0 && !VirtualCashLedger.Debit(house, ZoneOf(lender).Currency, netProceeds,
				    bid.Bidder, lender, "LoanRecovery",
				    $"Cleared auction proceeds for loan #{contract.Id}", house.ProfitsBankAccount, now, out _))
			{
				return false;
			}

			var (principal, interest) = payment > 0
				? ApplyPayment(contract, terms, payment)
				: (0M, 0M);
			if (payment > 0)
			{
				Credit(lender, payment, $"Recovery proceeds for loan #{contract.Id}");
				Post(contract, "RecoveryPayment", principal, interest, payment,
					$"Auction recovery for {item.Asset.Name}", now, bid.BidderId,
					$"recovery:{house.Id}:{contract.Id}:{item.Asset.Id}:{item.ListingDateTime.GetDateTimeString()}");
			}

			contract.Surplus += Math.Max(0M, netProceeds - payment);
			terms.CustodyStatus = "Auctioned";
			contract.SecurityKey = null;
			if (contract.Principal + contract.Interest <= 0)
			{
				contract.Principal = 0;
				contract.Interest = 0;
				contract.Status = "Closed";
			}
			else if (!terms.Recourse)
			{
				var writeOffPrincipal = contract.Principal - terms.RolledInterest;
				var writeOffInterest = contract.Interest + terms.RolledInterest;
				Post(contract, "WriteOff", writeOffPrincipal, writeOffInterest,
					writeOffPrincipal + writeOffInterest, "Non-recourse collateral recovery shortfall", now,
					operationKey: $"recovery:{house.Id}:{contract.Id}:{item.Asset.Id}:{item.ListingDateTime.GetDateTimeString()}:writeoff");
				contract.Principal = 0;
				contract.Interest = 0;
				contract.Status = "Closed";
			}
			else
			{
				contract.Status = "Active";
				PauseRecoveryDeficiency(contract, terms, now);
			}

			Store(contract, terms);
			FMDB.Context.SaveChanges();
			return true;
		}
	}

	private static void PauseRecoveryDeficiency(LendingContract contract, LoanTerms terms, MudDateTime now)
	{
		terms.Bills.Clear();
		terms.Bills.Add(new LoanInstalment
		{
			Due = now.GetDateTimeString(),
			Principal = contract.Principal,
			Interest = contract.Interest
		});
		terms.PeriodStart = now.GetDateTimeString();
		terms.LastAccrual = terms.PeriodStart;
		terms.NextDue = terms.PeriodStart;
		terms.PeriodsBilled = terms.Periods;
		terms.FractionalInterest = 0;
		terms.InterestPausedForRecovery = true;
	}

	internal static bool HasActivePropertyLien(IProperty property)
	{
		if (!HasPortfolio(property.Gameworld)) return false;
		using (new FMDB())
		{
			return FMDB.Context.LendingContracts.Any(x =>
				x.SecurityKey == $"Property:{property.Id}" && (x.Status == "Active" || x.Status == "Recovery"));
		}
	}

	internal static void CreateEstateClaims(ICharacter character, Func<IEconomicZone, Estate> getEstate, long? zoneId = null)
	{
		if (!HasPortfolio(character.Gameworld)) return;
		var borrowerId = CharacterInstanceIdentityComparer.IdentityId(character);
		using (new FMDB())
		{
			var loans = FMDB.Context.LendingContracts
				.Where(x => (!zoneId.HasValue || x.EconomicZoneId == zoneId.Value) && x.BorrowerId == borrowerId && (x.Status == "Active" || x.Status == "Recovery") && !x.EstateId.HasValue)
				.ToList();
			foreach (var loan in loans)
			{
				if (loan.Status == "Recovery" && HasRecoveryAuction(character.Gameworld, loan))
				{
					var recoveryZone = character.Gameworld.EconomicZones.Get(loan.EconomicZoneId);
					if (recoveryZone != null) getEstate(recoveryZone);
					continue;
				}
				var zone = character.Gameworld.EconomicZones.Get(loan.EconomicZoneId);
				if (zone is null) continue;
				var accrualTerms = Terms(loan);
				Accrue(loan, accrualTerms, zone.FinancialPeriodReferenceCalendar.CurrentDateTime);
				var lender = Lender(character.Gameworld, loan);
				if (lender is null) continue;
				var estate = getEstate(zone);
				if (estate is null) continue;
				var terms = Terms(loan);
				var securedProperty = loan.SecurityKey?.StartsWith("Property:", StringComparison.Ordinal) == true
					? character.Gameworld.Properties.Get(terms.AssetId)
					: null;
				IFrameworkItem? target = securedProperty?.PropertyOwners.Any(x => x.Owner is ICharacter owner &&
					CharacterInstanceIdentityComparer.SameIdentity(owner, character)) == true
					? securedProperty
					: null;
				if (loan.Status == "Recovery" && securedProperty != null &&
				    securedProperty.PropertyOwners.Count() == 1 && securedProperty.PropertyOwners.Single().Owner == lender)
				{
					using (AuthorizeSecurityTransfer(securedProperty.Id)) securedProperty.TransferProperty(estate);
					if (!estate.Assets.Any(x => x.Asset.FrameworkItemEquals(securedProperty.Id, securedProperty.FrameworkItemType)))
						estate.AddAsset(new MudSharp.Economy.Estates.EstateAsset(estate, securedProperty, false));
					terms.CustodyStatus = "Estate";
					target = securedProperty;
					loan.Status = "Active";
				}
				if (loan.SecurityKey != null && terms.Kind == "Pawn" && terms.CustodyStatus == "Held")
				{
					var item = character.Gameworld.TryGetItem(terms.AssetId, true);
					if (item != null && item.Owner == lender)
					{
						RegisterEstatePawn(estate, item);
						terms.CustodyStatus = "Estate";
						target = item;
					}
				}
				var reason = $"LendingContract:{loan.Id}";
				var existing = terms.LoanEstateClaimId.HasValue
					? estate.Claims.FirstOrDefault(x => x.Id == terms.LoanEstateClaimId.Value)
					: null;
				if (existing is not null)
				{
					terms.LoanEstateClaimId = existing.Id;
					loan.EstateId = estate.Id;
					Store(loan, terms);
					continue;
				}
				var amount = loan.Principal + loan.Interest;
				if (amount <= 0) continue;
				var claim = new EstateClaim(estate, lender, amount, reason, ClaimStatus.Approved,
					loan.SecurityKey is not null, target);
				estate.AddClaim(claim);
				terms.LoanEstateClaimId = claim.Id;
				loan.EstateId = estate.Id;
				Store(loan, terms);
			}
			FMDB.Context.SaveChanges();

			var returnedPawns = FMDB.Context.LendingContracts
				.Where(x => (!zoneId.HasValue || x.EconomicZoneId == zoneId.Value) && x.BorrowerId == borrowerId && !x.EstateId.HasValue &&
					(x.Status == "Paid" || x.Status == "Closed"))
				.ToList();
			foreach (var loan in returnedPawns)
			{
				var terms = Terms(loan);
				if (terms.Kind != "Pawn" || terms.CustodyStatus is not ("Held" or "Ready")) continue;
				var zone = character.Gameworld.EconomicZones.Get(loan.EconomicZoneId);
				if (zone is null) continue;
				var estate = getEstate(zone);
				var item = character.Gameworld.TryGetItem(terms.AssetId, true);
				var lender = Lender(character.Gameworld, loan);
				if (estate is null || item is null || (item.Owner != character && item.Owner != lender)) continue;
				RegisterEstatePawn(estate, item);
				terms.CustodyStatus = "Estate";
				loan.EstateId = estate.Id;
				Store(loan, terms);
			}
			var surplusLoans = FMDB.Context.LendingContracts
				.Where(x => (!zoneId.HasValue || x.EconomicZoneId == zoneId.Value) && x.BorrowerId == borrowerId && !x.EstateId.HasValue && x.Surplus > 0)
				.ToList();
			foreach (var loan in surplusLoans)
			{
				var zone = character.Gameworld.EconomicZones.Get(loan.EconomicZoneId);
				if (zone is null) continue;
				var estate = getEstate(zone);
				if (estate is null) continue;
				loan.EstateId = estate.Id;
			}
			FMDB.Context.SaveChanges();
		}
	}

	private static void RegisterEstatePawn(Estate estate, IGameItem item)
	{
		item.InInventoryOf?.Take(item);
		item.ContainedIn?.Take(item);
		item.Location?.Extract(item);
		item.SetOwner(estate);
		if (!estate.Assets.Any(x => x.Asset.FrameworkItemEquals(item.Id, item.FrameworkItemType)))
			estate.AddAsset(new MudSharp.Economy.Estates.EstateAsset(estate, item, false));
	}

	internal static bool IsEstateLoanClaim(IEstateClaim claim)
	{
		if (!claim.Reason.StartsWith("LendingContract:", StringComparison.Ordinal) ||
		    !long.TryParse(claim.Reason["LendingContract:".Length..], out var id)) return false;
		var world = claim.Estate.EconomicZone.Gameworld;
		if (!HasPortfolio(world)) return false;
		using (new FMDB())
		{
			var loan = FMDB.Context.LendingContracts.Find(id);
			if (loan is null || loan.EstateId != claim.Estate.Id ||
			    loan.BorrowerId != CharacterInstanceIdentityComparer.IdentityId(claim.Estate.Character)) return false;
			var terms = Terms(loan);
			var lender = Lender(world, loan);
			return terms.LoanEstateClaimId == claim.Id && lender is not null &&
			       claim.Claimant.FrameworkItemEquals(lender.Id, lender.FrameworkItemType);
		}
	}

	internal static bool IsSecuredEstateLoanClaim(IEstateClaim claim) =>
		claim.IsSecured && claim.TargetItem is IProperty or IGameItem && IsEstateLoanClaim(claim);

	internal static bool CanSettleEstateClaimFromGeneralPool(IEstateClaim claim)
	{
		if (!IsEstateLoanClaim(claim) || !long.TryParse(claim.Reason["LendingContract:".Length..], out var id)) return true;
		using (new FMDB())
		{
			var loan = FMDB.Context.LendingContracts.Find(id);
			return loan is not null && Terms(loan).Recourse;
		}
	}

	internal static void TransferPropertyIntoEstate(IEstate estate, IProperty property)
	{
		if (!HasActivePropertyLien(property))
		{
			property.TransferProperty(estate);
			return;
		}

		if (!HasVerifiedEstatePropertyClaim(estate, property))
			throw new InvalidOperationException("This estate cannot transfer property secured by an unclaimed loan.");
		using (AuthorizeSecurityTransfer(property.Id)) property.TransferProperty(estate);
	}

	internal static IDisposable AuthorizeEstatePropertyAuctionTransfer(IEstate estate, AuctionItem item,
		IFrameworkItem buyer)
	{
		if (item.Seller != estate || item.Asset is not IProperty property || item.PropertyShare != 1M ||
		    buyer == estate || property.PropertyOwners.Count() != 1 || property.PropertyOwners.First().Owner != estate ||
		    !estate.Assets.Any(x => x.Asset.FrameworkItemEquals(property.Id, property.FrameworkItemType)) ||
		    !HasVerifiedEstatePropertyClaim(estate, property))
			throw new InvalidOperationException("This auction is not authorised to transfer pledged estate property.");
		return AuthorizeSecurityTransfer(property.Id);
	}

	private static bool HasVerifiedEstatePropertyClaim(IEstate estate, IProperty property)
	{
		var world = estate.EconomicZone.Gameworld;
		if (!HasPortfolio(world)) return false;
		using (new FMDB())
		{
			var contracts = FMDB.Context.LendingContracts
				.Where(x => x.SecurityKey == $"Property:{property.Id}" && x.EstateId == estate.Id &&
					(x.Status == "Active" || x.Status == "Recovery"))
				.ToList();
			return contracts.Any(contract =>
			{
				var terms = Terms(contract);
				var lender = Lender(world, contract);
				if (lender is null) return false;
				return terms.LoanEstateClaimId.HasValue && estate.Claims.Any(claim =>
					claim.Id == terms.LoanEstateClaimId.Value && claim.IsSecured && claim.TargetItem != null &&
					claim.TargetItem.FrameworkItemEquals(property.Id, property.FrameworkItemType) &&
					claim.Claimant.FrameworkItemEquals(CharacterInstanceIdentityComparer.FrameworkItemId(lender),
						lender.FrameworkItemType));
			});
		}
	}

	internal static decimal SettleEstateClaim(IEstate estate, IEstateClaim claim, decimal payout,
		string operationStage = "general")
	{
		var world = estate.EconomicZone.Gameworld;
		if (!HasPortfolio(world) || !IsEstateLoanClaim(claim) || payout <= 0) return 0M;
		if (!long.TryParse(claim.Reason["LendingContract:".Length..], out var id)) return 0M;
		using (new FMDB())
		{
			var loan = FMDB.Context.LendingContracts.Find(id);
			if (loan is null || loan.EstateId != estate.Id || loan.Status is not ("Active" or "Recovery")) return 0M;
			var lender = Lender(world, loan);
			if (lender is null) return 0M;
			var terms = Terms(loan);
			if (terms.LoanEstateClaimId != claim.Id ||
			    loan.BorrowerId != CharacterInstanceIdentityComparer.IdentityId(estate.Character) ||
			    !claim.Claimant.FrameworkItemEquals(lender.Id, lender.FrameworkItemType)) return 0M;
			Accrue(loan, terms, estate.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime);
			var amount = Math.Min(payout, loan.Principal + loan.Interest);
			if (amount <= 0) return 0M;
			var (principal, interest) = ApplyPayment(loan, terms, amount);
			Credit(lender, amount, $"Estate payment for loan #{loan.Id}");
			Post(loan, "EstatePayment", principal, interest, amount,
				$"Estate claim #{claim.Id} for estate #{estate.Id}",
				estate.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime,
				operationKey: $"estate:{estate.Id}:claim:{claim.Id}:{operationStage}");
			if (loan.Principal + loan.Interest <= 0)
			{
				loan.Principal = 0;
				loan.Interest = 0;
				loan.Status = "Closed";
				loan.SecurityKey = null;
			}
			claim.Amount = Math.Max(0, claim.Amount - amount);
			if (claim.Amount <= 0) claim.Status = ClaimStatus.Approved;
			Store(loan, terms);
			FMDB.Context.SaveChanges();
			return amount;
		}
	}

	internal static void FinaliseEstateClaims(IEstate estate)
	{
		var world = estate.EconomicZone.Gameworld;
		if (!HasPortfolio(world)) return;
		using (new FMDB())
		{
			var loans = FMDB.Context.LendingContracts.Where(x => x.EstateId == estate.Id &&
				(x.Status == "Active" || x.Status == "Recovery")).ToList();
			foreach (var loan in loans)
			{
				var terms = Terms(loan);
				if (loan.SecurityKey != null && terms.CustodyStatus != "Estate" &&
				    (loan.Status == "Recovery" || terms.Kind == "Pawn" && terms.CustodyStatus == "Held"))
				{
					loan.Status = "Recovery";
					loan.EstateId = null;
					Store(loan, terms);
					continue;
				}
				var lossPrincipal = loan.Principal - terms.RolledInterest;
				var lossInterest = loan.Interest + terms.RolledInterest;
				if (lossPrincipal + lossInterest > 0)
				{
					var lender = Lender(world, loan);
					if (lender is not null)
						Post(loan, "WriteOff", lossPrincipal, lossInterest, lossPrincipal + lossInterest,
							$"Unpaid balance after estate #{estate.Id} was finalised",
							estate.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime,
							operationKey: $"estate:{estate.Id}:finalise:{loan.Id}");
				}
				loan.Principal = loan.Interest = 0;
				loan.SecurityKey = null;
				loan.Status = "Closed";
				loan.EstateId = null;
				Store(loan, terms);
			}
			var closedLoans = FMDB.Context.LendingContracts
				.Where(x => x.EstateId == estate.Id && (x.Status == "Paid" || x.Status == "Closed"))
				.ToList();
			foreach (var loan in closedLoans)
			{
				var terms = Terms(loan);
				if (terms.CustodyStatus == "Estate") terms.CustodyStatus = "Distributed";
				loan.EstateId = null;
				Store(loan, terms);
			}
			FMDB.Context.SaveChanges();
		}
	}

	internal static bool HasPendingRecoveryAuctions(IEstate estate)
	{
		var world = estate.EconomicZone.Gameworld;
		if (!HasPortfolio(world)) return false;
		var borrowerId = CharacterInstanceIdentityComparer.IdentityId(estate.Character);
		using (new FMDB())
		{
			var loans = FMDB.Context.LendingContracts
				.Where(x => x.BorrowerId == borrowerId && x.EconomicZoneId == estate.EconomicZone.Id && x.Status == "Recovery")
				.ToList();
			return loans.Any(x => HasRecoveryAuction(world, x));
		}
	}

	internal static bool TryCollectEstateSurplus(IEstate estate, out decimal total)
	{
		total = 0M;
		if (!HasPortfolio(estate.Gameworld)) return true;
		using (new FMDB())
		{
			var loans = FMDB.Context.LendingContracts
				.Where(x => x.EstateId == estate.Id && x.Surplus > 0)
				.ToList();
			if (loans.Any(x => x.CurrencyId != estate.EconomicZone.Currency.Id)) return false;
			foreach (var loan in loans)
			{
				// Recovery removed the entire net sale price from the auction house.
				// This amount is already reserved for the borrower and can be assigned directly.
				var amount = loan.Surplus;
				loan.Surplus = 0M;
				Post(loan, "SurplusDistribution", 0M, 0M, amount, $"Surplus distributed through estate #{estate.Id}",
					estate.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime,
					operationKey: $"estate:{estate.Id}:surplus:{loan.Id}");
				total += amount;
			}
			FMDB.Context.SaveChanges();
			return true;
		}
	}

	private static bool HasRecoveryAuction(IFuturemud world, LendingContract contract)
	{
		var terms = Terms(contract);
		return terms.AuctionHouseId > 0 &&
		       world.AuctionHouses.Get(terms.AuctionHouseId)?.ActiveAuctionItems
			       .Any(x => x.RecoveryContractId == contract.Id) == true;
	}

	internal static IDisposable AuthorizeSecurityTransfer(long propertyId)
	{
		var authorised = _authorisedSecurityTransfers.Value ??= new HashSet<long>();
		authorised.Add(propertyId);
		return new SecurityTransferScope(propertyId);
	}

	internal static bool IsSecurityTransferAuthorised(long propertyId) =>
		_authorisedSecurityTransfers.Value?.Contains(propertyId) == true;

	private sealed class SecurityTransferScope(long propertyId) : IDisposable
	{
		public void Dispose() => _authorisedSecurityTransfers.Value?.Remove(propertyId);
	}
}
