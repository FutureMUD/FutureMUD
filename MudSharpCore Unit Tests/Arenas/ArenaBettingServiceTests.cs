using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaBettingServiceTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString())
		.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static ArenaBettingService CreateService(FuturemudDatabaseContext context, Mock<IArenaFinanceService> financeMock, Mock<IArenaBetPaymentService> paymentMock, Dictionary<long, ICharacter> characters)
	{
		var charactersSet = new Mock<IUneditableAll<ICharacter>>();
		charactersSet.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => characters.TryGetValue(id, out var value) ? value : null);
		charactersSet.Setup(x => x.Has(It.IsAny<ICharacter>()))
			.Returns<ICharacter>(c => c is not null && characters.ContainsKey(c.Id));
		charactersSet.Setup(x => x.Has(It.IsAny<long>())).Returns<long>(id => characters.ContainsKey(id));
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.Characters).Returns(charactersSet.Object);
		gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>())).Returns<long, bool>((id, _) => characters.TryGetValue(id, out var value) ? value : null);
		return new ArenaBettingService(gameworld.Object, financeMock.Object, paymentMock.Object, () => context);
	}

	private static IArenaEvent BuildEvent(Mock<ICombatArena> arena, BettingModel model, ArenaEventState state, IEnumerable<IArenaParticipant>? participants = null)
	{
		var eventTypeSide = new Mock<IArenaEventTypeSide>();
		eventTypeSide.Setup(x => x.Index).Returns(0);
		var eventType = new Mock<IArenaEventType>();
		eventType.Setup(x => x.BettingModel).Returns(model);
		eventType.Setup(x => x.Sides).Returns(new[] { eventTypeSide.Object });
		var evt = new Mock<IArenaEvent>();
		evt.Setup(x => x.Id).Returns(42L);
		evt.Setup(x => x.State).Returns(state);
		evt.Setup(x => x.Arena).Returns(arena.Object);
		evt.Setup(x => x.EventType).Returns(eventType.Object);
		evt.Setup(x => x.Participants).Returns(participants ?? new List<IArenaParticipant>());
		evt.Setup(x => x.Name).Returns("Test Bout");
		return evt.Object;
	}

	[TestMethod]
	public void CanBet_ReturnsFalse_WhenStateClosed()
	{
		using var context = BuildContext();
		var financeMock = new Mock<IArenaFinanceService>();
		var paymentMock = new Mock<IArenaBetPaymentService>();
		var characters = new Dictionary<long, ICharacter>();
		var service = CreateService(context, financeMock, paymentMock, characters);
		var arena = new Mock<ICombatArena>();
		var evt = new Mock<IArenaEvent>();
		evt.Setup(x => x.State).Returns(ArenaEventState.Completed);
		evt.Setup(x => x.Participants).Returns(new List<IArenaParticipant>());
		evt.Setup(x => x.Id).Returns(1L);
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.Id).Returns(5L);
		var result = service.CanBet(actor.Object, evt.Object);
		Assert.IsFalse(result.Truth);
	}

	[TestMethod]
	public void PlaceBet_FixedOdds_CreatesBetAndCreditsArena()
	{
		using var context = BuildContext();
		var financeMock = new Mock<IArenaFinanceService>();
		var paymentMock = new Mock<IArenaBetPaymentService>();
		paymentMock.Setup(x => x.CollectStake(It.IsAny<ICharacter>(), It.IsAny<IArenaEvent>(), It.IsAny<decimal>()))
		           .Returns((true, string.Empty));
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.Id).Returns(10L);
		var characters = new Dictionary<long, ICharacter> { { 10L, actor.Object } };
		var arena = new Mock<ICombatArena>();
		var evt = BuildEvent(arena, BettingModel.FixedOdds, ArenaEventState.RegistrationOpen);
		var service = CreateService(context, financeMock, paymentMock, characters);
		service.PlaceBet(actor.Object, evt, 0, 100m);
		var bet = context.ArenaBets.Single();
		Assert.AreEqual(100m, bet.Stake);
		Assert.AreEqual(0, bet.SideIndex);
		Assert.IsTrue(bet.FixedDecimalOdds.HasValue);
		arena.Verify(x => x.Credit(100m, It.Is<string>(s => s.Contains("stake"))), Times.Once);
		paymentMock.Verify(x => x.CollectStake(actor.Object, evt, 100m), Times.Once);
	}

	[TestMethod]
	public void Settle_Solvent_CreatesPayoutRecords()
	{
		using var context = BuildContext();
		var financeMock = new Mock<IArenaFinanceService>();
		financeMock.Setup(x => x.IsSolvent(It.IsAny<ICombatArena>(), It.IsAny<decimal>())).Returns((true, string.Empty));
		var arena = new Mock<ICombatArena>();
		arena.Setup(x => x.EnsureFunds(It.IsAny<decimal>())).Returns((true, string.Empty));
		var currency = new Mock<ICurrency>();
		currency.Setup(x => x.Describe(It.IsAny<decimal>(), It.IsAny<CurrencyDescriptionPatternType>()))
			.Returns<decimal, CurrencyDescriptionPatternType>((amount, _) => amount.ToString("F2"));
		arena.SetupGet(x => x.Currency).Returns(currency.Object);
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.Id).Returns(5L);
		var outputHandler = new Mock<IOutputHandler>();
		outputHandler.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		outputHandler.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		actor.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
		var paymentMock = new Mock<IArenaBetPaymentService>();
		paymentMock.Setup(x => x.CollectStake(It.IsAny<ICharacter>(), It.IsAny<IArenaEvent>(), It.IsAny<decimal>()))
		           .Returns((true, string.Empty));
		paymentMock.Setup(x => x.TryDisburse(It.IsAny<ICharacter>(), It.IsAny<IArenaEvent>(), It.IsAny<decimal>()))
		           .Returns(true);
		var characters = new Dictionary<long, ICharacter> { { 5L, actor.Object } };
		var service = CreateService(context, financeMock, paymentMock, characters);
		var evt = BuildEvent(arena, BettingModel.FixedOdds, ArenaEventState.RegistrationOpen);
		service.PlaceBet(actor.Object, evt, 0, 50m);
		service.Settle(evt, ArenaOutcome.Win, new[] { 0 });
		Assert.AreEqual(1, context.ArenaBetPayouts.Count());
		var payout = context.ArenaBetPayouts.Single();
		Assert.IsFalse(payout.IsBlocked);
		Assert.IsNotNull(payout.CollectedAt);
		arena.Verify(x => x.Debit(It.IsAny<decimal>(), It.Is<string>(s => s.Contains("payout"))), Times.AtLeastOnce);
	}

	[TestMethod]
	public void Settle_InsufficientFunds_BlocksPayouts()
	{
		using var context = BuildContext();
		var financeMock = new Mock<IArenaFinanceService>();
		financeMock.Setup(x => x.IsSolvent(It.IsAny<ICombatArena>(), It.IsAny<decimal>())).Returns((false, "insolvent"));
		var paymentMock = new Mock<IArenaBetPaymentService>();
		paymentMock.Setup(x => x.CollectStake(It.IsAny<ICharacter>(), It.IsAny<IArenaEvent>(), It.IsAny<decimal>()))
		           .Returns((true, string.Empty));
		var arena = new Mock<ICombatArena>();
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.Id).Returns(7L);
		var characters = new Dictionary<long, ICharacter> { { 7L, actor.Object } };
		var service = CreateService(context, financeMock, paymentMock, characters);
		var evt = BuildEvent(arena, BettingModel.FixedOdds, ArenaEventState.RegistrationOpen);
		var bettor = new Mock<ICharacter>();
		bettor.Setup(x => x.Id).Returns(7L);
		service.PlaceBet(bettor.Object, evt, 0, 25m);
		service.Settle(evt, ArenaOutcome.Win, new[] { 0 });
		financeMock.Verify(x => x.BlockPayout(arena.Object, evt, It.Is<IEnumerable<(ICharacter Winner, decimal Amount)>>(p => p.Count() == 1)), Times.Once);
		Assert.AreEqual(0, context.ArenaBetPayouts.Count());
	}

	[TestMethod]
	public void RefundAll_MarksBetsCancelled()
	{
		using var context = BuildContext();
		var financeMock = new Mock<IArenaFinanceService>();
		var paymentMock = new Mock<IArenaBetPaymentService>();
		paymentMock.Setup(x => x.CollectStake(It.IsAny<ICharacter>(), It.IsAny<IArenaEvent>(), It.IsAny<decimal>()))
		           .Returns((true, string.Empty));
		paymentMock.Setup(x => x.TryDisburse(It.IsAny<ICharacter>(), It.IsAny<IArenaEvent>(), It.IsAny<decimal>()))
		           .Returns(true);
		var arena = new Mock<ICombatArena>();
		var currency = new Mock<ICurrency>();
		currency.Setup(x => x.Describe(It.IsAny<decimal>(), It.IsAny<CurrencyDescriptionPatternType>()))
			.Returns<decimal, CurrencyDescriptionPatternType>((amount, _) => amount.ToString("F2"));
		arena.SetupGet(x => x.Currency).Returns(currency.Object);
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.Id).Returns(9L);
		var outputHandler = new Mock<IOutputHandler>();
		outputHandler.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		outputHandler.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		actor.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
		var characters = new Dictionary<long, ICharacter> { { 9L, actor.Object } };
		var service = CreateService(context, financeMock, paymentMock, characters);
		var evt = BuildEvent(arena, BettingModel.FixedOdds, ArenaEventState.RegistrationOpen);
		service.PlaceBet(actor.Object, evt, 0, 10m);
		service.RefundAll(evt, "cancelled");
		var bet = context.ArenaBets.Single();
		Assert.IsTrue(bet.IsCancelled);
		arena.Verify(x => x.Debit(10m, It.Is<string>(s => s.Contains("refund"))), Times.Once);
	}
}
