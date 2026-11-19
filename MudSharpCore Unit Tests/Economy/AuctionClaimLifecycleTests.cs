#nullable enable

using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests.Economy;

[TestClass]
public class AuctionClaimLifecycleTests
{
	[TestMethod]
	public void Claim_WinningBidderReceivesCustodyBeforeActivation()
	{
		var world = new Mock<IFuturemud>();
		var buyer = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		buyer.SetupGet(x => x.Id).Returns(21);
		world.Setup(x => x.TryGetCharacter(21, true)).Returns(buyer.Object);
		var body = new Mock<IBody>();
		buyer.SetupGet(x => x.Body).Returns(body.Object);
		var item = new Mock<IGameItem>();
		var house = new Mock<IAuctionHouse>();
		house.SetupGet(x => x.Name).Returns("Lifecycle");
		var lot = new AuctionItem { Asset = item.Object, Seller = Mock.Of<ICharacter>(x => x.Id == 12) };
		house.SetupGet(x => x.UnclaimedItems).Returns([
			new UnclaimedAuctionItem
			{
				AuctionItem = lot,
				WinningBid = new AuctionBid
				{
					BidderId = 21,
					BidDateTime = new MudDateTime(MudDateTime.Never) { Gameworld = world.Object }
				}
			}
		]);
		var steps = new List<string>();
		house.Setup(x => x.ClaimItem(lot)).Callback(() => steps.Add("release"));
		item.Setup(x => x.SetOwner(buyer.Object)).Callback(() => steps.Add("owner"));
		body.Setup(x => x.CanGet(item.Object, 0, ItemCanGetIgnore.None)).Returns(true);
		body.Setup(x => x.Get(item.Object, 0, null, true, ItemCanGetIgnore.None)).Callback(() => steps.Add("custody"));
		item.Setup(x => x.Login()).Callback(() => steps.Add("activate"));

		typeof(EconomyModule).GetMethod("AuctionClaim", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [buyer.Object, house.Object, new StringStack("")]);

		CollectionAssert.AreEqual(new[] { "release", "owner", "custody", "activate" }, steps);
	}
}
