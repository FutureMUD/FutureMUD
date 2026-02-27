#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Models;

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
		SeedArenaEvent(context, 1, 5);
		context.ArenaBetPayouts.Add(new ArenaBetPayout
		{
			ArenaEventId = 1,
			CharacterId = 10,
			Amount = 50m,
			PayoutType = (int)ArenaPayoutType.Bet,
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
		Assert.IsTrue(result.Reason.Contains("short", StringComparison.InvariantCultureIgnoreCase));
	}

	[TestMethod]
	public void UnblockPayouts_FundsBlockedPayout_AndLeavesCollectible()
	{
		using var context = BuildContext();
		SeedArenaEvent(context, 2, 8);
		context.ArenaBetPayouts.Add(new ArenaBetPayout
		{
			ArenaEventId = 2,
			CharacterId = 20,
			Amount = 30m,
			PayoutType = (int)ArenaPayoutType.Bet,
			IsBlocked = true,
			CreatedAt = DateTime.UtcNow
		});
		context.SaveChanges();

		var arena = new Mock<ICombatArena>();
		arena.Setup(x => x.Id).Returns(8L);
		arena.Setup(x => x.EnsureFunds(30m)).Returns((true, string.Empty));
		arena.Setup(x => x.AvailableFunds()).Returns(1_000m);
		var service = new ArenaFinanceService(() => context);

		service.UnblockPayouts(arena.Object);

		var payout = context.ArenaBetPayouts.Single();
		Assert.IsFalse(payout.IsBlocked);
		Assert.IsNull(payout.CollectedAt);
		arena.Verify(x => x.Debit(30m, It.Is<string>(s => s.Contains("payout funding"))), Times.Once);
	}

	[TestMethod]
	public void AccrueAppearancePayouts_RespectsNpcToggle()
	{
		using var context = BuildContext();
		SeedArenaEvent(context, 30, 9);
		SeedArenaEvent(context, 31, 9);

		var arena = new Mock<ICombatArena>();
		arena.SetupGet(x => x.Id).Returns(9L);
		arena.Setup(x => x.EnsureFunds(It.IsAny<decimal>())).Returns((true, string.Empty));
		arena.Setup(x => x.AvailableFunds()).Returns(1_000m);
		var service = new ArenaFinanceService(() => context);

		var pc = BuildParticipant(100, isNpc: false);
		var npc = BuildParticipant(200, isNpc: true);
		var participants = new[] { pc, npc };

		var noNpcEvent = BuildAppearanceEvent(30, arena.Object, 15m, payNpcAppearance: false, participants);
		service.AccrueAppearancePayouts(noNpcEvent.Object);

		var withNpcEvent = BuildAppearanceEvent(31, arena.Object, 15m, payNpcAppearance: true, participants);
		service.AccrueAppearancePayouts(withNpcEvent.Object);

		var firstEventPayouts = context.ArenaBetPayouts.Where(x => x.ArenaEventId == 30).ToList();
		Assert.AreEqual(1, firstEventPayouts.Count);
		Assert.AreEqual(100L, firstEventPayouts[0].CharacterId);
		Assert.AreEqual((int)ArenaPayoutType.Appearance, firstEventPayouts[0].PayoutType);
		Assert.IsFalse(firstEventPayouts[0].IsBlocked);

		var secondEventPayouts = context.ArenaBetPayouts.Where(x => x.ArenaEventId == 31).ToList();
		Assert.AreEqual(2, secondEventPayouts.Count);
		CollectionAssert.AreEquivalent(new[] { 100L, 200L }, secondEventPayouts.Select(x => x.CharacterId).ToArray());
		Assert.IsTrue(secondEventPayouts.All(x => !x.IsBlocked));
		arena.Verify(x => x.Debit(15m, It.IsAny<string>()), Times.Exactly(3));
	}

	[TestMethod]
	public void AccrueAppearancePayouts_BlocksWhenArenaCannotFund()
	{
		using var context = BuildContext();
		SeedArenaEvent(context, 40, 10);

		var arena = new Mock<ICombatArena>();
		arena.SetupGet(x => x.Id).Returns(10L);
		arena.Setup(x => x.EnsureFunds(It.IsAny<decimal>())).Returns((false, "insufficient"));
		arena.Setup(x => x.AvailableFunds()).Returns(0m);
		var service = new ArenaFinanceService(() => context);

		var participant = BuildParticipant(300, isNpc: false);
		var arenaEvent = BuildAppearanceEvent(40, arena.Object, 25m, payNpcAppearance: false, new[] { participant });

		service.AccrueAppearancePayouts(arenaEvent.Object);

		var payout = context.ArenaBetPayouts.Single();
		Assert.IsTrue(payout.IsBlocked);
		Assert.AreEqual((int)ArenaPayoutType.Appearance, payout.PayoutType);
		arena.Verify(x => x.Debit(It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void GetUnclaimedMoney_SumsOnlyUnblockedUncollectedBetAndAppearance()
	{
		using var context = BuildContext();
		SeedArenaEvent(context, 51, 20);
		SeedArenaEvent(context, 52, 20);
		SeedArenaEvent(context, 53, 21);

		context.ArenaBetPayouts.AddRange(
			new ArenaBetPayout
			{
				ArenaEventId = 51,
				CharacterId = 1,
				Amount = 10m,
				PayoutType = (int)ArenaPayoutType.Bet,
				IsBlocked = false,
				CreatedAt = DateTime.UtcNow
			},
			new ArenaBetPayout
			{
				ArenaEventId = 51,
				CharacterId = 1,
				Amount = 20m,
				PayoutType = (int)ArenaPayoutType.Appearance,
				IsBlocked = false,
				CreatedAt = DateTime.UtcNow
			},
			new ArenaBetPayout
			{
				ArenaEventId = 52,
				CharacterId = 1,
				Amount = 30m,
				PayoutType = (int)ArenaPayoutType.Bet,
				IsBlocked = true,
				CreatedAt = DateTime.UtcNow
			},
			new ArenaBetPayout
			{
				ArenaEventId = 52,
				CharacterId = 1,
				Amount = 40m,
				PayoutType = (int)ArenaPayoutType.Appearance,
				IsBlocked = false,
				CreatedAt = DateTime.UtcNow,
				CollectedAt = DateTime.UtcNow
			},
			new ArenaBetPayout
			{
				ArenaEventId = 52,
				CharacterId = 1,
				Amount = 50m,
				PayoutType = 99,
				IsBlocked = false,
				CreatedAt = DateTime.UtcNow
			},
			new ArenaBetPayout
			{
				ArenaEventId = 53,
				CharacterId = 1,
				Amount = 60m,
				PayoutType = (int)ArenaPayoutType.Bet,
				IsBlocked = false,
				CreatedAt = DateTime.UtcNow
			});
		context.SaveChanges();

		var arena = new Mock<ICombatArena>();
		arena.SetupGet(x => x.Id).Returns(20L);
		var service = new ArenaFinanceService(() => context);

		var total = service.GetUnclaimedMoney(arena.Object);

		Assert.AreEqual(30m, total);
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

	private static void SeedArenaEvent(FuturemudDatabaseContext context, long eventId, long arenaId)
	{
		context.ArenaEvents.Add(new MudSharp.Models.ArenaEvent
		{
			Id = eventId,
			ArenaId = arenaId,
			ArenaEventTypeId = 1,
			State = 0,
			BringYourOwn = false,
			RegistrationDurationSeconds = 0,
			PreparationDurationSeconds = 0,
			BettingModel = 0,
			AppearanceFee = 0,
			VictoryFee = 0,
			PayNpcAppearanceFee = false,
			CreatedAt = DateTime.UtcNow,
			ScheduledAt = DateTime.UtcNow
		});
		context.SaveChanges();
	}

	private static IArenaParticipant BuildParticipant(long characterId, bool isNpc)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(characterId);
		var participant = new Mock<IArenaParticipant>();
		participant.SetupGet(x => x.Character).Returns(character.Object);
		participant.SetupGet(x => x.CharacterId).Returns(characterId);
		participant.SetupGet(x => x.IsNpc).Returns(isNpc);
		return participant.Object;
	}

	private static Mock<IArenaEvent> BuildAppearanceEvent(long eventId, ICombatArena arena, decimal appearanceFee,
		bool payNpcAppearance, IEnumerable<IArenaParticipant> participants)
	{
		var arenaEvent = new Mock<IArenaEvent>();
		arenaEvent.SetupGet(x => x.Id).Returns(eventId);
		arenaEvent.SetupGet(x => x.Arena).Returns(arena);
		arenaEvent.SetupGet(x => x.AppearanceFee).Returns(appearanceFee);
		arenaEvent.SetupGet(x => x.PayNpcAppearanceFee).Returns(payNpcAppearance);
		arenaEvent.SetupGet(x => x.Participants).Returns(participants);
		return arenaEvent;
	}
}
