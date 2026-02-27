#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaEventResolveTests
{
	[TestMethod]
	public void Resolve_CallsBetSettlementAndAppearanceAccrualOnce()
	{
		var bettingService = new Mock<IArenaBettingService>();
		var financeService = new Mock<IArenaFinanceService>();
		var gameworld = BuildGameworld(bettingService.Object, financeService.Object);

		var arenaModel = new MudSharp.Models.Arena
		{
			Id = 100,
			Name = "Resolve Test Arena",
			EconomicZoneId = 1,
			CurrencyId = 1,
			CreatedAt = DateTime.UtcNow,
			IsDeleted = false,
			SignupEcho = string.Empty
		};
		var eventTypeModel = new MudSharp.Models.ArenaEventType
		{
			Id = 200,
			ArenaId = 100,
			Name = "Resolve Test Type",
			BringYourOwn = false,
			RegistrationDurationSeconds = 0,
			PreparationDurationSeconds = 0,
			BettingModel = (int)BettingModel.FixedOdds,
			EliminationMode = (int)ArenaEliminationMode.NoElimination,
			AllowSurrender = true,
			AppearanceFee = 0m,
			VictoryFee = 0m,
			PayNpcAppearanceFee = false,
			EloStyle = (int)ArenaEloStyle.TeamAverage,
			EloKFactor = 32m
		};
		var eventModel = new MudSharp.Models.ArenaEvent
		{
			Id = 300,
			ArenaId = 100,
			ArenaEventTypeId = 200,
			State = (int)ArenaEventState.Live,
			BringYourOwn = false,
			RegistrationDurationSeconds = 0,
			PreparationDurationSeconds = 0,
			BettingModel = (int)BettingModel.FixedOdds,
			AppearanceFee = 0m,
			VictoryFee = 0m,
			PayNpcAppearanceFee = false,
			CreatedAt = DateTime.UtcNow,
			ScheduledAt = DateTime.UtcNow
		};

		var arena = new CombatArena(arenaModel, gameworld.Object);
		var eventType = new ArenaEventType(eventTypeModel, arena, _ => null);
		var arenaEvent = new ArenaEvent(eventModel, arena, eventType);

		arenaEvent.Resolve();
		arenaEvent.Resolve();

		bettingService.Verify(
			x => x.Settle(arenaEvent, It.IsAny<ArenaOutcome>(), It.IsAny<IEnumerable<int>>()),
			Times.Once);
		financeService.Verify(x => x.AccrueAppearancePayouts(arenaEvent), Times.Once);
	}

	private static Mock<IFuturemud> BuildGameworld(IArenaBettingService bettingService,
		IArenaFinanceService financeService)
	{
		var gameworld = new Mock<IFuturemud>();
		var zone = new Mock<IEconomicZone>();
		zone.SetupGet(x => x.Id).Returns(1L);
		zone.SetupGet(x => x.Name).Returns("Zone");
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(1L);
		currency.SetupGet(x => x.Name).Returns("Currency");

		var zones = new Mock<IUneditableAll<IEconomicZone>>();
		zones.Setup(x => x.Get(1L)).Returns(zone.Object);
		var currencies = new Mock<IUneditableAll<ICurrency>>();
		currencies.Setup(x => x.Get(1L)).Returns(currency.Object);
		var bankAccounts = new Mock<IUneditableAll<IBankAccount>>();
		var futureProgs = new Mock<IUneditableAll<IFutureProg>>();
		futureProgs.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg?)null);
		var cells = new Mock<IUneditableAll<ICell>>();
		cells.Setup(x => x.Get(It.IsAny<long>())).Returns((ICell?)null);
		var arenaScheduler = new Mock<IArenaScheduler>();
		var saveManager = new Mock<ISaveManager>();

		gameworld.SetupGet(x => x.EconomicZones).Returns(zones.Object);
		gameworld.SetupGet(x => x.Currencies).Returns(currencies.Object);
		gameworld.SetupGet(x => x.BankAccounts).Returns(bankAccounts.Object);
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cells.Object);
		gameworld.SetupGet(x => x.ArenaScheduler).Returns(arenaScheduler.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.SetupGet(x => x.ArenaBettingService).Returns(bettingService);
		gameworld.SetupGet(x => x.ArenaFinanceService).Returns(financeService);
		gameworld.SetupGet(x => x.CombatArenas).Returns(new All<ICombatArena>());
		gameworld.Setup(x => x.SystemMessage(It.IsAny<string>(), It.IsAny<bool>()));

		return gameworld;
	}
}
