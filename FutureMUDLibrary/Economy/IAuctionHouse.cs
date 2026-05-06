using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Economy
{
    public enum AuctionLotType
    {
        Item,
        Property
    }

    public record AuctionBid
    {
        public long BidderId { get; init; }

        public ICharacter Bidder
        {
            get => BidDateTime.Gameworld.TryGetCharacter(BidderId, true)!;

            init => BidderId = value.Id;
        }
        public decimal Bid { get; init; }
        public MudDateTime BidDateTime { get; init; } = null!;

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
        public IFrameworkItem Asset { get; init; } = null!;
        public IFrameworkItem Seller { get; init; } = null!;
        public IFrameworkItem? PayoutTarget { get; init; }
        public decimal PropertyShare { get; init; } = 1.0M;
        public decimal MinimumPrice { get; init; }
        public decimal? BuyoutPrice { get; init; }
        public MudDateTime ListingDateTime { get; init; } = null!;
        public MudDateTime FinishingDateTime { get; init; } = null!;

        public AuctionLotType LotType => Asset switch
        {
            IProperty => AuctionLotType.Property,
            _ => AuctionLotType.Item
        };

        public IGameItem? Item => Asset as IGameItem;
        public IProperty? Property => Asset as IProperty;

        public bool IsSeller(IFrameworkItem? seller)
        {
            return seller != null &&
                   Seller.FrameworkItemType.Equals(seller.FrameworkItemType, StringComparison.OrdinalIgnoreCase) &&
                   Seller.Id == seller.Id;
        }

        public XElement SaveToXml(IEnumerable<AuctionBid> bids)
        {
            return new XElement("ActiveItem",
                new XAttribute("kind", LotType.ToString()),
                new XAttribute("assetid", Asset.Id),
                new XAttribute("assettype", Asset.FrameworkItemType),
                new XAttribute("sellerid", Seller.Id),
                new XAttribute("sellertype", Seller.FrameworkItemType),
                new XAttribute("payoutid", PayoutTarget?.Id ?? 0L),
                new XAttribute("payouttype", PayoutTarget?.FrameworkItemType ?? "None"),
                new XAttribute("character", Seller is ICharacter ch ? ch.Id : 0L),
                new XAttribute("item", Item?.Id ?? 0L),
                new XAttribute("share", PropertyShare),
                new XAttribute("price", MinimumPrice),
                new XAttribute("buyout", BuyoutPrice.HasValue ? BuyoutPrice.Value : "none"),
                new XAttribute("list", ListingDateTime.GetDateTimeString()),
                new XAttribute("finish", FinishingDateTime.GetDateTimeString()),
                new XAttribute("account", PayoutTarget is IBankAccount account ? account.Id : 0L),
                from bid in bids
                select bid.SaveToXml()
            );
        }

        #region Implementation of IKeyworded

        public IEnumerable<string> Keywords => Asset switch
        {
            IKeyworded keyworded => keyworded.Keywords,
            _ => Enumerable.Empty<string>()
        };
        public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
        {
            return Asset switch
            {
                IKeyworded keyworded => keyworded.GetKeywordsFor(voyeur),
                _ => Enumerable.Empty<string>()
            };
        }

        public bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = false, bool useContainsOverStartsWith = false)
        {
            return Asset switch
            {
                IKeyworded keyworded => keyworded.HasKeyword(targetKeyword, voyeur, abbreviated, useContainsOverStartsWith),
                _ => false
            };
        }

        public bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = false, bool useContainsOverStartsWith = false)
        {
            return Asset switch
            {
                IKeyworded keyworded => keyworded.HasKeywords(targetKeywords, voyeur, abbreviated, useContainsOverStartsWith),
                _ => false
            };
        }

        #endregion
    }

    public record UnclaimedAuctionItem
    {
        public AuctionItem AuctionItem { get; init; } = null!;
        public AuctionBid? WinningBid { get; init; }

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
        public long AssetId { get; init; }
        public string AssetType { get; init; } = null!;
        public string AssetDescription { get; init; } = null!;
        public bool Sold { get; init; }
        public decimal SalePrice { get; init; }
        public MudDateTime ResultDateTime { get; init; } = null!;
        public long SellerId { get; init; }
        public string SellerType { get; init; } = null!;
        public long? PayoutTargetId { get; init; }
        public string? PayoutTargetType { get; init; }
        public long SoldToId { get; init; }
        public bool PaidOutAtTime { get; init; }

        public XElement SaveToXml()
        {
            return new XElement("Result",
                new XAttribute("itemid", AssetId),
                new XAttribute("assetid", AssetId),
                new XAttribute("assettype", AssetType),
                new XAttribute("character", SellerType.Equals("Character", StringComparison.OrdinalIgnoreCase) ? SellerId : 0L),
                new XAttribute("sellerid", SellerId),
                new XAttribute("sellertype", SellerType),
                new XAttribute("payoutid", PayoutTargetId ?? 0L),
                new XAttribute("payouttype", PayoutTargetType ?? "None"),
                new XAttribute("soldto", SoldToId),
                new XAttribute("sold", Sold),
                new XAttribute("price", SalePrice),
                new XAttribute("paid", PaidOutAtTime),
                new XElement("Date", ResultDateTime.GetDateTimeString()),
                new XElement("Description", new XCData(AssetDescription))
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
        CollectionDictionary<AuctionItem, AuctionBid> AuctionBids { get; }
        DecimalCounter<long> BidderRefundsOwed { get; }
        void AddAuctionItem(AuctionItem item);
        void AddBid(AuctionItem item, AuctionBid bid);
        void BuyoutItem(AuctionItem item, AuctionBid bid);
        void ClaimItem(AuctionItem item);
        bool ClaimRefund(ICharacter actor);
        decimal CurrentBid(AuctionItem item);
        void CancelItem(AuctionItem item);
    }
}
