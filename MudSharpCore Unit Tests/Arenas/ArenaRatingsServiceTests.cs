#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaRatingsServiceTests
{
    [TestMethod]
    public void GetRating_ReturnsDefault_WhenNoRecordExists()
    {
        using FuturemudDatabaseContext context = BuildContext();
        ArenaRatingsService service = CreateService(context);
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(42L);
        Mock<ICombatantClass> combatantClass = new();
        combatantClass.SetupGet(x => x.Id).Returns(7L);

        decimal rating = service.GetRating(character.Object, combatantClass.Object);

        Assert.AreEqual(1500.0m, rating);
    }

    [TestMethod]
    public void UpdateRatings_CreatesAndUpdatesRecords()
    {
        using FuturemudDatabaseContext context = BuildContext();
        ArenaRatingsService service = CreateService(context);
        Mock<ICombatArena> arena = new();
        arena.SetupGet(x => x.Id).Returns(11L);

        (Mock<IArenaParticipant> Participant, Mock<ICharacter> Character, Mock<ICombatantClass> CombatantClass) participantA = BuildParticipant(10L, 100L, 0, 1500.0m);
        (Mock<IArenaParticipant> Participant, Mock<ICharacter> Character, Mock<ICombatantClass> CombatantClass) participantB = BuildParticipant(20L, 200L, 1, 1500.0m);

        Mock<IArenaEvent> evt = new();
        evt.SetupGet(x => x.Arena).Returns(arena.Object);
        evt.SetupGet(x => x.Participants)
           .Returns(new[] { participantA.Participant.Object, participantB.Participant.Object });

        Dictionary<ICharacter, decimal> deltas = new()
        {
            { participantA.Character.Object, 12.5m },
            { participantB.Character.Object, -7.5m }
        };

        service.UpdateRatings(evt.Object, deltas);

        List<ArenaRating> records = context.ArenaRatings.ToList();
        Assert.AreEqual(2, records.Count);
        Assert.AreEqual(1512.50m, records.Single(x => x.CharacterId == 10L).Rating);
        Assert.AreEqual(1492.50m, records.Single(x => x.CharacterId == 20L).Rating);

        Dictionary<ICharacter, decimal> followUp = new()
        {
            { participantA.Character.Object, -2.5m }
        };

        service.UpdateRatings(evt.Object, followUp);

        ArenaRating updated = context.ArenaRatings.Single(x => x.CharacterId == 10L);
        Assert.AreEqual(1510.00m, updated.Rating);
    }

    [TestMethod]
    public void ApplyDefaultElo_ComputesWinnerAndLoserDeltas()
    {
        using FuturemudDatabaseContext context = BuildContext();
        ArenaRatingsService service = CreateService(context);
        Mock<ICombatArena> arena = new();
        arena.SetupGet(x => x.Id).Returns(5L);

        (Mock<IArenaParticipant> Participant, Mock<ICharacter> Character, Mock<ICombatantClass> CombatantClass) winningParticipant = BuildParticipant(30L, 300L, 0, 1500.0m);
        (Mock<IArenaParticipant> Participant, Mock<ICharacter> Character, Mock<ICombatantClass> CombatantClass) losingParticipant = BuildParticipant(40L, 400L, 1, 1500.0m);

        Mock<IArenaEvent> evt = new();
        evt.SetupGet(x => x.Arena).Returns(arena.Object);
        evt.SetupGet(x => x.Participants)
           .Returns(new[] { winningParticipant.Participant.Object, losingParticipant.Participant.Object });
        evt.Setup(x => x.Outcome)
           .Returns(ArenaOutcome.Win);
        evt.Setup(x => x.WinningSides)
           .Returns([0]);

        service.ApplyDefaultElo(evt.Object);

        Dictionary<long, decimal> ratings = context.ArenaRatings.ToDictionary(x => x.CharacterId, x => x.Rating);
        Assert.AreEqual(1516.00m, ratings[30L]);
        Assert.AreEqual(1484.00m, ratings[40L]);
    }

    [TestMethod]
    public void ApplyDefaultElo_UsesCharacterIds_WhenCharactersAreNotLoaded()
    {
        using FuturemudDatabaseContext context = BuildContext();
        ArenaRatingsService service = CreateService(context);
        Mock<ICombatArena> arena = new();
        arena.SetupGet(x => x.Id).Returns(9L);

        (Mock<IArenaParticipant> Participant, Mock<ICharacter> Character, Mock<ICombatantClass> CombatantClass) winner = BuildParticipant(50L, 500L, 0, 1500.0m, includeCharacter: false);
        (Mock<IArenaParticipant> Participant, Mock<ICharacter> Character, Mock<ICombatantClass> CombatantClass) loser = BuildParticipant(60L, 600L, 1, 1500.0m, includeCharacter: false);

        Mock<IArenaEvent> evt = new();
        evt.SetupGet(x => x.Arena).Returns(arena.Object);
        evt.SetupGet(x => x.Participants)
            .Returns(new[] { winner.Participant.Object, loser.Participant.Object });
        evt.SetupGet(x => x.Outcome).Returns(ArenaOutcome.Win);
        evt.SetupGet(x => x.WinningSides).Returns([0]);

        service.ApplyDefaultElo(evt.Object);

        Dictionary<long, decimal> ratings = context.ArenaRatings.ToDictionary(x => x.CharacterId, x => x.Rating);
        Assert.AreEqual(1516.00m, ratings[50L]);
        Assert.AreEqual(1484.00m, ratings[60L]);
    }

    [TestMethod]
    public void ApplyDefaultElo_RespectsEventTypeStyleAndKFactor()
    {
        using FuturemudDatabaseContext context = BuildContext();
        ArenaRatingsService service = CreateService(context);
        Mock<ICombatArena> arena = new();
        arena.SetupGet(x => x.Id).Returns(12L);
        Mock<IArenaEventType> eventType = new();
        eventType.SetupGet(x => x.EloStyle).Returns(ArenaEloStyle.PairwiseIndividual);
        eventType.SetupGet(x => x.EloKFactor).Returns(16.0m);

        (Mock<IArenaParticipant> Participant, Mock<ICharacter> Character, Mock<ICombatantClass> CombatantClass) winner = BuildParticipant(70L, 700L, 0, 1500.0m);
        (Mock<IArenaParticipant> Participant, Mock<ICharacter> Character, Mock<ICombatantClass> CombatantClass) loser = BuildParticipant(80L, 800L, 1, 1500.0m);

        Mock<IArenaEvent> evt = new();
        evt.SetupGet(x => x.Arena).Returns(arena.Object);
        evt.SetupGet(x => x.EventType).Returns(eventType.Object);
        evt.SetupGet(x => x.Participants).Returns(new[] { winner.Participant.Object, loser.Participant.Object });
        evt.SetupGet(x => x.Outcome).Returns(ArenaOutcome.Win);
        evt.SetupGet(x => x.WinningSides).Returns([0]);

        service.ApplyDefaultElo(evt.Object);

        Dictionary<long, decimal> ratings = context.ArenaRatings.ToDictionary(x => x.CharacterId, x => x.Rating);
        Assert.AreEqual(1508.00m, ratings[70L]);
        Assert.AreEqual(1492.00m, ratings[80L]);
    }

    [TestMethod]
    public void GetArenaRatings_ReturnsOnlyRequestedArena()
    {
        using FuturemudDatabaseContext context = BuildContext();
        context.Characters.AddRange(
            BuildCharacter(1, "Alice"),
            BuildCharacter(2, "Bob"),
            BuildCharacter(3, "Cara"));
        context.ArenaCombatantClasses.AddRange(
            BuildCombatantClass(10, 1, "Heavyweight"),
            BuildCombatantClass(11, 1, "Lightweight"));
        context.ArenaRatings.AddRange(
            new ArenaRating { ArenaId = 1, CharacterId = 1, CombatantClassId = 10, Rating = 1610m, LastUpdatedAt = DateTime.UtcNow },
            new ArenaRating { ArenaId = 1, CharacterId = 2, CombatantClassId = 11, Rating = 1425m, LastUpdatedAt = DateTime.UtcNow },
            new ArenaRating { ArenaId = 2, CharacterId = 3, CombatantClassId = 10, Rating = 1775m, LastUpdatedAt = DateTime.UtcNow });
        context.SaveChanges();

        ArenaRatingsService service = CreateService(context);
        Mock<ICombatArena> arena = new();
        arena.SetupGet(x => x.Id).Returns(1L);

        List<ArenaRatingSummary> ratings = service.GetArenaRatings(arena.Object).ToList();

        Assert.AreEqual(2, ratings.Count);
        Assert.IsTrue(ratings.All(x => x.ArenaId == 1L));
        Assert.AreEqual("Alice", ratings[0].CharacterName);
        Assert.AreEqual(1610m, ratings[0].Rating);
    }

    [TestMethod]
    public void GetCharacterRatings_ReturnsOnlyRequestedCharacterWithinArena()
    {
        using FuturemudDatabaseContext context = BuildContext();
        context.Characters.AddRange(
            BuildCharacter(1, "Alice"),
            BuildCharacter(2, "Bob"));
        context.ArenaCombatantClasses.AddRange(
            BuildCombatantClass(20, 1, "Blade"),
            BuildCombatantClass(21, 1, "Brawl"));
        context.ArenaRatings.AddRange(
            new ArenaRating { ArenaId = 1, CharacterId = 1, CombatantClassId = 20, Rating = 1530m, LastUpdatedAt = DateTime.UtcNow },
            new ArenaRating { ArenaId = 1, CharacterId = 1, CombatantClassId = 21, Rating = 1490m, LastUpdatedAt = DateTime.UtcNow },
            new ArenaRating { ArenaId = 1, CharacterId = 2, CombatantClassId = 20, Rating = 1720m, LastUpdatedAt = DateTime.UtcNow },
            new ArenaRating { ArenaId = 2, CharacterId = 1, CombatantClassId = 20, Rating = 1800m, LastUpdatedAt = DateTime.UtcNow });
        context.SaveChanges();

        ArenaRatingsService service = CreateService(context);
        Mock<ICombatArena> arena = new();
        arena.SetupGet(x => x.Id).Returns(1L);
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(1L);

        List<ArenaRatingSummary> ratings = service.GetCharacterRatings(arena.Object, character.Object).ToList();

        Assert.AreEqual(2, ratings.Count);
        Assert.IsTrue(ratings.All(x => x.ArenaId == 1L && x.CharacterId == 1L));
        CollectionAssert.AreEquivalent(new[] { 20L, 21L }, ratings.Select(x => x.CombatantClassId).ToArray());
    }

    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static ArenaRatingsService CreateService(FuturemudDatabaseContext context)
    {
        Mock<IFuturemud> gameworld = new();
        return new ArenaRatingsService(gameworld.Object, () => context);
    }

    private static MudSharp.Models.Character BuildCharacter(long id, string name)
    {
        return new MudSharp.Models.Character
        {
            Id = id,
            Name = name,
            CreationTime = DateTime.UtcNow,
            EffectData = string.Empty,
            BirthdayDate = string.Empty,
            NeedsModel = string.Empty,
            LongTermPlan = string.Empty,
            ShortTermPlan = string.Empty,
            IntroductionMessage = string.Empty,
            PositionTargetType = string.Empty,
            PositionEmote = string.Empty,
            Outfits = string.Empty,
            NameInfo = string.Empty
        };
    }

    private static MudSharp.Models.ArenaCombatantClass BuildCombatantClass(long classId, long arenaId, string name)
    {
        return new MudSharp.Models.ArenaCombatantClass
        {
            Id = classId,
            ArenaId = arenaId,
            Name = name,
            Description = string.Empty,
            EligibilityProgId = 1,
            DefaultSignatureColour = string.Empty
        };
    }

    private static (Mock<IArenaParticipant> Participant, Mock<ICharacter> Character, Mock<ICombatantClass> CombatantClass) BuildParticipant(
        long characterId,
        long classId,
        int sideIndex,
        decimal? startingRating = null,
        bool isNpc = false,
        bool includeCharacter = true)
    {
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(characterId);
        Mock<ICombatantClass> combatantClass = new();
        combatantClass.SetupGet(x => x.Id).Returns(classId);
        Mock<IArenaParticipant> participant = new();
        participant.SetupGet(x => x.CharacterId).Returns(characterId);
        participant.SetupGet(x => x.Character).Returns(includeCharacter ? character.Object : null);
        participant.SetupGet(x => x.CombatantClass).Returns(combatantClass.Object);
        participant.SetupGet(x => x.SideIndex).Returns(sideIndex);
        participant.SetupGet(x => x.IsNpc).Returns(isNpc);
        participant.SetupGet(x => x.StartingRating).Returns(startingRating);
        return (participant, character, combatantClass);
    }

    private sealed class TestProgVariable : IProgVariable
    {
        private readonly object _value;
        private readonly ProgVariableTypes _type;

        public TestProgVariable(object value, ProgVariableTypes type)
        {
            _value = value;
            _type = type;
        }

        public ProgVariableTypes Type => _type;

        public object GetObject => _value;

        public IProgVariable GetProperty(string property)
        {
            throw new NotSupportedException();
        }
    }
}
