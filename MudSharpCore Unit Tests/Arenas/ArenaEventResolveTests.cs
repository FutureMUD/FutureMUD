#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaEventResolveTests
{
    [TestMethod]
    public void Resolve_CallsBetSettlementAndAppearanceAccrualOnce()
    {
        Mock<IArenaBettingService> bettingService = new();
        Mock<IArenaFinanceService> financeService = new();
        Mock<IFuturemud> gameworld = BuildGameworld(bettingService.Object, financeService.Object);

        MudSharp.Models.Arena arenaModel = new()
        {
            Id = 100,
            Name = "Resolve Test Arena",
            EconomicZoneId = 1,
            CurrencyId = 1,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            SignupEcho = string.Empty
        };
        MudSharp.Models.ArenaEventType eventTypeModel = new()
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
        MudSharp.Models.ArenaEvent eventModel = new()
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

        CombatArena arena = new(arenaModel, gameworld.Object);
        ArenaEventType eventType = new(eventTypeModel, arena, _ => null);
        ArenaEvent arenaEvent = new(eventModel, arena, eventType);

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
        Mock<IFuturemud> gameworld = new();
        Mock<IEconomicZone> zone = new();
        zone.SetupGet(x => x.Id).Returns(1L);
        zone.SetupGet(x => x.Name).Returns("Zone");
        Mock<ICurrency> currency = new();
        currency.SetupGet(x => x.Id).Returns(1L);
        currency.SetupGet(x => x.Name).Returns("Currency");

        Mock<IUneditableAll<IEconomicZone>> zones = new();
        zones.Setup(x => x.Get(1L)).Returns(zone.Object);
        Mock<IUneditableAll<ICurrency>> currencies = new();
        currencies.Setup(x => x.Get(1L)).Returns(currency.Object);
        Mock<IUneditableAll<IBankAccount>> bankAccounts = new();
        Mock<IUneditableAll<IFutureProg>> futureProgs = new();
        futureProgs.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg?)null);
        Mock<IUneditableAll<ICell>> cells = new();
        cells.Setup(x => x.Get(It.IsAny<long>())).Returns((ICell?)null);
        Mock<IArenaScheduler> arenaScheduler = new();
        Mock<ISaveManager> saveManager = new();

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
