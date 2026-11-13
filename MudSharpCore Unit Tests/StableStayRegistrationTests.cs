#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Stables;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.NPC;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StableStayRegistrationTests
{
	[TestMethod]
	public void LedgerOpeningBalance_WithoutAnActor_UsesANullIdentity()
	{
		var entry = new StableLedgerEntry(Mock.Of<IStableStay>(), StableLedgerEntryType.OpeningBalance,
			null!, null, 5m, "Opening debt fixture");
		Assert.IsNull(entry.ActorId);
		Assert.IsNull(entry.ActorName);
	}

	[TestMethod]
	public void QuoteDailyFee_UnknownOwnerUsesFixedFeeWithoutEvaluatingOwnerProg()
	{
		var fixture = CreateStable(dailyFee: 12.5m);
		var mount = new Mock<ICharacter>();
		var dailyFeeProg = new Mock<IFutureProg>();
		fixture.Stable.DailyFeeProg = dailyFeeProg.Object;

		var fee = fixture.Stable.QuoteDailyFee(mount.Object, null);

		Assert.AreEqual(12.5m, fee);
		dailyFeeProg.Verify(x => x.ExecuteDecimal(It.IsAny<decimal>(), It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void QuoteDailyFee_UnknownOwnerStillClampsFixedFeeAtZero()
	{
		var fixture = CreateStable(dailyFee: -3m);
		var mount = new Mock<ICharacter>();

		Assert.AreEqual(0m, fixture.Stable.QuoteDailyFee(mount.Object, null));
	}

	[TestMethod]
	public void RegisterStay_RejectsNonNpcMountBeforePersistence()
	{
		var fixture = CreateStable();

		Assert.ThrowsExactly<ArgumentException>(() => fixture.Stable.RegisterStay(
			new Mock<ICharacter>().Object, null));
	}

	[TestMethod]
	public void RegisterStay_RejectsNegativeOpeningDebtBeforePersistence()
	{
		var fixture = CreateStable();
		var mount = new Mock<INPC>();
		mount.SetupGet(x => x.IsPrimaryInstance).Returns(true);

		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => fixture.Stable.RegisterStay(
			mount.Object, null, -0.01m));
	}

	private static (Stable Stable, Mock<IFuturemud> World) CreateStable(decimal dailyFee = 0m)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var zone = new Mock<IEconomicZone>();
		var room = new Mock<IRoom>();
		zone.SetupGet(x => x.Id).Returns(1);
		room.SetupGet(x => x.Id).Returns(1);
		var zones = new All<IEconomicZone>();
		var rooms = new All<IRoom>();
		var progs = new All<IFutureProg>();
		zones.Add(zone.Object);
		rooms.Add(room.Object);
		world.SetupGet(x => x.EconomicZones).Returns(zones);
		world.SetupGet(x => x.Rooms).Returns(rooms);
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		var stable = new Stable(new MudSharp.Models.Stable
		{
			Id = 1,
			Name = "Test Stable",
			EconomicZoneId = 1,
			RoomId = 1,
			DailyFee = dailyFee,
			EmployeeRecords = "<Employees/>"
		}, world.Object);
		return (stable, world);
	}
}
