using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Economy;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AuctionXmlSerializationTests
{
	[TestMethod]
	public void AuctionItem_SaveToXml_ItemLot_WritesGenericAndLegacyAttributes()
	{
		var asset = new Mock<IGameItem>();
		asset.SetupGet(x => x.Id).Returns(11L);
		asset.SetupGet(x => x.Name).Returns("sabre");
		asset.SetupGet(x => x.FrameworkItemType).Returns("GameItem");

		var seller = new Mock<ICharacter>();
		seller.SetupGet(x => x.Id).Returns(22L);
		seller.SetupGet(x => x.Name).Returns("Seller");
		seller.SetupGet(x => x.FrameworkItemType).Returns("Character");

		var payout = new Mock<IBankAccount>();
		payout.SetupGet(x => x.Id).Returns(33L);
		payout.SetupGet(x => x.Name).Returns("Account");
		payout.SetupGet(x => x.FrameworkItemType).Returns("BankAccount");

		var item = new AuctionItem
		{
			Asset = asset.Object,
			Seller = seller.Object,
			PayoutTarget = payout.Object,
			MinimumPrice = 100.0M,
			BuyoutPrice = 150.0M,
			ListingDateTime = MudDateTime.Never,
			FinishingDateTime = MudDateTime.Never
		};

		var xml = item.SaveToXml(new[]
		{
			new AuctionBid
			{
				BidderId = 44L,
				Bid = 125.0M,
				BidDateTime = MudDateTime.Never
			}
		});

		Assert.AreEqual("Item", xml.Attribute("kind")?.Value);
		Assert.AreEqual("11", xml.Attribute("assetid")?.Value);
		Assert.AreEqual("GameItem", xml.Attribute("assettype")?.Value);
		Assert.AreEqual("22", xml.Attribute("sellerid")?.Value);
		Assert.AreEqual("Character", xml.Attribute("sellertype")?.Value);
		Assert.AreEqual("33", xml.Attribute("payoutid")?.Value);
		Assert.AreEqual("BankAccount", xml.Attribute("payouttype")?.Value);
		Assert.AreEqual("22", xml.Attribute("character")?.Value);
		Assert.AreEqual("11", xml.Attribute("item")?.Value);
		Assert.AreEqual("33", xml.Attribute("account")?.Value);
		Assert.AreEqual(1, xml.Elements("Bid").Count());
	}

	[TestMethod]
	public void AuctionItem_SaveToXml_PropertyEstateLot_WritesEstateSellerMetadata()
	{
		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Id).Returns(77L);
		property.SetupGet(x => x.Name).Returns("West Manor");
		property.SetupGet(x => x.FrameworkItemType).Returns("Property");

		var estate = new Mock<IEstate>();
		estate.SetupGet(x => x.Id).Returns(88L);
		estate.SetupGet(x => x.Name).Returns("Estate");
		estate.SetupGet(x => x.FrameworkItemType).Returns("Estate");

		var item = new AuctionItem
		{
			Asset = property.Object,
			Seller = estate.Object,
			PayoutTarget = estate.Object,
			MinimumPrice = 2500.0M,
			BuyoutPrice = null,
			ListingDateTime = MudDateTime.Never,
			FinishingDateTime = MudDateTime.Never
		};

		var xml = item.SaveToXml(Enumerable.Empty<AuctionBid>());

		Assert.AreEqual("Property", xml.Attribute("kind")?.Value);
		Assert.AreEqual("77", xml.Attribute("assetid")?.Value);
		Assert.AreEqual("Property", xml.Attribute("assettype")?.Value);
		Assert.AreEqual("88", xml.Attribute("sellerid")?.Value);
		Assert.AreEqual("Estate", xml.Attribute("sellertype")?.Value);
		Assert.AreEqual("88", xml.Attribute("payoutid")?.Value);
		Assert.AreEqual("Estate", xml.Attribute("payouttype")?.Value);
		Assert.AreEqual("0", xml.Attribute("character")?.Value);
		Assert.AreEqual("0", xml.Attribute("item")?.Value);
		Assert.AreEqual("0", xml.Attribute("account")?.Value);
		Assert.IsTrue(item.IsSeller(estate.Object));
		Assert.IsFalse(item.IsSeller(new FrameworkItemStub { Id = 99L, Name = "Other" }));
	}

	[TestMethod]
	public void AuctionResult_SaveToXml_WritesGenericSellerAndAssetAttributes()
	{
		var result = new AuctionResult
		{
			AssetId = 101L,
			AssetType = "Property",
			AssetDescription = "West Manor",
			Sold = true,
			SalePrice = 9000.0M,
			ResultDateTime = MudDateTime.Never,
			SellerId = 88L,
			SellerType = "Estate",
			PayoutTargetId = 88L,
			PayoutTargetType = "Estate",
			SoldToId = 44L,
			PaidOutAtTime = true
		};

		var xml = result.SaveToXml();

		Assert.AreEqual("101", xml.Attribute("itemid")?.Value);
		Assert.AreEqual("101", xml.Attribute("assetid")?.Value);
		Assert.AreEqual("Property", xml.Attribute("assettype")?.Value);
		Assert.AreEqual("0", xml.Attribute("character")?.Value);
		Assert.AreEqual("88", xml.Attribute("sellerid")?.Value);
		Assert.AreEqual("Estate", xml.Attribute("sellertype")?.Value);
		Assert.AreEqual("88", xml.Attribute("payoutid")?.Value);
		Assert.AreEqual("Estate", xml.Attribute("payouttype")?.Value);
		Assert.AreEqual("44", xml.Attribute("soldto")?.Value);
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), xml.Attribute("sold")?.Value);
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), xml.Attribute("paid")?.Value);
		Assert.AreEqual("West Manor", xml.Element("Description")?.Value);
	}
}
