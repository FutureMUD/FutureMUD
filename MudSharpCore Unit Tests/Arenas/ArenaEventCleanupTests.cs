#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaEventCleanupTests
{
    [TestMethod]
    public void Cleanup_ParticipantInCombat_LeavesCombat()
    {
        Mock<ICombat> combat = new();
        Mock<IOutputHandler> outputHandler = new();
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(500L);
        character.SetupGet(x => x.IsPlayerCharacter).Returns(true);
        character.SetupGet(x => x.Combat).Returns(combat.Object);
        character.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
        character.Setup(x => x.CombinedEffectsOfType<ArenaPreparingEffect>())
            .Returns([]);
        character.Setup(x => x.CombinedEffectsOfType<ArenaStagingEffect>())
            .Returns([]);
        character.Setup(x => x.CombinedEffectsOfType<ArenaParticipantPreparationEffect>())
            .Returns([]);

        Mock<IFuturemud> gameworld = BuildGameworld(character.Object);
        IArenaEvent arenaEvent = BuildLiveEvent(gameworld.Object);

        arenaEvent.Cleanup();

        combat.Verify(x => x.LeaveCombat(character.Object), Times.Once);
    }

    private static IArenaEvent BuildLiveEvent(IFuturemud gameworld)
    {
        Arena arenaModel = new()
        {
            Id = 100,
            Name = "Cleanup Test Arena",
            EconomicZoneId = 1,
            CurrencyId = 1,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            SignupEcho = string.Empty
        };

        MudSharp.Models.ArenaCombatantClass classModel = new()
        {
            Id = 400,
            ArenaId = 100,
            Name = "General",
            Description = string.Empty,
            EligibilityProgId = 1,
            ResurrectNpcOnDeath = false,
            FullyRestoreNpcOnCompletion = true,
            DefaultSignatureColour = string.Empty
        };
        arenaModel.ArenaCombatantClasses.Add(classModel);

        MudSharp.Models.ArenaEventType eventTypeModel = new()
        {
            Id = 200,
            ArenaId = 100,
            Name = "Cleanup Type",
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
        arenaModel.ArenaEventTypes.Add(eventTypeModel);

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
        eventModel.ArenaSignups.Add(new MudSharp.Models.ArenaSignup
        {
            Id = 600,
            ArenaEventId = 300,
            CharacterId = 500,
            CombatantClassId = 400,
            SideIndex = 0,
            IsNpc = false,
            StageName = string.Empty,
            SignatureColour = string.Empty,
            SignedUpAt = DateTime.UtcNow
        });
        arenaModel.ArenaEvents.Add(eventModel);

        CombatArena arena = new(arenaModel, gameworld);
        return arena.ActiveEvents.Single();
    }

    private static Mock<IFuturemud> BuildGameworld(ICharacter participant)
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
        Mock<IUneditableAll<IRandomNameProfile>> randomNameProfiles = new();
        randomNameProfiles.Setup(x => x.Get(It.IsAny<long>())).Returns((IRandomNameProfile?)null);
        Mock<IUneditableAll<ICell>> cells = new();
        cells.Setup(x => x.Get(It.IsAny<long>())).Returns((ICell?)null);
        Mock<IArenaScheduler> arenaScheduler = new();
        Mock<IArenaParticipationService> participationService = new();
        Mock<ISaveManager> saveManager = new();

        gameworld.SetupGet(x => x.EconomicZones).Returns(zones.Object);
        gameworld.SetupGet(x => x.Currencies).Returns(currencies.Object);
        gameworld.SetupGet(x => x.BankAccounts).Returns(bankAccounts.Object);
        gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);
        gameworld.SetupGet(x => x.RandomNameProfiles).Returns(randomNameProfiles.Object);
        gameworld.SetupGet(x => x.Cells).Returns(cells.Object);
        gameworld.SetupGet(x => x.ArenaScheduler).Returns(arenaScheduler.Object);
        gameworld.SetupGet(x => x.ArenaParticipationService).Returns(participationService.Object);
        gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
        gameworld.SetupGet(x => x.CombatArenas).Returns(new All<ICombatArena>());
        gameworld.SetupGet(x => x.Actors).Returns(new All<ICharacter>());
        gameworld.Setup(x => x.TryGetCharacter(500L, It.IsAny<bool>())).Returns(participant);
        gameworld.Setup(x => x.SystemMessage(It.IsAny<string>(), It.IsAny<bool>()));

        return gameworld;
    }
}
