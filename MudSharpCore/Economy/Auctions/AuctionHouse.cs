using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Auctions;

public class AuctionHouse : SaveableItem, IAuctionHouse
{
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
				Definition =
					new XElement("Definition",
						from result in AuctionResults
						select result.SaveToXml(),
						from item in ActiveAuctionItems
						select item.SaveToXml(AuctionBids[item]),
						from item in UnclaimedItems
						select item.SaveToXml(AuctionBids[item.AuctionItem]),
						from item in CharacterRefundsOwed
						select new XElement("Refund", new XAttribute("character", item.Key),
							new XAttribute("amount", item.Value))
					).ToString()
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

	private void AuctionTick()
	{
		var now = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		var finished = ActiveAuctionItems.Where(x => x.FinishingDateTime <= now).ToList();
		foreach (var item in finished)
		{
			var winningBid = CurrentAuctionBid(item);
			if (winningBid == null)
			{
				AuctionHouseCell.Handle(new EmoteOutput(new Emote(
					$"The auction for $0 has ended. There were no bids placed on the item.", item.Item, item.Item)));
			}
			else
			{
				AuctionHouseCell.Handle(new EmoteOutput(new Emote(
					$"The auction for $0 has ended. The winning bid was $1.", item.Item, item.Item,
					new DummyPerceivable(EconomicZone.Currency.Describe(CurrentBid(item),
						CurrencyDescriptionPatternType.ShortDecimal)))));
			}

			_activeAuctionItems.Remove(item);
			_unclaimedItems.Add(new UnclaimedAuctionItem
			{
				AuctionItem = item,
				WinningBid = winningBid
			});

			var paid = true;
			if (winningBid != null)
			{
				if (ProfitsBankAccount.CanWithdraw(winningBid.Bid, true).Truth)
				{
					ProfitsBankAccount.WithdrawFromTransfer(winningBid.Bid, item.BankAccount.Bank.Code,
						item.BankAccount.AccountNumber, $"Payment for successful auction of {item.Item.Name}");
					item.BankAccount.DepositFromTransfer(winningBid.Bid, ProfitsBankAccount.Bank.Code,
						ProfitsBankAccount.AccountNumber,
						$"Proceeds from a successful auction with of {item.Item.Name} with {Name}");
				}
				else
				{
					paid = false;
					CharacterRefundsOwed[item.ListingCharacterId] += winningBid.Bid;
				}
			}

			_auctionResults.Add(new AuctionResult
			{
				ItemId = item.Item.Id,
				ItemDescription = item.Item.HowSeen(item.Item, colour: false,
					flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings),
				Sold = winningBid != null,
				SalePrice = winningBid?.Bid ?? 0.0M,
				ResultDateTime = now,
				ListingCharacterId = item.ListingCharacterId,
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
				ItemId = long.Parse(result.Attribute("itemid").Value),
				Sold = bool.Parse(result.Attribute("sold").Value),
				SoldToId = long.Parse(result.Attribute("soldto").Value),
				ResultDateTime = new MudDateTime(result.Element("Date").Value, Gameworld),
				SalePrice = decimal.Parse(result.Attribute("price").Value),
				ItemDescription = result.Element("Description").Value,
				ListingCharacterId = long.Parse(result.Attribute("character").Value),
				PaidOutAtTime = bool.Parse(result.Attribute("paid").Value)
			});
		}

		foreach (var item in definition.Elements("ActiveItem"))
		{
			var auctionItem = new AuctionItem
			{
				Item = Gameworld.TryGetItem(long.Parse(item.Attribute("item").Value), true),
				MinimumPrice = decimal.Parse(item.Attribute("price").Value),
				BuyoutPrice =
					item.Attribute("buyout").Value.Equals("none", StringComparison.InvariantCultureIgnoreCase)
						? null
						: decimal.Parse(item.Attribute("buyout").Value),
				ListingDateTime = new MudDateTime(item.Attribute("list").Value, Gameworld),
				FinishingDateTime = new MudDateTime(item.Attribute("finish").Value, Gameworld),
				BankAccount = Gameworld.BankAccounts.Get(long.Parse(item.Attribute("account").Value)),
				ListingCharacterId = long.Parse(item.Attribute("character").Value)
			};
			if (auctionItem.Item is null)
			{
				continue;
			}

			_activeAuctionItems.Add(auctionItem);

			foreach (var bid in item.Elements("Bid"))
			{
				AuctionBids.Add(auctionItem, new AuctionBid
				{
					BidderId = long.Parse(bid.Attribute("bidder").Value),
					Bid = decimal.Parse(bid.Attribute("bid").Value),
					BidDateTime = new MudDateTime(bid.Attribute("date").Value, Gameworld)
				});
			}
		}

		foreach (var unclaimed in definition.Elements("Unclaimed"))
		{
			var item = unclaimed.Element("ActiveItem");
			var auctionItem = new AuctionItem
			{
				Item = Gameworld.TryGetItem(long.Parse(item.Attribute("item").Value), true),
				MinimumPrice = decimal.Parse(item.Attribute("price").Value),
				BuyoutPrice =
					item.Attribute("buyout").Value.Equals("none", StringComparison.InvariantCultureIgnoreCase)
						? null
						: decimal.Parse(item.Attribute("buyout").Value),
				ListingDateTime = new MudDateTime(item.Attribute("list").Value, Gameworld),
				FinishingDateTime = new MudDateTime(item.Attribute("finish").Value, Gameworld),
				BankAccount = Gameworld.BankAccounts.Get(long.Parse(item.Attribute("account").Value)),
				ListingCharacterId = long.Parse(item.Attribute("character").Value)
			};

			if (auctionItem.Item is null)
			{
				continue;
			}

			foreach (var bid in item.Elements("Bid"))
			{
				AuctionBids.Add(auctionItem, new AuctionBid
				{
					BidderId = long.Parse(bid.Attribute("bidder").Value),
					Bid = decimal.Parse(bid.Attribute("bid").Value),
					BidDateTime = new MudDateTime(bid.Attribute("date").Value, Gameworld)
				});
			}

			if (unclaimed.Element("NoBids") == null)
			{
				var bid = unclaimed.Element("Bid");
				_unclaimedItems.Add(new UnclaimedAuctionItem
				{
					AuctionItem = auctionItem,
					WinningBid = new AuctionBid
					{
						BidderId = long.Parse(bid.Attribute("bidder").Value),
						Bid = decimal.Parse(bid.Attribute("bid").Value),
						BidDateTime = new MudDateTime(bid.Attribute("date").Value, Gameworld)
					}
				});
			}
			else
			{
				_unclaimedItems.Add(new UnclaimedAuctionItem
				{
					AuctionItem = auctionItem,
					WinningBid = null
				});
			}
		}

		foreach (var refund in definition.Elements("Refund"))
		{
			CharacterRefundsOwed[long.Parse(refund.Attribute("character").Value)] =
				decimal.Parse(refund.Attribute("amount").Value);
		}
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
		dbitem.Definition =
			new XElement("Definition",
				from result in AuctionResults
				select result.SaveToXml(),
				from item in ActiveAuctionItems
				select item.SaveToXml(AuctionBids[item]),
				from item in UnclaimedItems
				select item.SaveToXml(AuctionBids[item.AuctionItem]),
				from item in CharacterRefundsOwed
				select new XElement("Refund", new XAttribute("character", item.Key),
					new XAttribute("amount", item.Value))
			).ToString();
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

	public CollectionDictionary<AuctionItem, AuctionBid> AuctionBids { get; } = new();
	public DecimalCounter<long> CharacterRefundsOwed { get; } = new();

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
			CharacterRefundsOwed[highestExistingBid.BidderId] += highestExistingBid.Bid;
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
		var owed = CharacterRefundsOwed[actor.Id];
		if (!ProfitsBankAccount.CanWithdraw(owed, false).Truth)
		{
			return false;
		}

		ProfitsBankAccount.WithdrawFromTransaction(owed, "Refund for failed bid");
		ProfitsBankAccount.Bank.CurrencyReserves[ProfitsBankAccount.Currency] -= owed;
		ProfitsBankAccount.Bank.Changed = true;
		CharacterRefundsOwed[actor.Id] = 0.0M;
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
			CharacterRefundsOwed[highestExistingBid.BidderId] += highestExistingBid.Bid;
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
			$"Refunds Owed: {EconomicZone.Currency.Describe(CharacterRefundsOwed.Sum(x => x.Value), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Pending Payments: {EconomicZone.Currency.Describe(ActiveAuctionItems.SelectNotNull(CurrentAuctionBid).Sum(x => x.Bid), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Total Commitments: {EconomicZone.Currency.Describe(CharacterRefundsOwed.Sum(x => x.Value) + ActiveAuctionItems.SelectNotNull(CurrentAuctionBid).Sum(x => x.Bid), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Unclaimed Items: {UnclaimedItems.Count().ToString("N0", actor).ColourValue()}");
		return sb.ToString();
	}

	#endregion
}