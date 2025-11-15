using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Models;
using System.Linq;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaFinanceServiceTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString())
		.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void IsSolvent_AccountsForBlockedPayouts()
	{
		using var context = BuildContext();
		context.ArenaEvents.Add(new MudSharp.Models.ArenaEvent
		{
			Id = 1,
			ArenaId = 5,
			ArenaEventTypeId = 1,
			State = 0,
			BringYourOwn = false,
			RegistrationDurationSeconds = 0,
			PreparationDurationSeconds = 0,
			BettingModel = 0,
			AppearanceFee = 0,
			VictoryFee = 0,
			CreatedAt = DateTime.UtcNow,
			ScheduledAt = DateTime.UtcNow
		});
		context.ArenaBetPayouts.Add(new ArenaBetPayout
		{
			ArenaEventId = 1,
			CharacterId = 10,
			Amount = 50m,
			IsBlocked = true,
			CreatedAt = DateTime.UtcNow
		});
		context.SaveChanges();
		var arena = new Mock<ICombatArena>();
		arena.Setup(x => x.Id).Returns(5L);
		arena.Setup(x => x.AvailableFunds()).Returns(70m);
		var service = new ArenaFinanceService(() => context);
		var result = service.IsSolvent(arena.Object, 40m);
		Assert.IsFalse(result.Truth);
		Assert.IsTrue(result.Reason.Contains("short"));
	}

	[TestMethod]
	public void UnblockPayouts_PaysWhenFundsAvailable()
	{
		using var context = BuildContext();
		context.ArenaEvents.Add(new MudSharp.Models.ArenaEvent
		{
			Id = 2,
			ArenaId = 8,
			ArenaEventTypeId = 1,
			State = 0,
			BringYourOwn = false,
			RegistrationDurationSeconds = 0,
			PreparationDurationSeconds = 0,
			BettingModel = 0,
			AppearanceFee = 0,
			VictoryFee = 0,
			CreatedAt = DateTime.UtcNow,
			ScheduledAt = DateTime.UtcNow
		});
		context.ArenaBetPayouts.Add(new ArenaBetPayout
		{
			ArenaEventId = 2,
			CharacterId = 20,
			Amount = 30m,
			IsBlocked = true,
			CreatedAt = DateTime.UtcNow
		});
		context.SaveChanges();
		var arena = new Mock<ICombatArena>();
		arena.Setup(x => x.Id).Returns(8L);
		arena.Setup(x => x.EnsureFunds(30m)).Returns((true, string.Empty));
		var service = new ArenaFinanceService(() => context);
		service.UnblockPayouts(arena.Object);
		var payout = context.ArenaBetPayouts.Single();
		Assert.IsFalse(payout.IsBlocked);
		Assert.IsNotNull(payout.CollectedAt);
		arena.Verify(x => x.Debit(30m, It.Is<string>(s => s.Contains("payout"))), Times.Once);
	}

	[TestMethod]
	public void WithholdTax_RecordsSnapshotAndDebits()
	{
		using var context = BuildContext();
		var arena = new Mock<ICombatArena>();
		arena.Setup(x => x.Id).Returns(11L);
		arena.Setup(x => x.Debit(15m, It.IsAny<string>()));
		var service = new ArenaFinanceService(() => context);
		service.WithholdTax(arena.Object, 15m, "withhold");
		var snapshot = context.ArenaFinanceSnapshots.Single();
		Assert.AreEqual(15m, snapshot.TaxWithheld);
		arena.Verify(x => x.Debit(15m, "withhold"), Times.Once);
	}
}
