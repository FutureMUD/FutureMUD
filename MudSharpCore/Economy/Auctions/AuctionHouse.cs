using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Estates;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Auctions;

public class AuctionHouse : SaveableItem, IAuctionHouse, IPostCharacterLoadFinalisable
{
	private sealed record PendingAuctionItemLoad(XElement Item, bool IsUnclaimed, AuctionBid? WinningBid);

	private XElement SaveDefinition()
	{
		return new XElement("Definition",
			from result in AuctionResults
			select result.SaveToXml(),
			from item in ActiveAuctionItems
			select item.SaveToXml(AuctionBids[item]),
			from item in UnclaimedItems
			select item.SaveToXml(AuctionBids[item.AuctionItem]),
			from item in BidderRefundsOwed
			select new XElement("Refund", new XAttribute("character", item.Key),
				new XAttribute("amount", item.Value)),
			from item in _sellerPaymentsOwed.Where(x => x.Value > 0.0M)
			select new XElement("SellerPayment",
				new XAttribute("targetid", item.Key.Id),
				new XAttribute("targettype", item.Key.Type),
				new XAttribute("amount", item.Value))
		);
	}

	public AuctionHouse(IEconomicZone zone, string name, ICell cell, IBankAccount account)
	{
		Gameworld = zone.Gameworld;
		EconomicZone = zone;
		_name = name;
		AuctionHouseCell = cell;
		cell.CellProposedForDeletion += Cell_CellProposedForDeletion;
		ProfitsBankAccount = account;
		AuctionListingFeeFlat = Gameworld.GetStaticDecimal("DefaultAuctionHouseListingFeeFlat");
		AuctionListingFeeRate = Gameworld.GetStaticDecimal("DefaultAuctionHouseListingFeeRate");
		DefaultListingTime = TimeSpan.FromSeconds(Gameworld.GetStaticDouble("DefaultAuctionHouseListingTime"));
		using (new FMDB())
		{
			var dbitem = new Models.AuctionHouse
			{
				Name = name,
				EconomicZoneId = zone.Id,
				ProfitsBankAccountId = account.Id,
				AuctionListingFeeFlat = AuctionListingFeeFlat,
				AuctionListingFeeRate = AuctionListingFeeRate,
				AuctionHouseCellId = cell.Id,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.AuctionHouses.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += AuctionTick;
	}

	private void Cell_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason($"That room is the auction house location for auction house #{Id:N0} ({Name.ColourName()})");
	}

	private static IFrameworkItem LoadFrameworkItem(IFuturemud gameworld, long id, string type)
	{
		if (id == 0 || string.IsNullOrWhiteSpace(type) || type.EqualTo("none"))
		{
			return null;
		}

		return new FrameworkItemReference(id, type, gameworld).GetItem;
	}

	private AuctionItem LoadAuctionItem(XElement item)
	{
		var assetId = long.Parse(item.Attribute("assetid")?.Value ?? item.Attribute("item")?.Value ?? "0");
		var assetType = item.Attribute("assettype")?.Value ?? "GameItem";
		var sellerId = long.Parse(item.Attribute("sellerid")?.Value ?? item.Attribute("character")?.Value ?? "0");
		var sellerType = item.Attribute("sellertype")?.Value ??
		                 (item.Attribute("character") != null ? "Character" : "None");
		var payoutId = long.Parse(item.Attribute("payoutid")?.Value ?? item.Attribute("account")?.Value ?? "0");
		var payoutType = item.Attribute("payouttype")?.Value ??
		                 (item.Attribute("account") != null ? "BankAccount" : "None");
		var asset = assetType.EqualTo("Property")
			? (IFrameworkItem)Gameworld.Properties.Get(assetId)
			: Gameworld.TryGetItem(assetId, true);
		var seller = LoadFrameworkItem(Gameworld, sellerId, sellerType);
		var payoutTarget = LoadFrameworkItem(Gameworld, payoutId, payoutType);
		if (asset == null || seller == null)
		{
			return null;
		}

		return new AuctionItem
		{
			Asset = asset,
			Seller = seller,
			PayoutTarget = payoutTarget,
			PropertyShare = decimal.Parse(item.Attribute("share")?.Value ?? "1.0"),
			MinimumPrice = decimal.Parse(item.Attribute("price").Value),
			BuyoutPrice =
				item.Attribute("buyout").Value.Equals("none", StringComparison.InvariantCultureIgnoreCase)
					? null
					: decimal.Parse(item.Attribute("buyout").Value),
			ListingDateTime = new MudDateTime(item.Attribute("list").Value, Gameworld),
			FinishingDateTime = new MudDateTime(item.Attribute("finish").Value, Gameworld)
		};
	}

	private void AddLoadedAuctionItem(AuctionItem auctionItem, IEnumerable<XElement> bids)
	{
		_activeAuctionItems.Add(auctionItem);
		foreach (var bid in bids)
		{
			AuctionBids.Add(auctionItem, LoadBid(bid, Gameworld));
		}
	}

	private void AddLoadedUnclaimedAuctionItem(AuctionItem auctionItem, IEnumerable<XElement> bids, AuctionBid? winningBid)
	{
		foreach (var bid in bids)
		{
			AuctionBids.Add(auctionItem, LoadBid(bid, Gameworld));
		}

		_unclaimedItems.Add(new UnclaimedAuctionItem
		{
			AuctionItem = auctionItem,
			WinningBid = winningBid
		});
	}

	private void LogSkippedAuctionLoad(XElement item, string listType)
	{
		var assetId = item.Attribute("assetid")?.Value ?? item.Attribute("item")?.Value ?? "0";
		var assetType = item.Attribute("assettype")?.Value ?? "GameItem";
		var sellerId = item.Attribute("sellerid")?.Value ?? item.Attribute("character")?.Value ?? "0";
		var sellerType = item.Attribute("sellertype")?.Value ??
		                 (item.Attribute("character") != null ? "Character" : "None");
		ConsoleUtilities.WriteLine(
			$"#1Warning: Skipping {listType} auction entry in auction house #{Id:N0} ({Name}) because {assetType} #{assetId} or {sellerType} #{sellerId} could not be resolved after character loading.#0");
	}

	private static AuctionBid LoadBid(XElement bid, IFuturemud gameworld)
	{
		return new AuctionBid
		{
			BidderId = long.Parse(bid.Attribute("bidder").Value),
			Bid = decimal.Parse(bid.Attribute("bid").Value),
			BidDateTime = new MudDateTime(bid.Attribute("date").Value, gameworld)
		};
	}

	private string DescribeLot(AuctionItem item, IPerceiver voyeur)
	{
		return item.Asset switch
		{
			IGameItem gameItem => gameItem.HowSeen(voyeur,
				flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings),
			IProperty property =>
				$"{item.PropertyShare.ToString("P2", voyeur).ColourValue()} ownership share in {property.Name.ColourName()}",
			_ => item.Asset.Name.ColourName()
		};
	}

	private string DescribeLotPlain(AuctionItem item)
	{
		return item.Asset switch
		{
			IGameItem gameItem => gameItem.HowSeen(gameItem, colour: false,
				flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings),
			IProperty property => $"{item.PropertyShare:P2} ownership share in {property.Name}",
			_ => item.Asset.Name
		};
	}

	private static void TransferPropertyShare(AuctionItem item, ICharacter bidder, decimal price)
	{
		if (item.Asset is not IProperty property)
		{
			return;
		}

		var owner = property.PropertyOwners.FirstOrDefault(x =>
			x.Owner.FrameworkItemEquals(item.Seller.Id, item.Seller.FrameworkItemType));
		if (owner == null)
		{
			return;
		}

		var transferShare = Math.Min(owner.ShareOfOwnership, item.PropertyShare);
		if (transferShare <= 0.0M)
		{
			return;
		}

		if (transferShare >= 1.0M && owner.ShareOfOwnership >= 1.0M && property.PropertyOwners.Count() == 1)
		{
			property.TransferProperty(bidder, price);
			return;
		}

		property.LastSaleValue = price;
		property.DivestOwnership(owner, transferShare / owner.ShareOfOwnership, bidder);
	}

	private bool TryPaySeller(AuctionItem item, decimal amount)
	{
		if (amount <= 0.0M)
		{
			return true;
		}

		return TryPaySellerTarget(item.PayoutTarget ?? item.Seller, amount, DescribeLotPlain(item));
	}

	private bool TryPaySellerTarget(IFrameworkItem payoutTarget, decimal amount, string assetDescription)
	{
		if (amount <= 0.0M || payoutTarget == null)
		{
			return true;
		}

		if (payoutTarget is IEstate estate)
		{
			if (!ProfitsBankAccount.CanWithdraw(amount, true).Truth)
			{
				return false;
			}

			ProfitsBankAccount.WithdrawFromTransaction(amount,
				$"Auction proceeds held for estate #{estate.Id} from {assetDescription}");
			ProfitsBankAccount.Bank.CurrencyReserves[ProfitsBankAccount.Currency] -= amount;
			ProfitsBankAccount.Bank.Changed = true;
			return true;
		}

		if (payoutTarget is IBankAccount account)
		{
			if (!ProfitsBankAccount.CanWithdraw(amount, true).Truth)
			{
				return false;
			}

			ProfitsBankAccount.WithdrawFromTransfer(amount, account.Bank.Code, account.AccountNumber,
				$"Payment for successful auction of {assetDescription}");
			account.DepositFromTransfer(amount, ProfitsBankAccount.Bank.Code, ProfitsBankAccount.AccountNumber,
				$"Proceeds from a successful auction of {assetDescription} with {Name}");
			return true;
		}

		var payoutAccount = Gameworld.BankAccounts.FirstOrDefault(x =>
			x.AccountStatus == BankAccountStatus.Active &&
			x.Currency == EconomicZone.Currency &&
			x.IsAccountOwner(payoutTarget));
		if (payoutAccount != null)
		{
			if (!ProfitsBankAccount.CanWithdraw(amount, true).Truth)
			{
				return false;
			}

			ProfitsBankAccount.WithdrawFromTransfer(amount, payoutAccount.Bank.Code, payoutAccount.AccountNumber,
				$"Payment for successful auction of {assetDescription}");
			payoutAccount.DepositFromTransfer(amount, ProfitsBankAccount.Bank.Code,
				ProfitsBankAccount.AccountNumber,
				$"Proceeds from a successful auction of {assetDescription} with {Name}");
			return true;
		}

		return false;
	}

	private void QueueSellerPayment(AuctionItem item, decimal amount)
	{
		var payoutTarget = item.PayoutTarget ?? item.Seller;
		if (payoutTarget == null || amount <= 0.0M)
		{
			return;
		}

		_sellerPaymentsOwed[(payoutTarget.Id, payoutTarget.FrameworkItemType)] += amount;
		Changed = true;
	}

	private void RetrySellerPayments()
	{
		foreach (var payment in _sellerPaymentsOwed.Where(x => x.Value > 0.0M).ToList())
		{
			var target = LoadFrameworkItem(Gameworld, payment.Key.Id, payment.Key.Type);
			if (target == null)
			{
				continue;
			}

			if (!TryPaySellerTarget(target, payment.Value, $"deferred auction proceeds from {Name}"))
			{
				continue;
			}

			_sellerPaymentsOwed.Remove(payment.Key);
			Changed = true;
		}
	}

	private void AuctionTick()
	{
		RetrySellerPayments();
		var now = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		var finished = ActiveAuctionItems.Where(x => x.FinishingDateTime <= now).ToList();
		foreach (var item in finished)
		{
			var winningBid = CurrentAuctionBid(item);
			if (winningBid == null)
			{
				AuctionHouseCell.Handle(
					$"The auctioneers announce that the auction for {DescribeLotPlain(item)} has ended without any bids.");
			}
			else
			{
				AuctionHouseCell.Handle(
					$"The auctioneers announce that the auction for {DescribeLotPlain(item)} has ended with a winning bid of {EconomicZone.Currency.Describe(CurrentBid(item), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
			}

			_activeAuctionItems.Remove(item);

			var paid = true;
			if (winningBid != null)
			{
				paid = TryPaySeller(item, winningBid.Bid);
				if (!paid)
				{
					QueueSellerPayment(item, winningBid.Bid);
				}
			}

			if (item.Seller is IEstate estateSeller)
			{
				estateSeller.RecordAuctionCompletion(item, winningBid);
			}

			switch (item.Asset)
			{
				case IProperty property when winningBid != null:
					TransferPropertyShare(item, winningBid.Bidder, winningBid.Bid);
					break;
				case IGameItem when winningBid != null:
					_unclaimedItems.Add(new UnclaimedAuctionItem
					{
						AuctionItem = item,
						WinningBid = winningBid
					});
					break;
				case IGameItem when winningBid == null && item.Seller is not IEstate:
					_unclaimedItems.Add(new UnclaimedAuctionItem
					{
						AuctionItem = item,
						WinningBid = null
					});
					break;
			}

			_auctionResults.Add(new AuctionResult
			{
				AssetId = item.Asset.Id,
				AssetType = item.Asset.FrameworkItemType,
				AssetDescription = DescribeLotPlain(item),
				Sold = winningBid != null,
				SalePrice = winningBid?.Bid ?? 0.0M,
				ResultDateTime = now,
				SellerId = item.Seller.Id,
				SellerType = item.Seller.FrameworkItemType,
				PayoutTargetId = item.PayoutTarget?.Id,
				PayoutTargetType = item.PayoutTarget?.FrameworkItemType,
				SoldToId = winningBid?.BidderId ?? 0L,
				PaidOutAtTime = paid
			});
			Changed = true;
		}
	}

	public AuctionHouse(Models.AuctionHouse dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += AuctionTick;
		EconomicZone = Gameworld.EconomicZones.Get(dbitem.EconomicZoneId);
		_id = dbitem.Id;
		_name = dbitem.Name;
		AuctionHouseCell = Gameworld.Cells.Get(dbitem.AuctionHouseCellId);
		AuctionHouseCell.CellProposedForDeletion += Cell_CellProposedForDeletion;
		ProfitsBankAccount = Gameworld.BankAccounts.Get(dbitem.ProfitsBankAccountId);
		AuctionListingFeeFlat = dbitem.AuctionListingFeeFlat;
		AuctionListingFeeRate = dbitem.AuctionListingFeeRate;
		DefaultListingTime = TimeSpan.FromSeconds(dbitem.DefaultListingTime);

		var definition = XElement.Parse(dbitem.Definition);
		foreach (var result in definition.Elements("Result"))
		{
			_auctionResults.Add(new AuctionResult
			{
				AssetId = long.Parse(result.Attribute("assetid")?.Value ?? result.Attribute("itemid")?.Value ?? "0"),
				AssetType = result.Attribute("assettype")?.Value ?? "GameItem",
				Sold = bool.Parse(result.Attribute("sold").Value),
				SoldToId = long.Parse(result.Attribute("soldto").Value),
				ResultDateTime = new MudDateTime(result.Element("Date").Value, Gameworld),
				SalePrice = decimal.Parse(result.Attribute("price").Value),
				AssetDescription = result.Element("Description").Value,
				SellerId = long.Parse(result.Attribute("sellerid")?.Value ?? result.Attribute("character")?.Value ?? "0"),
				SellerType = result.Attribute("sellertype")?.Value ??
				             (result.Attribute("character") != null ? "Character" : "None"),
				PayoutTargetId = long.Parse(result.Attribute("payoutid")?.Value ?? "0") switch
				{
					0L => null,
					var value => value
				},
				PayoutTargetType = result.Attribute("payouttype")?.Value,
				PaidOutAtTime = bool.Parse(result.Attribute("paid").Value)
			});
		}

		foreach (var item in definition.Elements("ActiveItem"))
		{
			_pendingAuctionItemLoads.Add(new PendingAuctionItemLoad(new XElement(item), false, null));
		}

		foreach (var unclaimed in definition.Elements("Unclaimed"))
		{
			var item = unclaimed.Element("ActiveItem");
			if (item == null)
			{
				continue;
			}

			_pendingAuctionItemLoads.Add(new PendingAuctionItemLoad(
				new XElement(item),
				true,
				unclaimed.Element("NoBids") == null && unclaimed.Element("Bid") is XElement bid
					? LoadBid(bid, Gameworld)
					: null));
		}

		foreach (var refund in definition.Elements("Refund"))
		{
			BidderRefundsOwed[long.Parse(refund.Attribute("character").Value)] =
				decimal.Parse(refund.Attribute("amount").Value);
		}

		foreach (var payment in definition.Elements("SellerPayment"))
		{
			_sellerPaymentsOwed[(
				long.Parse(payment.Attribute("targetid").Value),
				payment.Attribute("targettype").Value
			)] = decimal.Parse(payment.Attribute("amount").Value);
		}

		Gameworld.RegisterPostCharacterLoadFinalisable(this);
	}

	public void FinaliseLoading()
	{
		if (!_pendingAuctionItemLoads.Any())
		{
			return;
		}

		foreach (var pending in _pendingAuctionItemLoads)
		{
			var auctionItem = LoadAuctionItem(pending.Item);
			if (auctionItem == null)
			{
				LogSkippedAuctionLoad(pending.Item, pending.IsUnclaimed ? "unclaimed" : "active");
				continue;
			}

			if (pending.IsUnclaimed)
			{
				AddLoadedUnclaimedAuctionItem(auctionItem, pending.Item.Elements("Bid"), pending.WinningBid);
				continue;
			}

			AddLoadedAuctionItem(auctionItem, pending.Item.Elements("Bid"));
		}

		_pendingAuctionItemLoads.Clear();
	}

	#region Overrides of FrameworkItem

	public override string FrameworkItemType => "AuctionHouse";

	#endregion

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.AuctionHouses.Find(Id);
		dbitem.Name = Name;
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.AuctionHouseCellId = AuctionHouseCell.Id;
		dbitem.AuctionListingFeeFlat = AuctionListingFeeFlat;
		dbitem.AuctionListingFeeRate = AuctionListingFeeRate;
		dbitem.ProfitsBankAccountId = ProfitsBankAccount.Id;
		dbitem.DefaultListingTime = DefaultListingTime.TotalSeconds;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	#endregion

	[CanBeNull]
	private AuctionBid CurrentAuctionBid(AuctionItem item)
	{
		return AuctionBids[item].FirstMax(x => x.Bid);
	}

	public decimal CurrentBid(AuctionItem item)
	{
		return AuctionBids[item].Select(x => x.Bid).DefaultIfEmpty(0.0M).Max();
	}

	#region Implementation of IAuctionHouse

	public IEconomicZone EconomicZone { get; set; }
	public ICell AuctionHouseCell { get; set; }
	public IBankAccount ProfitsBankAccount { get; set; }
	public decimal AuctionListingFeeFlat { get; set; }
	public decimal AuctionListingFeeRate { get; set; }
	public TimeSpan DefaultListingTime { get; set; }

	private readonly List<AuctionResult> _auctionResults = new();
	public IEnumerable<AuctionResult> AuctionResults => _auctionResults;

	private readonly List<UnclaimedAuctionItem> _unclaimedItems = new();
	public IEnumerable<UnclaimedAuctionItem> UnclaimedItems => _unclaimedItems;

	private readonly List<AuctionItem> _activeAuctionItems = new();
	public IEnumerable<AuctionItem> ActiveAuctionItems => _activeAuctionItems;
	private readonly List<PendingAuctionItemLoad> _pendingAuctionItemLoads = new();

	public CollectionDictionary<AuctionItem, AuctionBid> AuctionBids { get; } = new();
	public DecimalCounter<long> BidderRefundsOwed { get; } = new();
	private readonly DecimalCounter<(long Id, string Type)> _sellerPaymentsOwed = new();

	public void AddAuctionItem(AuctionItem item)
	{
		_activeAuctionItems.Add(item);
		Changed = true;
	}

	public void AddBid(AuctionItem item, AuctionBid bid)
	{
		var highestExistingBid = AuctionBids[item].FirstMax(x => x.Bid);
		if (highestExistingBid != null)
		{
			BidderRefundsOwed[highestExistingBid.BidderId] += highestExistingBid.Bid;
		}

		AuctionBids.Add(item, bid);
		ProfitsBankAccount.Deposit(bid.Bid);
		ProfitsBankAccount.Bank.CurrencyReserves[EconomicZone.Currency] += bid.Bid;
		ProfitsBankAccount.Bank.Changed = true;
		Changed = true;
	}

	public void ClaimItem(AuctionItem item)
	{
		_unclaimedItems.RemoveAll(x => x.AuctionItem == item);
		Changed = true;
	}

	public bool ClaimRefund(ICharacter actor)
	{
		var owed = BidderRefundsOwed[actor.Id];
		if (!ProfitsBankAccount.CanWithdraw(owed, false).Truth)
		{
			return false;
		}

		ProfitsBankAccount.WithdrawFromTransaction(owed, "Refund for failed bid");
		ProfitsBankAccount.Bank.CurrencyReserves[ProfitsBankAccount.Currency] -= owed;
		ProfitsBankAccount.Bank.Changed = true;
		BidderRefundsOwed[actor.Id] = 0.0M;
		Changed = true;
		return true;
	}

	public void CancelItem(AuctionItem item)
	{
		_activeAuctionItems.Remove(item);
		Changed = true;
		var highestExistingBid = AuctionBids[item].FirstMax(x => x.Bid);
		if (highestExistingBid != null)
		{
			BidderRefundsOwed[highestExistingBid.BidderId] += highestExistingBid.Bid;
		}

		AuctionBids.Remove(item);
	}

	#endregion

	#region Implementation of IEditableItem

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "zone":
			case "economiczone":
			case "ez":
				return BuildingCommandEconomicZone(actor, command);
			case "fee":
				return BuildingCommandFee(actor, command);
			case "rate":
				return BuildingCommandRate(actor, command);
			case "bank":
				return BuildingCommandBank(actor, command);
			case "time":
				return BuildingCommandTime(actor, command);
			case "location":
				return BuildingCommandLocation(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options for this command:

  #3auction set name <name>#0 - renames the auction house
  #3auction set economiczone <which>#0 - changes the economic zone
  #3auction set fee <amount>#0 - sets the flat fee for listing an item
  #3auction set rate <%>#0 - sets the percentage fee for listing an item
  #3auction set bank <bank code>:<accn>#0 - changes the bank account for revenues
  #3auction set time <time period>#0 - sets the amount of time auctions run for
  #3auction set location#0 - changes the location of the auction house to the current cell".SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandTime(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How long should auction listings with this auction house last for (in game time)?");
			return false;
		}

		if (!TimeSpan.TryParse(command.SafeRemainingArgument, actor, out var time))
		{
			actor.OutputHandler.Send(
				"That is not a valid amount of time. Generally speaking the format is days:hours:minutes:seconds and it matches the way your account's cultureinfo handles timespans.");
			return false;
		}

		if (time <= TimeSpan.Zero)
		{
			actor.OutputHandler.Send($"You cannot have auctions last for no time.");
			return false;
		}

		DefaultListingTime = time;
		Changed = true;
		actor.OutputHandler.Send(
			$"This auction house will now list items for {time.Describe(actor).ColourValue()} of in-game time.");
		return true;
	}

	private bool BuildingCommandBank(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify a bank account into which any revenue will be transferred. Use the format BANKCODE:ACCOUNT#.");
			return false;
		}

		var bankString = command.SafeRemainingArgument;
		var (bankAccount, error) = Bank.FindBankAccount(bankString, null, actor);
		if (bankAccount is null)
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		ProfitsBankAccount = bankAccount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This auction house will now use bank account {bankAccount.Bank.Code.ColourValue()}:{bankAccount.AccountNumber.ToString("F0", actor).ColourValue()} for all revenue.");
		return true;
	}

	private bool BuildingCommandRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What percentage rate of the auction sale price should this auction house charge as a fee?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentageDecimal(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send("That is not a valid percentage.");
			return false;
		}

		if (value < 0.0M || value > 1.0M)
		{
			actor.OutputHandler.Send(
				$"Fees cannot be less than {0.0.ToString("P0", actor).ColourValue()} or greater than {1.0.ToString("P0", actor).ColourValue()}.");
			return false;
		}

		AuctionListingFeeRate = value;
		actor.OutputHandler.Send(
			$"A fee of {value.ToString("P3", actor).ColourValue()} of the auction sale price will now be charged.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandFee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must enter a valid amount of {EconomicZone.Currency.Name.ColourName()} for the listing fee.");
			return false;
		}

		if (!EconomicZone.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var fee))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of {EconomicZone.Currency.Name.ColourName()}.");
			return false;
		}

		if (fee < 0.0M)
		{
			actor.OutputHandler.Send("The listing fee cannot be negative.");
			return false;
		}

		AuctionListingFeeFlat = fee;
		Changed = true;
		actor.OutputHandler.Send(
			$"The listing fee for this auction house is now {EconomicZone.Currency.Describe(fee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEconomicZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone do you want to move this auction house to?");
			return false;
		}

		var zone = actor.Gameworld.EconomicZones.GetByIdOrName(command.SafeRemainingArgument);
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return false;
		}

		if (EconomicZone.Currency != zone.Currency)
		{
			actor.OutputHandler.Send(
				"You cannot currently change auction houses into economic zones that have different currencies.");
			return false;
		}

		EconomicZone = zone;
		Changed = true;
		actor.OutputHandler.Send(
			$"This auction house now belongs to the {EconomicZone.Name.ColourName()} economic zone.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this auction house?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.AuctionHouses.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already an auction house by the name {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the auction house from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandLocation(ICharacter actor, StringStack command)
	{
		if (Gameworld.AuctionHouses.Any(x => x.AuctionHouseCell == actor.Location))
		{
			actor.OutputHandler.Send(
				"There is already an auction house in this location. Only one auction house may be in a room at any time.");
			return false;
		}

		AuctionHouseCell.CellProposedForDeletion -= Cell_CellProposedForDeletion;
		AuctionHouseCell = actor.Location;
		AuctionHouseCell.CellProposedForDeletion -= Cell_CellProposedForDeletion;
		AuctionHouseCell.CellProposedForDeletion += Cell_CellProposedForDeletion;
		Changed = true;
		actor.OutputHandler.Send("This auction house is now based in your current location.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Auction House {Name.ColourName()} (#{Id.ToString("N0", actor)})");
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourValue()}");
		sb.AppendLine(
			$"Location: {AuctionHouseCell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)} (#{AuctionHouseCell.Id.ToString("N0", actor)})");
		sb.AppendLine(
			$"Bank Account: {ProfitsBankAccount.AccountNumber.ToString("F0", actor).ColourValue()} with {ProfitsBankAccount.Bank.Code.ColourName()}");
		sb.AppendLine(
			$"Listing Fee: {EconomicZone.Currency.Describe(AuctionListingFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} + {AuctionListingFeeRate.ToString("P3", actor).ColourValue()}");
		sb.AppendLine($"Listing Time: {DefaultListingTime.Describe(actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Current Listings: {ActiveAuctionItems.Count().ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Bank Account Balance: {EconomicZone.Currency.Describe(ProfitsBankAccount.CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Refunds Owed: {EconomicZone.Currency.Describe(BidderRefundsOwed.Sum(x => x.Value), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Seller Proceeds Owed: {EconomicZone.Currency.Describe(_sellerPaymentsOwed.Sum(x => x.Value), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Pending Payments: {EconomicZone.Currency.Describe(ActiveAuctionItems.SelectNotNull(CurrentAuctionBid).Sum(x => x.Bid), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Total Commitments: {EconomicZone.Currency.Describe(BidderRefundsOwed.Sum(x => x.Value) + _sellerPaymentsOwed.Sum(x => x.Value) + ActiveAuctionItems.SelectNotNull(CurrentAuctionBid).Sum(x => x.Bid), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Unclaimed Items: {UnclaimedItems.Count().ToString("N0", actor).ColourValue()}");
		return sb.ToString();
	}

	#endregion
}
