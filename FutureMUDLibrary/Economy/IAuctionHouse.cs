using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy
{
	public record AuctionBid
	{
		public long BidderId { get; init; }

		public ICharacter Bidder
		{
			get
			{
				return BidDateTime.Gameworld.TryGetCharacter(BidderId, true);
			}

			init
			{
				BidderId = value.Id;
			}
		}
		public decimal Bid { get; init; }
		public MudDateTime BidDateTime { get; init; }

		public XElement SaveToXml()
		{
			return new XElement("Bid",
				new XAttribute("bidder", BidderId),
				new XAttribute("bid", Bid),
				new XAttribute("date", BidDateTime.GetDateTimeString())
			);
		}
	}

	public record AuctionItem : IKeyworded
	{
		public IGameItem Item { get; init; }
		public long ListingCharacterId { get; init; }
		public decimal MinimumPrice { get; init; }
		public decimal? BuyoutPrice { get; init; }
		public MudDateTime ListingDateTime { get; init; }
		public MudDateTime FinishingDateTime { get; init; }
		public IBankAccount BankAccount { get; init; }

		public XElement SaveToXml(IEnumerable<AuctionBid> bids)
		{
			return new XElement("ActiveItem",
				new XAttribute("character", ListingCharacterId),
				new XAttribute("item", Item.Id),
				new XAttribute("price", MinimumPrice),
				new XAttribute("buyout", BuyoutPrice.HasValue ? BuyoutPrice.Value : "none"),
				new XAttribute("list", ListingDateTime.GetDateTimeString()),
				new XAttribute("finish", FinishingDateTime.GetDateTimeString()),
				new XAttribute("account", BankAccount.Id),
				from bid in bids
				select bid.SaveToXml()
			);
		}

		#region Implementation of IKeyworded

		public IEnumerable<string> Keywords => Item.Keywords;
		public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
		{
			return Item.GetKeywordsFor(voyeur);
		}

		public bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = false)
		{
			return Item.HasKeyword(targetKeyword, voyeur, abbreviated);
		}

		public bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = false)
		{
			return Item.HasKeywords(targetKeywords, voyeur, abbreviated);
		}

		#endregion
	}

	public record UnclaimedAuctionItem
	{
		public AuctionItem AuctionItem { get; init; }
		[CanBeNull] public AuctionBid WinningBid { get; init; }

		public XElement SaveToXml(IEnumerable<AuctionBid> bids)
		{
			return new XElement("Unclaimed",
				AuctionItem.SaveToXml(bids),
				WinningBid?.SaveToXml() ?? new XElement("NoBids")
			);
		}
	}

	public record AuctionResult
	{
		public long ItemId { get; init; }
		public string ItemDescription { get; init; }
		public bool Sold { get; init; }
		public decimal SalePrice { get; init; }
		public MudDateTime ResultDateTime { get; init; }
		public long ListingCharacterId { get; init; }
		public long SoldToId { get; init; }
		public bool PaidOutAtTime { get; init; }

		public XElement SaveToXml()
		{
			return new XElement("Result",
				new XAttribute("itemid", ItemId),
				new XAttribute("character", ListingCharacterId),
				new XAttribute("soldto", SoldToId),
				new XAttribute("sold", Sold),
				new XAttribute("price", SalePrice),
				new XAttribute("paid", PaidOutAtTime),
				new XElement("Date", ResultDateTime.GetDateTimeString()),
				new XElement("Description", new XCData(ItemDescription))
			);
		}
	}

	public interface IAuctionHouse : ISaveable, IEditableItem
	{
		IEconomicZone EconomicZone { get; }
		ICell AuctionHouseCell { get; }
		IBankAccount ProfitsBankAccount { get; }
		TimeSpan DefaultListingTime { get; }
		decimal AuctionListingFeeFlat { get; }
		decimal AuctionListingFeeRate { get; }
		IEnumerable<AuctionResult> AuctionResults { get; }
		IEnumerable<UnclaimedAuctionItem> UnclaimedItems { get; }
		IEnumerable<AuctionItem> ActiveAuctionItems { get; }
		CollectionDictionary<AuctionItem,AuctionBid> AuctionBids { get; }
		DecimalCounter<long> CharacterRefundsOwed { get; }
		void AddAuctionItem(AuctionItem item);
		void AddBid(AuctionItem item, AuctionBid bid);
		void ClaimItem(AuctionItem item);
		bool ClaimRefund(ICharacter actor);
		decimal CurrentBid(AuctionItem item);
		void CancelItem(AuctionItem item);
	}
}
