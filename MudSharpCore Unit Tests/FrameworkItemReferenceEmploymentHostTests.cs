#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Economy;
using MudSharp.Economy.Property;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FrameworkItemReferenceEmploymentHostTests
{
	[TestMethod]
	public void GetItem_AllEmploymentHostTypes_ResolveAfterPersistenceRoundTrip()
	{
		var gameworld = new Mock<IFuturemud>();
		var auction = FrameworkItem<IAuctionHouse>(1L, "AuctionHouse");
		var arena = FrameworkItem<ICombatArena>(2L, "CombatArena");
		var stable = FrameworkItem<IStable>(3L, "Stable");
		var hospital = FrameworkItem<IHospital>(4L, "Hospital");
		var hotel = FrameworkItem<IHotel>(5L, "Hotel");
		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Id).Returns(6L);
		property.SetupGet(x => x.Name).Returns("Hotel Property");
		property.SetupGet(x => x.FrameworkItemType).Returns("Property");
		property.SetupGet(x => x.ExistingHotel).Returns(hotel.Object);

		gameworld.SetupGet(x => x.AuctionHouses).Returns(new All<IAuctionHouse> { auction.Object });
		gameworld.SetupGet(x => x.CombatArenas).Returns(new All<ICombatArena> { arena.Object });
		gameworld.SetupGet(x => x.Stables).Returns(new All<IStable> { stable.Object });
		gameworld.SetupGet(x => x.Hospitals).Returns(new All<IHospital> { hospital.Object });
		gameworld.SetupGet(x => x.Properties).Returns(new All<IProperty> { property.Object });

		Assert.AreSame(auction.Object, new FrameworkItemReference(1L, "AuctionHouse", gameworld.Object).GetItem);
		Assert.AreSame(arena.Object, new FrameworkItemReference(2L, "CombatArena", gameworld.Object).GetItem);
		Assert.AreSame(stable.Object, new FrameworkItemReference(3L, "Stable", gameworld.Object).GetItem);
		Assert.AreSame(hospital.Object, new FrameworkItemReference(4L, "Hospital", gameworld.Object).GetItem);
		Assert.AreSame(hotel.Object, new FrameworkItemReference(5L, "Hotel", gameworld.Object).GetItem);
	}

	private static Mock<T> FrameworkItem<T>(long id, string type) where T : class, IFrameworkItem
	{
		var item = new Mock<T>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Name).Returns(type);
		item.SetupGet(x => x.FrameworkItemType).Returns(type);
		return item;
	}
}
